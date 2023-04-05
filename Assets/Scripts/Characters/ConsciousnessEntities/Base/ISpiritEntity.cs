using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.ConsciousnessEntities.Base
{
    public interface ISpiritEntity
    {
        bool IsActive { get; set; }
        event Action<Vector2, InputActionPhase> MoveDirectionAction;
        event Action<InputActionPhase> InteractAction;
    }
}