using System;
using GameSystems.Base;
using UnityEngine;

namespace Objects.Base
{
    public abstract class Unit : MonoBehaviour
    {
        public GameSystemsContainer SystemsContainer => UnitSystemsContainer;
        
        [SerializeField] protected GameSystemsContainer UnitSystemsContainer = new GameSystemsContainer();

        private void Awake() => UnitSystemsContainer.InitSystems();
        protected virtual void OnDisable() => UnitSystemsContainer.ShutDownSystems();
        protected virtual void Update() => UnitSystemsContainer.UpdateSystems();
        protected virtual void FixedUpdate() => UnitSystemsContainer.UpdatePhysicsSystems();
    }
}