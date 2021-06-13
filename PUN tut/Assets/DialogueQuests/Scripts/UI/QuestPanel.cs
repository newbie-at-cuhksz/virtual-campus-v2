using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests
{

    public class QuestPanel : UIPanel {

        [Header("Quests")]
        public bool show_failed = true;
        public bool show_completed = true;

        [Header("Grid Display")]
        public RectTransform content;
        public GridLayoutGroup grid;
        public QuestPanelLine line_template;
        public int nb_per_row = 2;

        private List<QuestPanelLine> lines = new List<QuestPanelLine>();
        private QuestFilter filter = QuestFilter.None;

        private static QuestPanel _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
            line_template.gameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();

            if (NarrativeControls.Get())
            {
                NarrativeControls.Get().onPressJournal += TogglePanel;
            }
        }

        protected override void Update () {

            base.Update();
        }

        private void RefreshPanel()
        {
            foreach (QuestPanelLine line in lines)
                line.Hide();

            List<QuestData> all_quest;
            if (show_failed && show_completed)
                all_quest = QuestData.GetAllStarted();
            else if (show_completed)
                all_quest = QuestData.GetAllActiveOrCompleted();
            else if (show_failed)
                all_quest = QuestData.GetAllActiveOrFailed();
            else
                all_quest = QuestData.GetAllActive();

            all_quest.Sort((p1, p2) =>
            {
                return (p1.sort_order == p2.sort_order)
                    ? p1.title.CompareTo(p2.title) : p1.sort_order.CompareTo(p2.sort_order);
            });

            foreach (QuestPanelLine line in lines)
                Destroy(line.gameObject);
            lines.Clear();

            for (int i = 0; i < all_quest.Count; i++)
            {
                QuestData quest = all_quest[i];
                if (MatchFilter(quest))
                {
                    GameObject line_obj = Instantiate(line_template.gameObject, grid.transform);
                    QuestPanelLine line = line_obj.GetComponent<QuestPanelLine>();
                    lines.Add(line);
                    line.SetLine(quest);
                }
            }

            float height = (grid.spacing.y + grid.cellSize.y) * Mathf.CeilToInt(lines.Count / (float)nb_per_row) + 20f;
            content.sizeDelta = new Vector2(content.sizeDelta.x, height);
        }

        private bool MatchFilter(QuestData quest)
        {
            if (filter == QuestFilter.Active)
                return quest.IsActive();
            if (filter == QuestFilter.Completed)
                return quest.IsCompleted();
            if (filter == QuestFilter.Failed)
                return quest.IsFailed();
            return true; //No filter
        }

        public void Filter(QuestFilter filter)
        {
            if (this.filter == filter)
                this.filter = QuestFilter.None;
            else
                this.filter = filter;

           RefreshPanel();
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            filter = QuestFilter.Active;
            RefreshPanel();
        }

        public void TogglePanel()
        {
            if (IsVisible())
                Hide();
            else
                Show();
        }

        //For compatiblity with previous version, does same thing than Show()
        public void ShowPanel()
        {
            Show();
        }

        public QuestFilter GetFilter()
        {
            return filter;
        }

        public static QuestPanel Get()
        {
            return _instance;
        }
    }

}
