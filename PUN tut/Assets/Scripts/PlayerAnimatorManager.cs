using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField]
        private float directionDampTime = .25f;
        private Animator animator;
        private FloatingJoystick floatingJoystick;

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            GameObject mainCanvas = GameObject.Find("Canvas");
            floatingJoystick = mainCanvas.transform.Find("Floating Joystick").GetComponent<FloatingJoystick>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            if (!animator)
            {
                return;
            }
            // deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }
            }
            float h = floatingJoystick.Horizontal;
            float v = floatingJoystick.Vertical;
            if (v < 0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion
    }
}