using GameData.AnimationTags;
using GameData.Layers;
using GameSystems.Base;
using UnityEngine;

namespace Objects.Base
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public abstract class Item : MonoBehaviour, IDrop, IPickup
    {
        public Bounds RenderBounds => _meshFilter.sharedMesh.bounds;
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
        
        [SerializeField]private MeshFilter _meshFilter;
        
        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            _animator ??= GetComponent<Animator>();
            _rigidbody ??= GetComponent<Rigidbody>();
            _gameObject ??= gameObject;
            _transform ??= transform;
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

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _animator.StopPlayback();
            _animator.cullingMode = AnimatorCullingMode.CullCompletely;
            _animator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);

            _gameObject.ChangeFamilyLayers(LayerMask.NameToLayer(GameLayers.DEFAULT_LAYER));
        }
    }
}