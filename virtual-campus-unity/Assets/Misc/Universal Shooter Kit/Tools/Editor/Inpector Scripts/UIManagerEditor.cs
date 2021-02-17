using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(UIManager))]
	public class UIManagerEditor : Editor
	{
		public UIManager script;
		public ReorderableList graphicsButtons;
		public ReorderableList bloodHitMarks;
		private GUIStyle style;

		public void Awake()
		{
			script = (UIManager) target;
		}

		private void OnEnable()
		{
			
			graphicsButtons = new ReorderableList(serializedObject, serializedObject.FindProperty("SinglePlayerGame._GraphicsButtons"), true, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Graphic Settings Button");
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), "Quality Layer");
				},
				
				onAddCallback = items =>
				{
					script.SinglePlayerGame._GraphicsButtons.Add(new UIHelper.singlePlayerGame.GraphicsButtons());
				},

				onRemoveCallback = items =>
				{
					script.SinglePlayerGame._GraphicsButtons.Remove(script.SinglePlayerGame._GraphicsButtons[items.index]);
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.SinglePlayerGame._GraphicsButtons[index].Button = (Button)
						EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight),
							script.SinglePlayerGame._GraphicsButtons[index].Button, typeof(Button), true);

					script.SinglePlayerGame._GraphicsButtons[index].QualityIndex = EditorGUI.Popup(
						new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), script.SinglePlayerGame._GraphicsButtons[index].QualityIndex, QualitySettings.names);
				}
			};
			
			bloodHitMarks = new ReorderableList(serializedObject, serializedObject.FindProperty("CharacterUI.hitMarkers"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Damage Marks");
				},
				
				onAddCallback = items =>
				{
					script.CharacterUI.hitMarkers.Add(null);
				},

				onRemoveCallback = items =>
				{
					script.CharacterUI.hitMarkers.Remove(script.CharacterUI.hitMarkers[items.index]);
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.CharacterUI.hitMarkers[index] = (RawImage) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.CharacterUI.hitMarkers[index], typeof(RawImage), true);
				}
			};
			
			EditorApplication.update += Update;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		void Update()
		{		
			
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			style = new GUIStyle(EditorStyles.helpBox) {richText = true, fontSize = 10};
			
			EditorGUILayout.Space();
			
#if !UNITY_2018_3_OR_NEWER
			EditorGUILayout.LabelField("Place this prefab in a scene, adjust UI elements, then <b><color=green>apply changes</color></b>.", style);
#else
			EditorGUILayout.LabelField("Open this prefab, to adjust UI elements.", style);
#endif

			EditorGUILayout.Space();
			
			EditorGUILayout.BeginVertical("box");
			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Single-player Game", "Multiplayer Game", "Character UI"});
			EditorGUILayout.EndVertical();

			switch (script.inspectorTab)
			{
				case 0:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Pause Menu", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.PauseMainObject"), new GUIContent("Main Object"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.Exit"), new GUIContent("Exit Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.Resume"), new GUIContent("Resume Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.Restart"), new GUIContent("Restart Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.Options"), new GUIContent("Options Button"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Options Menu", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("helpbox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.OptionsMainObject"), new GUIContent("Main Object"));
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SinglePlayerGame.OptionsBack"), new GUIContent("Back Button"));
					EditorGUILayout.Space();
					graphicsButtons.DoLayoutList();
					EditorGUILayout.EndVertical();

					break;


				case 1:

					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("box");
					script.multiplayerGameInspectorTab = GUILayout.Toolbar(script.multiplayerGameInspectorTab, new[] {"Lobby", "Room", "UI Prefabs"});
					EditorGUILayout.EndVertical();
					
					switch (script.multiplayerGameInspectorTab)
					{
						case 0:
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("helpbox");

							script.lobbyInspectorTabTop = GUILayout.Toolbar(script.lobbyInspectorTabTop, new[] {"Main Menu", "Game Modes Menu", "Maps Menu"});

							switch (script.lobbyInspectorTabTop)
							{
								case 0:
									script.lobbyInspectorTabMiddle = 3;
									script.lobbyInspectorTabBottom = 3;
									script.currentLobbyInspectorTab = 0;
									break;
								
								case 1:
									script.lobbyInspectorTabMiddle = 3;
									script.lobbyInspectorTabBottom = 3;
									script.currentLobbyInspectorTab = 1;
									break;
								
								case 2:
									script.lobbyInspectorTabMiddle = 3;
									script.lobbyInspectorTabBottom = 3;
									script.currentLobbyInspectorTab = 2;
									break;
							}

							
							script.lobbyInspectorTabMiddle = GUILayout.Toolbar(script.lobbyInspectorTabMiddle, new[] {"Loadout Menu", "Avatars Menu", "Characters Menu"});
							
							switch (script.lobbyInspectorTabMiddle)
							{
								case 0:
									script.lobbyInspectorTabTop = 3;
									script.lobbyInspectorTabBottom = 3;
									script.currentLobbyInspectorTab = 3;
									break;
								
								case 1:
									script.lobbyInspectorTabTop = 3;
									script.lobbyInspectorTabBottom = 3;
									script.currentLobbyInspectorTab = 4;
									break;
								
								case 2:
									script.lobbyInspectorTabTop = 3;
									script.lobbyInspectorTabBottom = 3;
									script.currentLobbyInspectorTab = 5;
									break;
							}
							
							script.lobbyInspectorTabBottom = GUILayout.Toolbar(script.lobbyInspectorTabBottom, new[] {"All Rooms Menu", "Create Room Menu"});
							
							switch (script.lobbyInspectorTabBottom)
							{
								case 0:
									script.lobbyInspectorTabMiddle = 3;
									script.lobbyInspectorTabTop = 3;
									script.currentLobbyInspectorTab = 6;
									break;
								
								case 1:
									script.lobbyInspectorTabMiddle = 3;
									script.lobbyInspectorTabTop = 3;
									script.currentLobbyInspectorTab = 7;
									break;
								
//								case 2:
//									script.lobbyInspectorTabMiddle = 3;
//									script.lobbyInspectorTabTop = 3;
//									script.currentLobbyInspectorTab = 5;
//									break;
							}
							EditorGUILayout.EndVertical();

							switch (script.currentLobbyInspectorTab)
							{
								case 0:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.Nickname"), new GUIContent("Nickname"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.PlayerScore"), new GUIContent("Score"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.Avatar"), new GUIContent("Avatar"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.ChangeAvatarButton"), new GUIContent("Change Avatar"));
									EditorGUILayout.EndVertical();
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.ConnectionStatus"), new GUIContent("Connection Status"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.RegionsDropdown"), new GUIContent("Regions Dropdown"));

//									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.ConnectButton"), new GUIContent("Connect Button"));
									EditorGUILayout.EndVertical();
									
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.CurrentModeAndMap"), new GUIContent("Current Mode & Map"));
									//bottun to choose map
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.ChooseGameModeButton"), new GUIContent("Choose Mode & Map"));
									//character change button
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.ChangeCharacter"), new GUIContent("Change Character"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.PlayButton"), new GUIContent("Play"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.LoadoutButton"), new GUIContent("Open Loadout"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.AllRoomsButton"), new GUIContent("All Rooms"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.CreateRoomButton"), new GUIContent("Create Room"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.CreditsButton"), new GUIContent("Credits"));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreditsMenu.MainObject"), new GUIContent("Credits MainObject"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreditsMenu.BackButton"), new GUIContent("Credits BackButton"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.BGMAudioSource"), new GUIContent("BGM Audio Source"));
									EditorGUILayout.EndVertical();

									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.SettingsMenu.MainObject"), new GUIContent("Settings Menu"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.SettingsMenu.BackButton"), new GUIContent("Settings Menu BackButton"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MainMenu.SettingsButton"), new GUIContent("Settings Button"));
									EditorGUILayout.EndVertical();


									break;
								
								case 1:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.GameModesMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.GameModesMenu.Content"), new GUIContent("Content"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.GameModesMenu.Info"), new GUIContent("Mode Info"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.GameModesMenu.MapsButton"), new GUIContent("Choose Map"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.GameModesMenu.MapButtonText"), new GUIContent("Button Text"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.GameModesMenu.BackButton"), new GUIContent("Close"));
									EditorGUILayout.EndVertical();
									break;
								
								case 2:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MapsMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MapsMenu.Content"), new GUIContent("Content"));
									EditorGUILayout.EndVertical();
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MapsMenu.GameModesButton"), new GUIContent("Choose Game Mode"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MapsMenu.GameModesButtonText"), new GUIContent("Button Text"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.MapsMenu.BackButton"), new GUIContent("Close"));

									EditorGUILayout.EndVertical();
									break;
								
								case 3:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.LoadoutMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.LoadoutMenu.Content"), new GUIContent("Content"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.LoadoutMenu.Info"), new GUIContent("Weapon Info"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.LoadoutMenu.EquipButton"), new GUIContent("Equip & Remove"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.LoadoutMenu.EquipButtonText"), new GUIContent("Button Text"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.LoadoutMenu.BackButton"), new GUIContent("Close"));

									EditorGUILayout.EndVertical();
									break;
								
								case 4:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AvatarsMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AvatarsMenu.Content"), new GUIContent("Content"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AvatarsMenu.BackButton"), new GUIContent("Close"));
									EditorGUILayout.EndVertical();
									break;
								
								case 5:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CharactersMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CharactersMenu.UpButton"), new GUIContent("->"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CharactersMenu.DownButton"), new GUIContent("<-"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CharactersMenu.BackButton"), new GUIContent("Close"));
									EditorGUILayout.EndVertical();
									break;
								
								case 6:
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AllRoomsMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AllRoomsMenu.Content"), new GUIContent("Content"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AllRoomsMenu.Password"), new GUIContent("Password"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AllRoomsMenu.BackButton"), new GUIContent("Close"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.AllRoomsMenu.JoinButton"), new GUIContent("Join"));
									EditorGUILayout.EndVertical();
							
									break;
								
								
								case 7:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreateRoomMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreateRoomMenu.GameName"), new GUIContent("Game Name"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreateRoomMenu.Password"), new GUIContent("Password"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreateRoomMenu.CurrentModeAndMap"), new GUIContent("Selected Level & Map"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreateRoomMenu.CreateButton"), new GUIContent("Create"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.CreateRoomMenu.BackButton"), new GUIContent("Close"));
									EditorGUILayout.EndVertical();
									break;
							}
							
							break;


						case 1:
							
//							EditorGUILayout.LabelField("<b>Note:</b> " + "\n\n" +
//							                           " ◆ Teams UI - these elements are used for modes in which the <b>[Use Teams]</b> parameter <b>is active</b>. Players will be divided into teams and will be displayed in two lists." + "\n\n" +
//							                           " ◆ Not Teams UI - these are used for modes in which the <b>[Use Teams]</b> parameter <b>is not active</b>. All players will be displayed in one list.", style);
							
							EditorGUILayout.Space();
							script.roomInspectorTabTop = GUILayout.Toolbar(script.roomInspectorTabTop, new[] {"Start Menu", "Pause Menu", "Game Over Menu"});

							switch (script.roomInspectorTabTop)
							{
								case 0:
                                    script.roomInspectorTabDown = 3;
                                    script.roomInspectorTabMid = 3;
									script.currentRoomInspectorTab = 0;
									break;
								case 1:
                                    script.roomInspectorTabDown = 3;
                                    script.roomInspectorTabMid = 3;
									script.currentRoomInspectorTab = 1;
									break;
								case 2:
                                    script.roomInspectorTabDown = 3;
                                    script.roomInspectorTabMid = 3;
									script.currentRoomInspectorTab = 2;
									break;
							}

							script.roomInspectorTabMid = GUILayout.Toolbar(script.roomInspectorTabMid, new[] {"Pre-Match Menus", "Match Stats", "Death Screens"});

							switch (script.roomInspectorTabMid)
							{
								case 0:
                                    script.roomInspectorTabDown = 3;
                                    script.roomInspectorTabTop = 3;
									script.currentRoomInspectorTab = 3;
									break;
								case 1:
                                    script.roomInspectorTabDown = 3;
                                    script.roomInspectorTabTop = 3;
									script.currentRoomInspectorTab = 4;
									break;
								case 2:
                                    script.roomInspectorTabDown = 3;
                                    script.roomInspectorTabTop = 3;
									script.currentRoomInspectorTab = 5;
									break;
							}

                            script.roomInspectorTabDown = GUILayout.Toolbar(script.roomInspectorTabDown, new[] { "Loading Screen" });

                            switch (script.roomInspectorTabDown)
                            {
                                case 0:
                                    script.roomInspectorTabTop = 3;
                                    script.roomInspectorTabMid = 3;
                                    script.currentRoomInspectorTab = 6;
                                    break;
                            }

                            

                            switch (script.currentRoomInspectorTab)
							{
                                

                                case 6:

                                    EditorGUILayout.Space();
                                    EditorGUILayout.HelpBox("The loading screen", MessageType.Info);
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginVertical("helpbox");
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.LoadingScreen.MainObject"), new GUIContent("Main Object"));

                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.LoadingScreen.Status"), new GUIContent("Status"));
                                    EditorGUILayout.EndVertical();

                                    break;


                                case 0:
									
									EditorGUILayout.Space();
									EditorGUILayout.HelpBox("This is the menu in which players are searched. It is the same for all modes.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.MainObject"), new GUIContent("Main Object"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.FindPlayersTimer"), new GUIContent("Timer"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.FindPlayersStatsText"), new GUIContent("Status"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.PlayersContent"), new GUIContent("Players Content"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.ExitButton"), new GUIContent("Exit Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.StartButton"), new GUIContent("Start Button"));
									
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.ChooseMapButton"), new GUIContent("Choose Map Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.MapsMenuRoom"), new GUIContent("Maps Menu Room"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.SwitchButton"), new GUIContent("Switch Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.ChangeButton"), new GUIContent("Change Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.TeamList"), new GUIContent("Team List"));
									
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.TeamListContent"), new GUIContent("Team List Content"));
									
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.ChangeCharacter"), new GUIContent("Change Character Menu"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.ExitCharacterButton"), new GUIContent("Exit Character Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.LeftCharacterButton"), new GUIContent("Left Character Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.RightCharacterButton"), new GUIContent("Right Character Button"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.IconTypeShwon"), new GUIContent("Icon Type Shwon"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.ExitButtonMapMenu"), new GUIContent("Exit Button Map Menu"));

									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.StartMenu.MapMenuContent"), new GUIContent("Map Menu Content"));

									EditorGUILayout.EndVertical();

									break;

								case 1:
									EditorGUILayout.Space();
									EditorGUILayout.HelpBox("This is the menu in which statistics of all players are displayed (kills/deaths, score, status). It is also used during a pause.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.ExitButton"), new GUIContent("Exit"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.ResumeButton"), new GUIContent("Resume"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.HelpBox("If the player creates a custom room, this text will display the name and password that he has set.", MessageType.Info);
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.CurrentGameAndPassword"), new GUIContent("Game Name & Pause"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Teams UI", EditorStyles.boldLabel);

									EditorGUILayout.BeginVertical("helpbox");

									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.TeamsPauseMenuMain"), new GUIContent("Main Object"));
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("1st Team", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.RedTeamName"), new GUIContent("Name"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.RedTeamScore"), new GUIContent("Score"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.RedTeamTotalWins"), new GUIContent("Rounds Won"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.RedTeamContent"), new GUIContent("Content"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("2nd Team", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.BlueTeamName"), new GUIContent("Name"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.BlueTeamScore"), new GUIContent("Score"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.BlueTeamTotalWins"), new GUIContent("Rounds Won"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.BlueTeamContent"), new GUIContent("Content"));

									EditorGUILayout.EndVertical();
									
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();

									EditorGUILayout.LabelField("Not Teams UI", EditorStyles.boldLabel);

									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.NotTeamsPauseMenuMain"), new GUIContent("Main Object"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PauseMenu.NotTeamsContent"), new GUIContent("Players Content"));
									EditorGUILayout.EndVertical();
									break;

								case 2:
									EditorGUILayout.Space();
									EditorGUILayout.HelpBox("This menu is displayed after the end of the round or the whole match and it shows who won.", MessageType.Info);
									EditorGUILayout.Space();

									EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PlayAgainButton"), new GUIContent("Play Again"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.MatchStatsButton"), new GUIContent("Show Stats"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.ExitButton"), new GUIContent("Exit"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.BackButton"), new GUIContent("Back"));

									EditorGUILayout.EndVertical();

									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Not Teams UI", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.NotTeamsMainObject"), new GUIContent("Main Object"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.NotTeamsStatus"), new GUIContent("Place Status"));
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("1st Player", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(0).FindPropertyRelative("MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(0).FindPropertyRelative("Nickname"), new GUIContent("Nickname"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(0).FindPropertyRelative("Score"), new GUIContent("Score"));

									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("2nd Player", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(1).FindPropertyRelative("MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(1).FindPropertyRelative("Nickname"), new GUIContent("Nickname"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(1).FindPropertyRelative("Score"), new GUIContent("Score"));

									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("3rd Player", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(2).FindPropertyRelative("MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(2).FindPropertyRelative("Nickname"), new GUIContent("Nickname"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.PodiumPlayers").GetArrayElementAtIndex(2).FindPropertyRelative("Score"), new GUIContent("Score"));

									EditorGUILayout.EndVertical();
									EditorGUILayout.EndVertical();

									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Teams UI", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.TeamsMainObject"), new GUIContent("Main Object"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.TeamsStatus"), new GUIContent("Victory Status"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.RoundStatusText"), new GUIContent("Round Status"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.VictoryImage"), new GUIContent("Victory Image"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.DefeatImage"), new GUIContent("Defeat Image"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.DrawImage"), new GUIContent("Draw Image"));
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("1st Team", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.RedTeamName"), new GUIContent("Name"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.RedTeamScore"), new GUIContent("Score"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.RedTeamLogoPlaceholder"), new GUIContent("Logo Placeholder"));
									EditorGUILayout.EndVertical();
									
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("2nd Team", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.BlueTeamName"), new GUIContent("Name"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.BlueTeamScore"), new GUIContent("Score"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.GameOverMenu.BlueTeamLogoPlaceholder"), new GUIContent("Logo Placeholder"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									break;

								case 4:
									EditorGUILayout.Space();
									EditorGUILayout.HelpBox("There are all UI that are displayed during the game.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.KillStatsContent"), new GUIContent("K/D Content"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.MatchTimer"), new GUIContent("Match Timer"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.TargetText"), new GUIContent("Match Target"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.AddScorePopup"), new GUIContent("Add Score Popup"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.TeamLOGO"), new GUIContent("Team LOGO"));
                                    EditorGUILayout.EndVertical();
									EditorGUILayout.Space();

									EditorGUILayout.HelpBox("If the [Use Respawns] parameter is not active, the player won't be respawned. This mode calls 'Survival'." + "\n" +
									                           "If this parameter is active it is the 'Normal' mode (also it contains Domination & Hard Point).", MessageType.Info);

									EditorGUILayout.BeginVertical("helpbox");
									script.roomMatchStatsTab = GUILayout.Toolbar(script.roomMatchStatsTab, new[] {"Normal Mode", "Survival Mode"});
									EditorGUILayout.EndVertical();
									
									switch (script.roomMatchStatsTab)
									{
										case 0:
											
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("Teams UI", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.TeamsMatchUIMain"), new GUIContent("Main Object"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.RedTeamMatchStats"), new GUIContent("1st Team Score"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.BlueTeamMatchStats"), new GUIContent("2nd Team Score"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.TeamImagePlaceholder"), new GUIContent("Logo Placeholder"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("UI for Domination", EditorStyles.boldLabel);
											EditorGUILayout.HelpBox("If the [Match Target] is Point Retention and [Points Count] = 3, these elements will be displayed.", MessageType.Info);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.DominationMain"), new GUIContent("Main Object"));
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("A Point", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.A_CurrentFill"), new GUIContent("Current Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.A_CapturedFill"), new GUIContent("Captured Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.A_ScreenTargetTexture"), new GUIContent("Screen Target Texture"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("B Point", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.B_CurrentFill"), new GUIContent("Current Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.B_CapturedFill"), new GUIContent("Captured Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.B_ScreenTargetTexture"), new GUIContent("Screen Target Texture"));

											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("C Point", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.C_CurrentFill"), new GUIContent("Current Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.C_CapturedFill"), new GUIContent("Captured Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.C_ScreenTargetTexture"), new GUIContent("Screen Target Texture"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("UI for Hard Point", EditorStyles.boldLabel);
											EditorGUILayout.HelpBox("If the [Match Target] is Point Retention and [Points Count] = 1, these elements will be displayed.", MessageType.Info);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.HardPointMain"), new GUIContent("Main Object"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.HardPoint_CurrentFill"), new GUIContent("Current Fill"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.HardPoint_CapturedFill"), new GUIContent("Captured Fill"));			
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.HardPoint_ScreenTargetTexture"), new GUIContent("Screen Target Texture"));

											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.HardPointTimer"), new GUIContent("Timer"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("Not Teams UI", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.NotTeamsMatchUIMain"), new GUIContent("Main Object"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.CurrentPlaceText"), new GUIContent("Player Place"));
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("Player Score", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.PlayerStats"), new GUIContent("Score"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.PlayerStatsBackground"), new GUIContent("Background"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.PlayerStatsHighlight"), new GUIContent("Background Highlight"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("1st Place Score", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.FirstPlaceStats"), new GUIContent("Score"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.FirstPlaceStatsBackground"), new GUIContent("Background"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.EndVertical();

											break;

										case 1:
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("Teams UI", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.TeamsSurvivalMain"), new GUIContent("Main Object"));
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("1st Team", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.RedTeamLogoPlaceholder"), new GUIContent("Logo Placeholder"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.RedTeamPlayersList"), new GUIContent("Players list"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("2nd Team", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.BlueTeamLogoPlaceholder"), new GUIContent("Logo Placeholder"));
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.BlueTeamPlayersList"), new GUIContent("Players list"));
											EditorGUILayout.EndVertical();
											EditorGUILayout.EndVertical();
											
											EditorGUILayout.Space();
											EditorGUILayout.LabelField("Not Teams UI", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("helpbox");
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.NotTeamsSurvivalMain"), new GUIContent("Main Object"));
											EditorGUILayout.Space();
											EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.MatchStats.PlayersList"), new GUIContent("Players List"));
											EditorGUILayout.EndVertical();

											break;
									}

									break;

								case 5:
									EditorGUILayout.Space();
									
									EditorGUILayout.LabelField("Reborn Timer", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.HelpBox("If the [Use Respawns]parameter is active, this timer will be displayed after the player's death.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.TimerAfterDeath.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.TimerAfterDeath.LaunchButton"), new GUIContent("Launch Match Button"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.TimerAfterDeath.RestartTimer"), new GUIContent("Timer"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.EndVertical();
									
									EditorGUILayout.Space();
									
									EditorGUILayout.LabelField("Spectate Screen", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.HelpBox("If the [Use Respawns] parameter is not active, the player won't be respawned. " + "\n" +
									                         "But you can see other players after a character's death; these screen is needed for that.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.SpectateMenu.ChangeCameraButton"), new GUIContent("Change Camera"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.SpectateMenu.MatchStatsButton"), new GUIContent("Show Stats"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.SpectateMenu.BackButton"), new GUIContent("Back"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.SpectateMenu.ExitButton"), new GUIContent("Exit"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.SpectateMenu.PlayerStats"), new GUIContent("Status Text"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.EndVertical();
									break;

								case 3:
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Pre-match Game", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.HelpBox("This menu is displayed in a pre-match game.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PreMatchMenu.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.PreMatchMenu.Status"), new GUIContent("Status Text"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.LabelField("Start Match Timer", EditorStyles.boldLabel);
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.HelpBox("This is the timer before match.", MessageType.Info);
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.TimerBeforeMatch.MainObject"), new GUIContent("Main Object"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.TimerBeforeMatch.StartMatchTimer"), new GUIContent("Timer"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.TimerBeforeMatch.Background"), new GUIContent("Background"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.EndVertical();
									break;
							}

							break;
						
						case 2:
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("helpbox");
							EditorGUILayout.HelpBox("These prefabs will be instantiated in different places during the game." + "\n" +
							                        "Each prefab has the [UIPlaceholder] component and you can adjust it.", MessageType.Info);
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.gameModePlaceholder"), new GUIContent("Game Mode"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.mapPlaceholder"), new GUIContent("Map"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.weaponPlaceholder"), new GUIContent("Weapon"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.avatarPlaceholder"), new GUIContent("Avatar"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.matchStatsPlaceholder"), new GUIContent("K/D Stats"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameRoom.playerInfoPlaceholder"), new GUIContent("Player Info"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("MultiplayerGameLobby.roomInfoPlaceholder"), new GUIContent("Room Info"));
							EditorGUILayout.EndVertical();
							
							break;
					}


					break;


				case 2:
					
					EditorGUILayout.Space();
					script.characterUiInspectorTab = GUILayout.Toolbar(script.characterUiInspectorTab, new[] {"Game UI", "Inventory"});

					switch (script.characterUiInspectorTab)
					{
						case 0:
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("helpbox");
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.MainObject"), new GUIContent("Main Object"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.WeaponAmmo"), new GUIContent("Ammo"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.WeaponAmmoImagePlaceholder"), new GUIContent("Image Placeholder"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Health"), new GUIContent("Health"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.HealthBar"), new GUIContent("Health bar"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.PickupHUD"), new GUIContent("Pick-up HUD"));
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							//
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.PickupEggHUD"), new GUIContent("Pick-up-egg HUD"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.ShowEggNum"), new GUIContent("ShowEggNum"));
							EditorGUILayout.Space();
							//
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("Blood Splatter", EditorStyles.boldLabel);
							EditorGUILayout.BeginVertical("helpbox");
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.bloodSplatter"), new GUIContent("Splatter Screen"));
							EditorGUILayout.Space();
							bloodHitMarks.DoLayoutList();
							EditorGUILayout.EndVertical();
							break;

						case 1:
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("helpbox");
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.MainObject"), new GUIContent("Main Object"));
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							
							EditorGUILayout.LabelField("Weapons Slots", EditorStyles.boldLabel);
							EditorGUILayout.BeginVertical("helpbox");
							
							script.curWeaponSlot = EditorGUILayout.Popup("Slot №", script.curWeaponSlot, new[] {"1", "2", "3", "4", "5", "6", "7", "8"});
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("helpbox");
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.WeaponsButtons").GetArrayElementAtIndex(script.curWeaponSlot), new GUIContent("Main Button"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.WeaponImagePlaceholder").GetArrayElementAtIndex(script.curWeaponSlot), new GUIContent("Image Placeholder"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.WeaponAmmoText").GetArrayElementAtIndex(script.curWeaponSlot), new GUIContent("Ammo Text"));
							EditorGUILayout.EndVertical();
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.BeginVertical("helpbox");
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.UpWeaponButton"), new GUIContent("<- Weapon"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.DownWeaponButton"), new GUIContent("-> Weapon"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.WeaponsCount"), new GUIContent("Weapons Count"));
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							
							EditorGUILayout.LabelField("Health Slot", EditorStyles.boldLabel);
							EditorGUILayout.BeginVertical("helpbox");
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.UpHealthButton"), new GUIContent("<- Health"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.DownHealthButton"), new GUIContent("-> Health"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.HealthButton"), new GUIContent("Main Button"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.HealthImage"), new GUIContent("Image Placeholder"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.HealthKitsCount"), new GUIContent("Kits Count"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.CurrentHealthValue"), new GUIContent("Kit Value"));
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							
							EditorGUILayout.LabelField("Ammo Slot", EditorStyles.boldLabel);
							EditorGUILayout.BeginVertical("helpbox");
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.UpAmmoButton"), new GUIContent("<- Ammo"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.DownAmmoButton"), new GUIContent("-> Ammo"));
							EditorGUILayout.Space();
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.AmmoButton"), new GUIContent("Main Button"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.AmmoImage"), new GUIContent("Image Placeholder"));
							EditorGUILayout.Space();
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.AmmoKitsCount"), new GUIContent("Kits Count"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterUI.Inventory.CurrentAmmoValue"), new GUIContent("Kit Value"));
							
							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
							
							break;

					}



					break;
			}

//			DrawDefaultInspector();
			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
				EditorUtility.SetDirty(script.gameObject);
		}
	}
}
