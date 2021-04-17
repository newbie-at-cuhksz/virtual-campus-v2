/*
 * This script should be add to `Chat Panel` > ChatModeSwitchButton.
 * 
 * There are in total 3 chat mode:
 *   1. All (World + Bubble)
 *   2. Bubble
 *   3. World
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatModeSwitchButton : MonoBehaviour
{
    public bool IsWorldChatMode = true;
    public bool IsBubbleChatMode = true;

    private string chatMode = "All";
    private Text buttonText;


    private void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        buttonText.text = chatMode;
    }


    public void SwitchChatMode()
    {
        // All ==> Bubble ==> World ==> All ==> ...

        if (IsWorldChatMode == true && IsBubbleChatMode == true) // currently in "All" mode
        {
            // switch to "Bubble" mode
            chatMode = "Bubble";
            buttonText.text = chatMode;
            IsWorldChatMode = false;
            IsBubbleChatMode = true;
        } 
        else if (IsWorldChatMode == false && IsBubbleChatMode == true) // currently in "Bubble" mode
        {
            // switch to "World" mode
            chatMode = "World";
            buttonText.text = chatMode;
            IsWorldChatMode = true;
            IsBubbleChatMode = false;
        }
        else if (IsWorldChatMode == true && IsBubbleChatMode == false) // currently in "World" mode
        {
            // switch to "All" mode
            chatMode = "All";
            buttonText.text = chatMode;
            IsWorldChatMode = true;
            IsBubbleChatMode = true;
        }
    }
}
