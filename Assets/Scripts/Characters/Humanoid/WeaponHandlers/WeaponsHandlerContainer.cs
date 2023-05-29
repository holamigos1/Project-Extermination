using System.Collections.Generic;
using Characters.Humanoid.Base;
using GameItems.Base;
using UnityEngine;
using Weapons;
using Weapons.Range;

namespace Characters.Humanoid.WeaponHandlers
{
    public class WeaponsHandlerContainer
    {
        public WeaponsHandlerContainer(Animator animator, HumanoidBodyParameters parameters)
        {
            _parametersRef = parameters;
            
            _weaponsHandlers = new Dictionary<int, CharacterWeaponHandler>(100)
            {
                //TODO Избегать повторения названий оружия
                { nameof(Pistol).GetHashCode(), new PistolAnimationHandler(animator, parameters) }
            };
        }
        
        public CharacterWeaponHandler CurrentWeaponHandler { get; private set; }
        public Weapon CurrentWeapon { get; private set; }

        private readonly Dictionary<int, CharacterWeaponHandler> _weaponsHandlers;
        private HumanoidBodyParameters _parametersRef;

        public CharacterWeaponHandler TakeWeaponHandler(Weapon weapon)
        {
            CurrentWeaponHandler = _weaponsHandlers[weapon.GetType().Name.GetHashCode()];
            CurrentWeapon = weapon;
            return CurrentWeaponHandler;
        }

        public bool IsMatchToCurrentEquipedWeapon(GameItem item)
        {
            if (item is Weapon weapon)
               return weapon == CurrentWeapon;

            return false;
        }
        
        public bool IsWeapon(GameItem item)
        {
            if (item is Weapon) return true;
            return false;
        }
    }
}