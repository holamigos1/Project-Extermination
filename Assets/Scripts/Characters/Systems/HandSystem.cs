using Systems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class HandSystem : GameSystem
    {
        public HandSystem(GameSystemsContainer container, Transform handPoint) : base(container)
        {
            _handPoint = handPoint;
        }

        public GameObject EquippedGameObject => _equippedGameObject;

        private GameObject _equippedGameObject;
        private Transform _handPoint;

        public void Equip(GameObject gameObjectInst)
        {
            if (_equippedGameObject != null)
            {
                Debug.LogWarning("Попытка взять предмет не удалсь, т.к. уже есть объект в руке!");
                
                //TODO Дописать логику проброса объекта из руки в инвентарь и взятии нового предмета по требованию.
                return;
            }

            _equippedGameObject = UnityEngine.Object.Instantiate(gameObjectInst, _handPoint);
        }

        public void Drop()
        {
            if (_equippedGameObject == null) return;

            Rigidbody objRB =_equippedGameObject.GetComponent<Rigidbody>();
            objRB.isKinematic = false;
            objRB.useGravity = true;
            objRB.transform.parent = null;//теперь не дочка объекта руки

            _equippedGameObject = null;
        }
    }
}
