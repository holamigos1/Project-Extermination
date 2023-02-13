using UnityEngine;

namespace Characters.Systems
{
    public class HandSystem
    {
        public HandSystem(Transform handPoint)
        {
            _handPoint = handPoint;
        }
        
        public GameObject EquippedGameObject => _equippedGameObject;
        
        private GameObject _equippedGameObject;
        private Transform _handPoint;
        
        public void Equip(GameObject gameObject)
        {
            if(_equippedGameObject != null) return;
            
            
        }
    }
}
