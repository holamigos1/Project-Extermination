using System;
using Characters.ConsciousnessEntities.Base;
using Characters.Data.Base;
using GameAnimation.AnimatorControllers;
using GameObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public class HumanCharacter : MonoBehaviour
    {
        [SerializeField] private ConsciousnessEntityData CharactersConsciousnessEntityData;
        [SerializeField] private HumanoidBody _viewController;
        
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
            
        private IHumanEntity _currentHumanDriver;

        public IHumanEntity HumanDriver
        {
            get => _currentHumanDriver;
            private set
            {
                if(_currentHumanDriver != null && _currentHumanDriver.Equals(value)) return;
                
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
            _viewController = _transform.GetComponentsInAllChildren<HumanoidBody>().First();
            
            if(_viewController == null) 
                Debug.LogError($"{nameof(HumanoidBody)} controller is not exist in Character game object hierarchy!");
        }
#endif
        
        private void Awake()
        {
            if (CharactersConsciousnessEntityData is IHumanEntityCreator entityCreator)
                HumanDriver = entityCreator.CreateEntityInstance();
            else Debug.LogError($"{CharactersConsciousnessEntityData.name} type of {nameof(ConsciousnessEntityData)} is not designed to control this human being!");
            
            HumanDriver.UpdateEntity();
        }

        private void OnDestroy()
        {
            HumanDriver = null;
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
            //_viewController.Sprint();
        }

        private void OnReloadAction(InputActionPhase actionPhase)
        {
            //_viewController.Reload();
        }

        private void OnMoveDirectionAction(Vector2 direction, InputActionPhase actionPhase)
        {
            Debug.Log(direction);
        }

        private void OnJumpAction(InputActionPhase actionPhase)
        {
            //_viewController.Jump();
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