// Designed by Kinemation, 2023

using System.Collections.Generic;
using System.Collections;
using GameData.AnimationTags;
using GameData.Layers;
using GameData.Tags;
using GameObjects.Base;
using Kinemation.FPSFramework.Runtime.Core;
using Misc;
using UnityEngine;

namespace Weapons
{
    public class Weapon : GameItem
    {
        public bool IsEquipped { get; protected set; }
        public bool IsInHand => (ItemTransform.parent != null) && 
                                (ItemTransform.parent.CompareTag(GameTags.HAND_TAG));

        public bool IsReady => ItemAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.IDLE);
        public float Damage => _damage;

        [SerializeField] 
        private float _damage = 10f;
        
        [Tooltip("Список Transform позиций к которым можно прицелиться.")]
        [SerializeField] private List<Transform> _scopes;
        [SerializeField] public WeaponAnimData gunData;
        [SerializeField] public RecoilAnimData recoilData;
        
        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;
        
        private int _scopeIndex;

        private void Start()
        {
            if (IsInHand) Equip();
        }
        
        public void Equip()
        {
            ItemGameObject.SetActive(true);
            ItemAnimator.enabled = true;
            ItemAnimator.SetTrigger(AnimationParams.ITEM_EQUIP_TRIGGER);
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

        private void OnDrawGizmos()
        {
            //TODO Удали как будет не нужно
            Gizmos.color = Color.yellow;
            Bounds boudns = transform.RenderBounds();
            Gizmos.DrawWireCube(boudns.center, boudns.size);
        }
        
        public void Drop()
        {
            IsEquipped = false;
            Owner = null;
            
            ItemRigidbody.isKinematic = false;
            ItemRigidbody.useGravity = true;
            
            ItemAnimator.StopPlayback();
            ItemAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);

            ItemGameObject.ChangeGameObjsLayers(GameLayers.DEFAULT_LAYER);
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

        private void PlayFireAnim()
        {
            if (ItemAnimator == null)
                return;
            
            ItemAnimator.Play("Fire", 0, 0f);
        }
    }
}