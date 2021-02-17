using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
#if PHOTON_UNITY_NETWORKING
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

namespace GercStudio.USK.Scripts
{
    public class Lobby : 
#if PHOTON_UNITY_NETWORKING
        MonoBehaviourPunCallbacks, ILobbyCallbacks
#else
    MonoBehaviour
#endif
    {
        public List<PUNHelper.PhotonLevel> Maps = new List<PUNHelper.PhotonLevel>{new PUNHelper.PhotonLevel()};

        public UIManager UiManager;
        public UIManager currentUIManager;

        public List<PUNHelper.GameMode> GameModes;
        public List<PUNHelper.WeaponSlot> AllWeapons;
        public List<Controller> Characters;

        public List<Texture> DefaultAvatars;

        public List<UIPlaceholder> AllWeaponsPlaceholders;
        public List<UIPlaceholder> AllGameModesPlaceholders;
        public List<UIPlaceholder> AllMapsPlaceholders;
        public List<UIPlaceholder> AllAvatarsPlaceholders;

        public ProjectSettings projectSettings;

        public RuntimeAnimatorController characterAnimatorController;

        public GameObject DefaultCamera;
        public GameObject RoomListingPrefab;
        public GameObject FindRooms;
        public GameObject MainMenu;
        public GameObject CreateRooms;
        public GameObject GameModesMain;
        private GameObject CurrentWeapon;
        private GameObject CurrentCharacter;

        public Transform WeaponSpawnPoint;
        public Transform CharacterSpawnPoint;

        private PUNHelper.CameraPosition currentCameraPositions;
        public PUNHelper.CameraPosition MainMenuPositions;
        public PUNHelper.CameraPosition LoadoutPositions;
        public PUNHelper.CameraPosition CharacterPositions;

        public string RedTeamName;
        public string BlueTeamName;
        public string checkConnectionServer = "https://google.com";

        public bool showWeaponName;
        public bool showWeaponDamage;
        public bool showWeaponRateOfAttack;
        public bool showWeaponAmmo;
        public bool showWeaponScatter;
        public bool showWeaponWeight;

        public bool showModePlayers;
        public bool showModeTimeLimit;
        public bool showModeScoreLimit;
        public bool showModeDescription;
        public bool showModeRoundsCount;
        
        public bool openAnyMenu;

        public Texture RedTeamLogo;
        public Texture BlueTeamLogo;

        #region InspectorVariables

        public int upInspectorTab;
        public int downInspectorTab;
        public int currentInspectorTab;
        public int currentMode;
        public int currentCameraMode;
        public int lastCameraMode;

        #endregion

        private bool launchAgain;

        public List<int> selectedWeapons;
        public int WeaponIndex;
        public int GameModeIndex;
        public int MapIndex;
        public int CharacterIndex;
        public int AvatarIndex;

        public int PlayerScore;
        
        public int normKill;
        public int fireKill;
        public int explosionKill;
        public int headshot;
        public int meleeKill;
        public int capturePoint;
        public int capturePointAssist;
        public int killAssist;

#if PHOTON_UNITY_NETWORKING
        private List<RoomInfo> AllRooms = new List<RoomInfo>();

        private bool isConnected;
        private bool firstTake = true;
        private string seletedRoomName;
#endif

        private void Awake()
        {

            PlayerPrefs.SetString("SelectedWeapons", "9");

#if PHOTON_UNITY_NETWORKING
            if (UiManager)
            {
                currentUIManager = Instantiate(UiManager.gameObject).GetComponent<UIManager>();
            }
            else
            {
                Debug.LogError("UI Manager was not be loaded.");
            }
            
            currentUIManager.HideAllMultiplayerLobbyUI();
            currentUIManager.HideAllMultiplayerRoomUI();
            currentUIManager.CharacterUI.DisableAll();
            
            if(currentUIManager.SinglePlayerGame.PauseMainObject)
                currentUIManager.SinglePlayerGame.PauseMainObject.SetActive(false);
                
            if(currentUIManager.SinglePlayerGame.OptionsMainObject)
                currentUIManager.SinglePlayerGame.OptionsMainObject.SetActive(false);
            
            currentUIManager.MultiplayerGameLobby.MainMenu.ActivateAll(true);
#else
            Debug.LogWarning("To use the multiplayer mode, import PUN2 from Asset Store.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
#endif
        }

#if PHOTON_UNITY_NETWORKING
        void Start()
        {
            string DefaultName = String.Empty;
            
            var tempCharacters = new List<Controller>();
            
            foreach (var character in Characters)
            {
                if (!character)
                {
                    tempCharacters.Add(character);
                }
                else
                {
                    if (!character.gameObject.GetComponent<CharacterSync>())
                    {
                        CharacterHelper.AddMultiplayerScripts(character.gameObject);
                    }
                }
            }

            foreach (var tempCharacter in tempCharacters)
            {
                Characters.Remove(tempCharacter);
            }
            
            
//            var tempLevels = new List<CharacterHelper.PhotonLevel>();
//            foreach (var level in Levels)
//            {
//                if (!level.LevelButton)
//                {
//                    tempLevels.Remove(level);
//                }
//            }
//
//            foreach (var tempLevel in tempLevels)
//            {
//                Levels.Remove(tempLevel);
//            }

            if (currentUIManager.MultiplayerGameLobby.MainMenu.Nickname)
            {
                if (PlayerPrefs.HasKey("PlayerName"))
                {
                    DefaultName = PlayerPrefs.GetString("PlayerName");
                    currentUIManager.MultiplayerGameLobby.MainMenu.Nickname.text = DefaultName;
                }
            }

            PhotonNetwork.NickName = DefaultName;

            if(PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (MainMenu != null) MainMenu.SetActive(true);
            if (CreateRooms != null) CreateRooms.SetActive(false);
            if (FindRooms != null) FindRooms.SetActive(false);
            if (GameModesMain != null) GameModesMain.SetActive(false);

            if (currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton) currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton.onClick.AddListener(RandomRoomClick);
            if (currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton) currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton.onClick.AddListener(delegate { OpenMenu("allRooms"); });
            if (currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton) currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton.onClick.AddListener(delegate { OpenMenu("createGame"); });
            if (currentUIManager.MultiplayerGameLobby.MainMenu.CreditsButton) currentUIManager.MultiplayerGameLobby.MainMenu.CreditsButton.onClick.AddListener(delegate { OpenMenu("creditsMenu"); });
            if (currentUIManager.MultiplayerGameLobby.MainMenu.SettingsButton) currentUIManager.MultiplayerGameLobby.MainMenu.SettingsButton.onClick.AddListener(delegate { OpenMenu("settingsMenu"); });
            if (currentUIManager.MultiplayerGameLobby.MainMenu.ChooseGameModeButton) currentUIManager.MultiplayerGameLobby.MainMenu.ChooseGameModeButton.onClick.AddListener(delegate { OpenMenu("gameModes"); });
            if (currentUIManager.MultiplayerGameLobby.MainMenu.Nickname) currentUIManager.MultiplayerGameLobby.MainMenu.Nickname.onValueChanged.AddListener(SetName);
            if (currentUIManager.MultiplayerGameLobby.MainMenu.LoadoutButton) currentUIManager.MultiplayerGameLobby.MainMenu.LoadoutButton.onClick.AddListener(delegate { OpenMenu("loadout"); });
            if (currentUIManager.MultiplayerGameLobby.MainMenu.ChangeAvatarButton) currentUIManager.MultiplayerGameLobby.MainMenu.ChangeAvatarButton.onClick.AddListener(delegate { OpenMenu("avatars"); });

            if (currentUIManager.MultiplayerGameLobby.MainMenu.ChangeCharacter) currentUIManager.MultiplayerGameLobby.MainMenu.ChangeCharacter.onClick.AddListener(delegate { OpenMenu("characters"); });

            if (currentUIManager.MultiplayerGameLobby.CreateRoomMenu.CreateButton) currentUIManager.MultiplayerGameLobby.CreateRoomMenu.CreateButton.onClick.AddListener(CreateRoomClick);
            
            if (currentUIManager.MultiplayerGameLobby.GameModesMenu.MapsButton) currentUIManager.MultiplayerGameLobby.GameModesMenu.MapsButton.onClick.AddListener(delegate { OpenMenu("maps"); });
            if (currentUIManager.MultiplayerGameLobby.MapsMenu.GameModesButton) currentUIManager.MultiplayerGameLobby.MapsMenu.GameModesButton.onClick.AddListener(delegate { OpenMenu("gameModes"); });

            if (currentUIManager.MultiplayerGameLobby.MapsMenu.BackButton) currentUIManager.MultiplayerGameLobby.MapsMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });
            if (currentUIManager.MultiplayerGameLobby.LoadoutMenu.BackButton) currentUIManager.MultiplayerGameLobby.LoadoutMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });
            if (currentUIManager.MultiplayerGameLobby.GameModesMenu.BackButton) currentUIManager.MultiplayerGameLobby.GameModesMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });
            if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.BackButton) currentUIManager.MultiplayerGameLobby.AllRoomsMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });
            if (currentUIManager.MultiplayerGameLobby.CreateRoomMenu.BackButton) currentUIManager.MultiplayerGameLobby.CreateRoomMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });

            if (currentUIManager.MultiplayerGameLobby.LoadoutMenu.EquipButton) currentUIManager.MultiplayerGameLobby.LoadoutMenu.EquipButton.onClick.AddListener(Equip);

            if (currentUIManager.MultiplayerGameLobby.AvatarsMenu.BackButton) currentUIManager.MultiplayerGameLobby.AvatarsMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });

            if (currentUIManager.MultiplayerGameLobby.CreditsMenu.BackButton) currentUIManager.MultiplayerGameLobby.CreditsMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });
            if (currentUIManager.MultiplayerGameLobby.SettingsMenu.BackButton) currentUIManager.MultiplayerGameLobby.SettingsMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu");
                                                                                currentUIManager.MultiplayerGameLobby.SettingsMenu.DisableAll(); });

            if (currentUIManager.MultiplayerGameLobby.CharactersMenu.BackButton) currentUIManager.MultiplayerGameLobby.CharactersMenu.BackButton.onClick.AddListener(delegate { OpenMenu("mainMenu"); });
            if (currentUIManager.MultiplayerGameLobby.CharactersMenu.UpButton) currentUIManager.MultiplayerGameLobby.CharactersMenu.UpButton.onClick.AddListener(delegate { ChangeCharacter("+"); });
            if (currentUIManager.MultiplayerGameLobby.CharactersMenu.DownButton) currentUIManager.MultiplayerGameLobby.CharactersMenu.DownButton.onClick.AddListener(delegate { ChangeCharacter("-"); });

            for (var i = 0; i < Maps.Count; i++)
            {
                var level = Maps[i];

                if (currentUIManager.MultiplayerGameLobby.mapPlaceholder)
                {
                   var placeholder = Instantiate(currentUIManager.MultiplayerGameLobby.mapPlaceholder, currentUIManager.MultiplayerGameLobby.MapsMenu.Content).GetComponent<UIPlaceholder>();
                   
                   AllMapsPlaceholders.Add(placeholder);

                   placeholder.name = level.Name;

                   if (placeholder.Name)
                       placeholder.Name.text = level.Name;

                   if (placeholder.ImagePlaceholder && level.Image)
                       placeholder.ImagePlaceholder.texture = level.Image;
                   
                   if(placeholder.SelectionIndicator)
                       placeholder.SelectionIndicator.gameObject.SetActive(false);
                   
                   placeholder.gameObject.SetActive(true);
                   
                    if (level.Image)
                        placeholder.ImagePlaceholder.texture = level.Image;
                    
                    var i1 = i;
                    
                    if(placeholder.Button)
                        placeholder.Button.onClick.AddListener(delegate { SetMap(i1); });
                }
            }

            for (var i = 0; i < GameModes.Count; i++)
            {
                var gameMode = GameModes[i];

                if (currentUIManager.MultiplayerGameLobby.gameModePlaceholder && currentUIManager.MultiplayerGameLobby.GameModesMenu.Content)
                {
                    var placeholder = Instantiate(currentUIManager.MultiplayerGameLobby.gameModePlaceholder.gameObject, currentUIManager.MultiplayerGameLobby.GameModesMenu.Content).GetComponent<UIPlaceholder>();
                    
                    AllGameModesPlaceholders.Add(placeholder);
                        
                    placeholder.name = gameMode.Name;
                    
                    if(placeholder.Name)
                        placeholder.Name.text = gameMode.Name;

                    if(placeholder.ImagePlaceholder && gameMode.Image)
                        placeholder.ImagePlaceholder.texture = gameMode.Image;
                    
                    if(placeholder.SelectionIndicator)
                        placeholder.SelectionIndicator.gameObject.SetActive(false);

                    placeholder.gameObject.SetActive(true);
                    
                    if (gameMode.Image)
                        placeholder.ImagePlaceholder.texture = gameMode.Image;

                    var i1 = i;
                    
                    if(placeholder.Button)
                        placeholder.Button.onClick.AddListener(delegate { SetMode(i1); });
                }
            }

            for (var i = 0; i < AllWeapons.Count; i++)
            {
                var weapon = AllWeapons[i];
                
                if (weapon.weapon && currentUIManager.MultiplayerGameLobby.weaponPlaceholder)
                {
                    var placeholder = Instantiate(currentUIManager.MultiplayerGameLobby.weaponPlaceholder.gameObject, currentUIManager.MultiplayerGameLobby.LoadoutMenu.Content).GetComponent<UIPlaceholder>();
                    
                    AllWeaponsPlaceholders.Add(placeholder);
                    
                    placeholder.gameObject.SetActive(true);
                    
                    if(placeholder.SelectionIndicator)
                        placeholder.SelectionIndicator.gameObject.SetActive(false);

                    if (weapon.weapon.WeaponImage && placeholder.ImagePlaceholder)
                        placeholder.ImagePlaceholder.texture = weapon.weapon.WeaponImage;

                    if (placeholder.Name)
                        placeholder.Name.text = weapon.weapon.name;

                    var i1 = i;
                    
                    if(placeholder.Button)
                        placeholder.Button.onClick.AddListener(delegate { SetWeapon(i1); });
                }
            }

            for (var i = 0; i < DefaultAvatars.Count; i++)
            {
                var avatar = DefaultAvatars[i];

                if (avatar && currentUIManager.MultiplayerGameLobby.avatarPlaceholder)
                {
                    var placeholder = Instantiate(currentUIManager.MultiplayerGameLobby.avatarPlaceholder.gameObject, currentUIManager.MultiplayerGameLobby.AvatarsMenu.Content).GetComponent<UIPlaceholder>();
                    
                    AllAvatarsPlaceholders.Add(placeholder);
                    
                    placeholder.gameObject.SetActive(true);
                    
                    if(placeholder.SelectionIndicator)
                        placeholder.SelectionIndicator.gameObject.SetActive(false);

                    if (placeholder.ImagePlaceholder)
                        placeholder.ImagePlaceholder.texture = avatar;

                    var i1 = i;
                    
                    if(placeholder.Button)
                        placeholder.Button.onClick.AddListener(delegate { SetAvatar(i1); });
                }
            }

            if (!PlayerPrefs.HasKey("CurrentCharacter"))
            {
                PlayerPrefs.SetInt("CurrentCharacter", 0);
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "tttype", 0} });
                //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "tttypeup", 0 } });
            }
            
            if(!PlayerPrefs.HasKey("GameModeIndex"))
                PlayerPrefs.SetInt("GameModeIndex", 0);
            
            if(!PlayerPrefs.HasKey("MapIndex"))
                PlayerPrefs.SetInt("MapIndex", 0);
            
            if(!PlayerPrefs.HasKey("WeaponIndex"))
                PlayerPrefs.SetInt("WeaponIndex", 0);
            
            if(!PlayerPrefs.HasKey("AvatarIndex"))
                PlayerPrefs.SetInt("AvatarIndex", 0);

            if (!PlayerPrefs.HasKey("PlayerScore"))
                PlayerPrefs.SetInt("PlayerScore", 0);

            SetPlayer(PlayerPrefs.GetInt("CurrentCharacter"));
            SetMode(PlayerPrefs.GetInt("GameModeIndex"));
            SetMap(PlayerPrefs.GetInt("MapIndex"));
            SetWeapon(PlayerPrefs.GetInt("WeaponIndex"));
            SetAvatar(PlayerPrefs.GetInt("AvatarIndex"));
            SetScore(PlayerPrefs.GetInt("PlayerScore"));

            PlayerPrefs.SetString("RedTeamName", RedTeamName);
            PlayerPrefs.SetString("BlueTeamName", BlueTeamName);
            
            PlayerPrefs.SetString("RedTeamLogo", RedTeamLogo.name);
            PlayerPrefs.SetString("BlueTeamLogo", BlueTeamLogo.name);
            
            SetEquippedWeapons();

            currentCameraPositions = MainMenuPositions;

            if (DefaultCamera)
            {
                DefaultCamera.transform.position = currentCameraPositions.position;
                DefaultCamera.transform.rotation = currentCameraPositions.rotation;
            }

            if (PlayerPrefs.GetInt("LaunchAgain") == 1)
            {
                launchAgain = true;
                PlayerPrefs.SetInt("LaunchAgain", 0);
            }
            
            UpdateMainMenu();
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown)
            {
                currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.ClearOptions();
                currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.AddOptions(PUNHelper.PhotonRegions);
            }

            if (currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton.interactable = false;

            if (currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton.interactable = false;
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton.interactable = false;
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "Disconnected from Server";
            
            isConnected = false;
            
            if (Helper.GetHtmlFromUri(checkConnectionServer) == "")
            {
                if (currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                    currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "No Internet Connection";
                
                if (currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown)
                    currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.gameObject.SetActive(false);

                StartCoroutine(CheckInternetConnection());
            }
            else
            {
                if (!PhotonNetwork.IsConnected)
                {
                    //Debug.Log("Try connect");
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
        }

        public void FixedUpdate()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            isConnected = PhotonNetwork.IsConnected;
        }

        private void Update()
        {
            if (isConnected && currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "Connected | Ping - " + PhotonNetwork.GetPing() + " ms";

            if (currentUIManager.MultiplayerGameLobby.LoadoutMenu.EquipButtonText)
            {
                currentUIManager.MultiplayerGameLobby.LoadoutMenu.EquipButtonText.text = selectedWeapons.Contains(WeaponIndex) ? "Remove" : "Equip";
            }

            if (DefaultCamera)
            {
                DefaultCamera.transform.position = Vector3.Slerp(DefaultCamera.transform.position, currentCameraPositions.position, 0.6f);
                DefaultCamera.transform.rotation = Quaternion.Lerp(DefaultCamera.transform.rotation, currentCameraPositions.rotation, 0.3f);
            }
        }

//        void ListRoom(RoomInfo room)
//        {
//            if (room.IsVisible)
//            {
//                var tempListing = Instantiate(RoomListingPrefab, roomsPanel);
//                var tempButton = tempListing.GetComponent<RoomButton>();
//                tempButton.RoomName = room.Name;
//                tempButton.CurrentPlayer = room.PlayerCount;
//                tempButton.RoomSize = room.MaxPlayers;
//                tempButton.Enemies = (bool) room.CustomProperties["e"];
//                tempButton.LevelName = Maps[(int) room.CustomProperties["m"]].Name;
////                tempButton.GameModeIndex = Enum.GetName(typeof(ModeOfGame), (ModeOfGame) room.CustomProperties["gm"]);
//                tempButton.SetRoom();
//            }
//        }

        //set up character instance
        void SetPlayer(int index)
        {
            if (!Characters[index]) return;
            
            CharacterIndex = index;
                
            PlayerPrefs.SetString("CharacterPrefabName", Characters[CharacterIndex].name);
            PlayerPrefs.SetInt("CurrentCharacter", CharacterIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "tttype", CharacterIndex } });
            //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "tttypeup", 0} });
            //print("loading " + Characters[CharacterIndex].name);
            //for(int i = 0;i <6; i++)
            //{
            //    print("!!!! iterating names: " + Characters[i].name);
            //}

            if (CharacterSpawnPoint)
            {
                if (CurrentCharacter)
                    Destroy(CurrentCharacter);

                if (Characters[CharacterIndex])
                {
                    //instantiate the players
                    CurrentCharacter = Instantiate(Characters[CharacterIndex].gameObject, CharacterSpawnPoint.transform.position, CharacterSpawnPoint.transform.rotation);
                    CurrentCharacter.GetComponent<Animator>().runtimeAnimatorController = characterAnimatorController;
                }
            }
        }

        public void ChangeCharacter(string type)
        {
            var index = CharacterIndex;

            if (type == "+")
            {
                index++;

                if (index > Characters.Count - 1)
                    index = 0;
            }
            else
            {
                index--;

                if (index < 0)
                    index = Characters.Count - 1;
            }
            
            SetPlayer(index);
        }
        

        public void CreateRoom(string password, string roomName)
        {
            var customValues = new Hashtable();
            
            customValues.Add("gm", GameModeIndex);

            customValues.Add("psw", password);
            customValues.Add("gn", roomName);
            
            // time before match started
            customValues.Add("st", 3);

            customValues.Add("m", Maps.Count > 0 ? MapIndex : 1);

//          customValues.Add("ur", GameModes[GameModeIndex].CanRespawn);

            if (!GameModes[GameModeIndex].CanRespawn)
                GameModes[GameModeIndex].matchTarget = 4;
            
            if(!GameModes[GameModeIndex].Teams && GameModes[GameModeIndex].matchTarget == 3)
                GameModes[GameModeIndex].matchTarget = 1;

            if (GameModes[GameModeIndex].CanRespawn)
            {
                if (GameModes[GameModeIndex].matchTarget == 4)
                    GameModes[GameModeIndex].matchTarget = 5;
            }


            switch (GameModes[GameModeIndex].matchTarget)
            {
                

                case 0:
                    customValues.Add("tar", PUNHelper.MatchTarget.WithoutTarget);
                    customValues.Add("tv", 0);
                    customValues.Add("tl", GameModes[GameModeIndex].TimeLimit ? GameModes[GameModeIndex].targetTime : 0);
                    break;
                
                case 1:
                    customValues.Add("tar", PUNHelper.MatchTarget.Kills);
                    customValues.Add("tv", GameModes[GameModeIndex].targetKills);
                    customValues.Add("tl", GameModes[GameModeIndex].TimeLimit ? GameModes[GameModeIndex].targetTime : 0);
                    break;
                case 2:
                    customValues.Add("tar", PUNHelper.MatchTarget.Points);
                    customValues.Add("tv", GameModes[GameModeIndex].targetPoints);
                    customValues.Add("tl", GameModes[GameModeIndex].TimeLimit ? GameModes[GameModeIndex].targetTime : 0);
                    break;
                case 3:
                    customValues.Add("tar", PUNHelper.MatchTarget.Domination);
                    customValues.Add("tv", GameModes[GameModeIndex].targetPoints);
                    customValues.Add("tl", GameModes[GameModeIndex].TimeLimit ? GameModes[GameModeIndex].targetTime : 0);
                    break;


                case 4:
                    customValues.Add("tar", PUNHelper.MatchTarget.Survive);
                    customValues.Add("tv", 0);
                    customValues.Add("tl", GameModes[GameModeIndex].TimeLimit ? GameModes[GameModeIndex].targetTime : 0);
                    break;


                case 5:
                    customValues.Add("tar", PUNHelper.MatchTarget.Robbery);
                    customValues.Add("tv", GameModes[GameModeIndex].targetEgg);
                    customValues.Add("tl", GameModes[GameModeIndex].TimeLimit ? GameModes[GameModeIndex].targetTime : 0);
                    break;
            }

            customValues.Add("ut", GameModes[GameModeIndex].Teams);
            
            customValues.Add("wpmg", false);

            customValues.Add("rc", 0);
            customValues.Add("bc", 0);
            
            customValues.Add("rs", 0);
            customValues.Add("bs", 0);

            if (GameModes[GameModeIndex].Teams)
            {
                switch (GameModes[GameModeIndex].spawnMethod)
                {
                    case 0:
                        customValues.Add("sm", PUNHelper.SpawnMethod.Random);
                        break;
                    case 1:
                        customValues.Add("sm", PUNHelper.SpawnMethod.OnBases);
                        break;
                }
            }
            else
            {
                customValues.Add("sm", PUNHelper.SpawnMethod.Random);
            }

            switch (GameModes[GameModeIndex].KillMethod)
            {
                case 0:
                    customValues.Add("km", PUNHelper.CanKillOthers.Everyone);
                    break;
                case 1:
                    customValues.Add("km", PUNHelper.CanKillOthers.NoOne);
                    break;
                case 2:
                    customValues.Add("km", GameModes[GameModeIndex].Teams ? PUNHelper.CanKillOthers.OnlyOpponents : PUNHelper.CanKillOthers.Everyone);
                    break;
            }

            customValues.Add("e", GameModes[GameModeIndex].Enemies);

            customValues.Add("gs", false);

            customValues.Add("mp", GameModes[GameModeIndex].minPlayerCount);

            customValues.Add("cr", 1);

            customValues.Add("act", PUNHelper.Teams.Null);
            customValues.Add("acv", 0f);
            
            customValues.Add("bct", PUNHelper.Teams.Null);
            customValues.Add("bcv", 0f);
            
            customValues.Add("cct", PUNHelper.Teams.Null);
            customValues.Add("ccv", 0f);
            
            customValues.Add("hpct", PUNHelper.Teams.Null);
            customValues.Add("hpcv", 0f);

            if (!GameModes[GameModeIndex].Teams || GameModes[GameModeIndex].Teams && GameModes[GameModeIndex].matchTarget == 0 && GameModes[GameModeIndex].CanRespawn)
                GameModes[GameModeIndex].Rounds = 1;
            
            customValues.Add("r", GameModes[GameModeIndex].Rounds);

            customValues.Add("rrw", 0);
            customValues.Add("brw", 0);
            
            customValues.Add("oshok", GameModes[GameModeIndex].oneShotOneKill);
            
            customValues.Add("cpt", GameModes[GameModeIndex].capturePointTimeout);
            customValues.Add("cps", GameModes[GameModeIndex].captureScore);
            customValues.Add("ci", GameModes[GameModeIndex].captureImmediately);
            customValues.Add("hpt", GameModes[GameModeIndex].hardPointTimeout);
            customValues.Add("pc", GameModes[GameModeIndex].pointsCount == 0 ? 3 : 1);
            customValues.Add("chp", 0);


            var roomOpt = new RoomOptions 
            {
                MaxPlayers = (byte) GameModes[GameModeIndex].maxPlayersCount, 
                IsOpen = true, IsVisible = true, 
                CustomRoomProperties = customValues
            };

            var value = new string[5];
            value[0] = "m";
            value[1] = "gm";
            value[2] = "e";
            value[3] = "psw";
            value[4] = "gn";
            roomOpt.CustomRoomPropertiesForLobby = value;
            
            
            PhotonNetwork.CreateRoom(Helper.GenerateRandomString(10), roomOpt);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Player name is empty");
                return;
            }

            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString("PlayerName", name);
        }

        static Predicate<RoomInfo> ByName(string name)
        {
            return room => room.Name == name;
        }

        #region UIManaged

        void SetMap(int index)
        {
            MapIndex = index;
            PlayerPrefs.SetInt("MapIndex", MapIndex);
            
            foreach (var placeholder in AllMapsPlaceholders.Where(placeholder => placeholder.SelectionIndicator))
            {
                placeholder.SelectionIndicator.gameObject.SetActive(false);
            }
            
            if(AllMapsPlaceholders.Count > 0 && AllMapsPlaceholders[index].SelectionIndicator)
                AllMapsPlaceholders[index].SelectionIndicator.gameObject.SetActive(true);

            if (currentUIManager.MultiplayerGameLobby.GameModesMenu.MapButtonText)
                currentUIManager.MultiplayerGameLobby.GameModesMenu.MapButtonText.text = "Map - " + Maps[MapIndex].Name;
        }

        void SetMode(int index)
        {
            GameModeIndex = index;
            PlayerPrefs.SetInt("GameModeIndex", GameModeIndex);

            foreach (var placeholder in AllGameModesPlaceholders.Where(placeholder => placeholder.SelectionIndicator))
            {
                placeholder.SelectionIndicator.gameObject.SetActive(false);
            }
            
            if(AllGameModesPlaceholders.Count > 0 && AllGameModesPlaceholders[index].SelectionIndicator)
                AllGameModesPlaceholders[index].SelectionIndicator.gameObject.SetActive(true);

            if (currentUIManager.MultiplayerGameLobby.MapsMenu.GameModesButtonText)
                currentUIManager.MultiplayerGameLobby.MapsMenu.GameModesButtonText.text = "Game Mode - " + GameModes[GameModeIndex].Name;

            if (currentUIManager.MultiplayerGameLobby.GameModesMenu.Info)
            {
                var info = "";
                var players = "[min: " + GameModes[index].minPlayerCount * (GameModes[index].Teams ? 2 : 1) + "  " + "max: " + GameModes[index].maxPlayersCount * (GameModes[index].Teams ? 2 : 1) + "]";
                var timeLimit = GameModes[index].TimeLimit ?  GameModes[index].targetTime.ToString() : "∞";
                var targetScore = GameModes[index].matchTarget == 1 ? GameModes[index].targetKills.ToString() : GameModes[index].targetPoints.ToString();
                var roundsCount = GameModes[index].Rounds.ToString();

                if (showModePlayers)
                    info += "Players - " + players;

                if (showModeScoreLimit)
                {
                    if (GameModes[index].matchTarget != 0)
                    {
                        if (GameModes[index].CanRespawn)
                            info += (GameModes[index].matchTarget == 0 ? "  |  Target Kills - " : "  |  Target Score - ") + targetScore;
                        else info += "  |  Target - Kill Everyone";
                    }
                    else info += "  |  Without Target";
                }

                if (GameModes[index].matchTarget != 0)
                {
                    if (showModeRoundsCount)
                        info += "  |  Rounds - " + roundsCount;
                }

                if (showModeTimeLimit)
                    info += "  |  Time Limit - " + timeLimit + " min.";


                if (showModeDescription)
                    info += "\n\n" + GameModes[index].Description;

                currentUIManager.MultiplayerGameLobby.GameModesMenu.Info.text = info;
            }
        }

        void SetScore(int score)
        {
            PlayerScore = score;

            PlayerPrefs.SetInt("NormKill", normKill);
            PlayerPrefs.SetInt("Headshot", headshot);
            PlayerPrefs.SetInt("ExplosionKill", explosionKill);
            PlayerPrefs.SetInt("FireKill", fireKill);
            PlayerPrefs.SetInt("MeleeKill", meleeKill);
            PlayerPrefs.SetInt("KillAssist", killAssist);
            PlayerPrefs.SetInt("CaptureAssist", capturePointAssist);
            PlayerPrefs.SetInt("CapturePoint", capturePoint);

            if (currentUIManager.MultiplayerGameLobby.MainMenu.PlayerScore)
                currentUIManager.MultiplayerGameLobby.MainMenu.PlayerScore.text = "Score: " + PlayerScore;
        }

        void SetAvatar(int index)
        {
            AvatarIndex = index;

            PlayerPrefs.SetInt("AvatarIndex", AvatarIndex);
//            PlayerPrefs.SetString("Avatar", DefaultAvatars[AvatarIndex].name);
            
            foreach (var placeholder in AllAvatarsPlaceholders.Where(placeholder => placeholder.SelectionIndicator))
            {
                placeholder.SelectionIndicator.gameObject.SetActive(false);
            }
            
            if(AllAvatarsPlaceholders.Count > 0 && AllAvatarsPlaceholders[index].SelectionIndicator)
                AllAvatarsPlaceholders[index].SelectionIndicator.gameObject.SetActive(true);
        }

        void SetWeapon(int index)
        {
            WeaponIndex = index;
            PlayerPrefs.SetInt("WeaponIndex", WeaponIndex);

            foreach (var placeholder in AllWeaponsPlaceholders.Where(placeholder => placeholder.Background && placeholder.Button))
            {
                var color = new Color(placeholder.Button.colors.normalColor.a, placeholder.Button.colors.normalColor.g, placeholder.Button.colors.normalColor.b, placeholder.Background.color.a);
                placeholder.Background.color = color;
            }
            
            if (AllWeaponsPlaceholders.Count > 0 && AllWeaponsPlaceholders[index].Background && AllWeaponsPlaceholders[index].Button)
            {
                var color = new Color(AllWeaponsPlaceholders[index].Button.colors.pressedColor.r, AllWeaponsPlaceholders[index].Button.colors.pressedColor.g, AllWeaponsPlaceholders[index].Button.colors.pressedColor.b, AllWeaponsPlaceholders[index].Background.color.a);
                AllWeaponsPlaceholders[index].Background.color = color;
            }

            if (WeaponSpawnPoint)
            {
                if(CurrentWeapon)
                    Destroy(CurrentWeapon);

                CurrentWeapon = Instantiate(AllWeapons[WeaponIndex].weapon.gameObject, WeaponSpawnPoint.position, AllWeapons[WeaponIndex].weapon.transform.rotation);
            }

            if (currentUIManager.MultiplayerGameLobby.LoadoutMenu.Info)
            {
                var info = "";
                
                var name = AllWeapons[WeaponIndex].weapon.name;
                
                var damage = AllWeapons[WeaponIndex].weapon.Attacks[0].AttackType != WeaponsHelper.TypeOfAttack.Bullets ? 
                    AllWeapons[WeaponIndex].weapon.Attacks[0].weapon_damage.ToString() : 
                    AllWeapons[WeaponIndex].weapon.Attacks[0].BulletsSettings[0].weapon_damage.ToString();
                
                var scatter = AllWeapons[WeaponIndex].weapon.Attacks[0].AttackType != WeaponsHelper.TypeOfAttack.Bullets ?
                    AllWeapons[WeaponIndex].weapon.Attacks[0].ScatterOfBullets.ToString() :
                    AllWeapons[WeaponIndex].weapon.Attacks[0].BulletsSettings[0].ScatterOfBullets.ToString();
                
                var rate = AllWeapons[WeaponIndex].weapon.Attacks[0].RateOfAttack.ToString();
                var ammo = AllWeapons[WeaponIndex].weapon.Attacks[0].maxAmmo + " / " + AllWeapons[WeaponIndex].weapon.Attacks[0].inventoryAmmo;
                var weight = AllWeapons[WeaponIndex].weapon.Weight.ToString();
                
                if (showWeaponName)
                    info += "<size=50>"+name+"</size>" + "\n";
                
                if (showWeaponDamage)
                    info += "\n"+ "Damage: " + damage;
                
                if (showWeaponAmmo)
                    info += "\n"+ "Ammo: " + ammo;
                
                if (showWeaponScatter)
                    info += "\n" + "Scatter: "+ scatter;
                
                if (showWeaponRateOfAttack)
                    info += "\n" + "Rate of Shoot: " + rate;
                
                if(showWeaponWeight)
                    info += "\n"+ "Weight: " + weight;

                currentUIManager.MultiplayerGameLobby.LoadoutMenu.Info.text = info;
            }
        }

        void Equip()
        {
            if (selectedWeapons.Contains(WeaponIndex))
            {
                selectedWeapons.Remove(WeaponIndex);
                
                if(AllWeaponsPlaceholders[WeaponIndex].SelectionIndicator)
                    AllWeaponsPlaceholders[WeaponIndex].SelectionIndicator.gameObject.SetActive(false);
            }
            else
            {
                selectedWeapons.Add(WeaponIndex);
                
                if(AllWeaponsPlaceholders[WeaponIndex].SelectionIndicator)
                    AllWeaponsPlaceholders[WeaponIndex].SelectionIndicator.gameObject.SetActive(true);
            }

            var stringValue = "";
            
            foreach (var index in selectedWeapons)
            {
                stringValue += index + ",";
            }
            
            if(stringValue != "")
                stringValue = stringValue.Remove(stringValue.Length - 1);
            
            PlayerPrefs.SetString("SelectedWeapons", stringValue);
        }

        void SetEquippedWeapons()
        {
            var stringValue = PlayerPrefs.GetString("SelectedWeapons");
            
            if(stringValue.Length == 0)
                return;
            
            selectedWeapons.Clear();
            
            selectedWeapons.AddRange(Array.ConvertAll(stringValue.Split(','), int.Parse));

            foreach (var index in selectedWeapons)
            {
                if(AllWeaponsPlaceholders[index].SelectionIndicator)
                    AllWeaponsPlaceholders[index].SelectionIndicator.gameObject.SetActive(true);
            }
        }

        void UpdateMainMenu()
        {
            if(currentUIManager.MultiplayerGameLobby.MainMenu.CurrentModeAndMap)
                currentUIManager.MultiplayerGameLobby.MainMenu.CurrentModeAndMap.text = GameModes[GameModeIndex].Name + " \n" + Maps[MapIndex].Name;

            if (currentUIManager.MultiplayerGameLobby.MainMenu.Avatar)
                currentUIManager.MultiplayerGameLobby.MainMenu.Avatar.texture = DefaultAvatars[AvatarIndex];
        }

        void FindRoomsClick()
        {
            MainMenu.SetActive(false);
            FindRooms.SetActive(true);
        }

        void ChoiceMode()
        {
            MainMenu.SetActive(false);
            GameModesMain.SetActive(true);
        }

        void CreateRoomsClick()
        {
            MainMenu.SetActive(false);
            CreateRooms.SetActive(true);
        }

        void RandomRoomClick()
        {
            var foundRoom = false;

            if (AllRooms.Count > 0)
            {
                foreach (var room in AllRooms)
                {
                    if ((int) room.CustomProperties["gm"] == GameModeIndex && room.IsOpen && room.IsVisible && (string) room.CustomProperties["psw"] == "")
                    {
                        foundRoom = true;
                        PhotonNetwork.JoinRoom(room.Name);
                        break;
                    }
                }
                
                if(!foundRoom)
                    CreateRoom("", "");
            }
            else
            {
                CreateRoom("", "");
            }
        }

        void CreateRoomClick()
        {
            var password = "";
            var gameName = "";

            if (currentUIManager.MultiplayerGameLobby.CreateRoomMenu.Password)
            {
                password = currentUIManager.MultiplayerGameLobby.CreateRoomMenu.Password.text;
            }

            if (currentUIManager.MultiplayerGameLobby.CreateRoomMenu.GameName)
            {
                gameName = currentUIManager.MultiplayerGameLobby.CreateRoomMenu.GameName.text;

                if (gameName == "")
                    gameName = "Game " + Helper.GenerateRandomString(4);
            }

            CreateRoom(password, gameName);
        }

        void JoinSpecificRoom(string name, string password)
        {
            var room = AllRooms.Find(info => info.Name == name);
            
            if (room.PlayerCount < GameModes[(int) room.CustomProperties["gm"]].maxPlayersCount * (GameModes[(int) room.CustomProperties["gm"]].Teams ? 2 : 1) && room.IsOpen)
            {
                if (password != "")
                {
                    if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password && currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton)
                    {
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password.gameObject);
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton.gameObject);

                        currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton.onClick.AddListener(delegate { CheckPassword(name, password); });
                    }

                    seletedRoomName = name;
                }
                else
                {
                    if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password && currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton)
                    {
                        currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password.gameObject.SetActive(false);
                        currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton.gameObject.SetActive(false);
                    }
                    
                    PhotonNetwork.JoinRoom(name);
                }
            }
        }

        void CheckPassword(string name, string password)
        {
            if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password)
            {
                if (password == currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password.text)
                {
                    var room = AllRooms.Find(info => info.Name == name);
                
                    if(room != null && room.PlayerCount < GameModes[(int) room.CustomProperties["gm"]].maxPlayersCount * (GameModes[(int) room.CustomProperties["gm"]].Teams ? 2 : 1) && room.IsOpen)
                        PhotonNetwork.JoinRoom(name);
                }
                else
                { 
                    currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password.text = "";
                    currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password.placeholder.GetComponent<Text>().text = "Wrong Password!";
                }
            }
        }


        //```` load sub menu
        void OpenMenu(string type)
        {
            currentUIManager.HideAllMultiplayerLobbyUI();

            openAnyMenu = true;

            switch (type)
            {
                case "gameModes":
                    currentUIManager.MultiplayerGameLobby.GameModesMenu.ActivateAll();
                    break;

                case "maps":
                    currentUIManager.MultiplayerGameLobby.MapsMenu.ActivateAll();
                    break;

                case "loadout":
                    currentCameraPositions = LoadoutPositions;
                    currentUIManager.MultiplayerGameLobby.LoadoutMenu.ActivateAll();
                    break;
                
                case "avatars":
                    currentUIManager.MultiplayerGameLobby.AvatarsMenu.ActivateAll();
                    break;
                
                case "characters":
                    currentCameraPositions = CharacterPositions;
                    currentUIManager.MultiplayerGameLobby.CharactersMenu.ActivateAll();
                    break;
                
                case "allRooms":
                    currentUIManager.MultiplayerGameLobby.AllRoomsMenu.ActivateAll();
                    break;
                
                case "createGame":
                    currentUIManager.MultiplayerGameLobby.CreateRoomMenu.ActivateAll();

                    if (currentUIManager.MultiplayerGameLobby.CreateRoomMenu.CurrentModeAndMap)
                        currentUIManager.MultiplayerGameLobby.CreateRoomMenu.CurrentModeAndMap.text =  GameModes[GameModeIndex].Name + " | " + Maps[MapIndex].Name;
                    break;
                
                case "mainMenu":
                    
                    UpdateMainMenu();
                    
                    currentCameraPositions = MainMenuPositions;
                    currentUIManager.MultiplayerGameLobby.MainMenu.ActivateAll(isConnected);
                    break;

                case "creditsMenu":

                    currentUIManager.MultiplayerGameLobby.CreditsMenu.ActivateAll();

                    break;

                case "settingsMenu":

                    currentUIManager.MultiplayerGameLobby.SettingsMenu.ActivateAll();

                    break;
            }
        }

        void Connect()
        {
            if (!PhotonNetwork.IsConnected && Helper.GetHtmlFromUri(checkConnectionServer) != "")
            {
                //PhotonNetwork.ConnectToRegion("eu");
                //PhotonNetwork.ConnectUsingSettings();
            }
        }

        void FindRoomsBackButtonClick()
        {
            MainMenu.SetActive(true);
            FindRooms.SetActive(false);
        }

        void CreateRoomsBackButtonClick()
        {
            MainMenu.SetActive(true);
            CreateRooms.SetActive(false);
        }

        void GameModesBackBattonClick()
        {
            MainMenu.SetActive(true);
            GameModesMain.SetActive(false);
        }

        void ChangeRegion(int value)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectToRegion(PUNHelper.ConvertRegionToCode(value));
        }

//        void TimeSliderChange(float value)
//        {
//            TimeValueText.text = "Match Time: " + value + " min";
//            if (TimeSlider.value > TimeSlider.maxValue - 1)
//            {
//                UnlimetedTime = true;
//                TimeValueText.text = "Match Time: ∞";
//            }
//            else
//            {
//                UnlimetedTime = false;
//            }
//        }

//        void MinPlayersChange(float value)
//        {
//            MinPlayersText.text = "Min Players: " + value;
//
//        }
        
//        void MaxPlayersChange(float value)
//        {
//            MaxPlayersText.text = "Max Players: " + value;
//        }

        #endregion

        #region PhotonCallBacks

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                if (AllRooms.Count > 0)
                {
                    if (!AllRooms.Exists(_room => _room.Name == room.Name))
                    {
                        if (room.PlayerCount > 0)
                            AllRooms.Add(room);
                    }
                    else
                    {
                        AllRooms.Remove(room);

                        if (room.PlayerCount <= 0)
                        {
                            if (room.Name == seletedRoomName)
                            {
                                if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password && currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton)
                                {
                                    currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Password.gameObject.SetActive(false);
                                    currentUIManager.MultiplayerGameLobby.AllRoomsMenu.JoinButton.gameObject.SetActive(false);
                                }

                                seletedRoomName = "";
                            }
                        }
                        else
                        {
                            AllRooms.Add(room);
                        }
                    }
                }
                else
                {
                    if (room.PlayerCount > 0)
                        AllRooms.Add(room);
                }
            }

            UpdateRooms();
        }

        void UpdateRooms()
        {
            if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Content)
            {
                foreach (Transform child in currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Content)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var room in AllRooms)
            {
                if (currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Content)
                {
                    if (currentUIManager.MultiplayerGameLobby.roomInfoPlaceholder)
                    {
                        var roomInfo = Instantiate(currentUIManager.MultiplayerGameLobby.roomInfoPlaceholder.gameObject, currentUIManager.MultiplayerGameLobby.AllRoomsMenu.Content);
                        var script = roomInfo.GetComponent<UIPlaceholder>();

                        if ((string) room.CustomProperties["gn"] != "")
                            script.Name.text = (string) room.CustomProperties["gn"];
                        else script.Name.text = "Auto Generated Room";

                        script.Mode.text = GameModes[(int) room.CustomProperties["gm"]].Name;
                        script.Map.text = Maps[(int) room.CustomProperties["m"]].Name;
                        script.Count.text = room.PlayerCount + " / " + GameModes[(int) room.CustomProperties["gm"]].maxPlayersCount * (GameModes[(int) room.CustomProperties["gm"]].Teams ? 2 : 1);
                        
                        if (script.ImagePlaceholder)
                            script.ImagePlaceholder.gameObject.SetActive((string) room.CustomProperties["psw"] != "");
                        
                        script.Button.onClick.AddListener(delegate { JoinSpecificRoom(room.Name, (string) room.CustomProperties["psw"]); });
                    }
                }
            }
        }
        
         public void PlayerManager()
        {
            var player = PhotonNetwork.LocalPlayer;
            
            var room = PhotonNetwork.CurrentRoom;

            //assign members using pointer
            if ((bool) room.CustomProperties["ut"])
            {
                if ((int) room.CustomProperties["rc"] > (int) room.CustomProperties["bc"])
                {
                    player.SetCustomProperties(new Hashtable {{"pt", PUNHelper.Teams.Blue}});

                    var count = (int) room.CustomProperties["bc"];
                    room.SetCustomProperties(new Hashtable {{"bc", count + 1}});
                }
                else if ((int) room.CustomProperties["rc"] < (int) room.CustomProperties["bc"])
                {
                    player.SetCustomProperties(new Hashtable {{"pt", PUNHelper.Teams.Red}});

                    var count = (int) room.CustomProperties["rc"];
                    room.SetCustomProperties(new Hashtable {{"rc", count + 1}});
                }
                else if((int) room.CustomProperties["rc"] == (int) room.CustomProperties["bc"])
                {
                    var teamIndex = Random.Range(0, 2);
                    var team = teamIndex == 0 ? PUNHelper.Teams.Red : PUNHelper.Teams.Blue;

                    player.SetCustomProperties(new Hashtable {{"pt", team}});
                    

                    int count;

                    switch (teamIndex)
                    {
                        case 0:
                            count = (int) room.CustomProperties["rc"];
                            room.SetCustomProperties(new Hashtable {{"rc", count + 1}});
                            break;
                        case 1:
                            count = (int) room.CustomProperties["bc"];
                            room.SetCustomProperties(new Hashtable {{"bc", count + 1}});
                            break;
                    }
                }

                //set the player prefabs according to their team numbers
                int playerType;

                if (PlayerPrefs.HasKey("CurrentCharacter")){
                    playerType = PlayerPrefs.GetInt("CurrentCharacter") % 3;
                }
                else
                {
                    playerType = 0;
                }

                if((PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Blue)
                {
                    //PlayerPrefs.SetInt("CurrentCharacter", playerPrefabNum);
                    SetPlayer(playerType);
                    //print("blue prefabs"+ playerType.ToString());
                }
                else{
                    //PlayerPrefs.SetInt("CurrentCharacter", playerPrefabNum+3);
                    SetPlayer(playerType + 3);
                    //print("red prefabs"+(playerType + 3).ToString());
                }
            }
            else
            {
                if((PUNHelper.CanKillOthers)room.CustomProperties["km"] == PUNHelper.CanKillOthers.OnlyOpponents)
                    room.SetCustomProperties(new Hashtable{{"km", PUNHelper.CanKillOthers.Everyone}});
                
                player.SetCustomProperties(new Hashtable {{"pt", PUNHelper.Teams.Null}});
            }
            
            player.SetCustomProperties(
                new Hashtable {{"km", (PUNHelper.CanKillOthers)room.CustomProperties["km"]}, 
                    {"k", 0}, {"d", 0}, {"s", 0}, {"pl", 0}, {"lft", false}, {"wl", false}, {"ac", false},
                    {"bc", false}, {"cc", false}, {"hpc", false}, {"ai", DefaultAvatars[AvatarIndex].name}, 
                    {"wi", PlayerPrefs.GetString("SelectedWeapons")}, {"cms", 0}});

            if (projectSettings)
            {
                projectSettings.weaponSlots.Clear();
                projectSettings.weaponsIndices.Clear();

                if (GameModes[GameModeIndex].WeaponsToUse == PUNHelper.WeaponsToUse.All)
                {
                    projectSettings.useAllWeapons = true;
                    projectSettings.weaponSlots.AddRange(AllWeapons);
                    projectSettings.weaponsIndices.AddRange(selectedWeapons);
                }
                else
                {
                    projectSettings.useAllWeapons = false;
                    projectSettings.weaponSlots.AddRange(GameModes[GameModeIndex].WeaponsForThisMode);
                }
            }
        }

         public override void OnConnectedToMaster()
         {
             PhotonNetwork.AutomaticallySyncScene = false;
             
             if (!PhotonNetwork.InLobby)
             {
                 PhotonNetwork.JoinLobby(TypedLobby.Default);
             }
         }

         public override void OnJoinedLobby()
        {
            if (currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "Connected | Ping - " + PhotonNetwork.GetPing() + " ms"; //PhotonNetwork.CloudRegion.Substring(0, PhotonNetwork.CloudRegion.Length - 2).ToUpper();

            if (currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown)
            {
                currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.gameObject.SetActive(true);

                if (firstTake)
                {
                    currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.value = PUNHelper.ConvertCodeToRegion(PhotonNetwork.CloudRegion);
                    currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.onValueChanged.AddListener(ChangeRegion);
                    firstTake = false;
                }
            }

            isConnected = true;

//            if(currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton)
//                currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton.gameObject.SetActive(false);
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton.interactable = true;
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton.interactable = true;
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton.interactable = true;

            if (launchAgain)
            {
                StartCoroutine(LaunchAgainTimeout());
                PlayerPrefs.SetInt("LaunchAgain", 0);
                launchAgain = false;
            }
        }


        IEnumerator LaunchAgainTimeout()
        {
            yield return new WaitForSeconds(3);
            RandomRoomClick();
        }
        
//        IEnumerator LoadLevel()
//        {
//            yield return new WaitForSeconds(0.5f);
//            PhotonNetwork.LoadLevel(Maps[(int) PhotonNetwork.CurrentRoom.CustomProperties["m"]].Name);
//        }

        IEnumerator CheckInternetConnection()
        {
            while (true)
            {
                yield return new WaitForSeconds(3);

                if (Helper.GetHtmlFromUri(checkConnectionServer) != "")
                {
                    Debug.Log(Helper.GetHtmlFromUri(checkConnectionServer));

                    if (currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                        currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "Disconnected from Server";

                    if (currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown)
                        currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.gameObject.SetActive(true);

//                    if(currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton)
//                        currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton.gameObject.SetActive(true);

                    PhotonNetwork.ConnectUsingSettings();
                    
                    StopCoroutine(CheckInternetConnection());
                    break;
                }
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            print("Failed create room: " + returnCode + "\n" + message);
        }

        public override void OnCreatedRoom()
        {
            //print("RoomManager is created");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            print("Game didn't found, but a new one has been created");
            CreateRoom("", "");
        }

        public override void OnJoinedRoom()
        {
            foreach (var player in PhotonNetwork.PlayerListOthers)
            {
                if (player.NickName == PhotonNetwork.NickName)
                {
                    PhotonNetwork.NickName = PhotonNetwork.NickName + " (" + Random.Range(100, 10000) + ")";
                }
            }

            //initialize the players' information (team number)
            PlayerManager();

            //load new scene
            //PhotonNetwork.LoadLevel(Maps[(int) PhotonNetwork.CurrentRoom.CustomProperties["m"]].Name);
            PhotonNetwork.LoadLevel("WaitingRoom");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "loadStatus", 0 } });
            print("loading map "+ Maps[(int)PhotonNetwork.CurrentRoom.CustomProperties["m"]].Name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "Disconnected from Server";
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.PlayButton.interactable = false;
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.AllRoomsButton.interactable = false;
            
            if (currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton)
                currentUIManager.MultiplayerGameLobby.MainMenu.CreateRoomButton.interactable = false;
            
            isConnected = false;
            
            if (Helper.GetHtmlFromUri(checkConnectionServer) == "")
            {
                if (currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus)
                    currentUIManager.MultiplayerGameLobby.MainMenu.ConnectionStatus.text = "No Internet Connection";

                if (currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown)
                    currentUIManager.MultiplayerGameLobby.MainMenu.RegionsDropdown.gameObject.SetActive(false);
//                
//                if(currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton)
//                    currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton.gameObject.SetActive(false);

                StartCoroutine(CheckInternetConnection());
            }
            else
            {
//                if(currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton)
//                    currentUIManager.MultiplayerGameLobby.MainMenu.ConnectButton.gameObject.SetActive(true);
            }
        }

        #endregion

#endif
    }
}




