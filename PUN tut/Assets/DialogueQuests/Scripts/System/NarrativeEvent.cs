using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script for narrative event (triggering quests or other)
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace DialogueQuests
{

    public enum NarrativeEventType 
    {
        Manual = -2, //By script only
        AutoTrigger = -1, //As soon as conditions are met
        InteractActor = 0, //When interact with actor
        NearActor = 2, //Just go near an actor
        AtStart = 5, //After scene load
        EnterRegion = 10, //Enter a region
        LeaveRegion = 11, //Exit a resion
        AfterEvent = 20, //After another event
    }
    
    public class NarrativeEvent : MonoBehaviour
    {
        [Tooltip("Optional: Only use if you plan to save the NarrativeData, so it can reference to it.")]
        public string event_id;

        [Header("Trigger")]
        [Tooltip("How this event will be triggered")]
        public NarrativeEventType trigger_type;
        [Tooltip("Which object will trigger this event")]
        public GameObject trigger_target;
        [Tooltip("Which actor will trigger this event")]
        public ActorData trigger_actor;
        [Tooltip("Number of times it can trigger. Put 0 for Infinity")]
        public int trigger_limit = 1; //0 means infinite

        [Header("Dialogue")]
        public DialogueMessageType dialogue_type;
        [Tooltip("Should it pause gameplay?")]
        public bool pause_gameplay = false;
        public bool skipable = false;

        [Header("Comment, no effect")]
        [TextArea(3, 5)]
        public string comment;

        public UnityAction onStart;
        public UnityAction onEnd;

        private int trigger_count = 0;
        private Actor last_triggerer;
        private int priority = 0;

        private static List<NarrativeEvent> event_list = new List<NarrativeEvent>();

        private List<NarrativeCondition> conditions = new List<NarrativeCondition>();
        private List<NarrativeEffect> effects = new List<NarrativeEffect>();
        private List<NarrativeEventLine> event_lines = new List<NarrativeEventLine>();

        void Awake()
        {
            event_list.Add(this);
            conditions.AddRange(GetComponents<NarrativeCondition>());
            effects.AddRange(GetComponents<NarrativeEffect>());
            priority = transform.GetSiblingIndex(); //First one has priority over other ones

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child_obj = transform.GetChild(i).gameObject;
                if (child_obj.GetComponent<NarrativeEffect>() || child_obj.GetComponent<DialogueMessage>())
                {
                    NarrativeEventLine child = new NarrativeEventLine();
                    child.game_obj = child_obj;
                    child.parent = this;
                    child.dialogue = child_obj.GetComponent<DialogueMessage>();
                    child.conditions.AddRange(child_obj.GetComponents<NarrativeCondition>());
                    child.effects.AddRange(child_obj.GetComponents<NarrativeEffect>());

                    int index = 0;
                    foreach (DialogueChoice choice in child_obj.GetComponents<DialogueChoice>())
                    {
                        child.choices.Add(choice);
                        choice.choice_index = index;
                        index++;
                    }

                    event_lines.Add(child);
                }
            }
        }

        void OnDestroy()
        {
            event_list.Remove(this);
        }

        void Start()
        {
            if (trigger_type == NarrativeEventType.AtStart)
            {
                OnTriggerEvent();
            }

            if (trigger_type == NarrativeEventType.InteractActor && trigger_target != null && trigger_actor == null)
            {
                Actor actor_trigger = trigger_target.GetComponent<Actor>();
                actor_trigger.onInteract += OnTriggerEvent;
            }
            
            if (trigger_type == NarrativeEventType.NearActor && trigger_target != null && trigger_actor == null)
            {
                Actor actor_trigger = trigger_target.GetComponent<Actor>();
                actor_trigger.onNear += OnTriggerEvent;
            }

            if (trigger_type == NarrativeEventType.EnterRegion && trigger_target != null)
            {
                Region region_trigger = trigger_target.GetComponent<Region>();
                region_trigger.onEnterRegion += OnTriggerEvent;
            }
            if (trigger_type == NarrativeEventType.LeaveRegion && trigger_target != null)
            {
                Region region_trigger = trigger_target.GetComponent<Region>();
                region_trigger.onExitRegion += OnTriggerEvent;
            }

            if (trigger_type == NarrativeEventType.AfterEvent)
            {
                NarrativeEvent event_trigger = trigger_target.GetComponent<NarrativeEvent>();
                if (event_trigger)
                {
                    event_trigger.onEnd += () => { OnTriggerEvent(event_trigger.GetLastTriggerer()); };
                }
            }

        }

        void Update()
        {
            if (!NarrativeManager.IsActive())
                return;

            //Auto trigger
            if (NarrativeManager.IsReady() && trigger_type == NarrativeEventType.AutoTrigger)
            {
                TriggerIfConditionsMet(Actor.GetPlayerActor());
            }
        }

        private void OnTriggerEvent()
        {
            if (NarrativeManager.IsReady())
            {
                Actor player = Actor.GetPlayerActor();
                TriggerIfConditionsMet(player);
            }
        }

        private void OnTriggerEvent(Actor player)
        {
            if (NarrativeManager.IsReady())
            {
                TriggerIfConditionsMet(player);
            }
        }

        public void AddActor(Actor actor)
        {
            if (trigger_type == NarrativeEventType.InteractActor && actor != null)
            {
                actor.onInteract += OnTriggerEvent;
            }
            if (trigger_type == NarrativeEventType.NearActor && actor != null)
            {
                actor.onNear += OnTriggerEvent;
            }
        }

        public void AddConditions(NarrativeCondition[] group_conditions)
        {
            conditions.AddRange(group_conditions);
        }
		
		public void Trigger()
		{
			Trigger(Actor.GetPlayerActor());
		}

        public void Trigger(Actor player)
        {
            if (NarrativeManager.Get().GetCurrent() != this)
            {
                last_triggerer = player;
                NarrativeManager.Get().AddToTriggerList(this);
            }
        }

        public void TriggerIfConditionsMet(Actor player)
        {
            if (AreConditionsMet(player))
            {
                Trigger(player);
            }
        }

        public void TriggerImmediately(Actor player)
        {
            if (NarrativeManager.Get().GetCurrent() != this)
            {
                last_triggerer = player;
                NarrativeManager.Get().StartEvent(this);
            }
        }

        public void IncreaseCounter()
        {
            //Increment
            int cur_val = NarrativeData.Get().GetTriggerCount(event_id);
            NarrativeData.Get().SetTriggerCount(event_id, cur_val + 1);
            trigger_count++;
        }

        public float TriggerEffects()
        {
            float wait_timer = 0f;
            foreach (NarrativeEffect effect in effects)
            {
                if (effect.enabled)
                {
                    effect.Trigger(last_triggerer);
                    wait_timer += effect.GetWaitTime();
                }
            }
            return wait_timer;
        }

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

            int game_trigger_count = NarrativeData.Get().GetTriggerCount(event_id);
            bool below_max = (trigger_limit == 0 || game_trigger_count < trigger_limit)
                && (trigger_limit == 0 || trigger_count < trigger_limit);

            return conditions_met && below_max && gameObject.activeSelf;
        }

        public int GetTriggerCount()
        {
            return trigger_count;
        }

        public Actor GetLastTriggerer()
        {
            return last_triggerer;
        }

        public int GetPriority()
        {
            return priority;
        }

        public List<NarrativeEventLine> GetLines()
        {
            return event_lines;
        }
		
		public static NarrativeEvent Get(string event_id)
		{
			foreach (NarrativeEvent evt in event_list)
            {
                if (evt.event_id == event_id)
                    return evt;
            }
			return null;
		}

        public static List<NarrativeEvent> GetAllOf(Actor actor)
        {
            List<NarrativeEvent> valid_list = new List<NarrativeEvent>();
            foreach (NarrativeEvent evt in event_list)
            {
                if (evt.trigger_actor == actor.data)
                    valid_list.Add(evt);
            }
            return valid_list;
        }

        //Call if you want to reset the trigger count of all events
        public static void ResetAll()
        {
            foreach (NarrativeEvent evt in GetAll())
            {
                evt.trigger_count = 0;
                NarrativeData.Get().SetTriggerCount(evt.event_id, 0);
            }
        }

        public static NarrativeEvent[] GetAll()
        {
            return event_list.ToArray();
        }
    }
}