using System;
using Characters.ConsciousnessEntities.Base;
using NewInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.ConsciousnessEntities
{
    public class PlayerHumanEntity : IHumanEntity
    {
        public PlayerHumanEntity(int instanceID)
        {
            _fpsCharacterInputActionsHandler = new FPSCharacterInputActionsHandler();
            _instanceID = instanceID;
        }
        
        private readonly FPSCharacterInputActionsHandler _fpsCharacterInputActionsHandler;
        private readonly int _instanceID;
        
        public override int GetHashCode() => 
            _instanceID;

        ~PlayerHumanEntity()
        {
            _fpsCharacterInputActionsHandler?.Dispose();
        }

        public void UpdateEntity()
        {
            
        }

        event Action<Vector2, InputActionPhase> IHumanEntity.MoveDirectionAction
        {
            add => _fpsCharacterInputActionsHandler.MoveDirectionAction += value;
            remove => _fpsCharacterInputActionsHandler.MoveDirectionAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.InteractAction
        {
            add => _fpsCharacterInputActionsHandler.InteractAction += value;
            remove => _fpsCharacterInputActionsHandler.InteractAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.ReloadAction
        {
            add => _fpsCharacterInputActionsHandler.ReloadAction += value;
            remove => _fpsCharacterInputActionsHandler.ReloadAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.ThrowAction
        {
            add => _fpsCharacterInputActionsHandler.ThrowAction += value;
            remove => _fpsCharacterInputActionsHandler.ThrowAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.SitDownAction
        {
            add => _fpsCharacterInputActionsHandler.SitDownAction += value;
            remove => _fpsCharacterInputActionsHandler.SitDownAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.JumpAction
        {
            add => _fpsCharacterInputActionsHandler.JumpAction += value;
            remove => _fpsCharacterInputActionsHandler.JumpAction -= value;
        }
        event Action<InputActionPhase> IHumanEntity.SprintAction
        {
            add => _fpsCharacterInputActionsHandler.SprintAction += value;
            remove => _fpsCharacterInputActionsHandler.SprintAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.AttackAction
        {
            add => _fpsCharacterInputActionsHandler.AttackAction += value;
            remove => _fpsCharacterInputActionsHandler.AttackAction -= value;
        }

        event Action<InputActionPhase> IHumanEntity.AimAction
        {
            add => _fpsCharacterInputActionsHandler.AimAction += value;
            remove => _fpsCharacterInputActionsHandler.AimAction -= value;
        }
    }
}