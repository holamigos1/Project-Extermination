using System;
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
        public GameObject thisObject => _itemGameObject;
        public PickUpType PickUpType => _pickUpType;
        public bool IsPickuped => _isItemPickuped;
        public GameSystemsContainer SystemsContainer => _itemSystemsContainer;
        public Animator ItemAnimator => _itemAnimator;
        public Rigidbody ItemRigidbody => _itemRigidbody;  
        public GameObject ItemGameObject => _itemGameObject;
        public Transform ItemTransform => _itemTransform;
        
        
        [SerializeField] 
        private PickUpType _pickUpType;
        
        [SerializeField]
        private MeshFilter _meshFilter;
        
        private bool _isItemPickuped;
        private GameSystemsContainer _itemSystemsContainer;
        private Animator _itemAnimator;
        private Rigidbody _itemRigidbody;  
        private GameObject _itemGameObject;
        private Transform _itemTransform;
        
        protected virtual void OnEnable() =>
            Init();
        
        public void Init()
        {
            Debug.Log(_itemAnimator);
            _itemAnimator ??= GetComponent<Animator>();
            _itemRigidbody ??= GetComponent<Rigidbody>();
            _itemGameObject ??= gameObject;
            _itemTransform ??= transform;
        }
        
        public Item Pickup()
        {
            _itemGameObject.SetActive(false);
            
            _itemRigidbody.isKinematic = true;
            _itemRigidbody.useGravity = false;
            
            _isItemPickuped = true;
            
            return this;
        }

        public void Drop()
        {
            _isItemPickuped = false;

            _itemRigidbody.isKinematic = false;
            _itemRigidbody.useGravity = true;
            
            _itemAnimator.StopPlayback();
            _itemAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
            _itemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);

            _itemGameObject.ChangeGameObjsLayers(GameLayers.DEFAULT_LAYER);
        }
        
        protected virtual void OnDestroy()
        {
            _itemAnimator = null;
            _itemRigidbody = null;  
            _itemGameObject = null;
            _itemTransform = null;
        }
    }
}