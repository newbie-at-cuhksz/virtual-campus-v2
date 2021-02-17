using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : Editor
    {
        public Controller script;
        private Animator _animator;

        private ReorderableList bloodHoles;

        private CameraController camera;
        private InventoryManager manager;

        public void Awake()
        {
            script = (Controller) target;
            _animator = script.gameObject.GetComponent<Animator>();
        }


        void OnEnable()
        {
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

            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {

            if (script)
                if (!manager)
                    manager = script.GetComponent<InventoryManager>();
//
//            foreach (var col in script.BodyParts)
//            {
//                if(col.gameObject.GetComponent<BodyPartCollider>())
//                    DestroyImmediate(col.gameObject.GetComponent<BodyPartCollider>());
//            }

            if (Application.isPlaying)
            {
                if (!camera && script.thisCamera)
                    camera = script.CameraController;
            }
            else
            {
                script.PlayerHealthPercent = script.PlayerHealth;

                if (script && !script.characterAnimatorController)
                {
                    script.characterAnimatorController = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/Assets/_Animator Controllers/Accurate.controller", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }

                if (script && !script.projectSettings)
                {
                    script.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }


                if (script && !script.FeetAudioSource)
                {
                    var tempCharacter = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);

                    var controller = tempCharacter.GetComponent<Controller>();

                    controller.FeetAudioSource = new GameObject("FeetAudio").AddComponent<AudioSource>();
                    controller.FeetAudioSource.transform.parent = tempCharacter.transform;
                    controller.FeetAudioSource.transform.localPosition = Vector3.zero;

#if !UNITY_2018_3_OR_NEWER
                    PrefabUtility.ReplacePrefab(tempCharacter, PrefabUtility.GetPrefabParent(tempCharacter), ReplacePrefabOptions.ConnectToPrefab);
#else
                    PrefabUtility.SaveAsPrefabAssetAndConnect(tempCharacter, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCharacter), InteractionMode.AutomatedAction);
#endif

                    DestroyImmediate(tempCharacter);

                    EditorUtility.SetDirty(script.gameObject);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }

//                if (script && !script.thisCamera.GetComponent<CameraController>().upPart && !script.gameObject.activeInHierarchy)
//                {
//                    var tempCharacter = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
//
//                    var cameraController = tempCharacter.GetComponent<Controller>().thisCamera.GetComponent<CameraController>();
//
//                    var parts = CharacterHelper.CreateCrosshair(tempCharacter.GetComponent<InventoryManager>().canvas.transform);
//                    cameraController.Crosshair = parts[0].transform;
//
//                    cameraController.upPart = parts[1].GetComponent<Image>();
//                    cameraController.downPart = parts[2].GetComponent<Image>();
//                    cameraController.rightPart = parts[3].GetComponent<Image>();
//                    cameraController.leftPart = parts[4].GetComponent<Image>();
//                    cameraController.middlePart = parts[5].GetComponent<Image>();
//
//#if !UNITY_2018_3_OR_NEWER
//                    PrefabUtility.ReplacePrefab(tempCharacter, PrefabUtility.GetPrefabParent(tempCharacter), ReplacePrefabOptions.ConnectToPrefab);
//#else
//                    PrefabUtility.SaveAsPrefabAssetAndConnect(tempCharacter, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCharacter), InteractionMode.AutomatedAction);
//#endif
//
//                    DestroyImmediate(tempCharacter);
//
//                    EditorUtility.SetDirty(script.gameObject);
//                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//                }

                if (script.characterTag > script.projectSettings.CharacterTags.Count - 1)
                {
                    script.characterTag = script.projectSettings.CharacterTags.Count - 1;
                }

                if (_animator)
                {
                    if (_animator.isHuman)
                    {
                        if (!script.BodyObjects.RightHand)
                        {
                            script.BodyObjects.RightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.LeftHand)
                        {
                            script.BodyObjects.LeftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.Head)
                        {
                            script.BodyObjects.Head = _animator.GetBoneTransform(HumanBodyBones.Head);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.TopBody)
                        {
                            script.BodyObjects.TopBody = _animator.GetBoneTransform(HumanBodyBones.Spine);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.Hips)
                        {
                            script.BodyObjects.Hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.Chest)
                        {
                            script.BodyObjects.Chest = _animator.GetBoneTransform(HumanBodyBones.Chest);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("If you are going to use this character in multiplayer mode, make sure that it is in the [Resources] folder.", MessageType.Info);
            EditorGUILayout.Space();

            if (!script.BodyParts[0] || script.BodyParts[0] && !script.BodyParts[0].GetComponent<Rigidbody>())
            {
                EditorGUILayout.HelpBox("Generate Body Colliders in the [Health] tab.", MessageType.Info);
                EditorGUILayout.Space();
            }

            script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop, new[] {"Camera", "Movement"});

            switch (script.inspectorTabTop)
            {
                case 0:
                    script.currentInspectorTab = 0;
                    script.inspectorTabDown = 3;
                    break;


                case 1:
                    script.currentInspectorTab = 1;
                    script.inspectorTabDown = 3;
                    break;
            }

            script.inspectorTabDown = GUILayout.Toolbar(script.inspectorTabDown, new[] {"Health", "Other Parameters"});

            switch (script.inspectorTabDown)
            {
                case 0:
                    script.currentInspectorTab = 2;
                    script.inspectorTabTop = 3;
                    break;

                case 1:
                    script.currentInspectorTab = 3;
                    script.inspectorTabTop = 3;
                    break;
            }



            switch (script.currentInspectorTab)
            {
                case 0:

                    EditorGUILayout.Space();
                    script.cameraInspectorTab = GUILayout.Toolbar(script.cameraInspectorTab, new[] {"Third Person", "First Person", "Top Down"});

                    switch (script.cameraInspectorTab)
                    {
                        case 0:

                            if (!Application.isPlaying)
                                script.TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;

                            script.moveInspectorTab = 0;
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.BeginVertical("HelpBox");
                            script.CameraParameters.activeTP = EditorGUILayout.ToggleLeft("Active", script.CameraParameters.activeTP);

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpXMouseSensitivity"), new GUIContent("X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpYMouseSensitivity"), new GUIContent("Y Sensitivity"));

                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpAimDepth"), new GUIContent("Aim depth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpAimXMouseSensitivity"), new GUIContent("(Aim) X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpAimYMouseSensitivity"), new GUIContent("(Aim) Y Sensitivity"));

                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpXLimitMax"), new GUIContent("Min X"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpXLimitMin"), new GUIContent("Max X"));

                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpSmoothX"), new GUIContent("X Smooth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpSmoothY"), new GUIContent("Y Smooth"));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("When the character walks, the camera moves away from him.", MessageType.Info);
                            script.SmoothCameraWhenMoving = EditorGUILayout.ToggleLeft("Smooth Camera", script.SmoothCameraWhenMoving);
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("The camera follows the character’s head.", MessageType.Info);
                            script.CameraFollowCharacter = EditorGUILayout.ToggleLeft("Bobbing", script.CameraFollowCharacter);
                            EditorGUILayout.EndVertical();

//                            EditorGUILayout.LabelField("Bobbing", EditorStyles.boldLabel);
//                            EditorGUILayout.BeginVertical("HelpBox");
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.TPIdleCurve"), new GUIContent("Idle"));
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.TPWalkCurve"), new GUIContent("Walk"));
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.TPRunCurve"), new GUIContent("Run"));
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.TPCrouchCurve"), new GUIContent("Crouch"));
//                            EditorGUILayout.Space();
//                            EditorGUILayout.HelpBox("You can adjust the camera bobbing for attacks in the [WeaponController] script that is on each weapon.", MessageType.Info);
//                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                            break;

                        case 1:

                            if (!Application.isPlaying)
                                script.TypeOfCamera = CharacterHelper.CameraType.FirstPerson;

                            script.moveInspectorTab = 1;
                            EditorGUILayout.Space();
                            
                            EditorGUILayout.HelpBox("Set the 'Head' layer for the character's head, then the camera will not see it.", MessageType.Info);
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.BeginVertical("HelpBox");
                            script.CameraParameters.activeFP = EditorGUILayout.ToggleLeft("Active", script.CameraParameters.activeFP);
                            EditorGUILayout.EndVertical();


                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpXMouseSensitivity"), new GUIContent("X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpYMouseSensitivity"), new GUIContent("Y Sensitivity"));


                            EditorGUILayout.Space();


                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpAimDepth"), new GUIContent("Aim depth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpAimXMouseSensitivity"), new GUIContent("(Aim) X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpAimYMouseSensitivity"), new GUIContent("(Aim) Y Sensitivity"));


                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpXLimitMin"), new GUIContent("Min X"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpXLimitMax"), new GUIContent("Max X"));


                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpXSmooth"), new GUIContent("X Smooth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fpYSmooth"), new GUIContent("Y Smooth"));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.HelpBox("If this checkbox is not active, the character’s hands very accurately follow the camera." + "\n\n" +
                                                    "If active, the hands movement is smoother (like in most first-person games).", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothWeapons"), new GUIContent("Smooth Weapons"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Bobbing", EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.FPIdleCurve"), new GUIContent("Idle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.FPWalkCurve"), new GUIContent("Walk"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.FPRunCurve"), new GUIContent("Run"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.FPCrouchCurve"), new GUIContent("Crouch"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();

                            break;

                        case 2:

                            if (!Application.isPlaying)
                                script.TypeOfCamera = CharacterHelper.CameraType.TopDown;

                            script.moveInspectorTab = 2;
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.BeginVertical("HelpBox");
                            script.CameraParameters.activeTD = EditorGUILayout.ToggleLeft("Active", script.CameraParameters.activeTD);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            script.CameraParameters.lockCamera = EditorGUILayout.ToggleLeft("Lock Camera", script.CameraParameters.lockCamera);
                            if (script.CameraParameters.lockCamera)
                            {
                                EditorGUILayout.BeginVertical("HelpBox");
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.CursorImage"), new GUIContent("Cursor Image"));
                                EditorGUILayout.Space();
                                
                                script.CameraParameters.lookAtCursor = EditorGUILayout.ToggleLeft("Aim at Cursor", script.CameraParameters.lookAtCursor);
                                EditorGUILayout.HelpBox("If this feature is active, the character will aim where the cursor is pointing." + "\n" +
                                                        "If not, the character will aim always forward.", MessageType.Info);
                                
                                if (script.CameraParameters.lookAtCursor)
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tdXLimitMin"), new GUIContent("Body Rotation(x) Min"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tdXLimitMax"), new GUIContent("Body Rotation(x) Max"));
                                }
                                
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                script.CameraParameters.alwaysTDAim = EditorGUILayout.ToggleLeft("Always Aim", script.CameraParameters.alwaysTDAim);
                                EditorGUILayout.HelpBox("If this feature is active, the character will always aim. " + "\n" +
                                                        "If not, the character will aim when the button is pressed (like in TP view).", MessageType.Info);
                            }

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tdXMouseSensitivity"), new GUIContent("Sensitivity"));

                            if (!script.CameraParameters.lockCamera)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tdSmoothX"), new GUIContent("Smooth"));
                            }

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                            break;
                    }

                    break;

                case 1:
                    EditorGUILayout.Space();

                    script.moveInspectorTab = GUILayout.Toolbar(script.moveInspectorTab, new[] {"Third Person", "First Person", "Top Down"});

                    switch (script.moveInspectorTab)
                    {
                        case 0:

                            script.cameraInspectorTab = 0;

                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical("HelpBox");


                            EditorGUILayout.HelpBox( /*"In this mode, transitions between animations are faster, character's movement more accurate." + "\n\n" +*/
                                "Movement in the Aim state is based on the Controller, other states are based on the Root Motion animations." + "\n\n" +
                                "You can adjust the movement and jump settings manually, and also speed multiplier for other animations.", MessageType.Info);

                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.LabelField("Animation speed multiplier for all states (without aim)");
                            script.TPspeedOffset = EditorGUILayout.Slider(script.TPspeedOffset, 0.1f, 2);
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical("HelpBox");
                            script.TPSpeedInspectorTab = GUILayout.Toolbar(script.TPSpeedInspectorTab, new[] {"Aim Walk", "Aim Run", "Jump"});
                            switch (script.TPSpeedInspectorTab)
                            {
                                case 0:
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.NormForwardSpeed"), new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.NormBackwardSpeed"), new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.NormLateralSpeed"), new GUIContent("Lateral speed"));
                                    break;

                                case 1:
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.RunForwardSpeed"), new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.RunBackwardSpeed"), new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.RunLateralSpeed"), new GUIContent("Lateral speed"));
                                    break;

                                case 2:
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.JumpHeight"), new GUIContent("Height"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TPSpeed.JumpSpeed"), new GUIContent("Speed"));
                                    EditorGUILayout.HelpBox("You can also change the gravity in [Edit -> Project Settings -> Physics -> Gravity (Y value)].", MessageType.Info);
                                    break;
                            }

                            EditorGUILayout.EndVertical();

                            EditorGUILayout.EndVertical();
                            break;

                        case 1:
                            EditorGUILayout.Space();

                            script.cameraInspectorTab = 1;

                            EditorGUILayout.BeginVertical("HelpBox");
                            script.inspectorSettingsTab = GUILayout.Toolbar(script.inspectorSettingsTab, new[] {"Walk", "Run", "Crouch", "Jump"});

                            switch (script.inspectorSettingsTab)
                            {
                                case 0:
//                                    EditorGUILayout.Space();

                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.NormForwardSpeed"), new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.NormBackwardSpeed"), new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.NormLateralSpeed"), new GUIContent("Lateral speed"));
                                    EditorGUILayout.EndVertical();
                                    break;
                                case 1:
//                                    EditorGUILayout.Space();
//                            script.activeSprint = EditorGUILayout.Toggle("Enabled", script.activeSprint);
//                            EditorGUILayout.Space();
//                            if (script.activeSprint)
//                            {

                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.RunForwardSpeed"),
                                        new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.RunBackwardSpeed"),
                                        new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.RunLateralSpeed"),
                                        new GUIContent("Lateral speed"));
                                    EditorGUILayout.EndVertical();
//                            }

//                            EditorGUILayout.Space();
                                    break;
                                case 2:

//                                    EditorGUILayout.Space();
//                            script.activeCrouch = EditorGUILayout.Toggle("Enabled", script.activeCrouch);
//                            EditorGUILayout.Space();
//                            if (script.activeCrouch)
//                            {

//                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.CrouchForwardSpeed"), new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.CrouchBackwardSpeed"), new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.CrouchLateralSpeed"), new GUIContent("Lateral speed"));
                                    EditorGUILayout.Space();

                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchHeight"), new GUIContent("Crouch depth"));
                                    EditorGUILayout.EndVertical();

//                            }
//                            EditorGUILayout.Space();
                                    break;
                                case 3:
//                                    EditorGUILayout.Space();
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("activeJump"),
//                                new GUIContent("Enabled"));
//                            EditorGUILayout.Space();
//                            if (script.activeJump)
//                            {
//                                    EditorGUILayout.BeginVertical("HelpBox");
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.JumpHeight"), new GUIContent("Height"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FPSpeed.JumpSpeed"), new GUIContent("Speed"));
                                    EditorGUILayout.HelpBox("You can also change the gravity in [Edit -> Project Settings -> Physics -> Gravity (Y value)].", MessageType.Info);
                                    EditorGUILayout.EndVertical();
//                            }

//                            EditorGUILayout.Space();
                                    break;
                            }

                            break;

                        case 2:

                            script.cameraInspectorTab = 2;

                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical("HelpBox");
//                            script.MovementType = (CharacterHelper.MovementType) EditorGUILayout.EnumPopup("Movement Mode", script.MovementType);
//                            switch (script.MovementType)
//                            {
//                                case CharacterHelper.MovementType.FastAndAccurate:
                            EditorGUILayout.HelpBox("All movement is based on the Controller." + "\n\n" +
                                "You can adjust the speed and jump settings manually.", MessageType.Info);

                            EditorGUILayout.Space();

                            script.TDSpeedInspectorTab = GUILayout.Toolbar(script.TDSpeedInspectorTab, new[] {"Walk", "Run", "Jump"});

                            switch (script.TDSpeedInspectorTab)
                            {
                                case 0:
//                                            EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.NormForwardSpeed"), new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.NormBackwardSpeed"), new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.NormLateralSpeed"), new GUIContent("Lateral speed"));

                                    break;
                                case 1:
//                                            EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.RunForwardSpeed"), new GUIContent("Forward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.RunBackwardSpeed"), new GUIContent("Backward speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.RunLateralSpeed"), new GUIContent("Lateral speed"));
                                    break;

                                case 2:
//                                            EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.JumpHeight"), new GUIContent("Height"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TDSpeed.JumpSpeed"), new GUIContent("Speed"));
                                    EditorGUILayout.HelpBox("You can also change the gravity in [Edit -> Project Settings -> Physics -> Gravity (Y value)].", MessageType.Info);
                                    break;
                            }

//                                    break;
//                                case CharacterHelper.MovementType.Realistic:
//                                    EditorGUILayout.HelpBox("In this mode, transitions between animations are slower, animator controller has more 'Start'/'Stop' states." + "\n\n" +
//                                                            "All movement is based on the Root Motion animations.", MessageType.Info);
//
//                                    break;
//                            }

                            EditorGUILayout.EndVertical();
                            break;
                    }

                    break;

                case 2:
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerHealth"), new GUIContent("Health Value"));
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    bloodHoles.DoLayoutList();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical("HelpBox");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerHealthBar"), new GUIContent("Health Bar"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerHealthBarBackground"), new GUIContent("Health Bar Background"));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    if (!script.BodyParts[0] || script.BodyParts[0] && !script.BodyParts[0].GetComponent<Rigidbody>())
                    {
#if !UNITY_2018_3_OR_NEWER
                        EditorGUILayout.HelpBox("Place this prefab in a scene to create the Body Colliders, then apply changes.", MessageType.Info);
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
                        EditorGUILayout.HelpBox("An enemy's weapon damage will be multiplied by these values.", MessageType.Info);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("headMultiplier"), new GUIContent("Head"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("bodyMultiplier"), new GUIContent("Body"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("handsMultiplier"), new GUIContent("Hands"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("legsMultiplier"), new GUIContent("Legs"));
                        EditorGUILayout.EndVertical();
                    }

                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("Ragdoll"), new GUIContent("Ragdoll"));
                    EditorGUILayout.EndVertical();

                    break;


                case 3:
                    EditorGUILayout.Space();

                    script.otherSettingsInspectorTab = GUILayout.Toolbar(script.otherSettingsInspectorTab, new[] {"Tag", "Noise"});

                    switch (script.otherSettingsInspectorTab)
                    {
                        case 0:

                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("HelpBox");
                            script.characterTag = EditorGUILayout.Popup("Character's Tags", script.characterTag, script.projectSettings.CharacterTags.ToArray());

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
                                    if (!script.projectSettings.CharacterTags.Contains(script.curName))
                                    {
                                        script.rename = false;
                                        script.projectSettings.CharacterTags[script.characterTag] = script.curName;
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

                            EditorGUI.BeginDisabledGroup(script.projectSettings.CharacterTags.Count <= 1);
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
                                    script.projectSettings.CharacterTags.Remove(script.projectSettings.CharacterTags[script.characterTag]);
                                    script.characterTag = script.projectSettings.CharacterTags.Count - 1;
                                    script.delete = false;
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                            }

                            EditorGUI.EndDisabledGroup();
                            EditorGUILayout.EndVertical();

                            if (GUILayout.Button("Add new tag"))
                            {
                                if (!script.projectSettings.CharacterTags.Contains("Character " + script.projectSettings.CharacterTags.Count))
                                    script.projectSettings.CharacterTags.Add("Character " + script.projectSettings.CharacterTags.Count);
                                else script.projectSettings.CharacterTags.Add("Character " + Random.Range(10, 100));

                                script.characterTag = script.projectSettings.CharacterTags.Count - 1;

                                EditorUtility.SetDirty(script);
                                EditorUtility.SetDirty(script.projectSettings);
                            }

                            break;

                        case 1:
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Noise Radius", EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("HelpBox");
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("IdleNoise"), new GUIContent("Idle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("MovementNoise"), new GUIContent("Walk"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintMovementNoise"), new GUIContent("Run"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchIdleNoise"), new GUIContent("Crouch Idle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchMovementNoise"), new GUIContent("Crouch Walk"));

                            EditorGUILayout.EndVertical();
                            break;
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

        void CreateRagdoll()
        {
            if (!script.gameObject.activeInHierarchy)
            {
                var tempCharacter = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                var controller = tempCharacter.GetComponent<Controller>();

                foreach (var part in controller.BodyParts)
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

                Helper.CreateRagdoll(controller.BodyParts, tempCharacter.GetComponent<Animator>());


#if !UNITY_2018_3_OR_NEWER
                PrefabUtility.ReplacePrefab(tempCharacter, PrefabUtility.GetPrefabParent(tempCharacter), ReplacePrefabOptions.ConnectToPrefab);
#else
				PrefabUtility.SaveAsPrefabAssetAndConnect(tempCharacter, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCharacter), InteractionMode.AutomatedAction);
#endif

                DestroyImmediate(tempCharacter);
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

                Helper.CreateRagdoll(script.BodyParts, script.gameObject.GetComponent<Animator>());

            }
        }
    }
}

