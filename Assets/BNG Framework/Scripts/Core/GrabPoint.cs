using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public class GrabPoint : MonoBehaviour {

        [Header("Pose")]
        /// <summary>
        /// Set to Default to inherit Grabbable's HandPose. Otherwise this HandPose will be used
        /// </summary>
        public HandPoseId HandPose;

        [Header("Valid Hands")]
        [Tooltip("Can this Grab Point be used by a left-handed Grabber?")]
        public bool LeftHandIsValid = true;

        [Tooltip("Can this Grab Point be used by a right-handed Grabber?")]
        public bool RightHandIsValid = true;

        [Header("Parent to")]
        /// <summary>
        /// If specified, the Hand Model will be placed here when snapped
        /// </summary>
        [Tooltip("If specified, the Hand Model will be parented here when snapped")]
        public Transform HandPosition;

        [Header("Angle Restriction")]
        /// <summary>
        /// GrabPoint is not considered valid if the angle between the GrabPoint and Grabber is greater than this amount
        /// </summary>
        [Tooltip("GrabPoint is not considered valid if the angle between the GrabPoint and Grabber is greater than this amount")]
        [Range(0.0f, 360.0f)]
        public float MaxDegreeDifferenceAllowed = 360;

        [Header("Finger Blending")]
        [Tooltip("Minimum value Hand Animator will blend to. Example : If IndexBlendMin = 0.4 and Trigger button is not held down, the LayerWeight will be set to 0.4")]
        [Range(0.0f, 1.0f)]
        public float IndexBlendMin = 0;

        [Tooltip("Maximum value Hand Animator will blend to. Example : If IndexBlendMax = 0.6 and Trigger button is held all the way down, the LayerWeight will be set to 0.6")]
        [Range(0.0f, 1.0f)]
        public float IndexBlendMax = 0;

        [Tooltip("Minimum value Hand Animator will blend to if thumb control is not being touched.")]
        [Range(0.0f, 1.0f)]
        public float ThumbBlendMin = 0;

        [Tooltip("Maximum value Hand Animator will blend to if thumb control is being touched.")]
        [Range(0.0f, 1.0f)]
        public float ThumbBlendMax = 0;

        // Taken from defaults in Demo - offset between "Models" and Grabber
        Vector3 previewModelOffsetLeft = new Vector3(0.007f, -0.0179f, 0.0071f);
        Vector3 previewModelOffsetRight = new Vector3(-0.01f, -0.0179f, 0.0071f);

        [Header("Editor")]
        [Tooltip("Show a green arc in the Scene view representing MaxDegreeDifferenceAllowed")]
        public bool ShowAngleGizmo = true;

#if UNITY_EDITOR
        // Make sure animators update in the editor mode to show hand positions
        // By using OnDrawGizmosSelected we only call this function if the object is selected in the editor
        void OnDrawGizmosSelected() {
            updateChildAnimators();
            UpdatePreviewTransforms();

            drawEditorArc();
        }

        /// <summary>
        /// Draw an arc in the editor representing MaxDegreeDifferenceAllowed
        /// </summary>
        void drawEditorArc() {

            // Draw arc representing the MaxDegreeDifferenceAllowed of the Grab Point
            if (ShowAngleGizmo && MaxDegreeDifferenceAllowed != 0 && MaxDegreeDifferenceAllowed != 360) {
                Vector3 from = Quaternion.AngleAxis(-0.5f * MaxDegreeDifferenceAllowed, transform.up) * (-transform.forward - Vector3.Dot(-transform.forward, transform.up) * transform.up);

                UnityEditor.Handles.color = new Color(0, 1, 0, 0.1f);
                UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, from, MaxDegreeDifferenceAllowed, 0.05f);
            }
        }
#endif

        public void UpdatePreviewTransforms() {
            Transform leftHandPreview = transform.Find("LeftHandModelsEditorPreview");            
            Transform rightHandPreview = transform.Find("RightHandModelsEditorPreview");

            // If there is a Hand in the scene, use that offset instead of our defaults
            if(GameObject.Find("LeftController/Grabber") != null) {
                previewModelOffsetLeft = GameObject.Find("LeftController/Grabber").GetComponent<Grabber>().HandsGraphics.position - GameObject.Find("LeftController/Grabber").transform.position;
            }

            if (GameObject.Find("RightController/Grabber") != null) {
                previewModelOffsetRight = GameObject.Find("RightController/Grabber").GetComponent<Grabber>().HandsGraphics.position - GameObject.Find("RightController/Grabber").transform.position;
            }

            if (leftHandPreview) {
                leftHandPreview.localPosition = previewModelOffsetLeft;
                leftHandPreview.localEulerAngles = Vector3.zero;
            }

            if(rightHandPreview) {
                rightHandPreview.localPosition = previewModelOffsetRight;
                rightHandPreview.localEulerAngles = Vector3.zero;
            }
        }

        void updateChildAnimators() {

            var animators = GetComponentsInChildren<Animator>();
            for (int x = 0; x < animators.Length; x++) {
                animators[x].Update(Time.deltaTime);
#if UNITY_EDITOR
                // Only set dirty if not in prefab mode
                if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) {
                    UnityEditor.EditorUtility.SetDirty(animators[x].gameObject);
                }
#endif
            }
        }
    }
}