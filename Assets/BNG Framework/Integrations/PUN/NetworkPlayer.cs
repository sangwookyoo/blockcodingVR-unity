using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable {

        [Tooltip("Transform of the local player's head to track. This will be applied to the Remote Player's Head Transform")]
        public Transform PlayerHeadTransform;
        public Transform PlayerLeftHandTransform;
        public Transform PlayerRightHandTransform;

        [Tooltip("Transform of the remote player's head. This will be updated during Update")]
        public Transform RemoteHeadTransform;

        // Store positions to move between updates
        private Vector3 _syncHeadStartPosition = Vector3.zero;
        private Vector3 _syncHeadEndPosition = Vector3.zero;
        private Quaternion _syncHeadStartRotation = Quaternion.identity;
        private Quaternion _syncHeadEndRotation = Quaternion.identity;

        [Tooltip("Transform of the remote player's left hand / controller. This will be updated during Update")]
        public Transform RemoteLeftHandTransform;

        // Store positions to move between updates
        private Vector3 _syncLHandStartPosition = Vector3.zero;
        private Vector3 _syncLHandEndPosition = Vector3.zero;
        private Quaternion _syncLHandStartRotation = Quaternion.identity;
        private Quaternion _syncLHandEndRotation = Quaternion.identity;

        [Tooltip("Transform of the remote player's right hand / controller. This will be updated during Update")]
        public Transform RemoteRightHandTransform;

        // Store positions to move between updates
        private Vector3 _syncRHandStartPosition = Vector3.zero;
        private Vector3 _syncRHandEndPosition = Vector3.zero;
        private Quaternion _syncRHandStartRotation = Quaternion.identity;
        private Quaternion _syncRHandEndRotation = Quaternion.identity;

        // Send Hand Animation info to others
        public HandController LeftHandController;
        public HandController RightHandController;

        // Receive Animation info
        public Animator RemoteLeftHandAnimator;
        public Animator RemoteRightHandAnimator;

        [Tooltip("Local Player's Left Grabber. Used to determine which objects are nearby")]
        public Grabber LeftGrabber;
        GrabbablesInTrigger gitLeft;

        [Tooltip("Local Player's Right Grabber. Used to determine which objects are nearby")]
        public Grabber RightGrabber;
        GrabbablesInTrigger gitRight;

        [Tooltip("How fast to animate the fingers on the remote players hands")]
        public float HandAnimationSpeed = 20f;

        // Used for Hand Interpolation
        private float _syncLeftGripStart;
        private float _syncRightGripStart;
        private float _syncLeftPointStart;
        private float _syncRightPointStart;
        private float _syncLeftThumbStart;
        private float _syncRightThumbStart;

        private float _syncLeftGripEnd;
        private float _syncRightGripEnd;
        private float _syncLeftPointEnd;
        private float _syncRightPointEnd;
        private float _syncLeftThumbEnd;
        private float _syncRightThumbEnd;

        // Interpolation values
        private float _lastSynchronizationTime = 0f;
        private float _syncDelay = 0f;
        private float _syncTime = 0f;

        // network request grabbable permission
        protected double lastRequestTime = 0;
        protected float requestInterval = 0.1f; // 0.1 = 10 times per second
        Dictionary<int, double> requestedGrabbables;

        bool disabledObjects = false;

        private bool _syncLeftHoldingItem;
        private bool _syncRightHoldingItem;

        void Start() {
            LeftGrabber = GameObject.Find("LeftController").GetComponentInChildren<Grabber>();
            gitLeft = LeftGrabber.GetComponent<GrabbablesInTrigger>();

            RightGrabber = GameObject.Find("RightController").GetComponentInChildren<Grabber>();
            gitRight = RightGrabber.GetComponent<GrabbablesInTrigger>();

            requestedGrabbables = new Dictionary<int, double>();
        }

        void Update() {

            // Check for request to grab object
            checkGrabbablesTransfer();

            // Remote Player
            if (!photonView.IsMine) {

                if(disabledObjects) {
                    toggleObjects(true);
                }

                // Keeps latency in mind to keep player in sync
                _syncTime += Time.deltaTime;
                float synchValue = _syncTime / _syncDelay;

                // Update Head and Hands Positions
                updateRemotePositionRotation(RemoteHeadTransform, _syncHeadStartPosition, _syncHeadEndPosition, _syncHeadStartRotation, _syncHeadEndRotation, synchValue);
                updateRemotePositionRotation(RemoteLeftHandTransform, _syncLHandStartPosition, _syncLHandEndPosition, _syncLHandStartRotation, _syncLHandEndRotation, synchValue);
                updateRemotePositionRotation(RemoteRightHandTransform, _syncRHandStartPosition, _syncRHandEndPosition, _syncRHandStartRotation, _syncRHandEndRotation, synchValue);

                // Update animation info
                if (RemoteLeftHandAnimator) {
                    _syncLeftGripStart = Mathf.Lerp(_syncLeftGripStart, _syncLeftGripEnd, Time.deltaTime * HandAnimationSpeed);
                    RemoteLeftHandAnimator.SetFloat("Flex", _syncLeftGripStart);
                    RemoteLeftHandAnimator.SetLayerWeight(0, 1);

                    _syncLeftPointStart = Mathf.Lerp(_syncLeftPointStart, _syncLeftPointEnd, Time.deltaTime * HandAnimationSpeed);
                    RemoteLeftHandAnimator.SetLayerWeight(2, _syncLeftPointStart);

                    _syncLeftThumbStart = Mathf.Lerp(_syncLeftThumbStart, _syncLeftThumbEnd, Time.deltaTime * HandAnimationSpeed);
                    RemoteLeftHandAnimator.SetLayerWeight(1, _syncLeftThumbStart);

                    // Default to grip if holding an item
                    if(_syncLeftHoldingItem) {
                        RemoteLeftHandAnimator.SetLayerWeight(0, 0);
                        RemoteLeftHandAnimator.SetFloat("Flex", 1);
                        RemoteLeftHandAnimator.SetFloat(1, 0);
                        RemoteLeftHandAnimator.SetFloat(2, 0);
                    }
                    else {
                        RemoteLeftHandAnimator.SetInteger("Pose", 0);
                    }
                }
                if (RemoteRightHandAnimator) {
                    _syncRightGripStart = Mathf.Lerp(_syncRightGripStart, _syncRightGripEnd, Time.deltaTime * HandAnimationSpeed);
                    RemoteRightHandAnimator.SetFloat("Flex", _syncRightGripStart);

                    _syncRightPointStart = Mathf.Lerp(_syncRightPointStart, _syncRightPointEnd, Time.deltaTime * HandAnimationSpeed);
                    RemoteRightHandAnimator.SetLayerWeight(2, _syncRightPointStart);

                    _syncRightThumbStart = Mathf.Lerp(_syncRightThumbStart, _syncRightThumbEnd, Time.deltaTime * HandAnimationSpeed);
                    RemoteRightHandAnimator.SetLayerWeight(1, _syncRightThumbStart);

                    // Default to grip if holding an item
                    if (_syncRightHoldingItem) {
                        RemoteRightHandAnimator.SetLayerWeight(0, 0);
                        RemoteRightHandAnimator.SetFloat("Flex", 1);
                        RemoteRightHandAnimator.SetLayerWeight(1, 0);
                        RemoteRightHandAnimator.SetLayerWeight(2, 0);
                    }
                    else {
                        RemoteRightHandAnimator.SetInteger("Pose", 0);
                    }
                }
            }
            else {
                if(!disabledObjects) {
                    toggleObjects(false);
                }
            }
        }

        public void AssignPlayerObjects() {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            PlayerHeadTransform = getChildTransformByName(player.transform, "CenterEyeAnchor");

            // Using an explicit Transform name to make sure we grab the right one in the scene
            PlayerLeftHandTransform = GameObject.Find("ModelsLeft").transform;
            LeftHandController = PlayerLeftHandTransform.parent.GetComponentInChildren<HandController>();
           
            PlayerRightHandTransform = GameObject.Find("ModelsRight").transform;
            RightHandController = PlayerRightHandTransform.parent.GetComponentInChildren<HandController>();
        }

        Transform getChildTransformByName(Transform search, string name) {
            Transform[] children = search.GetComponentsInChildren<Transform>();
            for (int x = 0; x < children.Length; x++) {
                Transform child = children[x];
                if (child.name == name) {
                    return child;
                }
            }

            return null;
        }

        void toggleObjects(bool enableObjects) {
            RemoteHeadTransform.gameObject.SetActive(enableObjects);
            RemoteLeftHandTransform.gameObject.SetActive(enableObjects);
            RemoteRightHandTransform.gameObject.SetActive(enableObjects);
            disabledObjects = !enableObjects;
        }
        
        void checkGrabbablesTransfer() {

            // Cap the request period
            if (PhotonNetwork.Time - lastRequestTime < requestInterval) {
                return;
            }           
           
            requestOwnerShipForNearbyGrabbables(gitLeft);
            requestOwnerShipForNearbyGrabbables(gitRight);
        }

        void requestOwnerShipForNearbyGrabbables(GrabbablesInTrigger grabbables) {

            if(grabbables == null) {
                return;
            }

            // In Hand
            foreach (var grab in grabbables.NearbyGrabbables) {
                PhotonView view = grab.Value.GetComponent<PhotonView>();

                if (view != null && RecentlyRequested(view) == false && !view.AmOwner) {
                    RequestGrabbableOwnership(view);
                }
            }

            // Remote Grabbables
            foreach (var grab in grabbables.ValidRemoteGrabbables) {
                PhotonView view = grab.Value.GetComponent<PhotonView>();

                if (view != null && RecentlyRequested(view) == false && !view.AmOwner) {
                    RequestGrabbableOwnership(view);
                }
            }
        }

        public virtual bool RecentlyRequested(PhotonView view) {
            // Previously requested if in list and requested less than 3 seconds ago
            return requestedGrabbables != null && requestedGrabbables.ContainsKey(view.ViewID) && PhotonNetwork.Time - requestedGrabbables[view.ViewID] < 3f;
        }

        public virtual void RequestGrabbableOwnership(PhotonView view) {

            lastRequestTime = PhotonNetwork.Time;

            if (requestedGrabbables.ContainsKey(view.ViewID)) {
                requestedGrabbables[view.ViewID] = lastRequestTime;
            }
            else {
                requestedGrabbables.Add(view.ViewID, lastRequestTime);
            }

            view.RequestOwnership();
        }

        void updateRemotePositionRotation(Transform moveTransform, Vector3 startPosition, Vector3 endPosition, Quaternion syncStartRotation, Quaternion syncEndRotation, float syncValue) {
            float dist = Vector3.Distance(startPosition, endPosition);

            // If far away just teleport there
            if (dist > 0.5f) {
                moveTransform.position = endPosition;
                moveTransform.rotation = syncEndRotation;
            }
            else {
                moveTransform.position = Vector3.Lerp(startPosition, endPosition, syncValue);
                moveTransform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncValue);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            // This is our player, send our positions to the other players
            if (stream.IsWriting) {

                // Player Head / Hand Information
                stream.SendNext(PlayerHeadTransform.position);
                stream.SendNext(PlayerHeadTransform.rotation);
                stream.SendNext(PlayerLeftHandTransform.position);
                stream.SendNext(PlayerLeftHandTransform.rotation);
                stream.SendNext(PlayerRightHandTransform.position);
                stream.SendNext(PlayerRightHandTransform.rotation);

                // Hand Animator Info
                if(LeftHandController) {
                    stream.SendNext(LeftHandController.GripAmount);
                    stream.SendNext(LeftHandController.PointAmount);
                    stream.SendNext(LeftHandController.ThumbAmount);
                    stream.SendNext(LeftHandController.PoseId);
                    stream.SendNext(LeftHandController.grabber.HoldingItem);
                }
                if (RightHandController) {
                    stream.SendNext(RightHandController.GripAmount);
                    stream.SendNext(RightHandController.PointAmount);
                    stream.SendNext(RightHandController.ThumbAmount);
                    stream.SendNext(RightHandController.PoseId);
                    stream.SendNext(RightHandController.grabber.HoldingItem);
                }
            }
            else {
                // Remote player, receive data
                // Head
                this._syncHeadStartPosition = RemoteHeadTransform.position;
                this._syncHeadEndPosition = (Vector3)stream.ReceiveNext();
                this._syncHeadStartRotation = RemoteHeadTransform.rotation;
                this._syncHeadEndRotation = (Quaternion)stream.ReceiveNext();

                // Left Hand
                this._syncLHandStartPosition = RemoteLeftHandTransform.position;
                this._syncLHandEndPosition = (Vector3)stream.ReceiveNext();
                this._syncLHandStartRotation = RemoteLeftHandTransform.rotation;
                this._syncLHandEndRotation = (Quaternion)stream.ReceiveNext();

                // Right Hand
                this._syncRHandStartPosition = RemoteRightHandTransform.position;
                this._syncRHandEndPosition = (Vector3)stream.ReceiveNext();
                this._syncRHandStartRotation = RemoteRightHandTransform.rotation;
                this._syncRHandEndRotation = (Quaternion)stream.ReceiveNext();

                // Left Hand Animation Updates
                if(RemoteLeftHandAnimator) {
                    _syncLeftGripEnd = (float)stream.ReceiveNext();
                    _syncLeftPointEnd = (float)stream.ReceiveNext();
                    _syncLeftThumbEnd = (float)stream.ReceiveNext();

                    // Can set hand pose immediately
                    RemoteLeftHandAnimator.SetInteger("Pose", (int)stream.ReceiveNext());

                    _syncLeftHoldingItem = (bool)stream.ReceiveNext();
                }

                if (RemoteRightHandAnimator) {
                    _syncRightGripEnd = (float)stream.ReceiveNext();
                    _syncRightPointEnd = (float)stream.ReceiveNext();
                    _syncRightThumbEnd = (float)stream.ReceiveNext();

                    // Can set hand pose immediately
                    RemoteRightHandAnimator.SetInteger("Pose", (int)stream.ReceiveNext());

                    _syncRightHoldingItem = (bool)stream.ReceiveNext();
                }

                _syncTime = 0f;
                _syncDelay = Time.time - _lastSynchronizationTime;
                _lastSynchronizationTime = Time.time;
            }
        }

        // Handle Ownership Requests (Ex: Grabbable Ownership)
        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) {

            bool amOwner = targetView.AmOwner || (targetView.Owner == null && PhotonNetwork.IsMasterClient);

            NetworkedGrabbable netGrabbable = targetView.gameObject.GetComponent<NetworkedGrabbable>();
            if (netGrabbable != null) {
                // Authorize transfer of ownership if we're not holding it
                if (amOwner && !netGrabbable.BeingHeld) {
                    targetView.TransferOwnership(requestingPlayer.ActorNumber);
                    return;
                }
            }
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player requestingPlayer) {
            // Debug.Log("OnOwnershipTransfered to Player " + requestingPlayer);
        }
    }
}