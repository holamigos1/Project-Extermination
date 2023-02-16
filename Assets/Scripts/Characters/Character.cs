using System;
using Characters.Systems;
using Objects.Base;
using Systems.Base;
using Systems.GameCamera;
using UnityEngine;

namespace Characters
{
    public class Character : Unit
    {
        public GameSystemsContainer SystemsContainer => _systemsContainer;
        
        [SerializeField] 
        private Transform _handPosition;
        
        private GameSystemsContainer _systemsContainer;
        
        private void Awake()
        { 
            CameraSystem.CurrentMainCamera = Camera.main;
            
            _systemsContainer = new GameSystemsContainer();
            _systemsContainer.AddSystem(new GrubSystem(_systemsContainer, _handPosition));
            _systemsContainer.AddSystem(new HandSystem(_systemsContainer, _handPosition));
            _systemsContainer.AddSystem(new RaycastSystem(this, _systemsContainer));
            _systemsContainer.AddSystem(new PlayerInputMediatorSystem(_systemsContainer));
        }
        private void OnDisable() => _systemsContainer.ShutDownSystems();
        private void Update() => _systemsContainer.UpdateSystems();

        private void FixedUpdate() => _systemsContainer.UpdatePhysicsSystems();
    }
}