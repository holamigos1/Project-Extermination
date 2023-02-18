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
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
    }
}