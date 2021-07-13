// this script should be attached to the Board gameobject (prefabs)
// the script invokes the UI of board content when the player click on the board
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class InfoPanelOnClick_Board : MonoBehaviour
    {
        #region Public Fields
        //[Tooltip("放入info panel的GameObject")]
        //private GameObject InfoPanel;
        //[Tooltip("放入panel的prefab，用于在info panel上显示指定panel")]
        //public GameObject ChildPanelPrefab;
        #endregion

        #region Private Fields
        static private GameObject _CurrentGameObject;
        public static PhotonView BoardPhotonView;
        public static InfoPanelOnClick_Board instance;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            _CurrentGameObject = null;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseDown()
        {

            /*InfoPanel.SetActive(true);
            if (_CurrentGameObject != gameObject)
            {
                _CurrentGameObject = gameObject;
                // destroy child object
                for (int i = 0; i < InfoPanel.transform.childCount; i++)
                {
                    Destroy(InfoPanel.transform.GetChild(i).gameObject);
                }}*/
            Debug.Log("the board is clicked");
            BoardPhotonView = transform.GetComponent<PhotonView>();
            Debug.Log("the onclick should be triggered");
            EventManager_Board.instance.ClickBoard();


            // update panel infomation
        }
    }
}

