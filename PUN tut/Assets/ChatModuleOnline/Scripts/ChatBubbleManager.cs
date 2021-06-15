/*
 * This script should be attach to the player prefeb.
 * And it should be add to the PhotonView component (of the player prefeb) > Observed Components.
 * 
 * Remember to set the 3 public variables of this script in Unity Editor.
 */


/*
 * Tutorial Link: https://www.youtube.com/watch?v=7cOtVzjy-6o
 * 
 * Tutorial Link 2: https://www.youtube.com/watch?v=-7DeiwuMRFw&list=PLWeGoBm1YHVjY3vZ2qMFb95rAmyiVtBdz&index=2
 * 
 * Buy Public Chat Demo: https://www.infogamerhub.com/photon-chat-lesson-1/
 * 
 * PUN2 Rename: https://doc.photonengine.com/en-us/pun/v2/getting-started/migration-notes
 * `PhotonTargets enum` is now `Photon.Pun.RpcTarget`
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


//namespace Com.MyCompany.MyGame // who add this??
namespace ChatModuleOnline
{
    public class ChatBubbleManager : MonoBehaviour, IPunObservable
    {
        //public Player plMove;
        public PhotonView photonView;
        public GameObject BubbleSpeechObject;
        public Text UpdatedText;

        private SendButton sendButton;
        private bool ableToShowBubble = true;


        private void Awake()
        {
            //Inorder to find gameobject which is not active, use tranform.find instead of GameObject.find
            GameObject mainCanvas = GameObject.Find("Canvas");

            sendButton = mainCanvas.transform.Find("Chat Panel").Find("SendButton").GetComponent<SendButton>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (sendButton.ToSendBubble)
                {
                    sendButton.ToSendBubble = false;

                    string tmpbubbleText = sendButton.BubbleText;
                    if (tmpbubbleText != "" && tmpbubbleText.Length > 0 && ableToShowBubble)
                    {
                        ableToShowBubble = false;

                        photonView.RPC("RPCSendMessage", Photon.Pun.RpcTarget.AllBuffered, tmpbubbleText);
                        BubbleSpeechObject.SetActive(true);
                    }
                }
            }
        }


        [PunRPC]
        private void RPCSendMessage(string message)
        {
            UpdatedText.text = message;

            StartCoroutine("Remove");
        }


        IEnumerator Remove()
        {
            yield return new WaitForSeconds(4f);
            BubbleSpeechObject.SetActive(false);
            ableToShowBubble = true;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(BubbleSpeechObject.activeSelf);
            }
            else if (stream.IsReading)
            {
                BubbleSpeechObject.SetActive((bool)stream.ReceiveNext());
            }
        }
    }
}
