using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueQuests.Demo
{
    /// <summary>
    /// Main character script
    /// </summary>

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerCharacterDemo : MonoBehaviour
    {
        [Header("Movement")]
        public float move_speed = 4f;
        public float move_accel = 8;
        public float rotate_speed = 180f;
        public float fall_speed = 20f;
        public float ground_detect_dist = 0.1f;
        public LayerMask ground_layer = ~0;

        public UnityAction<string, float> onTriggerAnim;

        private Rigidbody rigid;
        private CapsuleCollider collide;

        private Vector3 move;
        private Vector3 facing;
        private Vector3 move_average;
        private Vector3 prev_pos;

        private bool auto_move = false;
        private Vector3 auto_move_target;
        private Vector3 auto_move_target_next;
        private float auto_move_timer = 0f;

        private bool is_grounded = false;
        private bool is_fronted = false;
        private bool is_action = false;

        private static PlayerCharacterDemo _instance;

        void Awake()
        {
            _instance = this;
            rigid = GetComponent<Rigidbody>();
            collide = GetComponentInChildren<CapsuleCollider>();
            facing = transform.forward;
        }

        private void Start()
        {
            PlayerControlsMouseDemo mouse_controls = PlayerControlsMouseDemo.Get();
            mouse_controls.onClickFloor += OnClickFloor;
            mouse_controls.onClick += OnClick;
            mouse_controls.onRightClick += OnRightClick;
        }

        void FixedUpdate()
        {
            PlayerControlsDemo controls = PlayerControlsDemo.Get();
            PlayerControlsMouseDemo mcontrols = PlayerControlsMouseDemo.Get();
            Vector3 tmove = Vector3.zero;

            //Moving
            auto_move_timer += Time.fixedDeltaTime;
            if (auto_move && auto_move_timer > 0.02f) //auto_move_timer to let the navmesh time to calculate a path
            {
                Vector3 move_dir_total = auto_move_target - transform.position;
                Vector3 move_dir_next = auto_move_target_next - transform.position;
                Vector3 move_dir = move_dir_next.normalized * Mathf.Min(move_dir_total.magnitude, 1f);
                move_dir.y = 0f;

                float move_dist = Mathf.Min(move_speed , move_dir.magnitude * 10f);
                tmove = move_dir.normalized * move_dist;
            }
            else 
            {
                Vector3 cam_move = TheCameraDemo.Get().GetRotation() * controls.GetMove();
                tmove = cam_move * move_speed;
            }

            //Stop moving if doing action
            if (is_action)
                tmove = Vector3.zero;

            //Check ground
            DetectGrounded();

            //Falling
            if (!is_grounded )
            {
                tmove += Vector3.down * fall_speed;
            }

            //Do move
            move = Vector3.Lerp(move, tmove, move_accel * Time.fixedDeltaTime);
            rigid.velocity = move;

            //Facing
            if (!is_action && IsMoving())
            {
                facing = new Vector3(move.x, 0f, move.z).normalized;
            }

            Quaternion targ_rot = Quaternion.LookRotation(facing, Vector3.up);
            rigid.MoveRotation(Quaternion.RotateTowards(rigid.rotation, targ_rot, rotate_speed * Time.fixedDeltaTime));

            //Fronted (need to be done after facing)
            DetectFronted();

            //Traveled calcul
            Vector3 last_frame_travel = transform.position - prev_pos;
            move_average = Vector3.MoveTowards(move_average, last_frame_travel, 1f * Time.fixedDeltaTime);
            prev_pos = transform.position;

            //Stop auto move
            bool stuck_somewhere = move_average.magnitude < 0.02f && auto_move_timer > 1f;
            if (stuck_somewhere)
                auto_move = false;

            if (controls.IsMoving())
                auto_move = false;
        }

        private void Update()
        {
            PlayerControlsDemo controls = PlayerControlsDemo.Get();


            //Stop move
            Vector3 move_dir = auto_move_target - transform.position;
            if (auto_move && !is_action && move_dir.magnitude < 0.35f)
            {
                auto_move = false;
            }

            //Press Action button
            if (!is_action)
            {
                if (controls.IsPressAction())
                {
                    
                }
            }
        }

        //Detect if character is on the floor
        private void DetectGrounded()
        {
            Vector3 scale = transform.lossyScale;
            float hradius = collide.height * scale.y * 0.5f + ground_detect_dist; //radius is half the height minus offset
            float radius = collide.radius * (scale.x + scale.y) * 0.5f;

            Vector3 center = collide.transform.position + Vector3.Scale(collide.center, scale);
            Vector3 p1 = center;
            Vector3 p2 = center + Vector3.left * radius;
            Vector3 p3 = center + Vector3.right * radius;
            Vector3 p4 = center + Vector3.forward * radius;
            Vector3 p5 = center + Vector3.back * radius;

            RaycastHit h1, h2, h3, h4, h5;
            bool f1 = Physics.Raycast(p1, Vector3.down, out h1, hradius, ground_layer.value);
            bool f2 = Physics.Raycast(p2, Vector3.down, out h2, hradius, ground_layer.value);
            bool f3 = Physics.Raycast(p3, Vector3.down, out h3, hradius, ground_layer.value);
            bool f4 = Physics.Raycast(p4, Vector3.down, out h4, hradius, ground_layer.value);
            bool f5 = Physics.Raycast(p5, Vector3.down, out h5, hradius, ground_layer.value);

            is_grounded = f1 || f2 || f3 || f4 || f5;

            //Debug.DrawRay(p1, Vector3.down * hradius);
            //Debug.DrawRay(p2, Vector3.down * hradius);
            //Debug.DrawRay(p3, Vector3.down * hradius);
            //Debug.DrawRay(p4, Vector3.down * hradius);
            //Debug.DrawRay(p5, Vector3.down * hradius);
        }

        //Detect if there is an obstacle in front of the character
        private void DetectFronted()
        {
            Vector3 scale = transform.lossyScale;
            float hradius = collide.height * scale.y * 0.5f - 0.02f; //radius is half the height minus offset
            float radius = collide.radius * (scale.x + scale.y) * 0.5f + 0.5f;

            Vector3 center = collide.transform.position + Vector3.Scale(collide.center, scale);
            Vector3 p1 = center;
            Vector3 p2 = center + Vector3.up * hradius;
            Vector3 p3 = center + Vector3.down * hradius;

            RaycastHit h1, h2, h3;
            bool f1 = Physics.Raycast(p1, facing, out h1, radius);
            bool f2 = Physics.Raycast(p2, facing, out h2, radius);
            bool f3 = Physics.Raycast(p3, facing, out h3, radius);

            is_fronted = f1 || f2 || f3;

            //Debug.DrawRay(p1, facing * radius);
            //Debug.DrawRay(p2, facing * radius);
            //Debug.DrawRay(p3, facing * radius);
        }

        public void FaceTorward(Vector3 pos)
        {
            Vector3 face = (pos - transform.position);
            face.y = 0f;
            if (face.magnitude > 0.01f)
            {
                facing = face.normalized;
            }
        }

        public void TriggerAnim(string anim, float duration = 0f)
        {
            if (onTriggerAnim != null)
                onTriggerAnim.Invoke(anim, duration);
        }

        //Just animate the character for X seconds, and prevent it from doing other things, then callback
        public void TriggerAction(string anim, Vector3 face_at, float duration = 0.5f, UnityAction callback = null)
        {
            if (!is_action)
            {
                FaceTorward(face_at);
                TriggerAnim(anim, duration);
                StartCoroutine(RunAction(duration, callback));
            }
        }

        private IEnumerator RunAction(float action_duration, UnityAction callback)
        {
            is_action = true;
            yield return new WaitForSeconds(action_duration);
            is_action = false;
            if (callback != null)
                callback.Invoke();
        }

        //----- Player Orders ----------

        public void MoveTo(Vector3 pos)
        {
            auto_move = true;
            auto_move_target = pos;
            auto_move_target_next = pos;
            auto_move_timer = 0f;
        }

        public void StopMove()
        {
            auto_move = false;
            auto_move_target = transform.position;
            move = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }

        //------- Mouse Clicks --------

        private void OnClick(Vector3 pos)
        {
            
        }

        private void OnRightClick(Vector3 pos)
        {

        }

        private void OnClickFloor(Vector3 pos)
        {
            MoveTo(pos);
        }


        public bool IsMoving()
        {
            Vector3 moveXZ = new Vector3(move.x, 0f, move.z);
            return moveXZ.magnitude > move_speed * 0.25f;
        }

        public Vector3 GetMove()
        {
            return move;
        }

        public Vector3 GetFacing()
        {
            return facing;
        }

        public bool IsFronted()
        {
            return is_fronted;
        }

        public static PlayerCharacterDemo Get()
        {
            return _instance;
        }
    }

}