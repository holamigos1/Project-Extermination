using Systems.Base;
using UnityEngine;

namespace Objects.Base
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public abstract class Item : MonoBehaviour, IDrop, IPickup
    {
        public GameObject thisObject => _thisGameObject;
        public PickUpType PickUpType => _pickUpType;
        public bool IsPickuped => _isPickuped;
        public GameSystemsContainer SystemsContainer => _systemsContainer;
        
        [SerializeField] 
        private PickUpType _pickUpType;
        
        protected bool _isPickuped;
        protected GameSystemsContainer _systemsContainer;
        protected Animator _animator;
        protected Rigidbody _rigidbody;
        protected GameObject _thisGameObject;
        
        private void OnEnable()
        {
            if(_animator == null) _animator = GetComponent<Animator>();
            if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if(_thisGameObject == null) _thisGameObject = gameObject;
        }
        
        public GameObject Pickup()
        {
            _thisGameObject.SetActive(false);
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _isPickuped = true;
            return _thisGameObject;
        }

        public void Drop()
        {
            _isPickuped = false;
            _animator.StopPlayback();
            _animator.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _thisGameObject.ChangeFamilyLayers(LayerMask.NameToLayer(Data.Layers.GameLayers.DEFAULT_LAYER));
        }
    }
}