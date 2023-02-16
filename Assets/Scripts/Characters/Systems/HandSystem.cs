using Objects.Base;
using Systems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class HandSystem : GameSystem
    {
        public GameObject EquippedGameObject => _equippedGameObject;
        
        public HandSystem(GameSystemsContainer container, Transform handPoint) : base(container)
        {
            _handPoint = handPoint;
            if (_handPoint.GetChild(0) != null) _equippedGameObject = _handPoint.GetChild(0).gameObject;
        }

        private GameObject _equippedGameObject;
        private Transform _handPoint;

        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "Object pickuped" when data != null:
                    GameObject gameObject = data as GameObject;
                    Equip(gameObject);
                    break;
                
                case "KeyDown" when data != null:
                    string actionCode = data.ToString();
                    if (actionCode == "Drop") DropFromHand();
                    break;
            }
        }

        public override void Update()
        {
            
        }

        public void Equip(GameObject gameObjectInst)
        {
            Debug.Log($"вЗЯЛ {gameObjectInst.name}");
            
            if (_equippedGameObject != null)
            {
                Debug.LogWarning("Попытка взять предмет не удалась, т.к. уже есть объект в руке!");
                
                //TODO Дописать логику проброса объекта из руки в инвентарь и взятии нового предмета по требованию.
                return;
            }

            _equippedGameObject = Object.Instantiate(gameObjectInst, _handPoint);
            _equippedGameObject.transform.localPosition = Vector3.zero;
            _equippedGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            
            GameObject.Destroy(gameObjectInst);
            _equippedGameObject.SetActive(true);
        }

        public void DropFromHand()
        {
            if (_equippedGameObject == null) return;

            Rigidbody objRB =_equippedGameObject.GetComponent<Rigidbody>();
            objRB.isKinematic = false;
            objRB.useGravity = true;
            objRB.transform.parent = null;//теперь не дочка объекта руки

            if (_equippedGameObject.TryGetComponent(out IDrop dropableObj))
            {
                dropableObj.Drop();
            }

            _equippedGameObject = null;
        }
    }
}
