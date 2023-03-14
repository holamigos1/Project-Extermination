using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Objects.Base
{
    [SelectionBase]
    public abstract class Unit : MonoBehaviour, ISystemsContainable
    {
        public GameSystemsContainer SystemsContainer => UnitSystemsContainer;
        
        [SerializeField] [HideLabel]
        protected GameSystemsContainer UnitSystemsContainer;

        protected virtual void Awake() => UnitSystemsContainer.InitSystems();
        protected virtual void OnEnable() => UnitSystemsContainer.StartAllSystems();
        protected virtual void Update() => UnitSystemsContainer.UpdateSystems();
        protected virtual void FixedUpdate() => UnitSystemsContainer.UpdatePhysicsSystems();
        protected virtual void OnDisable() => UnitSystemsContainer.ShutDownSystems();
    }
}