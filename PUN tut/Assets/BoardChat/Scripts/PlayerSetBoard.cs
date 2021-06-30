// this script should be attached to the 'setBoardController' to control the player to set the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class PlayerSetBoard : MonoBehaviourPun
    {
        private System.DateTime utcDate;
        private void Start()
        {

        }


        public void placeBoard(string input)
        {
            
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            Vector3 position = PlayerManager.LocalPlayerInstance.transform.position;
            utcDate = System.DateTime.UtcNow;

            object[] instanceData = new object[2];
            instanceData[0] = input;
            int[] dateInfo = {utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute};
            instanceData[1] = dateInfo;
            if (!(PhotonNetwork.IsMasterClient)) 
            {
                photonView.RPC("masterClientInstantiate", RpcTarget.MasterClient, position, rotation, instanceData);
            } else 
                PhotonNetwork.InstantiateRoomObject("Board1", position + rotation * (new Vector3(0f, 0f, 2f)), rotation, 0, instanceData);

            // below is for set
            //Debug.Log("the Board text is:" + instanceData[0]);
            //Debug.Log("the set board time is: " + ((int[])instanceData[1])[3]);
            //Debug.Log("the board should be set");
        }

        [PunRPC]
        void masterClientInstantiate(Vector3 pos, Quaternion rot, object[] pass_data)
        {
            PhotonNetwork.InstantiateRoomObject("Board1", pos + rot * (new Vector3(0f, 0f, 2f)), rot, 0, pass_data);
        }
        
    }
}