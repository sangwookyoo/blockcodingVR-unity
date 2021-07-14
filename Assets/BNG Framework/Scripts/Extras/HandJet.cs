using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNG {


    /// <summary>
    /// Like a jetpack, but for your hands.
    /// </summary>
    public class HandJet : GrabbableEvents {

        public float JetForce = 10f;
        public ParticleSystem JetFX;

        CharacterController characterController;
        PlayerGravity playerGravity;

        AudioSource audioSource;

        void Start() {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if(player) {
                characterController = player.GetComponentInChildren<CharacterController>();
                playerGravity = player.GetComponentInChildren<PlayerGravity>();
            }
            else {
                Debug.Log("No player object found.");
            }

            audioSource = GetComponent<AudioSource>();
        }

        public override void OnTrigger(float triggerValue) {

            if(triggerValue > 0.25f) {
                doJet(triggerValue);
            }
            else {
                stopJet();
            }

            base.OnTrigger(triggerValue);
        }

        public override void OnGrab(Grabber grabber) {
            // enforce gravity
            ChangeGravity(false);
        }

        public void ChangeGravity(bool gravityOn) {
            if(playerGravity) {
                playerGravity.ToggleGravity(gravityOn);
            }
        }

        public override void OnRelease() {
            stopJet();

            // enforce gravity
            ChangeGravity(true);
        }

        void doJet(float triggerValue) {
            Vector3 moveDirection = transform.forward * JetForce;
            characterController.Move(moveDirection * Time.deltaTime * triggerValue);

            // Gravity is always off while jetting
            ChangeGravity(false);

            // Sound
            if (!audioSource.isPlaying) {
                audioSource.pitch = Time.timeScale;
                audioSource.Play();
            }

            // Particle FX
            if(JetFX != null && !JetFX.isPlaying) {
                JetFX.Play();
            }

            //Haptics
            if(input && thisGrabber != null) {
                input.VibrateController(0.1f, 0.5f, 0.2f, thisGrabber.HandSide);
            }
        }

        void stopJet() {

            if (audioSource.isPlaying) {
                audioSource.Stop();
            }

            if (JetFX != null && JetFX.isPlaying) {
                JetFX.Stop();
            }
        }

        public override void OnTriggerUp() {
            stopJet();
            base.OnTriggerUp();
        }
    }
}

