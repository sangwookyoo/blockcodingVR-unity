#define VRIF
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_2018_4_OR_NEWER
using UnityEngine.XR;
#endif
#if STEAM_VR_SDK
using Valve.VR;
#endif

namespace BNG {

    public enum ControllerHand {
        Left,
        Right,
        None
    }

    /// <summary>
    /// Controller Options available to bind buttons to via Inspector. You can use GetControllerBindingValue() to determine if that button has been pressed.
    /// </summary>
    public enum ControllerBinding {
        None,
        AButton,
        AButtonDown,
        BButton,
        BButtonDown,
        XButton,
        XButtonDown,
        YButton,
        YButtonDown,
        LeftTrigger,
        LeftTriggerDown,
        LeftGrip,
        LeftGripDown,
        LeftThumbstick,
        LeftThumbstickDown,
        RightTrigger,
        RightTriggerDown,
        RightGrip,
        RightGripDown,
        RightThumbstick,
        RightThumbstickDown,
        StartButton,
        StartButtonDown,
        BackButton,
        BackButtonDown
    }

    

    /// <summary>
    /// Controller Options available to bind buttons to via Inspector. Input is relative to the controller holding it.
    /// Ex : Button 1 = Button A if held in Right controller, Button X if held in Left.
    /// </summary>
    public enum GrabbedControllerBinding {
        None,
        Button1, // Button A, X
        Button1Down,
        Button2, // Button B, Y
        Button2Down,
        Trigger,
        TriggerDown,
        Grip,
        GripDown
    }

    public enum InputAxis {
        None,
        LeftThumbStickAxis,
        LeftTouchPadAxis,
        RightThumbStickAxis,
        RightTouchPadAxis
    }

    public enum ControllerType {
        None,
        Unknown,
        OculusTouch,
        Wand,
        Knuckles
    }

    public enum HandControl {
        LeftGrip,
        RightGrip,
        LeftTrigger,
        RightTrigger,
        None
    }

    public enum GrabButton {
        Grip,
        Trigger,
        Inherit
    }

    public enum HoldType {
        HoldDown, // Hold down the grab button
        Toggle,   // Click the grab button down to switch between hold and release
        Inherit   // Inherit from Grabber
    }    

    public enum XRInputSource {
        XRInput,
        OVRInput,
        SteamVR,
        Pico
    }

    public enum SDKProvider {
        Unknown,
        OculusSDK,
        OpenVR
    }

    /// <summary>
    /// A proxy for handling input from various input providers such as OVRInput, XRInput, and SteamVR. 
    /// </summary>
    public class InputBridge : MonoBehaviour {

        /// <summary>
        /// Instance of our Singleton
        /// </summary>
        public static InputBridge Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<InputBridge>();
                    if (_instance == null) {
                        _instance = new GameObject("InputBridge").AddComponent<InputBridge>();
                    }
                }
                return _instance;
            }
        }
        private static InputBridge _instance;

        [SerializeField]        
        public XRInputSource InputSource = XRInputSource.XRInput;
       
        [SerializeField]
        public TrackingOriginModeFlags TrackingOrigin = TrackingOriginModeFlags.Floor;

        [Header("Grip")]
        /// <summary>
        /// How far Left Grip is Held down. Values : 0 - 1 (Fully Open / Closed)
        /// </summary>
        public float LeftGrip = 0;

        /// <summary>
        /// Left Grip was pressed down this frame, but not last
        /// </summary>
        public bool LeftGripDown = false;

        /// <summary>
        /// How far Right Grip is Held down. Values : 0 - 1 (Fully Open / Closed)
        /// </summary>
        public float RightGrip = 0;

        /// <summary>
        /// Right Grip was pressed down this frame, but not last
        /// </summary>
        public bool RightGripDown = false;

        [Header("Trigger")]
        /// <summary>
        /// How far Left Trigger is Held down. Values : 0 - 1 (Fully Open / Closed)
        /// </summary>
        public float LeftTrigger = 0;
        public bool LeftTriggerNear = false;
        public bool LeftTriggerUp = false;

        /// <summary>
        /// Returns true if Left Trigger was held down this frame but not the last
        /// </summary>
        public bool LeftTriggerDown = false;

        public float RightTrigger = 0;
        public bool RightTriggerUp = false;

        /// <summary>
        /// Returns true if Right Trigger was held down this frame but not the last
        /// </summary>
        public bool RightTriggerDown = false;
        public bool RightTriggerNear = false;

        public bool LeftThumbNear = false;
        public bool RightThumbNear = false;

        [Header("Thumbstick")]
        /// <summary>
        /// Pressed down this frame, but not last
        /// </summary>
        public bool LeftThumbstickDown = false;

        /// <summary>
        /// Released this frame but not last
        /// </summary>
        public bool LeftThumbstickUp = false;

        /// <summary>
        /// Pressed down this frame, but not last
        /// </summary>
        public bool RightThumbstickDown = false;

        /// <summary>
        /// Released this frame but not last
        /// </summary>
        public bool RightThumbstickUp = false;

        /// <summary>
        /// Currently Held Down
        /// </summary>
        public bool LeftThumbstick = false;
        public bool RightThumbstick = false;

        [Header("Buttons")]
        /// <summary>
        /// Is the A button currently being held down
        /// </summary>
        public bool AButton = false;

        /// <summary>
        /// Returns true if the A Button was pressed down this frame but not last
        /// </summary>
        [Tooltip("Returns true if the A Button was pressed down this frame but not last")]
        public bool AButtonDown = false;

        // A Button Up this frame but down the last
        public bool AButtonUp = false;

        /// <summary>
        /// Is the B button currently being held down
        /// </summary>
        public bool BButton = false;

        /// <summary>
        /// Returns true if the B Button was pressed down this frame but not last
        /// </summary>
        [Tooltip("Returns true if the B Button was pressed down this frame but not last")]
        public bool BButtonDown = false;

        // B Button Up this frame but down the last
        public bool BButtonUp = false;

        public bool XButton = false;

        /// <summary>
        /// Returns true if the X Button was pressed down this frame but not last
        /// </summary>
        [Tooltip("Returns true if the X Button was pressed down this frame but not last")]
        public bool XButtonDown = false;

        // X Button Up this frame but down the last
        public bool XButtonUp = false;

        public bool YButton = false;
        /// <summary>
        /// Returns true if the Y Button was pressed down this frame but not last
        /// </summary>
        public bool YButtonDown = false;
        public bool YButtonUp = false;

        public bool StartButton = false;
        public bool StartButtonDown = false;
        public bool BackButton = false;
        public bool BackButtonDown = false;

        [Header("Axis")]
        public Vector2 LeftThumbstickAxis;
        public Vector2 RightThumbstickAxis;

        public Vector2 LeftTouchPadAxis;
        public Vector2 RightTouchPadAxis;

        /// <summary>
        /// Thumbstick X must be greater than this amount to be considered valid
        /// </summary>
        [Tooltip("Thumbstick X must be greater than this amount to be considered valid")]
        public float ThumbstickDeadzoneX = 0.001f;

        /// <summary>
        /// Thumbstick Y must be greater than this amount to be considered valid
        /// </summary>
        [Tooltip("Thumbstick Y must be greater than this amount to be considered valid")]
        public float ThumbstickDeadzoneY = 0.001f;

#if UNITY_2019_3_OR_NEWER
        static List<InputDevice> devices = new List<InputDevice>();
#endif

        // What threshold constitutes a "down" event.
        // For example, pushing the trigger down 20% (0.2) of the way considered starting a trigger down event
        // This is used in XRInput
        private float _downThreshold = 0.2f;
        bool SteamVRSupport = false;

        [Header("HMD / Hardware")]
        public ControllerType ConnectedControllerType;

        [Tooltip("Is there an HMD present and in use.")]
        public bool HMDActive = false;

        public SDKProvider LoadedSDK { get; private set; }

        public bool IsOculusDevice { get; private set; }

        public bool IsOculusQuest { get; private set; }

        public bool IsHTCDevice { get; private set; }

        public bool IsValveIndexController { get; private set; }

        /// <summary>
        /// Returns true if the controller has both a Touchpad and a Joystick. Currently only the Valve Index has both.
        /// </summary>
        [Tooltip("Returns true if the controller has both a Touchpad and a Joystick. Currently on the Valve Index has both.")]
        public bool SupportsBothTouchPadAndJoystick;

        /// <summary>
        /// Returns true if the controllers support the 'indexTouch' (or 'near trigger') XR input mapping. Currently only Oculus devices on the Oculus SDK support index touch. OpenVR is not supported.
        /// </summary>
        [Tooltip("Returns true if the controllers support the 'indexTouch' XR input mapping. Currently only Oculus devices on the Oculus SDK support thumb touch. OpenVR is not supported.")]
        public bool SupportsIndexTouch;

        /// <summary>
        /// Returns true if the controllers support the 'ThumbTouch' (or near thumbstick) XR input mapping. Currently only Oculus devices on the Oculus SDK support index touch. OpenVR is not supported.
        /// </summary>
        [Tooltip("Returns true if the controllers support the 'ThumbTouch' (or near thumbstick) XR input mapping. Currently only Oculus devices on the Oculus SDK support thumb touch. OpenVR is not supported.")]
        public bool SupportsThumbTouch;

        // Events
        /// <summary>
        /// Called after update loop.
        /// </summary>
        public delegate void InputAction();
        public static event InputAction OnInputsUpdated;

        /// <summary>
        /// Called once a controller has been successfully detected
        /// </summary>
        public delegate void ControllerFoundAction();
        public static event InputAction OnControllerFound;

        private void Awake() {
            // Destroy any duplicate instances that may have been created
            if (_instance != null && _instance != this) {
                Destroy(this);
                return;
            }

            _instance = this;

#if UNITY_2019_3_OR_NEWER
            InputDevices.deviceConfigChanged += onDeviceChanged;
            InputDevices.deviceConnected += onDeviceChanged;
            InputDevices.deviceDisconnected += onDeviceChanged;
#endif

            // Update all device properties
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            setDeviceProperties();
        }

        void Start() {

            SetTrackingOriginMode(TrackingOrigin);

#if STEAM_VR_SDK
            SteamVRSupport = true;

            if(InputSource != XRInputSource.SteamVR) {
                Debug.Log("SteamVR SDK detected. Switching Input Source from " + InputSource.ToString() + " to SteamVR.");
                InputSource = XRInputSource.SteamVR;
            }

            // Set the default action set if not provided
            SteamVR_ActivateActionSetOnLoad VRIFLoader = FindObjectOfType<SteamVR_ActivateActionSetOnLoad>();
            if (VRIFLoader == null) {
                Debug.Log("SteamVR_ActivateActionSetOnLoad component not found - adding VRIF custom actions default.");
                VRIFLoader = gameObject.AddComponent<SteamVR_ActivateActionSetOnLoad>();
                VRIFLoader.actionSet = SteamVR_Actions.VRIF;
            }

            SteamVR.Initialize();
#endif
        }

        void Update() {
            UpdateDeviceActive();
            UpdateInputs();
        }

        public virtual void UpdateInputs() {
            // SteamVR uses an action system
            if (InputSource == XRInputSource.SteamVR && SteamVRSupport) {
                UpdateSteamInput();
            }
            // Use OVRInput to get more Oculus Specific inputs, such as "Near Touch"
            else if (InputSource == XRInputSource.OVRInput) {
#if OCULUS_INTEGRATION
                LeftThumbstickAxis = ApplyDeadZones(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick), ThumbstickDeadzoneX, ThumbstickDeadzoneY);
                RightThumbstickAxis = ApplyDeadZones(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick), ThumbstickDeadzoneX, ThumbstickDeadzoneY);

                LeftGrip = correctValue(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch));
                LeftGripDown = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);

                RightGrip = correctValue(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch));
                RightGripDown = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);

                LeftTrigger = correctValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch));
                LeftTriggerUp = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
                LeftTriggerDown = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);

                RightTrigger = correctValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch));
                RightTriggerUp = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
                RightTriggerDown = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

                LeftTriggerNear = OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
                LeftThumbNear = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, OVRInput.Controller.LTouch);

                RightTriggerNear = OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
                RightThumbNear = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, OVRInput.Controller.RTouch);

                AButton = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);
                AButtonDown = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
                AButtonUp = OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch);

                BButton = OVRInput.Get(OVRInput.Button.Two);
                BButtonDown = OVRInput.GetDown(OVRInput.Button.Two);
                BButtonUp = OVRInput.GetUp(OVRInput.Button.Two);

                XButton = OVRInput.Get(OVRInput.Button.Three);
                XButtonDown = OVRInput.GetDown(OVRInput.Button.Three);
                XButtonUp = OVRInput.GetUp(OVRInput.Button.Three);

                YButton = OVRInput.Get(OVRInput.Button.Four);
                YButtonDown = OVRInput.GetDown(OVRInput.Button.Four);
                YButtonUp = OVRInput.GetUp(OVRInput.Button.Four);

                StartButton = OVRInput.Get(OVRInput.Button.Start);
                StartButtonDown = OVRInput.GetDown(OVRInput.Button.Start);

                BackButton = OVRInput.Get(OVRInput.Button.Back);
                BackButtonDown = OVRInput.GetDown(OVRInput.Button.Back);

                LeftThumbstickDown = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);
                LeftThumbstickUp = OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);

                RightThumbstickDown = OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);
                RightThumbstickUp = OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);

                LeftThumbstick = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);
                RightThumbstick = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);
#endif
            }
            // Use XRInput
            else if(InputSource == XRInputSource.XRInput) {
#if UNITY_2019_3_OR_NEWER
                // Refresh XR devices
                InputDevices.GetDevices(devices);

                // Left XR Controller
                var primaryLeftController = GetLeftController();

                // Right XR Controller
                var primaryRightController = GetRightController();

                // For most cases thumbstick is on the primary2DAxis
                // However, if the Controller has both a touchpad and a controller on it (i.e. Valve Index Knuckles) then the thumbstick axis is actually on the secondary axis, not the primary axis
                InputFeatureUsage<Vector2> thumbstickAxis = SupportsBothTouchPadAndJoystick ? CommonUsages.secondary2DAxis : CommonUsages.primary2DAxis;
                InputFeatureUsage<Vector2> thumbstickAxisSecondary = SupportsBothTouchPadAndJoystick ? CommonUsages.primary2DAxis : CommonUsages.secondary2DAxis;
                InputFeatureUsage<bool> thumbstickAxisClick = SupportsBothTouchPadAndJoystick ? CommonUsages.secondary2DAxisClick : CommonUsages.primary2DAxisClick;

                var prevBool = LeftThumbstick;
                LeftThumbstick = getFeatureUsage(primaryLeftController, thumbstickAxisClick);
                LeftThumbstickDown = prevBool == false && LeftThumbstick == true;
                LeftThumbstickUp = prevBool == true && LeftThumbstick == false;

                prevBool = RightThumbstick;
                RightThumbstick = getFeatureUsage(primaryRightController, thumbstickAxisClick);
                RightThumbstickDown = prevBool == false && RightThumbstick == true;
                RightThumbstickUp = prevBool == true && RightThumbstick == false;

                LeftTouchPadAxis = ApplyDeadZones(getFeatureUsage(primaryLeftController, thumbstickAxisSecondary), ThumbstickDeadzoneX, ThumbstickDeadzoneY);
                LeftThumbstickAxis = ApplyDeadZones(getFeatureUsage(primaryLeftController, thumbstickAxis), ThumbstickDeadzoneX, ThumbstickDeadzoneY);

                RightTouchPadAxis = ApplyDeadZones(getFeatureUsage(primaryRightController, thumbstickAxisSecondary), ThumbstickDeadzoneX, ThumbstickDeadzoneY);
                RightThumbstickAxis = ApplyDeadZones(getFeatureUsage(primaryRightController, thumbstickAxis), ThumbstickDeadzoneX, ThumbstickDeadzoneY);

                // Store copy of previous value so we can determine if we need to call OnDownEvent
                var prevVal = LeftGrip;
                LeftGrip = correctValue(getFeatureUsage(primaryLeftController, CommonUsages.grip));
                LeftGripDown = prevVal < _downThreshold && LeftGrip >= _downThreshold;

                prevVal = RightGrip;
                RightGrip = correctValue(getFeatureUsage(primaryRightController, CommonUsages.grip));
                RightGripDown = prevVal < _downThreshold && RightGrip >= _downThreshold;

                prevVal = LeftTrigger;
                LeftTrigger = correctValue(getFeatureUsage(primaryLeftController, CommonUsages.trigger));
                LeftTriggerUp = LeftTrigger == 0;
                LeftTriggerDown = prevVal < _downThreshold && LeftTrigger >= _downThreshold;

                prevVal = RightTrigger;
                RightTrigger = correctValue(getFeatureUsage(primaryRightController, CommonUsages.trigger));
                RightTriggerUp = RightTrigger == 0;
                RightTriggerDown = prevVal < _downThreshold && RightTrigger >= _downThreshold;

                // While OculusUsages.indexTouch is recommended, only CommonUsages.indexTouch is currently providing proper values
#pragma warning disable 0618

                LeftTriggerNear = getFeatureUsage(primaryLeftController, CommonUsages.indexTouch) > 0;
                LeftThumbNear = getFeatureUsage(primaryLeftController, CommonUsages.thumbTouch) > 0;

#if USING_XR_SDK
                LeftThumbNear = getFeatureUsage(primaryLeftController, OculusUsages.indexTouch) > 0;
#endif

#if USING_COMPATIBLE_OCULUS_XR_PLUGIN_VERSION
                LeftThumbNear = getFeatureUsage(primaryLeftController, OculusUsages.indexTouch) > 0;
#endif
                RightTriggerNear = getFeatureUsage(primaryRightController, CommonUsages.indexTouch) > 0;
                RightThumbNear = getFeatureUsage(primaryRightController, CommonUsages.thumbTouch) > 0;

#pragma warning restore 0618
                prevBool = AButton;
                AButton = getFeatureUsage(primaryRightController, CommonUsages.primaryButton);
                AButtonDown = prevBool == false && AButton == true;
                AButtonUp = prevBool == true && AButton == false;

                prevBool = BButton;
                BButton = getFeatureUsage(primaryRightController, CommonUsages.secondaryButton);
                BButtonDown = prevBool == false && BButton == true;
                BButtonUp = prevBool == true && BButton == false;

                prevBool = XButton;
                XButton = getFeatureUsage(primaryLeftController, CommonUsages.primaryButton);
                XButtonDown = prevBool == false && XButton == true;
                XButtonUp = prevBool == true && XButton == false;

                prevBool = YButton;
                YButton = getFeatureUsage(primaryLeftController, CommonUsages.secondaryButton);
                YButtonDown = prevBool == false && YButton == true;
                YButtonUp = prevBool == true && YButton == false;

                prevBool = StartButton;
                StartButton = getFeatureUsage(primaryRightController, CommonUsages.menuButton);
                StartButtonDown = prevBool == false && StartButton == true;

                prevBool = BackButton;
                BackButton = getFeatureUsage(primaryLeftController, CommonUsages.menuButton);
                BackButtonDown = prevBool == false && BackButton == true;
#endif
            }
            else if(InputSource == XRInputSource.Pico) {
                UpdatePicoInput();
            }

            // Call events
            OnInputsUpdated?.Invoke();
        }

        public virtual void UpdateSteamInput() {
#if STEAM_VR_SDK

            LeftThumbstickAxis = ApplyDeadZones(SteamVR_Actions.vRIF_LeftThumbstickAxis.axis, ThumbstickDeadzoneX, ThumbstickDeadzoneY);
            RightThumbstickAxis = ApplyDeadZones(SteamVR_Actions.vRIF_RightThumbstickAxis.axis, ThumbstickDeadzoneX, ThumbstickDeadzoneY);
            LeftThumbstick = SteamVR_Actions.vRIF_LeftThumbstickDown.state;
            LeftThumbstickDown = SteamVR_Actions.vRIF_LeftThumbstickDown.stateDown;
            RightThumbstick = SteamVR_Actions.vRIF_RightThumbstickDown.state;
            RightThumbstickDown = SteamVR_Actions.vRIF_RightThumbstickDown.stateDown;
            
            LeftThumbNear = SteamVR_Actions.vRIF_LeftThumbstickNear.state;
            RightThumbNear = SteamVR_Actions.vRIF_RightThumbstickNear.state;

            var prevVal = LeftGrip;
            LeftGrip = LeftGrip = correctValue(SteamVR_Actions.vRIF_LeftGrip.axis);
            LeftGripDown = prevVal < _downThreshold && LeftGrip >= _downThreshold;

            prevVal = RightGrip;
            RightGrip = correctValue(SteamVR_Actions.vRIF_RightGrip.axis);
            RightGripDown = prevVal < _downThreshold && RightGrip >= _downThreshold;
            
            LeftTrigger = correctValue(SteamVR_Actions.vRIF_LeftTrigger.axis);
            RightTrigger = correctValue(SteamVR_Actions.vRIF_RightTrigger.axis);

            AButton = SteamVR_Actions.vRIF_AButton.state;
            AButtonDown = SteamVR_Actions.vRIF_AButton.stateDown;
            AButtonUp = SteamVR_Actions.vRIF_AButton.stateUp;
            BButton = SteamVR_Actions.vRIF_BButton.state;
            BButtonDown = SteamVR_Actions.vRIF_BButton.stateDown;
            BButtonUp = SteamVR_Actions.vRIF_AButton.stateUp;
            XButton = SteamVR_Actions.vRIF_XButton.state;
            XButtonDown = SteamVR_Actions.vRIF_XButton.stateDown;
            XButtonUp = SteamVR_Actions.vRIF_XButton.stateUp;
            YButton = SteamVR_Actions.vRIF_YButton.state;
            YButtonDown = SteamVR_Actions.vRIF_YButton.stateDown;
            YButtonUp = SteamVR_Actions.vRIF_YButton.stateUp;
#endif
        }

        public virtual void UpdatePicoInput() {
#if PICO_SDK
            int rightHand = 1;
            int leftHand = 0;

            LeftThumbstickAxis = ApplyDeadZones(Pvr_UnitySDKAPI.Controller.UPvr_GetAxis2D(leftHand), ThumbstickDeadzoneX, ThumbstickDeadzoneY);
            RightThumbstickAxis = ApplyDeadZones(Pvr_UnitySDKAPI.Controller.UPvr_GetAxis2D(rightHand), ThumbstickDeadzoneX, ThumbstickDeadzoneY);
            LeftThumbstick = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.TOUCHPAD);
            LeftThumbstickDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.TOUCHPAD);
            RightThumbstick = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.TOUCHPAD);
            RightThumbstickDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.TOUCHPAD);

            LeftThumbNear = Pvr_UnitySDKAPI.Controller.UPvr_IsTouching(leftHand);
            RightThumbNear = Pvr_UnitySDKAPI.Controller.UPvr_IsTouching(rightHand);

            LeftGrip = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.Left) ? 1f : 0;
            LeftGripDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.Left);

            RightGrip = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.Right) ? 1f : 0;
            RightGripDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.Right);

            LeftTrigger = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerTriggerValue(leftHand) / 255f;
            RightTrigger = Pvr_UnitySDKAPI.Controller.UPvr_GetControllerTriggerValue(rightHand) / 255f;
            AButton = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.A);
            AButtonDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.A);
            BButton = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.B);
            BButtonDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(rightHand, Pvr_UnitySDKAPI.Pvr_KeyCode.B);
            XButton = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.X);
            XButtonDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.X);
            YButton = Pvr_UnitySDKAPI.Controller.UPvr_GetKey(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.Y);
            YButtonDown = Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(leftHand, Pvr_UnitySDKAPI.Pvr_KeyCode.Y);
#endif
        }

        public virtual void UpdateDeviceActive() {

            InputDevice hmd = GetHMD();

            // Can bail early
            if (hmd.isValid == false) {
                HMDActive = false;
                return;
            }

            // Make sure the device supports the presence feature
            bool userPresent = false;
            bool presenceFeatureSupported = hmd.TryGetFeatureValue(CommonUsages.userPresence, out userPresent);
            if(presenceFeatureSupported) {
                HMDActive = userPresent;
            }
            else {
#if UNITY_2019_4
                // 2019.4 XRDevice.userPresence works, but CommonUsage does not
                // In 2020 XRDevice.userPresence does not work.
                if (XRDevice.userPresence == UserPresenceState.NotPresent) {
                    HMDActive = false;
                    return;
                }
#endif

                HMDActive = XRSettings.isDeviceActive;
            }
        }

        /// <summary>
        /// Round to nearest thousandth. This can alleviate some floating point precision errors found when using certain inputs.
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        float correctValue(float inputValue) {
            return (float)System.Math.Round(inputValue * 1000f) / 1000f;
        }


        /// <summary>
        /// Returns true if the given binding is pressed
        /// </summary>
        public bool GetControllerBindingValue(ControllerBinding val) {
            if (val == ControllerBinding.AButton && AButton) { return true; }
            if (val == ControllerBinding.AButtonDown && AButtonDown) { return true; }
            if (val == ControllerBinding.BButton && BButton) { return true; }
            if (val == ControllerBinding.BButtonDown && BButtonDown) { return true; }
            if (val == ControllerBinding.XButton && XButton) { return true; }
            if (val == ControllerBinding.XButtonDown && XButtonDown) { return true; }
            if (val == ControllerBinding.YButton && YButton) { return true; }
            if (val == ControllerBinding.YButtonDown && YButtonDown) { return true; }
            if (val == ControllerBinding.LeftTrigger && LeftTrigger > _downThreshold) { return true; }
            if (val == ControllerBinding.LeftTriggerDown && LeftTriggerDown) { return true; }
            if (val == ControllerBinding.LeftGrip && LeftGrip > _downThreshold) { return true; }
            if (val == ControllerBinding.LeftGripDown && LeftGripDown) { return true; }
            if (val == ControllerBinding.LeftThumbstick && LeftThumbstick) { return true; }
            if (val == ControllerBinding.LeftThumbstickDown && LeftThumbstickDown) { return true; }
            if (val == ControllerBinding.RightTrigger && RightTrigger > _downThreshold) { return true; }
            if (val == ControllerBinding.RightTriggerDown && RightTriggerDown) { return true; }
            if (val == ControllerBinding.RightGrip && RightGrip > _downThreshold) { return true; }
            if (val == ControllerBinding.RightGripDown && RightGripDown) { return true; }
            if (val == ControllerBinding.RightThumbstick && RightThumbstick) { return true; }
            if (val == ControllerBinding.RightThumbstickDown && RightThumbstickDown) { return true; }
            if (val == ControllerBinding.StartButton && StartButton) { return true; }
            if (val == ControllerBinding.StartButtonDown && StartButtonDown) { return true; }
            if (val == ControllerBinding.BackButton && BackButton) { return true; }
            if (val == ControllerBinding.BackButtonDown && BackButtonDown) { return true; }

            return false;
        }

        public bool GetGrabbedControllerBinding(GrabbedControllerBinding val, ControllerHand hand) {
            if(hand == ControllerHand.Right) {
                if (val == GrabbedControllerBinding.Button1 && AButton) { return true; }
                if (val == GrabbedControllerBinding.Button1Down && AButtonDown) { return true; }
                if (val == GrabbedControllerBinding.Button2 && BButton) { return true; }
                if (val == GrabbedControllerBinding.Button2Down && BButtonDown) { return true; }
                if (val == GrabbedControllerBinding.Grip && RightGrip > _downThreshold) { return true; }
                if (val == GrabbedControllerBinding.GripDown && RightGripDown) { return true; }
                if (val == GrabbedControllerBinding.Trigger && RightTrigger > _downThreshold) { return true; }
                if (val == GrabbedControllerBinding.TriggerDown && RightTriggerDown) { return true; }
            }
            else if (hand == ControllerHand.Left) {
                if (val == GrabbedControllerBinding.Button1 && XButton) { return true; }
                if (val == GrabbedControllerBinding.Button1Down && XButtonDown) { return true; }
                if (val == GrabbedControllerBinding.Button2 && YButton) { return true; }
                if (val == GrabbedControllerBinding.Button2Down && YButtonDown) { return true; }
                if (val == GrabbedControllerBinding.Grip && LeftGrip > _downThreshold) { return true; }
                if (val == GrabbedControllerBinding.GripDown && LeftGripDown) { return true; }
                if (val == GrabbedControllerBinding.Trigger && LeftTrigger > _downThreshold) { return true; }
                if (val == GrabbedControllerBinding.TriggerDown && LeftTriggerDown) { return true; }
            }

            return false;
        }

        public Vector2 GetInputAxisValue(InputAxis val) {
            if (val == InputAxis.LeftThumbStickAxis) { return LeftThumbstickAxis; }
            if (val == InputAxis.RightThumbStickAxis) { return RightThumbstickAxis; }
            if (val == InputAxis.LeftTouchPadAxis) { return LeftTouchPadAxis; }
            if (val == InputAxis.RightTouchPadAxis) { return RightTouchPadAxis; }

            return Vector3.zero;
        }

        Vector2 ApplyDeadZones(Vector2 pos, float deadZoneX, float deadZoneY) {

            if (Mathf.Abs(pos.x) < deadZoneX) {
                pos.x = 0f;
            }

            if (Mathf.Abs(pos.y) < deadZoneY) {
                pos.y = 0f;
            }

            return pos;
        }

        // Called when an input device has changed (connect / disconnect, etc.)
        void onDeviceChanged(InputDevice inputDevice) {
            setDeviceProperties();
        }

        void setDeviceProperties() {

            // Update device properties such as device name, controller properties, etc.
            // We only want to update this information if a device has changed in order to skip unnecessary checks every frame
            IsOculusDevice = GetIsOculusDevice();
            IsOculusQuest = GetIsOculusQuest();
            IsHTCDevice = GetIsHTCDevice();
            IsValveIndexController = GetIsValveIndexController();

            // Set the SDK we are using
            LoadedSDK = GetLoadedSDK();

            // Get specific device support
            SupportsIndexTouch = GetSupportsIndexTouch();
            SupportsThumbTouch = GetSupportsThumbTouch();

            // Currently only the Valve Index has both a touchpad and a joystick on the same controller
            SupportsBothTouchPadAndJoystick = IsValveIndexController;

            // Update Controller Type
            ConnectedControllerType = GetControllerType();

            // Make sure our input source is set to SteamVR for Knuckles. 
            if(ConnectedControllerType == ControllerType.Knuckles && InputSource != XRInputSource.SteamVR && SteamVRSupport) {
                Debug.Log("Knuckle controllers found. Switching Input Source to SteamVR.");
                InputSource = XRInputSource.SteamVR;
            }

            // Call any events
            if(!string.IsNullOrEmpty(InputBridge.Instance.GetControllerName())) {
                OnControllerFound?.Invoke();
            }
        }

        /// <summary>
        /// Returns true if the controllers support the 'indexTouch' XR input mapping.Currently only Oculus devices on the Oculus SDK support index touch.OpenVR is not supported.
        /// </summary>
        /// <returns></returns>
        public virtual bool GetSupportsIndexTouch() {
            return IsOculusDevice && LoadedSDK == SDKProvider.OculusSDK;
        }

        public virtual SDKProvider GetLoadedSDK() {

            // Can exit early if no device name has been picked up yet
            if (XRSettings.loadedDeviceName == null) {
                return SDKProvider.Unknown;
            }

            string deviceName = XRSettings.loadedDeviceName.ToLower();

            // Example : "oculus display"
            if (deviceName.StartsWith("oculus")) {
                return SDKProvider.OculusSDK;
            }
            // Example : "OpenVR Display"
            else if (deviceName.StartsWith("openvr")) {
                return SDKProvider.OpenVR;
            }

            return SDKProvider.Unknown;
        }

        public virtual bool GetSupportsThumbTouch() {
            return IsOculusDevice && LoadedSDK == SDKProvider.OculusSDK;
        }

        public virtual bool GetIsOculusDevice() {

            var primaryHMD = GetHMD();

            // OpenVR Format
            if (primaryHMD != null && primaryHMD.manufacturer == "Oculus") {
                return true;
            }

#if UNITY_2019_2_OR_NEWER
            return XRSettings.loadedDeviceName != null && XRSettings.loadedDeviceName.ToLower().Contains("oculus");
#else
            return true;
#endif
        }

        public virtual bool GetIsOculusQuest() {
#if UNITY_2019_2_OR_NEWER

            var primaryHMD = GetHMD();

            // Example : "OpenVR Headset(Oculus Quest)"
            if (primaryHMD != null && primaryHMD.name != null && primaryHMD.name.EndsWith("(Oculus Quest)")) {
                return true;
            }
            // Non-OpenVR version use "contains" on string. 
            else if (primaryHMD != null && primaryHMD.name != null && primaryHMD.name.Contains("Oculus Quest")) {
                return true;
            }

            //  Fallback to refresh rate
            return GetIsOculusDevice() && XRDevice.refreshRate == 72f;
#else
            if (Application.platform == RuntimePlatform.Android) {
                return true;
            }
            
            return false;
#endif
        }

        public virtual bool GetIsHTCDevice() {
            // Is HTC Device
#if UNITY_2019_2_OR_NEWER
            var primaryHMD = GetHMD();

            // OpenVR Format
            if (primaryHMD != null && primaryHMD.manufacturer == "HTC") {
                return true;
            }

            return XRSettings.loadedDeviceName.StartsWith("HTC");
#else
           return false;
#endif
        }

        public InputDevice GetHMD() {
            InputDevices.GetDevices(devices);

            var hmds = new List<InputDevice>();
            var dc1 = InputDeviceCharacteristics.HeadMounted;
            InputDevices.GetDevicesWithCharacteristics(dc1, hmds);

            return hmds.FirstOrDefault();
        }

        public InputDevice GetLeftController() {
            InputDevices.GetDevices(devices);

            var leftHandedControllers = new List<InputDevice>();
            var dc = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(dc, leftHandedControllers);
            return leftHandedControllers.FirstOrDefault();
        }

        public InputDevice GetRightController() {
            InputDevices.GetDevices(devices);

            var rightHandedControllers = new List<InputDevice>();
            var dc = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(dc, rightHandedControllers);

            return rightHandedControllers.FirstOrDefault();
        }

        public virtual ControllerType GetControllerType() {

            if (IsValveIndexController) {
                return ControllerType.Knuckles;
            }
            else if (IsOculusDevice) {
                return ControllerType.OculusTouch;
            }
            else if (IsHTCDevice) {
                return ControllerType.Wand;
            }

            return ControllerType.Unknown;
        }

        public Vector3 GetControllerVelocity(ControllerHand hand) {
            InputDevice inputDevice = hand == ControllerHand.Left ? GetLeftController() : GetRightController();
            return getFeatureUsage(inputDevice, CommonUsages.deviceVelocity);
        }

        public Vector3 GetControllerAngularVelocity(ControllerHand hand) {
            InputDevice inputDevice = hand == ControllerHand.Left ? GetLeftController() : GetRightController();
            return getFeatureUsage(inputDevice, CommonUsages.deviceAngularVelocity);
        }

        /// <summary>
        /// Get the name of the primary controller
        /// </summary>
        /// <returns>The name of the primary controller. Returns empty if no controller found</returns>
        public virtual string GetControllerName() {

            // First try right controller
            var rightHandedControllers = new List<InputDevice>();
            var dc = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(dc, rightHandedControllers);
            var primaryRightController = rightHandedControllers.FirstOrDefault();

            // Return name of the found controller
            if (primaryRightController != null && !System.String.IsNullOrEmpty(primaryRightController.name)) {
                return primaryRightController.name;
            }

            // No right controller found, try the left
            var leftHandedControllers = new List<InputDevice>();
            dc = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(dc, leftHandedControllers);
            var primaryLeftController = leftHandedControllers.FirstOrDefault();

            // Return name of the found controller
            if (primaryLeftController != null && !System.String.IsNullOrEmpty(primaryLeftController.name)) {
                return primaryLeftController.name;
            }

            return string.Empty;
        }

        public virtual bool GetIsValveIndexController() {
            var rightHandedControllers = new List<InputDevice>();
            var dc = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(dc, rightHandedControllers);
            var primaryRightController = rightHandedControllers.FirstOrDefault();

            // Are we using Valve Index Controllers?
            if (primaryRightController != null && !System.String.IsNullOrEmpty(primaryRightController.name)) {
                return primaryRightController.name.Contains("Knuckles");
            }

            return false;
        }

#if UNITY_2019_2_OR_NEWER
        float getFeatureUsage(InputDevice device, InputFeatureUsage<float> usage, bool clamp = true) {
            float val;
            device.TryGetFeatureValue(usage, out val);

            return Mathf.Clamp01(val);
        }

        bool getFeatureUsage(InputDevice device, InputFeatureUsage<bool> usage) {
            bool val;
            if (device.TryGetFeatureValue(usage, out val)) {
                return val;
            }

            return val;
        }

        Vector2 getFeatureUsage(InputDevice device, InputFeatureUsage<Vector2> usage) {
            Vector2 val;
            if (device.TryGetFeatureValue(usage, out val)) {
                return val;
            }

            return val;
        }

        Vector3 getFeatureUsage(InputDevice device, InputFeatureUsage<Vector3> usage) {
            Vector3 val;
            if (device.TryGetFeatureValue(usage, out val)) {
                return val;
            }

            return val;
        }
#endif

        public virtual void SetTrackingOriginMode(TrackingOriginModeFlags trackingOrigin) {
#if UNITY_2019_4
            // 2019.4 Needs to use XRDevice.SetTrackingSpaceType; TrySetTrackingOriginMode does not function properly.
            if (trackingOrigin == TrackingOriginModeFlags.Floor) {
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
            }
#endif
            // Any other versions (including 2019.4) may go ahead and use TrySetTrackingSpaceType
            StartCoroutine(changeOriginModeRoutine(trackingOrigin));
        }

        IEnumerator changeOriginModeRoutine(TrackingOriginModeFlags trackingOrigin) {

            // Wait one frame as Unity has an issue with calling this immediately
            yield return null;

            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            int subSystemsCount = subsystems.Count;

            if (subSystemsCount > 0) {
                for (int x = 0; x < subSystemsCount; x++) {
                    if (subsystems[x].TrySetTrackingOriginMode(trackingOrigin)) {
                        Debug.Log("Successfully set TrackingOriginMode to " + trackingOrigin);
                    }
                    else {
                        Debug.Log("Failed to set TrackingOriginMode to " + trackingOrigin);
                    }
                }
            }
            else {
#if UNITY_2020
                Debug.LogWarning("No subsystems detected. Unable to set Tracking Origin to " + trackingOrigin);
#endif
            }
        }

        // Start Vibration on controller
        public void VibrateController(float frequency, float amplitude, float duration, ControllerHand hand) {
            
            if (InputSource == XRInputSource.OVRInput) {
                StartCoroutine(Vibrate(frequency, amplitude, duration, hand));
            }
            else if (InputSource == XRInputSource.SteamVR && SteamVRSupport) {
#if STEAM_VR_SDK
                if (hand == ControllerHand.Right) {
                    SteamVR_Actions.vRIF_Haptic.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.RightHand);
                }
                else {
                    SteamVR_Actions.vRIF_Haptic.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.LeftHand);
                }                
#endif
            }
            // Default / Fallback to XRInput
            else {
                if (hand == ControllerHand.Right) {
                    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, devices);
                }
                else {
                    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, devices);
                }

                for (int x = 0; x < devices.Count; x++) {
                    HapticCapabilities capabilities;
                    if (devices[x].TryGetHapticCapabilities(out capabilities)) {
                        if (capabilities.supportsImpulse) {
                            uint channel = 0;
                            devices[x].SendHapticImpulse(channel, amplitude, duration);
                        }
                    }
                }
            }
        }

        IEnumerator Vibrate(float frequency, float amplitude, float duration, ControllerHand hand) {
#if OCULUS_INTEGRATION
            // Start vibration
            if (hand == ControllerHand.Right) {
                OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
            }
            else if (hand == ControllerHand.Left) {
                OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
            }

            yield return new WaitForSeconds(duration);

            // Stop vibration
            if (hand == ControllerHand.Right) {
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            }
            else if (hand == ControllerHand.Left) {
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            }
#else
            yield return new WaitForSeconds(duration);
#endif
        }
    }
}