using Sirenix.OdinInspector;
using Systems.Base;
using UnityEngine;

namespace Objects.Base
{
    public abstract class Unit : MonoBehaviour
    {
        public GameSystemsContainer SystemsContainer => _systemsContainer;
        protected GameSystemsContainer _systemsContainer = new GameSystemsContainer();
        
        [ShowInInspector] public float HealthPoints => _healthPoints;

        private float _healthPoints = 100;
    }
}