using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BNG {
    public class NetworkedGrabbable : Grabbable, IPunObservable {

        PhotonView view;
        Rigidbody rb;

        // Used to Lerp our position when we are not the owner
        private Vector3 _syncStartPosition = Vector3.zero;
        private Vector3 _syncEndPosition = Vector3.zero;
        private Quaternion _syncStartRotation = Quaternion.identity;
        private Quaternion _syncEndRotation = Quaternion.identity;
        private bool _syncBeingHeld = false;

        // Interpolation values
        private float _lastSynchronizationTime = 0f;
        private float _syncDelay = 0f;
        private float _syncTime = 0f;

        void Start() {
            view = GetComponent<PhotonView>();
            rb = GetComponent<Rigidbody>();
        }        

        public override void Update() {

            base.Update();

            // Check if owner has left or is unassigned
            CheckForNullOwner();

            // Remote Player
            if (!view.IsMine && view.Owner != null && _syncEndPosition != null && _syncEndPosition != Vector3.zero) {

                rb.isKinematic = true;

                // Keeps latency in mind to keep object in sync
                _syncTime += Time.deltaTime;
                float syncValue = _syncTime / _syncDelay;
                float dist = Vector3.Distance(_syncStartPosition, _syncEndPosition);

                // If far away just teleport there
                if (dist > 3f) {
                    transform.position = _syncEndPosition;
                    transform.rotation = _syncEndRotation;
                }
                else {
                    transform.position = Vector3.Lerp(_syncStartPosition, _syncEndPosition, syncValue);
                    transform.rotation = Quaternion.Lerp(_syncStartRotation, _syncEndRotation, syncValue);
                }

                BeingHeld = _syncBeingHeld;
            }
            // Our object. Does not need to be forced to kinematic
            else if (view.IsMine) {
                if(rb) {
                    rb.isKinematic = wasKinematic;
                }

                BeingHeld = heldByGrabbers != null && heldByGrabbers.Count > 0;
            }
        }

        /// <summary>
        /// Enforce an owner on scene objects
        /// </summary>
        protected bool requestingOwnerShip = false;
        public virtual void CheckForNullOwner() {

            // Only master client should check for empty owner
            if (!PhotonNetwork.IsMasterClient) {
                return;
            }

            // No longer requesting ownership since this view is mine
            if (requestingOwnerShip && view.AmOwner) {
                requestingOwnerShip = false;
            }

            // Currently waiting for ownership request
            if (requestingOwnerShip) {
                return;
            }

            // Master Client should Request Ownership if not yet set. This could be a scene object or if ownership was lost
            if (view.AmOwner == false && view.Owner == null) {
                requestingOwnerShip = true;
                view.TransferOwnership(PhotonNetwork.MasterClient);
            }
        }

        public override bool IsGrabbable() {

            // If base isn't grabbable we can bail early
            if (base.IsGrabbable() == false) {
                return false;
            }

            // No Photon View attached
            if (view == null) {
                return true;
            }

            // We own this object. It is Grabbable
            if (view.IsMine) {
                return true;
            }

            // Not yet connected
            if (!PhotonNetwork.IsConnected) {
                return true;
            }

            return false;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            // This is our object, send our positions to the other players
            if (stream.IsWriting && view.IsMine) {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(BeingHeld);
            }
            // Receive Updates
            else {
                // Position
                _syncStartPosition = transform.position;
                _syncEndPosition = (Vector3)stream.ReceiveNext();

                // Rotation
                _syncStartRotation = transform.rotation;
                _syncEndRotation = (Quaternion)stream.ReceiveNext();

                // Status
                _syncBeingHeld = (bool)stream.ReceiveNext();

                _syncTime = 0f;
                _syncDelay = Time.time - _lastSynchronizationTime;
                _lastSynchronizationTime = Time.time;
            }
        }
    }
}
