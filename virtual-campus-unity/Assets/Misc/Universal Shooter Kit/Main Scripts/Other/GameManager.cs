// GercStudio
// © 2018-2020

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public List<SpawnZone> EnemiesSpawnZones;

        public List<Helper.EnemyInGameManager> Enemies = new List<Helper.EnemyInGameManager>();
        public List<Helper.CharacterInGameManager> Characters = new List<Helper.CharacterInGameManager>();

        public UIManager UIManager;
        public UIManager CurrentUIManager;
        
        public int inspectorTab;
        public int CurrentCharacter;
        public int LastCharacter;
        public int CurrentQuality;
        
        public List<Controller> controllers;

        public bool gameStarted;

        public List<InventoryManager> inventoryManager;
        public List<CameraController> cameraController;

        private bool selectMenuItemWithGamepad;
        private bool isPause;
        private bool isOptions;
        private bool cameraFlyingStep1;
        private bool cameraFlyingStep2;
        private bool switchingCamera;
        private bool gamepadInput;
        private int _quantity;
        private int currentMenuItem;
        private int currentOptionsMenuItem;

        public ProjectSettings projectSettings;

        private Camera cameraForSwitching;
        public Camera defaultCamera;
        
        private Color[] normButtonsColors = new Color[6];
        private Sprite[] normButtonsSprites = new Sprite[6];

        private float _timeout;

        private bool setbutton;

        void Awake()
        {
            if (UIManager)
            {
                CurrentUIManager = Instantiate(UIManager.gameObject).GetComponent<UIManager>();
            }
            else
            {
                Debug.LogError("UI Manager was not be loaded.");
            }

            CurrentUIManager.HideAllMultiplayerRoomUI();
            
            CurrentUIManager.CharacterUI.Inventory.ActivateAll();
            
            CurrentUIManager.CharacterUI.DisableAll();
            
            CurrentUIManager.CharacterUI.ActivateAll();

            gameStarted = true;
            
            cameraForSwitching = Helper.NewCamera("Camera for switching", transform, "GameManager");
            Destroy(cameraForSwitching.GetComponent<AudioListener>());

            if(defaultCamera)
                defaultCamera.gameObject.SetActive(false);
            
            cameraForSwitching.gameObject.SetActive(false);

            CurrentQuality = QualitySettings.GetQualityLevel();

#if UNITY_EDITOR
            foreach (var character in Characters.Where(character => character.characterPrefab))
            {
                if(character.characterPrefab.GetComponent<CharacterSync>())
                    CharacterHelper.RemoveMultiplayerScripts(character.characterPrefab);
            }
#endif

            if (EnemiesSpawnZones.Count == 0) Debug.LogWarning("(Game Manager) There are not any spawn zone for enemies in the scene.");

            StartCoroutine("SpawnEnemies");
            
            CurrentUIManager.SinglePlayerGame.ActivateAll();
            
            if (CurrentUIManager.SinglePlayerGame.PauseMainObject)
                CurrentUIManager.SinglePlayerGame.PauseMainObject.SetActive(false);
            
            if(CurrentUIManager.SinglePlayerGame.OptionsMainObject)
                CurrentUIManager.SinglePlayerGame.OptionsMainObject.SetActive(false);
            
            if (CurrentUIManager.SinglePlayerGame.Exit)
            {
                UIHelper.MangeButtons(CurrentUIManager.SinglePlayerGame.Exit, ExitGame, ref normButtonsColors[0], ref normButtonsSprites[0]);
            }

            if (CurrentUIManager.SinglePlayerGame.Restart)
            {
                UIHelper.MangeButtons(CurrentUIManager.SinglePlayerGame.Restart, RestartScene, ref normButtonsColors[1], ref normButtonsSprites[1]);
            }

            if (CurrentUIManager.SinglePlayerGame.Resume)
            {
                UIHelper.MangeButtons(CurrentUIManager.SinglePlayerGame.Resume, delegate { Pause(true); }, ref normButtonsColors[2], ref normButtonsSprites[2]);
            }

            if (CurrentUIManager.SinglePlayerGame.Options)
            {
                UIHelper.MangeButtons(CurrentUIManager.SinglePlayerGame.Options, OptionsMenu, ref normButtonsColors[3], ref normButtonsSprites[3]);
            }
            
            if (CurrentUIManager.SinglePlayerGame.OptionsBack)
            {
                UIHelper.MangeButtons(CurrentUIManager.SinglePlayerGame.OptionsBack, OptionsBackEvent, ref normButtonsColors[5], ref normButtonsSprites[5]);
            }

            foreach (var button in CurrentUIManager.SinglePlayerGame._GraphicsButtons)
            {
                if (button.Button)
                {
                    UIHelper.MangeButtons(button.Button, delegate { SetQuality(button.QualityIndex); }, ref normButtonsColors[4], ref normButtonsSprites[4]);
                }
            }
        }

        private void Start()
        {
            var hasCharacter = false;
            
            if (Characters.Count > 0)
            {
                for (var i = 0; i < Characters.Count; i++)
                {
                    var character = Characters[i];
                    if (!character.characterPrefab) continue;

                    hasCharacter = true;
                    var position = Vector3.zero;
                    var rotation = Quaternion.identity;

                    if (character.spawnZone)
                    {
                        position = CharacterHelper.SpawnPosition(character.spawnZone);
                        rotation = Quaternion.Euler(0, character.spawnZone.spawnDirection, 0);
                    }

                    var name = character.characterPrefab.name;
                    var instantiateChar = Instantiate(character.characterPrefab, position, rotation);

                    instantiateChar.name = name;
                    
                    controllers.Add(instantiateChar.GetComponent<Controller>());
                    inventoryManager.Add(instantiateChar.GetComponent<InventoryManager>());
                    cameraController.Add(instantiateChar.GetComponent<Controller>().CameraController);
                    
                    StartCoroutine(rotationTimeout());

                    var controllerScript = instantiateChar.GetComponent<Controller>();
                    var inventoryManagerScript = instantiateChar.GetComponent<InventoryManager>();
                   
                    controllerScript.enabled = true;
                    inventoryManagerScript.enabled = true;

                    controllerScript.ActiveCharacter = i == 0;
//                    inventoryManagerScript.canvas.gameObject.SetActive(i == 0);
                    
                    if(Application.isMobilePlatform || projectSettings.mobileDebug)
                        controllerScript.UIManager.UIButtonsMainObject.SetActive(true);

                    if (controllerScript.thisCamera.GetComponent<AudioListener>())
                        //controllerScript.thisCamera.GetComponent<AudioListener>().enabled = i == 0;
                        controllerScript.thisCamera.GetComponent<AudioListener>().enabled = false;

                }
            }
            else
            {
                Debug.LogError("Game Manager hasn't any character.");
                Debug.Break();
            }

            if (!hasCharacter)
            {
                Debug.LogError("Game Manager hasn't any character.");
                Debug.Break();
            }

//            if (CurrentUIManager.uiButtons[12])
//            {
//                CurrentUIManager.uiButtons[12].gameObject.SetActive(Characters.Count > 1);
//                CurrentUIManager.buttonsWereActive[12] = Characters.Count > 1;
//            }

            CurrentCharacter = 0;
        }

        public void SwitchCharacter()
        {
            if(switchingCamera || controllers.Count < 2)
                return;

            var newCharacterIndex = CurrentCharacter;

            newCharacterIndex++;
            if (newCharacterIndex > Characters.Count - 1)
                newCharacterIndex = 0;

            if (CurrentCharacter != newCharacterIndex)
            {
                switchingCamera = true;
                controllers[CurrentCharacter].ActiveCharacter = false;
                
                if(Application.isMobilePlatform || projectSettings.mobileDebug)
                    controllers[CurrentCharacter].UIManager.UIButtonsMainObject.SetActive(false);
            
                controllers[CurrentCharacter].anim.SetBool("Attack", false);
                controllers[CurrentCharacter].anim.SetBool("Move", false);
                controllers[CurrentCharacter].anim.SetFloat("Horizontal", 0);
                controllers[CurrentCharacter].anim.SetFloat("Vertical", 0);
                
                if(controllers[CurrentCharacter].thisCamera.GetComponent<AudioListener>())
                    controllers[CurrentCharacter].thisCamera.GetComponent<AudioListener>().enabled = false;
                
                if(controllers[CurrentCharacter].WeaponManager.WeaponController && controllers[CurrentCharacter].WeaponManager.WeaponController.isAimEnabled)
                    controllers[CurrentCharacter].WeaponManager.WeaponController.Aim(true, false, false);

                cameraForSwitching.gameObject.SetActive(true);
                cameraForSwitching.transform.SetPositionAndRotation(controllers[CurrentCharacter].thisCamera.transform.position,
                    controllers[CurrentCharacter].thisCamera.transform.rotation);
                
                LastCharacter = CurrentCharacter;
                CurrentCharacter = newCharacterIndex;
                StartCoroutine(FlyCamera());
            }
            
        }

        IEnumerator FlyCamera()
        {
            while (true)
            {
                var dist = Vector3.Distance(controllers[LastCharacter].transform.position, controllers[CurrentCharacter].transform.position);

                var currentHeight = controllers[CurrentCharacter].transform.position + Vector3.up * dist / 3;
                var lastHeight = controllers[CurrentCharacter].transform.position + Vector3.up * dist / 3;

                var checkTopDown = controllers[LastCharacter].thisCamera.transform.position.y < currentHeight.y &&
                                   controllers[CurrentCharacter].thisCamera.transform.position.y < lastHeight.y;
                
                if (dist > 25 && checkTopDown)
                {
                    var currentControllerScript = controllers[CurrentCharacter].GetComponent<Controller>();

                    if (!cameraFlyingStep1 && !cameraFlyingStep2)
                    {
                        cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                            controllers[LastCharacter].transform.position + Vector3.up * dist / 3, 5 * Time.deltaTime);

                        var rot = controllers[LastCharacter].thisCamera.transform.eulerAngles;
                        
                        cameraForSwitching.transform.rotation = Quaternion.Slerp(cameraForSwitching.transform.rotation, Quaternion.Euler(90, rot.y, rot.z), 0.5f);
                        
                        
                        if(currentControllerScript.UIManager.CharacterUI.crosshairMainObject)
                            currentControllerScript.UIManager.CharacterUI.crosshairMainObject.gameObject.SetActive(false);

                        //
                        //
                        if (CurrentUIManager.CharacterUI.PickupHUD)
                            CurrentUIManager.CharacterUI.PickupHUD.gameObject.SetActive(false);
                        //
                        //

                        cameraFlyingStep1 = Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position,
                            controllers[LastCharacter].transform.position + Vector3.up * dist / 3);
                    }

                    if (cameraFlyingStep1 && !cameraFlyingStep2)
                    {

                        cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                            controllers[CurrentCharacter].transform.position + Vector3.up * dist / 3, 3 * Time.deltaTime);


                        cameraFlyingStep2 = Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position,
                            controllers[CurrentCharacter].transform.position + Vector3.up * dist / 3);

                    }

                    if (cameraFlyingStep2)
                    {
                        cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                            controllers[CurrentCharacter].thisCamera.transform.position, 5 * Time.deltaTime);

                        var speed = controllers[CurrentCharacter].TypeOfCamera == CharacterHelper.CameraType.FirstPerson ? 5f : 2.5f;
                        
                        cameraForSwitching.transform.rotation = Quaternion.Slerp(cameraForSwitching.transform.rotation,
                            controllers[CurrentCharacter].thisCamera.transform.rotation, speed * Time.deltaTime);

                        if (currentControllerScript.UIManager.CharacterUI.crosshairMainObject && controllers[CurrentCharacter].TypeOfCamera != CharacterHelper.CameraType.ThirdPerson)
                            currentControllerScript.UIManager.CharacterUI.crosshairMainObject.gameObject.SetActive(true);
                        
                        if (Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position, controllers[CurrentCharacter].thisCamera.transform.position,
                            cameraForSwitching.transform.eulerAngles, controllers[CurrentCharacter].thisCamera.transform.eulerAngles))
                        {
                            controllers[CurrentCharacter].ActiveCharacter = true;
                            
                            if(Application.isMobilePlatform || projectSettings.mobileDebug)
                                controllers[CurrentCharacter].UIManager.UIButtonsMainObject.SetActive(true);

                            controllers[CurrentCharacter].thisCamera.GetComponent<Camera>().enabled = true;

                            cameraFlyingStep1 = false;
                            cameraFlyingStep2 = false;
                            switchingCamera = false;
                            
                            if(controllers[CurrentCharacter].thisCamera.GetComponent<AudioListener>())
                                //controllers[CurrentCharacter].thisCamera.GetComponent<AudioListener>().enabled = true;
                            
                            StopCoroutine(FlyCamera());
                            break;
                        }
                    }
                }
                else
                {
                    cameraForSwitching.transform.position = Helper.MoveObjInNewPosition(cameraForSwitching.transform.position,
                        controllers[CurrentCharacter].thisCamera.transform.position, 5 * Time.deltaTime);

                    cameraForSwitching.transform.rotation = Quaternion.Slerp(cameraForSwitching.transform.rotation,
                        controllers[CurrentCharacter].thisCamera.transform.rotation, 2.5f * Time.deltaTime);

                    if (Helper.ReachedPositionAndRotation(cameraForSwitching.transform.position, controllers[CurrentCharacter].thisCamera.transform.position,
                        cameraForSwitching.transform.eulerAngles, controllers[CurrentCharacter].thisCamera.transform.eulerAngles))
                    {
                        controllers[CurrentCharacter].ActiveCharacter = true;
                        controllers[CurrentCharacter].thisCamera.GetComponent<Camera>().enabled = true;
                        
                        if(Application.isMobilePlatform || projectSettings.mobileDebug)
                            controllers[CurrentCharacter].UIManager.UIButtonsMainObject.SetActive(true);

                        cameraFlyingStep1 = false;
                        cameraFlyingStep2 = false;
                        switchingCamera = false;

                        StopCoroutine(FlyCamera());
                        break;
                    }
                }

                yield return 0;
            }
        }

        void Update()
        {
            if(!gameStarted || Characters.Count == 0 || controllers.Count <= 0 || !controllers[CurrentCharacter] || inventoryManager.Count <= 0 || !inventoryManager[CurrentCharacter])
                return;
            
            if (controllers[CurrentCharacter].projectSettings.ButtonsActivityStatuses[18] && (Input.GetKeyDown(controllers[CurrentCharacter]._gamepadCodes[16]) || Input.GetKeyDown(controllers[CurrentCharacter]._keyboardCodes[18]) ||
                    Helper.CheckGamepadAxisButton(16, controllers[CurrentCharacter]._gamepadButtonsAxes, controllers[CurrentCharacter].hasAxisButtonPressed, 
                        "GetKeyDown", controllers[CurrentCharacter].projectSettings.AxisButtonValues[16])))
            {
                SwitchCharacter();
            }

            CheckGamepadInput();
            GamepadInput();

            if (controllers[CurrentCharacter].PlayerHealth <= 0)
            {
                if(CurrentUIManager.SinglePlayerGame.Restart)
                {
                    CurrentUIManager.SinglePlayerGame.Restart.gameObject.SetActive(true);

                    if (CurrentUIManager.SinglePlayerGame.PauseMainObject)
                        CurrentUIManager.SinglePlayerGame.PauseMainObject.SetActive(true);

                    if (CurrentUIManager.SinglePlayerGame.Exit)
                        CurrentUIManager.SinglePlayerGame.Exit.gameObject.SetActive(true);
                }
                else StartCoroutine("FastRestart");
            }
            else
            {
                if (controllers[CurrentCharacter].projectSettings.ButtonsActivityStatuses[10] && (Input.GetKeyDown(controllers[CurrentCharacter]._gamepadCodes[10]) || Input.GetKeyDown(controllers[CurrentCharacter]._keyboardCodes[10]) ||
                    Helper.CheckGamepadAxisButton(10, controllers[CurrentCharacter]._gamepadButtonsAxes, controllers[CurrentCharacter].hasAxisButtonPressed, 
                        "GetKeyDown", controllers[CurrentCharacter].projectSettings.AxisButtonValues[10])))
                {
                    if (inventoryManager[CurrentCharacter].Controller.UIManager.CharacterUI.Inventory.MainObject)
                        if (inventoryManager[CurrentCharacter].Controller.UIManager.CharacterUI.Inventory.MainObject.activeSelf)
                            return;

                    Pause(true);
                }
            }
        }

        public void OptionsMenu()
        {
            isOptions = !isOptions;
            
            if (isOptions)
            {
                ChangeUIVisability(false, true);
            }
            else
            {
                ChangeUIVisability(true, false);
            }
        }

        public void OptionsBackEvent()
        {
            isOptions = false;
            ChangeUIVisability(true, false);
        }

        public void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
            CurrentQuality = index;
            
            DeactivateAllButtons();
        }
        

        public void Pause(bool showUI)
        {
            isPause = !isPause;

            if (!isPause)
                isOptions = false;
            
            controllers[CurrentCharacter].isPause = isPause;
            
            if(showUI)
                ChangeUIVisability(isPause, false);

            Time.timeScale = isPause ? 0 : 1;
        }

        void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void ExitGame()
        {
            Application.Quit();
        }

        void ChangeUIVisability(bool mainUI, bool optionsUI)
        {
            DeactivateAllButtons();
            
            if (CurrentUIManager.SinglePlayerGame.PauseMainObject)
                CurrentUIManager.SinglePlayerGame.PauseMainObject.SetActive(mainUI);

            if (Application.isMobilePlatform || projectSettings.mobileDebug)
                controllers[CurrentCharacter].UIManager.UIButtonsMainObject.SetActive(!isPause);

            if (CurrentUIManager.SinglePlayerGame.Exit)
                CurrentUIManager.SinglePlayerGame.Exit.gameObject.SetActive(mainUI);

            if (CurrentUIManager.SinglePlayerGame.Resume)
                CurrentUIManager.SinglePlayerGame.Resume.gameObject.SetActive(mainUI);
            
            if (CurrentUIManager.SinglePlayerGame.Options)
                CurrentUIManager.SinglePlayerGame.Options.gameObject.SetActive(mainUI);
            
            if (CurrentUIManager.SinglePlayerGame.OptionsMainObject)
                CurrentUIManager.SinglePlayerGame.OptionsMainObject.SetActive(optionsUI);
            
            if(CurrentUIManager.SinglePlayerGame.OptionsBack)
                CurrentUIManager.SinglePlayerGame.OptionsBack.gameObject.SetActive(optionsUI);

            foreach (var button in CurrentUIManager.SinglePlayerGame._GraphicsButtons)
            {
                if(button.Button)
                    button.Button.gameObject.SetActive(optionsUI);
            }
        }

        void CheckGamepadInput()
        {
            var gamePadVector = new Vector2(Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[0]) + Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[2]), Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[1]) + Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[3]) + Input.GetAxis("Gamepad 8th axis"));

            if (gamePadVector.magnitude > 0)
            {
                controllers[CurrentCharacter].CameraController.canUseCursorInPause = false;
                gamepadInput = true;
            }
            
            var mouseVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            if (mouseVector.magnitude > 0)
            {
                if(gamepadInput)
                    DeactivateAllButtons();
                
                controllers[CurrentCharacter].CameraController.canUseCursorInPause = true;
                gamepadInput = false;
            }
        }

        void GamepadInput()
        {
            if (!isPause)
                return;
            
            if (!gamepadInput)
                return;

            var vector = new Vector2(Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[0]) + Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[2]), Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[1]) + Input.GetAxis(controllers[CurrentCharacter]._gamepadAxes[3]) + Input.GetAxis("Gamepad 8th axis"));

            vector.Normalize();

            if (Math.Abs(vector.y - 1) < 0.4f & !selectMenuItemWithGamepad)
            {
                DeactivateAllButtons();
                selectMenuItemWithGamepad = true;

                if (!isOptions)
                {
                    currentMenuItem++;

                    if (currentMenuItem > 2)
                        currentMenuItem = 2;
                }
                else
                {
                    currentOptionsMenuItem++;

                    if (currentOptionsMenuItem > CurrentUIManager.SinglePlayerGame._GraphicsButtons.Count)
                        currentOptionsMenuItem = CurrentUIManager.SinglePlayerGame._GraphicsButtons.Count;
                }
            }
            else if (Math.Abs(vector.y + 1) < 0.4f & !selectMenuItemWithGamepad)
            {
                DeactivateAllButtons();
                selectMenuItemWithGamepad = true;

                if (!isOptions)
                {
                    currentMenuItem--;

                    if (currentMenuItem < 0)
                        currentMenuItem = 0;
                }
                else
                {
                    currentOptionsMenuItem--;

                    if (currentOptionsMenuItem < 0)
                        currentOptionsMenuItem = 0;
                }


            }
            else if (Math.Abs(vector.y) < 0.1f)
            {
                selectMenuItemWithGamepad = false;
            }

            if (!isOptions)
            {
                switch (currentMenuItem)
                {
                    case 0:
                    {
                        if (CurrentUIManager.SinglePlayerGame.Restart)
                        {
                            Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Restart, CurrentUIManager.SinglePlayerGame.Restart.colors.highlightedColor, CurrentUIManager.SinglePlayerGame.Restart.spriteState.highlightedSprite);

                            if (controllers[CurrentCharacter].PlayerHealth <= 0)
                            {
                                if (Input.GetKey(KeyCode.Joystick1Button1)) Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Restart, CurrentUIManager.SinglePlayerGame.Restart.colors.pressedColor, CurrentUIManager.SinglePlayerGame.Restart.spriteState.pressedSprite);

                                if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                                    RestartScene();
                            }
                        }

                        if (CurrentUIManager.SinglePlayerGame.Resume)
                        {
                            Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Resume, CurrentUIManager.SinglePlayerGame.Resume.colors.highlightedColor, CurrentUIManager.SinglePlayerGame.Resume.spriteState.highlightedSprite);

                            if (controllers[CurrentCharacter].PlayerHealth > 0)
                            {
                                if (Input.GetKey(KeyCode.Joystick1Button1)) Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Resume, CurrentUIManager.SinglePlayerGame.Resume.colors.pressedColor, CurrentUIManager.SinglePlayerGame.Resume.spriteState.pressedSprite);

                                if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                                    Pause(true);
                            }
                        }

                        break;
                    }
                    case 1:
                    {
                        if (CurrentUIManager.SinglePlayerGame.Options)
                        {
                            Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Options, CurrentUIManager.SinglePlayerGame.Options.colors.highlightedColor, CurrentUIManager.SinglePlayerGame.Options.spriteState.highlightedSprite);

                            if (Input.GetKey(KeyCode.Joystick1Button1))
                                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Options, CurrentUIManager.SinglePlayerGame.Options.colors.pressedColor, CurrentUIManager.SinglePlayerGame.Options.spriteState.pressedSprite);

                            if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                                OptionsMenu();
                        }

                        break;
                    }
                    case 2:
                    {
                        if (CurrentUIManager.SinglePlayerGame.Exit)
                        {
                            Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Exit, CurrentUIManager.SinglePlayerGame.Exit.colors.highlightedColor, CurrentUIManager.SinglePlayerGame.Exit.spriteState.highlightedSprite);

                            if (Input.GetKey(KeyCode.Joystick1Button1))
                                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Exit, CurrentUIManager.SinglePlayerGame.Exit.colors.pressedColor, CurrentUIManager.SinglePlayerGame.Exit.spriteState.pressedSprite);

                            if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                                ExitGame();
                        }

                        break;
                    }
                }
            }
            else
            {
                if (currentOptionsMenuItem == CurrentUIManager.SinglePlayerGame._GraphicsButtons.Count)
                {
                    Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.OptionsBack, CurrentUIManager.SinglePlayerGame.OptionsBack.colors.highlightedColor, CurrentUIManager.SinglePlayerGame.OptionsBack.spriteState.highlightedSprite);

                    if (Input.GetKey(KeyCode.Joystick1Button1))
                        Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.OptionsBack, CurrentUIManager.SinglePlayerGame.OptionsBack.colors.pressedColor, CurrentUIManager.SinglePlayerGame.OptionsBack.spriteState.pressedSprite);

                    if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                        OptionsBackEvent();
                }
                else if (CurrentUIManager.SinglePlayerGame._GraphicsButtons.Count > 0 && CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button)
                {
                    if (CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].QualityIndex != CurrentQuality)
                        Helper.ChangeColor(CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button, CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button.colors.highlightedColor, CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button.spriteState.highlightedSprite);

                    if (Input.GetKey(KeyCode.Joystick1Button1))
                        Helper.ChangeColor(CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button, CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button.colors.pressedColor, CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].Button.spriteState.pressedSprite);

                    if (Input.GetKeyUp(KeyCode.Joystick1Button1))
                        SetQuality(CurrentUIManager.SinglePlayerGame._GraphicsButtons[currentOptionsMenuItem].QualityIndex);
                }
            }
        }

        void DeactivateAllButtons()
        {
            if(CurrentUIManager.SinglePlayerGame.Exit)
                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Exit,normButtonsColors[0], normButtonsSprites[0]);
            
            if(CurrentUIManager.SinglePlayerGame.Restart)
                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Restart,normButtonsColors[1], normButtonsSprites[1]);
            
            if(CurrentUIManager.SinglePlayerGame.Resume)
                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Resume,normButtonsColors[2], normButtonsSprites[2]);
            
            if(CurrentUIManager.SinglePlayerGame.Options)
                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.Options,normButtonsColors[3], normButtonsSprites[3]);
            
            if(CurrentUIManager.SinglePlayerGame.OptionsBack)
                Helper.ChangeColor(CurrentUIManager.SinglePlayerGame.OptionsBack,normButtonsColors[5], normButtonsSprites[5]);

            foreach (var button in CurrentUIManager.SinglePlayerGame._GraphicsButtons)
            {
                if (button.Button)
                {
                    if (button.QualityIndex != CurrentQuality)
                        Helper.ChangeColor(button.Button, normButtonsColors[4], normButtonsSprites[4]);
                    else Helper.ChangeColor(button.Button, button.Button.colors.pressedColor, button.Button.spriteState.pressedSprite);
                    
                    button.Button.gameObject.SetActive(false);
                    button.Button.gameObject.SetActive(true);
                }
            }
        }

        public void RemoveEnemyFromScene(int index)
        {
            Enemies[index].currentSpawnCount--;
        }
        
        IEnumerator rotationTimeout()
        {
            yield return new WaitForSeconds(0.1f);
                
            controllers[0].transform.rotation = Quaternion.Euler(0, Characters[0].spawnZone.spawnDirection, 0);
            cameraController[0]._mouseAbsolute = new Vector2(Characters[0].spawnZone.spawnDirection, 0);
        }

        IEnumerator FastRestart()
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            StopCoroutine("FastRestart");
        }

        IEnumerator SpawnEnemies()
        {
            while (true)
            {
                for (var i = 0; i < Enemies.Count; i++)
                {
                    var enemy = Enemies[i];
                    enemy.currentTime += Time.deltaTime;

                    if (enemy.count > enemy.currentSpawnCount && EnemiesSpawnZones.Count > 0 && enemy.enemyPrefab)
                    {
                        if (enemy.currentTime >= enemy.spawnTimeout)
                        {
                            enemy.currentTime = 0;

                            var spawn = enemy.currentSpawnMethodIndex == 0 ? EnemiesSpawnZones[Random.Range(0, EnemiesSpawnZones.Count)] : enemy.spawnZone;

                            if (!spawn)
                            {
                                Debug.LogWarning("No zones were found for the spawn of the enemy");
                                continue;
                            }
                            
                            var position = CharacterHelper.SpawnPosition(spawn);
                            var rotation = Quaternion.Euler(0, spawn.spawnDirection, 0);

                            if (spawn)
                            {
                                var name = enemy.enemyPrefab.name;
                                var _enemy = Instantiate(enemy.enemyPrefab.gameObject, position, rotation);

                                _enemy.GetComponent<EnemyController>().indexInGameManager = i;
                                _enemy.name = name;

                                enemy.currentSpawnCount++;

                                if (enemy.movementBehavior)
                                    _enemy.GetComponent<EnemyController>().Behaviour = enemy.movementBehavior;
                            }
                        }
                    }
                }

                yield return 0;
            }
        }
        
    }
}