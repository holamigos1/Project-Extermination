using Data.AnimationTags;
using Data.Layers;
using Systems.Base;
using UnityEngine;

namespace Objects.Base
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public abstract class Item : MonoBehaviour, IDrop, IPickup
    {
        public GameObject thisObject => _gameObject;
        public PickUpType PickUpType => _pickUpType;
        public bool IsPickuped => _isPickuped;
        public GameSystemsContainer SystemsContainer => _systemsContainer;
        
        [SerializeField] 
        private PickUpType _pickUpType;
        
        protected bool _isPickuped;
        protected GameSystemsContainer _systemsContainer;
        protected Animator _animator;
        protected Rigidbody _rigidbody;
        protected GameObject _gameObject;
        protected Transform _transform;
        
        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            if(_animator == null) _animator = GetComponent<Animator>();
            if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if(_gameObject == null) _gameObject = gameObject;
            if(_transform == null) _transform = transform;
        }
        
        public GameObject Pickup()
        {
            _gameObject.SetActive(false);
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            _isPickuped = true;
            
            return _gameObject;
        }

        public void Drop()
        {
            _isPickuped = false;
            _animator.StopPlayback();
            _animator.enabled = false;
            _animator.cullingMode = AnimatorCullingMode.CullCompletely;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _animator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);
            _gameObject.ChangeFamilyLayers(LayerMask.NameToLayer(GameLayers.DEFAULT_LAYER));
        }
    }
}