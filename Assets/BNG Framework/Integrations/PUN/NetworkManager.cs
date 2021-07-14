using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNG {
    public class NetworkManager : MonoBehaviourPunCallbacks {

        /// <summary>
        /// Maximum number of players per room. If the room is full, a new radom one will be created.
        /// </summary>
        [Tooltip("Maximum number of players per room. If the room is full, a new random one will be created. 0 = No Max.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 0;

        [Tooltip("If true, the JoinRoomName will try to be Joined On Start. If false, need to call JoinRoom yourself.")]
        public bool JoinRoomOnStart = true;

        [Tooltip("If true, do not destroy this object when moving to another scene")]
        public bool dontDestroyOnLoad = true;

        public string JoinRoomName = "RandomRoom";

        [Tooltip("Game Version can be used to separate rooms.")]
        public string GameVersion = "1";

        [Tooltip("Name of the Player object to spawn. Must be in a /Resources folder.")]
        public string RemotePlayerObjectName = "RemotePlayer";

        [Tooltip("Optional GUI Text element to output debug information.")]
        public Text DebugText;

        ScreenFader sf;

        void Awake() {
            // Required if you want to call PhotonNetwork.LoadLevel() 
            PhotonNetwork.AutomaticallySyncScene = true;

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(this.gameObject);
            }

            if(Camera.main != null) {
                sf = Camera.main.GetComponentInChildren<ScreenFader>(true);
            }
        }

        void Start() {
            // Connect to Random Room if Connected to Photon Server
            if (PhotonNetwork.IsConnected) {
                if (JoinRoomOnStart) {
                    LogText("Joining Room : " + JoinRoomName);
                    PhotonNetwork.JoinRoom(JoinRoomName);
                }
            }
            // Otherwise establish a new connection. We can then connect via OnConnectedToMaster
            else {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = GameVersion;
            }
        }

        void Update() {
            // Show Loading Progress
            if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1) {
                Debug.Log(PhotonNetwork.LevelLoadingProgress);
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            LogText("Room does not exist. Creating <color=yellow>" + JoinRoomName + "</color>");
            PhotonNetwork.CreateRoom(JoinRoomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, TypedLobby.Default);
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            Debug.Log("OnJoinRandomFailed Failed, Error : " + message);
        }

        public override void OnConnectedToMaster() {

            LogText("Connected to Master Server. \n");

            if (JoinRoomOnStart) {
                LogText("Joining Room : <color=aqua>" + JoinRoomName + "</color>");
                PhotonNetwork.JoinRoom(JoinRoomName);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            base.OnPlayerEnteredRoom(newPlayer);

            float playerCount = PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount : 0;

            LogText("Connected players : " + playerCount);           
        }

        public override void OnJoinedRoom() {

            LogText("Joined Room. Creating Remote Player Representation.");

            // Network Instantiate the object used to represent our player. This will have a View on it and represent the player         
            GameObject player = PhotonNetwork.Instantiate(RemotePlayerObjectName, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            NetworkPlayer np = player.GetComponent<NetworkPlayer>();
            if (np) {
                np.transform.name = "MyRemotePlayer";
                np.AssignPlayerObjects();
            }
        }

        public override void OnDisconnected(DisconnectCause cause) {
            LogText("Disconnected from PUN due to cause : " + cause);

            if (!PhotonNetwork.ReconnectAndRejoin()) {
                LogText("Reconnect and Joined.");
            }

            base.OnDisconnected(cause);
        }

        public void LoadScene(string sceneName) {
            // Fade Screen out
            StartCoroutine(doLoadLevelWithFade(sceneName));
        }

        IEnumerator doLoadLevelWithFade(string sceneName) {

            if (sf) {
                sf.DoFadeIn();
                yield return new WaitForSeconds(sf.SceneFadeInDelay);
            }

            PhotonNetwork.LoadLevel(sceneName);

            yield return null;
        }

        void LogText(string message) {

            // Output to worldspace to help with debugging.
            if (DebugText) {
                DebugText.text += "\n" + message;
            }

            Debug.Log(message);
        }
    }
}

