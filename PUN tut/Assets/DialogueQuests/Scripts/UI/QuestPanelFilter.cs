using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests {

    public enum QuestFilter {
        None=0,
        Active=10,
        Completed=20,
        Failed=30,
    }

    public class QuestPanelFilter : MonoBehaviour
    {
        public QuestFilter filter;
        public Image highlight;

        private QuestPanel quest_panel;

        void Start()
        {
            quest_panel = GetComponentInParent<QuestPanel>();
            Button button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        void Update()
        {
            if (highlight != null && quest_panel != null)
                highlight.enabled = quest_panel.GetFilter() == filter;
        }

        void OnClick()
        {
            if(quest_panel != null)
                quest_panel.Filter(filter);
        }
    }
}
