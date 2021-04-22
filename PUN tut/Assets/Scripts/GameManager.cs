using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields
        public static GameManager Instance;
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        [Tooltip("The instance of info panel")]
        public GameObject InfoPanel;

        public GameObject ChatPanel;

        #endregion


        #region Private Field
        private GameObject playerInstance;

        #endregion

        #region Unity callbacks
        void Start()
        {
            InfoPanel.SetActive(false);
            ChatPanel.SetActive(false);

            Instance = this;
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    playerInstance=PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    // Initialize player instance using scene infomation
                    playerInstance.GetComponent<InfoPanelOnClick>().InfoPanel = this.InfoPanel;
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        private void Update()
        {
            // 鼠标左键按下, 若未点到物体则消除info panel
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray,out hit))
                {
                    if(hit.collider.gameObject.GetComponent<HighlightableObject>()==null)
                    {
                        InfoPanel.SetActive(false);
                    }
                }
                else
                {
                    InfoPanel.SetActive(false);
                }
            }
        }


        #endregion

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion

        #region Private Methods


        void LoadArena()
        {
            //if (!PhotonNetwork.IsMasterClient)
            //{
            //    Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            //}
            //Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }


        #endregion
    }
}
