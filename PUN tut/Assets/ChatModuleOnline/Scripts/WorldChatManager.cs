/*
 * This script should be attached to `Chat Pannel` as a component.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif


public class WorldChatManager : MonoBehaviour, IChatClientListener
{
    public int HistoryLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context

    public string UserName { get; set; }

    private string selectedChannelName = "World"; // Note that in this `WorldChatManager` class, there is only one Channel, named "World"

    public ChatClient chatClient;

    #if !PHOTON_UNITY_NETWORKING
    [SerializeField]
    #endif
    protected internal ChatAppSettings chatAppSettings;

    public InputField InputFieldChat;   // set in inspector
    public Text CurrentChannelText;     // set in inspector


    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        // `UserName` should be set when the player join the game!!!!!! Wait for the API!!!
        if (string.IsNullOrEmpty(this.UserName))
        {
            this.UserName = "user" + Environment.TickCount % 99; //made-up username
        }

        #if PHOTON_UNITY_NETWORKING
        this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        #endif


        // code in `Connect()` in the demo script
        this.chatClient = new ChatClient(this);
        #if !UNITY_WEBGL
        this.chatClient.UseBackgroundWorkerForSending = true;
        #endif
        this.chatClient.AuthValues = new AuthenticationValues(this.UserName);
        this.chatClient.ConnectUsingSettings(this.chatAppSettings);
    }


    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }


    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }


    public void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
    }


    //public void OnEnterSend()
    //{
    //    if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
    //    {
    //        this.SendChatMessage(this.InputFieldChat.text);
    //        this.InputFieldChat.text = "";
    //    }
    //}


    //public void OnClickSend()
    //{
    //    if (this.InputFieldChat != null)
    //    {
    //        this.SendChatMessage(this.InputFieldChat.text);
    //        this.InputFieldChat.text = "";
    //    }
    //}

    public void WorldChatSend(bool IsBubbleChatMode)
    {
        if (this.InputFieldChat != null)
        {
            this.SendChatMessage(this.InputFieldChat.text);
            
            if (!IsBubbleChatMode) // if we do not need to send a bubble chat, clear the InputFieldChat (see `SendButton.cs` => OnClickSend() method to see the reason why we need this if-statement )
            {
                this.InputFieldChat.text = "";
            }
        }
    }


    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }


        // Send public (world) message
        // Private chat feature will be add later
        this.chatClient.PublishMessage(this.selectedChannelName, inputLine);
    }


    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnConnected()
    {
        this.chatClient.Subscribe(this.selectedChannelName, this.HistoryLengthToFetch); // Subscribe the channel "World"

        this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    }


    public void OnDisconnected()
    {
        // Do nothing
    }


    public void OnChatStateChange(ChatState state)
    {
        // Do nothing.
    }


    public void OnSubscribed(string[] channels, bool[] results)
    {
        // Do nothing.
    }


    public void OnUnsubscribed(string[] channels)
    {
        // Do nothing.
    }


    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(this.selectedChannelName))
        {
            // update text
            this.ShowChannel(this.selectedChannelName);
        }
    }


    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // Do nothing at this stage.
        // Add codes when the private chat feature is added!!!!
    }


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        // Do nothing.
    }


    public void OnUserSubscribed(string channel, string user)
    {
        // Do nothing.
    }


    public void OnUserUnsubscribed(string channel, string user)
    {
        // Do nothing.
    }


    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        //this.selectedChannelName = channelName;
        this.CurrentChannelText.text = channel.ToStringMessages();
    }


    #region UI control

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}
