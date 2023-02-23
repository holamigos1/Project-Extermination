using System;
using Data.AnimationTags;
using Data.Tags;
using Objects.Base;
using UnityEngine;

namespace Weapons.Basic
{
    
    public abstract class Weapon : Item, IEquip
    {
        public bool IsEquip => _isEquip;
        
        private bool _isEquip;
        
        private void Start()
        {
            //_animator.enabled = false;
            if (_transform.parent != null && _transform.parent.CompareTag(GameTags.HAND_TAG))
            {
                Equip();
            }
        }
        
        public void Equip()
        {
            Init();
            _gameObject.SetActive(true);
            _animator.enabled = true;
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            _animator.SetBool(AnimationParams.IS_ITEM_EQUIPED, _isEquip = true);
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
        
        //////// Animations ////////

        public void SetReady(string boolString)
        {
            bool boolValue = Boolean.Parse(boolString); 
            _animator.SetBool(AnimationParams.IS_ITEM_READY, boolValue);
        }

        public virtual void PlayFireAction()
        {
            if(!IsEquip) return;
        }
        
        public void PlayAimAction()
        {
            if(!IsEquip) return;
        }
        
        ////////////////////////////
    }
}