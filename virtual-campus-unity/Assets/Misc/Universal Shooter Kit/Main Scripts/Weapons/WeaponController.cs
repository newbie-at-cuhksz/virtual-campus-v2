// GercStudio
// © 2018-2019

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class WeaponController : MonoBehaviour
    {
        public Controller Controller;
        public InventoryManager WeaponManager;
        public WeaponController OriginalScript;

        public GameObject ScopeScreen;

        public WeaponsHelper.IKObjects IkObjects;
        public CharacterHelper.BodyObjects BodyObjects;

        public List<WeaponsHelper.Attack> Attacks = new List<WeaponsHelper.Attack>() {new WeaponsHelper.Attack()};
        
        public List<WeaponsHelper.IKSlot> IkSlots = new List<WeaponsHelper.IKSlot>{new WeaponsHelper.IKSlot()};

        public IKHelper.IkDebugMode DebugMode;

        public int currentAttack;
        
        [Range(20,1)]public float scopeDepth;
        [Range(20,1)]public float aimTextureDepth;

        public float BarrelRotationSpeed;
        public WeaponsHelper.WeaponWeight Weight;

        public AudioClip DropWeaponAudio;
        public AudioClip PickUpWeaponAudio;

        public WeaponsHelper.WeaponAnimation characterAnimations;

        public Texture WeaponImage;
        public Texture AimCrosshairTexture;

        public List<WeaponsHelper.WeaponInfo> WeaponInfos = new List<WeaponsHelper.WeaponInfo>{new WeaponsHelper.WeaponInfo()};
        public List<WeaponsHelper.WeaponInfo> CurrentWeaponInfo = new List<WeaponsHelper.WeaponInfo>{new WeaponsHelper.WeaponInfo()};

        public Vector3 Direction;
        public Vector3 LastDirection;
        
        public ProjectSettings projectSettings;

        public RaycastHit Hit;

        #region Inspector Variables

        public string currentTab;
        public List<string> enumNames = new List<string>{"Slot 1"};
        public List<string> attacksNames = new List<string>{"Bullet attack"};

        public int inspectorTabTop;
        public int bulletTypeInspectorTab;
        public int inspectorTabBottom;
        public int settingsSlotIndex;
        public int lastSettingsSlotIndex;
        public int aimInspectorTabIndex;
        
        public bool ActiveDebug;
        public bool PickUpWeapon;
        public bool showCrosshairPositions;

        public Canvas inspectorCanvas;
        
        public Image upPart;
        public Image leftPart;
        public Image rightPart;
        public Image downPart;
        public Image middlePart;
        
        public string curName;

        public bool delete;
        public bool rename;
        public bool renameError;
        
        #endregion
        
        public bool autoReload;
        public bool aimForAttack = true;
        public bool canUseValuesInAdjustment;
        public bool switchToFpCamera;
        public bool wasSetSwitchToFP;
        public bool useScope;
        public bool useAimTexture;
        public bool isReloadEnabled;
        public bool reloadVoidHasBeenActivated;
        public bool attackAudioPlay;
        public bool isAimEnabled;
        public bool uiButtonAttack;
        public bool activeAimMode = true;
        public bool canAttack;
        public bool canDrawGrenadesPath = true;
        public bool setHandsPositionsAim = true;
        public bool setHandsPositionsObjectDetection = true;
        public bool setHandsPositionsCrouch = true;
        public bool enableObjectDetectionMode = true;
        public bool isShotgun;

        //Check bools for IK
        public bool CanUseIK;
        public bool CanUseElbowIK;
        public bool CanUseWallIK;
        public bool CanUseCrouchIK;
        public bool CanUseAimIK;
        public bool hasAimIKChanged;
        public bool hasWallIKChanged;
        public bool hasCrouchIKChanged;
        public bool DetectObject;
        public bool pinLeftObject;

        //Check bools for Photon synchronization
        public bool isMultiplayerWeapon;
        public bool MultiplayerReload;
        public bool MultiplayerBulletAttack;
        public bool MultiplayerFire;
        public bool MultiplayerRocket;
        public bool MultiplayerRocketRaycast;
        public bool MultiplayerChangeAttack;

        // temporary camera for processing the direction of shooting with top down view
        public Transform tempCamera;

        private bool playNoAmmoSound;
        private bool hasAttackButtonPressed;
        //private bool canGrenadeExplosion;
        private bool setColliderPosition;
        private bool aimWasSetBeforeAttack;
        public bool aimTimeout = true;
        public bool applyChanges = true;
        private bool startReload;
        public bool ActiveCrouchHands;
        public bool activeAimByGamepadButton;
        public bool firstTake;
        public bool canUseSmoothWeaponRotation;
        
        private float rateOfAttackTimer;
        private float disableAimAfterAttackTimer;
        private float currentNoiseRadius;
        public float _scatter;
        [Range(0.1f, 5)] public float setAimSpeed = 1;
        public float aimTimer;
        public float crouchTimer;
        public bool setCrouchHandsFirstly = true;

        private bool activateMeleeTimer;

        private int lastTrailPoint;
        public int animationIndex;
        public int crouchAnimationIndex;
        public int lastAttackAnimationIndex = -1;
        public int lastCrouchAttackAnimationIndex = -1;
        public int numberOfUsedHands = 2;
        
        private Vector3 lastTrailPosition;

        public IKHelper.HandsPositions RightHandPositions;
        public IKHelper.HandsPositions LeftHandPositions;

        private void OnEnable()
        {
            if (Controller && !Controller.AdjustmentScene)
            {
                WeaponsHelper.SetHandsSettingsSlot(ref settingsSlotIndex, Controller.characterTag, Controller.TypeOfCamera, this);
            }

            inspectorCanvas.gameObject.SetActive(false);
                
            aimTimeout = true;
            applyChanges = true;

            lastSettingsSlotIndex = settingsSlotIndex;
            
            CurrentWeaponInfo.Clear();

            for (var i = 0; i < WeaponInfos.Count; i++)
            {
                var info = new WeaponsHelper.WeaponInfo();
                info.Clone(WeaponInfos[i]);
                CurrentWeaponInfo.Add(info);
            }

            WeaponsHelper.PlaceWeapon(CurrentWeaponInfo[settingsSlotIndex], transform);

            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            
            if(GetComponent<BoxCollider>()) GetComponent<BoxCollider>().isTrigger = true;
            else if (GetComponent<SphereCollider>()) GetComponent<SphereCollider>().isTrigger = true;
            
            BodyObjects = Controller.BodyObjects;
                
            if (!Controller)
                    return;

            BarrelRotationSpeed = 0;

            foreach (var attack in Attacks)
            {
                if (attack.Magazine && attack.TempMagazine.Count == 0 && attack.Magazine.activeInHierarchy)
                {
                    HideAndCreateNewMagazine();
                }

                if (attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets)
                {
                    if (bulletTypeInspectorTab == 0)
                    {
                        if(attack.BulletsSettings[0].Active)
                            attack.currentBulletType = 0;
                        else attack.currentBulletType = 1;
                    }
                    else if (bulletTypeInspectorTab == 1)
                    {
                        if (attack.BulletsSettings[1].Active)
                            attack.currentBulletType = 1;
                        else attack.currentBulletType = 0;
                    }
                }
                else if(attack.AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                {
                    attack.currentBulletType = 1;
                    attack.BulletsSettings[1].Active = true;
                }
            }

            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                ChangeBulletType();

            if (!isMultiplayerWeapon)
            {
                tempCamera = new GameObject("tempCamera").transform;
                tempCamera.hideFlags = HideFlags.HideInHierarchy;

                _scatter = Attacks[currentAttack].ScatterOfBullets;
            }
            
            foreach (var attack in Attacks.Where(attack => attack.AttackCollider))
            {
                attack.AttackCollider.enabled = false;
            }

            rateOfAttackTimer = Attacks[currentAttack].RateOfAttack;

            CanUseIK = false;

            DetectObject = false;

            if (GetComponent<AudioSource>()) GetComponent<AudioSource>().enabled = true;

            if (!Controller.AdjustmentScene) SetIK();
            else StartCoroutine("SetIKTimeout");
        }

        void SetIK()
        {
            if(!IkObjects.RightObject)
                Helper.CreateObjects(IkObjects, transform, Controller.AdjustmentScene, true, Controller.projectSettings.CubesSize, Helper.CubeSolid.Wire);
            
            IKHelper.CheckIK(ref CanUseElbowIK, ref CanUseIK, ref CanUseAimIK, ref CanUseWallIK, ref CanUseCrouchIK, CurrentWeaponInfo[settingsSlotIndex]);

            if (Controller.isCrouch && CanUseCrouchIK && Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                ActiveCrouchHands = true;
            
            IKHelper.PlaceAllIKObjects(this, CurrentWeaponInfo[settingsSlotIndex],true, Controller.DirectionObject);
            
            canUseValuesInAdjustment = true;
        }

        IEnumerator SetIKTimeout()
        {
            yield return new WaitForSeconds(0.5f);
            SetIK();
            StopCoroutine("SetIKTimeout");
        }

        void Start()
        {
            if (useAimTexture)
                switchToFpCamera = true;

            if (!Controller.AdjustmentScene)
                ActiveDebug = false;


            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                Attacks[currentAttack].maxAmmo = Attacks[currentAttack].inventoryAmmo;

            Attacks[currentAttack].curAmmo = Attacks[currentAttack].maxAmmo;
            
//            if(!Controller.AdjustmentScene && !CanUseWallIK && enableObjectDetectionMode)
//                Debug.LogWarning("You haven't set the position of the hands for the Objects Detection mode. Go to the [Tools -> USK -> Adjust] to do that.", gameObject);
        }

        void Update()
        {
            if (lastSettingsSlotIndex != settingsSlotIndex)
            {
                WeaponsHelper.SetWeaponPositions(this, true, Controller.DirectionObject);
                lastSettingsSlotIndex = settingsSlotIndex;
            }

            if (Mathf.Abs(Controller.CameraController.mouseDelta.x) > 0)
                canUseSmoothWeaponRotation = true;

            WeaponsHelper.SmoothWeponMovement(this);

            if (!Controller.AdjustmentScene && Controller.ColliderToObjectsDetection && !DetectObject && setHandsPositionsObjectDetection)
            {
                if(Attacks[currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee && Attacks[currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade && Attacks[currentAttack].AttackSpawnPoint)
                    Controller.ColliderToObjectsDetection.transform.position = Attacks[currentAttack].AttackSpawnPoint.transform.position;
                else if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                    Controller.ColliderToObjectsDetection.transform.position = IkObjects.RightObject.transform.position;
            }

            currentNoiseRadius = Controller.noiseRadius * Attacks[currentAttack].AttackNoiseRadiusMultiplier;

            WeaponsHelper.MinigunBarrelRotation(this);

            rateOfAttackTimer += Time.deltaTime;
            disableAimAfterAttackTimer += Time.deltaTime;

            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && isAimEnabled && disableAimAfterAttackTimer > 5 && aimWasSetBeforeAttack && !isReloadEnabled)
            {
                Aim(true, false, false);
                aimWasSetBeforeAttack = false;
            }

            IKHelper.ManageHandsPositions(this);
            
            if (!WeaponManager || Controller && Controller.AdjustmentScene)
                return;

            if (isMultiplayerWeapon || Controller && !Controller.ActiveCharacter)
                return;
            
            CheckWall();

            if (autoReload && Attacks[currentAttack].curAmmo <= 0 && !isReloadEnabled && !startReload)
            {
                startReload = true;
                StartCoroutine("ReloadTimeout");
            }

            CheckButtons();
        }

        void PlayNoAmmoSound()
        {
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee || Controller.CameraController.cameraPause || isReloadEnabled)
                return;

            if (Attacks[currentAttack].NoAmmoShotAudio)
                GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].NoAmmoShotAudio);
        }

        public void ChangeBulletType()
        {
            Attacks[currentAttack].weapon_damage = Attacks[currentAttack].BulletsSettings[Attacks[currentAttack].currentBulletType].weapon_damage;
            Attacks[currentAttack].RateOfAttack = Attacks[currentAttack].BulletsSettings[Attacks[currentAttack].currentBulletType].RateOfShoot;
            Attacks[currentAttack].ScatterOfBullets = Attacks[currentAttack].BulletsSettings[Attacks[currentAttack].currentBulletType].ScatterOfBullets;
            _scatter = Attacks[currentAttack].ScatterOfBullets;
        }

        public void ChangeAttack()
        {
            if (!isMultiplayerWeapon)
            {
                if (isReloadEnabled /*|| WeaponManager.creategrenade*/ || WeaponManager.isPickUp || Controller.isPause || Controller.CameraController.cameraPause)
                    return;

                MultiplayerChangeAttack = true;
            }

            var newAttack = 0;
            
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].BulletsSettings[0].Active &&  Attacks[currentAttack].BulletsSettings[1].Active)
            {
                if (Attacks[currentAttack].currentBulletType == 0)
                {
                    Attacks[currentAttack].currentBulletType++;
                    ChangeBulletType();
                    WeaponManager.SetWeaponAnimations(true);
                    return;
                }
            }
            
            newAttack = currentAttack + 1;
            
            if (newAttack > Attacks.Count - 1) newAttack = 0;

            currentAttack = newAttack;
            
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].BulletsSettings[0].Active && Attacks[currentAttack].BulletsSettings[1].Active)
            {
                if (Attacks[currentAttack].currentBulletType == 1)
                {
                    Attacks[currentAttack].currentBulletType--;
                    ChangeBulletType();
                }
                ChangeBulletType();
            }
            
            if (Application.isMobilePlatform || Controller.projectSettings.mobileDebug)
            {
//                Controller.UIManager.uiButtons[5].gameObject.SetActive(!Attacks[currentAttack].autoAttack);
            }

            WeaponManager.SetCrosshair();
            WeaponManager.SetWeaponAnimations(true);
        }

        void CheckButtons()
        {
            if (!Controller.projectSettings.mobileDebug)
            {
                if (!switchToFpCamera)
                {
                    if (Controller.projectSettings.ButtonsActivityStatuses[5] && (Input.GetKey(Controller._gamepadCodes[5]) || Helper.CheckGamepadAxisButton(5, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKey", Controller.projectSettings.AxisButtonValues[5])))
                    {
                        if (!isAimEnabled)
                        {
                            activeAimByGamepadButton = true;
                            Aim(false, false, true);
                        }
                    }
                    else
                    {
                        if (isAimEnabled && activeAimByGamepadButton)
                        {
                            Aim(false, false, true);
                        }
                    }
                }
                else
                {
                    if (Controller.projectSettings.ButtonsActivityStatuses[5] && (Input.GetKeyDown(Controller._gamepadCodes[5]) || Helper.CheckGamepadAxisButton(5, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[5])))
                    {
                        activeAimByGamepadButton = false;
                        Aim(false, false, false);
                    }
                }

                if (Controller.projectSettings.ButtonsActivityStatuses[5] && Input.GetKeyDown(Controller._keyboardCodes[5]))
                {
                    activeAimByGamepadButton = false;
                    Aim(false, false, false);
                }
            }

            if (Controller.projectSettings.ButtonsActivityStatuses[19] && (Input.GetKeyDown(Controller._gamepadCodes[17]) || Input.GetKeyDown(Controller._keyboardCodes[19]) || Helper.CheckGamepadAxisButton(17, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[17])))
                ChangeAttack();


            if (!Attacks[currentAttack].autoAttack)
            {
                if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets ||
                    Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 0
                    || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                {
                    if (Controller.projectSettings.ButtonsActivityStatuses[3] && (!Controller.projectSettings.mobileDebug && (Helper.CheckGamepadAxisButton(3, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[3])
                       || Input.GetKeyDown(Controller._gamepadCodes[3]) || Input.GetKeyDown(Controller._keyboardCodes[3])) || uiButtonAttack))
                    {
                        playNoAmmoSound = false;
                        Attack(true, "Single");
                    }
                    else
                    {
                        Attack(false, "Single");
                    }
                }
                else
                {
                    if (Controller.projectSettings.ButtonsActivityStatuses[3] && (!Controller.projectSettings.mobileDebug && (Input.GetKeyDown(Controller._gamepadCodes[3]) || Input.GetKeyDown(Controller._keyboardCodes[3]) ||
                         Helper.CheckGamepadAxisButton(3, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[3])) || uiButtonAttack))
                    {
                        playNoAmmoSound = false;
                    }

                    if (Controller.projectSettings.ButtonsActivityStatuses[3] && (!Controller.projectSettings.mobileDebug && (Input.GetKey(Controller._gamepadCodes[3]) || Input.GetKey(Controller._keyboardCodes[3]) ||
                        uiButtonAttack || Helper.CheckGamepadAxisButton(3, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKey", Controller.projectSettings.AxisButtonValues[3])) || uiButtonAttack))
                    {
                        Attack(true, "Auto");
                    }
                    else
                    {
                        Attack(false, "Auto");
                    }
                }
            }
            else
            {
                if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
                {
                    if (Physics.Raycast(Controller.thisCamera.transform.position, Controller.thisCamera.transform.forward, out Hit, 10000f, Helper.layerMask()))
                    {
                        if (Hit.collider.transform.root.gameObject.GetComponent<EnemyController>() && Hit.distance <= Attacks[currentAttack].attackDistance &&
                            (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && isAimEnabled || Controller.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson))
                        {
                            if ((Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 0 ||
                                 Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee ||
                                 Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher) && rateOfAttackTimer > Attacks[currentAttack].RateOfAttack)
                            {
                                Attack(true, "Single");
                            }
                            else if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 1 ||
                                     Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Flame || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                            {
                                Attack(true, "Auto");
                            }
                        }
                        else
                        {
                            Attack(false, "Single");
                            Attack(false, "Auto");

                        }
                    }
                    else
                    {
                        Attack(false, "Single");
                        Attack(false, "Auto");
                    }
                }
                else
                {
                    if (Controller.CameraController.useCameraJoystic)
                    {
                        if ((Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 0 ||
                             Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee ||
                             Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher) && rateOfAttackTimer > Attacks[currentAttack].RateOfAttack)
                        {
                            Attack(true, "Single");
                        }
                        else if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 1 ||
                                 Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Flame || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                        {
                            Attack(true, "Auto");
                        }
                    }
                }
            }


            if (Controller.projectSettings.ButtonsActivityStatuses[4] && (Input.GetKeyDown(Controller._gamepadCodes[4]) || Input.GetKeyDown(Controller._keyboardCodes[4]) ||
                Helper.CheckGamepadAxisButton(4, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown",
                    Controller.projectSettings.AxisButtonValues[4])))
                Reload();
        }

        public void SwitchAttack(string type)
        {
            Controller.noiseRadius = Mathf.Lerp(Controller.noiseRadius, currentNoiseRadius, 5 * Time.deltaTime);
            
            switch (type)
            {
                case "Single":
                {
                    switch (Attacks[currentAttack].AttackType)
                    {
                        case WeaponsHelper.TypeOfAttack.Bullets:
                            if (Attacks[currentAttack].currentBulletType == 0)
                                BulletAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Rockets:
                        case WeaponsHelper.TypeOfAttack.GrenadeLauncher:
                            RocketAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Melee:
                            MeleeAttack();
                            break;
                        case WeaponsHelper.TypeOfAttack.Grenade:
                            var fullBody = Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !isAimEnabled;
                            ThrowGrenade(fullBody);
                            break;
                    }

                    break;
                }

                case "Auto":
                {
                    switch (Attacks[currentAttack].AttackType)
                    {
                        case WeaponsHelper.TypeOfAttack.Flame:
                            FireAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Bullets:
                            if(Attacks[currentAttack].currentBulletType == 1)
                               BulletAttack();
                            return;
                        case WeaponsHelper.TypeOfAttack.Minigun:
                            if(BarrelRotationSpeed >= 20)
                                BulletAttack();
                            break;
                    }

                    break;
                }
            }
        }

        IEnumerator SetAimBeforeAttackTimeout(string type)
        {
            while (true)
            {
                if (setHandsPositionsAim)
                {
                    yield return new WaitForSeconds(0.1f);

                    SwitchAttack(type);
                    StopCoroutine("SetAimBeforeAttackTimeout");
                    
                    break;
                }

                yield return 0;
            }
        }

        public void Attack(bool isAttack, string type)
        {
            if (isAttack && !isReloadEnabled && !Controller.isPause && !Controller.CameraController.cameraPause && canAttack && !WeaponManager.isPickUp && Attacks[currentAttack].curAmmo > 0)
            {
                disableAimAfterAttackTimer = 0;

                if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !isAimEnabled && activeAimMode && !DetectObject && aimForAttack)
                {
                    Aim(false, false, false);
                    aimWasSetBeforeAttack = true;
                    StartCoroutine(SetAimBeforeAttackTimeout(type));
                }
                else
                {
                    if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Minigun)
                        BarrelRotationSpeed += 10 * Time.deltaTime;

                    if (BarrelRotationSpeed > 30)
                        BarrelRotationSpeed = 30;

                    SwitchAttack(type);
                }
            }
            else
            {
                switch (type)
                {
                    case "Single":
                    {
                        if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee)
                        {
                            float rateOfAttackLimit;
                            
                            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                                rateOfAttackLimit = Attacks[currentAttack].RateOfAttack / 2;
                            else rateOfAttackLimit = Attacks[currentAttack].WeaponAttacks[animationIndex].length;
                            
                            if (rateOfAttackTimer > rateOfAttackLimit && activateMeleeTimer)
                            {
                                MeleeAttackOff();
                            }
                        }

                        else if (Attacks[currentAttack].WeaponAttacks[0] && rateOfAttackTimer > Attacks[currentAttack].RateOfAttack || rateOfAttackTimer > Attacks[currentAttack].WeaponAttacks[0].length || isReloadEnabled)
                        {
                            Controller.anim.SetBool("Attack", false);
                        }

                        break;
                    }

                    case "Auto":
                    {
                        if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Flame)
                            FireAttackOff();

                        Controller.anim.SetBool("Attack", false);
                        
                        BarrelRotationSpeed -= 10 * Time.deltaTime;

                        break;
                    }
                }

                if (!playNoAmmoSound && Attacks[currentAttack].curAmmo <= 0)
                {
                    PlayNoAmmoSound();
                    playNoAmmoSound = true;
                }

                if (BarrelRotationSpeed < 0)
                    BarrelRotationSpeed = 0;

            }
        }

        public void MeleeAttackOff()
        {
            if (Attacks[currentAttack].AttackCollider)
                Attacks[currentAttack].AttackCollider.enabled = false;
            
            activateMeleeTimer = false;
            Controller.anim.SetBool("Attack", false);
            Controller.anim.SetBool("Pause", false);
            Controller.anim.SetBool("MeleeAttack", false);
            
#if PHOTON_UNITY_NETWORKING
            if (!isMultiplayerWeapon && Controller.CharacterSync)
                Controller.CharacterSync.MeleeAttack(false, 0, 0);
#endif
        }

        #region BulletAttack

        public void BulletAttack()
        {
            if (rateOfAttackTimer > Attacks[currentAttack].RateOfAttack || isMultiplayerWeapon)
            {
                if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && Attacks[currentAttack].currentBulletType == 0)
                    uiButtonAttack = false;

                rateOfAttackTimer = 0;

                if (Attacks[currentAttack].AttackAudio)
                    GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].AttackAudio);
                else Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [Attack Audio].", gameObject);

                Controller.anim.SetBool("Attack", true);
                Controller.anim.CrossFade("Attack", 0, 1);

                if(!isMultiplayerWeapon)
                    MultiplayerBulletAttack = true;

                if (Attacks[currentAttack].AttackSpawnPoint)
                {
                    Attacks[currentAttack].curAmmo -= 1;

                    if (Attacks[currentAttack].MuzzleFlash)
                    {
                        var Flash = Instantiate(Attacks[currentAttack].MuzzleFlash, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                        Flash.transform.parent = gameObject.transform;
                        Helper.ChangeLayersRecursively(Flash.transform, "Character");
                    }
                    else
                    {
                        Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [MuzzleFlash].", gameObject);
                    }

                    if (Attacks[currentAttack].Shell && Attacks[currentAttack].ShellPoint && Attacks[currentAttack].SpawnShells)
                    {
                        var _shell = Instantiate(Attacks[currentAttack].Shell, Attacks[currentAttack].ShellPoint.position, Attacks[currentAttack].ShellPoint.localRotation);
                        Helper.ChangeLayersRecursively(_shell.transform, "Character");
                        _shell.hideFlags = HideFlags.HideInHierarchy;
                        _shell.gameObject.AddComponent<ShellControll>().ShellPoint = Attacks[currentAttack].ShellPoint;
                    }

                    var shotgunPoints = new List<RaycastHit>();

                    if (WeaponsHelper.UpdateAttackDirection(this, ref Hit))
                    {
                        var HitRotation = Quaternion.FromToRotation(Vector3.forward, Hit.normal);

                        if (Attacks[currentAttack].visualizeBullets)
                        {
                            WeaponsHelper.CreateTrail(Attacks[currentAttack].AttackSpawnPoint, Hit.point, WeaponManager.trailMaterial);

                            if (isShotgun)
                            {
                                for (int i = 0; i < Random.Range(4, 7); i++)
                                {
                                    var hit = new RaycastHit();

                                    if (WeaponsHelper.UpdateAttackDirection(this, ref hit))
                                    {
                                        shotgunPoints.Add(hit);
                                        WeaponsHelper.CreateTrail(Attacks[currentAttack].AttackSpawnPoint, shotgunPoints[shotgunPoints.Count - 1].point, WeaponManager.trailMaterial);
                                    }
                                }
                            }
                        }

                        var bloodHoles = new List<Texture>();
                        
                        
                        if (Hit.transform && Hit.transform.GetComponent<BodyPartCollider>())
                        {
                            var bodyColliderScript = Hit.transform.GetComponent<BodyPartCollider>();

                            if (bodyColliderScript.EnemyController)
                            {
                                var enemyScript = bodyColliderScript.EnemyController;

                                bloodHoles = enemyScript.BloodHoles;
                                
                                enemyScript.PlayDamageAnimation();
                                
                                enemyScript.EnemyHealth -= Attacks[currentAttack].weapon_damage * bodyColliderScript.damageMultiplayer;
                                enemyScript.GetShotFromWeapon(Attacks[currentAttack].currentBulletType == 0 ? 1.1f : 0.7f);
                            }
                            else if (bodyColliderScript.Controller)
                            {
                                var opponentController = bodyColliderScript.Controller;
                                if (!isMultiplayerWeapon)
                                {
                                    switch (Controller.CanKillOthers)
                                    {
                                        case PUNHelper.CanKillOthers.OnlyOpponents:
                                            if (opponentController.MyTeam != Controller.MyTeam || opponentController.MyTeam == Controller.MyTeam && Controller.MyTeam == PUNHelper.Teams.Null)
                                            {
#if PHOTON_UNITY_NETWORKING
                                                if (opponentController.PlayerHealth - Attacks[currentAttack].weapon_damage * bodyColliderScript.damageMultiplayer <= 0 && Controller.CharacterSync)
                                                {
                                                    if(bodyColliderScript.bodyPart != BodyPartCollider.BodyPart.Head)
                                                        Controller.CharacterSync.AddScore(PlayerPrefs.GetInt("NormKill"), "bullet");
                                                    else  Controller.CharacterSync.AddScore(PlayerPrefs.GetInt("Headshot"), "headshot");
                                                }
                                                
                                                if (opponentController.CharacterSync && Controller.CharacterSync)
                                                {
                                                    opponentController.CharacterSync.CreateHitMark(Controller.CharacterSync.photonView.ViewID);
                                                }
#endif
                                                opponentController.Damage((int)(Attacks[currentAttack].weapon_damage * bodyColliderScript.damageMultiplayer), Controller.CharacterName, WeaponImage, Controller.oneShotOneKill);
                                            }
                                            
                                            break;
                                        
                                        case PUNHelper.CanKillOthers.Everyone:
#if PHOTON_UNITY_NETWORKING
                                            if (opponentController.MyTeam != Controller.MyTeam || opponentController.MyTeam == Controller.MyTeam && Controller.MyTeam == PUNHelper.Teams.Null)
                                            {

                                                if (opponentController.PlayerHealth - Attacks[currentAttack].weapon_damage * bodyColliderScript.damageMultiplayer <= 0 && Controller.CharacterSync)
                                                {
                                                    if(bodyColliderScript.bodyPart != BodyPartCollider.BodyPart.Head)
                                                        Controller.CharacterSync.AddScore(PlayerPrefs.GetInt("NormKill"), "bullet");
                                                    else  Controller.CharacterSync.AddScore(PlayerPrefs.GetInt("Headshot"), "headshot");                                                
                                                }
                                            }

                                            if (opponentController.CharacterSync && Controller.CharacterSync)
                                            {
                                                opponentController.CharacterSync.CreateHitMark(Controller.CharacterSync.photonView.ViewID);
                                            }
#endif
                                            opponentController.Damage((int) (Attacks[currentAttack].weapon_damage * bodyColliderScript.damageMultiplayer), Controller.CharacterName, WeaponImage, Controller.oneShotOneKill);

                                            break;
                                        
                                        case PUNHelper.CanKillOthers.NoOne:
                                            break;
                                    }
                                }
                                
                                bloodHoles = bodyColliderScript.Controller.BloodHoles;
                            }
                            
                            if (WeaponManager.BloodProjector)
                            {
                                WeaponsHelper.CreateBlood(WeaponManager.BloodProjector, Hit.point - Direction.normalized * 0.15f, Quaternion.LookRotation(Direction), Hit.transform, bloodHoles);

                                if (isShotgun)
                                {
                                    foreach (var point in shotgunPoints)
                                    {
                                        WeaponsHelper.CreateBlood(WeaponManager.BloodProjector, point.point - Direction.normalized * 0.15f, Quaternion.LookRotation(Direction), point.transform, bloodHoles);
                                    }
                                }
                            }
                        }


                        if (Hit.collider && Hit.collider.GetComponent<FlyingProjectile>())
                        {
                            if (!Hit.collider.GetComponent<FlyingProjectile>().isTracer)
                            {
                                Hit.collider.GetComponent<FlyingProjectile>().Explosion();
                            }
                        }

                        if (Hit.transform && Hit.transform.GetComponent<Rigidbody>())
                        {
                            Hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(Direction * 800, Hit.point);
                        }

                        if (Hit.collider && Hit.collider.GetComponent<Surface>())
                        {
                            var surface = Hit.collider.GetComponent<Surface>();
                            if (surface.Material)
                            {
                                if (surface.Sparks)
                                {
                                    var _point = Hit.point + Hit.normal * Random.Range(0.01f, 0.04f);
                                    
                                    WeaponsHelper.CreateSparks(surface, _point, HitRotation, Hit.transform);

                                    if (isShotgun)
                                    {
                                        foreach (var point in shotgunPoints)
                                        {
                                            WeaponsHelper.CreateSparks(surface, _point, Quaternion.FromToRotation(Vector3.forward, point.normal), Hit.transform);
                                        }
                                    }
                                }

                                if (surface.Hit)
                                {
                                    var _point = Hit.point + Hit.normal * Random.Range(0.01f, 0.04f);

                                    WeaponsHelper.CreateHitPoint(surface, _point, Quaternion.FromToRotation(-Vector3.forward, Hit.normal), Hit.transform);

                                    foreach (var point in shotgunPoints)
                                    {
                                        WeaponsHelper.CreateHitPoint(surface, point.point + point.normal * 0.01f, Quaternion.FromToRotation(-Vector3.forward, point.normal), Hit.transform);
                                    }
                                }
                            }
                        }
                    }
//                    else
//                    {
//                        MultiplayerBulletHit = false;
//                    }
                }
                else
                {
                    Debug.LogError("(Weapon) <color=red>Missing component</color> [AttackSpawnPoint]", gameObject);
                }
            }

//            else
//            {
//                if (Attacks[currentAttack].RateOfShoot >= 1)
//                    characterAnimations.anim.SetBool("Attack", false);
//            }
        }

        #endregion

        #region RocketsAttack

        public void RocketAttack()
        {
            if ((rateOfAttackTimer > Attacks[currentAttack].RateOfAttack || isMultiplayerWeapon) && !isReloadEnabled)
            {
                uiButtonAttack = false;
                
                Controller.anim.SetBool("Attack", true);
                Controller.anim.CrossFade("Attack", 0, 1);

                if (Attacks[currentAttack].AttackAudio)
                    GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].AttackAudio);
                else Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [AttackAudio]. Add it, otherwise the sound of shooting won't be played.", gameObject);

                GameObject rocket = null;

                if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets)
                {
                    if (Attacks[currentAttack].TempMagazine[0])
                    {
                        rocket = Attacks[currentAttack].TempMagazine[0];
                        rocket.transform.parent = null;
                    }
                    else if (Attacks[currentAttack].AttackSpawnPoint && Attacks[currentAttack].Magazine)
                    {
                        rocket = Instantiate(Attacks[currentAttack].Magazine, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                        rocket.SetActive(true);
                    }
                    
                    var rocketScript = rocket.AddComponent<FlyingProjectile>();
                    rocketScript.startPosition = Controller.transform.position;
                    rocketScript.isMultiplayerWeapon = isMultiplayerWeapon;
                    rocketScript.isRocket = true;
                    rocketScript.ApplyForce = true;
                    if(WeaponImage) rocketScript.WeaponImage = WeaponImage;
                    rocketScript.Particles = Attacks[currentAttack].AttackEffects;

                    if (WeaponsHelper.UpdateAttackDirection(this, ref Hit))
                    {
                        if (!isMultiplayerWeapon)
                            MultiplayerRocket = true;

                        rocketScript.TargetPoint = Hit.point;
                    }

                    rocketScript.Camera = Controller.thisCamera.transform;
                    rocketScript.isRaycast = rocketScript.TargetPoint != Vector3.zero;
                    rocketScript.Speed = Attacks[currentAttack].FlySpeed;
                    rocketScript.explosion = Attacks[currentAttack].Explosion;
                    rocketScript.damage = Attacks[currentAttack].weapon_damage;

                    Attacks[currentAttack].TempMagazine.Clear();

                    foreach (var effect in Attacks[currentAttack].AttackEffects)
                    {
                        var effectEmission = effect.emission;
                        effectEmission.enabled = true;
                    }

                    if (Controller.CharacterName != null)
                        rocketScript.Owner = Controller;

                }
                else
                {
                    if (Attacks[currentAttack].AttackSpawnPoint && Attacks[currentAttack].Magazine)
                    {
                        rocket = Instantiate(Attacks[currentAttack].Magazine, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                        rocket.SetActive(true);
                    }
                    
                    if (!isMultiplayerWeapon)
                        MultiplayerRocket = true;

                    var grenadeScript = rocket.AddComponent<FlyingProjectile>();
                    grenadeScript.startPosition = Controller.transform.position;
                    grenadeScript.isMultiplayerWeapon = isMultiplayerWeapon;
                    grenadeScript.ApplyForce = true;
                    grenadeScript.enabled = true;
                    if(WeaponImage) grenadeScript.WeaponImage = WeaponImage;
                    grenadeScript.ExplodeWhenTouchGround = Attacks[currentAttack].ExplodeWhenTouchGround;
                    grenadeScript.Speed = Attacks[currentAttack].FlySpeed;
                    grenadeScript.GrenadeExplosionTime = Attacks[currentAttack].GrenadeExplosionTime;
                    grenadeScript.damage = Attacks[currentAttack].weapon_damage;
                    grenadeScript.ownerID = Controller.gameObject.GetInstanceID();

                    if (Controller.CharacterName != null)
                        grenadeScript.Owner = Controller;

                    if (Attacks[currentAttack].Explosion)
                        grenadeScript.explosion = Attacks[currentAttack].Explosion;

                    if (!rocket.GetComponent<BoxCollider>() && !rocket.GetComponent<SphereCollider>() && !rocket.GetComponent<MeshCollider>())
                        rocket.AddComponent<SphereCollider>();

                    grenadeScript.isGrenade = true;

                    if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.tdModeLikeTp)
                    {
                        if (!Controller.CameraParameters.lockCamera)
                        {
                            grenadeScript.m_CurrentVelocity = Controller.thisCamera.transform.TransformDirection(Vector3.up * Attacks[currentAttack].FlySpeed);
                        }
                        else
                        {
                            if (!Controller.CameraParameters.lookAtCursor)
                            {
//                                var dir = Controller.CameraController.BodyLookAt.position - Attacks[currentAttack].AttackSpawnPoint.position;
//                                var speed = Vector3.Distance(new Vector3(Controller.CameraController.BodyLookAt.position.x, 0, Controller.CameraController.BodyLookAt.position.z), new Vector3(Attacks[currentAttack].AttackSpawnPoint.position.x, 0, Attacks[currentAttack].AttackSpawnPoint.position.z));
//                                dir.y = 0;
//                                Direction = dir.normalized;
//                                grenadeScript.m_CurrentVelocity = 1.7f * speed * Direction;

                                var tempPos = Controller.CameraController.BodyLookAt.position;
                                tempPos.y = Controller.transform.position.y;

                                RaycastHit hit;
                                if (Physics.Raycast(Controller.CameraController.BodyLookAt.position, Vector3.down, out hit))
                                {
                                    tempPos = hit.point;
                                }

                                grenadeScript.m_CurrentVelocity = ProjectileHelper.ComputeVelocityToHitTargetAtTime(Attacks[currentAttack].AttackSpawnPoint.position, tempPos, Physics.gravity.y, 10 / Attacks[currentAttack].FlySpeed);
                            }
                            else
                            {
                                grenadeScript.m_CurrentVelocity = ProjectileHelper.ComputeVelocityToHitTargetAtTime(Attacks[currentAttack].AttackSpawnPoint.position, Controller.CameraController.BodyLookAt.position, Physics.gravity.y, 10 / Attacks[currentAttack].FlySpeed);
                            }
                        }
                    }
                    else
                        grenadeScript.m_CurrentVelocity = Controller.thisCamera.transform.TransformDirection(Vector3.forward * Attacks[currentAttack].FlySpeed);

                    var rigidBody = rocket.AddComponent<Rigidbody>();
                    rigidBody.useGravity = false;
                    rigidBody.isKinematic = true;

                    if (!grenadeScript.ExplodeWhenTouchGround)
                        grenadeScript.StartCoroutine("GrenadeFlying");
                }

                Attacks[currentAttack].curAmmo -= 1;
                rateOfAttackTimer = 0;
            }
        }

        #endregion

        #region FireAttack

        public void FireAttack()
        {
            if (!isReloadEnabled)
            {
                Controller.anim.SetBool("Attack", true);
                
                if (!attackAudioPlay)
                {
                    Controller.anim.CrossFade("Attack", 0, 1, Time.deltaTime, 10);
                    
                    if (Attacks[currentAttack].AttackAudio)
                    {
                        GetComponent<AudioSource>().loop = true;
                        GetComponent<AudioSource>().clip = Attacks[currentAttack].AttackAudio;
                        GetComponent<AudioSource>().Play();
                        attackAudioPlay = true;
                    }
                    else
                    {
                        Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [AttackAudio].", gameObject);
                    }
                }

                if (Attacks[currentAttack].AttackSpawnPoint && Attacks[currentAttack].AttackEffects.Length > 0)
                {
                    Attacks[currentAttack].curAmmo -= 1 * Time.deltaTime;

                    //if (!isMultiplayerWeapon)
                    MultiplayerFire = true;

                    foreach (var effect in Attacks[currentAttack].AttackEffects)
                    {
                        if (effect)
                        {
                            var _effect = Instantiate(effect, Attacks[currentAttack].AttackSpawnPoint.position, Attacks[currentAttack].AttackSpawnPoint.rotation);
                            _effect.gameObject.hideFlags = HideFlags.HideInHierarchy;
                        }
                    }

                }
                else
                {
                    Debug.LogError("(Weapon) <color=red>Missing components</color>: [AttackSpawnPoint] and/or [Fire_prefab]. Add it, otherwise the flamethrower won't attack.", gameObject);
                }

                if (Attacks[currentAttack].AttackCollider)
                    Attacks[currentAttack].AttackCollider.enabled = true;
                else
                {
                    Debug.LogError("(Weapon) <color=red>Missing components</color>: [FireCollider]. Add it, otherwise the flamethrower won't attack.", gameObject);
                    Debug.Break();
                }
            }
        }

        public void FireAttackOff()
        {
            if (Attacks[currentAttack].AttackAudio)
                if (attackAudioPlay)
                {
                    attackAudioPlay = false;
                    GetComponent<AudioSource>().Stop();
                }

            if (Attacks[currentAttack].AttackCollider)
                Attacks[currentAttack].AttackCollider.enabled = false;
        }

        #endregion

        public void ThrowGrenade(bool fullBody)
        {
            if (rateOfAttackTimer > Attacks[currentAttack].RateOfAttack || isMultiplayerWeapon)
            {
#if PHOTON_UNITY_NETWORKING
                if(!isMultiplayerWeapon && Controller.CharacterSync)
                    Controller.CharacterSync.ThrowGrenade(fullBody);
#endif

                if (!fullBody)
                {
                    uiButtonAttack = false;
                    
                    rateOfAttackTimer = 0;
                    
                    Controller.anim.SetBool("Attack", true);
                    Controller.anim.CrossFade("Attack", 0, 1);

                    StartCoroutine("FlyGrenade");
                    WeaponManager.StartCoroutine("TakeGrenade");
                }
                else
                {
                    Controller.anim.SetBool("Pause", true);
                    Controller.anim.SetBool("LaunchGrenade", true);
                    StartCoroutine("FullBodyGrenadeLaunch");
                }
            }
        }

        IEnumerator FullBodyGrenadeLaunch()
        {
            while (true)
            {
                if (Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Grenade_Throw"))
                {
                    StartCoroutine("FlyGrenade");
                    StopCoroutine("FullBodyGrenadeLaunch");
                    WeaponManager.StartCoroutine("TakeGrenade");
                    
                    break;
                }
                yield return 0;
            }

        }

        IEnumerator FlyGrenade()
        {
            yield return new WaitForSeconds(Attacks[currentAttack].WeaponAttacks[0].length);
            LaunchGrenade();
            StopCoroutine("FlyGrenade");
        }

        public void LaunchGrenade()
        {
            var tempGrenade = Instantiate(gameObject, transform.localPosition, transform.localRotation, transform.parent);

            Destroy(tempGrenade.GetComponent<WeaponController>());
            Destroy(tempGrenade.GetComponent<LineRenderer>());

            var grenadeScript = tempGrenade.AddComponent<FlyingProjectile>();
            grenadeScript.startPosition = Controller.transform.position;
            grenadeScript.isMultiplayerWeapon = isMultiplayerWeapon;
            grenadeScript.isGrenade = true;
            if (WeaponImage) grenadeScript.WeaponImage = WeaponImage;
            grenadeScript.FlashExplosion = Attacks[currentAttack].FlashExplosion;
            grenadeScript.ApplyForce = Attacks[currentAttack].ApplyForce;
            grenadeScript.stickOnObject = Attacks[currentAttack].StickToObject;

            if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.tdModeLikeTp)
            {
                if (!Controller.CameraParameters.lockCamera)
                {
                    grenadeScript.m_CurrentVelocity = Controller.thisCamera.transform.TransformDirection(Vector3.up * Attacks[currentAttack].FlySpeed);
                }
                else
                {
                    if (Controller.CameraParameters.lookAtCursor)
                    {
                        grenadeScript.m_CurrentVelocity = ProjectileHelper.ComputeVelocityToHitTargetAtTime(transform.position, Controller.CameraController.BodyLookAt.position, Physics.gravity.y, 10 / Attacks[currentAttack].FlySpeed);
                    }
                    else
                    {
//                        var dir = Controller.CameraController.BodyLookAt.position - transform.position;
//                        var speed = Vector3.Distance(new Vector3(Controller.CameraController.BodyLookAt.position.x, 0, Controller.CameraController.BodyLookAt.position.z), new Vector3(transform.position.x, 0, transform.position.z));
//                        dir.y = 0;
//                        Direction = dir.normalized;
//                        grenadeScript.m_CurrentVelocity = Direction * speed * 1.7f;

                        var tempPos = Controller.CameraController.BodyLookAt.position;
                        tempPos.y = Controller.transform.position.y;

                        RaycastHit hit;
                        if (Physics.Raycast(Controller.CameraController.BodyLookAt.position, Vector3.down, out hit))
                            tempPos = hit.point;

                        grenadeScript.m_CurrentVelocity = ProjectileHelper.ComputeVelocityToHitTargetAtTime(transform.position, tempPos, Physics.gravity.y, 10 / Attacks[currentAttack].FlySpeed);
                    }
                }
            }
            else
            {
                grenadeScript.m_CurrentVelocity = Controller.thisCamera.transform.TransformDirection(Vector3.forward * Attacks[currentAttack].FlySpeed);
            }

            grenadeScript.enabled = true;
            grenadeScript.ExplodeWhenTouchGround = Attacks[currentAttack].ExplodeWhenTouchGround;
            grenadeScript.Speed = Attacks[currentAttack].FlySpeed;
            grenadeScript.GrenadeExplosionTime = Attacks[currentAttack].GrenadeExplosionTime;
            grenadeScript.damage = Attacks[currentAttack].weapon_damage;

            if (Controller.CharacterName != null)
                grenadeScript.Owner = Controller;

            if (Attacks[currentAttack].Explosion)
                grenadeScript.explosion = Attacks[currentAttack].Explosion;

            tempGrenade.SetActive(true);
            tempGrenade.transform.parent = null;

            if (Attacks[currentAttack].AttackAudio)
                tempGrenade.GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].AttackAudio);
            else Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [Attack Audio].", gameObject);


            if (!grenadeScript.ExplodeWhenTouchGround)
                grenadeScript.StartCoroutine("GrenadeFlying");

            Attacks[currentAttack].curAmmo -= 1;

            gameObject.SetActive(false);

            canDrawGrenadesPath = false;
        }

        #region MeleeAttack

        public void MeleeAttack()
        {
            if (rateOfAttackTimer > Attacks[currentAttack].RateOfAttack || isMultiplayerWeapon)
            {
                if (!isMultiplayerWeapon)
                {
                    if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                    {
                        animationIndex = WeaponsHelper.GetRandomIndex(Attacks[currentAttack].WeaponAttacksFullBody, ref lastAttackAnimationIndex);
                        
                        crouchAnimationIndex = WeaponsHelper.GetRandomIndex(Attacks[currentAttack].WeaponAttacksFullBodyCrouch, ref lastCrouchAttackAnimationIndex);
                    }
                    else
                    {
                        animationIndex = WeaponsHelper.GetRandomIndex(Attacks[currentAttack].WeaponAttacks, ref lastAttackAnimationIndex);
                    }
                    
#if PHOTON_UNITY_NETWORKING
                    if(Controller.CharacterSync)
                        Controller.CharacterSync.MeleeAttack(true, animationIndex, crouchAnimationIndex);
#endif
                }

                if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                {
                    if (Attacks[currentAttack].WeaponAttacksFullBody[animationIndex])
                        Controller.ClipOverrides["_FullbodyMeleeAttack"] = Attacks[currentAttack].WeaponAttacksFullBody[animationIndex];

                    if (Attacks[currentAttack].WeaponAttacksFullBodyCrouch[crouchAnimationIndex])
                        Controller.ClipOverrides["_FullbodyCrouchMeleeAttack"] = Attacks[currentAttack].WeaponAttacksFullBodyCrouch[crouchAnimationIndex];
                }
                else
                {
                    if (Attacks[currentAttack].WeaponAttacks[animationIndex])
                        Controller.ClipOverrides["_WeaponAttack"] = Attacks[currentAttack].WeaponAttacks[animationIndex];
                }

                Controller.newController.ApplyOverrides(Controller.ClipOverrides);
                
                Controller.anim.CrossFade("Attack", 0, 1);

                if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                    Controller.anim.Play(!Controller.isCrouch ? "Melee" : "Crouch Melee", 0, 0);

                Controller.anim.SetBool("MeleeAttack", true);
                Controller.anim.SetBool("Attack", true);
                
                if (!isMultiplayerWeapon)
                {
                    uiButtonAttack = false;
                    
                    if (Attacks[currentAttack].AttackCollider)
                        Attacks[currentAttack].AttackCollider.enabled = true;
                    else
                    {
                        Debug.LogWarning("(Weapon) <color=red>Missing component</color> [Melee Collider].", gameObject);
                    }

                    rateOfAttackTimer = 0;

//                    StopCoroutine("MeleeAttackTimeOut");

//                    if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
//                        StartCoroutine("MeleeAttackTimeOut");
//                    else 
                    activateMeleeTimer = true;
                }
            }
        }

        #endregion

        public void Reload()
        {
            if(Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade || Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack") || 
               Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Attack") || isReloadEnabled || reloadVoidHasBeenActivated || 
               Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.isCrouch && !Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Idle") && !Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Aim_Idle"))
                return;
            
            var pause = false;

            if (!isMultiplayerWeapon)
                pause = Controller.isPause && Controller.CameraController.cameraPause;

            reloadVoidHasBeenActivated = true;
            
            if (Attacks[currentAttack].inventoryAmmo > 0 && Attacks[currentAttack].curAmmo < Attacks[currentAttack].maxAmmo && !pause && !DetectObject || isMultiplayerWeapon)
            {
                if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && isAimEnabled)
                { 
                    Aim(false, true, false);
                    
                    StartCoroutine(ReloadTimeout());
                }
                else
                {
                    isReloadEnabled = true;

                    Controller.anim.SetBool("Reload", true);
                    
                    if(!isMultiplayerWeapon)
                        MultiplayerReload = true;
                    
                    PlayReloadAudio();
                    StartCoroutine(DisableAnimation());
                    StartCoroutine(ReloadProcess());
                }

            }
        }
        
        void PlayReloadAudio()
        {
            if (Attacks[currentAttack].ReloadAudio)
                GetComponent<AudioSource>().PlayOneShot(Attacks[currentAttack].ReloadAudio);
            else
            {
                Debug.LogWarning("(Weapon) <color=yellow>Missing component</color> [ReloadAudio]. Add it, otherwise the sound of reloading won't be played.", gameObject);
            }

            StopCoroutine("PlayReloadAudio");
        }

        public void CrouchHands()
        {
            if(Controller.AdjustmentScene || !setHandsPositionsAim || !setHandsPositionsObjectDetection || Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
                return;

            if (setHandsPositionsCrouch && CanUseCrouchIK && !isAimEnabled && !DetectObject)
            {
                if (!ActiveCrouchHands)
                {
                    setHandsPositionsCrouch = false;
                    ActiveCrouchHands = true;

                    IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftHandPosition, CurrentWeaponInfo[settingsSlotIndex].RightHandPosition,
                        CurrentWeaponInfo[settingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[settingsSlotIndex].RightHandRotation, this);
                }
                else if (ActiveCrouchHands)
                {
                    if (!CurrentWeaponInfo[settingsSlotIndex].disableIkInNormalState)
                    {
                        setHandsPositionsCrouch = false;

                        IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftCrouchHandPosition, CurrentWeaponInfo[settingsSlotIndex].RightCrouchHandPosition,
                            CurrentWeaponInfo[settingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[settingsSlotIndex].RightCrouchHandRotation, this);
                    }

                    ActiveCrouchHands = false;
                }
            }
        }

        public void Aim(bool instantly, bool notSendToMultiplayer, bool gamepadInput)
        {
            if (!isMultiplayerWeapon)
            {
                if (!activeAimMode || Controller.CameraController.cameraOcclusion ||
                    isReloadEnabled /*|| DetectObject*/ ||  Controller.changeCameraTypeTimeout < 0.5f ||
                    Controller.anim.GetBool("Pause") || !aimTimeout || !Controller.anim.GetBool("HasWeaponTaken") || 
                    Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Take Weapon") || Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon") ||
                    isShotgun && (Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack") || Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Attack")))
                    return;
            }

            if (isAimEnabled)
                activeAimByGamepadButton = false;
            else
            {
                if (gamepadInput)
                    activeAimByGamepadButton = true;
            }

            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Attacks[currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee || Controller.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson)
            {
                if (CanUseAimIK)
                {
                    if (!WeaponsHelper.CanAim(isMultiplayerWeapon, Controller)) return;

                    if (!isAimEnabled && setHandsPositionsAim)
                    {
                        if (!isMultiplayerWeapon)
                        {
                            Controller.CameraController.Aim();

                            _scatter = Attacks[currentAttack].ScatterOfBullets / 2;

#if PHOTON_UNITY_NETWORKING
                            if(Controller.CharacterSync)
                                Controller.CharacterSync.Aim();
#endif
                            
                            aimTimeout = false;
                        }

                        isAimEnabled = true;
                        
                        if (!DetectObject && setHandsPositionsCrouch && setHandsPositionsObjectDetection)
                        {
                            setHandsPositionsAim = false;
                            
                            if(Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                                setCrouchHandsFirstly = false;
                            
                            IkObjects.LeftObject.parent = IkObjects.RightObject;

                            Controller.anim.SetBool("CanWalkWithWeapon", false);

                            if (!Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Take Weapon") && !Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon"))
                            {
                                Controller.anim.CrossFade("Idle", 0, 1);
                                Controller.anim.CrossFade("Idle", 0, 2);
                            }

                            if (Controller.isCrouch && Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                            {
                                IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftCrouchHandPosition, CurrentWeaponInfo[settingsSlotIndex].RightCrouchHandPosition,
                                    CurrentWeaponInfo[settingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[settingsSlotIndex].RightCrouchHandRotation, this);
                            }
                            else
                            {
                                IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftHandPosition, CurrentWeaponInfo[settingsSlotIndex].RightHandPosition,
                                    CurrentWeaponInfo[settingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[settingsSlotIndex].RightHandRotation, this);
                            }
                        }
                    }
                    else if (isAimEnabled && setHandsPositionsAim)
                    {
                        if (switchToFpCamera && Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !instantly && !isMultiplayerWeapon)
                        {
                            Controller.CameraController.DeepAim();
                            return;
                        }

                        if (!isMultiplayerWeapon)
                        {
                            Controller.CameraController.Aim();
                            _scatter = Attacks[currentAttack].ScatterOfBullets;
                            aimTimeout = false;
                            
#if PHOTON_UNITY_NETWORKING
                            if (!notSendToMultiplayer && Controller.CharacterSync)
                                Controller.CharacterSync.Aim();
#endif
                        }

                        isAimEnabled = false;
                        
                        if (!DetectObject && setHandsPositionsCrouch && setHandsPositionsObjectDetection)
                        {
                            aimWasSetBeforeAttack = false;
                            setHandsPositionsAim = false;
                            
                            if(Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                                setCrouchHandsFirstly = false;
                            
                            IkObjects.LeftObject.parent = IkObjects.RightObject;

                            Controller.anim.SetBool("CanWalkWithWeapon", false);

                            if (!Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Take Weapon") && !Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon"))
                            {
                                Controller.anim.CrossFade("Idle", 0, 1);
                                Controller.anim.CrossFade("Idle", 0, 2);
                            }

                            IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftAimPosition, CurrentWeaponInfo[settingsSlotIndex].RightAimPosition,
                                CurrentWeaponInfo[settingsSlotIndex].LeftAimRotation, CurrentWeaponInfo[settingsSlotIndex].RightAimRotation, this);

                        }
                    }
                }
                else
                {
                    Debug.LogWarning("You don't set positions of character's hands for aiming. Open the Adjustment Scene [Tools -> USK -> Adjust] to do that.", gameObject);
                }
            }
            else
            {
                if (!WeaponsHelper.CanAim(isMultiplayerWeapon, Controller)) return;
                
                Controller.CameraController.Aim();
            }
        }
        
        void CheckWall()
        {
            if (!CanUseWallIK || !Controller.anim.GetBool("HasWeaponTaken") || 
                Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Take Weapon") || Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon")
                || !enableObjectDetectionMode || Controller.AdjustmentScene || 
                Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !setHandsPositionsCrouch || !setHandsPositionsAim || !setHandsPositionsObjectDetection || 
                isShotgun && Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
                return;
            
            var mask = ~ (LayerMask.GetMask("Character") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Grass") | LayerMask.GetMask("Head"));

            var size = Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee ? 5 : 10;
            
            var hitColliders = Physics.OverlapBox(Controller.ColliderToObjectsDetection.position, Vector3.one / size, Controller.ColliderToObjectsDetection.rotation, mask);
            
            if (DetectObject)
            {
                var nearObject = false;

                foreach (var col in hitColliders)
                {
                    if (!col) continue;
                    
                    if (!col.transform.root.GetComponent<Controller>() && !col.transform.root.CompareTag("Smoke"))
                        nearObject = true;
                }

                if (nearObject) return;
                
                DetectObject = false;
                setHandsPositionsObjectDetection = false;
                applyChanges = false;
                Controller.CameraController.LayerCamera.SetActive(false);
                
                Controller.anim.SetBool("CanWalkWithWeapon", false);
                
                if (!Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Take Weapon") &&
                    !Controller.anim.GetCurrentAnimatorStateInfo(2).IsName("Take Weapon"))
                {
                    Controller.anim.CrossFade("Idle", 0, 1);
                    Controller.anim.CrossFade("Idle", 0, 2);
                }
                
                IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftHandWallPosition, CurrentWeaponInfo[settingsSlotIndex].RightHandWallPosition,
                    CurrentWeaponInfo[settingsSlotIndex].LeftHandWallRotation, CurrentWeaponInfo[settingsSlotIndex].RightHandWallRotation, this);

//                if (IkObjects.RightObject)
//                    IKHelper.SmoothPositionChange(IkObjects.RightObject, CurrentWeaponInfo[SettingsSlotIndex].RightHandWallPosition,
//                        CurrentWeaponInfo[SettingsSlotIndex].RightHandWallRotation, BodyObjects.TopBody);
//
//                if (IkObjects.LeftObject && numberOfUsedHands == 1)
//                    //(Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee))
//                    IKHelper.SmoothPositionChange(IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandWallPosition,
//                        CurrentWeaponInfo[SettingsSlotIndex].LeftHandWallRotation, BodyObjects.TopBody);
            }
            else
            {

                if (hitColliders.Any(collider => !collider.transform.root.GetComponent<Controller>() && !collider.transform.root.CompareTag("Smoke")))
                {
                    if (!isMultiplayerWeapon)
                        Controller.CameraController.LayerCamera.SetActive(true);
                    
                    DetectObject = true;
                    applyChanges = false;
                    setHandsPositionsObjectDetection = false;

                    if (isAimEnabled)
                    {
                        IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftAimPosition, CurrentWeaponInfo[settingsSlotIndex].RightAimPosition,
                            CurrentWeaponInfo[settingsSlotIndex].LeftAimRotation, CurrentWeaponInfo[settingsSlotIndex].RightAimRotation, this);
                    }
                    else
                    {
                        if (!Controller.isCrouch)
                        {
                            IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftHandPosition, CurrentWeaponInfo[settingsSlotIndex].RightHandPosition,
                                CurrentWeaponInfo[settingsSlotIndex].LeftHandRotation, CurrentWeaponInfo[settingsSlotIndex].RightHandRotation, this);
                        }
                        else
                        {
                            IKHelper.ChangeIKPosition(CurrentWeaponInfo[settingsSlotIndex].LeftCrouchHandPosition, CurrentWeaponInfo[settingsSlotIndex].RightCrouchHandPosition,
                                CurrentWeaponInfo[settingsSlotIndex].LeftCrouchHandRotation, CurrentWeaponInfo[settingsSlotIndex].RightCrouchHandRotation, this);
                        }
                    }
//                    if (IkObjects.RightObject)
//                        IKHelper.SmoothPositionChange(IkObjects.RightObject, CurrentWeaponInfo[SettingsSlotIndex].RightHandPosition,
//                            CurrentWeaponInfo[SettingsSlotIndex].RightHandRotation, BodyObjects.TopBody);
//
//                    if (IkObjects.LeftObject && numberOfUsedHands == 1)
//                        //(Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade || Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee))
//                        IKHelper.SmoothPositionChange(IkObjects.LeftObject, CurrentWeaponInfo[SettingsSlotIndex].LeftHandPosition,
//                            CurrentWeaponInfo[SettingsSlotIndex].LeftHandRotation, BodyObjects.TopBody);
                }
            }
        }

        IEnumerator DisableAnimation()
        {
            yield return new WaitForSeconds(Attacks[currentAttack].WeaponReload.length);

            Controller.anim.SetBool("CanWalkWithWeapon", false);
            Controller.anim.SetBool("Reload", false);

            StartCoroutine("WalkWithWeaponTimeout");
            StopCoroutine("DisableAnimation");
        }

        IEnumerator ReloadTimeout()
        {
            while (true)
            {
                if (setHandsPositionsAim)
                {
                    yield return new WaitForSeconds(0.1f);
                    
                    isReloadEnabled = true;
                    Controller.anim.SetBool("Reload", true);

                    if (!isMultiplayerWeapon)
                        MultiplayerReload = true;

                    PlayReloadAudio();
                    StartCoroutine(DisableAnimation());
                    StartCoroutine(ReloadProcess());

                    StopCoroutine("ReloadTimeout");
                    break;
                }

                yield return 0;
            }
        }

        IEnumerator ReloadProcess()
        {
            yield return new WaitForSeconds(Attacks[currentAttack].WeaponReload.length - 0.3f);
            
            isReloadEnabled = false;
            reloadVoidHasBeenActivated = false;

            if (Attacks[currentAttack].inventoryAmmo < Attacks[currentAttack].maxAmmo - Attacks[currentAttack].curAmmo)
            {
                Attacks[currentAttack].curAmmo += Attacks[currentAttack].inventoryAmmo;
                Attacks[currentAttack].inventoryAmmo = 0;
            }
            else
            {
                Attacks[currentAttack].inventoryAmmo -= Attacks[currentAttack].maxAmmo - Attacks[currentAttack].curAmmo;
                Attacks[currentAttack].curAmmo += Attacks[currentAttack].maxAmmo - Attacks[currentAttack].curAmmo;
            }

            attackAudioPlay = false;
            startReload = false;
            
            StopCoroutine("ReloadProcess");
        }

        public IEnumerator WalkWithWeaponTimeout()
        {
            yield return new WaitForSeconds(0.5f);
            Controller.anim.SetBool("CanWalkWithWeapon", true);
            StopCoroutine("WalkWithWeaponTimeout");
        }

        public void HideAndCreateNewMagazine()
        {
            Attacks[currentAttack].Magazine.SetActive(false);
            
            if (Attacks[currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Rockets)
            {
                foreach (var effect in Attacks[currentAttack].AttackEffects)
                {
                    if (effect.emission.enabled)
                    {
                        var effectEmission = effect.emission;
                        effectEmission.enabled = false;
                    }
                }
            }
            
            var oldMag = Attacks[currentAttack].Magazine;
            var newMag = Instantiate(oldMag);
            newMag.transform.parent = oldMag.transform.parent;
            newMag.transform.localPosition = oldMag.transform.localPosition;
            newMag.transform.localEulerAngles = oldMag.transform.localEulerAngles;
            newMag.SetActive(true);
            Attacks[currentAttack].TempMagazine.Add(newMag);
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
//            Handles.zTest = CompareFunction.Less;
//            Handles.color = Color.green;
//            Handles.CubeHandleCap(0, Controller.ColliderToObjectsDetection.position, Controller.ColliderToObjectsDetection.rotation, 0.1f, EventType.Repaint);
//            
//            Handles.zTest = CompareFunction.Greater;
//            Handles.color = new Color(0, 1, 0, 0.3f);
//            Handles.CubeHandleCap(0, Controller.ColliderToObjectsDetection.position, Controller.ColliderToObjectsDetection.rotation, 0.1f, EventType.Repaint);

            
            if(Application.isPlaying || Attacks.Any(attack => attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade))
                return;
            
            if (Attacks[currentAttack].ShellPoint)
            {
                Handles.zTest = CompareFunction.Less;
                Handles.color = new Color32(250, 170, 0, 255);
                Handles.SphereHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.identity, 0.02f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.LookRotation(Attacks[currentAttack].ShellPoint.forward), 0.2f, EventType.Repaint);

                Handles.zTest = CompareFunction.Greater;
                Handles.color = new Color32(250, 170, 0, 50);
                Handles.SphereHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.identity, 0.02f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].ShellPoint.position, Quaternion.LookRotation(Attacks[currentAttack].ShellPoint.forward), 0.2f, EventType.Repaint);
            }

            if (Attacks[currentAttack].AttackSpawnPoint)
            {
                Handles.zTest = CompareFunction.Less;
                Handles.color = new Color32(250, 0, 0, 255);
                Handles.SphereHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.identity, 0.02f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.LookRotation(Attacks[currentAttack].AttackSpawnPoint.forward),
                    0.2f, EventType.Repaint);

                Handles.zTest = CompareFunction.Greater;
                Handles.color = new Color32(250, 0, 0, 50);
                Handles.SphereHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.identity, 0.02f, EventType.Repaint);
                Handles.ArrowHandleCap(0, Attacks[currentAttack].AttackSpawnPoint.position, Quaternion.LookRotation(Attacks[currentAttack].AttackSpawnPoint.forward),
                    0.2f, EventType.Repaint);
            }
        }
#endif
    }
}





	


				
	
	