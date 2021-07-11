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
        [Tooltip("放入info panel的GameObject")]
        private GameObject InfoPanel;
        [Tooltip("放入panel的prefab，用于在info panel上显示指定panel")]
        public GameObject ChildPanelPrefab;
        #endregion

        #region Private Fields
        static private GameObject _CurrentGameObject;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            _CurrentGameObject = null;
            InfoPanel = SetBoardManager.info_panel;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseDown()
        {
            
            InfoPanel.SetActive(true);
            if (_CurrentGameObject != gameObject)
            {
                _CurrentGameObject = gameObject;
                // destroy child object
                for (int i = 0; i < InfoPanel.transform.childCount; i++)
                {
                    Destroy(InfoPanel.transform.GetChild(i).gameObject);
                }
                // 实例化child object
                GameObject ChildPanel = Instantiate(ChildPanelPrefab) as GameObject; // instantiate the panel which holds the board content
                ChildPanel.transform.parent = InfoPanel.transform;
                PhotonView PV = transform.GetComponent<PhotonView>(); // get the photon view of the board
                object[] data = PV.InstantiationData;
                
                // display the board content on the UI
                Debug.Log("onclick, the text is: "+ (string)data[2]); // access the data stored in this board
                ChildPanel.transform.Find("Text").GetComponent<Text>().text = (string)data[2];
            }


            // update panel infomation
        }
    }
}

