using GameSystems.Base;
using Objects.Base;
using Weapons;

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
                    HandleEquippedItem(data as Item);
                    break;
                
                case "Item Dropped" when data != null:
                    HandleDropItem(data as Item);
                    break;
                
                case "KeyDown" when data != null:
                    if (data.ToString() == "Fire") Fire();
                    if (data.ToString() == "Aim") Aim();
                    break;
            }
        }

        private void HandleDropItem(Item item)
        {
            if(item.TryGetComponent(out Weapon weaponItem) is false) return;
            //TODO Отсюда можно пробросить запуск анимации выкидывания пушки
            if(_weaponInHand == weaponItem) _weaponInHand = null;
        }
        
        private void HandleEquippedItem(Item item)
        {
            if (item.TryGetComponent(out _weaponInHand) is false) return;
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