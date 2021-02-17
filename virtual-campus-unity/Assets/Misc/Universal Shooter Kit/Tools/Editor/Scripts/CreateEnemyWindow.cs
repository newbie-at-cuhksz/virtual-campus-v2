// GercStudio
// © 2018-2020

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;

namespace GercStudio.USK.Scripts
{
    public class CreateEnemyWindow : EditorWindow
    {
        public GameObject Model;

        private bool enemyAdded;
        private bool hasCreated;
        private bool startCreation;
        
        private RagdollHelper.BodyParameters bodyParameters = new RagdollHelper.BodyParameters();

        private float startVal;
        private float progress;

        private Vector2 scrollPos;

        private GUIStyle LabelStyle;

        [MenuItem("Tools/USK/Create/Enemy")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CreateEnemyWindow), true, "", true).ShowUtility();
        }

        private void Awake()
        {
            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle {normal = {textColor = Color.black}, fontStyle = FontStyle.Bold, fontSize = 12, alignment = TextAnchor.MiddleCenter};
            }
        }

        void OnEnable()
        {
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (Model)
            {
                if (!enemyAdded)
                {
                    enemyAdded = true;
                }
                else
                {
                    if (Model.GetComponent<Animator>())
                    {
                        if (!Model.GetComponent<Animator>().avatar)
                        {
                            DestroyImmediate(Model.GetComponent<Animator>());
                            Model.AddComponent<Animator>();

//                            if (!Model.GetComponent<Animator>().avatar)
//                            {
//                                DestroyImmediate(Model.GetComponent<Animator>());
//                                Model = null;
//                                modelError = true;
//                            }
                        }

//                        else
//                        {
//                            if (!Model.GetComponent<Animator>().avatar.isHuman)
//                            {
//                                Model = null;
//                                modelError = true;
//                            }
//                            else
//                            {
//                                modelError = false;
//                            }
//                        }
                    }
                    else
                    {
                        Model.AddComponent<Animator>();
                    }
                }

                if (startCreation & progress > 1.16f)
                {
                    AddScripts();
//                    CreateBodyColliders();
                    SaveEnemyToPrefab();
                    hasCreated = true;
                    startVal = (float) EditorApplication.timeSinceStartup;

                    startCreation = false;
                }
            }
            else
            {
                enemyAdded = false;
            }

            if (hasCreated)
            {
                if (progress > 10)
                {
                    hasCreated = false;

                    Model = null;
                }
            }
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.Space();
            GUILayout.Label("Create Enemy", LabelStyle);
            EditorGUILayout.Space();
            if (hasCreated)
            {
                var style = new GUIStyle {normal = {textColor = Color.green}, fontStyle = FontStyle.Bold, fontSize = 10, alignment = TextAnchor.MiddleCenter};
                EditorGUILayout.LabelField("Enemy has been created", style);
                EditorGUILayout.HelpBox("Add other parameters (like animations, sounds, etc) to the [EnemyController] script.", MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginVertical("HelpBox");
//            if (modelError)
//            {
//                EditorGUILayout.HelpBox(
//                    "Enemy's model must be the Humanoid type.",
//                    MessageType.Warning);
//            }

            Model = (GameObject) EditorGUILayout.ObjectField("Enemy's Model", Model, typeof(GameObject), false);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (Model)
            {
                if (startCreation)
                {
                    if (progress < 0.3f)
                        EditorGUI.ProgressBar(new Rect(3, GUILayoutUtility.GetLastRect().max.y + 10, position.width - 6, 20), progress / 1, "Creation.");
                    else if (progress > 0.3f && progress < 0.6f)
                        EditorGUI.ProgressBar(new Rect(3, GUILayoutUtility.GetLastRect().max.y + 10, position.width - 6, 20), progress / 1, "Creation..");
                    else if (progress > 0.6f)
                        EditorGUI.ProgressBar(new Rect(3, GUILayoutUtility.GetLastRect().max.y + 10, position.width - 6, 20), progress / 1, "Creation...");
                }
            }


            EditorGUI.BeginDisabledGroup(!Model);

            if (!startCreation)
                if (GUILayout.Button("Create"))
                {
                    startVal = (float) EditorApplication.timeSinceStartup;
                    startCreation = true;
                }


            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();

            progress = (float) (EditorApplication.timeSinceStartup - startVal);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

//        void CreateRagdoll()
//        {
//            if (Ragdoll.GetComponent<Controller>())
//                DestroyImmediate(Ragdoll.GetComponent<Controller>());
//
//            foreach (var comp in Ragdoll.GetComponents<Component>())
//            {
//                if (!(comp is Animator) & !(comp is Transform))
//                {
//                    DestroyImmediate(comp);
//                }
//            }
//
//            var ragdollBuilderType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
//            var windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
//
//            if (windows == null || windows.Length == 0)
//            {
//                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
//                windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
//            }
//
//            if (windows != null && windows.Length > 0)
//            {
//                var ragdollWindow = windows[0] as ScriptableWizard;
//
//                var animator = Ragdoll.GetComponent<Animator>();
//                SetFieldValue(ragdollWindow, "pelvis", animator.GetBoneTransform(HumanBodyBones.Hips));
//                SetFieldValue(ragdollWindow, "leftHips", animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
//                SetFieldValue(ragdollWindow, "leftKnee", animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
//                SetFieldValue(ragdollWindow, "leftFoot", animator.GetBoneTransform(HumanBodyBones.LeftFoot));
//                SetFieldValue(ragdollWindow, "rightHips", animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
//                SetFieldValue(ragdollWindow, "rightKnee", animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
//                SetFieldValue(ragdollWindow, "rightFoot", animator.GetBoneTransform(HumanBodyBones.RightFoot));
//                SetFieldValue(ragdollWindow, "leftArm", animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
//                SetFieldValue(ragdollWindow, "leftElbow", animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
//                SetFieldValue(ragdollWindow, "rightArm", animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
//                SetFieldValue(ragdollWindow, "rightElbow", animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
//                SetFieldValue(ragdollWindow, "middleSpine", animator.GetBoneTransform(HumanBodyBones.Spine));
//                SetFieldValue(ragdollWindow, "head", animator.GetBoneTransform(HumanBodyBones.Head));
//
//                var method = ragdollWindow.GetType().GetMethod("CheckConsistency",
//                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//                if (method != null)
//                {
//                    ragdollWindow.errorString = (string) method.Invoke(ragdollWindow, null);
//                    ragdollWindow.isValid = string.IsNullOrEmpty(ragdollWindow.errorString);
//                }
//
//                saveRagdoll = false;
//            }
//        }

        /*private void SetFieldValue(ScriptableWizard obj, string name, object value)
        {
            if (value == null)
            {
                return;
            }

            var field = obj.GetType().GetField(name);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }*/

        /*void SaveRagdollToPrefab()
        {
            if (Ragdoll.GetComponent<Animator>())
                DestroyImmediate(Ragdoll.GetComponent<Animator>());

            Ragdoll.AddComponent<DestroyObject>().DestroyTime = 7;

            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/Ragdolls/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/Ragdolls/");
            }
            
#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + Model.name + " Ragdoll.prefab");
            PrefabUtility.ReplacePrefab(Ragdoll, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(Ragdoll, "Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + Model.name + " Ragdoll.prefab");
#endif
            
            DestroyImmediate(Ragdoll);
            
            Ragdoll = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + Model.name + " Ragdoll.prefab",
                typeof(GameObject)) as GameObject;

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + Model.name + " Ragdoll.prefab",
                typeof(GameObject)) as GameObject);
        }*/

        void SaveEnemyToPrefab()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/Enemies/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/Enemies/");
            }

            var index = 0;
            while (AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/Enemies/" + Model.name + " " + index + ".prefab", typeof(GameObject)) != null)
            {
                index++;
            }

#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/Enemies/" + Model.name + " " + index + ".prefab");
            PrefabUtility.ReplacePrefab(Model, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(Model, "Assets/Universal Shooter Kit/Prefabs/Enemies/" + Model.name + " " + index + ".prefab");
#endif

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/Enemies/" + Model.name + " " + index + ".prefab",
                typeof(GameObject)));

            DestroyImmediate(Model);
        }

        void AddScripts()
        {
            var name = Model.name;

            Model = Instantiate(Model, Vector3.zero, Quaternion.Euler(Vector3.zero));
            Model.SetActive(true);

            Model.name = name;

            if (!Model.GetComponent<EnemyController>())
                Model.AddComponent<EnemyController>();

            var controller = Model.GetComponent<EnemyController>();

            controller.DirectionObject = new GameObject("Direction").transform;
            controller.DirectionObject.parent = controller.transform;
            controller.DirectionObject.localPosition = Vector3.zero;

            controller.BodyParts = new List<Transform> {null, null, null, null, null, null, null, null, null, null, null};

//            if(Ragdoll)
//                controller.Ragdoll = Ragdoll.transform;

            AIHelper.CreateNewStateCanvas(controller, controller.transform);

            controller.trailMaterial = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/Trail Mat.mat", typeof(Material)) as Material;

            controller.AnimatorController = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/_Animator Controllers/AI.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
        }

        void CreateBodyColliders()
        {
            if (Model.GetComponent<Animator>() && Model.GetComponent<Animator>().avatar && Model.GetComponent<Animator>().avatar.isHuman)
            {
                RagdollHelper.GetBones(bodyParameters, Model.GetComponent<Animator>());
                RagdollHelper.SetBodyParts(bodyParameters, Model.GetComponent<EnemyController>());

                RagdollHelper.CreateColliders(bodyParameters);
            }
        }
    }
}

