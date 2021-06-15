/*
 * This script should be attached to ChatBubbleCanvas (in the player prefeb).
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


namespace ChatModuleOnline
{
    public class ChatBubbleCanvas : MonoBehaviour
    {
        private GameObject mainCamera;
        void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }
}
