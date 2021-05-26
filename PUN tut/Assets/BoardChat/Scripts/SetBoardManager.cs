// this script is attached to the setBoardController
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{ 
    public class SetBoardManager : MonoBehaviour
    {

        #region private field
        [SerializeField]
        private GameObject boardPrefab;
        //[SerializeField]
        //private Vector3 CustomOffset;
        #endregion

        #region Public field
        public GameObject infoPanelInstance;
        public static GameObject info_panel;
        public static GameObject BoardSetted;

        #endregion
        // Start is called before the first frame update
        void Start()
        {
            SetBoardManager.info_panel = this.infoPanelInstance;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /*public void placeBoard(string input)
        {
            Debug.Log("the input is: " + input);
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            object [] instanceData = new object[1];
            instanceData[0] = input;
            BoardSetted = PhotonNetwork.InstantiateRoomObject("Board1", PlayerManager.LocalPlayerInstance.transform.position + rotation * (new Vector3(0f, 0f, 2f)), rotation, 0, instanceData);
            //BoardSetted.gameObject.GetComponent<BoardContent>().BoardText = input;
            Debug.Log("the Board text is:" + instanceData[0]);
            Debug.Log("the board should be set");
        }*/
    }

}