using GameAnimation.Sheets;
using Misc;
using UnityEngine;

namespace Characters.Humanoid
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HumanoidBody : MonoBehaviour
    {
        public Vector3 Velocity => _humanoidBodyParameters.MovementAcceleration;
        public Vector3 RootPositionDelta => _rootPositionDelta;
        public Quaternion RootRotationDelta => _rootRotationDelta;
        
        [SerializeField] private HumanParametersSheet _humanAnimatorSheet;
        
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private Animator _animator;

        //TODO Контроллер глаз
        
        private HumanoidBodyParameters _humanoidBodyParameters;
        public HumanHead HeadController { get; private set; }
        
        private Vector3 _rootPositionDelta;
        private Quaternion _rootRotationDelta;

        #if UNITY_EDITOR
        private void Reset()
        {
            _animator = GetComponent<Animator>();
            _transform = transform;
            _gameObject = gameObject;   
            _humanAnimatorSheet = AssetDataBaseExtensions.LoadAssetAtFilter<HumanParametersSheet>
                ($"t:{nameof(HumanParametersSheet)}");

            if (_humanAnimatorSheet == null)
                Debug.LogError($"Создай список {nameof(HumanParametersSheet)} в проекте!");
        }
        #endif

        private void Awake()
        {
            if (_transform == null) Debug.LogError($"Reset {nameof(HumanoidBody)}!");
            if (_gameObject == null) Debug.LogError($"Reset {nameof(HumanoidBody)}!");
            if (_animator == null) Debug.LogError($"Reset {nameof(HumanoidBody)}!");
            
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
            _rootRotationDelta = Quaternion.AngleAxis(direction.x, Vector3.up);
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
        }

        private void OnAnimatorIK(int layerIndex)
        {

        }

        private void FixedUpdate() //TODO Не забудь поменять Execution Order в пользу контроллера персонажа а не его тела
        {
            if (_rootRotationDelta != Quaternion.identity) 
                _animator.stabilizeFeet = false;
            else _animator.stabilizeFeet = true;

            _rootPositionDelta = Vector3.zero; //подтираю дельты
            _rootRotationDelta = Quaternion.identity;
        }
    }
}
