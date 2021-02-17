using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GercStudio.USK.Scripts
{

    public class Explosion : MonoBehaviour
    {
        [HideInInspector] public float radius = 2;
        [HideInInspector] public float force = 100;
        [HideInInspector] public float time = 1;

        [HideInInspector] public int damage;
        [HideInInspector] public int instanceId;
        
        [HideInInspector] public string ownerName;
        [HideInInspector] public bool applyForce;
        [HideInInspector] public Controller owner;
        [HideInInspector] public Vector3 startPosition;

        [HideInInspector] public Texture weaponImage;

        private List<int> charactersIds = new List<int>{-1};

        private bool anyDamage;

        void Start()
        {
            ExplosionProcess();
//            Transform[] allChildren = GetComponentsInChildren<Transform>();
//            foreach (Transform child in allChildren)
//            {
//                if (child.GetInstanceID() != GetInstanceID())
//                {
//                    child.parent = null;
//                    child.gameObject.AddComponent<DestroyObject>().destroy_time = 5;
//                }
//            }
        }

        void ExplosionProcess()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, radius);
            
            foreach (var collider in hitColliders)
            {
                if (collider.transform.root.GetComponent<EnemyController>())
                {
                    var enemyScript = collider.transform.root.GetComponent<EnemyController>();
                    enemyScript.EnemyHealth -= damage;
                    enemyScript.GetShotFromWeapon(1.5f);
                    enemyScript.PlayDamageAnimation();
                    
                    break;
                }
                
                if (collider.GetComponent<Rigidbody>() && applyForce && !collider.transform.root.GetComponent<Controller>())
                    collider.GetComponent<Rigidbody>().AddExplosionForce(force * 50, transform.position, radius, 0.0f);
                
                if (collider.transform.root.gameObject.GetComponent<Controller>())
                {
                    if (charactersIds.All(id => id != collider.transform.root.gameObject.GetInstanceID()))
                    {
                        var opponentController = collider.transform.root.gameObject.GetComponent<Controller>();
                        
                        charactersIds.Add(opponentController.gameObject.GetInstanceID());
                        
                        if (owner)
                        {
                            switch (owner.CanKillOthers)
                            {
                                case PUNHelper.CanKillOthers.OnlyOpponents:
                                    
                                    if (opponentController.MyTeam != owner.MyTeam || opponentController.MyTeam == owner.MyTeam && owner.MyTeam == PUNHelper.Teams.Null)
                                    {
#if PHOTON_UNITY_NETWORKING
                                        if (opponentController.PlayerHealth - damage <= 0 && owner.CharacterSync && opponentController != owner)
                                        {
                                            owner.CharacterSync.AddScore(PlayerPrefs.GetInt("ExplosionKill"), "explosion");
                                        }
#endif
                                        
                                        CreateHitMarker(opponentController);

                                        opponentController.ExplosionDamage(damage, owner.CharacterName, weaponImage ? weaponImage : null, opponentController.oneShotOneKill);
                                    }

                                    break;

                                case PUNHelper.CanKillOthers.Everyone:
                                    
#if PHOTON_UNITY_NETWORKING
                                    if (opponentController.MyTeam != owner.MyTeam || opponentController.MyTeam == owner.MyTeam && owner.MyTeam == PUNHelper.Teams.Null)
                                    {

                                        if (opponentController.PlayerHealth - damage <= 0 && owner.CharacterSync && opponentController != owner)
                                        {
                                            owner.CharacterSync.AddScore(PlayerPrefs.GetInt("ExplosionKill"), "explosion");
                                        }
                                    }
#endif

                                    CreateHitMarker(opponentController);
                                    
                                    opponentController.ExplosionDamage(damage, owner.CharacterName, weaponImage ? weaponImage : null, opponentController.oneShotOneKill);

                                    break;

                                case PUNHelper.CanKillOthers.NoOne:
                                    break;
                            }
                        }
                        else
                        {
                            if (ownerName == "Enemy")
                            {
                                opponentController.ExplosionDamage(damage, "Enemy", weaponImage ? weaponImage : null, false);
                                
                                var direction = startPosition - opponentController.transform.position;
                                var targetPosition = startPosition + direction * 1000;
                                CharacterHelper.CreateHitMarker(opponentController, null, targetPosition);
                            }
                        }
                    }
                }

                if (collider.GetComponent<FlyingProjectile>() && collider.gameObject.GetInstanceID() != instanceId)
                {
                    collider.GetComponent<FlyingProjectile>().Explosion();
                    break;
                }
            }

            //Destroy(gameObject, Time);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        void CreateHitMarker(Controller opponentController)
        {
            if (opponentController != owner)
            {
#if PHOTON_UNITY_NETWORKING
                if (opponentController.CharacterSync)
                    opponentController.CharacterSync.CreateHitMark(startPosition);
#endif
            }
            else
            {
                var direction = transform.position - opponentController.transform.position;
                var targetPosition = transform.position + direction * 1000;
                CharacterHelper.CreateHitMarker(opponentController, null, targetPosition);
            }
        }
    }

}



