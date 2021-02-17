using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GercStudio.USK.Scripts
{
	public static class InputHelper
	{
		public static void CheckMobileJoystick(GameObject Stick, GameObject Outline, ref int touchId, ProjectSettings projectSettings, ref Vector2 MobileTouchjPointA, ref Vector2 MobileTouchjPointB, ref Vector2 MobileMoveStickDirection, Controller controller)
		{
			var empty = false;
			MobileJoystick(Stick, Outline, ref touchId, projectSettings, ref MobileTouchjPointA, ref MobileTouchjPointB, ref MobileMoveStickDirection, controller, "controller", ref empty);
		}
		
		public static void CheckMobileJoystick(GameObject Stick, GameObject Outline, ref int touchId, ProjectSettings projectSettings, ref Vector2 MobileTouchjPointA, ref Vector2 MobileTouchjPointB, ref Vector2 MobileMoveStickDirection, ref bool useJoystic)
		{
			Controller empty = null;
			MobileJoystick(Stick, Outline, ref touchId, projectSettings, ref MobileTouchjPointA, ref MobileTouchjPointB, ref MobileMoveStickDirection, empty, "camera", ref useJoystic);
		}

		static void MobileJoystick(GameObject Stick, GameObject Outline,ref int touchId, ProjectSettings projectSettings, ref Vector2 MobileTouchjPointA, ref Vector2 MobileTouchjPointB, ref Vector2 MobileMoveStickDirection, Controller controller, string Type, ref bool useJoystic)
		{
			if (Input.touches.Length > 0)
			{
				for (var i = 0; i < Input.touches.Length; i++)
				{
					var touch = Input.GetTouch(i);

					if (touchId == -1 && touch.phase == TouchPhase.Began && 
					    (Type == "controller" ? touch.position.x < Screen.width / 2 : touch.position.x > Screen.width / 2) && touch.position.y < Screen.height / 2)
					{
						var eventSystem = GameObject.FindObjectOfType<EventSystem>();

						if (!eventSystem.currentInputModule.IsPointerOverGameObject(touch.fingerId))
						{
							touchId = touch.fingerId;

							MobileTouchjPointA = touch.position;

							Stick.gameObject.SetActive(true);
							Outline.gameObject.SetActive(true);

							Stick.transform.position = MobileTouchjPointA;
							Outline.transform.position = MobileTouchjPointA;
						}
					}

					if (touch.fingerId == touchId)
					{
//						if ((Type == "controller" ? touch.position.x < Screen.width / 2 : touch.position.x > Screen.width / 2)) //&& touch.position.y < Screen.height / 2)
//						{
							MobileTouchjPointB = new Vector2(touch.position.x, touch.position.y);

							var offset = MobileTouchjPointB - MobileTouchjPointA;

							if (Type == "controller")
							{
								if (offset.x > 0.1f || offset.y > 0.1f || offset.x < -0.1 || offset.y < -0.1f)
								{
									if (!controller.anim.GetBool("Move"))
										controller.anim.SetBool("MoveButtonHasPressed", true);
									
									controller.hasMoveButtonPressed = true;
								}
							}
							else
							{
								if (offset.x > 0.1f || offset.y > 0.1f || offset.x < -0.1 || offset.y < -0.1f)
								{
									useJoystic = true;
								}
							}

							MobileMoveStickDirection = Vector2.ClampMagnitude(offset, projectSettings.StickRange);

							Stick.transform.position = new Vector2(
								MobileTouchjPointA.x + MobileMoveStickDirection.x,
								MobileTouchjPointA.y + MobileMoveStickDirection.y);

							MobileMoveStickDirection /= projectSettings.StickRange;
//						}
						if (touch.phase == TouchPhase.Ended)
						{
							touchId = -1;
							MobileMoveStickDirection = Vector2.zero;
							Stick.gameObject.SetActive(false);
							Outline.gameObject.SetActive(false);
						}
					}
				}
			}
			else
			{
				Stick.gameObject.SetActive(false);
				Outline.gameObject.SetActive(false);
			}
		}
		
		public static void CheckTouchCamera(ref int touchId, ref Vector2 mouseDelta, Controller controller)
		{
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touches.Length; i++)
				{
					var touch = Input.GetTouch(i);

					if (touch.position.x > Screen.width / 2 & touchId == -1 & touch.phase == TouchPhase.Began)
					{
						touchId = touch.fingerId;
					}

					if (touch.fingerId == touchId)
					{
						if (touch.position.x > Screen.width / 2)
							mouseDelta = touch.deltaPosition / 75;
						else
						{
							mouseDelta = Vector2.zero;
						}

						if (touch.phase == TouchPhase.Ended)
						{
							touchId = -1;
							mouseDelta = Vector2.zero;
						}
					}
				}
			}
			
			if(controller.UIManager.cameraStick)
				controller.UIManager.cameraStick.gameObject.SetActive(false);
					
			if(controller.UIManager.cameraStickOutline)
				controller.UIManager.cameraStickOutline.gameObject.SetActive(false);
		}
	}
}
