using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(Lobby))]
    public class LobbyEditor : Editor
    {
        public Lobby script;

        private ReorderableList charactersList;
        private ReorderableList levelsList;

        private ReorderableList allWeaponsList;

//       private ReorderableList[] specificWeaponsList = new ReorderableList[0];
        private ReorderableList gameModesList;
        private ReorderableList avatarsList;

        private GUIStyle style;

        private int currentMode;

        public void Awake()
        {
            script = (Lobby) target;
        }

        public void OnEnable()
        {
            UpdateWeaponsList(script.currentMode);

            charactersList = new ReorderableList(serializedObject, serializedObject.FindProperty("Characters"), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Character"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.Characters.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.Characters.Remove(script.Characters[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.Characters[index] = (Controller) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.Characters[index], typeof(Controller), false);
                }
            };

            levelsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Maps"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Scene");

                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Image");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.Maps.Add(new PUNHelper.PhotonLevel());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.Maps.Remove(script.Maps[items.index]);
                    }
                },


                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
//                   script.Levels[index].LevelName = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), script.Levels[index].LevelName);

                    script.Maps[index].Scene = (SceneAsset) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                        script.Maps[index].Scene, typeof(SceneAsset), false);

                    script.Maps[index].Image = (Texture) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight),
                        script.Maps[index].Image, typeof(Texture), true);
                }
            };

            gameModesList = new ReorderableList(serializedObject, serializedObject.FindProperty("GameModes"), true, true, true, true)
            {
                index = script.currentMode,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Name");

                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Image");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.GameModes.Add(new PUNHelper.GameMode());
//                        script.currentMode = script.GameModes.Count - 1;
//                        items.index = script.currentMode;
//                        UpdateWeaponsList(script.currentMode);
                    }
                },

                onSelectCallback = items =>
                {
                    script.currentMode = items.index;
                    UpdateWeaponsList(script.currentMode);
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.GameModes.Remove(script.GameModes[items.index]);
                        var newModeIndex = script.currentMode - 1;
                        if (newModeIndex < 0)
                            newModeIndex = 0;
                        script.currentMode = newModeIndex;
                        items.index = newModeIndex;
                        UpdateWeaponsList(script.currentMode);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.GameModes[index].Name = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), script.GameModes[index].Name);

                    script.GameModes[index].Image =
                        (Texture) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), script.GameModes[index].Image, typeof(Texture), false);
                }
            };

            allWeaponsList = new ReorderableList(serializedObject, serializedObject.FindProperty("AllWeapons"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight), "Weapon Prefab");

                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width / 3 - 10, EditorGUIUtility.singleLineHeight), "Slot in Inventory");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.AllWeapons.Add(new PUNHelper.WeaponSlot());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.AllWeapons.Remove(script.AllWeapons[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.AllWeapons[index].weapon = (WeaponController) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
                        script.AllWeapons[index].weapon, typeof(WeaponController), false);

                    script.AllWeapons[index].slot = EditorGUI.Popup(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width / 3 - 10, EditorGUIUtility.singleLineHeight), script.AllWeapons[index].slot, new[] {"1", "2", "3", "4", "5", "6", "7", "8"});
                }
            };

            avatarsList = new ReorderableList(serializedObject, serializedObject.FindProperty("DefaultAvatars"), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Avatar"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.DefaultAvatars.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.DefaultAvatars.Remove(script.DefaultAvatars[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.DefaultAvatars[index] = (Texture) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.DefaultAvatars[index], typeof(Texture), false);
                }
            };

            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        public void Update()
        {
            if (!Application.isPlaying)
            {
                foreach (var level in script.Maps)
                {
                    if (!level.Scene) continue;

                    if (!string.Equals(level.Scene.name, level.Name, StringComparison.Ordinal))
                        level.Name = level.Scene.name;
                }

                if (script && !script.UiManager)
                {
                    script.UiManager = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI Manager.prefab", typeof(UIManager)) as UIManager;

                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                if (script && !script.characterAnimatorController)
                {
                    script.characterAnimatorController = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/_Animator Controllers/Controller for Lobby.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                if (script && !script.projectSettings)
                {
                    script.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;

                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            style = new GUIStyle(EditorStyles.helpBox) {richText = true, fontSize = 10};

            EditorGUILayout.Space();
#if !PHOTON_UNITY_NETWORKING
            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.HelpBox("To use the multiplayer mode, import PUN2 from Asset Store" + "\n" + 
                                    "(If Photon is already in the project and you still see this message, restart Unity)", MessageType.Info);
            if (GUILayout.Button("Open Asset Store"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/network/pun-2-free-119922");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(true);
#endif


            script.upInspectorTab = GUILayout.Toolbar(script.upInspectorTab, new[] {"Game Modes", "Characters", "Weapons"});

            switch (script.upInspectorTab)
            {
                case 0:
                    script.currentInspectorTab = 0;
                    script.downInspectorTab = 3;
                    break;


                case 1:
                    script.currentInspectorTab = 1;
                    script.downInspectorTab = 3;
                    break;

                case 2:
                    script.currentInspectorTab = 2;
                    script.downInspectorTab = 3;
                    break;
            }

            script.downInspectorTab = GUILayout.Toolbar(script.downInspectorTab, new[] {"Maps", "Score", "Other Parameters"});

            switch (script.downInspectorTab)
            {
                case 0:
                    script.currentInspectorTab = 3;
                    script.upInspectorTab = 3;
                    break;

                case 1:
                    script.currentInspectorTab = 4;
                    script.upInspectorTab = 3;
                    break;

                case 2:
                    script.currentInspectorTab = 5;
                    script.upInspectorTab = 3;
                    break;
            }

            switch (script.currentInspectorTab)
            {
                case 1:
                    EditorGUILayout.Space();
                    charactersList.DoLayoutList();
                    EditorGUILayout.Space();
                    break;

                case 3:
                    EditorGUILayout.Space();
                    levelsList.DoLayoutList();
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Make sure that all scenes are in the <b><color=blue>Build Settings</color></b> (including the Lobby scene).", style);

                    if (GUILayout.Button("Add all scenes to the Build Settings"))
                    {
                        var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

                        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(SceneManager.GetActiveScene().path, true));

                        foreach (var sceneAsset in script.Maps)
                        {
                            if (!sceneAsset.Scene) continue;

                            var scenePath = AssetDatabase.GetAssetPath(sceneAsset.Scene);

                            if (!string.IsNullOrEmpty(scenePath))
                                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                        }

                        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
                        EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }

                    break;

                case 2:

                    EditorGUILayout.Space();
                    allWeaponsList.DoLayoutList();
                    break;

                case 0:
                    EditorGUILayout.Space();

                    gameModesList.DoLayoutList();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    var curModeSerialized = serializedObject.FindProperty("GameModes").GetArrayElementAtIndex(script.currentMode);
                    var gameMode = script.GameModes[script.currentMode];

                    GUILayout.BeginVertical("Mode: " + script.GameModes[script.currentMode].Name, "window");

                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("Teams"), new GUIContent("Use Teams"));
                    if (gameMode.Teams)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("1st Team", EditorStyles.boldLabel);
                        EditorGUILayout.BeginVertical("helpbox");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RedTeamName"), new GUIContent("Name"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RedTeamLogo"), new GUIContent("Logo"));

                        if (script.RedTeamLogo)
                        {
                            if (Resources.Load(script.RedTeamLogo.name) == null)
                                EditorGUILayout.LabelField("<b><color=red>The image must be in the</color> <color=blue>[Resources]</color> <color=red>folder.</color></b>", style);
                        }
                        else EditorGUILayout.LabelField("<b>Please note: the image must be in the <color=blue>[Resources]</color> folder.</b>", style);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("2nd Team", EditorStyles.boldLabel);
                        EditorGUILayout.BeginVertical("helpbox");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("BlueTeamName"), new GUIContent("Name"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("BlueTeamLogo"), new GUIContent("Logo"));
                        EditorGUILayout.EndVertical();

                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");

                    if (gameMode.Teams)
                    {
                        EditorGUILayout.LabelField("<b><color=green>Everyone</color></b> - player can kill everyone, including teammates." + "\n" +
                                                   "<b><color=green>No One</color></b> - player can't kill anyone." + "\n" +
                                                   "<b><color=green>Only Opponents</color></b> - player can kill only enemies.", style);

                        gameMode.KillMethod = EditorGUILayout.Popup("Kill Mode", gameMode.KillMethod, PUNHelper.CanKillOther.ToArray());

                        if (gameMode.KillMethod > 2)
                            gameMode.KillMethod = 2;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("<b><color=green>Everyone</color></b> - player can kill everyone." + "\n" +
                                                   "<b><color=green>No One</color></b> - player can't kill anyone.", style);

                        gameMode.KillMethod = EditorGUILayout.Popup("Kill Mode", gameMode.KillMethod, new[] {PUNHelper.CanKillOther[0], PUNHelper.CanKillOther[1]});

                        if (gameMode.KillMethod > 1)
                            gameMode.KillMethod = 1;
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");

                    EditorGUILayout.LabelField("If this parameter <b>is not active</b>, the characters won't be respawned, and <b><color=green>[Match Target]</color></b> is to <b>kill everyone.</b>" + "\n\n" +
                                               "If <b>active</b>, characters will be respawned after death, and you can set <b>any</b> <b><color=green>[Match Target]</color></b>.", style);

                    EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("CanRespawn"), new GUIContent("Use Respawns"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("helpbox");

                    if (!gameMode.Teams)
                    {
                        EditorGUILayout.LabelField("When the <color=green><b>[Use Teams]</b></color> parameter is not active, the characters will be spawned <b>only at random points</b>.", style);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("<color=green><b>Random</b></color> - at the <b>beginning of the match</b>, characters will be spawns <b>at their bases</b>. " +
                                                   "<b>During the game</b>, they will be spawned <b>in random points</b>." + "\n\n" +
                                                   "<color=green><b>On Bases</b></color> - the characters will be spawned <b>only at their bases</b>.", style);

                    }

                    EditorGUILayout.Space();
                    EditorGUI.BeginDisabledGroup(!gameMode.Teams);
                    gameMode.spawnMethod = EditorGUILayout.Popup("Spawn Method", gameMode.spawnMethod, PUNHelper.SpawnMethods.ToArray());
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");

                    if (gameMode.CanRespawn)
                    {
                        if (gameMode.Teams)
                        {
                            gameMode.matchTarget = EditorGUILayout.Popup("Match Target", gameMode.matchTarget, PUNHelper.MatchTargets.ToArray());

                            //if (gameMode.matchTarget > 4)
                                //gameMode.matchTarget = 4;
                        }
                        else
                        {
                            gameMode.matchTarget = EditorGUILayout.Popup("Match Target", gameMode.matchTarget, new[] {PUNHelper.MatchTargets[0], PUNHelper.MatchTargets[1], PUNHelper.MatchTargets[2]});

                            if (gameMode.matchTarget > 2)
                                gameMode.matchTarget = 2;
                        }

                        switch (gameMode.matchTarget)
                        {
                            case 1:
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("targetKills"), new GUIContent("Kills Limit"));
                                break;

                            case 2:
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("targetPoints"), new GUIContent("Score Limit"));
                                break;

                            case 3:
                                EditorGUILayout.Space();
                                gameMode.pointsCount = EditorGUILayout.Popup("Points Count", gameMode.pointsCount, new[] {"Three", "One"});
                                EditorGUILayout.Space();

                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("captureImmediately"), new GUIContent("Capture Immediately"));
                                EditorGUILayout.Space();
                                if (gameMode.pointsCount == 1)
                                {
                                    EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("hardPointTimeout"), new GUIContent("Timeout (sec)"));
                                }

                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("targetPoints"), new GUIContent("Score Limit"));
                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("captureScore"), new GUIContent("Score for capturing point"));
                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("capturePointTimeout"), new GUIContent("Earn score every (sec)"));
                                break;

                            case 4:
                                EditorGUILayout.Space();
                                EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("targetEgg"), new GUIContent("Egg Limit"));
                                break;
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("When the <color=green><b>[Use Respawns]</b></color> parameter is not active, the character has <b>one life</b> and the player’s target is to kill all opponents." + "\n\n" +
                                                   "Also, can be used with teams - the team that kills all opponents wins.", style);
//                       EditorGUILayout.Space();
//                       EditorGUILayout.HelpBox("When the [Use Respawns] parameter is not active, the character has ONE LIFE and the player’s target is to kill all opponents." + "\n\n" +
//                                               "Also, can be used with teams - the team that kills all opponents wins.", MessageType.Info);
                        EditorGUILayout.Space();

                        EditorGUI.BeginDisabledGroup(true);
                        gameMode.matchTarget = EditorGUILayout.Popup("Match Target", gameMode.matchTarget, PUNHelper.MatchTargets.ToArray());
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");

                    EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("TimeLimit"), new GUIContent("Time Limit"));
                    if (gameMode.TimeLimit)
                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("targetTime"), new GUIContent("Minutes"));

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("helpbox");
                    if (gameMode.Teams)
                    {
                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("minPlayerCount"), new GUIContent("Min Players (in each team)"));
                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("maxPlayersCount"), new GUIContent("Max Players (in each team)"));
                    }
                    else
                    {

                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("minPlayerCount"), new GUIContent("Min Players"));
                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("maxPlayersCount"), new GUIContent("Max Players"));
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("helpbox");

                    if (gameMode.Teams)
                    {
                        if (gameMode.matchTarget == 0 && gameMode.CanRespawn)
                            EditorGUILayout.LabelField("This parameter cannot be used when the <b><color=green>[Match Target]</color></b> is <b>Without Purpose</b>.", style);
                        EditorGUI.BeginDisabledGroup(gameMode.matchTarget == 0 && gameMode.CanRespawn);

                        if (gameMode.matchTarget != 0 || gameMode.matchTarget == 0 && !gameMode.CanRespawn)
                            EditorGUILayout.LabelField("The team that won more rounds will win.", style);

                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("Rounds"), new GUIContent("Rounds"));
                        EditorGUI.EndDisabledGroup();

                        if (gameMode.matchTarget == 0 && gameMode.CanRespawn)
                            gameMode.Rounds = 1;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("This parameter cannot be used without teams.", style);
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("Rounds"), new GUIContent("Rounds"));
                        EditorGUI.EndDisabledGroup();

                        gameMode.Rounds = 1;
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("WeaponsToUse"), new GUIContent("Weapons to Use"));

                    if (gameMode.WeaponsToUse == PUNHelper.WeaponsToUse.Specific)
                    {
                        EditorGUILayout.Space();
                        script.GameModes[script.currentMode].weaponsList.DoLayoutList();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("oneShotOneKill"), new GUIContent("One Shot - One Kill"));

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
//                   
//                   EditorGUILayout.BeginVertical("helpbox");
//                   EditorGUILayout.PropertyField(curModeSerialized.FindPropertyRelative("Enemies"), new GUIContent("Add Enemies"));
//                   EditorGUILayout.EndVertical();
//                   EditorGUILayout.Space();


                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.LabelField("Description");
                    gameMode.Description = EditorGUILayout.TextArea(gameMode.Description);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();


                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    break;

                case 4:

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("normKill"), new GUIContent("Norm Kill"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("headshot"), new GUIContent("Headshot"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("explosionKill"), new GUIContent("Explosion Kill"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fireKill"), new GUIContent("Fire Kill"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeKill"), new GUIContent("Melee Kill"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("killAssist"), new GUIContent("Kill Assistant"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("capturePoint"), new GUIContent("Capture Point"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("capturePointAssist"), new GUIContent("Capture Point Assistant"));

                    EditorGUILayout.EndVertical();

                    break;

                case 5:

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.LabelField("This server is needed to check the internet connection." + "\n" +
                                               "It should be like 'https://[name].[domain]'", style);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("checkConnectionServer"), new GUIContent("Server"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Main Camera", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultCamera"), new GUIContent("Camera"));
                    EditorGUILayout.Space();

                    if (script.DefaultCamera)
                    {
                        EditorGUILayout.LabelField("Positions and Rotations", EditorStyles.boldLabel);
                        EditorGUILayout.BeginVertical("helpbox");
                        script.currentCameraMode = GUILayout.Toolbar(script.currentCameraMode, new[] {"Main Menu", "Loadout Menu", "Characters Menu"});

                        if (script.lastCameraMode != script.currentCameraMode)
                        {
                            switch (script.currentCameraMode)
                            {
                                case 0:
                                    script.DefaultCamera.transform.position = script.MainMenuPositions.position;
                                    script.DefaultCamera.transform.rotation = script.MainMenuPositions.rotation;
                                    break;

                                case 1:
                                    script.DefaultCamera.transform.position = script.LoadoutPositions.position;
                                    script.DefaultCamera.transform.rotation = script.LoadoutPositions.rotation;
                                    break;

                                case 2:
                                    script.DefaultCamera.transform.position = script.CharacterPositions.position;
                                    script.DefaultCamera.transform.rotation = script.CharacterPositions.rotation;
                                    break;
                            }

                            script.lastCameraMode = script.currentCameraMode;
                        }

                        EditorGUILayout.Space();


                        if (GUILayout.Button("Save Position & Rotation"))
                        {
                            switch (script.currentCameraMode)
                            {
                                case 0:
                                    script.MainMenuPositions.position = script.DefaultCamera.transform.position;
                                    script.MainMenuPositions.rotation = script.DefaultCamera.transform.rotation;
                                    break;
                                case 1:
                                    script.LoadoutPositions.position = script.DefaultCamera.transform.position;
                                    script.LoadoutPositions.rotation = script.DefaultCamera.transform.rotation;
                                    break;
                                case 2:
                                    script.CharacterPositions.position = script.DefaultCamera.transform.position;
                                    script.CharacterPositions.rotation = script.DefaultCamera.transform.rotation;
                                    break;
                            }
                        }
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Spawn Points for preview", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponSpawnPoint"), new GUIContent("Weapons Spawn Point"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CharacterSpawnPoint"), new GUIContent("Character Spawn Point"));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Default Avatars", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    avatarsList.DoLayoutList();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("UI Parameters", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.LabelField("Weapons", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showWeaponName"), new GUIContent("Show Name"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showWeaponDamage"), new GUIContent("Show Damage"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showWeaponAmmo"), new GUIContent("Show Ammo"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showWeaponScatter"), new GUIContent("Show Scatter"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showWeaponRateOfAttack"), new GUIContent("Show Rate of Attack"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showWeaponWeight"), new GUIContent("Show Weight"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Game Modes", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showModeScoreLimit"), new GUIContent("Show Match Target"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showModePlayers"), new GUIContent("Show Players Count"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showModeTimeLimit"), new GUIContent("Show Time Limit"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showModeRoundsCount"), new GUIContent("Show Rounds Count"));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("showModeDescription"), new GUIContent("Show Mode Description"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                    break;
            }

//           DrawDefaultInspector();

#if !PHOTON_UNITY_NETWORKING
            EditorGUI.EndDisabledGroup();
#endif

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script.gameObject);

                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        void UpdateWeaponsList(int _index)
        {
            script.GameModes[_index].weaponsList = new ReorderableList(serializedObject, serializedObject.FindProperty("GameModes").GetArrayElementAtIndex(script.currentMode).FindPropertyRelative("WeaponsForThisMode"), true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight), "Weapon Prefab");

                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width / 3 - 10, EditorGUIUtility.singleLineHeight), "Slot in Inventory");
                },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.GameModes[_index].WeaponsForThisMode.Add(new PUNHelper.WeaponSlot());
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.GameModes[_index].WeaponsForThisMode.Remove(script.GameModes[script.currentMode].WeaponsForThisMode[items.index]);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.GameModes[_index].WeaponsForThisMode[index].weapon = (WeaponController) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
                        script.GameModes[_index].WeaponsForThisMode[index].weapon, typeof(WeaponController), false);

                    script.GameModes[_index].WeaponsForThisMode[index].slot = EditorGUI.Popup(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width / 3 - 10, EditorGUIUtility.singleLineHeight), script.GameModes[_index].WeaponsForThisMode[index].slot, new[] {"1", "2", "3", "4", "5", "6", "7", "8"});
                }
            };
        }
    }
}


