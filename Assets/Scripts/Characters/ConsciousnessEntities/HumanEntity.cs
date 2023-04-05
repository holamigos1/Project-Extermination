using System;
using Characters.ConsciousnessEntities.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.ConsciousnessEntities
{
    public class HumanEntity : IHumanEntity
    {
        public HumanEntity(int instanceID) //TODO сюда прокидывать классы логики ИИ пресонажей
        {
            _instanceID = instanceID;
        }
        
        public bool IsActive { get; set; }
        
        public event Action<Vector2, InputActionPhase> MoveDirectionAction;
        public event Action<InputActionPhase> InteractAction;
        public event Action<InputActionPhase> ReloadAction;
        public event Action<InputActionPhase> ThrowAction;
        public event Action<InputActionPhase> SitDownAction;
        public event Action<InputActionPhase> JumpAction;
        public event Action<InputActionPhase> SprintAction;
        public event Action<InputActionPhase> AttackAction;
        public event Action<InputActionPhase> AimAction;

        private readonly int _instanceID;

        ~HumanEntity()
        {
            
        }
        
        public void Dispose()
        {
            
        }
        
        public override int GetHashCode() => 
            _instanceID;

        public void UpdateEntity()
        {
            //TODO Логику ИИ персонажей писать тут
        }
    }
}