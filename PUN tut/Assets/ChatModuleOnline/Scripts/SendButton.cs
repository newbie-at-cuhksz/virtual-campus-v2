/*
 * This script should be attach to ChatInputField > SendButton
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ChatModuleOnline
{
    public class SendButton : MonoBehaviour
    {
        public bool ToSendBubble = false;                    // This is a signal for `ChatBubbleManager.cs` to use
        public string BubbleText = "";                       // `ChatBubbleManager.cs` read text from here
        public ChatModeSwitchButton chatModeSwitchButton;    // set in inspector (used to chat the Chat Mode)
        public WorldChatManager worldChatManager;            // set in inspector
        public InputField ChatInputField;                    // set in inspector

        public bool IsWorldChatMode;                         // This is also a signal for `ChatBubbleManager.cs` to use
        private bool IsBubbleChatMode;


        private void Update()
        {
            // get current chat mode
            IsWorldChatMode = chatModeSwitchButton.IsWorldChatMode;
            IsBubbleChatMode = chatModeSwitchButton.IsBubbleChatMode;
        }


        public void OnClickSend() // This method is also used in `ChatInputField` > Input Field > On End Edit, so that we can use "Enter" key to send the message.
        {
            BubbleText = "";

            if (IsBubbleChatMode)
            {
                if (ChatInputField.text != "" && ChatInputField.text.Length > 0)
                {
                    BubbleText = ChatInputField.text;
                    ToSendBubble = true;
                }
            }

            if (IsWorldChatMode)
                worldChatManager.WorldChatSend( false ); // always clear the input field
            else
                ChatInputField.text = "";

            // The input field will always be cleared after a SEND button press,
            // although sometimes bubble chat will not be sent successfully.
        }
    }
}
