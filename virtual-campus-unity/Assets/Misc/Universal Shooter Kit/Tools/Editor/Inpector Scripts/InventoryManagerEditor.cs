using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(InventoryManager))]
    public class InventoryManagerEditor : Editor
    {
        private Vector3 defaultSize;

        private InventoryManager script;

        private Animator animator;

        private float startVal;
        private float progress;

        private ReorderableList[] weaponsList = new ReorderableList[8];
        private ReorderableList handsAnimations;
        private ReorderableList fullBodyAnimations;

        private bool weaponPrefabWarning;

        private bool greandePrefabWarning;

        public void Awake()
        {
            script = (InventoryManager) target;
        }

        private void OnEnable()
        {
            for (var i = 0; i < 8; i++)
            {
                var i1 = i;
                weaponsList[i] = new ReorderableList(serializedObject, serializedObject.FindProperty("slots").GetArrayElementAtIndex(i).FindPropertyRelative("weaponSlotInInspector"), true, true, true, true)
                {
                    drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Weapons"); },
                    onAddCallback = items =>
                    {
                        if (script.slots[i1] == null)
                            script.slots[i1] = new CharacterHelper.InventorySlot();

                        script.slots[i1].weaponSlotInInspector.Add(null);

                    },

                    onRemoveCallback = items => { script.slots[i1].weaponSlotInInspector.Remove(script.slots[i1].weaponSlotInInspector[items.index]); },

                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        if (!script.slots[i1].weaponSlotInInspector[index].weapon && !script.slots[i1].weaponSlotInInspector[index].fistAttack)
                        {
                            if (!script.HasFistAttack)
                            {
                                script.slots[i1].weaponSlotInInspector[index].weapon = (GameObject) EditorGUI.ObjectField(
                                    new Rect(rect.x, rect.y, rect.width / 1.5f, EditorGUIUtility.singleLineHeight),
                                    script.slots[i1].weaponSlotInInspector[index].weapon, typeof(GameObject), false);

                                script.slots[i1].weaponSlotInInspector[index].fistAttack = EditorGUI.ToggleLeft(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10, EditorGUIUtility.singleLineHeight)
                                    , "Fist Attack", script.slots[i1].weaponSlotInInspector[index].fistAttack);
                            }
                            else
                            {
                                script.slots[i1].weaponSlotInInspector[index].weapon = (GameObject) EditorGUI.ObjectField(
                                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                    script.slots[i1].weaponSlotInInspector[index].weapon, typeof(GameObject), false);
                            }

                        }
                        else if (script.slots[i1].weaponSlotInInspector[index].weapon)
                        {
                            script.slots[i1].weaponSlotInInspector[index].weapon = (GameObject) EditorGUI.ObjectField(
                                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                script.slots[i1].weaponSlotInInspector[index].weapon, typeof(GameObject), false);
                        }
                        else if (script.slots[i1].weaponSlotInInspector[index].fistAttack)
                        {
                            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - rect.width / 2, EditorGUIUtility.singleLineHeight), "Set all variables below ↓");

                            script.slots[i1].weaponSlotInInspector[index].fistAttack = EditorGUI.ToggleLeft(new Rect(rect.x + rect.width / 1.5f + 10, rect.y, rect.width - rect.width / 1.5f - 10, EditorGUIUtility.singleLineHeight)
                                , "Fist Attack", script.slots[i1].weaponSlotInInspector[index].fistAttack);
                        }
                    }
                };
            }

            handsAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("fistAttackHandsAnimations"), false, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Hands Animations (for FP and TD views)"); },

                onAddCallback = items => { script.fistAttackHandsAnimations.Add(null); },

                onRemoveCallback = items =>
                {
                    if (script.fistAttackHandsAnimations.Count == 1)
                        return;

                    script.fistAttackHandsAnimations.Remove(script.fistAttackHandsAnimations[items.index]);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.fistAttackHandsAnimations[index] = (AnimationClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.fistAttackHandsAnimations[index], typeof(AnimationClip), false);
                }
            };

            fullBodyAnimations = new ReorderableList(serializedObject, serializedObject.FindProperty("fistAttackFullBodyAnimations"), false, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Full-body animations (for TP view)"); },

                onAddCallback = items => { script.fistAttackFullBodyAnimations.Add(null); },

                onRemoveCallback = items =>
                {
                    if (script.fistAttackFullBodyAnimations.Count == 1)
                        return;

                    script.fistAttackFullBodyAnimations.Remove(script.fistAttackFullBodyAnimations[items.index]);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.fistAttackFullBodyAnimations[index] = (AnimationClip) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        script.fistAttackFullBodyAnimations[index], typeof(AnimationClip), false);
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
            for (var i = 0; i < 8; i++)
            {
//                var WeaponCount = script.slots[i].weaponsCount;

                for (var j = 0; j < script.slots[i].weaponSlotInInspector.Count; j++)
                {
                    if (script.slots[i].weaponSlotInInspector[j] != null)
                    {
                        if (script.slots[i].weaponSlotInInspector[j].weapon)
                        {
                            if (!script.slots[i].weaponSlotInInspector[j].weapon.GetComponent<WeaponController>())
                            {
                                weaponPrefabWarning = true;
                                script.slots[i].weaponSlotInInspector[j].weapon = null;
                            }
                            else
                            {
                                weaponPrefabWarning = false;
                            }
                        }
                    }
                }
            }

            var fistAttack = false;

            for (var i = 0; i < 8; i++)
            {
                foreach (var slot in script.slots[i].weaponSlotInInspector)
                {
                    if (slot != null && slot.fistAttack)
                    {
                        fistAttack = true;
                        break;
                    }
                }
            }

            script.HasFistAttack = fistAttack;

            if (Application.isPlaying)
            {
                if (!animator)
                    animator = script.GetComponent<Animator>();
            }
            else
            {
                if (script && !script.RightHandCollider && !script.gameObject.activeInHierarchy)
                {
                    var tempCharacter = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

                    var controller = tempCharacter.GetComponent<InventoryManager>();

                    controller.RightHandCollider = Helper.NewCollider("Right Collider", "Melee Collider", tempCharacter.GetComponent<Controller>().BodyObjects.RightHand);
                    controller.LeftHandCollider = Helper.NewCollider("Left Collider", "Melee Collider", tempCharacter.GetComponent<Controller>().BodyObjects.LeftHand);

#if !UNITY_2018_3_OR_NEWER
                    PrefabUtility.ReplacePrefab(tempCharacter, PrefabUtility.GetPrefabParent(tempCharacter), ReplacePrefabOptions.ConnectToPrefab);
#else
                    PrefabUtility.SaveAsPrefabAssetAndConnect(tempCharacter, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCharacter), InteractionMode.AutomatedAction);
#endif

                    DestroyImmediate(tempCharacter);
                }

                if (!script.trailMaterial)
                    script.trailMaterial = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/Trail Mat.mat", typeof(Material)) as Material;

                if (!script.BloodProjector)
                    script.BloodProjector = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/Blood Projector.prefab", typeof(Projector)) as Projector;



//                if (script && !script.FlashImage && !script.gameObject.activeInHierarchy)
//                {
//
//                    var tempCharacter = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
//
//                    var _script = tempCharacter.GetComponent<InventoryManager>();
//
//                    _script.FlashImage = Helper.NewImage("Flash Image", _script.canvas.transform, Vector2.one, Vector2.zero);
//                    Helper.SetAndStretchToParentSize(_script.FlashImage.GetComponent<RectTransform>(), _script.canvas.GetComponent<RectTransform>(), true);
//                    _script.FlashImage.color = new Color(1, 1, 1, 0);
//                    _script.FlashImage.gameObject.SetActive(false);
//
//
//#if !UNITY_2018_3_OR_NEWER
//                    PrefabUtility.ReplacePrefab(tempCharacter, PrefabUtility.GetPrefabParent(tempCharacter), ReplacePrefabOptions.ConnectToPrefab);
//#else
//                    PrefabUtility.SaveAsPrefabAssetAndConnect(tempCharacter, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCharacter), InteractionMode.AutomatedAction);
//#endif
//
//                    DestroyImmediate(tempCharacter);
//                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            script.inventoryTabUp = GUILayout.Toolbar(script.inventoryTabUp,
                new[] {"Slot 1", "Slot 2", "Slot 3", "Slot 4"});

            switch (script.inventoryTabUp)
            {
                case 0:
                    script.inventoryTabMiddle = 4;
                    script.currentInventorySlot = 0;
                    break;
                case 1:
                    script.inventoryTabMiddle = 4;
                    script.currentInventorySlot = 1;
                    break;
                case 2:
                    script.inventoryTabMiddle = 4;
                    script.currentInventorySlot = 2;
                    break;
                case 3:
                    script.inventoryTabMiddle = 4;
                    script.currentInventorySlot = 3;
                    break;
            }

            script.inventoryTabMiddle = GUILayout.Toolbar(script.inventoryTabMiddle,
                new[] {"Slot 5", "Slot 6", "Slot 7", "Slot 8"});

            switch (script.inventoryTabMiddle)
            {
                case 0:
                    script.inventoryTabUp = 4;
                    script.currentInventorySlot = 4;
                    break;
                case 1:
                    script.inventoryTabUp = 4;
                    script.currentInventorySlot = 5;
                    break;
                case 2:
                    script.inventoryTabUp = 4;
                    script.currentInventorySlot = 6;
                    break;
                case 3:
                    script.inventoryTabUp = 4;
                    script.currentInventorySlot = 7;
                    break;
            }

            for (var i = 0; i < 8; i++)
            {
                if (script.currentInventorySlot == i)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("HelpBox");
                    if (!Application.isPlaying)
                    {
                        weaponsList[script.currentInventorySlot].DoLayoutList();

                        if (script.slots[i].weaponSlotInInspector.Any(item => item != null && item.fistAttack))
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("Fist Attack Parameters", EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("HelpBox");

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("FistIcon"), new GUIContent("Fist Image"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("RateOfAttack"), new GUIContent("Rate of Attack"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("FistDamage"), new GUIContent("Damage"));
                            EditorGUILayout.Space();

                            EditorGUI.BeginDisabledGroup(true);
                            {
                            }
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftHandCollider"), new GUIContent("Left Collider"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("RightHandCollider"), new GUIContent("Right Collider"));
                            EditorGUI.EndDisabledGroup();
                            EditorGUILayout.Space();

                            EditorGUILayout.HelpBox("Add the [PlayAttackSound] event on attack animations to set the exact playing time of the punch sound.", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("fistAttackAudio"), new GUIContent("Sound"));
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("HandsIdle"), new GUIContent("Idle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("HandsWalk"), new GUIContent("Walk"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("HandsRun"), new GUIContent("Run"));
                            EditorGUILayout.Space();
                            handsAnimations.DoLayoutList();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            fullBodyAnimations.DoLayoutList();
                            EditorGUILayout.EndVertical();
                        }
                    }
                    else if (Application.isPlaying && (script.Controller && !script.Controller.AdjustmentScene || !script.gameObject.activeInHierarchy))
                    {

                        weaponsList[script.currentInventorySlot].DoLayoutList();

                        if (script.slots[i].weaponSlotInInspector.Any(item => item.fistAttack))
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("FistIcon"), new GUIContent("Fist Image"));
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("HandsIdle"), new GUIContent("Idle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("HandsWalk"), new GUIContent("Walk"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("HandsRun"), new GUIContent("Run"));

                            EditorGUILayout.EndVertical();
                        }
                    }
                    else if (Application.isPlaying && script.Controller && script.Controller.AdjustmentScene)
                    {
                        EditorGUILayout.LabelField("You are in the [Adjustment scene]. " + "\n" + "To choose a weapon use [Adjustment] script here:");

                        EditorGUILayout.Space();
                        EditorGUILayout.ObjectField("Adjustment object", FindObjectOfType<Adjustment>(), typeof(Adjustment), true);
                    }

                    if (weaponPrefabWarning)
                        EditorGUILayout.HelpBox("Your weapon should has [WeaponController] script", MessageType.Warning);


                    EditorGUILayout.EndVertical();
                }
            }


            switch (script.currentInventorySlot)
            {
                case 9:
                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.HelpBox("You can change the texture, text, color and other graphic parameters of this slot.", MessageType.Info);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AmmoButton"),
                        new GUIContent("Slot UI"), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndVertical();
                    break;
                case 10:
                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.HelpBox(
                        "You can change the texture, text, color and other graphic parameters of this slot.",
                        MessageType.Info);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("HealthButton"), new GUIContent("Slot UI"), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndVertical();
                    break;
            }


            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}


