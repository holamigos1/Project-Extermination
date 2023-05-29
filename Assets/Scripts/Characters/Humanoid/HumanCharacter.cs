﻿using Characters.ConsciousnessEntities.Base;
using GameItems.Base;
using Misc.Extensions;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace Characters.Humanoid
{
    [RequireComponent(typeof(CharacterController))]
    public class HumanCharacter : MonoBehaviour
    {
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
                    _currentHumanDriver.LookDirectionAction -= OnLookDirectionAction;
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
                _currentHumanDriver.LookDirectionAction += OnLookDirectionAction;
                _currentHumanDriver.ReloadAction += OnReloadAction;
                _currentHumanDriver.SprintAction += OnSprintAction;
                _currentHumanDriver.ThrowAction += OnThrowAction;
                _currentHumanDriver.SitDownAction += OnSitDownAction;
            }
        }
        
        //TODO Поле растёт, убери всё в классы настроект!
        [Tooltip("Перенеси сюда ConsciousnessEntityData чтобы у персонажа было сознание.")]
        [SerializeField] private ConsciousnessEntityData CharactersConsciousnessEntity;
        [SerializeField] private HumanoidBody _bodyController;
        [Tooltip("Перенеси сюда MultiAimConstraint который управляет взглядом головы.")]
        [SerializeField] private MultiAimConstraint _headAimConstraint;
        [SerializeField] private AimRoot _aimRoot;
        [Tooltip("Слои объектов с которыми персонаж может взаимодействовать.")]
        [SerializeField] private LayerMask _rayBlockingMask; 

        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private CharacterController _characterController;
            
        private IHumanEntity _currentHumanDriver;

#if UNITY_EDITOR
        private void Reset() //TODO жирно
        {
            _transform = transform;
            _gameObject = gameObject;
            _gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _bodyController = _transform.GetComponentsInChildren<HumanoidBody>().First();
            _characterController = GetComponent<CharacterController>();
            _aimRoot = _transform.GetComponentsInChildren<AimRoot>().First();

            if(_bodyController == null) 
                Debug.LogError($"{nameof(HumanoidBody)} controller is not exist in Character game object hierarchy!");
        }
        #endif
        
        private void Awake() //TODO жирно
        {
            if (CharactersConsciousnessEntity is IHumanEntityCreator entityCreator) 
                HumanDriver = entityCreator.CreateEntityInstance();
            else Debug.LogError($"{CharactersConsciousnessEntity.name} type of {nameof(ConsciousnessEntityData)} is not designed to control this human being!");

            if (_transform == null) Debug.LogError($"Reset {nameof(HumanCharacter)}!");
            if (_gameObject == null) Debug.LogError($"Reset {nameof(HumanCharacter)}!");
            if (_characterController == null) Debug.LogError($"Reset {nameof(HumanCharacter)}!");

            _characterController.detectCollisions = true;
            _characterController.enableOverlapRecovery = true;
            _aimRoot.SetClamping(_headAimConstraint.data.limits, _headAimConstraint.data.limits);
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
                    //Debug.Log(CollisionFlags.None);
                    break;
                case CollisionFlags.Sides:
                    //Debug.Log(CollisionFlags.Sides);
                    break;
                case CollisionFlags.Above:
                   // Debug.Log(CollisionFlags.Above);
                    break;
                case CollisionFlags.Below:
                    //Debug.Log(CollisionFlags.Below);
                    break;
            }
        }

        private void Update()
        {
            HumanDriver.UpdateEntity();
            _aimRoot.SyncWithHeadBone(_bodyController.HeadController.HeadTransform);
        }

        private void FixedUpdate()
        {
            _characterController.Move(_bodyController.RootPositionDelta + Physics.gravity * Time.smoothDeltaTime);
            _transform.rotation *= _bodyController.RootRotationDelta;
        }

        private void OnLookDirectionAction(Vector2 directionVelocity, InputActionPhase actionPhase)
        {
            directionVelocity *= Time.smoothDeltaTime;

            _aimRoot.Pitch(-directionVelocity.y);// минус тк +X это наклон вниз, нада вверх
            bool isClamped = _aimRoot.Yaw(directionVelocity.x);
            
            if(isClamped) 
                _bodyController.ApplyRotationDirection(directionVelocity);
        }
        
        private void OnMoveDirectionAction(Vector2 directionVelocity, InputActionPhase actionPhase)
        {
            _bodyController.ApplyMovementDirection(directionVelocity);
        }

        private void OnSitDownAction(InputActionPhase actionPhase)
        {
            //_viewController.SitDown();
        }

        private void OnThrowAction(InputActionPhase actionPhase) =>
            _bodyController.ApplyThrowItem();
        

        private void OnSprintAction(InputActionPhase actionPhase)
        {
            if(actionPhase == InputActionPhase.Started) _bodyController.ApplySprint(true);
            if(actionPhase == InputActionPhase.Canceled) _bodyController.ApplySprint(false);
        }

        private void OnReloadAction(InputActionPhase actionPhase) =>
            _bodyController.ApplyReload();
        

        private void OnJumpAction(InputActionPhase actionPhase)
        {
            if(actionPhase == InputActionPhase.Started) 
                _bodyController.ApplyJump();
        }

        private void OnInteractAction(InputActionPhase actionPhase)
        {
            if(actionPhase != InputActionPhase.Started) return;
            
            GameObject raycastObject = _aimRoot.GetRayBlockObject(_rayBlockingMask);
            
            if (raycastObject == null) return;
            if (raycastObject.TryGetComponent(out GameItem item))
                _bodyController.ApplyPickUp(item);
        }

        private void OnAttackAction(InputActionPhase actionPhase)
        {
            if(actionPhase == InputActionPhase.Started)
                _bodyController.ApplyAttack();
        }

        private void OnAimAction(InputActionPhase actionPhase)
        {
            if(actionPhase == InputActionPhase.Started)
                _bodyController.ApplyAim(true);
            if(actionPhase == InputActionPhase.Canceled)
                _bodyController.ApplyAim(false);
        }
    }
}