using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BNG {

    public enum GrabType {
        Snap,
        Precise
    }

    public enum GrabPhysics {
        None = 2,
        PhysicsJoint = 0,
        FixedJoint = 3,
        Velocity = 4,
        Kinematic = 1
    }

    public enum OtherGrabBehavior {
        None,
        SwapHands,
        DualGrab
    }

    /// <summary>
    /// An object that can be picked up by a Grabber
    /// </summary>
    public class Grabbable : MonoBehaviour {

        // [HideInInspector]
        public bool BeingHeld = false;
        List<Grabber> validGrabbers;

        /// <summary>
        /// The grabber that is currently holding us. Null if not being held
        /// </summary>        
        protected List<Grabber> heldByGrabbers;

        public List<Grabber> HeldByGrabbers {
            get {
                return heldByGrabbers;
            }
        }

        /// <summary>
        /// Save whether or not the RigidBody was kinematic on Start.
        /// </summary>
        protected bool wasKinematic;
        protected bool usedGravity;
        protected CollisionDetectionMode initialCollisionMode;
        protected RigidbodyInterpolation initialInterpolationMode;

        /// <summary>
        /// Is the object being pulled towards the Grabber
        /// </summary>
        protected bool remoteGrabbing;


        [Header("Grab Settings")]
        /// <summary>
        /// Configure which button is used to initiate the grab
        /// </summary>
        [Tooltip("Configure which button is used to initiate the grab")]
        public GrabButton GrabButton = GrabButton.Inherit;

        /// <summary>
        /// 'Inherit' will inherit this setting from the Grabber. 'Hold' requires the user to hold the GrabButton down. 'Toggle' will drop / release the Grabbable on button activation.
        /// </summary>
        [Tooltip("'Inherit' will inherit this setting from the Grabber. 'Hold' requires the user to hold the GrabButton down. 'Toggle' will drop / release the Grabbable on button activation.")]
        public HoldType Grabtype = HoldType.Inherit;

        /// <summary>
        /// Kinematic Physics locks the object in place on the hand / grabber. PhysicsJoint allows collisions with the environment.
        /// </summary>
        [Tooltip("Kinematic Physics locks the object in place on the hand / grabber. Physics Joint and Velocity types allow collisions with the environment.")]
        public GrabPhysics GrabPhysics = GrabPhysics.Velocity;

        /// <summary>
        /// Snap to a location or grab anywhere on the object
        /// </summary>
        [Tooltip("Snap to a location or grab anywhere on the object")]
        public GrabType GrabMechanic = GrabType.Precise;

        /// <summary>
        /// How fast to Lerp the object to the hand
        /// </summary>
        [Tooltip("How fast to Lerp the object to the hand")]
        public float GrabSpeed = 15f;

        /// <summary>
        /// Can the object be picked up from far away. Must be within RemoteGrabber Trigger
        /// </summary>
        [Header("Remote Grab")]
        [Tooltip("Can the object be picked up from far away. Must be within RemoteGrabber Trigger")]
        public bool RemoteGrabbable = false;

        /// <summary>
        /// Max Distance Object can be Remote Grabbed. Not applicable if RemoteGrabbable is false
        /// </summary>
        [Tooltip("Max Distance Object can be Remote Grabbed. Not applicable if RemoteGrabbable is false")]
        public float RemoteGrabDistance = 2f;

        /// <summary>
        /// Multiply controller's velocity times this when throwing
        /// </summary>
        [Header("Throwing")]
        [Tooltip("Multiply controller's velocity times this when throwing")]
        public float ThrowForceMultiplier = 2f;

        /// <summary>
        /// Multiply controller's angular velocity times this when throwing
        /// </summary>
        [Tooltip("Multiply controller's angular velocity times this when throwing")]
        public float ThrowForceMultiplierAngular = 1.5f; // Multiply Angular Velocity times this

        /// <summary>
        /// Drop the item if object's center travels this far from the Grabber's Center (in meters). Set to 0 to disable distance break.
        /// </summary>
        [Tooltip("Drop the item if object's center travels this far from the Grabber's Center (in meters). Set to 0 to disable distance break.")]
        public float BreakDistance = 0;

        /// <summary>
        /// Enabling this will hide the Transform specified in the Grabber's HandGraphics property
        /// </summary>
        [Header("Hand Options")]
        [Tooltip("Enabling this will hide the Transform specified in the Grabber's HandGraphics property")]
        public bool HideHandGraphics = false;

        /// <summary>
        ///  Parent this object to the hands for better stability.
        ///  Not recommended for child grabbers
        /// </summary>
        [Tooltip("Parent this object to the hands for better stability during movement (Recommended unless this object is a Kinematic Rigidbody or child Grabbable / Rigidbody)")]
        public bool ParentToHands = true;

        /// <summary>
        /// If true, the hand model will be attached to the grabbed object
        /// </summary>
        [Tooltip("If true, the hand model will be attached to the grabbed object. This separates it from a 1:1 match with the controller, but may look more realistic.")]
        public bool ParentHandModel = true;

        [Tooltip("If true, the hand model will snap to the nearest GrabPoint. Otherwise the hand model will stay with the Grabber.")]
        public bool SnapHandModel = true;

        /// <summary>
        /// Set to false to disable dropping. If false, will be permanently attached to whatever grabs this.
        /// </summary>
        [Header("Misc")]
        [Tooltip("Set to false to disable dropping. If false, will be permanently attached to whatever grabs this.")]
        public bool CanBeDropped = true;

        /// <summary>
        /// Can this object be snapped to snap zones? Set to false if you never want this to be snappable. Further filtering can be done on the SnapZones
        /// </summary>
        [Tooltip("Can this object be snapped to snap zones? Set to false if you never want this to be snappable. Further filtering can be done on the SnapZones")]
        public bool CanBeSnappedToSnapZone = true;

        [Tooltip("If true, the object will always have kinematic disabled when dropped, even if it was initially kinematic.")]
        public bool ForceDisableKinematicOnDrop = false;

        /// <summary>
        /// Animator ID of the Hand Pose to use
        /// </summary>
        [Header("Default Hand Pose")]
        [Tooltip("This HandPose Id will be passed to the Hand Animator when equipped. You can add new hand poses in the HandPoseDefinitions.cs file.")]
        public HandPoseId CustomHandPose = HandPoseId.Default;
        protected HandPoseId initialHandPoseId;

        /// <summary>
        /// What to do if another grabber grabs this while equipped. DualGrab is currently unsupported.
        /// </summary>
        [Header("Secondary Grab")]
        [Tooltip("What to do if another grabber grabs this while equipped. DualGrab is currently unsupported.")]
        public OtherGrabBehavior SecondaryGrabBehavior = OtherGrabBehavior.None;

        /// <summary>
        /// The Grabbable can only be grabbed if this grabbable is being held.
        /// Example : If you only want a weapon part to be grabbable if the weapon itself is being held.
        /// </summary>
        [Tooltip("The Grabbable can only be grabbed if this grabbable is being held. Example : If you only want a weapon part to be grabbable if the weapon itself is being held.")]
        public Grabbable OtherGrabbableMustBeGrabbed = null;


        [Header("Secondary Grab Look Settings")]
        [Tooltip("Look at this object if it is being held. For example, a rifle may look at a Grabable Grip on the object.")]
        public Grabbable SecondaryGrabbable;

        [Tooltip("How quickly to Lerp towards the SecondaryGrabbable")]
        public float SecondHandLookSpeed = 40f;


        [Header("Physics Joint Settings")]
        /// <summary>
        /// How much Spring Force to apply to the joint when something comes in contact with the grabbable
        /// A higher Spring Force will make the Grabbable more rigid
        /// </summary>
        [Tooltip("A higher Spring Force will make the Grabbable more rigid")]
        public float CollisionSpring = 3000;

        /// <summary>
        /// How much Slerp Force to apply to the joint when something is in contact with the grabbable
        /// </summary>
        [Tooltip("How much Slerp Force to apply to the joint when something is in contact with the grabbable")]
        public float CollisionSlerp = 500;

        [Tooltip("How to restrict the Configurable Joint's xMotion when colliding with an object. Position can be free, completely locked, or limited.")]
        public ConfigurableJointMotion CollisionLinearMotionX = ConfigurableJointMotion.Free;

        [Tooltip("How to restrict the Configurable Joint's yMotion when colliding with an object. Position can be free, completely locked, or limited.")]
        public ConfigurableJointMotion CollisionLinearMotionY = ConfigurableJointMotion.Free;

        [Tooltip("How to restrict the Configurable Joint's zMotion when colliding with an object. Position can be free, completely locked, or limited.")]
        public ConfigurableJointMotion CollisionLinearMotionZ = ConfigurableJointMotion.Free;

        [Tooltip("Restrict the rotation around the X axes to be Free, completely Locked, or Limited when colliding with an object.")]
        public ConfigurableJointMotion CollisionAngularMotionX = ConfigurableJointMotion.Free;

        [Tooltip("Restrict the rotation around the Y axes to be Free, completely Locked, or Limited when colliding with an object.")]
        public ConfigurableJointMotion CollisionAngularMotionY = ConfigurableJointMotion.Free;

        [Tooltip("Restrict the rotation around Z axes to be Free, completely Locked, or Limited when colliding with an object.")]
        public ConfigurableJointMotion CollisionAngularMotionZ = ConfigurableJointMotion.Free;


        [Tooltip("If true, the object's velocity will be adjusted to match the grabber. This is in addition to any forces added by the configurable joint.")]
        public bool ApplyCorrectiveForce = true;

        [Header("Velocity Grab Settings")]
        public float MoveVelocityForce = 3000f;
        public float MoveAngularVelocityForce = 90f;

        /// <summary>
        /// Time in seconds (Time.time) when we last grabbed this item
        /// </summary>
        [HideInInspector]
        public float LastGrabTime;

        /// <summary>
        /// Time in seconds (Time.time) when we last dropped this item
        /// </summary>
        [HideInInspector]
        public float LastDropTime;

        /// <summary>
        /// Set to True to throw the Grabbable by applying the controller velocity to the grabbable on drop. 
        /// Set False if you don't want the object to be throwable, or want to apply your own force manually
        /// </summary>
        [HideInInspector]
        public bool AddControllerVelocityOnDrop = true;

        // Total distance between the Grabber and Grabbable.
        float journeyLength;

        public float OriginalScale { get; private set; }

        // Keep track of objects that are colliding with us
        [Header("Shown for Debug : ")]
        [SerializeField]
        protected List<Collider> collisions;

        // Last time in seconds (Time.time) since we had a valid collision
        protected float lastCollisionSeconds;

        /// <summary>
        /// How many seconds we've gone without collisions
        /// </summary>
        protected float lastNoCollisionSeconds;

        // If Time.time < requestSpringTime, force joint to be springy
        protected float requestSpringTime;

        /// <summary>
        /// If Grab Mechanic is set to Snap, set position and rotation to this Transform on the primary Grabber
        /// </summary>
        protected Transform primaryGrabOffset;

        /// <summary>
        /// Returns the active GrabPoint component if object is held and a GrabPoint has been assigneed
        /// </summary>
        [HideInInspector]
        public GrabPoint ActiveGrabPoint;

        /// <summary>
        /// Considered grabbing with two-hands if the SecondaryGrabbable is being held
        /// </summary>
        bool grabbingTwoHands {
            get {
                return SecondaryGrabbable != null && SecondaryGrabbable.BeingHeld;
            }
        }

        [HideInInspector]
        public Vector3 SecondaryLookOffset;

        [HideInInspector]
        public Transform SecondaryLookAtTransform;

        [HideInInspector]
        public Transform LocalOffsetTransform;

        Vector3 grabPosition {
            get {
                if (primaryGrabOffset != null) {
                    return primaryGrabOffset.position;
                }
                else {
                    return transform.position;
                }
            }
        }

        [HideInInspector]
        public Vector3 GrabPositionOffset {
            get {
                if (primaryGrabOffset) {
                    return primaryGrabOffset.transform.localPosition;
                }

                return Vector3.zero;
            }
        }

        [HideInInspector]
        public Vector3 GrabRotationOffset {
            get {
                if (primaryGrabOffset) {
                    return primaryGrabOffset.transform.localEulerAngles;
                }
                return Vector3.zero;
            }
        }

        private Transform _grabTransform;

        // Position this on the grabber to get a precise location
        public Transform grabTransform {
            get {
                if (_grabTransform != null) {
                    return _grabTransform;
                }

                _grabTransform = new GameObject().transform;
                _grabTransform.parent = this.transform;
                _grabTransform.name = "Grab Transform";
                _grabTransform.localPosition = Vector3.zero;
                _grabTransform.hideFlags = HideFlags.HideInHierarchy;

                return _grabTransform;
            }
        }

        [Header("Grab Points")]
        /// <summary>
        /// If Grab Mechanic is set to Snap, the closest GrabPoint will be used. Add a SnapPoint Component to a GrabPoint to specify custom hand poses and rotation.
        /// </summary>
        [Tooltip("If Grab Mechanic is set to Snap, the closest GrabPoint will be used. Add a SnapPoint Component to a GrabPoint to specify custom hand poses and rotation.")]
        public List<Transform> GrabPoints;

        /// <summary>
        /// Can the object be moved towards a Grabber. 
        /// Levers, buttons, doorknobs, and other types of objects cannot be moved because they are attached to another object or are static.
        /// </summary>
        public bool CanBeMoved {
            get {
                return _canBeMoved;
            }
        }
        private bool _canBeMoved;

        protected Transform originalParent;
        protected InputBridge input;
        protected ConfigurableJoint connectedJoint;
        protected Vector3 previousPosition;
        protected float lastItemTeleportTime;
        protected bool recentlyTeleported;

        /// <summary>
        /// Set this to false if you need to see Debug field or don't want to use the custom inspector
        /// </summary>
        [HideInInspector]
        public bool UseCustomInspector = true;

        /// <summary>
        /// If a BNGPlayerController is provided we can check for player movements and make certain adjustments to physics.
        /// </summary>
        protected BNGPlayerController player {
            get {
                return GetBNGPlayerController();
            }
        }
        private BNGPlayerController _player;
        protected Collider col;
        protected Rigidbody rigid;
        protected Grabber flyingTo;

        protected List<GrabbableEvents> events;

        public bool DidParentHands {
            get {
                return didParentHands;
            }
        }
        protected bool didParentHands = false;

        void Awake() {
            col = GetComponent<Collider>();
            rigid = GetComponent<Rigidbody>();
            input = InputBridge.Instance;

            events = GetComponents<GrabbableEvents>().ToList();
            collisions = new List<Collider>();

            // Try parent if no rigid found here
            if (rigid == null && transform.parent != null) {
                rigid = transform.parent.GetComponent<Rigidbody>();
            }

            // Store initial rigidbody properties so we can reset them later as needed
            if (rigid) {
                initialCollisionMode = rigid.collisionDetectionMode;
                initialInterpolationMode = rigid.interpolation;
                wasKinematic = rigid.isKinematic;
                usedGravity = rigid.useGravity;
            }

            // Store initial parent so we can reset later if needed
            UpdateOriginalParent(transform.parent);

            validGrabbers = new List<Grabber>();

            // Set Original Scale based in World coordinates if available
            if (transform.parent != null) {
                OriginalScale = transform.parent.TransformVector(transform.localScale).x;
            }
            else {
                OriginalScale = transform.localScale.x;
            }

            initialHandPoseId = CustomHandPose;

            // Store movement status
            _canBeMoved = canBeMoved();
        }

        public virtual void Update() {

            if (remoteGrabbing) {
                Vector3 grabbablePosition = transform.position;
                Vector3 grabberPosition = getGrabberWithOffsetWorldPosition(flyingTo);

                Quaternion remoteRotation = getRemoteRotation(flyingTo);
                float dist = Vector3.Distance(grabbablePosition, grabberPosition);

                // reached destination, snap to final transform position
                // Typically this won't be hit as the Grabber trigger will pick it up first
                if (dist <= 0.002f) {
                    movePosition(grabberPosition);
                    moveRotation(grabTransform.rotation);

                    if(rigid) {
                        rigid.velocity = Vector3.zero;
                    }

                    if (flyingTo != null) {
                        flyingTo.GrabGrabbable(this);
                    }
                }
                // Getting close so speed up
                else if (dist < 0.05f) {
                    movePosition(Vector3.Lerp(transform.position, grabberPosition, Time.deltaTime * GrabSpeed * 1.5f));
                    moveRotation(Quaternion.Slerp(transform.rotation, remoteRotation, Time.deltaTime * GrabSpeed * 1.5f));
                }
                // Normal Lerp
                else {
                    movePosition(Vector3.Lerp(transform.position, grabberPosition, Time.deltaTime * GrabSpeed));
                    moveRotation(Quaternion.Slerp(transform.rotation, remoteRotation, Time.deltaTime * GrabSpeed));
                }
            }

            if (BeingHeld) {
                // Something happened to our Grabber. Drop the item
                if (heldByGrabbers == null) {
                    DropItem(null, true, true);
                    return;
                }

                // Make sure all collisions are valid
                filterCollisions();

                // Update collision time
                if (collisions != null && collisions.Count > 0) {
                    lastCollisionSeconds = Time.time;
                    lastNoCollisionSeconds = 0;
                }
                else if (collisions != null && collisions.Count <= 0) {
                    lastNoCollisionSeconds += Time.deltaTime;
                }

                // Update item recently teleported time
                if (Vector3.Distance(transform.position, previousPosition) > 0.1f) {
                    lastItemTeleportTime = Time.time;
                }
                recentlyTeleported = Time.time - lastItemTeleportTime < 0.2f;

                // Loop through held grabbers and see if we need to drop the item, fire off events, etc.
                for (int x = 0; x < heldByGrabbers.Count; x++) {
                    Grabber g = heldByGrabbers[x];

                    // Should we drop the item if it's too far away?
                    if (!recentlyTeleported && BreakDistance > 0 && Vector3.Distance(grabPosition, g.transform.position) > BreakDistance) {
                        Debug.Log("Break Distance Exceeded. Dropping item.");
                        DropItem(g, true, true);
                        break;
                    }

                    // Should we drop the item if no longer holding the required Grabbable?
                    if (OtherGrabbableMustBeGrabbed != null && !OtherGrabbableMustBeGrabbed.BeingHeld) {
                        // Fixed joints work ok. Configurable Joints have issues
                        if (GetComponent<ConfigurableJoint>() != null) {
                            DropItem(g, true, true);
                            break;
                        }
                    }

                    // Fire off any relevant events
                    callEvents(g);
                }

                // Check to parent the hand models to the Grabbable
                checkParentHands();

                // Position Hands in proper place
                positionHandGraphics();

                // Rotate the grabber to look at our secondary object
                checkSecondaryLook();

                // Keep track of where we were each frame
                previousPosition = transform.position;
            }
        }


        public virtual void FixedUpdate() {
            if (BeingHeld) {

                // Reset all collisions every physics update
                // These are then populated in OnCollisionEnter / OnCollisionStay to make sure we have the most up to date collision info
                // This can create garbage so only do this if we are holding the object
                if (BeingHeld && collisions != null && collisions.Count > 0) {
                    collisions = new List<Collider>();
                }

                // Update any physics properties here
                if (GrabPhysics == GrabPhysics.PhysicsJoint) {
                    UpdatePhysicsJoints();
                }
                else if (GrabPhysics == GrabPhysics.FixedJoint) {
                    UpdateFixedJoints();
                }
                else if (GrabPhysics == GrabPhysics.Kinematic) {
                    UpdateKinematicPhysics();
                }
                else if (GrabPhysics == GrabPhysics.Velocity) {
                    UpdateVelocityPhysics();
                }
            }
        }

        protected Vector3 getGrabberWithOffsetWorldPosition(Grabber grabber) {
            Vector3 grabberPosition = grabber.transform.position;

            // If there is a GrabPoint then we should offset the destination
            Transform grabPoint = GetClosestGrabPoint(grabber);
            if (grabPoint != null) {
                Vector3 localOffset = transform.position - grabPoint.position;
                grabberPosition += localOffset;
            }

            return grabberPosition;
        }

        protected void positionHandGraphics() {

            if (ParentHandModel && didParentHands) {
                if (GrabMechanic == GrabType.Snap) {
                    Grabber g = GetPrimaryGrabber();
                    g.HandsGraphics.localPosition = g.handsGraphicsGrabberOffset;
                    g.HandsGraphics.localEulerAngles = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// Is this object able to be grabbed. Does not check for valid Grabbers, only if it isn't being held, is active, etc.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsGrabbable() {

            // Not valid if not active
            if (!isActiveAndEnabled) {
                return false;
            }

            // Not valid if being held and the object has no secondary grab behavior
            if (BeingHeld == true && SecondaryGrabBehavior == OtherGrabBehavior.None) {
                return false;
            }

            // Make sure grabbed conditions are met
            if (OtherGrabbableMustBeGrabbed != null && !OtherGrabbableMustBeGrabbed.BeingHeld) {
                return false;
            }

            return true;
        }

        public virtual void UpdateFixedJoints() {
            // Set to continuous dynamic while being held
            if (rigid != null && rigid.isKinematic) {
                rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }

            // Adjust item velocity. This smooths out forces while becoming rigid
            if (ApplyCorrectiveForce) {
                moveWithVelocity();
            }

            // Set parent to us to keep movement smoothed
            bool recentCollision = Time.time - lastCollisionSeconds <= 0.1f;
            if (ParentToHands && !recentCollision) {
                transform.parent = GetPrimaryGrabber().transform;
            }
            else if (ParentToHands && recentCollision) {
                transform.parent = null;
            }
        }

        public virtual void UpdatePhysicsJoints() {

            // Bail if no joint connected
            if (connectedJoint == null || rigid == null) {
                return;
            }

            // Set to continuous dynamic while being held
            if (rigid.isKinematic) {
                rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }

            // Update Joint poisition in real time
            if (GrabMechanic == GrabType.Snap) {
                connectedJoint.anchor = Vector3.zero;
                connectedJoint.connectedAnchor = GrabPositionOffset;
            }

            // Check if something is requesting a springy joint
            // For example, a gun may wish to make the joint springy in order to apply recoil to a weapon via AddForce
            bool forceSpring = Time.time < requestSpringTime;

            // Only snap to a rigid grip if it's been a short delay after our last collision
            // This prevents the joint from rapidly becoming stiff / springy which will cause jittery behaviour
            bool afterCollision = collisions.Count == 0 && lastNoCollisionSeconds >= 0.1f;

            // Nothing touching it so we can stick to hand rigidly
            // Two-Handed weapons currently react much more smoothly if the joint is rigid, due to how the LookAt system works
            if ((grabbingTwoHands || afterCollision) && !forceSpring) {
                // Lock Angular, XYZ Motion
                // Make joint very rigid
                connectedJoint.rotationDriveMode = RotationDriveMode.Slerp;
                connectedJoint.xMotion = ConfigurableJointMotion.Limited;
                connectedJoint.yMotion = ConfigurableJointMotion.Limited;
                connectedJoint.zMotion = ConfigurableJointMotion.Limited;
                connectedJoint.angularXMotion = ConfigurableJointMotion.Limited;
                connectedJoint.angularYMotion = ConfigurableJointMotion.Limited;
                connectedJoint.angularZMotion = ConfigurableJointMotion.Limited;

                SoftJointLimit sjl = connectedJoint.linearLimit;
                sjl.limit = 15f;

                SoftJointLimitSpring sjlsp = connectedJoint.linearLimitSpring;
                sjlsp.spring = 3000;
                sjlsp.damper = 10f;

                // Set X,Y, and Z drive to our values
                // Set X,Y, and Z drive to our values
                setPositionSpring(CollisionSpring, 10f);

                // Slerp drive used for rotation
                setSlerpDrive(CollisionSlerp, 10f);

                bool recentCollision = Time.time - lastCollisionSeconds <= 0.1f;
                bool playerRecentlyMoved = false;
                if (player != null) {
                    playerRecentlyMoved = player.RecentlyMoved();
                }

                // Adjust item velocity. This smooths out forces while becoming rigid
                if (ApplyCorrectiveForce) {
                    moveWithVelocity();
                }

                // Set parent to us to keep movement smoothed
                if (ParentToHands && playerRecentlyMoved && !recentCollision) {
                    Grabber g = GetPrimaryGrabber();
                    transform.parent = g.transform;
                }
                else if (ParentToHands && !playerRecentlyMoved) {
                    transform.parent = null;
                }
            }
            else {
                // Make Springy
                connectedJoint.rotationDriveMode = RotationDriveMode.Slerp;
                connectedJoint.xMotion = CollisionLinearMotionX;
                connectedJoint.yMotion = CollisionLinearMotionY;
                connectedJoint.zMotion = CollisionLinearMotionZ;
                connectedJoint.angularXMotion = CollisionAngularMotionX;
                connectedJoint.angularYMotion = CollisionAngularMotionY;
                connectedJoint.angularZMotion = CollisionAngularMotionZ;

                SoftJointLimitSpring sp = connectedJoint.linearLimitSpring;
                sp.spring = 5000;
                sp.damper = 5;

                // Set X,Y, and Z drive to our values
                setPositionSpring(CollisionSpring, 5f);

                // Slerp drive used for rotation
                setSlerpDrive(CollisionSlerp, 5f);

                // No parent if we are in contact with something. Player movement should be independent
                if (ParentToHands) {
                    transform.parent = null;
                }
            }

            // This prevents the grabbable from being super far away if we teleported while the Grabbable was also colliding with an object
            if (recentlyTeleported && ParentToHands) {
                Grabber g = GetPrimaryGrabber();
                transform.parent = g.transform;

                // Large Distance, snap to close to grabber position so joint can pull us back into position
                if (Vector3.Distance(transform.position, previousPosition) > 0.2f) {
                    // Using localPosition instead of RigidBody .MovePosition() so we can move through objects
                    transform.localPosition = Vector3.zero;
                }
            }
        }

        void setPositionSpring(float spring, float damper) {

            if(connectedJoint == null) {
                return;
            }

            JointDrive xDrive = connectedJoint.xDrive;
            xDrive.positionSpring = spring;
            xDrive.positionDamper = damper;
            connectedJoint.xDrive = xDrive;

            JointDrive yDrive = connectedJoint.yDrive;
            yDrive.positionSpring = spring;
            yDrive.positionDamper = damper;
            connectedJoint.yDrive = yDrive;

            JointDrive zDrive = connectedJoint.zDrive;
            zDrive.positionSpring = spring;
            zDrive.positionDamper = damper;
            connectedJoint.zDrive = zDrive;
        }

        void setSlerpDrive(float slerp, float damper) {
            if(connectedJoint) {
                JointDrive slerpDrive = connectedJoint.slerpDrive;
                slerpDrive.positionSpring = slerp;
                slerpDrive.positionDamper = damper;
                connectedJoint.slerpDrive = slerpDrive;
            }
        }

        /// <summary>
        /// Apply a velocity on our Grabbable towards out Grabber
        /// </summary>
        void moveWithVelocity() {

            Vector3 positionDelta;
            if (GrabMechanic == GrabType.Snap) {
                positionDelta = (getGrabberWithOffsetWorldPosition(GetPrimaryGrabber()) - transform.position);
            }
            else {
                positionDelta = grabTransform.position - transform.position;
            }

            // Move towards hand using velocity

            if (rigid) {
                rigid.velocity = Vector3.MoveTowards(rigid.velocity, (positionDelta * MoveVelocityForce) * Time.deltaTime, 1f);
            }
        }

        void rotateWithVelocity() {

            Quaternion rotationDelta;
            if (GrabMechanic == GrabType.Snap) {
                rotationDelta = GetPrimaryGrabber().transform.rotation * Quaternion.Inverse(transform.rotation);
            }
            else {
                rotationDelta = grabTransform.rotation * Quaternion.Inverse(transform.rotation);
            }

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);

            // Use closest rotation. If over 180 degrees, rotate the other way
            if (angle > 180) {
                angle -= 360;
            }

            if (angle != 0) {
                Vector3 angularTarget = angle * axis;
                angularTarget = (angularTarget * MoveAngularVelocityForce) * Time.deltaTime;
                rigid.angularVelocity = Vector3.MoveTowards(rigid.angularVelocity, angularTarget, MoveAngularVelocityForce);
            }
        }

        public virtual void UpdateKinematicPhysics() {

            // Distance moved equals elapsed time times speed.
            float distCovered = (Time.time - LastGrabTime) * GrabSpeed;

            // How far along have we traveled
            float fractionOfJourney = distCovered / journeyLength;

            if (GrabMechanic == GrabType.Snap) {
                // Set our position as a fraction of the distance between the markers.
                Grabber g = GetPrimaryGrabber();

                // Update local transform in real time
                if (g != null) {
                    if (ParentToHands) {
                        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero - GrabPositionOffset, fractionOfJourney);
                        transform.localRotation = Quaternion.Lerp(transform.localRotation, grabTransform.localRotation, Time.deltaTime * 10);
                    }
                    // Position the object in world space using physics
                    else {
                        Vector3 grabbablePosition = transform.position;
                        Vector3 grabberPosition = getRemotePosition(g);

                        movePosition(Vector3.Lerp(grabbablePosition, grabberPosition, fractionOfJourney));
                        moveRotation(Quaternion.Lerp(transform.rotation, grabTransform.rotation, Time.deltaTime * 20));
                    }
                }
                else {
                    movePosition(Vector3.Lerp(transform.position, GrabPositionOffset, fractionOfJourney));
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, grabTransform.localRotation, Time.deltaTime * 10);
                }
            }
            else if (GrabMechanic == GrabType.Precise) {
                movePosition(grabTransform.position);
                moveRotation(grabTransform.rotation);
            }
        }

        public virtual void UpdateVelocityPhysics() {

            // Make sure rotation is always free
            if(connectedJoint != null) {
                connectedJoint.xMotion = ConfigurableJointMotion.Free;
                connectedJoint.yMotion = ConfigurableJointMotion.Free;
                connectedJoint.zMotion = ConfigurableJointMotion.Free;
                connectedJoint.angularXMotion = ConfigurableJointMotion.Free;
                connectedJoint.angularYMotion = ConfigurableJointMotion.Free;
                connectedJoint.angularZMotion = ConfigurableJointMotion.Free;
            }

            // Make sure linear spring is off
            // Set X,Y, and Z drive to our values
            setPositionSpring(0, 0.5f);

            // Slerp drive used for rotation
            setSlerpDrive(5, 0.5f);

            moveWithVelocity();
            rotateWithVelocity();

            // Parent to our hands if no colliisions present
            // This makes our object move 1:1 with our controller
            if (ParentToHands) {
                // Parent to hands if no collisions
                bool afterCollision = collisions.Count == 0 && lastNoCollisionSeconds >= 0.2f;
                // Set parent to us to keep movement smoothed
                if (afterCollision) {
                    Grabber g = GetPrimaryGrabber();
                    transform.parent = g.transform;
                }
                else {
                    transform.parent = null;
                }
            }
        }

        void checkParentHands() {

            Grabber g = GetPrimaryGrabber();

            if (ParentHandModel && !didParentHands && g != null) {

                // Precise - Go ahead and parent hands model immediately 
                if (GrabMechanic == GrabType.Precise) {
                    parentHandGraphics(g);
                }
                // Snap - Hand Models if close enough
                else {
                    Vector3 grabbablePosition = transform.position;
                    Vector3 grabberPosition = g.transform.position;

                    // If there is a GrabPoint then we should offset the destination
                    Transform grabPoint = GetClosestGrabPoint(g);
                    if (grabPoint != null) {
                        grabberPosition += transform.position - grabPoint.position;
                    }

                    float distance = Vector3.Distance(grabbablePosition, grabberPosition);

                    // If object can be moved towards the grabber, wait until the item is close before snapping hand to it
                    if (CanBeMoved) {
                        // Close enough to snap hand graphics
                        if (distance < 0.007f) {
                            // Snap position
                            parentHandGraphics(g);
                        }
                    }
                    else {
                        // Can't be moved so go ahead and snap
                        if (grabTransform != null && distance < 0.1f) {

                            Debug.Log("Moved Hand Model" + transform.name);

                            // Snap position
                            parentHandGraphics(g);
                            positionHandGraphics();

                            // positionHandGraphics();
                            if (g.HandsGraphics != null) {
                                g.HandsGraphics.localEulerAngles = Vector3.zero;
                                g.HandsGraphics.localPosition = g.handsGraphicsGrabberOffset;
                            }
                        }
                        else {
                            Debug.Log("Did not move Hand Model " + transform.name);
                        }
                    }
                }
            }
        }

        // Can this object be moved towards an object, or is it fixed in place / attached to something else
        bool canBeMoved() {

            if (GetComponent<Rigidbody>() == null) {
                return false;
            }

            if (GetComponent<Joint>() != null) {
                return false;
            }

            return true;
        }

        void checkSecondaryLook() {

            // Create transform to look at if we are looking at a precise grab
            if (grabbingTwoHands) {
                if (SecondaryLookAtTransform == null) {
                    Grabber thisGrabber = GetPrimaryGrabber();
                    Grabber secondaryGrabber = SecondaryGrabbable.GetPrimaryGrabber();

                    GameObject o = new GameObject();
                    SecondaryLookAtTransform = o.transform;
                    SecondaryLookAtTransform.name = "LookAtTransformTemp";
                    // Precise grab can use current grabber position
                    if (SecondaryGrabbable.GrabMechanic == GrabType.Precise) {
                        SecondaryLookAtTransform.position = secondaryGrabber.transform.position;
                    }
                    // Otherwise use snap point
                    else {
                        Transform grabPoint = SecondaryGrabbable.GetGrabPoint();
                        if (grabPoint) {
                            SecondaryLookAtTransform.position = grabPoint.position;
                        }
                        else {
                            SecondaryLookAtTransform.position = SecondaryGrabbable.transform.position;
                        }

                        SecondaryLookAtTransform.position = SecondaryGrabbable.transform.position;
                    }

                    if (SecondaryLookAtTransform && thisGrabber) {
                        SecondaryLookAtTransform.parent = thisGrabber.transform;
                        SecondaryLookAtTransform.localEulerAngles = Vector3.zero;
                        SecondaryLookAtTransform.localPosition = new Vector3(0, 0, SecondaryLookAtTransform.localPosition.z);

                        // Move parent back to grabber
                        SecondaryLookAtTransform.parent = secondaryGrabber.transform;
                    }
                }
            }

            // We should not be aiming at anything if a Grabbable was specified
            if (SecondaryGrabbable != null && !SecondaryGrabbable.BeingHeld && SecondaryLookAtTransform != null) {
                clearLookAtTransform();
            }

            Grabber heldBy = GetPrimaryGrabber();
            if (heldBy) {
                Transform grabberTransform = heldBy.transform;

                if (SecondaryLookAtTransform != null) {
                    Vector3 initialRotation = grabberTransform.localEulerAngles;

                    Quaternion dest = Quaternion.LookRotation(SecondaryLookAtTransform.position - grabberTransform.position, Vector3.up);
                    grabberTransform.rotation = Quaternion.Slerp(grabberTransform.rotation, dest, Time.deltaTime * SecondHandLookSpeed);

                    // Exclude rotations to only x and y
                    grabberTransform.localEulerAngles = new Vector3(grabberTransform.localEulerAngles.x, grabberTransform.localEulerAngles.y, initialRotation.z);
                }
                else {
                    rotateGrabber(true);
                }
            }
        }

        void rotateGrabber(bool lerp = false) {
            Grabber heldBy = GetPrimaryGrabber();
            if (heldBy != null) {
                Transform grabberTransform = heldBy.transform;

                if (lerp) {
                    grabberTransform.localRotation = Quaternion.Slerp(grabberTransform.localRotation, Quaternion.Inverse(Quaternion.Euler(GrabRotationOffset)), Time.deltaTime * 20);
                }
                else {
                    grabberTransform.localRotation = Quaternion.Inverse(Quaternion.Euler(GrabRotationOffset));
                }
            }
        }

        public Transform GetGrabPoint() {
            return primaryGrabOffset;
        }

        public virtual void GrabItem(Grabber grabbedBy) {

            // Make sure we release this item
            if (BeingHeld && SecondaryGrabBehavior != OtherGrabBehavior.DualGrab) {
                DropItem(false, true);
            }

            // Make sure all values are reset first
            ResetGrabbing();

            // Officially being held
            BeingHeld = true;
            LastGrabTime = Time.time;

            // Set where the item will move to on the grabber
            primaryGrabOffset = GetClosestGrabPoint(grabbedBy);

            // Set the active Grab Point that we will be using
            if (primaryGrabOffset) {
                ActiveGrabPoint = primaryGrabOffset.GetComponent<GrabPoint>();
            }
            else {
                ActiveGrabPoint = null;
            }

            // Update Hand Pose Id
            if (primaryGrabOffset != null && ActiveGrabPoint != null) {
                CustomHandPose = primaryGrabOffset.GetComponent<GrabPoint>().HandPose;
            }
            else {
                CustomHandPose = initialHandPoseId;
            }

            // Update held by properties
            addGrabber(grabbedBy);
            grabTransform.parent = grabbedBy.transform;

            rotateGrabber(false);

            // Hide the hand graphics if necessary
            if (HideHandGraphics) {
                grabbedBy.HideHandGraphics();
            }

            // Use center of grabber if snapping
            if (GrabMechanic == GrabType.Snap) {
                grabTransform.localEulerAngles = Vector3.zero;
                grabTransform.localPosition = GrabPositionOffset;
            }
            // Precision hold can use position of what we're grabbing
            else if (GrabMechanic == GrabType.Precise) {
                grabTransform.position = transform.position;
                grabTransform.rotation = transform.rotation;
            }

            // Move Hand Model
            if (GrabMechanic == GrabType.Precise && SnapHandModel && primaryGrabOffset != null && grabbedBy.HandsGraphics != null) {
                grabbedBy.HandsGraphics.transform.parent = primaryGrabOffset;
                grabbedBy.HandsGraphics.localPosition = grabbedBy.handsGraphicsGrabberOffset;
                grabbedBy.HandsGraphics.localEulerAngles = grabbedBy.handsGraphicsGrabberOffsetRotation;
            }

            // First remove any connected joints if necessary
            var projectile = GetComponent<Projectile>();
            if (projectile) {
                var fj = GetComponent<FixedJoint>();
                if (fj) {
                    Destroy(fj);
                }
            }

            // Setup any relevant joints or required components
            if (GrabPhysics == GrabPhysics.PhysicsJoint) {
                setupConfigJointGrab(grabbedBy, GrabMechanic);
            }
            else if (GrabPhysics == GrabPhysics.Velocity) {
                setupVelocityGrab(grabbedBy, GrabMechanic);
            }
            else if (GrabPhysics == GrabPhysics.FixedJoint) {
                setupFixedJointGrab(grabbedBy, GrabMechanic);
            }
            else if (GrabPhysics == GrabPhysics.Kinematic) {
                setupKinematicGrab(grabbedBy, GrabMechanic);
            }

            // Let events know we were grabbed
            for (int x = 0; x < events.Count; x++) {
                events[x].OnGrab(grabbedBy);
            }

            checkParentHands();

            journeyLength = Vector3.Distance(grabPosition, grabbedBy.transform.position);
        }

        protected virtual void setupConfigJointGrab(Grabber grabbedBy, GrabType grabType) {
            // Set up the new connected joint
            if (GrabMechanic == GrabType.Precise) {
                connectedJoint = grabbedBy.GetComponent<ConfigurableJoint>();
                connectedJoint.connectedBody = rigid;
                // Just let the autoconfigure handle the calculations for us
                connectedJoint.autoConfigureConnectedAnchor = true;
            }

            // Set up the physics joint for snapping
            else if (GrabMechanic == GrabType.Snap) {
                // Need to Fix Rotation on Snap Physics when close by
                //Quaternion originalRotation = transform.rotation;
                transform.rotation = grabTransform.rotation;

                // Setup joint
                setupConfigJoint(grabbedBy);

                rigid.MoveRotation(grabTransform.rotation);
            }
        }

        protected virtual void setupFixedJointGrab(Grabber grabbedBy, GrabType grabType) {
            FixedJoint joint = grabbedBy.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rigid;

            // Setup Fixed Joint in place
            if (GrabMechanic == GrabType.Precise) {
                // Just let the autoconfigure handle the calculations for us
                joint.autoConfigureConnectedAnchor = true;
            }
            // Setup the snap point manually
            else if (GrabMechanic == GrabType.Snap) {
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = Vector3.zero;
                joint.connectedAnchor = GrabPositionOffset;
            }
        }

        protected virtual void setupKinematicGrab(Grabber grabbedBy, GrabType grabType) {
            if (ParentToHands) {
                transform.parent = grabbedBy.transform;
            }

            if (rigid != null) {
                rigid.isKinematic = true;
            }
        }

        protected virtual void setupVelocityGrab(Grabber grabbedBy, GrabType grabType) {
            // Setup joint to be used when moving with velocity
            if (GrabMechanic == GrabType.Precise) {
                connectedJoint = grabbedBy.GetComponent<ConfigurableJoint>();
                connectedJoint.connectedBody = rigid;
                // Just let the autoconfigure handle the calculations for us
                connectedJoint.autoConfigureConnectedAnchor = true;
            }
            // Set up the connected joint for snapping
            else if (GrabMechanic == GrabType.Snap) {
                transform.rotation = grabTransform.rotation;
                // Setup joint
                setupConfigJoint(grabbedBy);
                rigid.MoveRotation(grabTransform.rotation);
            }
        }

        public virtual void GrabRemoteItem(Grabber grabbedBy) {
            flyingTo = grabbedBy;
            grabTransform.parent = grabbedBy.transform;
            grabTransform.localEulerAngles = Vector3.zero;
            grabTransform.localPosition = -GrabPositionOffset;

            grabTransform.localEulerAngles = GrabRotationOffset;

            if (rigid) {
                rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rigid.isKinematic = true;
            }

            remoteGrabbing = true;
        }

        public void ResetGrabbing() {
            if (rigid) {
                rigid.isKinematic = wasKinematic;
            }

            flyingTo = null;

            remoteGrabbing = false;

            collisions = new List<Collider>();
        }

        public virtual void DropItem(Grabber droppedBy, bool resetVelocity, bool resetParent) {

            // Nothing holding us
            if (heldByGrabbers == null) {
                BeingHeld = false;
                return;
            }

            if (resetParent) {
                ResetParent();
            }

            //disconnect all joints and set the connected object to null
            removeConfigJoint();

            // Remove Fixed Joint
            if (GrabPhysics == GrabPhysics.FixedJoint && droppedBy != null) {
                FixedJoint joint = droppedBy.gameObject.GetComponent<FixedJoint>();
                if (joint) {
                    GameObject.Destroy(joint);
                }
            }

            //  If something called drop on this item we want to make sure the parent knows about it
            // Reset's Grabber position, grabbable state, etc.
            if (droppedBy) {
                droppedBy.DidDrop();
            }

            // Release item and apply physics force to it
            if (rigid != null) {
                rigid.isKinematic = wasKinematic;
                rigid.useGravity = usedGravity;
                rigid.interpolation = initialInterpolationMode;
                rigid.collisionDetectionMode = initialCollisionMode;
            }

            // Override Kinematic status if specified
            if (ForceDisableKinematicOnDrop) {
                rigid.isKinematic = false;
                // Free of constraints if they were set
                if (rigid.constraints == RigidbodyConstraints.FreezeAll) {
                    rigid.constraints = RigidbodyConstraints.None;
                }
            }

            // On release event
            if (events != null) {
                for (int x = 0; x < events.Count; x++) {
                    events[x].OnRelease();
                }
            }

            // No longer have a primary Grab Offset set
            primaryGrabOffset = null;

            // No longer looking at a 2h object
            clearLookAtTransform();

            removeGrabber(droppedBy);

            BeingHeld = heldByGrabbers != null && heldByGrabbers.Count > 0;
            didParentHands = false;
            LastDropTime = Time.time;
            CustomHandPose = initialHandPoseId;

            // Apply velocity last
            if (rigid && resetVelocity && droppedBy && AddControllerVelocityOnDrop) {
                // Make sure velocity is passed on
                Vector3 velocity = droppedBy.GetGrabberAveragedVelocity() + droppedBy.GetComponent<Rigidbody>().velocity;
                Vector3 angularVelocity = droppedBy.GetGrabberAveragedAngularVelocity() + droppedBy.GetComponent<Rigidbody>().angularVelocity;

                if (gameObject.activeSelf) {
                    Release(velocity, angularVelocity);
                }
            }
        }

        void clearLookAtTransform() {
            if (SecondaryLookAtTransform != null && SecondaryLookAtTransform.transform.name == "LookAtTransformTemp") {
                GameObject.Destroy(SecondaryLookAtTransform.gameObject);
            }

            SecondaryLookAtTransform = null;
        }

        void callEvents(Grabber g) {
            if (events.Any()) {
                ControllerHand hand = g.HandSide;

                // Right Hand Controls
                if (hand == ControllerHand.Right) {
                    foreach (var e in events) {
                        e.OnGrip(input.RightGrip);
                        e.OnTrigger(input.RightTrigger);

                        if (input.RightTriggerUp) {
                            e.OnTriggerUp();
                        }
                        if (input.RightTriggerDown) {
                            e.OnTriggerDown();
                        }
                        if (input.AButton) {
                            e.OnButton1();
                        }
                        if (input.AButtonDown) {
                            e.OnButton1Down();
                        }
                        if (input.AButtonUp) {
                            e.OnButton1Up();
                        }
                        if (input.BButton) {
                            e.OnButton2();
                        }
                        if (input.BButtonDown) {
                            e.OnButton2Down();
                        }
                        if (input.BButtonUp) {
                            e.OnButton2Up();
                        }
                    }
                }

                // Left Hand Controls
                if (hand == ControllerHand.Left) {
                    for (int x = 0; x < events.Count; x++) {
                        GrabbableEvents e = events[x];
                        e.OnGrip(input.LeftGrip);
                        e.OnTrigger(input.LeftTrigger);

                        if (input.LeftTriggerUp) {
                            e.OnTriggerUp();
                        }
                        if (input.LeftTriggerDown) {
                            e.OnTriggerDown();
                        }
                        if (input.XButton) {
                            e.OnButton1();
                        }
                        if (input.XButtonDown) {
                            e.OnButton1Down();
                        }
                        if (input.XButtonUp) {
                            e.OnButton1Up();
                        }
                        if (input.YButton) {
                            e.OnButton2();
                        }
                        if (input.YButtonDown) {
                            e.OnButton2Down();
                        }
                        if (input.YButtonUp) {
                            e.OnButton2Up();
                        }
                    }
                }
            }
        }       

        public virtual void DropItem(Grabber droppedBy) {
            DropItem(droppedBy, true, true);
        }

        public virtual void DropItem(bool resetVelocity, bool resetParent) {
            DropItem(GetPrimaryGrabber(), resetVelocity, resetParent);
        }

        public void ResetScale() {
            transform.localScale = new Vector3(OriginalScale, OriginalScale, OriginalScale);
        }

        public void ResetParent() {
            transform.parent = originalParent;
        }

        public void UpdateOriginalParent(Transform newOriginalParent) {
            originalParent = newOriginalParent;
        }

        public void UpdateOriginalParent() {
            UpdateOriginalParent(transform.parent);
        }

        public ControllerHand GetControllerHand(Grabber g) {
            if(g != null) {
                return g.HandSide;
            }

            return ControllerHand.None;
        }
        
        /// <summary>
        /// Returns the Grabber that first grabbed this item. Return null if not being held.
        /// </summary>
        /// <returns></returns>
        public virtual Grabber GetPrimaryGrabber() {
            if(heldByGrabbers != null) {
                for (int x = 0; x < heldByGrabbers.Count; x++) {
                    Grabber g = heldByGrabbers[x];
                    if (g != null && g.HeldGrabbable == this) {
                        return g;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the closest valid grabber. 
        /// </summary>
        /// <returns>Returns null if no valid Grabbers in range</returns>
        public virtual Grabber GetClosestGrabber() {

            Grabber closestGrabber = null;
            float lastDistance = 9999;

            if (validGrabbers != null) {

                for (int x = 0; x < validGrabbers.Count; x++) {
                    Grabber g = validGrabbers[x];
                    if (g != null) {
                        float dist = Vector3.Distance(grabPosition, g.transform.position);
                        if(dist < lastDistance) {
                            closestGrabber = g;
                        }
                    }
                }
            }

            return closestGrabber;
        }

        public virtual Transform GetClosestGrabPoint(Grabber grabber) {
            Transform grabPoint = null;
            float lastDistance = 9999;
            float lastAngle = 360;
            if(GrabPoints != null) {
                int grabCount = GrabPoints.Count;
                for (int x = 0; x < grabCount; x++) {
                    Transform g = GrabPoints[x];

                    // Transform may have been destroyed
                    if (g == null) {
                        continue;
                    }

                    float thisDist = Vector3.Distance(g.transform.position, grabber.transform.position);
                    if (thisDist <= lastDistance) {

                        // Check for GrabPoint component that may override some values
                        GrabPoint gp = g.GetComponent<GrabPoint>();
                        if (gp) {

                            // Not valid for this hand side
                            if((grabber.HandSide == ControllerHand.Left && !gp.LeftHandIsValid) || (grabber.HandSide == ControllerHand.Right && !gp.RightHandIsValid)) {
                                continue;
                            }

                            // Angle is too great
                            float currentAngle = Quaternion.Angle(grabber.transform.rotation, g.transform.rotation);
                            if (currentAngle > gp.MaxDegreeDifferenceAllowed) {
                                continue;
                            }

                            // Last angle was better, don't use this one
                            if (currentAngle > lastAngle && gp.MaxDegreeDifferenceAllowed != 360) {
                                continue;
                            }

                            lastAngle = currentAngle;
                        }

                        grabPoint = g;
                        lastDistance = thisDist;
                    }
                }
            }

            return grabPoint;
        }

        /// <summary>
        /// Throw the object by applying velocity
        /// </summary>
        /// <param name="velocity">How much velocity to apply to the grabbable. Multiplied by ThrowForceMultiplier</param>
        /// <param name="angularVelocity">How much angular velocity to apply to the grabbable.</param>
        public virtual void Release(Vector3 velocity, Vector3 angularVelocity) {
            Vector3 releaseVelocity = velocity * ThrowForceMultiplier;

            // Make sure this is a valid velocity
            if (float.IsInfinity(releaseVelocity.x) || float.IsNaN(releaseVelocity.x)) {
                return;
            }

            // It isn't strictly necessary to update the velocity twice, but I've found this gives slightly more responsive feeling throws
            rigid.velocity = releaseVelocity;
            rigid.angularVelocity = angularVelocity;
        }

        public virtual bool IsValidCollision(Collision collision) {
            return IsValidCollision(collision.collider);
        }

        public virtual bool IsValidCollision(Collider col) {

            // Ignore Projectiles from grabbable collision
            // This way our grabbable stays rigid when projectils come in contact
            string transformName = col.transform.name;
            if (transformName.Contains("Projectile") || transformName.Contains("Bullet") || transformName.Contains("Clip")) {
                return false;
            }

            // Ignore Character Joints as these cause jittery issues
            if (transformName.Contains("Joint")) {
                return false;
            }

            // Ignore Character Controllers
            CharacterController cc = col.gameObject.GetComponent<CharacterController>();
            if (cc && col) {
                Physics.IgnoreCollision(col, cc, true);
                return false;
            }

            return true;
        }

        public virtual void parentHandGraphics(Grabber g) {
            if (g.HandsGraphics != null) {
                // Set to specified Grab Transform
                if(primaryGrabOffset != null) {
                    g.HandsGraphics.transform.parent = primaryGrabOffset;
                }
                else {
                    g.HandsGraphics.transform.parent = transform;
                }
                
                didParentHands = true;
            }
        }

        void setupConfigJoint(Grabber g) {
            connectedJoint = g.GetComponent<ConfigurableJoint>();
            connectedJoint.autoConfigureConnectedAnchor = false;
            connectedJoint.connectedBody = rigid;

            float anchorOffsetVal = (1 / g.transform.localScale.x) * -1;
            connectedJoint.anchor = Vector3.zero;

            connectedJoint.connectedAnchor = GrabPositionOffset;
        }

        void removeConfigJoint() {
            if (connectedJoint != null) {
                connectedJoint.anchor = Vector3.zero;
                connectedJoint.connectedBody = null;
            }
        }

        void addGrabber(Grabber g) {
            if (heldByGrabbers == null) {
                heldByGrabbers = new List<Grabber>();
            }

            if (!heldByGrabbers.Contains(g)) {
                heldByGrabbers.Add(g);
            }
        }

        void removeGrabber(Grabber g) {
            if (heldByGrabbers == null) {
                heldByGrabbers = new List<Grabber>();
            }
            else if (heldByGrabbers.Contains(g)) {
                heldByGrabbers.Remove(g);
            }

            Grabber removeGrabber = null;
            // Clean up any other latent grabbers
            for (int x = 0; x < heldByGrabbers.Count; x++) {
                Grabber grab = heldByGrabbers[x];
                if (grab.HeldGrabbable == null || grab.HeldGrabbable != this) {
                    removeGrabber = grab;
                }
            }

            if (removeGrabber) {
                heldByGrabbers.Remove(removeGrabber);
            }
        }

        /// <summary>
        /// Moves the Grabbable using MovePosition if rigidbody present. Otherwise use transform.position
        /// </summary>
        void movePosition(Vector3 worldPosition) {
            if (rigid) {
                rigid.MovePosition(worldPosition);
            }
            else {
                transform.position = worldPosition;
            }
        }

        /// <summary>
        /// Rotates the Grabbable using MoveRotation if rigidbody present. Otherwise use transform.rotation
        /// </summary>
        void moveRotation(Quaternion worldRotation) {
            if (rigid) {
                rigid.MoveRotation(worldRotation);
            }
            else {
                transform.rotation = worldRotation;
            }
        }

        protected Vector3 getRemotePosition(Grabber toGrabber) {

            if (toGrabber != null) {
                Transform pointPosition = GetClosestGrabPoint(toGrabber);

                if(pointPosition) {
                    Vector3 grabberPosition = toGrabber.transform.position;

                    // If there is a GrabPoint then we should offset the destination
                    Transform grabPoint = GetClosestGrabPoint(toGrabber);
                    if (grabPoint != null) {
                        grabberPosition += transform.position - grabPoint.position;
                    }

                    return grabberPosition;
                }

                return grabTransform.position;
            }

            return grabTransform.position;
        }

        protected Quaternion getRemoteRotation(Grabber grabber) {

            if (grabber != null) {
                Transform point = GetClosestGrabPoint(grabber);
                if (point) {
                    Quaternion originalRot = grabTransform.rotation;
                    grabTransform.localRotation *= Quaternion.Inverse(point.localRotation);
                    Quaternion result = grabTransform.rotation;

                    grabTransform.rotation = originalRot;

                    return result;
                }
            }

            return grabTransform.rotation;
        }

        void filterCollisions() {
            for (int x = 0; x < collisions.Count; x++) {
                if (collisions[x] == null || !collisions[x].enabled || !collisions[x].gameObject.activeSelf) {
                    collisions.Remove(collisions[x]);
                    break;
                }
            }
        }

        /// <summary>
        /// A BNGPlayerController is optional, but if one is available we can check the last moved time in order to strengthen the physics joint during quick movements. This helps prevent jitter or flying objects in certain situations.
        /// </summary>
        /// <returns></returns>
        public virtual BNGPlayerController GetBNGPlayerController() {

            if (_player != null) {
                return _player;
            }

            // The player object can be used to determine if the object is about to move rapidly
            if (GameObject.FindGameObjectWithTag("Player")) {
                return _player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<BNGPlayerController>();
            }
            else {
                return _player = FindObjectOfType<BNGPlayerController>();
            }
        }

        /// <summary>
        /// Request the Grabbable to use a springy joint for the next X seconds
        /// </summary>
        /// <param name="seconds">How many seconds to make the Grabbable springy.</param>
        public virtual void RequestSpringTime(float seconds) {
            float requested = Time.time + seconds;

            // Only apply if our request is longer than the current request
            if(requested > requestSpringTime) {
                requestSpringTime = requested;
            }
        }

        public virtual void AddValidGrabber(Grabber grabber) {

            if (validGrabbers == null) {
                validGrabbers = new List<Grabber>();
            }

            if (!validGrabbers.Contains(grabber)) {
                validGrabbers.Add(grabber);
            }
        }

        public virtual void RemoveValidGrabber(Grabber grabber) {
            if (validGrabbers != null && validGrabbers.Contains(grabber)) {
                validGrabbers.Remove(grabber);
            }
        }

        /// <summary>
        /// You can comment this function out if you don't need precise contacts. Otherwise this is necessary to check for world collisions while being held
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionStay(Collision collision) {

            // Can bail early
            if (!BeingHeld) {
                return;
            }

            for (int x = 0; x < collision.contacts.Length; x++) {
                ContactPoint contact = collision.contacts[x];
                // Keep track of how many objects we are colliding with
                if (BeingHeld && IsValidCollision(contact.otherCollider) && !collisions.Contains(contact.otherCollider)) {
                    collisions.Add(contact.otherCollider);
                }
            }
        }

        private void OnCollisionEnter(Collision collision) {
            // Keep track of how many objects we are colliding with
            if (BeingHeld && IsValidCollision(collision) && !collisions.Contains(collision.collider)) {
                collisions.Add(collision.collider);
            }
        }

        private void OnCollisionExit(Collision collision) {
            // We only care about collisions when being held, so we can skip this check otherwise
            if (BeingHeld && collisions.Contains(collision.collider)) {
                collisions.Remove(collision.collider);
            }
        }

        bool quitting = false;
        void OnApplicationQuit() {
            quitting = true;
        }

        void OnDestroy() {
            if(BeingHeld && !quitting) {
                DropItem(false, false);
            }
        }

        void OnDrawGizmosSelected() {
            // Show Grip Points
            Gizmos.color = new Color(0, 1, 0, 0.5f);

            if (GrabPoints != null && GrabPoints.Count > 0) {
                for (int i = 0; i < GrabPoints.Count; i++) {
                    Transform p = GrabPoints[i];
                    if (p != null) {
                        Gizmos.DrawSphere(p.position, 0.02f);
                    }
                }
            }
            else {
                Gizmos.DrawSphere(transform.position, 0.02f);
            }
        }     
    }
}