using System;
using System.Collections;
using GameExtensions;
using Objects.Base;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
            // / 2 чтобы декаль жоско не размазывало когда стреляешь под углом
            Vector3 decalPosition = ProjectileTransform.position - ProjectileTransform.forward / 2;
            
            if (collision.GetMaterialType(out MaterialType materialType))
                SpawnDecal(decalPosition, 
                    ProjectileTransform.rotation, 
                    collision.transform, 
                    BulletDecalsContainer.GetBulletHoleSprite(materialType));
            
            if (collision.gameObject.TryGetComponent(out TerrainCollider terrainCollider))
                SpawnDecal(decalPosition,
                    ProjectileTransform.rotation, 
                    collision.transform,
                    BulletDecalsContainer.GetBulletHoleSprite(MaterialType.Defualt));
                        
            ProjectileHit?.Invoke(this, collision);
            Destroy(gameObject);
            //dfsadsadsdfgdfgfsdsadSDSADSD
        }
        
        private void SpawnDecal(Vector3 position, Quaternion rotation, Transform parent, Sprite decalSprite) //TODO Вынести логику спавна декалей отсюда
        {
            DecalProjector decalInst = Instantiate(_decalProjector, position, rotation);
            Texture2D croppedTexture = new Texture2D( (int)decalSprite.rect.width, (int)decalSprite.rect.height);
            Color[] pixels = decalSprite.texture.GetPixels((int)decalSprite.textureRect.x, 
                                                            (int)decalSprite.textureRect.y, 
                                                            (int)decalSprite.textureRect.width, 
                                                            (int)decalSprite.textureRect.height );
            
            croppedTexture.SetPixels( pixels );
            croppedTexture.Apply();

            var newDecalMat = new Material(decalInst.material);
            newDecalMat.SetTexture("Base_Map", croppedTexture);
            decalInst.material = newDecalMat;
            decalInst.transform.parent = parent;
            decalInst.StartCoroutine(DestroyDecal(decalInst.gameObject, DESTROY_DECAL_DELAY));
        }

        private IEnumerator DestroyDecal(GameObject decalInst, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(decalInst);
        }
    }
}