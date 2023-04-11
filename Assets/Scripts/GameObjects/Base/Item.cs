using GameSystems.Base;
using UnityEngine;

namespace GameObjects.Base
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public abstract class Item : MonoBehaviour, IPickup
    {
        public bool IsPickuped => _isItemPickuped;
        public GameSystemsContainer SystemsContainer => _itemSystemsContainer;
        public Animator ItemAnimator
        {
            get
            {
                if(_itemAnimator == null)
                    _itemAnimator = GetComponent<Animator>();
                return _itemAnimator;
            }
        }
        public Rigidbody ItemRigidbody
        {
            get
            {
                if(_itemRigidbody == null)
                    _itemRigidbody = GetComponent<Rigidbody>();
                return _itemRigidbody;
            }
        }
        public GameObject ItemGameObject
        {
            get
            {
                if(_itemGameObject == null)
                    _itemGameObject = gameObject;
                return _itemGameObject;
            }
        }
        public Transform ItemTransform { 
            get
            {
                if(_itemTransform == null)
                    _itemTransform = transform;
                return _itemTransform;
            }
        }
        

        private bool _isItemPickuped;
        private GameSystemsContainer _itemSystemsContainer;
        private Animator _itemAnimator;
        private Rigidbody _itemRigidbody;  
        private GameObject _itemGameObject;
        private Transform _itemTransform;
        
        
        public Item Pickup()
        {
            ItemGameObject.SetActive(false);
            
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;
            
            _isItemPickuped = true;
            
            return this;
        }
    }
}