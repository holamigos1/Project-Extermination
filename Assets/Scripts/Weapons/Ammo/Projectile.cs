using System;
using System.Collections;
using Objects.Base;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Weapons.Range.Base;

namespace Weapons.Ammo
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public abstract class Projectile : MonoBehaviour
    {
        public event Action<Projectile ,Collision> ProjectileHit;
        public float Damage => _damage;
        public Transform ProjectileTransform { 
            get
            {
                if(_projectileTransform == null)
                    _projectileTransform = transform;
                return _projectileTransform;
            }
        }
        public Rigidbody ProjectileRigidbody
        {
            get
            {
                if(_projectileRigidbody == null)
                    _projectileRigidbody = GetComponent<Rigidbody>();
                return _projectileRigidbody;
            }
        }
        
        
        [SerializeField] 
        private DecalProjector _decalProjector;
        
        private const float DESTROY_DECAL_DELAY = 4f;
        
        private Rigidbody _projectileRigidbody;
        private Transform _projectileTransform;
        private float _damage;

        public void Init(float damage) =>
            _damage = damage;
        
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.GetMaterialType(out MaterialType materialType))
            {
                var decalSprite = BulletDecalsContainer.GetBulletHoleSprite(materialType);
                SpawnDecal(ProjectileTransform.position - ProjectileTransform.forward/2, ProjectileTransform.rotation, decalSprite.texture);
            }
            
            ProjectileHit?.Invoke(this, collision);
            Destroy(gameObject);
        }
        
        private void SpawnDecal(Vector3 position, Quaternion rotation, Texture decalTexture)
        {
            DecalProjector inst = Instantiate(_decalProjector, position, rotation);

            var d = new Material(inst.material);

            //inst.material.
            inst.StartCoroutine(DestroyDecal(inst.gameObject, DESTROY_DECAL_DELAY));
        }

        private IEnumerator DestroyDecal(GameObject decalInst, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(decalInst);
        }
    }
}