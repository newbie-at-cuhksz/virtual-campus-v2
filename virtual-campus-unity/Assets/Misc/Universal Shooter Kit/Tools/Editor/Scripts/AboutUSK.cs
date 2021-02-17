using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	public class AboutUSK : EditorWindow
	{

		private Font font;

		private GUIStyle LabelStyle;

		private Vector2 scrollPos;

		private const string Version = "1.6.2";

		[MenuItem("Window/USK/About")]
		public static void ShowWindow()
		{
			GetWindow(typeof(AboutUSK), true, "About USK", true).ShowUtility();
		}

		
		private void Awake()
		{
			if(!font)
				font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
			
			if (LabelStyle != null) return;

			LabelStyle = new GUIStyle
			{
				normal = {textColor = Color.black},
				fontStyle = FontStyle.Bold,
				fontSize = 14,
				alignment = TextAnchor.MiddleCenter
			};
		}
		
		private void OnGUI()
		{
//			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
			
			EditorGUILayout.Space();
			LabelStyle.fontStyle = FontStyle.Bold;
			LabelStyle.fontSize = 15;
			GUILayout.Label("Universal Shooter Kit", LabelStyle);
			GUILayout.Label(Version + " version", LabelStyle);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			LabelStyle.fontStyle = FontStyle.Normal;
			LabelStyle.fontSize = 12;
			GUILayout.Label("Support email: gercstudio@gmail.com", LabelStyle);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUILayout.Label("Copyright © 2020 GercStudio " + "\n" + "All rights reserved", LabelStyle);
			
		}
	}
}
