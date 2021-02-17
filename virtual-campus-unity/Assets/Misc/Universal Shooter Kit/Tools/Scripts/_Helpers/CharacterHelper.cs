using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
	public static class CharacterHelper
	{
		
		[Serializable]
		public class Weapon
		{
			public GameObject weapon;
			public bool fistAttack;
			public List<Kit> WeaponAmmoKits = new List<Kit>();
		}
		
		[Serializable]
		public class Kit
		{
			public int AddedValue;
			public Texture Image;

			public string PickUpId;
			public string ammoType;
		}
		
		[Serializable]
		public class InventorySlot
		{
			//public bool hideAllWeaponsSlot;
            
			public int currentWeaponInSlot;
//			public int weaponsCount = 1;
//			public Button SlotButton;
			public List<WeaponsHelper.WeaponSlotInInventory> weaponSlotInInspector = new List<WeaponsHelper.WeaponSlotInInventory>{new WeaponsHelper.WeaponSlotInInventory()};
			public List<Weapon> weaponSlotInGame;
//			public Text curAmmoText;
//			public RawImage SlotImage;
		}
		
		[Serializable]
		public class CharacterOffset
		{
			public Vector3 directionObjRotation;
			
			public float xRotationOffset;
			public float yRotationOffset;
			public float zRotationOffset;
			public float CharacterHeight = -0.51f;
			
			public Vector3 SaveTime;
			public Vector3 SaveDate;
			
			public bool HasTime;

			public void Clone(CharacterOffset cloneFrom)
			{
				directionObjRotation = cloneFrom.directionObjRotation;
				
				xRotationOffset = cloneFrom.xRotationOffset;
				yRotationOffset = cloneFrom.yRotationOffset;
				zRotationOffset = cloneFrom.zRotationOffset;
				CharacterHeight = cloneFrom.CharacterHeight;

				SaveTime = cloneFrom.SaveTime;
				SaveDate = cloneFrom.SaveDate;
				HasTime = cloneFrom.HasTime;
			}
		}

		[Serializable]
		public class CameraOffset
		{
			public float tpCameraOffsetX;
			public float tpCameraOffsetY;
			//public Vector3 tpCameraRotationOffset;
			
			public float normCameraOffsetX;
			public float normCameraOffsetY;
			public float normDistance;

//			public Vector3 cameraNormRotationOffset;
//			public Vector3 cameraAimRotationOffset;

			public float aimCameraOffsetX;
			public float aimCameraOffsetY;
			public float aimDistance;

			public float tdCameraOffsetX;
			public float tdCameraOffsetY;
			
			public float tdLockCameraOffsetX;
			public float tdLockCameraOffsetY;

			public float tdLockCameraAngle = 90;

			public Vector3 cameraObjPos;
			public Vector3 cameraObjRot;

			public float TDLockCameraDistance;
			public float TD_Distance;
			public float Distance;
			public float TopDownAngle = 80;

			public void Clone(CameraOffset cloneFrom)
			{
				tpCameraOffsetX = cloneFrom.tpCameraOffsetX;
				tpCameraOffsetY = cloneFrom.tpCameraOffsetY;
				
				aimCameraOffsetX = cloneFrom.aimCameraOffsetX;
				aimCameraOffsetY = cloneFrom.aimCameraOffsetY;
				
				normCameraOffsetX = cloneFrom.normCameraOffsetX;
				normCameraOffsetY = cloneFrom.normCameraOffsetY;

//				cameraNormRotationOffset = cloneFrom.cameraNormRotationOffset;
//				cameraAimRotationOffset = cloneFrom.cameraAimRotationOffset;
//				tpCameraRotationOffset = cloneFrom.tpCameraRotationOffset;

				tdCameraOffsetX = cloneFrom.tdCameraOffsetX;
				tdCameraOffsetY = cloneFrom.tdCameraOffsetY;
				
				tdLockCameraOffsetX = cloneFrom.tdLockCameraOffsetX;
				tdLockCameraOffsetY = cloneFrom.tdLockCameraOffsetY;

				cameraObjPos = cloneFrom.cameraObjPos;
				cameraObjRot = cloneFrom.cameraObjRot;

				tdLockCameraAngle = cloneFrom.tdLockCameraAngle;
				TDLockCameraDistance = cloneFrom.TDLockCameraDistance;
				
				TopDownAngle = cloneFrom.TopDownAngle;
				TD_Distance = cloneFrom.TD_Distance;
				
				Distance = cloneFrom.Distance;
				normDistance = cloneFrom.normDistance;
				aimDistance = cloneFrom.aimDistance;
			}
		}
		
		[Serializable]
		public class BodyObjects
		{
			public Transform RightHand;
			public Transform LeftHand;
			public Transform TopBody;
			public Transform Head;
			public Transform Hips;
			public Transform Chest;
		}
		
		[Serializable]
		public class Speeds
		{
			public float NormForwardSpeed = 4;
			public float NormBackwardSpeed = 3;
			public float NormLateralSpeed = 3;
			public float RunForwardSpeed = 8;
			public float RunBackwardSpeed = 6;
			public float RunLateralSpeed = 6;
			public float CrouchForwardSpeed = 2;
			public float CrouchBackwardSpeed = 1.5f;
			public float CrouchLateralSpeed = 1.5f;
			
			[Range(10, 100)] public float JumpSpeed = 50;
			[Range(1, 20)] public float JumpHeight = 2;
		}

		[Serializable]
		public class CameraParameters
		{
			[Range(60, 30)] public float tpAimDepth = 40f;
			[Range(60, 30)] public float fpAimDepth = 40f;
			
			[Range(1, 10)] public float tpXMouseSensitivity = 5f;
			[Range(1, 10)] public float tpYMouseSensitivity = 5f;
			[Range(1, 5)] public float tpAimXMouseSensitivity = 2f;
			[Range(1, 5)] public float tpAimYMouseSensitivity = 2f;
			[Range(-90, 0)] public float tpXLimitMin = -40f;
			[Range(0, 90)] public float tpXLimitMax = 80f;
			[Range(0.01f, 0.5f)] public float tpSmoothX = 0.05f;
			[Range(0.01f, 0.5f)] public float tpSmoothY = 0.1f;
			[Range(0.01f, 0.5f)] public float tdSmoothX = 0.1f;

			[Range(1, 5)] public float fpAimXMouseSensitivity = 1f;
			[Range(1, 5)] public float fpAimYMouseSensitivity = 1f;
			[Range(1, 10)] public float fpXMouseSensitivity = 2f;
			[Range(1, 10)] public float fpYMouseSensitivity = 2f;
			[Range(0.1f, 5)] public float fpXSmooth = 3f;
			[Range(0.1f, 5)] public float fpYSmooth = 3f;
			[Range(-90, 0)] public float fpXLimitMin = -80;
			[Range(0, 90)] public float fpXLimitMax = 80;
			
			[Range(-90, 0)] public float tdXLimitMin = -30;
			[Range(0, 90)] public float tdXLimitMax = 40;
			[Range(1, 20)] public float tdXMouseSensitivity = 5f;

			public bool activeFP = true;
			public bool activeTP = true;
			public bool activeTD = true;
			public bool lockCamera;
			public bool lookAtCursor;
			public bool alwaysTDAim = true;

			public Sprite CursorImage;

			public AnimationCurve FPIdleCurve = AnimationCurve.Linear(0, 0, 1, 0);
			public AnimationCurve FPWalkCurve = AnimationCurve.Linear(0, 0, 1, 0);
			public AnimationCurve FPRunCurve = AnimationCurve.Linear(0, 0, 1, 0);
			public AnimationCurve FPCrouchCurve = AnimationCurve.Linear(0, 0, 1, 0);
		}

		public enum CameraType
		{
			ThirdPerson,
			FirstPerson,
			TopDown
		}

		public enum MovementType
		{
			Realistic,
			FastAndAccurate
		}
		
		

#if UNITY_EDITOR
		public static GameObject[] CreateCrosshair(Transform parent)
		{
			var crosshair = new GameObject("Crosshair") {layer = 5};
			crosshair.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
			crosshair.transform.SetParent(parent);

			crosshair.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			
			var parts = new GameObject[6];

			parts[0] = crosshair;

			parts[1] = Helper.newCrosshairPart("Up", new Vector2(0, 30), new Vector2(27, 27), crosshair);
			parts[2] = Helper.newCrosshairPart("Down", new Vector2(0, -30), new Vector2(27, 27), crosshair);
			parts[3] = Helper.newCrosshairPart("Right", new Vector2(30, 0), new Vector2(27, 27), crosshair);
			parts[4] = Helper.newCrosshairPart("Left", new Vector2(-30, 0), new Vector2(27, 27), crosshair);
			
			parts[5] = Helper.newCrosshairPart("Middle", new Vector2(0, 0), new Vector2(27, 27), crosshair);

			return parts;
		}

		public static GameObject CreateCrosshair(Transform parent, string type)
		{
//			if (type != "pickup")
//			{
//				var crosshair = new GameObject("Crosshair") {layer = 5};
//				crosshair.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
//				crosshair.transform.SetParent(parent);
//				
//				crosshair.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
//				
//
//				Helper.newCrosshairPart("Up", new Vector2(0, 30), new Vector2(4, 20), crosshair);
//				Helper.newCrosshairPart("Down", new Vector2(0, -30), new Vector2(4, 20), crosshair);
//				Helper.newCrosshairPart("Right", new Vector2(30, 0), new Vector2(20, 4), crosshair);
//				Helper.newCrosshairPart("Left", new Vector2(-30, 0), new Vector2(20, 4), crosshair);
//				
//				return crosshair;
//			}

			var pickUpIcon = new GameObject("PickUp Icon") {layer = 5};
			pickUpIcon.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
			pickUpIcon.transform.SetParent(parent);
			pickUpIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			var outline = pickUpIcon.AddComponent<Outline>();
			outline.effectColor = new Color(0, 0, 0, 1);

			var sprite = AssetDatabase.LoadAssetAtPath(
				"Assets/Universal Shooter Kit/Textures & Materials/Character/Inventory/HandIcon.png", typeof(Sprite)) as Sprite;

			pickUpIcon.AddComponent<Image>().sprite = sprite;

			return pickUpIcon;
		}
#endif
		
		public static void ChangeTDMode(Controller controller)
		{
			if (controller.CameraParameters.lockCamera)
			{
				if (!controller.CameraParameters.lookAtCursor)
					controller.CameraParameters.lookAtCursor = true;
				else
				{
					controller.tdModeLikeTp = true;
					controller.CameraParameters.alwaysTDAim = false;
					controller.CameraParameters.lockCamera = false;
					controller.TypeOfCamera = CameraType.ThirdPerson;
				}
			}
			else
			{
				if (!controller.CameraParameters.alwaysTDAim)
				{
					controller.tdModeLikeTp = false;
					controller.TypeOfCamera = CameraType.TopDown;
					controller.CameraParameters.alwaysTDAim = true;
				}
				else
				{
					controller.CameraParameters.lockCamera = true;
					controller.CameraParameters.lookAtCursor = false;
				}
			}
		}

		public static void CheckCameraPoints(Vector3 from, Vector3 to, List<GameObject> disabledObjects, Camera camera)
		{
			RaycastHit hitInfo;
			var clipPlanePoints = Helper.NearPoints(to, camera);
			var hiddenObjects = new List<GameObject>();

			if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, Helper.layerMask()) ||
			    Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, Helper.layerMask()) ||
			    Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, Helper.layerMask()) ||
			    Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, Helper.layerMask()))
			{

				if (hitInfo.collider.GetComponent<MeshRenderer>())
				{
					if (!disabledObjects.Exists(obj => obj.GetInstanceID() == hitInfo.collider.GetInstanceID()))
					{
						disabledObjects.Add(hitInfo.collider.gameObject);
					}
					
					hiddenObjects.Add(hitInfo.collider.gameObject);
					hitInfo.collider.GetComponent<MeshRenderer>().enabled = false;
				}
			}

			foreach (var obj in disabledObjects)
			{
				if (!hiddenObjects.Contains(obj))
				{
					obj.GetComponent<MeshRenderer>().enabled = true;
					disabledObjects.Remove(obj);
					break;
				}
			}
		}

		public static float CheckCameraPoints(Vector3 from, Vector3 to, List<GameObject> disabledObjects, Transform transform, Camera camera)
		{
			var nearestDistance = -1f;
			RaycastHit hitInfo;
			var clipPlanePoints = Helper.NearPoints(to, camera);

			foreach (var obj in disabledObjects)
			{
				obj.GetComponent<MeshRenderer>().enabled = true;
				disabledObjects.Remove(obj);
			}

			if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, Helper.layerMask()))
				nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			if (Physics.Linecast(from, to + transform.forward * - camera.nearClipPlane, out hitInfo, Helper.layerMask()))
				if (hitInfo.distance < nearestDistance || nearestDistance == -1f)
					nearestDistance = hitInfo.distance;

			return nearestDistance;
		}

		public static Vector3 CalculatePosition(float RotationX, float RotationY, float distance, Controller thisController, float height, bool isJump)
		{
			var direction = new Vector3(0, 0, -distance);
			var rotation = Quaternion.Euler(RotationX, RotationY, 0);

			return new Vector3(thisController.transform.position.x, !isJump ? height + 2 : thisController.BodyObjects.Head.position.y, thisController.transform.position.z) + rotation * direction;
		}

		public static Vector3 CalculatePosition(float RotationY, float distance, Controller thisController, float tdAngle, float height, string type)
		{
			var direction = new Vector3(0, 0, -distance);

			var rotation = type == "body" ? Quaternion.Euler(0, RotationY, 0) : Quaternion.Euler(tdAngle, RotationY, 0);

			return new Vector3(thisController.transform.position.x, type != "body" ? height + 2 : 0, thisController.transform.position.z) + rotation * direction;
		}

		public static void ResetCameraParameters(CameraType curType, CameraType newType, Controller controller)
		{
			ChangeCameraType(curType, newType, controller);
//			ResetAnimations(controller, true);
		}

		public static void SwitchCamera(CameraType curType, CameraType newType, Controller controller)
		{
			if (curType == CameraType.ThirdPerson && controller.tdModeLikeTp)
			{
				curType = CameraType.TopDown;
				controller.tdModeLikeTp = false;
			}
			
			if(curType == newType)
				return;
			
			if (!controller.AdjustmentScene)
			{
				if (newType == CameraType.TopDown && !controller.CameraParameters.alwaysTDAim && !controller.CameraParameters.lockCamera)
				{
					newType = CameraType.ThirdPerson;
					controller.tdModeLikeTp = true;
				}
			}

			ChangeCameraType(curType, newType, controller);
			ResetAnimations(controller);
		}

		static void ChangeCameraType(CameraType curType, CameraType newType, Controller controller)
		{
			if (curType != newType)
				controller.changeCameraType = true;

			if (controller.WeaponManager.WeaponController)
			{
				controller.WeaponManager.WeaponController.setHandsPositionsAim = true;
				controller.WeaponManager.WeaponController.setHandsPositionsCrouch = true;
				controller.WeaponManager.WeaponController.setHandsPositionsObjectDetection = true;
			}

			switch (newType)
			{
				case CameraType.ThirdPerson:
					
					controller.TypeOfCamera = CameraType.ThirdPerson;
					controller.cameraInspectorTab = 0;
					
					controller.anim.SetLayerWeight(2, controller.WeaponManager.hasAnyWeapon ? 1 : 0);
					controller.anim.SetLayerWeight(1, 0);
					controller.currentAnimatorLayer = 2;

					controller.anim.SetBool("TPS", true);
					controller.anim.SetBool("FPS", false);
					controller.anim.SetBool("TDS", false);

					if (controller.WeaponManager.WeaponController && controller.WeaponManager.WeaponController.switchToFpCamera && controller.WeaponManager.WeaponController.isAimEnabled)
					{
						controller.WeaponManager.WeaponController.switchToFpCamera = false;
						controller.WeaponManager.WeaponController.wasSetSwitchToFP = true;
					}
					
					switch (curType)
					{
						case CameraType.FirstPerson:
							controller.CameraController.setCameraType = false;
							SetFPCamera(controller);
							
							if(controller.isCrouch)
								controller.WeaponManager.WeaponController.CrouchHands();
							
							break;
						case CameraType.TopDown:
							controller.CameraController.setCameraType = true;
							break;
					}

					break;
				case CameraType.FirstPerson:

					if (controller.CameraParameters.activeFP)
					{
//						if (controller.WeaponManager.WeaponController && controller.WeaponManager.WeaponController.ActiveCrouchHands)
//							controller.WeaponManager.WeaponController.CrouchHands();
						
						controller.TypeOfCamera = CameraType.FirstPerson;

						controller.CameraController.maxMouseAbsolute = controller.middleAngleX + controller.CameraParameters.fpXLimitMax;
						controller.CameraController.minMouseAbsolute = controller.middleAngleX + controller.CameraParameters.fpXLimitMin;

						var offset = new Vector3(controller.CharacterOffset.xRotationOffset, controller.CharacterOffset.yRotationOffset, controller.CharacterOffset.zRotationOffset);
						controller.CameraController.targetDirection = controller.BodyObjects.TopBody.eulerAngles - offset;

						controller.CameraController._mouseAbsolute = new Vector2(0, 0);
						controller.CameraController._smoothMouse = new Vector2(0, 0);
						controller.CameraController.setCameraType = false;

						controller.anim.SetBool("FPS", true);
						controller.anim.SetBool("TPS", false);
						controller.anim.SetBool("TDS", false);
						controller.anim.SetBool("HasWeaponTaken", false);
						controller.WeaponManager.SmoothIKSwitch = 0;

						controller.CameraController.setCameraType = false;

						controller.anim.SetLayerWeight(2, 0);
						controller.anim.SetLayerWeight(3, 0);
						controller.anim.SetLayerWeight(1, 1);
						
						controller.currentAnimatorLayer = 1;
						controller.cameraInspectorTab = 1;

						if (controller.isCrouch)
						{
							controller.defaultCharacterCenter += controller.CrouchHeight;
							controller.CharacterController.center = new Vector3(controller.CharacterController.center.x, controller.defaultCharacterCenter, controller.CharacterController.center.z);
						}
					}

					break;
				case CameraType.TopDown:

					if (controller.CameraParameters.activeTD)
					{
						if(controller.isCrouch)
							controller.DeactivateCrouch();
						
						controller.cameraInspectorTab = 2;
						controller.TypeOfCamera = CameraType.TopDown;

						controller.anim.SetBool("TDS", true);
						controller.anim.SetBool("TPS", false);
						controller.anim.SetBool("FPS", false);

						controller.anim.SetLayerWeight(2, 1);//controller.WeaponManager.hasAnyWeapon ? 1 : 0);
						controller.anim.SetLayerWeight(1, 0);
						controller.anim.SetLayerWeight(3, 0);
						controller.currentAnimatorLayer = 2;
						

						switch (curType)
						{
							case CameraType.ThirdPerson:
								controller.CameraController.setCameraType = true;
								break;
							case CameraType.FirstPerson:
							{
								controller.CameraController.BodyLookAt.position = controller.transform.position;
								controller.CameraController.setCameraType = false;
								SetFPCamera(controller);
								break;
							}
						}
					}

					break;
			}

			if (controller.AdjustmentScene)
			{
				controller.transform.position = Vector3.zero;
				controller.transform.eulerAngles = Vector3.zero;
				controller.BodyObjects.TopBody.eulerAngles = Vector3.zero;
				controller.CameraController.mouseX = 0;
				controller.CameraController.mouseY = 0;
			}
			
			controller.WeaponManager.SetCrosshair();

			controller.CameraController.Reset();
		}
		
		static void ResetAnimations(Controller controller)//, bool fullReset)
		{
			if (controller.WeaponManager.hasAnyWeapon)// && (fullReset && controller.anim.GetBool("HasWeaponTaken") || !fullReset))
			{
				if (controller.WeaponManager.WeaponController)
				{
					if (!controller.AdjustmentScene)
						WeaponsHelper.SetHandsSettingsSlot(ref controller.WeaponManager.WeaponController.settingsSlotIndex, controller.characterTag, controller.TypeOfCamera, controller.WeaponManager.WeaponController);

					controller.WeaponManager.WeaponController.StartCoroutine("WalkWithWeaponTimeout");
				}

				controller.anim.CrossFade("Idle", 0, 1);
				controller.anim.CrossFade("Idle", 0, 2);
				controller.anim.SetBool("CanWalkWithWeapon", false);
			}
		}

		static void SetFPCamera(Controller controller)
		{
			controller.CameraController.canViewTarget = false;
			controller.StartCoroutine(controller.CameraController.cameraTimeout());
								
			if (controller.isCrouch)
			{
				controller.defaultCharacterCenter -= controller.CrouchHeight;
				controller.CharacterController.center = new Vector3(controller.CharacterController.center.x, controller.defaultCharacterCenter, controller.CharacterController.center.z);
			}
		}
		
		public static int FindWeapon(InventorySlot[] slots, int currentSlot, int currentWeaponIndex,  bool plus)
		{
			if (plus)
			{
				for (var i = currentWeaponIndex; i <= slots[currentSlot].weaponSlotInGame.Count - 1; i++)
				{
					if (CheckWeapon(slots, currentSlot, i) != -1)
					{
						return CheckWeapon(slots, currentSlot, i);
					}
				}
			}
			else
			{
				for (var i = currentWeaponIndex; i >= 0; i--)
				{
					if (CheckWeapon(slots, currentSlot, i) != -1)
					{
						return CheckWeapon(slots, currentSlot, i);
					}
				}
			}

			return -1;
		}

		static int CheckWeapon(InventorySlot[] slots, int currentSlot, int i)
		{
			var weapon = slots[currentSlot].weaponSlotInGame[i];
			
			if (weapon.weapon)
			{
				var wController = weapon.weapon.GetComponent<WeaponController>();
				if (wController.Attacks[wController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade && wController.Attacks[wController.currentAttack].curAmmo > 0)
				{
					return i;
				}
				
				if (wController.Attacks[wController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
				{
					return i;
				}
			}
			else if (weapon.fistAttack)
			{
				return i;
			}

			return -1;
		}
		
		public static void CreateHitMarker(Controller controller, Transform enemy, Vector3 targetPosition)
		{
			if (controller.UIManager.CharacterUI.hitMarkers.Count > 0)
			{
				var index = Random.Range(0, controller.UIManager.CharacterUI.hitMarkers.Count);
				var hitMarker = Object.Instantiate(controller.UIManager.CharacterUI.hitMarkers[index], controller.UIManager.CharacterUI.hitMarkers[index].transform.parent);
				hitMarker.gameObject.SetActive(true);
				hitMarker.color = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, 0);
				var script = hitMarker.gameObject.AddComponent<HitMarker>();
				script.targetPosition = targetPosition;
				script.player = controller;
				
				if(enemy)
					script.enemy = enemy;
			}
		}

		public static void PlayStepSound(Surface surface, AudioSource source, int tagIndex, float volume, string type)
		{
			if (surface && surface.Material)
			{

				List<AudioClip> soundsArray = new List<AudioClip>();
				
				if (type == "character")
				{
					if (surface.CharacterFootstepsSounds.Length - 1 >= tagIndex && surface.CharacterFootstepsSounds[tagIndex].FootstepsAudios.Count > 0)
						soundsArray = surface.CharacterFootstepsSounds[tagIndex].FootstepsAudios;
				}
				else
				{
					if (surface.EnemyFootstepsSounds.Length - 1 >= tagIndex && surface.EnemyFootstepsSounds[tagIndex].FootstepsAudios.Count > 0)
						soundsArray = surface.EnemyFootstepsSounds[tagIndex].FootstepsAudios;
				}

				if (soundsArray.Count > 0)
				{
					var sound = soundsArray[Random.Range(0, soundsArray.Count - 1)];

					if (sound)
					{
						source.Stop();
						source.clip = sound;
						
						if (volume > 1)
							volume = 1;
						
						source.volume = volume;
						source.PlayOneShot(source.clip);
					}
					//else Debug.Log("(Surface [<Color=green>" + surface.Material.name + "</color>]) Not all values of footsteps sounds are filled.");
				}
				//else Debug.Log("(Surface [<Color=green>" + surface.Material.name + "</color>]) Not all values of footsteps sounds are filled.");
			}
		}

		public static void SetInventoryUI(Controller controller, InventoryManager inventoryManager)
		{
			
#if PHOTON_UNITY_NETWORKING
			if(controller.GetComponent<PhotonView>() && !controller.GetComponent<PhotonView>().IsMine)
				return;
#endif
			
			inventoryManager.ScopeScreenTexture = new RenderTexture(1024, 1024, 24);

            if (controller.UIManager.CharacterUI.Inventory.HealthButton)
            {
                controller.UIManager.CharacterUI.Inventory.normButtonsColors[8] = controller.UIManager.CharacterUI.Inventory.HealthButton.colors.normalColor;
                controller.UIManager.CharacterUI.Inventory.normButtonsSprites[8] = controller.UIManager.CharacterUI.Inventory.HealthButton.GetComponent<Image>().sprite;
            }

            if (controller.UIManager.CharacterUI.Inventory.AmmoButton)
            {
                controller.UIManager.CharacterUI.Inventory.normButtonsColors[9] = controller.UIManager.CharacterUI.Inventory.AmmoButton.colors.normalColor;
                controller.UIManager.CharacterUI.Inventory.normButtonsSprites[9] = controller.UIManager.CharacterUI.Inventory.AmmoButton.GetComponent<Image>().sprite;
            }

            if (controller.UIManager.CharacterUI.Inventory.UpWeaponButton)
            {
	            controller.UIManager.CharacterUI.Inventory.UpWeaponButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.UpWeaponButton.onClick.AddListener(delegate { inventoryManager.DownInventoryValue("weapon"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.DownWeaponButton)
            {
	            controller.UIManager.CharacterUI.Inventory.DownWeaponButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.DownWeaponButton.onClick.AddListener(delegate { inventoryManager.UpInventoryValue("weapon"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.UpHealthButton)
            {
	            controller.UIManager.CharacterUI.Inventory.UpHealthButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.UpHealthButton.onClick.AddListener(delegate { inventoryManager.UpInventoryValue("health"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.DownHealthButton)
            {
	            controller.UIManager.CharacterUI.Inventory.DownHealthButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.DownHealthButton.onClick.AddListener(delegate { inventoryManager.DownInventoryValue("health"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.UpAmmoButton)
            {
	            controller.UIManager.CharacterUI.Inventory.UpAmmoButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.UpAmmoButton.onClick.AddListener(delegate { inventoryManager.UpInventoryValue("ammo"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.DownAmmoButton)
            {
	            controller.UIManager.CharacterUI.Inventory.DownAmmoButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.DownAmmoButton.onClick.AddListener(delegate { inventoryManager.DownInventoryValue("ammo"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.HealthButton)
            {
	            controller.UIManager.CharacterUI.Inventory.HealthButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.HealthButton.onClick.AddListener(delegate { inventoryManager.UseKit("health"); });
            }

            if (controller.UIManager.CharacterUI.Inventory.AmmoButton)
            {
	            controller.UIManager.CharacterUI.Inventory.AmmoButton.onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.AmmoButton.onClick.AddListener(delegate { inventoryManager.UseKit("ammo"); });
            }
            
            for (var i = 0; i < 8; i++)
            {
	            var slot = i;

	            if (!controller.UIManager.CharacterUI.Inventory.WeaponsButtons[i])
		            continue;
                
	            controller.UIManager.CharacterUI.Inventory.WeaponsButtons[i].onClick.RemoveAllListeners();
	            controller.UIManager.CharacterUI.Inventory.WeaponsButtons[i].onClick.AddListener(delegate { inventoryManager.SelectWeaponInInventory(slot); });

	            switch (controller.UIManager.CharacterUI.Inventory.WeaponsButtons[i].transition)
	            {
		            case Selectable.Transition.ColorTint:
			            controller.UIManager.CharacterUI.Inventory.normButtonsColors[i] = controller.UIManager.CharacterUI.Inventory.WeaponsButtons[i].colors.normalColor;
			            break;
		            case Selectable.Transition.SpriteSwap:
			            controller.UIManager.CharacterUI.Inventory.normButtonsSprites[i] = controller.UIManager.CharacterUI.Inventory.WeaponsButtons[i].GetComponent<Image>().sprite;
			            break;
	            }
            }
		}
		
		public static Vector3 SpawnPosition(SpawnZone spawnPoint)
		{
			var position = new Vector3(
				spawnPoint.transform.position.x + Random.Range(-spawnPoint.transform.localScale.x / 2, spawnPoint.transform.localScale.x / 2),
				spawnPoint.transform.position.y,
				spawnPoint.transform.position.z + Random.Range(-spawnPoint.transform.localScale.z / 2, spawnPoint.transform.localScale.z / 2));

			RaycastHit hit;
			if (Physics.Raycast(position + Vector3.up, Vector3.down, out hit))
			{
				position.y = hit.point.y;
			}
			
			return position;
		}
		
				
#if PHOTON_UNITY_NETWORKING

		public static void AddMultiplayerScripts(GameObject character)
		{
			var photonView = !character.GetComponent<PhotonView>() ? character.AddComponent<PhotonView>() : character.GetComponent<PhotonView>();

			var photonAnimatorView = !character.GetComponent<PhotonAnimatorView>() ? character.AddComponent<PhotonAnimatorView>() : character.GetComponent<PhotonAnimatorView>();
			
			photonAnimatorView.SetLayerSynchronized(0, PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetLayerSynchronized(1, PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetLayerSynchronized(2, PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetLayerSynchronized(3, PhotonAnimatorView.SynchronizeType.Continuous);
			
			var transformView = !character.GetComponent<PhotonTransformView>() ? character.AddComponent<PhotonTransformView>() : character.GetComponent<PhotonTransformView>();

			var characterSync = !character.GetComponent<CharacterSync>() ? character.AddComponent<CharacterSync>() : character.GetComponent<CharacterSync>();

			if (photonView && characterSync && photonAnimatorView && transformView)
			{
				photonView.ObservedComponents = new List<Component> {characterSync, photonAnimatorView , transformView};
				photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
			}
		}
#endif

#if UNITY_EDITOR
		public static void RemoveMultiplayerScripts(GameObject character)
		{
			character.GetComponent<InventoryManager>().enabled = false;
			character.GetComponent<Controller>().enabled = false;
			
			var tempCharacter = (GameObject) PrefabUtility.InstantiatePrefab(character);

#if PHOTON_UNITY_NETWORKING
			if(tempCharacter.GetComponent<PhotonAnimatorView>())
				Object.DestroyImmediate(tempCharacter.GetComponent<PhotonAnimatorView>());
			
			if(tempCharacter.GetComponent<PhotonTransformView>())
				Object.DestroyImmediate(tempCharacter.GetComponent<PhotonTransformView>());

			if(tempCharacter.GetComponent<PhotonView>())
				Object.DestroyImmediate(tempCharacter.GetComponent<PhotonView>());
#endif
			
			if(tempCharacter.GetComponent<CharacterSync>())
				Object.DestroyImmediate(tempCharacter.GetComponent<CharacterSync>());
			
			if(tempCharacter.GetComponent<CharacterController>())
				Object.DestroyImmediate(tempCharacter.GetComponent<CharacterController>());
			
			if(tempCharacter.GetComponent<NavMeshObstacle>())
				Object.DestroyImmediate(tempCharacter.GetComponent<NavMeshObstacle>());


	#if !UNITY_2018_3_OR_NEWER
			PrefabUtility.ReplacePrefab(tempCharacter, PrefabUtility.GetPrefabParent(tempCharacter), ReplacePrefabOptions.ConnectToPrefab);
	#else
            PrefabUtility.SaveAsPrefabAssetAndConnect(tempCharacter, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempCharacter), InteractionMode.AutomatedAction);
	#endif
                    
			Object.DestroyImmediate(tempCharacter);
			
			character.GetComponent<InventoryManager>().enabled = true;
			character.GetComponent<Controller>().enabled = true;
		}

#endif

#if PHOTON_UNITY_NETWORKING
		
		public static void SetAnimatorViewComponents(PhotonAnimatorView photonAnimatorView)
		{
			photonAnimatorView.SetParameterSynchronized("Move", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Crouch", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("FPS", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("TPS", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("TDS", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Aim", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Horizontal", PhotonAnimatorView.ParameterType.Float,
				PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetParameterSynchronized("Vertical", PhotonAnimatorView.ParameterType.Float,
				PhotonAnimatorView.SynchronizeType.Continuous);

			photonAnimatorView.SetParameterSynchronized("FallingHeight", PhotonAnimatorView.ParameterType.Float,
				PhotonAnimatorView.SynchronizeType.Continuous);

			photonAnimatorView.SetParameterSynchronized("OnFloor", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Continuous);

			photonAnimatorView.SetParameterSynchronized("OnFloorForward", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Continuous);

			photonAnimatorView.SetParameterSynchronized("Jump", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Angle", PhotonAnimatorView.ParameterType.Float,
				PhotonAnimatorView.SynchronizeType.Continuous);

			photonAnimatorView.SetParameterSynchronized("CameraAngle", PhotonAnimatorView.ParameterType.Float,
				PhotonAnimatorView.SynchronizeType.Continuous);

			photonAnimatorView.SetParameterSynchronized("MoveButtonHasPressed", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Sprint", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Attack", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("PressMoveAxis", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("NoWeapons", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("HasWeaponTaken", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Disabled);

			photonAnimatorView.SetParameterSynchronized("CanWalkWithWeapon", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Disabled);

			photonAnimatorView.SetParameterSynchronized("Forward", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Backward", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Left", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("Right", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("ForwardLeft", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("ForwardRight", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("BackwardLeft", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetParameterSynchronized("BackwardRight", PhotonAnimatorView.ParameterType.Bool,
				PhotonAnimatorView.SynchronizeType.Discrete);

			photonAnimatorView.SetLayerSynchronized(0, PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetLayerSynchronized(1, PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetLayerSynchronized(2, PhotonAnimatorView.SynchronizeType.Continuous);
			photonAnimatorView.SetLayerSynchronized(3, PhotonAnimatorView.SynchronizeType.Continuous);
		}
#endif
		
	}
}

