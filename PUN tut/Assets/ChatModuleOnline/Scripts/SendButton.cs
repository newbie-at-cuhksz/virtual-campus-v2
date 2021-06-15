/*
 * This script should be attach to ChatInputField > SendButton
 * 
 * This "SendButton" refer to send world chat message button.
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
        public string BubbleText = "";                       // `ChatBubbleManager.cs` read text from here; it is also used in world chat (this var is like a buffer)
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
            BubbleText = ""; // this var is like a buffer

            // check the input field
            if (ChatInputField.text != "" && ChatInputField.text.Length > 0)
            {
                BubbleText = ChatInputField.text; // buffer the message

                if (IsBubbleChatMode)
                    ToSendBubble = true;

                if (IsWorldChatMode)
                    worldChatManager.WorldChatSend(BubbleText);
            }
            
            // clear the input field
            // The input field will always be cleared after a SEND button press,
            // although sometimes bubble chat will not be sent successfully.
            ChatInputField.text = "";
        }
    }
}
