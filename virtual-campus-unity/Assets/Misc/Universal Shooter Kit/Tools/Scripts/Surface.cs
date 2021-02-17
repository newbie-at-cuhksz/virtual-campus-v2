// GercStudio
// © 2018-2019

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
    public class Surface : MonoBehaviour
    {
        public SurfaceParameters Material; 
        public bool Cover;
        public bool Grass;
        
        [HideInInspector] public Transform Sparks;
        [HideInInspector] public Transform Hit;
        [HideInInspector] public AudioClip HitAudio;
        [HideInInspector] public List<AudioClip> ShellDropSounds;
        [HideInInspector] public SurfaceParameters.FootstepsSounds[] CharacterFootstepsSounds;
        [HideInInspector] public SurfaceParameters.FootstepsSounds[] EnemyFootstepsSounds;
        [HideInInspector] public GameObject Shadow;

        void Awake()
        {
            if (Material)
            {
                Array.Resize(ref CharacterFootstepsSounds, Material.CharacterFootstepsSounds.Length);
                Array.Resize(ref EnemyFootstepsSounds, Material.EnemyFootstepsSounds.Length);

                
                Sparks = Material.Sparks;
                Hit = Material.Hit;
                
                CharacterFootstepsSounds = Material.CharacterFootstepsSounds;
                EnemyFootstepsSounds = Material.EnemyFootstepsSounds;
                
                ShellDropSounds = Material.ShellDropSounds;
                HitAudio = Material.HitAudio;
            }

            if (Grass)
                gameObject.layer = 10;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Grass) return;
            
            if(other.transform.root.gameObject.GetComponent<EnemyController>())
            {
                other.transform.root.gameObject.GetComponent<EnemyController>().inGrass = true;
            }

            if (other.transform.root.gameObject.GetComponent<Controller>() && other.transform.name != "Noise Collider")//&& !other.transform.CompareTag("FireCollider"))
            {
                other.transform.root.gameObject.GetComponent<Controller>().inGrass = true;
            }
            
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!Grass) return;
            
            if(other.transform.root.gameObject.GetComponent<EnemyController>())
            {
                other.transform.root.gameObject.GetComponent<EnemyController>().inGrass = false;
            }
            
            if(other.transform.root.gameObject.GetComponent<Controller>() && other.transform.name != "Noise Collider") //&& !other.transform.CompareTag("Fire Collider"))
            {
                other.transform.root.gameObject.GetComponent<Controller>().inGrass = false;
            }
        }
    }
}


