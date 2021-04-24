using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{

    public class SetBoardManager : MonoBehaviour
    {

        #region private field
        [SerializeField]
        private GameObject boardPrefab;
        //[SerializeField]
        //private Vector3 CustomOffset;

        public GameObject infoPanelInstance;
        public static GameObject info_panel;

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

        public void placeBoard()
        {
            Quaternion rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            GameObject BoardInstance = Instantiate(boardPrefab, PlayerManager.LocalPlayerInstance.transform.position + rotation * (new Vector3(0f, 0f, 2f)), rotation);
        }
    }

}