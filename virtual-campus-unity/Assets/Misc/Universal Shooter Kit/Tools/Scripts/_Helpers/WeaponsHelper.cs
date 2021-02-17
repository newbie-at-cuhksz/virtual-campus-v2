using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
	public static class WeaponsHelper 
	{
		[Serializable]
		public class WeaponInfo
		{
			public Vector3 SaveTime;
			public Vector3 SaveDate;
			
			public bool HasTime;
			public bool disableIkInNormalState;
			public bool disableIkInCrouchState;
			public bool disableElbowIK = true;

			public Vector3 WeaponSize;
            
			public Vector3 WeaponPosition;
			public Vector3 WeaponRotation;
            
			public Vector3 RightHandPosition;
			public Vector3 LeftHandPosition;
            
			public Vector3 RightHandRotation;
			public Vector3 LeftHandRotation;
			
			public Vector3 RightCrouchHandPosition;
			public Vector3 LeftCrouchHandPosition;
            
			public Vector3 RightCrouchHandRotation;
			public Vector3 LeftCrouchHandRotation;
            
			public Vector3 RightAimPosition;
			public Vector3 LeftAimPosition;
            
			public Vector3 RightAimRotation;
			public Vector3 LeftAimRotation;

			public Vector3 RightElbowPosition;
			public Vector3 LeftElbowPosition;

			public Vector3 RightHandWallPosition;
			public Vector3 LeftHandWallPosition;
            
			public Vector3 RightHandWallRotation;
			public Vector3 LeftHandWallRotation;

			public float FingersRightX;
			public float FingersLeftX;
            
			public float FingersRightY;
			public float FingersLeftY;
            
			public float FingersRightZ;
			public float FingersLeftZ;
            
			public float ThumbRightX;
			public float ThumbLeftX;
            
			public float ThumbRightY;
			public float ThumbLeftY;
            
			public float ThumbRightZ;
			public float ThumbLeftZ;

//			public Vector3 CheckWallsColliderSize = Vector3.one;
//			public Vector3 CheckWallsBoxPosition;
//			public Vector3 CheckWallsBoxRotation;
			
			public float timeInHand_FPS = 2;
			public float timeBeforeCreating_FPS = 1;
			
			public float timeInHand_TPS = 2;
			public float timeBeforeCreating_TPS = 1;

			public void CloneToAim(WeaponInfo CloneFrom, string CloneFromState)
			{
				switch (CloneFromState)
				{
					case "Aim":
						RightAimPosition = CloneFrom.RightAimPosition;
						RightAimRotation = CloneFrom.RightAimRotation;
						LeftAimPosition = CloneFrom.LeftAimPosition;
						LeftAimRotation = CloneFrom.LeftAimRotation;
						break;
					case "Norm":
						RightAimPosition = CloneFrom.RightHandPosition;
						RightAimRotation = CloneFrom.RightHandRotation;
						LeftAimPosition = CloneFrom.LeftHandPosition;
						LeftAimRotation = CloneFrom.LeftHandRotation;
						break;
					case "Wall":
						RightAimPosition = CloneFrom.RightHandWallPosition;
						RightAimRotation = CloneFrom.RightHandWallRotation;
						LeftAimPosition = CloneFrom.LeftHandWallPosition;
						LeftAimRotation = CloneFrom.LeftHandWallRotation;
						break;
					case "Crouch":
						RightAimPosition = CloneFrom.RightCrouchHandPosition;
						RightAimRotation = CloneFrom.RightCrouchHandRotation;
						LeftAimPosition = CloneFrom.LeftCrouchHandPosition;
						LeftAimRotation = CloneFrom.LeftCrouchHandRotation;
						break;
				}

				WeaponClone(CloneFrom);
			}

			public void CloneToNorm(WeaponInfo CloneFrom, string CloneFromState)
			{
				switch (CloneFromState)
				{
					case "Aim":
						RightHandPosition = CloneFrom.RightAimPosition;
						RightHandRotation = CloneFrom.RightAimRotation;
						LeftHandPosition = CloneFrom.LeftAimPosition;
						LeftHandRotation = CloneFrom.LeftAimRotation;
						break;
					case "Norm":
						disableIkInNormalState = CloneFrom.disableIkInNormalState;
						
						RightHandPosition = CloneFrom.RightHandPosition;
						RightHandRotation = CloneFrom.RightHandRotation;
						LeftHandPosition = CloneFrom.LeftHandPosition;
						LeftHandRotation = CloneFrom.LeftHandRotation;
						break;
					case "Wall":
						RightHandPosition = CloneFrom.RightHandWallPosition;
						RightHandRotation = CloneFrom.RightHandWallRotation;
						LeftHandPosition = CloneFrom.LeftHandWallPosition;
						LeftHandRotation = CloneFrom.LeftHandWallRotation;
						break;
					case "Crouch":
						RightHandPosition = CloneFrom.RightCrouchHandPosition;
						RightHandRotation = CloneFrom.RightCrouchHandRotation;
						LeftHandPosition = CloneFrom.LeftCrouchHandPosition;
						LeftHandRotation = CloneFrom.LeftCrouchHandRotation;
						break;
				}
				
				WeaponClone(CloneFrom);
			}

			public void CloneToWall(WeaponInfo CloneFrom, string CloneFromState)
			{
				
				switch (CloneFromState)
				{
					case "Aim":
						RightHandWallPosition = CloneFrom.RightAimPosition;
						RightHandWallRotation = CloneFrom.RightAimRotation;
						LeftHandWallPosition = CloneFrom.LeftAimPosition;
						LeftHandWallRotation = CloneFrom.LeftAimRotation;
						break;
					case "Norm":
						RightHandWallPosition = CloneFrom.RightHandPosition;
						RightHandWallRotation = CloneFrom.RightHandRotation;
						LeftHandWallPosition = CloneFrom.LeftHandPosition;
						LeftHandWallRotation = CloneFrom.LeftHandRotation;
						break;
					case "Wall":
						RightHandWallPosition = CloneFrom.RightHandWallPosition;
						RightHandWallRotation = CloneFrom.RightHandWallRotation;
						LeftHandWallPosition = CloneFrom.LeftHandWallPosition;
						LeftHandWallRotation = CloneFrom.LeftHandWallRotation;
						break;
					case "Crouch":
						RightHandWallPosition = CloneFrom.RightCrouchHandPosition;
						RightHandWallRotation = CloneFrom.RightCrouchHandRotation;
						LeftHandWallPosition = CloneFrom.LeftCrouchHandPosition;
						LeftHandWallRotation = CloneFrom.LeftCrouchHandRotation;
						break;
				}

//				CheckWallsColliderSize = CloneFrom.CheckWallsColliderSize;
//				CheckWallsBoxPosition = CloneFrom.CheckWallsBoxPosition;
//				CheckWallsBoxRotation = CloneFrom.CheckWallsBoxRotation;
				
				WeaponClone(CloneFrom);
			}

			public void CloneToCrouch(WeaponInfo CloneFrom, string CloneFromState)
			{
				switch (CloneFromState)
				{
					case "Aim":
						RightCrouchHandPosition = CloneFrom.RightAimPosition;
						RightCrouchHandRotation = CloneFrom.RightAimRotation;
						LeftCrouchHandPosition = CloneFrom.LeftAimPosition;
						LeftCrouchHandRotation = CloneFrom.LeftAimRotation;
						break;
					case "Norm":
						RightCrouchHandPosition = CloneFrom.RightHandPosition;
						RightCrouchHandRotation = CloneFrom.RightHandRotation;
						LeftCrouchHandPosition = CloneFrom.LeftHandPosition;
						LeftCrouchHandRotation = CloneFrom.LeftHandRotation;
						break;
					case "Wall":
						RightCrouchHandPosition = CloneFrom.RightHandWallPosition;
						RightCrouchHandRotation = CloneFrom.RightHandWallRotation;
						LeftCrouchHandPosition = CloneFrom.LeftHandWallPosition;
						LeftCrouchHandRotation = CloneFrom.LeftHandWallRotation;
						break;
					case "Crouch":
						disableIkInCrouchState = CloneFrom.disableIkInCrouchState;
						
						RightCrouchHandPosition = CloneFrom.RightCrouchHandPosition;
						RightCrouchHandRotation = CloneFrom.RightCrouchHandRotation;
						LeftCrouchHandPosition = CloneFrom.LeftCrouchHandPosition;
						LeftCrouchHandRotation = CloneFrom.LeftCrouchHandRotation;
						break;
				}

				WeaponClone(CloneFrom);
			}

			public void ElbowsClone(WeaponInfo CloneFrom)
			{
				disableElbowIK = CloneFrom.disableElbowIK;
				
				RightElbowPosition = CloneFrom.RightElbowPosition;
				LeftElbowPosition = CloneFrom.LeftElbowPosition;
			}

			public void FingersClone(WeaponInfo CloneFrom)
			{
				FingersRightX = CloneFrom.FingersRightX;
				FingersRightY = CloneFrom.FingersRightY;
				FingersRightZ = CloneFrom.FingersRightZ;

				FingersLeftX = CloneFrom.FingersLeftX;
				FingersLeftY = CloneFrom.FingersLeftY;
				FingersLeftZ = CloneFrom.FingersLeftZ;

				ThumbRightX = CloneFrom.ThumbRightX;
				ThumbRightY = CloneFrom.ThumbRightY;
				ThumbRightZ = CloneFrom.ThumbRightZ;

				ThumbLeftX = CloneFrom.ThumbLeftX;
				ThumbLeftY = CloneFrom.ThumbLeftY;
				ThumbLeftZ = CloneFrom.ThumbLeftZ;
			}

			public void WeaponClone(WeaponInfo CloneFrom)
			{
				WeaponSize = CloneFrom.WeaponSize;
				WeaponPosition = CloneFrom.WeaponPosition;
				WeaponRotation = CloneFrom.WeaponRotation;
			}

			public void Clone(WeaponInfo CloneFrom)
			{
				HasTime = CloneFrom.HasTime;
				SaveTime = CloneFrom.SaveTime;
				SaveDate = CloneFrom.SaveDate;
				
				WeaponClone(CloneFrom);

				CloneToNorm(CloneFrom, "Norm");
				
				CloneToCrouch(CloneFrom, "Crouch");
				
				CloneToAim(CloneFrom, "Aim");

				CloneToWall(CloneFrom, "Wall");
				
				ElbowsClone(CloneFrom);

				FingersClone(CloneFrom);

				timeInHand_FPS = CloneFrom.timeInHand_FPS;
				timeBeforeCreating_FPS = CloneFrom.timeBeforeCreating_FPS;

				timeInHand_TPS = CloneFrom.timeInHand_TPS;
				timeBeforeCreating_TPS = CloneFrom.timeBeforeCreating_TPS;
			}
		}
		
		[Serializable]
		public class WeaponAnimation
		{
			public AnimationClip WeaponIdle;
//			public AnimationClip WeaponAttack;
//			public AnimationClip WeaponReload;
			public AnimationClip TakeWeapon;
			public AnimationClip WeaponWalk;
			public AnimationClip WeaponRun;
			public AnimationClip RemoveWeapon;

//			public Animator anim;
		}

		[Serializable]
		public class BulletsSettings
		{
			public bool Active = true;
			[Range(1, 100)] public int weapon_damage;
			[Range(0, 10)] public float RateOfShoot = 0.5f;
			[Range(0, 0.1f)] public float ScatterOfBullets;
		}

		[Serializable]
		public class Attack
		{
			public TypeOfAttack AttackType = TypeOfAttack.Bullets;
			public Helper.RotationAxes BarrelRotationAxes;
			public List<BulletsSettings> BulletsSettings = new List<BulletsSettings>{new BulletsSettings(), new BulletsSettings()};
			
			public List<AnimationClip> WeaponAttacks = new List<AnimationClip>{null};
			public List<AnimationClip> WeaponAttacksFullBody = new List<AnimationClip>{null};
			public List<AnimationClip> WeaponAttacksFullBodyCrouch = new List<AnimationClip>{null};
			public AnimationClip WeaponAutoShoot;
			public AnimationClip WeaponReload;
			
			[Range(0, 10)] public float RateOfAttack = 0.5f;
			[Range(0, 0.1f)] public float ScatterOfBullets;
			[Range(1, 50)] public float FlySpeed = 20;
			[Range(0.1f, 30)]public float GrenadeExplosionTime = 3;
			[Range(1, 100)]public float attackDistance = 50;
			[Range(10, 200)] public float CrosshairSize = 25;
			[Range(1, 20)] public float AttackNoiseRadiusMultiplier = 2;
			//public List<float> attackEffectsEmissionValues = new List<float>();
			
			public float curAmmo = 12;
			public float maxAmmo = 12;
			public float inventoryAmmo = 24;
			
			[Range(0, 100)] public int weapon_damage = 10;
			public int inspectorTab;
			public int currentBulletType;
			
			public Vector2[] crosshairPartsPositions = 
			{
				new Vector2(0, 0), new Vector2(0, 30), new Vector2(0, -30), new Vector2(30, 0), new Vector2(-30, 0)
			};

			public List<GameObject> TempMagazine = new List<GameObject>();
			public GameObject Magazine;
			public GameObject Barrel;
			public GameObject MuzzleFlash;
			public GameObject Shell;
			
			public Transform AttackSpawnPoint;
			public Transform ShellPoint;
			public Transform Explosion;
			
			public BoxCollider AttackCollider;

			public AnimationCurve bobShooting;
			
			public ParticleSystem[] AttackEffects;
			
			public AudioClip AttackAudio;
			public AudioClip ReloadAudio;
			public AudioClip NoAmmoShotAudio;
			
			public Sprite UpPart;
			public Sprite DownPart;
			public Sprite LeftPart;
			public Sprite RightPart;
			public Sprite MiddlePart;

			public CrosshairType sightType;

			public bool StickToObject;
			public bool autoAttack;
			public bool visualizeBullets = true;
			public bool ExplodeWhenTouchGround = true;
			public bool showTrajectory = true;
			public bool SpawnShells = true;
			public bool ApplyForce = true;
			public bool FlashExplosion;

            public bool isToxin;

			public string AmmoType = "gun";
		}

		[Serializable]
		public class IKSlot
		{
			public int fpsSettingsSlot;
			public int tpsSettingsSlot;
			public int tdsSettingsSlot;
			
			public int currentTag;
		}

		[Serializable]
		public class WeaponSlotInInventory
		{
			public GameObject weapon;

			public bool fistAttack;
			
//			public int tpSlotIndex;
//			public int tdSlotIndex;
//			public int fpSlotIndex;
//			public List<string> saveSlotsNames;
		}

		[Serializable]
		public class GrenadeSlot
		{
			public GameObject Grenade;
			public int grenadeAmmo;
//			public int saveSlotIndex;
			public WeaponController GrenadeScript;
		}
		
		[Serializable]
		public class IKObjects
		{
			public Transform RightObject;
			public Transform LeftObject;
			
			public Transform RightAimObject;
			public Transform LeftAimObject;
			
			public Transform RightCrouchObject;
			public Transform LeftCrouchObject;

			public Transform RightWallObject;
			public Transform LeftWallObject;

			public Transform RightElbowObject;
			public Transform LeftElbowObject;
		}

		[Serializable]
		public class GrenadeParameters
		{
			[Range(1, 100)] public float GrenadeSpeed = 20;
			[Range(0.1f, 30)]public float GrenadeExplosionTime = 3;
			
			public bool ExplodeWhenTouchGround;
			
			public GameObject GrenadeExplosion;
			
			public AudioClip ThrowAudio;

			public AnimationClip GrenadeThrow_FPS;
			public AnimationClip GrenadeThrow_TPS_TDS;
		}
		
//		[Serializable]
//		public class GrenadeInfo
//		{
//			
//		}

		public enum WeaponWeight
		{
			Light, Medium, Heavy
		}
		
		public enum TypeOfAttack
		{
			Bullets,
			Rockets,
			GrenadeLauncher,
			Flame,
			Melee,
			Grenade,
			Minigun
        }

		public enum CrosshairType
		{
			OnePart, TwoParts, FourParts
		}
		

		public static void PlaceWeapon(WeaponInfo weaponInfo, Transform target)
		{
			if (weaponInfo.WeaponSize != Vector3.zero)
				target.localScale = weaponInfo.WeaponSize;

			target.localPosition = weaponInfo.WeaponPosition;
			target.localEulerAngles = weaponInfo.WeaponRotation;
		}

		public static void SetWeaponPositions(WeaponController weaponController, bool placeAll, Transform dirObj)
		{

			IKHelper.CheckIK(ref weaponController.CanUseElbowIK, ref weaponController.CanUseIK, ref weaponController.CanUseAimIK, 
					ref weaponController.CanUseWallIK, ref weaponController.CanUseCrouchIK, weaponController.CurrentWeaponInfo[weaponController.settingsSlotIndex]);

			var slot = weaponController.CurrentWeaponInfo[weaponController.settingsSlotIndex];

			IKHelper.PlaceAllIKObjects(weaponController, slot, placeAll, dirObj);

			PlaceWeapon(weaponController.CurrentWeaponInfo[weaponController.settingsSlotIndex], weaponController.transform);
		}

//		static void ResetIKValues(WeaponController script)
//		{
//			ResetIKValue(ref script.CurrentWeaponInfo.RightHandPosition, ref script.CurrentWeaponInfo.RightHandRotation,
//				script.IkObjects.RightObject, script.Controller.RightHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftHandPosition, ref script.CurrentWeaponInfo.LeftHandRotation,
//				script.IkObjects.LeftObject, script.Controller.LeftHand, script.Controller.TopBody);
//			
//			ResetIKValue(ref script.CurrentWeaponInfo.RightAimPosition, ref script.CurrentWeaponInfo.RightAimRotation,
//				script.IkObjects.RightAimObject, script.Controller.RightHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftAimPosition, ref script.CurrentWeaponInfo.LeftAimRotation,
//				script.IkObjects.LeftAimObject, script.Controller.LeftHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.RightHandWallPosition,
//				ref script.CurrentWeaponInfo.RightHandWallRotation,
//				script.IkObjects.RightWallObject, script.Controller.RightHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftHandWallPosition,
//				ref script.CurrentWeaponInfo.LeftHandWallRotation,
//				script.IkObjects.LeftWallObject, script.Controller.LeftHand, script.Controller.TopBody);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.RightElbowPosition, script.IkObjects.RightElbowObject,
//				script.Controller, script.Controller.transform.right);
//
//			ResetIKValue(ref script.CurrentWeaponInfo.LeftElbowPosition, script.IkObjects.LeftElbowObject,
//				script.Controller, -script.Controller.transform.right);
//		}

//		static bool CheckIKValue(Vector3 value1, Vector3 value2, Transform obj, Transform parent)
//		{
//			if (value1 == Vector3.zero & value2 == Vector3.zero)
//			{
//				obj.parent = parent;
//				obj.localPosition = Vector3.zero;
//				obj.localRotation = Quaternion.Euler(-90, 0, 0);
//				//obj.parent = parent2;
//
//				return false;
////				value1 = obj.localPosition;
////				value2 = obj.localEulerAngles;
//			}
//
//			return true;
//		}

//		static bool CheckIKValue(Vector3 value1, Transform obj, Vector3 dir)
//		{
//			if (value1 == Vector3.zero)
//			{
//				obj.localPosition = dir * 2;
//				value1 = obj.localPosition;
//				return false;
//			}
//
//			return true;
//		}

		public static void SetHandsSettingsSlot(ref int SettingsSlotIndex, int tag, WeaponController weaponController, bool enable)
		{
			if (weaponController.IkSlots.Any(slot => slot.currentTag == tag))
			{
				var _slot = weaponController.IkSlots.Find(slot => slot.currentTag == tag);

				SettingsSlotIndex = enable ? _slot.fpsSettingsSlot : _slot.tpsSettingsSlot;
			}
		}

		public static void SetHandsSettingsSlot(ref int SettingsSlotIndex, int tag, CharacterHelper.CameraType type, WeaponController weaponController)
		{
			if (weaponController.IkSlots.Any(slot => slot.currentTag == tag))
			{
				var _slot = weaponController.IkSlots.Find(slot => slot.currentTag == tag);
				
				switch (type)
				{
					case CharacterHelper.CameraType.FirstPerson:
						SettingsSlotIndex = _slot.fpsSettingsSlot;
						break;
					case CharacterHelper.CameraType.ThirdPerson:
						SettingsSlotIndex = !weaponController.Controller.tdModeLikeTp ? _slot.tpsSettingsSlot : _slot.tdsSettingsSlot;
						break;
					case CharacterHelper.CameraType.TopDown:
						SettingsSlotIndex = _slot.tdsSettingsSlot;
						break;
				}
			}
			else
			{
				switch (type)
				{
					case CharacterHelper.CameraType.FirstPerson:
						SettingsSlotIndex = weaponController.IkSlots[0].fpsSettingsSlot;
						break;
					case CharacterHelper.CameraType.ThirdPerson:
						SettingsSlotIndex = weaponController.IkSlots[0].tpsSettingsSlot;
						break;
					case CharacterHelper.CameraType.TopDown:
						SettingsSlotIndex = weaponController.IkSlots[0].tdsSettingsSlot;
						break;
				}
			}
		}

		public static void MinigunBarrelRotation(WeaponController weaponController)
		{
			if (weaponController.Attacks[weaponController.currentAttack].AttackType == TypeOfAttack.Minigun && !weaponController.Controller.isPause && !weaponController.Controller.CameraController.cameraPause)
			{
				if (weaponController.Attacks[weaponController.currentAttack].Barrel)
				{
					var transformLocalEulerAngles = weaponController.Attacks[weaponController.currentAttack].Barrel.transform.localEulerAngles;
					switch (weaponController.Attacks[weaponController.currentAttack].BarrelRotationAxes)
					{
						case Helper.RotationAxes.X:
							transformLocalEulerAngles.x += weaponController.BarrelRotationSpeed;
							break;
						case Helper.RotationAxes.Y:
							transformLocalEulerAngles.y += weaponController.BarrelRotationSpeed;
							break;
						case Helper.RotationAxes.Z:
							transformLocalEulerAngles.z += weaponController.BarrelRotationSpeed;
							break;
					}
					transformLocalEulerAngles.y += weaponController.BarrelRotationSpeed;
					weaponController.Attacks[weaponController.currentAttack].Barrel.transform.localEulerAngles = transformLocalEulerAngles;
				}
			}
		}

		public static void SetWeaponController(GameObject instantiatedWeapon, GameObject originalWeapon, int saveSlot, InventoryManager manager, Controller controller, Transform parent)
		{
			var weaponController = instantiatedWeapon.GetComponent<WeaponController>();

			SetWeaponController(weaponController, instantiatedWeapon, originalWeapon, parent, controller.BodyObjects);

//			weaponController.tpsSettingsSlot = saveSlot;
			weaponController.WeaponManager = manager;
			weaponController.Controller = controller;
			
			weaponController.enabled = true;
		}

		public static void SetWeaponController(GameObject instantiatedWeapon, GameObject originalWeapon, InventoryManager manager, Controller controller, Transform parent)
		{
			var weaponController = instantiatedWeapon.GetComponent<WeaponController>();
			
			SetWeaponController(weaponController, instantiatedWeapon, originalWeapon, parent, controller.BodyObjects);
			
			weaponController.WeaponManager = manager;
			weaponController.Controller = controller;
			
			weaponController.enabled = true;
		}
		
		public static void InstantiateWeapon(GameObject weapon, int index, InventoryManager manager, Controller controller)
		{
			var name = weapon.name;
			var instantiatedWeapon = Object.Instantiate(weapon);
			instantiatedWeapon.name = name;
		
			manager.slots[index].currentWeaponInSlot = 0;
			manager.slots[index].weaponSlotInGame.Add(new CharacterHelper.Weapon {weapon = instantiatedWeapon});
			manager.hasAnyWeapon = true;
			manager.allWeaponsCount++;
			
//			if(controller.AdjustmentScene)
//				manager.slots[index].weaponSlotInInspector[0].weapon = instantiatedWeapon;

			SetWeaponController(instantiatedWeapon, weapon, manager, controller, controller.transform);
		}
		
//		public static void SetWeaponController (GameObject instantiatedWeapon, GameObject originalWeapon, AIController controller, Transform parent)
//		{
//			var weaponController = instantiatedWeapon.GetComponent<WeaponController>();
//			
//			SetWeaponController(weaponController, instantiatedWeapon, originalWeapon,parent, controller.BodyObjects);
//			
//			weaponController.WeaponParent = Helper.Parent.Enemy;
//			weaponController.AIController = controller;
//			
//			weaponController.enabled = true;
//			
//		}

		static void SetWeaponController(WeaponController weaponController, GameObject instantiatedWeapon, GameObject originalWeapon, Transform parent, CharacterHelper.BodyObjects objects)
		{
			weaponController = instantiatedWeapon.GetComponent<WeaponController>();
			weaponController.OriginalScript = originalWeapon.GetComponent<WeaponController>();
//			weaponController.Parent = parent;

			foreach (var attack in weaponController.Attacks)
			{
				attack.curAmmo = attack.AttackType != TypeOfAttack.Grenade ? attack.maxAmmo : attack.inventoryAmmo;
			}

//			if (weaponController.Attacks[weaponController.currentAttack].AttackType != TypeOfAttack.Grenade)
//			{
				if (objects.RightHand && instantiatedWeapon.transform.parent != objects.RightHand)
					instantiatedWeapon.transform.parent = objects.RightHand;
//			}
//			else
//			{
//				if (objects.LeftHand && instantiatedWeapon.transform.parent != objects.LeftHand)
//					instantiatedWeapon.transform.parent = objects.LeftHand;
//			}
		}

		public static int GetRandomIndex(List<AnimationClip> animations, ref int lastAttackAnimationIndex)
		{
			var animationIndex = Random.Range(0, animations.Count);
                    
			if (animationIndex == lastAttackAnimationIndex)
			{
				animationIndex++;

				if (animationIndex > animations.Count - 1)
					animationIndex = 0;
			}

			lastAttackAnimationIndex = animationIndex;

			return animationIndex;
		}

		public static void AddTrail(GameObject Tracer, Vector3 TargetPoint, Material Material, float Size)
		{
			var tracerScript = Tracer.gameObject.AddComponent<FlyingProjectile>();
			Tracer.hideFlags = HideFlags.HideInHierarchy;
			tracerScript.isTracer = true;
			tracerScript.TargetPoint = TargetPoint;
			tracerScript.Speed = 300;
			var trail = Tracer.AddComponent<TrailRenderer>();
			trail.shadowCastingMode = ShadowCastingMode.Off;
			trail.autodestruct = true;
			trail.textureMode = LineTextureMode.Stretch;
			trail.receiveShadows = false;
			trail.time = 3;
			trail.material = Material;
			trail.widthMultiplier = 0.1f;
			var curve = new AnimationCurve();
			curve.AddKey(0, Size);
			curve.AddKey(1, 0);
			trail.widthCurve = curve;
			trail.startColor = new Color32(255, 100, 0, 255);
			trail.endColor = new Color32(255, 200, 0, 200);
		}

		public static void CreateTrail(Transform AttackPoint, Vector3 DirectionPoint, Material TrailMaterial)
		{
			var tracer = new GameObject("Tracer");
			tracer.transform.position = AttackPoint.position;
			tracer.transform.rotation = AttackPoint.rotation;
                                
			AddTrail(tracer, DirectionPoint, TrailMaterial, 0.3f);
		}

		public static void CreateSparks(Surface surface, Vector3 position, Quaternion rotation, Transform parent)
		{
			var spark = GameObject.Instantiate(surface.Sparks, position, rotation);
			spark.hideFlags = HideFlags.HideInHierarchy;
			spark.transform.parent = parent;
			
			if (surface.HitAudio)
			{
				var _audio = !parent.gameObject.GetComponent<AudioSource>() ? parent.gameObject.AddComponent<AudioSource>() : parent.gameObject.GetComponent<AudioSource>();
				_audio.clip = surface.HitAudio;
				_audio.spatialBlend = 1;
				_audio.minDistance = 10;
				_audio.maxDistance = 100;
				_audio.PlayOneShot(parent.gameObject.GetComponent<AudioSource>().clip);
			}
		}

		public static void CreateHitPoint(Surface surface, Vector3 position, Quaternion rotation, Transform parent)
		{
			var hitGO = GameObject.Instantiate(surface.Hit, position, rotation).transform;
			hitGO.hideFlags = HideFlags.HideInHierarchy;
			hitGO.parent = parent;
		}

		public static bool CanAim(bool isMultiplayerWeapon, Controller controller)
		{
			var canAim = false;
			
			if (!isMultiplayerWeapon)
			{
				if (controller.TypeOfCamera != CharacterHelper.CameraType.TopDown && !controller.isPause && !controller.CameraController.Occlusion)
				{
					canAim = true;
				}
			}
			else
			{
				canAim = true;
			}

			return canAim;
		}

		public static void CreateBlood(Projector BloodProjector, Vector3 Position, Quaternion Rotation, Transform Parent, List<Texture> BloodHoles)
		{
			var hole = Object.Instantiate(BloodProjector, Position, Rotation);
			hole.transform.parent = Parent;
			hole.gameObject.hideFlags = HideFlags.HideInHierarchy;
                                    
			var index = Random.Range(0, BloodHoles.Count - 1);
                                    
			if(BloodHoles[index])
				hole.material.mainTexture = BloodHoles[index];
		}
		
		public static bool UpdateAttackDirection(WeaponController script, ref RaycastHit raycastHit)
        {
            if (!script.DetectObject && !script.Controller.CameraController.cameraOcclusion && (script.Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && script.isAimEnabled || script.Controller.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson))
            {
                if (Mathf.Abs(script.Controller.anim.GetFloat("CameraAngle")) < 60)
                {
	                if (script.Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || script.Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && script.Controller.tdModeLikeTp)
	                {
		                if (!script.Controller.CameraParameters.lockCamera)
		                {
			                script.tempCamera.position = script.Controller.thisCamera.transform.position;
			                script.tempCamera.rotation = Quaternion.Euler(90, script.Controller.thisCamera.transform.eulerAngles.y, script.Controller.thisCamera.transform.eulerAngles.z);
			                script.Direction = script.tempCamera.TransformDirection(Vector3.up + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), 0));
		                }
		                else
		                {
			                if (script.Controller.CameraParameters.lookAtCursor)
			                {
				                if (!script.Controller.bodyLimit)
				                {
					                var dir = script.Controller.CameraController.BodyLookAt.position - script.Attacks[script.currentAttack].AttackSpawnPoint.position;
					                script.Direction = dir + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter)) * 10;
					                script.LastDirection = script.Controller.transform.InverseTransformDirection(dir);
				                }
				                else
				                {
					                var dir = script.Controller.transform.TransformDirection(script.LastDirection);
					                script.Direction = dir + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter)) * 10;
				                }
			                }
			                else
			                {
				                var tempPos = script.Controller.CameraController.BodyLookAt.position;
				                tempPos = new Vector3(tempPos.x, script.Attacks[script.currentAttack].AttackSpawnPoint.position.y, tempPos.z);
				                var dir = tempPos - script.Attacks[script.currentAttack].AttackSpawnPoint.position;
				                script.Direction = dir + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter)) * 10;
			                }
		                }

//                        Debug.DrawRay(script.Attacks[script.currentAttack].AttackSpawnPoint.position, script.Direction, Color.red,100);
		                if (Physics.Raycast(script.Attacks[script.currentAttack].AttackSpawnPoint.position, script.Direction, out raycastHit, 10000f, Helper.layerMask()))
			                return true;
	                }
	                else
	                {
		                script.Direction = script.Controller.thisCamera.transform.TransformDirection(Vector3.forward + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), 0));

		                if (Physics.Raycast(script.Controller.thisCamera.transform.position, script.Direction, out raycastHit, 10000f, Helper.layerMask()))
			                return true;

		                raycastHit = new RaycastHit();
		                raycastHit.point = script.Attacks[script.currentAttack].AttackSpawnPoint.position + script.Controller.thisCamera.transform.forward * 100 + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), 0);
		                return true;
	                }
                }
                else
                {
	                raycastHit = new RaycastHit();
	                raycastHit.point = script.Attacks[script.currentAttack].AttackSpawnPoint.position + script.Attacks[script.currentAttack].AttackSpawnPoint.forward * 100 + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), 0);
	                return true;
                }
            }
            else
            {
	            raycastHit = new RaycastHit();
	            raycastHit.point = script.Attacks[script.currentAttack].AttackSpawnPoint.position + script.Attacks[script.currentAttack].AttackSpawnPoint.forward * 100 + new Vector3(Random.Range(-script._scatter, script._scatter), Random.Range(-script._scatter, script._scatter), 0);
	            return true;
            }

            return false;
        }
		
		public static void ShowGrenadeTrajectory(bool enable, Transform startPoint, LineRenderer lineRenderer, Controller controller, WeaponController weaponController, Transform characterTransform)
        {
            if (enable)
            {
                if(!lineRenderer.enabled)
                    lineRenderer.enabled = true;
                
                lineRenderer.startColor = new Color(1, 1, 1, Mathf.Lerp(lineRenderer.startColor.a, 0, 5 * Time.deltaTime));
                lineRenderer.endColor = new Color(1, 1, 1, Mathf.Lerp(lineRenderer.endColor.a, 1, 5 * Time.deltaTime));
                
                var lastPoint = 0;
                var lastPos = Vector3.zero;
                for (int i = 0; i < 50; i++)
                {
                    var currentTime = 0.05f * i;

                    var trajectoryPosition = Vector3.zero;
                    
                    if (controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && controller.tdModeLikeTp)
                    {
                        if (!controller.CameraParameters.lockCamera)
                        {
                            trajectoryPosition = ProjectileHelper.ComputePositionAtTimeAhead(startPoint.position, controller.thisCamera.transform.TransformDirection(Vector3.up * weaponController.Attacks[weaponController.currentAttack].FlySpeed), Physics.gravity.y, currentTime);
                        }
                        else
                        {
	                        var starPoint = weaponController.Attacks[weaponController.currentAttack].AttackType != TypeOfAttack.Grenade ? weaponController.Attacks[weaponController.currentAttack].AttackSpawnPoint.position : weaponController.transform.position;
                           
	                        if (weaponController.Controller.CameraParameters.lookAtCursor)
                            {
	                            trajectoryPosition = ProjectileHelper.ComputePositionAtTimeAhead(startPoint.position, ProjectileHelper.ComputeVelocityToHitTargetAtTime(starPoint, weaponController.Controller.CameraController.BodyLookAt.position, Physics.gravity.y, 10 / weaponController.Attacks[weaponController.currentAttack].FlySpeed), Physics.gravity.y, currentTime);
                            }
                            else
                            {
//	                            var dir = weaponController.Controller.CameraController.BodyLookAt.position - starPoint;
//	                            var speed = Vector3.Distance(new Vector3(weaponController.Controller.CameraController.BodyLookAt.position.x, 0, weaponController.Controller.CameraController.BodyLookAt.position.z), new Vector3(starPoint.x, 0, starPoint.z));
//	                            dir.y = 0;
//	                            var velocity = dir.normalized * speed * 1.8f;
//	                            trajectoryPosition = ProjectileHelper.ComputePositionAtTimeAhead(startPoint.position, velocity, Physics.gravity.y, currentTime);

	                            var tempPos = weaponController.Controller.CameraController.BodyLookAt.position;
	                            tempPos.y = weaponController.Controller.transform.position.y;

	                            RaycastHit hit;
	                            if (Physics.Raycast(weaponController.Controller.CameraController.BodyLookAt.position, Vector3.down, out hit))
		                            tempPos = hit.point;

	                            trajectoryPosition = ProjectileHelper.ComputePositionAtTimeAhead(startPoint.position, ProjectileHelper.ComputeVelocityToHitTargetAtTime(starPoint, tempPos, Physics.gravity.y, 10 / weaponController.Attacks[weaponController.currentAttack].FlySpeed), Physics.gravity.y, currentTime);
                            }
                        }
                    }
                    else
                    {
                        trajectoryPosition = ProjectileHelper.ComputePositionAtTimeAhead(startPoint.position, controller.thisCamera.transform.TransformDirection(Vector3.forward * weaponController.Attacks[weaponController.currentAttack].FlySpeed), Physics.gravity.y, currentTime);
                    }

                    lineRenderer.SetPosition(i, trajectoryPosition);

                    if (Physics.OverlapSphere(trajectoryPosition, 0.1f, Helper.layerMask()).Length > 0)
                    {
                        lastPoint = i;
                        lastPos = trajectoryPosition;
                        break;
                    }
                }

                if (lastPoint > 0)
                {
                    for (int i = lastPoint; i < 50; i++)
                    {
                        lineRenderer.SetPosition(i, lastPos);
                    }
                }
            }
            else
            {
                if (lineRenderer)
                {
                    lineRenderer.startColor = new Color(1, 1, 1, Mathf.Lerp(lineRenderer.startColor.a, 0, 7 * Time.deltaTime));
                    lineRenderer.endColor = new Color(1, 1, 1, Mathf.Lerp(lineRenderer.endColor.a, 0, 7 * Time.deltaTime));

                    if (lineRenderer.endColor == new Color(1, 1, 1, 0) && lineRenderer.enabled)
                        lineRenderer.enabled = false;
                }
            }
        }

		public static void SmoothWeponMovement(WeaponController weaponController)
		{
			if (!weaponController.isMultiplayerWeapon && !weaponController.Controller.AdjustmentScene && !weaponController.firstTake && weaponController.Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson &&
			    weaponController.Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Idle") && weaponController.Controller.anim.GetBool("HasWeaponTaken") 
                && weaponController.Controller.anim.GetBool("CanWalkWithWeapon"))
            {
	            SetHand(ref weaponController.RightHandPositions, weaponController.IkObjects.RightObject, ref weaponController.firstTake);
	            SetHand(ref weaponController.LeftHandPositions, weaponController.IkObjects.LeftObject, ref weaponController.firstTake);
            }
            else if (weaponController.canUseSmoothWeaponRotation && !weaponController.isMultiplayerWeapon && weaponController.Controller.smoothWeapons && !weaponController.Controller.AdjustmentScene && weaponController.firstTake && weaponController.Controller.anim.GetBool("HasWeaponTaken") && !weaponController.Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Take Weapon") &&
                     !weaponController.isAimEnabled && weaponController.setHandsPositionsAim && !weaponController.DetectObject && weaponController.setHandsPositionsObjectDetection && weaponController.Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson &&
                     (weaponController.isShotgun && !weaponController.Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack") || !weaponController.isShotgun))
            {
	            
	            var rotationLimit = 0;
	            var positionLimit = 0.3f;
	            var speed = 6f;
                
	            switch (weaponController.Weight)
	            {
		            case WeaponWeight.Light:
			            rotationLimit = 6;
			            speed = 9;
			            break;
		            case WeaponWeight.Medium:
			            rotationLimit = 7;
			            speed = 7;
			            break;
		            case WeaponWeight.Heavy:
			            rotationLimit = 9;
			            speed = 6;
			            break;
	            }

	            CalculateHandPosition(ref weaponController.RightHandPositions, weaponController.IkObjects.RightObject, weaponController, rotationLimit, positionLimit, speed);

	            if (weaponController.numberOfUsedHands == 1)//weaponController.Attacks[weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee || weaponController.Attacks[weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade)
	            {
		            CalculateHandPosition(ref weaponController.LeftHandPositions, weaponController.IkObjects.LeftObject, weaponController, rotationLimit, positionLimit, speed / 1.5f);
	            }
            }
		}

		static void SetHand(ref IKHelper.HandsPositions handsPositions, Transform ikHandle, ref bool firstTake)
		{
			handsPositions.startHandRotation = new Vector2(ikHandle.localEulerAngles.x, ikHandle.localEulerAngles.y);
			handsPositions.startHandPosition = new Vector2(ikHandle.localPosition.y, ikHandle.localPosition.z);

			if (handsPositions.startHandRotation.x > 180)
				handsPositions.startHandRotation.x -= 360;

			if (handsPositions.startHandRotation.y > 180)
				handsPositions.startHandRotation.y -= 360;

			handsPositions.currentHandRotation = handsPositions.startHandRotation;
			handsPositions.currentHandPosition = handsPositions.startHandPosition;

			firstTake = true;
		}

		static void CalculateHandPosition(ref IKHelper.HandsPositions handsPositions, Transform ikHandle, WeaponController weaponController, float rotationLimit, float positionLimit, float speed)
		{
			if (weaponController.Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Idle") || weaponController.Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Walk") || 
                    weaponController.Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
                {
	                handsPositions.desiredHandRotation.x = handsPositions.startHandRotation.x + weaponController.Controller.CameraController.mouseDelta.x;
	                handsPositions.desiredHandRotation.y = handsPositions.startHandRotation.y + weaponController.Controller.CameraController.mouseDelta.y;
                    
	                handsPositions.desiredHandPosition.x = handsPositions.startHandPosition.x + weaponController.Controller.CameraController.mouseDelta.x / 500;
                    handsPositions.desiredHandPosition.y = handsPositions.startHandPosition.y + weaponController.Controller.CameraController.mouseDelta.y / 500;
                }
                else
                {
                    handsPositions.desiredHandRotation.x = handsPositions.startHandRotation.x;
                    handsPositions.desiredHandRotation.y = handsPositions.startHandRotation.y;

                    handsPositions.desiredHandPosition.x = handsPositions.startHandPosition.x;
                    handsPositions.desiredHandPosition.y = handsPositions.startHandPosition.y;
                }

                if (handsPositions.desiredHandRotation.x > 180)
                    handsPositions.desiredHandRotation.x -= 360;

                if (handsPositions.desiredHandRotation.y > 180)
                    handsPositions.desiredHandRotation.y -= 360;
                

                if (handsPositions.desiredHandRotation.x > handsPositions.startHandRotation.x + rotationLimit) handsPositions.desiredHandRotation.x = handsPositions.startHandRotation.x + rotationLimit;
                else if (handsPositions.desiredHandRotation.x < handsPositions.startHandRotation.x - rotationLimit) handsPositions.desiredHandRotation.x = handsPositions.startHandRotation.x - rotationLimit;

                if (handsPositions.desiredHandRotation.y > handsPositions.startHandRotation.y + rotationLimit) handsPositions.desiredHandRotation.y = handsPositions.startHandRotation.y + rotationLimit;
                else if (handsPositions.desiredHandRotation.y < handsPositions.startHandRotation.y - rotationLimit) handsPositions.desiredHandRotation.y = handsPositions.startHandRotation.y - rotationLimit;

                handsPositions.currentHandRotation.x = Mathf.Lerp(handsPositions.currentHandRotation.x, handsPositions.desiredHandRotation.x, speed * Time.deltaTime);
                handsPositions.currentHandRotation.y = Mathf.Lerp(handsPositions.currentHandRotation.y, handsPositions.desiredHandRotation.y, speed * Time.deltaTime);
                
                if (handsPositions.desiredHandPosition.x > handsPositions.startHandPosition.x + positionLimit) handsPositions.desiredHandPosition.x = handsPositions.startHandPosition.x + positionLimit;
                else if (handsPositions.desiredHandPosition.x < handsPositions.startHandPosition.x - positionLimit) handsPositions.desiredHandPosition.x = handsPositions.startHandPosition.x - positionLimit;
                
                if (handsPositions.desiredHandPosition.y > handsPositions.startHandPosition.y + positionLimit) handsPositions.desiredHandPosition.y = handsPositions.startHandPosition.y + positionLimit;
                else if (handsPositions.desiredHandPosition.y < handsPositions.startHandPosition.y - positionLimit) handsPositions.desiredHandPosition.y = handsPositions.startHandPosition.y - positionLimit;
                
                handsPositions.currentHandPosition.x = Mathf.Lerp(handsPositions.currentHandPosition.x, handsPositions.desiredHandPosition.x, speed / 2 * Time.deltaTime);
                handsPositions.currentHandPosition.y = Mathf.Lerp(handsPositions.currentHandPosition.y, handsPositions.desiredHandPosition.y, speed / 2 *Time.deltaTime);
                
                ikHandle.localEulerAngles = new Vector3(handsPositions.currentHandRotation.x, handsPositions.currentHandRotation.y, ikHandle.localEulerAngles.z);
                ikHandle.localPosition = new Vector3(ikHandle.localPosition.x, handsPositions.currentHandPosition.x,  handsPositions.currentHandPosition.y);

		}
	}
}

