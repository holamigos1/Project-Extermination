using System.Collections.Generic;
using Controllers;
using Fragsurf.Movement;
using UnityEngine;

namespace OtherСomponents.SourseMovment.Movement
{
    public class CharacterMovemnet : MonoBehaviour, ISurfControllable
    {
        public enum ColliderType
        {
            Capsule,
            Box
        }

        ///// Fields /////

        [Header("Physics Settings")] public Vector3 colliderSize = new(1f, 2f, 1f);
        public float weight = 75f;
        public float rigidbodyPushForce = 2f;
        public bool solidCollider;
        [Header("View Settings")] public Transform viewTransform;
        public Transform playerRotationTransform;
        [Header("Crouching setup")] public float crouchingHeightMultiplier = 0.5f;
        public float crouchingSpeed = 10f;
        [Header("Features")] public bool crouchingEnabled = true;
        public bool slidingEnabled;
        public bool laddersEnabled = true;
        public bool supportAngledLadders = true;
        [Header("Step offset (can be buggy, enable at your own risk)")]
        public bool useStepOffset;
        public float stepOffset = 0.35f;
        [Header("Movement Config")]
        [SerializeField] public MovementConfig movementConfig;
        private Vector3 _angles;
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
        [HideInInspector]
        public ColliderType collisionType =>
            ColliderType.Box; // Capsule doesn't work anymore; I'll have to figure out why some other time, sorry.

        public MovementConfig moveConfig => movementConfig;

        private void Awake()
        {
            _controller.playerTransform = playerRotationTransform;

            if (viewTransform != null)
            {
                _controller.camera = viewTransform;
                _controller.cameraYPos = viewTransform.localPosition.y;
            }
        }

        private void Start()
        {
            _colliderObject = new GameObject("PlayerCollider");
            _colliderObject.layer = gameObject.layer;
            _colliderObject.transform.SetParent(transform);
            _colliderObject.transform.rotation = Quaternion.identity;
            _colliderObject.transform.localPosition = Vector3.zero;
            _colliderObject.transform.SetSiblingIndex(0);

            // Water check
            _cameraWaterCheckObject = new GameObject("Camera water check");
            _cameraWaterCheckObject.layer = gameObject.layer;
            _cameraWaterCheckObject.transform.position = viewTransform.position;

            SphereCollider _cameraWaterCheckSphere = _cameraWaterCheckObject.AddComponent<SphereCollider>();
            _cameraWaterCheckSphere.radius = 0.1f;
            _cameraWaterCheckSphere.isTrigger = true;

            Rigidbody _cameraWaterCheckRb = _cameraWaterCheckObject.AddComponent<Rigidbody>();
            _cameraWaterCheckRb.useGravity = false;
            _cameraWaterCheckRb.isKinematic = true;

            _cameraWaterCheck = _cameraWaterCheckObject.AddComponent<CameraWaterCheck>();

            _prevPosition = transform.position;

            if (viewTransform == null)
                viewTransform = Camera.main.transform;

            if (playerRotationTransform == null && transform.childCount > 0)
                playerRotationTransform = transform.GetChild(0);

            collider = gameObject.GetComponent<Collider>();

            if (collider != null)
                Destroy(collider);

            // rigidbody is required to collide with triggers
            _rb = gameObject.GetComponent<Rigidbody>();
            if (_rb == null)
                _rb = gameObject.AddComponent<Rigidbody>();

            _allowCrouch = crouchingEnabled;

            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.angularDrag = 0f;
            _rb.drag = 0f;
            _rb.mass = weight;


            switch (collisionType)
            {
                // Box collider
                case ColliderType.Box:

                    collider = _colliderObject.AddComponent<BoxCollider>();

                    var boxc = (BoxCollider)collider;
                    boxc.size = colliderSize;

                    _defaultHeight = boxc.size.y;

                    break;

                // Capsule collider
                case ColliderType.Capsule:

                    collider = _colliderObject.AddComponent<CapsuleCollider>();

                    var capc = (CapsuleCollider)collider;
                    capc.height = colliderSize.y;
                    capc.radius = colliderSize.x / 2f;

                    _defaultHeight = capc.height;

                    break;
            }

            moveData.slopeLimit = movementConfig.slopeLimit;

            moveData.rigidbodyPushForce = rigidbodyPushForce;

            moveData.slidingEnabled = slidingEnabled;
            moveData.laddersEnabled = laddersEnabled;
            moveData.angledLaddersEnabled = supportAngledLadders;

            moveData.playerTransform = transform;
            moveData.viewTransform = viewTransform;
            moveData.viewTransformDefaultLocalPos = viewTransform.localPosition;

            moveData.defaultHeight = _defaultHeight;
            moveData.crouchingHeight = crouchingHeightMultiplier;
            moveData.crouchingSpeed = crouchingSpeed;

            collider.isTrigger = !solidCollider;
            moveData.origin = transform.position;
            _startPosition = transform.position;

            moveData.useStepOffset = useStepOffset;
            moveData.stepOffset = stepOffset;
        }

        private void Update()
        {
            _colliderObject.transform.rotation = Quaternion.identity;

            //UpdateTestBinds ();
            UpdateMoveData();

            // Previous movement code
            var positionalMovement = transform.position - _prevPosition;
            transform.position = _prevPosition;
            moveData.origin += positionalMovement;

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

            moveData.cameraUnderwater = _cameraWaterCheck.IsUnderwater();
            _cameraWaterCheckObject.transform.position = viewTransform.position;
            moveData.underwater = _underwater;

            if (_allowCrouch) _controller.Crouch(this, movementConfig, Time.deltaTime);

            _controller.ProcessMovement(this, movementConfig, Time.deltaTime);

            transform.position = moveData.origin;
            _prevPosition = transform.position;

            _colliderObject.transform.rotation = Quaternion.identity;
        }

        private void OnEnable()
        {
            if (_rb != null) moveData.velocity = _rb.velocity;
            //prevPosition = transform.position;
            //moveData.
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.rigidbody == null)
                return;

            var relativeVelocity = collision.relativeVelocity * collision.rigidbody.mass / 50f;
            var impactVelocity = new Vector3(relativeVelocity.x * 0.0025f, relativeVelocity.y * 0.00025f,
                relativeVelocity.z * 0.0025f);

            var maxYVel = Mathf.Max(moveData.velocity.y, 10f);
            var newVelocity = new Vector3(moveData.velocity.x + impactVelocity.x,
                Mathf.Clamp(moveData.velocity.y + Mathf.Clamp(impactVelocity.y, -0.5f, 0.5f), -maxYVel, maxYVel),
                moveData.velocity.z + impactVelocity.z);

            newVelocity = Vector3.ClampMagnitude(newVelocity, Mathf.Max(moveData.velocity.magnitude, 30f));
            moveData.velocity = newVelocity;
        }

        ///// Methods /////

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, colliderSize);
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

        ///// Properties /////

        public MoveType moveType => MoveType.Walk;
        public MoveData moveData { get; } = new();

        public new Collider collider { get; private set; }

        public GameObject groundObject { get; set; }

        public Vector3 baseVelocity { get; }

        public Vector3 forward => viewTransform.forward;
        public Vector3 right => viewTransform.right;
        public Vector3 up => viewTransform.up;

        private void UpdateTestBinds()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                ResetPosition();
        }

        private void ResetPosition()
        {
            moveData.velocity = Vector3.zero;
            moveData.origin = _startPosition;
        }

        private void UpdateMoveData()
        {
            moveData.verticalAxis = Input.GetAxisRaw("Vertical");
            moveData.horizontalAxis = Input.GetAxisRaw("Horizontal");

            moveData.sprinting = Input.GetButton("Sprint");

            if (Input.GetButtonDown("Crouch")) moveData.crouching = true;
            
            moveData.crouching = true;

            if (!Input.GetButton("Crouch")) moveData.crouching = false;

            var moveLeft = moveData.horizontalAxis < 0f;
            var moveRight = moveData.horizontalAxis > 0f;
            var moveFwd = moveData.verticalAxis > 0f;
            var moveBack = moveData.verticalAxis < 0f;
            var jump = Input.GetButton("Jump");

            if (!moveLeft && !moveRight) moveData.sideMove = 0f;
            else if (moveLeft) moveData.sideMove = -moveConfig.acceleration;
            else if (moveRight) moveData.sideMove = moveConfig.acceleration;

            if (!moveFwd && !moveBack) moveData.forwardMove = 0f;
            else if (moveFwd) moveData.forwardMove = moveConfig.acceleration;
            else if (moveBack) moveData.forwardMove = -moveConfig.acceleration;

            if (Input.GetButtonDown("Jump")) moveData.wishJump = true;
            if (!Input.GetButton("Jump")) moveData.wishJump = false;
            
            moveData.viewAngles = _angles;
        }

        private void DisableInput()
        {
            moveData.verticalAxis = 0f;
            moveData.horizontalAxis = 0f;
            moveData.sideMove = 0f;
            moveData.forwardMove = 0f;
            moveData.wishJump = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float from, float to)
        {
            if (angle < 0f) angle = 360 + angle;
            if (angle > 180f) return Mathf.Max(angle, 360 + from);

            return Mathf.Min(angle, to);
        }
    }
}