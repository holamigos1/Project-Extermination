using System;
using GameData.AnimationTags;
using GameData.Tags;
using GameSystems.Base;
using Objects.Base;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : Item, IEquip
    {
        public bool IsEquipped => _isEquipped;
        public bool IsInHand => (ItemTransform.parent != null) && 
                                (ItemTransform.parent.CompareTag(GameTags.HAND_TAG));
        
        private bool _isEquipped;
        
        private void Start()
        {
            if (IsInHand) Equip();
        }
        
        public void Equip()
        {
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
            bool boolValue = bool.Parse(boolString); 
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_READY, boolValue);
        }

        public virtual void PlayFireAction()
        {
            if(ItemAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.IDLE) == false) return;
            if(!IsEquipped) return;
        }
        
        public void PlayAimAction()
        {
            if(!IsEquipped) return;
        }
        
        ////////////////////////////

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Bounds boudns = transform.RenderBounds();
            Gizmos.DrawWireCube(boudns.center, boudns.size);
        }
    }
}