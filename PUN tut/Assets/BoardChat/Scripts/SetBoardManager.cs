// this script is attached to the setBoardButton in the upper right of the UI.
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

        public void placeBoard(string input)
        {
            Debug.Log("the input is: " + input);
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            BoardSetted = PhotonNetwork.InstantiateRoomObject("Board1", PlayerManager.LocalPlayerInstance.transform.position + rotation * (new Vector3(0f, 0f, 2f)), rotation);
            BoardSetted.gameObject.GetComponent<BoardContent>().BoardText = input;
            Debug.Log("the Board text is:" + BoardSetted.gameObject.GetComponent<BoardContent>().BoardText);
        }
    }

}