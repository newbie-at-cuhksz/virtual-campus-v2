/*
 * This script should be attached to "Friend List Item" prefab, the one to be instantiated by 好友系统
 * 
 * This component/script maintains the following:
 *   - NickName of this friend item
 *   - Uid of this friend item (should be added later)
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ChatModuleOnline
{
    public class FriendItemButton : MonoBehaviour
    {
        public string NickName { get; set; }
        // public string Uid { get; set; }     // the uid field should be add later

        [SerializeField]
        private Text NickNameUI;               // set in inspector (edit "Friend List Item" prefab)

        private WorldChatManager worldChatManager;
        private PrivateChatSendButton privateChatSendButton;


        private void Start()
        {
            worldChatManager = GameObject.Find("WorldChatManagerScript").GetComponent<WorldChatManager>();

            // find privateChatSendButton
            GameObject mainCanvas = GameObject.Find("Canvas");
            privateChatSendButton = mainCanvas.transform.Find("Friend Panel").Find("Private Chat Panel").Find("Private Chat Send Button").GetComponent<PrivateChatSendButton>();
        }

        public void UpdateNickNameUI()
        {
            NickNameUI.text = NickName;
        }


        /// <summary>
        /// When the friend item button is clicked, 
        ///   1. update privateChatTarget
        ///   2. show corresponding private chat channel
        /// </summary>
        public void OnClickShowPrivateChatChannel()
        {
            // 1. update privateChatTarget
            privateChatSendButton.privateChatTarget = NickName;         // player's uid may be used later

            // 2. show corresponding private chat channel
            privateChatSendButton.UpdatePrivateChatChannel();
        }

    }
}
