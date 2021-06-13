using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main manager script for dialogues and quests
/// </summary>

namespace DialogueQuests
{
    public class NarrativeManager : MonoBehaviour
    {
        public UnityAction<NarrativeEvent> onEventStart;
        public UnityAction<NarrativeEvent> onEventEnd;

        public UnityAction<NarrativeEventLine, DialogueMessage> onDialogueMessageStart;
        public UnityAction<NarrativeEventLine, DialogueMessage> onDialogueMessageEnd;

        public UnityAction onPauseGameplay;
        public UnityAction onUnpauseGameplay;

        public UnityAction<string, AudioClip, float> onPlaySFX;
        public UnityAction<string, AudioClip, float> onPlayMusic;
        public UnityAction<string> onStopMusic;

        public UnityAction<QuestData> onQuestStart;
        public UnityAction<QuestData> onQuestComplete;
        public UnityAction<QuestData> onQuestFail;
        public UnityAction<QuestData> onQuestCancel;

        [HideInInspector]
        public bool use_custom_audio = false; //Set this to true to use your own audio system (and connect to the audio 3 events)

        [HideInInspector] public List<ActorData> actor_list = new List<ActorData>();
        [HideInInspector] public List<QuestData> quest_list = new List<QuestData>();

        private NarrativeEvent current_event = null;
        private NarrativeEventLine current_event_line = null;
        private DialogueMessage current_message = null;
        private ActorData current_actor = null;

        private List<NarrativeEvent> trigger_list = new List<NarrativeEvent>();
        private Queue<NarrativeEventLine> event_line_queue = new Queue<NarrativeEventLine>();
        private Dictionary<string, int> random_values = new Dictionary<string, int>();

        private float event_timer = 0f;
        private float pause_duration = 0f;
        private bool is_paused = false;
        private bool should_unpause = false;

        private static NarrativeManager _instance;
        private void Awake()
        {
            _instance = this;

            NarrativeData.LoadLast();
        }

        void Start()
        {

        }

        void Update()
        {
            //Events
            event_timer += Time.deltaTime;

            if (current_event != null)
            {
                //Stop dialogue
                if (current_message != null)
                {
                    bool auto_stop = current_event.dialogue_type == DialogueMessageType.InGame;
                    if (auto_stop)
                    {
                        if (event_timer > current_message.duration)
                        {
                            StopDialogue();
                        }
                    }
                }
                else if (current_event_line != null)
                {
                    if (event_timer > pause_duration)
                    {
                        StopEventLine();
                    }
                }
            }

            if (event_timer > pause_duration)
            {
                if (current_event_line == null && event_line_queue.Count > 0)
                {
                    NarrativeEventLine next = event_line_queue.Dequeue();
                    next.TriggerLineIfMet();
                }
                else if (current_event_line == null && current_event != null && event_line_queue.Count == 0)
                {
                    StopEvent();
                }
                else if (current_event == null && trigger_list.Count > 0)
                {
                    NarrativeEvent next = GetPriorityTriggerList();
                    trigger_list.Clear();
                    StartEvent(next);
                }
            }

            if (should_unpause && event_timer > 0.1f)
            {
                should_unpause = false;
                is_paused = false;
                if (onUnpauseGameplay != null)
                    onUnpauseGameplay.Invoke();
            }

            //Automated quests
            if (current_event == null)
            {
                Actor player = Actor.GetPlayerActor();
                foreach (QuestData quest in QuestData.GetAll())
                {
                    if (quest is QuestAutoData)
                    {
                        QuestAutoData quest_auto = (QuestAutoData)quest;

                        if (!quest_auto.IsStarted())
                        {
                            if (quest_auto.AreStartConditionsMet(player))
                                quest_auto.Start();
                        }

                        else if (quest_auto.IsActive())
                        {
                            quest_auto.Update();
                            if (quest_auto.AreConditionsMet(player))
                                quest_auto.Complete();
                        }
                    }
                }
            }
        }

        public void AddToTriggerList(NarrativeEvent narrative_event)
        {
            if (!IsInTriggerList(narrative_event))
                trigger_list.Add(narrative_event);
        }

        public bool IsInTriggerList(NarrativeEvent narrative_event)
        {
            return trigger_list.Contains(narrative_event);
        }

        private NarrativeEvent GetPriorityTriggerList()
        {
            if (trigger_list.Count > 0) {
                NarrativeEvent priority = trigger_list[0];
                foreach (NarrativeEvent evt in trigger_list)
                {
                    if (evt.GetPriority() < priority.GetPriority())
                        priority = evt;
                }
                return priority;
            }
            return null;
        }

        public void StartEvent(NarrativeEvent narrative_event)
        {
            if (current_event != narrative_event)
            {
                StopEvent();

                //Debug.Log("Start Cinematic: " + cinematic_trigger.gameObject.name);

                current_event = narrative_event;
                current_message = null;
                event_timer = 0f;
                should_unpause = is_paused && !current_event.pause_gameplay;

                current_event.IncreaseCounter();

                pause_duration = current_event.TriggerEffects();

                foreach (NarrativeEventLine line in current_event.GetLines())
                    event_line_queue.Enqueue(line);

                if (onEventStart != null)
                    onEventStart.Invoke(narrative_event);
                if (narrative_event.onStart != null)
                    narrative_event.onStart.Invoke();

                if (narrative_event.pause_gameplay && !is_paused && onPauseGameplay != null)
                {
                    is_paused = true;
                    onPauseGameplay.Invoke();
                }
            }
        }

        public void StopEvent()
        {
            StopDialogue();

            if (current_event != null)
            {
                //Debug.Log("Stop Cinematic");
                NarrativeEvent trigger = current_event;
                current_event = null;
                current_event_line = null;
                current_message = null;
                current_actor = null;
                event_timer = 0f;
                pause_duration = 0f;
                should_unpause = is_paused;

                event_line_queue.Clear();

                if (onEventEnd != null)
                    onEventEnd.Invoke(trigger);
                if (trigger.onEnd != null)
                    trigger.onEnd.Invoke();
            }
        }

        public void StartEventLine(NarrativeEventLine line)
        {
            if (current_event_line != line)
            {
                current_event_line = line;
                current_message = null;
                event_timer = 0f;
                pause_duration = 0f;

                pause_duration = current_event_line.TriggerEffects();

                if (line.dialogue != null)
                {
                    StartDialogue(line.dialogue);
                }
            }
        }

        public void StopEventLine()
        {
            if (current_event_line != null)
            {
                current_event_line = null;
                current_message = null;
                current_actor = null;
                event_timer = 0f;
                pause_duration = 0f;
            }
        }

        public void StartDialogue(DialogueMessage dialogue)
        {
            StopDialogue();

            //Debug.Log("Start Dialogue " + dialogue_index);
            current_message = dialogue;
            current_actor = dialogue.actor;
            pause_duration = current_message.pause;

            NarrativeData.Get().AddToHistory(new DialogueMessageData(current_actor.actor_id, current_actor.GetTitle(), current_message.GetText()));

            event_timer = 0f;

            if (onDialogueMessageStart != null)
                onDialogueMessageStart.Invoke(current_event_line, current_message);

            if (current_message.onStart != null)
                current_message.onStart.Invoke();
        }

        public void SelectChoice(int choice_index)
        {
            if (current_event_line != null && current_event_line.GetChoice(choice_index) != null) {

                StopDialogue();

                DialogueChoice choice = current_event_line.GetChoice(choice_index);
                if (choice.onSelect != null)
                    choice.onSelect.Invoke(choice.choice_index);

                if (choice.go_to != null)
                    choice.go_to.TriggerImmediately(current_event.GetLastTriggerer());
            }
        }

        public void StopDialogue()
        {
            if (current_message != null) {

                if (current_message.onEnd != null)
                    current_message.onEnd.Invoke();
                if (onDialogueMessageEnd != null)
                    onDialogueMessageEnd.Invoke(current_event_line, current_message);

                pause_duration = current_message.pause;
                current_message = null;
                current_actor = null;
                event_timer = 0f;
            }
        }

        public void StartQuest(QuestData quest)
        {
            if (quest != null && !NarrativeData.Get().IsQuestStarted(quest.quest_id))
            {
                NarrativeData.Get().StartQuest(quest.quest_id);

                if (onQuestStart != null)
                    onQuestStart.Invoke(quest);
                if (quest is QuestAutoData)
                    ((QuestAutoData)quest).OnStart();
            }
        }

        public void CompleteQuest(QuestData quest)
        {
            if (quest != null && NarrativeData.Get().IsQuestActive(quest.quest_id))
            {
                NarrativeData.Get().CompleteQuest(quest.quest_id);

                if (onQuestComplete != null)
                    onQuestComplete.Invoke(quest);
                if (quest is QuestAutoData)
                    ((QuestAutoData)quest).OnComplete();
            }
        }

        public void FailQuest(QuestData quest)
        {
            if (quest != null && NarrativeData.Get().IsQuestActive(quest.quest_id))
            {
                NarrativeData.Get().FailQuest(quest.quest_id);

                if (onQuestFail != null)
                    onQuestFail.Invoke(quest);
                if (quest is QuestAutoData)
                    ((QuestAutoData)quest).OnFail();
            }
        }

        public void CancelQuest(QuestData quest)
        {
            if (quest != null && NarrativeData.Get().IsQuestActive(quest.quest_id))
            {
                NarrativeData.Get().CancelQuest(quest.quest_id);

                if (onQuestCancel != null)
                    onQuestCancel.Invoke(quest);
                if (quest is QuestAutoData)
                    ((QuestAutoData)quest).OnCancel();
            }
        }

        public int GetQuestStatus(QuestData quest)
        {
            if (quest != null)
                return NarrativeData.Get().GetQuestStatus(quest.quest_id);
            return 0;
        }

        public void SetQuestProgress(QuestData quest, string progress, int value)
        {
            if (quest != null)
                NarrativeData.Get().SetQuestProgress(quest.quest_id, progress, value);
        }

        public void AddQuestProgress(QuestData quest, string progress, int value)
        {
            if (quest != null)
                NarrativeData.Get().AddQuestProgress(quest.quest_id, progress, value);
        }

        public int GetQuestProgress(QuestData quest, string progress)
        {
            if (quest != null)
                return NarrativeData.Get().GetQuestProgress(quest.quest_id, progress);
            return 0;
        }

        public void PlaySFX(string channel, AudioClip clip, float vol = 0.8f)
        {
            if (onPlaySFX != null)
                onPlaySFX.Invoke(channel, clip, vol);

            if (!use_custom_audio && TheAudio.Get())
                TheAudio.Get().PlaySFX(channel, clip, vol);
        }

        public void PlayMusic(string channel, AudioClip clip, float vol = 0.4f)
        {
            if (onPlayMusic != null)
                onPlayMusic.Invoke(channel, clip, vol);
            
            if(!use_custom_audio && TheAudio.Get())
                TheAudio.Get().PlayMusic(channel, clip, vol);
        }

        public void StopMusic(string channel)
        {
            if (onStopMusic != null)
                onStopMusic.Invoke(channel);
            
            if(!use_custom_audio && TheAudio.Get())
                TheAudio.Get().StopMusic(channel);
        }

        public void RollRandomValue(string id, int min, int max)
        {
            int value = Random.Range(min, max + 1);
            random_values[id] = value;
        }

        public int GetRandomValue(string id)
        {
            if (random_values.ContainsKey(id))
                return random_values[id];
            return 0;
        }

        //Currently has an event/dialogue running
        public bool IsRunning()
        {
            return (current_event != null);
        }

        public NarrativeEvent GetCurrent()
        {
            return current_event;
        }

        public NarrativeEventLine GetCurrentLine()
        {
            return current_event_line;
        }

        public DialogueMessage GetCurrentMessage()
        {
            return current_message;
        }

        public ActorData GetCurrentActor()
        {
            return current_actor;
        }

        //Is enabled
        public static bool IsActive()
        {
            return _instance != null && _instance.enabled;
        }

        //Is ready to receive events (not already one running)
        public static bool IsReady()
        {
            return IsActive() && !_instance.IsRunning();
        }

        public static NarrativeManager Get()
        {
            return _instance;
        }
    }

}