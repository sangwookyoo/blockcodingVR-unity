using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Helper for joystick type physical inputs
    /// </summary>
    public class JoystickControl : MonoBehaviour {

        /// <summary>
        /// Minimum angle the Level can be rotated
        /// </summary>
        public float MinDegrees = -45f;

        /// <summary>
        /// Maximum angle the Level can be rotated
        /// </summary>
        public float MaxDegrees = 45f;

        /// <summary>
        /// Current Percentage of joystick on X axis (left / right)
        /// </summary>
        public float LeverPercentageX = 0;

        /// <summary>
        /// Current Percentage of joystick on Z axis (forward / back)
        /// </summary>
        public float LeverPercentageZ = 0;

        public bool UseSmoothLook = true;
        public float SmoothLookSpeed = 15f;

        /// <summary>
        /// If true, the joystick's rigidbody will be kinematic when not being held. Enable this if you don't want your joystick to interact with physics or if you need moving platform support.
        /// </summary>
        public bool KinematicWhileInactive = false;

        /// <summary>
        /// Event called when Joystick value is changed
        /// </summary>
        public FloatFloatEvent onJoystickChange;


        Grabbable grab;
        Rigidbody rb;

        // Keep track of Joystick Rotation
        Vector3 currentRotation;
        public float angleX;
        public float angleY;

        void Start() {
            grab = GetComponent<Grabbable>();
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update() {

            // Update Kinematic Status.
            if (rb) {
                rb.isKinematic = KinematicWhileInactive && !grab.BeingHeld;
            }                        

            // Lock our local position and axis in Update to avoid jitter
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);


            // Get the modified angle of of the lever. Use this to get percentage based on Min and Max angles.
            currentRotation = transform.localEulerAngles;
            angleX = Mathf.Floor(currentRotation.x);
            angleX = (angleX > 180) ? angleX - 360 : angleX;

            angleY = Mathf.Floor(currentRotation.y);
            angleY = (angleY > 180) ? angleY - 360 : angleY;

            // Cap Angles X
            if (angleX > MaxDegrees) {
                transform.localEulerAngles = new Vector3(MaxDegrees, currentRotation.y, currentRotation.z);
            }
            else if (angleX < MinDegrees) {
                transform.localEulerAngles = new Vector3(MinDegrees, currentRotation.y, currentRotation.z);
            }

            // Cap Angles Z
            if (angleY > MaxDegrees) {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotation.y, MaxDegrees);
            }
            else if (angleY < MinDegrees) {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotation.y, MinDegrees);
            }

            // Set percentage of level position
            LeverPercentageX = (angleX - MinDegrees) / (MaxDegrees - MinDegrees) * 100;
            LeverPercentageZ = (angleY - MinDegrees) / (MaxDegrees - MinDegrees) * 100;

            // Lever value changed event
            OnJoystickChange(LeverPercentageX, LeverPercentageZ);
        }

        void FixedUpdate() {
            // Align lever with Grabber
            doJoystickLook();
        }

        void doJoystickLook() {

            // Do Lever Look
            if (grab != null && grab.BeingHeld) {

                // Store original rotation to be used with smooth look
                Quaternion originalRot = transform.rotation;

                // Use the Grabber as our look target
                // Convert to local position so we can remove the x axis
                Vector3 localTargetPosition = transform.InverseTransformPoint(grab.GetPrimaryGrabber().transform.position);

                // Convert back to world position 
                Vector3 targetPosition = transform.TransformPoint(localTargetPosition);
                transform.LookAt(targetPosition, transform.up);

                if (UseSmoothLook) {
                    Quaternion newRot = transform.rotation;
                    transform.rotation = originalRot;
                    transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * SmoothLookSpeed);
                }
            }
            else if (grab != null && !grab.BeingHeld && rb.isKinematic) {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * SmoothLookSpeed);
            }
        }
        // Callback for lever percentage change
        public virtual void OnJoystickChange(float leverX, float leverY) {
            if (onJoystickChange != null) {
                onJoystickChange.Invoke(leverX, leverY);
            }
        }
    }


    
}
