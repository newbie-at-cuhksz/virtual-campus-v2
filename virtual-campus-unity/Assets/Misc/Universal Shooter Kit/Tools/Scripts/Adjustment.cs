using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace GercStudio.USK.Scripts
{
	
	public class Adjustment : MonoBehaviour
	{
#if UNITY_EDITOR
		public List<Controller> Characters;
		public List<WeaponController> Weapons;
		
		public List<GameObject> CharactersPrefabs;
		public List<GameObject> WeaponsPrefabs;

		public List<int> CopyToList = new List<int> {1};
		public List<GameObject> hideObjects;
		public List<string> WeaponsNames;
		public List<Vector3> currentScales;
		public List<CharacterHelper.CharacterOffset> CurrentCharacterOffsets;
		public List<CharacterHelper.CameraOffset> CurrentCameraOffsets;

		public UIManager UIManager;

		public GameObject Background;
		//public GameObject Point;

		public Camera enemyCamera;
		public Camera menuCamera;

		public ProjectSettings settings;

		public CharacterHelper.CameraType oldCameraType;

		public Controller currentController;
		public WeaponController CurrentWeaponController;
		
		public SerializedObject SerializedWeaponController;

		public int copyFromIKState;
		public int copyFromSlot;
		public int copyFromWeaponSlot;
		public int enemyState;
		public int oldEnemyState;
		public int inspectorTab;
		public int oldInspectorTab;
		public int ikInspectorTab;

		public int generalInspectorTab;
		
		public int characterIndex = int.MaxValue;
		public int weaponIndex = int.MaxValue;
		public int enemyIndex = int.MaxValue;
		
//		public float animationValue;
//		public float takeGrenadeTime_FPS;
//		public float throwGrenadeTime_FPS;

		[Range(-180,180)]public float dirObjRotX;
		[Range(-180,180)]public float dirObjRotY;
		[Range(-180,180)]public float dirObjRotZ;
		
//		public float takeGrenadeTime_TPS;
//		public float throwGrenadeTime_TPS;

		public string[] IKStateNames = {"Norm", "Aim", "Wall", "Crouch"};
		
		public bool isPause;

		public bool hide;
//		public bool playGrenadeAnimation;
		public bool oldNormBoolValue;
		public bool oldCrouchBoolValue;
		public bool oldPinObjValue;

		public enum animType
		{
			FPS, TPS_TDS
		}

		public animType AnimType;

		public enum AdjustmentType
		{
			Enemy, Character
		}
		
		public AdjustmentType Type;
		
		public IKHelper.IkDebugMode oldDebugModeIndex;

		public void Awake()
		{
#if UNITY_EDITOR 
			foreach (var character in Characters.Where(character => character.gameObject))
			{
				if(character.gameObject.GetComponent<CharacterSync>())
					CharacterHelper.RemoveMultiplayerScripts(character.gameObject);
			}
#endif
		}

		public void Start()
		{
			weaponIndex = -1;
			CurrentWeaponController = null;
			
			Selection.activeObject = gameObject;
			
			Background.SetActive(false);

			foreach (var weapon in WeaponsPrefabs)
			{
				Destroy(weapon);
			}
			WeaponsPrefabs.Clear();

			oldInspectorTab = inspectorTab;

			var scripts = new List<WeaponController>();
			
			for (var i = 0; i < Weapons.Count; i++)
			{
				if (Weapons[i])
				{
					WeaponsPrefabs.Add(Instantiate(Weapons[i].gameObject));
					currentScales.Add(Vector3.one);
					WeaponsNames.Add(Weapons[i].name);
					WeaponsPrefabs[WeaponsPrefabs.Count - 1].transform.position = Vector3.zero;
					var tempWeapon = Weapons[i];
					Weapons[i] = WeaponsPrefabs[WeaponsPrefabs.Count - 1].GetComponent<WeaponController>();
					currentScales[currentScales.Count - 1] = WeaponsPrefabs[WeaponsPrefabs.Count - 1].transform.localScale;
					Weapons[i].settingsSlotIndex = 0;
					Weapons[i].gameObject.SetActive(false);
					WeaponsPrefabs[WeaponsPrefabs.Count - 1] = tempWeapon.gameObject;
				}
				else
				{
					scripts.Add(Weapons[i]);
				}
			}

			foreach (var script in scripts)
			{
				Weapons.Remove(script);
			}

			var characterControllers = new List<Controller>();
			
			for (var i = 0; i < Characters.Count; i++)
			{
				if (Characters[i])
				{
					characterIndex = 0;
					
					CharactersPrefabs.Add(Instantiate(Characters[i].gameObject));
					
					CharactersPrefabs[i].transform.position = Vector3.zero;
					var tempCharacter = Characters[i];

					Characters[i] = CharactersPrefabs[i].GetComponent<Controller>();
					Characters[i].gameObject.SetActive(false);
					Characters[i].gameObject.hideFlags = HideFlags.HideInHierarchy;

					Characters[i].thisCamera.SetActive(false);
					Characters[i].thisCamera.hideFlags = HideFlags.HideInHierarchy;
					
					var curCharOffset = new CharacterHelper.CharacterOffset();
					curCharOffset.Clone(Characters[i].CharacterOffset);
					CurrentCharacterOffsets.Add(curCharOffset);

					var curCamOffset = new CharacterHelper.CameraOffset();
					curCamOffset.Clone(Characters[i].CameraController.CameraOffset);
					CurrentCameraOffsets.Add(curCamOffset);

					if (Characters[i].gameObject.GetComponent<Controller>().UIManager.CharacterUI.WeaponAmmo)
						Characters[i].gameObject.GetComponent<Controller>().UIManager.CharacterUI.WeaponAmmo.gameObject.SetActive(false);

					CharactersPrefabs[i] = tempCharacter.gameObject;

					Characters[i].OriginalScript = CharactersPrefabs[i].GetComponent<Controller>();
					Characters[i].CameraController.OriginalScript = CharactersPrefabs[i].GetComponent<Controller>().thisCamera.GetComponent<CameraController>();
				}
				else
				{
					characterControllers.Add(Characters[i]);
				}
			}
			
			foreach (var script in characterControllers)
			{
				Characters.Remove(script);
			}
			
			if (Characters.Count > 0)
			{
				ActiveCharacter(0, false);
				menuCamera.gameObject.SetActive(false);
				inspectorTab = 0;
			}
//			else if (Enemies.Count > 0)
//			{
//				ActiveEnemy(0);
//				menuCamera.gameObject.SetActive(false);
//				upInspectorTab = 1;
//			}
			else
			{
				Debug.LogWarning("You should add any character to the Adjustment script");
				EditorApplication.isPlaying = false;
			}
			
			Pause();
		}

		public void LateUpdate()
		{
//			var allObjects = FindObjectsOfType<Object>();
//
//			foreach (var obj in allObjects)
//			{
//				obj.hideFlags = HideFlags.None;
//			}
			
			ActiveEditorTracker.sharedTracker.isLocked = true;

			if (characterIndex != -1 && Characters.Count > 0 && inspectorTab == 0)
				currentController = Characters[characterIndex];

//			if (enemyIndex != -1 && Enemies.Count > 0 && upInspectorTab == 1)
//			{
//				CurrentAiController = Enemies[enemyIndex];
//			}

//			if (upInspectorTab == 1 && Enemies.Count > 0)
//				Type = AdjustmentType.Enemy;
//			else
			if (inspectorTab == 0 && Characters.Count > 0)
				Type = AdjustmentType.Character;

			if (currentController)
			{
				currentController.DebugMode = isPause;
				currentController.isPause = isPause;

				if (currentController.DebugMode)
				{
					var center = currentController.CharacterController.center;
					center = new Vector3(center.x, -currentController.CharacterOffset.CharacterHeight, center.z);
					currentController.CharacterController.center = center;
					currentController.defaultCharacterCenter = -currentController.CharacterOffset.CharacterHeight;

					currentController.DirectionObject.localEulerAngles = new Vector3(dirObjRotX, dirObjRotY, dirObjRotZ);
				}
			}

			if (oldInspectorTab != inspectorTab)
			{
				if (inspectorTab == 0)// || upInspectorTab == 1)
				{
					if (CurrentWeaponController && CurrentWeaponController.ActiveDebug)
					{
//						if (CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
							Helper.HideAllObjects(CurrentWeaponController.IkObjects);
//						else
//						{
							//Characters[index].anim.CrossFade("Walk (without weapons)", 0.01f, 2);
//						}
					}

					switch (inspectorTab)
					{
						case 0:
						{
							ActiveCharacter(characterIndex, true);
							enemyCamera.gameObject.SetActive(false);
							break;
						}

						case 2:
						{
							if (currentController)
							{
								currentController.gameObject.SetActive(false);
								currentController.thisCamera.SetActive(false);
								
								currentController.gameObject.hideFlags = HideFlags.HideInHierarchy;
								currentController.thisCamera.hideFlags = HideFlags.HideInHierarchy;
							}
							enemyCamera.gameObject.SetActive(true);
							break;
						}
					}
				}

				oldInspectorTab = inspectorTab;
			}


			if (CurrentWeaponController && CurrentWeaponController.canUseValuesInAdjustment)
			{
				SetAllHandlesSize();

				var curInfo = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex];

				curInfo.WeaponSize = CurrentWeaponController.transform.localScale;
				curInfo.WeaponPosition = CurrentWeaponController.transform.localPosition;
				curInfo.WeaponRotation = CurrentWeaponController.transform.localEulerAngles;

				curInfo.RightHandPosition = CurrentWeaponController.IkObjects.RightObject.localPosition;
				curInfo.RightHandRotation = CurrentWeaponController.IkObjects.RightObject.localEulerAngles;

				curInfo.LeftHandPosition = CurrentWeaponController.IkObjects.LeftObject.localPosition;
				curInfo.LeftHandRotation = CurrentWeaponController.IkObjects.LeftObject.localEulerAngles;

				if (CurrentWeaponController.hasAimIKChanged)
				{
					curInfo.RightAimPosition = CurrentWeaponController.IkObjects.RightAimObject.localPosition;
					curInfo.RightAimRotation = CurrentWeaponController.IkObjects.RightAimObject.localEulerAngles;

					curInfo.LeftAimPosition = CurrentWeaponController.IkObjects.LeftAimObject.localPosition;
					curInfo.LeftAimRotation = CurrentWeaponController.IkObjects.LeftAimObject.localEulerAngles;
				}

				if (CurrentWeaponController.hasWallIKChanged)
				{
					curInfo.RightHandWallPosition = CurrentWeaponController.IkObjects.RightWallObject.localPosition;
					curInfo.RightHandWallRotation = CurrentWeaponController.IkObjects.RightWallObject.localEulerAngles;

					curInfo.LeftHandWallPosition = CurrentWeaponController.IkObjects.LeftWallObject.localPosition;
					curInfo.LeftHandWallRotation = CurrentWeaponController.IkObjects.LeftWallObject.localEulerAngles;
				}

				if (CurrentWeaponController.hasCrouchIKChanged)
				{
					curInfo.RightCrouchHandPosition = CurrentWeaponController.IkObjects.RightCrouchObject.localPosition;
					curInfo.RightCrouchHandRotation = CurrentWeaponController.IkObjects.RightCrouchObject.localEulerAngles;

					curInfo.LeftCrouchHandPosition = CurrentWeaponController.IkObjects.LeftCrouchObject.localPosition;
					curInfo.LeftCrouchHandRotation = CurrentWeaponController.IkObjects.LeftCrouchObject.localEulerAngles;
				}

				curInfo.LeftElbowPosition =
					CurrentWeaponController.IkObjects.LeftElbowObject.localPosition;
				curInfo.RightElbowPosition =
					CurrentWeaponController.IkObjects.RightElbowObject.localPosition;
				
				CurrentWeaponController.ActiveDebug = isPause && inspectorTab == 1;

				
				if (oldPinObjValue != CurrentWeaponController.pinLeftObject)
				{
					if (CurrentWeaponController.pinLeftObject)
					{
						CurrentWeaponController.IkObjects.LeftObject.parent = CurrentWeaponController.IkObjects.RightObject;
						CurrentWeaponController.IkObjects.LeftAimObject.parent = CurrentWeaponController.IkObjects.RightAimObject;
						CurrentWeaponController.IkObjects.LeftCrouchObject.parent = CurrentWeaponController.IkObjects.RightCrouchObject;
						CurrentWeaponController.IkObjects.LeftWallObject.parent = CurrentWeaponController.IkObjects.RightWallObject;
					}
					else
					{
						CurrentWeaponController.IkObjects.LeftObject.parent = CurrentWeaponController.BodyObjects.TopBody;
						CurrentWeaponController.IkObjects.LeftAimObject.parent = CurrentWeaponController.BodyObjects.TopBody;
						CurrentWeaponController.IkObjects.LeftCrouchObject.parent = CurrentWeaponController.BodyObjects.TopBody;
						CurrentWeaponController.IkObjects.LeftWallObject.parent = CurrentWeaponController.BodyObjects.TopBody;
					}

					oldPinObjValue = CurrentWeaponController.pinLeftObject;
				}



				if (oldNormBoolValue != CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState)
				{
					CheckIKObjects();
					oldNormBoolValue = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState;
				}

				if (oldCrouchBoolValue != CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState)
				{
					CheckIKObjects();
					IKHelper.CheckIK(ref CurrentWeaponController.CanUseElbowIK, ref CurrentWeaponController.CanUseIK, ref CurrentWeaponController.CanUseAimIK,
						ref CurrentWeaponController.CanUseWallIK, ref CurrentWeaponController.CanUseCrouchIK, CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex]);

					if (!CurrentWeaponController.CanUseCrouchIK)
						IKHelper.PlaceAllIKObjects(CurrentWeaponController, CurrentWeaponController.WeaponInfos[CurrentWeaponController.settingsSlotIndex],false, currentController.DirectionObject);

					oldCrouchBoolValue = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState;
				}

//				if (CurrentWeaponController.DebugMode == WeaponsHelper.IkDebugMode.Aim && currentController.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
//					Point.SetActive(true);
//				else Point.SetActive(false);

				if (oldDebugModeIndex != CurrentWeaponController.DebugMode)
				{
					CheckIKObjects();

					Selection.objects = new Object[0];

					if (CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Crouch && !currentController.isCrouch && currentController.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
					{
						currentController.ActivateCrouch();
					}
					else if (CurrentWeaponController.DebugMode != IKHelper.IkDebugMode.Crouch && currentController.isCrouch)
					{
						currentController.DeactivateCrouch();
					}

					if ((oldDebugModeIndex == IKHelper.IkDebugMode.Norm && CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState ||
					     oldDebugModeIndex == IKHelper.IkDebugMode.Crouch && CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState) &&
					    (CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Aim && !CurrentWeaponController.CanUseAimIK ||
					     CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Wall && !CurrentWeaponController.CanUseWallIK) ||
					    CurrentWeaponController.DebugMode == IKHelper.IkDebugMode.Crouch && !CurrentWeaponController.CanUseCrouchIK)
						IKHelper.PlaceAllIKObjects(CurrentWeaponController, CurrentWeaponController.WeaponInfos[CurrentWeaponController.settingsSlotIndex],false, currentController.DirectionObject);

					oldDebugModeIndex = CurrentWeaponController.DebugMode;
				}
			}


			if (isPause && !Cursor.visible)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			
			for (var i = 0; i < Weapons.Count; i++)
			{
				if (weaponIndex != i && Weapons[i].gameObject.hideFlags != HideFlags.HideInHierarchy)
				{
					Weapons[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
					Weapons[i].gameObject.SetActive(false);
				}
				else if (weaponIndex == i && Weapons[i].gameObject.hideFlags == HideFlags.HideInHierarchy)
				{
//					if (Type == AdjustmentType.Enemy && Weapons[i].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)||
//					    Type == AdjustmentType.CharacterUI && Weapons[i].Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade) ||
//					    Weapons[i].Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
//					{
						Weapons[i].gameObject.hideFlags = HideFlags.None;
						Weapons[i].gameObject.SetActive(true);
//					}
				}
			}
			
//			for (var i = 0; i < Characters.Count; i++)
//			{
//				if (characterIndex != i && Characters[i].gameObject.hideFlags != HideFlags.HideInHierarchy)
//				{
//					Characters[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
//					
//					if (Characters[i].CameraController && Characters[i].CameraController.MainCamera)
//						Characters[i].CameraController.MainCamera.hideFlags = HideFlags.HideInHierarchy;
//					
//					Characters[i].gameObject.SetActive(false);
//					Characters[i].thisCamera.SetActive(false);
//				}
//				else if (characterIndex == i && Characters[i].gameObject.hideFlags == HideFlags.HideInHierarchy)
//				{
//					Characters[i].gameObject.hideFlags = HideFlags.None;
//					
//					if (Characters[i].CameraController && Characters[i].CameraController.MainCamera)
//						Characters[i].CameraController.MainCamera.hideFlags = HideFlags.None;
//					
//					Characters[i].thisCamera.SetActive(true);
//					Characters[i].gameObject.SetActive(true);
//				}
//			}
			
//			if (Input.GetKeyDown(KeyCode.Escape))
//			{
//				Pause();
//			}
		}

		void SetAllHandlesSize()
		{
			if(!CurrentWeaponController.canUseValuesInAdjustment)
				return;
			
			SetHandleSize(CurrentWeaponController.IkObjects.LeftCrouchObject);
			SetHandleSize(CurrentWeaponController.IkObjects.RightCrouchObject);
				
			SetHandleSize(CurrentWeaponController.IkObjects.RightAimObject);
			SetHandleSize(CurrentWeaponController.IkObjects.LeftAimObject);
				
			SetHandleSize(CurrentWeaponController.IkObjects.RightWallObject);
			SetHandleSize(CurrentWeaponController.IkObjects.LeftWallObject);
				
			SetHandleSize(CurrentWeaponController.IkObjects.RightObject);
			SetHandleSize(CurrentWeaponController.IkObjects.LeftObject);
			
			SetHandleSize(CurrentWeaponController.IkObjects.RightElbowObject);
			SetHandleSize(CurrentWeaponController.IkObjects.LeftElbowObject);
		}
		


		void SetHandleSize(Transform obj)
		{
			obj.localScale = new Vector3(settings.CubesSize / 100, settings.CubesSize / 100, settings.CubesSize / 100);
		}

		public void CheckIKObjects()
		{
			if (!CurrentWeaponController.ActiveDebug || inspectorTab == 0) return;// || upInspectorTab == 1) return;
			
			switch (CurrentWeaponController.DebugMode)
			{
				case IKHelper.IkDebugMode.Aim:
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightWallObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftWallObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftObject);

					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftAimObject);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightAimObject);
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightCrouchObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftCrouchObject);
					
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject);

					CurrentWeaponController.activeAimMode = true;
//					CurrentWeaponController.activeAimTP = true;
//					CurrentWeaponController.activeAimFP = true;
					break;
				case IKHelper.IkDebugMode.Wall:
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightWallObject);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftWallObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightAimObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftAimObject);
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightCrouchObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftCrouchObject);
					
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject);
					Helper.HideIKObjects(false, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject);
					break;
				case IKHelper.IkDebugMode.Norm:
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.RightObject);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.LeftObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightAimObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftAimObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightWallObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftWallObject);
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightCrouchObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftCrouchObject);
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject);
					break;
				case IKHelper.IkDebugMode.Crouch:
					
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightAimObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftAimObject);

					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.RightWallObject);
					Helper.HideIKObjects(true, HideFlags.HideInHierarchy, CurrentWeaponController.IkObjects.LeftWallObject);
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.RightCrouchObject);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.LeftCrouchObject);
					
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.RightElbowObject);
					Helper.HideIKObjects(CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState, HideFlags.None, CurrentWeaponController.IkObjects.LeftElbowObject);
					break;
				
			}
		}

		void Pause()
		{
			isPause = !isPause;

			if (!isPause && CurrentWeaponController)// && CurrentWeaponController.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
			{
				Helper.HideAllObjects(CurrentWeaponController.IkObjects);
			}

//			if (CurrentWeaponController && CurrentWeaponController.Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
//			{
//				CurrentWeaponController.gameObject.SetActive(isPause);
//				CurrentWeaponController.WeaponManager.grenadeController = isPause ? CurrentWeaponController : null;
//				currentController.anim.CrossFade("Walk (without weapons)", 0.01f, 2);
//			}
			
			Background.SetActive(isPause);
			
			Cursor.visible = isPause;
			Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;

			if (currentController)
			{
				currentController.CameraController.cameraDebug = isPause;
				currentController.isPause = isPause;
				
//				if (currentController.CameraController && currentController.CameraController.crosshair)
//					currentController.CameraController.crosshair.gameObject.SetActive(!isPause);
			}
		}

		public void ActiveCharacter(int index, bool changeInspectorTab)
		{
//			if (!changeInspectorTab)
//			{
//				Characters[characterIndex].gameObject.SetActive(false);
//				Characters[characterIndex].thisCamera.SetActive(false);
//				
//
//				
//				
//				//Characters[index].anim.CrossFade("Walk (without weapons)", 0.01f, 2);
//			}


			Characters[characterIndex].gameObject.hideFlags = HideFlags.HideInHierarchy;
			
			if(Characters[index].CameraController.MainCamera)
				Characters[index].CameraController.MainCamera.hideFlags = HideFlags.HideInHierarchy;
			else Characters[index].thisCamera.hideFlags = HideFlags.HideInHierarchy;
			
			Characters[characterIndex].gameObject.SetActive(false);
			Characters[characterIndex].thisCamera.SetActive(false);
			Characters[characterIndex].ActiveCharacter = false;
			
			oldCameraType = Characters[index].TypeOfCamera;
			
			Characters[index].gameObject.SetActive(true);
			Characters[index].thisCamera.SetActive(true);
			Characters[index].ActiveCharacter = true;
			
			Characters[index].anim.Rebind();

			if(Characters[index].transform.Find("Canvas"))
				Characters[index].transform.Find("Canvas").gameObject.SetActive(false);
			
			Characters[index].gameObject.hideFlags = HideFlags.None;
			
			if(Characters[index].CameraController.MainCamera)
				Characters[index].CameraController.MainCamera.hideFlags = HideFlags.None;
			else Characters[index].thisCamera.hideFlags = HideFlags.None;

			Characters[index].anim.SetBool("NoWeapons", true);

			Characters[index].CameraController.SetAnimVariables();
			
			dirObjRotX = Characters[index].CharacterOffset.directionObjRotation.x;
			dirObjRotY = Characters[index].CharacterOffset.directionObjRotation.y;
			dirObjRotZ = Characters[index].CharacterOffset.directionObjRotation.z;
			
			ResetWeapons();

			//CurrentWeaponController = null;
			weaponIndex = -1;
		}

		public void ResetWeapons()
		{
			if (CurrentWeaponController)
				Helper.HideAllObjects(CurrentWeaponController.IkObjects);

			CurrentWeaponController = null;

			foreach (var character in Characters)
			{
				var manager = character.gameObject.GetComponent<InventoryManager>();
				manager.slots[0].weaponSlotInGame.Clear();
				manager.WeaponController = null;
				manager.hasAnyWeapon = false;

				if (!manager.currentWeapon) continue;
				manager.currentWeapon.transform.parent = null;
				manager.currentWeapon = null;
			}
		}

//		public IEnumerator SetByAnimationTimeout()
//		{
//			yield return new WaitForSeconds(1);
//			CurrentWeaponController.IkObjects.LeftObject.parent = currentController.BodyObjects.LeftHand;
//			CurrentWeaponController.IkObjects.LeftObject.localPosition = Vector3.zero;
//			CurrentWeaponController.IkObjects.LeftObject.localEulerAngles = Vector3.zero;
//			
//			CurrentWeaponController.IkObjects.RightObject.parent = currentController.BodyObjects.RightHand;
//			CurrentWeaponController.IkObjects.RightObject.localPosition = Vector3.zero;
//			CurrentWeaponController.IkObjects.RightObject.localEulerAngles = Vector3.zero;
//			
//			yield return new WaitForSeconds(2);
//			CurrentWeaponController.IkObjects.RightObject.parent = currentController.BodyObjects.TopBody;
//			CurrentWeaponController.IkObjects.LeftObject.parent = currentController.BodyObjects.TopBody;
//			
//			CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.SettingsSlotIndex].disableIkInNormalState = false;
//			CurrentWeaponController.IkObjects.RightObject.rotation = Quaternion.LookRotation(currentController.DirectionObject.forward, currentController.DirectionObject.right);
//			CurrentWeaponController.IkObjects.LeftObject.rotation = Quaternion.LookRotation(currentController.DirectionObject.forward, -currentController.DirectionObject.right);
//			
//			StopCoroutine("SetByAnimationTimeout");
//		}

		public void SaveData()
		{
			CurrentWeaponController.OriginalScript.WeaponInfos[CurrentWeaponController.settingsSlotIndex].Clone(CurrentWeaponController.WeaponInfos[CurrentWeaponController.settingsSlotIndex]);

			EditorUtility.SetDirty(CurrentWeaponController.OriginalScript);
		}

		public void CopyWeaponData()
		{
			switch (CurrentWeaponController.DebugMode)
			{
				case IKHelper.IkDebugMode.Aim:
					CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex]
						.CloneToAim(Weapons[copyFromWeaponSlot].WeaponInfos[copyFromSlot], IKStateNames[copyFromIKState]);
					break;

				case IKHelper.IkDebugMode.Wall:
					CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex]
						.CloneToWall(Weapons[copyFromWeaponSlot].WeaponInfos[copyFromSlot], IKStateNames[copyFromIKState]);
					break;

				case IKHelper.IkDebugMode.Crouch:
					CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex]
						.CloneToCrouch(Weapons[copyFromWeaponSlot].WeaponInfos[copyFromSlot], IKStateNames[copyFromIKState]);
					break;

				case IKHelper.IkDebugMode.Norm:
					CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex]
						.CloneToNorm(Weapons[copyFromWeaponSlot].WeaponInfos[copyFromSlot], IKStateNames[copyFromIKState]);
					break;
			}
			
			WeaponsHelper.SetWeaponPositions(CurrentWeaponController, true, currentController.DirectionObject);

		}

		public IEnumerator SetDefault()
		{
			yield return new WaitForSeconds(0.1f);
			
			IKHelper.PlaceAllIKObjects(CurrentWeaponController, CurrentWeaponController.WeaponInfos[CurrentWeaponController.settingsSlotIndex], true, currentController.DirectionObject);
			currentController.WeaponManager.DebugIKValue = 1;
			
			StopCoroutine("SetDefault");
		}

		public IEnumerator CopyTimeout()
		{
			CurrentWeaponController.IkObjects.LeftObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			CurrentWeaponController.IkObjects.LeftAimObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			CurrentWeaponController.IkObjects.LeftCrouchObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			CurrentWeaponController.IkObjects.LeftWallObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			
			yield return new WaitForSeconds(0.3f);

			CopyWeaponData();
			
			yield return new WaitForSeconds(0.5f);
			
			CurrentWeaponController.pinLeftObject = true;
			
			StopCoroutine("CopyTimeout");
			
		}

		public IEnumerator SaveTimeout()
		{
			CurrentWeaponController.IkObjects.LeftObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			CurrentWeaponController.IkObjects.LeftAimObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			CurrentWeaponController.IkObjects.LeftCrouchObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			CurrentWeaponController.IkObjects.LeftWallObject.parent = CurrentWeaponController.BodyObjects.TopBody;
			
			yield return new WaitForSeconds(0.3f);

			var curInfo = CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex];

			CurrentWeaponController.WeaponInfos[CurrentWeaponController.settingsSlotIndex].Clone(curInfo);

			SaveData();
										
			IKHelper.CheckIK(ref CurrentWeaponController.CanUseElbowIK,
				ref CurrentWeaponController.CanUseIK, ref CurrentWeaponController.CanUseAimIK,
				ref CurrentWeaponController.CanUseWallIK, ref CurrentWeaponController.CanUseCrouchIK, curInfo);

			if (!CurrentWeaponController.CanUseIK)
				CurrentWeaponController.CanUseIK = true;

			yield return new WaitForSeconds(0.5f);
			
			CurrentWeaponController.pinLeftObject = true;
			
			StopCoroutine("SaveTimeout");
		}
		
		public void OnDrawGizmos()
		{
			if (Application.isPlaying && currentController && inspectorTab == 0)
			{
				Handles.zTest = CompareFunction.Less;

				if (currentController.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					Handles.color = new Color32(0, 0, 255, 255);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.forward), 1,
						EventType.Repaint);

					Handles.color = new Color32(255, 0, 0, 255);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.right), 1,
						EventType.Repaint);
				}
				else
				{
					Handles.color = new Color32(255, 255, 0, 255);
					Handles.ArrowHandleCap(0, currentController.CameraController.CameraPosition.position, currentController.CameraController.CameraPosition.rotation, 1, EventType.Repaint);
					Handles.SphereHandleCap(0, currentController.CameraController.CameraPosition.position, Quaternion.identity, 0.1f, EventType.Repaint);
				}

				Handles.zTest = CompareFunction.Greater;

				if (currentController.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					Handles.color = new Color32(0, 0, 255, 50);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.forward), 1,
						EventType.Repaint);

					Handles.color = new Color32(255, 0, 0, 50);
					Handles.ArrowHandleCap(0, currentController.DirectionObject.position, Quaternion.LookRotation(currentController.DirectionObject.right), 1,
						EventType.Repaint);
				}
				else
				{
					Handles.color = new Color32(255, 255, 0, 50);
					Handles.ArrowHandleCap(0, currentController.CameraController.CameraPosition.position, currentController.CameraController.CameraPosition.rotation, 1, EventType.Repaint);
					Handles.SphereHandleCap(0, currentController.CameraController.CameraPosition.position, Quaternion.identity, 0.1f, EventType.Repaint);
				}
			}

			if (!Application.isPlaying || !CurrentWeaponController || !CurrentWeaponController.canUseValuesInAdjustment) return;
			
			if (inspectorTab != 0)
			{
				switch (CurrentWeaponController.DebugMode)
				{
					case IKHelper.IkDebugMode.Norm:
						if (!CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInNormalState)
						{
							DrawIKHandle(CurrentWeaponController.IkObjects.RightObject, Color.red, "cube");
							
							if(!CurrentWeaponController.pinLeftObject)
								DrawIKHandle(CurrentWeaponController.IkObjects.LeftObject, Color.red, "cube");

							DrawIKHandle(CurrentWeaponController.IkObjects.RightElbowObject, new Color32(0, 150, 0, 255), "elbow");
							DrawIKHandle(CurrentWeaponController.IkObjects.LeftElbowObject, new Color32(0, 150, 0, 255), "elbow");
						}

						break;
					case IKHelper.IkDebugMode.Aim:
						DrawIKHandle(CurrentWeaponController.IkObjects.RightAimObject, Color.blue, "cube");
						
						if(!CurrentWeaponController.pinLeftObject)
							DrawIKHandle(CurrentWeaponController.IkObjects.LeftAimObject, Color.blue, "cube");

						DrawIKHandle(CurrentWeaponController.IkObjects.RightElbowObject, new Color32(0, 150, 0, 255), "elbow");
						DrawIKHandle(CurrentWeaponController.IkObjects.LeftElbowObject, new Color32(0, 150, 0, 255), "elbow");
						break;
					case IKHelper.IkDebugMode.Wall:
						DrawIKHandle(CurrentWeaponController.IkObjects.RightWallObject, Color.yellow, "cube");
						
						if(!CurrentWeaponController.pinLeftObject)
							DrawIKHandle(CurrentWeaponController.IkObjects.LeftWallObject, Color.yellow, "cube");

						DrawIKHandle(CurrentWeaponController.IkObjects.RightElbowObject, new Color32(0, 150, 0, 255), "elbow");
						DrawIKHandle(CurrentWeaponController.IkObjects.LeftElbowObject, new Color32(0, 150, 0, 255), "elbow");
						break;
					case IKHelper.IkDebugMode.Crouch:
						if (!CurrentWeaponController.CurrentWeaponInfo[CurrentWeaponController.settingsSlotIndex].disableIkInCrouchState)
						{
							DrawIKHandle(CurrentWeaponController.IkObjects.RightCrouchObject, Color.magenta, "cube");
							
							if(!CurrentWeaponController.pinLeftObject)
								DrawIKHandle(CurrentWeaponController.IkObjects.LeftCrouchObject, Color.magenta, "cube");

							DrawIKHandle(CurrentWeaponController.IkObjects.RightElbowObject, new Color32(0, 150, 0, 255), "elbow");
							DrawIKHandle(CurrentWeaponController.IkObjects.LeftElbowObject, new Color32(0, 150, 0, 255), "elbow");
						}

						break;
				}
			}

//			if (!Application.isPlaying || !CurrentWeaponController || CurrentWeaponController.DebugMode != WeaponsHelper.IkDebugMode.Wall) return; //||
			    //!CurrentWeaponController.ColliderToCheckWalls) 
			
//			Handles.matrix = CurrentWeaponController.ColliderToCheckWalls.localToWorldMatrix;

//			if (Selection.gameObjects.Contains(CurrentWeaponController.ColliderToCheckWalls.gameObject))
//			{
//				Handles.zTest = CompareFunction.Greater;
//				Handles.color = new Color32(255, 255, 0, 50);
//				Handles.DrawWireCube(Vector3.zero, Vector3.one);
//
//				Handles.zTest = CompareFunction.Less;
//				Handles.color = new Color32(255, 255, 0, 255);
//				Handles.DrawWireCube(Vector3.zero, Vector3.one);
//			}
//			else
//			{
//				Handles.zTest = CompareFunction.Greater;
//				Handles.color = new Color32(255, 100, 0, 30);
//				Handles.DrawWireCube(Vector3.zero, Vector3.one);
//
//				Handles.zTest = CompareFunction.Less;
//				Handles.color = new Color32(255, 100, 0, 150);
//				Handles.DrawWireCube(Vector3.zero, Vector3.one);
//			}
		}

		void DrawIKHandle(Transform obj, Color32 color, string type)
		{
			Handles.matrix = obj.localToWorldMatrix;
			Gizmos.matrix = obj.localToWorldMatrix;
			
			if (settings.CubeSolid == Helper.CubeSolid.Solid)
			{
				if (!Selection.gameObjects.Contains(obj.gameObject))
				{
					Handles.zTest = CompareFunction.Greater;
					Handles.color = new Color32(color.r, color.g, color.b, 50);
					Gizmos.color = new Color32(color.r, color.g, color.b, 50);

					if (type != "elbow")
						Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
					else Gizmos.DrawSphere(Vector3.zero, 1);

					Handles.zTest = CompareFunction.Less;
					Handles.color = new Color32(color.r, color.g, color.b, 255);
					Gizmos.color = new Color32(color.r, color.g, color.b, 255);

					if (type != "elbow")
						Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
					else Gizmos.DrawSphere(Vector3.zero, 1);
				}
				else
				{
					Handles.zTest = CompareFunction.Greater;
					Handles.color = new Color32(0, 255, 0, 50);
					Gizmos.color = new Color32(0, 255, 0, 50);

					if (type != "elbow")
						Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
					else Gizmos.DrawSphere(Vector3.zero, 1);

					Handles.zTest = CompareFunction.Less;
					Handles.color = new Color32(0, 255, 0, 255);
					Gizmos.color = new Color32(0, 255, 0, 255);

					if (type != "elbow")
						Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
					else Gizmos.DrawSphere(Vector3.zero, 1);
				}
			}
			else
			{
				if (Selection.gameObjects.Contains(obj.gameObject))
				{
					Handles.zTest = CompareFunction.Greater;
					Handles.color = new Color32(0, 255, 0, 50);
					Gizmos.color = new Color32(0, 255, 0, 50);

					if (type != "elbow")
						Handles.DrawWireCube(Vector3.zero, Vector3.one);
					else Gizmos.DrawWireSphere(Vector3.zero, 1);

					Handles.zTest = CompareFunction.Less;
					Handles.color = new Color32(0, 255, 0, 255);
					Gizmos.color = new Color32(0, 255, 0, 255);

					if (type != "elbow")
						Handles.DrawWireCube(Vector3.zero, Vector3.one);
					else Gizmos.DrawWireSphere(Vector3.zero, 1);
				}
				else
				{
					Handles.zTest = CompareFunction.Greater;
					Handles.color = new Color32(color.r, color.g, color.b, 50);
					Gizmos.color = new Color32(color.r, color.g, color.b, 50);

					if (type != "elbow")
						Handles.DrawWireCube(Vector3.zero, Vector3.one);
					else Gizmos.DrawWireSphere(Vector3.zero, 1);

					Handles.zTest = CompareFunction.Less;
					Handles.color = new Color32(color.r, color.g, color.b, 255);
					Gizmos.color = new Color32(color.r, color.g, color.b, 255);

					if (type != "elbow")
						Handles.DrawWireCube(Vector3.zero, Vector3.one);
					else Gizmos.DrawWireSphere(Vector3.zero, 1);
				}
			}
		}
#endif
	}
}
