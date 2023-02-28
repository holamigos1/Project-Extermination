using System;
using GameData.AnimationTags;
using GameData.Weapons;
using UnityEngine;
using Weapons.Basic;

namespace Weapons
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
        
        //Кусок дерьма
        protected Action AttackButtonPressed = delegate { };
        protected Action AttackButtonRelised = delegate { };
        protected Action AttackButtonClicked = delegate { };
        protected Action ReloadButtonDown = delegate { };
        protected Action AltAttackMouseDown = delegate { };
        protected Action AltAttackMouseUp = delegate { };
        protected Action SwitchModeButtonDown = delegate { };
        //
        
        protected Action UpdateAction = delegate { };

        public void Update() //Нада перекодить под перебинд в будущем!!
        {
            if (Input.GetMouseButtonDown(0)) AttackButtonPressed();
            if (Input.GetMouseButton(0)) AttackButtonClicked();
            if (Input.GetMouseButtonUp(0)) AttackButtonRelised();
            if (Input.GetMouseButtonDown(1)) AltAttackMouseDown();
            if (Input.GetMouseButtonUp(1)) AltAttackMouseUp();
            if (Input.GetKeyDown(KeyCode.R)) ReloadButtonDown();
            if (Input.GetKeyDown(KeyCode.F)) SwitchModeButtonDown();
            UpdateAction();
        }

        public bool isReady { get; set; }

        public Animator thisAnimator => animator;

        public virtual void Reload()
        {
            animator.SetTrigger(AnimationParams.RELOAD_TRIGGER);
        }

        public bool isCharged { get; set; }

        public float shootDealy { get; set; } = 1;

        public float recoilForce { get; set; } = 1;

        public virtual void PullTrigger()
        {
            
        }
        
        public virtual void Shoot()
        {
            if (isReady) animator.SetTrigger(AnimationParams.SHOOT_TRIGGER);
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
            animator.SetTrigger(AnimationParams.HIDE);
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