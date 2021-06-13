using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueQuests
{
    public class DialoguePanel : UIPanel
    {
        public DialogueMessageType type;

        [Header("Ref")]
        public Image portrait;
        public Animator portrait_animator;
        public Image title_box;
        public Text title;
        public Text text;
        public Image arrow;

        [Header("Type FX")]
        public bool type_fx = true;
        public float type_fx_speed = 30f;

        [Header("Choices")]
        public DialogueChoiceButton[] choices;

        private NarrativeEventLine current_line;
        private string current_text = "";
        private bool text_skipable;
        private bool event_skipable;
        
        private string set_anim = "";
        private bool should_hide = false;
        private float hide_timer = 0f;
        private int selected_arrow = 0;
        private RectTransform arrow_rect;

        private Coroutine text_anim;
        private bool text_anim_completed = true;

        private static DialoguePanel _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;

            if (arrow != null)
            {
                arrow_rect = arrow.GetComponent<RectTransform>();
                arrow.enabled = false;
            }
        }

        protected override void Start()
        {
            base.Start();
            Hide();

            if (NarrativeManager.Get())
            {
                NarrativeManager.Get().onDialogueMessageStart += OnStart;
                NarrativeManager.Get().onDialogueMessageEnd += OnEnd;
            }

            if(NarrativeControls.Get())
            {
                NarrativeControls.Get().onPressTalk += OnClickChoice;
                NarrativeControls.Get().onPressTalk += OnClickSkip;
                NarrativeControls.Get().onPressTalkMouse += OnClickSkip;
                NarrativeControls.Get().onPressArrow += OnClickArrow;
                NarrativeControls.Get().onPressCancel += OnPressCancel;
                NarrativeControls.Get().onPressCancelMouse += OnPressCancel;
            }
        }

        protected override void Update()
        {
            base.Update();

            hide_timer += Time.deltaTime;
            if (IsVisible() && should_hide && hide_timer > 0.2f)
                Hide();

            //Set current choice visibility
            if (IsVisible() && NarrativeControls.Get().keyboard_controls)
            {
                foreach (DialogueChoiceButton button in choices)
                    button.SetHighlight(false);

                if(arrow != null)
                    arrow.enabled = HasChoices();

                if (selected_arrow >= 0 && selected_arrow < choices.Length) {

                    DialogueChoiceButton button = choices[selected_arrow];
                    button.SetHighlight(true);

                    if (arrow_rect != null)
                    {
                        arrow_rect.anchoredPosition = button.GetRect().anchoredPosition
                        + (Vector2.left * button.GetRect().sizeDelta.x * 0.5f)
                        + (Vector2.left * arrow_rect.sizeDelta.x * 0.5f);
                    }
                }
            }

            //Set anim
            if (IsVisible() && portrait_animator != null)
            {
                if (portrait_animator.runtimeAnimatorController != null && !string.IsNullOrEmpty(set_anim))
                {
                    if (HasParameter(portrait_animator, set_anim))
                        portrait_animator.SetTrigger(set_anim);
                    else
                        portrait_animator.Rebind();
                    set_anim = "";
                }
            }
        }

        public void SetDialogue(NarrativeEventLine line, DialogueMessage msg)
        {
            current_line = line;
            current_text = msg.GetText();
            this.text.text = current_text;

            if (title != null)
                title.text = msg.actor ? msg.actor.GetTitle() : "";

            if (portrait_animator != null)
            {
                portrait_animator.runtimeAnimatorController = msg.actor.animation;
                set_anim = msg.anim; //Set animation next frame, or it wont work if controller was just set
            }

            if (portrait != null)
                portrait.color = Color.white; //Revert from animation
            if (portrait != null)
                portrait.sprite = msg.actor ? msg.actor.portrait : null;

            if (title_box != null)
                title_box.enabled = msg.actor != null;
            if (portrait != null)
                portrait.enabled = msg.actor != null;
            if (title != null)
                title.enabled = msg.actor != null;

            text_skipable = line.parent.dialogue_type == DialogueMessageType.DialoguePanel;
            event_skipable = line.parent.skipable;
            text_anim_completed = true;
            should_hide = false;
            selected_arrow = 0;

            if (type_fx && type_fx_speed > 1f)
            {
                text.text = "";
                gameObject.SetActive(true); //Allow starting coroutine
                text_anim_completed = false;
                text_anim = StartCoroutine(AnimateText());
            }

            foreach (DialogueChoiceButton button in choices)
                button.HideButton();

            if (line.choices.Count > 0 && choices.Length > 0)
            {
                text_skipable = false;
                for (int i = 0; i < line.choices.Count; i++)
                {
                    if (i < choices.Length)
                    {
                        DialogueChoiceButton button = choices[i];
                        DialogueChoice choice = line.choices[i];
                        button.ShowButton(i, choice);
                    }
                }
            }
        }

        public void SkipTextAnim()
        {
            this.text.text = current_text;
            text_anim_completed = true;
            if(text_anim != null)
                StopCoroutine(text_anim);
        }

        public bool IsTextAnimCompleted()
        {
            return text_anim_completed;
        }

        IEnumerator AnimateText()
        {
            for (int i = 0; i < (current_text.Length + 1); i++)
            {
                this.text.text = current_text.Substring(0, i);
                yield return new WaitForSeconds(1f/type_fx_speed);
            }
            text_anim_completed = true;
        }

        public void OnClickChoice()
        {
            if (IsVisible() && HasChoices() && IsTextAnimCompleted())
            {
                NarrativeManager.Get().SelectChoice(selected_arrow);
            }
        }

        public void OnClickSkip()
        {
            if (IsVisible() && !HasChoices() && text_skipable)
            {
                if (IsTextAnimCompleted())
                    NarrativeManager.Get().StopDialogue();
                else
                    SkipTextAnim();
            }
        }

        public void OnPressCancel()
        {
            if (IsVisible() && event_skipable)
            {
                NarrativeManager.Get().StopEvent();
                Hide();
            }
        }

        public void OnClickArrow(Vector2 arrow)
        {
            if (IsVisible() && HasChoices())
            {
                if (arrow.x > 0.5f)
                    selected_arrow++;
                if (arrow.x < -0.5f)
                    selected_arrow--;
                if (arrow.y > 0.5f)
                    selected_arrow += 2;
                if (arrow.y < -0.5f)
                    selected_arrow -= 2;

                selected_arrow = Mathf.Clamp(selected_arrow, 0, current_line.choices.Count-1);
            }
        }

        public bool HasChoices() {
            return current_line != null && current_line.choices.Count > 0;
        }

        private bool HasParameter(Animator animator, string name)
        {
            if (animator.runtimeAnimatorController != null)
            {
                foreach (AnimatorControllerParameter param in animator.parameters)
                {
                    if (param.name == name)
                        return true;
                }
            }
            return false;
        }

        private void OnStart(NarrativeEventLine line, DialogueMessage msg)
        {
            if (line.parent.dialogue_type == type)
            {
                SetDialogue(line, msg);
                Show();

                NarrativeManager.Get().PlaySFX("dialogue", msg.audio_clip);
            }
        }

        private void OnEnd(NarrativeEventLine line, DialogueMessage msg)
        {
            if (IsVisible())
            {
                if (text_anim != null)
                    StopCoroutine(text_anim);
                text.text = current_text;
                should_hide = true;
                hide_timer = 0f;
            }
        }

        public static DialoguePanel Get()
        {
            return _instance;
        }
    }

}