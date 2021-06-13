using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests
{

    public class DialogueChoice : MonoBehaviour
    {
        [TextArea(1, 1)]
        public string text;
        public NarrativeEvent go_to;


        [HideInInspector]
        public int choice_index;

        public UnityAction<int> onSelect;

        private NarrativeCondition[] conditions;

        private void Awake()
        {
            conditions = GetComponents<NarrativeCondition>();
        }

        public string GetText()
        {
            return NarrativeTool.Translate(text);
        }

        public bool AreConditionsMet(Actor triggerer = null)
        {
            bool met = true;
            foreach (NarrativeCondition condition in conditions)
            {
                met = met && condition.IsMet(triggerer);
            }
            return met;
        }
    }

}
