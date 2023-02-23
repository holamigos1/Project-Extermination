using Characters.Systems;
using Objects.Base;
using Systems.GameCamera;
using UnityEngine;

namespace Characters
{
    public class Character : Unit
    {
        [SerializeField] private Transform _handPosition;
        [SerializeField] private LayerMask _rayblockingLayers;
        
        private void Awake()
        { 
            CameraSystem.CurrentMainCamera = Camera.main;
            
            _systemsContainer.AddSystem(new HandSystem(_handPosition));
            _systemsContainer.AddSystem(new RaycastSystem(new RaycastSystemData(this, _rayblockingLayers)));
            _systemsContainer.AddSystem(new OldInputMediatorSystem());
            _systemsContainer.AddSystem(new WeaponMediatorSystem());
        }

        private void OnDisable() => _systemsContainer.ShutDownSystems();
        private void Update() => _systemsContainer.UpdateSystems();
        private void FixedUpdate() => _systemsContainer.UpdatePhysicsSystems();
    }
}