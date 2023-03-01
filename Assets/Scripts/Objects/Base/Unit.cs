using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Objects.Base
{
    public abstract class Unit : MonoBehaviour
    {
        public GameSystemsContainer SystemsContainer => UnitSystemsContainer;
        [ShowInInspector] public float HealthPoints => _healthPoints;
        
        
        protected GameSystemsContainer UnitSystemsContainer = new GameSystemsContainer();

        
        private float _healthPoints = 100;
        
        
        protected virtual void OnDisable() => UnitSystemsContainer.ShutDownSystems();
        protected virtual void Update() => UnitSystemsContainer.UpdateSystems();
        protected virtual void FixedUpdate() => UnitSystemsContainer.UpdatePhysicsSystems();
    }
}