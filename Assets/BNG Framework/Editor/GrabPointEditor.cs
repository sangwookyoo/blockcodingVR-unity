using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BNG {

    [CustomEditor(typeof(GrabPoint))]
    [CanEditMultipleObjects]
    public class GrabPointEditor : Editor {

        public GameObject LeftHandPreview;
        bool showingLeftHand = false;

        public GameObject RightHandPreview;
        bool showingRightHand = false;

        // Define a texture and GUIContent
        private Texture buttonLeftTexture;
        private Texture buttonLeftTextureSelected;
        private GUIContent buttonLeftContent;

        private Texture buttonRightTexture;
        private Texture buttonRightTextureSelected;

        private GUIContent buttonRightContent;

        GrabPoint grabPoint;

        public override void OnInspectorGUI() {

            grabPoint = (GrabPoint)target;
            bool inPrefabMode = false;
#if UNITY_EDITOR
            inPrefabMode = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#endif

            // Double check that there wasn't an object left in the scene
            checkForExistingPreview();

            // Load the texture resource
            if (buttonLeftTexture == null) {
                buttonLeftTexture = (Texture)Resources.Load("handIcon", typeof(Texture));
                buttonLeftTextureSelected = (Texture)Resources.Load("handIconSelected", typeof(Texture));
                buttonRightTexture = (Texture)Resources.Load("handIconRight", typeof(Texture));
                buttonRightTextureSelected = (Texture)Resources.Load("handIconSelectedRight", typeof(Texture));
            }


            GUILayout.Label("Toggle Hand Preview : ", EditorStyles.boldLabel);

            if(inPrefabMode) {
                GUILayout.Label("(Some preview features disabled in prefab mode)", EditorStyles.largeLabel);
            }

            GUILayout.BeginHorizontal();

            // Show / Hide Left Hand
            if (showingLeftHand) {

                // Define a GUIContent which uses the texture
                buttonLeftContent = new GUIContent(buttonLeftTextureSelected);

                if (!grabPoint.LeftHandIsValid || GUILayout.Button(buttonLeftContent)) {
                    GameObject.DestroyImmediate(LeftHandPreview);
                    showingLeftHand = false;
                }
            }
            else {
                buttonLeftContent = new GUIContent(buttonLeftTexture);

                if (grabPoint.LeftHandIsValid && GUILayout.Button(buttonLeftContent)) {
                    // Create and add the Editor preview
                    LeftHandPreview = Instantiate(Resources.Load("LeftHandModelsEditorPreview", typeof(GameObject))) as GameObject;
                    LeftHandPreview.transform.name = "LeftHandModelsEditorPreview";
                    LeftHandPreview.transform.parent = grabPoint.transform;
                    LeftHandPreview.transform.localPosition = Vector3.zero;
                    LeftHandPreview.transform.localEulerAngles = Vector3.zero;
                    LeftHandPreview.gameObject.hideFlags = HideFlags.HideAndDontSave;

                    showingLeftHand = true;
                }
            }

            // Show / Hide Right Hand
            if (showingRightHand) {

                // Define a GUIContent which uses the texture
                buttonRightContent = new GUIContent(buttonRightTextureSelected);

                if (!grabPoint.RightHandIsValid || GUILayout.Button(buttonRightContent)) {
                    GameObject.DestroyImmediate(RightHandPreview);
                    showingRightHand = false;
                }
            }
            else {
                buttonRightContent = new GUIContent(buttonRightTexture);

                if (grabPoint.RightHandIsValid && GUILayout.Button(buttonRightContent)) {
                    // Create and add the Editor preview
                    RightHandPreview = Instantiate(Resources.Load("RightHandModelsEditorPreview", typeof(GameObject))) as GameObject;
                    RightHandPreview.transform.name = "RightHandModelsEditorPreview";
                    RightHandPreview.transform.parent = grabPoint.transform;
                    RightHandPreview.transform.localPosition = Vector3.zero;
                    RightHandPreview.transform.localEulerAngles = Vector3.zero;
                    RightHandPreview.gameObject.hideFlags = HideFlags.HideAndDontSave;

                    showingRightHand = true;
                }
            }

            GUILayout.EndHorizontal();

            updateEditorAnimation();

            base.OnInspectorGUI();
        }

        void updateEditorAnimation() {

            if (LeftHandPreview) {
                var anim = LeftHandPreview.GetComponentInChildren<Animator>();
                updateAnimator(anim, (int)grabPoint.HandPose);
            }

            if (RightHandPreview) {
                var anim = RightHandPreview.GetComponentInChildren<Animator>();

                updateAnimator(anim, (int)grabPoint.HandPose);
            }
        }

        void updateAnimator(Animator anim, int handPose) {
            if (anim != null) {

                // Do Fist Pose
                if (handPose == 0) {

                    // 0 = Hands Open, 1 = Grip closes                        
                    anim.SetFloat("Flex", 1);

                    anim.SetLayerWeight(0, 1);
                    anim.SetLayerWeight(1, 0);
                    anim.SetLayerWeight(2, 0);
                }
                else {
                    anim.SetLayerWeight(0, 0);
                    anim.SetLayerWeight(1, 0);
                    anim.SetLayerWeight(2, 0);
                }

                anim.SetInteger("Pose", handPose);
                anim.Update(Time.deltaTime);

#if UNITY_EDITOR
                // Only set dirty if not in prefab mode
                if(UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) {
                    UnityEditor.EditorUtility.SetDirty(anim.gameObject);
                }
#endif
            }
        }

        void checkForExistingPreview() {
            if (LeftHandPreview == null && !showingLeftHand) {
                Transform lt = grabPoint.transform.Find("LeftHandModelsEditorPreview");
                if (lt) {
                    LeftHandPreview = lt.gameObject;
                    showingLeftHand = true;
                }
            }

            if (RightHandPreview == null && !showingRightHand) {
                Transform rt = grabPoint.transform.Find("RightHandModelsEditorPreview");
                if (rt) {
                    RightHandPreview = rt.gameObject;
                    showingRightHand = true;
                }
            }
        }
    }
}

