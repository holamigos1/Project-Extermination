using System;
using Data.AnimationTags;
using Data.Weapons;
using UnityEngine;

namespace Weapons.Basic
{
    public abstract class RangeWeapon : MonoBehaviour, IShootable,
        IReloadable, ISwitchMode, IAnimaiable, IWeapon
    {
        [SerializeField] private AudioSource ShootSound;
        [SerializeField] private AudioSource ReloadSound;
        [SerializeField] private AudioSource SwitchModeSound;
        [SerializeField] private AudioSource PickUpSound;
        public WeaponMode weaponMode;
        public Animator animator;
        protected Action LeftMouseDown = delegate { };
        protected Action LeftMouseNotPressed = delegate { };
        protected Action LeftMousePressed = delegate { };
        protected Action LeftMouseUp = delegate { };
        protected Action ReloadButtonDown = delegate { };
        protected Action RightMouseDown = delegate { };
        protected Action RightMouseUp = delegate { };
        protected Action SwitchModeButtonDown = delegate { };
        protected Action UpdateAction = delegate { };

        public void Update() //Нада перекодить под перебинд в будущем!!
        {
            if (Input.GetMouseButtonDown(0)) LeftMouseDown();
            if (Input.GetMouseButton(0)) LeftMousePressed();
            if (Input.GetMouseButtonUp(0))
            {
                LeftMouseNotPressed();
                LeftMouseUp();
            }

            if (Input.GetMouseButtonDown(1)) RightMouseDown();
            if (Input.GetMouseButtonUp(1)) RightMouseUp();
            if (Input.GetKeyDown(KeyCode.R)) ReloadButtonDown();
            if (Input.GetKeyDown(KeyCode.F)) SwitchModeButtonDown();
            UpdateAction();
        }

        public bool isReady { get; set; }

        public Animator thisAnimator => animator;

        public virtual void Reload()
        {
            animator.SetTrigger(AnimationTags.RELOAD_TRIGGER);
        }

        public bool isCharged { get; set; }

        public float shootDealy { get; set; } = 1;

        public float recoilForce { get; set; } = 1;

        public virtual void Shoot()
        {
            if (isReady) animator.SetTrigger(AnimationTags.SHOOT_TRIGGER);
        }

        public virtual void CreateBullet()
        {
        }

        public WeaponMode WeaponMode => weaponMode;

        public virtual void SwitchMode(WeaponMode mode)
        {
            weaponMode = mode;
            //animator.SetTrigger(AnimationTags.SWITCH_MODE_TRIGGER);
        }



        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
            animator.SetTrigger(AnimationTags.HIDE_TRIGGER);
        }

        public void DisableThis()
        {
            gameObject.SetActive(false);
        }

        public void EnableThis()
        {
            gameObject.SetActive(true);
        }

        public void Set_NotReady()
        {
            isReady = true;
        }

        public void Set_Ready()
        {
            isReady = false;
        }

        public void PlaySound_Shoot()
        {
            if (ShootSound != null) ShootSound.Play();
        }

        public void PlaySound_Reload()
        {
            if (ReloadSound != null) ReloadSound.Play();
        }

        public void PlaySound_SwitchMode()
        {
            if (SwitchModeSound != null) SwitchModeSound.Play();
        }

        public void PlaySound_PickUp()
        {
            if (PickUpSound != null) PickUpSound.Play();
        }
    }
}