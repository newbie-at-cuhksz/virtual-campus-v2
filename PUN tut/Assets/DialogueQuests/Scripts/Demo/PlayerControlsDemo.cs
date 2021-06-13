using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests.Demo
{

    /// <summary>
    /// Keyboard controls manager
    /// </summary>

    public class PlayerControlsDemo : MonoBehaviour
    {
        public KeyCode action_key1 = KeyCode.Space;
        public KeyCode action_key2 = KeyCode.Return;

        public KeyCode cam_rotate_left = KeyCode.Q;
        public KeyCode cam_rotate_right = KeyCode.E;

        public delegate Vector2 MoveAction();
        public delegate bool PressAction();

        [HideInInspector]
        public bool gamepad_linked = false; 
        public MoveAction gamepad_move;
        public MoveAction gamepad_camera; //Triggers
        public PressAction gamepad_action; //A

        private Vector3 move;
        private float rotate_cam;
        private bool press_action;
        private bool paused = false;

        private static PlayerControlsDemo _instance;

        void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            if (NarrativeManager.Get())
            {
                NarrativeManager.Get().onPauseGameplay += () => { paused = true; };
                NarrativeManager.Get().onUnpauseGameplay += () => { paused = false; };
            }
        }

        void Update()
        {
            move = Vector3.zero;
            rotate_cam = 0f;
            press_action = false;

            if (paused)
                return;

            if (Input.GetKey(KeyCode.A))
                move += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                move += Vector3.right;
            if (Input.GetKey(KeyCode.W))
                move += Vector3.forward;
            if (Input.GetKey(KeyCode.S))
                move += Vector3.back;

            if (Input.GetKey(KeyCode.LeftArrow))
                move += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow))
                move += Vector3.right;
            if (Input.GetKey(KeyCode.UpArrow))
                move += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow))
                move += Vector3.back;

            if (Input.GetKey(cam_rotate_left))
                rotate_cam += -1f;
            if (Input.GetKey(cam_rotate_right))
                rotate_cam += 1f;

            if (Input.GetKeyDown(action_key1) || Input.GetKeyDown(action_key2))
                press_action = true;

            if (gamepad_linked)
            {
                Vector2 gmove = gamepad_move.Invoke();
                move += new Vector3(gmove.x, 0f, gmove.y);
                rotate_cam += gamepad_camera.Invoke().x;
                press_action = press_action || gamepad_action.Invoke();
            }

            move = move.normalized * Mathf.Min(move.magnitude, 1f);
        }

        public bool IsMoving()
        {
            return move.magnitude > 0.1f;
        }

        public bool IsPressAction()
        {
            return press_action;
        }

        public Vector3 GetMove()
        {
            return move;
        }

        public float GetRotateCam()
        {
            return rotate_cam;
        }

        public bool IsPaused()
        {
            return paused;
        }

        public static PlayerControlsDemo Get()
        {
            return _instance;
        }
    }

}