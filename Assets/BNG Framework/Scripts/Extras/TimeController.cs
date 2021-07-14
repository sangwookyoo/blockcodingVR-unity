using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Press Y to slow time by modifying Time.timeScale and Time.fixedDeltaTime
    /// </summary>
    public class TimeController : MonoBehaviour {

        /// <summary>
        /// Timescale to slow down to if slow down key is pressed
        /// </summary>
        [Tooltip("Timescale to slow down to if slow down key is pressed")]
        public float SlowTimeScale = 0.5f;

        /// <summary>
        /// If true, Y Button will always slow time. Useful for debugging. Otherwise call SlowTime / ResumeTime yourself
        /// </summary>
        [Tooltip("If true, Y Button will always slow time. Useful for debugging. Otherwise call SlowTime / ResumeTime yourself")]
        public bool YKeySlowsTime = true;

        [Tooltip("(Optional) Play this clip when starting to slow time")]
        public AudioClip SlowTimeClip;

        [Tooltip("(Optional) Play this clip when ending slow mo")]
        public AudioClip SpeedupTimeClip;

        /// <summary>
        /// If true, will set Time.fixedDeltaTime to the device refresh rate
        /// </summary>
        [Tooltip("If true, will set Time.fixedDeltaTime to the device refresh rate")]
        public bool SetFixedDelta = false;

        public bool TimeSlowing
        {
            get { return _slowingTime;  }
        }
        bool _slowingTime = false;
        bool routineRunning = false;

        float originalFixedDelta;
        AudioSource audioSource;

        public bool ForceTimeScale = false;

        // Start is called before the first frame update
        void Start() {
            
            if(SetFixedDelta) {
                Time.fixedDeltaTime = (Time.timeScale / UnityEngine.XR.XRDevice.refreshRate);
            }

            originalFixedDelta = Time.fixedDeltaTime;

            audioSource = GetComponent<AudioSource>();
        }

        void Update() {

            if ((YKeySlowsTime && InputBridge.Instance.YButton) || ForceTimeScale) {
                SlowTime();
            }
            else {
                ResumeTime();
            }
        }

        public void SlowTime() {
           
            if(!_slowingTime) {

                // Make sure we aren't running a routine
                if(resumeRoutine != null) {
                    StopCoroutine(resumeRoutine);
                }

                // Play Slow time clip
                audioSource.clip = SlowTimeClip;
                audioSource.Play();

                // Haptics
                InputBridge.Instance.VibrateController(0.1f, 0.2f, SpeedupTimeClip.length, ControllerHand.Left);

                Time.timeScale = SlowTimeScale;
                Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;

                _slowingTime = true;
            }
        }

        private IEnumerator resumeRoutine;
        public void ResumeTime() {
            // toggled over; play audio cue
            // Don't resume until we're done playing the initial sound clip
            if(_slowingTime && !audioSource.isPlaying && !routineRunning) {

                resumeRoutine = resumeTimeRoutine();
                StartCoroutine(resumeRoutine);
            }
        }

        IEnumerator resumeTimeRoutine() {
            routineRunning = true;

            audioSource.clip = SpeedupTimeClip;
            audioSource.Play();

            InputBridge.Instance.VibrateController(0.1f, 0.2f, SpeedupTimeClip.length, ControllerHand.Left);

            // Wait for a split second before resuming time again
            yield return new WaitForSeconds(0.35f);

            Time.timeScale = 1;
            Time.fixedDeltaTime = originalFixedDelta;

            _slowingTime = false;
            routineRunning = false;
        }
    }
}

