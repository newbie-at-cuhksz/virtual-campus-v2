using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    [CustomEditor(typeof(WeaponController))]
    public class WeaponControllerEditor : Editor
    {
        private WeaponController script;

        private InventoryManager manager;

        private ReorderableList tagsList;
        private ReorderableList fpAnimations;
        private ReorderableList fullBodyAnimations;
        
        private ReorderableList fullBodyCrouchAnimations;

        private void Awake()
        {
            script = (WeaponController) target;
        }

        private void OnEnable()
        {

            tagsList = new ReorderableList(serializedObject, serializedObject.FindProperty("IkSlots"), false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), "Tag");

                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 + 15, rect.y, rect.width / 4 - 7,
                                EditorGUIUtility.singleLineHeight), "FP View");
                        
                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), "TP View");
                        
                        EditorGUI.LabelField(new Rect(rect.x +  3 * rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), "TD View");
                },
                
                onAddCallback = items =>
                {
                    script.IkSlots.Add(new WeaponsHelper.IKSlot());
                },
                
                onRemoveCallback = items =>
                {
                    if(script.IkSlots.Count == 1)
                        return;

                    script.IkSlots.Remove(script.IkSlots[items.index]);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.IkSlots[index].currentTag = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width/4, EditorGUIUtility.singleLineHeight),
                        script.IkSlots[index].currentTag, script.projectSettings.CharacterTags.ToArray());

                    script.IkSlots[index].fpsSettingsSlot = EditorGUI.Popup(new Rect(rect.x + rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), script.IkSlots[index].fpsSettingsSlot, script.enumNames.ToArray());
                    
                    script.IkSlots[index].tpsSettingsSlot = EditorGUI.Popup(new Rect(rect.x + rect.width / 2 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), script.IkSlots[index].tpsSettingsSlot, script.enumNames.ToArray());

                    script.IkSlots[index].tdsSettingsSlot = EditorGUI.Popup(new Rect(rect.x + 3 * rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight),  script.IkSlots[index].tdsSettingsSlot, script.enumNames.ToArray());
                }
            };
            
            fpAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack).FindPropertyRelative("WeaponAttacks"), false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Hands Animations (for FP and TD views)");
                },
                
                onAddCallback = items =>
                {
                    script.Attacks[script.currentAttack].WeaponAttacks.Add(null);
                },
                
                onRemoveCallback = items =>
                {
                    if(script.Attacks[script.currentAttack].WeaponAttacks.Count == 1)
                        return;

                    script.Attacks[script.currentAttack].WeaponAttacks.Remove(script.Attacks[script.currentAttack].WeaponAttacks[items.index]);
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.Attacks[script.currentAttack].WeaponAttacks[index] = (AnimationClip)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.Attacks[script.currentAttack].WeaponAttacks[index], typeof(AnimationClip), false);
                }
            };
            
            fullBodyAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack).FindPropertyRelative("WeaponAttacksFullBody"), false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Full-body animations (for TP view)");
                },
                
                onAddCallback = items =>
                {
                    script.Attacks[script.currentAttack].WeaponAttacksFullBody.Add(null);
                },
                
                onRemoveCallback = items =>
                {
                    if(script.Attacks[script.currentAttack].WeaponAttacksFullBody.Count == 1)
                        return;

                    script.Attacks[script.currentAttack].WeaponAttacksFullBody.Remove(script.Attacks[script.currentAttack].WeaponAttacksFullBody[items.index]);
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.Attacks[script.currentAttack].WeaponAttacksFullBody[index] = (AnimationClip)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.Attacks[script.currentAttack].WeaponAttacksFullBody[index], typeof(AnimationClip), false);
                }
            };
            
            fullBodyCrouchAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack).FindPropertyRelative("WeaponAttacksFullBodyCrouch"), false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Full-body crouch animations (for TP view)");
                },
                
                onAddCallback = items =>
                {
                    script.Attacks[script.currentAttack].WeaponAttacksFullBodyCrouch.Add(null);
                },
                
                onRemoveCallback = items =>
                {
                    if(script.Attacks[script.currentAttack].WeaponAttacksFullBodyCrouch.Count == 1)
                        return;

                    script.Attacks[script.currentAttack].WeaponAttacksFullBodyCrouch.Remove(script.Attacks[script.currentAttack].WeaponAttacksFullBodyCrouch[items.index]);
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.Attacks[script.currentAttack].WeaponAttacksFullBodyCrouch[index] = (AnimationClip)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.Attacks[script.currentAttack].WeaponAttacksFullBodyCrouch[index], typeof(AnimationClip), false);
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
                if (!script.projectSettings)
                {
                    script.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                
                if (ActiveEditorTracker.sharedTracker.isLocked)
                    ActiveEditorTracker.sharedTracker.isLocked = false;
                
                if (script && !script.inspectorCanvas && !script.gameObject.activeInHierarchy && script.gameObject.activeSelf)
                {
                    var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

                    tempWeapon.GetComponent<WeaponController>().inspectorCanvas = Helper.NewCanvas("Canvas", new Vector2(1920, 1080), tempWeapon.transform);
                    
                    var parts = CharacterHelper.CreateCrosshair(tempWeapon.GetComponent<WeaponController>().inspectorCanvas.transform);

                    tempWeapon.GetComponent<WeaponController>().upPart = parts[1].GetComponent<Image>();
                    tempWeapon.GetComponent<WeaponController>().downPart = parts[2].GetComponent<Image>();
                    tempWeapon.GetComponent<WeaponController>().rightPart = parts[3].GetComponent<Image>();
                    tempWeapon.GetComponent<WeaponController>().leftPart = parts[4].GetComponent<Image>();
                    tempWeapon.GetComponent<WeaponController>().middlePart = parts[5].GetComponent<Image>();
                    
#if !UNITY_2018_3_OR_NEWER
                    PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                    PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                    
                    DestroyImmediate(tempWeapon);
                }
                else
                {
                    if (script.inspectorTabTop == 0 && script.Attacks[script.currentAttack].inspectorTab == 2 && script.inspectorCanvas)
                    {
                        script.inspectorCanvas.gameObject.SetActive(true);
                        
                        script.upPart.GetComponent<RectTransform>().sizeDelta = new Vector2(script.Attacks[script.currentAttack].CrosshairSize, script.Attacks[script.currentAttack].CrosshairSize);
                        script.downPart.GetComponent<RectTransform>().sizeDelta = new Vector2(script.Attacks[script.currentAttack].CrosshairSize, script.Attacks[script.currentAttack].CrosshairSize);
                        script.leftPart.GetComponent<RectTransform>().sizeDelta = new Vector2(script.Attacks[script.currentAttack].CrosshairSize, script.Attacks[script.currentAttack].CrosshairSize);
                        script.rightPart.GetComponent<RectTransform>().sizeDelta = new Vector2(script.Attacks[script.currentAttack].CrosshairSize, script.Attacks[script.currentAttack].CrosshairSize);
                        script.middlePart.GetComponent<RectTransform>().sizeDelta = new Vector2(script.Attacks[script.currentAttack].CrosshairSize, script.Attacks[script.currentAttack].CrosshairSize);

                        script.upPart.GetComponent<RectTransform>().anchoredPosition = script.Attacks[script.currentAttack].crosshairPartsPositions[1];
                        script.downPart.GetComponent<RectTransform>().anchoredPosition = script.Attacks[script.currentAttack].crosshairPartsPositions[2];
                        script.rightPart.GetComponent<RectTransform>().anchoredPosition = script.Attacks[script.currentAttack].crosshairPartsPositions[3];
                        script.leftPart.GetComponent<RectTransform>().anchoredPosition = script.Attacks[script.currentAttack].crosshairPartsPositions[4];

                        script.upPart.sprite = script.Attacks[script.currentAttack].UpPart ? script.Attacks[script.currentAttack].UpPart : null;

                        script.downPart.sprite = script.Attacks[script.currentAttack].DownPart ? script.Attacks[script.currentAttack].DownPart : null;

                        script.leftPart.sprite = script.Attacks[script.currentAttack].LeftPart ? script.Attacks[script.currentAttack].LeftPart : null;

                        script.rightPart.sprite = script.Attacks[script.currentAttack].RightPart ? script.Attacks[script.currentAttack].RightPart : null;
                        
                        script.middlePart.sprite = script.Attacks[script.currentAttack].MiddlePart ? script.Attacks[script.currentAttack].MiddlePart : null;
                        
                        switch (script.Attacks[script.currentAttack].sightType)
                        {
                            case WeaponsHelper.CrosshairType.OnePart:
                                script.middlePart.gameObject.SetActive(true);
                                script.rightPart.gameObject.SetActive(false);
                                script.leftPart.gameObject.SetActive(false);
                                script.upPart.gameObject.SetActive(false);
                                script.downPart.gameObject.SetActive(false);
									
                                break;
                            case WeaponsHelper.CrosshairType.TwoParts:

                                script.middlePart.gameObject.SetActive(script.middlePart.sprite);

                                script.rightPart.gameObject.SetActive(true);
                                script.leftPart.gameObject.SetActive(true);
									
                                script.upPart.gameObject.SetActive(false);
                                script.downPart.gameObject.SetActive(false);
									
                                break;
                            case WeaponsHelper.CrosshairType.FourParts:
									
                                script.middlePart.gameObject.SetActive(script.middlePart.sprite);

                                script.rightPart.gameObject.SetActive(true);
                                script.leftPart.gameObject.SetActive(true);
									
                                script.upPart.gameObject.SetActive(true);
                                script.downPart.gameObject.SetActive(true);
									
                                break;
                        }
                    }
                    else if(script.inspectorCanvas && (script.inspectorTabTop != 0 || script.Attacks[script.currentAttack].inspectorTab != 2))
                    {
                        script.inspectorCanvas.gameObject.SetActive(false);
                    }
                }
                

                if (!script) return;

//                if (script.Attacks[script.currentAttack].AttackEffects.Length > 0)
//                {
//                    foreach (var effect in script.Attacks[script.currentAttack].AttackEffects)
//                    {
//                        if (effect && effect.emission.enabled)
//                        {
//                            var effectEmission = effect.emission;
//                            effectEmission.enabled = false;
//                        }
//                    }
//                }

                if (!script.gameObject.GetComponent<Rigidbody>())
                    script.gameObject.AddComponent<Rigidbody>();

                if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                {
                    if (!script.gameObject.GetComponent<BoxCollider>())
                        script.gameObject.AddComponent<BoxCollider>();
                    else script.gameObject.GetComponent<BoxCollider>().enabled = true;

                    if (script.gameObject.GetComponent<CapsuleCollider>())
                        script.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                }
                else
                {
                    if (!script.gameObject.GetComponent<CapsuleCollider>())
                        script.gameObject.AddComponent<CapsuleCollider>();
                    else script.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    if (script.gameObject.GetComponent<BoxCollider>())
                        script.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
                
                
                script.PickUpWeapon = script.gameObject.activeInHierarchy;

                if (!script.PickUpWeapon && !script.gameObject.activeInHierarchy && script.gameObject.activeSelf)
                {
                    if (script.gameObject.GetComponent<PickUp>())
                    {
                        var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                        DestroyImmediate(tempWeapon.GetComponent<PickUp>());
#if !UNITY_2018_3_OR_NEWER
                        PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                        PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                        DestroyImmediate(tempWeapon);
                    }

                    script.enabled = false;
                }
                else 
                {
                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        if (!script.gameObject.GetComponent<PickUp>())
                            script.gameObject.AddComponent<PickUp>();

                        script.gameObject.GetComponent<PickUp>().PickUpType = PickUp.TypeOfPickUp.Weapon;
                        if(script.Attacks.Any(attack => attack.AttackCollider))
                        {
                            var _attacks = script.Attacks.FindAll(attack => attack.AttackCollider);
                            foreach (var _attack in _attacks)
                            {
                                if(_attack.AttackCollider.enabled)
                                    _attack.AttackCollider.enabled = false;
                            }
                        }
                    }

                    script.enabled = false;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(script.PickUpWeapon 
                ? "This weapon is a pick-up item. Adjust the [Collider size] in the [PickUp script]."
                : "To use this weapon as a pick-up item, just place it in a scene.", MessageType.Info);

            EditorGUILayout.Space();

                script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop,
                    new [] {"Attacks", "Weapon Settings", "Aim Settings"});


                switch (script.inspectorTabTop)
                {
                    case 0:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Attacks";
                        break;
                    case 1:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Weapon Settings";
                        break;
                    case 2:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Aim Settings";
                        break;
                }

                script.inspectorTabBottom = GUILayout.Toolbar(script.inspectorTabBottom,
                new [] {"Animations", "Sounds"});

            switch (script.inspectorTabBottom)
            {
                case 0:
                    script.inspectorTabTop = 3;
                    script.currentTab = "Animations";
                    break;
                case 1:
                    script.inspectorTabTop = 3;
                    script.currentTab = "Sounds";
                    break;
            }


            switch (script.currentTab)
            {
                case "Attacks":

                    EditorGUILayout.Space();
                    

                        if (script.Attacks.Count > 0)
                        {
                            EditorGUILayout.BeginVertical("box");

                            script.currentAttack = EditorGUILayout.Popup("Weapon attacks", script.currentAttack, script.attacksNames.ToArray());
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
                                EditorGUILayout.BeginVertical("box");
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
                                    if (!script.attacksNames.Contains(script.curName))
                                    {
                                        script.rename = false;
                                        script.attacksNames[script.currentAttack] = script.curName;
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

                            EditorGUI.BeginDisabledGroup(script.Attacks.Count <= 1);
                            if (!script.delete)
                            {
                                if (GUILayout.Button("Delete"))
                                {
                                    script.delete = true;
                                }
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.LabelField("Are you sure?");
                                EditorGUILayout.BeginHorizontal();


                                if (GUILayout.Button("No"))
                                {
                                    script.delete = false;
                                }

                                if (GUILayout.Button("Yes"))
                                {
                                    script.attacksNames.Remove(script.attacksNames[script.currentAttack]);
                                    script.Attacks.Remove(script.Attacks[script.currentAttack]);
                                    script.currentAttack = script.Attacks.Count - 1;
                                    script.delete = false;
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                            }

                            EditorGUI.EndDisabledGroup();
                            EditorGUILayout.EndVertical();
                        }


                    EditorGUI.BeginDisabledGroup(script.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade));

                    if (GUILayout.Button("Add a new one"))
                    {
                        script.Attacks.Add(new WeaponsHelper.Attack());
                        script.Attacks[script.Attacks.Count - 1].BulletsSettings.Add(new WeaponsHelper.BulletsSettings());
                        script.Attacks[script.Attacks.Count - 1].BulletsSettings.Add(new WeaponsHelper.BulletsSettings());

                        if (!script.attacksNames.Contains("Attack " + script.Attacks.Count))
                            script.attacksNames.Add("Attack " + script.Attacks.Count);
                        else script.attacksNames.Add("Attack " + Random.Range(10, 100));

                        script.currentAttack = script.Attacks.Count - 1;

                        break;
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (script.Attacks.Count > 0)
                    {
                        var _attack = script.Attacks[script.currentAttack];
                        var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack);

                        GUILayout.BeginVertical("Attack: " + script.attacksNames[script.currentAttack], "window");
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack).FindPropertyRelative("AttackType"), new GUIContent("Type"));

                        EditorGUILayout.Space();
                        

                        script.Attacks[script.currentAttack].inspectorTab = GUILayout.Toolbar(script.Attacks[script.currentAttack].inspectorTab, new[] {"Parameters", "Effects", "Crosshair"});
                        
                        EditorGUILayout.Space();
                        switch (script.Attacks[script.currentAttack].inspectorTab)
                        {
                            case 1:
                                switch (_attack.AttackType)
                                {
                                    case WeaponsHelper.TypeOfAttack.Rockets:

                                        EditorGUILayout.Space();
                                        if (script.Attacks[script.currentAttack].Magazine)
                                        {
                                            EditorGUILayout.BeginVertical("HelpBox");
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Explosion"), new GUIContent("Explosion"));
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackEffects"), new GUIContent("Effects"), true);
                                            EditorGUILayout.EndVertical();
                                        }

                                        break;
                                    case WeaponsHelper.TypeOfAttack.Flame:
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackEffects"), new GUIContent("Effects"), true);
                                        EditorGUILayout.EndVertical();
                                        break;
                                    case WeaponsHelper.TypeOfAttack.Melee:
                                    {

                                        break;
                                    }
                                    case WeaponsHelper.TypeOfAttack.Bullets:
                                    case WeaponsHelper.TypeOfAttack.Minigun:
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("visualizeBullets"), new GUIContent("Bullet trail"));
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("MuzzleFlash"), new GUIContent("Muzzle flash (prefab)"));
                                        EditorGUILayout.EndVertical();

                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Shell"), new GUIContent("Shell (prefab)"));
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ShellPoint"), new GUIContent("Shells spawn point"));
                                        CheckPoint(_attack, "shell");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("SpawnShells"), new GUIContent("Spawn Shells Immediately"));
                                        EditorGUILayout.HelpBox("If this checkbox is active, the shells appear immediately when the character shoots." + "\n" +
                                                               "If you need a delay in the appearance of shells (for example, for a shotgun or rifle), add an event called [SpawnShell] to the shot animation.", MessageType.Info);
                                        EditorGUILayout.EndVertical();

                                        break;
                                    case WeaponsHelper.TypeOfAttack.Grenade:

                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Explosion"), new GUIContent("Explosion"));
                                        EditorGUILayout.EndVertical();
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.HelpBox("If active, the explosion will push objects with the [Rigidbody] component.", MessageType.Info);
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ApplyForce"), new GUIContent("Apply Force"));
                                        
                                        EditorGUILayout.EndVertical();
                                        
                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.HelpBox("Use it, if you need a Flash Grenade.", MessageType.Info);
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("FlashExplosion"), new GUIContent("Flash Effect"));
                                        
                                        EditorGUILayout.EndVertical();
                                        //EditorGUILayout.EndVertical();

                                        EditorGUILayout.Space();
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.HelpBox("Use it, if you need a Toxic Babe.", MessageType.Info);
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("isToxin"), new GUIContent("Toxic"));

                                        EditorGUILayout.EndVertical();
                                        EditorGUILayout.EndVertical();

                                        break;
                                    case WeaponsHelper.TypeOfAttack.GrenadeLauncher:


                                        EditorGUILayout.BeginVertical("HelpBox");
                                        script.Attacks[script.currentAttack].showTrajectory = EditorGUILayout.ToggleLeft("Show Grenade Trajectory", script.Attacks[script.currentAttack].showTrajectory);

                                        script.Attacks[script.currentAttack].ExplodeWhenTouchGround = EditorGUILayout.ToggleLeft("Explode when touch an object", script.Attacks[script.currentAttack].ExplodeWhenTouchGround);

                                        if (!script.Attacks[script.currentAttack].ExplodeWhenTouchGround)
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("GrenadeExplosionTime"), new GUIContent("Explosion Time"));
                                        EditorGUILayout.EndVertical();
                                        
                                        EditorGUILayout.Space();

                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Explosion"), new GUIContent("Explosion"));
                                        EditorGUILayout.EndVertical();
                                        EditorGUILayout.Space();
                                        
                                        EditorGUILayout.BeginVertical("HelpBox");
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("FlySpeed"), new GUIContent("Speed"));
                                        EditorGUILayout.EndVertical();

                                        break;

                                }

                                break;

                            case 0:

                                if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Melee && _attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackSpawnPoint"), new GUIContent("Attack Point"));
                                    CheckPoint(_attack, "attack");
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }

                                if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Flame)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackCollider"), new GUIContent("Fire collider"));
                                    CheckCollider(_attack, "fire");
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }

                                if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Melee)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackCollider"), new GUIContent("Attack collider"));
                                    CheckCollider(_attack, "knife");
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }

                                if(_attack.AttackType == WeaponsHelper.TypeOfAttack.Rockets)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Magazine"), new GUIContent("Rocket"));
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }
                                
                                if(_attack.AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Magazine"), new GUIContent("Grenade"));
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }

                                if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.HelpBox("Set the minigun barrel and rotation axis.", MessageType.Info);
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Barrel"), new GUIContent("Barrel"));
                                    script.Attacks[script.currentAttack].BarrelRotationAxes = (Helper.RotationAxes)EditorGUILayout.EnumPopup("Rotation Axis", script.Attacks[script.currentAttack].BarrelRotationAxes);
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }
                                
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("◆ TP and FP views:" + "\n" +
                                                        "When you aim the crosshair at the enemy and the character is closer than the set distance the weapon will attack." + "\n\n" +
                                                        "◆ TD view (mobile):" + "\n" +
                                                        "While you're using the camera joystick, the character is shooting."
                                                        , MessageType.Info);
                                script.Attacks[script.currentAttack].autoAttack = EditorGUILayout.ToggleLeft("Auto Attack", script.Attacks[script.currentAttack].autoAttack);
                                
                                
                                if (script.Attacks[script.currentAttack].autoAttack)
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("attackDistance"), new GUIContent("Distance"));

                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();

                                if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                                {
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    script.Attacks[script.currentAttack].ExplodeWhenTouchGround = EditorGUILayout.ToggleLeft("Explode when touch an object", script.Attacks[script.currentAttack].ExplodeWhenTouchGround);
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }

                                EditorGUILayout.BeginVertical("HelpBox");
                                if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets)
                                {
                                    EditorGUILayout.HelpBox("You can use single, automatic or both shooting types." + "\n" +
                                                            "In the game you can switch between them by pressing the [" + script.projectSettings.KeyBoardCodes[19] + "] button.", MessageType.Info);

                                    script.bulletTypeInspectorTab = EditorGUILayout.Popup("Shooting type", script.bulletTypeInspectorTab, new[] {"Single", "Auto"});
                                    EditorGUILayout.Space();
                                    
                                    switch (script.bulletTypeInspectorTab)
                                    {
                                        case 0:
                                            
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                .FindPropertyRelative("Active"), new GUIContent("Active"));

                                            if (script.Attacks[script.currentAttack].BulletsSettings[0].Active)
                                            {

                                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                    .FindPropertyRelative("weapon_damage"), new GUIContent("Damage of attack"));

                                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                    .FindPropertyRelative("RateOfShoot"), new GUIContent("Rate of Shoot"));

                                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                    .FindPropertyRelative("ScatterOfBullets"), new GUIContent("Scatter of bullets"));
                                                
                                                EditorGUILayout.Space();
                                                script.isShotgun = EditorGUILayout.ToggleLeft("Is Shotgun", script.isShotgun);
                                            }

                                            break;

                                        case 1:

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                .FindPropertyRelative("Active"), new GUIContent("Active"));

                                            if (script.Attacks[script.currentAttack].BulletsSettings[1].Active)
                                            {

                                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                    .FindPropertyRelative("weapon_damage"), new GUIContent("Damage"));

                                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                    .FindPropertyRelative("RateOfShoot"), new GUIContent("Rate of Shoot"));

                                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                    .FindPropertyRelative("ScatterOfBullets"), new GUIContent("Scatter of bullets"));
                                            }

                                            break;
                                        
                                    }
                                }
                                else if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                                {
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                        .FindPropertyRelative("weapon_damage"), new GUIContent("Damage"));

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                        .FindPropertyRelative("RateOfShoot"), new GUIContent("Rate of Shoot"));

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                        .FindPropertyRelative("ScatterOfBullets"), new GUIContent("Scatter of bullets"));
                                }
                                else if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                                {
                                    //script.Attacks[script.currentAttack].StickToObject = EditorGUILayout.ToggleLeft("Stick to an object", script.Attacks[script.currentAttack].StickToObject);

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("FlySpeed"), new GUIContent("Speed"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("GrenadeExplosionTime"), new GUIContent("Time before explosion"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("weapon_damage"), new GUIContent("Damage"));
                               
                                }
                                else
                                {

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("weapon_damage"),
                                        _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame ? new GUIContent("Damage (per 1 sec)") : new GUIContent("Damage"));

                                    if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Flame) //&& _attack.AttackType != WeaponsHelper.TypeOfAttack.Melee)
                                    {
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("RateOfAttack"), new GUIContent("Rate of Attack"));

                                        if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Melee && _attack.AttackType != WeaponsHelper.TypeOfAttack.GrenadeLauncher)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ScatterOfBullets"),
                                                _attack.AttackType != WeaponsHelper.TypeOfAttack.Rockets
                                                    ? new GUIContent("Scatter of bullets")
                                                    : new GUIContent("Scatter of rockets"));
                                        }

                                        if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Rockets)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("FlySpeed"), new GUIContent("Speed"));
                                        }
                                    }
                                }
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();
                                
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("This parameter controls how many times the radius of the noise increases during the attack." + "\n" +
                                                        "If you don't use enemies, you don't need to use this variable", MessageType.Info);
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackNoiseRadiusMultiplier"), new GUIContent("Noise Multiplayer"));
                                EditorGUILayout.EndVertical();


                                if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Melee)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("maxAmmo"), new GUIContent("Count of ammo in magazine"));


                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("inventoryAmmo"),
                                        _attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade ? new GUIContent("Count in inventory") : new GUIContent("Count of ammo in inventory"));

                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                    
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.HelpBox("Write the same type in the PickUp script for ammo.", MessageType.Info);
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AmmoType"), new GUIContent("Ammo Name"));
                                    EditorGUILayout.EndVertical();
                                    
                                }

                                if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Melee && _attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade && _attack.AttackType != WeaponsHelper.TypeOfAttack.Rockets && _attack.AttackType != WeaponsHelper.TypeOfAttack.GrenadeLauncher)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.HelpBox("Model for synchronization with animation." + "\n" +
                                                            "Read more about that in the Documentation (section 'Weapons').", MessageType.Info);
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Magazine"), new GUIContent("Magazine (optional)"));
                                    EditorGUILayout.EndVertical();
                                }
                               

                                break;

                            case 2:

                                EditorGUILayout.BeginVertical("HelpBox");
                                _attack.sightType = (WeaponsHelper.CrosshairType)EditorGUILayout.EnumPopup("Type", _attack.sightType);
                                EditorGUILayout.Space();
                                switch (_attack.sightType)
                                {
                                    case WeaponsHelper.CrosshairType.OnePart:
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("MiddlePart"), new GUIContent("Middle Part"));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("CrosshairSize"), new GUIContent("Scale"));
                                        break;
                                    case WeaponsHelper.CrosshairType.TwoParts:

                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("LeftPart"));
                                        if (script.showCrosshairPositions)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("crosshairPartsPositions").GetArrayElementAtIndex(4), new GUIContent("Position"));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("RightPart"));
                                        if (script.showCrosshairPositions)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("crosshairPartsPositions").GetArrayElementAtIndex(3), new GUIContent("Position"));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("MiddlePart"), new GUIContent("Middle Part (optional)"));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("CrosshairSize"), new GUIContent("Scale"));

                                        break;
                                    case WeaponsHelper.CrosshairType.FourParts:

                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("UpPart"));
                                        if (script.showCrosshairPositions)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("crosshairPartsPositions").GetArrayElementAtIndex(1), new GUIContent("Position"));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("DownPart"));
                                        if (script.showCrosshairPositions)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("crosshairPartsPositions").GetArrayElementAtIndex(2), new GUIContent("Position"));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("LeftPart"));
                                        if (script.showCrosshairPositions)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("crosshairPartsPositions").GetArrayElementAtIndex(4), new GUIContent("Position"));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("RightPart"));
                                        if (script.showCrosshairPositions)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("crosshairPartsPositions").GetArrayElementAtIndex(3), new GUIContent("Position"));
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("MiddlePart"), new GUIContent("Middle Part (optional)"));
                                        EditorGUILayout.Space();
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("CrosshairSize"), new GUIContent("Scale"));

                                        break;
                                }

                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();

                                script.showCrosshairPositions = GUILayout.Toggle(script.showCrosshairPositions, "Adjust Positions", "Button");

                                break;

                        }
                        
                        EditorGUILayout.EndVertical();
                        
                        EditorGUILayout.Space();
                    }
                    

                    break;

                case "Weapon Settings":
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("HelpBox");
                    switch (script.Weight)
                    {
                        case WeaponsHelper.WeaponWeight.Light:
                            EditorGUILayout.HelpBox("The heavier weapon, the slower a character moves." + "\n" +
                                                    "Light weapon like a pistol, knife, grenade, etc.", MessageType.Info);
                            break;

                        case WeaponsHelper.WeaponWeight.Medium:
                            EditorGUILayout.HelpBox("The heavier weapon, the slower a character moves." + "\n" +
                                                    "Medium weapon like a rifle, grenade launcher, etc.", MessageType.Info);
                            break;
                        case WeaponsHelper.WeaponWeight.Heavy:
                            EditorGUILayout.HelpBox("The heavier weapon, the slower a character moves." + "\n" +
                                                    "Heavy weapon like a minigun, flamethrower, etc.", MessageType.Info);
                            break;
                   
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Weight"), new GUIContent("Weapon Weight"));

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.HelpBox("This image will be displayed in the inventory.", MessageType.Info);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponImage"), new GUIContent("Weapon Image"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    if (script.Attacks[script.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee && script.Attacks[script.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                    {
                        EditorGUILayout.BeginVertical("HelpBox");
                        EditorGUILayout.HelpBox("When the ammo runs out the character reloads the weapon.", MessageType.Info);
                        script.autoReload = EditorGUILayout.ToggleLeft("Auto Reload", script.autoReload);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }
                    
                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.HelpBox("When a character comes close to an object, he removes his hands " + "\n\n" +
                                            "(Hand position is adjusted using IK [Tools -> USK -> Adjust]).", MessageType.Info);
                    script.enableObjectDetectionMode = EditorGUILayout.ToggleLeft("Object Detection", script.enableObjectDetectionMode);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.BeginVertical("HelpBox");
                   
                    if (script.numberOfUsedHands == 2)
                    {
                        EditorGUILayout.HelpBox("In this mode, the left hand depends on the right. " + "\n\n" +
                                                "Use this if the character holds this weapon in two hands. In this case, the IK system will be more accurate.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("In this mode, the left and right hands are independent of each other." + "\n\n" +
                                                "Use this if the character holds this weapon in one hand (grenades, knives, etc.)", MessageType.Info);
                    }

                    var index = script.numberOfUsedHands - 1;
                    index = EditorGUILayout.Popup("Number of Used Hands", index, new []{"One-Handed", "Two-Handed"});
                    script.numberOfUsedHands = index + 1;
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();


                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.HelpBox("For different characters you can use different IK settings, " +
                                            "just set the necessary settings for each tag." + "\n\n" +
                                            "You can change the character's tag in the [Controller] script." + "\n\n" +
                                            "Also for each type of camera, you can set an own slot.", MessageType.Info);
                    tagsList.DoLayoutList();

                    EditorGUILayout.EndVertical();

                    break;

                case "Aim Settings":
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("HelpBox");

                    script.activeAimMode = EditorGUILayout.ToggleLeft("Active Aim Mode", script.activeAimMode, EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();

                    script.aimInspectorTabIndex = GUILayout.Toolbar(script.aimInspectorTabIndex, new[] {"First Person", "Third Person", "Top Down"});
                    EditorGUILayout.Space();

                    switch (script.aimInspectorTabIndex)
                    {
                        case 0:
                            EditorGUI.BeginDisabledGroup(!script.activeAimMode);

                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("The speed at which the character will aim.", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("setAimSpeed"), new GUIContent("Aim Speed"));
                            EditorGUILayout.EndVertical();
                            
                          
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("Use this to add a scope model to the weapon.", MessageType.Info);
                                EditorGUILayout.Space();
                                script.useScope = EditorGUILayout.ToggleLeft("Active", script.useScope);

                                if (script.useScope)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ScopeScreen"), new GUIContent("Scope Screen"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scopeDepth"), new GUIContent("Depth"));
                                }

                                EditorGUILayout.EndVertical();
                                
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("If this option is active, the weapon will use the texture as a sight.", MessageType.Info);

                                script.useAimTexture = EditorGUILayout.ToggleLeft("Active", script.useAimTexture);

                                if (script.useAimTexture)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AimCrosshairTexture"), new GUIContent("Texture"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("aimTextureDepth"), new GUIContent("Depth"));
                                }

                                EditorGUILayout.EndVertical();
                            EditorGUI.EndDisabledGroup();
                            
                            break;

                        case 1:
//                            EditorGUILayout.BeginVertical("HelpBox");
//                            script.activeAimTP = EditorGUILayout.ToggleLeft("Active Aim mode", script.activeAimTP, EditorStyles.boldLabel);
//                            EditorGUILayout.EndVertical();
//                            EditorGUILayout.Space();
//                            EditorGUILayout.Space();
                            EditorGUI.BeginDisabledGroup(!script.activeAimMode);
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("The speed at which the character will aim.", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("setAimSpeed"), new GUIContent("Aim Speed"));
                            EditorGUILayout.EndVertical();
                            
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("The camera will switch to first-person view when aiming", MessageType.Info);

                                script.switchToFpCamera = EditorGUILayout.ToggleLeft("Switch to FP View", script.switchToFpCamera);
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("When you press the [Attack] button the character will first aim and then attack.", MessageType.Info);
                                script.aimForAttack = EditorGUILayout.ToggleLeft("Aim before Attack", script.aimForAttack);
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("Use this to add a scope model to the weapon.", MessageType.Info);

                                script.useScope = EditorGUILayout.ToggleLeft("Active", script.useScope);

                                if (script.useScope)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ScopeScreen"), new GUIContent("Scope Screen"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scopeDepth"), new GUIContent("Depth"));
                                }

                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Texture Aiming", EditorStyles.boldLabel);
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.HelpBox("If this option is active, the weapon will use the texture as a sight.", MessageType.Info);

                                script.useAimTexture = EditorGUILayout.ToggleLeft("Active", script.useAimTexture);

                                if (script.useAimTexture)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AimCrosshairTexture"), new GUIContent("Texture"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("aimTextureDepth"), new GUIContent("Depth"));
                                }

                                EditorGUILayout.EndVertical();
                            EditorGUI.EndDisabledGroup();

                            break;
                        
                        case 2:
                            EditorGUI.BeginDisabledGroup(!script.activeAimMode);
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("The speed at which the character will aim.", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("setAimSpeed"), new GUIContent("Aim Speed"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("When you press the [Attack] button the character will first aim and then attack.", MessageType.Info);
                            script.aimForAttack = EditorGUILayout.ToggleLeft("Aim before Attack", script.aimForAttack);
                            EditorGUILayout.EndVertical();
                            EditorGUI.EndDisabledGroup();
                            break;
                    }
                    if (script.activeAimMode)
                    {
                        EditorGUILayout.Space();
                    }
//                    }

                    break;

                case "Animations":
                    EditorGUILayout.Space();
                    if (script.Attacks.Count > 0)
                    {
                        EditorGUILayout.BeginVertical("HelpBox");

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.WeaponIdle"), new GUIContent("Idle"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.WeaponWalk"), new GUIContent("Walk"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.WeaponRun"), new GUIContent("Run"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.TakeWeapon"), new GUIContent("Take from Inventory"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();

                        for (var i = 0; i < script.Attacks.Count; i++)
                        {
                            var attack = script.Attacks[i];
                            var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(i);

                            EditorGUILayout.LabelField("Attacks: " + script.attacksNames[i], EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("HelpBox");
                            switch (attack.AttackType)
                            {
                                case WeaponsHelper.TypeOfAttack.Bullets:
                                {
                                    if(attack.BulletsSettings[1].Active)
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAutoShoot"), new GUIContent("Auto Shoot"));
                                
                                    if(attack.BulletsSettings[0].Active)
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacks").GetArrayElementAtIndex(0), new GUIContent("Single Shoot"));
                                    break;
                                }
                                case WeaponsHelper.TypeOfAttack.Grenade:
                                    
                                    EditorGUILayout.HelpBox("Add the [ThrowGrenade] event on throw animations to set the exact time of the grenade throw, otherwise, the grenade will be launched at the end of the animation.", MessageType.Info);

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacks").GetArrayElementAtIndex(0), new GUIContent("Throw (FP)"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacksFullBody").GetArrayElementAtIndex(0), new GUIContent("Throw (Full-body)"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacksFullBodyCrouch").GetArrayElementAtIndex(0), new GUIContent("Throw (Crouch Full-body)"));

                                    break;
                                case WeaponsHelper.TypeOfAttack.Melee:
                                    
                                    fpAnimations.DoLayoutList();
                                    EditorGUILayout.Space(); 
                                    EditorGUILayout.Space(); 
                                    EditorGUILayout.Space(); 
                                    fullBodyAnimations.DoLayoutList();
                                    EditorGUILayout.Space(); 
                                    fullBodyCrouchAnimations.DoLayoutList();
                                    
                                    break;
                                case WeaponsHelper.TypeOfAttack.Rockets:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacks").GetArrayElementAtIndex(0), new GUIContent("Attack"));
                                    break;
                                case WeaponsHelper.TypeOfAttack.GrenadeLauncher:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacks").GetArrayElementAtIndex(0), new GUIContent("Attack"));
                                    break;
                                case WeaponsHelper.TypeOfAttack.Flame:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacks").GetArrayElementAtIndex(0), new GUIContent("Attack"));
                                    break;
                                case WeaponsHelper.TypeOfAttack.Minigun:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttacks").GetArrayElementAtIndex(0), new GUIContent("Auto Shoot"));
                                    break;
                            }


                            if (attack.AttackType != WeaponsHelper.TypeOfAttack.Melee && attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                            {
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponReload"), new GUIContent("Reload"));
                            }

                            EditorGUILayout.EndVertical();
                            if (i < script.Attacks.Count - 1)
                                EditorGUILayout.Space();
                        }
//                        }
//                        else
//                        {
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeThrow_FPS"), new GUIContent("Throw (FP)"));
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeThrow_TPS_TDS"), new GUIContent("Throw (TP/TD)"));
//                        }

                    }

                    break;

                case "Sounds":
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PickUpWeaponAudio"), new GUIContent("Pickup"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("DropWeaponAudio"), new GUIContent("Drop"));

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                    for (var i = 0; i < script.Attacks.Count; i++)
                    {
                        var attack = script.Attacks[i];
                        var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(i);

                        EditorGUILayout.LabelField("Attacks: " + script.attacksNames[i], EditorStyles.boldLabel);
                        EditorGUILayout.BeginVertical("HelpBox");

                        if (attack.AttackType == WeaponsHelper.TypeOfAttack.Melee)
                        {
                            EditorGUILayout.HelpBox("Add the [PlayAttackSound] event on attack animations to set the exact playing time of the attack sound.", MessageType.Info);
                        }
                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackAudio"), new GUIContent("Attack"));

                        if (attack.AttackType != WeaponsHelper.TypeOfAttack.Melee && attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                        {
                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ReloadAudio"), new GUIContent("Reload"));
                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("NoAmmoShotAudio"), new GUIContent("Attack without ammo"));
                        }

                        EditorGUILayout.EndVertical();
                        if (i < script.Attacks.Count - 1)
                            EditorGUILayout.Space();
                    }


                    break;
            }

            serializedObject.ApplyModifiedProperties();

//            DrawDefaultInspector();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        void CheckPoint(WeaponsHelper.Attack _attack, string type)
        {
            if (type == "attack" && !_attack.AttackSpawnPoint || type != "attack" && !_attack.ShellPoint)
            {
                if (!script.gameObject.activeInHierarchy)
                {
                    if (GUILayout.Button("Create point"))
                    {
                        var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                        if (type == "attack") tempWeapon.GetComponent<WeaponController>().Attacks[script.currentAttack].AttackSpawnPoint = Helper.NewPoint(tempWeapon, "Attack Point");
                        else tempWeapon.GetComponent<WeaponController>().Attacks[script.currentAttack].ShellPoint = Helper.NewPoint(tempWeapon, "Shell Spawn Point");

#if !UNITY_2018_3_OR_NEWER
                        PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                        PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                        
                        DestroyImmediate(tempWeapon);
                    }
                }
                else
                {
                    if (GUILayout.Button("Create point"))
                    {
                        if (type == "attack") script.Attacks[script.currentAttack].AttackSpawnPoint = Helper.NewPoint(script.gameObject, "Attack Point");
                        else  script.Attacks[script.currentAttack].ShellPoint = Helper.NewPoint(script.gameObject, "Shell Spawn Point");
                    }
                }
            }
            else if (type == "attack" && _attack.AttackSpawnPoint || type != "attack" && _attack.ShellPoint)
            {
                if (type == "attack" && _attack.AttackSpawnPoint.localPosition == Vector3.zero)
                    EditorGUILayout.HelpBox("Adjust the position of the [Attack Point]", MessageType.Warning);
                
                else if(type != "attack" && _attack.ShellPoint.localPosition == Vector3.zero)
                    EditorGUILayout.HelpBox("Adjust the position of the [Shell Spawn Point]", MessageType.Warning);
            }
        }

        void CheckCollider(WeaponsHelper.Attack _attack, string type)
        {
            if (_attack.AttackCollider)
            {
                if (_attack.AttackCollider.transform.localScale == Vector3.one)
                {
                    EditorGUILayout.HelpBox("Adjust the size of the" + (type == "fire" ? " Fire Collider." : " Melee Collider.") +
                                            " It's the area that will deal damage.", MessageType.Warning);
                }
            }
            else
            {
                if (!script.gameObject.activeInHierarchy)
                {
                    if (GUILayout.Button("Create collider"))
                    {
                        var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                        tempWeapon.GetComponent<WeaponController>().Attacks[script.currentAttack].AttackCollider = type == "fire"
                            ? Helper.NewCollider("Fire Collider", "Fire", tempWeapon.transform)
                            : Helper.NewCollider("Melee Collider", "Melee Collider", tempWeapon.transform);
#if !UNITY_2018_3_OR_NEWER
                        PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                        PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                        DestroyImmediate(tempWeapon);
                    }
                }
                else
                {
                    if (GUILayout.Button("Create collider"))
                    {
                        script.Attacks[script.currentAttack].AttackCollider = type == "fire"
                            ? Helper.NewCollider("Fire Collider", "Fire", script.transform)
                            : Helper.NewCollider("Melee Collider", "Melee Collider", script.transform);
                    }
                }
            }
        }
    }
}


