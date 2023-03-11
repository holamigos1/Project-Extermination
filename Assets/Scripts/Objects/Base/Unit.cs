using System.Linq;
using GameSystems.Base;
using UnityEngine;

namespace Objects.Base
{
    public abstract class Unit : MonoBehaviour, ISystemsContainable
    {
        public GameSystemsContainer SystemsContainer => UnitSystemsContainer;
        
        [SerializeField] 
        protected GameSystemsContainer UnitSystemsContainer = new GameSystemsContainer();

        protected virtual void Awake() => UnitSystemsContainer.InitSystems();
        protected virtual void OnEnable() => UnitSystemsContainer.StartAllSystems();
        protected virtual void Update() => UnitSystemsContainer.UpdateSystems();
        protected virtual void FixedUpdate() => UnitSystemsContainer.UpdatePhysicsSystems();
        protected virtual void OnDisable() => UnitSystemsContainer.ShutDownSystems();
    }
}