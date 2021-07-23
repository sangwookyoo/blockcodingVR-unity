using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Apply Gravity to a CharacterController or RigidBody
    /// </summary>
    public class PlayerGravity : MonoBehaviour {

        [Tooltip("If true, will apply gravity to the CharacterController component, or RigidBody if no CC is present.")]

        public bool GravityEnabled = true;

        [Tooltip("Amount of Gravity to apply to the CharacterController or Rigidbody. Default is 'Physics.gravity'.")]
        public Vector3 Gravity = Physics.gravity;

        CharacterController characterController;
        Rigidbody playerRigidbody;

        private float _movementY;
        private Vector3 _initialGravityModifier;

        // Save us a null check in FixedUpdate
        private bool _validRigidBody = false;

        void Start() {
            characterController = GetComponent<CharacterController>();
            playerRigidbody = GetComponent<Rigidbody>();

            _validRigidBody = playerRigidbody != null;

            _initialGravityModifier = Gravity;
        }

        // Apply Gravity in LateUpdate to ensure it gets applied after any character movement is applied in Update
        void LateUpdate() {

            // Apply Gravity to Character Controller
            if (GravityEnabled && characterController != null && characterController.enabled) {
                _movementY += Gravity.y * Time.deltaTime;
                characterController.Move(new Vector3(0, _movementY, 0) * Time.deltaTime);

                if (characterController.isGrounded) {
                    _movementY = 0;
                }
            }
        }

        void FixedUpdate() {
            // Apply Gravity to Rigidbody Controller
            if (_validRigidBody && GravityEnabled) {
                playerRigidbody.AddForce(Gravity * (playerRigidbody.mass * playerRigidbody.mass));
            }
        }

        public void ToggleGravity(bool gravityOn) {

            GravityEnabled = gravityOn;

            if (gravityOn) {
                Gravity = _initialGravityModifier;
            }
            else {
                Gravity = Vector3.zero;
            }
        }
    }
}

