using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if PHOTON_UNITY_NETWORKING
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif


namespace GercStudio.USK.Scripts
{
    public enum PhotonEventCodes
    {
        ChangeHealth = 0,
        ChangeWeapon = 1,
        PickUp = 2,
        Grenade = 3,
        DropWeapon = 4,
        Bullets = 5,
        Fire = 6,
        Rocket = 7,
        BulletHit = 8,
        ChangeCameraType = 9,
        ChangeEnemyHealth = 10,
        PlayerDeath = 11,
        Reload = 12,
        Aim = 13, 
        ChangeAttack = 14, 
        Crouch = 15,
        MeleeAttack = 16,
        UpdatePlayerList = 17,
        SetKillAssistants = 18,
        UpdateKillAssistants = 19,
        ChangeTDMode = 20, 
        CreateHitMark = 21
    }

    public class CharacterSync :
#if PHOTON_UNITY_NETWORKING
        MonoBehaviourPun, IPunObservable
#else
    MonoBehaviour
#endif
    {
#if PHOTON_UNITY_NETWORKING
        [HideInInspector]
        public float currentHealth;

        [HideInInspector]
        public List<string> KillersNames;
        
        private Controller controller;
        private InventoryManager weaponManager;
        private WeaponController weaponController;

        private RoomManager RoomManager;
        
        private GameObject grenade;
        
        private float camera_Distance;
        private float camera_Angle;

        private Vector3 camera_Direction;
        private Vector3 camera_NetworkPosition;
        private Vector3 camera_StoredPosition;

        private Quaternion camera_NetworkRotation;
        
        private Vector3 CameraPosition;
        private Vector3 CameraRotation;
        
        private float destroyTimeOut;

        private bool hasTimerStarted;
        private bool sendEvent = true;
        private bool firstTake = false;
    

        #region StartMethods
        
            private void OnEnable()
            {
                if(FindObjectOfType<Lobby>()) return;
                
                firstTake = true;
                PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
            }
        
            private void OnDisable()
            {
                PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            }
        
            private void Awake()
            {
                controller = GetComponent<Controller>();

            /*
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            foreach (Transform tran in GetComponentsInChildren<Transform>())
            {
                tran.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            */

                if (FindObjectOfType<Lobby>())
                {
                    if(controller.PlayerHealthBarBackground)
                        controller.PlayerHealthBarBackground.gameObject.SetActive(false);
				
                    if(controller.PlayerHealthBar)
                        controller.PlayerHealthBar.gameObject.SetActive(false);
                    
                    return;
                }
                
                weaponManager = GetComponent<InventoryManager>();
                
                RoomManager = FindObjectOfType<RoomManager>();

                controller.CharacterSync = this;
                
                controller.CameraController.BodyLookAt = new GameObject("BodyLookAt").transform;
                controller.CameraController.BodyLookAt.hideFlags = HideFlags.HideInHierarchy;

                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
                    controller.isMultiplayerCharacter = true;
                }
                else if (photonView.IsMine && PhotonNetwork.IsConnected)
                {

                }
            }

            void Start()
            {

                if(FindObjectOfType<Lobby>()) return;
                
                controller.MyTeam = (PUNHelper.Teams) photonView.Controller.CustomProperties["pt"];
                
                if(!(bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"]) controller.CanKillOthers = PUNHelper.CanKillOthers.NoOne;
                else controller.CanKillOthers = (PUNHelper.CanKillOthers) PhotonNetwork.CurrentRoom.CustomProperties["km"];

                controller.oneShotOneKill = (bool) PhotonNetwork.CurrentRoom.CustomProperties["oshok"];
                
                currentHealth = controller.PlayerHealth;
                controller.CharacterName = photonView.Owner.NickName;

                CharacterHelper.SetAnimatorViewComponents(GetComponent<PhotonAnimatorView>());

                if (!photonView.IsMine && PhotonNetwork.IsConnected)
                {
//                    if ((Application.isMobilePlatform || controller.projectSettings.mobileDebug) && controller.UIManager.UIButtonsMainObject)
//                        controller.UIManager.UIButtonsMainObject.SetActive(false);
                    
                    controller.thisCamera.SetActive(false);
                    
                    if (weaponManager.LeftHandCollider)
                        weaponManager.LeftHandCollider.enabled = false;

                    if (weaponManager.RightHandCollider)
                        weaponManager.RightHandCollider.enabled = false;
                    
                    Helper.ChangeLayersRecursively(gameObject.transform, "Default");

                    controller.CameraController.LayerCamera = Helper.NewCamera("LayerCamera", transform, "Sync").gameObject;
                    controller.CameraController.LayerCamera.SetActive(false);
                    
                    StartCoroutine(EnableHealthBarTimeout());
                    
                }
                else
                {
                    if (Application.isMobilePlatform || controller.projectSettings.mobileDebug)
                    {
                        if (RoomManager)
                            controller.UIManager.uiButtons[9].onClick.AddListener(delegate { RoomManager.Pause(true); });
                    }

                    if (controller.PlayerHealthBar && controller.PlayerHealthBarBackground)
                    {
                        controller.PlayerHealthBar.gameObject.SetActive(false);
                        controller.PlayerHealthBarBackground.gameObject.SetActive(false);
                    }

                    controller.thisCamera.SetActive(true);

//                    if ((Application.isMobilePlatform || controller.projectSettings.mobileDebug) && controller.UIManager.UIButtonsMainObject)
//                        controller.UIManager.UIButtonsMainObject.SetActive(true);
                }
            }

            IEnumerator EnableHealthBarTimeout()
            {
                yield return new WaitForSeconds(1);
                
                if (controller.PlayerHealthBar && controller.PlayerHealthBarBackground)
                {
                    if ((PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] != controller.MyTeam ||
                        (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == controller.MyTeam && controller.MyTeam == PUNHelper.Teams.Null)
                    {
                        controller.PlayerHealthBar.gameObject.SetActive(true);
                        controller.PlayerHealthBarBackground.gameObject.SetActive(true);
                    }
                    else
                    {
                        controller.PlayerHealthBar.gameObject.SetActive(false);
                        controller.PlayerHealthBarBackground.gameObject.SetActive(false);
                    }
                }
                
                StopCoroutine(EnableHealthBarTimeout());
            }

            #endregion
    
        #region UpdateSynchElements

        void FixedUpdate()
        {
            if(FindObjectOfType<Lobby>()) return;
            
            weaponController = weaponManager.WeaponController;
           
            if (!photonView.IsMine && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (Mathf.Abs(currentHealth - controller.PlayerHealth) > 0.1f)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };
                    object[] content =
                    {
                        photonView.ViewID, controller.PlayerHealth, controller.KillerName
                    };
                    PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeHealth, content, options, SendOptions.SendReliable);
                    currentHealth = controller.PlayerHealth;
                }
                
//                controller.thisCamera.transform.position = CameraPosition;
//                controller.thisCamera.transform.eulerAngles = CameraRotation;
            }
            else if(photonView.IsMine && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (RoomManager.MatchTarget == PUNHelper.MatchTarget.Domination)
                {
                    PhotonNetwork.LocalPlayer.SetCustomProperties(Vector3.Distance(RoomManager.A_Point.transform.position, transform.position) < RoomManager.A_Point.Radius ? new Hashtable {{"ac", true}} : new Hashtable {{"ac", false}});
                    PhotonNetwork.LocalPlayer.SetCustomProperties(Vector3.Distance(RoomManager.B_Point.transform.position, transform.position) < RoomManager.B_Point.Radius ? new Hashtable {{"bc", true}} : new Hashtable {{"bc", false}});
                    PhotonNetwork.LocalPlayer.SetCustomProperties(Vector3.Distance(RoomManager.C_Point.transform.position, transform.position) < RoomManager.C_Point.Radius ? new Hashtable {{"cc", true}} : new Hashtable {{"cc", false}});

                    if (RoomManager.CurrentHardPoint)
                    {
                        if (Mathf.Abs((transform.position - RoomManager.CurrentHardPoint.transform.position).x) < RoomManager.CurrentHardPoint.Size.x / 2 &&
                            Mathf.Abs((transform.position - RoomManager.CurrentHardPoint.transform.position).y) < RoomManager.CurrentHardPoint.Size.y / 2 &&
                            Mathf.Abs((transform.position - RoomManager.CurrentHardPoint.transform.position).z) < RoomManager.CurrentHardPoint.Size.z / 2)
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"hpc", true}});
                        }
                        else
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"hpc", false}});
                        }
                    }
                }
            }
        }

        void Update()
        {
            if(FindObjectOfType<Lobby>()) return;
            
            if (!photonView.IsMine)
            {
                controller.thisCamera.transform.position = Vector3.Lerp(controller.thisCamera.transform.position, CameraPosition, 5);
                controller.thisCamera.transform.eulerAngles = Vector3.Lerp(controller.thisCamera.transform.eulerAngles, CameraRotation, 5);
            }
        }

        void LateUpdate()
        {
            if(FindObjectOfType<Lobby>()) return;
            
            if (!photonView.IsMine && PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                {
                    if (controller.CameraController.BodyLookAt)
                        controller.BodyLookAt(controller.CameraController.BodyLookAt);
                }
                else
                {
                    controller.BodyObjects.TopBody.localEulerAngles = controller.BodyLocalEulerAngles;
                }

                if(controller.PlayerHealthBar)
                    controller.PlayerHealthBar.fillAmount = currentHealth / controller.PlayerHealthPercent;
                

                if (controller.PlayerHealthBar && controller.PlayerHealthBarBackground && RoomManager.Player)
                {
//                    controller.PlayerHealthBar.transform.LookAt(RoomManager.Player.GetComponent<Controller>().thisCamera.transform);
                    controller.PlayerHealthBarBackground.transform.LookAt(RoomManager.Player.GetComponent<Controller>().thisCamera.transform);
                }

                if (weaponController)
                {
                    if (!weaponController.isMultiplayerWeapon)
                        weaponController.isMultiplayerWeapon = true;

                    if (weaponController.gameObject.layer == 8)
                        Helper.ChangeLayersRecursively(weaponController.transform, "Default");
                }
            }
            else if (photonView.IsMine & PhotonNetwork.IsConnected & PhotonNetwork.InRoom)
            {
                if (controller.changeCameraType)
                {
                    var options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.AddToRoomCache,
                        Receivers = ReceiverGroup.Others
                    };
                    RaiseEventSender(controller.TypeOfCamera, PhotonEventCodes.ChangeCameraType, options);
                    controller.changeCameraType = false;
                }

                if (weaponController)
                {
//                    if (weaponController.gameObject.layer != 8)
//                        Helper.ChangeLayersRecursively(weaponController.transform, "Character");

                    if (weaponController.isMultiplayerWeapon)
                        weaponController.isMultiplayerWeapon = false;

                    if (weaponController.MultiplayerChangeAttack)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.AddToRoomCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.ChangeAttack, options);
                        weaponController.MultiplayerChangeAttack = false;
                    }

                    if (controller.multiplayerCrouch)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.AddToRoomCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.Crouch, options);
                        controller.multiplayerCrouch = false;
                    }

                    if (weaponController.MultiplayerReload)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(null, PhotonEventCodes.Reload, options);
                        weaponController.MultiplayerReload = false;
                    }

                    if (weaponController.MultiplayerBulletAttack)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        object[] content =
                        {
                            photonView.ViewID,
                            controller.thisCamera.transform.position,
                            controller.thisCamera.transform.rotation
                        };
                        PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.Bullets, content, options, SendOptions.SendReliable);

                        weaponController.MultiplayerBulletAttack = false;
                    }

                    if (weaponController.MultiplayerFire)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        RaiseEventSender(true, PhotonEventCodes.Fire, options);

                        weaponController.MultiplayerFire = false;
                        sendEvent = false;
                    }
                    else
                    {
                        if (!sendEvent)
                        {
                            var options = new RaiseEventOptions
                            {
                                CachingOption = EventCaching.DoNotCache,
                                Receivers = ReceiverGroup.Others
                            };

                            RaiseEventSender(false, PhotonEventCodes.Fire, options);
                            sendEvent = true;
                        }
                    }

                    if (weaponController.MultiplayerRocket)
                    {
                        var options = new RaiseEventOptions
                        {
                            CachingOption = EventCaching.DoNotCache,
                            Receivers = ReceiverGroup.Others
                        };
                        object[] content =
                        {
                            photonView.ViewID,
                            weaponController.Hit.point,
                            weaponController.MultiplayerRocketRaycast,
                            controller.thisCamera.transform.position,
                            controller.thisCamera.transform.rotation
                        };
                        PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.Rocket, content, options, SendOptions.SendReliable);
                        weaponController.MultiplayerRocket = false;
                    }
                }
            }
        }

        #endregion

        public void PickUp()
        {
            var room = FindObjectOfType<PickUpManager>();
            if(room) room.PickUp(photonView.ViewID, weaponManager.currentPickUpId);
            else Debug.LogError("You must add the [PickUp Manger] script to the scene.");
        }

        public void ChangeTDMode()
        {

            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            RaiseEventSender(null, PhotonEventCodes.ChangeTDMode, options);
        }

        public void DropWeapon(bool getNewWeapon)
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            object[] content =
            {
                photonView.ViewID,
                weaponManager.DropIdMultiplayer,
                weaponManager.DropDirection,
                getNewWeapon
            };
            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.DropWeapon, content, options, SendOptions.SendReliable);
        }

        public void ThrowGrenade(bool fullBody)
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            object[] content =
            {
                photonView.ViewID,
                controller.thisCamera.transform.position,
                controller.thisCamera.transform.rotation,
                fullBody
            };
            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.Grenade, content, options, SendOptions.SendReliable);
        }

        public void ChangeWeapon(bool hasWeapon)
        {
//            Debug.LogError("change weapon event");
            
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };
            object[] content =
            {
                photonView.ViewID,
                weaponManager.currentSlot,
                weaponManager.slots[weaponManager.currentSlot].currentWeaponInSlot,
                hasWeapon
            };

            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeWeapon, content, options, SendOptions.SendReliable);
        }

        public void MeleeAttack(bool value, int animationIndex, int crouchAnimationIndex)
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };
            object[] content =
            {
                photonView.ViewID,
                value,
                animationIndex,
                crouchAnimationIndex
            };

            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.MeleeAttack, content, options, SendOptions.SendReliable);
        }
        
        public void MeleeAttack(bool value, int animationIndex)
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };
            object[] content =
            {
                photonView.ViewID,
                value,
                animationIndex
            };

            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.MeleeAttack, content, options, SendOptions.SendReliable);
        }

        public void Aim()
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            RaiseEventSender(null, PhotonEventCodes.Aim, options);
        }

        public void UseHealthKit()
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            object[] content = {photonView.ViewID, controller.PlayerHealth};

            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.ChangeHealth, content, options, SendOptions.SendReliable);
        }

        //function for the local player
        public void AddScore(int score, string type)
        {
            if (RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup)
            {
                return;

                RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.gameObject.SetActive(true);
                
                switch (type)
                {
                    case "bullet":
                        RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + score + " (Normal Kill)";
                        break;
                    
                    case "headshot":
                        RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + score + " (Headshot)";
                        break;
                    
                    case "fire":
                        RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + score + " (Toasted)";

                        break;
                    
                    case "explosion":
                        RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + score + " (Blew up)";

                        break;
                    
                    case "melee":
                        RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + score + " (Melee Kill)";

                        break;
                }
            }

            RoomManager.StartCoroutine(RoomManager.AddScorePopupDisableTimeout());

            var _score = (int)PhotonNetwork.LocalPlayer.CustomProperties["cms"];
            _score += score;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable{{"cms", _score}});

            if (RoomManager.MatchTarget == PUNHelper.MatchTarget.Points)
            {
                var currentScore = (int) PhotonNetwork.CurrentRoom.CustomProperties[controller.MyTeam == PUNHelper.Teams.Red ? "rs" : "bs"];
                currentScore += score;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{controller.MyTeam == PUNHelper.Teams.Red ? "rs" : "bs", currentScore}});
            }
        }

        public void UpdateKillAssists(string name)
        {
            if (KillersNames.Contains(name)) return;
            
            KillersNames.Add(name);

            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };
            RaiseEventSender(name, PhotonEventCodes.UpdateKillAssistants, options);
        }

        public void CreateHitMark(int id)
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            
            RaiseEventSender(id, PhotonEventCodes.CreateHitMark, options);
        }
        
        public void CreateHitMark(Vector3 startPosition)
        {
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            
            RaiseEventSender(startPosition, PhotonEventCodes.CreateHitMark, options);
        }

        public void AddScoreToAssistants()
        {
            if (RoomManager.useTeams)
            {
                var content = new List<object> {photonView.ViewID};
                
                foreach (var name in KillersNames.Where(name => name != controller.KillerName && name != controller.CharacterName))
                {
                    content.Add(name);
                    PhotonNetwork.PlayerListOthers.ToList().Find(player => player.NickName == name).SetCustomProperties(new Hashtable {{"cms", PlayerPrefs.GetInt("KillAssist")}});
                }
                
                var options = new RaiseEventOptions
                {
                    CachingOption = EventCaching.DoNotCache,
                    Receivers = ReceiverGroup.Others
                };
                
                PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.SetKillAssistants, content, options, SendOptions.SendReliable);
            }
        }
        
        //for the local player//
        public void Destroy()
        {
            var PlayersList = PhotonNetwork.PlayerList;
            var customValues = new Hashtable();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"ac", false}});
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"bc", false}});
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"cc", false}});

            //update player death count
            var deaths = (int) photonView.Owner.CustomProperties["d"] + 1;
            customValues.Add("d", deaths);
            photonView.Owner.SetCustomProperties(customValues);

            foreach (var player in PlayersList)
            {
                if (controller.KillerName == player.NickName && controller.KillerName != photonView.Owner.NickName)
                {
                    customValues.Clear();
                    var kills = (int) player.CustomProperties["k"];
                    kills++;
                    customValues.Add("k", kills);
                    player.SetCustomProperties(customValues);

                    if (RoomManager.useTeams && RoomManager.MatchTarget == PUNHelper.MatchTarget.Kills)
                    {
                        int currentKills;
                        switch ((PUNHelper.Teams) player.CustomProperties["pt"])
                        {
                            case PUNHelper.Teams.Red:
                                currentKills = (int) PhotonNetwork.CurrentRoom.CustomProperties["rs"];
                                currentKills++;
                                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"rs", currentKills}});
                                break;
                            
                            case PUNHelper.Teams.Blue:
                                currentKills = (int) PhotonNetwork.CurrentRoom.CustomProperties["bs"];
                                currentKills++;
                                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"bs", currentKills}});
                                break;
                        }
                    }
                }
            }

            if (RoomManager.MatchTarget == PUNHelper.MatchTarget.Survive)
            {
                CalculatePlayerPlace(RoomManager);
            }
            
            RoomManager.currentUIManager.CharacterUI.DisableAll();

            var allControllers = FindObjectsOfType<Controller>();
            allControllers.ToList().Remove(controller);

            RoomManager.ClearPlayerList(false);
            RoomManager.AddPlayersToList(false);

            RoomManager.StartCoroutine(RoomManager.RestartGame());
            RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent.gameObject.SetActive(true);
            RoomManager.StartCoroutine(RoomManager.StatsEnabledTimer());
            
            if(RoomManager.MatchTarget == PUNHelper.MatchTarget.Survive)
                RoomManager.deadPlayers.Add(PhotonNetwork.LocalPlayer);
            
            RoomManager.ClearPlayersIcons();
            RoomManager.AddPlayersIcons();
           
            //match stats here
            var tempScript = Instantiate(RoomManager.currentUIManager.MultiplayerGameRoom.matchStatsPlaceholder.gameObject, RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent).GetComponent<UIPlaceholder>();
            tempScript.gameObject.SetActive(true);
            tempScript.KillerName.text = controller.KillerName;
            tempScript.VictimName.text = photonView.Owner.NickName;
            if(controller.KilledWeaponImage && tempScript.WeaponIcon) tempScript.WeaponIcon.texture = controller.KilledWeaponImage;
            
            controller.CameraController.enabled = false;
            
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            RaiseEventSender(null, PhotonEventCodes.PlayerDeath, options);
            
            if (GetComponent<PhotonView>())
                GetComponent<PhotonView>().enabled = false;

            enabled = false;
        }

        public void UpdatePlayersList()
        {
            RoomManager.ClearPlayersIcons();
            RoomManager.AddPlayersIcons();
            
            RoomManager.ClearPlayerList(false);
            RoomManager.AddPlayersToList(false);
                    
            var options = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            };
            RaiseEventSender(null, PhotonEventCodes.UpdatePlayerList, options);
        }

        public List<int> GetSelectedWeapons()
        {
            var stringValue =  (string)photonView.Owner.CustomProperties["wi"];
            var selectedWeapons = new List<int>();

            if (stringValue != "")
                selectedWeapons.AddRange(Array.ConvertAll(stringValue.Split(','), int.Parse));
            else
                selectedWeapons.Add(9);

            return selectedWeapons;
        }

        void CalculatePlayerPlace(RoomManager roomManager)
        {
            if (!roomManager.useTeams)
            {
                var numberOfLivePlayers = PhotonNetwork.PlayerList.Count(player => (int) player.CustomProperties["d"] == 0);
                numberOfLivePlayers++;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"pl", numberOfLivePlayers}});
            }
            else
            {
                var numberOfLivePlayers = PhotonNetwork.PlayerList.Where(player => (PUNHelper.Teams)player.CustomProperties["pt"] == controller.MyTeam).Count(player => (int) player.CustomProperties["d"] == 0);
                numberOfLivePlayers++;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"pl", numberOfLivePlayers}});
            }
        }
        

        #region ProcessingSynchElements
    
        void RaiseEventSender(object value, PhotonEventCodes code, RaiseEventOptions options)
        {
            object[] content =
            {
                photonView.ViewID, value
            };
            PhotonNetwork.RaiseEvent((byte) code, content, options, SendOptions.SendReliable);
        }

        void OnEvent(EventData photonEvent)
        {
            PhotonEventCodes eventCode = (PhotonEventCodes) photonEvent.Code;
            object[] data = photonEvent.CustomData as object[];
            if (data != null)
                if ((int) data[0] == photonView.ViewID)
                {
                    if (eventCode == PhotonEventCodes.UpdatePlayerList)
                    {
                        if (data.Length == 2)
                        {
                            RoomManager.ClearPlayerList(false);
                            RoomManager.AddPlayersToList(false);
                            
                            RoomManager.ClearPlayersIcons();
                            RoomManager.AddPlayersIcons();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeHealth)
                    {
                        if (data.Length == 3)
                        {
                            controller.PlayerHealth = (float) data[1];
                            currentHealth = controller.PlayerHealth;

                            if (controller.PlayerHealth <= 0)
                            {
                                controller.KillerName = (string) data[2];

                                foreach (var character in FindObjectsOfType<CharacterSync>())
                                {
                                    if (character.controller.CharacterName == controller.KillerName)
                                        if(character.weaponController && character.weaponController.WeaponImage)
                                            controller.KilledWeaponImage = (Texture2D)character.weaponController.WeaponImage;
                                }
                            }
                        }
                        else if (data.Length == 2)
                        {
                            controller.PlayerHealth = (float) data[1];
                            currentHealth = controller.PlayerHealth;
                        }

                    }
                    else if (eventCode == PhotonEventCodes.PlayerDeath)
                    {
                        if (data.Length == 2)
                        {
                            foreach (var part in controller.BodyParts)
                            {
                                part.GetComponent<Rigidbody>().isKinematic = false;
                            }
                            
                            controller.anim.enabled = false;
                            controller.enabled = false;
                            weaponManager.enabled = false;
                            
                            if(weaponManager.WeaponController)
                                Destroy(weaponManager.WeaponController.gameObject);

                            RoomManager.ClearPlayerList(false);
                            RoomManager.AddPlayersToList(false);

                            if (controller.PlayerHealthBar && controller.PlayerHealthBarBackground)
                            {
                                controller.PlayerHealthBar.gameObject.SetActive(false);
                                controller.PlayerHealthBarBackground.gameObject.SetActive(false);
                            }

                            if(RoomManager.MatchTarget == PUNHelper.MatchTarget.Survive)
                                RoomManager.deadPlayers.Add(photonView.Owner);
                            
                            RoomManager.ClearPlayersIcons();
                            RoomManager.AddPlayersIcons();

                            if (RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent)
                            {
                                RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent.gameObject.SetActive(true);
                                RoomManager.StartCoroutine(RoomManager.StatsEnabledTimer());
                                var tempScript = Instantiate(RoomManager.currentUIManager.MultiplayerGameRoom.matchStatsPlaceholder.gameObject, RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent).GetComponent<UIPlaceholder>();
                                tempScript.gameObject.SetActive(true);
                                tempScript.KillerName.text = controller.KillerName;
                                tempScript.VictimName.text = photonView.Owner.NickName;
                                if (controller.KilledWeaponImage && tempScript.WeaponIcon)
                                {
                                    tempScript.WeaponIcon.texture = controller.KilledWeaponImage;
                                }
                            }
                            
                            if (GetComponent<PhotonView>())
                                GetComponent<PhotonView>().enabled = false;
                            
                            enabled = false;
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeWeapon)
                    {
                        if (data.Length == 4)
                        {
                            weaponManager.currentSlot = (int) data[1];
                            weaponManager.slots[weaponManager.currentSlot].currentWeaponInSlot = (int) data[2];

                            if ((bool) data[3])
                            {
                                weaponManager.hasWeaponChanged = true;
                            }
                            else
                            {
                                weaponManager.hideAllWeapons = true;
                            }

                            weaponManager.SwitchNewWeapon(false);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Aim)
                    {
                        if (data.Length == 2)
                        {
                            if(weaponController)
                                weaponController.Aim(false, false, false);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeTDMode)
                    {
                        if (data.Length == 2)
                        {
                            CharacterHelper.ChangeTDMode(controller);
                            
                            CharacterHelper.ResetCameraParameters(controller.TypeOfCamera, controller.TypeOfCamera, controller);

                            if(weaponManager.WeaponController)
                                WeaponsHelper.SetWeaponPositions(weaponManager.WeaponController, true, controller.DirectionObject);
                            
                        }
                    }
                    else if (eventCode == PhotonEventCodes.CreateHitMark)
                    {
                        if (data.Length == 2 && !controller.isMultiplayerCharacter)
                        {
                            if (data[1] is int)
                            {
                                Transform player = null;

                                foreach (var character in FindObjectsOfType<CharacterSync>())
                                {
                                    if (character.photonView.ViewID == (int) data[1])
                                    {
                                        player = character.transform;
                                    }
                                }

                                if (player)
                                {
                                    var direction = player.position - controller.transform.position;
                                    var targetPosition = player.position + direction * 1000;
                                    CharacterHelper.CreateHitMarker(controller, player, targetPosition);
                                }
                            }
                            else
                            {
                                var direction = (Vector3)data[1] - controller.transform.position;
                                var targetPosition = (Vector3)data[1] + direction * 1000;
                                CharacterHelper.CreateHitMarker(controller, null, targetPosition);
                            }
                        }
                    }
                    else if (eventCode == PhotonEventCodes.MeleeAttack)
                    {
                        if (data.Length == 4)
                        {
                            if ((bool) data[1])
                            {
                                weaponController.animationIndex = (int) data[2];
                                weaponController.crouchAnimationIndex = (int) data[3];
                                
                                weaponController.MeleeAttack();
                            }
                            else weaponController.MeleeAttackOff();
                        }
                        else if (data.Length == 3)
                        {
                            if ((bool) data[1])
                            {
                                weaponManager.animationIndex = (int) data[2];
                                weaponManager.Punch();
                            }
                            else
                            {
                                weaponManager.DisablePunchAttack();
                            }
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Crouch)
                    {
                        controller.isCrouch = !controller.isCrouch;

                        if (controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                        {
                            if (controller.WeaponManager.hasAnyWeapon)
                                weaponManager.WeaponController.CrouchHands();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Grenade)
                    {
                        if (data.Length == 4)
                        {
                            controller.thisCamera.transform.position = (Vector3) data[1];
                            controller.thisCamera.transform.rotation = (Quaternion) data[2];

                            weaponController.ThrowGrenade((bool) data[3]);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeAttack)
                    {
                        if (data.Length == 2)
                        {
                            weaponController.ChangeAttack();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Reload)
                    {
                        if (data.Length == 2)
                        {
                            weaponController.Reload();
//                            if (weaponController.Attacks[weaponController.currentAttack].ReloadAudio)
//                                weaponController.GetComponent<AudioSource>().PlayOneShot(weaponController.Attacks[weaponController.currentAttack].ReloadAudio);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Bullets)
                    {
                        if (data.Length == 3)
                        {
                            controller.thisCamera.transform.position = (Vector3) data[1];
                            controller.thisCamera.transform.rotation = (Quaternion) data[2];

                            weaponController.BulletAttack();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Fire)
                    {
                        if (data.Length == 2)
                        {
                            if (!(bool) data[1])
                            {
                                weaponController.GetComponent<AudioSource>().Stop();
                                weaponController.attackAudioPlay = false;
                            }
                            else
                            {
                                foreach (var effect in weaponController.Attacks[weaponController.currentAttack].AttackEffects)
                                {
                                    if (effect)
                                    {
                                        var _effect = Instantiate(effect, weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.position, weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.rotation);
                                        _effect.gameObject.hideFlags = HideFlags.HideInHierarchy;
                                    }
                                }

                                if (!weaponController.attackAudioPlay)
                                {
                                    controller.anim.CrossFade("Attack", 0, 1, Time.deltaTime, 10);

                                    if (weaponController.Attacks[weaponController.currentAttack].AttackAudio)
                                    {
                                        weaponController.GetComponent<AudioSource>().clip = weaponController.Attacks[weaponController.currentAttack].AttackAudio;
                                        weaponController.GetComponent<AudioSource>().Play();
                                        weaponController.attackAudioPlay = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (eventCode == PhotonEventCodes.Rocket)
                    {
                        if (data.Length == 5)
                        {
                            controller.thisCamera.transform.rotation = (Quaternion) data[4];
                            controller.thisCamera.transform.position = (Vector3) data[3];

                            weaponController.RocketAttack();
                        }
                    }
                    else if (eventCode == PhotonEventCodes.ChangeCameraType)
                    {
                        if (data.Length == 2)
                        {
                            CharacterHelper.SwitchCamera(controller.TypeOfCamera, (CharacterHelper.CameraType) data[1], controller);
                            
                            if(weaponManager.WeaponController)
                                WeaponsHelper.SetWeaponPositions(weaponManager.WeaponController, true, controller.DirectionObject);

                        }
                    }
                    else if (eventCode == PhotonEventCodes.DropWeapon)
                    {
                        if (data.Length == 4)
                        {
                            weaponManager.DropIdMultiplayer = (string) data[1];
                            weaponManager.DropDirection = (Vector3) data[2];
                            weaponManager.DropWeapon((bool) data[3]);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.SetKillAssistants)
                    {
                        for (var index = 1; index < data.Length; index++)
                        {
                            var name = (string) data[index];
                            
                            if (name == PhotonNetwork.LocalPlayer.NickName)
                            {
                                if (RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup)
                                {
                                    RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.gameObject.SetActive(true);
                                    RoomManager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + PlayerPrefs.GetInt("KillAssist") + " (Kill Assistant)";
                                    RoomManager.StartCoroutine(RoomManager.AddScorePopupDisableTimeout());
                                }
                            }
                        }
                    }
                    else if(eventCode == PhotonEventCodes.UpdateKillAssistants)
                    {
                        if (data.Length == 2)
                        {
                            if (!KillersNames.Contains((string) data[1]))
                                KillersNames.Add((string) data[1]);
                        }
                    }
                    else if (eventCode == PhotonEventCodes.BulletHit)
                    {
                        if (data.Length == 3)
                        {
                            
                        }
                    }
                }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
         {
             if (stream.IsWriting)
             {
                 stream.SendNext(controller.BodyLocalEulerAngles);
                 stream.SendNext(controller.CurrentRotation);
                 stream.SendNext(controller.SmoothIKSwitch);
                 stream.SendNext(controller.currentGravity);
                 stream.SendNext(controller.thisCamera.transform.position);
                 stream.SendNext(controller.thisCamera.transform.eulerAngles);
                 stream.SendNext(controller.currentCharacterControllerCenter);

                 if(controller.CameraController.BodyLookAt)
                    stream.SendNext(controller.CameraController.BodyLookAt.position);

                 stream.SendNext(controller.bodyRotationDownLimit_x);
                 stream.SendNext(controller.bodyRotationDownLimit_y);
                 stream.SendNext(controller.bodyRotationUpLimit_x);
                 stream.SendNext(controller.bodyRotationUpLimit_y);
                 
                 if(weaponManager.WeaponController)
                    stream.SendNext(weaponManager.WeaponController.BarrelRotationSpeed);
                 
             }
             else
             {
                 controller.BodyLocalEulerAngles = (Vector3) stream.ReceiveNext();
                 controller.CurrentRotation = (Quaternion) stream.ReceiveNext();
                 controller.SmoothIKSwitch = (float) stream.ReceiveNext();
                 controller.currentGravity = (float) stream.ReceiveNext();
                 CameraPosition = (Vector3) stream.ReceiveNext();
                 CameraRotation = (Vector3) stream.ReceiveNext();

                 var yValue = (float) stream.ReceiveNext();
                 
                 if(controller.CharacterController)
                    controller.CharacterController.center = new Vector3(controller.CharacterController.center.x, yValue, controller.CharacterController.center.z);

                 if(controller.CameraController.BodyLookAt)
                    controller.CameraController.BodyLookAt.position = (Vector3) stream.ReceiveNext();

                 controller.bodyRotationDownLimit_x = (float) stream.ReceiveNext();
                 controller.bodyRotationDownLimit_y = (float) stream.ReceiveNext();
                 controller.bodyRotationUpLimit_x = (float) stream.ReceiveNext();
                 controller.bodyRotationUpLimit_y = (float) stream.ReceiveNext();
                 
                 if(weaponManager.WeaponController)
                     weaponManager.WeaponController.BarrelRotationSpeed = (float) stream.ReceiveNext();

                 if (firstTake)
                     firstTake = false;
             }
         }
    
         #endregion
#endif
    }
}


