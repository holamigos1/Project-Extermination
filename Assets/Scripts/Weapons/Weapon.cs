using System;
using GameData.AnimationTags;
using GameData.Tags;
using Objects.Base;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : Item, IEquip
    {
        public bool IsEquipped => _isEquipped;
        public bool IsInHand => (ItemTransform.parent != null) && (ItemTransform.parent.CompareTag(GameTags.HAND_TAG));
        
        private bool _isEquipped;
        
        private void Start()
        {
            if (IsInHand) Equip();
        }
        
        public void Equip()
        {
            Init();
            ItemGameObject.SetActive(true);
            ItemAnimator.enabled = true;
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, _isEquipped = true);
            ItemAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;
        }
        
        //////// Animations ////////

        public void SetReady(string boolString)
        {
            bool boolValue = Boolean.Parse(boolString); 
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_READY, boolValue);
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