using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueQuests.Demo
{

    /// <summary>
    /// Manages all character animations
    /// </summary>

    [RequireComponent(typeof(PlayerCharacterDemo))]
    public class PlayerCharacterAnimDemo : MonoBehaviour
    {
        public string move_anim = "Move";

        private PlayerCharacterDemo character;
        private Animator animator;

        private static PlayerCharacterAnimDemo _instance;

        void Awake()
        {
            _instance = this;
            character = GetComponent<PlayerCharacterDemo>();
            animator = GetComponentInChildren<Animator>();

            character.onTriggerAnim += OnTriggerAnim;

            if (animator == null)
                enabled = false;
        }

        void Update()
        {
            //animator.enabled = !paused;

            if (animator.enabled)
            {
                animator.SetBool(move_anim, character.IsMoving());
            }
        }

        private void OnTriggerAnim(string anim, float duration)
        {
            if(!string.IsNullOrEmpty(anim))
                animator.SetTrigger(anim);
        }

        public static PlayerCharacterAnimDemo Get()
        {
            return _instance;
        }
    }

}