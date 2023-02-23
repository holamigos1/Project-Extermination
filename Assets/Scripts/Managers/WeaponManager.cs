using System;
using System.Collections.Generic;
using Scripts.GameEnums;
using Scripts.Weapons;
using UnityEngine;

namespace Scripts.GameEnums
{
    public enum WeaponType
    {
        none,
        CROWBAR,
        PISTOL,
        SHOTGUN,
        OPS,
        RIFLE
    }
}

namespace Scripts.Managers
{
    /// <summary>
    ///     выступает в роли инвентаря для оружия и определяет что сейчас выбрано из оружия и в каком состоянии оно находится
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        public List<GameObject> availableWeapons;
        [SerializeField] private Transform WeaponsRoot;
        [SerializeField] private Transform IventoryRoot;
        public IWeapon Current_picked_weapon;

        public void Start()
        {
            try
            {
                Current_picked_weapon = WeaponsRoot.GetChild(0).gameObject.GetComponent<IWeapon>();
                AddWeapon(WeaponsRoot.GetChild(0).gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) PickUpSelectedWeapon(WeaponType.CROWBAR);
            if (Input.GetKeyDown(KeyCode.Alpha2)) PickUpSelectedWeapon(WeaponType.PISTOL);
            if (Input.GetKeyDown(KeyCode.Alpha3)) PickUpSelectedWeapon(WeaponType.SHOTGUN);
            if (Input.GetKeyDown(KeyCode.Alpha4)) PickUpSelectedWeapon(WeaponType.OPS);
        }


        public void AddWeapon(GameObject weapon)
        {
            if (availableWeapons.Contains(weapon))
                //add ammo
                return;

            availableWeapons.Add(Instantiate(weapon, IventoryRoot));
        }

        private void PickUpSelectedWeapon(WeaponType weaponType)
        {
            if (Current_picked_weapon != null && Current_picked_weapon._weaponType == weaponType)
            {
                Current_picked_weapon.Hide();
                Current_picked_weapon = null;
                return;
            }

            foreach (var weapon in availableWeapons)
                if (weapon.GetComponent<IWeapon>()._weaponType == weaponType)
                {
                    weapon.SetActive(true);
                    Current_picked_weapon = weapon.GetComponent<IWeapon>();
                }
        }
    }
}