// this script should be attached to the player to control the player to set the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerSetBoard : MonoBehaviour
    {
        public PhotonView PV;
        public void placeBoard(string input)
        {
            Debug.Log("the board should be placed");
            Debug.Log("ismine is " + PV.IsMine);
            Debug.Log("the position of the player is: " + PlayerManager.LocalPlayerInstance.transform.position);
            Debug.Log("the input is: " + input);
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            object[] instanceData = new object[1];
            instanceData[0] = input;
            PhotonNetwork.Instantiate("Board1", PlayerManager.LocalPlayerInstance.transform.position + rotation * (new Vector3(0f, 0f, 2f)), rotation, 0, instanceData);
            //BoardSetted.gameObject.GetComponent<BoardContent>().BoardText = input;
            Debug.Log("the Board text is:" + instanceData[0]);
            Debug.Log("the board should be set");
        }
    }
}