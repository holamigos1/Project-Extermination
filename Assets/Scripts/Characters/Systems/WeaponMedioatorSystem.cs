using Systems.Base;
using UnityEngine;
using Weapons.Basic;

namespace Characters.Systems
{
    public class WeaponMediatorSystem : GameSystem
    {
        private Weapon _weaponInHand;
        
        public override void OnNotify(string message, object data)
        {
            switch (message)
            {
                case "Item Equipped" when data != null:
                    HandleEquippedItem(data as GameObject);
                    break;
                
                case "Item Dropped" when data != null:
                    HandleDropItem(data as GameObject);
                    break;
                
                case "KeyDown" when data != null:
                    if (data.ToString() == "Fire") Fire();
                    if (data.ToString() == "Aim") Aim();
                    break;
            }
        }


        private void HandleDropItem(GameObject item)
        {
            if(item.TryGetComponent(out Weapon weaponItem) is false) return;
            //TODO Отсюда можно пробросить запуск анимации выкидывания пушки
            if(_weaponInHand == weaponItem) _weaponInHand = null;
        }
        
        private void HandleEquippedItem(GameObject item)
        {
            if(item.TryGetComponent(out Weapon weaponObj) is false) return;
            _weaponInHand = weaponObj;
            _weaponInHand.Equip();
        }

        private void Fire()
        {
            if(_weaponInHand == null) return;
            _weaponInHand.PlayFireAction();
        }

        private void Aim()
        {
            if(_weaponInHand == null) return;
            _weaponInHand.PlayAimAction();
        }
    }
}