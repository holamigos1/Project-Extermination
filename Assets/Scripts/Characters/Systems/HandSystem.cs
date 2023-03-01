using GameData.Layers;
using GameSystems.Base;
using Objects.Base;
using UnityEngine;

namespace Characters.Systems
{
    /// <summary>
    /// Система управления поведенем рук персонажей 
    /// </summary>
    public class HandSystem : GameSystem
    {
        public Item EquippedItem => _equippedItem;
        
        public HandSystem(Transform handPoint)
        {
            _handPoint = handPoint;
            _handLocalStartPoint = _handPoint.localPosition;
        }
        
        private readonly Vector3 _handLocalStartPoint;
        
        private Item _equippedItem;
        private Transform _handPoint;

        public override void Start() 
        {
            base.Start();
            if (_handPoint.GetFirstChildObj().TryGetComponent(out Item itemIns)) 
                Equip(itemIns);
        }

        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "Object pickuped" when data != null:
                    Equip(data as Item);
                    break;
                
                case "KeyDown" when data != null:
                    if (data.ToString() == "Drop") DropFromHand();
                    if (data.ToString() == "Interact") Grub();
                    break;
            }
        }

        private async void Grub()
        {
            var requestResponse = 
                await SystemsСontainer.MakeAsyncRequest("Get raycast object")!;
    
            if (requestResponse.IsEmpty()) return;
            
            GameObject requestObj = requestResponse.GetFirstAs<GameObject>();
            
            if (requestObj.TryGetComponent(out Item itemObj) is false) return;
            
            if (itemObj.PickUpType == PickUpType.InHand)
            {
                if (_equippedItem == null) 
                    Equip(itemObj.Pickup());
                else Debug.LogWarning("Попытка взять предмет не удалась, т.к. уже есть объект в руке!");
                    
                //TODO Логика перемещения в инвентарь
            }
            
            if (itemObj.PickUpType == PickUpType.InInventory)
            {
                //TODO Логика перемещения в инвентарь
            }
        }

        private void Equip(Item itemInst)
        {
            if (_equippedItem != null)
            {
                Debug.LogWarning("Попытка взять предмет не удалась, т.к. уже есть объект в руке!");
                //TODO Дописать логику проброса объекта из руки в инвентарь и взятии нового предмета по требованию.
                return;
            }
            
            Debug.Log($"Взял предмет {itemInst.name} с типом {itemInst.GetType().Name}");
            
            _equippedItem = itemInst;
            
            _equippedItem.ItemTransform.parent = _handPoint;
            _equippedItem.ItemTransform.localPosition = Vector3.zero;
            _equippedItem.ItemTransform.localRotation = Quaternion.Euler(Vector3.zero);
            
            _equippedItem.ItemGameObject.ChangeGameObjsLayers(GameLayers.FIRST_PERSON_LAYER);
            _equippedItem.ItemGameObject.SetActive(true);
            
            _handPoint.localPosition = new Vector3(_handPoint.localPosition.x,
                                                    - _equippedItem.ItemTransform.RenderBounds().extents.y,
                                                    _handPoint.localPosition.z);
            
            SystemsСontainer.NotifySystems("Item Equipped", _equippedItem);
        }

        private void DropFromHand()
        {
            if (_equippedItem == null) return;

            _equippedItem.ItemRigidbody.isKinematic = false;
            _equippedItem.ItemRigidbody.useGravity = true;
            _equippedItem.ItemRigidbody.transform.parent = null;//теперь не дочка объекта руки

            if (_equippedItem.TryGetComponent(out IDrop dropableObj))
                dropableObj.Drop();
            
            SystemsСontainer.NotifySystems("Item Dropped", _equippedItem);

            _handPoint.localPosition = _handLocalStartPoint;
            _equippedItem = null;
        }

        private void ChangeHandPivot(Vector3 position)
        {
            _handPoint.transform.position = position;
        }
    }
}
