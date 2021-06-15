/*
 * This script should be attached to `WorldChatManagerScript`, which is an empty gameobject or script-holder.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
//using System.Collections.Generic;

//using UnityEngine;
using UnityEngine.UI;

using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

namespace ChatModuleOnline
{
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

        //public InputField InputFieldChat;     // set in inspector (for world chat)
        public Text CurrentChannelText;         // set in inspector (for world chat)
        public Text CurrentPrivateChannelText;  // set in inspector (for private chat)

        public PrivateChatSendButton privateChatSendButton;   // set in inspector (for private chat)


        public void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            // `UserName` should be set when the player join the game
            //this.UserName = Com.MyCompany.MyGame.PlayerManager.LocalPlayerInstance.GetComponent<PhotonView>().Owner.NickName; // dependence: `Com.MyCompany.MyGame.PlayerManager.LocalPlayerInstance`
            this.UserName = PlayerPrefs.GetString("PlayerName"); // later `this.UserName` should be set by getting the player's nickname (and UID if needed) from database
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


        /// <summary> Send out message to the channel named "World"
        public void WorldChatSend(string message)
        {
            if (message != "" && message.Length > 0)
            {
                // Send public (world) message
                this.chatClient.PublishMessage(this.selectedChannelName, message);
            }
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
            if (channelName.Equals(this.selectedChannelName)) // if the channel "World" receive new message
            {
                // update text
                this.ShowWorldChannel(this.selectedChannelName);
            }
        }


        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            if (channelName.Equals(this.chatClient.GetPrivateChannelNameByUser(privateChatSendButton.privateChatTarget)))
                privateChatSendButton.UpdatePrivateChatChannel();
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


        public void ShowWorldChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                return;
            }

            ChatChannel channel = null;
            bool found = this.chatClient.TryGetChannel(channelName, out channel);
            if (!found)
            {
                Debug.Log("ShowWorldChannel failed to find channel: " + channelName);
                return;
            }

            //this.selectedChannelName = channelName;
            this.CurrentChannelText.text = channel.ToStringMessages(); // update text 
        }


        // the following methods are for private chat

        /// <summary> send private `message` to `privateChatTarget`, return true if send successfully
        public bool PrivateChatSend(string privateChatTarget, string message)
        {
            bool sendPrivateSuccess = false;
            if (!string.IsNullOrEmpty(message))
            {
                sendPrivateSuccess = this.chatClient.SendPrivateMessage(privateChatTarget, message);

                if (!sendPrivateSuccess)
                    Debug.Log("WorldChatManager.PrivateChatSend(): fail to send private message");
            }
            return sendPrivateSuccess;
        }


        public void ShowPrivateChatChannel(string privateChatTarget)
        {
            string currentPrivateChannelName = this.chatClient.GetPrivateChannelNameByUser(privateChatTarget);

            if (string.IsNullOrEmpty(currentPrivateChannelName))
            {
                return;
            }

            ChatChannel channel = null;
            bool found = this.chatClient.TryGetChannel(currentPrivateChannelName, out channel);
            if (!found)
            {
                Debug.Log("ShowPrivateChatChannel failed to find channel: " + currentPrivateChannelName + "\n(It is OK is see this message if it is the first time to try to chat with this friend.)");
                this.CurrentPrivateChannelText.text = "No messages. \nStart to chat now!";
                return;
            }

            this.CurrentPrivateChannelText.text = channel.ToStringMessages();
        }
    }
}
