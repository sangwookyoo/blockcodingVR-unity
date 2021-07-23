using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class HandModelSelector : MonoBehaviour {

        /// <summary>
        /// Child index of the hand model to use if nothing stored in playerprefs and LoadHandSelectionFromPrefs true
        /// </summary>
        [Tooltip("Child index of the hand model to use if nothing stored in playerprefs or LoadHandSelectionFromPrefs set to false")]        
        public int DefaultHandsModel = 1;

        /// <summary>
        /// If true, hand model will be saved and loaded from player prefs. If false DefaultHandModel will be loaded.
        /// </summary>
        [Tooltip("If true, the selected hand model will be saved and loaded from player prefs")]  
        public bool LoadHandSelectionFromPrefs = false;


        [Tooltip("Input used to toggle between hands")]
        public ControllerBinding ToggleHandsInput = ControllerBinding.RightThumbstickDown;

        /// <summary>
        /// This transform holds all of the hand models. Can be used to enabled / disabled various hand options
        /// </summary>
        [Tooltip("This transform holds all of the hand models. Can be used to enabled / disabled various hand options.")]
        public Transform LeftHandGFXHolder;

        /// <summary>
        /// This transform holds all of the hand models. Can be used to enabled / disabled various hand options
        /// </summary>
        [Tooltip("This transform holds all of the hand models. Can be used to enabled / disabled various hand options")]
        public Transform RightHandGFXHolder;
        private int _selectedHandGFX = 0;

        /// <summary>
        /// Used for demo IK Hands / Body option
        /// </summary>
        [Tooltip("Used for IK Hands / Body option")]
        public CharacterIK IKBody;

        /// <summary>
        /// This is the start point of a line for UI purposes. We may want to move this around if we change models or controllers.        
        /// </summary>
        UIPointer uiPoint;       

        void Start() {
            uiPoint = GetComponentInChildren<UIPointer>();

            // Load new Hands or default
            if (LoadHandSelectionFromPrefs) {
                ChangeHandsModel(PlayerPrefs.GetInt("HandSelection", DefaultHandsModel), false);
            }
            else {
                ChangeHandsModel(DefaultHandsModel, false);
            }
        }

        void Update() {
            // Cycle through hand models with Right Thumbstick
            if (ToggleHandsInput.GetDown()) {
                ChangeHandsModel(_selectedHandGFX + 1, LoadHandSelectionFromPrefs);
            }
        }

        public void ChangeHandsModel(int childIndex, bool save = false) {

            // Deactivate any previous models
            if(LeftHandGFXHolder.childCount > _selectedHandGFX) {
                LeftHandGFXHolder.GetChild(_selectedHandGFX).gameObject.SetActive(false);
                RightHandGFXHolder.GetChild(_selectedHandGFX).gameObject.SetActive(false);
            }

            // Loop back to beginning if we went over
            _selectedHandGFX = childIndex;
            if (_selectedHandGFX > LeftHandGFXHolder.childCount - 1) {
                _selectedHandGFX = 0;
            }

            // Activate New
            GameObject leftHand = LeftHandGFXHolder.GetChild(_selectedHandGFX).gameObject;
            GameObject rightHand = RightHandGFXHolder.GetChild(_selectedHandGFX).gameObject;

            leftHand.SetActive(true);
            rightHand.SetActive(true);

            // Update any animators
            HandController leftControl = LeftHandGFXHolder.parent.GetComponent<HandController>();
            HandController rightControl = RightHandGFXHolder.parent.GetComponent<HandController>();
            if (leftControl && rightControl) {
                leftControl.HandAnimator = leftHand.GetComponentInChildren<Animator>();
                rightControl.HandAnimator = rightHand.GetComponentInChildren<Animator>();
            }

            // Enable / Disable IK Character. For demo purposes only
            if (IKBody != null) {
                IKBody.gameObject.SetActive(leftHand.transform.name.Contains("IK"));
            }

            // Change UI Pointer position depending on if we're using Oculus Hands or Oculus Controller Model
            // This is for the demo. Typically this would be fixed to a bone or transform
            // Oculus Touch Controller is positioned near the front
            if (leftHand.transform.name.StartsWith("OculusTouchForQuestAndRift") && uiPoint != null) {
                uiPoint.transform.localPosition = new Vector3(0, 0, 0.0462f);
                uiPoint.transform.localEulerAngles = new Vector3(0, -4.5f, 0);
            }
            // Hand Model
            else if (_selectedHandGFX != 0 && uiPoint != null) {
                uiPoint.transform.localPosition = new Vector3(0.045f, 0.07f, 0.12f);
                uiPoint.transform.localEulerAngles = new Vector3(-9.125f, 4.65f, 0);
            }

            if (save) {
                PlayerPrefs.SetInt("HandSelection", _selectedHandGFX);
            }
        }
    }
}