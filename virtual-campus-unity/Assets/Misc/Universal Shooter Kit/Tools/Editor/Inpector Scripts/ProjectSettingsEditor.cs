using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(ProjectSettings))]
	public class ProjectSettingsEditor : Editor
	{
		private ProjectSettings script;

		public void Awake()
		{
			script = (ProjectSettings) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			script.tab = GUILayout.Toolbar(script.tab, new string[] {"Character", "Weapons", "Inventory", "Other"});
			
			switch (script.tab)
			{
				case 0:
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadAxes[0] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Forward/backward", script.GamepadAxes[0]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(0), new GUIContent("Invert"));

					script.GamepadAxes[1] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Right/left", script.GamepadAxes[1]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(1), new GUIContent("Invert"));
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.LabelField("Keyboard", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.KeyBoardCodes[12] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Forward", script.KeyBoardCodes[12]);
					script.KeyBoardCodes[13] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Backward", script.KeyBoardCodes[13]);
					script.KeyBoardCodes[14] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Right", script.KeyBoardCodes[14]);
					script.KeyBoardCodes[15] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Left", script.KeyBoardCodes[15]);
					
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.LabelField("Mobile", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					
					script.uiButtons[16] = (Button)EditorGUILayout.ObjectField("Move stick", script.uiButtons[16], typeof(Button), false);
					script.uiButtons[15] = (Button)EditorGUILayout.ObjectField("Stick outline", script.uiButtons[15], typeof(Button), false);
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("StickRange"), new GUIContent("Stick range (in px)"));
		
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Rotate Camera", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.LabelField("Gamepad", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadAxes[2] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Horizontal axis", script.GamepadAxes[2]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(2), new GUIContent("Invert"));

					script.GamepadAxes[3] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Vertical axis", script.GamepadAxes[3]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("invertAxes").GetArrayElementAtIndex(3), new GUIContent("Invert"));
					EditorGUILayout.EndVertical();
		
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Change Camera Type", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[11] = GUILayout.Toggle(script.ButtonsActivityStatuses[11], script.ButtonsActivityStatuses[11] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[11]);
					script.KeyBoardCodes[11] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[11]);
					if(CheckGamepadCode(11))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[11] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[11]);
					if (CheckGamepadCode(11))
					{
						script.AxisButtonValues[11] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[11]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[2] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[2], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Change Character", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[18] = GUILayout.Toggle(script.ButtonsActivityStatuses[18], script.ButtonsActivityStatuses[18] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[18]);
					script.KeyBoardCodes[18] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[18]);
					if(CheckGamepadCode(16))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[16] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[16]);
					if (CheckGamepadCode(16))
					{
						script.AxisButtonValues[16] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[16]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[12] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[12], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Sprint", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[0] = GUILayout.Toggle(script.ButtonsActivityStatuses[0], script.ButtonsActivityStatuses[0] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[0]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("PressSprintButton"), new GUIContent("Press Button"));
					EditorGUILayout.HelpBox(script.PressSprintButton ? "Hold the button to run." : "Click the button to run. Press again to stop running.", MessageType.Info);
					script.KeyBoardCodes[0] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[0]);
					if(CheckGamepadCode(0))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[0] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[0]);
					if (CheckGamepadCode(0))
					{
						script.AxisButtonValues[0] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[0]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[6] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[6], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Crouch", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[1] = GUILayout.Toggle(script.ButtonsActivityStatuses[1], script.ButtonsActivityStatuses[1] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[1]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("PressCrouchButton"), new GUIContent("Press Button"));
					EditorGUILayout.HelpBox(script.PressCrouchButton ? "Hold the button to crouch." : "Click the button to crouch. Press again to stop squatting.", MessageType.Info);
					script.KeyBoardCodes[1] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[1]);
					if(CheckGamepadCode(1))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[1] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[1]);
					if (CheckGamepadCode(1))
					{
						script.AxisButtonValues[1] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[1]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[7] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[7], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Jump", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[2] = GUILayout.Toggle(script.ButtonsActivityStatuses[2], script.ButtonsActivityStatuses[2] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[2]);
					script.KeyBoardCodes[2] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[2]);
					if(CheckGamepadCode(2))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[2] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[2]);
					if (CheckGamepadCode(2))
					{
						script.AxisButtonValues[2] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[2]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[8] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[8], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					break;
				
				case 1:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Attack", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[3] = GUILayout.Toggle(script.ButtonsActivityStatuses[3], script.ButtonsActivityStatuses[3] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[3]);
					script.KeyBoardCodes[3] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[3]);
					if(CheckGamepadCode(3))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[3] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[3]);
					if (CheckGamepadCode(3))
					{
						script.AxisButtonValues[3] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[3]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[5] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[5], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Reload", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[4] = GUILayout.Toggle(script.ButtonsActivityStatuses[4], script.ButtonsActivityStatuses[4] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[4]);
					script.KeyBoardCodes[4] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[4]);
					if(CheckGamepadCode(4))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[4] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[4]);
					if (CheckGamepadCode(4))
					{
						script.AxisButtonValues[4] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[4]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[1] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[1], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Aim", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[5] = GUILayout.Toggle(script.ButtonsActivityStatuses[5], script.ButtonsActivityStatuses[5] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[5]);
					script.KeyBoardCodes[5] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[5]);
					if(CheckGamepadCode(5)) 
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[5] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[5]);
					if (CheckGamepadCode(5))
					{
						script.AxisButtonValues[5] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[5]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[0] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[0], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Change Attack Type", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[19] = GUILayout.Toggle(script.ButtonsActivityStatuses[19], script.ButtonsActivityStatuses[19] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[19]);
					script.KeyBoardCodes[19] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[19]);
					if(CheckGamepadCode(17))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[17] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[17]);
					if (CheckGamepadCode(17))
					{
						script.AxisButtonValues[17] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[17]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[3] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[3], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					break;
				
				case 2:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Open/Close Inventory", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[7] = GUILayout.Toggle(script.ButtonsActivityStatuses[7], script.ButtonsActivityStatuses[7] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[7]);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("PressInventoryButton"), new GUIContent("Press button"), true);
					EditorGUILayout.HelpBox(script.PressInventoryButton
							? "Hold the button to keep the inventory open."
							: "Click the button to open the inventory, then click again to close.", MessageType.Info);
					script.KeyBoardCodes[7] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[7]);
					
					if(CheckGamepadCode(7))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[7] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[7]);
					if (CheckGamepadCode(7))
					{
						script.AxisButtonValues[7] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[7]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[10] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[10], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Pick up Object", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.KeyBoardCodes[8] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[8]);
					
					if(CheckGamepadCode(8))
						EditorGUILayout.BeginVertical("HelpBox");
					
					script.GamepadCodes[8] =
						(Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[8]);
					
					if (CheckGamepadCode(8))
					{
						script.AxisButtonValues[8] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[8]);
						EditorGUILayout.EndVertical();
					}
					
					script.uiButtons[11] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[11], typeof(Button), false);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Drop weapon", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[9] = GUILayout.Toggle(script.ButtonsActivityStatuses[9], script.ButtonsActivityStatuses[9] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[9]);
					script.KeyBoardCodes[9] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[9]);
					if(CheckGamepadCode(9))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[9] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[9]);
					if (CheckGamepadCode(9))
					{
						script.AxisButtonValues[9] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[9]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[4] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[4], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Change weapon", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[16] = GUILayout.Toggle(script.ButtonsActivityStatuses[16], script.ButtonsActivityStatuses[16] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[16]);
					EditorGUILayout.LabelField("Up", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.KeyBoardCodes[16] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[16]);
					if(CheckGamepadCode(14)) 
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[14] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[14]);
					if (CheckGamepadCode(14))
					{
						script.AxisButtonValues[14] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[14]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[14] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[14], typeof(Button), false);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.LabelField("Down", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.KeyBoardCodes[17] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[17]);
					if(CheckGamepadCode(15)) 
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[15] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[15]);
					if (CheckGamepadCode(15))
					{
						script.AxisButtonValues[15] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[15]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[13] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[13], typeof(Button), false);
					EditorGUILayout.EndVertical();
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					
					EditorGUILayout.LabelField("Change weapon in slot", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadAxes[4] = (Helper.GamepadAxes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadAxes[4]);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Use health", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					
					
					script.GamepadCodes[12] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[12]);
					
					if (CheckGamepadCode(12))
						script.AxisButtonValues[12] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[12]);

					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Use ammo", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[13] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[13]);
					
					if (CheckGamepadCode(13))
						script.AxisButtonValues[13] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[13]);
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Stick to choice weapons", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script._Stick = (ProjectSettings.Stick) EditorGUILayout.EnumPopup("Stick", script._Stick);
					EditorGUILayout.EndVertical();
					
					break;
				case 3:
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Pause", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					script.ButtonsActivityStatuses[10] = GUILayout.Toggle(script.ButtonsActivityStatuses[10], script.ButtonsActivityStatuses[10] ? "Active" : "Inactive", "Button");
					EditorGUILayout.Space();
					EditorGUI.BeginDisabledGroup(!script.ButtonsActivityStatuses[10]);
					script.KeyBoardCodes[10] = (Helper.KeyBoardCodes) EditorGUILayout.EnumPopup("Keyboard", script.KeyBoardCodes[10]);
					if(CheckGamepadCode(10))
						EditorGUILayout.BeginVertical("HelpBox");
					script.GamepadCodes[10] = (Helper.GamepadCodes) EditorGUILayout.EnumPopup("Gamepad", script.GamepadCodes[10]);
					if (CheckGamepadCode(10))
					{
						script.AxisButtonValues[10] = (Helper.AxisButtonValue) EditorGUILayout.EnumPopup("Axis value", script.AxisButtonValues[10]);
						EditorGUILayout.EndVertical();
					}
					script.uiButtons[9] = (Button)EditorGUILayout.ObjectField("Mobile", script.uiButtons[9], typeof(Button), false);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
					
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.HelpBox("Use this checkbox if you are going to test a mobile game in the Editor.", MessageType.Info);
					script.mobileDebug = EditorGUILayout.ToggleLeft("Mobile Debug", script.mobileDebug);
					EditorGUILayout.EndVertical();
					
					break;
			}
			serializedObject.ApplyModifiedProperties();

//			DrawDefaultInspector();

			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
			}
		}

		bool CheckGamepadCode(int number)
		{
			if (script.GamepadCodes[number] == Helper.GamepadCodes._3rdAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._4thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._5thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._6thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._7thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._8thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._9thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._10thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._11thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._12thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._13thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._14thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._15thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._16thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._17thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._18thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes._19thAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes.XAxis ||
			    script.GamepadCodes[number] == Helper.GamepadCodes.YAxis)
			{
				return true;
			}
			return false;
		}
	}
}
