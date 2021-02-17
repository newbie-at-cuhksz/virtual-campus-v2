// GercStudio
// © 2018-2019

using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class Controller : MonoBehaviour
    {
        public CharacterController CharacterController;
        public InventoryManager WeaponManager;
        public Controller OriginalScript;
        public CharacterSync CharacterSync;
        public UIManager UIManager;

        public PUNHelper.Teams MyTeam;
        public PUNHelper.CanKillOthers CanKillOthers;

        public CharacterHelper.CameraType TypeOfCamera;
        public CharacterHelper.CameraParameters CameraParameters;

        public CharacterHelper.MovementType MovementType = CharacterHelper.MovementType.FastAndAccurate;

        public Animator anim;

        public RuntimeAnimatorController characterAnimatorController;

        public AudioSource FeetAudioSource;

        public Helper.AnimationClipOverrides ClipOverrides;

        public CharacterHelper.CharacterOffset CharacterOffset;

        public CharacterHelper.Speeds FPSpeed;
        public CharacterHelper.Speeds TPSpeed;
        public CharacterHelper.Speeds TDSpeed;

        [Range(0.1f, 2)] public float TPspeedOffset = 1;

        [Range(1, 1000)] public float PlayerHealth = 100;
        public float PlayerHealthPercent = 100;

        [Range(0, 50)] public float CrouchIdleNoise;
        [Range(0, 50)] public float CrouchMovementNoise;
        [Range(0, 50)] public float SprintMovementNoise;
        [Range(0, 50)] public float MovementNoise;
        [Range(0, 50)] public float IdleNoise;
        [Range(0, 50)] public float JumpNoise;

        public float bodyRotationUpLimit_y;
        public float bodyRotationDownLimit_y;
        public float bodyRotationUpLimit_x;
        public float bodyRotationDownLimit_x;
        public float defaultHeight = -1;
        public float pressButtonTimeout;
        public float changeCameraTypeTimeout;
        public float currentCharacterControllerCenter;
        public float speedDevider = 1;
        public float headMultiplier = 1;
        public float bodyMultiplier = 1;
        public float handsMultiplier = 1;
        public float legsMultiplier = 1;

        public int characterTag;

        public float noiseRadius;
        public float CurrentSpeed;
        [Range(0.1f, 1)] public float CrouchHeight = 0.5f;

        public float defaultCharacterCenter;
        public float middleAngleX;

        #region InspectorParameters

        public int TDSpeedInspectorTab;
        public int TPSpeedInspectorTab;
        public int moveInspectorTab;
        public int inspectorTabTop;
        public int inspectorTabDown;
        public int currentInspectorTab;
        public int otherSettingsInspectorTab;
        public int inspectorSettingsTab;
        public int cameraInspectorTab;

        public string curName;

        public bool delete;
        public bool rename;
        public bool renameError;

        #endregion

        public bool bodyLimit;
        public bool smoothWeapons = true;
        public bool oneShotOneKill;
        public bool ActiveCharacter;
        public bool isMultiplayerCharacter;
        public bool multiplayerCrouch;
        public bool activeJump = true;
        public bool activeCrouch = true;
        public bool changeCameraType;
        public bool isPause;
        public bool DebugMode;
        public bool[] hasAxisButtonPressed = new bool[18];
        public bool AdjustmentScene;
        public bool SmoothCameraWhenMoving = false;
        public bool CameraFollowCharacter = true;
        public bool hasMoveButtonPressed;
        public bool tdModeLikeTp = true; //TP-TD
        public bool onNavMesh;
        public bool inGrass;

        public CharacterHelper.BodyObjects BodyObjects = new CharacterHelper.BodyObjects();
        public IKHelper.FeetIKVariables IKVariables;

        public List<Transform> BodyParts = new List<Transform> { null, null, null, null, null, null, null, null, null, null, null };
        public Transform DirectionObject;
        public Transform ColliderToObjectsDetection;

        public GameObject thisCamera;

        public SphereCollider noiseCollider;

        public CameraController CameraController;

        public List<Texture> BloodHoles = new List<Texture> { null };

        public Image PlayerHealthBar;
        public Image PlayerHealthBarBackground;

        public Texture2D KilledWeaponImage;

        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right,
            Stationary,
            ForwardLeft,
            ForwardRight,
            BackwardLeft,
            BackwardRight
        }

        public Direction MoveDirection;

        public Vector3 directionVector;
        public Vector3 MoveVector;
        public Vector3 BodyLocalEulerAngles;

        public Quaternion RotationAngle;
        public Quaternion CurrentRotation;

        public AnimatorOverrideController newController;

        public ProjectSettings projectSettings;

        public KeyCode[] _keyboardCodes = new KeyCode[20];
        public KeyCode[] _gamepadCodes = new KeyCode[18];

        public string[] _gamepadAxes = new string[5];
        public string[] _gamepadButtonsAxes = new string[18];

        public List<HitMarker> hitMarkers;

        public string KillerName;
        public string CharacterName;

        private RaycastHit distanceInfo;
        private RaycastHit heightInfo;

        private Transform bodylooks;

        private bool isObstacle;
        private bool CanMove;
        private bool wasRunningActiveBeforeJump;
        public bool isSprint;
        public bool isJump;
        public bool isCrouch;
        private bool deactivateCrouch;
        private bool activateCrouch;
        private bool isCeiling;
        private bool leftStepSound;
        private bool rightStepSound;
        private bool instantiateRagdoll;

        // steps for jumping
        private bool startJumping;
        private bool flyingUp;
        private bool flyingDown;

        private bool crouchTimeOut = true;
        private bool setDefaultDistance;
        private bool onGround;
        private bool firstTake = true;
        private bool meleeDamage;

        private bool clickMoveButton;
        private float defaultDistance;
        public float SmoothIKSwitch = 1;
        private float BodyHeight;
        private float JumpPosition;

        private float hipsAngleX;
        private float spineAngleX;
        public float headHeight = -1;
        public float currentGravity;
        private float defaultGravity;
        private float newJumpHeight;
        private float healthPercent;
        private float checkOnNavMeshTimer;

        private float angleBetweenCharacterAndCamera;

        private int touchId = -1;
        public int currentAnimatorLayer;

        private Vector3 CheckCollisionVector;

        private Vector2 MobileMoveStickDirection;
        private Vector2 MobileTouchjPointA, MobileTouchjPointB;

        private RaycastHit HeightInfo;

        private bool clickButton;
        public bool isCharacterInLobby;

        private void OnAnimatorMove()
        {
            if (isCharacterInLobby) return;

            if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson) return;

            switch (MovementType)
            {
                case CharacterHelper.MovementType.FastAndAccurate:
                    {
                        anim.SetFloat("Speed Devider", 1 * TPspeedOffset / speedDevider);

                        switch (TypeOfCamera)
                        {
                            case CharacterHelper.CameraType.ThirdPerson:
                                if (!isJump && !anim.GetBool("Aim"))
                                    transform.position += anim.deltaPosition * TPspeedOffset / speedDevider;
                                break;
                            case CharacterHelper.CameraType.TopDown:
                                break;
                        }


                        if (TypeOfCamera != CharacterHelper.CameraType.TopDown && !isJump && !anim.GetBool("Aim"))
                        {
                            transform.rotation = anim.rootRotation;
                        }

                        break;
                    }
                case CharacterHelper.MovementType.Realistic:

                    //					anim.SetFloat("Speed Devider", 1 / speedDevider);
                    //					
                    //					transform.position += anim.deltaPosition / speedDevider;
                    //					
                    //					if(TypeOfCamera != CharacterHelper.CameraType.TopDown || TypeOfCamera == CharacterHelper.CameraType.TopDown && !CameraParameters.LockCamera)
                    //						transform.rotation = anim.rootRotation;

                    break;
            }
        }

        private void Awake()
        {
            if (FindObjectOfType<Lobby>())
            {
                isCharacterInLobby = true;
                return;
            }

            /*
            gameObject.layer = LayerMask.NameToLayer("Character");

            foreach (Transform tran in GetComponentsInChildren<Transform>())
            {
                tran.gameObject.layer = LayerMask.NameToLayer("Character");
            }
            */

            if (FindObjectOfType<GameManager>())
            {
                UIManager = FindObjectOfType<GameManager>().CurrentUIManager;
                CanKillOthers = PUNHelper.CanKillOthers.Everyone;

                if (PlayerHealthBarBackground)
                    PlayerHealthBarBackground.gameObject.SetActive(false);

                if (PlayerHealthBar)
                    PlayerHealthBar.gameObject.SetActive(false);
            }
            else if (FindObjectOfType<RoomManager>())
            {
                UIManager = FindObjectOfType<RoomManager>().currentUIManager;
            }
#if UNITY_EDITOR
            else if (FindObjectOfType<Adjustment>())
            {
                UIManager = FindObjectOfType<Adjustment>().UIManager;
            }
#endif
            else
            {
                Debug.LogError("UI Manager was not be loaded.");
                Debug.Break();
            }

            WeaponManager = gameObject.GetComponent<InventoryManager>();

            anim = gameObject.GetComponent<Animator>();

            if (!projectSettings)
            {
                Debug.LogError("<color=red>Missing component</color> [Project Settings]. Please reimport this kit.");
                Debug.Break();
            }

            if (!WeaponManager)
            {
                Debug.LogWarning("<color=yellow>Missing Component</color> [Weapon Manager] script. Please, add it.");
                Debug.Break();
            }

            if (!CharacterController)
            {
                CharacterController = gameObject.AddComponent<CharacterController>();
                //				Debug.LogWarning("<color=yellow>Missing Component</color> [Character Controller]. Please, add it.");
                //				Debug.Break();
            }

            if (!gameObject.GetComponent<NavMeshObstacle>())
            {
                var script = gameObject.AddComponent<NavMeshObstacle>();
                script.shape = NavMeshObstacleShape.Capsule;
                script.carving = true;
            }

            if (!anim)
            {
                Debug.LogWarning("<color=yellow>Missing Component</color> [Animator]. Please, add it.");
                Debug.Break();
            }
        }

        void Start()
        {
            Helper.ManageBodyColliders(BodyParts, this);

            ColliderToObjectsDetection = new GameObject("ColliderToCheckObjects").transform;
            ColliderToObjectsDetection.parent = BodyObjects.TopBody;
            ColliderToObjectsDetection.hideFlags = HideFlags.HideInHierarchy;
        }

        void OnEnable()
        {
            if (isCharacterInLobby) return;

            StopAllCoroutines();

            WeaponManager.pressInventoryButton = projectSettings.PressInventoryButton;

            if (firstTake)
            {
                //tdModeLikeTp = false;

                if (CameraParameters.activeFP && TypeOfCamera == CharacterHelper.CameraType.FirstPerson) TypeOfCamera = CharacterHelper.CameraType.FirstPerson;
                else if (CameraParameters.activeTP && TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                {
                    TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
                }

                else if (CameraParameters.activeTD && TypeOfCamera == CharacterHelper.CameraType.TopDown)
                {
                    if (CameraParameters.alwaysTDAim || CameraParameters.lockCamera)
                        TypeOfCamera = CharacterHelper.CameraType.TopDown;
                    else
                    {
                        TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
                        tdModeLikeTp = true;
                    }
                }
                else if (CameraParameters.activeFP) TypeOfCamera = CharacterHelper.CameraType.FirstPerson;
                else if (CameraParameters.activeTD)
                {
                    if (CameraParameters.alwaysTDAim || CameraParameters.lockCamera)
                        TypeOfCamera = CharacterHelper.CameraType.TopDown;
                    else
                    {
                        TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
                        tdModeLikeTp = true;
                    }
                }
                else if (CameraParameters.activeTP) TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
                else
                {
                    //Debug.LogError("Please select any active camera view.", gameObject);
                    //Debug.Break();
                }

                healthPercent = PlayerHealth;

                if ((Application.isMobilePlatform || projectSettings.mobileDebug) && !UIManager.UIButtonsMainObject)
                {
                    UIManager.UIButtonsMainObject = Helper.NewCanvas("UI Buttons MainObject", new Vector2(1920, 1080), UIManager.transform).gameObject;

                    for (var i = 0; i < 17; i++)
                    {
                        UIManager.uiButtons[i] = Instantiate(projectSettings.uiButtons[i], UIManager.UIButtonsMainObject.transform);
                    }

                    Destroy(UIManager.uiButtons[15].GetComponent<Button>());
                    Destroy(UIManager.uiButtons[16].GetComponent<Button>());

                    UIManager.moveStick = UIManager.uiButtons[16].gameObject;
                    UIManager.moveStickOutline = UIManager.uiButtons[15].gameObject;

                    UIManager.cameraStickOutline = Instantiate(UIManager.uiButtons[15].gameObject, UIManager.UIButtonsMainObject.transform, true);
                    UIManager.cameraStick = Instantiate(UIManager.uiButtons[16].gameObject, UIManager.UIButtonsMainObject.transform, true);

                    UIManager.cameraStick.GetComponent<RectTransform>().anchoredPosition = UIManager.moveStick.GetComponent<RectTransform>().anchoredPosition;
                    UIManager.cameraStickOutline.GetComponent<RectTransform>().anchoredPosition = UIManager.moveStickOutline.GetComponent<RectTransform>().anchoredPosition;

                    Helper.AddButtonsEvents(UIManager.uiButtons, WeaponManager, this);

                    var gameManager = FindObjectOfType<GameManager>();

                    if (gameManager)
                    {
                        if (UIManager.uiButtons[9])
                        {
                            UIManager.uiButtons[9].onClick.AddListener(delegate { gameManager.Pause(true); });
                        }

                        if (UIManager.uiButtons[12])
                        {
                            UIManager.uiButtons[12].onClick.AddListener(gameManager.SwitchCharacter);
                        }
                    }

                    UIHelper.ManageUIButtons(this, WeaponManager, UIManager, CharacterSync);
                }

                firstTake = false;
            }
            else
            {
                PlayerHealth = healthPercent;
            }

            for (var i = 0; i < 20; i++)
            {
                Helper.ConvertKeyCodes(ref _keyboardCodes[i], projectSettings.KeyBoardCodes[i]);
            }

            for (var i = 0; i < 18; i++)
            {
                Helper.ConvertGamepadCodes(ref _gamepadCodes[i], projectSettings.GamepadCodes[i]);
            }

            for (var i = 0; i < 18; i++)
            {
                Helper.ConvertAxes(ref _gamepadButtonsAxes[i], projectSettings.GamepadCodes[i]);
            }

            for (var i = 0; i < 5; i++)
            {
                Helper.ConvertAxes(ref _gamepadAxes[i], projectSettings.GamepadAxes[i]);
            }

            MoveVector = Vector3.zero;

            anim.Rebind();

            anim.runtimeAnimatorController = characterAnimatorController;

            newController = new AnimatorOverrideController(anim.runtimeAnimatorController);
            anim.runtimeAnimatorController = newController;
            ClipOverrides = new Helper.AnimationClipOverrides(newController.overridesCount);
            newController.GetOverrides(ClipOverrides);

            if (DirectionObject)
            {
                DirectionObject.localEulerAngles = CharacterOffset.directionObjRotation;
            }
            else
            {
                Debug.LogError("<color=yellow>Missing component</color>: [Direction Object]. Please create your character again.");
                Debug.Break();
            }

            if (!thisCamera || !CameraController)
            {
                var foundObjects = FindObjectsOfType<CameraController>();
                foreach (var camera in foundObjects)
                {
                    if (camera.transform.parent == transform)
                    {
                        CameraController = camera.GetComponent<CameraController>();
                        thisCamera = CameraController.gameObject;
                    }
                }
            }

            if (thisCamera)
            {
                thisCamera.SetActive(true);
            }
            else if (!thisCamera || !CameraController)
            {
                Debug.LogError("<Color=red>Missing component</color> [This camera] in Controller Script", gameObject);
                Debug.Break();
            }

            StartCoroutine("SetDefaultHeight");

            if (FeetAudioSource)
                FeetAudioSource.hideFlags = HideFlags.HideInHierarchy;

            defaultGravity = Physics.gravity.y;
            currentGravity = defaultGravity;

            deactivateCrouch = true;

            var center = CharacterController.center;
            center = new Vector3(center.x, -CharacterOffset.CharacterHeight, center.z);
            CharacterController.center = center;
            defaultCharacterCenter = -CharacterOffset.CharacterHeight;

            CharacterController.skinWidth = 0.01f;
            CharacterController.height = 1;
            //			CharacterController.radius = 0.22f;

            if (!noiseCollider)
                Helper.CreateNoiseCollider(transform, this);

            if (isMultiplayerCharacter)
            {
                CameraController.SetAnimVariables();
                return;
            }

            MovementType = CharacterHelper.MovementType.FastAndAccurate;

            Helper.ChangeLayersRecursively(transform, "Character");

            Input.simulateMouseWithTouches = false;

            if (CameraController)
            {
                CameraController.maxMouseAbsolute = middleAngleX + CameraParameters.fpXLimitMax;
                CameraController.minMouseAbsolute = middleAngleX + CameraParameters.fpXLimitMin;
            }

            gameObject.tag = "Player";

            //GetComponent<AudioListener>().enabled = true;
        }


        void Update()
        {
            if (isCharacterInLobby) return;

            /*
            Vector2 mouseDirection = Input.mousePosition;
            mouseDirection -= new Vector2(Screen.width / 2, Screen.height / 2);
            Vector3 lookDirection = new Vector3(mouseDirection.x, 0, mouseDirection.y);
            transform.LookAt(transform.position + lookDirection);
            */

            noiseCollider.radius = noiseRadius;

            GetDamageInBodyColliders();

            if (isMultiplayerCharacter || !ActiveCharacter)
                return;

            CheckHealth();

            currentCharacterControllerCenter = CharacterController.center.y;

            if (TypeOfCamera != CharacterHelper.CameraType.TopDown)
                anim.SetFloat("CameraAngle", Helper.AngleBetween(transform.forward, thisCamera.transform.forward));
            else
            {
                anim.SetFloat("CameraAngle", Helper.AngleBetween(transform.forward, !CameraParameters.lockCamera ? thisCamera.transform.forward :
                    CameraController.BodyLookAt.position - transform.position));
            }

            changeCameraTypeTimeout += Time.deltaTime;

            if (!AdjustmentScene)
            {
                if (projectSettings.ButtonsActivityStatuses[11] && (Input.GetKeyDown(_gamepadCodes[11]) || Input.GetKeyDown(_keyboardCodes[11]) || Helper.CheckGamepadAxisButton(11, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", projectSettings.AxisButtonValues[11])))
                    ChangeCameraType();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.C))
                    ChangeCameraType();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                if (!isPause && !isJump && !anim.GetBool("Pause") && (!WeaponManager.WeaponController || !WeaponManager.WeaponController.isReloadEnabled))
                {
                    if (TypeOfCamera == CharacterHelper.CameraType.TopDown || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && tdModeLikeTp)
                    {
                        CharacterHelper.ChangeTDMode(this);
                        CameraController.ReloadParameters();

                        if (WeaponManager.WeaponController)
                            WeaponsHelper.SetWeaponPositions(WeaponManager.WeaponController, true, DirectionObject);

#if PHOTON_UNITY_NETWORKING
                        if (CharacterSync)
                            CharacterSync.ChangeTDMode();
#endif
                    }
                }
            }

            if (projectSettings.ButtonsActivityStatuses[2] && (Input.GetKeyDown(_gamepadCodes[2]) || Input.GetKeyDown(_keyboardCodes[2]) || Helper.CheckGamepadAxisButton(2, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", projectSettings.AxisButtonValues[2])))
                Jump();

            if (projectSettings.PressSprintButton)
            {
                if ((Input.GetKey(_gamepadCodes[0]) || Input.GetKey(_keyboardCodes[0]) || Helper.CheckGamepadAxisButton(0, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKey", projectSettings.AxisButtonValues[0])) && projectSettings.ButtonsActivityStatuses[0])
                    Sprint(true, "press");
                else Sprint(false, "press");
            }
            else
            {
                if ((Input.GetKeyDown(_gamepadCodes[0]) || Input.GetKeyDown(_keyboardCodes[0]) ||
                     Helper.CheckGamepadAxisButton(0, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", projectSettings.AxisButtonValues[0])) && projectSettings.ButtonsActivityStatuses[0])
                {
                    //					if(isCrouch)
                    //						DeactivateCrouch();

                    Sprint(true, "click");
                }
            }

            if (projectSettings.PressCrouchButton)
            {
                if (projectSettings.ButtonsActivityStatuses[1] && (Input.GetKey(_gamepadCodes[1]) || Input.GetKey(_keyboardCodes[1]) || Helper.CheckGamepadAxisButton(1, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKey", projectSettings.AxisButtonValues[1])))
                {
                    if (!activateCrouch)
                    {
                        if (isSprint)
                            DeactivateSprint();

                        Crouch(true, "press");
                        deactivateCrouch = false;
                        activateCrouch = true;
                        crouchTimeOut = false;
                        StartCoroutine("CrouchTimeout");
                    }
                }
                else
                {
                    if (!deactivateCrouch && crouchTimeOut)
                    {
                        Crouch(false, "press");
                    }
                }
            }
            else
            {
                if (projectSettings.ButtonsActivityStatuses[1] && (Input.GetKeyDown(_gamepadCodes[1]) || Input.GetKeyDown(_keyboardCodes[1]) || Helper.CheckGamepadAxisButton(1, _gamepadButtonsAxes, hasAxisButtonPressed, "GetKeyDown", projectSettings.AxisButtonValues[1])))
                {
                    if (crouchTimeOut)
                    {
                        if (isSprint)
                            DeactivateSprint();

                        Crouch(true, "click");
                        crouchTimeOut = false;
                        StartCoroutine("CrouchTimeout");
                    }
                }
            }

            GetLocomotionInput();
            SnapAlignCharacterWithCamera();
            ProcessMotion();

            if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson || MovementType == CharacterHelper.MovementType.FastAndAccurate) JumpingProcess();

            HeightDetection();

            checkOnNavMeshTimer += Time.deltaTime;

            if (checkOnNavMeshTimer > 2)
            {
                checkOnNavMeshTimer = 0;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(new Vector3(transform.position.x, transform.position.y, transform.position.z), out hit, 1000, NavMesh.AllAreas))
                {
                    onNavMesh = hit.distance <= 7;
                }
                else
                {
                    onNavMesh = false;
                }
            }
        }

        void HeightDetection()
        {
            if (defaultHeight == -1)
                return;

            RaycastHit info;
            RaycastHit info2;
            //			Debug.DrawRay(BodyObjects.Hips.position - transform.forward, Vector3.down * 10, anim.GetBool("OnFloor") ? Color.green : Color.red);
            //			Debug.DrawRay(BodyObjects.Hips.position + transform.forward * 2, Vector3.down * 10, anim.GetBool("OnFloorForward") ? Color.green : Color.red);

            if (Physics.Raycast(BodyObjects.Hips.position - transform.forward, Vector3.down, out info, 100, Helper.layerMask()))
            {
                anim.SetBool("OnFloor", !(defaultHeight + 1 < info.distance));
            }

            if (Physics.Raycast(BodyObjects.Hips.position + transform.forward * 2, Vector3.down, out info2, 100, Helper.layerMask()))
            {
                if (defaultHeight + 1 < info2.distance)
                {
                    if (anim.GetBool("OnFloorForward"))
                        anim.SetFloat("FallingHeight", info2.distance);

                    anim.SetBool("OnFloorForward", false);
                }
                else
                {
                    anim.SetBool("OnFloorForward", true);
                }
            }
        }

        IEnumerator SetDefaultHeight()
        {
            yield return new WaitForSeconds(0.1f);
            RaycastHit info;

            if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out info, 100, Helper.layerMask()))
                defaultHeight = info.distance;

            if (Physics.Raycast(BodyObjects.Head.position, Vector3.down, out info, 100, Helper.layerMask()))
                headHeight = info.distance;

            onGround = true;

            StopCoroutine("SetDefaultHeight");
        }

        void GetLocomotionInput()
        {
            directionVector = Vector3.zero;

            if (!isPause)
            {
                hasMoveButtonPressed = false;

                if (Application.isMobilePlatform || projectSettings.mobileDebug)
                {
                    InputHelper.CheckMobileJoystick(UIManager.moveStick, UIManager.moveStickOutline, ref touchId, projectSettings, ref MobileTouchjPointA, ref MobileTouchjPointB, ref MobileMoveStickDirection, this);
                    directionVector = new Vector3(MobileMoveStickDirection.x, 0, MobileMoveStickDirection.y);
                }
                else
                {
                    if (Mathf.Abs(Input.GetAxis(_gamepadAxes[0])) > 0.1f || Mathf.Abs(Input.GetAxis(_gamepadAxes[1])) > 0.1f)
                    {
                        hasMoveButtonPressed = false;

                        var Horizontal = Input.GetAxis(_gamepadAxes[0]);
                        var Vertical = Input.GetAxis(_gamepadAxes[1]);

                        if (projectSettings.invertAxes[0])
                            Horizontal *= -1;

                        if (projectSettings.invertAxes[1])
                            Vertical *= -1;

                        if (Mathf.Abs(Horizontal) > 0.1f || Mathf.Abs(Vertical) > 0.1f)
                        {
                            if (!anim.GetBool("Move"))
                                anim.SetBool("MoveButtonHasPressed", true);

                            hasMoveButtonPressed = true;
                        }

                        directionVector = new Vector3(Horizontal, 0, Vertical);
                    }
                    else
                    {
                        hasMoveButtonPressed = false;

                        //						CharacterHelper.CheckButton(this, _keyboardCodes[12], "forward");
                        //						CharacterHelper.CheckButton(this, _keyboardCodes[13], "backward");
                        //						CharacterHelper.CheckButton(this, _keyboardCodes[14], "right");
                        //						CharacterHelper.CheckButton(this, _keyboardCodes[15], "left");
                        //						
                        if (Input.GetKeyDown(_keyboardCodes[12]))
                        {
                            anim.SetBool("MoveButtonHasPressed", false);
                            clickMoveButton = true;
                        }
                        if (Input.GetKey(_keyboardCodes[12]))
                        {
                            directionVector += Vector3.forward;
                            hasMoveButtonPressed = true;
                        }
                        if (Input.GetKeyUp(_keyboardCodes[12]))
                        {
                            clickMoveButton = false;
                            directionVector += Vector3.forward;

                            if (!anim.GetBool("Move"))
                                anim.SetBool("MoveButtonHasPressed", true);
                        }

                        if (Input.GetKeyDown(_keyboardCodes[14]))
                        {
                            clickMoveButton = true;
                            anim.SetBool("MoveButtonHasPressed", false);
                        }
                        if (Input.GetKey(_keyboardCodes[14]))
                        {
                            directionVector += Vector3.right;
                            hasMoveButtonPressed = true;
                        }
                        if (Input.GetKeyUp(_keyboardCodes[14]))
                        {
                            clickMoveButton = false;
                            directionVector += Vector3.right;

                            if (!anim.GetBool("Move"))
                                anim.SetBool("MoveButtonHasPressed", true);
                        }

                        if (Input.GetKeyDown(_keyboardCodes[13]))
                        {
                            clickMoveButton = true;
                            anim.SetBool("MoveButtonHasPressed", false);
                        }
                        if (Input.GetKey(_keyboardCodes[13]))
                        {
                            directionVector -= Vector3.forward;
                            hasMoveButtonPressed = true;
                        }
                        if (Input.GetKeyUp(_keyboardCodes[13]))
                        {
                            clickMoveButton = false;
                            directionVector -= Vector3.forward;
                            if (!anim.GetBool("Move"))
                                anim.SetBool("MoveButtonHasPressed", true);
                        }

                        if (Input.GetKeyDown(_keyboardCodes[15]))
                        {
                            clickMoveButton = true;
                            anim.SetBool("MoveButtonHasPressed", false);
                        }
                        if (Input.GetKey(_keyboardCodes[15]))
                        {
                            directionVector -= Vector3.right;
                            hasMoveButtonPressed = true;
                        }
                        if (Input.GetKeyUp(_keyboardCodes[15]))
                        {
                            clickMoveButton = false;
                            directionVector -= Vector3.right;

                            if (!anim.GetBool("Move"))
                                anim.SetBool("MoveButtonHasPressed", true);
                        }
                    }
                }

                anim.SetBool("PressMoveAxis", hasMoveButtonPressed);

                if (!anim.GetCurrentAnimatorStateInfo(1).IsName("Attack") && !anim.GetCurrentAnimatorStateInfo(2).IsName("Attack"))
                {
                    if (hasMoveButtonPressed)
                    {
                        if (isSprint)
                        {
                            noiseRadius = Mathf.Lerp(noiseRadius, SprintMovementNoise / (inGrass ? 2 : 1), 5 * Time.deltaTime);
                        }
                        else if (isCrouch)
                        {
                            noiseRadius = Mathf.Lerp(noiseRadius, CrouchMovementNoise / (inGrass ? 2 : 1), 5 * Time.deltaTime);
                        }
                        else
                        {
                            noiseRadius = Mathf.Lerp(noiseRadius, MovementNoise / (inGrass ? 2 : 1), 5 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        noiseRadius = Mathf.Lerp(noiseRadius, !isCrouch ? IdleNoise : CrouchIdleNoise, 5 * Time.deltaTime);
                    }
                }

                //				if(directionVector.magnitude > 0)
                CheckCollisionVector = directionVector * 100;

                if (CanMove)
                {
                    if (hasMoveButtonPressed)
                    {
                        if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                        {
                            anim.SetFloat("Horizontal", directionVector.x, 0.5f, Time.deltaTime);
                            anim.SetFloat("Vertical", directionVector.z, 0.5f, Time.deltaTime);
                        }
                        else
                        {
                            anim.SetFloat("Horizontal", directionVector.x);
                            anim.SetFloat("Vertical", directionVector.z);
                        }
                    }
                    else
                    {
                        anim.SetFloat("Horizontal", directionVector.x);
                        anim.SetFloat("Vertical", directionVector.z);
                    }
                }
                else
                {
                    anim.SetFloat("Horizontal", 0);
                    anim.SetFloat("Vertical", 0);
                }
            }
            else
            {
                anim.SetFloat("Horizontal", 0, 0.3f, Time.deltaTime);
                anim.SetFloat("Vertical", 0, 0.3f, Time.deltaTime);

                Sprint(false, "press");
            }

            if (!isPause)
            {

                if (clickMoveButton && WeaponManager.WeaponController && WeaponManager.WeaponController.isAimEnabled && isCrouch && TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
                {
                    WeaponManager.WeaponController.Aim(true, false, false);
                }

                MoveVector = new Vector3(anim.GetFloat("Horizontal"), 0, anim.GetFloat("Vertical"));

                angleBetweenCharacterAndCamera = Helper.AngleBetween(transform.TransformDirection(new Vector3(-directionVector.x, directionVector.y, directionVector.z)), thisCamera.transform.forward);

                anim.SetFloat("Angle", angleBetweenCharacterAndCamera);

                if (!hasMoveButtonPressed)
                {
                    anim.SetBool("Move", false);
                }
                else
                {
                    if (MovementType == CharacterHelper.MovementType.Realistic)
                    {
                        if (Mathf.Abs(MoveVector.x) > 0.5f || Math.Abs(MoveVector.z) > 0.5f)
                        {
                            anim.SetBool("MoveButtonHasPressed", false);
                            //						pressButtonTimeout = 0;
                            anim.SetBool("Move", true);
                            clickMoveButton = false;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(MoveVector.x) > 0.2f || Math.Abs(MoveVector.z) > 0.2f)
                        {
                            anim.SetBool("MoveButtonHasPressed", false);
                            anim.SetBool("Move", true);
                            //						pressButtonTimeout = 0;
                            clickMoveButton = false;
                        }
                    }
                }

                //			if (clickMoveButton)
                //			{
                //				pressButtonTimeout += Time.deltaTime;
                //			}

                if (SmoothCameraWhenMoving)
                {
                    if (hasMoveButtonPressed)
                    {
                        if (!isSprint && !isCrouch)
                            CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 6, 3 * Time.deltaTime);
                        else if (isSprint)
                            CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 7, 3 * Time.deltaTime);
                        else if (isCrouch)
                            CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 6, 3 * Time.deltaTime);
                    }
                    else
                    {
                        if (!isCrouch && !isSprint)
                            CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 5, 3 * Time.deltaTime);

                        else if (isCrouch && !CameraController.CameraAim)
                        {
                            CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 5, 3 * Time.deltaTime);
                        }
                        else if (isCrouch && CameraController.CameraAim)
                        {
                            CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 5, 3 * Time.deltaTime);
                        }
                        else if (isSprint) CameraController.cameraMovementDistance = Mathf.Lerp(CameraController.cameraMovementDistance, 5.5f, 3 * Time.deltaTime);
                    }
                }


                CurrentMoveDirection();
            }
        }

        public void GetDamageInBodyColliders()
        {
            var getAnyDamage = false;

            foreach (var bodyPart in BodyParts)
            {
                var bodyColliderScript = bodyPart.gameObject.GetComponent<BodyPartCollider>();

                if (bodyColliderScript.gettingDamage)
                {
                    if (bodyColliderScript.attackType == "Fire")
                    {
                        if (!bodyColliderScript) return;
                        if (bodyColliderScript.attacking.GetComponent<EnemyController>())
                        {
                            PlayerHealth -= bodyColliderScript.attacking.GetComponent<EnemyController>().Attacks[0].Damage * Time.deltaTime;

                            if (PlayerHealth <= 0)
                            {
                                KillerName = "Enemy";
                            }

                            break;
                        }
                        else if (bodyColliderScript.attacking.GetComponent<Controller>())
                        {
                            if (bodyColliderScript.attacking.GetComponent<Controller>().gameObject.GetInstanceID() == gameObject.GetInstanceID()) return;

                            switch (CanKillOthers)
                            {
                                case PUNHelper.CanKillOthers.OnlyOpponents:
                                    if (MyTeam == bodyColliderScript.attacking.GetComponent<Controller>().MyTeam && MyTeam != PUNHelper.Teams.Null)
                                        return;
                                    break;
                                case PUNHelper.CanKillOthers.Everyone:
                                    break;
                                case PUNHelper.CanKillOthers.NoOne:
                                    return;
                            }

                            var weaponController = bodyColliderScript.attacking.GetComponent<Controller>().WeaponManager.WeaponController;

                            var deltaTime = Time.deltaTime;

#if PHOTON_UNITY_NETWORKING
                            if (CharacterSync)
                                CharacterSync.UpdateKillAssists(weaponController.Controller.CharacterName);
#endif
                            if (PlayerHealth - weaponController.Attacks[weaponController.currentAttack].weapon_damage * deltaTime <= 0 && !bodyColliderScript.registerDeath)
                            {
#if PHOTON_UNITY_NETWORKING
                                if (weaponController.Controller.CharacterSync)
                                    weaponController.Controller.CharacterSync.AddScore(PlayerPrefs.GetInt("FireKill"), "fire");
#endif

                                KillerName = weaponController.Controller.CharacterName;
                                KilledWeaponImage = (Texture2D)weaponController.WeaponImage;

#if PHOTON_UNITY_NETWORKING
                                if (CharacterSync)
                                    CharacterSync.AddScoreToAssistants();
#endif

                                bodyColliderScript.registerDeath = true;
                            }

                            PlayerHealth -= weaponController.Attacks[weaponController.currentAttack].weapon_damage * deltaTime;

                            break;
                        }
                    }
                    else if (bodyColliderScript.attackType == "Melee")
                    {
                        getAnyDamage = true;

                        if (!meleeDamage)
                        {
                            if (bodyColliderScript.attacking.GetComponent<EnemyController>())
                            {
                                PlayerHealth -= bodyColliderScript.attacking.GetComponent<EnemyController>().Attacks[0].Damage;
                                if (PlayerHealth <= 0)
                                {
                                    KillerName = "Enemy";
                                }

                                meleeDamage = true;

                                break;
                            }
                            else if (bodyColliderScript.attacking.GetComponent<Controller>())
                            {
                                if (bodyColliderScript.attacking.GetComponent<Controller>().gameObject.GetInstanceID() == gameObject.GetInstanceID()) return;

                                switch (CanKillOthers)
                                {
                                    case PUNHelper.CanKillOthers.OnlyOpponents:
                                        if (MyTeam == bodyColliderScript.attacking.gameObject.GetComponent<Controller>().MyTeam && MyTeam != PUNHelper.Teams.Null)
                                            return;

                                        break;
                                    case PUNHelper.CanKillOthers.Everyone:
                                        break;
                                    case PUNHelper.CanKillOthers.NoOne:
                                        return;
                                }

                                var inventoryManager = bodyColliderScript.attacking.GetComponent<Controller>().WeaponManager;
                                var damage = 0;
                                var hasWeapon = false;

                                if (inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame.Count > 0)
                                {
                                    if (inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].fistAttack)
                                    {
                                        damage = (int)inventoryManager.FistDamage;
                                    }
                                    else if (inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].weapon)
                                    {
                                        var weaponController = inventoryManager.WeaponController;
                                        damage = weaponController.Attacks[weaponController.currentAttack].weapon_damage;
                                        hasWeapon = true;
                                    }
                                }

                                if (oneShotOneKill)
                                    damage = (int)PlayerHealth + 50;

#if PHOTON_UNITY_NETWORKING
                                if (CharacterSync)
                                    CharacterSync.UpdateKillAssists(inventoryManager.Controller.CharacterName);
#endif

                                if (PlayerHealth - damage <= 0)
                                {
#if PHOTON_UNITY_NETWORKING
                                    if (inventoryManager.Controller.CharacterSync)
                                        inventoryManager.Controller.CharacterSync.AddScore(PlayerPrefs.GetInt("MeleeKill"), "melee");
#endif

                                    KillerName = bodyColliderScript.attacking.GetComponent<Controller>().CharacterName;

                                    if (hasWeapon && inventoryManager.WeaponController.WeaponImage)
                                        KilledWeaponImage = (Texture2D)inventoryManager.WeaponController.WeaponImage;
                                    else
                                    {
                                        if (inventoryManager.FistIcon)
                                            KilledWeaponImage = (Texture2D)inventoryManager.FistIcon;
                                    }

#if PHOTON_UNITY_NETWORKING
                                    if (CharacterSync)
                                        CharacterSync.AddScoreToAssistants();
#endif
                                }

                                PlayerHealth -= damage;

                                meleeDamage = true;

                                break;
                            }
                        }
                    }
                }
            }

            if (!getAnyDamage)
                meleeDamage = false;
        }

        void CheckHealth()
        {
            if (UIManager.CharacterUI.Health)
            {
                UIManager.CharacterUI.Health.text = PlayerHealth < 0 ? "0" : PlayerHealth.ToString("F0");
            }

            if (UIManager.CharacterUI.bloodSplatter)
            {
                if (!UIManager.CharacterUI.bloodSplatter.gameObject.activeSelf)
                    UIManager.CharacterUI.bloodSplatter.gameObject.SetActive(true);

                if (PlayerHealth < 40)
                {
                    var healthPercentage = 100 - (PlayerHealth / 30 * 100);

                    UIManager.CharacterUI.bloodSplatter.color = new Color(1, 1, 1, healthPercentage / 100);
                }
                else
                {
                    UIManager.CharacterUI.bloodSplatter.color = new Color(1, 1, 1, 0);
                }
            }

            if (UIManager.CharacterUI.HealthBar)
            {
                if (PlayerHealth >= 75)
                    UIManager.CharacterUI.HealthBar.color = Color.green;
                if (PlayerHealth >= 50 & PlayerHealth < 75)
                    UIManager.CharacterUI.HealthBar.color = Color.yellow;
                if (PlayerHealth >= 25 & PlayerHealth < 50)
                    UIManager.CharacterUI.HealthBar.color = new Color32(255, 140, 0, 255);
                if (PlayerHealth < 25)
                    UIManager.CharacterUI.HealthBar.color = Color.red;

                UIManager.CharacterUI.HealthBar.fillAmount = PlayerHealth / healthPercent;
            }

            if (PlayerHealth <= 0)
            {
                foreach (var part in BodyParts)
                {
                    part.GetComponent<Rigidbody>().isKinematic = false;
                }

                for (int i = 0; i < 8; i++)
                {

                    switch (UIManager.CharacterUI.Inventory.WeaponsButtons[i].transition)
                    {
                        case Selectable.Transition.ColorTint:

                            var colorBlock = UIManager.CharacterUI.Inventory.WeaponsButtons[i].colors;
                            colorBlock.normalColor = UIManager.CharacterUI.Inventory.normButtonsColors[i];
                            UIManager.CharacterUI.Inventory.WeaponsButtons[i].colors = colorBlock;
                            break;

                        case Selectable.Transition.SpriteSwap:
                            UIManager.CharacterUI.Inventory.WeaponsButtons[i].GetComponent<Image>().sprite = UIManager.CharacterUI.Inventory.normButtonsSprites[i];
                            break;
                    }
                }

                UIManager.CharacterUI.DisableAll();

                UIManager.CharacterUI.PickupHUD.gameObject.SetActive(false);
                UIManager.CharacterUI.PickupEggHUD.gameObject.SetActive(false);

                anim.enabled = false;
                WeaponManager.enabled = false;
                enabled = false;

                WeaponManager.StopAllCoroutines();

                Helper.CameraExtensions.LayerCullingShow(CameraController.Camera, "Head");

                if (WeaponManager.WeaponController)
                    Destroy(WeaponManager.WeaponController.gameObject);


                var enemies = FindObjectsOfType<EnemyController>();
                foreach (var enemy in enemies)
                {
                    enemy.Players.Clear();
                }


                if (CharacterSync)
                {
#if PHOTON_UNITY_NETWORKING
                    CharacterSync.Destroy();
#endif
                }
                else
                {
                    CameraController.enabled = false;
                }

                if (GetComponent<EggBag>())
                {
                    GetComponent<EggBag>().DropEgg();
                }

                if (!Application.isMobilePlatform && !projectSettings.mobileDebug)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }


        void ProcessMotion()
        {
            if (isPause)
                return;

            if (TypeOfCamera != CharacterHelper.CameraType.TopDown)
            {
                MoveVector = !isJump ? transform.TransformDirection(MoveVector) : thisCamera.transform.TransformDirection(MoveVector);
            }
            else
            {
                if (!CameraParameters.lockCamera)
                    MoveVector = transform.TransformDirection(MoveVector);
            }

            if (TypeOfCamera != CharacterHelper.CameraType.TopDown)
            {
                CheckCollisionVector = thisCamera.transform.TransformDirection(CheckCollisionVector);
            }
            else
            {
                if (!CameraParameters.lockCamera)
                    CheckCollisionVector = transform.TransformDirection(CheckCollisionVector);

            }


            if (MoveVector.magnitude > 1)
                MoveVector = MoveVector.normalized;


            //			if (CheckCollisionVector.magnitude > 1)
            CheckCollisionVector = CheckCollisionVector.normalized;

            if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson || MovementType == CharacterHelper.MovementType.FastAndAccurate)
            {
                float speed = 0;

                switch (TypeOfCamera)
                {
                    case CharacterHelper.CameraType.ThirdPerson:
                        speed = MoveSpeed(TPSpeed);
                        break;
                    case CharacterHelper.CameraType.FirstPerson:
                        speed = MoveSpeed(FPSpeed);
                        break;
                    case CharacterHelper.CameraType.TopDown:
                        speed = MoveSpeed(TDSpeed);
                        break;
                }

                CurrentSpeed = MoveDirection == Direction.Stationary ? Mathf.Lerp(CurrentSpeed, speed, 0.5f * Time.deltaTime) : Mathf.Lerp(CurrentSpeed, speed, 3 * Time.deltaTime);

                if (!onGround || isJump)
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, 2, 3 * Time.deltaTime);
                }

                if (CurrentSpeed < 0)
                    CurrentSpeed = 0;
            }

            var checkCollisionPoint1 = Vector3.zero;
            var checkCollisionPoint2 = Vector3.zero;
            Vector3 checkDir;
            RaycastHit hit;
            RaycastHit hit2;

            if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out hit, 100, (Helper.layerMask())))
			{
				if(Physics.Raycast(BodyObjects.Hips.position + BodyObjects.Hips.forward, Vector3.down, out hit2, 100, Helper.layerMask()))
				{
					checkCollisionPoint2 = hit2.point + hit2.normal * 2;
				}
				else
				{
					checkCollisionPoint2 = hit.point + hit.normal * 2;
				}
				checkCollisionPoint1 = hit.point + hit.normal * 2;
			}

			checkDir = checkCollisionPoint2 - checkCollisionPoint1;
			
//			var rightDirection = Vector3.Cross(CheckCollisionVector, Vector3.up);
//			var leftDirection = -rightDirection;

//			Debug.DrawRay(BodyObjects.Head.position,rightDirection * 10, Color.red);
//
			Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + headHeight, transform.position.z),new Vector3(CheckCollisionVector.x, checkDir.y, CheckCollisionVector.z) * 10, Color.green);
			
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + headHeight, transform.position.z), new Vector3(CheckCollisionVector.x, checkDir.y, CheckCollisionVector.z), out distanceInfo, CheckCollisionVector.magnitude * 10, Helper.layerMask()))
			{
				if (!distanceInfo.collider || distanceInfo.collider && !distanceInfo.collider.isTrigger)
				{
					if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
					{
						if (distanceInfo.distance < 1)
						{
							CurrentSpeed = 0;
							isObstacle = true;
//							canMoveTimer = 0;
						}
						else
						{
//							canMoveTimer += Time.deltaTime;
//							if (canMoveTimer > 0.1f) 
								isObstacle = false;
						}
					}
					else
					{
						if (distanceInfo.distance < (!isSprint ? 1 : 3))
						{
//							canMoveTimer = 0;
							isObstacle = true;
						}
						else if (distanceInfo.distance > (!isSprint ? 1.35f : 3.35f))
						{
							isObstacle = false;
						}
					}
				}
			}
			else
			{
				if (gameObject.activeSelf)
				{
//					canMoveTimer += Time.deltaTime;
//					if (canMoveTimer > 0.1f) 
						isObstacle = false;
						
//						if(directionVector.magnitude > 0)
//							anim.SetBool("Move", true);
				}
//					StartCoroutine(MovePause());
			}


			MoveVector = new Vector3(MoveVector.x * CurrentSpeed, 0, MoveVector.z * CurrentSpeed);

			if (!isObstacle)
			{
				switch (TypeOfCamera)
				{
					case CharacterHelper.CameraType.ThirdPerson:
						if(!isCrouch && (anim.GetBool("Aim") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Melee") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch Melee") && !anim.GetBool("Pause") || isJump))
							CharacterController.Move(new Vector3(MoveVector.x, 0, MoveVector.z) * Time.deltaTime);
						break;
					case CharacterHelper.CameraType.FirstPerson:
						CharacterController.Move(new Vector3(MoveVector.x, 0, MoveVector.z) * Time.deltaTime);
						break;
					case CharacterHelper.CameraType.TopDown:
						if(MovementType == CharacterHelper.MovementType.FastAndAccurate && !anim.GetCurrentAnimatorStateInfo(0).IsName("Grenade_Throw"))
							CharacterController.Move(new Vector3(MoveVector.x, 0, MoveVector.z) * Time.deltaTime);
						break;
				}

				CanMove = true;
			}
			else
			{
				anim.SetBool("Move", false);
				CanMove = false;
			}

			CharacterController.Move(new Vector3(0, currentGravity, 0) * Time.deltaTime);
		}

		public void Jump()
		{
			if (isCrouch || isJump || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !anim.GetBool("HasWeaponTaken"))
				return;

			if (activeJump & !isPause && onGround)
			{
				if (Physics.Raycast(BodyObjects.Head.position, Vector3.up, out HeightInfo, 100, Helper.layerMask()))
				{
					if (HeightInfo.distance <= headHeight + 2)
					{
						newJumpHeight = HeightInfo.distance - 1;

						if (newJumpHeight < 2)
						{
							Debug.Log("Your character has not jump because the ceiling is too low.");
							return;
						}
						isCeiling = true;
					}
				}
				
				isJump = true;

				if (TypeOfCamera != CharacterHelper.CameraType.FirstPerson && MovementType == CharacterHelper.MovementType.Realistic)
				{
					currentGravity = 0;
					anim.SetBool("Jump", true);
					StartCoroutine("JumpTimeout");
				}
			}
		}

		IEnumerator JumpTimeout()
		{
			yield return new WaitForSeconds(1);
			anim.SetBool("Jump", false);
			currentGravity = defaultGravity;
			isJump = false;
			StopCoroutine("JumpTimeout");
		}

		IEnumerator CrouchTimeout()
		{
			yield return new WaitForSeconds(1);
			crouchTimeOut = true;
			StopCoroutine("CrouchTimeout");
		}

		#region JumpingProcess

		void JumpingProcess()
		{
			if (isJump && !startJumping)
			{
				SmoothIKSwitch = 0;

				anim.SetBool("Jump", true);

				if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out HeightInfo, 100, Helper.layerMask()))
				{
					defaultDistance = HeightInfo.distance;
					setDefaultDistance = true;
				}

				if (!isCeiling)
				{
					switch (TypeOfCamera)
					{
						case CharacterHelper.CameraType.ThirdPerson:
							JumpPosition = transform.position.y + TPSpeed.JumpHeight;
							break;
						case CharacterHelper.CameraType.FirstPerson:
							JumpPosition = transform.position.y + FPSpeed.JumpHeight;
							break;
						case CharacterHelper.CameraType.TopDown:
							JumpPosition = transform.position.y + TDSpeed.JumpHeight;
							break;
					}
				}
				else JumpPosition = transform.position.y + newJumpHeight;
				
				flyingUp = true;
				startJumping = true;
			}

			if (startJumping && flyingUp && !flyingDown)
			{
				currentGravity = 0;
				
				var jumpPos = new Vector3(transform.position.x, JumpPosition, transform.position.z);
				var speed = 0f;
				
				switch (TypeOfCamera)
				{
					case CharacterHelper.CameraType.ThirdPerson:
						speed = TPSpeed.JumpSpeed;
						break;
					case CharacterHelper.CameraType.FirstPerson:
						speed = FPSpeed.JumpSpeed;
						break;
					case CharacterHelper.CameraType.TopDown:
						speed = TDSpeed.JumpSpeed;
						break;
				}
				
				transform.position = Vector3.Lerp(transform.position, jumpPos, 0.1f * speed * Time.deltaTime);

				if (Math.Abs(transform.position.y - JumpPosition) < 0.1f)
				{
					flyingDown = true;
					flyingUp = false;
				}
			}

			if (flyingDown)
			{
				currentGravity = Mathf.Lerp(currentGravity, defaultGravity, 1 * Time.deltaTime);
				
				if (Math.Abs(currentGravity - defaultGravity) < 0.5f)
				{
					currentGravity = defaultGravity;
				}
			}

			if (setDefaultDistance)
			{
				if (onGround)
				{
					if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out HeightInfo, 100, Helper.layerMask()))
					{
						if (Math.Abs(HeightInfo.distance - defaultDistance) > 0.5f)
						{
							if (isJump)
							{
								if (!startJumping || !flyingDown)
									return;
							}

							onGround = false;
//							SmoothIKSwitch = 0;
							anim.SetBool("Jump", true);
						}
					}
					else
					{
						onGround = false;
//						SmoothIKSwitch = 0;

						anim.SetBool("Jump", true);
					}
				}
				else
				{
					if (Physics.Raycast(BodyObjects.Hips.position, Vector3.down, out HeightInfo, 100, Helper.layerMask()))
					{
						if (DebugMode)
						{
							defaultDistance = HeightInfo.distance;
							anim.SetBool("Jump", false);
						}
						else
						{
							if (Math.Abs(HeightInfo.distance - defaultDistance) < 0.1f)
							{
								anim.SetBool("Jump", false);
								SmoothIKSwitch = 1;
								onGround = true;
								startJumping = false;
								flyingUp = false;
								flyingDown = false;
								isCeiling = false;
								isJump = false;
							}
						}
					}
				}
			}
		}

		#endregion

		public void Sprint(bool active, string type)
		{
			if (!isPause && !isJump) //&& !isCrouch)
			{
				if (type == "press")
				{
					if(isCrouch)
						DeactivateCrouch();
					
					if (active)
						ActivateSprint();
					else
						DeactivateSprint();
				}
				else
				{
					if(isCrouch)
						DeactivateCrouch();
					
					if(!isSprint)
						ActivateSprint();
					else
						DeactivateSprint();
				}
			}
		}

		void DeactivateSprint()
		{
			isSprint = false;
			anim.SetBool("Sprint", false);
		}
		
		void ActivateSprint()
		{
			isSprint = true;
			anim.SetBool("Sprint", true);
		}

		public void Crouch(bool active, string type)
		{
			if (!activeCrouch || isPause || isJump || TypeOfCamera == CharacterHelper.CameraType.TopDown ||
			    TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !anim.GetBool("HasWeaponTaken")) return;
			
			
			if (type == "press")
			{
				if (active)
					ActivateCrouch();
				else
					DeactivateCrouch();
			}
			else
			{
				if (!isCrouch)
					ActivateCrouch();
				else
					DeactivateCrouch();
			}
		}

		public void ActivateCrouch()
		{
			if (!isMultiplayerCharacter)
			{
				if (isSprint) Sprint(false, "press");
				
				anim.SetBool("Crouch", true);

				if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					defaultCharacterCenter += CrouchHeight;
					StartCoroutine("ChangeBodyHeight");
				}
				else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				{
					if (WeaponManager.hasAnyWeapon && !isCrouch)
						WeaponManager.WeaponController.CrouchHands();
					
					multiplayerCrouch = true;
				}
				isCrouch = true;
			}
		}

		public void DeactivateCrouch()
		{

			if (!isMultiplayerCharacter)
			{
				RaycastHit hit;
				RaycastHit hit2;

				if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, 100, Helper.layerMask()))
				{
					if (Physics.Raycast(BodyObjects.Hips.transform.position, Vector3.down, out hit2, 100, Helper.layerMask()))
					{
						if (hit.point.y - 1 - hit2.point.y < headHeight * 1.5f)
						{
							return;
						}
					}
				}

				deactivateCrouch = true;
				activateCrouch = false;

				if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					defaultCharacterCenter -= CrouchHeight;
					StartCoroutine("ChangeBodyHeight");
				}
				else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				{
					if (WeaponManager.hasAnyWeapon && isCrouch)
						WeaponManager.WeaponController.CrouchHands();
					
					multiplayerCrouch = true;
				}
				
				anim.SetBool("Crouch", false);
				isCrouch = false;
			}
		}

		public void ChangeCameraType()
		{
			if(!AdjustmentScene && isPause || isJump || anim.GetBool("Pause") || WeaponManager.WeaponController && WeaponManager.WeaponController.isReloadEnabled)
				return;
			
			if(changeCameraTypeTimeout <= 1 || WeaponManager.WeaponController && !WeaponManager.WeaponController.setHandsPositionsAim)
				return;
			
			if(CameraController.deepAim)
				CameraController.deepAim = false;

			if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson && CameraParameters.activeTD)
			{
				if (WeaponManager.WeaponController)
				{
					if (WeaponManager.WeaponController && (CameraParameters.lockCamera || CameraParameters.alwaysTDAim) && WeaponManager.WeaponController.isAimEnabled)
					{
						WeaponManager.WeaponController.Aim(true, false, false);
						StartCoroutine(ChangeCameraTimeout(CharacterHelper.CameraType.TopDown));
					}
					else
					{
						CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
					}
				}
				else 
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson && CameraParameters.activeTP)
			{
//				if (WeaponManager.WeaponController && !WeaponManager.WeaponController.isAimEnabled || WeaponManager.WeaponController && WeaponManager.WeaponController.isAimEnabled && WeaponManager.WeaponController.activeAimTP)
					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
//				else if(WeaponManager.WeaponController && !WeaponManager.WeaponController.activeAimTP && WeaponManager.WeaponController.isAimEnabled)
//				{
//					WeaponManager.WeaponController.Aim(true, false, false);
//					StartCoroutine(ChangeCameraTimeout(CharacterHelper.CameraType.ThirdPerson));
//				}
//				else if(!WeaponManager.WeaponController)
//					CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
				
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraParameters.activeFP)
			{
//				if (WeaponManager.WeaponController)
//				{
//					if (!WeaponManager.WeaponController.isAimEnabled || WeaponManager.WeaponController && WeaponManager.WeaponController.isAimEnabled && WeaponManager.WeaponController.activeAimFP)
//					{
						CharacterHelper.SwitchCamera(TypeOfCamera, tdModeLikeTp ? CharacterHelper.CameraType.ThirdPerson : CharacterHelper.CameraType.FirstPerson, this);
//					}
//					else if (!WeaponManager.WeaponController.activeAimFP && WeaponManager.WeaponController.isAimEnabled)
//					{
//						WeaponManager.WeaponController.Aim(true, false, false);
//						StartCoroutine(ChangeCameraTimeout(CharacterHelper.CameraType.FirstPerson));
//					}
//				}
//				else
//				{
//					CharacterHelper.SwitchCamera(TypeOfCamera, tdModeLikeTp ? CharacterHelper.CameraType.ThirdPerson : CharacterHelper.CameraType.FirstPerson, this);
//				}
				
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraParameters.activeTD)
			{
				if (WeaponManager.WeaponController)
				{
					if (WeaponManager.WeaponController && (CameraParameters.lockCamera || CameraParameters.alwaysTDAim) && WeaponManager.WeaponController.isAimEnabled)
					{
						WeaponManager.WeaponController.Aim(true, false, false);
						StartCoroutine("ChangeCameraTimeout");
					}
					else CharacterHelper.SwitchCamera(TypeOfCamera, tdModeLikeTp ? CharacterHelper.CameraType.ThirdPerson : CharacterHelper.CameraType.TopDown, this);

				}
				else CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.TopDown, this);
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.TopDown && CameraParameters.activeTP)
			{
				CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
			}
			else if (TypeOfCamera == CharacterHelper.CameraType.TopDown && CameraParameters.activeFP)
			{
				CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.FirstPerson, this);
			}
			
			changeCameraTypeTimeout = 0;
		}

		public void ChangeCameraType(CharacterHelper.CameraType type)
		{
			if(!AdjustmentScene && isPause)
				return;
			
			if(changeCameraTypeTimeout <= 0.5f || WeaponManager.WeaponController && !WeaponManager.WeaponController.setHandsPositionsAim)
				return;
			
			changeCameraTypeTimeout = 0;
			
			CharacterHelper.SwitchCamera(TypeOfCamera, type, this);
		}

		public void SnapAlignCharacterWithCamera()
		{
			if (!anim.GetBool("Aim") && (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Forward") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Forward_Start") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Start_90_L") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Start_90_R") ||
			                            //|| anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Start_180_L") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Start_180_R") || 
			                             anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Forward") || anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Start") ||
										anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Start_90_L") || anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Start_90_R")) ||
										anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Walk_Forward"))
			{
				var angle = angleBetweenCharacterAndCamera;

				var _angle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);

				var speed = 3;

				if (Mathf.Abs(angle) < 135)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, _angle, speed * Time.deltaTime);
				}
			}
			else if ((Math.Abs(MoveVector.x) > 0.8f || Math.Abs(MoveVector.z) > 0.8f) && (anim.GetBool("Aim") || TypeOfCamera == CharacterHelper.CameraType.FirstPerson || TypeOfCamera == CharacterHelper.CameraType.TopDown && !CameraParameters.lockCamera))
			{
				var angle = Mathf.DeltaAngle(transform.eulerAngles.y, thisCamera.transform.parent.eulerAngles.y);
				
				var _angle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);

				var speed = 5;
				transform.rotation = Quaternion.Slerp(transform.rotation, _angle, speed * Time.deltaTime);

			}
			else if (MovementType == CharacterHelper.MovementType.FastAndAccurate && (anim.GetBool("Aim") || TypeOfCamera == CharacterHelper.CameraType.TopDown || anim.GetCurrentAnimatorStateInfo(0).IsName("Falling Loop")))
			{
				var directionAngle = Helper.AngleBetween(transform.forward, CameraController.BodyLookAt.position - transform.position);
				
				var angle = Mathf.DeltaAngle(transform.eulerAngles.y, thisCamera.transform.parent.eulerAngles.y);
				
				var _angle = TypeOfCamera == CharacterHelper.CameraType.TopDown
					? Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (!CameraParameters.lockCamera ? angle : directionAngle), transform.eulerAngles.z)
					: Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);

				transform.rotation = Quaternion.Slerp(transform.rotation, _angle, 5 * Time.deltaTime);

			}
			else if (MovementType == CharacterHelper.MovementType.Realistic && TypeOfCamera == CharacterHelper.CameraType.TopDown && CameraParameters.lockCamera)
			{
				var directionAngle = Helper.AngleBetween(transform.forward, CameraController.BodyLookAt.position - transform.position);
				var _angle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + directionAngle, transform.eulerAngles.z);
				
				transform.rotation = Quaternion.Slerp(transform.rotation, _angle, 3 * Time.deltaTime);
			}

			CurrentRotation = transform.rotation;
		}

		public void BodyLookAt(Transform bodyLookAt)
		{
            /*

			if (!isMultiplayerCharacter)
			{
				if (WeaponManager.WeaponController)
				{
					if (!WeaponManager.hasAnyWeapon || TypeOfCamera != CharacterHelper.CameraType.TopDown &&
					    (!anim.GetBool("Aim") || anim.GetBool("Aim") && WeaponManager.WeaponController.Attacks[WeaponManager.WeaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Melee)
						 || isCrouch && !anim.GetBool("Aim") &&
					    !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Aim_Idle") &&
					    !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Aim_Turn_90_L") &&
					    !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Aim_Turn_90_R"))
					{
						var speed = isCrouch ? 10 : 3;

						bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 0, speed * Time.deltaTime);
						bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, 0, speed * Time.deltaTime);

						bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, 0, speed * Time.deltaTime);
						bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, 0, speed * Time.deltaTime);

					}
					else
					{
						var minLimit = TypeOfCamera != CharacterHelper.CameraType.TopDown ? CameraParameters.fpXLimitMin : CameraParameters.tdXLimitMin;
						var maxLimit = TypeOfCamera != CharacterHelper.CameraType.TopDown ? CameraParameters.fpXLimitMax : CameraParameters.tdXLimitMax;
						
						if (Math.Abs(anim.GetFloat("CameraAngle")) < 45)
						{
							bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 60, 3 * Time.deltaTime);
							bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, -60, 3 * Time.deltaTime);

							bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, maxLimit + 30, 3 * Time.deltaTime);
							bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, minLimit, 3 * Time.deltaTime);
						}
						else
						{
							bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 60, 1 * Time.deltaTime);
							bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, -60, 1 * Time.deltaTime);

							bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, maxLimit + 30, 1 * Time.deltaTime);
							bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, minLimit, 1 * Time.deltaTime);
						}
					}
				}
				else
				{
					bodyRotationUpLimit_y = Mathf.Lerp(bodyRotationUpLimit_y, 0, 3 * Time.deltaTime);
					bodyRotationDownLimit_y = Mathf.Lerp(bodyRotationDownLimit_y, 0, 3 * Time.deltaTime);

					bodyRotationUpLimit_x = Mathf.Lerp(bodyRotationUpLimit_x, 0, 3 * Time.deltaTime);
					bodyRotationDownLimit_x = Mathf.Lerp(bodyRotationDownLimit_x, 0, 3 * Time.deltaTime);
				}
			}

			var direction = bodyLookAt.position - DirectionObject.position;

            
			var middleAngleX = Helper.AngleBetween(direction, DirectionObject).x;
			var middleAngleY = Helper.AngleBetween(direction, DirectionObject).y;

			if (middleAngleY > bodyRotationUpLimit_y)
				middleAngleY = bodyRotationUpLimit_y;
			else if (middleAngleY < bodyRotationDownLimit_y)
				middleAngleY = bodyRotationDownLimit_y;

			if (middleAngleX > bodyRotationUpLimit_x)
			{
				middleAngleX = bodyRotationUpLimit_x;
				bodyLimit = true;
			}
			else if (middleAngleX < bodyRotationDownLimit_x)
			{
				middleAngleX = bodyRotationDownLimit_x;
				bodyLimit = true;
			}
			else
			{
				bodyLimit = false;
			}

			if (AdjustmentScene) return;
			
			if (!isCrouch)
			{
				BodyObjects.TopBody.RotateAround(DirectionObject.position, Vector3.up, -middleAngleY);
				BodyObjects.TopBody.RotateAround(DirectionObject.position, DirectionObject.TransformDirection(Vector3.right), -middleAngleX);
			}
			else
			{
				BodyObjects.TopBody.RotateAround(DirectionObject.position, Vector3.up, -middleAngleY);
				BodyObjects.TopBody.RotateAround(DirectionObject.position, DirectionObject.TransformDirection(Vector3.right), -middleAngleX);
			}

    */

		}

		public void TopBodyOffset()
		{

			if (!AdjustmentScene)
			{
				BodyObjects.TopBody.Rotate(Vector3.right, CharacterOffset.xRotationOffset);
				BodyObjects.TopBody.Rotate(Vector3.up, CharacterOffset.yRotationOffset);
				BodyObjects.TopBody.Rotate(Vector3.forward, CharacterOffset.zRotationOffset);
			}
			else
			{
				BodyObjects.TopBody.eulerAngles = new Vector3(CharacterOffset.xRotationOffset, CharacterOffset.yRotationOffset, CharacterOffset.zRotationOffset);
			}
		}

		public void BodyRotate()
		{
            /*

			if (DebugMode) return;

			BodyLocalEulerAngles = BodyObjects.TopBody.localEulerAngles;

			if (BodyLocalEulerAngles.x > 180)
				BodyLocalEulerAngles.x -= 360;
			if (BodyLocalEulerAngles.y > 180)
				BodyLocalEulerAngles.y -= 360;
			
			var hipsAngleY = transform.eulerAngles.y;
			var spineAngleY = BodyObjects.TopBody.eulerAngles.y - CharacterOffset.yRotationOffset;
			var middleAngleY = Mathf.DeltaAngle(hipsAngleY, spineAngleY);

			if (TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
			{
				if (middleAngleY > 50)
				{
					RotationAngle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + (middleAngleY - 50), transform.eulerAngles.z);

					transform.rotation = Quaternion.Slerp(transform.rotation, RotationAngle, middleAngleY - 50 * Time.deltaTime);
				}

				else if (middleAngleY < -50)
				{
					RotationAngle = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - (-50 - middleAngleY), transform.eulerAngles.z);

					transform.rotation = Quaternion.Slerp(transform.rotation, RotationAngle, -middleAngleY - 50 * Time.deltaTime);
				}
			}

			hipsAngleX = transform.eulerAngles.x;
			spineAngleX = BodyObjects.TopBody.eulerAngles.x - CharacterOffset.xRotationOffset;
			middleAngleX = Mathf.DeltaAngle(hipsAngleX, spineAngleX);


//			if (-middleAngleX > CameraParameters.fps_MaxRotationX)
//			{
//				CameraController.maxMouseAbsolute = CameraController._mouseAbsolute.y;
//			}
//			else if (-middleAngleX < CameraParameters.fps_MinRotationX)
//			{
//				CameraController.minMouseAbsolute = CameraController._mouseAbsolute.y;
//			}
*/
		}
		

		private void CurrentMoveDirection()
		{
			var forward = false;
			var backward = false;
			var left = false;
			var right = false;
			
			NullDirectionAnimations();

			var newDirVector = Helper.AngleBetween(transform.forward, directionVector);
			
			if (TypeOfCamera != CharacterHelper.CameraType.TopDown || TypeOfCamera == CharacterHelper.CameraType.TopDown && !CameraParameters.lockCamera)
			{
				if (directionVector.z > 0)
					forward = true;
				if (directionVector.z < 0)
					backward = true;
				if (directionVector.x > 0)
					right = true;
				if (directionVector.x < 0)
					left = true;

				if (forward)
				{
					if (left)
					{
						MoveDirection = Direction.ForwardLeft;
						
						if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
							anim.SetBool("ForwardLeft", true);
					}
					else if (right)
					{
						MoveDirection = Direction.ForwardRight;
						
						if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
							anim.SetBool("ForwardRight", true);
					}
					else
					{
						MoveDirection = Direction.Forward;
						
						if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
							anim.SetBool("Forward", true);
					}
				}
				else if (backward)
				{
					if (left)
					{
						MoveDirection = Direction.BackwardLeft;
						
						if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
							anim.SetBool("BackwardLeft", true);
					}
					else if (right)
					{
						MoveDirection = Direction.BackwardRight;
						
						if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
							anim.SetBool("BackwardRight", true);
					}
					else
					{
						MoveDirection = Direction.Backward;
						
						if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
							anim.SetBool("Backward", true);
					}
				}
				else if (right)
				{
					MoveDirection = Direction.Right;
					
					if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim)
						anim.SetBool("Right", true);
				}
				else if (left)
				{
					MoveDirection = Direction.Left;
					
					if(TypeOfCamera != CharacterHelper.CameraType.ThirdPerson || TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraController.CameraAim) 
						anim.SetBool("Left", true);
				}
				else
				{
					MoveDirection = Direction.Stationary;
				}
			}
			else
			{
				if(Math.Abs(directionVector.x) < 0.01f && Math.Abs(directionVector.z) < 0.01f)
					MoveDirection = Direction.Stationary;
				else
				{
					if (newDirVector > -23 && newDirVector <= 23)
					{
						MoveDirection = Direction.Forward;
						anim.SetBool("Forward", true);
					}
					else if (newDirVector > 23 && newDirVector <= 68)
					{
						MoveDirection = Direction.ForwardRight;
						anim.SetBool("ForwardRight", true);
					}
					else if(newDirVector > 68 && newDirVector <= 113)
					{
						MoveDirection = Direction.Right;
						anim.SetBool("Right", true);
					}
					else if(newDirVector > 113 && newDirVector <= 158)
					{
						MoveDirection = Direction.BackwardRight;
						anim.SetBool("BackwardRight", true);
					}
					else if(newDirVector <= -23 && newDirVector > -68)
					{
						MoveDirection = Direction.ForwardLeft;
						anim.SetBool("ForwardLeft", true);
					}
					else if (newDirVector <= -68 && newDirVector > -113)
					{
						MoveDirection = Direction.Left;
						anim.SetBool("Left", true);
					}
					else if (newDirVector <= -113 && newDirVector > -158)
					{
						MoveDirection = Direction.BackwardLeft;
						anim.SetBool("BackwardLeft", true);
					}
					else
					{
						MoveDirection = Direction.Backward;
						anim.SetBool("Backward", true);
					}
				}
			}
		}

		void NullDirectionAnimations()
		{
			anim.SetBool("Forward", false);
			anim.SetBool("ForwardRight", false);
			anim.SetBool("ForwardLeft", false);
			anim.SetBool("Left", false);
			anim.SetBool("Right", false);
			anim.SetBool("BackwardLeft", false);
			anim.SetBool("BackwardRight", false);
			anim.SetBool("Backward", false);
		}

		float MoveSpeed(CharacterHelper.Speeds speeds)
		{
			var moveSpeed = 0f;

			switch (MoveDirection)
			{
				case Direction.Stationary:
					moveSpeed = 0;
					break;
				case Direction.Forward:
					moveSpeed = ChoiceSpeed(speeds.NormForwardSpeed, speeds.RunForwardSpeed, speeds.CrouchForwardSpeed);
					break;
				case Direction.Backward:
					moveSpeed = ChoiceSpeed(speeds.NormBackwardSpeed, speeds.RunBackwardSpeed, speeds.CrouchBackwardSpeed);
					break;
				case Direction.Right:
					moveSpeed = ChoiceSpeed(speeds.NormLateralSpeed, speeds.RunLateralSpeed, speeds.CrouchLateralSpeed);
					break;
				case Direction.Left:
					moveSpeed = ChoiceSpeed(speeds.NormLateralSpeed, speeds.RunLateralSpeed, speeds.CrouchLateralSpeed);
					break;
				case Direction.ForwardRight:
					moveSpeed = ChoiceSpeed(speeds.NormForwardSpeed, speeds.RunForwardSpeed, speeds.CrouchForwardSpeed);
					break;
				case Direction.ForwardLeft:
					moveSpeed = ChoiceSpeed(speeds.NormForwardSpeed, speeds.RunForwardSpeed, speeds.CrouchForwardSpeed);
					break;
				case Direction.BackwardRight:
					moveSpeed = ChoiceSpeed(speeds.NormBackwardSpeed, speeds.RunBackwardSpeed, speeds.CrouchBackwardSpeed);
					break;
				case Direction.BackwardLeft:
					moveSpeed = ChoiceSpeed(speeds.NormBackwardSpeed, speeds.RunBackwardSpeed, speeds.CrouchBackwardSpeed);
					break;
			}
			
			return moveSpeed / speedDevider;
		}

		float ChoiceSpeed(float norm, float run, float crouch)
		{
			float speed;
			
			if (isSprint)
				speed = run;
			else if (isCrouch)
				speed = crouch;
			else
				speed = norm;
			
			return speed;
		}

		IEnumerator ChangeCameraTimeout(CharacterHelper.CameraType type)
		{
			while (true)
			{
				if (WeaponManager.WeaponController.setHandsPositionsAim)
				{
					if(type == CharacterHelper.CameraType.TopDown && tdModeLikeTp)
						CharacterHelper.SwitchCamera(TypeOfCamera, CharacterHelper.CameraType.ThirdPerson, this);
					else CharacterHelper.SwitchCamera(TypeOfCamera, type, this);

					StopCoroutine("ChangeCameraTimeout");
					break;
				}

				yield return 0;
			}
		}

		public void PlayStepSound(float volume)
		{
			var hit = new RaycastHit();

			if (Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out hit, 100, Helper.layerMask()))
			{
				var surface = hit.collider.GetComponent<Surface>();
				
				if (FeetAudioSource && surface && surface.CharacterFootstepsSounds.Length > 0)
					CharacterHelper.PlayStepSound(surface, FeetAudioSource, characterTag, volume, "character");

			}
		}

		IEnumerator ChangeBodyHeight()
		{
			while (true)
			{
				var crouchHeight = Mathf.Lerp(CharacterController.center.y, defaultCharacterCenter, 5 * Time.deltaTime);
				CharacterController.center = new Vector3(CharacterController.center.x, crouchHeight, CharacterController.center.z);

				if (Math.Abs(crouchHeight - defaultCharacterCenter) < 0.1f && isCrouch)
				{
					CharacterController.center = new Vector3(CharacterController.center.x, defaultCharacterCenter, CharacterController.center.z);
					StopCoroutine("ChangeBodyHeight");
					break;
				}

				if (Math.Abs(crouchHeight - defaultCharacterCenter) < 0.1f && !isCrouch)
				{
					CharacterController.center = new Vector3(CharacterController.center.x, defaultCharacterCenter, CharacterController.center.z);
					StopCoroutine("ChangeBodyHeight");
					break;
				}

				yield return 0;
			}
		}
		
		#region HealthMethods

		public void Damage(int damage, string killerName, Texture WeaponImage, bool oneShot)
        {
	        if (killerName == "Enemy")
            {
                if (PlayerHealth - damage <= 0)
                {
                    KillerName = killerName;
                    if(WeaponImage) KilledWeaponImage = (Texture2D)WeaponImage;
                }

                PlayerHealth -= damage;
            }
            else
            {
	            if (oneShot)
		            damage = (int)PlayerHealth + 50;

#if PHOTON_UNITY_NETWORKING
	            if (CharacterSync)
		            CharacterSync.UpdateKillAssists(killerName);
#endif

	            if (PlayerHealth - damage <= 0)
                {
	                KillerName = killerName;
	                if(WeaponImage)KilledWeaponImage = (Texture2D)WeaponImage;
	                
#if PHOTON_UNITY_NETWORKING
	                if(CharacterSync)
		                CharacterSync.AddScoreToAssistants();
#endif
                }

                PlayerHealth -= damage;
            }
        }

        public void ExplosionDamage(int damage, string killerName, Texture WeaponImage, bool oneShot)
        {
	        if (killerName == "Enemy")
            {
                if (PlayerHealth - damage <= 0)
                {
                    KillerName = killerName;
                    if(WeaponImage) KilledWeaponImage = (Texture2D)WeaponImage;
                }

                PlayerHealth -= damage;
            }
            else
            {
	            if (oneShot)
		            damage = (int)PlayerHealth + 50;
	            
#if PHOTON_UNITY_NETWORKING
	            if (CharacterSync)
		            CharacterSync.UpdateKillAssists(killerName);
#endif

	            if (PlayerHealth - damage <= 0)
	            {
		            KillerName = killerName;
		            if(WeaponImage) KilledWeaponImage = (Texture2D)WeaponImage;
		            
#if PHOTON_UNITY_NETWORKING
		            if(CharacterSync)
			            CharacterSync.AddScoreToAssistants();
#endif
	            }
                    
	            PlayerHealth -= damage;
            }
        }

        #endregion
		
		#region FeetIK

		void OnAnimatorIK(int layerIndex)
		{
			if(isCharacterInLobby) return;
			
			IKVariables.LastPelvisPosition = anim.bodyPosition.y;
			
			if (layerIndex != 0) return;

			if (!isMultiplayerCharacter)
			{
				if (TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
				{
					if (isJump && (anim.GetCurrentAnimatorStateInfo(0).IsName("Jumple_Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Movemement_L")
					                                                                         || anim.GetCurrentAnimatorStateInfo(0).IsName("Falling Loop") ||
					                                                                         anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Land_Hard")
					                                                                         || anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Land_Walk Loop") ||
					                                                                         anim.GetCurrentAnimatorStateInfo(0).IsName("Start Falling")))
					{
						if (SmoothIKSwitch > 0.1f)
							SmoothIKSwitch = Mathf.Lerp(SmoothIKSwitch, 0, 5 * Time.deltaTime);
						else SmoothIKSwitch = 0;
					}
					else
					{
						if (SmoothIKSwitch < 0.9f)
							SmoothIKSwitch = Mathf.Lerp(SmoothIKSwitch, 0, 5 * Time.deltaTime);
						else SmoothIKSwitch = 1;
					}
				}
				else
				{
					if (!isJump)
						SmoothIKSwitch = 1;
				}
			}

			anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, SmoothIKSwitch);
			anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat("RightFoot"));

			IKHelper.MoveFeetToIkPoint(this, "right");

			anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, SmoothIKSwitch);
			anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("LeftFoot"));

			IKHelper.MoveFeetToIkPoint(this, "left");
		}

		void FixedUpdate()
		{
			if(isCharacterInLobby) return;

            IKHelper.AdjustFeetTarget(this, "right");
			IKHelper.AdjustFeetTarget(this, "left");

			IKHelper.FeetPositionSolver(this, "right");
			IKHelper.FeetPositionSolver(this, "left");
		}

		#endregion


#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if(AdjustmentScene)
				return;

			var position = BodyObjects.Hips.position;
			var up = transform.up;
			
			if (Application.isPlaying)
			{
				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 255, 255, 255);

				
				Handles.DrawWireDisc(position, up, noiseRadius);

				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 255, 255, 50);
				Handles.DrawWireDisc(position, up, noiseRadius);
			}
			else
			{
				Handles.zTest = CompareFunction.Less;
				Handles.color = new Color32(255, 255, 255, 255);
				Handles.DrawWireDisc(position, up, IdleNoise);

				Handles.zTest = CompareFunction.Greater;
				Handles.color = new Color32(255, 255, 255, 50);
				Handles.DrawWireDisc(position, up, IdleNoise);
			}
		}
#endif
	}
}


