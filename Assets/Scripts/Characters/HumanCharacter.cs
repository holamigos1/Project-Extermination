using System;
using Characters.ConsciousnessEntities.Base;
using Characters.Data.Base;
using GameObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class HumanCharacter : MonoBehaviour
    {
        [SerializeField] private ConsciousnessEntityData CharactersConsciousnessEntityData;
        [SerializeField] private HumanoidBody _bodyController;
        
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private CharacterController _characterController;
            
        private IHumanEntity _currentHumanDriver;

        public IHumanEntity HumanDriver
        {
            get => _currentHumanDriver;
            private set
            {
                if(_currentHumanDriver != null && _currentHumanDriver.Equals(value)) return;
                
                //TODO Если функции колбеков будут жирными, раскидай их по соответсвующим обработчиками и подпиши их тут
                
                if (_currentHumanDriver != null)
                {
                    _currentHumanDriver.AimAction -= OnAimAction; 
                    _currentHumanDriver.AttackAction -= OnAttackAction;
                    _currentHumanDriver.InteractAction -= OnInteractAction;
                    _currentHumanDriver.JumpAction -= OnJumpAction;
                    _currentHumanDriver.MoveDirectionAction -= OnMoveDirectionAction;
                    _currentHumanDriver.ReloadAction -= OnReloadAction;
                    _currentHumanDriver.SprintAction -= OnSprintAction;
                    _currentHumanDriver.ThrowAction -= OnThrowAction;
                    _currentHumanDriver.SitDownAction -= OnSitDownAction;
                }

                _currentHumanDriver = value;
                
                if(_currentHumanDriver == null) return;
                
                _currentHumanDriver.AimAction += OnAimAction;
                _currentHumanDriver.AttackAction += OnAttackAction;
                _currentHumanDriver.InteractAction += OnInteractAction;
                _currentHumanDriver.JumpAction += OnJumpAction;
                _currentHumanDriver.MoveDirectionAction += OnMoveDirectionAction;
                _currentHumanDriver.ReloadAction += OnReloadAction;
                _currentHumanDriver.SprintAction += OnSprintAction;
                _currentHumanDriver.ThrowAction += OnThrowAction;
                _currentHumanDriver.SitDownAction += OnSitDownAction;
            }
        }

        #if UNITY_EDITOR
        private void Reset()
        {
            _transform = transform;
            _gameObject = gameObject;
            _bodyController = _transform.GetComponentsInAllChildren<HumanoidBody>().First();
            _characterController = GetComponent<CharacterController>();
            
            if(_bodyController == null) 
                Debug.LogError($"{nameof(HumanoidBody)} controller is not exist in Character game object hierarchy!");
        }
        #endif
        
        private void Awake()
        {
            if (CharactersConsciousnessEntityData is IHumanEntityCreator entityCreator)
                HumanDriver = entityCreator.CreateEntityInstance();
            else Debug.LogError($"{CharactersConsciousnessEntityData.name} type of {nameof(ConsciousnessEntityData)} is not designed to control this human being!");

            _characterController.enableOverlapRecovery = true;
            _characterController.detectCollisions = true;
            HumanDriver.UpdateEntity();
        }

        private void OnDestroy()
        {
            HumanDriver = null;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            switch (hit.controller.collisionFlags)
            {
                case CollisionFlags.None:
                    Debug.Log(CollisionFlags.None);
                    break;
                case CollisionFlags.Sides:
                    Debug.Log(CollisionFlags.Sides);
                    break;
                case CollisionFlags.Above:
                    Debug.Log(CollisionFlags.Above);
                    break;
                case CollisionFlags.Below:
                    Debug.Log(CollisionFlags.Below);
                    break;
            }
        }
        private void FixedUpdate()
        {
            _characterController.Move(_bodyController.rootMotionPhysicsDelta);
        }

        private void OnSitDownAction(InputActionPhase actionPhase)
        {
            //_viewController.SitDown();
        }

        private void OnThrowAction(InputActionPhase actionPhase)
        {
            //_viewController.Throw();
        }

        private void OnSprintAction(InputActionPhase actionPhase)
        {
            if(actionPhase == InputActionPhase.Started) _bodyController.ApplySprint(true);
            if(actionPhase == InputActionPhase.Canceled) _bodyController.ApplySprint(false);
        }

        private void OnReloadAction(InputActionPhase actionPhase)
        {
            //_viewController.Reload();
        }

        private void OnMoveDirectionAction(Vector2 direction, InputActionPhase actionPhase)
        {
            _bodyController.ApplyMovementDirection(direction);
        }

        private void OnJumpAction(InputActionPhase actionPhase)
        {
            if(actionPhase == InputActionPhase.Started) 
                _bodyController.ApplyJump();
        }

        private void OnInteractAction(InputActionPhase actionPhase)
        {
            //_viewController.Interact();
        }

        private void OnAttackAction(InputActionPhase actionPhase)
        {
            //_viewController.Attack();
        }

        private void OnAimAction(InputActionPhase actionPhase)
        {
            //_viewController.Aim();
        }
    }
}