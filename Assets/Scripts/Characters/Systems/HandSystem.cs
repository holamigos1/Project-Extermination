using System;
using GameData.Layers;
using GameExtensions;
using GameSystems.Base;
using Objects.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Systems
{
    /// <summary>
    /// Система управления поведенем рук персонажей 
    /// </summary>
    [Serializable]
    public class HandSystem : GameSystem
    {
        [Title("Обработчик руки персонажа.", 
            "Руководит поведением предметов в руке.")]
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219
        private string info = "";
        
        [ShowInInspector] [LabelText("Подобранный предмет")]
        public Item EquippedItem => _equippedItem;

        public HandSystem() { }
        
        public HandSystem(Transform handTransform)
        {
            _handTransform = handTransform;
            _handLocalStartPoint = _handTransform.localPosition;
        }
        
        private Vector3 _handLocalStartPoint;
        
        [SerializeField] [Required] [LabelText("Объект руки")]
        private Transform _handTransform;
        private Item _equippedItem;
        

        public override void Start() 
        {
            SystemsСontainer.Notify += OnNotify;
            
            _handLocalStartPoint = _handTransform.localPosition;
            if (_handTransform.HasChild() == false) return;
            if (_handTransform.GetFirstChildObj().TryGetComponent(out Item itemIns)) 
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
            
            if (_equippedItem == null) 
                Equip(itemObj.Pickup());
            else Debug.LogWarning("Попытка взять предмет не удалась, т.к. уже есть объект в руке!");
        }

        private void Equip(Item itemInst)
        {
            if (_equippedItem != null) return;

            _equippedItem = itemInst;
            
            _equippedItem.ItemTransform.parent = _handTransform;
            _equippedItem.ItemTransform.localPosition = Vector3.zero;
            _equippedItem.ItemTransform.localRotation = Quaternion.Euler(Vector3.zero);
            
            _equippedItem.ItemGameObject.ChangeGameObjsLayers(GameLayers.FIRST_PERSON_LAYER);
            _equippedItem.ItemGameObject.SetActive(true);
            
            _handTransform.localPosition = new Vector3(_handTransform.localPosition.x,
                                                    - _equippedItem.ItemTransform.RenderBounds().extents.y,
                                                    _handTransform.localPosition.z);
            
            SystemsСontainer.NotifySystems("Item Equipped", _equippedItem);
        }

        private void DropFromHand()
        {
            if (_equippedItem == null) return;

            _equippedItem.ItemRigidbody.isKinematic = false;
            _equippedItem.ItemRigidbody.useGravity = true;
            
            _equippedItem.ItemTransform.parent = null;

            if (_equippedItem.TryGetComponent(out IDrop dropableObj))
                dropableObj.Drop();
            
            SystemsСontainer.NotifySystems("Item Dropped", _equippedItem);

            _handTransform.localPosition = _handLocalStartPoint;
            _equippedItem = null;
        }

        private void ChangeHandPivot(Vector3 position)
        {
            _handTransform.transform.position = position;
        }
    }
}
