// this script should be attached to the 'setBoardController' to control the player to set the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerSetBoard : MonoBehaviourPun
    {

        /// <summary>
        /// to transfer the ownership to set the board as a room object
        /// </summary>
        private void onSetBoard()
        {
            base.photonView.RequestOwnership();
        }

        
        public void placeBoard(string input)
        {
            Debug.Log("the photon ID of the current local player is:" + PhotonNetwork.LocalPlayer.ActorNumber);
            Debug.Log("the master client of the room is: " + PhotonNetwork.NetworkingClient.CurrentRoom.MasterClientId);
            
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            Vector3 position = PlayerManager.LocalPlayerInstance.transform.position;

            object[] instanceData = new object[1];
            instanceData[0] = input;
            Debug.Log("check, and now the isMasterClient is" + PhotonNetwork.IsMasterClient);
            if (!(PhotonNetwork.IsMasterClient)) 
            {
                Debug.Log("the transfer is successful?: " + PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer));
                Debug.Log("change, and the local player now is: " + PhotonNetwork.LocalPlayer.ActorNumber);
                Debug.Log("change, and the master client now is: " + PhotonNetwork.NetworkingClient.CurrentRoom.MasterClientId);
            } 
            
            PhotonNetwork.InstantiateRoomObject("Board1", position + rotation * (new Vector3(0f, 0f, 2f)), rotation, 0, instanceData);

            Debug.Log("the Board text is:" + instanceData[0]);
            Debug.Log("the board should be set");
        }
        
    }
}