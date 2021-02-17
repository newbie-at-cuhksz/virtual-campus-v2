using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if PHOTON_UNITY_NETWORKING
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

namespace GercStudio.USK.Scripts
{
    [RequireComponent(typeof(PickUpManager))]
    public class RoomManager :
#if PHOTON_UNITY_NETWORKING
        MonoBehaviourPunCallbacks
#else
    MonoBehaviour
#endif
    {
        public List<SpawnZone> PlayersSpawnAreas;
        public List<SpawnZone> RedTeamSpawnAreas;
        public List<SpawnZone> BlueTeamSpawnAreas;

        public PUNHelper.MatchTarget MatchTarget;

        public UIManager UiManager;
        public UIManager currentUIManager;

        public ProjectSettings projectSettings;

        public List<GameObject> Enemies;
        public GameObject CharacterCamera;
        public GameObject Player;

        public List<CapturePoint> HardPoints;
        public CapturePoint CurrentHardPoint;
        public CapturePoint A_Point; 
        public CapturePoint B_Point; 
        public CapturePoint C_Point;

        public List<Camera> SpectateCameras;
        public Camera DefaultCamera;

#if PHOTON_UNITY_NETWORKING

        public List<Player> deadPlayers = new List<Player>();
        public List<Player> leftPlayers = new List<Player>();

#endif
       
        public float TimeForMatch;
        public int TargetScore;
        public int inspectorTab;
        public int generateSpawmPointFunctionCount;
        
        public bool useTeams;
        public bool foundSpawnPoint;
        

#if PHOTON_UNITY_NETWORKING
        private Controller controller;
        
        private float addScoresTimer;
        private float getCaptureScoresTimeout;
        private float restartTime = 5;
        private float findPlayersTimer;
        private float startMatchTimer;
        private float changeHardPointTimer;
        private float changeHardPointTimeout;

        private double startTime = -1;
        private double pastMatchTime;
        private double leftMatchTime;

        private int hardPointIndex; 
        private int spectateCameraIndex = -1;
        private int roundsCount;
        private int currentRound;
        private int captureScore;
        private int pointsCount;

        private bool isRestartTimer;
        public bool isPause;
        private bool isEnemies;
        private bool useMatchTime;
        private bool gameOver;
        private bool wasPreMatchGame;
        public bool canPause;
        private bool captureImmediately;
        public bool GameStarted;
        
        private GameObject playerAvatarPlaceholder;

        private PUNHelper.Teams aLastCapturedTeam;
        private PUNHelper.Teams bLastCapturedTeam;
        private PUNHelper.Teams cLastCapturedTeam;
        private PUNHelper.Teams hardPointLastCapturedTeam;

        public float preMatchTimer = 10;

        private Color32 curFirstPlaceBackgroundColor = new Color32(0,0,0,0);
        private Color32 curPlayerBackgroundColor = new Color32(0,0,0,0);
#endif
        

        void Awake()
        {
#if !PHOTON_UNITY_NETWORKING
        Debug.LogWarning("To use the multiplayer mode, import PUN2 from Asset Store.");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif

#else
            if (!PhotonNetwork.InRoom)
            {
                Debug.LogWarning("You aren't in the Photon.RoomManager - Connect to this scene in the Lobby.");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                if (UiManager)
                {
                    currentUIManager = Instantiate(UiManager.gameObject).GetComponent<UIManager>();
                }
                else
                {
                    Debug.LogError("UI Manager was not be loaded.");
                }

                currentUIManager.HideAllMultiplayerRoomUI();
                currentUIManager.HideAllMultiplayerLobbyUI();
                
                if(currentUIManager.SinglePlayerGame.PauseMainObject)
                    currentUIManager.SinglePlayerGame.PauseMainObject.SetActive(false);
                
                if(currentUIManager.SinglePlayerGame.OptionsMainObject)
                    currentUIManager.SinglePlayerGame.OptionsMainObject.SetActive(false);
                
                currentUIManager.CharacterUI.Inventory.ActivateAll();
                currentUIManager.CharacterUI.DisableAll();

                var removedCameras = new List<Camera>();
                
                foreach (var camera in SpectateCameras)
                {
                    if(camera)
                        camera.gameObject.SetActive(false);
                    else removedCameras.Add(camera);
                }

                foreach (var removedCamera in removedCameras)
                {
                    SpectateCameras.Remove(removedCamera);
                }
                
                Time.timeScale = 1;

                captureImmediately = (bool) PhotonNetwork.CurrentRoom.CustomProperties["ci"];
                
                getCaptureScoresTimeout = (float) PhotonNetwork.CurrentRoom.CustomProperties["cpt"];
                
                captureScore = (int) PhotonNetwork.CurrentRoom.CustomProperties["cps"];
                
                pointsCount = (int) PhotonNetwork.CurrentRoom.CustomProperties["pc"];

                useTeams = (bool) PhotonNetwork.CurrentRoom.CustomProperties["ut"];
                
                changeHardPointTimeout = (int) PhotonNetwork.CurrentRoom.CustomProperties["hpt"];

                roundsCount = (int) PhotonNetwork.CurrentRoom.CustomProperties["r"];

                currentRound = (int) PhotonNetwork.CurrentRoom.CustomProperties["cr"];

                MatchTarget = (PUNHelper.MatchTarget) PhotonNetwork.CurrentRoom.CustomProperties["tar"];

                TargetScore = (int) PhotonNetwork.CurrentRoom.CustomProperties["tv"];

                if ((int) PhotonNetwork.CurrentRoom.CustomProperties["tl"] > 0)
                {
                    TimeForMatch = (int) PhotonNetwork.CurrentRoom.CustomProperties["tl"] * 60;
                    useMatchTime = true;
                }

                if (MatchTarget == PUNHelper.MatchTarget.WithoutTarget)
                    preMatchTimer = 0.1f;

                if (MatchTarget == PUNHelper.MatchTarget.Domination)
                {
                    foreach (var point in HardPoints.Where(point => point))
                    {
                        point.gameObject.SetActive(false);
                    }
                    
                    if (pointsCount == 1)
                    {
                        changeHardPointTimer = changeHardPointTimeout;

                        CurrentHardPoint = HardPoints[(int) PhotonNetwork.CurrentRoom.CustomProperties["chp"]];

                        if (!CurrentHardPoint)
                            Debug.LogWarning("You have not set all Hard Points for this scene.");
                    }
                    else
                    {
                        if (A_Point)
                            A_Point.gameObject.SetActive(true);
                        else Debug.LogWarning("There is not the 'A' Capture point in the scene.");
                        
                        if (B_Point)
                            B_Point.gameObject.SetActive(true);
                        else Debug.LogWarning("There is not the 'B' Capture point in the scene.");
                        
                        if (C_Point)
                            C_Point.gameObject.SetActive(true);
                        else Debug.LogWarning("There is not the 'C' Capture point in the scene.");
                    }
                }
                else
                {
                    if (A_Point)
                        A_Point.gameObject.SetActive(false);

                    if (B_Point)
                        B_Point.gameObject.SetActive(false);
                        
                    if (C_Point)
                        C_Point.gameObject.SetActive(false);
                }
                
//                PhotonNetwork.IsMessageQueueRunning = true;
                

                if (currentUIManager.MultiplayerGameRoom.PauseMenu.CurrentGameAndPassword)
                {
                    var password = (string) PhotonNetwork.CurrentRoom.CustomProperties["psw"];
                    var name = (string) PhotonNetwork.CurrentRoom.CustomProperties["gn"];

                    if (name != "")
                        currentUIManager.MultiplayerGameRoom.PauseMenu.CurrentGameAndPassword.text = "game name: " + name;
                    
                    if(password != "")
                        currentUIManager.MultiplayerGameRoom.PauseMenu.CurrentGameAndPassword.text += " | password: " + password;
                }

                if (currentUIManager.MultiplayerGameRoom.PauseMenu.ExitButton)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.ExitButton.onClick.AddListener(LeaveMatch);

                if (currentUIManager.MultiplayerGameRoom.PauseMenu.ResumeButton)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.ResumeButton.onClick.AddListener(delegate { Pause(true); });
                
                if(currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamName)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamName.text = PlayerPrefs.GetString("RedTeamName");
                
                if(currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamName)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamName.text = PlayerPrefs.GetString("BlueTeamName");
                
                if(currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamName)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamName.text = PlayerPrefs.GetString("RedTeamName");
                
                if(currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamName)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamName.text = PlayerPrefs.GetString("BlueTeamName");

                if (currentUIManager.MultiplayerGameRoom.GameOverMenu.PlayAgainButton)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.PlayAgainButton.onClick.AddListener(PlayAgain);

                if (currentUIManager.MultiplayerGameRoom.GameOverMenu.MatchStatsButton)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.MatchStatsButton.onClick.AddListener(delegate { OpenMatchStats("GameOver"); });
                
                if(currentUIManager.MultiplayerGameRoom.GameOverMenu.ExitButton)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.ExitButton.onClick.AddListener(LeaveMatch);
                
                if(currentUIManager.MultiplayerGameRoom.GameOverMenu.BackButton)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.BackButton.onClick.AddListener(delegate { CloseMatchStats("GameOver"); });

                if (currentUIManager.MultiplayerGameRoom.TimerAfterDeath.LaunchButton)
                    currentUIManager.MultiplayerGameRoom.TimerAfterDeath.LaunchButton.onClick.AddListener(LaunchGameAfterDeath);
                
                if(currentUIManager.MultiplayerGameRoom.SpectateMenu.ExitButton)
                    currentUIManager.MultiplayerGameRoom.SpectateMenu.ExitButton.onClick.AddListener(LeaveMatch);
                
                if(currentUIManager.MultiplayerGameRoom.StartMenu.ExitButton)
                    currentUIManager.MultiplayerGameRoom.StartMenu.ExitButton.onClick.AddListener(LeaveMatch);
                
                if(currentUIManager.MultiplayerGameRoom.SpectateMenu.ChangeCameraButton)
                    currentUIManager.MultiplayerGameRoom.SpectateMenu.ChangeCameraButton.onClick.AddListener(ChooseSpectateCamera);
                
                if(currentUIManager.MultiplayerGameRoom.SpectateMenu.MatchStatsButton)
                    currentUIManager.MultiplayerGameRoom.SpectateMenu.MatchStatsButton.onClick.AddListener(delegate { OpenMatchStats("Spectate"); });

                if(currentUIManager.MultiplayerGameRoom.SpectateMenu.BackButton)
                    currentUIManager.MultiplayerGameRoom.SpectateMenu.BackButton.onClick.AddListener(delegate { CloseMatchStats("Spectate"); });

                if (currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground)
                    curFirstPlaceBackgroundColor = currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground.color;
                
                if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsBackground)
                    curPlayerBackgroundColor = currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsBackground.color;

                if(DefaultCamera)
                    DefaultCamera.gameObject.SetActive(true);

                StartCoroutine(FindPlayersTimeout());
            }
#endif
        }
#if PHOTON_UNITY_NETWORKING
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["e"])
                    isEnemies = true;
            }

            ClearPlayerList(false);
            AddPlayersToList(true);

            StartCoroutine(GetPlayerPropertiesAndSetParameters());
        }

        private void Update()
        {
            if (!Player) return;

            if (!controller)
                controller = Player.GetComponent<Controller>();
            
            
            if (controller.projectSettings.ButtonsActivityStatuses[10] && (Input.GetKeyDown(controller._gamepadCodes[10]) || Input.GetKeyDown(controller._keyboardCodes[10]) ||
                Helper.CheckGamepadAxisButton(10, controller._gamepadButtonsAxes, controller.hasAxisButtonPressed, "GetKeyDown", controller.projectSettings.AxisButtonValues[10])))
            {
                Pause(true);
            }

            if(!GameStarted)
                return;


            if (useTeams)
            {
                if (MatchTarget == PUNHelper.MatchTarget.Domination)
                {
                    if (pointsCount == 3)
                    {
                        PUNHelper.CheckPoint("ac", ref aLastCapturedTeam, currentUIManager, this, captureImmediately, A_Point.gameObject, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget);
                        PUNHelper.CheckPoint("bc", ref bLastCapturedTeam, currentUIManager, this, captureImmediately, B_Point.gameObject, currentUIManager.MultiplayerGameRoom.MatchStats.B_ScreenTarget);
                        PUNHelper.CheckPoint("cc", ref cLastCapturedTeam, currentUIManager, this, captureImmediately, C_Point.gameObject, currentUIManager.MultiplayerGameRoom.MatchStats.C_ScreenTarget);

                        PUNHelper.SetScreenTarget(currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget, A_Point.gameObject, controller);
                        PUNHelper.SetScreenTarget(currentUIManager.MultiplayerGameRoom.MatchStats.B_ScreenTarget, B_Point.gameObject, controller);
                        PUNHelper.SetScreenTarget(currentUIManager.MultiplayerGameRoom.MatchStats.C_ScreenTarget, C_Point.gameObject, controller);
                        
;                        if (PhotonNetwork.IsMasterClient)
                        {
                            addScoresTimer += Time.deltaTime;

                            if (addScoresTimer >= getCaptureScoresTimeout)
                            {
                                PUNHelper.CalculateCapturedScores("act", captureScore);
                                PUNHelper.CalculateCapturedScores("bct", captureScore);
                                PUNHelper.CalculateCapturedScores("cct", captureScore);

                                addScoresTimer = 0;
                            }
                        }
                    }
                    else
                    {
                        if (HardPoints.Count == 0 || !CurrentHardPoint) return;
                        
                        changeHardPointTimer -= Time.deltaTime;

                        if (currentUIManager.MultiplayerGameRoom.MatchStats.HardPointTimer)
                            currentUIManager.MultiplayerGameRoom.MatchStats.HardPointTimer.text = PUNHelper.FormatTime(changeHardPointTimer);

                        if (changeHardPointTimer <= 0)
                        {
                            changeHardPointTimer = changeHardPointTimeout;
                            
                            CurrentHardPoint.gameObject.SetActive(false);

                            hardPointIndex++;
                            if (hardPointIndex > HardPoints.Count - 1)
                                hardPointIndex = 0;

                            CurrentHardPoint = HardPoints[hardPointIndex];
                            
                            if(PhotonNetwork.IsMasterClient)
                                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{"chp", hardPointIndex}});

                            CurrentHardPoint.gameObject.SetActive(true);

                            PUNHelper.ZeroizeHardPoint(currentUIManager, ref hardPointLastCapturedTeam, CurrentHardPoint.gameObject, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget);
                        }

                        PUNHelper.CheckPoint("hpc", ref hardPointLastCapturedTeam, currentUIManager, this, captureImmediately, CurrentHardPoint.gameObject, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget);
                        
                        PUNHelper.SetScreenTarget(currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget, CurrentHardPoint.gameObject, controller);

                        if (PhotonNetwork.IsMasterClient)
                        {
                            addScoresTimer += Time.deltaTime;

                            if (addScoresTimer >= getCaptureScoresTimeout)
                            {
                                PUNHelper.CalculateCapturedScores("hpct", captureScore);
                                addScoresTimer = 0;
                            }
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if(!PhotonNetwork.InRoom)
                return;

            
            if(!GameStarted)//!(bool)PhotonNetwork.CurrentRoom.CustomProperties["gs"]
                return;


            if (roundsCount == 1)
            {
                if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamTotalWins)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamTotalWins.text = " ";

                if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins.text = " ";

                if (currentUIManager.MultiplayerGameRoom.MatchStats.TargetText)
                {
                    switch (MatchTarget)
                    {
                        case PUNHelper.MatchTarget.Points:
                        case PUNHelper.MatchTarget.Domination:
                            currentUIManager.MultiplayerGameRoom.MatchStats.TargetText.text = TargetScore + " scores to win";
                            break;
                        case PUNHelper.MatchTarget.Kills:
                            currentUIManager.MultiplayerGameRoom.MatchStats.TargetText.text = TargetScore + " kills to win";
                            break;
                        case PUNHelper.MatchTarget.Survive:
                            currentUIManager.MultiplayerGameRoom.MatchStats.TargetText.text = "Kill everyone to win";
                            break;
                    }
                }
            }
            else
            {
                if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamTotalWins)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamTotalWins.text = "ROUNDS WON " + (int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"];

                if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins.text = "ROUNDS WON " + (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"];


                if (currentUIManager.MultiplayerGameRoom.MatchStats.TargetText)
                {
                    switch (MatchTarget)
                    {
                        case PUNHelper.MatchTarget.Points:
                        case PUNHelper.MatchTarget.Domination:
                            currentUIManager.MultiplayerGameRoom.MatchStats.TargetText.text = "Round: " + currentRound + "/" + roundsCount + " | " + TargetScore + " scores to win";
                            break;
                        case PUNHelper.MatchTarget.Kills:
                            currentUIManager.MultiplayerGameRoom.MatchStats.TargetText.text = "Round: " + currentRound + "/" + roundsCount + " | " + TargetScore + " kills to win";
                            break;
                        case PUNHelper.MatchTarget.Survive:
                            currentUIManager.MultiplayerGameRoom.MatchStats.TargetText.text = "Round: " + currentRound + "/" + roundsCount + " | " + "Kill everyone to win";
                            break;
                    }
                }
            }
            

            if (MatchTarget != PUNHelper.MatchTarget.Survive)
            {
                if (useTeams)
                {
                    if (currentUIManager.MultiplayerGameRoom.MatchStats.RedTeamMatchStats)
                        currentUIManager.MultiplayerGameRoom.MatchStats.RedTeamMatchStats.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"]).ToString();

                    if (currentUIManager.MultiplayerGameRoom.MatchStats.BlueTeamMatchStats)
                        currentUIManager.MultiplayerGameRoom.MatchStats.BlueTeamMatchStats.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["bs"]).ToString();

                    if (MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                    {
                        if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamScore)
                            currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"]).ToString();

                        if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamScore)
                            currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["bs"]).ToString();

                        if (((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"] >= TargetScore || (int) PhotonNetwork.CurrentRoom.CustomProperties["bs"] >= TargetScore) && !gameOver)
                        {
                            GameOver();
                        }
                    }
                    else
                    {
                        if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamScore)
                            currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamScore.text = " ";

                        if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamScore)
                            currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamScore.text = " ";
                    }
                }
                else
                {
                    if (MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                    {
                        foreach (var player in PhotonNetwork.PlayerList)
                        {
                            if (player.CustomProperties.Count <= 0) continue;

                            if (MatchTarget == PUNHelper.MatchTarget.Kills)
                            {
                                if ((int) player.CustomProperties["k"] >= TargetScore && !gameOver)
                                {
                                    GameOver();
                                }
                            }
                            else
                            {
                                if ((int) player.CustomProperties["cms"] >= TargetScore && !gameOver)
                                {
                                    GameOver();
                                }
                            }
                        }


                        var players = PhotonNetwork.PlayerList;
                        var sortPlayers = MatchTarget == PUNHelper.MatchTarget.Kills ? players.OrderByDescending(t => t.CustomProperties["k"]).ThenBy(t => t.CustomProperties["d"]).ToArray() : players.OrderByDescending(t => t.CustomProperties["cms"]).ToArray();

                        if (currentUIManager.MultiplayerGameRoom.MatchStats.CurrentPlaceText)
                        {
                            for (int i = 0; i < sortPlayers.Length; i++)
                            {
                                if (sortPlayers[i].IsLocal)
                                    currentUIManager.MultiplayerGameRoom.MatchStats.CurrentPlaceText.text = (i + 1).ToString();
                            }
                        }

                        if (sortPlayers[0].IsLocal)
                        {
                            if (currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground)
                                currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground.color = currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsHighlight;

                            if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStats && sortPlayers.Length > 1)
                                currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStats.text = "#2 - " + (MatchTarget == PUNHelper.MatchTarget.Kills ? sortPlayers[1].CustomProperties["k"] : sortPlayers[1].CustomProperties["cms"]);

                            if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsBackground)
                                currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsBackground.color = curPlayerBackgroundColor;
                        }
                        else
                        {
                            if (currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground)
                                currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground.color = curFirstPlaceBackgroundColor;

                            if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsBackground)
                                currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsBackground.color = currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStatsHighlight;

                            for (int i = 0; i < sortPlayers.Length; i++)
                            {
                                if (sortPlayers[i].IsLocal)
                                    if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStats)
                                        currentUIManager.MultiplayerGameRoom.MatchStats.PlayerStats.text = "#" + (i + 1) + " - " + (MatchTarget == PUNHelper.MatchTarget.Kills ? sortPlayers[i].CustomProperties["k"] : sortPlayers[i].CustomProperties["cms"]);
                            }
                        }

                        if (currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStats)
                            currentUIManager.MultiplayerGameRoom.MatchStats.FirstPlaceStats.text = "#1 - " + (MatchTarget == PUNHelper.MatchTarget.Kills ? sortPlayers[0].CustomProperties["k"] : sortPlayers[0].CustomProperties["cms"]);
                    }
                }
            }
            else
            {
                if (useTeams)
                {
                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamScore)
                        currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamScore.text = " ";

                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamScore)
                        currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamScore.text = " ";
                    
                    if ((PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Red)) == 0 ||
                         PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Blue)) == 0 ) 
                        && !gameOver)
                    {
                        GameOver();
                    }
                }
                else
                {
                    if (PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList()) == 1 && !gameOver)
                    {
                        GameOver();
                    }
                }
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("mt"))
            {
                if (GameStarted && useMatchTime && !gameOver)
                {
                    if (startTime == -1)
                    {
                        startTime = (double) PhotonNetwork.CurrentRoom.CustomProperties["mt"];
                    }

                    pastMatchTime = PhotonNetwork.Time - startTime;
                    leftMatchTime = TimeForMatch - pastMatchTime;

                    currentUIManager.MultiplayerGameRoom.MatchStats.MatchTimer.text = "Time left: " + PUNHelper.FormatTime(leftMatchTime);

                    if (leftMatchTime < 0)
                        GameOver();
                }
                else if (!useMatchTime)
                {
                    if (currentUIManager.MultiplayerGameRoom.MatchStats.MatchTimer)
                        currentUIManager.MultiplayerGameRoom.MatchStats.MatchTimer.gameObject.SetActive(false);
                }
            }
        }
        
        

        public void ClearPlayerList(bool onlyStartMenu)
        {
            if (!onlyStartMenu)
            {
                if (useTeams)
                {
                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamContent)
                    {
                        for (var i = currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamContent.childCount - 1; i >= 0; i--)
                        {
                            Destroy(currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamContent.GetChild(i).gameObject);
                        }
                    }

                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamContent)
                    {
                        for (var i = currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamContent.childCount - 1; i >= 0; i--)
                        {
                            Destroy(currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamContent.GetChild(i).gameObject);
                        }
                    }
                }
                else
                {
                    if (!currentUIManager.MultiplayerGameRoom.PauseMenu.NotTeamsContent) return;

                    for (var i = currentUIManager.MultiplayerGameRoom.PauseMenu.NotTeamsContent.childCount - 1; i >= 0; i--)
                    {
                        Destroy(currentUIManager.MultiplayerGameRoom.PauseMenu.NotTeamsContent.GetChild(i).gameObject);
                    }
                }
            }

            if (currentUIManager.MultiplayerGameRoom.StartMenu.PlayersContent)
            {
                for (var i = currentUIManager.MultiplayerGameRoom.StartMenu.PlayersContent.childCount - 1; i >= 0; i--)
                {
                    Destroy(currentUIManager.MultiplayerGameRoom.StartMenu.PlayersContent.GetChild(i).gameObject);
                }
            }
        }

        public void AddPlayersToList(bool onlyStartMenu)
        {
            if (!PhotonNetwork.InRoom) return;
            
            var _players = PhotonNetwork.PlayerList.ToList();//MatchTarget == PUNHelper.MatchTarget.Survive ? allPlayers : PhotonNetwork.PlayerList.ToList();

            if (!onlyStartMenu)
            {
                if (useTeams)
                {
                    var redSortPlayers = PUNHelper.ManagePlayers(_players.Where(player => (PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Red).ToList(), MatchTarget == PUNHelper.MatchTarget.Survive);
                    var blueSortPlayers = PUNHelper.ManagePlayers(_players.Where(player => (PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Blue).ToList(), MatchTarget == PUNHelper.MatchTarget.Survive);

                    redSortPlayers.AddRange(leftPlayers.Where(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Red));
                    blueSortPlayers.AddRange(leftPlayers.Where(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Blue));

//                    if (leftPlayers.Count > 0)
//                    {
//                        foreach (var player in leftPlayers)
//                        {
//                            if((PUNHelper.Teams)player.CustomProperties["t"] == PUNHelper.Teams.Red)
//                                redSortPlayers.Add(player);
//                            else blueSortPlayers.Add(player);
//                        }
//                    }

                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamContent)
                        InstantiateCharactersInfo(redSortPlayers, currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamContent, false, MatchTarget == PUNHelper.MatchTarget.Survive);

                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamContent)
                        InstantiateCharactersInfo(blueSortPlayers, currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamContent, false, MatchTarget == PUNHelper.MatchTarget.Survive);
                }
                else
                {
                    var sortPlayers = PUNHelper.ManagePlayers(_players, MatchTarget == PUNHelper.MatchTarget.Survive);
                    
                    if(leftPlayers.Count > 0)
                        sortPlayers.AddRange(leftPlayers);
                    
                    if (currentUIManager.MultiplayerGameRoom.PauseMenu.NotTeamsContent)
                        InstantiateCharactersInfo(sortPlayers.ToList(), currentUIManager.MultiplayerGameRoom.PauseMenu.NotTeamsContent, false, MatchTarget == PUNHelper.MatchTarget.Survive);
                }
            }
            else
            {
                if (currentUIManager.MultiplayerGameRoom.StartMenu.PlayersContent)
                    StartCoroutine(InstantiateCharactersInfoTimeout());
            }
        }

        public void ClearPlayersIcons()
        {
            if (!useTeams)
            {
                if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayersList)
                {
                    for (var i = currentUIManager.MultiplayerGameRoom.MatchStats.PlayersList.childCount - 1; i >= 0; i--)
                    {
                        Destroy(currentUIManager.MultiplayerGameRoom.MatchStats.PlayersList.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                if (currentUIManager.MultiplayerGameRoom.MatchStats.RedTeamPlayersList)
                {
                    for (var i = currentUIManager.MultiplayerGameRoom.MatchStats.RedTeamPlayersList.childCount - 1; i >= 0; i--)
                    {
                        Destroy(currentUIManager.MultiplayerGameRoom.MatchStats.RedTeamPlayersList.GetChild(i).gameObject);
                    } 
                }

                if (currentUIManager.MultiplayerGameRoom.MatchStats.BlueTeamPlayersList)
                {
                    for (var i = currentUIManager.MultiplayerGameRoom.MatchStats.BlueTeamPlayersList.childCount - 1; i >= 0; i--)
                    {
                        Destroy(currentUIManager.MultiplayerGameRoom.MatchStats.BlueTeamPlayersList.GetChild(i).gameObject);
                    } 
                }
                
            }
        }

        public void AddPlayersIcons()
        {
            if (!useTeams)
            {
                if (currentUIManager.MultiplayerGameRoom.MatchStats.PlayersList && playerAvatarPlaceholder)
                {
                    PUNHelper.InstantiateIcons(PhotonNetwork.PlayerList.ToList(), leftPlayers, deadPlayers, currentUIManager.MultiplayerGameRoom.MatchStats.PlayersList, playerAvatarPlaceholder.GetComponent<RawImage>());
                }
            }
            else
            {
                PUNHelper.InstantiateIcons(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Red), 
                    leftPlayers.FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Red), 
                    deadPlayers.FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Red), 
                    currentUIManager.MultiplayerGameRoom.MatchStats.RedTeamPlayersList, playerAvatarPlaceholder.GetComponent<RawImage>());
                
                PUNHelper.InstantiateIcons(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Blue), 
                    leftPlayers.FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Blue), 
                    deadPlayers.FindAll(player => (PUNHelper.Teams)player.CustomProperties["pt"] == PUNHelper.Teams.Blue), 
                    currentUIManager.MultiplayerGameRoom.MatchStats.BlueTeamPlayersList, playerAvatarPlaceholder.GetComponent<RawImage>());
            }
        }

        IEnumerator GetPlayerPropertiesAndSetParameters()
        {
            yield return new WaitForSeconds(2);
            
            playerAvatarPlaceholder = new GameObject("Avatar Placeholder");
            var image = playerAvatarPlaceholder.AddComponent<RawImage>();
            image.texture = Resources.Load((string)PhotonNetwork.LocalPlayer.CustomProperties["ai"]) as Texture;

            playerAvatarPlaceholder.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
               
            var layoutElement = playerAvatarPlaceholder.AddComponent<LayoutElement>();
            layoutElement.minWidth = 10;
            layoutElement.minHeight = 10;
            layoutElement.preferredHeight = 50;
            layoutElement.preferredWidth = 50;
            layoutElement.flexibleHeight = 2;

            playerAvatarPlaceholder.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                
            playerAvatarPlaceholder.AddComponent<Outline>().effectColor = Color.black;
            
            if (useTeams)
            {
                if (currentUIManager.MultiplayerGameRoom.MatchStats.TeamImagePlaceholder)
                    currentUIManager.MultiplayerGameRoom.MatchStats.TeamImagePlaceholder.texture = 
                        (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? 
                            Resources.Load(PlayerPrefs.GetString("RedTeamLogo")) as Texture :
                            Resources.Load(PlayerPrefs.GetString("BlueTeamLogo")) as Texture;

                if (currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamLogoPlaceholder)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamLogoPlaceholder.texture = Resources.Load(PlayerPrefs.GetString("BlueTeamLogo")) as Texture;

                if (currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamLogoPlaceholder)
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamLogoPlaceholder.texture = Resources.Load(PlayerPrefs.GetString("RedTeamLogo")) as Texture;
            }

            StopCoroutine(GetPlayerPropertiesAndSetParameters());
        }


        IEnumerator InstantiateCharactersInfoTimeout()
        {
            yield return new WaitForSeconds(1);
            InstantiateCharactersInfo(PhotonNetwork.PlayerList.ToList(), currentUIManager.MultiplayerGameRoom.StartMenu.PlayersContent, true, MatchTarget == PUNHelper.MatchTarget.Survive);
            StopCoroutine(InstantiateCharactersInfoTimeout());
        }
        
        void InstantiateCharactersInfo(List<Player> players, Transform parent, bool startMenu, bool isSurvival)
        {
            var _players = players;//isSurvival && !startMenu ? allPlayers : players;
            
            for (var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];

                if (player.CustomProperties.Count <= 0) continue;
                
                if (currentUIManager.MultiplayerGameRoom.playerInfoPlaceholder)
                {
                    var tempPrefab = Instantiate(currentUIManager.MultiplayerGameRoom.playerInfoPlaceholder.gameObject, parent);
                    var tempScript = tempPrefab.GetComponent<UIPlaceholder>();
                    
                    tempPrefab.SetActive(true);
                    
                    if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["wpmg"])
                    {
                        if (tempScript.Name)
                        {
                            tempScript.Name.text = player.NickName + ((bool) player.CustomProperties["lft"] ? " - (Left)" : " ") + PUNHelper.EmptyLine;
                        }
                    }
                    else
                    {
                        if (!startMenu)
                        {
                            if (!(bool) player.CustomProperties["lft"])
                            {
                                var status = (bool) player.CustomProperties["wl"] ? "Loaded" : "Loading";
                                
                                if(tempScript.Name)
                                    tempScript.Name.text = player.NickName + " - (" + status + ")" + PUNHelper.EmptyLine;;
                            }
                            else
                            {
                                if(tempScript.Name)
                                    tempScript.Name.text = player.NickName + " - (Left)" + PUNHelper.EmptyLine;
                            }
                        }
                        else
                        {
                            if(tempScript.Name)
                                tempScript.Name.text = player.NickName + PUNHelper.EmptyLine;
                        }
                    }

                    if(tempScript.KD)
                        tempScript.KD.text = !startMenu ? player.CustomProperties["k"] + " / " + player.CustomProperties["d"] : " ";
                    
                    if(tempScript.Rank)
                        tempScript.Rank.text = (i + 1).ToString();

                    if (tempScript.Score)
                        tempScript.Score.text = startMenu ? " " : ((int)player.CustomProperties["cms"]).ToString();

                    if (tempScript.Icon)
                    {
                        tempScript.Icon.texture = Resources.Load((string)player.CustomProperties["ai"]) as Texture;
                    }

                    if(player.NickName == PhotonNetwork.LocalPlayer.NickName)
                        if (tempPrefab.GetComponent<Image>())
                            tempPrefab.GetComponent<Image>().color = tempScript.HighlightedColor;
                }
            }
        }

        void SpawnEnemies()
        {
            if (isEnemies && PhotonNetwork.IsMasterClient)
            {
//                _timeout += Time.deltaTime;
                
//                if (_timeout >= timeout & _quantity < quantity & SpawnPoints.Count > 0)
//                {
//                    if (Enemies.Count > 0)
//                    {
//                        PhotonNetwork.InstantiateSceneObject(Enemies[Random.Range(0, Enemies.Count - 1)].name, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity, 0);
//                        _timeout = 0;
//                        _quantity++;
//                    }
//                }
            }
        }

        
        public void GameOver()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var customValues = new Hashtable {{"gs", false}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(customValues);
                PhotonNetwork.CurrentRoom.IsOpen = false;
//                PhotonNetwork.CurrentRoom.IsVisible = false;

                if (MatchTarget != PUNHelper.MatchTarget.Survive)
                {
                    if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"] >= TargetScore)
                    {
                        var victories = (int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"];
                        victories++;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"rs", TargetScore}, {"rrw", victories}});
                    }
                    else if ((int) PhotonNetwork.CurrentRoom.CustomProperties["bs"] >= TargetScore)
                    {
                        var victories = (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"];
                        victories++;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"bs", TargetScore}, {"brw", victories}});
                    }
                    // check if game over from timer, add the draw here
                    else if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"] > (int) PhotonNetwork.CurrentRoom.CustomProperties["bs"])
                    {
                        var victories = (int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"];
                        victories++;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"rrw", victories}});
                    }
                    else if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"] < (int) PhotonNetwork.CurrentRoom.CustomProperties["bs"])
                    {
                        var victories = (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"];
                        victories++;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"brw", victories}});
                    }
                }
                else
                {
                    var redLivePlayers = PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Red));
                    var blueLivePlayers = PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Blue));
                    
                    if (redLivePlayers > blueLivePlayers)
                    {
                        var victories = (int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"];
                        victories++;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"rrw", victories}});
                    }
                    else if (redLivePlayers < blueLivePlayers)
                    {
                        var victories = (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"];
                        victories++;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"brw", victories}});
                    }
                    else
                    {
                        
                    }
                }
            }
            
            if (!useTeams)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (!deadPlayers.Contains(player))
                    {
                        player.SetCustomProperties(new Hashtable{{"pl", 1}});
                        deadPlayers.Add(player);
                    }
                }
            }

            if (roundsCount > 1)
            {
                if (currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamTotalWins)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.RedTeamTotalWins.text = "ROUNDS WON " + (int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"];

                if (currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins)
                    currentUIManager.MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins.text = "ROUNDS WON " + (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"];
            }
            
            gameOver = true;
            GameStarted = false;
            StartCoroutine(GameOverTimeout());
        }

        IEnumerator GameOverTimeout()
        {
            canPause = false;

            currentUIManager.HideAllMultiplayerRoomUI();

            currentUIManager.CharacterUI.MainObject.SetActive(false);
            currentUIManager.CharacterUI.Inventory.MainObject.SetActive(false);
            
            Time.timeScale = 0.5f;
            
            yield return new WaitForSeconds(2);
            
            Time.timeScale = 1;
            
            StopAllCoroutines(); 

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                currentUIManager.UIButtonsMainObject.SetActive(false);
            
            if (Player)
                Player.GetComponent<Controller>().isPause = true;

            if (currentRound == roundsCount)
            {
                if (CharacterCamera)
                    Destroy(CharacterCamera);

                if (Player)
                    PhotonNetwork.Destroy(Player);
                
                if(DefaultCamera)
                    DefaultCamera.gameObject.SetActive(true);

                var currentScore = PlayerPrefs.GetInt("PlayerScore");
                currentScore += (int) PhotonNetwork.LocalPlayer.CustomProperties["cms"];
                PlayerPrefs.SetInt("PlayerScore", currentScore);
            }
            else
            {
                if(!Player)
                    InstantiateCharacters(true);
            }

            currentUIManager.HideAllMultiplayerRoomUI();

            if (currentRound < roundsCount)
            {
                StartCoroutine(StartNewRound());
            }

            ShowGameOverMenu();
        }

        void ShowGameOverMenu()
        {
            if (useTeams)
            {
                PUNHelper.Teams winner;

                if (MatchTarget != PUNHelper.MatchTarget.Survive && MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                {
                    if (roundsCount > currentRound || roundsCount == 1)
                    {
                        if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"] > (int) PhotonNetwork.CurrentRoom.CustomProperties["bs"])
                        {
                            winner = PUNHelper.Teams.Red;
                        }
                        else if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"] < (int) PhotonNetwork.CurrentRoom.CustomProperties["bs"])
                        {
                            winner = PUNHelper.Teams.Blue;
                        }
                        else
                        {
                            winner = PUNHelper.Teams.Null;
                        }
                    }
                    else
                    {
                        if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"] > (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"])
                        {
                            winner = PUNHelper.Teams.Red;
                        }
                        else if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"] < (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"])
                        {
                            winner = PUNHelper.Teams.Blue;
                        }
                        else
                        {
                            winner = PUNHelper.Teams.Null;
                        }
                    }

                    if (currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus)
                    {
                        switch (winner)
                        {
                            case PUNHelper.Teams.Red:
                                currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus.text = (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? "VICTORY" : "DEFEAT";
                                break;
                            case PUNHelper.Teams.Blue:
                                currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus.text = (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Blue ? "VICTORY" : "DEFEAT";
                                break;
                            case PUNHelper.Teams.Null:
                                currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus.text = "DRAW";
                                break;
                        }
                    }

                    if (roundsCount == 1)
                    {
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["rs"]).ToString();
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["bs"]).ToString();
                    }
                    else
                    {
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"]).ToString();
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["brw"]).ToString();
                    }
                    
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.ActivateTeamsScreen(winner, (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"], currentRound == roundsCount);

                }
                else if (MatchTarget == PUNHelper.MatchTarget.Survive)
                {
                    var redLivePlayers = PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Red));
                    var blueLivePlayers = PUNHelper.LivePlayerCount(PhotonNetwork.PlayerList.ToList().FindAll(player => (PUNHelper.Teams) player.CustomProperties["pt"] == PUNHelper.Teams.Blue));

                    if (roundsCount > currentRound || roundsCount == 1)
                    {
                        if (redLivePlayers > blueLivePlayers)
                            winner = PUNHelper.Teams.Red;
                        else if (redLivePlayers < blueLivePlayers)
                            winner = PUNHelper.Teams.Blue;
                        else
                            winner = PUNHelper.Teams.Null;
                    }
                    else
                    {
                        if ((int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"] > (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"])
                        {
                            winner = PUNHelper.Teams.Red;
                        }
                        else if((int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"] < (int) PhotonNetwork.CurrentRoom.CustomProperties["brw"])
                        {
                            winner = PUNHelper.Teams.Blue;
                        }
                        else
                        {
                            winner = PUNHelper.Teams.Null;
                        }
                    }

                    switch (winner)
                    {
                        case PUNHelper.Teams.Red:
                            currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus.text = (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? "VICTORY" : "DEFEAT";
                            break;
                        case PUNHelper.Teams.Blue:
                            currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus.text = (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Blue ? "VICTORY" : "DEFEAT";
                            break;
                        case PUNHelper.Teams.Null:
                            currentUIManager.MultiplayerGameRoom.GameOverMenu.TeamsStatus.text = "DRAW";
                            break;
                    }


                    if (roundsCount == 1)
                    {
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamScore.text = blueLivePlayers.ToString();
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamScore.text = redLivePlayers.ToString();
                    }
                    else
                    {
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.BlueTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["brw"]).ToString(); 
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.RedTeamScore.text = ((int) PhotonNetwork.CurrentRoom.CustomProperties["rrw"]).ToString();
                    }
                    
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.ActivateTeamsScreen(winner, (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"], currentRound == roundsCount);
                }
                else
                {
                    if(currentUIManager.MultiplayerGameRoom.GameOverMenu.PlayAgainButton)
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.GameOverMenu.PlayAgainButton.gameObject);
                    
                    if(currentUIManager.MultiplayerGameRoom.GameOverMenu.ExitButton)
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.GameOverMenu.ExitButton.gameObject);
                    
                    currentUIManager.MultiplayerGameRoom.PauseMenu.ActivateTeamsMenu(false, false);
                }
                
                if (currentUIManager.MultiplayerGameRoom.GameOverMenu.RoundStatusText)
                {
                    if (roundsCount == currentRound || roundsCount == 1)
                        currentUIManager.MultiplayerGameRoom.GameOverMenu.RoundStatusText.text = " ";
                    else currentUIManager.MultiplayerGameRoom.GameOverMenu.RoundStatusText.text = "End of round " + currentRound + "\n" + "Starting round " + (currentRound + 1) + "...";
                }
                
            }
            else
            {
                if (MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                {
                    currentUIManager.MultiplayerGameRoom.GameOverMenu.ActivateNotTeamsScreen();

                    Player[] sortPlayers;

                    if (MatchTarget != PUNHelper.MatchTarget.Survive)
                    {
                        sortPlayers = MatchTarget == PUNHelper.MatchTarget.Kills ? PhotonNetwork.PlayerList.OrderByDescending(t => t.CustomProperties["k"]).ThenBy(t => t.CustomProperties["d"]).ToArray() : PhotonNetwork.PlayerList.OrderBy(t => t.CustomProperties["cms"]).ToArray();
                    }
                    else
                    {
                        sortPlayers = deadPlayers.OrderBy(t => t.CustomProperties["pl"]).ToArray();
                    }

                    if (MatchTarget != PUNHelper.MatchTarget.Survive)
                    {
                        for (var i = 0; i < sortPlayers.Length; i++)
                        {
                            if (sortPlayers[i].IsLocal)
                            {
                                if (currentUIManager.MultiplayerGameRoom.GameOverMenu.NotTeamsStatus)
                                    currentUIManager.MultiplayerGameRoom.GameOverMenu.NotTeamsStatus.text = "YOU ARE #" + (i + 1);
                            }
                        }
                    }
                    else
                    {
                        var place = (int) PhotonNetwork.LocalPlayer.CustomProperties["pl"] + ((int) PhotonNetwork.LocalPlayer.CustomProperties["pl"] == 0 ? 1 : 0);

                        if (currentUIManager.MultiplayerGameRoom.GameOverMenu.NotTeamsStatus)
                            currentUIManager.MultiplayerGameRoom.GameOverMenu.NotTeamsStatus.text = "YOU ARE #" + place;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        if (i >= sortPlayers.Length)
                        {
                            currentUIManager.MultiplayerGameRoom.GameOverMenu.PodiumPlayers[i].MainObject.SetActive(false);
                            continue;
                        }

                        if (sortPlayers[i] != null)
                        {
                            var player = currentUIManager.MultiplayerGameRoom.GameOverMenu.PodiumPlayers[i];
                            player.Nickname.text = sortPlayers[i].NickName;
                            player.Score.text = ((int) sortPlayers[i].CustomProperties["k"]).ToString();
                        }
                    }
                }
                else
                {
                    if(currentUIManager.MultiplayerGameRoom.GameOverMenu.PlayAgainButton)
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.GameOverMenu.PlayAgainButton.gameObject);
                    
                    if(currentUIManager.MultiplayerGameRoom.GameOverMenu.ExitButton)
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.GameOverMenu.ExitButton.gameObject);
                    
                    currentUIManager.MultiplayerGameRoom.PauseMenu.ActivateNotTeamsMenu(false, false);
                    
                }
            }
        }

        void ShowSpectateMenu()
        {
            var place = (int) PhotonNetwork.LocalPlayer.CustomProperties["pl"] + ((int) PhotonNetwork.LocalPlayer.CustomProperties["pl"] == 0 ? 1 : 0);
            
            if (useTeams)
            {
                currentUIManager.MultiplayerGameRoom.MatchStats.ActivateTeamScreen("survival");
                currentUIManager.MultiplayerGameRoom.SpectateMenu.ActivateTeamsScreen();     
                
                if (currentUIManager.MultiplayerGameRoom.SpectateMenu.PlayerStats)
                    currentUIManager.MultiplayerGameRoom.SpectateMenu.PlayerStats.text = "YOU ARE #" + place + " in the team";
            }
            else
            {
                currentUIManager.MultiplayerGameRoom.MatchStats.ActivateNotTeamScreen("survival");
                currentUIManager.MultiplayerGameRoom.SpectateMenu.ActivateNotTeamsScreen();

                if (currentUIManager.MultiplayerGameRoom.SpectateMenu.PlayerStats)
                    currentUIManager.MultiplayerGameRoom.SpectateMenu.PlayerStats.text = "YOU ARE #" + place;
            }
        }
        
        void LaunchGame()
        {
            currentUIManager.HideAllMultiplayerRoomUI();

            if (!(bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
            {
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["wpmg"])
                {
                    if (MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                    {
                        currentUIManager.MultiplayerGameRoom.TimerBeforeMatch.ActivateAll();

                        StartCoroutine(StartMatchTimeout());
                    }
                    else
                    {
                        StartGame();
                    }
                }
                else
                {
                    currentUIManager.MultiplayerGameRoom.PreMatchMenu.ActivateAll();
                    StartCoroutine(PreMatchGame());

                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
//                        PhotonNetwork.CurrentRoom.IsVisible = false;
                    }
                }
            }
            else
            {
                canPause = true;
                GameStarted = true;
                gameOver = false;

                if (MatchTarget != PUNHelper.MatchTarget.Survive && MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                {
                    if (useTeams)
                    {
                        currentUIManager.MultiplayerGameRoom.MatchStats.ActivateTeamScreen("norm");

                        if (MatchTarget == PUNHelper.MatchTarget.Domination)
                        {
                            if ((int) PhotonNetwork.CurrentRoom.CustomProperties["pc"] == 3)
                                currentUIManager.MultiplayerGameRoom.MatchStats.ActivateDominationScreen();
                            else
                            {
                                if (CurrentHardPoint)
                                    CurrentHardPoint.gameObject.SetActive(true);
                                currentUIManager.MultiplayerGameRoom.MatchStats.ActivateHardPointScreen();
                            }
                        }
                    }

                    else currentUIManager.MultiplayerGameRoom.MatchStats.ActivateNotTeamScreen("norm");
                }
                else if (MatchTarget == PUNHelper.MatchTarget.Survive)
                {
                    if (useTeams) currentUIManager.MultiplayerGameRoom.MatchStats.ActivateTeamScreen("survival");
                    else currentUIManager.MultiplayerGameRoom.MatchStats.ActivateNotTeamScreen("survival");
                }
            }

            if (!(bool) PhotonNetwork.CurrentRoom.CustomProperties["wpmg"] || (bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
            {
                InstantiateCharacters((bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"]);
            }
            else if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["wpmg"] && !(bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
            {
                foreach (var player in FindObjectsOfType<Controller>())
                {
                    player.CanKillOthers = (PUNHelper.CanKillOthers) PhotonNetwork.CurrentRoom.CustomProperties["km"];
                }

                ClearPlayerList(false);
                AddPlayersToList(false);

                if (MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
                {
                    int index = 0;

                    if (!useTeams)
                    {
                        var allPlayers = FindObjectsOfType<Controller>().ToList();
                        allPlayers.Remove(Player.GetComponent<Controller>());

                        Player.transform.position = PUNHelper.SpawnPoint(PlayersSpawnAreas, allPlayers, this, ref index);
                        Player.transform.rotation = Quaternion.Euler(0, PlayersSpawnAreas[index].spawnDirection, 0);
                    }
                    else
                    {
                        Player.transform.position = PUNHelper.SpawnPoint((PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? RedTeamSpawnAreas : BlueTeamSpawnAreas, PUNHelper.GetOnlyTeammates(), this, ref index);
                        Player.transform.rotation = Quaternion.Euler(0, (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? RedTeamSpawnAreas[index].spawnDirection : BlueTeamSpawnAreas[index].spawnDirection, 0);
                    }
                    
                    //rebind character's scripts and components

                    Player.SetActive(false);

                    Player.GetComponent<Animator>().Rebind();

                    controller.isPause = true;

                    for (var i = 0; i < 8; i++)
                    {
                        foreach (var slot in controller.WeaponManager.slots[i].weaponSlotInGame.Where(slot => slot != null && slot.weapon))
                        {
                            Destroy(slot.weapon);
                        }

                        controller.WeaponManager.slots[i].weaponSlotInGame.Clear();
                        controller.WeaponManager.slots[i].currentWeaponInSlot = 0;
                    }

                    StartCoroutine(ActivateCharacter());
                }

                //
            }
        }

        IEnumerator ActivateCharacter()
        {
            yield return new WaitForSeconds(1);
            
            Player.SetActive(true);

            controller.GetComponent<CharacterSync>().UseHealthKit();

            controller.CameraController.ReloadParameters();

            StopCoroutine(ActivateCharacter());
        }

        IEnumerator StartNewRound()
        {
            yield return new WaitForSeconds(5);

            currentRound++;

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
                {
                    {"rs", 0}, {"bs", 0}, {"act", PUNHelper.Teams.Null}, {"bct", PUNHelper.Teams.Null}, {"cct", PUNHelper.Teams.Null}, {"hpct", PUNHelper.Teams.Null},
                    {"acv", 0f}, {"bcv", 0f}, {"ccv", 0f}, {"hpcv", 0f} ,{"st", 15}, {"cr", currentRound}
                });
            }
            
            if(MatchTarget == PUNHelper.MatchTarget.Survive)
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable{{"k",0}, {"d", 0}});
            
            yield return new WaitForSeconds(1);

            PUNHelper.ManagePointUI(currentUIManager, this);
            
            deadPlayers.Clear();
            
            ClearPlayersIcons();
            AddPlayersIcons();

            pastMatchTime = 0;
            leftMatchTime = 0;
            startTime = -1;
            
            LaunchGame();
            
            StopCoroutine(StartNewRound());
        }

        IEnumerator InstantiateCharacter(List<Controller> controllers, List<SpawnZone> spawnZones, bool immediately)
        {
            yield return new WaitForSeconds(!immediately ? Random.Range(1f, 3f) : 0);
            
            var index = 0;

            Player = PhotonNetwork.Instantiate(PlayerPrefs.GetString("CharacterPrefabName"), PUNHelper.SpawnPoint(spawnZones, controllers, this, ref index), Quaternion.Euler(0, spawnZones[index].spawnDirection, 0));
            
            currentUIManager.CharacterUI.ActivateAll();
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"wl", true}});

            PUNHelper.ManagePointUI(currentUIManager, this);
            
            canPause = true;

            
            Player.GetComponent<CharacterSync>().UpdatePlayersList();
            
            var controller = Player.GetComponent<Controller>();
            
            controller.ActiveCharacter = true;
            CharacterCamera = controller.thisCamera.gameObject;
            
            if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                currentUIManager.UIButtonsMainObject.SetActive(true);

            StopCoroutine("InstantiateCharacter");
        }

        public void ChooseSpectateCamera()
        {
            if (CharacterCamera && SpectateCameras.Count > 0)
            {
                var index = Helper.GetRandomIndex(ref spectateCameraIndex, SpectateCameras.Count);

                CharacterCamera.transform.position = SpectateCameras[index].transform.position;
                CharacterCamera.transform.rotation = SpectateCameras[index].transform.rotation;
            } 
        }

        void InstantiateCharacters(bool immediately)
        {
            if (!useTeams)
            {
                if (PlayersSpawnAreas.Count < 0)
                {
                    Debug.LogError("<Color=Red>Missing</Color> [Players Spawn Zones]. Set them up in the [RoomManager] component.", this);
                    Debug.Break();
                    return;
                }

                StartCoroutine(InstantiateCharacter(FindObjectsOfType<Controller>().ToList(), PlayersSpawnAreas, immediately));
            }
            else
            {
                if ((PUNHelper.SpawnMethod) PhotonNetwork.CurrentRoom.CustomProperties["sm"] == PUNHelper.SpawnMethod.OnBases)
                {
                    StartCoroutine(InstantiateCharacter(PUNHelper.GetOnlyTeammates(), (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? RedTeamSpawnAreas : BlueTeamSpawnAreas, immediately));
                }
                else
                {
                    if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
                    {
                        StartCoroutine(InstantiateCharacter(PUNHelper.GetOnlyTeammates(), PlayersSpawnAreas, (bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"]));
                    }
                    else
                    {
                        StartCoroutine(InstantiateCharacter(PUNHelper.GetOnlyTeammates(), (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == PUNHelper.Teams.Red ? RedTeamSpawnAreas : BlueTeamSpawnAreas, immediately));
                    }
                }
            }
        }

        public IEnumerator RestartGame()
        {
            yield return new WaitForSeconds(3);
            {
                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
                {
                    currentUIManager.HideAllMultiplayerRoomUI();

                    if (MatchTarget != PUNHelper.MatchTarget.Survive)
                    {
                        if (CharacterCamera) Destroy(CharacterCamera);
                        
                        if (DefaultCamera)
                            DefaultCamera.gameObject.SetActive(true);

                        var bloodSplatterColor = currentUIManager.CharacterUI.bloodSplatter.color;
                        bloodSplatterColor.a = 0;
                        currentUIManager.CharacterUI.bloodSplatter.color = bloodSplatterColor;
                        
                        currentUIManager.MultiplayerGameRoom.TimerAfterDeath.ActivateAll();

                        StartCoroutine(RestartTimeout());
                    }
                    else
                    {
                        ShowSpectateMenu();
                        ChooseSpectateCamera();
                    }
                    
                    if (Player)
                    {
                        PhotonNetwork.Destroy(Player);
                    }
                }

                StopCoroutine(RestartGame());
            }
        }

        IEnumerator RestartTimeout()
        {
            if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                currentUIManager.UIButtonsMainObject.SetActive(false);
            
            restartTime = 5;
            
            while (true)
            {
                restartTime -= Time.deltaTime;

                if (currentUIManager.MultiplayerGameRoom.TimerAfterDeath.RestartTimer)
                    currentUIManager.MultiplayerGameRoom.TimerAfterDeath.RestartTimer.text = "Start in " + restartTime.ToString("00") + " sec...";

                if (restartTime < 1)
                {
                    currentUIManager.MultiplayerGameRoom.TimerAfterDeath.RestartTimer.text = "Go!";
                }
                
                if (restartTime < 0)
                {
                    LaunchGame();
                    
                    if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                        currentUIManager.UIButtonsMainObject.SetActive(true);
                    
                    StopCoroutine(RestartTimeout());
                    break;
                }

                yield return 0;
            }
        }

        IEnumerator PreMatchGame()
        {
            while (true)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    var notInstantiatedPlayers = PhotonNetwork.PlayerList.ToList().FindAll(player => !(bool) player.CustomProperties["wl"]);

                    if (notInstantiatedPlayers.Count <= 0)
                    {
                        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"wpmg", true}});
                    }
                }

                if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["wpmg"])
                {
                    preMatchTimer -= Time.deltaTime;
                    
                    currentUIManager.MultiplayerGameRoom.PreMatchMenu.Status.text = "Start in " + (int) preMatchTimer;

                    if (preMatchTimer <= 0)
                    {
                        LaunchGame();
                        StopCoroutine(PreMatchGame());
                        break;
                    }
                }
                else
                {
                    if (!Player)
                    {
                        currentUIManager.MultiplayerGameRoom.PreMatchMenu.Status.text = "Character loading...";
                    }
                    else
                    {
                        var notLoadedPlayers = PhotonNetwork.PlayerList.ToList().FindAll(player => !(bool) player.CustomProperties["wl"]).Count;
                        currentUIManager.MultiplayerGameRoom.PreMatchMenu.Status.text = "Waiting " + notLoadedPlayers + " players...";
                    }
                }

                yield return 0;
            }
        }


        IEnumerator StartMatchTimeout()
        {
            startMatchTimer = (int) PhotonNetwork.CurrentRoom.CustomProperties["st"];
            var timePercent = startMatchTimer - 5;
            canPause = false;
            
            currentUIManager.CharacterUI.DisableAll();
            
            if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                currentUIManager.UIButtonsMainObject.SetActive(false);

            while (true)
            {
                int currentTime;

                startMatchTimer -= Time.deltaTime;

                if (PhotonNetwork.IsMasterClient)
                {
                    var customValues = new Hashtable {{"st", (int) startMatchTimer}};
                    PhotonNetwork.CurrentRoom.SetCustomProperties(customValues);
                }

                currentTime = (int) PhotonNetwork.CurrentRoom.CustomProperties["st"];

                if (currentUIManager.MultiplayerGameRoom.TimerBeforeMatch.StartMatchTimer)
                    if (currentTime < timePercent + 5)
                        currentUIManager.MultiplayerGameRoom.TimerBeforeMatch.StartMatchTimer.text = currentTime.ToString("00");
                

                if (currentUIManager.MultiplayerGameRoom.TimerBeforeMatch.Background)
                    currentUIManager.MultiplayerGameRoom.TimerBeforeMatch.Background.color = new Color(0, 0, 0, startMatchTimer / timePercent);

                if (currentTime < 1)
                {
                    currentUIManager.MultiplayerGameRoom.TimerBeforeMatch.StartMatchTimer.text = "Go!";

                    if (currentTime < 0)
                    {
                        StartGame();

                        StopCoroutine(StartMatchTimeout());
                        break;
                    }
                }
                yield return 0;
            }
        }

        void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var customValues = new Hashtable {{"gs", true}, {"mt", PhotonNetwork.Time}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(customValues);

                if (MatchTarget != PUNHelper.MatchTarget.Survive)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
//                    PhotonNetwork.CurrentRoom.IsVisible = true;
                }
            }

            currentUIManager.HideAllMultiplayerRoomUI();

            canPause = true;
            GameStarted = true;
            gameOver = false;

            if (MatchTarget != PUNHelper.MatchTarget.Survive && MatchTarget != PUNHelper.MatchTarget.WithoutTarget)
            {
                if (useTeams)
                {
                    currentUIManager.MultiplayerGameRoom.MatchStats.ActivateTeamScreen("norm");

                    if (MatchTarget == PUNHelper.MatchTarget.Domination)
                    {
                        if ((int) PhotonNetwork.CurrentRoom.CustomProperties["pc"] == 3)
                            currentUIManager.MultiplayerGameRoom.MatchStats.ActivateDominationScreen();
                        else
                        {
                            if (CurrentHardPoint)
                                CurrentHardPoint.gameObject.SetActive(true);
                            currentUIManager.MultiplayerGameRoom.MatchStats.ActivateHardPointScreen();
                        }
                    }
                }
                else currentUIManager.MultiplayerGameRoom.MatchStats.ActivateNotTeamScreen("norm");
            }
            else if(MatchTarget == PUNHelper.MatchTarget.Survive)
            {
                if (useTeams) currentUIManager.MultiplayerGameRoom.MatchStats.ActivateTeamScreen("survival");
                else currentUIManager.MultiplayerGameRoom.MatchStats.ActivateNotTeamScreen("survival");
            }
            else
            {
                if (useMatchTime && currentUIManager.MultiplayerGameRoom.MatchStats.MatchTimer)
                    Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.MatchStats.MatchTimer.gameObject);
            }

            currentUIManager.CharacterUI.ActivateAll();

            if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                currentUIManager.UIButtonsMainObject.SetActive(true);

            Player.GetComponent<Controller>().isPause = false;
        }

        IEnumerator FindPlayersTimeout()
        {
            var startTimeout = 0f;
            var minPlayers = (int) PhotonNetwork.CurrentRoom.CustomProperties["mp"] * (useTeams ? 2 : 1);
            canPause = false;

            while (true)
            {
                currentUIManager.MultiplayerGameRoom.StartMenu.ActivateScreen();

                findPlayersTimer += Time.deltaTime;

                if (findPlayersTimer > 20 && PhotonNetwork.PlayerList.Length < minPlayers)
                {
                    if (currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersStatsText)
                        currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersStatsText.text = "Not enough players to start";

                    if (currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersTimer)
                        currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersTimer.text = ":(";

                    if (findPlayersTimer > 23)
                    {
                        LeaveMatch();
                        StopCoroutine(FindPlayersTimeout());
                        break;
                    }
                }
                else
                {
                    if (currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersTimer)
                        currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersTimer.text = findPlayersTimer.ToString("00");

                    if (currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersStatsText)
                    {
                        if (minPlayers <= PhotonNetwork.PlayerList.Length)
                        {
                            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.IsOpen)// || PhotonNetwork.CurrentRoom.IsVisible)
                            {
                                PhotonNetwork.CurrentRoom.IsOpen = false;
//                                PhotonNetwork.CurrentRoom.IsVisible = false;
                            }

                            currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersStatsText.text = "Starting match...";
                        }
                        else currentUIManager.MultiplayerGameRoom.StartMenu.FindPlayersStatsText.text = "Finding " + (minPlayers - PhotonNetwork.PlayerList.Length) + " players...";
                    }
                }


                if (PhotonNetwork.PlayerList.Length >= minPlayers)
                {
                    startTimeout += Time.deltaTime;

                    if (startTimeout > 3)
                    {
                        LaunchGame();
                        StopCoroutine(FindPlayersTimeout());
                        break;
                    }
                }

                yield return 0;
            }
        }

        public void Pause(bool showUI)
        {
//            if(!(bool)PhotonNetwork.CurrentRoom.CustomProperties["gs"])
//                return;

            if (!canPause)
                return;

            var value = !isPause;

            if (showUI)
            {
                if (value)
                {
                    ClearPlayerList(false);
                    AddPlayersToList(false);
                }

                if (value)
                {
                    if (useTeams) currentUIManager.MultiplayerGameRoom.PauseMenu.ActivateTeamsMenu(true, (string) PhotonNetwork.CurrentRoom.CustomProperties["gn"] != "");
                    else currentUIManager.MultiplayerGameRoom.PauseMenu.ActivateNotTeamsMenu(true, (string) PhotonNetwork.CurrentRoom.CustomProperties["gn"] != "");
                }
                else
                {
                    currentUIManager.MultiplayerGameRoom.PauseMenu.DisableAll();
                }
            }

            if(value) controller.anim.SetBool("Move", false);
            
            controller.isPause = value;
            controller.CameraController.canUseCursorInPause = true;
            
            if ((Application.isMobilePlatform || projectSettings.mobileDebug) && currentUIManager.UIButtonsMainObject)
                currentUIManager.UIButtonsMainObject.SetActive(!value);

            isPause = value;
        }

        void CalculatePlayersCount(Player player)
        {
            int count;
            var room = PhotonNetwork.CurrentRoom;

            switch ((int) player.CustomProperties["pt"])
            {
                case 0:
                    count = (int) room.CustomProperties["rc"];
                    room.SetCustomProperties(new Hashtable {{"rc", count - 1}});
                    break;
                case 1:
                    count = (int) room.CustomProperties["bc"];
                    room.SetCustomProperties(new Hashtable {{"bc", count - 1}});
                    break;
            }
        }

        #region UIManaged
        

        void LaunchGameAfterDeath()
        {
            StopAllCoroutines(); 
            LaunchGame();
        }

        void OpenMatchStats(string type)
        {
            currentUIManager.HideAllMultiplayerRoomUI();

            if (useTeams) currentUIManager.MultiplayerGameRoom.PauseMenu.ActivateTeamsMenu(false, (string)PhotonNetwork.CurrentRoom.CustomProperties["gn"] != "");
            else currentUIManager.MultiplayerGameRoom.PauseMenu.ActivateNotTeamsMenu(false, (string)PhotonNetwork.CurrentRoom.CustomProperties["gn"] != "");

            switch (type)
            {
                case "GameOver":
                {
                    if (currentUIManager.MultiplayerGameRoom.GameOverMenu.BackButton)
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.GameOverMenu.BackButton.gameObject);
                    break;
                }
                case "Spectate":
                {
                    if (currentUIManager.MultiplayerGameRoom.SpectateMenu.BackButton)
                        Helper.EnableAllParents(currentUIManager.MultiplayerGameRoom.SpectateMenu.BackButton.gameObject);
                    break;
                }
            }
        }

        void CloseMatchStats(string type)
        {
            currentUIManager.HideAllMultiplayerRoomUI();

            switch (type)
            {
                case "GameOver":
                {
                    ShowGameOverMenu();
                    break;
                }
                case "Spectate":
                {
                    ShowSpectateMenu();
                    break;
                }
            }
        }

        void PlayAgain()
        {
            if (Player)
            {
                Destroy(Player.GetComponent<Controller>().CameraController.MainCamera.gameObject);
                PhotonNetwork.Destroy(Player);
            }
            
            PlayerPrefs.SetInt("LaunchAgain", 1);
            PhotonNetwork.LeaveRoom();
        }

        void LeaveMatch()
        {
            if (Player)
            {
                Destroy(Player.GetComponent<Controller>().CameraController.MainCamera.gameObject);
                PhotonNetwork.Destroy(Player);
            }
            
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PhotonCallBacks

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
//            if(!(bool)newMasterClient.CustomProperties["cl"])
//                newMasterClient.SetCustomProperties(new Hashtable{{"cl", true}});
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            //CheckCountPlayers();
            StartCoroutine(UpdatePlayerList());
//            CheckPlayersNames();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CalculatePlayersCount(otherPlayer);

            if (MatchTarget == PUNHelper.MatchTarget.Survive)
            {
                otherPlayer.SetCustomProperties(new Hashtable{{"lft", true}});
                leftPlayers.Add(otherPlayer);
            }

            if ((bool) PhotonNetwork.CurrentRoom.CustomProperties["gs"])
            {
                ClearPlayerList(false);
                AddPlayersToList(false);
            }
            else
            {
                ClearPlayerList(true);
                AddPlayersToList(true);
            }

            ClearPlayersIcons();
            AddPlayersIcons();
        }

        #endregion

        IEnumerator UpdatePlayerList()
        {
            yield return new WaitForSeconds(1);
            ClearPlayerList(true);
            AddPlayersToList(true);
            StopCoroutine(UpdatePlayerList());
        }

        public IEnumerator StatsEnabledTimer()
        {
            
            yield return new WaitForSeconds(5);
            
            if(currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent)
                currentUIManager.MultiplayerGameRoom.MatchStats.KillStatsContent.gameObject.SetActive(false);
            
            StopCoroutine(StatsEnabledTimer());
        }
        
        public IEnumerator AddScorePopupDisableTimeout()
        {
            yield return new WaitForSeconds(4);
            
            if(currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup)
                currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.gameObject.SetActive(false);
            
            StopCoroutine(AddScorePopupDisableTimeout());
        }
#endif
    }

}


