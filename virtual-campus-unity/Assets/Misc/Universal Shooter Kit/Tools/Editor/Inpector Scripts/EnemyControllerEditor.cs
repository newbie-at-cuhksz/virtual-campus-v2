using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(EnemyController))]
	public class EnemyControllerEditor : Editor
	{
		private EnemyController script;

		private ReorderableList damageAnimations;
		private ReorderableList findAnimations;
		private ReorderableList attackAnimations;
		private ReorderableList attackPoints;
		private ReorderableList damageColliders;
		private ReorderableList bloodHoles;
		private ReorderableList genericColliders;

		private GUIStyle style;

		private void Awake()
		{
			script = (EnemyController) target;
		}

		private void OnEnable()
		{
			findAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("FindAnimations"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Find Animations"); },

				onAddCallback = items => { script.FindAnimations.Add(null); },

				onRemoveCallback = items =>
				{
					if (script.FindAnimations.Count == 1)
						return;

					script.FindAnimations.Remove(script.FindAnimations[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.FindAnimations[index] = (AnimationClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
						script.FindAnimations[index], typeof(AnimationClip), false);
				}
			};
			damageAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("DamageAnimations"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Damage Reaction Animations"); },

				onAddCallback = items => { script.DamageAnimations.Add(null); },

				onRemoveCallback = items =>
				{
					if (script.DamageAnimations.Count == 1)
						return;

					script.DamageAnimations.Remove(script.DamageAnimations[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.DamageAnimations[index] = (AnimationClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
						script.DamageAnimations[index], typeof(AnimationClip), false);
				}
			};
			attackAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("MeleeAttackAnimations"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Attack Animations"); },

				onAddCallback = items => { script.Attacks[0].MeleeAttackAnimations.Add(null); },

				onRemoveCallback = items =>
				{
					if (script.Attacks[0].MeleeAttackAnimations.Count == 1)
						return;

					script.Attacks[0].MeleeAttackAnimations.Remove(script.Attacks[0].MeleeAttackAnimations[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Attacks[0].MeleeAttackAnimations[index] = (AnimationClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
						script.Attacks[0].MeleeAttackAnimations[index], typeof(AnimationClip), false);
				}
			};
			
			bloodHoles = new ReorderableList(serializedObject, serializedObject.FindProperty("BloodHoles"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Blood Holes"); },

				onAddCallback = items => { script.BloodHoles.Add(null); },

				onRemoveCallback = items =>
				{
					if (script.BloodHoles.Count == 1)
						return;

					script.BloodHoles.Remove(script.BloodHoles[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.BloodHoles[index] = (Texture) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
						script.BloodHoles[index], typeof(Texture), false);
				}
			};
			
			genericColliders = new ReorderableList(serializedObject, serializedObject.FindProperty("genericColliders"), false, true, true, true)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight), "Body Colliders");
					EditorGUI.LabelField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10, EditorGUIUtility.singleLineHeight), "Damage Multipliers");
				},

				onAddCallback = items => { script.genericColliders.Add(new AIHelper.GenericCollider()); },

				onRemoveCallback = items =>
				{
					script.genericColliders.Remove(script.genericColliders[items.index]);
				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.genericColliders[index].collider = (Collider) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight), script.genericColliders[index].collider, typeof(Collider), true);
					script.genericColliders[index].damageMultiplier = EditorGUI.FloatField(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10, EditorGUIUtility.singleLineHeight), script.genericColliders[index].damageMultiplier);
				}
			};
			
			attackPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("AttackSpawnPoints"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Attack Points"); },

				onAddCallback = items =>
				{
					if (!script.gameObject.activeInHierarchy)
					{
						var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

						var attackPoint = new GameObject("Attack Point");

						attackPoint.transform.parent = tempEnemy.transform;
						attackPoint.transform.localPosition = Vector3.zero;
						tempEnemy.GetComponent<EnemyController>().Attacks[0].AttackSpawnPoints.Add(attackPoint.transform);
						
#if !UNITY_2018_3_OR_NEWER
						PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
						
						DestroyImmediate(tempEnemy);
					}
					else
					{
						var attackPoint = new GameObject("Attack Point");
						attackPoint.transform.parent = script.transform;
						attackPoint.transform.localPosition = Vector3.zero;
						script.Attacks[0].AttackSpawnPoints.Add(attackPoint.transform);
					}
				},

				onRemoveCallback = items =>
				{
					if (script.Attacks[0].AttackSpawnPoints.Count == 1)
						return;

					if (!script.gameObject.activeInHierarchy)
					{
						var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

						if(tempEnemy.GetComponent<EnemyController>().Attacks[0].AttackSpawnPoints[items.index])
							DestroyImmediate(tempEnemy.GetComponent<EnemyController>().Attacks[0].AttackSpawnPoints[items.index].gameObject);
						
						tempEnemy.GetComponent<EnemyController>().Attacks[0].AttackSpawnPoints.Remove(tempEnemy.GetComponent<EnemyController>().Attacks[0].AttackSpawnPoints[items.index]);

#if !UNITY_2018_3_OR_NEWER
						PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
						
						DestroyImmediate(tempEnemy);
					}
					else
					{
						if(script.Attacks[0].AttackSpawnPoints[items.index])
							DestroyImmediate(script.Attacks[0].AttackSpawnPoints[items.index].gameObject);
						script.Attacks[0].AttackSpawnPoints.Remove(script.Attacks[0].AttackSpawnPoints[items.index]);
					}

				},

				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Attacks[0].AttackSpawnPoints[index] = (Transform) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
						script.Attacks[0].AttackSpawnPoints[index], typeof(Transform), true);
				}
			};
			
			damageColliders = new ReorderableList(serializedObject, serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("DamageColliders"), false, true, true, true)
			{
				drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Damage Colliders"); },

				onAddCallback = items =>
				{
					if (!script.gameObject.activeInHierarchy)
					{
						var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

						var collider = new GameObject("Damage Collider");
						collider.transform.parent = tempEnemy.transform;
						collider.transform.localPosition = Vector3.zero;
						collider.tag = script.Attacks[0].AttackType == AIHelper.AttackTypes.Fire ? "Fire" : "Melee Collider";
						tempEnemy.GetComponent<EnemyController>().Attacks[0].DamageColliders.Add(collider.AddComponent<BoxCollider>());
						
#if !UNITY_2018_3_OR_NEWER
						PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
						
						DestroyImmediate(tempEnemy);
					}
					else
					{
						var collider = new GameObject("Damage Collider");
						collider.transform.parent = script.transform;
						collider.transform.localPosition = Vector3.zero;
						collider.tag = script.Attacks[0].AttackType == AIHelper.AttackTypes.Fire ? "Fire" : "Melee Collider";
						script.Attacks[0].DamageColliders.Add(collider.AddComponent<BoxCollider>());
					}
				},
				
				onRemoveCallback = items =>
				{
					if (script.Attacks[0].DamageColliders.Count == 1)
						return;

					if (!script.gameObject.activeInHierarchy)
					{
						var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

						if(tempEnemy.GetComponent<EnemyController>().Attacks[0].DamageColliders[items.index])
							DestroyImmediate(tempEnemy.GetComponent<EnemyController>().Attacks[0].DamageColliders[items.index].gameObject);
						tempEnemy.GetComponent<EnemyController>().Attacks[0].DamageColliders.Remove(tempEnemy.GetComponent<EnemyController>().Attacks[0].DamageColliders[items.index]);

#if !UNITY_2018_3_OR_NEWER
						PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
						
						DestroyImmediate(tempEnemy);
					}
					else
					{
						if(script.Attacks[0].DamageColliders[items.index])
							DestroyImmediate(script.Attacks[0].DamageColliders[items.index].gameObject);
						
						script.Attacks[0].DamageColliders.Remove(script.Attacks[0].DamageColliders[items.index]);
					}

				},
				
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
					script.Attacks[0].DamageColliders[index] = (BoxCollider) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
						script.Attacks[0].DamageColliders[index], typeof(BoxCollider), true);
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
			if (!Application.isPlaying && script)
			{

				if (script.centralHorizontalAngle > script.peripheralHorizontalAngle)
					script.centralHorizontalAngle = script.peripheralHorizontalAngle - 1;
				
				
				if (!script.projectSettings)
				{
					script.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
					EditorUtility.SetDirty(script.gameObject);
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}
				
				if (!script.DirectionObject && !script.gameObject.activeInHierarchy && script.gameObject.activeSelf)
				{
					var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

					tempEnemy.GetComponent<EnemyController>().DirectionObject = new GameObject("Direction").transform;
					tempEnemy.GetComponent<EnemyController>().DirectionObject.parent = tempEnemy.transform;
					tempEnemy.GetComponent<EnemyController>().DirectionObject.localPosition = Vector3.zero;

#if !UNITY_2018_3_OR_NEWER
					PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
					PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif

					DestroyImmediate(tempEnemy);
				}
				else if (!script.DirectionObject && script.gameObject.activeInHierarchy && script.gameObject.activeSelf)
				{
					script.DirectionObject = new GameObject("Direction").transform;
					script.DirectionObject.parent = script.transform;
					script.DirectionObject.localPosition = Vector3.zero;
				}
				
				
				if (!script.FeetAudioSource && script.gameObject.activeSelf && !script.gameObject.activeInHierarchy)
				{
					var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

					var controller = tempEnemy.GetComponent<EnemyController>();
                    
					controller.FeetAudioSource = new GameObject("FeetAudio").AddComponent<AudioSource>();
					controller.FeetAudioSource.transform.parent = tempEnemy.transform;
					controller.FeetAudioSource.spatialBlend = 1;
					controller.FeetAudioSource.maxDistance = 100;
					controller.FeetAudioSource.minDistance = 1;
					controller.FeetAudioSource.transform.localPosition = Vector3.zero;
                    
#if !UNITY_2018_3_OR_NEWER
					PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
                    PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
                    
					DestroyImmediate(tempEnemy);
				}
				else if (!script.FeetAudioSource && script.gameObject.activeSelf && script.gameObject.activeInHierarchy)
				{
					script.FeetAudioSource = new GameObject("FeetAudio").AddComponent<AudioSource>();
					script.FeetAudioSource.transform.parent = script.transform;
					script.FeetAudioSource.transform.localPosition = Vector3.zero;
					script.FeetAudioSource.spatialBlend = 1;
					script.FeetAudioSource.maxDistance = 100;
					script.FeetAudioSource.minDistance = 1;
				}
				
				if  (!script.StateCanvas && script.UseStates && script.gameObject.activeSelf && !script.gameObject.activeInHierarchy)
				{
					var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

					var enemyScript = tempEnemy.GetComponent<EnemyController>();

					AIHelper.CreateNewStateCanvas(enemyScript, tempEnemy.transform);


#if !UNITY_2018_3_OR_NEWER
					PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
					DestroyImmediate(tempEnemy);
				}
				else if (!script.StateCanvas && script.UseStates && script.gameObject.activeSelf && script.gameObject.activeInHierarchy)
				{
					AIHelper.CreateNewStateCanvas(script, script.transform);
				}
				
				if  (!script.HealthCanvas && script.UseHealthBar && script.gameObject.activeSelf && !script.gameObject.activeInHierarchy)
				{
					var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

					var enemyScript = tempEnemy.GetComponent<EnemyController>();

					AIHelper.CreateNewHealthCanvas(enemyScript, tempEnemy.transform);


#if !UNITY_2018_3_OR_NEWER
					PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
						PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
					DestroyImmediate(tempEnemy);
				}
				else if (!script.HealthCanvas &&  script.gameObject.activeSelf &&  script.UseHealthBar && script.gameObject.activeInHierarchy)
				{
					AIHelper.CreateNewHealthCanvas(script, script.transform);
				}


				if (script.StateCanvas && script.UseStates)
				{
					script.StateCanvas.gameObject.SetActive(true);
				}
				else if(script.StateCanvas && !script.UseStates)
				{
					script.StateCanvas.gameObject.SetActive(false);
				}

				if(!script.trailMaterial)
					script.trailMaterial = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/Trail Mat.mat", typeof(Material)) as Material;


				if (!script.AnimatorController)
				{
					script.AnimatorController = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/_Animator Controllers/AI.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

				}

				if (!script.anim)
				{
					script.anim = script.GetComponent<Animator>();
				}
				else
				{
					if (!script.anim.runtimeAnimatorController)
						script.anim.runtimeAnimatorController = script.AnimatorController;

					if (script.anim.avatar && script.anim.avatar.isHuman)
					{
						script.isHuman = true;
					}
					else
					{
						script.isHuman = false;
//						script.RootMotionMovement = false;
					}

					if (script.gameObject.activeInHierarchy)
					{
						script.newController = new AnimatorOverrideController(script.anim.runtimeAnimatorController);
						script.anim.runtimeAnimatorController = script.newController;
						
						if (script.topInspectorTab != 2)
						{
							script.ClipOverrides = new Helper.AnimationClipOverrides(script.newController.overridesCount);
							script.newController.GetOverrides(script.ClipOverrides);

							if (script.Attacks[0].HandsIdleAnimation)
								script.ClipOverrides["_EnemyHandsIdle"] = script.Attacks[0].HandsIdleAnimation;

							if (script.IdleAnimation)
								script.ClipOverrides["_EnemyIdle"] = script.IdleAnimation;

							script.newController.ApplyOverrides(script.ClipOverrides);

							if (script.anim.avatar && script.anim.avatar.isHuman && script.Attacks[0].AttackType != AIHelper.AttackTypes.Melee)
							{
								script.anim.SetLayerWeight(1, 1);

								if (script.Attacks[0].HandsIdleAnimation)
									script.anim.Play("Idle", 1);
							}
							else if (script.anim.avatar && !script.anim.avatar.isHuman || script.Attacks[0].AttackType == AIHelper.AttackTypes.Melee)
							{
								script.anim.SetLayerWeight(1, 0);
							}

							if (script.IdleAnimation)
								script.anim.Play("Idle", 0);

							if (script.Attacks[0].HandsIdleAnimation || script.IdleAnimation)
								script.anim.Update(Time.deltaTime);
						}
						else
						{
							script.anim.Play("T-Pose", 0);
							script.anim.SetLayerWeight(1, 0);
							script.anim.Update(Time.deltaTime);
						}
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
//			foreach (var bp in script.BodyParts)
//			{
//				bp.gameObject.GetComponent<BodyPartCollider>().gettingDamage = EditorGUILayout.Toggle(bp.gameObject.GetComponent<BodyPartCollider>().gettingDamage);
//			}

			style = new GUIStyle(EditorStyles.helpBox) {richText = true, fontSize = 10};

			EditorGUILayout.Space();

			if (script.isHuman)
			{
				EditorGUILayout.LabelField("<b><color=green>Humanoid Avatar</color></b>", style);
			}
			else
			{
				EditorGUILayout.LabelField("<b><color=blue>Generic Avatar</color></b>", style);
			}
			
			EditorGUILayout.Space();

			if (script.gameObject.activeInHierarchy)
			{
				EditorGUILayout.HelpBox("Set the Waypoints Behaviour in the [Movement & Behaviour] -> [Behaviour] tab.", MessageType.Info);
				EditorGUILayout.Space();
			}

			if (script.isHuman && script.BodyParts.Count > 0 && (!script.BodyParts[0] || script.BodyParts[0] && !script.BodyParts[0].GetComponent<Rigidbody>()))
			{
				EditorGUILayout.HelpBox("Generate Body Colliders in the [Health] tab.", MessageType.Info);
				EditorGUILayout.Space();
			}

			script.topInspectorTab = GUILayout.Toolbar(script.topInspectorTab, new[] {"Movement & Behaviour", "Attacks", "Health"});

			switch (script.topInspectorTab)
			{
				case 0:

					EditorGUILayout.Space();
					
					script.movementBottomInspectorTab = GUILayout.Toolbar(script.movementBottomInspectorTab, new[] { "Vision Area", "Behaviour"});
					switch (script.movementBottomInspectorTab)
					{
						case 0:
							script.movementTopInspectorTab = 3;
							script.currentMovementInspectorTab = 1;
							break;
						
						case 1:
							script.movementTopInspectorTab = 3;
							script.currentMovementInspectorTab = 3;
							break;
					}
					
					script.movementTopInspectorTab = GUILayout.Toolbar(script.movementTopInspectorTab, new[] {"Movement", "Animations & Sounds"});
					switch (script.movementTopInspectorTab)
					{
						case 0:
							script.movementBottomInspectorTab = 3;
							script.currentMovementInspectorTab = 0;
							break;
						
						case 1:
							script.movementBottomInspectorTab = 3;
							script.currentMovementInspectorTab = 2;
							break;
					}

					switch (script.currentMovementInspectorTab)
					{
						case 0:
							EditorGUILayout.BeginVertical("HelpBox");

							if (!script.isHuman)
							{
								EditorGUILayout.HelpBox("This mode is not available for generic avatars, set speed manually.", MessageType.Info);
							}
							
//							EditorGUI.BeginDisabledGroup(!script.isHuman);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("RootMotionMovement"), new GUIContent("Root Motion Movement"));
//							EditorGUI.EndDisabledGroup();

//							if (script.isHuman)
//							{
								EditorGUILayout.Space();
								if (script.RootMotionMovement)
								{
									EditorGUILayout.HelpBox("Root Motion Movement is active:" + "\n\n" +
									                        "The movement and speed are based on Root Motion animations. You can adjust animations speed.", MessageType.Info);
								}
								else
								{
									EditorGUILayout.HelpBox("Root Motion Movement is not active:" + "\n\n" +
									                        "You can adjust enemy's speed manually.", MessageType.Info);
								}
//							}

							if (!script.RootMotionMovement)
							{
								if (!script.isHuman)
									EditorGUILayout.Space();
								
								EditorGUILayout.LabelField("Walk");
								EditorGUILayout.BeginVertical("HelpBox");
								EditorGUILayout.PropertyField(serializedObject.FindProperty("walkForwardSpeed"), new GUIContent("Forward Speed"));
								EditorGUILayout.EndVertical();
								EditorGUILayout.Space();
								EditorGUILayout.LabelField("Run");
								EditorGUILayout.BeginVertical("HelpBox");
								EditorGUILayout.PropertyField(serializedObject.FindProperty("runForwardSpeed"), new GUIContent("Forward Speed"));
								EditorGUILayout.PropertyField(serializedObject.FindProperty("runBackwardSpeed"), new GUIContent("Backward Speed"));

								if (script.allSidesMovement || script.UseCovers)
								{
									EditorGUILayout.PropertyField(serializedObject.FindProperty("runLateralSpeed"), new GUIContent("Lateral Speed"));
								}
								EditorGUILayout.EndVertical();
							}
							else
							{
								EditorGUILayout.PropertyField(serializedObject.FindProperty("SpeedOffset"), new GUIContent("Animation Speed Offset"));

							}
							EditorGUILayout.EndVertical();

							
							if (script.projectSettings)
							{
								EditorGUILayout.Space();
								EditorGUILayout.BeginVertical("helpbox");
								EditorGUILayout.HelpBox("Use the tag to set footsteps sounds for this enemy. " + "\n\n" +
								                        "Step sounds are set in the surface presets (Assets -> USK -> Presets -> Surfaces).", MessageType.Info);

								EditorGUILayout.BeginVertical("HelpBox");
								script.EnemyTag = EditorGUILayout.Popup("Enemy's Tags", script.EnemyTag, script.projectSettings.EnemiesTags.ToArray());

								if (!script.rename)
								{
									if (GUILayout.Button("Rename"))
									{
										script.rename = true;
										script.curName = "";
									}
								}
								else
								{
									EditorGUILayout.BeginVertical("helpbox");
									script.curName = EditorGUILayout.TextField("New name", script.curName);

									EditorGUILayout.BeginHorizontal();

									if (GUILayout.Button("Cancel"))
									{
										script.rename = false;
										script.curName = "";
										script.renameError = false;
									}

									if (GUILayout.Button("Save"))
									{
										if (!script.projectSettings.EnemiesTags.Contains(script.curName))
										{
											script.rename = false;
											script.projectSettings.EnemiesTags[script.EnemyTag] = script.curName;
											script.curName = "";
											script.renameError = false;
										}
										else
										{
											script.renameError = true;
										}
									}

									EditorGUILayout.EndHorizontal();

									if (script.renameError)
										EditorGUILayout.HelpBox("This name already exist.", MessageType.Warning);

									EditorGUILayout.EndVertical();
								}

								EditorGUI.BeginDisabledGroup(script.projectSettings.EnemiesTags.Count <= 1);
								if (!script.delete)
								{
									if (GUILayout.Button("Delete"))
									{
										script.delete = true;
									}
								}
								else
								{
									EditorGUILayout.BeginVertical("helpbox");
									EditorGUILayout.LabelField("Are you sure?");
									EditorGUILayout.BeginHorizontal();


									if (GUILayout.Button("No"))
									{
										script.delete = false;
									}

									if (GUILayout.Button("Yes"))
									{
										script.projectSettings.EnemiesTags.Remove(script.projectSettings.EnemiesTags[script.EnemyTag]);
										script.EnemyTag = script.projectSettings.EnemiesTags.Count - 1;
										script.delete = false;
									}

									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								}

								EditorGUI.EndDisabledGroup();
								EditorGUILayout.EndVertical();
								if (GUILayout.Button("Add new tag"))
								{
									if (!script.projectSettings.EnemiesTags.Contains("Enemy " + script.projectSettings.EnemiesTags.Count))
										script.projectSettings.EnemiesTags.Add("Enemy " + script.projectSettings.EnemiesTags.Count);
									else script.projectSettings.EnemiesTags.Add("Enemy " + Random.Range(10, 100));

									script.EnemyTag = script.projectSettings.EnemiesTags.Count - 1;

								}
								EditorGUILayout.EndVertical();
							}

							break;

						case 1:
							EditorGUILayout.BeginVertical("HelpBox");
							
							EditorGUILayout.HelpBox("Move and rotate this object so that it looks forward and located on the enemy's head.", MessageType.Info);
							EditorGUI.BeginDisabledGroup(true);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("DirectionObject"), new GUIContent("Direction Object"));
							EditorGUI.EndDisabledGroup();
							EditorGUILayout.Space();

//							if (!script.isHuman)
//							{
//								EditorGUILayout.HelpBox("Set the enemy's body part in which the Direction Object will be located during the game." + "\n" +
//								                        "(For example, it's needed so that the see area rotates with the enemy's head while finding)", MessageType.Info);
//								EditorGUILayout.PropertyField(serializedObject.FindProperty("DirectionObjectParent"), new GUIContent("Direction Object Parent"));
//							}
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("DistanceToSee"), new GUIContent("Distance"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("HeightToSee"), new GUIContent("Height"));
							EditorGUILayout.Space();
							EditorGUILayout.HelpBox("When a character falls into this area, the enemy will not immediately notice him.", MessageType.Info);
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("peripheralHorizontalAngle"), new GUIContent(script.UseStates ? "Peripheral Vision Angle" : "Vision Angle"));

							if (script.UseStates)
							{
								EditorGUILayout.Space();
								EditorGUILayout.HelpBox("When a character falls into this area, the enemy will immediately attack him.", MessageType.Info);
								EditorGUILayout.PropertyField(serializedObject.FindProperty("centralHorizontalAngle"), new GUIContent("Central Vision Angle"));
							}

							EditorGUILayout.EndVertical();
							break;

						case 2:
							EditorGUILayout.Space();
							script.animsAndSoundInspectorTab = GUILayout.Toolbar(script.animsAndSoundInspectorTab, new[] {"Animations", "Sounds"});
							switch (script.animsAndSoundInspectorTab)
							{
								case 0:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("IdleAnimation"), new GUIContent("Idle"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("WalkAnimation"), new GUIContent("Walk"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("RunAnimation"), new GUIContent("Run"));
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftRotationAnimation"), new GUIContent("Left Turn"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("RightRotationAnimation"), new GUIContent("Right Turn"));
									
									if (script.allSidesMovement || script.UseCovers)
									{
										EditorGUILayout.Space();
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(0), new GUIContent("Forward"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(1), new GUIContent("Forward Left"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(2), new GUIContent("Forward Right"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(3), new GUIContent("Left"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(4), new GUIContent("Right"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(5), new GUIContent("Backward Left"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(6), new GUIContent("Backward Right"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("AllSidesMovementAnimations").GetArrayElementAtIndex(7), new GUIContent("Backward"));
									}

									EditorGUILayout.Space();
									findAnimations.DoLayoutList();
									EditorGUILayout.Space();
									EditorGUILayout.PropertyField(serializedObject.FindProperty("damageAnimationTimeout"), new GUIContent("Animation Timeout"));
									damageAnimations.DoLayoutList();
									EditorGUILayout.EndVertical();
									break;
								
								case 1:
									EditorGUILayout.Space();
									if (script.UseStates)
									{
										EditorGUILayout.BeginVertical("HelpBox");
										EditorGUILayout.HelpBox("Hm, I saw someone.", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase1"), new GUIContent("Phase 1"));
										EditorGUILayout.Space();
										EditorGUILayout.HelpBox("He left, I'll look for him.", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase2"), new GUIContent("Phase 2"));
										EditorGUILayout.Space();
										EditorGUILayout.HelpBox("I see him! Attack!", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase3"), new GUIContent("Phase 3"));
										EditorGUILayout.Space();
										EditorGUILayout.HelpBox("I'll go find it.", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase4"), new GUIContent("Phase 4"));
										EditorGUILayout.Space();
										EditorGUILayout.HelpBox("I must've imagined it then.", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase5"), new GUIContent("Phase 5"));
										EditorGUILayout.Space();
										EditorGUILayout.EndVertical();
									}
									else
									{
										EditorGUILayout.HelpBox("If you use states there will be more phrases here.", MessageType.Info);
										
										EditorGUILayout.BeginVertical("HelpBox");
										EditorGUILayout.HelpBox("Damn, he left...", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase2"), new GUIContent("Phase 1"));
										EditorGUILayout.Space();
										EditorGUILayout.HelpBox("I see him! Attack!", MessageType.Info);
										EditorGUILayout.PropertyField(serializedObject.FindProperty("phrase3"), new GUIContent("Phase 2"));
										EditorGUILayout.EndVertical();
									}

									break;
							}
							
							
							break;
						
						case 3:
							EditorGUILayout.BeginVertical("HelpBox");
							
							EditorGUI.BeginDisabledGroup(!script.gameObject.activeInHierarchy);
							
							EditorGUILayout.PropertyField(serializedObject.FindProperty("Behaviour"), new GUIContent("Waypoints Behaviour"));
							EditorGUILayout.Space();
							EditorGUI.EndDisabledGroup();

							if (!script.UseStates)
								EditorGUILayout.HelpBox(
									"When [Use States] parameter is not active:" + "\n\n" +
									"◆ If the enemy sees or hears the player, he immediately attacks him." + "\n\n" +
									"◆ If the enemy doesn't see or hear a player, he immediately returns to a waypoint.", MessageType.Info);
							else
								EditorGUILayout.HelpBox(
									"When [Use States] parameter is active:" + "\n\n" +
									"◆ If the enemy sees (with peripheral vision) or hears a player for a while or the player has shot him a few times, " +
									"the enemy's warning state is activated and he will look for him. " + "\n\n" +
									"◆ If the enemy has found the player the attack state will be activated." + "\n\n" +
									"◆ If the enemy doesn't see or hear the player, he looks for him for а while again." +
									" And after that returns to a waypoint." + "\n\n" +
									"◆ If the enemy saw the player with central vision, he will attack immediately.", MessageType.Info);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("UseStates"), new GUIContent("Use States"));
							if (script.UseStates)
							{
								EditorGUILayout.Space();
								EditorGUI.BeginDisabledGroup(true);
								EditorGUILayout.PropertyField(serializedObject.FindProperty("StateCanvas"), new GUIContent("States UI"));
								EditorGUI.EndDisabledGroup();
							}
							EditorGUILayout.EndVertical();
							break;
					}

					break;


				case 1:
					
					EditorGUILayout.Space();
					script.attackInspectorTab = GUILayout.Toolbar(script.attackInspectorTab, new[] {"Attack Area", "Attack Parameters"});

					switch (script.attackInspectorTab)
					{
						case 0:
							
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("HelpBox");
							EditorGUILayout.HelpBox("◆ This value regulates how many percent is the attack zone relative to the See Area." + "\n\n" +
							                        "◆ During the game this enemy runs to the Attack Area and then attacks.", MessageType.Info);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("AttackDistancePercent"), new GUIContent("Distance Percent"));
							EditorGUILayout.EndVertical();
							break;
						
						case 1:

							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical("HelpBox");
							script.Attacks[0].AttackType = (AIHelper.AttackTypes)EditorGUILayout.EnumPopup("Type of Attack", script.Attacks[0].AttackType);

							switch (script.Attacks[0].AttackType)
							{
								case AIHelper.AttackTypes.Bullets:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Weapon"), new GUIContent("Weapon (optional)"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									attackPoints.DoLayoutList();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Damage"), new GUIContent("Damage"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Scatter"), new GUIContent("Scatter of Bullets"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("RateOfAttack"), new GUIContent("Rate of Shoot"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("MuzzleFlash"), new GUIContent("Muzzle Flash (prefab)"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("AttackAudio"), new GUIContent("Audio"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									
									if (script.isHuman)
									{
										EditorGUILayout.BeginVertical("HelpBox");
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsIdleAnimation"), new GUIContent("Idle with weapon animation"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsAttackAnimation"), new GUIContent("Attack with weapon animation"));
										EditorGUILayout.EndVertical();
										EditorGUILayout.Space();
									}

									EditorGUILayout.BeginVertical("HelpBox");
									if (script.UseCovers)
									{
										EditorGUILayout.HelpBox("If the enemy finds suitable cover (close enough to yourself and to the player), he will hide behind it." + "\n\n" +
										                        "To create a cover, add the [Surface] component to an object and active the [Cover] checkbox." + "\n\n" +
										                        "(Also, don't forget to add all-round motion animations in the [Movement] tab)", MessageType.Info);
									}
									else
									{
										EditorGUILayout.HelpBox("If the enemy finds suitable cover (close enough to yourself and to the player), he will hide behind it.", MessageType.Info);
									}

									EditorGUILayout.PropertyField(serializedObject.FindProperty("UseCovers"), new GUIContent("Use Covers"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									
									EditorGUILayout.BeginVertical("HelpBox");

									if (script.allSidesMovement)
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still." + "\n\n" +
										                        "(Don't forget to add all-round motion animations in the [Movement] tab)", MessageType.Info);
									}
									else
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still.", MessageType.Info);
									}

									EditorGUILayout.PropertyField(serializedObject.FindProperty("allSidesMovement"), new GUIContent("All Sides Movement"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("UseReload"), new GUIContent("Use Reload"));
									if (script.Attacks[0].UseReload)
									{
										EditorGUILayout.Space();
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("InventoryAmmo"), new GUIContent("Ammo In Magazine"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsReloadAnimation"), new GUIContent("Reload Animation"));
									}
									EditorGUILayout.EndVertical();
									break;
								case AIHelper.AttackTypes.Rockets:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Weapon"), new GUIContent("Weapon (optional)"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									attackPoints.DoLayoutList();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Damage"), new GUIContent("Damage"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Scatter"), new GUIContent("Scatter of Rockets"));
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("RateOfAttack"), new GUIContent("Rate of Launch"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Rocket"), new GUIContent("Rocket (model)"));
									if (script.Attacks[0].Rocket)
									{
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Explosion"), new GUIContent("Explosion"));
									}
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("AttackAudio"), new GUIContent("Audio"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									if (script.isHuman)
									{
										EditorGUILayout.BeginVertical("HelpBox");
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsIdleAnimation"), new GUIContent("Idle with weapon animation"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsAttackAnimation"), new GUIContent("Attack with weapon animation"));
										EditorGUILayout.EndVertical();
										EditorGUILayout.Space();
									}

									EditorGUILayout.BeginVertical("HelpBox");
									if (script.UseCovers)
									{
										EditorGUILayout.HelpBox("If the enemy finds suitable cover (close enough to yourself and to the player), he will hide behind it." + "\n\n" +
										                        "To create a cover, add the [Surface] component to an object and active the [Cover] checkbox." + "\n\n" +
										                        "(Also, don't forget to add all-round motion animations in the [Movement] tab)", MessageType.Info);
									}
									else
									{
										EditorGUILayout.HelpBox("If the enemy finds suitable cover (close enough to yourself and to the player), he will hide behind it.", MessageType.Info);
									}
									EditorGUILayout.PropertyField(serializedObject.FindProperty("UseCovers"), new GUIContent("Use Covers"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									
									EditorGUILayout.BeginVertical("HelpBox");

									if (script.allSidesMovement)
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still." + "\n\n" +
										                        "(Don't forget to add all-round motion animations in the [Movement] tab)", MessageType.Info);
									}
									else
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still.", MessageType.Info);
									}

									EditorGUILayout.PropertyField(serializedObject.FindProperty("allSidesMovement"), new GUIContent("All Sides Movement"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("UseReload"), new GUIContent("Use Reload"));
									if (script.Attacks[0].UseReload)
									{
										EditorGUILayout.Space();
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("InventoryAmmo"), new GUIContent("Ammo In Magazine"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsReloadAnimation"), new GUIContent("Reload Animation"));
									}
									EditorGUILayout.EndVertical();
									
									break;
								case AIHelper.AttackTypes.Fire:
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Weapon"), new GUIContent("Weapon (optional)"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									attackPoints.DoLayoutList();
									EditorGUILayout.Space();
									damageColliders.DoLayoutList();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Damage"), new GUIContent("Damage"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Fire"), new GUIContent("Fire (prefab)"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("AttackAudio"), new GUIContent("Audio"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									
									if (script.isHuman)
									{
										EditorGUILayout.BeginVertical("HelpBox");
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsIdleAnimation"), new GUIContent("Idle with weapon animation"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsAttackAnimation"), new GUIContent("Attack with weapon animation"));
										EditorGUILayout.EndVertical();
									}

									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");

									if (script.allSidesMovement)
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still." + "\n\n" +
										                        "(Don't forget to add all-round motion animations in the [Movement] tab)", MessageType.Info);
									}
									else
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still.", MessageType.Info);
									}

									EditorGUILayout.PropertyField(serializedObject.FindProperty("allSidesMovement"), new GUIContent("All Sides Movement"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("UseReload"), new GUIContent("Use Reload"));
									if (script.Attacks[0].UseReload)
									{
										EditorGUILayout.Space();
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("InventoryAmmo"), new GUIContent("Ammo In Magazine"));
										EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("HandsReloadAnimation"), new GUIContent("Reload Animation"));
									}
									EditorGUILayout.EndVertical();
									
									break;
								case AIHelper.AttackTypes.Melee:
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Weapon"), new GUIContent("Weapon (optional)"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									damageColliders.DoLayoutList();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("Damage"), new GUIContent("Damage"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("RateOfAttack"), new GUIContent("Rate of Attack"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.HelpBox("Add the [PlayAttackSound] event on attack animations to set the exact playing time of the attack sound.", MessageType.Info);
									EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(0).FindPropertyRelative("AttackAudio"), new GUIContent("Audio"));
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									EditorGUILayout.HelpBox("Add the [MeleeColliders] events with [on] and [off] parameters on attack animations to set the exact activating/deactivating time of damage colliders.", MessageType.Info);
									attackAnimations.DoLayoutList();
									EditorGUILayout.EndVertical();
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical("HelpBox");
									
									if (script.allSidesMovement)
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still." + "\n\n" +
										                        "(Don't forget to add all-round motion animations in the [Movement] tab)", MessageType.Info);
									}
									else
									{
										EditorGUILayout.HelpBox("If this checkbox is active, the enemy will move during the attack. If not, it will stand still.", MessageType.Info);
									}

									EditorGUILayout.PropertyField(serializedObject.FindProperty("allSidesMovement"), new GUIContent("All Sides Movement"));
									EditorGUILayout.EndVertical();
									
									break;
							}
							
							EditorGUILayout.EndVertical();
							break;
					}
					break;
				
				case 2:
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("EnemyHealth"), new GUIContent("Health"));
					EditorGUILayout.EndVertical();
					
					
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("HelpBox");
					if (script.isHuman)
					{
						if (!script.BodyParts[0] || script.BodyParts[0] && !script.BodyParts[0].GetComponent<Rigidbody>())
							EditorGUILayout.HelpBox("For humanoid enemies, a ragdoll is not needed (it will be automatically generated during the game)." + "\n\n" +
							                        "(Don't forget to generate body colliders)", MessageType.Info);
						else EditorGUILayout.HelpBox("For humanoid enemies, a ragdoll is not needed (it will be automatically generated during the game).", MessageType.Info);
					}
					EditorGUI.BeginDisabledGroup(script.isHuman);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ragdoll"), new GUIContent("Ragdoll"));
					EditorGUI.EndDisabledGroup();
						EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("UseHealthBar"), new GUIContent("Use Health Bar"));
					if (script.UseHealthBar)
					{
						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.PropertyField(serializedObject.FindProperty("HealthCanvas"), new GUIContent("Health UI"));
						EditorGUI.EndDisabledGroup();
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					bloodHoles.DoLayoutList();

					if (script.isHuman)
					{
						if (!script.BodyParts[0] || script.BodyParts[0] && !script.BodyParts[0].GetComponent<Rigidbody>())
						{
							EditorGUILayout.Space();
#if !UNITY_2018_3_OR_NEWER
							EditorGUILayout.HelpBox("Place this prefab in a scene to create the Body Colliders.", MessageType.Info);
#else
							EditorGUILayout.HelpBox("Open this prefab to create the Body Colliders.", MessageType.Info);
#endif
							EditorGUI.BeginDisabledGroup(!script.gameObject.activeInHierarchy);
							if (GUILayout.Button("Generate Body Colliders"))
							{
								CreateRagdoll();
							}

							EditorGUI.EndDisabledGroup();
						}
						else if (script.BodyParts[0] && script.BodyParts[0].GetComponent<Rigidbody>())
						{
							EditorGUILayout.LabelField("Damage Multipliers", EditorStyles.boldLabel);
							EditorGUILayout.BeginVertical("HelpBox");
							EditorGUILayout.HelpBox("A character's weapon damage will be multiplied by these values.", MessageType.Info);
							EditorGUILayout.PropertyField(serializedObject.FindProperty("headMultiplier"), new GUIContent("Head"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("bodyMultiplier"), new GUIContent("Body"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("handsMultiplier"), new GUIContent("Hands"));
							EditorGUILayout.PropertyField(serializedObject.FindProperty("legsMultiplier"), new GUIContent("Legs"));
							EditorGUILayout.EndVertical();
						}
					}
					else
					{
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						genericColliders.DoLayoutList();
					}

					EditorGUILayout.EndVertical();
					break;
			}

			serializedObject.ApplyModifiedProperties();
			
//			DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
		
		void CreateRagdoll()
        {
	        if (!script.gameObject.activeInHierarchy)
	        {
		        var tempEnemy = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
		        var enemyScript = tempEnemy.GetComponent<EnemyController>();
		       
		        foreach (var part in enemyScript.BodyParts)
		        {
			        if (part)
			        {
				        foreach (var comp in part.GetComponents<Component>())
				        {
					        if (comp is CharacterJoint || comp is Rigidbody || comp is CapsuleCollider)
					        {
						        DestroyImmediate(comp);
					        }
				        }
			        }
		        }
		        
#if !UNITY_2018_3_OR_NEWER
		        PrefabUtility.ReplacePrefab(tempEnemy, PrefabUtility.GetPrefabParent(tempEnemy), ReplacePrefabOptions.ConnectToPrefab);
#else
				PrefabUtility.SaveAsPrefabAssetAndConnect(tempEnemy, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempEnemy), InteractionMode.AutomatedAction);
#endif
						
		        DestroyImmediate(tempEnemy);
	        }
	        else
	        {
		        foreach (var part in script.BodyParts)
		        {
			        if (part)
			        {
				        foreach (var comp in part.GetComponents<Component>())
				        {
					        if (comp is CharacterJoint || comp is Rigidbody || comp is CapsuleCollider)
					        {
						        DestroyImmediate(comp);
					        }
				        }
			        }
		        }
	        }
	        
	        Helper.CreateRagdoll(script.BodyParts, script.gameObject.GetComponent<Animator>());
        }
	}
}
