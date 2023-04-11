using GameAnimation.Sheets;
using UnityEngine;

namespace Characters.Humanoid
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HumanoidBody : MonoBehaviour
    {
        public Vector3 Velocity => _humanoidBodyParameters.MovementAcceleration;
        public Vector3 RootPositionDelta => _rootPositionDelta;
        public Vector3 RootRotationDelta => _rootRotationDelta;
        
        [SerializeField] private HumanParametersSheet _humanAnimatorSheet;
        
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private Animator _animator;

        //TODO Контроллер глаз
        
        private HumanoidBodyParameters _humanoidBodyParameters;
        public HumanHead HeadController { get; private set; }
        
        private Vector3 _rootPositionDelta;
        private Vector3 _rootRotationDelta;

        #if UNITY_EDITOR
        private void Reset()
        {
            _animator = GetComponent<Animator>();
            _transform = transform;
            _gameObject = gameObject;   
            _humanAnimatorSheet = AssetDataBaseExtensions.LoadAssetAtFilter<HumanParametersSheet>
                ($"t:{nameof(HumanParametersSheet)}");
            
            if (_humanAnimatorSheet == null) 
                Debug.LogError("Где список параметров гуманоида в проекте?");
        }
        #endif

        private void Awake()
        {
            _humanoidBodyParameters = new HumanoidBodyParameters(_animator, _humanAnimatorSheet);
            HeadController = new HumanHead(_animator);
            
            _animator.stabilizeFeet = true;
        }

        private void Update()
        {
            _humanoidBodyParameters.FrameTick();
        }

        public void ApplyMovementDirection(Vector2 direction)
        {
            _humanoidBodyParameters.ForwardMovementParameter = direction.y;
            _humanoidBodyParameters.SideMovementParameter = direction.x;
        }

        public void ApplyRotationDirection(Vector2 direction)
        {
            
        }

        public void ApplySprint(bool isSprinting)
        {
            if (isSprinting) _humanoidBodyParameters.CurrentMovementType = HumanoidBodyParameters.MovementType.Sprinting;
            else _humanoidBodyParameters.CurrentMovementType = HumanoidBodyParameters.MovementType.Walking;
        }
        
        public void ApplyJump()
        {

        }
        
        private void OnAnimatorMove()
        {
            _rootPositionDelta += _animator.deltaPosition;
            //transform.rotation = Quaternion.Euler(_rootRotationDelta);
        }

        private void OnAnimatorIK(int layerIndex)
        {

        }

        private void FixedUpdate() //TODO Не забудь поменять Execution Order в пользу контроллера персонажа а не его тела
        {
            _rootPositionDelta = Vector3.zero;
        }
    }
}
