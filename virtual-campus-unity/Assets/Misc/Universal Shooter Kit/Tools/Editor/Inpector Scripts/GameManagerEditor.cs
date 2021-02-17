using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(GameManager))]
	public class GameManagerEditor : Editor {
		
		private GameManager script;

		private ReorderableList enemiesList;
		private ReorderableList spawnZonesList;
		private ReorderableList charactersList;
		private ReorderableList graphicsButtons;
		
		private GUIStyle style;
		
		private void Awake()
		{
			script = (GameManager) target;
		}

		private void OnEnable()
		{
			enemiesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Enemies"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), "Prefabs");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 5 + 2, rect.y, rect.width / 5, 
						EditorGUIUtility.singleLineHeight), "Behaviours");
					
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), "Spawn Methods");
					
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + 30, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), "∞");
					
					EditorGUI.LabelField(new Rect(rect.x + 2 * (rect.width / 5) + rect.width / 3.2f + 30, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight),"Count");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + 20 + rect.width / 12 + rect.width / 12 + 5, rect.y, 
							rect.width - (rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + rect.width / 12 + rect.width / 12 + 5),
							EditorGUIUtility.singleLineHeight), "Time");
				},

				onAddCallback = items => { script.Enemies.Add(null); },

				onRemoveCallback = items => { script.Enemies.Remove(script.Enemies[items.index]); },


				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Enemies[index].enemyPrefab = (EnemyController) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
						script.Enemies[index].enemyPrefab, typeof(EnemyController), false);
					
					script.Enemies[index].movementBehavior = (MovementBehavior) EditorGUI.ObjectField(
						new Rect(rect.x + rect.width / 5 + 5, rect.y, rect.width / 5, EditorGUIUtility.singleLineHeight),
						script.Enemies[index].movementBehavior, typeof(MovementBehavior), true);


					if (script.Enemies[index].currentSpawnMethodIndex == 0)
					{

						script.Enemies[index].currentSpawnMethodIndex = EditorGUI.Popup(
							new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5, rect.y, rect.width / 4 + 15,
								EditorGUIUtility.singleLineHeight), script.Enemies[index].currentSpawnMethodIndex,
							new[] {"Random", "Specific Point"});
					}
					else
					{
						script.Enemies[index].currentSpawnMethodIndex = EditorGUI.Popup(
							new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5, rect.y, rect.width / 8 + 10,
								EditorGUIUtility.singleLineHeight), script.Enemies[index].currentSpawnMethodIndex, new[] {"Random", "Specific Area"});

						script.Enemies[index].spawnZone = (SpawnZone) EditorGUI.ObjectField(
							new Rect(rect.x + rect.width / 5 + 5 + rect.width / 5 + 5 + rect.width / 8 + 10 + 3, rect.y, rect.width / 8 + 5,
								EditorGUIUtility.singleLineHeight),
							script.Enemies[index].spawnZone, typeof(SpawnZone), true);
					}
					
					script.Enemies[index].spawnConstantly = EditorGUI.Toggle(
						new Rect(rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + 30, rect.y, rect.width / 30,
							EditorGUIUtility.singleLineHeight), script.Enemies[index].spawnConstantly);
					
					script.Enemies[index].count = EditorGUI.IntField(
						new Rect(rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + 30 + rect.width / 30 + 10, rect.y, rect.width / 12,
							EditorGUIUtility.singleLineHeight), script.Enemies[index].count);

					script.Enemies[index].spawnTimeout = EditorGUI.FloatField(
						new Rect(rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + 20 + rect.width / 12 + rect.width / 12 + 10, rect.y,
							rect.width - (rect.x + rect.width / 5 + rect.width / 5 + 5 + rect.width / 4 + rect.width / 12 + rect.width / 12 + 10)
							, EditorGUIUtility.singleLineHeight), script.Enemies[index].spawnTimeout);
				},
			};

			charactersList = new ReorderableList(serializedObject, serializedObject.FindProperty("Characters"), true, true, true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), "Prefabs");
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), "Spawn Zones");
				},
				
				onAddCallback = items =>
				{
					script.Characters.Add(new Helper.CharacterInGameManager());
				},

				onRemoveCallback = items =>
				{
					script.Characters.Remove(script.Characters[items.index]);
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Characters[index].characterPrefab = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), script.Characters[index].characterPrefab, typeof(GameObject), false);
					script.Characters[index].spawnZone = (SpawnZone) EditorGUI.ObjectField(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight), script.Characters[index].spawnZone, typeof(SpawnZone), true);
				}
			};
			
			spawnZonesList = new ReorderableList(serializedObject, serializedObject.FindProperty("EnemiesSpawnZones"), false, true, true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Spawn Zones");
				},
				
				onAddCallback = items =>
				{
					script.EnemiesSpawnZones.Add(null);
				},

				onRemoveCallback = items =>
				{
					script.EnemiesSpawnZones.Remove(script.EnemiesSpawnZones[items.index]);
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.EnemiesSpawnZones[index] = (SpawnZone) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), script.EnemiesSpawnZones[index], typeof(SpawnZone), true);
				}
			};

			EditorApplication.update += Update;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Update()
		{
			if (!script || !script.gameObject.activeInHierarchy || Application.isPlaying) return;

			if (!script.UIManager)
			{
				script.UIManager = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI Manager.prefab", typeof(UIManager)) as UIManager;
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

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			style = new GUIStyle(EditorStyles.helpBox) {richText = true, fontSize = 10};
			
			EditorGUILayout.Space();
			script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Characters", "Enemies"});

			EditorGUILayout.Space();

			switch (script.inspectorTab)
			{
				case 0:
					
					charactersList.DoLayoutList();
		
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					break;
				case 1:
					
					enemiesList.DoLayoutList();
					EditorGUILayout.Space();
					spawnZonesList.DoLayoutList();
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("<b>Behavior</b> - movement behaviour in the current scene." + "\n\n" +
					                           "<b>Spawn method</b> - " + "\n" +
					                           "    <color=blue>Random</color> - one random point from the Spawn Zones will be chosen."+ "\n" +
					                           "    <color=blue>Specific point</color> - set a spawn point for the enemy as you need." + "\n\n" +
					                           "<b>∞</b>- Spawn enemies constantly." +"\n\n" +
					                           "<b>Count</b> - " + "\n" +
					                           "    If <color=blue>∞</color> is not active, this number means how many enemies will spawn during the game."+ "\n" +
					                           "    If <color=blue>∞</color> is active, this number means the limit of enemies in the scene." + "\n\n" +
					                           "<b>Time</b> - a break between the appearance of enemies (in seconds).", style);
					break;
			}
			
			serializedObject.ApplyModifiedProperties();

//			DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				
				if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
	}
}

