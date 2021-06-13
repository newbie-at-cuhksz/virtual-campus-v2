using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests
{

    public class QuestBox : UIPanel {

        public Text box_title;
        public Text quest_title;
        public Image quest_icon;
        public AudioClip show_audio;

        private float timer = 0f;
        private Animator animator;

        private static QuestBox _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
            animator = GetComponentInChildren<Animator>();
        }

        protected override void Update () {

            base.Update();

            if (IsVisible())
            {
                timer += Time.deltaTime;

                if (timer > 4f)
                {
                    Hide();
                }

            }
        }

        protected override void Start()
        {
            base.Start();

            NarrativeManager.Get().onQuestStart += (QuestData quest) => { ShowBox(quest, "New Quest"); };
            NarrativeManager.Get().onQuestComplete += (QuestData quest) => { ShowBox(quest, "Quest Completed!"); };
            NarrativeManager.Get().onQuestFail += (QuestData quest) => { ShowBox(quest, "Quest Failed!"); };
        }

        public void ShowBox(QuestData quest, string text)
        {
            box_title.text = text;
            quest_title.text = quest.GetTitle();

            if (quest_icon != null)
            {
                quest_icon.sprite = quest.icon;
                quest_icon.enabled = quest.icon != null;
            }

            timer = 0f;
            Show();

            if(animator != null)
                animator.Rebind();

            if (show_audio != null)
                NarrativeManager.Get().PlaySFX("quest", show_audio);
        }

        public void OnClick()
        {
            QuestPanel.Get().Show();
        }

        public static QuestBox Get()
        {
            return _instance;
        }
    }

}
