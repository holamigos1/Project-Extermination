using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewInput
{
    public class FPSCharacterInputActionsHandler : GameInputActions.IFPSCharacterActions, IDisposable
    {
        public FPSCharacterInputActionsHandler()
        {
            _inputActionsAsset = new GameInputActions();
            _inputActionsAsset.FPSCharacter.SetCallbacks(this);
            _inputActionsAsset.FPSCharacter.Enable();
        }
        
        public event Action<Vector2, InputActionPhase> MoveDirectionAction;
        public event Action<InputActionPhase> InteractAction;
        public event Action<InputActionPhase> ReloadAction;
        public event Action<InputActionPhase> ThrowAction;
        public event Action<InputActionPhase> SitDownAction;
        public event Action<InputActionPhase> JumpAction;
        public event Action<InputActionPhase> SprintAction;
        public event Action<InputActionPhase> AttackAction;
        public event Action<InputActionPhase> AimAction;
        
        private readonly GameInputActions _inputActionsAsset;

        public void Dispose()
        {
            _inputActionsAsset.FPSCharacter.Disable();
        }
        
        public void OnMove(InputAction.CallbackContext context) =>
            MoveDirectionAction?.Invoke(context.ReadValue<Vector2>(), context.action.phase);
        public void OnInteract(InputAction.CallbackContext context) =>
            InteractAction?.Invoke(context.action.phase);
        public void OnReload(InputAction.CallbackContext context) =>
            ReloadAction?.Invoke(context.action.phase);
        public void OnThrow(InputAction.CallbackContext context) =>
            ThrowAction?.Invoke(context.action.phase);
        public void OnSitDown(InputAction.CallbackContext context) =>
            SitDownAction?.Invoke(context.action.phase);
        public void OnJump(InputAction.CallbackContext context) =>
            JumpAction?.Invoke(context.action.phase);
        public void OnSprint(InputAction.CallbackContext context) =>
            SprintAction?.Invoke(context.action.phase);
        public void OnAttack(InputAction.CallbackContext context) =>
            AttackAction?.Invoke(context.action.phase);
        public void OnAim(InputAction.CallbackContext context) =>
            AimAction?.Invoke(context.action.phase);
    }
}