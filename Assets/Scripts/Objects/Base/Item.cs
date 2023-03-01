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
        public GameObject thisObject => ItemGameObject;
        public PickUpType PickUpType => _pickUpType;
        public bool IsPickuped => IsItemPickuped;
        public GameSystemsContainer SystemsContainer => ItemSystemsContainer;
        
        [SerializeField] 
        private PickUpType _pickUpType;
        
        protected bool IsItemPickuped;
        protected GameSystemsContainer ItemSystemsContainer;
        protected Animator ItemAnimator;
        protected Rigidbody ItemRigidbody;
        protected GameObject ItemGameObject;
        protected Transform ItemTransform;
        
        [SerializeField]
        private MeshFilter _meshFilter;
        
        protected virtual void OnEnable() =>
            Init();
        
        public void Init()
        {
            ItemAnimator ??= GetComponent<Animator>();
            ItemRigidbody ??= GetComponent<Rigidbody>();
            ItemGameObject ??= gameObject;
            ItemTransform ??= transform;
        }
        
        public Item Pickup()
        {
            ItemGameObject.SetActive(false);
            
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;
            
            IsItemPickuped = true;
            
            return this;
        }

        public void Drop()
        {
            IsItemPickuped = false;

            ItemRigidbody.isKinematic = false;
            ItemRigidbody.useGravity = true;
            
            ItemAnimator.StopPlayback();
            ItemAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
            ItemAnimator.SetBool(AnimationParams.IS_ITEM_EQUIPPED, false);

            ItemGameObject.ChangeFamilyLayers(LayerMask.NameToLayer(GameLayers.DEFAULT_LAYER));
        }
    }
}