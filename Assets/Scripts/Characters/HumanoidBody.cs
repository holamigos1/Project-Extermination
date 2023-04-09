using GameAnimation.Sheets;
using UnityEngine;

namespace Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HumanoidBody : MonoBehaviour
    {
        public Vector3 Velocity => _humanoidBodyParameters.MovementAcceleration;
        public Vector3 rootMotionPhysicsDelta => _animator.deltaPosition;
        
        [SerializeField] private HumanParametersSheet _humanAnimatorSheet;
        
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private Animator _animator;

        //TODO Контроллер глаз
        
        private HumanoidBodyParameters _humanoidBodyParameters;
        private Vector3 _rootMotionPhysicsDelta;

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
            _animator.applyRootMotion = true;
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
            Debug.Log("OnAnimatorMove");
            _rootMotionPhysicsDelta += _animator.deltaPosition;
        }

        private void FixedUpdate() //TODO Не забудь поменять Execution Order в пользу контроллера персонажа а не его тела
        {
            Debug.Log("FixedUpdate");
            _rootMotionPhysicsDelta = Vector3.zero;
        }
    }
}
