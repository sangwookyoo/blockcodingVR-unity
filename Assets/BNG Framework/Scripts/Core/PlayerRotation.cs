using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BNG {

    public enum RotationMechanic {
        Snap,
        Smooth
    }
    public class PlayerRotation : MonoBehaviour {

        [Header("Input")]
        [Tooltip("Set to false to skip Update")]
        public bool AllowInput = true;

        [Tooltip("Used to determine whether to turn left / right. This can be an X Axis on the thumbstick, for example. -1 to snap left, 1 to snap right.")]
        public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.RightThumbStickAxis };

        [Header("Smooth / Snap Turning")]
        [Tooltip("Snap rotation will rotate a fixed amount of degrees on turn. Smooth will linearly rotate the player.")]
        public RotationMechanic RotationType = RotationMechanic.Snap;

        [Tooltip("How many degrees to rotate if RotationType is set to 'Snap'")]
        public float SnapRotationAmount = 45f;

        [Tooltip("Thumbstick X axis must be >= this amount to be considered an input event")]
        public float SnapInputAmount = 0.75f;

        [Tooltip("How fast to rotate the player if RotationType is set to 'Smooth'")]
        public float SmoothTurnSpeed = 40f;

        [Tooltip("Thumbstick X axis must be >= this amount to be considered an input event")]
        public float SmoothTurnMinInput = 0.1f;

        [Header("Keyboard Input")]
        /// <summary>
        /// Allow Q,E to rotate player
        /// </summary>
        [Tooltip("Allow Q,E to rotate player")]
        public bool AllowKeyboardInputs = true;

        float recentSnapTurnTime;        

        /// <summary>
        /// How much to rotate this frame
        /// </summary>
        float rotationAmount = 0;

        float previousXInput;

        // Optionally provide a player controller to update last move time.
        BNGPlayerController player;

        void Start() {
            player = GetComponentInParent<BNGPlayerController>();
        }

        void Update() {

            if(!AllowInput) {
                return;
            }

            float xAxis = GetAxisInput();

            if (RotationType == RotationMechanic.Snap) {
                DoSnapRotation(xAxis);
            }

            else if (RotationType == RotationMechanic.Smooth) {
                DoSmoothRotation(xAxis);
            }

            // Store input for future checks
            previousXInput = xAxis;
        }

        /// <summary>
        /// Return a float between -1 and 1 to determine which direction to turn the character
        /// </summary>
        /// <returns></returns>
        public virtual float GetAxisInput() {

            // Use the largest, non-zero value we find in our input list
            float lastVal = 0;

            for (int i = 0; i < inputAxis.Count; i++) {
                float axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]).x;

                // Always take this value if our last entry was 0. 
                if(lastVal == 0) {
                    lastVal = axisVal;
                }
                else if (axisVal != 0 && axisVal > lastVal) {
                    lastVal = axisVal;
                }
            }

            return lastVal;
        }

        public virtual void DoSnapRotation(float xInput) {

            // Reset rotation amount before retrieving inputs
            rotationAmount = 0;

            // Snap Right
            if (xInput >= SnapInputAmount && previousXInput < SnapInputAmount) {
                rotationAmount += SnapRotationAmount;
            }
            // Snap Left
            else if (xInput <= -SnapInputAmount && previousXInput > -SnapInputAmount) {
                rotationAmount -= SnapRotationAmount;
            }

            // Only allow keyboard if no vr input provided
            if (AllowKeyboardInputs && rotationAmount == 0) {
                //Use keys to ratchet rotation
                if (Input.GetKeyDown(KeyCode.Q)) {
                    rotationAmount -= SnapRotationAmount;
                }

                if (Input.GetKeyDown(KeyCode.E)) {
                    rotationAmount += SnapRotationAmount;
                }
            }

            if(Math.Abs(rotationAmount) > 0) {
                // Apply rotation
                transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotationAmount, transform.eulerAngles.z));

                recentSnapTurnTime = Time.time;

                if(player) {
                    player.UpdateLastSnapTime();
                }                                
            }
        }

        public virtual bool RecentlySnapTurned() {
            return Time.time - recentSnapTurnTime <= 0.1f;
        }

        public virtual void DoSmoothRotation(float xInput) {

            // Reset rotation amount before retrieving inputs
            rotationAmount = 0;

            // Smooth Rotate Right
            if (xInput >= SmoothTurnMinInput) {
                rotationAmount += xInput * SmoothTurnSpeed * Time.deltaTime;
            }
            // Smooth Rotate Left
            else if (xInput <= -SmoothTurnMinInput) {
                rotationAmount += xInput * SmoothTurnSpeed * Time.deltaTime;
            }

            // Apply rotation
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotationAmount, transform.eulerAngles.z));
        }
    }
}

