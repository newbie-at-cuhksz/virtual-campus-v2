﻿// GercStudio
// © 2018-2019

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    public static class Helper
    {
#if UNITY_EDITOR
        [InitializeOnLoad]
        public static class SelectObjectWhenLoadScene
        {

            // constructor
            static SelectObjectWhenLoadScene()
            {
                EditorSceneManager.sceneOpened += SceneOpenedCallback;
            }

            static void SceneOpenedCallback(Scene _scene, OpenSceneMode _mode)
            {
                if (_scene.name == "Adjustment Scene")
                {
                    Selection.activeObject = Object.FindObjectOfType<Adjustment>();
                }
            }
        }
#endif
        
//        [MenuItem ("Export/MyExport")]
//        static void export()
//        {
//            AssetDatabase.ExportPackage (AssetDatabase.GetAllAssetPaths(),PlayerSettings.productName + ".unitypackage",ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
//        }
        
        public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
        {
            public AnimationClipOverrides(int capacity) : base(capacity)
            {
            }

            public AnimationClip this[string name]
            {
                get { return Find(x => x.Key.name.Equals(name)).Value; }
                set
                {
                    int index = FindIndex(x => x.Key.name.Equals(name));
                    if (index != -1)
                        this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
                }
            }
        }
        
        public static int layerMask()
        {
            var layerMask = ~ (LayerMask.GetMask("Character") | LayerMask.GetMask("Grass") | LayerMask.GetMask("Head") | LayerMask.GetMask("Objective") | LayerMask.GetMask("Weapon"));
            return layerMask;
        }
        

        public enum NextPointAction
        {
            NextPoint,RandomPoint,ClosestPoint,Stop
        }

        public enum RotationAxes
        {
            X, Y, Z
        }

        [Serializable]
        public class EnemyInGameManager
        {
            public EnemyController enemyPrefab;
            public int count;
            public float spawnTimeout;
            public float currentTime;
            public int currentSpawnMethodIndex;
            public int currentSpawnCount;
            public SpawnZone spawnZone;
            public MovementBehavior movementBehavior;
            public bool spawnConstantly;
        }

        [Serializable]
        public class CharacterInGameManager
        {
            public GameObject characterPrefab;
            public SpawnZone spawnZone;
        }
        
        
        public enum GamepadAxes
        {
            XAxis,
            YAxis,
            _3rdAxis, _4thAxis, _5thAxis, _6thAxis, _7thAxis, _8thAxis, _9thAxis, _10thAxis, _11thAxis, _12thAxis, 
            _13thAxis, _14thAxis, _15thAxis, _16thAxis, _17thAxis, _18thAxis, _19thAxis,
        }

        public enum KeyBoardCodes
        {
            LeftMouseButton,
            RightMouseButton,
            MiddleMouseButton, 
            Q, W, E, R, T, Y, U, I, O, P, A, S, D, F, G, H, J, K, L, Z, X, C, V, B, N, M, _1, _2, _3, _4, _5, _6, _7, _8, _9, _0,
            Space,
            Backspace,
            LeftShift,
            RightShift,
            LeftCtrl,
            RightCtrl,
            LeftAlt,
            RightAlt,
            Tab,
            Escape, 
            UpArrow, DownArrow, LeftArrow, RightArrow
        }

        public enum AxisButtonValue
        {
            Plus, Minus, Both
        }

        public enum GamepadCodes
        {
            JoystickButton0,
            JoystickButton1,
            JoystickButton2,
            JoystickButton3,
            JoystickButton4,
            JoystickButton5,
            JoystickButton6,
            JoystickButton7,
            JoystickButton8,
            JoystickButton9,
            JoystickButton10,
            JoystickButton11,
            JoystickButton12,
            JoystickButton13,
            JoystickButton14,
            JoystickButton15,
            JoystickButton16,
            JoystickButton17,
            JoystickButton18,
            JoystickButton19,
            XAxis,
            YAxis,
            _3rdAxis,
            _4thAxis,
            _5thAxis,
            _6thAxis,
            _7thAxis,
            _8thAxis,
            _9thAxis,
            _10thAxis,
            _11thAxis,
            _12thAxis,
            _13thAxis,
            _14thAxis,
            _15thAxis,
            _16thAxis,
            _17thAxis,
            _18thAxis,
            _19thAxis,
        }
        
        
        public enum CubeSolid
        {
            Solid, Wire 
        }

        public static void ConvertGamepadCodes(ref KeyCode gamepadKeys, GamepadCodes gamepadCodes)
        {
            switch (gamepadCodes)
            {
                case GamepadCodes.JoystickButton0:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton0;
                    break;
                case GamepadCodes.JoystickButton1:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton1;
                    break;
                case GamepadCodes.JoystickButton2:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton2;
                    break;
                case GamepadCodes.JoystickButton3:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton3;
                    break;
                case GamepadCodes.JoystickButton4:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton4;
                    break;
                case GamepadCodes.JoystickButton5:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton5;
                    break;
                case GamepadCodes.JoystickButton6:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton6;
                    break;
                case GamepadCodes.JoystickButton7:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton7;
                    break;
                case GamepadCodes.JoystickButton8:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton8;
                    break;
                case GamepadCodes.JoystickButton9:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton9;
                    break;
                case GamepadCodes.JoystickButton10:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton10;
                    break;
                case GamepadCodes.JoystickButton11:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton11;
                    break;
                case GamepadCodes.JoystickButton12:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton12;
                    break;
                case GamepadCodes.JoystickButton13:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton14:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton15:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton16:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton17:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton18:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton19:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                
            }
        }

        public static void ConvertKeyCodes(ref KeyCode keyboardKeys, KeyBoardCodes keyBoardCodes)
        {
            switch (keyBoardCodes)
            {
                case KeyBoardCodes.RightMouseButton:
                    keyboardKeys = KeyCode.Mouse1;
                    break;
                case KeyBoardCodes.LeftMouseButton:
                    keyboardKeys = KeyCode.Mouse0;
                    break;
                case KeyBoardCodes.MiddleMouseButton:
                    keyboardKeys = KeyCode.Mouse2;
                    break;
                case KeyBoardCodes.Q:
                    keyboardKeys = KeyCode.Q;
                    break;
                case KeyBoardCodes.W:
                    keyboardKeys = KeyCode.W;
                    break;
                case KeyBoardCodes.E:
                    keyboardKeys = KeyCode.E;
                    break;
                case KeyBoardCodes.R:
                    keyboardKeys = KeyCode.R;
                    break;
                case KeyBoardCodes.T:
                    keyboardKeys = KeyCode.T;
                    break;
                case KeyBoardCodes.Y:
                    keyboardKeys = KeyCode.Y;
                    break;
                case KeyBoardCodes.U:
                    keyboardKeys = KeyCode.U;
                    break;
                case KeyBoardCodes.I:
                    keyboardKeys = KeyCode.I;
                    break;
                case KeyBoardCodes.O:
                    keyboardKeys = KeyCode.O;
                    break;
                case KeyBoardCodes.P:
                    keyboardKeys = KeyCode.P;
                    break;
                case KeyBoardCodes.A:
                    keyboardKeys = KeyCode.A;
                    break;
                case KeyBoardCodes.S:
                    keyboardKeys = KeyCode.S;
                    break;
                case KeyBoardCodes.D:
                    keyboardKeys = KeyCode.D;
                    break;
                case KeyBoardCodes.F:
                    keyboardKeys = KeyCode.F;
                    break;
                case KeyBoardCodes.G:
                    keyboardKeys = KeyCode.G;
                    break;
                case KeyBoardCodes.H:
                    keyboardKeys = KeyCode.H;
                    break;
                case KeyBoardCodes.J:
                    keyboardKeys = KeyCode.J;
                    break;
                case KeyBoardCodes.K:
                    keyboardKeys = KeyCode.K;
                    break;
                case KeyBoardCodes.L:
                    keyboardKeys = KeyCode.L;
                    break;
                case KeyBoardCodes.Z:
                    keyboardKeys = KeyCode.Z;
                    break;
                case KeyBoardCodes.X:
                    keyboardKeys = KeyCode.X;
                    break;
                case KeyBoardCodes.C:
                    keyboardKeys = KeyCode.C;
                    break;
                case KeyBoardCodes.V:
                    keyboardKeys = KeyCode.V;
                    break;
                case KeyBoardCodes.B:
                    keyboardKeys = KeyCode.B;
                    break;
                case KeyBoardCodes.N:
                    keyboardKeys = KeyCode.N;
                    break;
                case KeyBoardCodes.M:
                    keyboardKeys = KeyCode.M;
                    break;
                case KeyBoardCodes._0:
                    keyboardKeys = KeyCode.Alpha0;
                    break;
                case KeyBoardCodes._1:
                    keyboardKeys = KeyCode.Alpha1;
                    break;
                case KeyBoardCodes._2:
                    keyboardKeys = KeyCode.Alpha2;
                    break;
                case KeyBoardCodes._3:
                    keyboardKeys = KeyCode.Alpha3;
                    break;
                case KeyBoardCodes._4:
                    keyboardKeys = KeyCode.Alpha4;
                    break;
                case KeyBoardCodes._5:
                    keyboardKeys = KeyCode.Alpha5;
                    break;
                case KeyBoardCodes._6:
                    keyboardKeys = KeyCode.Alpha6;
                    break;
                case KeyBoardCodes._7:
                    keyboardKeys = KeyCode.Alpha7;
                    break;
                case KeyBoardCodes._8:
                    keyboardKeys = KeyCode.Alpha8;
                    break;
                case KeyBoardCodes._9:
                    keyboardKeys = KeyCode.Alpha9;
                    break;
                case KeyBoardCodes.Space:
                    keyboardKeys = KeyCode.Space;
                    break;
                case KeyBoardCodes.Backspace:
                    keyboardKeys = KeyCode.Backspace;
                    break;
                case KeyBoardCodes.LeftShift:
                    keyboardKeys = KeyCode.LeftShift;
                    break;
                case KeyBoardCodes.RightShift:
                    keyboardKeys = KeyCode.RightShift;
                    break;
                case KeyBoardCodes.LeftCtrl:
                    keyboardKeys = KeyCode.LeftControl;
                    break;
                case KeyBoardCodes.RightCtrl:
                    keyboardKeys = KeyCode.RightControl;
                    break;
                case KeyBoardCodes.LeftAlt:
                    keyboardKeys = KeyCode.LeftAlt;
                    break;
                case KeyBoardCodes.RightAlt:
                    keyboardKeys = KeyCode.RightAlt;
                    break;
                case KeyBoardCodes.Tab:
                    keyboardKeys = KeyCode.Tab;
                    break;
                case KeyBoardCodes.Escape:
                    keyboardKeys = KeyCode.Escape;
                    break;
                case KeyBoardCodes.UpArrow:
                    keyboardKeys = KeyCode.UpArrow;
                    break;
                case KeyBoardCodes.DownArrow:
                    keyboardKeys = KeyCode.DownArrow;
                    break;
                case KeyBoardCodes.LeftArrow:
                    keyboardKeys = KeyCode.LeftArrow;
                    break;
                case KeyBoardCodes.RightArrow:
                    keyboardKeys = KeyCode.RightArrow;
                    break;
            }
        }



        public static void ConvertAxes(ref string axis, GamepadAxes gamepadAxes)
        {
            switch (gamepadAxes)
                {
                    case GamepadAxes.XAxis:
                        axis = "Gamepad Horizontal";
                        break;
                    case GamepadAxes.YAxis:
                        axis = "Gamepad Vertical";
                        break;
                    case GamepadAxes._3rdAxis:
                        axis = "Gamepad 3rd axis";
                        break;
                    case GamepadAxes._4thAxis:
                        axis = "Gamepad 4th axis";
                        break;
                    case GamepadAxes._5thAxis:
                        axis = "Gamepad 5th axis";
                        break;
                    case GamepadAxes._6thAxis:
                        axis = "Gamepad 6th axis";
                        break;
                    case GamepadAxes._7thAxis:
                        axis = "Gamepad 7th axis";
                        break;
                    case GamepadAxes._8thAxis:
                        axis = "Gamepad 8th axis";
                        break;
                    case GamepadAxes._9thAxis:
                        axis = "Gamepad 9th axis";
                        break;
                    case GamepadAxes._10thAxis:
                        axis = "Gamepad 10th axis";
                        break;
                    case GamepadAxes._11thAxis:
                        axis = "Gamepad 11th axis";
                        break;
                    case GamepadAxes._12thAxis:
                        axis = "Gamepad 12th axis";
                        break;
                    case GamepadAxes._13thAxis:
                        axis = "Gamepad 13th axis";
                        break;
                    case GamepadAxes._14thAxis:
                        axis = "Gamepad 14th axis";
                        break;
                    case GamepadAxes._15thAxis:
                        axis = "Gamepad 15th axis";
                        break;
                    case GamepadAxes._16thAxis:
                        axis = "Gamepad 16th axis";
                        break;
                    case GamepadAxes._17thAxis:
                        axis = "Gamepad 17th axis";
                        break;
                    case GamepadAxes._18thAxis:
                        axis = "Gamepad 18th axis";
                        break;
                    case GamepadAxes._19thAxis:
                        axis = "Gamepad 19th axis";
                        break;
                }
        }

        public static void ConvertAxes(ref string axis, GamepadCodes gamepadCodes)
        {
            switch (gamepadCodes)
            {
                case GamepadCodes.XAxis:
                    axis = "Gamepad Horizontal";
                    break;
                case GamepadCodes.YAxis:
                    axis = "Gamepad Vertical";
                    break;
                case GamepadCodes._3rdAxis:
                    axis = "Gamepad 3rd axis";
                    break;
                case GamepadCodes._4thAxis:
                    axis = "Gamepad 4th axis";
                    break;
                case GamepadCodes._5thAxis:
                    axis = "Gamepad 5th axis";
                    break;
                case GamepadCodes._6thAxis:
                    axis = "Gamepad 6th axis";
                    break;
                case GamepadCodes._7thAxis:
                    axis = "Gamepad 7th axis";
                    break;
                case GamepadCodes._8thAxis:
                    axis = "Gamepad 8th axis";
                    break;
                case GamepadCodes._9thAxis:
                    axis = "Gamepad 9th axis";
                    break;
                case GamepadCodes._10thAxis:
                    axis = "Gamepad 10th axis";
                    break;
                case GamepadCodes._11thAxis:
                    axis = "Gamepad 11th axis";
                    break;
                case GamepadCodes._12thAxis:
                    axis = "Gamepad 12th axis";
                    break;
                case GamepadCodes._13thAxis:
                    axis = "Gamepad 13th axis";
                    break;
                case GamepadCodes._14thAxis:
                    axis = "Gamepad 14th axis";
                    break;
                case GamepadCodes._15thAxis:
                    axis = "Gamepad 15th axis";
                    break;
                case GamepadCodes._16thAxis:
                    axis = "Gamepad 16th axis";
                    break;
                case GamepadCodes._17thAxis:
                    axis = "Gamepad 17th axis";
                    break;
                case GamepadCodes._18thAxis:
                    axis = "Gamepad 18th axis";
                    break;
                case GamepadCodes._19thAxis:
                    axis = "Gamepad 19th axis";
                    break;
            }
        }

        public static bool[] ButtonsStatus(int size)
        {
            var array = new List<bool>();
            
            for (var i = 0; i < size; i++)
            {
                array.Add(true);
            }

            return array.ToArray();
        }
        

        public static Color32[] colors =
        {
            new Color32(255, 190, 0, 255),
            new Color32(188, 140, 0, 255),
            new Color32(0, 67, 255, 255)
        };
       

        public struct ClipPlanePoints
        {
            public Vector3 UpperRight;
            public Vector3 UpperLeft;
            public Vector3 LowerRight;
            public Vector3 LowerLeft;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;

                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        public static ClipPlanePoints NearPoints(Vector3 pos, Camera camera)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;

            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }

        public static bool HasParameter(string paramName, Animator animator)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName) return true;
            }

            return false;
        }

        public static Vector3 MoveObjInNewPosition(Vector3 position,Vector3 newPosition, float Speed)
        {
            var x = Mathf.Lerp(position.x, newPosition.x, Speed);
            var y = Mathf.Lerp(position.y, newPosition.y, Speed);
            var z = Mathf.Lerp(position.z, newPosition.z, Speed);
            
            var newPos = new Vector3(x, y, z);

            return newPos;
        }

        public static void CopyTransformsRecurse(Transform src, Transform dst)
        {
            dst.position = src.position;
            dst.rotation = src.rotation;

            foreach (Transform child in dst)
            {
                var curSrc = src.Find(child.name);
                if (curSrc)
                    CopyTransformsRecurse(curSrc, child);
            }
        }

        public static int GetRandomIndex(ref int index, int count)
        {
            var tempIndex = Random.Range(0, count);
            
            if (tempIndex == index)
            {
                tempIndex++;

                if (tempIndex > count - 1)
                    tempIndex = 0;
            }

            index = tempIndex;

            return tempIndex;
        }

        public static void ChangeLayersRecursively(Transform trans, string name)
        {
            if(name == "Character" && trans.gameObject.layer == 11)
                return;

            trans.gameObject.layer = LayerMask.NameToLayer(name);
            
            foreach (Transform child in trans)
            {
                ChangeLayersRecursively(child, name);
                
//                child.gameObject.layer = LayerMask.NameToLayer(name);
            }
        }

        public static float ConvertAngle(float angle)
        {
            if (angle < 0)
            {
                angle += 360;

                if (angle > 360 - 50)
                {
                    angle -= 360;
                    angle = Mathf.Abs(angle);
                }
            }

            return angle;
        }

        public static float AngleBetween(Vector3 direction1, Vector3 direction2)
        {
            if (direction1 == Vector3.zero || direction2 == Vector3.zero) return 0;
            
            var dir1 = Quaternion.LookRotation(direction1);
            var dir1Angle = dir1.eulerAngles.y;
            if (dir1Angle > 180)
                dir1Angle -= 360;

            var dir2 = Quaternion.LookRotation(direction2);
            var dir2Angle = dir2.eulerAngles.y;
            if (dir2Angle > 180)
                dir2Angle -= 360;

            var middleAngle = Mathf.DeltaAngle(dir1Angle, dir2Angle);
            
            return middleAngle;
        }

        public static Vector2 AngleBetween(Vector3 direction1, Transform obj)
        {
            var look1 = Quaternion.LookRotation(direction1);

            var dir1AngleY = look1.eulerAngles.y;
            if (dir1AngleY > 180)
                dir1AngleY -= 360;
            
            var dir2AngleY = obj.eulerAngles.y;
            if (dir2AngleY > 180)
                dir2AngleY -= 360;

            var middleAngleY = Mathf.DeltaAngle(dir1AngleY, dir2AngleY);

            var dir1AngleX = look1.eulerAngles.x;
            if (dir1AngleX > 180)
                dir1AngleX -= 360;

            var dir2AngleX = obj.eulerAngles.x;
            if (dir2AngleX > 180)
                dir2AngleX -= 360;

            var middleAngleX = Mathf.DeltaAngle(dir1AngleX, dir2AngleX);

            return new Vector2(middleAngleX, middleAngleY);
        }

        public static bool ReachedPositionAndRotation(Vector3 position1, Vector3 position2, Vector3 angles1, Vector3 angles2)
        {
            return Math.Abs(position1.x - position2.x) < 0.2f && Math.Abs(position1.y - position2.y) < 0.2f && Math.Abs(position1.z - position2.z) < 0.2f &&
                   Math.Abs(angles1.x - angles2.x) < 0.2f && Math.Abs(angles1.y - angles2.y) < 0.2f && Math.Abs(angles1.z - angles2.z) < 0.2f;
        }
        
        public static bool ReachedPositionAndRotationAccurate(ref Vector3 position1, Vector3 position2, ref Vector3 angles1, Vector3 angles2)
        {
            var delX = Math.Abs(angles1.x - angles2.x);
            var delY = Math.Abs(angles1.y - angles2.y);
            var delZ = Math.Abs(angles1.z - angles2.z);

            if (delX > 180)
                delX -= 360;
            
            if (delY > 180)
                delY -= 360;
            
            if (delZ > 180)
                delZ -= 360;
            
            
            if (Math.Abs(position1.x - position2.x) < 0.01f && Math.Abs(position1.y - position2.y) < 0.01f && Math.Abs(position1.z - position2.z) < 0.01f && Math.Abs(delX) < 1 && Math.Abs(delY) < 1 && Math.Abs(delZ) < 1)
            {
//                position1 = position2;
//                angles1 = angles2;

                return true;
            }

            return false;
        }
        
        public static bool ReachedPositionAndRotation(Vector3 position1, Vector3 position2)
        {
            return Math.Abs(position1.x - position2.x) < 0.1f && Math.Abs(position1.y - position2.y) < 0.1f && Math.Abs(position1.z - position2.z) < 0.1f;
        }


        public static Camera NewCamera(string name, Transform parent, string type)
        {
            Camera camera = new GameObject(name).AddComponent<Camera>();

            if (type != "GameManager")
            {
                camera.cullingMask = 1 << 8;
                camera.depth = 1;
                camera.clearFlags = CameraClearFlags.Depth;
                camera.nearClipPlane = 0.01f;
            }

            camera.transform.parent = parent;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.Euler(0, 0, 0);

            return camera;
        }

        public static void ChangeColor(Button button, Color normColor, Sprite sprite)
        {
            switch (button.transition)
            {
                case Selectable.Transition.ColorTint:
                    var colors = button.colors;
                    colors.normalColor = normColor;
                    button.colors = colors;
                    break;
                case Selectable.Transition.SpriteSwap:
                    if (sprite)
                        button.GetComponent<Image>().sprite = sprite;
                    break;
            }
        }

        public static void ChangeButtonColor (UIManager uiManager, int slot, string type)
        {
            if (type != "norm")
            {
                if(uiManager.CharacterUI.Inventory.WeaponsButtons[slot])
                    ChangeColor(uiManager.CharacterUI.Inventory.WeaponsButtons[slot], uiManager.CharacterUI.Inventory.WeaponsButtons[slot].colors.highlightedColor , uiManager.CharacterUI.Inventory.WeaponsButtons[slot].spriteState.highlightedSprite);
            }
            else
            {
                if(uiManager.CharacterUI.Inventory.WeaponsButtons[slot])
                    ChangeColor(uiManager.CharacterUI.Inventory.WeaponsButtons[slot], uiManager.CharacterUI.Inventory.normButtonsColors[slot], uiManager.CharacterUI.Inventory.normButtonsSprites[slot]);
            }
        }

        public static void AddButtonsEvents(Button[] buttons, InventoryManager manager, Controller controller)
        {
            if (buttons[0])
            {
                buttons[0].onClick.AddListener(manager.UIAim);
//                buttons[0].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[5]);
            }

            if (buttons[1])
            {
                buttons[1].onClick.AddListener(manager.UIReload);
//                buttons[1].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[4]);
            }

            if (buttons[2])
            {
                buttons[2].onClick.AddListener(controller.ChangeCameraType);
//                buttons[2].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[11]);
            }

            if (buttons[3])
            {
                buttons[3].onClick.AddListener(manager.UIChangeAttackType);
//                buttons[3].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[19]);
            }

            if (buttons[4])
            {
                buttons[4].onClick.AddListener(delegate { manager.DropWeapon(true); });
//                buttons[4].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[9]);
            }

            if (buttons[8])
            {
                buttons[8].onClick.AddListener(controller.Jump);
//                buttons[8].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[2]);
            }

            if(buttons[11])
                buttons[11].onClick.AddListener(manager.UIPickUp);

            if (buttons[13])
            {
                buttons[13].onClick.AddListener(manager.WeaponDown);
//                buttons[13].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[16]);
            }

            if (buttons[14])
            {
                buttons[14].onClick.AddListener(manager.WeaponUp);
//                buttons[14].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[16]);
            }

            if (buttons[5])
            {
                addEventTriger(buttons[5].gameObject, manager, controller, "Attack");
//                buttons[5].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[3]);
            }

            if (buttons[6])
            {
                if (controller.projectSettings.PressSprintButton)
                    addEventTriger(buttons[6].gameObject, manager, controller, "Sprint");
                else buttons[6].onClick.AddListener(delegate { controller.Sprint(true, "click"); });
                
//                buttons[6].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[0]);
            }

            if (buttons[7])
            {
                if (controller.projectSettings.PressCrouchButton)
                    addEventTriger(buttons[7].gameObject, manager, controller, "Crouch");
                else buttons[7].onClick.AddListener(delegate { controller.Crouch(true, "click"); });
                
//                buttons[7].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[1]);
            }

            if (buttons[10])
            {
                if (manager.pressInventoryButton) addEventTriger(buttons[10].gameObject, manager, controller, "Inventory");
                else buttons[10].onClick.AddListener(manager.UIInventory);
                
//                buttons[10].gameObject.SetActive(controller.projectSettings.ButtonsActivityStatuses[7]);
            }
        }

        public static void addEventTriger(GameObject button, InventoryManager manager, Controller controller, string type)
        {
            var eventTrigger = button.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};

            switch (type)
            {
                case "Attack":
                    entry.callback.AddListener(data => { manager.UIAttack(); });
                    break;
                case "Sprint":
                    entry.callback.AddListener(data => { controller.Sprint(true, "press"); });
                    break;
                case "Crouch":
                    entry.callback.AddListener(data => { controller.Crouch(true, "press"); });
                    break;
                case "Inventory":
                    entry.callback.AddListener(data => { manager.UIActivateInventory(); });
                    break;
            }
            eventTrigger.triggers.Add(entry);
            entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerUp};

            switch (type)
            {
                case "Attack":
                    entry.callback.AddListener(data => { manager.UIEndAttack(); });
                    break;
                case "Sprint":
                    entry.callback.AddListener(data => { controller.Sprint(false, "press"); });
                    break;
                case "Crouch":
                    entry.callback.AddListener(data => { controller.Crouch(false, "press"); });
                    break;
                case "Inventory":
                    entry.callback.AddListener(data => { manager.UIDeactivateInventory(); });
                    break;
            }
            
            eventTrigger.triggers.Add(entry);
        }
        
        
        public static Transform NewObject(Transform parent, string name, PrimitiveType type, Color color, float size, CubeSolid mode)
        {
            color = new Color(color.r, color.g, color.b, 0);
            
            var sourse = GameObject.CreatePrimitive(type).transform;
            sourse.name = name;
            sourse.hideFlags = HideFlags.HideInHierarchy;
            sourse.GetComponent<MeshRenderer>().enabled = false;
            sourse.GetComponent<MeshRenderer>().material = NewMaterial(color);
            sourse.localScale = new Vector3(size / 100, size / 100, size / 100);
            sourse.parent = parent;
            sourse.localPosition = Vector3.zero;
            sourse.localRotation = Quaternion.Euler(Vector3.zero);
            ChangeLayersRecursively(sourse, "Character");

            return sourse;
        }
        
        public static Transform NewObject(Transform parent, string name)
        {
            var sourse = new GameObject(name).transform;
            sourse.hideFlags = HideFlags.HideInHierarchy;
            sourse.parent = parent;
            sourse.localPosition = Vector3.zero;
            sourse.localRotation = Quaternion.Euler(Vector3.zero);
            ChangeLayersRecursively(sourse, "Character");
            return sourse;
        }
        
        public static Material NewMaterial(Color color)
        {
            var mat = new Material(Shader.Find("Standard")) {name = "Standard", color = color};
            ChangeShaderMode.ChangeRenderMode(mat, ChangeShaderMode.BlendMode.Fade);
            return mat;
        }

        public static string GenerateRandomString(int count)
        {
            const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";

            var name = "";
            
            for (int i = 0; i < count; i++)
            {
                name += glyphs[Random.Range(0, glyphs.Length)];
            }

            return name;
        }

      

        public static bool CheckGamepadAxisButton(int number, string[] _gamepadButtonsAxes, bool[] hasAxisButtonPressed, string type, AxisButtonValue value)
        {
            if (_gamepadButtonsAxes[number] != "")
            {
                if (type == "GetKeyDown")
                {
                    switch (value)
                    {
                        case AxisButtonValue.Both:
                        {
                            if ((Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f || Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f) && !hasAxisButtonPressed[number])
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                        }

                        case AxisButtonValue.Plus:
                        {
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f && !hasAxisButtonPressed[number])
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                        }

                        case AxisButtonValue.Minus:
                        {
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f && !hasAxisButtonPressed[number])
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                        }
                    }

                    if (Input.GetAxis(_gamepadButtonsAxes[number]) < 0.1f && Input.GetAxis(_gamepadButtonsAxes[number]) > -0.1f && hasAxisButtonPressed[number])
                    {
                        hasAxisButtonPressed[number] = false;
                        return false;
                    }
                }
                else
                {
                    switch (value)
                    {
                        case AxisButtonValue.Both:
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f || Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f)
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;

                        case AxisButtonValue.Plus:
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f)
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;

                        case AxisButtonValue.Minus:

                            if (Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f)
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                    }


                    if (Math.Abs(Input.GetAxis(_gamepadButtonsAxes[number])) < 0.2f && Math.Abs(Input.GetAxis(_gamepadButtonsAxes[number])) > -0.2f)
                    {
                        hasAxisButtonPressed[number] = false;
                        return false;
                    }
                }
            }

            return false;
        }

        public static void CreateNoiseCollider(Transform parent, Controller script)
        {
            var noiseCollider = new GameObject("Noise Collider");
            noiseCollider.transform.parent = parent;
            noiseCollider.transform.localPosition= Vector3.zero;
            noiseCollider.transform.localEulerAngles = Vector3.zero;

            script.noiseCollider = noiseCollider.AddComponent<SphereCollider>();
            script.noiseCollider.isTrigger = true;
            var rb = noiseCollider.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            noiseCollider.hideFlags = HideFlags.HideInHierarchy;
        }

        public static void EnableAllParents(GameObject childObject)
        {
            var t = childObject.transform;
            childObject.SetActive(true);
            
            while (t.parent != null)
            {
                t.parent.gameObject.SetActive(true);
                t = t.parent;
            }
        }
        
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
  
        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static void SetLineRenderer (ref LineRenderer lineRenderer, GameObject parent, Material trailMaterial)
        {
            lineRenderer = parent.AddComponent<LineRenderer>();
            lineRenderer.enabled = false;
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.material = trailMaterial;
            lineRenderer.positionCount = 50;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            var curve = new AnimationCurve();
            curve.AddKey(0, 0.5f);
            curve.AddKey(1, 1);
            lineRenderer.widthCurve = curve;
        }

        public static void ManageBodyColliders(List<Transform> BodyParts, Controller controller)
        {
            ManageBodyCollider(BodyParts[0], controller, BodyPartCollider.BodyPart.Body, controller.bodyMultiplier);
            ManageBodyCollider(BodyParts[1], controller, BodyPartCollider.BodyPart.Body, controller.bodyMultiplier);
            ManageBodyCollider(BodyParts[2], controller, BodyPartCollider.BodyPart.Head, controller.headMultiplier);
            ManageBodyCollider(BodyParts[3], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[4], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[5], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[6], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[7], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            ManageBodyCollider(BodyParts[8], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            ManageBodyCollider(BodyParts[9], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            ManageBodyCollider(BodyParts[10], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            
            if(BodyParts[0])
                BodyParts[0].GetComponent<BodyPartCollider>().checkColliders = true;
        }
        
        public static void ManageBodyColliders(List<Transform> BodyParts, EnemyController controller)
        {
            ManageBodyCollider(BodyParts[0], controller, BodyPartCollider.BodyPart.Body, controller.bodyMultiplier);
            ManageBodyCollider(BodyParts[1], controller, BodyPartCollider.BodyPart.Body, controller.bodyMultiplier);
            ManageBodyCollider(BodyParts[2], controller, BodyPartCollider.BodyPart.Head, controller.headMultiplier);
            ManageBodyCollider(BodyParts[3], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[4], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[5], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[6], controller, BodyPartCollider.BodyPart.Legs, controller.legsMultiplier);
            ManageBodyCollider(BodyParts[7], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            ManageBodyCollider(BodyParts[8], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            ManageBodyCollider(BodyParts[9], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            ManageBodyCollider(BodyParts[10], controller, BodyPartCollider.BodyPart.Hands, controller.handsMultiplier);
            
            if(BodyParts[0])
                BodyParts[0].GetComponent<BodyPartCollider>().checkColliders = true;
        }

        public static void ManageBodyColliders(List<AIHelper.GenericCollider> colliders, EnemyController controller)
        {
            for (var i = 0; i < colliders.Count; i++)
            {
                ManageBodyCollider(colliders[i].collider.transform, controller, colliders[i].damageMultiplier, i == 0);
            }
        }
        

        private static void ManageBodyCollider(Transform bodyPart, Controller controller, BodyPartCollider.BodyPart bodyPartType, float multiplier)
        {
            if(!bodyPart)
                return;

            var script = bodyPart.gameObject.AddComponent<BodyPartCollider>();
            script.Controller = controller;
            script.bodyPart = bodyPartType;
            script.damageMultiplayer = multiplier;
            bodyPart.GetComponent<Rigidbody>().isKinematic = true;
        }
        
        private static void ManageBodyCollider(Transform bodyPart, EnemyController controller, BodyPartCollider.BodyPart bodyPartType, float multiplier)
        {
            if(!bodyPart)
                return;
            
            var script = bodyPart.gameObject.AddComponent<BodyPartCollider>();
            controller.allBodyColliders.Add(script);
            script.EnemyController = controller;
            script.bodyPart = bodyPartType;
            script.damageMultiplayer = multiplier;
            bodyPart.GetComponent<Rigidbody>().isKinematic = true;
        }

        private static void ManageBodyCollider(Transform collider, EnemyController controller, float multiplier, bool firstCollider)
        {
            if(!collider)
                return;
            
            var script = collider.gameObject.AddComponent<BodyPartCollider>();
            if (firstCollider)
                script.checkColliders = true;
            
            controller.allBodyColliders.Add(script);
            script.EnemyController = controller;
            script.damageMultiplayer = multiplier;
        }
        
        public static Canvas NewCanvas(string name, Vector2 size, Transform parent)
        {
            var canvas = new GameObject(name);
            canvas.AddComponent<RectTransform>();
            var _canvas = canvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = size;
            canvas.AddComponent<GraphicRaycaster>();
            canvas.transform.SetParent(parent);

            return _canvas;
        }

#if UNITY_EDITOR
        
         public static void CreateRagdoll (List<Transform> BodyParts, Animator animator)
        {
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
                
                BodyParts[0] = SetFieldValue(ragdollWindow, "pelvis", animator.GetBoneTransform(HumanBodyBones.Hips));
                BodyParts[1] = SetFieldValue(ragdollWindow, "middleSpine", animator.GetBoneTransform(HumanBodyBones.Spine));
                BodyParts[2] = SetFieldValue(ragdollWindow, "head", animator.GetBoneTransform(HumanBodyBones.Head));
                BodyParts[3] = SetFieldValue(ragdollWindow, "leftHips", animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
                BodyParts[4] = SetFieldValue(ragdollWindow, "leftKnee", animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
                SetFieldValue(ragdollWindow, "leftFoot", animator.GetBoneTransform(HumanBodyBones.LeftFoot));
                BodyParts[5] = SetFieldValue(ragdollWindow, "rightHips", animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
                BodyParts[6] = SetFieldValue(ragdollWindow, "rightKnee", animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
                SetFieldValue(ragdollWindow, "rightFoot", animator.GetBoneTransform(HumanBodyBones.RightFoot));
                BodyParts[7] = SetFieldValue(ragdollWindow, "leftArm", animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
                BodyParts[8] = SetFieldValue(ragdollWindow, "leftElbow", animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
                BodyParts[9] = SetFieldValue(ragdollWindow, "rightArm", animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
                BodyParts[10] = SetFieldValue(ragdollWindow, "rightElbow", animator.GetBoneTransform(HumanBodyBones.RightLowerArm));

                var method = ragdollWindow.GetType().GetMethod("CheckConsistency", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
               
                if (method != null)
                {
                    ragdollWindow.errorString = (string) method.Invoke(ragdollWindow, null);
                    ragdollWindow.isValid = string.IsNullOrEmpty(ragdollWindow.errorString);
                }
                
            }
        }
         
        private static Transform SetFieldValue(ScriptableWizard obj, string name, Transform value)
        {
            if (value == null)
            {
                return null;
            }

            var field = obj.GetType().GetField(name);
            if (field != null)
            { 
                field.SetValue(obj, value);
            }

            return value;
        }
        
        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Vector3.one);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (_height - (_radius * 2)) / 2;
 
                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
 
            }
        }
        
        [MenuItem("Tools/USK/Adjust")]
        public static void OpenAdjustmentScene()
        {
            if (SceneManager.GetActiveScene().name != "Adjustment Scene")
            {
                var inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
                inputs.oldScenePath = SceneManager.GetActiveScene().path;
                inputs.oldSceneName = SceneManager.GetActiveScene().name;
                
                if(EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] {SceneManager.GetActiveScene()}))
                    EditorSceneManager.OpenScene("Assets/Universal Shooter Kit/Tools/Assets/_Scenes/Adjustment Scene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("Edit/USK Project Settings/Input")]
        public static void Inputs()
        {
            var inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
            Selection.activeObject = inputs;
            EditorGUIUtility.PingObject(inputs);
        }
        
        [MenuItem("Edit/USK Project Settings/UI")]
        public static void UI()
        {
            var ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI Manager.prefab", typeof(UIManager)) as UIManager;
            Selection.activeObject = ui;
            EditorGUIUtility.PingObject(ui);
            
        }
        
        [MenuItem("GameObject/USK/Movement Behavior (AI)", false, 20)]
        public static void CreateWaypointBehavior()
        {
            var behavior = new GameObject("Movement Behaviour");
            behavior.AddComponent<MovementBehavior>();
            Selection.activeObject = behavior;
        }
        
        [MenuItem("GameObject/USK/Spawn Zone", false, 10)]
        public static void CreateSpawnZone()
        {
            var zone = new GameObject("Spawn Zone");
            zone.AddComponent<SpawnZone>();
            
            if (SceneView.lastActiveSceneView)
            {
                var transform = SceneView.lastActiveSceneView.camera.transform;
                zone.transform.position = transform.position + transform.forward * 10;
            }
            
            EditorGUIUtility.PingObject(zone);
        }


        [MenuItem("GameObject/USK/Game Manger", false, 1)]
        public static void CreateGameManger()
        {
            var manager = new GameObject("GameManager");
            var script = manager.AddComponent<GameManager>();
            
            script.defaultCamera = NewCamera("Default camera", manager.transform, "GameManager");

            var eventManager = new GameObject("EventSystem");
            eventManager.AddComponent<EventSystem>();
            eventManager.AddComponent<StandaloneInputModule>();
            eventManager.transform.parent = manager.transform;
            eventManager.transform.localPosition = Vector3.zero;
            eventManager.transform.localEulerAngles = Vector3.zero;

            Selection.activeObject = manager;
        }

#if PHOTON_UNITY_NETWORKING
        [MenuItem("GameObject/USK/Room Manager (Multiplayer)", false, 150)]
        public static void CreateRoomManager()
        {
            var manager = new GameObject("Room Manager");
            var script = manager.AddComponent<RoomManager>();
            
            script.DefaultCamera = NewCamera("Default camera", manager.transform, "GameManager");
            Object.DestroyImmediate(script.DefaultCamera.GetComponent<AudioListener>());
        }
        
        [MenuItem("GameObject/USK/Capture Point (Multiplayer)", false, 160)]
        public static void CreateCapturePoint()
        {
            var point = new GameObject("Capture Point");
            point.AddComponent<CapturePoint>();

            var canvas = point.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var image = point.AddComponent<RawImage>();
            image.raycastTarget = false;

            point.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
            point.gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(90, 0, 0);

            if (SceneView.lastActiveSceneView)
            {
                var transform = SceneView.lastActiveSceneView.camera.transform;
                point.transform.position = transform.position + transform.forward * 10;
            }
            
            EditorGUIUtility.PingObject(point);
        }
#endif

        public static void AddObjectIcon(GameObject obj, string icon)
        {
            var image = (Texture2D)Resources.Load(icon);
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { obj, image };
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
        
        public static GameObject CreateWayPoint()
        {
            var foundObjects = GameObject.FindGameObjectsWithTag("WayPoint");
            var curPointNumber = 0;

            foreach (var obj in foundObjects)
            {
                obj.name = "Waypoint " + curPointNumber;
                curPointNumber++;
            }

            var waypoint = new GameObject("Waypoint " + curPointNumber);
            waypoint.tag = "WayPoint";
            AddObjectIcon(waypoint, "DefaultWaypoint");

            if (SceneView.lastActiveSceneView)
            {
                var transform = SceneView.lastActiveSceneView.camera.transform;
                waypoint.transform.position = transform.position + transform.forward * 10;
            }
            
            Selection.activeObject = waypoint;
            EditorGUIUtility.PingObject(waypoint);
            
            return waypoint;
        }

        public static Transform NewPoint(GameObject parent, string name)
        {
            var point = new GameObject(name).transform;
            point.parent = parent.transform;
            point.localPosition = Vector3.zero;
            point.localRotation = Quaternion.Euler(Vector3.zero);
            point.localScale = Vector3.one;
            EditorUtility.SetDirty(parent.GetComponent<WeaponController>());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            return point;
        }

        public static BoxCollider NewCollider(string name, string tag, Transform parent)
        {
            var collider = new GameObject {name = name, tag = tag};
            var boxCollider = collider.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
				
            var rigidbody = collider.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            collider.transform.parent = parent;
            collider.transform.localPosition = Vector3.zero;
            collider.transform.localScale = Vector3.one;
            return boxCollider;

        }

        static RawImage NewImagePlace(string name, Transform parent, Transform parent2, Vector2 size)
        {
            var image = NewUIElement(name, parent, Vector2.zero, size, Vector3.one);

            var raw = image.AddComponent<RawImage>();
            raw.color = new Color(1, 1, 1, 0);

            raw.raycastTarget = false;

            image.transform.SetParent(parent2);

            return raw;
        }

        public static Image NewImage(string name, Transform parent, Vector2 size, Vector2 position)
        {
            var image = NewUIElement(name, parent, position, size, Vector3.one);

            var img = image.AddComponent<Image>();
            img.raycastTarget = false;

            return img;
        }


        static Button NewInventoryPart(string name, string type, Transform parent, Vector2 position, Vector2 size,
            Vector3 rotation,
            Sprite image)
        {
            var part = NewUIElement(name, parent, position, size, Vector3.one);

            var img = part.AddComponent<Image>();
            img.sprite = image;
            img.color = new Color32(255, 255, 255, 1);

            part.AddComponent<Mask>();
            part.AddComponent<RaycastMask>();

            var button = NewButton("Button", new Vector2(0, 0), new Vector2(200, 200), Vector3.one, image,
                part.transform);

            var colors = button.colors;

            colors.normalColor = new Color32(0, 67, 255, 170);

            if (type == "wheel")
                colors.highlightedColor = new Color32(255, 190, 0, 190);
            else
                colors.normalColor = new Color32(0, 67, 255, 170);
            
            colors.pressedColor = new Color32(223, 166, 0, 190);

            button.colors = colors;

            part.GetComponent<RectTransform>().eulerAngles = rotation;

            return button;
        }

        public static GameObject NewText(string name, Transform parent, Vector2 position, Vector2 size,
            string textContent, Font font, int textSize, TextAnchor textAlignment, Color textColor, bool needOutline)
        {
            var textObject = NewUIElement(name, parent, position, size, Vector3.one);
            
            var text = textObject.AddComponent<Text>();
            text.text = textContent;
            
            if(font)
                text.font = font;
            
            text.fontSize = textSize;
            text.alignment = textAlignment;
            text.color = textColor;

            if (!needOutline) return textObject;
            
            var outline = textObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, -1);

            return textObject;
        }
        
        public static Button NewButton(string name, Vector2 position, Vector2 size, Vector3 scale, Sprite sprite, Transform parent)
        {
            var button = NewUIElement(name, parent, position, size, scale);
            var image = button.AddComponent<Image>();
            image.sprite = sprite;
            var _button = button.AddComponent<Button>();
            
            return _button;
        }
        
        public static Button NewButton(string name, Vector2 position, Vector2 size, Vector3 scale, Color32[] colors, Transform parent)
        {
            var button = NewUIElement(name, parent, position, size, scale);
           // var image = button.AddComponent<Image>();
            
            var _button = button.AddComponent<Button>();

            var buttonColors = _button.colors;
            buttonColors.normalColor = colors[0];
            buttonColors.highlightedColor = colors[1];
            buttonColors.pressedColor = colors[2];
            _button.colors = buttonColors;
            
            return _button;
        }

        public static Canvas NewCanvas(string name, Vector2 size)
        {
            var canvas = new GameObject(name);
            canvas.AddComponent<RectTransform>();
            var _canvas = canvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = size;
            canvas.AddComponent<GraphicRaycaster>();

            return _canvas;
        }
        
        public static void SetAndStretchToParentSize(RectTransform obj, RectTransform parent, bool fullScreen)
        {
            obj.anchoredPosition = parent.position;
            if(!fullScreen)
                obj.anchorMin = new Vector2(0, 0);
            else obj.anchorMin = new Vector2(-1, -1);
            obj.anchorMax = new Vector2(1, 1);
            obj.pivot = new Vector2(0.5f, 0.5f);
            obj.sizeDelta = Vector2.zero;
            //obj.transform.SetParent(parent);
        }

        public static Canvas NewCanvas(string name, Transform parent)
        {
            var canvas = new GameObject(name);
            canvas.AddComponent<RectTransform>();
            var _canvas = canvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            canvas.AddComponent<GraphicRaycaster>();
            canvas.transform.SetParent(parent);
            return _canvas;
        }

        public static GameObject newCrosshairPart(string name, Vector2 positions, Vector2 size, GameObject parent)
        {
            GameObject crosshiarPart = NewUIElement(name, parent.transform, positions, size, Vector3.one);
            crosshiarPart.AddComponent<Image>().color = Color.white;
            Outline outline = crosshiarPart.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, -1);

            return crosshiarPart;
        }

#endif
        
        public static GameObject NewUIElement(string name, Transform parent, Vector2 position, Vector2 size, Vector3 scale)
        {
            var element = new GameObject(name);
            element.transform.SetParent(parent);
            var rectTransform = element.AddComponent<RectTransform>();
            element.AddComponent<CanvasRenderer>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = size;
            element.transform.localScale = scale;
            element.layer = 5;
            
            return element;
        }
        
        public static void HideIKObjects(bool value, HideFlags flag, Transform obj)
        {

            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.enabled = !value;
            
            obj.hideFlags = flag;

            if (value)
            {
                if (obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(false);
                }
            }
            else
            {
                //renderer.material = NewMaterial(color, mode);
                    
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                }
            }
        }
        
        public static void HideAllObjects(WeaponsHelper.IKObjects IkObjects)
        {
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightObject);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftObject);

            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightAimObject);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftAimObject);

            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightWallObject);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftWallObject);

            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightElbowObject);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftElbowObject);
            
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightCrouchObject);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftCrouchObject);
        }

        public static void CreateObjects(WeaponsHelper.IKObjects IkObjects, Transform parent, bool adjusment, bool hide, float size, CubeSolid mode)
        {
            IkObjects.RightObject = NewObject(parent, "Right Hand Object", PrimitiveType.Cube, Color.red, size, mode);
            Object.Destroy(IkObjects.RightObject.GetComponent<BoxCollider>());
            if(!adjusment && hide)
                Object.Destroy(IkObjects.RightObject.GetComponent<MeshRenderer>());

            IkObjects.LeftObject = NewObject(parent, "Left Hand Object", PrimitiveType.Cube, Color.red, size, mode);
            Object.Destroy(IkObjects.LeftObject.GetComponent<BoxCollider>());
            if(!adjusment && hide)
                Object.Destroy(IkObjects.LeftObject.GetComponent<MeshRenderer>());

            IkObjects.RightAimObject = NewObject(parent, "Right Aim Object", PrimitiveType.Cube, Color.blue, size, mode);
            Object.Destroy(IkObjects.RightAimObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightAimObject.GetComponent<MeshRenderer>());

            IkObjects.LeftAimObject = NewObject(parent, "Left Aim Object", PrimitiveType.Cube, Color.blue, size, mode);
            Object.Destroy(IkObjects.LeftAimObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftAimObject.GetComponent<MeshRenderer>());
            
            IkObjects.RightCrouchObject = NewObject(parent, "Right Crouch Object", PrimitiveType.Cube, Color.magenta, size, mode);
            Object.Destroy(IkObjects.RightCrouchObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightCrouchObject.GetComponent<MeshRenderer>());

            IkObjects.LeftCrouchObject = NewObject(parent, "Left Crouch Object", PrimitiveType.Cube, Color.magenta, size, mode);
            Object.Destroy(IkObjects.LeftCrouchObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftCrouchObject.GetComponent<MeshRenderer>());


            IkObjects.RightWallObject = NewObject(parent, "Right Hand Wall Object", PrimitiveType.Cube, Color.yellow, size, mode);
            Object.Destroy(IkObjects.RightWallObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightWallObject.GetComponent<MeshRenderer>());


            IkObjects.LeftWallObject = NewObject(parent, "Left Hand Wall Object", PrimitiveType.Cube, Color.yellow, size, mode);
            Object.Destroy(IkObjects.LeftWallObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftWallObject.GetComponent<MeshRenderer>());


            IkObjects.RightElbowObject = NewObject(parent, "Right Elbow Object", PrimitiveType.Sphere, Color.green, size, mode);
            Object.Destroy(IkObjects.RightElbowObject.GetComponent<SphereCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightElbowObject.GetComponent<MeshRenderer>());


            IkObjects.LeftElbowObject = NewObject(parent, "Left Elbow Object", PrimitiveType.Sphere, Color.green, size, mode);
            Object.Destroy(IkObjects.LeftElbowObject.GetComponent<SphereCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftElbowObject.GetComponent<MeshRenderer>());

        }

        public static void HandsIK(Controller controller, WeaponController weaponController, InventoryManager weaponManager, Transform LeftIKObject, Transform RightIKObject, Transform leftParent, Transform rightParent, float value, bool pinObj)
        {
            var L_ikObj = LeftIKObject;
            var R_ikObj = RightIKObject;

            if (weaponController.CanUseElbowIK && !weaponController.CurrentWeaponInfo[weaponController.settingsSlotIndex].disableElbowIK)
            {
                controller.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, value);
                controller.anim.SetIKHintPosition(AvatarIKHint.LeftElbow, weaponController.IkObjects.LeftElbowObject.position);

                controller.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, value);
                controller.anim.SetIKHintPosition(AvatarIKHint.RightElbow, weaponController.IkObjects.RightElbowObject.position);
            }
            
            if (weaponController.setHandsPositionsAim && weaponController.setHandsPositionsObjectDetection && weaponController.setHandsPositionsCrouch && value >= 1)
            {
                if (controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && controller.anim.GetBool("HasWeaponTaken") || controller.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson)
                {
                    R_ikObj.parent = rightParent;

                    if (!pinObj || weaponController.numberOfUsedHands == 1 || weaponController.isShotgun && controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
                    {
                        L_ikObj.parent = leftParent;
                    }
                    else
                    {
                        L_ikObj.parent = R_ikObj;
                    }
                }

            }
           
            if (value >= 0)
            {
                controller.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, value);
                controller.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, value);
            
                controller.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, value);
                controller.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, value);
                
                controller.anim.SetIKPosition(AvatarIKGoal.RightHand, R_ikObj.position);
                controller.anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(R_ikObj.eulerAngles));

                controller.anim.SetIKPosition(AvatarIKGoal.LeftHand, L_ikObj.position);
                controller.anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(L_ikObj.eulerAngles));
            }
        }

        private static void FingersRotation(Animator anim, float angle, HumanBodyBones finger, Vector3 axis)
        {
            anim.SetBoneLocalRotation(finger, anim.GetBoneTransform(finger).localRotation *= Quaternion.AngleAxis(angle, axis));
        }
        
        public static void FingersRotate(WeaponsHelper.WeaponInfo weaponInfo, Animator anim, string type)
        {
            if (type == "Weapon")
            {
                var leftAngleX = weaponInfo.FingersLeftX;
                var rightAngleX = weaponInfo.FingersRightX;

                var leftAngleY = weaponInfo.FingersLeftY;
                var rightAngleY = weaponInfo.FingersRightY;

                var leftAngleZ = weaponInfo.FingersLeftZ;
                var rightAngleZ = weaponInfo.FingersRightZ;

                var leftThumbAngleX = weaponInfo.ThumbLeftX;
                var rightThumbAngleX = weaponInfo.ThumbRightX;

                var leftThumbAngleY = weaponInfo.ThumbLeftY;
                var rightThumbAngleY = weaponInfo.ThumbRightY;

                var leftThumbAngleZ = weaponInfo.ThumbLeftZ;
                var rightThumbAngleZ = weaponInfo.ThumbRightZ;
                
                RotateFingersByAxis("X", leftAngleX, rightAngleX, leftThumbAngleX, rightThumbAngleX, anim, "Weapon");
                RotateFingersByAxis("Y", leftAngleY, rightAngleY, leftThumbAngleY, rightThumbAngleY, anim, "Weapon");
                RotateFingersByAxis("Z", leftAngleZ, rightAngleZ, leftThumbAngleZ, rightThumbAngleZ, anim, "Weapon");
            }
            else if (type == "Null")
            {
                RotateFingersByAxis("X", 0, 0, 0, 0, anim, "Reload");
                RotateFingersByAxis("Y", 0, 0, 0, 0, anim, "Reload");
                RotateFingersByAxis("Z", 0, 0, 0, 0, anim, "Reload");
            }
            else if  (type == "Grenade")
            {
                var leftAngleX = weaponInfo.FingersLeftX;
                var leftAngleY = weaponInfo.FingersLeftY;
                var leftAngleZ = weaponInfo.FingersLeftZ;
                
                var leftThumbAngleX = weaponInfo.ThumbLeftX;
                var leftThumbAngleY = weaponInfo.ThumbLeftY;
                var leftThumbAngleZ = weaponInfo.ThumbLeftY;

                RotateFingersByAxis("X", leftAngleX, 0, leftThumbAngleX, 0, anim, "Grenade");
                RotateFingersByAxis("Y", leftAngleY, 0, leftThumbAngleY, 0, anim, "Grenade");
                RotateFingersByAxis("Z", leftAngleZ, 0, leftThumbAngleZ, 0, anim, "Grenade");
            }
        }

        private static void RotateFingersByAxis(string axis, float leftAngle, float rightAngle, float leftThumbAngle,
            float rightThumbAngle, Animator anim, string type)
        {

            var axs = new Vector3();

            switch (axis)
            {
                case "X":
                    axs = Vector3.right;
                    break;
                case "Y":
                    axs = Vector3.up;
                    break;
                case "Z":
                    axs = Vector3.forward;
                    break;
            }

            if (type != "Grenade")
            {
                if (anim.GetBoneTransform(HumanBodyBones.RightIndexProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightIndexProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightIndexIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightIndexDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightIndexDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightRingProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightRingProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightRingIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightRingDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightRingDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightMiddleProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightMiddleIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightMiddleDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightLittleProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightLittleProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightLittleIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightLittleDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightLittleDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightThumbProximal))
                    FingersRotation(anim, rightThumbAngle, HumanBodyBones.RightThumbProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate))
                    FingersRotation(anim, rightThumbAngle, HumanBodyBones.RightThumbIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightThumbDistal))
                    FingersRotation(anim, rightThumbAngle, HumanBodyBones.RightThumbDistal, axs);

            }


            //left fingers

            if (anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftIndexProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftIndexIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftIndexDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftRingProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftRingProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftRingIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftRingDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftRingDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftMiddleProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftMiddleIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftMiddleDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftLittleProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftLittleIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftLittleDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal))
                FingersRotation(anim, leftThumbAngle, HumanBodyBones.LeftThumbProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate))
                FingersRotation(anim, leftThumbAngle, HumanBodyBones.LeftThumbIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal))
                FingersRotation(anim, leftThumbAngle, HumanBodyBones.LeftThumbDistal, axs);

        }

        public static class ChangeShaderMode
        {
            public enum BlendMode
            {
                Opaque,
                Cutout,
                Fade,
                Transparent
            }

            public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
            {
                switch (blendMode)
                {
                    case BlendMode.Opaque:
                        standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                        standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                        standardShaderMaterial.SetInt("_ZWrite", 1);
                        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        standardShaderMaterial.renderQueue = -1;
                        break;
                    case BlendMode.Cutout:
                        standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                        standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                        standardShaderMaterial.SetInt("_ZWrite", 1);
                        standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        standardShaderMaterial.renderQueue = 2450;
                        break;
                    case BlendMode.Fade:
                        standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                        standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        standardShaderMaterial.SetInt("_ZWrite", 0);
                        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                        standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        standardShaderMaterial.renderQueue = 3000;
                        break;
                    case BlendMode.Transparent:
                        standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                        standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        standardShaderMaterial.SetInt("_ZWrite", 0);
                        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                        standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        standardShaderMaterial.renderQueue = 3000;
                        break;
                }

            }
        }

        public static string GetHtmlFromUri(string resource)
        {
            var html = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(resource);
            try
            {
                using (var resp = (HttpWebResponse)req.GetResponse())
                {
                    var isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                    if (isSuccess)
                    {
                        using (var reader = new StreamReader(resp.GetResponseStream()))
                        {
                            var cs = new char[80];
                            reader.Read(cs, 0, cs.Length);
                            foreach(var ch in cs)
                            {
                                html +=ch;
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
            return html;
        }
        
        public static bool CheckConnection(string URL)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
 
                if (response.StatusCode == HttpStatusCode.OK) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }


        // Copyright 2014 Jarrah Technology (http://www.jarrahtechnology.com). All Rights Reserved. 
        public static class CameraExtensions {

            public static void LayerCullingShow(Camera cam, int layerMask) {
                cam.cullingMask |= layerMask;
            }

            public static void LayerCullingShow(Camera cam, string layer) {
                LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static void LayerCullingHide(Camera cam, int layerMask) {
                cam.cullingMask &= ~layerMask;
            }

            public static void LayerCullingHide(Camera cam, string layer) {
                LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static void LayerCullingToggle(Camera cam, int layerMask) {
                cam.cullingMask ^= layerMask;
            }

            public static void LayerCullingToggle(Camera cam, string layer) {
                LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static bool LayerCullingIncludes(Camera cam, int layerMask) {
                return (cam.cullingMask & layerMask) > 0;
            }

            public static bool LayerCullingIncludes(Camera cam, string layer) {
                return LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static void LayerCullingToggle(Camera cam, int layerMask, bool isOn) {
                var included = LayerCullingIncludes(cam, layerMask);
                if (isOn && !included) {
                    LayerCullingShow(cam, layerMask);
                } else if (!isOn && included) {
                    LayerCullingHide(cam, layerMask);
                }
            }

            public static void LayerCullingToggle(Camera cam, string layer, bool isOn) {
                LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);
            }
        }
        //
    }
}


