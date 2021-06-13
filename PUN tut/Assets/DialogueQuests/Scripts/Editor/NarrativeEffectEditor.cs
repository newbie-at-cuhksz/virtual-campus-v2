using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if SURVIVAL_ENGINE
using SurvivalEngine;
#endif

#if FARMING_ENGINE
using FarmingEngine;
#endif

namespace DialogueQuests.EditorTool
{

    [CustomEditor(typeof(NarrativeEffect))]
    public class NarrativeEffectEditor : Editor
    {
        SerializedProperty sprop;

        internal void OnEnable()
        {
            sprop = serializedObject.FindProperty("callfunc_evt");
        }

        public override void OnInspectorGUI()
        {
            NarrativeEffect myScript = target as NarrativeEffect;

            myScript.type = (NarrativeEffectType)AddEnumField("Type", myScript.type);

            if (myScript.type == NarrativeEffectType.CustomInt
                || myScript.type == NarrativeEffectType.CustomFloat
                || myScript.type == NarrativeEffectType.CustomString)
            {
                myScript.target_id = AddTextField("Target ID", myScript.target_id);

                if(myScript.type != NarrativeEffectType.CustomString)
                    myScript.oper = (NarrativeEffectOperator)AddEnumField("Operator", myScript.oper);

                if (myScript.type == NarrativeEffectType.CustomInt)
                {
                    myScript.value_int = AddIntField("Value Int", myScript.value_int);
                }

                if (myScript.type == NarrativeEffectType.CustomFloat)
                {
                    myScript.value_float = AddFloatField("Value Float", myScript.value_float);
                }

                if (myScript.type == NarrativeEffectType.CustomString)
                {
                    myScript.value_string = AddTextField("Value String", myScript.value_string);
                }

            }

            if (myScript.type == NarrativeEffectType.Move || myScript.type == NarrativeEffectType.Show || myScript.type == NarrativeEffectType.Hide
                || myScript.type == NarrativeEffectType.Spawn || myScript.type == NarrativeEffectType.Destroy
                || myScript.type == NarrativeEffectType.StartEvent || myScript.type == NarrativeEffectType.StartEventIfMet)
            {
                myScript.value_object = AddGameObjectField("Target", myScript.value_object);
            }

            if (myScript.type == NarrativeEffectType.Move || myScript.type == NarrativeEffectType.Spawn)
            {
                myScript.target_id = AddTextField("Region ID", myScript.target_id);
            }

            if (myScript.type == NarrativeEffectType.ActorRelation)
            {
                myScript.value_data = AddScriptableObjectField<ActorData>("Actor", myScript.value_data);

                myScript.oper = (NarrativeEffectOperator)AddEnumField("Operator", myScript.oper);
                myScript.value_int = AddIntField("Value", myScript.value_int);
            }

            if (myScript.type == NarrativeEffectType.StartQuest || myScript.type == NarrativeEffectType.CancelQuest
                || myScript.type == NarrativeEffectType.CompleteQuest || myScript.type == NarrativeEffectType.FailQuest)
            {
                myScript.value_data = AddScriptableObjectField<QuestData>("Quest", myScript.value_data);
            }

            if (myScript.type == NarrativeEffectType.QuestProgress)
            {
                myScript.value_data = AddScriptableObjectField<QuestData>("Quest", myScript.value_data);
                myScript.target_id = AddTextField("Progress ID", myScript.target_id);
                myScript.oper = (NarrativeEffectOperator)AddEnumField("Operator", myScript.oper);
                myScript.value_int = AddIntField("Value", myScript.value_int);
            }

            if (myScript.type == NarrativeEffectType.PlaySFX || myScript.type == NarrativeEffectType.PlayMusic)
            {
                myScript.value_string = AddTextField("Channel", myScript.value_string);
                myScript.value_audio = AddAudioField("Audio", myScript.value_audio);
                myScript.value_float = AddFloatField("Volume", myScript.value_float);
            }

            if (myScript.type == NarrativeEffectType.StopMusic)
            {
                myScript.value_string = AddTextField("Channel", myScript.value_string);
            }

#if SURVIVAL_ENGINE || FARMING_ENGINE
            
            if (myScript.type == NarrativeEffectType.Create)
            {
                myScript.value_data = AddScriptableObjectField<CraftData>("CraftData", myScript.value_data);
                myScript.target_id = AddTextField("Region ID", myScript.target_id);
            }

            if (myScript.type == NarrativeEffectType.PlayerAttribute)
            {
                myScript.value_attribute = (AttributeType)AddEnumField("Type", myScript.value_attribute);
                myScript.oper = (NarrativeEffectOperator)AddEnumField("Operator", myScript.oper);
                myScript.value_float = AddFloatField("Value", myScript.value_float);
            }

            if (myScript.type == NarrativeEffectType.GainXP)
            {
                myScript.value_int = AddIntField("XP", myScript.value_int);
            }

            if (myScript.type == NarrativeEffectType.GainItem || myScript.type == NarrativeEffectType.RemoveItem)
            {
                myScript.value_data = AddScriptableObjectField<ItemData>("Item", myScript.value_data);
                myScript.value_int = AddIntField("Quantity", myScript.value_int);
            }
#endif

#if FARMING_ENGINE
            if (myScript.type == NarrativeEffectType.GainGold)
            {
                myScript.value_int = AddIntField("Quantity", myScript.value_int);
            }

            if (myScript.type == NarrativeEffectType.OpenShop)
            {
                myScript.value_object = AddGameObjectField("Shop", myScript.value_object);
            }
#endif

            if (myScript.type == NarrativeEffectType.RollRandomValue)
            {
                myScript.target_id = AddTextField("ID", myScript.target_id);
                myScript.value_int = AddIntField("Max Value", myScript.value_int);
            }

            if (myScript.type == NarrativeEffectType.Wait)
            {
                myScript.value_float = AddFloatField("Time", myScript.value_float);
            }

            if (myScript.type == NarrativeEffectType.CustomEffect)
            {
                GUILayout.Label("Use a script that inherit from CustomEffect");
                myScript.value_custom = AddScriptableObjectField<CustomEffect>("Custom Effect", myScript.value_custom);
            }

            if (myScript.type == NarrativeEffectType.CallFunction)
            {

                //EditorGUIUtility.LookLikeControls();
                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(sprop, new GUIContent("List Callbacks", ""));
                }

                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
                GUILayout.EndHorizontal();
            }

            if (GUI.changed && !Application.isPlaying)
            {
                EditorUtility.SetDirty(myScript);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                AssetDatabase.SaveAssets();
            }
        }

        private string AddTextField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            string outval = EditorGUILayout.TextField(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private string AddTextAreaField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetShortLabelWidth());
            GUILayout.FlexibleSpace();
            EditorStyles.textField.wordWrap = true;
            string outval = EditorGUILayout.TextArea(value, GetLongWidth(), GUILayout.Height(50));
            GUILayout.EndHorizontal();
            return outval;
        }

        private int AddIntField(string label, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            int outval = EditorGUILayout.IntField(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private float AddFloatField(string label, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            float outval = EditorGUILayout.FloatField(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private System.Enum AddEnumField(string label, System.Enum value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            System.Enum outval = EditorGUILayout.EnumPopup(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private GameObject AddGameObjectField(string label, GameObject value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            GameObject outval = (GameObject)EditorGUILayout.ObjectField(value, typeof(GameObject), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private T AddScriptableObjectField<T>(string label, ScriptableObject value) where T : ScriptableObject
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            T outval = (T)EditorGUILayout.ObjectField(value, typeof(T), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private Sprite AddSpriteField(string label, Sprite value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            Sprite outval = (Sprite)EditorGUILayout.ObjectField(value, typeof(Sprite), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private AudioClip AddAudioField(string label, AudioClip value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            AudioClip outval = (AudioClip)EditorGUILayout.ObjectField(value, typeof(AudioClip), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private GUILayoutOption GetLabelWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f);
        }

        private GUILayoutOption GetWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f);
        }

        private GUILayoutOption GetShortLabelWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f);
        }

        private GUILayoutOption GetLongWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.65f);
        }
    }

}