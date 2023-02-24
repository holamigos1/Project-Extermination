using System;
using Data.AnimationTags;
using Data.Tags;
using Objects.Base;
using UnityEngine;

namespace Weapons.Basic
{
    public abstract class Weapon : Item, IEquip
    {
        public bool IsEquipped => _isEquipped;
        public bool IsInHand => _transform.parent != null && _transform.parent.CompareTag(GameTags.HAND_TAG);
        
        private bool _isEquipped;
        
        private void Start()
        {
            if (IsInHand) Equip();
        }
        
        public void Equip()
        {
            Init();
            _gameObject.SetActive(true);
            _animator.enabled = true;
            _animator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, _isEquipped = true);
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
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
            if(!IsEquipped) return;
        }
        
        public void PlayAimAction()
        {
            if(!IsEquipped) return;
        }
        
        ////////////////////////////
    }
}