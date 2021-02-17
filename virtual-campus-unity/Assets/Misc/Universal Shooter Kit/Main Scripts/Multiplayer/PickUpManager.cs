using UnityEngine;
#if PHOTON_UNITY_NETWORKING
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
#endif


namespace GercStudio.USK.Scripts
{
    public class PickUpManager :
#if PHOTON_UNITY_NETWORKING
        MonoBehaviourPun
#else
    MonoBehaviour
#endif
    {
#if PHOTON_UNITY_NETWORKING

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }
        
        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }
        
        public void PickUp(int playerID, string pickUpId)
        {
            RaiseEventOptions options = new RaiseEventOptions()
            {
                CachingOption = EventCaching.AddToRoomCacheGlobal,
                Receivers = ReceiverGroup.Others
            };
            object[] content =
            {
                playerID,
                pickUpId
            };

            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.PickUp, content, options, SendOptions.SendReliable);
        }

        void OnEvent(EventData photonEvent)
        {
            PhotonEventCodes eventCode = (PhotonEventCodes) photonEvent.Code;
            object[] data = photonEvent.CustomData as object[];
            if (data != null)
                if (eventCode == PhotonEventCodes.PickUp)
                {
                    if (data.Length == 2)
                    {
                        var foundObjects = FindObjectsOfType<PickUp>();
                        var players = FindObjectsOfType<Controller>();
                        var picUpId = (string) data[1];
                        var playerId = (int) data[0];
                        GameObject curPlayer = null;
                        var hasPlayer = false;
                        foreach (var player in players)
                        {
                            if (!player.GetComponent<PhotonView>()) continue;

                            if (player.GetComponent<PhotonView>().ViewID == playerId)
                            {
                                curPlayer = player.gameObject;
                                hasPlayer = true;
                            }
                        }

                        foreach (var obj in foundObjects)
                        {
                            if (obj.pickUpId == picUpId)
                            {
                                if (hasPlayer)
                                    obj.PickUpObject(curPlayer);
                                else
                                {
                                    Destroy(obj.gameObject);
                                }
                            }
                        }
                    }
                }
        }
#endif
    }
}

