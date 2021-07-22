using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BNG {

    /// <summary>
    /// Shows a ring at the grab point of a grabbable if within a certain distance
    /// </summary>
    public class RingHelper : MonoBehaviour {

        public Color RingColor = Color.white;

        /// <summary>
        /// Color to use if selected by primary controller
        /// </summary>
        public Color RingSelectedColor = Color.white;

        /// <summary>
        /// Color to use if selected by secondary controller
        /// </summary>
        public Color RingSecondarySelectedColor = Color.white;

        public float ringSizeInRange = 1500f;
        public float ringSizeGrabbable = 1100f; // Lower pixel size the bigger the font

        /// <summary>
        /// Don't show grab rings if left and right controllers / grabbers are  holding something
        /// </summary>
        public bool HideIfHandsAreFull = true;

        /// <summary>
        /// How fast to lerp the opacity if being hidden / shown
        /// </summary>
        public float RingFadeSpeed = 5;

        Canvas canvas;
        Text text;
        Grabbable grabbable;
        CanvasScaler scaler;

        /// <summary>
        /// Used to determine if hands are full
        /// </summary>
        Grabber leftGrabber;
        Grabber rightGrabber;
        bool handsFull = false;

        // Animate opacity
        private float _initalOpacity;
        private float _currentOpacity;

        Transform mainCam;

        // Start is called before the first frame update
        void Start() {
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;

            grabbable = transform.parent.GetComponent<Grabbable>();
            canvas = GetComponent<Canvas>();
            scaler = GetComponent<CanvasScaler>();
            text = GetComponent<Text>();

            if(text == null) {
                Debug.LogWarning("No Text Component Found on RingHelper");
                return;
            }

            _initalOpacity = text.color.a;
            _currentOpacity = _initalOpacity;

            // Assign left / right grabbers
            Grabber[] grabs = GameObject.FindGameObjectWithTag("Player").GetComponentsInChildren<Grabber>();
            foreach(var g in grabs) {
                if(g.HandSide == ControllerHand.Left) {
                    leftGrabber = g;
                }
                else if (g.HandSide == ControllerHand.Right) {
                    rightGrabber = g;
                }
            }
        }

        void Update() {

            // Bail if Text Component was removed or doesn't exist
            if (text == null || mainCam == null) {
                return;
            }

            bool grabbersExist = leftGrabber != null && rightGrabber != null;

            // Holding Item
            handsFull = grabbersExist && leftGrabber.HoldingItem && rightGrabber.HoldingItem;

            // Not a valid Grab
            if(grabbersExist && grabbable.GrabButton == GrabButton.Grip && !leftGrabber.FreshGrip && !rightGrabber.FreshGrip) {
                handsFull = true;
            }

            bool showRings = handsFull;
            

            // If being held or not active, immediately hide the ring
            if (grabbable.BeingHeld || !grabbable.isActiveAndEnabled) {
                canvas.enabled = false;
                return;
            }

            // Show if within range
            float currentDistance = Vector3.Distance(transform.position, mainCam.position);
            if(!handsFull && currentDistance <= grabbable.RemoteGrabDistance) {
                showRings = true;
            }
            else {
                showRings = false;                
            }

            // Animate ring opacity in / out
            if(showRings) {
                canvas.enabled = true;
                canvas.transform.LookAt(mainCam);

                // Resetting the text refreshes the render
                text.text = "o";

                // If a valid grabbable, increase size a bit
                if (grabbable.GetClosestGrabber() != null && grabbable.IsGrabbable()) {
                    scaler.dynamicPixelsPerUnit = ringSizeGrabbable;

                    Color selectedColor = getSelectedColor();
                    text.color = selectedColor;
                }
                else {
                    scaler.dynamicPixelsPerUnit = ringSizeInRange;
                    text.color = RingColor;
                }

                _currentOpacity += Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity > _initalOpacity) {
                    _currentOpacity = _initalOpacity;
                }

                Color colorCurrent = text.color;
                colorCurrent.a = _currentOpacity;
                text.color = colorCurrent;
            }
            else {

                _currentOpacity -= Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity <= 0) {
                    _currentOpacity = 0;
                    canvas.enabled = false;
                }
                else {
                    canvas.enabled = true;
                    Color colorCurrent = text.color;
                    colorCurrent.a = _currentOpacity;
                    text.color = colorCurrent;
                }
            }
        }

        Grabber closestGrabber;
        Color getSelectedColor() {

            // Use secondary color if closest grabber is on the left hand
            closestGrabber = grabbable.GetClosestGrabber();
            if (grabbable != null && closestGrabber != null) {
                if (closestGrabber.HandSide == ControllerHand.Left) {
                    return RingSecondarySelectedColor;
                }
            }

            return RingSelectedColor;
        }
    }
}

