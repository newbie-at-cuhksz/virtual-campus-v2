// GercStudio
// © 2018-2019

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	public class CameraController : MonoBehaviour
	{
		public CameraController OriginalScript;

		public CharacterHelper.CameraOffset CameraOffset = new CharacterHelper.CameraOffset();

		public Controller Controller;

		private Transform targetLookAt;
		public Transform CameraPosition;
//		public Transform Crosshair;
		public GameObject LayerCamera;

		public float CrosshairOffsetX;
		public float CrosshairOffsetY = 300;

		public float maxMouseAbsolute;
		public float minMouseAbsolute;
		public float cameraMovementDistance = 5;
		public float cameraDistanse;

		public bool CameraAim;
		public bool deepAim;
		public bool Occlusion;
		public bool cameraDebug;
		public bool setCameraType;
		public bool canViewTarget;
		public bool useCameraJoystic;
		public bool canUseCursorInPause;
		public bool cameraPause;
		public bool cameraOcclusion;

		public Camera Camera;
		public Camera AimCamera;
		
		public Vector2 targetDirection;
		public Vector2 _mouseAbsolute;
		public Vector2 _smoothMouse;

//		public Image upPart;
//		public Image leftPart;
//		public Image rightPart;
//		public Image downPart;
//		public Image middlePart;

		private Vector3 desiredCameraPosition = Vector3.zero;
		private Vector3 desiredBodyLookAtPosition = Vector3.zero;
		private Vector3 _position = Vector3.zero;
		private Vector3 bodyLookAtPosition = Vector3.zero;
		public Vector3 LastMousePosition;

		public Vector2 mouseDelta;
		private Vector2 TPmouseDelta;

		private Vector2[] currentCrosshairPositions = new Vector2[5];

		private Vector2 GamePadAxis;
		private Vector2 LastGamepadAxis;
		
		private Quaternion desiredRotation;

		private List<GameObject> disabledObjects = new List<GameObject>();
		private Transform preOcclededCamera;
		public Transform MainCamera;
		public Transform BodyLookAt;
		private Transform body;

		private float CurrentSensitivityX;
		private float CurrentSensitivityY;
		public float mouseX;
		public float mouseY;
		private float velX;
		private float velY;
		private float velZ;
		private float normDepth;
		private float CurrentDistance;
		private float CurrentOffsetX;
		private float CurrentOffsetY;
		private float desiredDistance;
		private float floorHeight;
		private float desiredOffsetX;
		private float bobTime;
		private float lastBobTime;
		private float curveTransitionValue;

//		private float desiredOffsetY;
		private float textuteAlpha;
		private float crosshairMultiplier = 1;
		
		private Vector2 MobileMoveStickDirection;
		private Vector2 MobileTouchjPointA, MobileTouchjPointB;

		private bool canChangeCurve;
		private bool cursorImageHasChanged;
		private bool useGamepad;

		private AnimationCurve currentCurve;
		private AnimationCurve newCurve;

		private int touchId = -1;

		private Collider[] _occlusionCollisers = new Collider[2];

		private void Awake()
		{
			if (FindObjectOfType<Lobby>())
			{
				Destroy(gameObject);
				return;
			}

			Camera = GetComponent<Camera>();

			Camera.cullingMask &= ~(1 << 15); //Layer Hidden = 15
			Camera.cullingMask &= ~(1 << 16); //Layer Minimap = 16

			Camera.renderingPath = RenderingPath.DeferredShading;

			Camera.fieldOfView = 20;

			MainCamera = new GameObject("Camera").transform;
            MainCamera.gameObject.AddComponent<Camera>().backgroundColor = new Color(0, 0, 0);
			MainCamera.gameObject.GetComponent<Camera>().cullingMask = ~(1 << 15); //Layer Hidden = 15
			MainCamera.gameObject.GetComponent<Camera>().cullingMask = ~(1 << 16); //Layer Minimap = 16
			MainCamera.gameObject.GetComponent<Camera>().enabled = false;
            //MainCamera.gameObject.GetComponent<AudioListener>().enabled = false;


            LayerCamera = Helper.NewCamera("LayerCamera", transform, "CameraController").gameObject;
			LayerCamera.SetActive(false);
			LayerCamera.hideFlags = HideFlags.HideInHierarchy;
			
			if (!Controller)
			{
				Debug.Log("Disconnect between camera and controller");
				Debug.Break();
				return;
			}
            

        }

        public void AddAudioListener()
        {

            foreach (var audioListeners in GetComponentsInChildren<AudioListener>())
            {
                audioListeners.enabled = false;
            }

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("AudioListener"))
            {
                Destroy(obj);
            }

            Transform player = null;
            while (!player)
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (obj.layer == LayerMask.NameToLayer("Character"))
                    {
                        player = obj.transform;
                    }
                }
            }
            //Debug.Log(transform.parent.position.y);


            GameObject audioListener = new GameObject();
            audioListener.name = "Audio Listener";
            audioListener.transform.position = new Vector3(0, -(transform.parent.position.y - player.position.y), 0);
            audioListener.transform.localEulerAngles = Vector3.zero;
            audioListener.AddComponent<AudioListener>();
            audioListener.tag = "AudioListener";
            var follow = audioListener.AddComponent<Follow>();
            follow.target = transform.parent;
            follow.yOffset = -(transform.parent.position.y - player.position.y);
            //Debug.Log(follow.yOffset);
        }

		void Start()
		{
			if (FindObjectOfType<Lobby>())
			{
//				Destroy(gameObject);
				return;
			}

//			foreach (Transform child in transform)
//			{
//				if (child.name == "LayerCamera")
//				{
//					LayerCamera = child.gameObject;
//					print("layer camera != null");
//				}
//			} 
			

			normDepth = Camera.fieldOfView;

			newCurve = Controller.CameraParameters.FPIdleCurve;
			currentCurve = newCurve;

			if (Controller.UIManager.CharacterUI.aimPlaceholder)
				Controller.UIManager.CharacterUI.aimPlaceholder.gameObject.SetActive(false);

			preOcclededCamera = new GameObject("preOclCamera").transform;
			preOcclededCamera.parent = MainCamera;
			preOcclededCamera.localPosition = Vector3.zero;
			preOcclededCamera.hideFlags = HideFlags.HideInHierarchy;

			transform.parent = MainCamera;
			transform.position = new Vector3(0, 0, 0);
			transform.rotation = Quaternion.Euler(0, 0, 0);

            

            body = Controller.BodyObjects.TopBody;
			BodyLookAt = new GameObject("BodyLookAt").transform;
			BodyLookAt.hideFlags = HideFlags.HideInHierarchy;
			
			LastGamepadAxis = new Vector2(Controller.transform.forward.x, Controller.transform.forward.z);
			targetDirection = MainCamera.localEulerAngles;

			CameraOffset.tpCameraOffsetX = CameraOffset.normCameraOffsetX;
			CameraOffset.tpCameraOffsetY = CameraOffset.normCameraOffsetY;
			CameraOffset.Distance = CameraOffset.normDistance;

			_position = Controller.BodyObjects.Head.transform.position;

			if (CameraPosition)
			{
				CameraPosition.parent = Controller.BodyObjects.Head;
				CameraPosition.localPosition = CameraOffset.cameraObjPos;
				CameraPosition.localEulerAngles = CameraOffset.cameraObjRot;
			}
			else
			{
				Debug.LogError("<color=red>Missing component</color>: [Camera position]", gameObject);
				Debug.Break();
			}

			if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
			{
				Helper.CameraExtensions.LayerCullingHide(Camera, "Head");
				Helper.CameraExtensions.LayerCullingHide(AimCamera, "Head");
				Helper.CameraExtensions.LayerCullingHide(LayerCamera.GetComponent<Camera>(), "Head");
			}

			AimCamera.gameObject.SetActive(false);

			SetAnimVariables();

			setCameraType = true;
			
			if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
			{
				mouseX = Controller.transform.localEulerAngles.y;
				mouseY = Controller.transform.localEulerAngles.x;
				
				if(Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown)
					BodyLookAt.position = Controller.transform.position;
			}
			else
			{
				mouseX = 0;
				mouseY = 0;
			}

			Reset();

            StartCoroutine(LateAddAudioListener());
        }

        IEnumerator LateAddAudioListener()
        {
            yield return new WaitForEndOfFrame();
            AddAudioListener();
        }

		void Update()
		{
			if (Controller && !Controller.ActiveCharacter)
			{
				if (GetComponent<Camera>().enabled)
					GetComponent<Camera>().enabled = false;

				if (AimCamera.enabled)
					AimCamera.enabled = false;
			}

			CheckGamepad();
			CrosshairAnimation();
			
			if (!CameraAim)
			{
				if (Math.Abs(CameraOffset.tpCameraOffsetX - CameraOffset.normCameraOffsetX) > 0.1f)
					CameraOffset.tpCameraOffsetX = Mathf.Lerp(CameraOffset.tpCameraOffsetX, CameraOffset.normCameraOffsetX, 5 * Time.deltaTime);

				if (Math.Abs(CameraOffset.tpCameraOffsetY - CameraOffset.normCameraOffsetY) > 0.1f)
					CameraOffset.tpCameraOffsetY = Mathf.Lerp(CameraOffset.tpCameraOffsetY, CameraOffset.normCameraOffsetY, 5 * Time.deltaTime);

//				if (Vector3.Distance(CameraOffset.tpCameraRotationOffset, CameraOffset.cameraNormRotationOffset) > 0.1f)
//					CameraOffset.tpCameraRotationOffset = Vector3.Lerp(CameraOffset.tpCameraRotationOffset, CameraOffset.cameraNormRotationOffset, 5 * Time.deltaTime);

				if (Math.Abs(CameraOffset.Distance - CameraOffset.normDistance) > 0.1f)
				{
					CameraOffset.Distance = Mathf.Lerp(CameraOffset.Distance, CameraOffset.normDistance, 10 * Time.deltaTime);
					Reset();
				}
				
				if(Controller.AdjustmentScene)
					Reset();
			}
			else
			{
				if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !Controller.tdModeLikeTp || Controller.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson)
				{
					if (Math.Abs(CameraOffset.tpCameraOffsetX - CameraOffset.aimCameraOffsetX) > 0.1f)
						CameraOffset.tpCameraOffsetX = Mathf.Lerp(CameraOffset.tpCameraOffsetX, CameraOffset.aimCameraOffsetX, 5 * Time.deltaTime);

					if (Math.Abs(CameraOffset.tpCameraOffsetY - CameraOffset.aimCameraOffsetY) > 0.1f)
						CameraOffset.tpCameraOffsetY = Mathf.Lerp(CameraOffset.tpCameraOffsetY, CameraOffset.aimCameraOffsetY, 5 * Time.deltaTime);

//				if (Vector3.Distance(CameraOffset.tpCameraRotationOffset, CameraOffset.cameraAimRotationOffset) > 0.1f)
//					CameraOffset.tpCameraRotationOffset = Vector3.Lerp(CameraOffset.tpCameraRotationOffset, CameraOffset.cameraAimRotationOffset, 5 * Time.deltaTime);

					if (Math.Abs(CameraOffset.Distance - CameraOffset.aimDistance) > 0.1f)
					{
						CameraOffset.Distance = Mathf.Lerp(CameraOffset.Distance, CameraOffset.aimDistance, 10 * Time.deltaTime);
						Reset();
					}

					if (Controller.AdjustmentScene)
						Reset();
				}
			}
			

			LayerCamera.GetComponent<Camera>().fieldOfView = Camera.fieldOfView;
			
			if (cameraDebug)
				Reset();


			desiredOffsetX = CurrentOffsetX;
			
			if(Controller.AdjustmentScene)
				return;
			
			var speed = Controller.WeaponManager.WeaponController ? Controller.WeaponManager.WeaponController.setAimSpeed : 1;

			if (CameraAim)
			{
				switch (Controller.TypeOfCamera)
				{
					case CharacterHelper.CameraType.FirstPerson:

						Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, Controller.CameraParameters.fpAimDepth, speed * 10 * Time.deltaTime);

						if (Controller.WeaponManager.WeaponController.setHandsPositionsAim)
							//(Math.Abs(Camera.fieldOfView - Controller.CameraParameters.FPAimDepth) < 10)
						{
							if (Controller.WeaponManager.WeaponController && Controller.WeaponManager.WeaponController.useAimTexture)
							{
								AimCamera.fieldOfView = Mathf.Lerp(AimCamera.fieldOfView, Controller.WeaponManager.WeaponController.aimTextureDepth, speed * 10 * Time.deltaTime);//0.5f);

								if (Controller.UIManager.CharacterUI.aimPlaceholder)
								{
									Controller.UIManager.CharacterUI.aimPlaceholder.gameObject.SetActive(true);

									var color = Controller.UIManager.CharacterUI.aimPlaceholder.color;

									color.a = Mathf.Lerp(color.a, 1, 0.5f);

									Controller.UIManager.CharacterUI.aimPlaceholder.color = color;
								}
							}
						}

						if (Controller.WeaponManager.WeaponController.setHandsPositionsAim)
							//(Math.Abs(Camera.fieldOfView - Controller.CameraParameters.FPAimDepth) < 7)
						{
							if (Controller.WeaponManager.WeaponController && Controller.WeaponManager.WeaponController.useAimTexture)
							{
								AimCamera.targetTexture = null;
								AimCamera.gameObject.SetActive(true);
							}
						}

						break;
					case CharacterHelper.CameraType.ThirdPerson:

						if(!Controller.tdModeLikeTp)
							Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, Controller.CameraParameters.tpAimDepth, speed);
						else Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, normDepth, speed);
						
						DisableAimTextures();

						break;
				}
			}
			else
			{
				Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, normDepth, speed * 10 * Time.deltaTime);

				DisableAimTextures();
			}
		}

		void LateUpdate()
		{
			if(CameraAim && Controller.WeaponManager.WeaponController && Controller.WeaponManager.WeaponController.useScope && Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				bobTime += 0.5f * Time.deltaTime;
			else bobTime += Time.deltaTime;
			
			if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
			{
				if (!CameraPosition)
					return;

				FPCameraUpdate();
			}
			else
			{
				if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown || Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && !Controller.CameraParameters.lockCamera)
					GetMouseAxis();

				switch (Controller.TypeOfCamera)
				{
					case CharacterHelper.CameraType.ThirdPerson:

						if (!Controller.tdModeLikeTp)
						{
							tpCheckIfOccluded();
							ChangeCurrentPosition();
							tpCalculateDesiredPosition();
						}
						else
						{
							tdCalculateDesiredPosition();
							CharacterHelper.CheckCameraPoints(Controller.BodyObjects.Head.position, transform.position, disabledObjects, Camera);
						}

						break;

					case CharacterHelper.CameraType.TopDown:

						tdCalculateDesiredPosition();
						CharacterHelper.CheckCameraPoints(Controller.BodyObjects.Head.position, transform.position, disabledObjects, Camera);

						break;
				}

				if (Time.timeScale > 0)
					UpdateCameraPosition();
			}
		}

		void ChangeCurrentPosition()
		{
			CurrentDistance = Mathf.Lerp(CurrentDistance, desiredDistance, 0.6f);

			CurrentOffsetX = Mathf.Lerp(CurrentOffsetX, desiredOffsetX, 0.6f);
		}

		void tdCalculateDesiredPosition()
		{
			RaycastHit hit;

			floorHeight = Physics.Raycast(Controller.BodyObjects.Hips.position, Vector3.down, out hit, 100, Helper.layerMask()) ? hit.point.y : Controller.transform.position.y;


            //TOPDOWN-ANGLE
            //desiredCameraPosition = CharacterHelper.CalculatePosition(mouseX, 10, Controller, CameraOffset.TopDownAngle - 10, floorHeight, "camera");
            desiredCameraPosition = CharacterHelper.CalculatePosition(mouseX, 34, Controller, CameraOffset.TopDownAngle - 10, floorHeight, "camera");
        }

		void tpCalculateDesiredPosition()
		{
			RaycastHit hit;
			floorHeight = Physics.Raycast(Controller.BodyObjects.Hips.position, Vector3.down, out hit, 100, Helper.layerMask()) ? hit.point.y : Controller.transform.position.y;
			
			desiredCameraPosition = CharacterHelper.CalculatePosition(mouseY, mouseX, cameraMovementDistance, Controller, floorHeight, Controller.isJump);
			desiredBodyLookAtPosition = CharacterHelper.CalculatePosition(mouseY, mouseX, 5, Controller, floorHeight, Controller.isJump);
		}

		void UpdateCameraPosition()
		{
			switch (Controller.TypeOfCamera)
			{
				case CharacterHelper.CameraType.ThirdPerson:
					if (!Controller.tdModeLikeTp)
					{
						TPCameraUpdate();
					}
					else
					{
						TDCameraUpdate();
					}
					break;

				case CharacterHelper.CameraType.TopDown:
					TDCameraUpdate();
					break;
			}

			if (body)
				Controller.BodyLookAt(BodyLookAt);

			Controller.BodyRotate();
		}

		public IEnumerator cameraTimeout()
		{
			yield return new WaitForSeconds(0.01f);
			canViewTarget = true;
			StopCoroutine("cameraTimeout");
		}

		void TPCameraUpdate()
		{
			transform.localEulerAngles = new Vector3(-1, 0, 0);

			var posX = Mathf.SmoothDamp(_position.x, desiredCameraPosition.x, ref velX, Controller.CameraParameters.tpSmoothX);
			var posY = Mathf.SmoothDamp(_position.y, desiredCameraPosition.y, ref velY, Controller.CameraParameters.tpSmoothY);
			var posZ = Mathf.SmoothDamp(_position.z, desiredCameraPosition.z, ref velZ, Controller.CameraParameters.tpSmoothX);
			_position = new Vector3(posX, posY, posZ);

			if (setCameraType)
			{
				if (Time.timeScale > 0)
					MainCamera.position = _position;

				transform.localPosition = new Vector3(CurrentOffsetX, CurrentOffsetY, CurrentDistance);

				if (Controller.CameraFollowCharacter)
					MainCamera.LookAt(Controller.BodyObjects.Head);
				else
				{
					if (Controller.headHeight == -1)
						MainCamera.LookAt(Controller.BodyObjects.Head);
					else
					{
						if (!Controller.isJump && !Controller.isCrouch)
							MainCamera.LookAt(new Vector3(Controller.transform.position.x, floorHeight + Controller.headHeight, Controller.transform.position.z));
						else MainCamera.LookAt(Controller.BodyObjects.Head);
					}
				}
			}
			else
			{
				MainCamera.position = Helper.MoveObjInNewPosition(MainCamera.position, _position, 0.5f);
				transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition, new Vector3(CurrentOffsetX, CurrentOffsetY, CurrentDistance), 0.5f);

				if (Math.Abs(MainCamera.position.x - _position.x) < 0.1f & Math.Abs(MainCamera.position.y - _position.y) < 0.1f &
				    Math.Abs(MainCamera.position.z - _position.z) < 0.1f & Math.Abs(transform.localPosition.x - CurrentOffsetX) < 0.01f &
				    Math.Abs(transform.localPosition.y - CurrentOffsetY) < 0.01f & Math.Abs(transform.localPosition.z - CurrentDistance) < 0.01f)
				{
					setCameraType = true;
				}

				if (canViewTarget)
				{
					MainCamera.LookAt(Controller.BodyObjects.Head);
				}
			}
			
			bodyLookAtPosition = desiredBodyLookAtPosition;

			BodyLookAt.position = bodyLookAtPosition;
			BodyLookAt.RotateAround(Controller.BodyObjects.Head.position, Vector3.right, 180);

			var newPos = BodyLookAt.position;
			BodyLookAt.position = bodyLookAtPosition;
			BodyLookAt.RotateAround(Controller.BodyObjects.Head.position, Vector3.up, 185);

			BodyLookAt.position = new Vector3(BodyLookAt.position.x, newPos.y, BodyLookAt.position.z);

		}

		void TDCameraUpdate()
		{
			transform.localEulerAngles = Vector3.zero;

			if (Controller.CameraParameters.lockCamera)
			{
				transform.localEulerAngles = new Vector3(CameraOffset.tdLockCameraAngle, 0, 0);
				MainCamera.eulerAngles = Vector3.zero;
			}

			var posX = Mathf.SmoothDamp(_position.x, desiredCameraPosition.x, ref velX, Controller.CameraParameters.tdSmoothX);
			var posY = Mathf.SmoothDamp(_position.y, desiredCameraPosition.y, ref velY, Controller.CameraParameters.tdSmoothX);
			var posZ = Mathf.SmoothDamp(_position.z, desiredCameraPosition.z, ref velZ, Controller.CameraParameters.tdSmoothX);
			_position = new Vector3(posX, posY, posZ);

			if (setCameraType)
			{
				if (Controller.CameraParameters.lockCamera)
				{
					MainCamera.position = new Vector3(Controller.transform.position.x, floorHeight + Vector3.up.y * 10, Controller.transform.position.z);
					transform.localPosition = new Vector3(CameraOffset.tdLockCameraOffsetX, CameraOffset.TDLockCameraDistance, CameraOffset.tdLockCameraOffsetY);
				}
				else
				{
					MainCamera.position = _position;
					transform.localPosition = new Vector3(CameraOffset.tdCameraOffsetX, CameraOffset.tdCameraOffsetY, CameraOffset.TD_Distance);
					MainCamera.LookAt(new Vector3(Controller.transform.position.x, floorHeight + Controller.headHeight, Controller.transform.position.z));
				}
			}
			else
			{
				if (!Controller.CameraParameters.lockCamera)
				{
					MainCamera.position = Helper.MoveObjInNewPosition(MainCamera.position, _position, 0.5f);
					transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition,
						new Vector3(CameraOffset.tdCameraOffsetX, CameraOffset.tdCameraOffsetY, CameraOffset.TD_Distance), 0.5f);

					if (Math.Abs(MainCamera.position.x - _position.x) < 0.1f &
					    Math.Abs(MainCamera.position.y - _position.y) < 0.1f &
					    Math.Abs(MainCamera.position.z - _position.z) < 0.1f &
					    Math.Abs(transform.localPosition.x - CameraOffset.tdCameraOffsetX) < 0.01f &
					    Math.Abs(transform.localPosition.y - CameraOffset.tdCameraOffsetY) < 0.01f &
					    Math.Abs(transform.localPosition.z - CameraOffset.TD_Distance) < 0.01f)
					{
						setCameraType = true;
					}

					if (canViewTarget)
//						MainCamera.LookAt(Controller.BodyObjects.Head);
						MainCamera.LookAt(new Vector3(Controller.transform.position.x, floorHeight + Controller.headHeight, Controller.transform.position.z));
				}
				else
				{
					var pos = new Vector3(Controller.transform.position.x, floorHeight + Vector3.up.y * 10, Controller.transform.position.z);
					var localPos = new Vector3(CameraOffset.tdLockCameraOffsetX, CameraOffset.TDLockCameraDistance, CameraOffset.tdLockCameraOffsetY);

					MainCamera.position = Helper.MoveObjInNewPosition(MainCamera.position, pos, 0.5f);
					transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition, localPos, 0.5f);

					if (Math.Abs(MainCamera.position.x - pos.x) < 0.1f && Math.Abs(MainCamera.position.y - pos.y) < 0.1f &&
					    Math.Abs(MainCamera.position.z - pos.z) < 0.1f && Math.Abs(transform.localPosition.x - localPos.x) < 0.01f &&
					    Math.Abs(transform.localPosition.y - localPos.y) < 0.01f && Math.Abs(transform.localPosition.z - localPos.z) < 0.01f)
					{
						setCameraType = true;
					}
				}
			}

			if (Controller.CameraParameters.lockCamera)
			{
				var height = Controller.defaultHeight > 0 ? Controller.defaultHeight : Controller.BodyObjects.Hips.position.y - Controller.transform.position.y;

				if (!Controller.isPause && !cameraPause)
				{
					if (!useGamepad && !Application.isMobilePlatform && !Controller.projectSettings.mobileDebug)
					{
						RaycastHit hit;

						if (Physics.Raycast(transform.position, transform.forward, out hit, 100, Helper.layerMask()))
						{
							cameraDistanse = hit.distance - height;
						}
						else
						{
							cameraDistanse = Vector3.Distance(Controller.BodyObjects.Hips.position, transform.position);
						}

						var cursorPosition = Vector3.zero;

						if (Controller.CameraParameters.CursorImage)
						{
							cursorPosition = new Vector3(Input.mousePosition.x + Controller.CameraParameters.CursorImage.texture.height / 2,
								Input.mousePosition.y - Controller.CameraParameters.CursorImage.texture.width / 2, cameraDistanse);
						}
						else cursorPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistanse);

						LastMousePosition = Camera.ScreenToWorldPoint(cursorPosition);

						cursorPosition.z = Input.mousePosition.z;
						if (Physics.Raycast(Camera.ScreenPointToRay(cursorPosition), out hit))
						{
							LastMousePosition = hit.point;
							LastMousePosition.y = Controller.BodyObjects.Hips.position.y;
						}

						var point = LastMousePosition;

						if (Controller.CameraParameters.lookAtCursor)
						{
							if (!Controller.WeaponManager.slots[Controller.WeaponManager.currentSlot].weaponSlotInGame[Controller.WeaponManager.slots[Controller.WeaponManager.currentSlot].currentWeaponInSlot].fistAttack &&
							    (Controller.WeaponManager.WeaponController && Controller.WeaponManager.WeaponController.Attacks[Controller.WeaponManager.WeaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee || !Controller.WeaponManager.WeaponController))
							{
								cursorPosition.z = Input.mousePosition.z;

								if (Physics.Raycast(Camera.ScreenPointToRay(cursorPosition), out hit))
								{
									point = hit.point;

									if (hit.transform.root.gameObject.GetComponent<Controller>() || hit.transform.root.gameObject.GetComponent<EnemyController>())
										point.y = hit.transform.root.position.y;
								}
							}
						}

						var speed = Mathf.Abs(point.y - BodyLookAt.position.y) > 1 ? 5 : 100;
						BodyLookAt.position = Vector3.MoveTowards(BodyLookAt.position, point, Controller.CameraParameters.tdXMouseSensitivity * speed * Time.deltaTime);
					}
					else
					{
						GetMouseAxis();

						var dir = GamePadAxis * 100;
						dir.Normalize();

						if (dir.magnitude > 0.9)
						{
							LastGamepadAxis = dir;
						}

						BodyLookAt.position = new Vector3(Controller.transform.position.x, floorHeight + height, Controller.transform.position.z) + new Vector3(LastGamepadAxis.x, 0, LastGamepadAxis.y) * 5;
					}
				}
			}
			else
			{
				BodyLookAt.position = new Vector3(MainCamera.position.x, Controller.BodyObjects.Hips.position.y, MainCamera.position.z);

				BodyLookAt.RotateAround(Controller.BodyObjects.Head.position, Vector3.up, 180);

				bodyLookAtPosition = _position;
			}
		}

		void FPCameraUpdate()
		{
			mouseY = 0;
			mouseX = MainCamera.eulerAngles.y;
			
			var targetOrientation = Quaternion.Euler(targetDirection);

			if (!Controller.isPause && ! cameraPause && Controller.ActiveCharacter)
			{
				if (Application.isMobilePlatform || Controller.projectSettings.mobileDebug)
					InputHelper.CheckTouchCamera(ref touchId, ref mouseDelta, Controller);
				else
				{
					if (Mathf.Abs(Input.GetAxisRaw(Controller._gamepadAxes[2])) > 0.1f || Mathf.Abs(Input.GetAxisRaw(Controller._gamepadAxes[3])) > 0.1f)
					{
						GamePadAxis.x = Input.GetAxisRaw(Controller._gamepadAxes[2]) / 2;
						if (Controller.projectSettings.invertAxes[2])
							GamePadAxis.x *= -1;
						GamePadAxis.y = Input.GetAxisRaw(Controller._gamepadAxes[3]) / 2;
						if (Controller.projectSettings.invertAxes[3])
							GamePadAxis.y *= -1;

						mouseDelta = new Vector2(GamePadAxis.x, GamePadAxis.y);
					}
					else
					{
						mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X") / 2, Input.GetAxisRaw("Mouse Y") / 2);
					}
				}
				
				mouseDelta = Vector2.Scale(mouseDelta,
					new Vector2(CurrentSensitivityX * Controller.CameraParameters.fpXSmooth, CurrentSensitivityY * Controller.CameraParameters.fpYSmooth));

				_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1 / Controller.CameraParameters.fpXSmooth);
				_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1 / Controller.CameraParameters.fpYSmooth);
				_mouseAbsolute += _smoothMouse;
			}

			if (_mouseAbsolute.y > maxMouseAbsolute)
			{
				_mouseAbsolute.y = maxMouseAbsolute;
			}
			else if (_mouseAbsolute.y < minMouseAbsolute)
			{
				_mouseAbsolute.y = minMouseAbsolute;
			}

			var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);

			desiredRotation = yRotation * Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

			if (setCameraType)
			{
				MainCamera.rotation = desiredRotation;
				body.rotation = MainCamera.rotation;

				Controller.TopBodyOffset();
				Controller.BodyRotate();

				if (Controller.gameObject.activeSelf)
				{
					if (Controller.anim.GetFloat("Horizontal") > 0.3f || Controller.anim.GetFloat("Horizontal") < -0.3f ||
					    Controller.anim.GetFloat("Vertical") > 0.3f || Controller.anim.GetFloat("Vertical") < -0.3f)
					{
						if (Controller.isCrouch)
						{
							newCurve = Controller.CameraParameters.FPCrouchCurve;
						}
						else if (Controller.isSprint)
						{
							newCurve = Controller.CameraParameters.FPRunCurve;
						}
						else
						{
							newCurve = Controller.CameraParameters.FPWalkCurve;
						}
					}
					else
					{
						newCurve = Controller.CameraParameters.FPIdleCurve;
					}
				}

				if (newCurve != currentCurve)
				{
					curveTransitionValue = Mathf.Lerp(curveTransitionValue, newCurve.Evaluate(newCurve.keys[0].time), 10 * Time.deltaTime);
					
					transform.eulerAngles = new Vector3(CameraPosition.eulerAngles.x, CameraPosition.eulerAngles.y, 0) - new Vector3(curveTransitionValue, 0, 0);
					
					if (Math.Abs(curveTransitionValue - newCurve.Evaluate(newCurve.keys[0].time)) < 0.01f)
					{
						currentCurve = newCurve;
						bobTime = currentCurve.keys[0].time;
					}
				}
				else
				{
					if (bobTime > currentCurve.keys[currentCurve.length - 1].time)
						bobTime = currentCurve.keys[0].time;

					lastBobTime = bobTime;
					curveTransitionValue = currentCurve.Evaluate(lastBobTime);

					var bobValue = currentCurve.Evaluate(lastBobTime);

					if (CameraAim && Controller.WeaponManager.WeaponController && Controller.WeaponManager.WeaponController.useAimTexture)
						bobValue /= 2;
					
					transform.eulerAngles = new Vector3(CameraPosition.eulerAngles.x, CameraPosition.eulerAngles.y, 0) - new Vector3(bobValue, 0, 0);

				}
				
				MainCamera.position = CameraPosition.position;
			}
			else
			{
				if (deepAim)
				{
					_mouseAbsolute.x = -3.5f;
					_mouseAbsolute.y = -6;
				}
				else
				{
					_mouseAbsolute.y = 0;
					_mouseAbsolute.x = 0;
				}

				if (Controller.isCrouch)
				{
					_mouseAbsolute.x = -17;
//					_mouseAbsolute.y = 55;
				}

				transform.localPosition = Helper.MoveObjInNewPosition(transform.localPosition, Vector3.zero, 0.8f);

				MainCamera.rotation = Quaternion.Slerp(MainCamera.rotation, desiredRotation, 0.5f);
				body.rotation = MainCamera.rotation;

				Controller.TopBodyOffset();
				Controller.BodyRotate();

				transform.rotation = Quaternion.Slerp(transform.rotation,
					Quaternion.Euler(CameraPosition.eulerAngles.x, CameraPosition.eulerAngles.y, 0), 0.5f);
				MainCamera.position = Helper.MoveObjInNewPosition(MainCamera.position, CameraPosition.position, 0.5f);

				if (Math.Abs(MainCamera.position.x - CameraPosition.position.x) < 0.6f &
				    Math.Abs(MainCamera.position.y - CameraPosition.position.y) < 0.6f &
				    Math.Abs(MainCamera.position.z - CameraPosition.position.z) < 0.6f)
				{
					setCameraType = true;
				}

			}
		}

		void CheckGamepad()
		{
			if (Mathf.Abs(Input.GetAxisRaw(Controller._gamepadAxes[2])) > 0.1f || Mathf.Abs(Input.GetAxisRaw(Controller._gamepadAxes[3])) > 0.1f)
			{
				useGamepad = true;
			}
			else if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f)
			{
				useGamepad = false;
			}
		}
		
		void GetMouseAxis()
		{
			if (Controller.isPause || cameraPause || !Controller.ActiveCharacter) return;

			if (!Application.isMobilePlatform && !Controller.projectSettings.mobileDebug)
			{
				if (Mathf.Abs(Input.GetAxisRaw(Controller._gamepadAxes[2])) > 0.1f || Mathf.Abs(Input.GetAxisRaw(Controller._gamepadAxes[3])) > 0.1f)
				{
					GamePadAxis.x = Input.GetAxisRaw(Controller._gamepadAxes[2]) * CurrentSensitivityX * 2;
					if (Controller.projectSettings.invertAxes[2])
						GamePadAxis.x *= -1;

					GamePadAxis.y = Input.GetAxisRaw(Controller._gamepadAxes[3]) * CurrentSensitivityY * 2;
					if (Controller.projectSettings.invertAxes[3])
						GamePadAxis.y *= -1;

					mouseX += GamePadAxis.x;

					if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
						mouseY -= GamePadAxis.y;
				}
				else
				{
					mouseX += Input.GetAxis("Mouse X") * CurrentSensitivityX;

					if (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
						mouseY -= Input.GetAxis("Mouse Y") * CurrentSensitivityY;
				}

				if(Controller.UIManager.cameraStick)
					Controller.UIManager.cameraStick.gameObject.SetActive(false);
				
				if(Controller.UIManager.cameraStickOutline)
					Controller.UIManager.cameraStickOutline.gameObject.SetActive(false);
			}
			else
			{
				if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && !Controller.CameraParameters.lockCamera || Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
				{
					if (Input.touchCount > 0)
					{
						for (var i = 0; i < Input.touches.Length; i++)
						{
							var touch = Input.GetTouch(i);

							if (touch.position.x > Screen.width / 2 && touchId == -1 && touch.phase == TouchPhase.Began)
							{
								touchId = touch.fingerId;
							}

							if (touch.fingerId == touchId)
							{
								if (touch.position.x > Screen.width / 2)
								{
									TPmouseDelta = touch.deltaPosition / 10;
								}
								else
								{
									TPmouseDelta = Vector2.zero;
								}

								if (touch.phase == TouchPhase.Ended)
								{
									touchId = -1;
									TPmouseDelta = Vector2.zero;
								}
							}
						}
					}
					
					if(Controller.UIManager.cameraStick)
						Controller.UIManager.cameraStick.gameObject.SetActive(false);
					
					if(Controller.UIManager.cameraStickOutline)
						Controller.UIManager.cameraStickOutline.gameObject.SetActive(false);
				}
				else if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && Controller.CameraParameters.lockCamera)
				{
					useCameraJoystic = false;
					InputHelper.CheckMobileJoystick(Controller.UIManager.cameraStick, Controller.UIManager.cameraStickOutline, ref touchId, Controller.projectSettings, ref MobileTouchjPointA, ref MobileTouchjPointB, ref MobileMoveStickDirection, ref useCameraJoystic);
					GamePadAxis = MobileMoveStickDirection;
				}
			}

			var vector = new Vector2(mouseX, mouseY);

			vector.x += TPmouseDelta.x;
			vector.y -= TPmouseDelta.y;

			
			mouseX = vector.x;
			
			if(Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown)
				mouseY = vector.y;

			mouseY = Helper.ClampAngle(mouseY, Controller.CameraParameters.tpXLimitMin, Controller.CameraParameters.tpXLimitMax);
		}

		public void DeepAim()
		{
			if (!deepAim)
			{
				if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
				{
					if (!CameraAim)
						Aim();
					
					Controller.ChangeCameraType(CharacterHelper.CameraType.FirstPerson);
					deepAim = true;

					SetSensitivity();
				}
			}
			else
			{
				if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson)
				{
					if (CameraAim)
						Aim();
					
					Controller.ChangeCameraType(CharacterHelper.CameraType.ThirdPerson);
					deepAim = false;
				}
			}
		}

		public void Aim()
		{
			if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && !CameraAim)
			{
				CameraAim = true;
				Controller.anim.SetBool("Aim", true);
				normDepth = Camera.fieldOfView;
				
				Reset();
			}
			else if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraAim)
			{
				Controller.anim.SetBool("Aim", false);
				CameraAim = false;
				Reset();
			}
			else if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && !CameraAim)
			{
				CameraAim = true;
				Controller.anim.SetBool("Aim", true);
				normDepth = Camera.fieldOfView;
				Reset();
			}
			else if (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && CameraAim)
			{
				Controller.anim.SetBool("Aim", false);
				CameraAim = false;

				if (deepAim)
				{
					DeepAim();
				}

				Reset();
			}
		}

		void CrosshairAnimation()
		{
			var characterUI = Controller.UIManager.CharacterUI;

			if (Controller.ActiveCharacter && !Controller.AdjustmentScene)
			{

				//mouse cursor adjustment
				var cursorState = Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && Controller.CameraParameters.lockCamera && !useGamepad && !Application.isMobilePlatform && !Controller.projectSettings.mobileDebug ||
				                  (Controller.isPause || cameraPause) && canUseCursorInPause || Controller.projectSettings.mobileDebug && (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && !Controller.CameraParameters.lockCamera || Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown) 
				                  || Controller.WeaponManager.inventoryIsOpened || Controller.PlayerHealth <= 0;

				
				//Cursor.visible = cursorState;

				
				if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && Controller.CameraParameters.lockCamera)
				{
					if (!Controller.isPause && !cameraPause && !Controller.UIManager.CharacterUI.Inventory.MainObject.activeSelf && Controller.PlayerHealth > 0)
					{
						if (Controller.CameraParameters.CursorImage && !cursorImageHasChanged)
						{
							Cursor.SetCursor(Controller.CameraParameters.CursorImage.texture, Vector2.zero, CursorMode.ForceSoftware);
							cursorImageHasChanged = true;
						}
					}
					else
					{
						if (cursorImageHasChanged)
						{
							Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
							cursorImageHasChanged = false;
						}
					}
				}
				else
				{
					if (cursorImageHasChanged)
					{
						Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
						cursorImageHasChanged = false;
					}
				}
				//


				// the crosshair animation and adjustment
				if (characterUI.crosshairMainObject)
				{
					//if game is active
					if (!Controller.isPause && !cameraPause && !Controller.UIManager.CharacterUI.Inventory.MainObject.activeSelf && !Controller.isMultiplayerCharacter)
					{

						//conditions under which the crosshair will be visible
						var crosshairState = !Controller.WeaponManager.isPickUp &&
						                     (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && !Controller.CameraParameters.lockCamera
						                      || Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && Controller.CameraParameters.lockCamera &&  (useGamepad || Application.isMobilePlatform || Controller.projectSettings.mobileDebug)
						                      || Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown) &&
						                     (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson && !CameraAim ||
						                      Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson) &&
						                     (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && CameraAim && !cameraOcclusion ||
						                      Controller.TypeOfCamera != CharacterHelper.CameraType.ThirdPerson) &&
						                     (Controller.TypeOfCamera == CharacterHelper.CameraType.FirstPerson &&
						                      (!Controller.isSprint || Controller.isSprint && !Controller.anim.GetBool("Move")) || Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson) &&
						                     Controller.WeaponManager.slots[Controller.WeaponManager.currentSlot].weaponSlotInGame.Count > 0 &&
						                     !Controller.WeaponManager.slots[Controller.WeaponManager.currentSlot].weaponSlotInGame[Controller.WeaponManager.slots[Controller.WeaponManager.currentSlot].currentWeaponInSlot].fistAttack;

						var gun = Controller.WeaponManager.WeaponController;



						//weapons related conditions
						var crosshairAdditionalState = !gun || gun && !gun.DetectObject && (Controller.TypeOfCamera != CharacterHelper.CameraType.TopDown && !gun.isReloadEnabled || Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown) &&
						                               (gun.Attacks[gun.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Grenade && !gun.isAimEnabled || gun.Attacks[gun.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade) &&
						                               (gun.Attacks[gun.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.GrenadeLauncher && !gun.isAimEnabled || gun.Attacks[gun.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.GrenadeLauncher);

                        //crosshairAdditionalState = true;


                        characterUI.crosshairMainObject.gameObject.SetActive(crosshairState && crosshairAdditionalState);



						if (Controller.anim.GetBool("Attack"))
						{
							crosshairMultiplier = 1.5f;
						}
						else if (Controller.anim.GetFloat("Horizontal") > 0.3f || Controller.anim.GetFloat("Horizontal") < -0.3f ||
						         Controller.anim.GetFloat("Vertical") > 0.3f || Controller.anim.GetFloat("Vertical") < -0.3f)
						{
							crosshairMultiplier = 2;
						}
						else
						{
							crosshairMultiplier = 1;
						}

						if (Controller.anim.GetBool("Aim"))
						{
//							crosshairMultiplayer /= 1.5f;
						}
						else
						{
//							crosshairMultiplayer *= 1.5f;
						}


						// the crosshair parts adjustment
						if (Controller.WeaponManager.WeaponController)
						{
							UIHelper.CalculateCrosshairPartsPositions(Controller, crosshairMultiplier, ref currentCrosshairPositions);

							characterUI.topCrosshairPart.rectTransform.anchoredPosition = currentCrosshairPositions[1];
							characterUI.bottomCrosshairPart.rectTransform.anchoredPosition = currentCrosshairPositions[2];
							characterUI.rightCrosshairPart.rectTransform.anchoredPosition = currentCrosshairPositions[3];
							characterUI.leftCrosshairPart.rectTransform.anchoredPosition = currentCrosshairPositions[4];

							if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.tdModeLikeTp)
							{
								characterUI.middleCrosshairPart.rectTransform.gameObject.SetActive(true);

								if (characterUI.middleCrosshairPart.gameObject.GetComponent<Outline>())
									characterUI.middleCrosshairPart.gameObject.GetComponent<Outline>().enabled = false;

								characterUI.DisableAllCrosshairParts();
							}
							else
							{
								characterUI.ManageCrosshairParts(Controller.WeaponManager.WeaponController.Attacks[Controller.WeaponManager.WeaponController.currentAttack].sightType);
							}
						}
						else
						{
							characterUI.middleCrosshairPart.gameObject.SetActive(false);

							characterUI.DisableAllCrosshairParts();
						}
						//


						//adjusting the crosshair position on the screen
						if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown || Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson && Controller.tdModeLikeTp)
						{
							if (!Controller.CameraParameters.lockCamera)
							{
								if (Cursor.lockState != CursorLockMode.Locked && !Controller.projectSettings.mobileDebug)
								{
									Cursor.lockState = CursorLockMode.Locked;
								}

								if (characterUI.crosshairMainObject.anchoredPosition != new Vector2(CrosshairOffsetX, CrosshairOffsetY))
								{
									characterUI.crosshairMainObject.anchorMin = new Vector2(0.5f, 0.5f);
									characterUI.crosshairMainObject.anchorMax = new Vector2(0.5f, 0.5f);

									characterUI.crosshairMainObject.anchoredPosition = new Vector2(CrosshairOffsetX, CrosshairOffsetY);
									characterUI.crosshairMainObject.gameObject.SetActive(false);
									characterUI.crosshairMainObject.gameObject.SetActive(true);
								}
							}
							else
							{
								if (Cursor.lockState != CursorLockMode.Confined && !Controller.projectSettings.mobileDebug)
								{
									Cursor.lockState = CursorLockMode.Confined;
								}

								if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown && Controller.CameraParameters.lockCamera && (useGamepad || Application.isMobilePlatform || Controller.projectSettings.mobileDebug))
								{
									if (characterUI.crosshairMainObject.anchorMin != new Vector2(0.5f, 0.5f))
									{
										characterUI.crosshairMainObject.anchorMin = new Vector2(0.5f, 0.5f);
										characterUI.crosshairMainObject.anchorMax = new Vector2(0.5f, 0.5f);

										characterUI.crosshairMainObject.gameObject.SetActive(false);
										characterUI.crosshairMainObject.gameObject.SetActive(true);
									}

									var position = Camera.WorldToScreenPoint(BodyLookAt.position);
									characterUI.crosshairMainObject.anchoredPosition = new Vector2(position.x - Screen.width / 2, position.y - Screen.height / 2);
								}
							}
						}
						else
						{
							if (Cursor.lockState != CursorLockMode.Locked && !Controller.projectSettings.mobileDebug)
							{
								Cursor.lockState = CursorLockMode.Locked;
							}

							if (characterUI.crosshairMainObject.anchoredPosition != new Vector2(0, 0))
							{
								characterUI.crosshairMainObject.anchorMin = new Vector2(0.5f, 0.5f);
								characterUI.crosshairMainObject.anchorMax = new Vector2(0.5f, 0.5f);

								characterUI.crosshairMainObject.anchoredPosition = new Vector2(0, 0);

								characterUI.crosshairMainObject.gameObject.SetActive(false);
								characterUI.crosshairMainObject.gameObject.SetActive(true);
							}
						}
						//

						//the pick-up icon adjustment
						if (!Application.isMobilePlatform && !Controller.projectSettings.mobileDebug)
						{
                            if (Controller.UIManager.CharacterUI.PickupHUD)
                            {
                                //Controller.UIManager.CharacterUI.PickupHUD.gameObject.SetActive(Controller.WeaponManager.isPickUp);
                                //Debug.Log(Controller.WeaponManager.isPickUp);

                            }
						}
						else
						{
							if (Controller.UIManager.uiButtons[11])
								Controller.UIManager.uiButtons[11].gameObject.SetActive(Controller.WeaponManager.isPickUp);
						}
						//
					}
					// if game isn't active, disable the crosshair and pick-up icons
					else
					{
						characterUI.crosshairMainObject.gameObject.SetActive(false);

						if (Controller.UIManager.CharacterUI.PickupHUD)
							Controller.UIManager.CharacterUI.PickupHUD.gameObject.SetActive(false);

						if (Controller.UIManager.uiButtons[11])
							Controller.UIManager.uiButtons[11].gameObject.SetActive(false);

						if (Controller.isPause || cameraPause || Controller.UIManager.CharacterUI.Inventory.MainObject.activeSelf || Controller.projectSettings.mobileDebug)
						{
							if (Cursor.lockState != CursorLockMode.None)
							{
								Cursor.lockState = CursorLockMode.None;
							}
						}
					}
				}

			}
		}

		void DisableAimTextures()
		{
			if (Controller.WeaponManager.WeaponController)
			{
				if (Controller.WeaponManager.WeaponController.useAimTexture && Controller.UIManager.CharacterUI.aimPlaceholder)
				{
					var color = Controller.UIManager.CharacterUI.aimPlaceholder.color;

					color.a = Mathf.Lerp(color.a, 0, 0.5f);

					Controller.UIManager.CharacterUI.aimPlaceholder.color = color;

					if (color.a <= 0.05f)
					{
						Controller.UIManager.CharacterUI.aimPlaceholder.gameObject.SetActive(false);
					}
				}
			}

			if (Controller.WeaponManager.WeaponController)
			{
				if (Controller.WeaponManager.WeaponController.useScope)
				{
					if (!AimCamera.gameObject.activeSelf)
						AimCamera.gameObject.SetActive(true);

					AimCamera.fieldOfView = Mathf.Lerp(AimCamera.fieldOfView, Controller.WeaponManager.WeaponController.scopeDepth, 0.5f);
					AimCamera.targetTexture = Controller.WeaponManager.ScopeScreenTexture;
				}
				else
				{
					AimCamera.targetTexture = null;
					if (AimCamera.gameObject.activeSelf)
						AimCamera.gameObject.SetActive(false);
				}
			}

			if (Controller.TypeOfCamera == CharacterHelper.CameraType.TopDown)
				AimCamera.gameObject.SetActive(false);
		}

		public void SetSensitivity()
		{
			if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
			{
				if (CameraAim)
				{
					CurrentSensitivityX = Controller.CameraParameters.tpAimXMouseSensitivity;
					CurrentSensitivityY = Controller.CameraParameters.tpAimYMouseSensitivity;
				}
				else
				{
					CurrentSensitivityX = Controller.CameraParameters.tpXMouseSensitivity;
					CurrentSensitivityY = Controller.CameraParameters.tpYMouseSensitivity;
				}
			}
			else
			{
				if (CameraAim)
				{
					if (Controller.WeaponManager.WeaponController && Controller.WeaponManager.WeaponController.useAimTexture)
					{
						CurrentSensitivityX = Controller.CameraParameters.fpAimXMouseSensitivity / 10;
						CurrentSensitivityY = Controller.CameraParameters.fpAimYMouseSensitivity / 10;
					}
					else
					{
						CurrentSensitivityX = Controller.CameraParameters.fpAimXMouseSensitivity;
						CurrentSensitivityY = Controller.CameraParameters.fpAimYMouseSensitivity;
					}
				}
				else
				{
					CurrentSensitivityX = Controller.CameraParameters.fpXMouseSensitivity;
					CurrentSensitivityY = Controller.CameraParameters.fpYMouseSensitivity;
				}
			}
		}

		public void ReloadParameters()
		{
			if(CameraAim)
				Aim();

			if (Controller.TypeOfCamera != CharacterHelper.CameraType.FirstPerson)
			{
				mouseX = Controller.transform.localEulerAngles.y;
				mouseY = Controller.transform.localEulerAngles.x;
			}
			else
			{
				mouseX = 0;
				mouseY = 0;
				Controller.BodyObjects.TopBody.localEulerAngles = Vector3.zero;
			}

			CharacterHelper.ResetCameraParameters(Controller.TypeOfCamera, Controller.TypeOfCamera, Controller);
		}

		public void Reset()
		{
			if(!Controller)
				return;
			
			switch (Controller.TypeOfCamera)
			{
				case CharacterHelper.CameraType.ThirdPerson:
				{
					SetSensitivity();
					
					Helper.CameraExtensions.LayerCullingShow(Camera, "Head");
					Helper.CameraExtensions.LayerCullingShow(AimCamera, "Head");
					Helper.CameraExtensions.LayerCullingShow(LayerCamera.GetComponent<Camera>(), "Head");
					
					if(Controller.tdModeLikeTp)
						break;

					CurrentDistance = CameraOffset.Distance;
					desiredDistance = CurrentDistance;

					CurrentOffsetX = CameraOffset.tpCameraOffsetX;
					CurrentOffsetY = CameraOffset.tpCameraOffsetY;

					desiredOffsetX = CurrentOffsetX;
					
					if (preOcclededCamera)
						preOcclededCamera.localPosition = new Vector3(CameraOffset.tpCameraOffsetX, CameraOffset.tpCameraOffsetY, CameraOffset.Distance);

					break;
				}

				case CharacterHelper.CameraType.TopDown:
				{
					Helper.CameraExtensions.LayerCullingShow(Camera, "Head");
					Helper.CameraExtensions.LayerCullingShow(AimCamera, "Head");
					Helper.CameraExtensions.LayerCullingShow(LayerCamera.GetComponent<Camera>(), "Head");
					
					CurrentSensitivityX = Controller.CameraParameters.tdXMouseSensitivity;
					CurrentSensitivityY = Controller.CameraParameters.tdXMouseSensitivity;
					break;
				}

				case CharacterHelper.CameraType.FirstPerson:
				{
					Helper.CameraExtensions.LayerCullingHide(Camera, "Head");
					Helper.CameraExtensions.LayerCullingHide(AimCamera, "Head");
					Helper.CameraExtensions.LayerCullingHide(LayerCamera.GetComponent<Camera>(), "Head");
					
					SetSensitivity();
					break;
				}
			}
		}

		public void SetAnimVariables()
		{
			switch (Controller.TypeOfCamera)
			{
				case CharacterHelper.CameraType.ThirdPerson:
				case CharacterHelper.CameraType.TopDown:

					if (Controller.TypeOfCamera == CharacterHelper.CameraType.ThirdPerson)
						Controller.anim.SetBool("TPS", true);
					else Controller.anim.SetBool("TDS", true);
					
					if (Controller.WeaponManager.hasAnyWeapon)
					{
						Controller.anim.SetLayerWeight(2, 1);
						Controller.anim.SetLayerWeight(1, 0);

						Controller.currentAnimatorLayer = 2;
					}

					break;

				case CharacterHelper.CameraType.FirstPerson:

					Controller.anim.SetBool("FPS", true);
					Controller.anim.SetLayerWeight(2, 0);
					Controller.anim.SetLayerWeight(3, 0);
					Controller.anim.SetLayerWeight(1, 1);
					Controller.currentAnimatorLayer = 1;

					break;
			}
		}

		void tpCheckIfOccluded()
		{
			RaycastHit Hit;
			var nearestDistance = CharacterHelper.CheckCameraPoints(Controller.BodyObjects.Head.position, transform.position, disabledObjects, transform, Camera);

			if (nearestDistance > -1)
			{
				desiredDistance += 0.2f;

				if (desiredDistance > 4.5f)
					desiredDistance = 4.5f;

				if (Physics.Raycast(transform.position - transform.right * 2, transform.right, out Hit, 5))
				{
					desiredOffsetX -= 0.1f;

					if (desiredOffsetX < 0.3f)
						desiredOffsetX = 0.3f;
				}
			}
			else
			{
				nearestDistance = CharacterHelper.CheckCameraPoints(Controller.BodyObjects.Head.position, preOcclededCamera.position, disabledObjects, transform, Camera);

				var canChangeDist = true;

				var layerMask = ~ (LayerMask.GetMask("Grass") | LayerMask.GetMask("Character") | LayerMask.GetMask("Enemy") | LayerMask.GetMask("Head"));

				var size = Physics.OverlapSphereNonAlloc(preOcclededCamera.position, 0.5f, _occlusionCollisers, layerMask);
				
				if (size > 0)
				{
					canChangeDist = false;
					cameraOcclusion = true;
				}

				if (!Physics.Raycast(transform.position - transform.right * 3, transform.right, out Hit, 6) &&
				    !Physics.Raycast(transform.position + transform.right * 3, -transform.right, out Hit, 6) && canChangeDist)
				{
					desiredOffsetX += 0.1f;

					if (desiredOffsetX > CameraOffset.tpCameraOffsetX)
						desiredOffsetX = CameraOffset.tpCameraOffsetX;
				}

				if (nearestDistance <= -1 && canChangeDist)
				{
					desiredDistance += 0.01f;
					
					if (desiredDistance > CameraOffset.Distance)
						desiredDistance = CameraOffset.Distance;
				}
				
				if(Math.Abs(desiredOffsetX - CameraOffset.tpCameraOffsetX) < 0.2f && Math.Abs(desiredDistance - CameraOffset.Distance) < 0.2f)
					cameraOcclusion = false;
			}
		}

		private void OnDrawGizmos()
		{
//			if (BodyLookAt)
//			{
//				Gizmos.color = Color.red;
//				Gizmos.DrawSphere(BodyLookAt.position, 1);
//			}
		}
	}
}


