using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	
	public class ProjectSettings : ScriptableObject
	{
		public Helper.GamepadAxes[] GamepadAxes = new Helper.GamepadAxes[5];
		public Helper.GamepadCodes[] GamepadCodes = new Helper.GamepadCodes[18];
		public Helper.AxisButtonValue[] AxisButtonValues = new Helper.AxisButtonValue[18];
		public Helper.KeyBoardCodes[] KeyBoardCodes = new Helper.KeyBoardCodes[20];
		public bool[] ButtonsActivityStatuses = Helper.ButtonsStatus(20);
		
		public bool PressSprintButton;
		public bool PressCrouchButton;
		public bool PressInventoryButton;
		public bool mobileDebug;
		
		public Button[] uiButtons = new Button[17];

		public List<string> CharacterTags = new List<string>{"Character"};
		public List<string> EnemiesTags = new List<string>{"Enemy"};

		public float CubesSize = 10;
		public Helper.CubeSolid CubeSolid = Helper.CubeSolid.Wire;

		public string oldScenePath;
		public string oldSceneName;

		public bool useAllWeapons;

		public List<int> weaponsIndices;
		public List<PUNHelper.WeaponSlot> weaponSlots;

		public enum Stick
		{
			MovementStick,
			CameraStick
		}

		public Stick _Stick;
		
		public bool[] invertAxes = new bool[5];

		public int tab;
		[Range(100, 1000)]
		public int StickRange;
	}
}
