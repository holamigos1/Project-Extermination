using System;
using GameObjects.Base;
using GameSystems.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Characters.Systems
{
    [Serializable]
    public class WeaponMediatorSystem : GameSystem
    {
        [Title("Обработчик оружия.", 
            "Отвечает за логику экипированного оружия.")]
        [ShowInInspector] [HideLabel] [DisplayAsString][PropertySpace(SpaceBefore = -5,SpaceAfter = -20)]
        #pragma warning disable CS0219, CS0414
        private string info = "";
        
        [SerializeField] [LabelText("Владелец оружия")]
        private Unit _ownerReference;
        private Weapon _weaponInHand;

        public override void Start()
        {
            SystemsСontainer.Notify += OnNotify;
        }
        
        public override void Stop()
        {
            SystemsСontainer.Notify -= OnNotify;
        }

        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "Item Equipped" when data != null:
                    HandleEquippedItem(data as GameItem);
                    break;
                
                case "Item Dropped" when data != null:
                    HandleDropItem(data as GameItem);
                    break;
                
                case "KeyDown" when data != null:
                    if (data.ToString() == "Fire") Fire();
                    if (data.ToString() == "Aim") Aim();
                    break;
            }
        }

        private void HandleDropItem(GameItem item)
        {
            if(item.TryGetComponent(out Weapon weaponItem) is false) return;
            //TODO Отсюда можно пробросить запуск анимации выкидывания пушки
            if(_weaponInHand == weaponItem) _weaponInHand = null;
        }
        
        private void HandleEquippedItem(GameItem item)
        {
            if (item.TryGetComponent(out _weaponInHand) is false) return;
            _weaponInHand.SetOwner(_ownerReference);
            _weaponInHand.Equip();
        }

        private void Fire()
        {
            _weaponInHand?.PlayFireAction();
        }

        private void Aim()
        {
            _weaponInHand?.PlayAimAction();
        }
    }
}