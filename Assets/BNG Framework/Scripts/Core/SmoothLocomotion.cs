using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public enum MovementVector {
        HMD,
        Controller
    }    

    public class SmoothLocomotion : MonoBehaviour {

        [Header("Movement : ")]
        public float MovementSpeed = 1.25f;

        [Tooltip("Used to determine which direction to move. Example : Left Thumbstick Axis or Touchpad. ")]
        public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.LeftThumbStickAxis };

        [Header("Sprint : ")]

        public float SprintSpeed = 1.5f;

        /// <summary>
        /// The key(s) to use to enabled Sprint. Override SprintKeyDown() if these don't fit your needs
        /// </summary>
        [Tooltip("The key(s) to use to initiate sprint. You can also override the SprintKeyDown() function to determine your sprint criteria.")]
        public List<ControllerBinding> SprintInput = new List<ControllerBinding>() { ControllerBinding.None };

        [Header("Strafe : ")]
        public float StrafeSpeed = 1f;
        public float StrafeSprintSpeed = 1.25f;

        [Header("Jump : ")]
        [Tooltip("Amount of 'force' to apply to the player during Jump")]
        public float JumpForce = 3f;

        /// <summary>
        /// The key(s) to use to enabled Sprint. Override SprintKeyDown() if these don't fit your needs
        /// </summary>
        [Tooltip("The key(s) to use to initiate a jump. You can also override the CheckJump() function to determine your jump criteria.")]
        public List<ControllerBinding> JumpInput = new List<ControllerBinding>() { ControllerBinding.None };

        [Header("Air Control : ")]
        [Tooltip("Can the player move when not grounded? Set to true if you want to be able to move the joysticks and have the player respond to input even when not grounded.")]
        public bool AirControl = true;

        [Tooltip("How fast the player can move in the air if AirControl = true. Example : 0.5 = Player will move at half the speed of MovementSpeed")]
        public float AirControlSpeed = 1f;

        [Header("Keyboard Input : ")]
        public bool AllowKeyboardInputs = true;

        BNGPlayerController playerController;
        CharacterController characterController;

        // Left / Right
        float movementX;

        // Up / Down
        float movementY;

        // Forwards / Backwards
        float movementZ;

        bool movementDisabled = false;

        private float _verticalSpeed = 0; // Keep track of vertical speed

        public virtual void Update() {
            CheckControllerReferences();
            UpdateInputs();            
            MoveCharacter();
        }

        public virtual void CheckControllerReferences() {
            // Component may be called while disabled, so check for references here
            if (playerController == null) {
                playerController = GetComponentInParent<BNGPlayerController>();
            }

            if(characterController == null) {
                characterController = GetComponent<CharacterController>();
            }
        }

        public virtual void UpdateInputs() {

            // Start by resetting our previous frame's inputs
            movementX = 0;
            movementY = 0;
            movementZ = 0;

            // Start with VR Controller Input
            Vector2 primaryAxis = GetAxisInput();
            if (playerController && playerController.IsGrounded()  ) {
                movementX = primaryAxis.x;
                movementZ = primaryAxis.y;
            }
            else if(AirControl) {
                movementX = primaryAxis.x * AirControlSpeed;
                movementZ = primaryAxis.y * AirControlSpeed;
            }

            // If VR Inputs not in use, check for keyboard inputs
            if(AllowKeyboardInputs && movementX == 0 && movementZ == 0) {
                GetKeyBoardInputs();
            }
            
            if(CheckJump()) {
                movementY += JumpForce;
            }

            if(SprintKeyDown()) {
                movementX *= StrafeSprintSpeed;
                movementZ *= SprintSpeed;
            }
            else {
                movementX *= StrafeSpeed;
                movementZ *= MovementSpeed;
            }            
        }

        public virtual Vector2 GetAxisInput() {
            // Use the largest, non-zero value we find in our input list
            Vector3 lastAxisValue = Vector3.zero;

            for (int i = 0; i < inputAxis.Count; i++) {
                Vector3 axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]);

                // Always take this value if our last entry was 0. 
                if (lastAxisValue == Vector3.zero) {
                    lastAxisValue = axisVal;
                }
                else if (axisVal != Vector3.zero && axisVal.magnitude > lastAxisValue.magnitude) {
                    lastAxisValue = axisVal;
                }
            }

            return lastAxisValue;
        }

        public virtual void MoveCharacter() {

            if(movementDisabled || characterController == null) {
                return;
            }

            Vector3 moveDirection = new Vector3(movementX, movementY, movementZ);
            moveDirection = transform.TransformDirection(moveDirection);

            // Check for jump value
            if (playerController != null && playerController.IsGrounded() && !movementDisabled) {
                // Reset jump speed if grounded
                _verticalSpeed = 0;
                if (CheckJump()) {
                    _verticalSpeed = JumpForce;
                }
            }

            moveDirection.y = _verticalSpeed;

            if(playerController) {
                playerController.LastPlayerMoveTime = Time.time;
            }
            

            if(moveDirection != Vector3.zero) {
                characterController.Move(moveDirection * Time.deltaTime);
            }
        }

        public virtual void GetKeyBoardInputs() {
            // Forward
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                movementZ += 1f;
            }
            // Back
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                movementZ -= 1f;
            }
            // Left
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                movementX -= 1f;
            }
            // Right
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                movementX += 1f;
            }
        }

        public virtual bool CheckJump() {

            // Don't jump if not grounded
            if(playerController != null && !playerController.IsGrounded()) {
                return false;
            }

            // Check for bound controller button
            for (int x = 0; x < JumpInput.Count; x++) {
                if (InputBridge.Instance.GetControllerBindingValue(JumpInput[x])) {
                    return true;
                }
            }

            // Keyboard input
            if (AllowKeyboardInputs && Input.GetKeyDown(KeyCode.Space)) {
                return true;
            }

            return false;
        }

        public virtual bool SprintKeyDown() {

            // Allow Keyboard Sprinting
            if(AllowKeyboardInputs &&  (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))) {
                return true;
            }

            // Check for bound controller button
            for (int x = 0; x < SprintInput.Count; x++) {
                if (InputBridge.Instance.GetControllerBindingValue(SprintInput[x])) {
                    return true;
                }
            }

            return false;
        }

        public virtual void EnableMovement() {
            movementDisabled = false;
        }

        public virtual void DisableMovement() {
            movementDisabled = true;
        }
    }
}

