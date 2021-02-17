using System;
using System.Collections;
using System.Collections.Generic;
using GercStudio.USK.Scripts;
using UnityEngine;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

namespace GercStudio.USK.Scripts
{

    public class BodyPartCollider : MonoBehaviour
    {
        [HideInInspector] public EnemyController EnemyController;
        [HideInInspector] public Controller Controller;

        public enum BodyPart
        {
            Head,
            Hands,
            Legs,
            Body
        }

        [HideInInspector] public BodyPart bodyPart;
        [HideInInspector] public bool checkColliders;
        [HideInInspector] public bool gettingDamage;
        [HideInInspector] public string attackType;
        [HideInInspector] public float damageMultiplayer = 2;
        [HideInInspector] public GameObject attacking;
        [HideInInspector] public bool registerDeath;

        private void LateUpdate()
        {
            gettingDamage = false;
            attackType = "";
            attacking = null;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Fire"))
            {
                gettingDamage = true;
                attackType = "Fire";
                attacking = other.transform.root.gameObject;
            }

            if (!checkColliders)
                return;

            if (EnemyController && other.CompareTag("Smoke"))
            {
                EnemyController.InSmoke = true;
            }

            if (EnemyController && other.transform.root.GetComponent<Controller>() && !other.CompareTag("Melee Collider") && !other.CompareTag("Fire"))
            {
                foreach (var player in EnemyController.Players)
                {
                    if (player.player.Equals(other.transform.root.gameObject))
                    {
                        if (!player.isObstacle)
                        {
                            player.HearPlayer = true;
                            EnemyController.IKnowWherePlayerIs = false;
                        }
                        else
                        {
                            player.HearPlayer = false;
                        }
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Melee Collider"))
            {
                gettingDamage = true;
                attackType = "Melee";
                attacking = other.transform.root.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!checkColliders)
                return;

            if (EnemyController)
            {
                if (other.CompareTag("Smoke"))
                {
                    EnemyController.InSmoke = false;
                }

                if (other.transform.root.GetComponent<Controller>())
                {
                    foreach (var player in EnemyController.Players)
                    {
                        if (player.player.Equals(other.transform.root.gameObject))
                        {
                            player.HearPlayer = false;
                        }
                    }
                }
            }
        }
    }
}
