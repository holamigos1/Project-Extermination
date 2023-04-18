using UnityEngine;

namespace GameObjects.Base
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public abstract class GameItem : MonoBehaviour, IPickup
    {
        //TODO Большучее поле, украти его [UPD ПОШЁЛ НАХУЙ]
        public Unit Owner { get; protected set; }
        public bool IsPickup => _isItemPickup;
        public Animator ItemAnimator => _itemAnimator;
        public Rigidbody ItemRigidbody => _itemRigidbody;
        public GameObject ItemGameObject => _itemGameObject;
        public Transform ItemTransform => _itemTransform;
        public Transform RightHandGrip => _rightHandGrip;
        
        [SerializeField] [Tooltip("Место за которое перснаж может взяться")]
        protected Transform _rightHandGrip;

        [SerializeField, HideInInspector] private Animator _itemAnimator;
        [SerializeField, HideInInspector] private Rigidbody _itemRigidbody;  
        [SerializeField, HideInInspector] private GameObject _itemGameObject;
        [SerializeField, HideInInspector] private Transform _itemTransform;
        private bool _isItemPickup;
        
        public void SetOwner(Unit newOwner) =>
            Owner = newOwner;
        
        private void Reset()
        {
            Debug.Log("dsad");
            _itemAnimator = GetComponent<Animator>();
            _itemRigidbody = GetComponent<Rigidbody>();
            _itemGameObject = gameObject;
            _itemTransform = transform;
        }

        public GameItem Pickup()
        {
            ItemRigidbody.isKinematic = true;
            ItemRigidbody.useGravity = false;
            
            _isItemPickup = true;
            
            return this;
        }
    }
}