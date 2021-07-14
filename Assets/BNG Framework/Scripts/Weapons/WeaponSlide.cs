using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Weapon slide on a pistol. Charges weapon and ejects casings.
    /// </summary>
    public class WeaponSlide : MonoBehaviour {

        /// <summary>
        /// Minimum distance slide will travel on Z axis
        /// </summary>
        public float MinLocalZ = -0.03f;

        /// <summary>
        /// Max distance slide will travel on Z axis
        /// </summary>
        public float MaxLocalZ = 0;

        // Keep track of which way we are sliding
        bool slidingBack = true;

        /// <summary>
        /// Is the Slide locked back due to last shot
        /// </summary>
        public bool LockedBack = false;

        /// <summary>
        /// Sound to play when slide is released back into position
        /// </summary>
        public AudioClip SlideReleaseSound;

        /// <summary>
        /// Sound to play after last shot has fired and slide is forced back
        /// </summary>
        public AudioClip LockedBackSound;

        /// <summary>
        /// When true, the slide will be set to 0 mass when not being held. This fixes jitter caused by the slide having a configurable joint attached to the weapon
        /// </summary>
        public bool ZeroMassWhenNotHeld = true;

        RaycastWeapon parentWeapon;
        Grabbable parentGrabbable;
        Vector3 initialLocalPos;
        Grabbable thisGrabbable;
        AudioSource audioSource;
        Rigidbody rigid;
        float initialMass;

        void Start() {
            initialLocalPos = transform.localPosition;
            audioSource = GetComponent<AudioSource>();
            parentWeapon = transform.parent.GetComponent<RaycastWeapon>();
            parentGrabbable = transform.parent.GetComponent<Grabbable>();
            thisGrabbable = GetComponent<Grabbable>();
            rigid = GetComponent<Rigidbody>();
            initialMass = rigid.mass;

            if (parentWeapon != null) {
                Physics.IgnoreCollision(GetComponent<Collider>(), parentWeapon.GetComponent<Collider>());
            }
        }

        // Update is called once per frame
        void Update() {
            float localZ = transform.localPosition.z;

            if (LockedBack) {
                transform.localPosition = new Vector3(initialLocalPos.x, initialLocalPos.y, MinLocalZ);

                // Not locking back if hand is holding this
                if (thisGrabbable != null && thisGrabbable.BeingHeld) {
                    UnlockBack();
                }
            }

            if (!LockedBack) {
                // Clamp values
                if (localZ <= MinLocalZ) {
                    transform.localPosition = new Vector3(initialLocalPos.x, initialLocalPos.y, MinLocalZ);

                    if (slidingBack) {
                        onSlideBack();
                    }
                }
                else if (localZ >= MaxLocalZ) {
                    transform.localPosition = new Vector3(initialLocalPos.x, initialLocalPos.y, MaxLocalZ);

                    // Moving forward
                    if (!slidingBack) {
                        onSlideForward();
                    }
                }
            }
        }

        public void OnSliderChange(float sliderValue) {
            GetComponent<Light>().intensity = sliderValue;
        }

        void FixedUpdate() {
            // Change mass of slider rigidbody. This prevents stuttering when the object is not held and the slide is back
            if (ZeroMassWhenNotHeld && parentGrabbable.BeingHeld && rigid) {
                rigid.mass = initialMass;
            }
            else if (ZeroMassWhenNotHeld && rigid) {
                // Set mass to very low to prevent stuttering when not held
                rigid.mass = 0.0001f;
            }
        }

        public void LockBack() {

            if (!LockedBack) {
                if (thisGrabbable.BeingHeld || parentGrabbable.BeingHeld) {
                    VRUtils.Instance.PlaySpatialClipAt(LockedBackSound, transform.position, 1f, 0.8f);
                }

                LockedBack = true;
            }
        }

        public void UnlockBack() {

            if (LockedBack) {
                if (thisGrabbable.BeingHeld || parentGrabbable.BeingHeld) {
                    VRUtils.Instance.PlaySpatialClipAt(SlideReleaseSound, transform.position, 1f, 0.9f);
                }

                LockedBack = false;

                // This is considered a charge
                if (parentWeapon != null) {
                    parentWeapon.OnWeaponCharged(false);
                }
            }
        }

        void onSlideBack() {

            if (thisGrabbable.BeingHeld || parentGrabbable.BeingHeld) {
                playSoundInterval(0, 0.2f, 0.9f);
            }

            if (parentWeapon != null) {
                parentWeapon.OnWeaponCharged(true);
            }

            slidingBack = false;
        }

        void onSlideForward() {

            if (thisGrabbable.BeingHeld || parentGrabbable.BeingHeld) {
                playSoundInterval(0.2f, 0.35f, 1f);
            }

            slidingBack = true;
        }

        void playSoundInterval(float fromSeconds, float toSeconds, float volume) {
            if (audioSource) {

                if (audioSource.isPlaying) {
                    audioSource.Stop();
                }

                audioSource.pitch = Time.timeScale;
                audioSource.time = fromSeconds;
                audioSource.volume = volume;
                audioSource.Play();
                audioSource.SetScheduledEndTime(AudioSettings.dspTime + (toSeconds - fromSeconds));
            }
        }
    }
}
