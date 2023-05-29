using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameItems.Base
{
    [SelectionBase]
    public abstract class Unit : MonoBehaviour, ISystemsContainable
    {
        public GameSystemsContainer SystemsContainer => UnitSystemsContainer;
        
        [SerializeField] [HideLabel]
        protected GameSystemsContainer UnitSystemsContainer;

        protected virtual void Awake() => Setup();
        protected virtual void OnEnable() => UnitSystemsContainer.StartAllSystems();
        protected virtual void Update() => UnitSystemsContainer.UpdateSystems();
        protected virtual void FixedUpdate() => UnitSystemsContainer.UpdatePhysicsSystems();
        protected virtual void OnDisable() => UnitSystemsContainer.ShutDownSystems();

        protected void Setup()
        {
            UnitSystemsContainer.SetOwner(this);
            UnitSystemsContainer.InitSystems();
        }
    }
}