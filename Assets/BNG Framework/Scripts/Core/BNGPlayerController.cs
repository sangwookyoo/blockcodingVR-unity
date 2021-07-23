using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public enum LocomotionType {
        Teleport,
        SmoothLocomotion,
        None
    }

    /// <summary>
    /// The BNGPlayerController handles basic player movement
    /// </summary>
    public class BNGPlayerController : MonoBehaviour {

        [Header("Camera Options : ")]

        [Tooltip("If true the CharacterController will move along with the HMD, as long as there are no obstacle's in the way")]
        public bool MoveCharacterWithCamera = true;

        [Tooltip("If true the CharacterController will rotate it's Y angle to match the HMD's Y angle")]
        public bool RotateCharacterWithCamera = true;

        [Header("Transform Setup ")]

        [Tooltip("The TrackingSpace represents your tracking space origin.")]
        public Transform TrackingSpace;

        [Tooltip("The CameraRig is a Transform that is used to offset the main camera. The main camera should be parented to this.")]
        public Transform CameraRig;

        [Tooltip("The CenterEyeAnchor is typically the Transform that contains your Main Camera")]
        public Transform CenterEyeAnchor;

        [Header("Moving Platform Support")]
        [Tooltip("If true, the player will be parented to any object below it that has the 'MovingPlatform' component attached.")]
        public bool CheckForMovingPlatforms = true;

        [Header("Ground checks : ")]
        [Tooltip("Raycast against these layers to check if player is grounded")]
        public LayerMask GroundedLayers;

        /// <summary>
        /// 0 means we are grounded
        /// </summary>
        [Tooltip("How far off the ground the player currently is. 0 = Grounded, 1 = 1 Meter in the air.")]
        public float DistanceFromGround = 0;

        [Header("Player Capsule Settings : ")]
        /// <summary>
        /// Minimum Height our Player's capsule collider can be (in meters)
        /// </summary>
        [Tooltip("Minimum Height our Player's capsule collider can be (in meters)")]
        public float MinimumCapsuleHeight = 0.4f;

        /// <summary>
        /// Maximum Height our Player's capsule collider can be (in meters)
        /// </summary>
        [Tooltip("Maximum Height our Player's capsule collider can be (in meters)")]
        public float MaximumCapsuleHeight = 3f;        

        [HideInInspector]
        public float LastTeleportTime;

        [Header("Player Y Offset : ")]
        /// <summary>
        /// Offset the height of the CharacterController by this amount
        /// </summary>
        [Tooltip("Offset the height of the CharacterController by this amount")]
        public float CharacterControllerYOffset = -0.025f;

        /// <summary>
        /// Height of our camera in local coords
        /// </summary>
        [HideInInspector]
        public float CameraHeight;

        [Header("Misc : ")]                

        [Tooltip("If true the Camera will be offset by ElevateCameraHeight if no HMD is active or connected. This prevents the camera from falling to the floor and can allow you to use keyboard controls.")]
        public bool ElevateCameraIfNoHMDPresent = true;

        [Tooltip("How high (in meters) to elevate the player camera if no HMD is present and ElevateCameraIfNoHMDPresent is true. 1.65 = about 5.4' tall. ")]
        public float ElevateCameraHeight = 1.65f;

        /// <summary>
        /// If player goes below this elevation they will be reset to their initial starting position.
        /// If the player goes too far away from the center they may start to jitter due to floating point precisions.
        /// Can also use this to detect if player somehow fell through a floor. Or if the "floor is lava".
        /// </summary>
        [Tooltip("Minimum Y position our player is allowed to go. Useful for floating point precision and making sure player didn't fall through the map.")]
        public float MinElevation = -6000f;

        /// <summary>
        /// If player goes above this elevation they will be reset to their initial starting position.
        /// If the player goes too far away from the center they may start to jitter due to floating point precisions.
        /// </summary>
        public float MaxElevation = 6000f;

        [HideInInspector]
        public float LastPlayerMoveTime;

        // The controller to manipulate
        CharacterController characterController;

        // Optional components can be used to update LastMoved Time
        PlayerRotation playerRotation;
        PlayerClimbing playerClimbing;

        // This the object that is currently beneath us
        RaycastHit groundHit;
        Transform mainCamera;
        Vector3 lastPlayerPosition;
        Quaternion lastPlayerRotation;
        float lastSnapTime;

        private Vector3 _initialPosition;
        private Transform _initialCharacterParent;

        void Start() {
            characterController = GetComponentInChildren<CharacterController>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

            if (characterController) {
                _initialCharacterParent = characterController.transform.parent;
                _initialPosition = characterController.transform.position;
            }
            else {
                _initialPosition = transform.position;
            }

            float initialY = _initialPosition.y;
            if (initialY < MinElevation) {
                Debug.LogWarning("Initial Starting Position is lower than Minimum Elevation. Increasing Min Elevation to " + MinElevation);
                MinElevation = initialY;
            }
            if (initialY > MaxElevation) {
                Debug.LogWarning("Initial Starting Position is greater than Maximum Elevation. Reducing Max Elevation to " + MaxElevation);
                MaxElevation = initialY;
            }

            playerRotation = GetComponentInChildren<PlayerRotation>();
            playerClimbing = GetComponentInChildren<PlayerClimbing>();

            // Player root must be at 0,0,0 for Tracking Space to work properly.
            // If this player transform was moved in the editor on load, we can fix it by moving the CharacterController to the position
            if (transform.position != Vector3.zero || transform.localEulerAngles != Vector3.zero) {
                Vector3 playerPos = transform.position;
                Quaternion playerRot = transform.rotation;

                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;

                if (characterController) {
                    characterController.transform.position = playerPos;
                    characterController.transform.rotation = playerRot;
                }

                Debug.Log("Player position not set to 0. Moving player to : " + playerPos);
            }
        }

        void Update() {

            // Sanity check for camera
            if (mainCamera == null && Camera.main != null) {
                mainCamera = Camera.main.transform;
            }
            
            // Update the Character Controller's Capsule Height to match our Camera position
            UpdateCharacterHeight();

            // Update the position of our camera rig to account for our player's height
            UpdateCameraRigPosition();

            // After positioning the camera rig, we can update our main camera's height
            UpdateCameraHeight();

            CheckCharacterCollisionMove();

            if (characterController) {

                // Align TrackingSpace with Camera
                if (RotateCharacterWithCamera) {
                    RotateTrackingSpaceToCamera();
                }

                // Update Last snap time based on character controller rotation
                if (Mathf.Abs(Quaternion.Angle(lastPlayerRotation, characterController.transform.rotation)) > 1) {
                    UpdateLastSnapTime();
                }
            }

            if(CheckForMovingPlatforms) {
                checkMovingPlatform();
            }

            // Update the last known player location at the end of the frame
            UpdateLastPlayerPosition();
        }
       
        void FixedUpdate() {

            UpdateDistanceFromGround();

            CheckPlayerElevationRespawn();
        }

        /// <summary>
        /// Check if the player has moved beyond the specified min / max elevation
        /// Player should never go above or below 6000 units as physics can start to jitter due to floating point precision
        /// Maybe they clipped through a floor, touched a set "lava" height, etc.
        /// </summary>
        public virtual void CheckPlayerElevationRespawn() {

            // No need for elevation checks
            if(MinElevation == 0 && MaxElevation == 0) {
                return;
            }

            // Check Elevation based on Character Controller height
            if(characterController != null && (characterController.transform.position.y < MinElevation || characterController.transform.position.y > MaxElevation)) {
                Debug.Log("Player out of bounds; Returning to initial position.");
                characterController.transform.position = _initialPosition;
            }
        }

        public virtual void UpdateDistanceFromGround() {

            if(characterController) {
                if (Physics.Raycast(characterController.transform.position, -characterController.transform.up, out groundHit, 20, GroundedLayers, QueryTriggerInteraction.Ignore)) {
                    DistanceFromGround = Vector3.Distance(characterController.transform.position, groundHit.point);
                    DistanceFromGround += characterController.center.y;
                    DistanceFromGround -= (characterController.height * 0.5f) + characterController.skinWidth;

                    // Round to nearest thousandth
                    DistanceFromGround = (float)Math.Round(DistanceFromGround * 1000f) / 1000f;
                }
                else {
                    DistanceFromGround = 9999f;
                }
            }
            // No CharacterController found. Update Distance based on current transform position
            else {
                if (Physics.Raycast(transform.position, transform.up, out groundHit, 20, GroundedLayers, QueryTriggerInteraction.Ignore)) {
                    DistanceFromGround = Vector3.Distance(transform.position, groundHit.point);
                    // Round to nearest thousandth
                    DistanceFromGround = (float)Math.Round(DistanceFromGround * 1000f) / 1000f;
                }
                else {
                    DistanceFromGround = 9999f;
                }
            }
        }

        public virtual void RotateTrackingSpaceToCamera() {
            Vector3 initialPosition = TrackingSpace.position;
            Quaternion initialRotation = TrackingSpace.rotation;

            // Move the character controller to the proper rotation / alignment
            characterController.transform.rotation = Quaternion.Euler(0.0f, CenterEyeAnchor.rotation.eulerAngles.y, 0.0f);

            // Now we can rotate our tracking space back to initial position / rotation
            TrackingSpace.position = initialPosition;
            TrackingSpace.rotation = initialRotation;
        }

        // Update the last time we snapped the player rotation
        public virtual void UpdateLastSnapTime() {
            lastSnapTime = Time.time; 
        }

        public virtual bool RecentlyMoved() {

            // Recently Moved if position changed to teleport of some kind
            if(characterController != null && Vector3.Distance(lastPlayerPosition, characterController.transform.position) > 0.001f) {
                return true;
            }

            // Considered recently moved if just teleported
            if (Time.time - LastTeleportTime < 0.1f) {
                return true;
            }

            // Considered recently moved if just moved using PlayerController (for example, snap turning)
            if (Time.time - LastPlayerMoveTime < 0.1f) {
                return true;
            }

            // Recently Snap Turned through rotation
            if(playerRotation != null && playerRotation.RecentlySnapTurned()) {
                return true;
            }

            // Recent Snap Turn
            if(Time.time - lastSnapTime < 0.2f) {
                return true;
            }

            return false;
        }        

        public virtual void UpdateCameraRigPosition() {

            float yPos = CharacterControllerYOffset;

            // Get character controller position based on the height and center of the capsule
            if (characterController != null) {
                yPos = -(0.5f * characterController.height) + characterController.center.y + CharacterControllerYOffset;
            }
            
            // Offset the capsule a bit while climbing. This allows the player to more easily hoist themselves onto a ledge / platform.
            if (playerClimbing != null && playerClimbing.GrippingAtLeastOneClimbable()) {
                yPos -= 0.25f;
            }

            // If no HMD is active, bump our rig up a bit so it doesn't sit on the floor
            if(!InputBridge.Instance.HMDActive && ElevateCameraIfNoHMDPresent) {
                yPos += ElevateCameraHeight;
            }

            CameraRig.transform.localPosition = new Vector3(CameraRig.transform.localPosition.x, yPos, CameraRig.transform.localPosition.z);
        }

        public virtual void UpdateCharacterHeight() {
            float minHeight = MinimumCapsuleHeight;
            // Increase Min Height if no HMD is present. This prevents our character from being really small
            if(!InputBridge.Instance.HMDActive && minHeight < 1f) {
                minHeight = 1f;
            }

            // Update Character Height based on Camera Height.
            if(characterController) {
                characterController.height = Mathf.Clamp(CameraHeight + CharacterControllerYOffset - characterController.skinWidth, minHeight, MaximumCapsuleHeight);

                // If we are climbing set the capsule center upwards
                if (playerClimbing != null && playerClimbing.GrippingAtLeastOneClimbable()) {
                    characterController.height = playerClimbing.ClimbingCapsuleHeight;
                    characterController.center = new Vector3(0, playerClimbing.ClimbingCapsuleCenter, 0);
                }
                else {
                    characterController.center = new Vector3(0, -0.25f, 0);
                }
            }
        }

        public virtual void UpdateCameraHeight() {
            // update camera height
            if (CenterEyeAnchor) {
                CameraHeight = CenterEyeAnchor.localPosition.y;
            }
        }

        /// <summary>
        /// Move the character controller to new camera position
        /// </summary>
        public virtual void CheckCharacterCollisionMove() {

            if(!MoveCharacterWithCamera || characterController == null) {
                return;
            }
            
            Vector3 initialCameraRigPosition = CameraRig.transform.position;
            Vector3 cameraPosition = CenterEyeAnchor.position;
            Vector3 delta = cameraPosition - characterController.transform.position;
            
            // Ignore Y position
            delta.y = 0;

            // Move Character Controller and Camera Rig to Camera's delta
            if (delta.magnitude > 0.0f) {
                characterController.Move(delta);

                // Move Camera Rig back into position
                CameraRig.transform.position = initialCameraRigPosition;
            }
        }

        public virtual void checkMovingPlatform() {
            bool onMovingPlatform = false;

            if (groundHit.collider != null && DistanceFromGround < 0.01f) {
                MoveToWaypoint waypoint = groundHit.collider.gameObject.GetComponent<MoveToWaypoint>();
                MovingPlatform platform = groundHit.collider.gameObject.GetComponent<MovingPlatform>();

                if (platform) {
                    onMovingPlatform = true;

                    // This is another potential method of moving the character instead of parenting it
                    // if (waypoint != null && waypoint.PositionDifference != Vector3.zero) {
                        //characterController.Move(platform.PositionDifference);
                    //}
                }
            }

            // For now we can parent the characterController object to move it along. Rigidbodies may want to change friction materials or alter the player's velocity
            if (characterController != null) {
                if (onMovingPlatform) {
                    characterController.transform.parent = groundHit.collider.transform;
                }
                else {
                    characterController.transform.parent = _initialCharacterParent;
                }
            }
        }

        /// <summary>
        /// Updates the last known player location. Can be used to determine if a player has moved or rotated since the previous frame
        /// </summary>
        public virtual void UpdateLastPlayerPosition() {
            // Store player position so we can compare against it next frame
            if (characterController) {
                lastPlayerPosition = characterController.transform.position;
                lastPlayerRotation = characterController.transform.rotation;
            }
            else {
                lastPlayerPosition = transform.position;
                lastPlayerRotation = transform.rotation;
            }
        }

        public bool IsGrounded() {

            // Immediately check for a positive from a CharacterController if it's present
            if(characterController != null) {
                if(characterController.isGrounded) {
                    return true;
                }
            }

            // DistanceFromGround is a bit more reliable as we can give a bit of leniency in what's considered grounded
            return DistanceFromGround <= 0.001f;
        }
    }
}
