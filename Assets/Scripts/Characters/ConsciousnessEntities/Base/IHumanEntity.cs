using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.ConsciousnessEntities.Base
{
    public interface IHumanEntity
    {
        event Action<Vector2, InputActionPhase> MoveDirectionAction;
        event Action<InputActionPhase> InteractAction;
        event Action<InputActionPhase> ReloadAction;
        event Action<InputActionPhase> ThrowAction;
        event Action<InputActionPhase> SitDownAction;
        event Action<InputActionPhase> JumpAction;
        event Action<InputActionPhase> SprintAction;
        event Action<InputActionPhase> AttackAction;
        event Action<InputActionPhase> AimAction;
        
        void UpdateEntity();
        int GetHashCode();
    }
}