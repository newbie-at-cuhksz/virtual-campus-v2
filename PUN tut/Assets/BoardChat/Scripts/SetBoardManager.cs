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

        
    }

}