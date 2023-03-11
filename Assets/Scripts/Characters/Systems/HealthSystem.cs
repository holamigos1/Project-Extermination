using System;
using GameSystems.Base;
using Sirenix.OdinInspector;

namespace Characters.Systems
{
    [Serializable]
    public class HealthSystem : GameSystem
    {
        public Action Died;
        
        public float HealthPoints => _healthPoints;
        
        private float _healthPoints;
        private float _maxHealthAmount;

        public HealthSystem(float healthAmount)
        {
            _maxHealthAmount = healthAmount;
            _healthPoints = healthAmount;
        }

        public void ApplyDamage(float damageAmount)
        {
            _healthPoints -= damageAmount;
            
            if (_healthPoints >= 0) return;
            
            _healthPoints = 0;
            Died?.Invoke();
        }

        public void HealthRestore(float healAmount)
        {
            _healthPoints += healAmount;
        }
    }
}