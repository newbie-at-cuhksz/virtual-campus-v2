using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(UIPlaceholder))]
	public class UIPlaceholderEditor : Editor
	{
		
		public UIPlaceholder script;

		public void Awake()
		{
			script = (UIPlaceholder) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.Space();

			script.ContentType = (PUNHelper.ContentType)EditorGUILayout.EnumPopup("Type",script.ContentType);
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("helpbox");
			switch (script.ContentType)
			{
				case PUNHelper.ContentType.Player:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("KD"), new GUIContent("Kill / Death"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Rank"), new GUIContent("Rank"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Score"), new GUIContent("Score"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"), new GUIContent("Icon"));

					EditorGUILayout.Space();
					EditorGUILayout.HelpBox("This color will be used to highlight your statistics in the table of players during the game.", MessageType.Info);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("HighlightedColor"), new GUIContent("Highlighted Color"));
					break;
				
				case PUNHelper.ContentType.Match:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("KillerName"), new GUIContent("Killer Nickname"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("VictimName"), new GUIContent("Victim Nickname"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponIcon"), new GUIContent("Weapon Icon"));
					break;
				
				case PUNHelper.ContentType.Weapon:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ImagePlaceholder"), new GUIContent("Image Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Button"), new GUIContent("Selection Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIndicator"), new GUIContent("Selection Indicator"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Background"), new GUIContent("Background"));
					break;
				case PUNHelper.ContentType.GameMode:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ImagePlaceholder"), new GUIContent("Image Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Button"), new GUIContent("Selection Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIndicator"), new GUIContent("Selection Indicator"));
					break;
				case PUNHelper.ContentType.Map:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ImagePlaceholder"), new GUIContent("Image Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Button"), new GUIContent("Selection Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIndicator"), new GUIContent("Selection Indicator"));
					break;
				case PUNHelper.ContentType.Avatar:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ImagePlaceholder"), new GUIContent("Image Placeholder"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Button"), new GUIContent("Selection Button"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIndicator"), new GUIContent("Selection Indicator"));
					break;
				case PUNHelper.ContentType.Room:
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Room Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"), new GUIContent("Game Mode"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Map"), new GUIContent("Map Name"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Count"), new GUIContent("Players Count"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("ImagePlaceholder"), new GUIContent("Lock Icon"));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("Button"), new GUIContent("Selection Button"));
					break;
			}
			EditorGUILayout.EndVertical();
			
			serializedObject.ApplyModifiedProperties();
			
			if (GUI.changed)
				EditorUtility.SetDirty(script.gameObject);

		}
	}
}
