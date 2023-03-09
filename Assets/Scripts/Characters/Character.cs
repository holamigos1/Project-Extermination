using Characters.Systems;
using GameSystems.GameCamera;
using Objects.Base;
using UnityEngine;

namespace Characters
{
    public class Character : Unit
    {
        [SerializeField] private Transform _handPosition;
        [SerializeField] private LayerMask _rayBlockLayers;
        
        private void Start()
        {
            CameraSystem.CurrentMainCamera = Camera.main; //TODO Character не должен заниматься камерой!
            
            UnitSystemsContainer.AddSystem(new RaycastSystem(new RaycastSystemData(this, _rayBlockLayers)));
            UnitSystemsContainer.AddSystem(new OldInputMediatorSystem());
            UnitSystemsContainer.AddSystem(new WeaponMediatorSystem());
            UnitSystemsContainer.AddSystem(new HandSystem(_handPosition));
            
            this.enabled = true;//Если это строчку удалить то Character компонет почему то сам по себе отрубается, держу в курсе
        }
    }
}