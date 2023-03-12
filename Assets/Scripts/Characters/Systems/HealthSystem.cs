using System;
using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Systems
{
    [Serializable]
    public class HealthSystem : GameSystem
    {
        [ShowInInspector]
        [ProgressBar(0, "MaxHealthAmount", ColorGetter = "GetHealthBarColor")]
        public float HealthPoints
        {
            get => _healthPoints;
            set
            {
                if (_healthPoints < value && Application.isPlaying) ApplyDamage(_healthPoints - value);
                if (_healthPoints > value && Application.isPlaying) HealthRestore(value - _healthPoints);
                _healthPoints = value;
            }
        }
        public float MaxHealthAmount => _maxHealthAmount;
        
        [SerializeField]
        [HideInInspector]
        private float _healthPoints;
        
        [SerializeField]
        private float _maxHealthAmount;
        
        [SerializeField]
        private bool _isImmortal;

        public HealthSystem()
        {
            _maxHealthAmount = 2077f;
            _healthPoints = _maxHealthAmount;
        }
        public HealthSystem(float healthAmount)
        {
            _maxHealthAmount = healthAmount;
            _healthPoints = healthAmount;
        }

        public override void Start()
        {
            SystemsСontainer.Notify += OnNotify;
        }
        
        public override void Stop()
        {
            SystemsСontainer.Notify -= OnNotify;
        }

        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "Apply Damage" when data != null:
                    ApplyDamage((float)data);
                    break;
            }
        }

        public void ApplyDamage(float damageAmount)
        {
            _healthPoints -= damageAmount;
            if (_isImmortal) _healthPoints += damageAmount;
            
            if (_healthPoints > 0) 
                SystemsСontainer.NotifySystems("Damage applied");
            else {
                _healthPoints = 0;
                SystemsСontainer.NotifySystems("Died");
            }
        }

        public void HealthRestore(float healAmount)
        {
            _healthPoints += healAmount;
            
            if (_healthPoints >= _maxHealthAmount) 
                _healthPoints = _maxHealthAmount;
        }
        
        private Color GetHealthBarColor(float value) =>
            Color.Lerp(Color.red, Color.green, value / _maxHealthAmount);
    }
}