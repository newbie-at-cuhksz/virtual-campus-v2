using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests
{

    public class QuestPanelLine : MonoBehaviour {

        public Text quest_title;
        public Text quest_text;
        public Image quest_icon;
        public Image quest_completed;

        public Sprite success_sprite;
        public Sprite fail_sprite;

        void Awake()
        {

        }

        public void SetLine(QuestData quest)
        {
            quest_title.text = quest.GetTitle();

            if (quest_text != null)
                quest_text.text = quest.GetDesc();

            if (quest_icon != null)
            {
                quest_icon.sprite = quest.icon;
                quest_icon.enabled = quest.icon != null;
            }

            if (quest_completed != null)
            {
                bool completed = NarrativeData.Get().IsQuestCompleted(quest.quest_id);
                bool failed = NarrativeData.Get().IsQuestFailed(quest.quest_id);
                quest_completed.enabled = completed || failed;
                quest_completed.sprite = completed ? success_sprite : fail_sprite;
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

}
