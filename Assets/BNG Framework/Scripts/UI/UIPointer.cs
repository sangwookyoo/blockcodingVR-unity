using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BNG {

    /// <summary>
    /// Point a line  at our GazePointer
    /// </summary>
    public class UIPointer : MonoBehaviour {

        [Tooltip("The controller side this pointer is on")]
        public ControllerHand ControllerSide = ControllerHand.Right;

        [Tooltip("If true this object will update the VRUISystem's Left or Right Transform property")]
        public bool AutoUpdateUITransforms = true;

        public GameObject cursor;
        private GameObject _cursor;

        /// <summary>
        /// 0.5 = Line Goes Half Way. 1 = Line reaches end.
        /// </summary>
        [Tooltip("Example : 0.5 = Line Goes Half Way. 1 = Line reaches end.")]
        public float LineDistanceModifier = 0.8f;

        /// <summary>
        /// Calls Events
        /// </summary>
        VRUISystem uiSystem;

        [Tooltip("LineRenderer to use when showing a valid UI Canvas. Leave null to attempt a GetComponent<> on this object.")]
        public LineRenderer lineRenderer;

        void Awake() {

            if(cursor) {
                _cursor = GameObject.Instantiate(cursor);
            }

            // If no Line Renderer was specified in the editor, check this Transform
            if (lineRenderer == null) {
                lineRenderer = GetComponent<LineRenderer>();
            }

            // Line Renderer is positioned using world space
            if(lineRenderer != null) {
                lineRenderer.useWorldSpace = true;
            }

            uiSystem = VRUISystem.Instance;           
        }

        void OnEnable() {
            // Automatically update VR System with out transforms
            if (AutoUpdateUITransforms && ControllerSide == ControllerHand.Left) {
                uiSystem.LeftPointerTransform = this.transform;
            }
            else if (AutoUpdateUITransforms && ControllerSide == ControllerHand.Right) {
                uiSystem.RightPointerTransform = this.transform;
            }

            uiSystem.UpdateControllerHand(ControllerSide);
        }

        void Update() {
            PointerEventData data = uiSystem.EventData;

            if (data == null) {
                return;
            }

            // Set position of the cursor
            if (_cursor != null ) {
                _cursor.transform.position = data.pointerCurrentRaycast.worldPosition;
                _cursor.transform.rotation = Quaternion.FromToRotation(Vector3.forward, data.pointerCurrentRaycast.worldNormal);
                _cursor.SetActive(data.pointerCurrentRaycast.distance > 0);
            }

            // Update linerenderer
            if (lineRenderer) {
                lineRenderer.useWorldSpace = false;
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, new Vector3(0, 0 , Vector3.Distance(transform.position, data.pointerCurrentRaycast.worldPosition) * LineDistanceModifier));
                lineRenderer.enabled = data.pointerCurrentRaycast.distance > 0;
            }
        }
    }
}

