using UnityEngine;
using UnityEditor;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(PickUp))]
    public class PickUpEditor : Editor
    {
        public PickUp script;

        private void Awake()
        {
            script = (PickUp) target;

            script.pickUpId = Helper.GenerateRandomString(20);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PickUpType"), new GUIContent("Pickup Type"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Method"), new GUIContent("Pickup Method"));
           
            if (script.Method == PickUp.PickUpMethod.Raycast)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"),
                    new GUIContent("Visibility distance"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ColliderSize"), new GUIContent("Collider size"));
            }
            
            if (script.PickUpType != PickUp.TypeOfPickUp.Weapon)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PickUpAudio"), new GUIContent("Pickup audio"));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            switch (script.PickUpType)
            {
                case PickUp.TypeOfPickUp.Health:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("health_add"), new GUIContent("Health add"));
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("This image will be displayed in the inventory.", MessageType.Info);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("KitImage"), new GUIContent("Image"));
                    
                    break;
                case PickUp.TypeOfPickUp.Ammo:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ammo_add"), new GUIContent("Ammo add"));
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("This ammo will be used only for weapons with the same type of ammunition. " +
                                            "Write the same name in the [WeaponController] script." + "\n" +
                                            "If name is empty, it will be used for all weapons", MessageType.Info);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoType"), new GUIContent("Ammo name"));
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("This image will be displayed in the inventory.", MessageType.Info);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("KitImage"), new GUIContent("Image"));
                    
                    break;
                case PickUp.TypeOfPickUp.Weapon:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Slots"), new GUIContent("Inventory Slot"));
                    break;
            }
            
            EditorGUILayout.EndVertical();

//            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(script.gameObject);
        }

    }

}


