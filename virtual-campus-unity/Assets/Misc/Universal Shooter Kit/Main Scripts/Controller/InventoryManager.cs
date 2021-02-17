using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

namespace GercStudio.USK.Scripts
{

    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] public CharacterHelper.InventorySlot[] slots = new CharacterHelper.InventorySlot[8];

        [SerializeField] public List<CharacterHelper.Kit> HealthKits;
        [SerializeField] public List<CharacterHelper.Kit> ReserveAmmo;
        
        public GameObject currentWeapon;

        public int currentSlot;
        public int previousSlot;
        public int previousWeaponInSlot;
        public int currentAmmoKit;
        public int currentHealthKit;
        public int allWeaponsCount;

        [Range(0, 10)]public float RateOfAttack = 0.7f;
        public float FistDamage = 20;
        public float DebugIKValue = 1;
        public float SmoothIKSwitch;
        public float SmoothHeadIKSwitch;
        public float flashTimeout;

        public int inventoryTabUp;
        public int inventoryTabMiddle;
        public int currentInventorySlot;

        public string DropIdMultiplayer;
        public string currentPickUpId;
        
        public bool hasWeaponTaken;
        public bool hasAnyWeapon;
        public bool isPickUp;
        public bool pickUpUiButton;
        public bool pressInventoryButton;
        public bool hideAllWeapons;
        public bool HasFistAttack;
        public bool inventoryIsOpened;

//        public Canvas canvas;
        
        public Texture FistIcon;
        
        public LineRenderer LineRenderer;

        public Projector BloodProjector;

        public AnimationClip HandsIdle;
        public AnimationClip HandsWalk;
        public AnimationClip HandsRun;
        
        public List<AnimationClip> fistAttackHandsAnimations;
        public List<AnimationClip> fistAttackFullBodyAnimations;
       
        public AudioClip fistAttackAudio;

        public BoxCollider LeftHandCollider;
        public BoxCollider RightHandCollider;
        
        public Material trailMaterial;

//        public RawImage aimTextureImage;

//        public Image FlashImage;
        
        public RenderTexture ScopeScreenTexture;

        public Vector3 DropDirection;

        public Controller Controller;
        public WeaponController WeaponController;

        private int weaponId;
        public int animationIndex;
        public int lastAttackAnimationIndex = -1;
        
        private float _rateOfAttack;
        
        private bool activateMeleeTimer;
        private bool canChangeWeaponInSlot;
        private bool tempIsAim;
        private bool closeInventory;
        public bool hasWeaponChanged;
        private bool pressedUIInventoryButton;
        private bool canDropWeapon = true;
        private bool gamepadInfo;
        private bool setWeaponLayer = true;
        private bool firstLayerSet;
        private bool UIButtonAttack;
        private bool gamepadInput;
        private bool fistInstance;
        public bool inMultiplayerLobby;
        public bool activeAimByGamepadButton;

        private GameObject currentDropWeapon;

        private RaycastHit wallHitInfoRH;

        private void OnEnable()
        {
            if (FindObjectOfType<Lobby>())
            {
                inMultiplayerLobby = true;
                
                return;
            }

            StopAllCoroutines();
            
            Controller = GetComponent<Controller>();
            SmoothIKSwitch = 0;
            hasWeaponTaken = false;
            currentSlot = 0;
            slots[0].currentWeaponInSlot = 0;
            
            if (!Controller.AdjustmentScene && Controller.UIManager.CharacterUI.WeaponAmmo)
                Controller.UIManager.CharacterUI.WeaponAmmo.gameObject.SetActive(true);

            FindWeapons();

            if (!LineRenderer)
            {
               Helper.SetLineRenderer(ref LineRenderer, gameObject, trailMaterial);
            }

            if(!gameObject.GetComponent<AudioSource>())
                gameObject.AddComponent<AudioSource>();

            if (LeftHandCollider)
                LeftHandCollider.enabled = false;
            
            if (RightHandCollider)
                RightHandCollider.enabled = false;

            if (!fistInstance)
            { 
                CharacterHelper.SetInventoryUI(Controller, this);
                fistInstance = true;
            }
            
            //FindWeapon(true, true);
            

            if (!Controller.AdjustmentScene 
#if PHOTON_UNITY_NETWORKING
                && (Controller.GetComponent<PhotonView>() && Controller.GetComponent<PhotonView>().IsMine || !Controller.GetComponent<PhotonView>())
#endif                
                )
                    DeactivateInventory();
            FindWeapon(true, true);
        }

        void Update()
        {
            if (inMultiplayerLobby)
            {
                return;
            }

            if (Controller.isMultiplayerCharacter || !Controller.ActiveCharacter || Controller.AdjustmentScene)
                return;

            if (WeaponController && WeaponController.gameObject.activeSelf)
            {
                var startPoint = WeaponController.Attacks[WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade ? WeaponController.transform : WeaponController.Attacks[WeaponController.currentAttack].AttackSpawnPoint;
                var enable = (WeaponController.Attacks[WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade || WeaponController.Attacks[WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher && WeaponController.Attacks[WeaponController.currentAttack].showTrajectory)
                             && (WeaponController.isAimEnabled || Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown) && WeaponController.canDrawGrenadesPath && !WeaponController.isReloadEnabled && hasWeaponTaken;

                WeaponsHelper.ShowGrenadeTrajectory(enable, startPoint, LineRenderer, Controller, WeaponController, transform);
            }

            FlashEffect();

            CheckPickUp();
            
            if (Controller.projectSettings.ButtonsActivityStatuses[9] && (Input.GetKeyDown(Controller._gamepadCodes[9]) || Input.GetKeyDown(Controller._keyboardCodes[9])||
                Helper.CheckGamepadAxisButton(9, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[9])))
                DropWeapon(true);

            if (Controller.projectSettings.ButtonsActivityStatuses[16] && (Input.GetKeyDown(Controller._gamepadCodes[14]) || Input.GetKeyDown(Controller._keyboardCodes[16])||
                Helper.CheckGamepadAxisButton(14, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[14])))
                WeaponUp();

            if (Controller.projectSettings.ButtonsActivityStatuses[16] && (Input.GetKeyDown(Controller._gamepadCodes[15]) || Input.GetKeyDown(Controller._keyboardCodes[17])||
                Helper.CheckGamepadAxisButton(15, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[15])))
                WeaponDown();

            if (slots[currentSlot].weaponSlotInGame.Count > 0 && slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
            {
                if (Controller.projectSettings.ButtonsActivityStatuses[3] && (!Controller.projectSettings.mobileDebug && (Input.GetKeyDown(Controller._gamepadCodes[3]) || Input.GetKeyDown(Controller._keyboardCodes[3]) ||
                     Helper.CheckGamepadAxisButton(3, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[3])) || UIButtonAttack))
                {
                    Punch();
                }

                if (!Controller.projectSettings.mobileDebug)
                {
                    if (Controller.projectSettings.ButtonsActivityStatuses[5] && Input.GetKeyDown(Controller._keyboardCodes[5]))
                    {
                        Aim();
                        activeAimByGamepadButton = false;
                    }

                    if (Controller.projectSettings.ButtonsActivityStatuses[5] && (Input.GetKey(Controller._gamepadCodes[5]) || Helper.CheckGamepadAxisButton(5, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKey", Controller.projectSettings.AxisButtonValues[5])))
                    {
                        if (!Controller.CameraController.CameraAim)
                        {
                            Aim();
                            activeAimByGamepadButton = true;
                        }
                    }
                    else
                    {
                        if (Controller.CameraController.CameraAim && activeAimByGamepadButton)
                        {
                            Aim();
                            activeAimByGamepadButton = false;
                        }
                    }
                }
            }

            MeleeAttackTimeout();

            if (Controller.UIManager.CharacterUI.Inventory.MainObject)
            {
                if (!Application.isMobilePlatform && !Controller.projectSettings.mobileDebug)
                {
                    if (pressInventoryButton)
                    {
                        if (Controller.projectSettings.ButtonsActivityStatuses[7] && (Input.GetKey(Controller._gamepadCodes[7]) || Input.GetKey(Controller._keyboardCodes[7]) ||
                            Helper.CheckGamepadAxisButton(7, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKey",
                                Controller.projectSettings.AxisButtonValues[7])))
                        {
                            ActivateInventory();
                        }
                        else
                        {
                            if (!closeInventory)
                            {
                                DeactivateInventory();
                                closeInventory = true;
                            }
                        }
                    }
                    else
                    {
                        if (Controller.projectSettings.ButtonsActivityStatuses[7] && !inventoryIsOpened &&
                            (Input.GetKeyDown(Controller._gamepadCodes[7]) || Input.GetKeyDown(Controller._keyboardCodes[7]) || 
                             Helper.CheckGamepadAxisButton(7, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown",
                                 Controller.projectSettings.AxisButtonValues[7])))
                        {
                            ActivateInventory();
                        }
                        else if (Controller.projectSettings.ButtonsActivityStatuses[7] && inventoryIsOpened &&
                                 (Input.GetKeyDown(Controller._gamepadCodes[7]) || Input.GetKeyDown(Controller._keyboardCodes[7]) ||
                                  Helper.CheckGamepadAxisButton(7, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[7])))
                        {
                            if (!closeInventory)
                            {
                                DeactivateInventory();
                                closeInventory = true;
                            }
                        }
                    }
                }
                else
                {
                    if (pressInventoryButton)
                    {
                        if (pressedUIInventoryButton)
                        {
                            ActivateInventory();
                        }
                        else
                        {
                            if (!closeInventory)
                            {
                                DeactivateInventory();
                                closeInventory = true;
                            }
                        }
                    }
                }

                if (!Controller.AdjustmentScene && inventoryIsOpened)
                    CheckInventoryButtons();
            }

            if (Controller.UIManager.CharacterUI.WeaponAmmo && !inventoryIsOpened)
            {
                if (WeaponController && !slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
                {
                    Controller.UIManager.CharacterUI.WeaponAmmo.color = WeaponController.Attacks[WeaponController.currentAttack].curAmmo > 0 ? Color.white : Color.red;

                    if (WeaponController.Attacks[WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee)
                    {
                        if (WeaponController.Attacks[WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                            Controller.UIManager.CharacterUI.WeaponAmmo.text = WeaponController.Attacks[WeaponController.currentAttack].curAmmo.ToString("F0") + "/" +
                                                                               WeaponController.Attacks[WeaponController.currentAttack].inventoryAmmo.ToString("F0");
                        else Controller.UIManager.CharacterUI.WeaponAmmo.text = WeaponController.Attacks[WeaponController.currentAttack].curAmmo.ToString("F0");
                    }
                    else Controller.UIManager.CharacterUI.WeaponAmmo.text = WeaponController.gameObject.name;
                }
                else Controller.UIManager.CharacterUI.WeaponAmmo.text = " ";
            }

            if (Controller.UIManager.CharacterUI.WeaponAmmoImagePlaceholder && !inventoryIsOpened)
            {
                if (slots[currentSlot].weaponSlotInGame.Count > 0 && !slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack && WeaponController)
                {
                    if (WeaponController.WeaponImage)
                        Controller.UIManager.CharacterUI.WeaponAmmoImagePlaceholder.texture = WeaponController.WeaponImage;
                }
                else if (slots[currentSlot].weaponSlotInGame.Count > 0 && slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
                {
                    if (FistIcon)
                        Controller.UIManager.CharacterUI.WeaponAmmoImagePlaceholder.texture = FistIcon;
                }
            }

            //Drop egg
            if (Input.GetKeyDown(KeyCode.E))
            {

#if PHOTON_UNITY_NETWORKING
                if (!Controller.GetComponent<PhotonView>().IsMine)
                    return;
#endif

                if (gameObject.GetComponent<EggBag>().bag.Count > 0)
                {

                    if (!pickupEggCooldown)
                    {
                        return;
                    }

                    pickupEggCooldown = false;
                    StartCoroutine(CoolDownEggPickUp());
                    int countBeforeDrop = gameObject.GetComponent<EggBag>().bag.Count;
                    gameObject.GetComponent<EggBag>().DropEgg();
                    if (countBeforeDrop - 1 > 0)
                    {
                        //can still drop the eggs since some eggs remains in bag
                        Controller.UIManager.CharacterUI.PickupEggHUD.UpdateHUD(1); //the parameter in UpdateHUD means dropping the eggs
                    }
                    else
                    {
                        Controller.UIManager.CharacterUI.PickupEggHUD.gameObject.SetActive(false);
                    }
                }
            }
              
        }

        void MeleeAttackTimeout()
        {
            _rateOfAttack += Time.deltaTime;
            
            float rateOfAttackLimit;

            if (fistAttackHandsAnimations.Count > 0 && fistAttackHandsAnimations[animationIndex])
            {
                if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                    rateOfAttackLimit = RateOfAttack / 2;
                else rateOfAttackLimit = fistAttackHandsAnimations[animationIndex].length;

                if (_rateOfAttack > rateOfAttackLimit && activateMeleeTimer)
                {
                    DisablePunchAttack();
                }

                if (_rateOfAttack > RateOfAttack && activateMeleeTimer)
                {
                    DisableColliders();

                    activateMeleeTimer = false;
                }
            }
        }

        void DisableColliders()
        {
            if (LeftHandCollider)
                LeftHandCollider.enabled = false;
                
            if (RightHandCollider)
                RightHandCollider.enabled = false;
        }

        public void DisablePunchAttack()
        {
            Controller.anim.SetBool("Attack", false);
            Controller.anim.SetBool("Pause", false);
            Controller.anim.SetBool("MeleeAttack", false);
            
#if PHOTON_UNITY_NETWORKING
            if(!Controller.isMultiplayerCharacter && Controller.CharacterSync)
                Controller.CharacterSync.MeleeAttack(false, 0);
#endif
        }

        void Aim()
        {
            if (!WeaponsHelper.CanAim(false, Controller)) return;

            if (slots[currentSlot].weaponSlotInGame.Count > 0 && slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack || slots[currentSlot].weaponSlotInGame.Count == 0)
            {
                Controller.CameraController.Aim();
            }
        }
        
        public void Punch()
        {
            if (!Controller.isMultiplayerCharacter && ((slots[currentSlot].weaponSlotInGame.Count > 0 && !slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack) || slots[currentSlot].weaponSlotInGame.Count == 0 || Controller.isPause || Controller.CameraController.cameraPause || _rateOfAttack < RateOfAttack)) return;
           
            DisablePunchAttack();

            if (!Controller.isMultiplayerCharacter)
            {
                animationIndex = Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson ? WeaponsHelper.GetRandomIndex(fistAttackFullBodyAnimations, ref lastAttackAnimationIndex) : WeaponsHelper.GetRandomIndex(fistAttackHandsAnimations, ref lastAttackAnimationIndex);
            }
                
            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
            {
                if (Controller.isCrouch || Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch->Idle") || Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Idle"))
                    return;

                if (fistAttackFullBodyAnimations[animationIndex])
                    Controller.ClipOverrides["_FullbodyMeleeAttack"] = fistAttackFullBodyAnimations[animationIndex];
            }
            else
            {
                if (fistAttackHandsAnimations[animationIndex])
                    Controller.ClipOverrides["_WeaponAttack"] = fistAttackHandsAnimations[animationIndex];
            }

            Controller.newController.ApplyOverrides(Controller.ClipOverrides);
                
            if(Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                Controller.anim.Play("Melee", 0, 0.2f);

            if (!Controller.isMultiplayerCharacter)
            {
#if PHOTON_UNITY_NETWORKING
                if(Controller.CharacterSync)
                    Controller.CharacterSync.MeleeAttack(true, animationIndex);
#endif

                if (LeftHandCollider)
                    LeftHandCollider.enabled = true;

                if (RightHandCollider)
                    RightHandCollider.enabled = true;
                    
                _rateOfAttack = 0;
                    
                activateMeleeTimer = true;
            }

            Controller.anim.SetBool("MeleeAttack", true);
            Controller.anim.SetBool("Attack", true);

            Controller.anim.CrossFade("Attack", 0, 1);
        }

        public void WeaponUp()
        {
            if (Controller.AdjustmentScene) return;

           if (WeaponController && (/*weaponController.IsAimEnabled ||*/ WeaponController.isReloadEnabled) || /*creategrenade ||*/ Controller.isPause || !hasWeaponTaken || Controller.anim.GetBool("Attack")) return;
           
            FindWeapon(true, true);
            SelectWeaponInInventory(currentSlot);

            Controller.UIManager.CharacterUI.ShowImage("weapon", this);
        }

        public void WeaponDown()
        {
            if (Controller.AdjustmentScene) return;

            if (WeaponController && ( /*weaponController.IsAimEnabled ||*/ WeaponController.isReloadEnabled) /*|| creategrenade */ || Controller.isPause || !hasWeaponTaken ||
                Controller.anim.GetBool("Attack")) return;

            FindWeapon(false, true);
            SelectWeaponInInventory(currentSlot);
            
            Controller.UIManager.CharacterUI.ShowImage("weapon", this);
        }

        void FindWeapon(bool plus, bool sendToNetwork)
        {
            if (slots[currentSlot].weaponSlotInGame.Count < 2 || CharacterHelper.FindWeapon(slots, currentSlot, slots[currentSlot].currentWeaponInSlot + (plus ? 1 : -1), plus) == -1)
            {
                Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "norm");

                hasAnyWeapon = false;
                slots[currentSlot].currentWeaponInSlot = 0;
                
                for (var i = 0; i < 8; i++)
                {
                    if (plus)
                    {
                        currentSlot++;

                        if (currentSlot > 7)
                            currentSlot = 0;
                        
                        slots[currentSlot].currentWeaponInSlot = 0;
                    }
                    else
                    {
                        currentSlot--;
                        
                        if (currentSlot < 0)
                            currentSlot = 7;
                        
                        slots[currentSlot].currentWeaponInSlot = slots[currentSlot].weaponSlotInGame.Count - 1;
                    }

                    if (slots[currentSlot].weaponSlotInGame.Count > 0 && CharacterHelper.FindWeapon(slots, currentSlot, slots[currentSlot].currentWeaponInSlot, plus) != -1)
                    {
                        slots[currentSlot].currentWeaponInSlot = CharacterHelper.FindWeapon(slots, currentSlot, slots[currentSlot].currentWeaponInSlot, plus);
                        
                        if (slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
                        {
                            hideAllWeapons = true;

                            GetNewWeapon(sendToNetwork);

                            hasAnyWeapon = false;
                            
                            break;
                        }

                        if (!WeaponController || WeaponController && WeaponController.gameObject.GetInstanceID() != slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].weapon.GetInstanceID())
                        {
                            hasWeaponChanged = true;

                            GetNewWeapon(sendToNetwork);

//                           Switch(currentSlot, false, true);
                            hasAnyWeapon = true;
                            break;
                        }

//                        hasAnyWeapon = true;
                    }
                }
                
                if (!hasAnyWeapon)
                {
//                    if (Controller.UIManager.CharacterUI.WeaponAmmo)
//                        Controller.UIManager.CharacterUI.WeaponAmmo.gameObject.SetActive(false);

                    hideAllWeapons = true;

                    GetNewWeapon(sendToNetwork);
                }
            }
            else
            {
                slots[currentSlot].currentWeaponInSlot = CharacterHelper.FindWeapon(slots, currentSlot, slots[currentSlot].currentWeaponInSlot + (plus ? 1 : -1), plus);

                if (slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
                {
                    hideAllWeapons = true;
                }
                else
                {
                    hasWeaponChanged = true;
                }

                GetNewWeapon(sendToNetwork);

            }
        }

        void GetNewWeapon(bool sendToNetwork)
        {
            if (WeaponController)
            {
                if (WeaponController.isAimEnabled)
                {
                    WeaponController.Aim(true, false, false);
                    StartCoroutine("SwitchWeaponTimeOut");
                }
                else
                {
                    SwitchNewWeapon(sendToNetwork);
                }
            }
            else
            {
                SwitchNewWeapon(sendToNetwork);
            }
        }

        void InventoryGamepadInputs()
        {
            var vector = Controller.projectSettings._Stick == ProjectSettings.Stick.MovementStick
                ? new Vector2(Input.GetAxis(Controller._gamepadAxes[0]), Input.GetAxis(Controller._gamepadAxes[1]))
                : new Vector2(Input.GetAxis(Controller._gamepadAxes[2]), Input.GetAxis(Controller._gamepadAxes[3]));

            if (Controller.projectSettings._Stick == ProjectSettings.Stick.MovementStick)
            {
                gamepadInput = Mathf.Abs(Input.GetAxis(Controller._gamepadAxes[0])) > 0.1f || Mathf.Abs(Input.GetAxis(Controller._gamepadAxes[1])) > 0.1f;
            }
            else
            {
                gamepadInput = Mathf.Abs(Input.GetAxis(Controller._gamepadAxes[2])) > 0.1f || Mathf.Abs(Input.GetAxis(Controller._gamepadAxes[3])) > 0.1f;
            }

            vector.y *= -1;
            
            vector.Normalize();

            if (Math.Abs(vector.x) < 0.4f & Math.Abs(vector.y - 1) < 0.4f)
            {
                if (slots[1].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(1);
                    DeselectAllSlots(1);
                    Helper.ChangeButtonColor(Controller.UIManager, 1, "high");
                }
            }
            else if (Math.Abs(vector.x - 0.707f) < 0.4f & Math.Abs(vector.y - 0.707f) < 0.4f)
            {
                if (slots[2].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(2);
                    DeselectAllSlots(2);
                    Helper.ChangeButtonColor(Controller.UIManager, 2, "high");
                }
            }
            else if (Math.Abs(vector.x - 1) < 0.4f & Math.Abs(vector.y) < 0.4f)
            {
                if (slots[3].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(3);
                    DeselectAllSlots(3);
                    Helper.ChangeButtonColor(Controller.UIManager, 3, "high");
                }
            }
            else if (Math.Abs(vector.x - 0.707f) < 0.4f & Math.Abs(vector.y + 0.707f) < 0.4f)
            {
                if (slots[4].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(4);
                    DeselectAllSlots(4);
                    Helper.ChangeButtonColor(Controller.UIManager, 4, "high");
                }
            }
            else if (Math.Abs(vector.x ) < 0.4f & Math.Abs(vector.y + 1) < 0.4f)
            {
                if (slots[5].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(5);
                    DeselectAllSlots(5);
                    Helper.ChangeButtonColor(Controller.UIManager, 5, "high");
                }
            }
            else if (Math.Abs(vector.x + 0.707f) < 0.4f & Math.Abs(vector.y + 0.707f) < 0.4f)
            {
                if (slots[6].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(6);
                    DeselectAllSlots(6);
                    Helper.ChangeButtonColor(Controller.UIManager, 6, "high");
                }
            }
            else if (Math.Abs(vector.x + 1) < 0.4f & Math.Abs(vector.y) < 0.4f)
            {
                if (slots[7].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(7);
                    DeselectAllSlots(7);
                    Helper.ChangeButtonColor(Controller.UIManager, 7, "high");
                }
            }
            else if (Math.Abs(vector.x + 0.707f) < 0.4f & Math.Abs(vector.y - 0.707f) < 0.4f)
            {
                if (slots[0].weaponSlotInGame.Count > 0)
                {
                    SelectWeaponInInventory(0);
                    DeselectAllSlots(0);
                    Helper.ChangeButtonColor(Controller.UIManager, 0, "high");
                }
            }
            
            var axis = Input.GetAxis(Controller._gamepadAxes[4]);

            if (Math.Abs(axis + 1) < 0.1f)
            {
                if (canChangeWeaponInSlot)
                {
                    DownInventoryValue("weapon");
                    canChangeWeaponInSlot = false;
                }
            }
            else if (Math.Abs(axis - 1) < 0.1f)
            {
                if (canChangeWeaponInSlot)
                {
                    UpInventoryValue("weapon");
                    canChangeWeaponInSlot = false;
                }
            }
            else if (Math.Abs(axis) < 0.1f)
            {
                if(!canChangeWeaponInSlot)
                    canChangeWeaponInSlot = true;
            }


            if (Input.GetKeyDown(Controller._gamepadCodes[12])|| 
                Helper.CheckGamepadAxisButton(12, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown",
                    Controller.projectSettings.AxisButtonValues[12]))
                UseKit("health");
            
            
            if (Input.GetKeyDown(Controller._gamepadCodes[13]) || 
                Helper.CheckGamepadAxisButton(13, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown",
                    Controller.projectSettings.AxisButtonValues[13]))
                if(slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 0)
                    UseKit("ammo");
        }

        void DeselectAllSlots(int curSlot)
        {
            for (var i = 0; i < 8; i++)
            {
                if (i != curSlot)
                    Helper.ChangeButtonColor(Controller.UIManager, i, "norm");
            }
            
            if(Controller.UIManager.CharacterUI.Inventory.HealthButton)
                Helper.ChangeColor(Controller.UIManager.CharacterUI.Inventory.HealthButton, Controller.UIManager.CharacterUI.Inventory.normButtonsColors[8], Controller.UIManager.CharacterUI.Inventory.normButtonsSprites[8]);
            
            if(Controller.UIManager.CharacterUI.Inventory.AmmoButton)
                Helper.ChangeColor(Controller.UIManager.CharacterUI.Inventory.AmmoButton, Controller.UIManager.CharacterUI.Inventory.normButtonsColors[9], Controller.UIManager.CharacterUI.Inventory.normButtonsSprites[9]);
        }

        void CheckInventoryButtons()
        {
            InventoryGamepadInputs();
            
            if (Controller.UIManager.CharacterUI.Inventory.UpHealthButton)
                Controller.UIManager.CharacterUI.Inventory.UpHealthButton.gameObject.SetActive(HealthKits.Count > 1 && !gamepadInput);

            if (Controller.UIManager.CharacterUI.Inventory.DownHealthButton)
                Controller.UIManager.CharacterUI.Inventory.DownHealthButton.gameObject.SetActive(HealthKits.Count > 1 && !gamepadInput);

            if (slots[currentSlot].weaponSlotInGame.Count > 0 && !gamepadInput)
            {
                if (Controller.UIManager.CharacterUI.Inventory.UpAmmoButton)
                    Controller.UIManager.CharacterUI.Inventory.UpAmmoButton.gameObject.SetActive(slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 1);
                
                if (Controller.UIManager.CharacterUI.Inventory.DownAmmoButton)
                    Controller.UIManager.CharacterUI.Inventory.DownAmmoButton.gameObject.SetActive(slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 1);

            }
            else
            {
                if (Controller.UIManager.CharacterUI.Inventory.UpAmmoButton)
                    Controller.UIManager.CharacterUI.Inventory.UpAmmoButton.gameObject.SetActive(false);
                
                if (Controller.UIManager.CharacterUI.Inventory.DownAmmoButton)
                    Controller.UIManager.CharacterUI.Inventory.DownAmmoButton.gameObject.SetActive(false);
            }
            
            if (Controller.UIManager.CharacterUI.Inventory.UpWeaponButton)
                Controller.UIManager.CharacterUI.Inventory.UpWeaponButton.gameObject.SetActive(slots[currentSlot].weaponSlotInGame.Count > 1);

            if (Controller.UIManager.CharacterUI.Inventory.DownWeaponButton)
                Controller.UIManager.CharacterUI.Inventory.DownWeaponButton.gameObject.SetActive(slots[currentSlot].weaponSlotInGame.Count > 1);

            if (Controller.UIManager.CharacterUI.Inventory.WeaponsCount)
            {
                Controller.UIManager.CharacterUI.Inventory.WeaponsCount.gameObject.SetActive(slots[currentSlot].weaponSlotInGame.Count > 1);

                Controller.UIManager.CharacterUI.Inventory.WeaponsCount.text = slots[currentSlot].currentWeaponInSlot + 1 + "/" + slots[currentSlot].weaponSlotInGame.Count;
            }
        }

        public void DropWeapon(int slot, bool getNewWeapon)
        {
            if (WeaponController.DropWeaponAudio)
                GetComponent<AudioSource>().PlayOneShot(WeaponController.DropWeaponAudio);

            Helper.ChangeLayersRecursively(WeaponController.transform, "Weapon");

            WeaponController = null;
            currentWeapon = null;

            var curIndex = slots[slot].currentWeaponInSlot;

            var curWeapon = slots[slot].weaponSlotInGame[curIndex];

            curWeapon.weapon.GetComponent<WeaponController>().enabled = false;

            if (!curWeapon.weapon.GetComponent<PickUp>())
            {
                var pickUpScript = curWeapon.weapon.AddComponent<PickUp>();
                pickUpScript.enabled = true;
                pickUpScript.PickUpType = PickUp.TypeOfPickUp.Weapon;
                pickUpScript.distance = 10;
                pickUpScript.Slots = slot;
                pickUpScript.Method = PickUp.PickUpMethod.Raycast;

                if (!Controller.isMultiplayerCharacter)
                {
                    if (pickUpScript.pickUpId == null)
                    {
                        pickUpScript.pickUpId = Helper.GenerateRandomString(20);
                        DropIdMultiplayer = pickUpScript.pickUpId;
                    }
                }
                else
                {
                    if (pickUpScript.pickUpId == null)
                    {
                        pickUpScript.pickUpId = DropIdMultiplayer;
                    }
                }
            }

            if (!Controller.isMultiplayerCharacter)
            {
                switch (Controller.TypeOfCamera)
                {
                    case CharacterHelper.CameraType.ThirdPerson:
                        DropDirection = Controller.DirectionObject.forward * 5;
                        break;
                    case CharacterHelper.CameraType.FirstPerson:
                        DropDirection = Controller.thisCamera.transform.forward * 5;
                        break;
                    case CharacterHelper.CameraType.TopDown:
                        DropDirection = Controller.DirectionObject.forward * 5;
                        break;
                }
            }

            curWeapon.weapon.GetComponent<BoxCollider>().isTrigger = false;

            curWeapon.weapon.transform.parent = null;

            curWeapon.weapon.SetActive(true);

            var rigidbody = curWeapon.weapon.GetComponent<Rigidbody>();

            rigidbody.velocity = 0.01f * Vector3.forward;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            foreach (var kit in slots[slot].weaponSlotInGame[curIndex].WeaponAmmoKits)
            {
                ReserveAmmo.Add(kit);
            }

            //var id = slots[currentSlot].weaponsInInventory[curIndex].weapon.GetInstanceID();
            slots[slot].weaponSlotInGame.Remove(curWeapon);

            if (getNewWeapon)
            {
                if (slots[slot].weaponSlotInGame.Count > 0)
                {
                    if (CharacterHelper.FindWeapon(slots, slot, slots[slot].currentWeaponInSlot, true) != -1)
                    {
                        slots[slot].currentWeaponInSlot = CharacterHelper.FindWeapon(slots, slot, slots[slot].currentWeaponInSlot, true);

                        if (slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].fistAttack) HandsOnly(false);
                        else Switch(slot, false, false);
                    }
                    else
                    {
                        FindWeapon(true, false);
                    }
                }
                else
                {
                    FindWeapon(true, false);
                }
            }
            else
            {
                HandsOnly(false);
            }

            // ChoiceNewWeapon(curIndex, "up", id, false);

            if (!Controller.isMultiplayerCharacter)
            {
#if PHOTON_UNITY_NETWORKING
                if (Controller.CharacterSync)
                    Controller.CharacterSync.DropWeapon(getNewWeapon);
#endif

                canDropWeapon = false;
                StartCoroutine(DropTimeOut(curWeapon));
            }

        }

        public void DropWeapon(bool getNewWeapon)
        {

            if ((!WeaponController || Controller.isPause || Controller.CameraController.cameraPause || Controller.AdjustmentScene || !canDropWeapon || !hasAnyWeapon || slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack || slots[currentSlot].weaponSlotInGame.Count <= 0 ||
                ((WeaponController.isAimEnabled || WeaponController.isReloadEnabled) && getNewWeapon) || !hasWeaponTaken || WeaponController.Attacks[WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade) && !Controller.isMultiplayerCharacter)
                return;

            if (WeaponController.DropWeaponAudio)
                GetComponent<AudioSource>().PlayOneShot(WeaponController.DropWeaponAudio);

            Helper.ChangeLayersRecursively(WeaponController.transform, "Weapon");

            WeaponController = null;
            currentWeapon = null;

            var curIndex = slots[currentSlot].currentWeaponInSlot;

            var curWeapon = slots[currentSlot].weaponSlotInGame[curIndex];

            curWeapon.weapon.GetComponent<WeaponController>().enabled = false;

            if (!curWeapon.weapon.GetComponent<PickUp>())
            {
                var pickUpScript = curWeapon.weapon.AddComponent<PickUp>();
                pickUpScript.enabled = true;
                pickUpScript.PickUpType = PickUp.TypeOfPickUp.Weapon;
                pickUpScript.distance = 10;
                pickUpScript.Slots = currentSlot + 1;
                pickUpScript.Method = PickUp.PickUpMethod.Raycast;

                if (!Controller.isMultiplayerCharacter)
                {
                    if (pickUpScript.pickUpId == null)
                    {
                        pickUpScript.pickUpId = Helper.GenerateRandomString(20);
                        DropIdMultiplayer = pickUpScript.pickUpId;
                    }
                }
                else
                {
                    if (pickUpScript.pickUpId == null)
                    {
                        pickUpScript.pickUpId = DropIdMultiplayer;
                    }
                }
            }

            if (!Controller.isMultiplayerCharacter)
            {
                switch (Controller.TypeOfCamera)
                {
                    case CharacterHelper.CameraType.ThirdPerson:
                        DropDirection = Controller.DirectionObject.forward * 5;
                        break;
                    case CharacterHelper.CameraType.FirstPerson:
                        DropDirection = Controller.thisCamera.transform.forward * 5 ;
                        break;
                    case CharacterHelper.CameraType.TopDown:
                        DropDirection = Controller.DirectionObject.forward * 5;
                        break;
                }
            }

            curWeapon.weapon.GetComponent<BoxCollider>().isTrigger = false;

            curWeapon.weapon.transform.parent = null;

            var rigidbody = curWeapon.weapon.GetComponent<Rigidbody>();

            rigidbody.velocity = DropDirection;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            foreach (var kit in slots[currentSlot].weaponSlotInGame[curIndex].WeaponAmmoKits)
            {
                ReserveAmmo.Add(kit);
            }

            //var id = slots[currentSlot].weaponsInInventory[curIndex].weapon.GetInstanceID();
            slots[currentSlot].weaponSlotInGame.Remove(curWeapon);

            if (getNewWeapon)
            {
                if (slots[currentSlot].weaponSlotInGame.Count > 0)
                {
                    if (CharacterHelper.FindWeapon(slots, currentSlot, slots[currentSlot].currentWeaponInSlot, true) != -1)
                    {
                        slots[currentSlot].currentWeaponInSlot = CharacterHelper.FindWeapon(slots, currentSlot, slots[currentSlot].currentWeaponInSlot, true);

                        if (slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack) HandsOnly(false);
                        else Switch(currentSlot, false, false);
                    }
                    else
                    {
                        FindWeapon(true, false);
                    }
                }
                else
                {
                    FindWeapon(true, false);
                }
            }
            else
            {
                HandsOnly(false);
            }

            // ChoiceNewWeapon(curIndex, "up", id, false);

            if (!Controller.isMultiplayerCharacter)
            {
#if PHOTON_UNITY_NETWORKING
                if (Controller.CharacterSync)
                    Controller.CharacterSync.DropWeapon(getNewWeapon);
#endif
                
                canDropWeapon = false;
                StartCoroutine(DropTimeOut(curWeapon));
            }
        }

        void ActivateInventory()
        {
            if (WeaponController)
                if (WeaponController.isAimEnabled && WeaponController.useAimTexture ||
                    WeaponController.isReloadEnabled /*|| creategrenade*/ || !hasWeaponTaken)
                    return;

            if (Controller.isPause || inventoryIsOpened || Controller.AdjustmentScene)
                return;

            previousSlot = currentSlot;
            previousWeaponInSlot = slots[currentSlot].currentWeaponInSlot;
            
            inventoryIsOpened = true;

            UIHelper.ManageUIButtons(Controller, this, Controller.UIManager, Controller.CharacterSync);

            CheckInventoryButtons();

            DeselectAllSlots(currentSlot);
            
            if (slots[currentSlot].weaponSlotInGame.Count > 0 && !slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
            {
                var _weaponController = slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();

                if (_weaponController.Attacks[_weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade
                    && _weaponController.Attacks[_weaponController.currentAttack].curAmmo > 0
                    || _weaponController.Attacks[_weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                    Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "high");
            }
            else
            {
                Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "high");
            }

            Controller.UIManager.CharacterUI.ShowImage("weapon", this);
            Controller.UIManager.CharacterUI.ShowImage("health", this);
            Controller.UIManager.CharacterUI.ShowImage("ammo", this);

            Controller.UIManager.CharacterUI.Inventory.MainObject.SetActive(true);

            Controller.hasMoveButtonPressed = false;

            Controller.CameraController.cameraPause = true;

            closeInventory = false;
        }

        void DeactivateInventory()
        {
            if (!Controller.UIManager.CharacterUI.Inventory.MainObject)
                return;

            Controller.UIManager.CharacterUI.Inventory.MainObject.SetActive(false);
            inventoryIsOpened = false;
            
            Controller.CameraController.cameraPause = false;

            if (slots[currentSlot].weaponSlotInGame.Count > 0 && !slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
            {
                var _controller = slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();

                if (_controller.Attacks[_controller.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade
                    && _controller.Attacks[_controller.currentAttack].curAmmo > 0
                    || _controller.Attacks[_controller.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                {
                    Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "high");
                }
                else
                {
                    Helper.ChangeButtonColor(Controller.UIManager, previousSlot, "high");
                    Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "norm");

                    currentSlot = previousSlot;
                    slots[currentSlot].currentWeaponInSlot = previousWeaponInSlot;
                }
            }
            else
            {
                Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "high");
            }

            if(Controller.UIManager.CharacterUI.Inventory.AmmoButton)
                Helper.ChangeColor(Controller.UIManager.CharacterUI.Inventory.AmmoButton, Controller.UIManager.CharacterUI.Inventory.normButtonsColors[9], Controller.UIManager.CharacterUI.Inventory.normButtonsSprites[9]);
            
            if(Controller.UIManager.CharacterUI.Inventory.HealthButton)
                Helper.ChangeColor(Controller.UIManager.CharacterUI.Inventory.HealthButton, Controller.UIManager.CharacterUI.Inventory.normButtonsColors[8], Controller.UIManager.CharacterUI.Inventory.normButtonsSprites[8]);

            UIHelper.ManageUIButtons(Controller, this, Controller.UIManager, Controller.CharacterSync);

            if (hasWeaponChanged || hideAllWeapons)
            {
                if (WeaponController)
                {
                    if (WeaponController.isAimEnabled)
                    {
                        WeaponController.Aim(true, false, false);
                        StartCoroutine("SwitchWeaponTimeOut");
                    }
                    else
                    {
                        SwitchNewWeapon(true);
                    }
                }
                else
                {
                    SwitchNewWeapon(true);
                }
            }
        }

        public void SwitchNewWeapon(bool sendToNetwork)
        {
            if (hasWeaponChanged)
            {
                Switch(currentSlot, false, sendToNetwork);
                hasWeaponChanged = false;
            }
            else if(hideAllWeapons)
            {
                //return;
                //Debug.Log(1);
                //HandsOnly(sendToNetwork);
                if (currentSlot < 0 || currentSlot > 7)
                {
                    Switch(0, false, sendToNetwork);
                    hasWeaponChanged = false;
                    return;
                }

                //Debug.Log(currentSlot);
                Switch(currentSlot, false, sendToNetwork);
                
                hasWeaponChanged = false;
            }
        }

        void HandsOnly(bool sendToNetwork)
        {
#if PHOTON_UNITY_NETWORKING
            if (!Controller.isMultiplayerCharacter && Controller.CharacterSync && sendToNetwork)
                Controller.CharacterSync.ChangeWeapon(false);
#endif

//            if (Controller.UIManager.CharacterUI.WeaponAmmo)
//                Controller.UIManager.CharacterUI.WeaponAmmo.gameObject.SetActive(false);
                    
            NullWeapons();

            
            Controller.anim.SetBool("NoWeapons", true);
                
            StopCoroutine("TakeWeapon");
            
            Controller.speedDevider = 1;
                
            Controller.anim.Play("Take Weapon", 1);
            Controller.anim.Play("Take Weapon", 2);
            
            SmoothIKSwitch = 0;
                
            SetHandsAnimations();
            
            UIHelper.ManageUIButtons(Controller, this, Controller.UIManager, Controller.CharacterSync);

            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                StartCoroutine(ChangeAnimatorLayers(0));
                
            hideAllWeapons = false;
            hasAnyWeapon = false;

        }

        void SetHandsAnimations()
        {
            if (HandsIdle)
                Controller.ClipOverrides["_WeaponIdle"] = HandsIdle;
            else Debug.LogWarning("<color=yellow>Missing Component</color> [Hands Idle] animation.");
            
            if (HandsWalk)
                Controller.ClipOverrides["_WeaponWalk"] = HandsWalk;
            else Debug.LogWarning("<color=yellow>Missing Component</color> [Hands Walk] animation.");

            if (HandsRun)
                Controller.ClipOverrides["_WeaponRun"] = HandsRun;
            else Debug.LogWarning("<color=yellow>Missing Component</color> [Hands Run] animation.");
            
            Controller.newController.ApplyOverrides(Controller.ClipOverrides);
        }
        
        IEnumerator ChangeAnimatorLayers(int value)
        {
            while (true)
            {
                Controller.anim.SetLayerWeight(2, Mathf.Lerp(Controller.anim.GetLayerWeight(2), value, 10 * Time.deltaTime));
                
                    if (Math.Abs(Controller.anim.GetLayerWeight(2) - value) < 0.1f)
                    {
                        Controller.anim.SetLayerWeight(2, value);
                        StopCoroutine("ChangeAnimatorLayers");
                        break;
                    }

                    yield return 0;
            }
        }

        IEnumerator SwitchWeaponTimeOut()
        {
            while (true)
            {
                if (WeaponController.setHandsPositionsAim)
                {
                    yield return new WaitForSeconds(0.1f);

                    SwitchNewWeapon(true);
                    StopCoroutine("SwitchWeaponTimeOut");
                    break;
                }
                
                yield return 0;
            }
        }


        public void SelectWeaponInInventory(int slot)
        {
            if (slots[slot].weaponSlotInGame.Count <= 0 && !slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].fistAttack)
                return;

            if (!slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].fistAttack)
            {
                weaponId = slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].weapon.GetInstanceID();

                var _controller = slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();
                
                if (_controller.Attacks[_controller.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade && _controller.Attacks[_controller.currentAttack].curAmmo > 0
                    || _controller.Attacks[_controller.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                {
                    hideAllWeapons = false;

                    if (currentWeapon)
                    {
                        if (hasAnyWeapon)
                        {
                            hasWeaponChanged = currentWeapon.GetInstanceID() != weaponId && slots[slot].weaponSlotInGame.Count > 0 && slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].weapon;
                        }
                        else hasWeaponChanged = true;
                    }
                    else
                    {
                        hasWeaponChanged = true;
                    }
                }
                else
                {
                    hasWeaponChanged = false;
                }
            }
            else
            {
                hideAllWeapons = true;
                hasWeaponChanged = false;
                //hideAllWeapons = false;
                //hasWeaponChanged = false;
            }

            if (!gamepadInput)
            {
                if (currentSlot != slot)
                {
                    Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "norm");
                }
            }
            
            currentSlot = slot;

            Helper.ChangeButtonColor(Controller.UIManager, currentSlot, "high");
        }

        public void UpInventoryValue(string type)
        {
            switch (type)
            {
                case "weapon":
                {
                    var curWeapon = slots[currentSlot].currentWeaponInSlot;
                    curWeapon++;

                    if (curWeapon > slots[currentSlot].weaponSlotInGame.Count - 1)
                        curWeapon = 0;

                    slots[currentSlot].currentWeaponInSlot = curWeapon;

                    Controller.UIManager.CharacterUI.ShowImage("weapon", this);
                    SelectWeaponInInventory(currentSlot);
                    break;
                }
                case "health":

                    var curKit = currentHealthKit;
                    curKit++;
                    if (curKit > HealthKits.Count - 1)
                        curKit = 0;
                    currentHealthKit = curKit;

                    Controller.UIManager.CharacterUI.ShowImage("health", this);
                    break;

                case "ammo":

                    curKit = currentAmmoKit;
                    curKit++;
                    if (curKit > slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count - 1)
                        curKit = 0;
                    currentAmmoKit = curKit;

                    Controller.UIManager.CharacterUI.ShowImage("ammo", this);
                    break;
            }
        }

        public void DownInventoryValue(string type)
        {
            switch (type)
            {
                case "weapon":
                {
                    var curWeapon = slots[currentSlot].currentWeaponInSlot;
                    curWeapon--;

                    if (curWeapon < 0)
                        curWeapon = slots[currentSlot].weaponSlotInGame.Count - 1;

                    slots[currentSlot].currentWeaponInSlot = curWeapon;

                    Controller.UIManager.CharacterUI.ShowImage("weapon", this);
                    SelectWeaponInInventory(currentSlot);
                    break;
                }
                case "health":

                    var curKit = currentHealthKit;
                    curKit--;
                    if (curKit < 0)
                        curKit = HealthKits.Count - 1;
                    currentHealthKit = curKit;

                    Controller.UIManager.CharacterUI.ShowImage("health", this);
                    break;
                case "ammo":
                    curKit = currentAmmoKit;
                    curKit--;
                    if (curKit < 0)
                        curKit = slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot]
                                     .WeaponAmmoKits.Count - 1;
                    currentAmmoKit = curKit;

                    Controller.UIManager.CharacterUI.ShowImage("ammo", this);
                    break;
            }
        }

        public void UseKit(string type)
        {
            switch (type)
            {
                case "health":
                    if (HealthKits.Count <= 0)
                        return;

                    Controller.PlayerHealth += HealthKits[currentHealthKit].AddedValue;
                    HealthKits.Remove(HealthKits[currentHealthKit]);
                    var curIndex = currentHealthKit;
                    curIndex++;
                    if (curIndex > HealthKits.Count - 1)
                        curIndex = 0;
                    currentHealthKit = curIndex;
                    Controller.UIManager.CharacterUI.ShowImage("health", this);
                    
#if PHOTON_UNITY_NETWORKING
                    if(!Controller.isMultiplayerCharacter && Controller.CharacterSync)
                        Controller.CharacterSync.UseHealthKit();
#endif
                    
                    break;
                case "ammo":
                    
                    if (slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count <= 0)
                        return;

                    var ammoKit = slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits[currentAmmoKit];
                    var weaponController = slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();

                    foreach (var attack in weaponController.Attacks)
                    {
                        if (attack.AmmoType == ammoKit.ammoType || ammoKit.ammoType == "")
                        {
                            weaponController.Attacks[weaponController.currentAttack].inventoryAmmo += ammoKit.AddedValue;

                            if (attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                            {
                                weaponController.Attacks[weaponController.currentAttack].curAmmo = weaponController.Attacks[weaponController.currentAttack].inventoryAmmo;
                            }

                            break;
                        }
                    }
                    
                    for (var i = 0; i < 8; i++)
                    {
                        foreach (var weapon in slots[i].weaponSlotInGame)
                        {
                            if (weapon.WeaponAmmoKits.Exists(x => x.PickUpId == ammoKit.PickUpId))
                            {
                                var kit = weapon.WeaponAmmoKits.Find(x => x.PickUpId == ammoKit.PickUpId);
                                weapon.WeaponAmmoKits.Remove(kit);
                            }
                        }
                    }

                    curIndex = currentAmmoKit;
                    curIndex++;
                    if (curIndex > slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count - 1)
                        curIndex = 0;
                    currentAmmoKit = curIndex;

                    Controller.UIManager.CharacterUI.ShowImage("ammo", this);
                    Controller.UIManager.CharacterUI.ShowImage("weapon", this);
                    
                    break;
            }
        }

        void CheckPickUp()
        {
            if (Controller.isPause || Controller.CameraController.cameraPause || Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown)
                return;

            if (WeaponController)
                if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && WeaponController.isAimEnabled || WeaponController.isReloadEnabled || /*creategrenade ||*/
                    !hasWeaponTaken)
                {
                    isPickUp = false;
                    return;
                }

            var Hit = new RaycastHit();

            if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
            {
                var direction = Controller.thisCamera.transform.TransformDirection(Vector3.forward);
                if (!Physics.Raycast(Controller.thisCamera.transform.position, direction, out Hit, 100, Helper.layerMask())) return;

            }
            else
            {
                if (!Physics.Raycast(Controller.BodyObjects.Head.position + transform.forward * 2, Vector3.down * 3, out Hit, 100, Helper.layerMask())) return;
            }

            {
                if (Hit.collider.GetComponent<PickUp>())
                {
                    if (!Hit.collider.GetComponent<PickUp>().isActiveAndEnabled) return;

                    var pickUp = Hit.collider.GetComponent<PickUp>();

                    if (pickUp.Method == PickUp.PickUpMethod.Raycast)
                    {
                        if (Hit.distance <= pickUp.distance)
                        {
                            isPickUp = true;

                            if (Input.GetKeyDown(Controller._gamepadCodes[8]) || Input.GetKeyDown(Controller._keyboardCodes[8]) || pickUpUiButton ||
                                Helper.CheckGamepadAxisButton(8, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[8]))
                            {
                                pickUp.PickUpObject(gameObject);
                                currentPickUpId = pickUp.pickUpId;

#if PHOTON_UNITY_NETWORKING
                                if (Controller.CharacterSync)
                                    Controller.CharacterSync.PickUp();
#endif

                                pickUpUiButton = false;
                            }
                        }
                        else
                        {
                            isPickUp = false;
                        }
                    }
                }
                else
                {
                    isPickUp = false;
                }
            }

        }
        /*
        void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PickUp>())
            {
                var pickUp = other.GetComponent<PickUp>();
                if (pickUp.Method == PickUp.PickUpMethod.Collider && pickUp.enabled)
                {
                    pickUp.PickUpObject(gameObject);
                    currentPickUpId = pickUp.pickUpId;
                    
#if PHOTON_UNITY_NETWORKING
                    if(Controller.CharacterSync)
                        Controller.CharacterSync.PickUp(); 
#endif
                }
            }
        }
        */

        void OnTriggerEnter(Collider other)
        {

#if PHOTON_UNITY_NETWORKING
            if (Controller.GetComponent<PhotonView>())
            {
                if (!Controller.GetComponent<PhotonView>().IsMine)
                    return;
            }
#endif

            if (other.GetComponent<PickUp>())
            {
                
                var pickUp = other.GetComponent<PickUp>();
                if (pickUp.Method == PickUp.PickUpMethod.Collider && pickUp.enabled)
                {
                    //other.GetComponent<WeaponController>().enabled = true;
                    Controller.UIManager.CharacterUI.PickupHUD.gameObject.SetActive(true);
                    //Debug.Log("testing here 2");
                    //other.GetComponent<WeaponController>().enabled = false;
                }
            }

            if (other.GetComponent<PickedUpEgg>())
            {
                if (gameObject.GetComponent<EggBag>().bag.Count < gameObject.GetComponent<EggBag>().capacity)
                {
                    Controller.UIManager.CharacterUI.PickupEggHUD.batteryFullPrompt.gameObject.SetActive(false);
                    Controller.UIManager.CharacterUI.PickupEggHUD.pickUpEggPrompt.gameObject.SetActive(true);
                }
                else
                {
                    Controller.UIManager.CharacterUI.PickupEggHUD.batteryFullPrompt.gameObject.SetActive(true);
                    Controller.UIManager.CharacterUI.PickupEggHUD.pickUpEggPrompt.gameObject.SetActive(false);
                }
                Controller.UIManager.CharacterUI.PickupEggHUD.gameObject.SetActive(true);
            }


            //+++++++
            /*
            if (other.GetComponent<PickedUpEgg>())
            {
                Debug.Log("enter the egg area");
                Controller.UIManager.CharacterUI.PickupEggHUD.gameObject.SetActive(true);
            }
            */
            /*
            if (other.GetComponent<Vault>())
            {
                Debug.Log("enter the vault");
                if (gameObject.GetComponent<EggBag>().bag.Count > 0)
                {
                    Controller.UIManager.CharacterUI.VaultHUD.gameObject.SetActive(true);
                }
                else
                {
                    Controller.UIManager.CharacterUI.VaultHUD.gameObject.SetActive(false);
                }
            }
            */


        }
        private void OnTriggerExit(Collider other)
        {

#if PHOTON_UNITY_NETWORKING
            if (Controller.GetComponent<PhotonView>())
            {
                if (!Controller.GetComponent<PhotonView>().IsMine)
                    return;
            }
#endif

            if (other.GetComponent<PickUp>())
            {
                Controller.UIManager.CharacterUI.PickupHUD.gameObject.SetActive(false);
            }
            
            if (other.GetComponent<PickedUpEgg>() || other.GetComponent<Vault>())
            {
                Controller.UIManager.CharacterUI.PickupEggHUD.gameObject.SetActive(false);
            }

        }

        private bool pickupCooldown = true;
        private bool pickupUpdate = true;
        private bool pickupEggCooldown = true;
        private bool pickupEggUpdate = true;
        private bool vaultEggCooldown = true;
        private bool vaultEggUpdate = true;


        //
        //
        //
        //
        //
        void OnTriggerStay(Collider other)
        {

#if PHOTON_UNITY_NETWORKING
            if (Controller.GetComponent<PhotonView>())
            {
                if (!Controller.GetComponent<PhotonView>().IsMine)
                    return;
            }
#endif

            if (other.GetComponent<PickUp>())
            {
                if (!other.GetComponent<PickUp>().enabled) return;
                var pickUp = other.GetComponent<PickUp>();
                if (!Controller.UIManager.CharacterUI.PickupHUD.gameObject.activeSelf)
                    Controller.UIManager.CharacterUI.PickupHUD.gameObject.SetActive(true);
                if (pickUp.Method == PickUp.PickUpMethod.Collider && pickUp.enabled)
                {
                    if (pickupUpdate)
                    {
                        pickupUpdate = false;
                        StartCoroutine(CoolDownPickUpUpdate());
                        if (pickUp.Slots == 1)
                        {
                            if (slots[0].weaponSlotInGame.Count >= 2)
                            {
                                if (currentSlot != 0)
                                {
                                    //Switch(0, false, false);
                                    Controller.UIManager.CharacterUI.PickupHUD.UpdateHUD(other.GetComponent<WeaponController>().WeaponImage, other.name,
                                        slots[0].weaponSlotInGame[slots[0].currentWeaponInSlot].weapon.name);
                                }
                                else
                                {
                                    Controller.UIManager.CharacterUI.PickupHUD.UpdateHUD(other.GetComponent<WeaponController>().WeaponImage, other.name,
                                        currentWeapon.name);
                                }
                            }
                            else
                            {
                                Controller.UIManager.CharacterUI.PickupHUD.UpdateHUD(other.GetComponent<WeaponController>().WeaponImage, other.name);
                            }
                        }
                        else if (pickUp.Slots == 2)
                        {
                            if (slots[1].weaponSlotInGame.Count >= 2)
                            {
                                if (currentSlot != 1)
                                {
                                    //Switch(0, false, false);
                                    Controller.UIManager.CharacterUI.PickupHUD.UpdateHUD(other.GetComponent<WeaponController>().WeaponImage, other.name,
                                        slots[1].weaponSlotInGame[slots[1].currentWeaponInSlot].weapon.name);
                                }
                                else
                                {
                                    Controller.UIManager.CharacterUI.PickupHUD.UpdateHUD(other.GetComponent<WeaponController>().WeaponImage, other.name,
                                        currentWeapon.name);
                                }
                            }
                            else
                            {
                                Controller.UIManager.CharacterUI.PickupHUD.UpdateHUD(other.GetComponent<WeaponController>().WeaponImage, other.name);
                            }
                        }
                    }
                    //Debug.Log(other);
                    if (Input.GetKeyDown(Controller._gamepadCodes[8]) || Input.GetKey(Controller._keyboardCodes[8]) || pickUpUiButton ||
                        Helper.CheckGamepadAxisButton(8, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[8]))
                    {

                        if (!pickupCooldown)
                        {
                            return;
                        }
                        pickupCooldown = false;
                        StartCoroutine(CoolDownPickUp());


                        if (pickUp.Slots == 1)
                        {
                            if (slots[0].weaponSlotInGame.Count >= 2)
                            {
                                DropWeapon(0, false);
                            }
                        }
                        else if (pickUp.Slots == 2)
                        {
                            if (slots[1].weaponSlotInGame.Count >= 2)
                            {
                                DropWeapon(1, false);
                            }
                        }
                        pickUp.PickUpObject(gameObject);
                        currentPickUpId = pickUp.pickUpId;
                        Controller.UIManager.CharacterUI.PickupHUD.gameObject.SetActive(false);

//????????????????????
#if PHOTON_UNITY_NETWORKING
                        if (Controller.CharacterSync)
                            Controller.CharacterSync.PickUp();
#endif
//????????????????????
                    }

                }
            }

            if (other.GetComponent<PickedUpEgg>())
            {
                if (!other.GetComponent<PickedUpEgg>().enabled) return;
                var pickUpEgg = other.GetComponent<PickedUpEgg>();
                if (pickUpEgg.enabled)
                {
                    //the egg bag is not full
                    if (gameObject.GetComponent<EggBag>().capacity > gameObject.GetComponent<EggBag>().bag.Count)
                    {
                        if (pickupEggUpdate)
                        {
                            Controller.UIManager.CharacterUI.PickupEggHUD.UpdateHUD();
                            pickupEggUpdate = false;
                            StartCoroutine(CoolDownEggPickUpUpdate());
                        }

                        //if (Input.GetKeyDown(Controller._gamepadCodes[8]) || Input.GetKeyDown(Controller._keyboardCodes[8]) || pickUpUiButton ||
                            //Helper.CheckGamepadAxisButton(8, Controller._gamepadButtonsAxes, Controller.hasAxisButtonPressed, "GetKeyDown", Controller.projectSettings.AxisButtonValues[8]))
                        if(Input.GetKey(KeyCode.E))
                        {
                            //Debug.Log("KEY.E");
                            if (!pickupEggCooldown)
                            {
                                return;
                            }
                            pickupEggCooldown = false;
                            //Debug.Log("PICKED");
                            StartCoroutine(CoolDownEggPickUp());
                            gameObject.GetComponent<EggBag>().GetEgg(other.gameObject);
                            Controller.UIManager.CharacterUI.PickupEggHUD.gameObject.SetActive(false);
                        }
                    }
                    //the egg bag is full
                    else
                    {
                        if (pickupEggUpdate)
                        {
                            Controller.UIManager.CharacterUI.PickupEggHUD.UPdateHUDFull();
                            pickupEggUpdate = false;
                            StartCoroutine(CoolDownEggPickUpUpdate());
                        }
                    }
                }
            }

            if (other.GetComponent<Vault>())
            {
                if (GetComponent<EggBag>().bag.Count > 0)
                {
                    Controller.UIManager.CharacterUI.PickupEggHUD.UpdateHUD(1); //the parameter in UpdateHUD means dropping the eggs
                }
                
            }

        }

        private IEnumerator CoolDownPickUpUpdate()
        {
            yield return new WaitForSeconds(0.1f);
            pickupUpdate = true;
        }

        private IEnumerator CoolDownPickUp()
        {
            yield return new WaitForSeconds(1f);
            pickupCooldown = true;
        }

        private IEnumerator CoolDownEggPickUpUpdate()
        {
            yield return new WaitForSeconds(0.1f);
            pickupEggUpdate = true;
        }

        private IEnumerator CoolDownEggPickUp()
        {
            yield return new WaitForSeconds(0.5f);
            pickupEggCooldown = true;
        }

        
        private IEnumerator CoolDownVaultUpdate()
        {
            yield return new WaitForSeconds(0.1f);
            vaultEggUpdate = true;
        }

        private IEnumerator CoolDownVaultPickUp()
        {
            yield return new WaitForSeconds(1f);
            vaultEggCooldown = true;
        }
        


        public void NullWeapons()
        {
            for (var i = 0; i < 8; i++)
            {
                foreach (var weapon in slots[i].weaponSlotInGame)
                {
                    if(weapon.weapon)
                        weapon.weapon.SetActive(false);
                }
            }
        }

        public void Switch(int slot, bool isGrenade, bool sendToNetwork)
        {

            StopCoroutine("TakeWeapon");

            NullWeapons();
            slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].weapon.SetActive(true);
            WeaponController = slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();
            WeaponController.canAttack = false;
            WeaponController.enabled = true;
            
            WeaponController.Controller = Controller;
            WeaponController.CurrentWeaponInfo.Clear();

            foreach (var weaponInfo in WeaponController.WeaponInfos)
            {
                var info = new WeaponsHelper.WeaponInfo();
                info.Clone(weaponInfo);
                WeaponController.CurrentWeaponInfo.Add(info);
            }

            if (!isGrenade)
            {
                ResetAnimatorParameters();
                SetWeaponAnimations(false);

                if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson && !Controller.isMultiplayerCharacter
                                                                                      && (!Controller.isCrouch && !WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState ||
                                                                                          Controller.isCrouch && !WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState))
                    StartCoroutine(ChangeAnimatorLayers(1));

                currentWeapon = slots[slot].weaponSlotInGame[slots[slot].currentWeaponInSlot].weapon;

                Helper.ChangeLayersRecursively(currentWeapon.transform, "Character");

                //here new stuff
                Controller.anim.SetBool("HasWeaponTaken", false);
                SmoothIKSwitch = 0;

                if (WeaponController.useScope && WeaponController.ScopeScreen)
                    WeaponController.ScopeScreen.GetComponent<MeshRenderer>().material.mainTexture = ScopeScreenTexture;

                if (WeaponController.useAimTexture && Controller.UIManager.CharacterUI.aimPlaceholder)
                    Controller.UIManager.CharacterUI.aimPlaceholder.texture = WeaponController.AimCrosshairTexture;
            }

            UIHelper.ManageUIButtons(Controller, this, Controller.UIManager, Controller.CharacterSync);
            SetCrosshair();

            firstLayerSet = false;
            hasWeaponTaken = false;
            
            if (!Controller.AdjustmentScene)
            {
                Controller.anim.Play("Take Weapon", 1);
                Controller.anim.Play("Take Weapon", 2);
                StartCoroutine("TakeWeapon");
            }
            else
            {
                Controller.anim.Play("Idle", 1);
                Controller.anim.Play("Idle", 2);
            }

            Controller.anim.SetBool("CanWalkWithWeapon", true);
            
            switch (WeaponController.Weight)
            {
                case WeaponsHelper.WeaponWeight.Light:
                    Controller.speedDevider = 1;
                    break;
                case WeaponsHelper.WeaponWeight.Medium:
                    Controller.speedDevider = 1.1f;
                    break;
              case WeaponsHelper.WeaponWeight.Heavy:
                  Controller.speedDevider = 1.2f;
                  break;
            }
            
            hasAnyWeapon = true;
            
            if (Controller.isMultiplayerCharacter)
                return;
            
#if PHOTON_UNITY_NETWORKING
            if(Controller.CharacterSync && sendToNetwork)
                Controller.CharacterSync.ChangeWeapon(true);
#endif
            if (!isGrenade)
            {
                if (Controller.CameraController.CameraAim)
                    Controller.CameraController.Aim();
            }
        }

        public void ResetAnimatorParameters()
        {
            foreach (var parameter in Controller.anim.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    if (parameter.name == "Aim" || parameter.name == "TakeWeapon" || parameter.name == "Attack" || parameter.name == "Reload"
                        || parameter.name == "Pause" || parameter.name == "HasWeaponTaken" || parameter.name == "CanWalkWithWeapon" || parameter.name == "NoWeapons")
                    {
                        Controller.anim.SetBool(parameter.name, false); //parameter.name == "NoWeapons");
                    }
                }

            }
        }

        public void SetCrosshair()
        {
            if(Controller.AdjustmentScene || !WeaponController
#if PHOTON_UNITY_NETWORKING
              || Controller.GetComponent<PhotonView>() && !Controller.GetComponent<PhotonView>().IsMine
#endif
              )
                return;
            
            var characterUI = Controller.UIManager.CharacterUI;

            if (characterUI.topCrosshairPart)
            {
                characterUI.topCrosshairPart.rectTransform.sizeDelta = new Vector2(WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize, WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize);
                characterUI.topCrosshairPart.sprite = WeaponController.Attacks[WeaponController.currentAttack].UpPart ? WeaponController.Attacks[WeaponController.currentAttack].UpPart : null;
            }

            if (characterUI.bottomCrosshairPart)
            {
                characterUI.bottomCrosshairPart.rectTransform.sizeDelta = new Vector2(WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize, WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize);
                characterUI.bottomCrosshairPart.sprite = WeaponController.Attacks[WeaponController.currentAttack].DownPart ? WeaponController.Attacks[WeaponController.currentAttack].DownPart : null;
            }

            if (characterUI.leftCrosshairPart)
            {
                characterUI.leftCrosshairPart.rectTransform.sizeDelta = new Vector2(WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize, WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize);
                characterUI.leftCrosshairPart.sprite = WeaponController.Attacks[WeaponController.currentAttack].LeftPart ? WeaponController.Attacks[WeaponController.currentAttack].LeftPart : null;
            }

            if (characterUI.rightCrosshairPart)
            {
                characterUI.rightCrosshairPart.rectTransform.sizeDelta = new Vector2(WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize, WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize);
                characterUI.rightCrosshairPart.sprite = WeaponController.Attacks[WeaponController.currentAttack].RightPart ? WeaponController.Attacks[WeaponController.currentAttack].RightPart : null;
            }

            if (characterUI.middleCrosshairPart)
            {
                characterUI.middleCrosshairPart.rectTransform.sizeDelta = new Vector2(WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize, WeaponController.Attacks[WeaponController.currentAttack].CrosshairSize);

                if (characterUI.middleCrosshairPart.gameObject.GetComponent<Outline>())
                    characterUI.middleCrosshairPart.gameObject.GetComponent<Outline>().enabled = true;
                
                if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.tdModeLikeTp)
                {
                    if (Controller.CameraParameters.CursorImage)
                    {
                        characterUI.middleCrosshairPart.rectTransform.sizeDelta = new Vector2(70, 70);
                        characterUI.middleCrosshairPart.sprite = Controller.CameraParameters.CursorImage;
                    }
                }
                else
                {
                    characterUI.middleCrosshairPart.sprite = WeaponController.Attacks[WeaponController.currentAttack].MiddlePart ? WeaponController.Attacks[WeaponController.currentAttack].MiddlePart : null;
                }
            }

//            Controller.CameraController.rightPart.gameObject.SetActive(false);
//            Controller.CameraController.rightPart.gameObject.SetActive(true);
        }

        public void SetWeaponAnimations(bool changeAttack)
        {
            if (WeaponController.Attacks[WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets)
            {
                if (WeaponController.Attacks[WeaponController.currentAttack].currentBulletType == 0)
                {
                    if (WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacks[0])
                        Controller.ClipOverrides["_WeaponAttack"] = WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacks[0];
                    else
                        Debug.LogWarning("<color=yellow>Missing Component</color> [Single Shoot] animation.", WeaponController.gameObject);
                }
                else
                {
                    if (WeaponController.Attacks[WeaponController.currentAttack].WeaponAutoShoot)
                        Controller.ClipOverrides["_WeaponAttack"] = WeaponController.Attacks[WeaponController.currentAttack].WeaponAutoShoot;
                    else
                        Debug.LogWarning("<color=yellow>Missing Component</color> [Auto Shoot] animation.", WeaponController.gameObject);
                }
            }
            else
            {
                if (WeaponController.Attacks[WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade)
                {
                    if (WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacksFullBody[0])
                        Controller.ClipOverrides["_GrenadeFullBody"] = WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacksFullBody[0];
                    
                    if (WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacksFullBodyCrouch[0])
                        Controller.ClipOverrides["_GrenadeCrouchFullBody"] = WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacksFullBodyCrouch[0];
                }
                
                if (WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacks[0])
                    Controller.ClipOverrides["_WeaponAttack"] = WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacks[0];
//                else Debug.LogWarning("<color=yellow>Missing Component</color> [WeaponAttack] animation.", weaponController.gameObject);
            }

            if (WeaponController.characterAnimations.WeaponIdle)
                Controller.ClipOverrides["_WeaponIdle"] = WeaponController.characterAnimations.WeaponIdle;
            else
                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon Idle] animation.",
                    WeaponController.gameObject);

            if (WeaponController.Attacks[WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee && 
                WeaponController.Attacks[WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
            {
                if (WeaponController.Attacks[WeaponController.currentAttack].WeaponReload)
                    Controller.ClipOverrides["_WeaponReload"] = WeaponController.Attacks[WeaponController.currentAttack].WeaponReload;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon Reload] animation.",
                        WeaponController.gameObject);
            }

            if (!changeAttack)
            {
                if (WeaponController.characterAnimations.WeaponWalk)
                    Controller.ClipOverrides["_WeaponWalk"] = WeaponController.characterAnimations.WeaponWalk;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon walk] animation.",
                        WeaponController.gameObject);

                if (WeaponController.characterAnimations.WeaponRun)
                    Controller.ClipOverrides["_WeaponRun"] = WeaponController.characterAnimations.WeaponRun;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon run] animation.",
                        WeaponController.gameObject);

                if (WeaponController.characterAnimations.TakeWeapon)
                    Controller.ClipOverrides["_TakeWeapon"] = WeaponController.characterAnimations.TakeWeapon;
                else
                    Debug.LogWarning("<color=yellow>Missing Component</color> [Take weapon] animation.",
                        WeaponController.gameObject);
            }

            Controller.newController.ApplyOverrides(Controller.ClipOverrides);

            StartCoroutine("SetAnimParameters");
        }

        public void FindWeapons()
        {
            hasAnyWeapon = false;

            if (FindObjectOfType<Adjustment>())
            {
                Controller.AdjustmentScene = true;
                return;
            }
            
//          var allWeapons = new List<GameObject>();

            if (!gameObject.GetComponent<CharacterSync>())
            {
                for (var i = 0; i < 8; i++)
                {
                    foreach (var slot in slots[i].weaponSlotInInspector)
                    {
                        var weapon = slot.weapon;

                        if (!weapon && !slot.fistAttack) continue;
                        if (weapon && !weapon.GetComponent<WeaponController>()) continue;

                        if (weapon)
                        {
                            WeaponsHelper.InstantiateWeapon(weapon, i, this, Controller);
                        }
                        else if (slot.fistAttack)
                        {
                            slots[i].weaponSlotInGame.Add(new CharacterHelper.Weapon {fistAttack = true});
                            allWeaponsCount++;
                        }
                    }
                }
            }
            else
            {
#if PHOTON_UNITY_NETWORKING
                /*
                if (Controller.projectSettings)
                {
                    for (var i = 0; i < Controller.projectSettings.weaponSlots.Count; i++)
                    {
                        var weaponSlot = Controller.projectSettings.weaponSlots[i];

                        if (weaponSlot.weapon)
                        {
                            if (Controller.projectSettings.useAllWeapons)
                            {
                                if (gameObject.GetComponent<PhotonView>().IsMine)
                                {
                                    if (Controller.projectSettings.weaponsIndices.Contains(i))
                                        WeaponsHelper.InstantiateWeapon(weaponSlot.weapon.gameObject, weaponSlot.slot, this, Controller);
                                }
                                else
                                {
                                    var weaponIndices = gameObject.GetComponent<CharacterSync>().GetSelectedWeapons();
                                    
                                    if (weaponIndices.Contains(i))
                                        WeaponsHelper.InstantiateWeapon(weaponSlot.weapon.gameObject, weaponSlot.slot, this, Controller);
                                }
                            }
                            else
                            {
                                WeaponsHelper.InstantiateWeapon(weaponSlot.weapon.gameObject, weaponSlot.slot, this, Controller);
                            }
                        }
                    }
                }
                */

                for (var i = 0; i < 8; i++)
                {
                    foreach (var slot in slots[i].weaponSlotInInspector)
                    {
                        var weapon = slot.weapon;

                        if (!weapon && !slot.fistAttack) continue;
                        if (weapon && !weapon.GetComponent<WeaponController>()) continue;

                        if (weapon)
                        {
                            WeaponsHelper.InstantiateWeapon(weapon, i, this, Controller);
                        }
                        else if (slot.fistAttack)
                        {
                            slots[i].weaponSlotInGame.Add(new CharacterHelper.Weapon { fistAttack = true });
                            allWeaponsCount++;
                        }
                    }
                }


                for (var i = 0; i < 8; i++)
                {
                    foreach (var slot in slots[i].weaponSlotInInspector.Where(slot => slot.fistAttack))
                    {
                        slots[i].weaponSlotInGame.Add(new CharacterHelper.Weapon {fistAttack = true});
                    }
                }
#endif
            }
        }
        
        void FlashEffect()
        {
            if(Controller.isMultiplayerCharacter || !Controller.UIManager.CharacterUI.flashPlaceholder)
                return;

            var flashImage = Controller.UIManager.CharacterUI.flashPlaceholder;
            
            flashTimeout += Time.deltaTime;
            
            if (flashTimeout > 2)
            {
                if(flashImage.gameObject.activeSelf)
                    flashImage.color = new Color(1, 1, 1, Mathf.Lerp(flashImage.color.a, 0, 0.5f * Time.deltaTime));

                if (flashImage.color.a <= 0.01f && Controller.thisCamera.GetComponent<Motion>())
                {
                    flashImage.color = new Color(1, 1, 1, 0);
                    flashImage.gameObject.SetActive(false);

                    var motion = Controller.thisCamera.GetComponent<Motion>();

                    motion.frameBlending = 0;
                    motion.sampleCount = 0;

                    motion.shutterAngle = Mathf.Lerp(motion.frameBlending, 0, 5 * Time.deltaTime);

                    if (motion.frameBlending <= 0.1f)
                    {
                        Destroy(Controller.thisCamera.GetComponent<Motion>());
                    }
                }
            }
        }

        IEnumerator TakeWeapon()
        {
            var time = 5f;
            
            if (Controller.isCrouch ||// && !weaponController.CurrentWeaponInfo[weaponController.SettingsSlotIndex].disableIkInCrouchState ||
                !Controller.isCrouch && !WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState)
                time = WeaponController.characterAnimations.TakeWeapon.length;

            else if (!Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState)
                time = WeaponController.characterAnimations.TakeWeapon.length / 2;
            
            yield return new WaitForSeconds(time);
            
            hasWeaponTaken = true;
            SmoothIKSwitch = 0;
            WeaponController.canDrawGrenadesPath = true;
            
            if (Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState ||
                !Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState)
            {
                firstLayerSet = true;
            }
            else
            {
                firstLayerSet = false;
            }
            
            Controller.anim.SetBool("HasWeaponTaken", false);
            StartCoroutine("ShootingTimeout"); 
            StopCoroutine("TakeWeapon");
        }

        IEnumerator DropTimeOut(CharacterHelper.Weapon curWeapon)
        {
            yield return new WaitForSeconds(1);
            curWeapon.weapon.GetComponent<PickUp>().enabled = true;
            canDropWeapon = true;
            StopCoroutine("DropTimeOut");

        }

        IEnumerator ShootingTimeout() 
        {
            while (true)
            {
                if (WeaponController && Controller.anim.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
                {
                    WeaponController.canAttack = true;
                    StopCoroutine("ShootingTimeout");
                    break;
                }

                yield return 0;
            }
        }

        IEnumerator SetAnimParameters()
        {
            yield return new WaitForSeconds(0.01f);
            Controller.anim.SetBool("TakeWeapon", true);

            StopCoroutine("SetAnimParameters");
        }


        #region MobileUI

        public void UIAttack()
        {
            UIButtonAttack = true;
            
            if (WeaponController)
            {
                WeaponController.uiButtonAttack = true;
            }
        }

        public void UIEndAttack()
        {
            UIButtonAttack = false;
            
            if (WeaponController)
            {
                WeaponController.uiButtonAttack = false;
            }
        }

        public void UIAim()
        {
            if (slots[currentSlot].weaponSlotInGame.Count > 0 && slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
            {
                Aim();
            }
            else if (WeaponController)
            {
                WeaponController.Aim(false, false, false);
            }
        }

        public void UIReload()
        {
            if(WeaponController)
                WeaponController.Reload();
        }

        public void UIChangeAttackType()
        {
            if(WeaponController)
                WeaponController.ChangeAttack();
        }

        public void UIPickUp()
        {
            pickUpUiButton = true;
        }

        public void UIInventory()
        {
            if (!Controller.UIManager.CharacterUI.Inventory.MainObject) return;

            if (inventoryIsOpened)
            {
                DeactivateInventory();
            }
            else
            {
                ActivateInventory();
            }
        }

        public void UIActivateInventory()
        {
            pressedUIInventoryButton = true;
        }

        public void UIDeactivateInventory()
        {
            pressedUIInventoryButton = false;
        }

        #endregion

        #region AnimationEvents
        
        public void ChangeMagazineVisibility(string value)
        {
            if(!WeaponController.Attacks[WeaponController.currentAttack].Magazine)
                return;
            
            switch (value)
            {
                case "show":
                    WeaponController.Attacks[WeaponController.currentAttack].Magazine.SetActive(true);
                    break;
                case "hide":
                    WeaponController.Attacks[WeaponController.currentAttack].Magazine.SetActive(false);
                    break;
                case "hideAndCreate":
                    WeaponController.HideAndCreateNewMagazine();
                    break;
            }
        }

        public void DropMagazine()
        {
            foreach (var magazine in WeaponController.Attacks[WeaponController.currentAttack].TempMagazine)
            {
                if (magazine)
                {
                    var tempMag = magazine;
                    tempMag.transform.parent = null;
                    tempMag.AddComponent<Rigidbody>();
                    tempMag.AddComponent<DestroyObject>().DestroyTime = 10;
                }   
            }
            
            WeaponController.Attacks[WeaponController.currentAttack].TempMagazine.Clear();
        }

        public void SpawnShell()
        {
            if (WeaponController.Attacks[WeaponController.currentAttack].Shell && WeaponController.Attacks[WeaponController.currentAttack].ShellPoint)
            {
                var _shell = Instantiate(WeaponController.Attacks[WeaponController.currentAttack].Shell, WeaponController.Attacks[WeaponController.currentAttack].ShellPoint.position, WeaponController.Attacks[WeaponController.currentAttack].ShellPoint.localRotation);
                Helper.ChangeLayersRecursively(_shell.transform, "Character");
                _shell.hideFlags = HideFlags.HideInHierarchy;
                _shell.gameObject.AddComponent<ShellControll>().ShellPoint = WeaponController.Attacks[WeaponController.currentAttack].ShellPoint;
            }
        }

        public void PlayAttackSound()
        {
            if (slots[currentSlot].weaponSlotInGame.Count > 0 && !slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack)
            {
                if (WeaponController.Attacks[WeaponController.currentAttack].AttackAudio)
                {
                    WeaponController.GetComponent<AudioSource>().loop = false;
                    WeaponController.GetComponent<AudioSource>().PlayOneShot(WeaponController.Attacks[WeaponController.currentAttack].AttackAudio);
                }
            }
            else
            {
                if (fistAttackAudio && GetComponent<AudioSource>())
                    GetComponent<AudioSource>().PlayOneShot(fistAttackAudio);
            }
        }

        public void ChangeParent(string side)
        {
            switch (side)
            {
                case "left":
                    currentWeapon.transform.parent = Controller.BodyObjects.LeftHand;
                    break;
                case "right":
                    currentWeapon.transform.parent = Controller.BodyObjects.RightHand;
                    break;
                case "rightAndPlace":
                    currentWeapon.transform.parent = Controller.BodyObjects.RightHand;
                    StopCoroutine("SetWeaponAnimations");
                    StartCoroutine("SetWeaponPosition");
                    break;
            }
        }
        
        IEnumerator SetWeaponPosition()
        {
            while (true)
            {
                currentWeapon.transform.localPosition =  Vector3.MoveTowards(currentWeapon.transform.localPosition, WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].WeaponPosition, 0.5f * Time.deltaTime);
                currentWeapon.transform.localRotation = Quaternion.Slerp(currentWeapon.transform.localRotation, Quaternion.Euler(WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].WeaponRotation), 10 * Time.deltaTime);

                if (Helper.ReachedPositionAndRotation(currentWeapon.transform.localPosition, WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].WeaponPosition,
                    currentWeapon.transform.localEulerAngles, WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].WeaponRotation))
                {
                    currentWeapon.transform.localPosition = WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].WeaponPosition;
                    currentWeapon.transform.localEulerAngles = WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].WeaponRotation;
                    StopCoroutine("SetWeaponAnimations");
                    break;
                }
                
                yield return 0;
            }
        }

        public void LaunchGrenade()
        {
            WeaponController.StopCoroutine("FlyGrenade");
            WeaponController.LaunchGrenade();
        }
        
        #endregion
        
        public IEnumerator TakeGrenade()
        {
            yield return new WaitForSeconds(WeaponController.Attacks[WeaponController.currentAttack].WeaponAttacks[0].length);
            TakeNewGreande();
            StopCoroutine("TakeGrenade");
        }

        public void TakeNewGreande()
        {
            Controller.anim.SetBool("Pause", false);
            Controller.anim.SetBool("LaunchGrenade", false);

            if (!Controller.isMultiplayerCharacter)
            {
                if (WeaponController.Attacks[WeaponController.currentAttack].curAmmo > 0)
                {
                    Switch(currentSlot, true, true);
                }
                else
                {
                    WeaponUp();
                }
            }
        }
        
        #region HandsIK

        private void OnAnimatorIK(int layerIndex)
        {
            if(inMultiplayerLobby) return;
            
            if (WeaponController && hasAnyWeapon)
            {
                if (WeaponController.isReloadEnabled)
                {
                    Helper.FingersRotate(null, Controller.anim, "Null");
                }
                else
                {
                    Helper.FingersRotate(WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex], Controller.anim, "Weapon");
                }

                if (!WeaponController.ActiveDebug)
                {
                    var disableIK = slots[currentSlot].weaponSlotInGame[slots[currentSlot].currentWeaponInSlot].fistAttack && !inventoryIsOpened ||
                                    WeaponController.isReloadEnabled || Controller.anim.GetBool("Pause") || !WeaponController.isAimEnabled && //!weaponController.DetectObject &&
                                    (!Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState ||
                                     Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState);
                    
                    if(disableIK)
                    {
                        if (SmoothIKSwitch > 0)
                            SmoothIKSwitch -= 1 * Time.deltaTime;
                        else
                        {
                            SmoothIKSwitch = 0;

                            Controller.anim.SetBool("HasWeaponTaken",
                                !Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState
                                ||  Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState);
                        }
                    }
                    else
                    {
                        if (SmoothIKSwitch < (!Controller.anim.GetBool("HasWeaponTaken") && Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson ? 2 : 1))
                        {
                            var cantIncrease = Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.isCrouch && !Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Idle") &&
                                               !Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Aim_Idle") && !Controller.anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Walk_Forward");

                            if (!cantIncrease)
                                SmoothIKSwitch += 1 * Time.deltaTime;
                        }
                        else
                        {
                            if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                            {
                                if (SmoothIKSwitch > 1.9)
                                {
                                    Controller.anim.SetBool("HasWeaponTaken", true);
                                }
                            }
                            else 
                            {
                                Controller.anim.SetBool("HasWeaponTaken", true);
                            }

                            firstLayerSet = true;
                            SmoothIKSwitch = 1;
                        }
                    }

                    if (!Controller.isMultiplayerCharacter)
                    {
                        if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !WeaponController.isReloadEnabled && (WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState ||
                            WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState))
                        {
                            if (setWeaponLayer && firstLayerSet)
                            {
                                Controller.anim.SetLayerWeight(2, SmoothIKSwitch);

                                if (SmoothIKSwitch <= 0)
                                {
                                    if (WeaponController && !WeaponController.setHandsPositionsAim && !WeaponController.isAimEnabled)
                                    {
                                        WeaponController.setHandsPositionsAim = true;
                                    }
                                }
                            }
                        }
                        else if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown)
                        {
                            Controller.anim.SetLayerWeight(2, 1);
                        }

                        
                        if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson &&
                            !Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState ||
                            Controller.isCrouch && WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState)
                        {
                            if (WeaponController.isReloadEnabled || !hasWeaponTaken)
                            {
                                setWeaponLayer = false;
                                Controller.anim.SetLayerWeight(2, Mathf.Lerp(Controller.anim.GetLayerWeight(2), 1, 2 * Time.deltaTime));
                            }
                            else if (!WeaponController.isReloadEnabled && !setWeaponLayer && hasWeaponTaken)
                            {
                                if (!WeaponController.isAimEnabled)
                                {
                                    Controller.anim.SetLayerWeight(2, Mathf.Lerp(Controller.anim.GetLayerWeight(2), 0, 2 * Time.deltaTime));

                                    if (Math.Abs(Controller.anim.GetLayerWeight(2)) < 0.1f)
                                    {
                                        Controller.anim.SetLayerWeight(2, 0);
                                        setWeaponLayer = true;
                                    }
                                    
                                }
                                else
                                {
                                    if (SmoothIKSwitch > 0.9f)
                                        setWeaponLayer = true;
                                }
                            }
                        }

                        if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson && hasAnyWeapon)
                        {
                            Controller.anim.SetLayerWeight(3, Mathf.Abs(Controller.anim.GetLayerWeight(2) - 1));
                        }
                        else
                        {
                            Controller.anim.SetLayerWeight(3, 0);
                        }
                    }
                    
                    if (layerIndex == Controller.currentAnimatorLayer)
                    {
                        if (WeaponController.IkObjects.RightObject && WeaponController.IkObjects.LeftObject)
                        {
                            if (WeaponController.CanUseIK && hasWeaponTaken)
                            {
                                Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftObject,
                                    WeaponController.IkObjects.RightObject, Controller.BodyObjects.LeftHand, Controller.BodyObjects.RightHand, SmoothIKSwitch, true);
                                
                                if(WeaponController.isAimEnabled && WeaponController.Attacks[WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                                {
                                    if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson && WeaponController.Attacks[WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee)
                                    {
                                        SmoothHeadIKSwitch = Mathf.Lerp(SmoothHeadIKSwitch, 1, 0.7f * Time.deltaTime);
                                        Controller.anim.SetLookAtWeight(SmoothHeadIKSwitch);
                                    }
                                }
                                else
                                {
                                    SmoothHeadIKSwitch = Mathf.Lerp(SmoothHeadIKSwitch, 0, 5 * Time.deltaTime);
                                    Controller.anim.SetLookAtWeight(SmoothHeadIKSwitch);
                                }
                                
                                Controller.anim.SetLookAtPosition(WeaponController.transform.position + Controller.DirectionObject.forward * 3);
                            }
                        }
                    }
                }
                else if (WeaponController.ActiveDebug && WeaponController.canUseValuesInAdjustment)
                {
                    if (!WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableElbowIK)
                    {
                        Controller.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                        Controller.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
                    }
                    else
                    {
                        Controller.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
                        Controller.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                    }
                    
                    Controller.anim.SetIKHintPosition(AvatarIKHint.LeftElbow, WeaponController.IkObjects.LeftElbowObject.position);
                    Controller.anim.SetIKHintPosition(AvatarIKHint.RightElbow, WeaponController.IkObjects.RightElbowObject.position);

                    switch (WeaponController.DebugMode)
                    {
                        case IKHelper.IkDebugMode.Aim:
                            
                            WeaponController.hasAimIKChanged = true;
                            
                            if(Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                Controller.anim.SetLayerWeight(2, 1);
                            
                            Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftAimObject,
                                WeaponController.IkObjects.RightAimObject, Controller.BodyObjects.TopBody, Controller.BodyObjects.TopBody, DebugIKValue, WeaponController.pinLeftObject);
                            
                            break;
                        case IKHelper.IkDebugMode.Wall:
                            WeaponController.hasWallIKChanged = true;
                            
                            if(Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                Controller.anim.SetLayerWeight(2, 1);
                            
                            Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftWallObject,
                                WeaponController.IkObjects.RightWallObject, Controller.BodyObjects.TopBody, Controller.BodyObjects.TopBody, DebugIKValue, WeaponController.pinLeftObject);
                            
                            break;
                        case IKHelper.IkDebugMode.Norm:
                            if (!WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInNormalState)
                            {
                                if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                {
                                    Controller.anim.SetLayerWeight(2, 1);
                                    Controller.anim.SetLayerWeight(3, 0);
                                }
                                
                                Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftObject,
                                    WeaponController.IkObjects.RightObject, Controller.BodyObjects.TopBody, Controller.BodyObjects.TopBody, DebugIKValue, WeaponController.pinLeftObject);
                            }
                            else
                            {
                                Controller.anim.SetLayerWeight(2, 0);

                                if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                {
                                    Controller.anim.SetLayerWeight(3, 1);
                                }

                                Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftObject,
                                    WeaponController.IkObjects.RightObject, Controller.BodyObjects.TopBody, Controller.BodyObjects.TopBody, 0, WeaponController.pinLeftObject);
                            }
                            break;
                        case IKHelper.IkDebugMode.Crouch:
                            if (!WeaponController.CurrentWeaponInfo[WeaponController.settingsSlotIndex].disableIkInCrouchState)
                            {
                                if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                {
                                    Controller.anim.SetLayerWeight(3, 0);
                                    Controller.anim.SetLayerWeight(2, 1);
                                }
                                
                                WeaponController.hasCrouchIKChanged = true;
                                
                                Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftCrouchObject,
                                    WeaponController.IkObjects.RightCrouchObject, Controller.BodyObjects.TopBody, Controller.BodyObjects.TopBody, DebugIKValue, WeaponController.pinLeftObject);
                            }
                            else
                            {
                                Controller.anim.SetLayerWeight(2, 0);

                                if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
                                {
                                    Controller.anim.SetLayerWeight(3, 1);
                                }

                                Helper.HandsIK(Controller, WeaponController, this, WeaponController.IkObjects.LeftCrouchObject,
                                    WeaponController.IkObjects.RightCrouchObject, Controller.BodyObjects.TopBody, Controller.BodyObjects.TopBody, 0, WeaponController.pinLeftObject);
                            }

                            break;
                    }
                }
            }
            else
            {
                Helper.FingersRotate(null, Controller.anim, "Null");
            }
        }
        #endregion
    }
}





