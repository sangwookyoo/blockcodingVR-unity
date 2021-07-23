using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public enum TeleportControls {
        // Rotate Thumbstick to initiate, release to teleport
        ThumbstickRotate,
        // Hold Thumbstick down to initiate, release to teleport
        ThumbstickDown,
        // Hold BButton to teleport, release to teleport
        BButton
    }

    /// <summary>
    /// A basic Teleport script that uses a parabolic arc to determine teleport location
    /// </summary>
    public class PlayerTeleport : MonoBehaviour {

        [Header("Colors")]

        [Tooltip("The LineRenderer to use when showing a teleport preview")]
        public LineRenderer TeleportLine;

        [Tooltip("If a Valid Teleport destination is found, color of 'TeleportLine' will be updated to this.")]
        public Color ValidTeleport = Color.green;

        [Tooltip("If a Valid Teleport destination is not found, color of 'TeleportLine' will be updated to this.")]
        public Color InvalidTeleport = Color.red;

        [Header("Hand Side")]
        [Tooltip("Whether the Teleport should initiate from the left or right controller. This affects input and where the teleport line should begin from.")]
        public ControllerHand HandSide = ControllerHand.Left;

        [Header("Transform Definitions")]
        [Tooltip("Where the Teleport Line should begin if using the left ControllerHand")]
        public Transform TeleportBeginTransform;

        [Tooltip("Where the Teleport Line should begin if using the right ControllerHand")]
        public Transform TeleportBeginTransformAlt;

        // Get the Starting Point of the Teleport transform depending on if left or right handed
        Transform teleportTransform {
            get {
                return HandSide == ControllerHand.Left ? TeleportBeginTransform : TeleportBeginTransformAlt;
            }
        }

        // Get the Thumbstick Axis depending on if left or right handed.
        Vector2 handedThumbstickAxis {
            get {
                return HandSide == ControllerHand.Left ? input.LeftThumbstickAxis : input.RightThumbstickAxis;
            }
        }

        [Tooltip("Transform indicating where Player should be placed on teleport.")]
        public Transform TeleportDestination;

        [Tooltip("GameObject to move around when initiating a teleport.")]
        public GameObject TeleportMarker;

        [Tooltip("Transform indicating direction Player will rotate to on teleport.")]
        public Transform DirectionIndicator;

        [Header("Teleport Physics")]

        public float MaxRange = 20f;

        [Tooltip("More segments means a smoother line, at the cost of performance.")]
        public int SegmentCount = 100;

        [Tooltip("How much velocity to apply when calculating a parabola. Set to a very high number for a straight line.")]
        public float SimulationVelocity = 500f;

        [Tooltip("Scale of each segment used when calculating parabola")]
        public float SegmentScale = 0.5f;

        [Header("Layers")]

        [Tooltip("Raycast layers to use when determining collision")]
        public LayerMask CollisionLayers;

        [Tooltip("Raycast layers to use when determining if the collided object is a valid teleport. If it is not valid then the line will be red and unable to teleport.")]
        public LayerMask ValidLayers;

        [Header("Controls")]
        [Tooltip("Method used to initiate a teleport. If these don't fit your needs you can override the KeyDownForTeleport() and KeyUpFromTeleport() methods.")]
        public TeleportControls ControlType = TeleportControls.ThumbstickRotate;

        [Tooltip("If true the user can rotate the teleport marker before initiating a teleport.")]
        public bool AllowTeleportRotation = true;
        private bool _reachThumbThreshold = false;

        [Header("Slope")]
        [Tooltip("Max Angle / Slope the teleport marker can be to be considered a valid teleport.")]
        public float MaxSlope = 60f;

        [Header("Screen Fade")]
        [Tooltip("Use ScreenFader on teleportation if true.")]
        public bool FadeScreenOnTeleport = true;

        [Tooltip("If FadeScreenOnTeleport = true, fade the screen at this speed.")]
        public float TeleportFadeSpeed = 10f;

        [Tooltip("Seconds to wait before initiating teleport. Useful if you want to fade the screen  before teleporting.")]
        public float TeleportDelay = 0.2f;        

        CharacterController controller;
        BNGPlayerController playerController;
        InputBridge input;
        Transform cameraRig;
        ScreenFader fader;

        bool aimingTeleport = false;
        bool validTeleport = false;
        bool teleportationEnabled = true;

        // How many frames teleport has been invalid for. 
        private int _invalidFrames = 0;

        // Initial Starting width of Line Renderer
        float _initialLineWidth;

        public delegate void OnBeforeTeleportAction();
        public static event OnBeforeTeleportAction OnBeforeTeleport;

        public delegate void OnAfterTeleportAction();
        public static event OnAfterTeleportAction OnAfterTeleport;

        void Start() {
            setupVariables();
        }

        bool setVariables = false;
        void setupVariables() {
            input = InputBridge.Instance;
            playerController = GetComponent<BNGPlayerController>();
            controller = GetComponentInChildren<CharacterController>();
            cameraRig = playerController.CameraRig;
            fader = cameraRig.GetComponentInChildren<ScreenFader>();

            // Make sure teleport line is a root object
            if (TeleportLine != null) {
                TeleportLine.transform.parent = null;
                TeleportLine.transform.position = Vector3.zero;
                TeleportLine.transform.rotation = Quaternion.identity;

                _initialLineWidth = TeleportLine.startWidth;
            }

            if (CollisionLayers == 0) {
                Debug.Log("Teleport layer not set. Setting Default.");
                CollisionLayers = 1;
            }

            // HTC Uses a Trackpad that is recognized as a Thumbstick. Force to Hold Downmode
            if (ControlType == TeleportControls.ThumbstickRotate && input.IsHTCDevice) {
                Debug.Log("HTC Device detected, switching Teleport Mode to Thumbstick Down");
                ControlType = TeleportControls.ThumbstickDown;
            }

            setVariables = true;
        }

        void Update() {

            // Are we pressing button to check for teleport?
            aimingTeleport = KeyDownForTeleport();            

            if (aimingTeleport) {
                // Ensure line is enabled if we are aiming
                TeleportLine.enabled = true;

                // Explicitly set width to force redraw of linerender
                Color updatedColor = validTeleport ? ValidTeleport : InvalidTeleport;
                if(!validTeleport && _invalidFrames < 3) {
                    updatedColor = ValidTeleport;
                }

                updatedColor.a = 1;
                TeleportLine.startColor = updatedColor;

                updatedColor.a = 0;
                TeleportLine.endColor = updatedColor;

                // Explicitly set width to force redraw of linerender
                TeleportLine.startWidth = _initialLineWidth;

                playerController.LastTeleportTime = Time.time;
                updateTeleport();
            }
            // released key, finish teleport or just hide graphics
            else if (KeyUpFromTeleport()) {
                if (validTeleport) {
                    tryTeleport();
                }
                else {
                    hideTeleport();
                }
            }
        }

        void FixedUpdate() {
            if (aimingTeleport) {
                calculateParabola();
            }
        }

        public void EnableTeleportation() {
            teleportationEnabled = true;
        }

        public void DisableTeleportation() {
            teleportationEnabled = false;
            validTeleport = false;
            aimingTeleport = false;

            hideTeleport();
        }

        // gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
        Collider _hitObject;
        private Vector3 _hitVector;
        float _hitAngle;

        void calculateParabola() {

            validTeleport = false;
            bool isDestination = false;

            Vector3[] segments = new Vector3[SegmentCount];

            segments[0] = teleportTransform.position;
            // Initial velocity
            Vector3 segVelocity = teleportTransform.forward * SimulationVelocity * Time.fixedDeltaTime;

            _hitObject = null;

            for (int i = 1; i < SegmentCount; i++) {

                // Hit something, so assign all future segments to this segment
                if (_hitObject != null) {
                    segments[i] = _hitVector;
                    continue;
                }

                // Time it takes to traverse one segment of length segScale (careful if velocity is zero)
                float segTime = (segVelocity.sqrMagnitude != 0) ? SegmentScale / segVelocity.magnitude : 0;

                // Add velocity from gravity for this segment's timestep
                segVelocity = segVelocity + Physics.gravity * segTime;

                // Check to see if we're going to hit a physics object
                RaycastHit hit;
                if (Physics.Raycast(segments[i - 1], segVelocity, out hit, SegmentScale, CollisionLayers)) {

                    // remember who we hit
                    _hitObject = hit.collider;

                    // set next position to the position where we hit the physics object
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;

                    // correct ending velocity, since we didn't actually travel an entire segment
                    segVelocity = segVelocity - Physics.gravity * (SegmentScale - hit.distance) / segVelocity.magnitude;                   

                    _hitAngle = Vector3.Angle(transform.up, hit.normal);

                    // Align marker to hit normal
                    TeleportMarker.transform.position = segments[i]; // hit.point;
                    TeleportMarker.transform.rotation = Quaternion.FromToRotation(TeleportMarker.transform.up, hit.normal) * TeleportMarker.transform.rotation;

                    // Snap to Teleport Destination
                    TeleportDestination td = _hitObject.GetComponent<TeleportDestination>();
                    if(td != null) {
                        isDestination = true;
                        TeleportMarker.transform.position = td.transform.position;
                        TeleportMarker.transform.rotation = td.transform.rotation;
                    }

                    _hitVector = segments[i];
                }
                // Nothing hit, continue line by settings next segment to the last
                else {
                    segments[i] = segments[i - 1] + segVelocity * segTime;
                }
            }

            validTeleport = _hitObject != null;

            // Make sure teleport location is valid
            // Destination Targets ignore checks
            if(validTeleport && !isDestination) {

                // Angle too steep
                if (_hitAngle > MaxSlope) {
                    validTeleport = false;
                }

                // Hit a grabbable object
                if (_hitObject.GetComponent<Grabbable>() != null) {
                    validTeleport = false;
                }

                // Hit a restricted zone
                if (_hitObject.GetComponent<InvalidTeleportArea>() != null) {
                    validTeleport = false;
                }

                // Something in the way via raycast
                if (!teleportClear() ) {
                    validTeleport = false;
                }
            }

            // Render the positions as a line
            TeleportLine.positionCount = SegmentCount;
            for (int i = 0; i < SegmentCount; i++) {
                TeleportLine.SetPosition(i, segments[i]);
            }

            if(!validTeleport) {
                _invalidFrames++;
            }
            else {
                _invalidFrames = 0;
            }
        }

        // Clear of obstacles
        bool teleportClear() {

            // Controller may have been cleared - double check it in clear method
            if(controller == null) {
                controller = GetComponentInChildren<CharacterController>();
            }

            // Something in the way via overlap sphere. Uses player capsule radius.
            Collider[] hitColliders = Physics.OverlapSphere(TeleportDestination.position, controller.radius, CollisionLayers, QueryTriggerInteraction.Ignore);
            if (hitColliders.Length > 0) {
                return false;
            }

            // Something in the way via Raycast up from teleport spot
            // Raycast from the ground up to the height of character controller
            if (Physics.Raycast(TeleportMarker.transform.position, TeleportMarker.transform.up, out RaycastHit hit, controller.height, CollisionLayers, QueryTriggerInteraction.Ignore)) {
                return false;
            }

            // Invalid Layer
            return ValidLayers == (ValidLayers | (1 << _hitObject.gameObject.layer));
        }

        void hideTeleport() {
            TeleportLine.enabled = false;

            if(TeleportMarker.activeSelf) {
                TeleportMarker.SetActive(false);
            }
        }

        // Raycast, update graphics
        void updateTeleport() {

            if(validTeleport) {

                // Rotate based on controller thumbstick
                rotateMarker();

                // Make sure constraint isn't active
                playerController.LastTeleportTime = Time.time;
            }

            // Show / Hide Teleport Marker
            TeleportMarker.SetActive(validTeleport);           
        }

        void rotateMarker() {

            if(AllowTeleportRotation) {

                if(DirectionIndicator != null && !DirectionIndicator.gameObject.activeSelf) {
                    DirectionIndicator.gameObject.SetActive(true);
                }

                Vector3 controllerDirection = new Vector3(handedThumbstickAxis.x, 0.0f, handedThumbstickAxis.y);

                //get controller pointing direction in world space
                controllerDirection = controller.transform.TransformDirection(controllerDirection);

                //get marker forward in local space
                Vector3 forward = TeleportMarker.transform.forward; // TeleportMarker.transform.InverseTransformDirection(TeleportMarker.transform.forward);

                //find the angle difference between the controller pointing direction and marker current forward
                float angle = Vector2.SignedAngle(new Vector2(controllerDirection.x, controllerDirection.z), new Vector2(forward.x, forward.z));

                //rotate marker in local space to match controller pointing direction
                TeleportMarker.transform.Rotate(Vector3.up, angle, Space.Self);
            }
            // Rotation Disabled
            else {
                if (DirectionIndicator != null && DirectionIndicator.gameObject.activeSelf) {
                    DirectionIndicator.gameObject.SetActive(false);
                }
            }
        }

        void tryTeleport() {

            if (validTeleport) {

                // Call any events, fade screen, etc.
                BeforeTeleport();

                Vector3 destination = TeleportDestination.position;
                Quaternion rotation = TeleportMarker.transform.rotation;
               
                // Override if we're looking at a teleport destination
                var dest = _hitObject.GetComponent<TeleportDestination>();
                if (dest != null) {
                    destination = dest.DestinationTransform.position;

                    if (dest.ForcePlayerRotation) {
                        rotation = dest.DestinationTransform.rotation;
                    }
                }

                StartCoroutine(doTeleport(destination, rotation, AllowTeleportRotation));
            }

            // We teleported, so update this value for next raycast
            validTeleport = false;
            aimingTeleport = false;

            hideTeleport();
        }

        public virtual void BeforeTeleport() {

            if(FadeScreenOnTeleport && fader) {
                fader.FadeInSpeed = TeleportFadeSpeed;
                fader.DoFadeIn();
            }

            // Call any Before Teleport Events
            OnBeforeTeleport?.Invoke();
        }

        public virtual void AfterTeleport() {
            
            if (FadeScreenOnTeleport && fader) {
                fader.DoFadeOut();
            }

            // Call any After Teleport Events
            OnAfterTeleport?.Invoke();
        }

        IEnumerator doTeleport(Vector3 playerDestination, Quaternion playerRotation, bool rotatePlayer) {

            if(!setVariables) {
                setupVariables();
            }

            if(TeleportDelay > 0) {
                yield return new WaitForSeconds(TeleportDelay);
            }

            controller.enabled = false;
            playerController.LastTeleportTime = Time.time;

            // Calculate teleport offset as character may have been resized
            float yOffset = 1 + cameraRig.localPosition.y - playerController.CharacterControllerYOffset;

            // Apply Teleport before offset is applied
            controller.transform.position = playerDestination;

            // Apply offset
            controller.transform.localPosition -= new Vector3(0, yOffset, 0);

            // Rotate player to TeleportMarker Rotation
            if (rotatePlayer) {
                controller.transform.rotation = playerRotation;

                // Force our character to remain upright
                controller.transform.eulerAngles = new Vector3(0, controller.transform.eulerAngles.y, 0);
            }

            // Call events, etc.
            AfterTeleport();            

            yield return new WaitForEndOfFrame();
            
            // Re-Enable the character controller so we can move again
            controller.enabled = true;
        }

        public void TeleportPlayer(Vector3 destination, Quaternion rotation) {
            StartCoroutine(doTeleport(destination, rotation, true));
        }

        public void TeleportPlayerToTransform(Transform destination) {
            StartCoroutine(doTeleport(destination.position, destination.rotation, true));
        }

        // Are we pressing proper key to initiate teleport?
        public virtual bool KeyDownForTeleport() {

            // Make sure we can use teleport
            if(!teleportationEnabled) {
                return false;
            }

            // Press stick in any direction to initiate teleport
            if (ControlType == TeleportControls.ThumbstickRotate) {
                if(Math.Abs(handedThumbstickAxis.x) >= 0.75 || Math.Abs(handedThumbstickAxis.y) >= 0.75) {
                    _reachThumbThreshold = true;
                    return true;
                }
                // In dead zone
                else if (_reachThumbThreshold && (Math.Abs(handedThumbstickAxis.x) > 0.25 || Math.Abs(handedThumbstickAxis.y) > 0.25)) {
                    return true;
                }
            }
            // Hold down Thumbstick to intiate
            else if (ControlType == TeleportControls.ThumbstickDown) {
                if (input.LeftThumbstick && HandSide == ControllerHand.Left) {
                    return true;
                }
                else if (input.RightThumbstick && HandSide == ControllerHand.Right) {
                    return true;
                }
            }
            else if(ControlType == TeleportControls.BButton) {
                return input.BButton;
            }

            return false;
        }

        public virtual bool KeyUpFromTeleport() {

            // Stick has returned back past input position
            if (ControlType == TeleportControls.ThumbstickRotate) {
                if(Math.Abs(input.LeftThumbstickAxis.x) <= 0.25 && Math.Abs(input.LeftThumbstickAxis.y) <= 0.25) {
                    // Reset threshold
                    _reachThumbThreshold = false;
                    return true;
                }

                return false;
            }
            // Or no longer holding down Thumbstick
            else if (ControlType == TeleportControls.ThumbstickDown) {
                return !input.LeftThumbstick;
            }
            // Or no longer holding down button
            else if (ControlType == TeleportControls.BButton) {
                return !input.BButton;
            }

            return true;
        }

        void OnDrawGizmosSelected() {
            if (controller != null && TeleportDestination.gameObject.activeSelf) {
                Color gizColor = Color.red;
                gizColor.a = 0.9f;
                Gizmos.color = gizColor;
                Gizmos.DrawSphere(TeleportDestination.position, controller.radius);
            }
        }
    }
}