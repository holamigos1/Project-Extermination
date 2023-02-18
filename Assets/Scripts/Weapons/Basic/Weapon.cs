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
            _animator.StopPlayback();
            _animator.enabled = false;
            if (transform.parent != null && transform.parent.CompareTag(Data.Tags.GameTags.HAND_TAG))
            {
                Equip();
            }
        }
        
        public void Equip()
        {
            _thisGameObject.SetActive(true);
            _isEquip = true;
            _animator.enabled = true;
            _animator.SetBool(Data.AnimationTags.AnimationTags.IS_ITEM_EQUIPED, true);
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
        
        //////// Animations ////////

        public void SetReady(string value)
        {
            Debug.Log(value);
            if(value == "true") _animator.SetBool("Is Ready",true);
            if(value == "false") _animator.SetBool("Is Ready",false);
        }
        
        ////////////////////////////
    }
}