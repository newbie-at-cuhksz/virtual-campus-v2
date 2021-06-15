/*
 * This script should be attached to "Private Chat Send Button"
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ChatModuleOnline
{
    public class PrivateChatSendButton : MonoBehaviour
    {
        [SerializeField]
        private InputField privateChatInputField;               // set in inspector
        private WorldChatManager worldChatManager;

        public string privateChatTarget = "";


        private void Start()
        {
            worldChatManager = GameObject.Find("WorldChatManagerScript").GetComponent<WorldChatManager>();
        }


        public void OnClickSendPrivateChat()
        {
            if (privateChatTarget != "" && privateChatInputField != null && privateChatInputField.text != "")
            {
                bool sendPrivateSuccess = worldChatManager.PrivateChatSend(privateChatTarget, privateChatInputField.text);

                // clear the privateChatInputField, if private message sent successfully
                if (sendPrivateSuccess)
                    privateChatInputField.text = "";
            }
        }


        public void UpdatePrivateChatChannel()
        {
            if (privateChatTarget != "")
                worldChatManager.ShowPrivateChatChannel(privateChatTarget);
        }
    }
}