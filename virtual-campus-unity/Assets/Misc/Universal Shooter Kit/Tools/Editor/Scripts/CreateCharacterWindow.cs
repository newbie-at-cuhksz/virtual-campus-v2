// GercStudio
// © 2018-2020

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;   
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
    public class CreateCharacterWindow : EditorWindow
    {

        public GameObject CharacterModel;
        public GameObject Ragdoll;

        private bool characterError;
        private bool characterAdded;
        private bool CameraParametersError;
        private bool hasCreated;
        private bool startCreation;
        private float startVal;
        private float progress;
        private Vector2 scrollPos;

        private RagdollHelper.BodyParameters bodyParameters = new RagdollHelper.BodyParameters();

        private GUIStyle LabelStyle;

        [MenuItem("Tools/USK/Create/Character")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CreateCharacterWindow), true, "", true).ShowUtility();
        }

        private void Awake()
        {
            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle();
                LabelStyle.normal.textColor = Color.black;
                LabelStyle.fontStyle = FontStyle.Bold;
                LabelStyle.fontSize = 12;
                LabelStyle.alignment = TextAnchor.MiddleCenter;
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
            if (CharacterModel)
            {
                if (!characterAdded)
                {
                    characterAdded = true;
                }
                else
                {
                    if (CharacterModel.GetComponent<Animator>())
                    {
                        if (CharacterModel.GetComponent<Animator>().avatar)
                        {
                            if (!CharacterModel.GetComponent<Animator>().avatar.isHuman)
                            {
                                CharacterModel = null;
                                characterError = true;
                            }
                            else
                            {
                                characterError = false;
                            }
                        }
                        else
                        {
                            DestroyImmediate(CharacterModel.GetComponent<Animator>());
                            CharacterModel.AddComponent<Animator>();

                            if (!CharacterModel.GetComponent<Animator>().avatar)
                            {
                                DestroyImmediate(CharacterModel.GetComponent<Animator>());
                                CharacterModel = null;
                                characterError = true;
                            }
                        }
                    }
                    else
                    {
                        CharacterModel.AddComponent<Animator>();
                    }
                }

                if (startCreation & progress > 1.16f)
                {
                    AddScripts();
                    SetVariables();
//                    CreateBodyColliders();
                    CreateUI();
                    CreateCamera();

                    SaveCharacterToPrefab();
                    hasCreated = true;
                    startVal = (float) EditorApplication.timeSinceStartup;


                    startCreation = false;
                }
            }
            else
            {
                Ragdoll = null;
                characterAdded = false;
            }

            if (hasCreated)
            {
                if (progress > 10)
                {
                    hasCreated = false;

                    CharacterModel = null;
                    Ragdoll = null;
                }
            }
        }

        private void OnGUI()
        {
            scrollPos =
                EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width),
                    GUILayout.Height(position.height));

            EditorGUILayout.Space();
            GUILayout.Label("Create Character", LabelStyle);
            EditorGUILayout.Space();
            if (hasCreated)
            {
                var style = new GUIStyle {normal = {textColor = Color.green}, fontStyle = FontStyle.Bold, fontSize = 10, alignment = TextAnchor.MiddleCenter};
                EditorGUILayout.LabelField("Character has been created", style);
                EditorGUILayout.HelpBox("To adjust the character open the Adjustment scene by pressing [Tools -> USK -> Adjust]", MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginVertical("HelpBox");
            if (characterError)
            {
                EditorGUILayout.HelpBox("Character's model must be the Humanoid type.", MessageType.Warning);
            }

            CharacterModel = (GameObject) EditorGUILayout.ObjectField("Character's Model", CharacterModel, typeof(GameObject), false);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (CharacterModel)
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


            EditorGUI.BeginDisabledGroup(!CharacterModel);

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

        private void SetFieldValue(ScriptableWizard obj, string name, object value)
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
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + CharacterModel.name + "Ragdoll.prefab");
            PrefabUtility.ReplacePrefab(Ragdoll, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(Ragdoll, "Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + CharacterModel.name + "Ragdoll.prefab");
#endif
            
            DestroyImmediate(Ragdoll);
            
            Ragdoll = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + CharacterModel.name + "Ragdoll.prefab",
                typeof(GameObject)) as GameObject;

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Prefabs/Ragdolls/" + CharacterModel.name + "Ragdoll.prefab",
                typeof(GameObject)) as GameObject);
        }

        void SaveCharacterToPrefab()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/Characters/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/Characters/");
            }

            var index = 0;
            while(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/Characters/" + CharacterModel.name + " " + index + ".prefab", typeof(GameObject)) != null)
            {
                index++;
            }
            
#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/Characters/" + CharacterModel.name + " " + index + ".prefab");
            PrefabUtility.ReplacePrefab(CharacterModel, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(CharacterModel, "Assets/Universal Shooter Kit/Prefabs/Characters/" + CharacterModel.name + " " + index + ".prefab");
#endif

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/Characters/" + CharacterModel.name + " " + index + ".prefab",
                typeof(GameObject)));

            DestroyImmediate(CharacterModel);

        }

        void AddScripts()
        {
            var name = CharacterModel.name;
            
            CharacterModel = Instantiate(CharacterModel, Vector3.zero, Quaternion.Euler(Vector3.zero));
            CharacterModel.SetActive(true);
            
            CharacterModel.name = name;
            
            if (!CharacterModel.GetComponent<Controller>())
                CharacterModel.AddComponent<Controller>();

            if (!CharacterModel.GetComponent<InventoryManager>())
                CharacterModel.AddComponent<InventoryManager>();

            var controller = CharacterModel.GetComponent<Controller>();
            var manager = CharacterModel.GetComponent<InventoryManager>();

            controller.BodyParts = new List<Transform> {null, null, null, null, null, null, null, null, null, null, null};
            
            if (!controller.CharacterController)
            {
                controller.CharacterController = CharacterModel.AddComponent<CharacterController>();
                controller.CharacterController.height = 1;
            }

            if(!manager.BloodProjector)
                manager.BloodProjector = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/Blood Projector.prefab", typeof(Projector)) as Projector;

            if(!manager.trailMaterial)
                manager.trailMaterial = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/Trail Mat.mat", typeof(Material)) as Material;

            if (!controller.DirectionObject)
            {
                controller.DirectionObject = new GameObject("Direction object").transform;
                controller.DirectionObject.parent = CharacterModel.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Spine);
                controller.DirectionObject.localPosition = Vector3.zero;
                controller.DirectionObject.localEulerAngles = Vector3.zero;
            }
        }

        void CreateUI()
        {
//            var manager = CharacterModel.GetComponent<InventoryManager>();

//            if (!manager.canvas)
//            {
//                manager.canvas = Helper.NewCanvas("Canvas", new Vector2(1920, 1080), CharacterModel.transform);
//            }

//            if (!manager.aimTextureImage)
//            {
//                var obj = Helper.NewUIElement("Aim texture", manager.canvas.transform, Vector2.zero, new Vector2(1920, 1080), Vector3.one);
//                manager.aimTextureImage = obj.AddComponent<RawImage>();
//                manager.aimTextureImage.raycastTarget = false;
//                obj.SetActive(false);
//            }

//            if (!manager.FlashImage)
//            {
//                manager.FlashImage = Helper.NewImage("Flash Image", manager.canvas.transform, Vector2.one, Vector2.zero);
//                Helper.SetAndStretchToParentSize(manager.FlashImage.GetComponent<RectTransform>(), manager.canvas.GetComponent<RectTransform>(), true);
//                manager.FlashImage.color = new Color(1, 1, 1, 0);
//                manager.FlashImage.gameObject.SetActive(false);
//            }
        }

        void CreateCamera()
        {
            var controller = CharacterModel.GetComponent<Controller>();
            if (!controller.thisCamera)
            {
                var camera = new GameObject("MainCamera");
                var cameraComponent = camera.AddComponent<Camera>();
                camera.tag = "MainCamera";
                var cameraController = camera.AddComponent<CameraController>();

                cameraComponent.nearClipPlane = 0.01f;
                camera.GetComponent<Camera>().cullingMask &= ~(1 << 15); // Layer 15 = Hidden

                camera.transform.parent = CharacterModel.transform;
                camera.transform.localPosition = new Vector3(0, 0, 0);
                camera.AddComponent<AudioListener>();

                controller.thisCamera = camera;
                controller.CameraController = camera.GetComponent<CameraController>();
                camera.GetComponent<CameraController>().Controller = controller;

                var aimCamera = new GameObject("AimCamera") {tag = "MainCamera"};
                aimCamera.transform.parent = camera.transform;
                aimCamera.transform.localPosition = Vector3.zero;
                aimCamera.transform.localEulerAngles = Vector3.zero;
                cameraController.AimCamera = aimCamera.AddComponent<Camera>();
                cameraController.AimCamera.nearClipPlane = 0.01f;

                cameraController.CameraPosition = Helper.NewObject(CharacterModel.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head), "FP Camera Position & Rotation");

                DestroyImmediate(cameraController.CameraPosition.GetComponent<BoxCollider>());
            }
        }

        void SetVariables()
        {
            var _animator = CharacterModel.GetComponent<Animator>();
            var controller = CharacterModel.GetComponent<Controller>();

            controller.characterAnimatorController = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/_Animator Controllers/Accurate.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;

            controller.BodyObjects.RightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            controller.BodyObjects.LeftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
            controller.BodyObjects.Head = _animator.GetBoneTransform(HumanBodyBones.Head);
            controller.BodyObjects.TopBody = _animator.GetBoneTransform(HumanBodyBones.Spine);
            controller.BodyObjects.Hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            
            controller.FeetAudioSource = new GameObject("FeetAudio").AddComponent<AudioSource>();
            controller.FeetAudioSource.transform.parent = CharacterModel.transform;
            controller.FeetAudioSource.transform.localPosition = Vector3.zero;

            controller.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
        }

        void CreateBodyColliders()
        {
            RagdollHelper.GetBones(bodyParameters, CharacterModel.GetComponent<Animator>());
            RagdollHelper.SetBodyParts(bodyParameters, CharacterModel.GetComponent<Controller>());

            RagdollHelper.CreateColliders(bodyParameters);
        }

       
    }
}
