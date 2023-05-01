using System;
using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Systems
{
    [Serializable]
    public class HealthSystem : GameSystem
    {
        [Title("Обработчик очков здоровья юнита.", 
            "Если эта сисетма отсуствует то обработка получения урона юнитом работать не будет.")]
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219, CS0414
        private string info = "";
        
        [ShowInInspector] [LabelText("Здоровье")]
        [ProgressBar(0, "MaxHealthAmount", ColorGetter = "GetHealthBarColor")]
        public float HealthPoints 
        {
            get => _healthPoints;
            set
            {
                if (value < _healthPoints && Application.isPlaying) ApplyDamage(_healthPoints - value);
                if (value > _healthPoints && Application.isPlaying) HealthRestore(value - _healthPoints);
                
                _healthPoints = value;
            }
        }
        public float MaxHealthAmount => _maxHealthAmount;
        
        [SerializeField]
        [HideInInspector]
        private float _healthPoints;
        
        [MinValue(1)]
        [SerializeField]
        [LabelText("Предел очков здоровья")]
        private float _maxHealthAmount;
        
        [SerializeField]
        [LabelText("Бессмертие")]
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
            if (_isImmortal == false) 
                _healthPoints -= damageAmount;
            
            if (_healthPoints > 0) 
                SystemsСontainer.NotifySystems("Damage applied", damageAmount);
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