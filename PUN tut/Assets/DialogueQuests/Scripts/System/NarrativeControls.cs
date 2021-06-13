using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests
{
    /// <summary>
    /// Manages Controls for the dialogue/quests system
    /// </summary>

    public class NarrativeControls : MonoBehaviour
    {
        public KeyCode talk_key = KeyCode.Space;
        public KeyCode journal_key = KeyCode.J;
        public KeyCode cancel_key = KeyCode.Backspace;
        public bool mouse_controls = true;
        public bool keyboard_controls = true;

        public UnityAction onPressTalk;
        public UnityAction onPressJournal;
        public UnityAction onPressCancel;
        public UnityAction onPressTalkMouse;
        public UnityAction onPressCancelMouse;
        public UnityAction<Vector2> onPressArrow;

        public delegate Vector2 MoveAction();
        public delegate bool PressAction();

        [HideInInspector]
        public bool gamepad_linked = false;
        public MoveAction gamepad_menu;
        public PressAction gamepad_talk; //A
        public PressAction gamepad_journal; //R1
        public PressAction gamepad_cancel; //B

        private Vector2 ui_move;
        private bool ui_moved;


        private static NarrativeControls _instance;

        void Awake()
        {
            _instance = this;
        }

        void Update()
        {
            //Key controls
            if (keyboard_controls && talk_key != KeyCode.None && Input.GetKeyDown(talk_key))
                OnPressTalk(false);
            if (keyboard_controls && journal_key != KeyCode.None && Input.GetKeyDown(journal_key))
                OnPressJournal();
            if (keyboard_controls && cancel_key != KeyCode.None && Input.GetKeyDown(cancel_key))
                OnPressCancel(false);
            if (mouse_controls && Input.GetMouseButtonDown(0))
                OnPressTalk(true);
            if (mouse_controls && Input.GetMouseButtonDown(1))
                OnPressCancel(true);

            if (keyboard_controls)
            {
                Vector3 arrow = GetArrowControl();
                if (!ui_moved && arrow.magnitude > 0.5f)
                {
                    ui_move = new Vector2(arrow.x, arrow.z);
                    ui_moved = true;
                    OnPressArrow(ui_move);
                }

                if (arrow.magnitude < 0.5f)
                    ui_moved = false;

                if (gamepad_linked)
                {
                    if (gamepad_talk.Invoke())
                        OnPressTalk(false);
                    if (gamepad_journal.Invoke())
                        OnPressJournal();
                    if (gamepad_cancel.Invoke())
                        OnPressCancel(false);
                }
            }
        }

        private Vector2 GetArrowControl()
        {
            Vector2 wasd = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
                wasd += Vector2.left;
            if (Input.GetKey(KeyCode.D))
                wasd += Vector2.right;
            if (Input.GetKey(KeyCode.W))
                wasd += Vector2.up;
            if (Input.GetKey(KeyCode.S))
                wasd += Vector2.down;

            Vector2 arrows = Vector2.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
                arrows += Vector2.left;
            if (Input.GetKey(KeyCode.RightArrow))
                arrows += Vector2.right;
            if (Input.GetKey(KeyCode.UpArrow))
                arrows += Vector2.up;
            if (Input.GetKey(KeyCode.DownArrow))
                arrows += Vector2.down;

            Vector2 menu = Vector2.zero;
            if (gamepad_linked)
            {
                menu += gamepad_menu.Invoke();
            }

            return wasd + arrows + menu;
        }

        private void OnPressArrow(Vector2 arrow)
        {
            if (onPressArrow != null)
                onPressArrow.Invoke(arrow);
        }

        private void OnPressTalk(bool mouse)
        {
            if (!mouse && onPressTalk != null)
                onPressTalk.Invoke();
            if (mouse && onPressTalkMouse != null)
                onPressTalkMouse.Invoke();
        }

        private void OnPressCancel(bool mouse)
        {
            if (!mouse && onPressCancel != null)
                onPressCancel.Invoke();
            if (mouse && onPressCancelMouse != null)
                onPressCancelMouse.Invoke();
        }

        private void OnPressJournal()
        {
            if (onPressJournal != null)
                onPressJournal.Invoke();
        }

        public static NarrativeControls Get()
        {
            return _instance;
        }
    }

}