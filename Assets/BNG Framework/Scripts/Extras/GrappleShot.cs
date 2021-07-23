using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class GrappleShot : GrabbableEvents {

        public float MaxRange = 100f;
        public float GrappleReelForce = 0.5f;

        // currentGrappleDistance must be greater than this to reel in
        public float MinReelDistance = 0.25f;

        public LayerMask GrappleLayers;

        public Transform MuzzleTransform;
        public Transform HitTargetPrefab;
        public LineRenderer GrappleLine;
        public LineRenderer HelperLine;

        public AudioClip GrappleShotSound;

        // Are we currently grappling towards an object
        bool grappling = false;
        // Were we grappling last frame
        bool wasGrappling = false;

        CharacterController characterController;
        BNGPlayerController bngController;
        PlayerGravity playerGravity;
        PlayerClimbing playerClimbing;

        AudioSource audioSource;

        // How far away the grapple is in meters
        public float currentGrappleDistance = 0;

        bool validTargetFound = false;// Is there something valid to grapple on to
        
        bool isDynamic = false; // Can we reel the object in
        Rigidbody grappleTargetRigid; // Object we're grappling
        Collider grappleTargetCollider; // Collider we're grappling
        Transform grappleTargetParent; // Store original parent
        bool requireRelease = false; // Require release to be pressed before continuing
        bool climbing = false; // Have we transitioned to climbing?

        // We will use the climbable as our means of moving and pulling the character around
        public Climbable ClimbHelper;

        // Start is called before the first frame update
        void Start() {

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) {
                characterController = player.GetComponentInChildren<CharacterController>();
                bngController = player.GetComponentInChildren<BNGPlayerController>();
                playerGravity = player.GetComponentInChildren<PlayerGravity>();
                playerClimbing = player.GetComponentInChildren<PlayerClimbing>();
            }
            else {
                Debug.Log("No player object found.");
            }

            audioSource = GetComponent<AudioSource>();
        }      

        private void LateUpdate() {
            // Draw Lines in LateUpdate
            // Draw Helper Line
            if (!grappling && grab.BeingHeld && !requireRelease) {
                drawGrappleHelper();
            }
            else {
                hideGrappleHelper();
            }

            // Draw Grappling Line
            if (grappling && validTargetFound && grab.BeingHeld) {
                drawGrappleLine();
            }
            else {
                hideGrappleLine();
            }
        }

        // Try to shoot or grapple
        public override void OnTrigger(float triggerValue) {

            updateGrappleDistance();

            // Fire gun if possible
            if (triggerValue >= 0.25f) {
                if (grappling) {
                    reelInGrapple(triggerValue);
                }
                else {
                    shootGrapple();
                }
            }
            else {
                grappling = false;
                requireRelease = false;                            
            }

            // User was grappling previous frame, but not now
            if(!grappling && wasGrappling) {
                onReleaseGrapple();
            }

            base.OnTrigger(triggerValue);
        }

        void updateGrappleDistance() {
            // Update Distance
            if (grappling) {
                currentGrappleDistance = Vector3.Distance(MuzzleTransform.position, HitTargetPrefab.position);
            }
            else {
                currentGrappleDistance = 0;
            }
        }

        public override void OnGrab(Grabber grabber) {
            base.OnGrab(grabber);
        }

        public override void OnRelease() {
            onReleaseGrapple();

            base.OnRelease();
        }

        // Called when grappling previous frame, but not this one
        void onReleaseGrapple() {

            // Reset gravity back to normal
            changeGravity(true);

            if(grappleTargetRigid && isDynamic) {
                grappleTargetRigid.useGravity = true;
                grappleTargetRigid.isKinematic = false;
                grappleTargetRigid.transform.parent = grappleTargetParent;

                // More reliable method of resetting parent :
                if(grappleTargetRigid.GetComponent<Grabbable>()) {
                    grappleTargetRigid.GetComponent<Grabbable>().ResetParent();
                }
            }

            // Reset Climbing
            ClimbHelper.transform.localPosition = Vector3.zero;
            playerClimbing.RemoveClimber(thisGrabber);
            climbing = false;

            grappling = false;
            validTargetFound = false;
            isDynamic = false;
            wasGrappling = false;
        }

        // Draw area where Grapple will land
        void drawGrappleHelper() {

            if (HitTargetPrefab) {

                RaycastHit hit;
                if (Physics.Raycast(MuzzleTransform.position, MuzzleTransform.forward, out hit, MaxRange, GrappleLayers, QueryTriggerInteraction.Ignore)) {

                    // Ignore other grapple shots
                    if (hit.transform.name.StartsWith("Grapple")) {
                        hideGrappleHelper();
                        validTargetFound = false;
                        isDynamic = false;
                        return;
                    }

                    showGrappleHelper(hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));

                    grappleTargetRigid = hit.collider.GetComponent<Rigidbody>();
                    grappleTargetCollider = hit.collider;
                    isDynamic = grappleTargetRigid != null && !grappleTargetRigid.isKinematic && grappleTargetRigid.useGravity;

                    // Parent the helper to the object we hit so it will be moved with it
                    // Only allow uniform scale so it doesn't warp
                    bool isUniformVector = hit.collider.transform.localScale.x == hit.collider.transform.localScale.y;
                    if (isUniformVector || isDynamic) {
                        HitTargetPrefab.parent = null;
                        HitTargetPrefab.localScale = Vector3.one;
                        HitTargetPrefab.transform.parent = hit.collider.transform;
                                               
                    }
                    else {
                        HitTargetPrefab.parent = null;
                        HitTargetPrefab.localScale = Vector3.one;
                    }

                    validTargetFound = true;

                    if(isDynamic) {
                        grappleTargetParent = grappleTargetRigid.transform.parent;
                    }
                    else {
                        grappleTargetParent = null;
                    }
                }
                else {
                    hideGrappleHelper();
                    validTargetFound = false;
                    isDynamic = false;
                }
            }
        }

        void drawGrappleLine() {
            GrappleLine.gameObject.SetActive(true);
            GrappleLine.SetPosition(0, MuzzleTransform.position);
            GrappleLine.SetPosition(1, HitTargetPrefab.position);
        }

        void hideGrappleLine() {            
            if (GrappleLine && GrappleLine.gameObject.activeSelf) {
                GrappleLine.gameObject.SetActive(false);
            }
        }

        void showGrappleHelper(Vector3 pos, Quaternion rot) {

            HitTargetPrefab.gameObject.SetActive(true);

            HitTargetPrefab.position = pos;
            HitTargetPrefab.rotation = rot;
            HitTargetPrefab.localScale = Vector3.one;

            if (HelperLine) {
                HelperLine.gameObject.SetActive(true);
                HelperLine.SetPosition(0, MuzzleTransform.position);
                HelperLine.SetPosition(1, pos);
            }
        }

        void hideGrappleHelper() {
            if(HitTargetPrefab && HitTargetPrefab.gameObject.activeSelf) {
                HitTargetPrefab.gameObject.SetActive(false);
            }

            if (HelperLine && HelperLine.gameObject.activeSelf) {
                HelperLine.gameObject.SetActive(false);
            }
        }

        void reelInGrapple(float triggerValue) {

            // Has the collider been destroyed or disabled?
            if(validTargetFound && grappleTargetCollider != null && !grappleTargetCollider.enabled) {
                dropGrapple();
                return;
            }

            if(validTargetFound && currentGrappleDistance > MinReelDistance) {
                
                // Move object towards our hand
                if(isDynamic) {
                    grappleTargetRigid.isKinematic = false;
                    grappleTargetRigid.transform.parent = grappleTargetParent;
                    grappleTargetRigid.useGravity = false;
                    grappleTargetRigid.AddForce((MuzzleTransform.position - grappleTargetRigid.transform.position) * 0.1f, ForceMode.VelocityChange);
                                                           
                    //r.MovePosition(MuzzleTransform.position * Time.deltaTime);
                }
                // Move character towards Hit location
                else {
                    Vector3 moveDirection = (HitTargetPrefab.position - MuzzleTransform.position) * GrappleReelForce;

                    // Turn off gravity before we move
                    changeGravity(false);
                    characterController.Move(moveDirection * Time.deltaTime * triggerValue);
                }
            }
            else if(validTargetFound && currentGrappleDistance <= MinReelDistance) {

                if (isDynamic) {
                    //grappleTargetRigid.useGravity = true;
                    grappleTargetRigid.velocity = Vector3.zero;
                    grappleTargetRigid.isKinematic = true;
                    grappleTargetRigid.transform.parent = transform;
                }

                if(!climbing && !isDynamic) {
                    // Add climbable / grabber
                    ClimbHelper.transform.localPosition = Vector3.zero;
                    playerClimbing.AddClimber(ClimbHelper, thisGrabber);
                    climbing = true;
                }
                
            }
        }

        // Shoot the valid out if valid target
        void shootGrapple() {
           
            if(validTargetFound) {

                // Play Grapple Sound
                if(GrappleShotSound && audioSource) {
                    audioSource.clip = GrappleShotSound;
                    audioSource.pitch = Time.timeScale;
                    audioSource.Play();
                }

                grappling = true;
                wasGrappling = true;
                requireRelease = true;
            }
        }

        void dropGrapple() {
            grappling = false;
            validTargetFound = false;
            isDynamic = false;
            wasGrappling = false;
        }

        void changeGravity(bool gravityOn) {
            if(playerGravity) {
                playerGravity.ToggleGravity(gravityOn);
            }
        }
    }
}