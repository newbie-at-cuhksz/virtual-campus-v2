using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests
{
    [CreateAssetMenu(fileName ="Quest", menuName = "DialogueQuests/Quest", order= 0)]
    public class QuestData : ScriptableObject {

        [Tooltip("Important: make sure all quests have a unique ID")]
        public string quest_id;

        public string title;
        public Sprite icon;
        [TextArea(3, 5)]
        public string desc;
        public int sort_order;

        public void Start(){ NarrativeManager.Get().StartQuest(this);}
        public void Complete(){ NarrativeManager.Get().CompleteQuest(this);}
        public void Fail(){ NarrativeManager.Get().FailQuest(this); }
        public void Cancel(){ NarrativeManager.Get().CancelQuest(this); }

        public void AddQuestProgress(string progress, int value){ NarrativeManager.Get().AddQuestProgress(this, progress, value); }
        public void SetQuestProgress(string progress, int value){ NarrativeManager.Get().SetQuestProgress(this, progress, value); }

        public bool IsStarted() { return NarrativeData.Get().IsQuestStarted(quest_id); }
        public bool IsActive() { return NarrativeData.Get().IsQuestActive(quest_id); }
        public bool IsCompleted() { return NarrativeData.Get().IsQuestCompleted(quest_id); }
        public bool IsFailed() { return NarrativeData.Get().IsQuestFailed(quest_id); }
        public int GetQuestStatus(){ return NarrativeData.Get().GetQuestStatus(quest_id);}
        public int GetQuestProgress(string progress) { return NarrativeData.Get().GetQuestProgress(quest_id, progress); }

        public string GetTitle()
        {
            return NarrativeTool.Translate(title);
        }

        public string GetDesc()
        {
            return NarrativeTool.Translate(desc);
        }

        public static void Load(QuestData quest)
        {
            if (NarrativeManager.Get())
            {
                List<QuestData> list = GetAll();
                if (!list.Contains(quest))
                {
                    list.Add(quest);
                }

                if (quest is QuestAutoData)
                    ((QuestAutoData)quest).OnLoad();
            }
        }

        public static QuestData Get(string actor_id)
        {
            if (NarrativeManager.Get())
            {
                foreach (QuestData quest in GetAll())
                {
                    if (quest.quest_id == actor_id)
                        return quest;
                }
            }
            return null;
        }

        public static List<QuestData> GetAllActive()
        {
            List<QuestData> valid_list = new List<QuestData>();
            foreach (QuestData aquest in GetAll())
            {
                if (NarrativeData.Get().IsQuestActive(aquest.quest_id))
                    valid_list.Add(aquest);
            }
            return valid_list;
        }

        public static List<QuestData> GetAllStarted()
        {
            List<QuestData> valid_list = new List<QuestData>();
            foreach (QuestData aquest in GetAll())
            {
                if (NarrativeData.Get().IsQuestStarted(aquest.quest_id))
                    valid_list.Add(aquest);
            }
            return valid_list;
        }

        public static List<QuestData> GetAllActiveOrCompleted()
        {
            List<QuestData> valid_list = new List<QuestData>();
            foreach (QuestData aquest in GetAll())
            {
                if (NarrativeData.Get().IsQuestActive(aquest.quest_id) || NarrativeData.Get().IsQuestCompleted(aquest.quest_id))
                    valid_list.Add(aquest);
            }
            return valid_list;
        }

        public static List<QuestData> GetAllActiveOrFailed()
        {
            List<QuestData> valid_list = new List<QuestData>();
            foreach (QuestData aquest in GetAll())
            {
                if (NarrativeData.Get().IsQuestActive(aquest.quest_id) || NarrativeData.Get().IsQuestFailed(aquest.quest_id))
                    valid_list.Add(aquest);
            }
            return valid_list;
        }

        public static List<QuestData> GetAll()
        {
            return NarrativeManager.Get().quest_list;
        }
    }
}
