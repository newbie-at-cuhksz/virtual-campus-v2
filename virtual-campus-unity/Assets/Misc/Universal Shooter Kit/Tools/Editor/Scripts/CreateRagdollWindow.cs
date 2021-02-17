// GercStudio
// © 2018-2020

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;

namespace GercStudio.USK.Scripts
{
    public class CreateRagdollWindow : EditorWindow
    {
        public GameObject Model;
        public GameObject Ragdoll;

        private bool modelError;
        private bool modelAdded;
        private bool saveRagdoll = true;

        private Vector2 scrollPos;

        private GUIStyle LabelStyle;

//        [MenuItem("Tools/Create Ragdoll")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CreateRagdollWindow), true, "", true).ShowUtility();
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
                if (Ragdoll & !saveRagdoll)
                {
                    if (Ragdoll.GetComponent<Animator>())
                        if (Ragdoll.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>())
                        {
                            SaveRagdollToPrefab();
                            saveRagdoll = true;
                        }
                }

                if (!modelAdded)
                {
                    modelAdded = true;
                }
                else
                {
                    if (Model.GetComponent<Animator>())
                    {
                        if (Model.GetComponent<Animator>().avatar)
                        {
                            if (!Model.GetComponent<Animator>().avatar.isHuman)
                            {
                                Model = null;
                                modelError = true;
                            }
                            else
                            {
                                modelError = false;
                            }
                        }
                        else
                        {
                            DestroyImmediate(Model.GetComponent<Animator>());
                            Model.AddComponent<Animator>();

                            if (!Model.GetComponent<Animator>().avatar)
                            {
                                DestroyImmediate(Model.GetComponent<Animator>());
                                Model = null;
                                modelError = true;
                            }
                        }
                    }
                    else
                    {
                        Model.AddComponent<Animator>();
                    }
                }
            }
            else
            {
                Ragdoll = null;
                modelAdded = false;
            }
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.Space();
            GUILayout.Label("Create Ragdoll (automatic)", LabelStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("HelpBox");
            if (modelError)
            {
                EditorGUILayout.HelpBox("Ragdoll model must be the Humanoid type.", MessageType.Warning);
            }

            Model = (GameObject) EditorGUILayout.ObjectField("Ragdoll Model", Model, typeof(GameObject), false);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (Model)
            {
                if (GUILayout.Button("Create Ragdoll"))
                {
                    Ragdoll = Instantiate(Model);
                    Ragdoll.name = Model.name + " Ragdoll";
                    CreateRagdoll();
                }
            }
            
            EditorGUILayout.EndScrollView();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void CreateRagdoll()
        {
            if (Ragdoll.GetComponent<Controller>())
                DestroyImmediate(Ragdoll.GetComponent<Controller>());

            foreach (var comp in Ragdoll.GetComponents<Component>())
            {
                if (!(comp is Animator) & !(comp is Transform))
                {
                    DestroyImmediate(comp);
                }
            }

            var ragdollBuilderType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
            var windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);

            if (windows == null || windows.Length == 0)
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
                windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
            }

            if (windows != null && windows.Length > 0)
            {
                var ragdollWindow = windows[0] as ScriptableWizard;

                var animator = Ragdoll.GetComponent<Animator>();
                SetFieldValue(ragdollWindow, "pelvis", animator.GetBoneTransform(HumanBodyBones.Hips));
                SetFieldValue(ragdollWindow, "leftHips", animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
                SetFieldValue(ragdollWindow, "leftKnee", animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
                SetFieldValue(ragdollWindow, "leftFoot", animator.GetBoneTransform(HumanBodyBones.LeftFoot));
                SetFieldValue(ragdollWindow, "rightHips", animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
                SetFieldValue(ragdollWindow, "rightKnee", animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
                SetFieldValue(ragdollWindow, "rightFoot", animator.GetBoneTransform(HumanBodyBones.RightFoot));
                SetFieldValue(ragdollWindow, "leftArm", animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
                SetFieldValue(ragdollWindow, "leftElbow", animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
                SetFieldValue(ragdollWindow, "rightArm", animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
                SetFieldValue(ragdollWindow, "rightElbow", animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
                SetFieldValue(ragdollWindow, "middleSpine", animator.GetBoneTransform(HumanBodyBones.Spine));
                SetFieldValue(ragdollWindow, "head", animator.GetBoneTransform(HumanBodyBones.Head));

                var method = ragdollWindow.GetType().GetMethod("CheckConsistency",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    ragdollWindow.errorString = (string) method.Invoke(ragdollWindow, null);
                    ragdollWindow.isValid = string.IsNullOrEmpty(ragdollWindow.errorString);
                }

                saveRagdoll = false;
            }
        }

        void SetFieldValue(ScriptableWizard obj, string name, object value)
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
        }

        void SaveRagdollToPrefab()
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
            
            if (Model)
                Model = null;

            if (Ragdoll)
                Ragdoll = null;
        }
    }
}

