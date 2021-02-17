using System;
using System.Collections.Generic;
using System.Linq;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
	public static class PUNHelper
	{
		public enum CanKillOthers
		{
			OnlyOpponents,
			Everyone,
			NoOne
		}

		public enum WeaponsToUse
		{
			All,
			Specific
		}

		public enum MatchTarget
		{
			Points,
			Kills,
			Domination,
			Survive, 
			WithoutTarget
		}

		public enum Teams
		{
			Red,
			Blue,
			Null
		}

		public enum ContentType
		{
			Player,
			Match,
			Weapon,
			GameMode,
			Map,
			Avatar, 
			Room
		}

		public enum SpawnMethod
		{
			Random,
			OnBases
		}

		public static List<string> PhotonRegions = new List<string>()
		{
			"Asia",
			"Australia",
			"Canada, East",
			"Europe",
			"India",
			"Japan",
			"Russia",
			"Russia, East",
			"South America",
			"South Korea",
			"USA, East",
			"USA, West"
		};

		public static List<string> SpawnMethods = new List<string> {"Random", "On Bases"};
		public static List<string> MatchTargets = new List<string> {"Without Purpose", "Kills", "Score", "Point Retention"};
		public static List<string> CanKillOther = new List<string> {"Everyone", "No One", "Only Opponents"};

		public static string EmptyLine = "                                                                        ";
		
		[Serializable]
		public class PhotonLevel
		{
			//public Button LevelButton;
			public string Name;
			public Texture Image;
#if UNITY_EDITOR
			public SceneAsset Scene;
#endif
		}

		[Serializable]
		public class CameraPosition
		{
			public Vector3 position;
			public Quaternion rotation;
		}

		[Serializable]
		public class GameMode
		{
			public string Name;
			public Texture Image;
			
			public bool TimeLimit;
			public bool Teams;
			public bool Enemies;
			public bool CanRespawn = true;
			public bool oneShotOneKill;
			public bool captureImmediately;

			[Range(1, 10)] public int Rounds = 1;
			[Range(1, 20)] public int maxPlayersCount = 20;
			[Range(1, 20)] public int minPlayerCount = 2;
			public int targetPoints = 200;
			public int targetTime = 10;
			public int targetKills = 20;
			
			public float capturePointTimeout = 3;
			
			public int captureScore = 2;
			public int matchTarget;
			public int spawnMethod;
			public int pointsCount;
			public int hardPointTimeout = 60;
			public int weaponsLimit = 100;
			public int KillMethod;
			public string Description = "\n\n\n";
			
			public WeaponsToUse WeaponsToUse;
			public List<WeaponSlot> WeaponsForThisMode;
#if UNITY_EDITOR
			public ReorderableList weaponsList;
#endif
		}

		[Serializable]
		public class WeaponSlot
		{
			public WeaponController weapon;
			public int slot;
		}
		
		#region UI Screens

		[Serializable]
		public class MatchStats
		{
			public GameObject TeamsSurvivalMain;
			public GameObject TeamsMatchUIMain;
			public GameObject NotTeamsMatchUIMain;
			public GameObject NotTeamsSurvivalMain;
			public GameObject DominationMain;
			public GameObject HardPointMain;

			public Transform KillStatsContent;
			public Transform RedTeamPlayersList;
			public Transform BlueTeamPlayersList;
			public Transform PlayersList;

			public Text RedTeamMatchStats;
			public Text BlueTeamMatchStats;
			public Text TargetText;
			public Text CurrentPlaceText;
			public Text FirstPlaceStats;
			public Text PlayerStats;
			public Text MatchTimer;
			public Text HardPointTimer;
			public Text AddScorePopup;

			public Image A_CurrentFill;
			public Image A_CapturedFill;
			public Image B_CurrentFill;
			public Image B_CapturedFill;
			public Image C_CurrentFill;
			public Image C_CapturedFill;
			public Image HardPoint_CurrentFill;
			public Image HardPoint_CapturedFill;
			

			public Texture A_ScreenTargetTexture;
			public Texture B_ScreenTargetTexture;
			public Texture C_ScreenTargetTexture;
			public Texture HardPoint_ScreenTargetTexture;

			public RawImage A_ScreenTarget;
			public RawImage B_ScreenTarget;
			public RawImage C_ScreenTarget;
			public RawImage FirstPlaceStatsBackground;
			public RawImage PlayerStatsBackground;
			public RawImage TeamImagePlaceholder;
			public RawImage RedTeamLogoPlaceholder; 
			public RawImage BlueTeamLogoPlaceholder;

			public Color32 PlayerStatsHighlight;
			

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Image))
					{
						var go = (Image) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}

				if (KillStatsContent)
				{
					Helper.EnableAllParents(KillStatsContent.gameObject);
					KillStatsContent.gameObject.SetActive(false);
				}
			}

			public void ActivateDominationScreen()
			{
				CreateTargetUI(ref A_ScreenTarget, A_ScreenTargetTexture, DominationMain.transform);
				CreateTargetUI(ref B_ScreenTarget, B_ScreenTargetTexture, DominationMain.transform);
				CreateTargetUI(ref C_ScreenTarget, C_ScreenTargetTexture, DominationMain.transform);
				
				if(DominationMain)
					Helper.EnableAllParents(DominationMain);

				if (A_ScreenTarget)
					Helper.EnableAllParents(A_ScreenTarget.gameObject);

				if(B_ScreenTarget)
					Helper.EnableAllParents(B_ScreenTarget.gameObject);
				
				if(C_ScreenTarget)
					Helper.EnableAllParents(C_ScreenTarget.gameObject);
				
				if(A_CurrentFill)
					Helper.EnableAllParents(A_CurrentFill.gameObject);
				
				if(B_CurrentFill)
					Helper.EnableAllParents(B_CurrentFill.gameObject);
				
				if(C_CurrentFill)
					Helper.EnableAllParents(C_CurrentFill.gameObject);
				
				if(A_CapturedFill)
					Helper.EnableAllParents(A_CapturedFill.gameObject);
				
				if(B_CapturedFill)
					Helper.EnableAllParents(B_CapturedFill.gameObject);
				
				if(C_CapturedFill)
					Helper.EnableAllParents(C_CapturedFill.gameObject);
			}

			
			public void ActivateHardPointScreen()
			{
				CreateTargetUI(ref A_ScreenTarget, HardPoint_ScreenTargetTexture, HardPointMain.transform);
				
				if(HardPointMain)
					Helper.EnableAllParents(HardPointMain);
				
				if(HardPoint_CapturedFill)
					Helper.EnableAllParents(HardPoint_CapturedFill.gameObject);
				
				if(HardPointTimer)
					Helper.EnableAllParents(HardPointTimer.gameObject);
			}
			
			void CreateTargetUI(ref RawImage point, Texture texture, Transform parent)
			{
				if (!point && texture)
				{
					point = Helper.NewUIElement(texture.name, parent, Vector2.zero, new Vector2(70, 70), Vector3.one).AddComponent<RawImage>();
					point.raycastTarget = false;
					point.texture = texture;
				}
			}
			

			public void ActivateTeamScreen(string type)
			{
				if (MatchTimer)
					Helper.EnableAllParents(MatchTimer.gameObject);
				
				if(TargetText)
					Helper.EnableAllParents(TargetText.gameObject);

				if (type != "survival")
				{
					if (TeamsMatchUIMain)
						Helper.EnableAllParents(TeamsMatchUIMain);

					if (RedTeamMatchStats)
						Helper.EnableAllParents(RedTeamMatchStats.gameObject);

					if (BlueTeamMatchStats)
						Helper.EnableAllParents(BlueTeamMatchStats.gameObject);

					if (TeamImagePlaceholder)
						Helper.EnableAllParents(TeamImagePlaceholder.gameObject);
				}
				else
				{
					if (TeamsSurvivalMain)
						Helper.EnableAllParents(TeamsSurvivalMain);

					if (RedTeamPlayersList)
						Helper.EnableAllParents(RedTeamPlayersList.gameObject);

					if (BlueTeamPlayersList)
						Helper.EnableAllParents(BlueTeamPlayersList.gameObject);
					
					if (RedTeamLogoPlaceholder)
						Helper.EnableAllParents(RedTeamLogoPlaceholder.gameObject);

					if (BlueTeamLogoPlaceholder)
						Helper.EnableAllParents(BlueTeamLogoPlaceholder.gameObject);
				}
			}

			public void ActivateNotTeamScreen(string type)
			{
				if (MatchTimer)
					Helper.EnableAllParents(MatchTimer.gameObject);
				
				if(TargetText)
					Helper.EnableAllParents(TargetText.gameObject);

				if (type != "survival")
				{
					if (NotTeamsMatchUIMain)
						Helper.EnableAllParents(NotTeamsMatchUIMain);

					if (CurrentPlaceText)
						Helper.EnableAllParents(CurrentPlaceText.gameObject);

					if (FirstPlaceStats)
						Helper.EnableAllParents(FirstPlaceStats.gameObject);

					if (PlayerStats)
						Helper.EnableAllParents(PlayerStats.gameObject);
					
					if (FirstPlaceStatsBackground)
						Helper.EnableAllParents(FirstPlaceStatsBackground.gameObject);

					if (PlayerStatsBackground)
						Helper.EnableAllParents(PlayerStatsBackground.gameObject);
				}
				else
				{
					if (NotTeamsSurvivalMain)
						Helper.EnableAllParents(NotTeamsSurvivalMain);
					
					if(PlayersList)
						Helper.EnableAllParents(PlayersList.gameObject);
				}
			}

		}

		[Serializable]
		public class SpectateMenu
		{
			public Button MatchStatsButton;
			public Button ExitButton;
			public Button ChangeCameraButton;
			public Button BackButton;

			public Text PlayerStats;
//			public Text MatchTimer;

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}
			
			public void ActivateTeamsScreen()
			{
				ActivateAllButtons();

				if (PlayerStats)
					Helper.EnableAllParents(PlayerStats.gameObject);

//				if (MatchTimer)
//					Helper.EnableAllParents(MatchTimer.gameObject);
			}

			public void ActivateNotTeamsScreen()
			{
				ActivateAllButtons();

				if (PlayerStats)
					Helper.EnableAllParents(PlayerStats.gameObject);

//				if (MatchTimer)
//					Helper.EnableAllParents(MatchTimer.gameObject);
			}

			private void ActivateAllButtons()
			{
				if(MatchStatsButton)
					Helper.EnableAllParents(MatchStatsButton.gameObject);
				
				if(ExitButton)
					Helper.EnableAllParents(ExitButton.gameObject);
				
				if(ChangeCameraButton)
					Helper.EnableAllParents(ChangeCameraButton.gameObject);
			}
		}

		[Serializable]
		public class GameOverMenu
		{
			public GameObject TeamsMainObject;
			public GameObject NotTeamsMainObject;

			public Text TeamsStatus;
			public Text NotTeamsStatus;
			
			public Text RedTeamScore;
			public Text BlueTeamScore;
			public Text BlueTeamName;
			public Text RedTeamName;
			public Text RoundStatusText;

			public RawImage VictoryImage;
			public RawImage DefeatImage;
			public RawImage DrawImage;
			public RawImage RedTeamLogoPlaceholder;
			public RawImage BlueTeamLogoPlaceholder;

			public Button PlayAgainButton;
			public Button ExitButton;
			public Button MatchStatsButton;
			public Button BackButton;

			public GameOverPlayerInfo[] PodiumPlayers = {new GameOverPlayerInfo(), new GameOverPlayerInfo(), new GameOverPlayerInfo()};
			
			
			public void ActivateNotTeamsScreen()
			{
				ActivateButtons();

				if (NotTeamsMainObject)
					Helper.EnableAllParents(NotTeamsMainObject);

				if (NotTeamsStatus)
					Helper.EnableAllParents(NotTeamsStatus.gameObject);

				PodiumPlayers[0].ActivateAll();
				PodiumPlayers[1].ActivateAll();
				PodiumPlayers[2].ActivateAll();
			}

			public void ActivateTeamsScreen(Teams winnerTeam, Teams playerTeam, bool activeButtons)
			{
				if(activeButtons)
					ActivateButtons();
				
				if(TeamsStatus)
					Helper.EnableAllParents(TeamsStatus.gameObject);

				if(RoundStatusText)
					Helper.EnableAllParents(RoundStatusText.gameObject);
				
				if (TeamsMainObject)
					Helper.EnableAllParents(TeamsMainObject);

				if (RedTeamScore)
					Helper.EnableAllParents(RedTeamScore.gameObject);

				if (BlueTeamScore)
					Helper.EnableAllParents(BlueTeamScore.gameObject);

				if (BlueTeamName)
					Helper.EnableAllParents(BlueTeamName.gameObject);

				if (RedTeamName)
					Helper.EnableAllParents(RedTeamName.gameObject);

				if (RedTeamLogoPlaceholder)
					Helper.EnableAllParents(RedTeamLogoPlaceholder.gameObject);

				if (BlueTeamLogoPlaceholder)
					Helper.EnableAllParents(BlueTeamLogoPlaceholder.gameObject);

				if (winnerTeam != Teams.Null)
				{
					if (winnerTeam == playerTeam)
					{
						if (VictoryImage)
							Helper.EnableAllParents(VictoryImage.gameObject);
					}
					else
					{
						if (DefeatImage)
							Helper.EnableAllParents(DefeatImage.gameObject);
					}
				}
				else
				{
					if(DrawImage)
						Helper.EnableAllParents(DrawImage.gameObject);
				}

			}

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}

				PodiumPlayers[0].DisableAll();
				PodiumPlayers[1].DisableAll();
				PodiumPlayers[2].DisableAll();
			}

			void ActivateButtons()
			{
				if (PlayAgainButton)
					Helper.EnableAllParents(PlayAgainButton.gameObject);

				if (ExitButton)
					Helper.EnableAllParents(ExitButton.gameObject);

				if (MatchStatsButton)
					Helper.EnableAllParents(MatchStatsButton.gameObject);
			}
		}
		
		[Serializable]
		public class PauseMenu
		{
			public GameObject TeamsPauseMenuMain;
			public GameObject NotTeamsPauseMenuMain;
			
			public Transform BlueTeamContent;
			public Transform RedTeamContent;
			public Transform NotTeamsContent;
			
			public Button ExitButton;
			public Button ResumeButton;
			
			public Text RedTeamName;
			public Text BlueTeamName;
			public Text RedTeamScore;
			public Text BlueTeamScore;
			public Text RedTeamTotalWins;
			public Text BlueTeamTotalWins;
			public Text CurrentGameAndPassword;

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Transform))
					{
						var go = (Transform) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}

			public void ActivateTeamsMenu(bool activeButtons, bool showMatchNameAndPassword)
			{
				if(activeButtons)
					ActivateButtons();
				
				if(showMatchNameAndPassword)
					if(CurrentGameAndPassword)
						Helper.EnableAllParents(CurrentGameAndPassword.gameObject);
				
				if(TeamsPauseMenuMain)
					Helper.EnableAllParents(TeamsPauseMenuMain);

				if(BlueTeamContent)
					Helper.EnableAllParents(BlueTeamContent.gameObject);
				
				if(RedTeamContent)
					Helper.EnableAllParents(RedTeamContent.gameObject);
				
				if(RedTeamName)
					Helper.EnableAllParents(RedTeamName.gameObject);
				
				if(BlueTeamName)
					Helper.EnableAllParents(BlueTeamName.gameObject);
				
				if(RedTeamScore)
					Helper.EnableAllParents(RedTeamScore.gameObject);
				
				if(BlueTeamScore)
					Helper.EnableAllParents(BlueTeamScore.gameObject);
				
				if(RedTeamTotalWins)
					Helper.EnableAllParents(RedTeamTotalWins.gameObject);
				
				if(BlueTeamTotalWins)
					Helper.EnableAllParents(BlueTeamTotalWins.gameObject);
			}

			public void ActivateNotTeamsMenu(bool activeButtons, bool showMatchNameAndPassword)
			{
				if(showMatchNameAndPassword)
					if(CurrentGameAndPassword)
						Helper.EnableAllParents(CurrentGameAndPassword.gameObject);
				
				if(activeButtons)
					ActivateButtons();
				
				if(NotTeamsPauseMenuMain)
					Helper.EnableAllParents(NotTeamsPauseMenuMain);
				
				if(NotTeamsContent)
					Helper.EnableAllParents(NotTeamsContent.gameObject);
			}

			void ActivateButtons()
			{
				if(ExitButton)
					Helper.EnableAllParents(ExitButton.gameObject);
				
				if(ResumeButton)
					Helper.EnableAllParents(ResumeButton.gameObject);
			}
		}
		
		[Serializable]
		public class StartMenu
		{
			public GameObject MainObject;
			
			public Transform PlayersContent;
			
			public Text FindPlayersStatsText;
			public Text FindPlayersTimer;

			public Button ExitButton;

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);
				
				if(PlayersContent)
					PlayersContent.gameObject.SetActive(false);
				
				if(FindPlayersTimer)
					FindPlayersTimer.gameObject.SetActive(false);
				
				if(FindPlayersStatsText)
					FindPlayersStatsText.gameObject.SetActive(false);
				
				if(ExitButton)
					ExitButton.gameObject.SetActive(false);
				
			}

			public void ActivateScreen()
			{
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(PlayersContent)
					Helper.EnableAllParents(PlayersContent.gameObject);
				
				if(FindPlayersTimer)
					Helper.EnableAllParents(FindPlayersTimer.gameObject);
				
				if(FindPlayersStatsText)
					Helper.EnableAllParents(FindPlayersStatsText.gameObject);
				
				if(ExitButton)
					Helper.EnableAllParents(ExitButton.gameObject);
			}
		}

		[Serializable]
		public class TimerBeforeMatch
		{
			public GameObject MainObject;
			
			public RawImage Background;
			
			public Text StartMatchTimer;

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);
				
				if(Background)
					Background.gameObject.SetActive(false);
				
				if(StartMatchTimer)
					StartMatchTimer.gameObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(Background)
					Helper.EnableAllParents(Background.gameObject);
				
				if(StartMatchTimer)
					Helper.EnableAllParents(StartMatchTimer.gameObject);
			}
		}
		
		[Serializable]
		public class TimerAfterDeath
		{
			public GameObject MainObject;
			
			public Button LaunchButton;
			public Text RestartTimer;

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);
				
				if(LaunchButton)
					LaunchButton.gameObject.SetActive(false);
				
				if(RestartTimer)
					RestartTimer.gameObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(LaunchButton)
					Helper.EnableAllParents(LaunchButton.gameObject);
				
				if(RestartTimer)
					Helper.EnableAllParents(RestartTimer.gameObject);
			}
		}

		[Serializable]
		public class GameOverPlayerInfo
		{
			public GameObject MainObject;
			
			public Text Nickname;
			public Text Score;

			public void DisableAll()
			{
				if (Nickname)
					Nickname.gameObject.SetActive(false);

				if (Score)
					Score.gameObject.SetActive(false);
				
				if(MainObject)
					MainObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if (Nickname)
					Helper.EnableAllParents(Nickname.gameObject);

				if (Score)
					Helper.EnableAllParents(Score.gameObject);
				
				if(MainObject)
					Helper.EnableAllParents(MainObject);
			}
		}

		[Serializable]
		public class PreMatchMenu
		{
			public GameObject MainObject;

			public Text Status;

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);
				
				if(Status)
					Status.gameObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if (MainObject)
					Helper.EnableAllParents(MainObject);

				if (Status)
					Helper.EnableAllParents(Status.gameObject);
			}
		}

		#endregion

		public static string FormatTime(double time)
		{
			int minutes = (int) time / 60;
			int seconds = (int) time - 60 * minutes;

			return string.Format("{0:00}:{1:00}", minutes, seconds);
		}

		public static Vector3 SpawnPoint(List<SpawnZone> spawnZones, List<Controller> controllers, RoomManager roomManager, ref int zoneIndex)
		{
			RaycastHit hit;
			Vector3 point;
			var goodPositions = new List<Vector3>();
			var spawnPosition = Vector3.zero;
			var indexes = new List<int>();

			for (var i = 0; i < spawnZones.Count; i++)
			{
				var spawnZone = spawnZones[i];
				roomManager.generateSpawmPointFunctionCount = 0;
				roomManager.foundSpawnPoint = false;

				point = CalculateSpawnPoint(spawnZone, ref roomManager.generateSpawmPointFunctionCount, ref roomManager.foundSpawnPoint, controllers);

				if (roomManager.foundSpawnPoint)
				{
					goodPositions.Add(point);
					indexes.Add(i);
				}
			}

			if (goodPositions.Count > 0)
			{
				var index = Random.Range(0, goodPositions.Count);
				spawnPosition = goodPositions[index];
				zoneIndex = index;
			}
			else
			{
				spawnPosition = CalculateSpawnPointWithLeastOpponents(spawnZones, controllers, ref zoneIndex);
			}
			
			if (Physics.Raycast(spawnPosition + Vector3.up, Vector3.down, out hit))
			{
				spawnPosition = new Vector3(spawnPosition.x, hit.point.y, spawnPosition.z);
			}

			return spawnPosition;
		}

		public static Vector3 CalculateSpawnPoint(SpawnZone spawnZone, ref int functionCount, ref bool foundPoint, List<Controller> allPlayers)
		{
			functionCount++;

			var spawnPoint = CharacterHelper.SpawnPosition(spawnZone);

			var noPlayersNearby = true;

			foreach (var player in allPlayers.Where(player => Vector3.Distance(player.transform.position, spawnPoint) < 10))
			{
				noPlayersNearby = false;
			}

			if (noPlayersNearby)
			{
				foundPoint = true;
				return spawnPoint;
			}

			if (functionCount < 5)
				spawnPoint = CalculateSpawnPoint(spawnZone, ref functionCount, ref foundPoint, allPlayers);

			return spawnPoint;
		}

		public static Vector3 CalculateSpawnPointWithLeastOpponents(List<SpawnZone> spawnZones, List<Controller> allPlayers, ref int zoneIndex)
		{
			var bestDistances = new List<float>();
			var bestPoints = new List<Vector3>();
			var indexes = new List<int>();

			for (var i = 0; i < spawnZones.Count; i++)
			{
				var zone = spawnZones[i];
				var allegedPoints = new List<Vector3>();
				var distances = new List<float>();

				indexes.Add(i);
				
				for (int j = 0; j < 5; j++)
				{
					distances.Add(float.MaxValue);
					allegedPoints.Add(CharacterHelper.SpawnPosition(zone));
				}

				for (var k = 0; k < allegedPoints.Count; k++)
				{
					foreach (var player in allPlayers)
					{
						if (Vector3.Distance(allegedPoints[k], player.transform.position) < distances[k])
							distances[k] = Vector3.Distance(allegedPoints[k], player.transform.position);
					}
				}

//				Debug.LogError("distance in bad point | area " + i + " dist " + distances.Max());
				bestPoints.Add(allegedPoints[distances.IndexOf(distances.Max())]);
				bestDistances.Add(distances.Max());
			}

//			Debug.LogError("best distance | area " + bestDistances.IndexOf(bestDistances.Max()) + " dist " + bestDistances.Max());
			
			zoneIndex = indexes[bestDistances.IndexOf(bestDistances.Max())];
			return bestPoints[bestDistances.IndexOf(bestDistances.Max())];
		}
		
#if PHOTON_UNITY_NETWORKING
		public static int LivePlayerCount(List<Player> players)
		{
			var numberOfLivePlayers = 0;
                    
			foreach (var player in players)
			{
				if (player.CustomProperties.Count <= 0) continue;

				if ((int) player.CustomProperties["d"] == 0)
				{
					numberOfLivePlayers++;
				}
			}

			return numberOfLivePlayers;
		}

		public static List<Controller> GetOnlyTeammates()
		{
			var allPlayers = GameObject.FindObjectsOfType<Controller>().ToList();
			var removedPlayers = allPlayers.Where(player => (PUNHelper.Teams) PhotonNetwork.LocalPlayer.CustomProperties["pt"] == player.MyTeam).ToList();

			foreach (var player in removedPlayers)
			{
				if (allPlayers.Contains(player))
					allPlayers.Remove(player);
			}

//			Debug.LogError("team instns | all players" + allPlayers.Count + " | my teammates " + removedPlayers.Count);

			return allPlayers;
		}

		public static List<Player> ManagePlayers(List<Player> _players, bool isSurvive)
		{
			var sortPlayers = new List<Player>();

			if (!isSurvive)
			{
				sortPlayers = _players.OrderByDescending(t => t.CustomProperties["k"]).ThenBy(t => t.CustomProperties["d"]).ToList();
			}
			else
			{
				var livePlayers = _players.ToList().Where(player => (int) player.CustomProperties["d"] == 0).ToList();
				var deadPlayers = _players.ToList().Where(player => (int) player.CustomProperties["d"] >= 1).ToList();

				livePlayers.OrderByDescending(t => t.CustomProperties["k"]);
				deadPlayers.OrderBy(t => t.CustomProperties["pl"]);
				
//				Debug.LogError(livePlayers.Count);

				sortPlayers.AddRange(livePlayers);
				sortPlayers.AddRange(deadPlayers);
				
				//sortPlayers = livePlayers.Concat(deadPlayers).ToList();
			}

			return sortPlayers;
		}

		public static void ManagePointUI(UIManager currentUIManager, RoomManager manager)
		{
			SetPointColor(currentUIManager.MultiplayerGameRoom.MatchStats.A_CapturedFill, manager.A_Point, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget, "act");
			SetPointColor(currentUIManager.MultiplayerGameRoom.MatchStats.B_CapturedFill, manager.B_Point, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget, "bct");
			SetPointColor(currentUIManager.MultiplayerGameRoom.MatchStats.C_CapturedFill, manager.C_Point, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget, "cct");
			SetPointColor(currentUIManager.MultiplayerGameRoom.MatchStats.HardPoint_CapturedFill, manager.CurrentHardPoint, currentUIManager.MultiplayerGameRoom.MatchStats.A_ScreenTarget, "hpct");

			if (currentUIManager.MultiplayerGameRoom.MatchStats.A_CurrentFill)
				currentUIManager.MultiplayerGameRoom.MatchStats.A_CurrentFill.fillAmount = (float) PhotonNetwork.CurrentRoom.CustomProperties["acv"];
            
			if (currentUIManager.MultiplayerGameRoom.MatchStats.B_CurrentFill)
				currentUIManager.MultiplayerGameRoom.MatchStats.B_CurrentFill.fillAmount = (float) PhotonNetwork.CurrentRoom.CustomProperties["bcv"];
            
			if (currentUIManager.MultiplayerGameRoom.MatchStats.C_CurrentFill)
				currentUIManager.MultiplayerGameRoom.MatchStats.C_CurrentFill.fillAmount = (float) PhotonNetwork.CurrentRoom.CustomProperties["ccv"];
			
			if (currentUIManager.MultiplayerGameRoom.MatchStats.HardPoint_CurrentFill)
				currentUIManager.MultiplayerGameRoom.MatchStats.HardPoint_CurrentFill.fillAmount = (float) PhotonNetwork.CurrentRoom.CustomProperties["hpcv"];
		}

		static void SetPointColor(Image capturedFill, CapturePoint point, RawImage screenTarget, string property)
		{
			if (capturedFill)
			{
				switch ((Teams) PhotonNetwork.CurrentRoom.CustomProperties[property])
				{
					case Teams.Null:
						
						if(point && point.gameObject.GetComponent<RawImage>())
							point.gameObject.GetComponent<RawImage>().color = Color.white;
						
						if(screenTarget)
							screenTarget.color = Color.white;

						capturedFill.color = new Color(0, 0, 0, 0);
						
						break;
					
					case Teams.Red:
						
						if(point && point.gameObject.GetComponent<RawImage>())
							point.gameObject.GetComponent<RawImage>().color = Color.red;
						
						if(screenTarget)
							screenTarget.color = Color.red;
						
						capturedFill.color = Color.red;
						
						break;
					
					case Teams.Blue:
						
						if(point && point.gameObject.GetComponent<RawImage>())
							point.gameObject.GetComponent<RawImage>().color = Color.blue;
						
						if(screenTarget)
							screenTarget.color = Color.blue;
						
						capturedFill.color = Color.blue;
						
						break;
				}
			}
		}

		public static void SetScreenTarget(RawImage pointImage, GameObject point, Controller controller)
		{
			if (pointImage && point)
			{
				var screenPosition = controller.CameraController.MainCamera.GetComponent<Camera>().WorldToScreenPoint(point.transform.position);
				var isTargetVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;

				if (isTargetVisible)
				{
					screenPosition.z = 0;
				}
				else
				{
					var angle = float.MinValue;
					var screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
					UIHelper.GetScreenTarget(ref screenPosition, ref angle, screenCenter, screenCenter * 0.8f);
				}

				pointImage.transform.position = screenPosition;
			}
		}

		public static void CheckPoint(string point, ref Teams lastCapturedTeam, UIManager currentUIManager, RoomManager manager, bool immediately, GameObject realPoint, RawImage screentTarget)
		{
			var capturingPlayers = PhotonNetwork.PlayerList.ToList().FindAll(player => player.CustomProperties.Count > 0 && (bool)player.CustomProperties["wl"] && (bool)player.CustomProperties[point]);
                    
			var redPlayers = capturingPlayers.FindAll(player => (Teams) player.CustomProperties["pt"] == Teams.Red);
			var bluePlayers =  capturingPlayers.FindAll(player => (Teams) player.CustomProperties["pt"] == Teams.Blue);

			var advantage = Math.Abs(redPlayers.Count - bluePlayers.Count);

			var capturedTeam = "";
			var capturedValue = "";
			Image capturedFill = null;
			Image currentFill = null;
			
			switch (point)
			{
				case "ac":
					capturedTeam = "act";
					capturedValue = "acv";
					capturedFill = currentUIManager.MultiplayerGameRoom.MatchStats.A_CapturedFill;
					currentFill = currentUIManager.MultiplayerGameRoom.MatchStats.A_CurrentFill;
					break;
				
				case "bc":
					capturedTeam = "bct";
					capturedValue = "bcv";
					capturedFill = currentUIManager.MultiplayerGameRoom.MatchStats.B_CapturedFill;
					currentFill = currentUIManager.MultiplayerGameRoom.MatchStats.B_CurrentFill;
					break;
				
				case "cc":
					capturedTeam = "cct";
					capturedValue = "ccv";
					capturedFill = currentUIManager.MultiplayerGameRoom.MatchStats.C_CapturedFill;
					currentFill = currentUIManager.MultiplayerGameRoom.MatchStats.C_CurrentFill;
					break;
				
				case "hpc":
					capturedTeam = "hpct";
					capturedValue = "hpcv";
					capturedFill = currentUIManager.MultiplayerGameRoom.MatchStats.HardPoint_CapturedFill;
					currentFill = currentUIManager.MultiplayerGameRoom.MatchStats.HardPoint_CurrentFill;
					break;
			}
			
			if (redPlayers.Count > bluePlayers.Count)
			{
				if (capturedTeam == "hpct" && bluePlayers.Count == 0 || capturedTeam != "hpct")
					CapturePoint(Teams.Red, ref lastCapturedTeam, currentFill, capturedFill, advantage, capturedTeam, immediately, redPlayers, bluePlayers, manager, realPoint, screentTarget);
				else if (capturedTeam == "hpct" && bluePlayers.Count > 0)
				{
					ZeroizeHardPoint(currentUIManager, ref lastCapturedTeam, realPoint, screentTarget);
				}
			}
			else if (redPlayers.Count < bluePlayers.Count)
			{
				if (capturedTeam == "hpct" && redPlayers.Count == 0 || capturedTeam != "hpct")
					CapturePoint(Teams.Blue, ref lastCapturedTeam, currentFill, capturedFill, advantage, capturedTeam, immediately, redPlayers, bluePlayers, manager, realPoint, screentTarget);
				else if (capturedTeam == "hpct" && redPlayers.Count > 0)
				{
					ZeroizeHardPoint(currentUIManager, ref lastCapturedTeam, realPoint, screentTarget);
				}
			}
			else
			{
				if (capturedTeam == "hpct" && redPlayers.Count > 0)
				{
					ZeroizeHardPoint(currentUIManager, ref lastCapturedTeam, realPoint, screentTarget);
				}    
			}
			

			if (capturingPlayers.Count == 0)
			{
				currentFill.fillAmount -= 1 * Time.deltaTime;
			}
			
			if(PhotonNetwork.IsMasterClient)
				PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{capturedValue, currentFill.fillAmount}});
		}

		public static void ZeroizeHardPoint(UIManager uiManager, ref Teams lastCapturedTeam, GameObject point, RawImage screenPoint)
		{
			var currentFill = uiManager.MultiplayerGameRoom.MatchStats.HardPoint_CurrentFill;
			var capturedFill = uiManager.MultiplayerGameRoom.MatchStats.HardPoint_CapturedFill;
			
			currentFill.fillAmount = 0;
			capturedFill.color = new Color(0, 0, 0, 0);
			
			if(point && point.GetComponent<RawImage>())
				point.GetComponent<RawImage>().color = Color.white;
			
			if(screenPoint)
				screenPoint.color = Color.white;

			if (lastCapturedTeam != Teams.Null)
			{
				lastCapturedTeam = Teams.Null;
				PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"hpct", lastCapturedTeam}});
			}
			
			if(PhotonNetwork.IsMasterClient)
				PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{"hpcv", currentFill.fillAmount}});
		}

		public static void CapturePoint(Teams team, ref Teams lastCapturedTeam, Image currentFill, Image capturedFill, int advantage, string property, bool immediately, List<Player> redPlayers, List<Player> bluePlayers, RoomManager manager, GameObject point, RawImage screenTarget)
		{
			if ((Teams) PhotonNetwork.CurrentRoom.CustomProperties[property] != team)
			{
				if (lastCapturedTeam != team)
				{
					currentFill.fillAmount = 0;
					lastCapturedTeam = team;
				}

				if ((Teams) PhotonNetwork.CurrentRoom.CustomProperties[property] == Teams.Null)
					capturedFill.color = new Color(0, 0, 0, 0);
				else capturedFill.color = (Teams) PhotonNetwork.CurrentRoom.CustomProperties[property] == Teams.Red ? Color.red : Color.blue;

				currentFill.color = team == Teams.Red ? Color.red : Color.blue;

				if (!immediately)
					currentFill.fillAmount += 0.1f * advantage * Time.deltaTime;
				else currentFill.fillAmount = 1;

				if (currentFill.fillAmount >= 1)
				{
					currentFill.color = new Color(0, 0, 0, 0);

					if (team == Teams.Red)
					{
						capturedFill.color = Color.red;
						
						if(point && point.GetComponent<RawImage>())
							point.GetComponent<RawImage>().color = Color.red;
						
						if(screenTarget)
							screenTarget.color = Color.red;

						if (redPlayers.Contains(PhotonNetwork.LocalPlayer))
						{
							manager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.gameObject.SetActive(true);
							manager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + PlayerPrefs.GetInt(redPlayers.Count > 1 ? "CaptureAssist" : "CapturePoint") + " (Capture)";
							manager.StartCoroutine(manager.AddScorePopupDisableTimeout());
						}
						
						if (PhotonNetwork.IsMasterClient)
						{
							foreach (var player in redPlayers)
							{
								var score = (int) player.CustomProperties["cms"];
								score += PlayerPrefs.GetInt(redPlayers.Count > 1 ? "CaptureAssist" : "CapturePoint");
								player.SetCustomProperties(new Hashtable {{"cms", score}});
							}
						}
					}
					else
					{
						capturedFill.color = Color.blue;
						
						if(point && point.GetComponent<RawImage>())
							point.GetComponent<RawImage>().color = Color.blue;
						
						if(screenTarget)
							screenTarget.color = Color.blue;

						if (bluePlayers.Contains(PhotonNetwork.LocalPlayer))
						{
							manager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.gameObject.SetActive(true);
							manager.currentUIManager.MultiplayerGameRoom.MatchStats.AddScorePopup.text = "+ " + PlayerPrefs.GetInt(bluePlayers.Count > 1 ? "CaptureAssist" : "CapturePoint") + " (Capture)";
							manager.StartCoroutine(manager.AddScorePopupDisableTimeout());
						}

						if (PhotonNetwork.IsMasterClient)
						{
							foreach (var player in bluePlayers)
							{
								var score = (int) player.CustomProperties["cms"];
								score += PlayerPrefs.GetInt(redPlayers.Count > 1 ? "CaptureAssist" : "CapturePoint");
								player.SetCustomProperties(new Hashtable {{"cms", score}});
							}
						}
					}

					PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{property, team}});
				}
			}
			else if ((Teams) PhotonNetwork.CurrentRoom.CustomProperties[property] == team && currentFill.fillAmount <= 1)
			{
				currentFill.color = new Color(0, 0, 0, 0);
				capturedFill.color = team == Teams.Red ? Color.red : Color.blue;
				
				if(point && point.GetComponent<RawImage>())
					point.GetComponent<RawImage>().color = team == Teams.Red ? Color.red : Color.blue;
				
				if(screenTarget)
					screenTarget.color = team == Teams.Red ? Color.red : Color.blue;
			}
		}

		public static void CalculateCapturedScores(string point, int addScore)
		{
			int score;

			switch ((Teams)PhotonNetwork.CurrentRoom.CustomProperties[point])
			{
				case Teams.Red:
					score = (int)PhotonNetwork.CurrentRoom.CustomProperties["rs"];
					score += addScore;
					PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{"rs", score}});
					break;
				
				case Teams.Blue:
					score = (int)PhotonNetwork.CurrentRoom.CustomProperties["bs"];
					score += addScore;
					PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable{{"bs", score}});
					break;
			}
		}

		public static void InstantiateIcons(List<Player> allPlayers, List<Player> leftPlayers, List<Player> deadPlayers, Transform parent, RawImage iconPrefab)
		{
			foreach (var player in allPlayers)
			{
				if ((int) player.CustomProperties["d"] == 0)
				{
					var icon = Object.Instantiate(iconPrefab, parent);
					icon.rectTransform.localEulerAngles = new Vector3(180, 0, 0);
					icon.texture = Resources.Load((string) player.CustomProperties["ai"]) as Texture;
				}

				// (example) icon.texture = player.texture;
			}

			foreach (var leftPlayer in leftPlayers)
			{
				var icon = Object.Instantiate(iconPrefab, parent);
				icon.texture = Resources.Load((string) leftPlayer.CustomProperties["ai"]) as Texture;
				var color = icon.color;
				color = new Color(color.a, color.b, color.g, 0.3f);
				icon.color = color;
			}

			foreach (var deadPlayer in deadPlayers)
			{
				if (!leftPlayers.Contains(deadPlayer))
				{
					var icon = Object.Instantiate(iconPrefab, parent);
					icon.texture = Resources.Load((string) deadPlayer.CustomProperties["ai"]) as Texture;
					var color = icon.color;
					color = new Color(color.a, color.b, color.g, 0.3f);
					icon.color = color;
				}
			}
		}

		public static string ConvertRegionToCode(int value)
		{
			switch (value)
			{
				case 0:
					return "asia";

				case 1:
					return "au";
               
				case 2:
					return "cae";
                
				case 3:
					return "eu";
                
				case 4:
					return "in";
                
				case 5:
					return "jp";
                
				case 6:
					return "ru";
                
				case 7:
					return "rue";
                
				case 8:
					return "sa";
                
				case 9:
					return "kr";
                
				case 10:
					return "us";
                
				case 11:
					return "usw";
			}

			return "";
		}
		
		public static int ConvertCodeToRegion(string value)
		{
			switch (value)
			{
				case "asia":
					return 0;

				case "au":
					return 1;
               
				case "cae":
					return 2;
                
				case "eu":
					return 3;
                
				case "in":
					return 4;
                
				case "jp":
					return 5;
                
				case "ru":
					return 6;
                
				case "rue":
					return 7;
                
				case "sa":
					return 8;
                
				case "kr":
					return 9;
                
				case "us":
					return 10;
                
				case "usw":
					return 11;
			}

			return 0;
		}
#endif
	}
}
