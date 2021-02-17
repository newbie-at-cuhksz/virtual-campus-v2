// GercStudio
// © 2018-2019

using System.Collections;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    public class EnemyAttack : MonoBehaviour
    {
        private bool attackAudioPlay;

//        private RaycastHit Hit;
        
        public EnemyController EnemyController;

        private AudioSource _audio;

        private AIHelper.EnemyAttack _attack;

        public bool FlashEffectTimeout;

        private bool disableColliders;
        private float disableCollidersTimeout;
        
        
        private void Start()
        {
            _audio = GetComponent<AudioSource>();
            
            _attack = EnemyController.Attacks[0];

            foreach (var collider in _attack.DamageColliders.Where(collider => collider))
            {
                collider.isTrigger = true;
                collider.enabled = false;
            }
        }

        void Update()
        {
            if (_attack != null)
            {
                if (_attack.AttackType == AIHelper.AttackTypes.Fire && !EnemyController.anim.GetBool("Attack"))
                {
                    if (_audio.isPlaying)
                    {
                        attackAudioPlay = false;
                        _audio.Stop();
                    }

                    if (_attack.DamageColliders.Count > 0)
                    {
                        foreach (var damageCollider in _attack.DamageColliders.Where(collider => collider))
                        {
                            if (!damageCollider.isTrigger)
                                damageCollider.isTrigger = true;

                            if (damageCollider.enabled)
                                damageCollider.enabled = false;
                        }
                    }
                }

//                if (_attack.AttackType == AIHelper.AttackTypes.Melee && !EnemyController.anim.GetBool("Attack"))
//                {

                disableCollidersTimeout += Time.deltaTime;
                
                if (disableColliders && disableCollidersTimeout > 1)
                {
                    disableColliders = false;
                    
                    if (_attack.DamageColliders.Count > 0)
                    {
                        foreach (var damageCollider in _attack.DamageColliders.Where(collider => collider))
                        {
                            if (!damageCollider.isTrigger)
                                damageCollider.isTrigger = true;

                            if (damageCollider.enabled)
                                damageCollider.enabled = false;
                        }
                    }
                }
            }
        }

        public IEnumerator ReloadTimeout()
        {
            yield return new WaitForSeconds(0.5f);
            EnemyController.anim.SetBool("Reload", false);
            StopCoroutine(ReloadTimeout());
        }
        
        public IEnumerator FlashGrenadeEffect()
        {
            yield return new WaitForSeconds(3);
            EnemyController.InSmoke = false;
            FlashEffectTimeout = false;
            StopCoroutine(FlashGrenadeEffect());
        }

        public void Attack(AIHelper.EnemyAttack Attack)
        {
            switch (Attack.AttackType)
            {
                case AIHelper.AttackTypes.Bullets:
                    BulletsAttack(Attack);
                    break;
                case AIHelper.AttackTypes.Rockets:
                    RocketsAttack(Attack);
                    break;
                case AIHelper.AttackTypes.Fire:
                    FireAttack(Attack);
                    break;
                case AIHelper.AttackTypes.Melee:
                    break;
            }
        }

        void RocketsAttack(AIHelper.EnemyAttack Attack)
        {
            if (Attack.AttackAudio)
                _audio.PlayOneShot(Attack.AttackAudio);
            
            if (Attack.AttackSpawnPoints.Count > 0)
            {
                if (Attack.UseReload)
                {
                    Attack.CurrentAmmo -= 1;
                }

                for (var i = 0; i < Attack.AttackSpawnPoints.Count; i++)
                {
                    if (Attack.AttackSpawnPoints[i])
                    {
                        var rocket = Instantiate(Attack.Rocket, Attack.AttackSpawnPoints[i].position, Attack.AttackSpawnPoints[i].rotation);
                        rocket.SetActive(true);

                        var RocketScript = rocket.AddComponent<FlyingProjectile>();
                        RocketScript.startPosition = transform.position;
                        RocketScript.isRocket = true;
                        RocketScript.ApplyForce = true;
                        RocketScript.Speed = 20;
                        RocketScript.isEnemy = true;
                        RocketScript.ownerName = "Enemy";
                        RocketScript.damage = Attack.Damage;
                        RocketScript.isRaycast = true;
                        
                        if(Attack.Explosion)
                            RocketScript.explosion = Attack.Explosion.transform;
                        
                        RocketScript.TargetPoint = EnemyController.Players[0].player.GetComponent<Controller>().BodyObjects.TopBody.position +
                                                    new Vector3(Random.Range(-Attack.Scatter, Attack.Scatter), Random.Range(-Attack.Scatter, Attack.Scatter), 0);

                    }
                }
            }

        }
        
        void BulletsAttack(AIHelper.EnemyAttack Attack)
        {
            if (Attack.AttackAudio)
                _audio.PlayOneShot(Attack.AttackAudio);
            
            if (Attack.AttackSpawnPoints.Count > 0)
            {
                if (Attack.UseReload)
                {
                    Attack.CurrentAmmo -= 1;
                }
                
                for (var i = 0; i < Attack.AttackSpawnPoints.Count; i++)
                {
                    if (Attack.AttackSpawnPoints[i])
                    {
                        var Direction = Attack.AttackSpawnPoints[i].TransformDirection(Vector3.forward + new Vector3(Random.Range(-Attack.Scatter, Attack.Scatter), Random.Range(-Attack.Scatter, Attack.Scatter), 0));
                       
                        if (Attack.MuzzleFlash)
                        {
                            var Flash = Instantiate(Attack.MuzzleFlash, Attack.AttackSpawnPoints[i].position, Attack.AttackSpawnPoints[i].rotation);
                            Flash.transform.parent = Attack.AttackSpawnPoints[i].transform;
                            Flash.gameObject.AddComponent<DestroyObject>().DestroyTime = 0.17f;
                        }

                        var randomRange = new Vector3(Random.Range(-Attack.Scatter, Attack.Scatter), Random.Range(-Attack.Scatter, Attack.Scatter), 0);
                        var dir = EnemyController.Players[0].player.GetComponent<Controller>().BodyObjects.TopBody.position + randomRange - Attack.AttackSpawnPoints[i].position;
                        var dist = Vector3.Distance(Attack.AttackSpawnPoints[i].position, EnemyController.Players[0].player.GetComponent<Controller>().BodyObjects.TopBody.position + randomRange);
                        var layerMask = ~ LayerMask.GetMask("Grass");
                        var hits = Physics.RaycastAll(Attack.AttackSpawnPoints[i].position, dir, dist, layerMask);

                        if (hits.Length > 0)
                        {
                            foreach (var hit in hits)
                            {
                                if(hit.transform.name == "Noise Collider")
                                    continue;
                                
                                var HitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                                var tracer = new GameObject("Tracer");
                                tracer.transform.position = Attack.AttackSpawnPoints[i].position;
                                tracer.transform.rotation = Attack.AttackSpawnPoints[i].rotation;

                                WeaponsHelper.AddTrail(tracer, hit.point, EnemyController.trailMaterial, 0.1f);

                                if (hit.transform.GetComponent<BodyPartCollider>())
                                {
                                    var controller = hit.transform.GetComponent<BodyPartCollider>().Controller;

                                    if (controller)
                                    {
                                        if (controller.WeaponManager.BloodProjector)
                                        {
                                            WeaponsHelper.CreateBlood(controller.WeaponManager.BloodProjector, hit.point - Direction.normalized * 0.15f, Quaternion.LookRotation(Direction), hit.transform, controller.BloodHoles);
                                        }

                                        controller.Damage(Attack.Damage, "Enemy", null, false);
                                        
                                        var direction = EnemyController.transform.position - controller.transform.position;
                                        var targetPosition = EnemyController.transform.position + direction * 1000;
                                        CharacterHelper.CreateHitMarker(controller, transform, targetPosition);
                                    }
                                }


                                if (hit.transform.GetComponent<Rigidbody>())
                                    hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(Direction * 500, hit.point);

                                if (hit.collider.GetComponent<Surface>())
                                {
                                    var surface = hit.collider.GetComponent<Surface>();
                                    if (surface.Material)
                                    {
                                        if (surface.Sparks & surface.Hit)
                                        {
                                            Instantiate(surface.Sparks, hit.point + (hit.normal * 0.01f), HitRotation);
                                            var hitGO = Instantiate(surface.Hit, hit.point + (hit.normal * 0.001f), HitRotation).transform;
                                            if (surface.HitAudio)
                                            {
                                                hitGO.gameObject.AddComponent<AudioSource>();
                                                hitGO.gameObject.GetComponent<AudioSource>().clip = surface.HitAudio;
                                                hitGO.gameObject.GetComponent<AudioSource>().PlayOneShot(hitGO.gameObject.GetComponent<AudioSource>().clip);
                                            }

                                            hitGO.parent = hit.transform;
                                        }
                                    }
                                }
                                
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("(Enemy) <color=red>Missing components [AttackSpawnPoint]</Color>. Add it, otherwise the enemy won't shoot.", gameObject);
                    }
                }
            }
            else
            {
                Debug.LogError("(Enemy) <color=red>Missing components</color> [AttackSpawnPoint]. Add it, otherwise the enemy won't shoot.", gameObject);
            }
        }

        public void FireAttack(AIHelper.EnemyAttack Attack)
        {
            if (!attackAudioPlay)
            {
                attackAudioPlay = true;
                _audio.clip = Attack.AttackAudio;
                _audio.Play();
            }
            
            if (Attack.AttackSpawnPoints.Count > 0)
            {
                Attack.CurrentAmmo -= 1 * Time.deltaTime;
                for (var i = 0; i < Attack.AttackSpawnPoints.Count; i++)
                {
                    if (Attack.Fire)
                    {
                        var fire = Instantiate(Attack.Fire, Attack.AttackSpawnPoints[i].position, Attack.AttackSpawnPoints[i].rotation);
                        fire.gameObject.hideFlags = HideFlags.HideInHierarchy;
                        fire.gameObject.SetActive(true);
                    }

                    if (Attack.DamageColliders[i] && !Attack.DamageColliders[i].enabled)
                        Attack.DamageColliders[i].enabled = true;
                }
            }
        }

        public void MeleeAttack()
        {
            MeleeColliders("on");
            disableColliders = true;
            disableCollidersTimeout = 0;
        }

        public void MeleeColliders(string status)
        {
            var attack = EnemyController.Attacks[0];

            if (attack.DamageColliders.Count > 0)
            {
                foreach (var collider in attack.DamageColliders.Where(collider => collider))
                {
                    collider.enabled = status == "on";
                }
            }
        }
    }

}





 

		




