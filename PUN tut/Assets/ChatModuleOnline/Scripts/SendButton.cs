/*
 * This script should be attach to ChatInputField > SendButton
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendButton : MonoBehaviour
{
    public bool ToSendBubble = false;                    // This is a signal for `ChatBubbleManager.cs` to use
    public ChatModeSwitchButton chatModeSwitchButton;    // set in inspector (used to chat the Chat Mode)
    public WorldChatManager worldChatManager;            // set in inspector

    private bool IsWorldChatMode;
    private bool IsBubbleChatMode;


    private void Update()
    {
        // get current chat mode
        IsWorldChatMode = chatModeSwitchButton.IsWorldChatMode;
        IsBubbleChatMode = chatModeSwitchButton.IsBubbleChatMode;
    }


    public void OnClickSend() // This method is also used in `ChatInputField` > Input Field > On End Edit, so that we can use "Enter" key to send the message.
    {
        if (IsWorldChatMode)
        {
            worldChatManager.WorldChatSend(IsBubbleChatMode);
        }

        if (IsBubbleChatMode)
        {
            if (!ToSendBubble)
            {
                ToSendBubble = true;

                StartCoroutine("DisableToSendBubble");
            }
        }
    }

    IEnumerator DisableToSendBubble()
    {
        yield return new WaitForSeconds(0.1f);
        ToSendBubble = false;
    }
}
