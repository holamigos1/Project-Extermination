using Characters.Systems;
using GameSystems.GameCamera;
using Objects.Base;
using UnityEngine;

namespace Characters
{
    public class Character : Unit
    {
        [SerializeField] private Transform _handPosition;
        [SerializeField] private LayerMask _rayblockingLayers;
        
        private void Awake()
        { 
            CameraSystem.CurrentMainCamera = Camera.main; //TODO Character не должен заниматься камерой!
            
            UnitSystemsContainer.AddSystem(new RaycastSystem(new RaycastSystemData(this, _rayblockingLayers)));
            UnitSystemsContainer.AddSystem(new OldInputMediatorSystem());
            UnitSystemsContainer.AddSystem(new WeaponMediatorSystem());
            UnitSystemsContainer.AddSystem(new HandSystem(_handPosition));
            
            Debug.Log("Character.enabled" + this.enabled);//Если это строчку удалить то Character компонет почему то сам по себе отрубается, держу в курсе
        }
    }
}