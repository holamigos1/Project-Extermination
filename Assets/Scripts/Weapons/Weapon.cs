// Designed by Kinemation and Sanya2286661337, 2023

using System.Collections.Generic;
using System.Collections;
using GameAnimation.Sheets;
using GameData.AnimationTags;
using GameData.Layers;
using GameData.Tags;
using GameItems.Base;
using Misc.Extensions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using Weapons.Basic;
using Weapons.DamageTypes;
using Weapons.Data;
using Weapons.Range.Base;
using Weapons.SoundsSheet;

namespace Weapons
{
    public class Weapon : GameItem
    {
        public bool IsEquipped { get; protected set; }
        public bool IsInHand => (ItemTransform.parent != null) && 
                                (ItemTransform.parent.CompareTag(GameTags.HAND_TAG));

        public bool IsReady => ItemAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.IDLE);
        public float Damage => _damage;

        [Header("Я почищу тут всё потом.. пока что тут дохуища непонятных настроек, \nпосмтори как сделано в других пушках и копипасть")]
        [SerializeField] 
        private float _damage = 10f;

        [SerializeField] private DecalProjector _bulletholeDecalProjector;
        [SerializeField] private LayerMask _deathRayBlockingLayers;
        [SerializeField] private RifleSoundSheet _rifleSoundSheet;
        [SerializeField] private AudioSource _weaponAudioSource;
        [SerializeField] private RifleAnimatorSheet _rifleAnimatorSheet;
        [SerializeField] private VisualEffect _muzzleFlashVFXComponent;
        [Tooltip("Список Transform позиций к которым можно прицелиться.")]
        [SerializeField] private List<Transform> _scopes;
        
        public WeaponAnimData gunData; //TODO А если это катана или пульт наведения лазненым спутником?
        public RecoilAnimData recoilData; //TODO Убери в скрипт Firearm
        public FireMode fireMode;//TODO Избавься от этого недоразумения...
        public float fireRate;
        public int burstAmount;

        private const int DESTROY_DECAL_DELAY = 30;
        private int _scopeIndex;
        private MuzzleFlashVFXAttributeContainer _VFXAttributeContainer;

        private new void Reset()
        {
            _muzzleFlashVFXComponent = GetComponent<VisualEffect>();
            _muzzleFlashVFXComponent = transform.GetComponentInChildren<VisualEffect>(true);
            _VFXAttributeContainer = new MuzzleFlashVFXAttributeContainer(_muzzleFlashVFXComponent);
        }

        protected void Start()
        {
            Debug.Log(ItemGameObject.name);
            if (IsInHand)
            {
                Equip();
            }
        }
        
        public void Equip()
        {
            ItemGameObject.SetActive(true);
            ItemAnimator.enabled = true;
            ItemAnimator.SetTrigger(AnimationParams.ITEM_EQUIP_TRIGGER); //TODO Используй AnimatorParametersSheet для имён стейтов
            ItemAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;

            StartCoroutine(OnEquipCoroutine());
        }

        private IEnumerator OnEquipCoroutine()
        {
            while (IsReady == false) //да она дохрена раз обращается к GetCurrentAnimatorStateInfo, и чо
                yield return null;
            
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, IsEquipped = true);
        }

        public virtual void PlayFireAction() { }
        
        public void PlayAimAction() { }

        private void OnDrawGizmos() //TODO Удали как будет не нужно
        {
            Gizmos.color = Color.yellow;
            Bounds boudns = transform.RenderBounds();
            Gizmos.DrawWireCube(boudns.center, boudns.size);
        }
        
        public void OnDrop()
        {
            IsEquipped = false;
            Owner = null;
            
            ItemRigidbody.isKinematic = false;
            ItemRigidbody.useGravity = true;
            
            ItemAnimator.StopPlayback();
            ItemAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);

            ItemGameObject.ChangeObjectHierarhyLayers(GameLayers.DEFAULT_LAYER);
        }

        public Transform GetScope()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > _scopes.Count - 1 ? 0 : _scopeIndex;
            return _scopes[_scopeIndex];
        }
        
        public void OnFire()
        {
            PlayFireAnim();
        }
        
        
        protected void ApplyDamage(GameObject target, Vector3 worldHitPos)
        {   
            //IF not human . .. bla bla
            Vector3 gunPivotWorldPos = gunData.gunAimData.pivotPoint.position;
            Vector3 directionToHitPoint = Vector3.Normalize(worldHitPos - gunPivotWorldPos);
            float distanceToHitPoint = Vector3.Distance(gunPivotWorldPos,worldHitPos);
            Vector3 decalWorldSpawnPoint = gunPivotWorldPos + directionToHitPoint * (distanceToHitPoint - 0.5f); //TODO magic 0.5f (UDP это оступ от точки попадания чтобы декаль в обекте не спаунилась)

            var hittable = target.GetComponent<IHittable>();
            hittable?.ApplyHit(new BulletHit(_damage, directionToHitPoint));
            
            if (target.GetMaterialType(out MaterialType materialType))
                SpawnDecal(decalWorldSpawnPoint, 
                    Quaternion.LookRotation(directionToHitPoint, ItemTransform.up), 
                    target.transform, 
                    BulletDecalsContainer.GetBulletHoleSprite(materialType));
            
            if (target.TryGetComponent(out TerrainCollider terrainCollider))
                SpawnDecal(decalWorldSpawnPoint,
                    Quaternion.LookRotation(directionToHitPoint, ItemTransform.up), 
                    target.transform,
                    BulletDecalsContainer.GetBulletHoleSprite(MaterialType.Defualt));
        }

        private void DoShotSound()
        {
            _weaponAudioSource.PlayOneShot(_rifleSoundSheet.ShootSound_1,0.75f);
        }

        private void CastDeathRay(Vector3 direction)
        {
            Ray ray = new Ray();
        }

        private void PlayFireAnim()
        {
            if (ItemAnimator == null)
                return;
            
            ItemAnimator.Play(_rifleAnimatorSheet.FireState.Hash, _rifleAnimatorSheet.DefaultLayer, 0f); //TODO Используй AnimatorParametersSheet для имён стейтов
            _muzzleFlashVFXComponent.SendEvent("OnFire"); //TODO А если какая-нибудь сука поменяет имя события?
            DoShotSound();

            Transform gunPivot = gunData.gunAimData.pivotPoint;
            var rayBlockingObj = gunPivot.GetRaycastBlockingObject(gunPivot.forward, _deathRayBlockingLayers);
            if (rayBlockingObj.collider != null && rayBlockingObj.collider.gameObject != null) 
                ApplyDamage(rayBlockingObj.collider.gameObject, rayBlockingObj.point);
        }
        
        private void SpawnDecal(Vector3 position, Quaternion rotation, Transform parent, Sprite decalSprite) //TODO Вынести логику спавна декалей отсюда
        {
            DecalProjector decalInst = Instantiate(_bulletholeDecalProjector, position, rotation);
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