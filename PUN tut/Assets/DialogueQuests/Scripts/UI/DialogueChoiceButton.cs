using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests {

    public class DialogueChoiceButton : MonoBehaviour
    {
        public Text text;
        public Image highlight;

        private RectTransform rect;
        private int index;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void ShowButton(int index, DialogueChoice choice)
        {
            this.index = index;
            text.text = choice.GetText();
            gameObject.SetActive(true);

            if (highlight != null)
                highlight.enabled = false;
        }

        public void SetHighlight(bool visible)
        {
            if (highlight != null)
                highlight.enabled = visible;
        }

        public void HideButton()
        {
            gameObject.SetActive(false);
            if (highlight != null)
                highlight.enabled = false;
        }

        public void OnClick()
        {
            NarrativeManager.Get().SelectChoice(index);
        }

        public RectTransform GetRect()
        {
            return rect;
        }
    }

}
