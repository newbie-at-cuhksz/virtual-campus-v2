using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class Web_BoardOnClick : MonoBehaviour
    {
        //#region Private Fields
        //static private GameObject _CurrentGameObject;
        //public static PhotonView BoardPhotonView;
        //public static Web_BoardOnClick instance;
        //#endregion

        // Start is called before the first frame update
        void Start()
        {
            //instance = this;
            //_CurrentGameObject = null;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseDown()
        {
            Debug.Log("the LGU board is clicked");
            App_Webview.instance.NewWebView("https://www.lgulife.com/");
        } 
    }
}