using GameData.Weapons;
using UnityEngine;

namespace Weapons.Basic
{
    public interface IReloadable
    {
        void Reload();
    }

    public interface IShootable
    {
        bool isCharged { get; set; }
        float shootDealy { get; set; }
        float recoilForce { get; set; }
        void Shoot();
        void CreateBullet();
    }

    public interface IWeapon
    {

        void Show();
        void Hide();
    }

    public interface IMelleWeapon
    {
        void Attack();
    }

    public interface ISwitchMode
    {
        public WeaponMode WeaponMode { get; }
        void SwitchMode(WeaponMode mode);
    }

    public interface IAnimaiable
    {
        public bool isReady { get; set; }
        public Animator thisAnimator { get; }
    }

    public interface IAmmoBasedWeapon
    {
        public float CurrentAmmo { get; set; }
        public float MaximumAmmo { get; set; }
    }
}