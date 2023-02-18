using Objects.Base;
using Systems.Base;
using UnityEngine;

namespace Characters.Systems
{
    public class HandSystem : GameSystem
    {
        public GameObject EquippedGameObject => _equippedGameObject;
        
        /// <summary>
        /// Система управления поведения рук персонажей 
        /// </summary>
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
                    if (data.ToString() == "Drop") DropFromHand();
                    if (data.ToString() == "Interact") Grub();
                    break;
            }
        }

        public override void Update()
        {
            
        }
        
        private async void Grub()
        {
            var requestResponse = await SystemsСontainer.MakeAsyncRequest("Get raycast object", null)!;
            if (requestResponse.GetFirstAs<GameObject>() == null) return;
            if (!requestResponse.GetFirstAs<GameObject>().TryGetComponent(out IPickup pickupObject)) return;
            
            if (pickupObject.PickUpType == PickUpType.InHand)
            {
                if (_equippedGameObject == null)
                {
                    GameObject gameObject = pickupObject.Pickup();
                    Equip(gameObject); 
                }
                else
                {
                    Debug.LogWarning("Попытка взять предмет не удалась, т.к. уже есть объект в руке!");
                    //TODO Логика перемещения в инвентарь
                }
            }
            
            if (pickupObject.PickUpType == PickUpType.InInventory)
            {
                //TODO Логика перемещения в инвентарь
            }
        }

        private void Equip(GameObject gameObjectInst)
        {
            Debug.Log($"вЗЯЛ {gameObjectInst.name}");
            
            if (_equippedGameObject != null)
            {
                Debug.LogWarning("Попытка взять предмет не удалась, т.к. уже есть объект в руке!");
                
                //TODO Дописать логику проброса объекта из руки в инвентарь и взятии нового предмета по требованию.
                return;
            }

            _equippedGameObject = Object.Instantiate(gameObjectInst, _handPoint);
            _equippedGameObject.name = gameObjectInst.name;
            _equippedGameObject.transform.localPosition = Vector3.zero;
            _equippedGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _equippedGameObject.ChangeFamilyLayers(LayerMask.NameToLayer(Data.Layers.GameLayers.FIRST_PERSON_LAYER));
            
            GameObject.Destroy(gameObjectInst);
            _equippedGameObject.SetActive(true);
        }

        private void DropFromHand()
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
