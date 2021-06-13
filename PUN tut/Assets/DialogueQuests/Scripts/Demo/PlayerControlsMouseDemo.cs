using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DialogueQuests.Demo
{

    /// <summary>
    /// Mouse/Touch controls manager
    /// </summary>

    public class PlayerControlsMouseDemo : MonoBehaviour
    {
        public LayerMask selectable_layer = ~0;
        public LayerMask floor_layer = (1 << 9); //Put to none to always return 0 as floor height

        public UnityAction<Vector3> onClick; //Always triggered on left click
        public UnityAction<Vector3> onRightClick; //Always triggered on right click
        public UnityAction<Vector3> onClickFloor; //When click on floor

        private bool using_mouse = false;
        private float mouse_scroll = 0f;
        private Vector2 mouse_delta = Vector2.zero;
        private bool mouse_hold_left = false;
        private bool mouse_hold_right = false;
        private bool paused = false;

        private float using_timer = 0f;
        private Vector3 last_pos;
        private Vector3 floor_pos; //World position the floor pointing at

        private float zoom_value = 0f;

        private static PlayerControlsMouseDemo _instance;

        void Awake()
        {
            _instance = this;
            last_pos = Input.mousePosition;
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
            if (paused)
                return;

            RaycastFloorPos();

            //Mouse click
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseClick();
            }

            //Mouse scroll
            mouse_scroll = Input.mouseScrollDelta.y;

            //Mouse delta
            mouse_delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            //Check for mouse usage
            Vector3 diff = (Input.mousePosition - last_pos);
            float dist = diff.magnitude;
            if (dist > 0.01f)
            {
                using_mouse = true;
                using_timer = 1f;
                last_pos = Input.mousePosition;
            }

            mouse_hold_left = Input.GetMouseButton(0) && !IsMouseOverUI();
            mouse_hold_right = Input.GetMouseButton(1) && !IsMouseOverUI();
            if (mouse_hold_left || mouse_hold_right)
                using_timer = 1f;

            //Is using mouse? (vs keyboard)
            using_timer -= Time.deltaTime;
            using_mouse = using_timer > 0f;

        }

        public void RaycastFloorPos()
        {
            Ray ray = GetMouseCameraRay();
            RaycastHit hit;
            bool success = Physics.Raycast(ray, out hit, 100f, floor_layer.value);
            if (success)
            {
                floor_pos = ray.GetPoint(hit.distance);
            }
            else
            {
                Plane plane = new Plane(Vector3.up, 0f);
                float dist;
                bool phit = plane.Raycast(ray, out dist);
                if (phit)
                {
                    floor_pos = ray.GetPoint(dist);
                }
            }

            //Debug.DrawLine(TheCamera.GetCamera().transform.position, floor_pos);
        }

        private void OnMouseClick()
        {
            if (IsMouseOverUI())
                return;

            if (onClick != null)
                onClick.Invoke(floor_pos);
            if (onClickFloor != null)
                onClickFloor.Invoke(floor_pos);
        }

        private void OnRightMouseClick()
        {
            if (IsMouseOverUI())
                return;

            if (onRightClick != null)
                onRightClick.Invoke(floor_pos);
        }

        public Vector2 GetScreenPos()
        {
            //In percentage
            Vector3 mpos = Input.mousePosition;
            return new Vector2(mpos.x / (float)Screen.width, mpos.y / (float)Screen.height);
        }

        public Vector3 GetPointingPos()
        {
            return floor_pos;
        }

        public bool IsUsingMouse()
        {
            return using_mouse;
        }

        public bool IsMouseHold()
        {
            return mouse_hold_left;
        }

        public bool IsMouseHoldRight()
        {
            return mouse_hold_right;
        }

        public float GetMouseScroll()
        {
            return mouse_scroll;
        }

        public Vector2 GetMouseDelta()
        {
            return mouse_delta;
        }

        public float GetTouchZoom()
        {
            return zoom_value;
        }

        public bool IsPaused()
        {
            return paused;
        }

        //Clamped to screen mouse pos
        private Vector3 GetClampMousePos()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0f, Screen.height);
            return mousePos;
        }

        //Get ray from camera to mouse
        private Ray GetMouseCameraRay()
        {
            return TheCameraDemo.GetCamera().ScreenPointToRay(GetClampMousePos());
        }

        //Check if mouse is on top of any UI element
        public bool IsMouseOverUI()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static PlayerControlsMouseDemo Get()
        {
            return _instance;
        }
    }

}
