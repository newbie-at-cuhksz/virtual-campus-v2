using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{

    public class NarrativeEventLine
    {
        public GameObject game_obj;
        public NarrativeEvent parent;
        public DialogueMessage dialogue = null;
        public List<DialogueChoice> choices = new List<DialogueChoice>();
        public List<NarrativeCondition> conditions = new List<NarrativeCondition>();
        public List<NarrativeEffect> effects = new List<NarrativeEffect>();

        public bool AreConditionsMet(Actor triggerer = null)
        {
            bool conditions_met = true;
            foreach (NarrativeCondition condition in conditions)
            {
                if (condition.enabled && !condition.IsMet(triggerer))
                {
                    conditions_met = false;
                }
            }
            return conditions_met && game_obj.activeSelf;
        }

        public DialogueChoice GetChoice(int index)
        {
            if (index >= 0 && index < choices.Count)
                return choices[index];
            return null;
        }

        public void TriggerLine()
        {
            NarrativeManager.Get().StartEventLine(this);
        }

        public void TriggerLineIfMet()
        {
            if (AreConditionsMet(parent.GetLastTriggerer()))
            {
                NarrativeManager.Get().StartEventLine(this);
            }
        }

        public float TriggerEffects()
        {
            float wait_timer = 0f;
            foreach (NarrativeEffect effect in effects)
            {
                if (effect.enabled)
                {
                    effect.Trigger(parent.GetLastTriggerer());
                    wait_timer += effect.GetWaitTime();
                }
            }
            return wait_timer;
        }
    }

}
