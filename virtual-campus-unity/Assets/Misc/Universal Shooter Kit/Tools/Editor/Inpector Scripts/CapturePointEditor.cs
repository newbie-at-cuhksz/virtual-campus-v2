using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(CapturePoint))]
	public class CapturePointEditor : Editor
	{
		
		public CapturePoint script;
		
		public void Awake()
		{
			script = (CapturePoint) target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("helpbox");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("type"), new GUIContent("Type"));
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("helpbox");
			if(script.type == CapturePoint.Type.Circle)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"), new GUIContent("Radius"));
			else EditorGUILayout.PropertyField(serializedObject.FindProperty("Size"), new GUIContent("Size"));
			EditorGUILayout.EndVertical();
			
			serializedObject.ApplyModifiedProperties();
			
			if (GUI.changed)
			{
				EditorUtility.SetDirty(script.gameObject);
               
				if(!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

		}
		
	}
}
