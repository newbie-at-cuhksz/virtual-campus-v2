using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GercStudio.USK.Scripts
{
	public static class UIHelper
	{
		[Serializable]
		public class singlePlayerGame
		{
			public GameObject PauseMainObject;
			public GameObject OptionsMainObject;
			
			public Button Exit;
			public Button Resume;
			public Button Options;
			public Button Restart;
			public Button OptionsBack;
			
			[Serializable]
			public class GraphicsButtons
			{
				public Button Button;
				public int QualityIndex;

				public void ActivateAll()
				{
					if(Button)
						Button.gameObject.SetActive(true);
				}
			}

			public List<GraphicsButtons> _GraphicsButtons = new List<GraphicsButtons>{new GraphicsButtons()};

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}

				foreach (var button in _GraphicsButtons)
				{
					button.ActivateAll();
				}
			}
		}
		
		[Serializable]
		public class CharacterUI
		{
			public GameObject MainObject;
			public Text WeaponAmmo;
			public Text Health;
			public Image HealthBar;
			public RawImage WeaponAmmoImagePlaceholder;
			public RawImage PickupImage;
			public RawImage bloodSplatter;
			public List<RawImage> hitMarkers;

			public RectTransform crosshairMainObject;
			
			public RawImage aimPlaceholder;
			
			public Image flashPlaceholder;
			
			public Image topCrosshairPart;
			public Image bottomCrosshairPart;
			public Image leftCrosshairPart;
			public Image rightCrosshairPart;
			public Image middleCrosshairPart;


			public Inventory Inventory;

			public void DisableAllCrosshairParts()
			{
				if (rightCrosshairPart)
					rightCrosshairPart.gameObject.SetActive(false);

				if (leftCrosshairPart)
					leftCrosshairPart.gameObject.SetActive(false);

				if (topCrosshairPart)
					topCrosshairPart.gameObject.SetActive(false);

				if (bottomCrosshairPart)
					bottomCrosshairPart.gameObject.SetActive(false);
			}

			public void ManageCrosshairParts(WeaponsHelper.CrosshairType type)
			{
				switch (type)
				{
					case WeaponsHelper.CrosshairType.OnePart:

						if (middleCrosshairPart)
							middleCrosshairPart.gameObject.SetActive(middleCrosshairPart.sprite);

						if (rightCrosshairPart)
							rightCrosshairPart.gameObject.SetActive(false);

						if (leftCrosshairPart)
							leftCrosshairPart.gameObject.SetActive(false);

						if (topCrosshairPart)
							topCrosshairPart.gameObject.SetActive(false);

						if (bottomCrosshairPart)
							bottomCrosshairPart.gameObject.SetActive(false);

						break;
					case WeaponsHelper.CrosshairType.TwoParts:

						if (middleCrosshairPart)
							middleCrosshairPart.gameObject.SetActive(middleCrosshairPart.sprite);

						if (rightCrosshairPart)
							rightCrosshairPart.gameObject.SetActive(true);

						if (leftCrosshairPart)
							leftCrosshairPart.gameObject.SetActive(true);

						if (topCrosshairPart)
							topCrosshairPart.gameObject.SetActive(false);

						if (bottomCrosshairPart)
							bottomCrosshairPart.gameObject.SetActive(false);

						break;
					case WeaponsHelper.CrosshairType.FourParts:

						if (middleCrosshairPart)
							middleCrosshairPart.gameObject.SetActive(middleCrosshairPart.sprite);

						if (rightCrosshairPart)
							rightCrosshairPart.gameObject.SetActive(true);
						
						if (leftCrosshairPart)
							leftCrosshairPart.gameObject.SetActive(true);

						if (topCrosshairPart)
							topCrosshairPart.gameObject.SetActive(true);
						
						if (bottomCrosshairPart)
							bottomCrosshairPart.gameObject.SetActive(true);

						break;

				}
			}

			public void ActivateAll()
			{
				if (MainObject)
					Helper.EnableAllParents(MainObject);

				if (WeaponAmmo)
					Helper.EnableAllParents(WeaponAmmo.gameObject);

				if (WeaponAmmoImagePlaceholder)
					Helper.EnableAllParents(WeaponAmmoImagePlaceholder.gameObject);

				if (Health)
					Helper.EnableAllParents(Health.gameObject);

				if (HealthBar)
					Helper.EnableAllParents(HealthBar.gameObject);
			}

			public void DisableAll()
			{
				if (MainObject)
					MainObject.SetActive(false);

				if (WeaponAmmo)
					WeaponAmmo.gameObject.SetActive(false);

				if (WeaponAmmoImagePlaceholder)
					WeaponAmmoImagePlaceholder.gameObject.SetActive(false);

				if (Health)
					Health.gameObject.SetActive(false);

				if (HealthBar)
					HealthBar.gameObject.SetActive(false);

				if (PickupImage)
					PickupImage.gameObject.SetActive(false);

				Inventory.MainObject.SetActive(false);
			}

			public void ShowImage(string type, InventoryManager inventoryManager)
			{
				switch (type)
				{
					case "weapon":
					{
						for (var i = 0; i < 8; i++)
						{
							if (inventoryManager.slots[i].weaponSlotInGame.Count <= 0)
							{
								var slotButton = Inventory.WeaponsButtons[i];

								if (!slotButton)
									continue;

								slotButton.interactable = false;

								Helper.ChangeButtonColor(inventoryManager.Controller.UIManager, i, "norm");

								if (Inventory.WeaponImagePlaceholder[i])
								{
									var img = Inventory.WeaponImagePlaceholder[i];
									img.color = new Color(1, 1, 1, 0);
								}

								if (Inventory.WeaponAmmoText[i])
									Inventory.WeaponAmmoText[i].gameObject.SetActive(false);

								continue;
							}

							if (Inventory.WeaponsButtons[i])
								Inventory.WeaponsButtons[i].interactable = true;

							if (!inventoryManager.slots[i].weaponSlotInGame[inventoryManager.slots[i].currentWeaponInSlot].fistAttack)
							{
								var weaponController = inventoryManager.slots[i].weaponSlotInGame[inventoryManager.slots[i].currentWeaponInSlot].weapon.GetComponent<WeaponController>();

								if (!weaponController.WeaponImage || !Inventory.WeaponsButtons[i])
									continue;

								var image = Inventory.WeaponImagePlaceholder[i];

								image.texture = weaponController.WeaponImage;

								image.color = new Color(1, 1, 1, 1);

								if (Inventory.WeaponAmmoText[i])
								{
									Inventory.WeaponAmmoText[i].gameObject.SetActive(true);
									if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee)
									{
										if (weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade)
										{
											Inventory.WeaponAmmoText[i].text = weaponController.Attacks[weaponController.currentAttack].curAmmo.ToString("F0") + "/" +
											                                   weaponController.Attacks[weaponController.currentAttack].inventoryAmmo;
										}
										else
										{
											Inventory.WeaponAmmoText[i].text = weaponController.Attacks[weaponController.currentAttack].curAmmo.ToString("F0");
										}
									}
									else
									{
										Inventory.WeaponAmmoText[i].text = inventoryManager.slots[i].weaponSlotInGame[inventoryManager.slots[i].currentWeaponInSlot].weapon.name;
									}
								}
							}
							else
							{
								if (!Inventory.WeaponImagePlaceholder[i] || !inventoryManager.FistIcon)
									continue;

								var image = Inventory.WeaponImagePlaceholder[i];

								image.texture = inventoryManager.FistIcon;

								image.color = new Color(1, 1, 1, 1);

								if (Inventory.WeaponAmmoText[i])
									Inventory.WeaponAmmoText[i].text = " ";
							}
						}

						break;
					}
					case "health":

						if (Inventory.HealthButton)
							Inventory.HealthButton.interactable = inventoryManager.HealthKits.Count > 0;

						if (Inventory.UpHealthButton)
							Inventory.HealthButton.interactable = inventoryManager.HealthKits.Count > 0;

						if (Inventory.DownHealthButton)
							Inventory.DownHealthButton.interactable = inventoryManager.HealthKits.Count > 0;

						if (Inventory.HealthKitsCount)
							Inventory.HealthKitsCount.text = inventoryManager.HealthKits.Count > 0 ? inventoryManager.currentHealthKit + 1 + "/" + inventoryManager.HealthKits.Count : "0";

						if (Inventory.CurrentHealthValue)
						{
							Inventory.CurrentHealthValue.gameObject.SetActive(inventoryManager.HealthKits.Count > 0);

							if (inventoryManager.HealthKits.Count > 0)
								Inventory.CurrentHealthValue.text = "+ " + inventoryManager.HealthKits[inventoryManager.currentHealthKit].AddedValue;
							else Inventory.CurrentHealthValue.text = " ";
						}

						if (Inventory.HealthImage)
							Inventory.HealthImage.color = new Color(1, 1, 1, inventoryManager.HealthKits.Count > 0 ? 1 : 0);

						foreach (var kit in inventoryManager.HealthKits)
						{
							if (inventoryManager.HealthKits.IndexOf(kit) == inventoryManager.currentHealthKit)
								if (Inventory.HealthImage)
									Inventory.HealthImage.texture = kit.Image;
						}

						break;

					case "ammo":

						if (inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame.Count > 0)
						{
							if (!inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].fistAttack)
							{
								var weaponController = inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].weapon.GetComponent<WeaponController>();

								if (inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count > 0 &&
								    weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee)
								{

									if (Inventory.AmmoButton)
										Inventory.AmmoButton.interactable = true;

									if (Inventory.UpAmmoButton)
										Inventory.UpAmmoButton.interactable = true;

									if (Inventory.DownAmmoButton)
										Inventory.DownAmmoButton.interactable = true;

									if (Inventory.AmmoKitsCount)
										Inventory.AmmoKitsCount.text = inventoryManager.currentAmmoKit + 1 + "/" + inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].WeaponAmmoKits.Count;

									if (Inventory.CurrentAmmoValue)
									{
										Inventory.CurrentAmmoValue.gameObject.SetActive(true);
										Inventory.CurrentAmmoValue.text = "+ " + inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].WeaponAmmoKits[inventoryManager.currentAmmoKit].AddedValue;
									}


									if (Inventory.AmmoImage)
										Inventory.AmmoImage.color = new Color(1, 1, 1, 1);
									Inventory.AmmoImage.texture = inventoryManager.slots[inventoryManager.currentSlot].weaponSlotInGame[inventoryManager.slots[inventoryManager.currentSlot].currentWeaponInSlot].WeaponAmmoKits[inventoryManager.currentAmmoKit].Image;

								}
								else NotActiveAmmoKits();

							}
							else NotActiveAmmoKits();

						}
						else NotActiveAmmoKits();

						break;
				}
			}

			private void NotActiveAmmoKits()
			{
				if (Inventory.AmmoButton)
					Inventory.AmmoButton.interactable = false;

				if (Inventory.UpAmmoButton)
					Inventory.UpAmmoButton.interactable = false;

				if (Inventory.DownAmmoButton)
					Inventory.DownAmmoButton.interactable = false;

				if (Inventory.AmmoKitsCount)
					Inventory.AmmoKitsCount.text = "0";

				if (Inventory.CurrentAmmoValue)
					Inventory.CurrentAmmoValue.gameObject.SetActive(false);

				if (Inventory.AmmoImage)
					Inventory.AmmoImage.color = new Color(1, 1, 1, 0);
			}
		}

		[Serializable]
		public class Inventory
		{
			public GameObject MainObject;

			public Button[] WeaponsButtons = new Button[8];
			public Text[] WeaponAmmoText = new Text[8];
			public RawImage[] WeaponImagePlaceholder = new RawImage[8];

			public Button UpWeaponButton;
			public Button DownWeaponButton;
			public Button UpHealthButton;
			public Button DownHealthButton;
			public Button UpAmmoButton;
			public Button DownAmmoButton;
			public Button AmmoButton;
			public Button HealthButton;

			public Text WeaponsCount;
			public Text AmmoKitsCount;
			public Text CurrentAmmoValue;
			public Text HealthKitsCount;
			public Text CurrentHealthValue;
			
			public RawImage HealthImage;
			public RawImage AmmoImage;

			public Color[] normButtonsColors = new Color[10];
			public Sprite[] normButtonsSprites = new Sprite[10];

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(RawImage))
					{
						var go = (RawImage) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}
			}
		}
		
		[Serializable]
		public class AllRoomsMenu
		{
			public GameObject MainObject;
			
			public Transform Content;

			public InputField Password;

			public Button BackButton;
			public Button JoinButton;

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);
				
				if(Content)
					Content.gameObject.SetActive(false);
				
				if(BackButton)
					BackButton.gameObject.SetActive(false);
				
				if(JoinButton)
					JoinButton.gameObject.SetActive(false);
				
				if(Password)
					Password.gameObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(Content)
					Helper.EnableAllParents(Content.gameObject);
				
				if(BackButton)
					Helper.EnableAllParents(BackButton.gameObject);
			}
		}

		[Serializable]
		public class CreateRoomMenu
		{
			public GameObject MainObject;

			public Button BackButton;
			public Button CreateButton;

			public InputField GameName;
			public InputField Password;

			public Text CurrentModeAndMap;

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(InputField))
					{
						var go = (InputField) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}

				if (CurrentModeAndMap)
					CurrentModeAndMap.gameObject.SetActive(false);

				if (MainObject)
					MainObject.SetActive(false);
			}

			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(InputField))
					{
						var go = (InputField) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}

				if (CurrentModeAndMap)
					Helper.EnableAllParents(CurrentModeAndMap.gameObject);

				if (MainObject)
					Helper.EnableAllParents(MainObject);
			}
		}

		[Serializable]
		public class LobbyMainUI
		{
			public GameObject MainObject;
			
			public Text ConnectionStatus;
			public Text CurrentModeAndMap;
			public Text PlayerScore;

			public RawImage Avatar;

			public Dropdown RegionsDropdown;

			public InputField Nickname;

			public Button ChooseGameModeButton;
			public Button PlayButton;
			public Button ChangeAvatarButton;
			public Button ChangeCharacter;
			public Button LoadoutButton;
			public Button AllRoomsButton;
			public Button CreateRoomButton;

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
				
				if(RegionsDropdown)
					RegionsDropdown.gameObject.SetActive(false);
				
				if(Nickname)
					Nickname.gameObject.SetActive(false);
				
				if(Avatar)
					Avatar.gameObject.SetActive(false);
					
				if(MainObject)
					MainObject.gameObject.SetActive(false);
			}

			public void ActivateAll(bool activateRegionsDropdown)
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}
				
				if(RegionsDropdown && activateRegionsDropdown)
					Helper.EnableAllParents(RegionsDropdown.gameObject);
				
				if(Nickname)
					Helper.EnableAllParents(Nickname.gameObject);
				
				if(Avatar)
					Helper.EnableAllParents(Avatar.gameObject);
					
				if(MainObject)
					Helper.EnableAllParents(MainObject.gameObject);
			}
		}
		
		[Serializable]
		public class LobbyGameModesUI
		{
			public GameObject MainObject;

			public Transform Content;
			
			public Text Info;
			public Text MapButtonText;
			
			public Button MapsButton;
			public Button BackButton;

			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
				
				if(Content)
					Content.gameObject.SetActive(false);
			}
			
			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}
				
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(Content)
					Helper.EnableAllParents(Content.gameObject);
			}
		}
		
		[Serializable]
		public class LobbyMapsUI
		{
			public GameObject MainObject;

			public Transform Content;

			public Button GameModesButton;
			public Button BackButton;

			public Text GameModesButtonText;
			
			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Transform))
					{
						var go = (Transform) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}

				if (GameModesButtonText)
					GameModesButtonText.gameObject.SetActive(false);
				
			}
			
			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Transform))
					{
						var go = (Transform) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}
				
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(GameModesButtonText)
					Helper.EnableAllParents(GameModesButtonText.gameObject);
			}
		}

		[Serializable]
		public class LobbyCharactersMenu
		{
			public GameObject MainObject;
			
			public Button UpButton;
			public Button DownButton;
			
			public Button BackButton;

			public void ActivateAll()
			{
				if(MainObject)
					Helper.EnableAllParents(MainObject);
				
				if(UpButton)
					Helper.EnableAllParents(UpButton.gameObject);
				
				if(DownButton)
					Helper.EnableAllParents(DownButton.gameObject);
				
				if(BackButton)
					Helper.EnableAllParents(BackButton.gameObject);
				
			}

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);
				
				if(UpButton)
					UpButton.gameObject.SetActive(false);
				
				if(DownButton)
					DownButton.gameObject.SetActive(false);
				
				if(BackButton)
					BackButton.gameObject.SetActive(false);
			}
		}
		
		[Serializable]
		public class LobbyLoadoutUI
		{
			public GameObject MainObject;

			public Transform Content;

			public Button EquipButton;
			public Button BackButton;
			
			public Text Info;
			public Text EquipButtonText;
			
			public void DisableAll()
			{
				foreach (var field in GetType().GetFields())
				{
					if (field.FieldType == typeof(GameObject))
					{
						var go = (GameObject) field.GetValue(this);
						if (go) go.SetActive(false);
					}
					else if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Transform))
					{
						var go = (Transform) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) go.gameObject.SetActive(false);
					}
				}
			}
			
			public void ActivateAll()
			{
				foreach (var field in GetType().GetFields())
				{ 
					if (field.FieldType == typeof(Button))
					{
						var go = (Button) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Transform))
					{
						var go = (Transform) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
					else if (field.FieldType == typeof(Text))
					{
						var go = (Text) field.GetValue(this);
						if (go) Helper.EnableAllParents(go.gameObject);
					}
				}

				if(MainObject)
					Helper.EnableAllParents(MainObject);
			}
		}

		[Serializable]
		public class AvatarsMenu
		{
			public GameObject MainObject;

			public Transform Content;

			public Button BackButton;

			public void DisableAll()
			{
				if(MainObject)
					MainObject.SetActive(false);

				if (Content)
					Content.gameObject.SetActive(false);
				
				if(BackButton)
					BackButton.gameObject.SetActive(false);
			}

			public void ActivateAll()
			{
				if(MainObject)
					Helper.EnableAllParents(MainObject);	
				
				if(Content)
					Helper.EnableAllParents(Content.gameObject);
				
				if(BackButton)
					Helper.EnableAllParents(BackButton.gameObject);
			}
		}

		public static void MangeButtons(Button button, UnityAction Void, ref Color color, ref Sprite sprite)
		{
			button.onClick.AddListener(Void);
			button.gameObject.SetActive(false);
				switch (button.transition)
				{
					case Selectable.Transition.ColorTint:
						color = button.colors.normalColor;
						break;
					case Selectable.Transition.SpriteSwap:
						sprite = button.GetComponent<Image>().sprite;
						break;
				}
			
		}

		public static void CalculateCrosshairPartsPositions(Controller controller, float crosshairMultiplier, ref Vector2[] currentCrosshairPositions)
		{
			if (Math.Abs(crosshairMultiplier - 1) > 0.1f)
			{
				currentCrosshairPositions[3] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[3].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[3].x * crosshairMultiplier, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[3].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[3].y * crosshairMultiplier, 5 * Time.deltaTime));

				currentCrosshairPositions[4] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[4].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[4].x * crosshairMultiplier, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[4].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[4].y * crosshairMultiplier, 5 * Time.deltaTime));

				currentCrosshairPositions[1] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[1].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[1].x * crosshairMultiplier, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[1].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[1].y * crosshairMultiplier, 5 * Time.deltaTime));

				currentCrosshairPositions[2] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[2].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[2].x * crosshairMultiplier, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[2].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[2].y * crosshairMultiplier, 5 * Time.deltaTime));

			}
			else
			{
				currentCrosshairPositions[3] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[3].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[3].x, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[3].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[3].y, 5 * Time.deltaTime));

				currentCrosshairPositions[4] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[4].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[4].x, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[4].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[4].y, 5 * Time.deltaTime));

				currentCrosshairPositions[1] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[1].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[1].x, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[1].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[1].y, 5 * Time.deltaTime));

				currentCrosshairPositions[2] = new Vector2(
					Mathf.Lerp(currentCrosshairPositions[2].x, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[2].x, 5 * Time.deltaTime),
					Mathf.Lerp(currentCrosshairPositions[2].y, controller.WeaponManager.WeaponController.Attacks[controller.WeaponManager.WeaponController.currentAttack].crosshairPartsPositions[2].y, 5 * Time.deltaTime));
			}
		}

		public static void GetScreenTarget(ref Vector3 screenPosition, ref float angle, Vector3 screenCentre, Vector3 screenBounds)
		{
			screenPosition -= screenCentre;
			
            if(screenPosition.z < 0)
            {
                screenPosition *= -1;
            }

            angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
            float slope = Mathf.Tan(angle);
            
            if(screenPosition.x > 0)
            {
	            screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
            }
            else
            {
                screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
            }
            if(screenPosition.y > screenBounds.y)
            {
	            screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
            }
            else if(screenPosition.y < -screenBounds.y)
            {
                screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
            }
            screenPosition += screenCentre;
		}

		public static void ManageUIButtons(Controller controller, InventoryManager manager, UIManager uiManager, bool isMultiplayer)
		{
			var weaponController = manager.WeaponController;
			var mainConditions = isMobile(controller.projectSettings) && !manager.inventoryIsOpened;
			bool activityConditions;
			
			//aim
			if (uiManager.uiButtons[0])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[5] &&
				                     weaponController && weaponController.activeAimMode;

				uiManager.uiButtons[0].gameObject.SetActive(activityConditions);
			}

			//reload
			if (uiManager.uiButtons[1])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[4] && manager.allWeaponsCount > 1 &&
				                     !manager.slots[manager.currentSlot].weaponSlotInGame[manager.slots[manager.currentSlot].currentWeaponInSlot].fistAttack &&
				                     weaponController && weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade &&
				                     weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Melee;
					
				uiManager.uiButtons[1].gameObject.SetActive(activityConditions);
			}
			
			//change camera type
			if (uiManager.uiButtons[2])
			{
				int countOfCameras;

				if (controller.CameraParameters.activeFP && (controller.CameraParameters.activeTP || controller.CameraParameters.activeTD))
					countOfCameras = 2;
				else if (controller.CameraParameters.activeTP && (controller.CameraParameters.activeFP || controller.CameraParameters.activeTD))
					countOfCameras = 2;
				else if (controller.CameraParameters.activeTD && (controller.CameraParameters.activeFP || controller.CameraParameters.activeTP))
					countOfCameras = 2;
				else countOfCameras = 1;

				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[11] && countOfCameras == 2;
					
				uiManager.uiButtons[2].gameObject.SetActive(activityConditions);
			}
			
			//change attack type
			if (uiManager.uiButtons[3])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[19] && weaponController && (weaponController.Attacks.Count > 1 || 
				                     weaponController.Attacks[weaponController.currentAttack].AttackType == WeaponsHelper.TypeOfAttack.Bullets && 
				                     weaponController.Attacks[weaponController.currentAttack].BulletsSettings[0].Active && weaponController.Attacks[weaponController.currentAttack].BulletsSettings[1].Active);
					
				uiManager.uiButtons[3].gameObject.SetActive(activityConditions);
			}
			
			//drop weapon
			if (uiManager.uiButtons[4])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[9] && manager.slots[manager.currentSlot].weaponSlotInGame.Count > 0 && !manager.slots[manager.currentSlot].weaponSlotInGame[manager.slots[manager.currentSlot].currentWeaponInSlot].fistAttack
					&& weaponController && weaponController.Attacks[weaponController.currentAttack].AttackType != WeaponsHelper.TypeOfAttack.Grenade;
					
				uiManager.uiButtons[4].gameObject.SetActive(activityConditions);
			}

			//attack
			if (uiManager.uiButtons[5])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[3] && manager.allWeaponsCount > 1;
					
				uiManager.uiButtons[5].gameObject.SetActive(activityConditions);
			}
			
			//sprint
			if (uiManager.uiButtons[6])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[0];
					
				uiManager.uiButtons[6].gameObject.SetActive(activityConditions);
			}
			
			//crouch
			if (uiManager.uiButtons[7])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[1];
					
				uiManager.uiButtons[7].gameObject.SetActive(activityConditions);
			}
			
			//jump
			if (uiManager.uiButtons[8])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[2];
					
				uiManager.uiButtons[8].gameObject.SetActive(activityConditions);
			}
			
			//pause
			if (uiManager.uiButtons[9])
			{
				activityConditions = isMobile(controller.projectSettings) && !manager.inventoryIsOpened && controller.projectSettings.ButtonsActivityStatuses[10];
					
				uiManager.uiButtons[9].gameObject.SetActive(activityConditions);
			}

			//open/close inventory
			if (uiManager.uiButtons[10])
			{
				activityConditions = isMobile(controller.projectSettings) && !controller.isPause && controller.projectSettings.ButtonsActivityStatuses[7];
					
				uiManager.uiButtons[10].gameObject.SetActive(activityConditions);
			}
			
			//pick up ?
//			if (uiManager.uiButtons[11])
//			{
//				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[2];
//					
//				uiManager.uiButtons[11].gameObject.SetActive(activityConditions);
//			}

			//switch character
			if (uiManager.uiButtons[12])
			{
				activityConditions = mainConditions &&
				                     !isMultiplayer && 
				                     controller.projectSettings.ButtonsActivityStatuses[18] && 
				                     Object.FindObjectOfType<GameManager>() && Object.FindObjectOfType<GameManager>().controllers.Count > 1;
					
				uiManager.uiButtons[12].gameObject.SetActive(activityConditions);
			}
			
			//weapon up
			if (uiManager.uiButtons[13])
			{
				
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[16] && manager.allWeaponsCount > 1;
					
				uiManager.uiButtons[13].gameObject.SetActive(activityConditions);
			}
			
			//weapon down
			if (uiManager.uiButtons[14])
			{
				activityConditions = mainConditions && controller.projectSettings.ButtonsActivityStatuses[16] && manager.allWeaponsCount > 1;
					
				uiManager.uiButtons[14].gameObject.SetActive(activityConditions);
			}
		}
		

		static bool isMobile(ProjectSettings projectSettings)
		{
			return Application.isMobilePlatform || projectSettings.mobileDebug;
		}
	}
}
