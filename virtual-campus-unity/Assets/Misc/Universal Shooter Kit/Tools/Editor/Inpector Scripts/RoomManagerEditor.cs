using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(RoomManager))]
    public class RoomManagerEditor : Editor
    {
        public RoomManager script;

//        private ReorderableList enemiesList;
        private ReorderableList spectateCamerasList;
        private ReorderableList spawnList;
        private ReorderableList redSpawnList;
        private ReorderableList blueSpawnList;
        private ReorderableList hardPointsList;
        

        public void Awake()
        {
            script = (RoomManager) target;
        }

        public void OnEnable()
        {
//            enemiesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Enemies"), false, true, true, true)
//            {
//                drawHeaderCallback = rect =>
//                {
//                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Enemy");
//                },
//
//                onAddCallback = items =>
//                {
//                    if (!Application.isPlaying)
//                    {
//                        script.Enemies.Add(null);
//                    }
//                },
//
//                onRemoveCallback = items =>
//                {
//                    if (!Application.isPlaying)
//                    {
//                        script.Enemies.Remove(script.Enemies[items.index]);
//                    }
//                },
//
//                drawElementCallback = (rect, index, isActive, isFocused) =>
//                {
//                    script.Enemies[index] = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
//                        script.Enemies[index], typeof(GameObject), false);
//                }
//            };

            spawnList = new ReorderableList(serializedObject, serializedObject.FindProperty("PlayersSpawnAreas"), false, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "For all players"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
//                        var area = new GameObject("Spawn Zone");
//                        var component = area.AddComponent<SpawnZone>();
                        script.PlayersSpawnAreas.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
//                        if (script.PlayersSpawnAreas[items.index])
//                            DestroyImmediate(script.PlayersSpawnAreas[items.index].gameObject);
                        script.PlayersSpawnAreas.RemoveAt(items.index);
                        
                        
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.PlayersSpawnAreas[index] = (SpawnZone) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.PlayersSpawnAreas[index], typeof(SpawnZone), true);
                }
            };
            
            
            spectateCamerasList = new ReorderableList(serializedObject, serializedObject.FindProperty("SpectateCameras"), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Cameras"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.SpectateCameras.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.SpectateCameras.RemoveAt(items.index);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.SpectateCameras[index] = (Camera) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.SpectateCameras[index], typeof(Camera), true);
                }
            };
            
            hardPointsList = new ReorderableList(serializedObject, serializedObject.FindProperty("HardPoints"), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Hard Points"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.HardPoints.Add(null);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        script.HardPoints.RemoveAt(items.index);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.HardPoints[index] = (CapturePoint) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.HardPoints[index], typeof(CapturePoint), true);
                }
            };

            
            blueSpawnList = new ReorderableList(serializedObject, serializedObject.FindProperty("BlueTeamSpawnAreas"), false, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "For the 2nd Team"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        var area = new GameObject("Spawn Zone");
                        var component = area.AddComponent<SpawnZone>();
                        script.BlueTeamSpawnAreas.Add(component);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        if (script.BlueTeamSpawnAreas[items.index])
                            DestroyImmediate(script.BlueTeamSpawnAreas[items.index].gameObject);
                        script.BlueTeamSpawnAreas.RemoveAt(items.index);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.BlueTeamSpawnAreas[index] = (SpawnZone) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.BlueTeamSpawnAreas[index], typeof(SpawnZone), true);
                }
            };
            
            redSpawnList = new ReorderableList(serializedObject, serializedObject.FindProperty("RedTeamSpawnAreas"), false, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "For the 1st Team"); },

                onAddCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        var area = new GameObject("Spawn Zone");
                        var component = area.AddComponent<SpawnZone>();
                        script.RedTeamSpawnAreas.Add(component);
                    }
                },

                onRemoveCallback = items =>
                {
                    if (!Application.isPlaying)
                    {
                        if (script.RedTeamSpawnAreas[items.index])
                            DestroyImmediate(script.RedTeamSpawnAreas[items.index].gameObject);
                        script.RedTeamSpawnAreas.RemoveAt(items.index);
                    }
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.RedTeamSpawnAreas[index] = (SpawnZone) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.RedTeamSpawnAreas[index], typeof(SpawnZone), true);
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
            if (!Application.isPlaying)
            {
                if (!script.UiManager)
                {
                    script.UiManager = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI Manager.prefab", typeof(UIManager)) as UIManager;
                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                
                if (!script.projectSettings)
                {
                    script.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
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
            
            script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Spawn Zones", "Capture Points", "Cameras"});

            EditorGUILayout.Space();
            switch (script.inspectorTab)
            {
                case 0:
                    EditorGUILayout.LabelField("Spawn Zones", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("Add the Spawn Zones if you are going to use this scene with game modes in which the [Use Teams] parameter is not active." + "\n\n" +
                                             "During the game all players will be respawned at a random point.", MessageType.Info);
                    
                    EditorGUILayout.Space();
                    spawnList.DoLayoutList();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Team Bases", EditorStyles.boldLabel);

                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("Set the bases zones if you are going to use this scene with game modes in which the [Use Teams] parameter is active." + "\n\n" +
                                               "Players will be respawned on their bases.", MessageType.Info);
                    
                    EditorGUILayout.Space();
                    redSpawnList.DoLayoutList();
                    EditorGUILayout.Space();
                    blueSpawnList.DoLayoutList();
                    EditorGUILayout.EndVertical();
            
                    EditorGUILayout.Space();
                    break;
                
                case 1:
                    EditorGUILayout.LabelField("Domination Points", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("Fill these points if you are going to use this scene with the game modes in which [Match Target] = Domination and [Points Count] = 3.", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("A_Point"), new GUIContent("A Point"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("B_Point"), new GUIContent("B Point"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("C_Point"), new GUIContent("C Point"));
                    EditorGUILayout.EndVertical();
            
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Hard Points", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("Fill these points if you are going to use this scene with the game modes in which [Match Target] = Domination and [Points Count] = 1." + "\n\n" +
                        "During the game, these points will switch.", MessageType.Info);
                    EditorGUILayout.Space();
                    hardPointsList.DoLayoutList();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    break;
                
                case 2:
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultCamera"), new GUIContent("Default Camera"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Spectate Cameras", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("helpbox");
                    EditorGUILayout.HelpBox("Fill these cameras if you are going to use this scene with the game modes in which [Use Respawn] parameter is not active." + "\n\n" +
                        "After death, players will be able to watch the match.", MessageType.Info);
                    EditorGUILayout.Space();

                    spectateCamerasList.DoLayoutList();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    break;
            }


            serializedObject.ApplyModifiedProperties();
            
#if !PHOTON_UNITY_NETWORKING
            EditorGUI.EndDisabledGroup();
#endif
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
                
                if(!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }

}

