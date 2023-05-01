using System;
using System.Collections.Generic;
using GameSystems.Base;
using Movement.SourseMovment;
using Movement.SourseMovment.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Systems
{
    [Serializable]
    public class CharacterMovement : GameSystem, ISurfControllable
    {
        [Title("Скрипт передвижения, спизженный с какого-то сайта.", 
            "Тут дохрена настроек и хуй знает чё за чё отвечает. Потыкай, узнаешь.")]
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219, CS0414
        private string info = "";
        
        public enum ColliderType
        {
            Capsule,
            Box
        }
        
        public MoveType MoveType => MoveType.Walk;
        public MoveData MoveData { get; } = new();

        public Collider SurfCollider { get; private set; }

        public GameObject GroundObject { get; set; }

        public Vector3 BaseVelocity { get; }

        public Vector3 Forward => _cameraTransform.forward;
        public Vector3 Right => _cameraTransform.right;
        public Vector3 Up => _cameraTransform.up;

        [Header("Physics Settings")] 
        [SerializeField] private Vector3 _colliderSize = new(1f, 2f, 1f);
        [SerializeField] private Vector3 _colliderCenter = new(0, 1f, 0);
        [SerializeField] private float _weight = 75f;
        [SerializeField] private float _rigidbodyPushForce = 2f;
        [SerializeField] private bool _solidCollider;
        [Header("View Settings")] 
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _rootTransform;
        [Header("Crouching setup")] 
        [SerializeField] private float _crouchingHeightMultiplier = 0.5f;
        [SerializeField] private float _crouchingSpeed = 10f;
        [Header("Features")] 
        [SerializeField] private bool _crouchingEnabled = true;
        [SerializeField] private bool _slidingEnabled;
        [SerializeField] private bool _laddersEnabled = true;
        [SerializeField] private bool _supportAngledLadders = true;
        [Header("Step offset (can be buggy, enable at your own risk)")]
        [SerializeField] private bool _useStepOffset;
        [SerializeField] private float _stepOffset = 0.35f;
        [Header("Movement Config")]
        [SerializeField] private MovementConfig _movementConfig;
        
        private Vector3 _angels;
        private CameraWaterCheck _cameraWaterCheck;
        private GameObject _cameraWaterCheckObject;
        private GameObject _colliderObject;
        private readonly SurfController _controller = new();
        private Vector3 _startPosition;
        private bool _allowCrouch = true; // This is separate because you shouldn't be able to toggle crouching on and off during gameplay for various reasons
        private float _defaultHeight;
        private int _numberOfTriggers;
        private Vector3 _prevPosition;
        private Rigidbody _rb;
        private readonly List<Collider> _triggers = new();
        private bool _underwater;
        
        public ColliderType collisionType =>
            ColliderType.Box; // Capsule doesn't work anymore; I'll have to figure out why some other time, sorry.

        public MovementConfig moveConfig => _movementConfig;
        
        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "OnCollisionStay" when data != null:
                    OnCollisionStay(data as Collision);
                    return;
            }
        }

        public override void Start()
        {
            SystemsСontainer.Update += Update;
            SystemsСontainer.Notify += OnNotify;
            
            _controller.playerTransform = _rootTransform;

            if (_cameraTransform != null)
            {
                _controller.camera = _cameraTransform;
                _controller.cameraYPos = _cameraTransform.localPosition.y;
            }

            _colliderObject = new GameObject("PlayerCollider");
            _colliderObject.layer = SystemsСontainer.SystemsOwner.gameObject.layer;
            _colliderObject.transform.SetParent(_rootTransform);
            _colliderObject.transform.rotation = Quaternion.identity;
            _colliderObject.transform.localPosition = Vector3.zero;
            _colliderObject.transform.SetSiblingIndex(0);

            // Water check
            _cameraWaterCheckObject = new GameObject("Camera water check");
            _cameraWaterCheckObject.layer = SystemsСontainer.SystemsOwner.gameObject.layer;
            _cameraWaterCheckObject.transform.position = _cameraTransform.position;

            SphereCollider _cameraWaterCheckSphere = _cameraWaterCheckObject.AddComponent<SphereCollider>();
            _cameraWaterCheckSphere.radius = 0.1f;
            _cameraWaterCheckSphere.isTrigger = true;

            Rigidbody _cameraWaterCheckRb = _cameraWaterCheckObject.AddComponent<Rigidbody>();
            _cameraWaterCheckRb.useGravity = false;
            _cameraWaterCheckRb.isKinematic = true;

            _cameraWaterCheck = _cameraWaterCheckObject.AddComponent<CameraWaterCheck>();

            _prevPosition = _rootTransform.position;

            if (_cameraTransform == null)
                _cameraTransform = Camera.main.transform;

            if (_rootTransform == null && _rootTransform.childCount > 0)
                _rootTransform = _rootTransform.GetChild(0); //TODO ??????????????????????

            SurfCollider = SystemsСontainer.SystemsOwner.GetComponent<Collider>();

            if (SurfCollider != null)
                UnityEngine.Object.Destroy(SurfCollider);

            // rigidbody is required to collide with triggers
            _rb = SystemsСontainer.SystemsOwner.GetComponent<Rigidbody>();
            
            if (_rb == null)
                _rb = SystemsСontainer.SystemsOwner.gameObject.AddComponent<Rigidbody>();

            if (_rb != null) MoveData.velocity = _rb.velocity;
            _allowCrouch = _crouchingEnabled;

            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.angularDrag = 0f;
            _rb.drag = 0f;
            _rb.mass = _weight;
            
            switch (collisionType)
            {
                // Box collider
                case ColliderType.Box:

                    SurfCollider = _colliderObject.AddComponent<BoxCollider>();

                    var boxc = (BoxCollider)SurfCollider;
                    boxc.size = _colliderSize;
                    boxc.center = _colliderCenter;
                    _defaultHeight = boxc.size.y;

                    break;

                // Capsule collider
                case ColliderType.Capsule:

                    SurfCollider = _colliderObject.AddComponent<CapsuleCollider>();

                    var capc = (CapsuleCollider)SurfCollider;
                    capc.height = _colliderSize.y;
                    capc.radius = _colliderSize.x / 2f;
                    capc.center = _colliderCenter;
                    _defaultHeight = capc.height;

                    break;
            }

            MoveData.slopeLimit = _movementConfig.SlopeLimit;

            MoveData.rigidbodyPushForce = _rigidbodyPushForce;

            MoveData.slidingEnabled = _slidingEnabled;
            MoveData.laddersEnabled = _laddersEnabled;
            MoveData.angledLaddersEnabled = _supportAngledLadders;

            MoveData.playerTransform = _rootTransform;
            MoveData.viewTransform = _cameraTransform;
            MoveData.viewTransformDefaultLocalPos = _cameraTransform.localPosition;

            MoveData.defaultHeight = _defaultHeight;
            MoveData.crouchingHeight = _crouchingHeightMultiplier;
            MoveData.crouchingSpeed = _crouchingSpeed;

            SurfCollider.isTrigger = !_solidCollider;
            MoveData.origin = _rootTransform.position;
            _startPosition = _rootTransform.position;

            MoveData.useStepOffset = _useStepOffset;
            MoveData.stepOffset = _stepOffset;
        }

        public override void Update()
        {
            _colliderObject.transform.rotation = Quaternion.identity;

            //UpdateTestBinds ();
            UpdateMoveData();

            // Previous movement code
            var positionalMovement = _rootTransform.position - _prevPosition;
            _rootTransform.position = _prevPosition;
            MoveData.origin += positionalMovement;

            // Triggers
            if (_numberOfTriggers != _triggers.Count)
            {
                _numberOfTriggers = _triggers.Count;

                _underwater = false;
                _triggers.RemoveAll(item => item == null);
                
                foreach (var trigger in _triggers)
                {
                    if (trigger == null)
                        continue;

                    if (trigger.GetComponentInParent<Water>())
                        _underwater = true;
                }
            }

            MoveData.cameraUnderwater = _cameraWaterCheck.IsUnderwater();
            _cameraWaterCheckObject.transform.position = _cameraTransform.position;
            MoveData.underwater = _underwater;

            if (_allowCrouch) _controller.Crouch(this, _movementConfig, Time.deltaTime);

            _controller.ProcessMovement(this, _movementConfig, Time.deltaTime);

            _rootTransform.position = MoveData.origin;
            _prevPosition = _rootTransform.position;

            _colliderObject.transform.rotation = Quaternion.identity;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.rigidbody == null)
                return;

            Vector3 relativeVelocity = collision.relativeVelocity * collision.rigidbody.mass / 50f;
            var impactVelocity = new Vector3(relativeVelocity.x * 0.0025f, relativeVelocity.y * 0.00025f,
                relativeVelocity.z * 0.0025f);

            float maxYVel = Mathf.Max(MoveData.velocity.y, 10f);
            var newVelocity = new Vector3(MoveData.velocity.x + impactVelocity.x,
                Mathf.Clamp(MoveData.velocity.y + Mathf.Clamp(impactVelocity.y, -0.5f, 0.5f), -maxYVel, maxYVel),
                MoveData.velocity.z + impactVelocity.z);

            newVelocity = Vector3.ClampMagnitude(newVelocity, Mathf.Max(MoveData.velocity.magnitude, 30f));
            MoveData.velocity = newVelocity;
        }

        ///// Methods /////

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_rootTransform.position, _colliderSize);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_triggers.Contains(other))
                _triggers.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_triggers.Contains(other))
                _triggers.Remove(other);
        }
        

        private void UpdateTestBinds()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                ResetPosition();
        }

        private void ResetPosition()
        {
            MoveData.velocity = Vector3.zero;
            MoveData.origin = _startPosition;
        }

        private void UpdateMoveData()
        {
            MoveData.verticalAxis = Input.GetAxisRaw("Vertical");
            MoveData.horizontalAxis = Input.GetAxisRaw("Horizontal");
            MoveData.sprinting = Input.GetButton("Sprint");

            if (Input.GetButtonDown("Crouch")) MoveData.crouching = true;
            
            MoveData.crouching = true;

            if (!Input.GetButton("Crouch")) MoveData.crouching = false;

            var moveLeft = MoveData.horizontalAxis < 0f;
            var moveRight = MoveData.horizontalAxis > 0f;
            var moveFwd = MoveData.verticalAxis > 0f;
            var moveBack = MoveData.verticalAxis < 0f;
            var jump = Input.GetButton("Jump");

            if (!moveLeft && !moveRight) MoveData.sideMove = 0f;
            else if (moveLeft) MoveData.sideMove = -moveConfig.acceleration;
            else if (moveRight) MoveData.sideMove = moveConfig.acceleration;

            if (!moveFwd && !moveBack) MoveData.forwardMove = 0f;
            else if (moveFwd) MoveData.forwardMove = moveConfig.acceleration;
            else if (moveBack) MoveData.forwardMove = -moveConfig.acceleration;

            if (Input.GetButtonDown("Jump")) MoveData.wishJump = true;
            if (!Input.GetButton("Jump")) MoveData.wishJump = false;
            
            MoveData.viewAngles = _angels;
        }

        private void DisableInput()
        {
            MoveData.verticalAxis = 0f;
            MoveData.horizontalAxis = 0f;
            MoveData.sideMove = 0f;
            MoveData.forwardMove = 0f;
            MoveData.wishJump = false;
        }
    }
}