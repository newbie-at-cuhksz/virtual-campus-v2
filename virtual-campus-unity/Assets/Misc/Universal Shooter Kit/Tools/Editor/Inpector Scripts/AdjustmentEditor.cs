using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(Adjustment))]
	public class AdjustmentEditor : Editor
	{
		private Adjustment script;

		private ReorderableList weaponsList;
		private ReorderableList charactersList;
		private ReorderableList copyToList;

		private string curName;

		private bool delete;
		private bool rename;
		private bool renameError;
		private int stateIndex;

		private Helper.RotationAxes axises;

		private void Awake()
		{
			script = (Adjustment) target;
		}

		private void OnEnable()
		{
			/*enemiesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Enemies"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						Application.isPlaying ? "Select an enemy to adjust" : "Add your enemies");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Status");
				},
				
				onAddCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Enemies.Add(null);
						script.EnemiesPrefabs.Add(null);
					}
				},
				
				onRemoveCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Enemies.Remove(script.Enemies[items.index]);
						script.EnemiesPrefabs.Remove((script.EnemiesPrefabs[items.index]));
					}
				},
				
				onSelectCallback = items =>
				{
					if (!script.Enemies[items.index])
						return;
					
					if (Application.isPlaying && script.enemyIndex != items.index)
					{
						script.ActiveEnemy(items.index);
						script.enemyIndex = items.index;
					}
					else if (!Application.isPlaying)
					{
						script.enemyIndex = items.index;
						
						if (!script.EnemiesPrefabs[items.index])
						{
							script.EnemiesPrefabs[items.index] = Instantiate(script.Enemies[items.index].gameObject, Vector3.zero, Quaternion.identity);
							script.EnemiesPrefabs[items.index].GetComponent<AIController>().OriginalScript = script.Enemies[items.index];
						}
					}

					script.CurrentAiController = script.Enemies[script.enemyIndex];
				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					if (Application.isPlaying && index == script.enemyIndex && script.Enemies[index] && script.isPause)
					{
						var options = new GUIStyle {normal = {textColor = Color.green}};
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10,
							EditorGUIUtility.singleLineHeight), "Adjustment", options);
					}

					script.Enemies[index] = (AIController) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 1.5f, 
							EditorGUIUtility.singleLineHeight), script.Enemies[index], typeof(AIController), false);
					
					
					if (!Application.isPlaying)
					{
						var enemy = script.EnemiesPrefabs[index];

						if (enemy)
						{
							if (!isActive && enemy.hideFlags == HideFlags.None)
							{
								enemy.hideFlags = HideFlags.HideInHierarchy;
								enemy.SetActive(false);
								EditorApplication.RepaintHierarchyWindow();
								EditorApplication.DirtyHierarchyWindowSorting();
							}
							else if (isActive && enemy.hideFlags == HideFlags.HideInHierarchy)
							{
								enemy.hideFlags = HideFlags.None;
								enemy.SetActive(true);
							}
						}
					}

					if (!Application.isPlaying && index == script.enemyIndex && script.CurrentAiController && isActive)
					{
						var currentController = script.EnemiesPrefabs[script.enemyIndex].GetComponent<AIController>();
						EditorGUILayout.BeginVertical("box");
						EditorGUILayout.BeginVertical("box");
						currentController.DistanceToSee = EditorGUILayout.Slider("Distance to see", currentController.DistanceToSee, 1, 100);
						script.CurrentAiController.DistanceToSee = currentController.DistanceToSee;
						
						currentController.horizontalAngleToSee = EditorGUILayout.Slider("Horizontal angle to see", currentController.horizontalAngleToSee, 1, 180);
						script.CurrentAiController.horizontalAngleToSee = currentController.horizontalAngleToSee;
						
						currentController.heightToSee = EditorGUILayout.Slider("Height to see", currentController.heightToSee, 1, 180);
						script.CurrentAiController.heightToSee = currentController.heightToSee;
						EditorGUILayout.EndVertical();
						EditorGUILayout.Space();
						EditorGUILayout.BeginVertical("box");
						currentController.HeadOffsetX = EditorGUILayout.Slider("Head offset X", currentController.HeadOffsetX, -90, 90);
						script.CurrentAiController.HeadOffsetX = currentController.HeadOffsetX;
						
						currentController.HeadOffsetY = EditorGUILayout.Slider("Head offset Y", currentController.HeadOffsetY, -90, 90);
						script.CurrentAiController.HeadOffsetY = currentController.HeadOffsetY;
						
						currentController.HeadOffsetZ = EditorGUILayout.Slider("Head offset Z", currentController.HeadOffsetZ, -90, 90);
						script.CurrentAiController.HeadOffsetZ = currentController.HeadOffsetZ;
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndVertical();
					}

				}
			};*/

			copyToList = new ReorderableList(serializedObject, serializedObject.FindProperty("CopyToList"), false, true,
				false, false)
			{
				drawHeaderCallback = rect =>
				{
					if (script.ikInspectorTab == 0)
					{
						EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 3 - 7, EditorGUIUtility.singleLineHeight), "Weapon:");
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3 - 7, EditorGUIUtility.singleLineHeight), "Settings Slot:");
						EditorGUI.LabelField(new Rect(rect.x + 2 * rect.width / 3, rect.y, rect.width / 3 - 7, EditorGUIUtility.singleLineHeight), "IK Mode:");
					}
					else
					{
						EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Weapon:");
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), "Settings Slot:");
					}

				},
				
				drawElementBackgroundCallback = (rect, index, active, focused) => { },
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					if (script.ikInspectorTab == 0)
					{
						script.copyFromWeaponSlot = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 3 - 7, EditorGUIUtility.singleLineHeight), script.copyFromWeaponSlot,
							script.WeaponsNames.ToArray());
						script.copyFromSlot = EditorGUI.Popup(new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3 - 7, EditorGUIUtility.singleLineHeight), script.copyFromSlot,
							script.Weapons[script.copyFromWeaponSlot].enumNames.ToArray());
						script.copyFromIKState = EditorGUI.Popup(new Rect(rect.x + 2 * rect.width / 3, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), script.copyFromIKState,
							script.IKStateNames);
					}
					else
					{
						script.copyFromWeaponSlot = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), script.copyFromWeaponSlot,
							script.WeaponsNames.ToArray());
						script.copyFromSlot = EditorGUI.Popup(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), script.copyFromSlot,
							script.Weapons[script.copyFromWeaponSlot].enumNames.ToArray());
					}
				}
			};
			
			charactersList = new ReorderableList(serializedObject, serializedObject.FindProperty("Characters"), true, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						Application.isPlaying ? "Select a character to adjust" : "Add your characters");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Status");
				},

				onAddCallback = items =>
				{
					if (!Application.isPlaying) script.Characters.Add(null);
				},

				onRemoveCallback = items =>
				{
					if (!Application.isPlaying) script.Characters.Remove(script.Characters[items.index]);
				},

				onSelectCallback = items =>
				{
					if (!script.Characters[items.index])
						return;

					if (Application.isPlaying && script.characterIndex != items.index)
					{
						script.ActiveCharacter(items.index, false);

						script.characterIndex = items.index;
					}
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					
					if (Application.isPlaying && index == script.characterIndex && script.Characters[index] && script.isPause)
					{
						var options = new GUIStyle {normal = {textColor = Color.green}};
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10,
							EditorGUIUtility.singleLineHeight), "Adjustment", options);
					}
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					script.Characters[index] = (Controller) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						script.Characters[index], typeof(Controller), false);
					EditorGUI.EndDisabledGroup();
				}
			};

			weaponsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Weapons"), false, true,
				true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						Application.isPlaying ? "Select a weapon to adjust" : "Add your weapons");

					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Status");
				},

				onAddCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Weapons.Add(null);
						script.WeaponsPrefabs.Add(null);
					}
				},

				onRemoveCallback = items =>
				{
					if (!Application.isPlaying)
					{
						script.Weapons.Remove(script.Weapons[items.index]);
						script.WeaponsPrefabs.Remove(script.WeaponsPrefabs[items.index]);
					}
				},

				onSelectCallback = items =>
				{
					if (!script.Weapons[items.index] || script.weaponIndex == items.index)
						return;

					if (Application.isPlaying)
					{
						script.ResetWeapons();

						var weaponManager = script.Characters[script.characterIndex].gameObject.GetComponent<InventoryManager>();
						WeaponsHelper.SetWeaponController(script.Weapons[items.index].gameObject,
							script.WeaponsPrefabs[items.index], 0, weaponManager,
							script.Characters[script.characterIndex].GetComponent<Controller>(), script.Characters[script.characterIndex].transform);

						weaponManager.hasAnyWeapon = true;
						weaponManager.slots[0].weaponSlotInGame.Add(new CharacterHelper.Weapon {weapon = script.Weapons[items.index].gameObject});
						weaponManager.slots[0].currentWeaponInSlot = 0;
						weaponManager.Switch(0, false, false);

						if (script.CurrentWeaponController)
							Helper.HideAllObjects(script.CurrentWeaponController.IkObjects);

						script.SerializedWeaponController = new SerializedObject(script.Weapons[items.index]);
					}

					script.weaponIndex = items.index;
					script.CurrentWeaponController = script.Weapons[script.weaponIndex];
					script.CheckIKObjects();
					script.oldDebugModeIndex = IKHelper.IkDebugMode.Aim;
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					if (Application.isPlaying && index == script.weaponIndex && script.Weapons[index])
					{
						var options = new GUIStyle {normal = {textColor = Color.green}};
						EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10,
							EditorGUIUtility.singleLineHeight), "Adjustment", options);
					}

					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					script.Weapons[index] = (WeaponController) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
						script.Weapons[index], typeof(WeaponController), false);
					EditorGUI.EndDisabledGroup();
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
			if (!Application.isPlaying)
			{
				if (!script.UIManager)
				{
					script.UIManager = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI Manager.prefab", typeof(UIManager)) as UIManager;
					EditorUtility.SetDirty(script.gameObject);
                 	EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}

//				if (script.upInspectorTab == 1)
//				{
//					if (script.WeaponsPrefabs[script.weaponIndex] && script.WeaponsPrefabs[script.weaponIndex].hideFlags != HideFlags.HideInHierarchy)
//					{
//						script.WeaponsPrefabs[script.weaponIndex].hideFlags = HideFlags.HideInHierarchy;
//						script.WeaponsPrefabs[script.weaponIndex].SetActive(false);
//					}
//				}
//				else if (script.upInspectorTab == 2 || script.upInspectorTab == 0)
//				{
//					if (script.EnemiesPrefabs[script.enemyIndex] && script.EnemiesPrefabs[script.enemyIndex].hideFlags != HideFlags.HideInHierarchy)
//					{
//						script.EnemiesPrefabs[script.enemyIndex].hideFlags = HideFlags.HideInHierarchy;
//						script.EnemiesPrefabs[script.enemyIndex].SetActive(false);
//					}
//				}
			}
			else
			{
				if(script.currentController)
					if (Input.GetKeyDown(script.currentController._gamepadCodes[11]) || Input.GetKeyDown(script.currentController._keyboardCodes[11]) ||
					    Helper.CheckGamepadAxisButton(11, script.currentController._gamepadButtonsAxes, script.currentController.hasAxisButtonPressed, "GetKeyDown",
						    script.currentController.projectSettings.AxisButtonValues[11]))
						script.currentController.ChangeCameraType();
				
//				if (script.CurrentWeaponController && script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
//				{
//					if (script.AnimType == Adjustment.animType.FPS)
//					{
//						script.currentController.anim.SetLayerWeight(2, 1);
//					}
//					else
//					{
//						script.currentController.anim.SetLayerWeight(2, 0);
//						script.currentController.anim.SetLayerWeight(3, 0);
//					}
//				}
			}

			if (script.hide)
			{
				foreach (var obj in script.hideObjects)
				{
					if (obj && obj.hideFlags != HideFlags.HideInHierarchy)
					{
						obj.hideFlags = HideFlags.HideInHierarchy;
					}
				}
			}
			else
			{
				foreach (var obj in script.hideObjects)
				{
					if (obj && obj.hideFlags != HideFlags.None)
						obj.hideFlags = HideFlags.None;
				}
			}
			
			if (Application.isPlaying && script.currentController &&
			    script.oldCameraType != script.Characters[script.characterIndex].TypeOfCamera)
			{
				script.oldCameraType = script.Characters[script.characterIndex].TypeOfCamera;
				Repaint();
			}
			
		}


		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (script.SerializedWeaponController != null)
				script.SerializedWeaponController.Update();

			EditorGUILayout.Space();

			script.generalInspectorTab = GUILayout.Toolbar(script.generalInspectorTab, new[] {"Pose Editor", "Settings"});

			switch (script.generalInspectorTab)
			{
				case 1:

					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("box");
					script.settings.CubesSize = EditorGUILayout.Slider("IK Handle Scale", script.settings.CubesSize, 1, 30);
					script.settings.CubeSolid = (Helper.CubeSolid) EditorGUILayout.EnumPopup("IK Handle Layer", script.settings.CubeSolid);
					EditorGUILayout.EndVertical();
					break;


				case 0:

					EditorGUILayout.Space();
					var style = new GUIStyle
					{
						normal = new GUIStyleState {textColor = new Color32(0, 180, 70, 255)}, fontStyle = FontStyle.Bold
					};

					if (!Application.isPlaying)
					{
						EditorGUILayout.LabelField("Place here your <color=yellow>prefabs</color>," + "\n" + "then go to the [Play Mode] to start adjustment.", style);
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					}
					else
					{
						if (!script.isPause)
						{
							EditorGUILayout.LabelField("Press the [<color=yellow>Esc</color>] button in the Game window.", style);
							EditorGUILayout.Space();
						}
					}


					EditorGUI.BeginDisabledGroup(Application.isPlaying && !script.isPause);

					script.inspectorTab = GUILayout.Toolbar(script.inspectorTab, new[] {"Characters", "Weapons"});

					switch (script.inspectorTab)
					{
						case 0:

							#region CharacterAdjustment

							EditorGUILayout.Space();
							charactersList.DoLayoutList();
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							script.Type = Adjustment.AdjustmentType.Character;

							if (Application.isPlaying && script.isPause && script.currentController && script.currentController.DebugMode)
							{
								var curCharInfo = script.currentController.CharacterOffset;
								var curCameraMode = "";

								switch (script.currentController.TypeOfCamera)
								{
									case CharacterHelper.CameraType.ThirdPerson:
										curCameraMode = "TP view";
										break;
									case CharacterHelper.CameraType.FirstPerson:
										curCameraMode = "FP view";
										break;
									case CharacterHelper.CameraType.TopDown:
										curCameraMode = "TD view";
										break;
								}
								GUILayout.BeginVertical(script.currentController.gameObject.name + "'s Settings | " + curCameraMode, "window");
								EditorGUILayout.Space(); 
								//
								EditorGUILayout.LabelField("Body Offset", EditorStyles.boldLabel);
								EditorGUILayout.BeginVertical("HelpBox");

								if (script.currentController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
								{
									EditorGUILayout.HelpBox("FP view" + "\n" +
									                        "Adjust the rotation of the character's body so that he looks in the right direction", MessageType.Info);

									curCharInfo.xRotationOffset = EditorGUILayout.Slider("Body rotation offset X", curCharInfo.xRotationOffset, -90, 90);
									curCharInfo.yRotationOffset = EditorGUILayout.Slider("Body rotation offset X", curCharInfo.yRotationOffset, -90, 90);
									curCharInfo.zRotationOffset = EditorGUILayout.Slider("Body rotation offset Z", curCharInfo.zRotationOffset, -90, 90);
								}
								else
								{
									EditorGUILayout.HelpBox("The blue arrow should look forward and red should look right.", MessageType.Info);

									EditorGUILayout.PropertyField(serializedObject.FindProperty("dirObjRotX"), new GUIContent("Body rotation offset X"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("dirObjRotY"), new GUIContent("Body rotation offset Y"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("dirObjRotZ"), new GUIContent("Body rotation offset Z"));
								}

//								EditorGUILayout.Space();
//
//								curCharInfo.CharacterHeight = EditorGUILayout.Slider("Character Height Offset", curCharInfo.CharacterHeight, -5, 5);
								EditorGUILayout.EndVertical();
//								EditorGUILayout.Space();

								var curCamera = script.currentController.CameraController;
								var curCamInfo = curCamera.CameraOffset;

								EditorGUILayout.Space();

								EditorGUILayout.LabelField("Camera parameters", EditorStyles.boldLabel);
								switch (script.currentController.TypeOfCamera)
								{
									case CharacterHelper.CameraType.ThirdPerson:
										EditorGUILayout.HelpBox("These parameters are responsible for the camera position. " + "\n" +
										                        "Press the [C] button (in the Game) to switch the camera type.", MessageType.Info);
										break;
									case CharacterHelper.CameraType.FirstPerson:
										EditorGUILayout.HelpBox("Use this object to adjust camera position and rotation in FP view." + "\n" +
										                        "Press the [C] button (in the Game) to switch the camera type.", MessageType.Info);
										break;
									case CharacterHelper.CameraType.TopDown:
										EditorGUILayout.HelpBox("These parameters are responsible for the camera position." + "\n" +
										                        "Press the [C] button (in the Game) to switch the camera type.", MessageType.Info);
										break;
								}
								

								switch (script.currentController.TypeOfCamera)
								{
									case CharacterHelper.CameraType.FirstPerson:

										EditorGUILayout.BeginVertical("HelpBox");
										EditorGUI.BeginDisabledGroup(true);
										curCamera.CameraPosition =
											(Transform) EditorGUILayout.ObjectField("Camera position", curCamera.CameraPosition, typeof(GameObject), true);
										EditorGUI.EndDisabledGroup();
										break;

									case CharacterHelper.CameraType.ThirdPerson:

										curCamera.CameraAim = EditorGUILayout.ToggleLeft("Aim", curCamera.CameraAim);

										EditorGUILayout.BeginVertical("HelpBox");
										if (!curCamera.CameraAim)
										{
											curCamInfo.normDistance = EditorGUILayout.Slider("Distance", curCamInfo.normDistance, -20, 20);
//											EditorGUILayout.Space();
											curCamInfo.normCameraOffsetX = EditorGUILayout.Slider("Camera offset in X axis", curCamInfo.normCameraOffsetX, -20, 20);
											curCamInfo.normCameraOffsetY = EditorGUILayout.Slider("Camera offset in Y axis", curCamInfo.normCameraOffsetY, -20, 20);
//											EditorGUILayout.Space();
//											curCamInfo.cameraNormRotationOffset = EditorGUILayout.Vector3Field("Camera Rotation Offset", curCamInfo.cameraNormRotationOffset);
										}
										else
										{
											curCamInfo.aimDistance = EditorGUILayout.Slider("Distance", curCamInfo.aimDistance, -20, 20);
//											EditorGUILayout.Space();
											curCamInfo.aimCameraOffsetX = EditorGUILayout.Slider("Camera Offset in X axis", curCamInfo.aimCameraOffsetX, -20, 20);
											curCamInfo.aimCameraOffsetY = EditorGUILayout.Slider("Camera Offset in Y axis", curCamInfo.aimCameraOffsetY, -20, 20);
//											EditorGUILayout.Space();
//											curCamInfo.cameraAimRotationOffset = EditorGUILayout.Vector3Field("Camera Rotation Offset", curCamInfo.cameraAimRotationOffset);
										}

										break;
									case CharacterHelper.CameraType.TopDown:

										EditorGUILayout.BeginVertical("HelpBox");
										curCamera.Controller.CameraParameters.lockCamera = EditorGUILayout.Toggle("Lock Camera", curCamera.Controller.CameraParameters.lockCamera);
										if (!curCamera.Controller.CameraParameters.lockCamera)
										{
											curCamInfo.TD_Distance = EditorGUILayout.Slider("Distance", curCamInfo.TD_Distance, -20, 20);
											curCamInfo.TopDownAngle = EditorGUILayout.Slider("Angle", curCamInfo.TopDownAngle, 60, 90);
											curCamInfo.tdCameraOffsetX = EditorGUILayout.Slider("Camera offset in X axis", curCamInfo.tdCameraOffsetX, -20, 20);
											curCamInfo.tdCameraOffsetY = EditorGUILayout.Slider("Camera offset in Y axis", curCamInfo.tdCameraOffsetY, -20, 20);
										}
										else
										{
											curCamInfo.TDLockCameraDistance = EditorGUILayout.Slider("Distance", curCamInfo.TDLockCameraDistance, -20, 20);
											curCamInfo.tdLockCameraAngle = EditorGUILayout.Slider("Angle", curCamInfo.tdLockCameraAngle, 60, 90);
											curCamInfo.tdLockCameraOffsetX = EditorGUILayout.Slider("Camera offset in X axis", curCamInfo.tdLockCameraOffsetX, -20, 20);
											curCamInfo.tdLockCameraOffsetY = EditorGUILayout.Slider("Camera offset in Y axis", curCamInfo.tdLockCameraOffsetY, -20, 20);
										}

										break;
								}

								EditorGUILayout.EndVertical();
								EditorGUILayout.EndVertical();
								EditorGUILayout.Space();
								EditorGUILayout.Space();


								if (!script.currentController.CharacterOffset.HasTime)
									EditorGUILayout.LabelField("Not any save", style);
								else
								{
									var time = script.currentController.CharacterOffset.SaveTime;
									var date = script.currentController.CharacterOffset.SaveDate;
									EditorGUILayout.LabelField("Last save: " + date.x + "/" + date.y + "/" + date.z + " " +
									                           time.x + ":" + time.y + ":" + time.z, style);
								}

								if (GUILayout.Button("Save"))
								{
									script.currentController.CharacterOffset.SaveDate =
										new Vector3(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
									script.currentController.CharacterOffset.SaveTime =
										new Vector3(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

									script.currentController.CharacterOffset.HasTime = true;

									script.currentController.CharacterOffset.directionObjRotation = new Vector3(script.dirObjRotX, script.dirObjRotY, script.dirObjRotZ);

									script.CurrentCharacterOffsets[script.characterIndex].Clone(script.currentController.CharacterOffset);
									script.currentController.OriginalScript.CharacterOffset.Clone(script.currentController.CharacterOffset);

									script.currentController.CameraController.CameraOffset.cameraObjPos =
										script.currentController.CameraController.CameraPosition.localPosition;

									script.currentController.CameraController.CameraOffset.cameraObjRot =
										script.currentController.CameraController.CameraPosition.localEulerAngles;

									script.CurrentCameraOffsets[script.characterIndex].Clone(script.currentController.CameraController.CameraOffset);
									script.currentController.CameraController.OriginalScript.CameraOffset.Clone(script.currentController.CameraController.CameraOffset);
									

//							script.currentController.CameraController.CameraPosition.parent = script.currentController.BodyObjects.Head;
								}

								EditorGUI.BeginDisabledGroup(!script.currentController.CharacterOffset.HasTime);
								if (GUILayout.Button("Return values from last save"))
								{
									script.currentController.CharacterOffset.Clone(script.CurrentCharacterOffsets[script.characterIndex]);
									script.currentController.CameraController.CameraOffset.Clone(script.CurrentCameraOffsets[script.characterIndex]);

									curCamera.CameraPosition.localPosition = curCamera.CameraOffset.cameraObjPos;
									curCamera.CameraPosition.localEulerAngles = curCamera.CameraOffset.cameraObjRot;

									script.currentController.DirectionObject.localEulerAngles = script.currentController.CharacterOffset.directionObjRotation;
								}

								EditorGUI.EndDisabledGroup();

								if (GUILayout.Button("Set default positions"))
								{
									script.currentController.CharacterOffset.xRotationOffset = 0;
									script.currentController.CharacterOffset.yRotationOffset = 0;
									script.currentController.CharacterOffset.zRotationOffset = 0;
									script.currentController.CharacterOffset.CharacterHeight = -1.1f;

									script.currentController.CharacterOffset.directionObjRotation = Vector3.zero;
									script.currentController.DirectionObject.eulerAngles = Vector3.zero;

									script.dirObjRotX = 0;
									script.dirObjRotY = 0;
									script.dirObjRotZ = 0;

									curCamera.CameraOffset.normDistance = 0;
									curCamera.CameraOffset.normCameraOffsetX = 0;
									curCamera.CameraOffset.normCameraOffsetY = 0;
									
//									curCamera.CameraOffset.cameraNormRotationOffset = Vector3.zero;
//									curCamera.CameraOffset.cameraAimRotationOffset = Vector3.zero;

									curCamera.CameraOffset.aimDistance = 0;
									curCamera.CameraOffset.aimCameraOffsetX = 0;
									curCamera.CameraOffset.aimCameraOffsetY = 0;

									curCamera.CameraOffset.cameraObjPos = Vector3.zero;
									curCamera.CameraOffset.cameraObjRot = Vector3.zero;

									curCamera.CameraPosition.localPosition = Vector3.zero;
									curCamera.CameraPosition.localEulerAngles = Vector3.zero;

									curCamera.CameraOffset.tdLockCameraAngle = 90;
									curCamera.CameraOffset.tdLockCameraOffsetX = 0;
									curCamera.CameraOffset.tdLockCameraOffsetY = 0;

									curCamera.CameraOffset.TopDownAngle = 80;
									curCamera.CameraOffset.tdCameraOffsetX = 0;
									curCamera.CameraOffset.tdCameraOffsetY = 0;

									curCamera.CameraOffset.TD_Distance = 0;

								}
							}

							#endregion

							break;
							

							//enemiesList.DoLayoutList();
							//EditorGUILayout.Space();
//					if (Application.isPlaying && script.CurrentAiController)
//					{
//						script.enemyState = GUILayout.Toolbar(script.enemyState, new[] {"Idle", "Walk", "Run"});
//
//						if (script.enemyState != script.oldEnemyState)
//						{
//							switch (script.enemyState)
//							{
//								case 0:
//									script.CurrentAiController.anim.Play("Idle", 0);
//									script.CurrentAiController.anim.SetBool("Move", false);
//									script.CurrentAiController.anim.SetBool("Attack state", false);
//									break;
//
//								case 1:
//									script.CurrentAiController.anim.Play("Walk", 0);
//									script.CurrentAiController.anim.SetBool("Move", true);
//									script.CurrentAiController.anim.SetBool("Attack state", false);
//									break;
//
//								case 2:
//									script.CurrentAiController.anim.Play("Run", 0);
//									script.CurrentAiController.anim.SetBool("Move", true);
//									script.CurrentAiController.anim.SetBool("Attack state", true);
//									break;
//							}
//
//							script.oldEnemyState = script.enemyState;
//						}
//						
//						EditorGUILayout.Space();
//					}

						case 1:

							#region WeaponAdjustment

							if (Application.isPlaying && script.isPause && !script.currentController)
							{
								EditorGUILayout.Space();
								EditorGUILayout.LabelField("First of all select any character.", style);
							}

							EditorGUI.BeginDisabledGroup(Application.isPlaying && !script.currentController);
							EditorGUILayout.Space();
							weaponsList.DoLayoutList();
							EditorGUILayout.Space();
							EditorGUI.EndDisabledGroup();


							if (Application.isPlaying && script.isPause && script.CurrentWeaponController && script.currentController && script.CurrentWeaponController.ActiveDebug)
							{

								#region attack_menu

								if (script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) && script.Type != Adjustment.AdjustmentType.Enemy ||
								    script.CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) &&
								    script.Type == Adjustment.AdjustmentType.Character ||
								    script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))

								{

									var curInfo = script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.settingsSlotIndex];


//									if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
//									{
									EditorGUILayout.BeginVertical("box");
									script.CurrentWeaponController.settingsSlotIndex = EditorGUILayout.Popup("Settings Slot",
										script.CurrentWeaponController.settingsSlotIndex, script.CurrentWeaponController.enumNames.ToArray());

									EditorGUILayout.Space();
									if (!rename)
									{
										if (GUILayout.Button("Rename"))
										{
											rename = true;
											curName = "";
										}
									}
									else
									{
										EditorGUILayout.BeginVertical("box");
										curName = EditorGUILayout.TextField("New name", curName);

										EditorGUILayout.BeginHorizontal();

										if (GUILayout.Button("Cancel"))
										{
											rename = false;
											renameError = false;
											curName = "";
										}

										if (GUILayout.Button("Save"))
										{
											if (!script.CurrentWeaponController.enumNames.Contains(curName))
											{
												rename = false;
												script.CurrentWeaponController.enumNames[script.CurrentWeaponController.settingsSlotIndex] = curName;
												script.CurrentWeaponController.OriginalScript.enumNames[script.CurrentWeaponController.settingsSlotIndex] = curName;
												curName = "";
												renameError = false;
											}
											else
											{
												renameError = true;
											}
										}

										EditorGUILayout.EndHorizontal();

										if (renameError)
											EditorGUILayout.HelpBox("This name already exist.", MessageType.Warning);

										EditorGUILayout.EndVertical();
									}


									EditorGUI.BeginDisabledGroup(script.CurrentWeaponController.WeaponInfos.Count <= 1);
									if (!delete)
									{
										if (GUILayout.Button("Delete slot"))
										{
											delete = true;
										}
									}
									else
									{
										EditorGUILayout.BeginVertical("box");
										EditorGUILayout.LabelField("Are you sure?");
										EditorGUILayout.BeginHorizontal();

										if (GUILayout.Button("No"))
										{
											delete = false;
										}

										if (GUILayout.Button("Yes"))
										{
//												if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))

											Helper.HideAllObjects(script.CurrentWeaponController.IkObjects);
											Selection.activeObject = script.gameObject;

											script.CurrentWeaponController.WeaponInfos.Remove(script.Weapons[script.weaponIndex]
												.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex]);

											script.CurrentWeaponController.CurrentWeaponInfo.Remove(
												script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.settingsSlotIndex]);

											script.CurrentWeaponController.enumNames.Remove(script.Weapons[script.weaponIndex]
												.enumNames[script.CurrentWeaponController.settingsSlotIndex]);

											script.CurrentWeaponController.OriginalScript.WeaponInfos.Remove(
												script.CurrentWeaponController.OriginalScript.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex]);

											script.CurrentWeaponController.OriginalScript.enumNames.Remove(
												script.CurrentWeaponController.OriginalScript.enumNames[script.CurrentWeaponController.settingsSlotIndex]);

											var newInfoIndex = script.CurrentWeaponController.settingsSlotIndex;

											newInfoIndex++;
											if (newInfoIndex > script.CurrentWeaponController.WeaponInfos.Count - 1)
											{
												newInfoIndex = 0;
											}

											script.CurrentWeaponController.settingsSlotIndex = newInfoIndex;
											delete = false;
										}

										EditorGUILayout.EndHorizontal();
										EditorGUILayout.EndVertical();
									}

									EditorGUI.EndDisabledGroup();
									EditorGUILayout.EndVertical();

									if (GUILayout.Button("Add new slot"))
									{
										script.CurrentWeaponController.WeaponInfos.Add(new WeaponsHelper.WeaponInfo());
										script.CurrentWeaponController.OriginalScript.WeaponInfos.Add(new WeaponsHelper.WeaponInfo());
										
										script.CurrentWeaponController.enumNames.Add("Slot " + (script.CurrentWeaponController.enumNames.Count + 1));
										script.CurrentWeaponController.OriginalScript.enumNames.Add("Slot " + (script.CurrentWeaponController.enumNames.Count + 1));
										

										script.CurrentWeaponController.CurrentWeaponInfo.Add(new WeaponsHelper.WeaponInfo());

										script.CurrentWeaponController.settingsSlotIndex = script.CurrentWeaponController.enumNames.Count - 1;
										
										if (script.CurrentWeaponController.OriginalScript)
											EditorUtility.SetDirty(script.CurrentWeaponController.OriginalScript);

										break;
									}
									

									#endregion

//									if (script.CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
//									{
									EditorGUILayout.Space();
									EditorGUILayout.Space();
									GUILayout.BeginVertical(script.CurrentWeaponController.gameObject.name + " Settings | "+ "Slot: " + script.CurrentWeaponController.enumNames[script.CurrentWeaponController.settingsSlotIndex], "window");
									script.ikInspectorTab = GUILayout.Toolbar(script.ikInspectorTab, new[] {"Hands", "Elbows", "Fingers"});
									
									EditorGUILayout.Space();
									
									switch (script.ikInspectorTab)
									{
										case 0:
//											if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Norm && !curInfo.disableIkInNormalState ||
//											    script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Crouch && !curInfo.disableIkInCrouchState
//											    || script.CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Norm
//											    || script.CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Crouch)
//											{
//												EditorGUILayout.HelpBox("Use these objects to adjust hands and elbows positions and rotations." + "\n" +
//												                        "To adjust other IK states switch the [Debug Mode].", MessageType.Info);
//											}

											EditorGUILayout.PropertyField(script.SerializedWeaponController.FindProperty("DebugMode"), new GUIContent("IK Mode"));
											
											EditorGUILayout.Space();

//											if (script.CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Wall)
//											{
//
//												EditorGUILayout.Space();
//												EditorGUILayout.BeginVertical("HelpBox");
//												EditorGUILayout.LabelField("Collider:", EditorStyles.boldLabel);
//												EditorGUI.BeginDisabledGroup(true);
//												script.CurrentWeaponController.ColliderToCheckWalls =
//												(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.ColliderToCheckWalls, typeof(Transform), true);
//												EditorGUI.EndDisabledGroup();
//												EditorGUILayout.HelpBox("During the game this Collider will check the collision with a wall for the weapon.", MessageType.Info);
//												EditorGUILayout.EndVertical();
//
//												EditorGUILayout.Space();
//											}

											if (script.CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Norm)
											{
												EditorGUILayout.BeginVertical("HelpBox");
												curInfo.disableIkInNormalState = EditorGUILayout.ToggleLeft("Disable IK in this state", curInfo.disableIkInNormalState);
												EditorGUILayout.EndVertical();
												EditorGUILayout.Space();
											}
											else if (script.CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Crouch)
											{
												EditorGUILayout.BeginVertical("HelpBox");
												curInfo.disableIkInCrouchState = EditorGUILayout.ToggleLeft("Disable IK in this state", curInfo.disableIkInCrouchState);
												EditorGUILayout.EndVertical();
												EditorGUILayout.Space();
											}

											if (script.CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Norm && !curInfo.disableIkInNormalState ||
											    script.CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Crouch && !curInfo.disableIkInCrouchState
											    || script.CurrentWeaponController.DebugMode != IKHelper.IkDebugMode.Norm
											    || script.CurrentWeaponController.DebugMode != IKHelper.IkDebugMode.Crouch)
											{
												switch (script.CurrentWeaponController.DebugMode)
												{
													case IKHelper.IkDebugMode.Norm:
														if (!curInfo.disableIkInNormalState)
														{
															
															EditorGUILayout.BeginVertical("HelpBox");
															EditorGUILayout.LabelField("Right Hand:", EditorStyles.boldLabel);
															EditorGUI.BeginDisabledGroup(true);
															script.CurrentWeaponController.IkObjects.RightObject =
																(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.RightObject, typeof(Transform), true);
															EditorGUI.EndDisabledGroup();
															EditorGUILayout.EndVertical();
															
															EditorGUILayout.Space();
															
															
															EditorGUILayout.BeginVertical("HelpBox");
															EditorGUILayout.LabelField("Left Hand:", EditorStyles.boldLabel);
															EditorGUI.BeginDisabledGroup(true);
															script.CurrentWeaponController.IkObjects.LeftObject =
																(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.LeftObject, typeof(Transform), true);
															EditorGUI.EndDisabledGroup();
															script.CurrentWeaponController.pinLeftObject = EditorGUILayout.ToggleLeft("Pin", script.CurrentWeaponController.pinLeftObject);
															EditorGUILayout.HelpBox("If this checkbox is active, the left hand depends on the right.", MessageType.Info);
															EditorGUILayout.EndVertical();

															EditorGUILayout.Space();
														}

														break;
													case IKHelper.IkDebugMode.Aim:

														EditorGUILayout.BeginVertical("HelpBox");
														EditorGUILayout.LabelField("Right Hand:", EditorStyles.boldLabel);
														EditorGUI.BeginDisabledGroup(true);
														script.CurrentWeaponController.IkObjects.RightAimObject =
															(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.RightAimObject, typeof(Transform), true);
														EditorGUI.EndDisabledGroup();
														EditorGUILayout.EndVertical();
														EditorGUILayout.Space();
														
														EditorGUILayout.BeginVertical("HelpBox");
														EditorGUILayout.LabelField("Left Hand:", EditorStyles.boldLabel);
														EditorGUI.BeginDisabledGroup(true);
														script.CurrentWeaponController.IkObjects.LeftAimObject =
															(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.LeftAimObject, typeof(Transform), true);
														EditorGUI.EndDisabledGroup();
														script.CurrentWeaponController.pinLeftObject = EditorGUILayout.ToggleLeft("Pin", script.CurrentWeaponController.pinLeftObject);
														EditorGUILayout.HelpBox("If this checkbox is active, the left hand depends on the right.", MessageType.Info);
														EditorGUILayout.EndVertical();
														
														EditorGUILayout.Space();

														break;
													case IKHelper.IkDebugMode.Wall:
														
															EditorGUILayout.BeginVertical("HelpBox");
															EditorGUILayout.LabelField("Right Hand:", EditorStyles.boldLabel);
															EditorGUI.BeginDisabledGroup(true);
															script.CurrentWeaponController.IkObjects.RightWallObject =
																(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.RightWallObject, typeof(Transform), true);
															EditorGUI.EndDisabledGroup();
															EditorGUILayout.EndVertical();
															EditorGUILayout.Space();

															EditorGUILayout.BeginVertical("HelpBox");
															EditorGUILayout.LabelField("Left Hand:", EditorStyles.boldLabel);
															EditorGUI.BeginDisabledGroup(true);
															script.CurrentWeaponController.IkObjects.LeftWallObject =
																(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.LeftWallObject, typeof(Transform), true);
															EditorGUI.EndDisabledGroup();
															script.CurrentWeaponController.pinLeftObject = EditorGUILayout.ToggleLeft("Pin", script.CurrentWeaponController.pinLeftObject);
															EditorGUILayout.HelpBox("If this checkbox is active, the left hand depends on the right.", MessageType.Info);
															EditorGUILayout.EndVertical();

															EditorGUILayout.Space();

															break;
													case IKHelper.IkDebugMode.Crouch:

														if (!curInfo.disableIkInCrouchState)
														{
															EditorGUILayout.BeginVertical("HelpBox");
															EditorGUILayout.LabelField("Right Hand:", EditorStyles.boldLabel);
															EditorGUI.BeginDisabledGroup(true);
															script.CurrentWeaponController.IkObjects.RightCrouchObject =
																(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.RightCrouchObject, typeof(Transform), true);
															EditorGUI.EndDisabledGroup();
															EditorGUILayout.EndVertical();
															EditorGUILayout.Space();
															
															EditorGUILayout.BeginVertical("HelpBox");
															EditorGUILayout.LabelField("Left Hand:", EditorStyles.boldLabel);
															EditorGUI.BeginDisabledGroup(true);
															script.CurrentWeaponController.IkObjects.LeftCrouchObject =
																(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.LeftCrouchObject, typeof(Transform), true);
															EditorGUI.EndDisabledGroup();
															script.CurrentWeaponController.pinLeftObject = EditorGUILayout.ToggleLeft("Pin", script.CurrentWeaponController.pinLeftObject);
															EditorGUILayout.HelpBox("If this checkbox is active, the left hand depends on the right.", MessageType.Info);
															EditorGUILayout.EndVertical();
															
															EditorGUILayout.Space();
														}

														break;
												}
											}

											EditorGUILayout.Space();
											EditorGUILayout.Space();
											
											
											EditorGUILayout.LabelField("Copy values from:", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("HelpBox");
											copyToList.DoLayoutList();

											if (GUILayout.Button("Copy"))
											{
												if (script.CurrentWeaponController.pinLeftObject)
												{
													script.CurrentWeaponController.pinLeftObject = false;
													script.StartCoroutine("CopyTimeout");
												}
												else
												{
													script.CopyWeaponData();
												}
											}
											
											EditorGUILayout.EndVertical();

											break;

										case 1:

											EditorGUILayout.BeginVertical("HelpBox");
											curInfo.disableElbowIK = EditorGUILayout.ToggleLeft("Disable Elbow IK", curInfo.disableElbowIK);
											EditorGUILayout.EndVertical();
											EditorGUILayout.Space();

											if ((script.CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Norm && !curInfo.disableIkInNormalState ||
											     script.CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Crouch && !curInfo.disableIkInCrouchState
											     || script.CurrentWeaponController.DebugMode != IKHelper.IkDebugMode.Norm
											     || script.CurrentWeaponController.DebugMode != IKHelper.IkDebugMode.Crouch) && !curInfo.disableElbowIK)
											{

												EditorGUILayout.BeginVertical("HelpBox");
												EditorGUILayout.LabelField("Right Elbow:", EditorStyles.boldLabel);
												EditorGUI.BeginDisabledGroup(true);
												script.CurrentWeaponController.IkObjects.RightElbowObject =
													(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.RightElbowObject, typeof(Transform), true);
												EditorGUI.EndDisabledGroup();
												EditorGUILayout.EndVertical();
												
												EditorGUILayout.Space();
												
												EditorGUILayout.BeginVertical("HelpBox");
												EditorGUILayout.LabelField("Left Elbow:", EditorStyles.boldLabel);
												EditorGUI.BeginDisabledGroup(true);
												script.CurrentWeaponController.IkObjects.LeftElbowObject =
													(Transform) EditorGUILayout.ObjectField(script.CurrentWeaponController.IkObjects.LeftElbowObject, typeof(Transform), true);
												EditorGUI.EndDisabledGroup();
												EditorGUILayout.EndVertical();
											}

											EditorGUILayout.Space();
											EditorGUILayout.Space();

											
											EditorGUILayout.LabelField("Copy values from:", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("HelpBox");
											copyToList.DoLayoutList();

											if (GUILayout.Button("Copy"))
											{
												script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.settingsSlotIndex]
													.ElbowsClone(script.Weapons[script.copyFromWeaponSlot].WeaponInfos[script.copyFromSlot]);
											}
											EditorGUILayout.EndVertical();

											break;


										case 2:
											
											EditorGUILayout.BeginVertical("HelpBox");
											axises = (Helper.RotationAxes) EditorGUILayout.EnumPopup("Fingers rotation axis", axises);
											EditorGUILayout.EndVertical();
											
											EditorGUILayout.Space();
											
											EditorGUILayout.BeginVertical("HelpBox");
											switch (axises)
											{
												case Helper.RotationAxes.X:

													curInfo.FingersRightX = EditorGUILayout.Slider("Right Fingers", curInfo.FingersRightX, -25, 25);

													curInfo.ThumbRightX = EditorGUILayout.Slider("Right Thumb", curInfo.ThumbRightX, -25, 25);

													EditorGUILayout.Space();

													curInfo.FingersLeftX = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftX, -25, 25);

													curInfo.ThumbLeftX = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftX, -25, 25);

													break;
												case Helper.RotationAxes.Y:

													curInfo.FingersRightY = EditorGUILayout.Slider("Right Fingers", curInfo.FingersRightY, -25, 25);

													curInfo.ThumbRightY = EditorGUILayout.Slider("Right Thumb", curInfo.ThumbRightY, -25, 25);

													EditorGUILayout.Space();

													curInfo.FingersLeftY = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftY, -25, 25);

													curInfo.ThumbLeftY = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftY, -25, 25);

													break;
												case Helper.RotationAxes.Z:

													curInfo.FingersRightZ = EditorGUILayout.Slider("Right Fingers", curInfo.FingersRightZ, -25, 25);

													curInfo.ThumbRightZ = EditorGUILayout.Slider("Right Thumb", curInfo.ThumbRightZ, -25, 25);

													EditorGUILayout.Space();

													curInfo.FingersLeftZ = EditorGUILayout.Slider("Left Fingers", curInfo.FingersLeftZ, -25, 25);

													curInfo.ThumbLeftZ = EditorGUILayout.Slider("Left Thumb", curInfo.ThumbLeftZ, -25, 25);

													break;
											}
											EditorGUILayout.EndVertical();

											EditorGUILayout.Space();
											EditorGUILayout.Space();

											
											EditorGUILayout.LabelField("Copy values from:", EditorStyles.boldLabel);
											EditorGUILayout.BeginVertical("HelpBox");
											copyToList.DoLayoutList();

											if (GUILayout.Button("Copy"))
											{
												script.CurrentWeaponController.CurrentWeaponInfo[script.CurrentWeaponController.settingsSlotIndex]
													.FingersClone(script.Weapons[script.copyFromWeaponSlot].WeaponInfos[script.copyFromSlot]);
											}

											EditorGUILayout.EndVertical();
											break;
									}

									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.Space();

									if (script.CurrentWeaponController.WeaponInfos.Count > 0 &&
									    !script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].HasTime)
										EditorGUILayout.LabelField("Not any save", style);
									else
									{
										var time = script.CurrentWeaponController
											.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].SaveTime;
										var date = script.CurrentWeaponController
											.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].SaveDate;
										EditorGUILayout.LabelField("Last save: " + date.x + "/" + date.y + "/" + date.z + " " +
										                           time.x + ":" + time.y + ":" + time.z, style);
									}

									if (GUILayout.Button("Save"))
									{

										curInfo.SaveDate = new Vector3(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
										curInfo.SaveTime = new Vector3(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

										curInfo.HasTime = true;

										if (script.CurrentWeaponController.pinLeftObject)
										{
											script.CurrentWeaponController.pinLeftObject = false;
											script.StartCoroutine("SaveTimeout");
										}
										else
										{
											script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].Clone(curInfo);

											script.SaveData();

											IKHelper.CheckIK(ref script.CurrentWeaponController.CanUseElbowIK,
												ref script.CurrentWeaponController.CanUseIK, ref script.CurrentWeaponController.CanUseAimIK,
												ref script.CurrentWeaponController.CanUseWallIK, ref script.CurrentWeaponController.CanUseCrouchIK, curInfo);

											if (!script.CurrentWeaponController.CanUseIK)
												script.CurrentWeaponController.CanUseIK = true;
										}
									}

									EditorGUI.BeginDisabledGroup(!curInfo.HasTime);
									if (GUILayout.Button("Return values from last save"))
									{
										curInfo.Clone(script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex]);

										if (curInfo.WeaponSize != Vector3.zero)
											script.CurrentWeaponController.transform.localScale = curInfo.WeaponSize;
										else
											script.CurrentWeaponController.transform.localScale = script.currentScales[script.weaponIndex];

										script.CurrentWeaponController.transform.localPosition = curInfo.WeaponPosition;
										script.CurrentWeaponController.transform.localEulerAngles = curInfo.WeaponRotation;


										if (curInfo.RightHandPosition != Vector3.zero && curInfo.RightHandRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.RightObject.localPosition = curInfo.RightHandPosition;
											script.CurrentWeaponController.IkObjects.RightObject.localEulerAngles = curInfo.RightHandRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.RightObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
											script.CurrentWeaponController.IkObjects.RightObject.localPosition = Vector3.zero;
											script.CurrentWeaponController.IkObjects.RightObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}

										if (curInfo.LeftHandPosition != Vector3.zero && curInfo.LeftHandRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.LeftObject.localPosition =
												curInfo.LeftHandPosition;
											script.CurrentWeaponController.IkObjects.LeftObject.localEulerAngles =
												curInfo.LeftHandRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.LeftObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
											script.CurrentWeaponController.IkObjects.LeftObject.localPosition = Vector3.zero;
											script.CurrentWeaponController.IkObjects.LeftObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}



										if (curInfo.RightCrouchHandPosition != Vector3.zero && curInfo.RightCrouchHandRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition = curInfo.RightCrouchHandPosition;
											script.CurrentWeaponController.IkObjects.RightCrouchObject.localEulerAngles = curInfo.RightCrouchHandRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.RightCrouchObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
											script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition = Vector3.zero;
											script.CurrentWeaponController.IkObjects.RightCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}

										if (curInfo.LeftCrouchHandPosition != Vector3.zero && curInfo.LeftCrouchHandRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition = curInfo.LeftCrouchHandPosition;
											script.CurrentWeaponController.IkObjects.LeftCrouchObject.localEulerAngles = curInfo.LeftCrouchHandRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.LeftCrouchObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
											script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition = Vector3.zero;
											script.CurrentWeaponController.IkObjects.LeftCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}



										if (curInfo.RightAimPosition != Vector3.zero && curInfo.RightAimRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.RightAimObject.localPosition = script.CurrentWeaponController
												.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].RightAimPosition;
											script.CurrentWeaponController.IkObjects.RightAimObject.localEulerAngles = script.CurrentWeaponController
												.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].RightAimRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.RightAimObject.parent =
												script.CurrentWeaponController.BodyObjects.RightHand;
											script.CurrentWeaponController.IkObjects.RightAimObject.localPosition = Vector3.up;
											script.CurrentWeaponController.IkObjects.RightAimObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}

										if (curInfo.LeftAimPosition != Vector3.zero && curInfo.LeftAimRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition =
												script.CurrentWeaponController.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].LeftAimPosition;
											script.CurrentWeaponController.IkObjects.LeftAimObject.localEulerAngles = script.CurrentWeaponController
												.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].LeftAimRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.LeftAimObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
											script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition = Vector3.up;
											script.CurrentWeaponController.IkObjects.LeftAimObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}



										if (curInfo.RightHandWallPosition != Vector3.zero && curInfo.RightHandWallRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.RightWallObject.localPosition =
												curInfo.RightHandWallPosition;
											script.CurrentWeaponController.IkObjects.RightWallObject.localEulerAngles =
												curInfo.RightHandWallRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.RightWallObject.parent =
												script.CurrentWeaponController.BodyObjects.RightHand;
											script.CurrentWeaponController.IkObjects.RightWallObject.localPosition = Vector3.up;
											script.CurrentWeaponController.IkObjects.RightWallObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}

										if (curInfo.LeftHandWallPosition != Vector3.zero && curInfo.LeftHandWallRotation != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition
												= curInfo.LeftHandWallPosition;
											script.CurrentWeaponController.IkObjects.LeftWallObject.localEulerAngles
												= curInfo.LeftHandWallRotation;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.LeftWallObject.parent =
												script.CurrentWeaponController.BodyObjects.RightHand;
											script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition = Vector3.up;
											script.CurrentWeaponController.IkObjects.LeftWallObject.localRotation = Quaternion.Euler(-90, 0, 0);
										}

										if (curInfo.LeftElbowPosition != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition = script.CurrentWeaponController
												.WeaponInfos[script.CurrentWeaponController.settingsSlotIndex].LeftElbowPosition;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition =
												script.currentController.DirectionObject.position - script.currentController.DirectionObject.right * 2;
										}

										if (curInfo.RightElbowPosition != Vector3.zero)
										{
											script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition =
												curInfo.RightElbowPosition;
										}
										else
										{
											script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition =
												script.currentController.DirectionObject.position + script.currentController.DirectionObject.right * 2;
										}
									}

									EditorGUI.EndDisabledGroup();

									if (GUILayout.Button("Set default values"))
									{

//										script.CurrentWeaponController.IkObjects.RightObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
//										script.CurrentWeaponController.IkObjects.RightObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.RightObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.LeftObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
//										script.CurrentWeaponController.IkObjects.LeftObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.LeftObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.RightCrouchObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
//										script.CurrentWeaponController.IkObjects.RightCrouchObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.RightCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.LeftCrouchObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
//										script.CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.LeftCrouchObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.RightAimObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
//										script.CurrentWeaponController.IkObjects.RightAimObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.RightAimObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.LeftAimObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
//										script.CurrentWeaponController.IkObjects.LeftAimObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.LeftAimObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.RightWallObject.parent = script.CurrentWeaponController.BodyObjects.RightHand;
//										script.CurrentWeaponController.IkObjects.RightWallObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.RightWallObject.localRotation = Quaternion.Euler(-90, 0, 0);
//
//										script.CurrentWeaponController.IkObjects.LeftWallObject.parent = script.CurrentWeaponController.BodyObjects.LeftHand;
//										script.CurrentWeaponController.IkObjects.LeftWallObject.localPosition = Vector3.zero;
//										script.CurrentWeaponController.IkObjects.LeftWallObject.localRotation = Quaternion.Euler(-90, 0, 0);

										script.currentController.WeaponManager.DebugIKValue = 0;
										script.StartCoroutine("SetDefault");

//										script.CurrentWeaponController.IkObjects.LeftElbowObject.localPosition =
//											script.currentController.DirectionObject.position - script.currentController.DirectionObject.right * 2;
//										script.CurrentWeaponController.IkObjects.RightElbowObject.localPosition =
//											script.currentController.DirectionObject.position + script.currentController.DirectionObject.right * 2;


										curInfo = new WeaponsHelper.WeaponInfo();


										script.CurrentWeaponController.transform.localPosition = curInfo.WeaponPosition;
										script.CurrentWeaponController.transform.localEulerAngles = curInfo.WeaponRotation;

										if (curInfo.WeaponSize != Vector3.zero)
											script.CurrentWeaponController.transform.localScale = curInfo.WeaponSize;
										else
											script.CurrentWeaponController.transform.localScale = script.currentScales[script.weaponIndex];

									}
								}

							}

							#endregion

							break;
					}

					break;
			}

			var inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;

			if (!Application.isPlaying && inputs.oldScenePath != "")
			{
				EditorGUILayout.Space();
				if (GUILayout.Button("Back to the [" + inputs.oldSceneName + "] scene"))
				{
					if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] {SceneManager.GetActiveScene()}))
						EditorSceneManager.OpenScene(inputs.oldScenePath, OpenSceneMode.Single);

					return;
				}
			}

			serializedObject.ApplyModifiedProperties();

			if (script.SerializedWeaponController != null)
				script.SerializedWeaponController.ApplyModifiedProperties();

//			DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);

				if (script.CurrentWeaponController)
					EditorUtility.SetDirty(script.CurrentWeaponController);

				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}
