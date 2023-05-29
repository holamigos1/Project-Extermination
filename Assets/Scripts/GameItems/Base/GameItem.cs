using System;
using UnityEngine;

namespace GameItems.Base
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public abstract class GameItem : MonoBehaviour, IPickup
    {
        //TODO Большучее поле, украти его [UPD ПОШЁЛ НАХУЙ]
        public Unit Owner { get; protected set; }
        public bool IsPickup => _isItemPickup;
        public Animator ItemAnimator => _itemBasicComponents.Animator;
        public Rigidbody ItemRigidbody => _itemBasicComponents.Rigidbody;
        public GameObject ItemGameObject => _itemBasicComponents.GameObject;
        public Transform ItemTransform => _itemBasicComponents.Transform;
        
        [SerializeField] private ItemBasicComponents _itemBasicComponents;
        private bool _isItemPickup;
        
        public void SetOwner(Unit newOwner) =>
            Owner = newOwner;
        
        public void Reset()
        {
            _itemBasicComponents.Animator = GetComponent<Animator>();
            _itemBasicComponents.Rigidbody = GetComponent<Rigidbody>();
            _itemBasicComponents.GameObject = gameObject;
            _itemBasicComponents.Transform = GetComponent<Transform>();
        }
        

        public GameItem Pickup()
        {
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;
            
            _isItemPickup = true;
            
            return this;
        }

        [Serializable]
        public struct ItemBasicComponents
        {
            public Animator Animator;
            public Rigidbody Rigidbody;
            public GameObject GameObject;
            public Transform Transform;
        }
    }
}