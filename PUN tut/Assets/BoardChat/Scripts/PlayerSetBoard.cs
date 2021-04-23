/*
 * 1. basic info
 * this script control the player to set the board
 * a button will show up to ask players to set board, the player can set the Board when clicking on the button
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerSetBoard : MonoBehaviourPun
    {
        #region Public Fields
        public GameObject setButton_Prefab; // the prefab of set button to instantiate
        public bool boardSetted; // keep track of if the board has been setted
        public static GameObject LocalPlayerInstance;
        #endregion

        #region Public Methods
        public void onClickTrigger()
        {
            boardSetted = true;
        }
        #endregion


        #region Private Methods

        private void Awake()
        {
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerSetBoard.LocalPlayerInstance = this.gameObject;
            }
        }

        //implement start and update
        private void Start()
        {
            //Instantiate(setButton_Prefab, transform); // intitiate the set board button
        }

        private void Update()
        {

        }
        #endregion

        #region SetBoard Method
        // invoke when the board is clicked
        public void onPlayerClickTrigger()
        {
            Debug.Log("debug for the setBoard in PlayerManager script is invoked");
            Quaternion rotation = LocalPlayerInstance.transform.rotation;
            Vector3 leap = rotation * (new Vector3(0f, 0f, 2f));
            PhotonNetwork.Instantiate("Board", LocalPlayerInstance.transform.position + leap, rotation, 0);
        }
        #endregion
    }
}

