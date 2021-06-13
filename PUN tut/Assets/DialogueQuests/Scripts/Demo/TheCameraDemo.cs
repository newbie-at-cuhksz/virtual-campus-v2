using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests.Demo
{
    /// <summary>
    /// Main camera script
    /// </summary>

    public class TheCameraDemo : MonoBehaviour
    {
        [Header("Move/Zoom")]
        public float move_speed = 10f;
        public float rotate_speed = 90f;
        public float zoom_speed = 0.5f;
        public float zoom_in_max = 0.5f;
        public float zoom_out_max = 1f;

        [Header("Target")]
        public GameObject follow_target;
        public Vector3 follow_offset;

        private Vector3 current_vel;
        private Vector3 rotated_offset;
        private Vector3 current_offset;
        private float current_rotate = 0f;
        private float current_zoom = 0f;
        private Transform target_transform;

        private Camera cam;

        private static TheCameraDemo _instance;

        void Awake()
        {
            _instance = this;
            cam = GetComponent<Camera>();
            rotated_offset = follow_offset;
            current_offset = follow_offset;

            GameObject cam_target = new GameObject("CameraTarget");
            target_transform = cam_target.transform;
            target_transform.position = transform.position;
            target_transform.rotation = transform.rotation;
        }

        private void Start()
        {
            if (follow_target == null && PlayerCharacterDemo.Get())
            {
                follow_target = PlayerCharacterDemo.Get().gameObject;
            }

        }

        void LateUpdate()
        {
            PlayerControlsDemo controls = PlayerControlsDemo.Get();
            PlayerControlsMouseDemo mouse = PlayerControlsMouseDemo.Get();

            //Rotate
            current_rotate = controls.GetRotateCam();
            current_rotate = -current_rotate; //Reverse rotate

            //Zoom 
            current_zoom += mouse.GetMouseScroll() * zoom_speed; //Mouse scroll zoom
            current_zoom = Mathf.Clamp(current_zoom, -zoom_out_max, zoom_in_max);

            rotated_offset = Quaternion.Euler(0, rotate_speed * current_rotate * Time.deltaTime, 0) * rotated_offset;
            current_offset = rotated_offset - rotated_offset * current_zoom;

            target_transform.RotateAround(follow_target.transform.position, Vector3.up, rotate_speed * current_rotate * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, target_transform.rotation, move_speed * Time.deltaTime);

            Vector3 target_pos = follow_target.transform.position + current_offset;
            target_transform.position = target_pos;
            transform.position = Vector3.SmoothDamp(transform.position, target_pos, ref current_vel, 1f / move_speed);

        }

        public void MoveToTarget(Vector3 target)
        {
            transform.position = target + current_offset;
        }

        public Vector3 GetTargetPos()
        {
            return transform.position - current_offset;
        }

        //Use as center for optimization
        public Vector3 GetTargetPosOffsetFace(float dist)
        {
            return transform.position - current_offset + GetFacingFront() * dist;
        }

        public Quaternion GetRotation()
        {
            return Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        public Vector3 GetFacingFront()
        {
            Vector3 dir = transform.forward;
            dir.y = 0f;
            return dir.normalized;
        }

        public Vector3 GetFacingRight()
        {
            Vector3 dir = transform.right;
            dir.y = 0f;
            return dir.normalized;
        }

        public Quaternion GetFacingRotation()
        {
            Vector3 facing = GetFacingFront();
            return Quaternion.LookRotation(facing.normalized, Vector3.up);
        }

        public Camera GetCam()
        {
            return cam;
        }

        public static Camera GetCamera()
        {
            Camera camera = _instance != null ? _instance.GetCam() : Camera.main;
            return camera;
        }

        public static TheCameraDemo Get()
        {
            return _instance;
        }
    }

}