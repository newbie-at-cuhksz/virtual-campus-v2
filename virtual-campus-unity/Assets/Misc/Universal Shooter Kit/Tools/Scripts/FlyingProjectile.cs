// GercStudio
// © 2018-2020
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
    public class FlyingProjectile : MonoBehaviour
    {
        [HideInInspector][Range(1, 50)] public float Speed = 10;

        [HideInInspector] public Controller Owner;

        [HideInInspector] public Transform explosion;
        [HideInInspector] public Transform Camera;
        [HideInInspector] public Transform OriginalCameraPosition;

        [HideInInspector] public Vector3 TargetPoint;
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 m_CurrentVelocity = new Vector3(1.0f, 0.0f, 0.0f);
        
        [HideInInspector] public int ownerID;
        [HideInInspector] public int damage = 10;
        [HideInInspector] public float GrenadeExplosionTime;
        
        [HideInInspector] public string ownerName;

        [HideInInspector] public bool ExplodeWhenTouchGround;
        [HideInInspector] public bool isRaycast;
        [HideInInspector] public bool isEnemy;
        [HideInInspector] public bool isTracer;
        [HideInInspector] public bool isRocket;
        [HideInInspector] public bool isGrenade;
        [HideInInspector] public bool stickOnObject;
        [HideInInspector] public bool ApplyForce;
        [HideInInspector] public bool FlashExplosion;
        [HideInInspector] public bool isMultiplayerWeapon;
        

        [HideInInspector] public Texture WeaponImage;
        
        [HideInInspector] public ParticleSystem[] Particles = {null};
        

        private float timeout;
        private float lifetimeout;
        private float _scatter;
        
        
        private bool isTopDown;
        private bool touchGround;
        private bool reachedPoint;

        private Rigidbody _rigidbody;

        void Start()
        {
            if (isGrenade)
                _rigidbody = GetComponent<Rigidbody>();
            
            if (!isRocket) return;

            if (Camera)
            {
                OriginalCameraPosition = new GameObject("OriginalCameraPosition").transform;
                OriginalCameraPosition.hideFlags = HideFlags.HideInHierarchy;
                OriginalCameraPosition.position = Camera.position;
                OriginalCameraPosition.rotation = Quaternion.Euler(Camera.eulerAngles.x, Camera.eulerAngles.y, Camera.eulerAngles.z);
            }
        }

        private void Update()
        {
            timeout += Time.deltaTime;
        }

        void FixedUpdate()
        {
            if (isRocket)
            {
                lifetimeout += Time.deltaTime;

                transform.Rotate(Vector3.forward, 10, Space.Self);
                
                if (!isEnemy)
                {
                    if (Particles.Length > 0)
                    {
                        foreach (var particle in Particles)
                        {
                            if (particle)
                            {
                                var effect = Instantiate(particle, transform.position, transform.rotation);
                            
                                var particleEmission = effect.GetComponent<ParticleSystem>().emission;
                                particleEmission.enabled = true;
                                var particleMain = effect.GetComponent<ParticleSystem>().main;
                                particleMain.loop = false;
                                effect.gameObject.hideFlags = HideFlags.HideInHierarchy;
                            }
                        }
                    }
                    
                    if (isRaycast)
                    {
                        transform.LookAt(TargetPoint);
                        transform.position = Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);

                        if (Helper.ReachedPositionAndRotation(transform.position, TargetPoint))
                        { 
                            Explosion();
                            return;
                        }
                    }
                    else
                    {
                        transform.Translate(Vector3.forward * Speed * Time.deltaTime, OriginalCameraPosition);
                    }
                }
                else
                {
                    if (!reachedPoint)
                    {
                        transform.LookAt(TargetPoint);
                        transform.position = Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);
                        if (Helper.ReachedPositionAndRotation(transform.position, TargetPoint))
                        {
                            reachedPoint = true;
                        }
                    }
                    else
                    {
                        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
                    }
                }
                
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 100))
                {
                    if (hit.transform.root.gameObject.GetInstanceID() != ownerID && hit.transform.root.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    {
                        if (Vector3.Distance(transform.position, hit.point) <= 1)
                        {
                            if (hit.transform.gameObject.layer == 8)
                            {
                                if (hit.transform.GetComponent<Controller>())
                                {
                                    if (Owner && hit.transform.GetComponent<Controller>().CharacterName != Owner.CharacterName || !Owner)
                                        Explosion();
                                }

                            }
                            else
                            {
                                Explosion();
                            }
                        }
                    }
                }
                    
                if (lifetimeout > 10)
                    Explosion();

            }
            else if(isTracer)
            {
                //transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                transform.LookAt(TargetPoint);
                transform.position = Vector3.MoveTowards(transform.position, TargetPoint, Speed * Time.deltaTime);
                var trail = GetComponent<TrailRenderer>();
                
                trail.startColor = new Color(1, 0.5f, 0, Mathf.Lerp(trail.startColor.a, 0, 2 * Time.deltaTime));
                trail.endColor = new Color(1, 0.7f, 0f, Mathf.Lerp(trail.endColor.a, 0, 2 * Time.deltaTime));
                
//                if (transform.position == TargetPoint)
//                {
//                    transform.position = TargetPoint;
//                    Destroy(gameObject, 2);
//                }
            }
            else if (isGrenade)
            {
                var mask = ~ LayerMask.GetMask("Grass");
                var objects = Physics.OverlapSphere(transform.position, 0.1f, mask);
                
                
                if (objects.Length > 0 && objects.ToList().Any(obj =>
                {
                    Transform root;
                    return (root = obj.transform.root).gameObject.GetInstanceID() != ownerID && root.gameObject.GetInstanceID() != gameObject.GetInstanceID() && !root.CompareTag("Smoke") &&
                           (timeout > 1 && root.gameObject.GetInstanceID() == Owner.gameObject.GetInstanceID() || root.gameObject.GetInstanceID() != Owner.gameObject.GetInstanceID());
                }))
                {
                    touchGround = true;
                    
                    if (!stickOnObject)
                    {
                        _rigidbody.useGravity = true;
                        _rigidbody.isKinematic = false;
                    }
                }
                else
                {
                    if (!touchGround)
                    {
                        var position = transform.position;
                        ProjectileHelper.UpdateProjectile(ref position, ref m_CurrentVelocity, Physics.gravity.y, Time.deltaTime);
                        transform.position = position;
                    }
                }
            }
        }
        

        public IEnumerator GrenadeFlying()
        {
            yield return new WaitForSeconds(GrenadeExplosionTime);
            Explosion();
            StopCoroutine("GrenadeFlying");
        }

        public void Explosion()
        {
            if (explosion)
            {
                var _explosion = Instantiate(explosion, transform.position, transform.rotation);

                if (!isMultiplayerWeapon)
                {
                    var script = _explosion.gameObject.AddComponent<Explosion>();
                    script.startPosition = startPosition;
                    script.damage = damage;
                    script.owner = Owner;
                    script.ownerName = ownerName;
                    script.applyForce = ApplyForce;
                    if (WeaponImage) script.weaponImage = WeaponImage;
                    script.instanceId = gameObject.GetInstanceID();
                }

                StopCoroutine("GrenadeFlying");

                if (isRocket)
                {
                    if (!isRaycast && !isEnemy)
                        Destroy(OriginalCameraPosition.gameObject);
                }
            }
            else
            {
                if(!FlashExplosion)
                    Debug.Log("(Weapon Controller) <color=yellow>Missing component</color>: [Explosion].", gameObject);
            }

            if (FlashExplosion)
            {
                var findArea = Physics.OverlapSphere(transform.position, 100);
                
                foreach (var obj in findArea)
                {
                    if (obj.gameObject.GetComponent<InventoryManager>())
                    {
                        RaycastHit hit;
                        if (!Physics.Linecast(transform.position + Vector3.up, obj.gameObject.GetComponent<Controller>().BodyObjects.Head.position, out hit, Helper.layerMask()))
                        {
                            DisableFlashEffect(obj);
                        }
                    }

                    if (obj.transform.root.gameObject.GetComponent<EnemyController>())
                    {
                        var script = obj.transform.root.gameObject.GetComponent<EnemyController>();
                        RaycastHit hit;
                        var layerMask = ~ (LayerMask.GetMask("Enemy") | LayerMask.GetMask("Grass"));
                        if (!Physics.Linecast(transform.position + Vector3.up, script.DirectionObject.position, out hit, layerMask))
                        {
                            script.InSmoke = true;

                            if (!script.EnemyAttack.FlashEffectTimeout)
                            {
                                script.EnemyAttack.FlashEffectTimeout = true;
                                script.EnemyAttack.StartCoroutine(script.EnemyAttack.FlashGrenadeEffect());
                            }
                        }
                    }
                }
            }
                
            Destroy(gameObject);
        }

        void Flash(Collider obj)
        {
            if(obj.gameObject.GetComponent<Controller>().isMultiplayerCharacter)
                return;
            
            var controller = obj.gameObject.GetComponent<Controller>();
           
            if(controller.UIManager.CharacterUI.flashPlaceholder)
                controller.UIManager.CharacterUI.flashPlaceholder.color = new Color(1, 1, 1, 1);
            
            controller.UIManager.CharacterUI.flashPlaceholder.gameObject.SetActive(true);

            if (!controller.thisCamera.GetComponent<Motion>())
            {
                controller.thisCamera.AddComponent<Motion>();
            }
        }

        void DisableFlashEffect(Collider obj)
        {
            if(obj.gameObject.GetComponent<Controller>().isMultiplayerCharacter)
                return;
            
            var manager = obj.gameObject.GetComponent<InventoryManager>();
            manager.flashTimeout = 0;

            if (obj.gameObject.GetComponent<Controller>().thisCamera.GetComponent<Motion>())
            {
                var motion = obj.gameObject.GetComponent<Controller>().thisCamera.GetComponent<Motion>();
                motion.shutterAngle = 360;
                motion.sampleCount = 10;
                motion.frameBlending = 1;
            }
            
            Flash(obj);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (isTracer || isRocket) return;
            
            if (other.collider.GetComponent<Surface>() && ExplodeWhenTouchGround)// && !other.collider.CompareTag("Smoke"))
            {
                Explosion();
            }
        }
    }
}




