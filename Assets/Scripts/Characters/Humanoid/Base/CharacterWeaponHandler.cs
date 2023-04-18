using GameAnimation.Data;
using GameAnimation.StateMachineBehaviours;
using UnityEngine;

namespace Characters.Humanoid.Base
{
    public abstract class CharacterWeaponHandler
    {
        public CharacterWeaponHandler(Animator animator, HumanoidBodyParameters bodyParameters, WeaponType type)
        {
            BodyParameters = bodyParameters;
            Animator = animator;
            Type = type;

            IsReady = false; //TODO УБЕРИ БЛ
            
            foreach (var hideBeh in animator.GetBehaviours<HideItemStateBehaviour>())
                if (hideBeh.WeaponType == type)
                    _hideBeh = hideBeh;
            
            foreach (var equipBeh in animator.GetBehaviours<EquipItemStateBehaviour>())
                if (equipBeh.WeaponType == type)
                    _equipBeh = equipBeh;
            
            _hideBeh.OnStateEntering += OnHide;
            _hideBeh.OnStateExiting += OnHided;
            _equipBeh.OnStateEntering += OnEquip;
            _equipBeh.OnStateExiting += OnEquiped;
        }
        
        ~CharacterWeaponHandler()
        {
            Debug.Log("ПАКА");
            BodyParameters.EquippedWeaponParameter = WeaponType.None;
            _hideBeh.OnStateEntering -= OnHide;
            _hideBeh.OnStateExiting -= OnHided;
            _equipBeh.OnStateEntering -= OnEquip;
            _equipBeh.OnStateExiting -= OnEquiped;
        }

        public WeaponType WeaponAnimationType => Type;
        public bool IsHidingWeapon { get; protected set; }
        public bool IsWeaponEquip { get; protected set; }
        public bool IsAiming { get; protected set; }
        public bool IsReady { get; protected set; }
        
        protected readonly HumanoidBodyParameters BodyParameters;
        protected readonly Animator Animator;
        protected readonly WeaponType Type;
        
        private readonly HideItemStateBehaviour _hideBeh;
        private readonly EquipItemStateBehaviour _equipBeh;
        
        public void EquipWeapon()
        {
            Debug.Log("SDD");
            Animator.SetTrigger(BodyParameters.HumanAnimatorSheet.EquipTrigger.Hash);
            BodyParameters.EquippedWeaponParameter = Type;
            BodyParameters.EquippedItemParameter = ItemType.Weapon;
        }
        
        public void HideWeapon()
        {
            if(IsReady == false) return;
            
            BodyParameters.IsHidingWeapon = true;
        }
        
        public void AttackWeapon()
        {
            Animator.SetTrigger(BodyParameters.HumanAnimatorSheet.AttackTrigger.Hash);
        }
        
        public void AimWeapon(bool isAim)
        {
            BodyParameters.IsAiming = isAim;
        }
        
        public void ReloadWeapon()
        {
            Animator.SetTrigger(BodyParameters.HumanAnimatorSheet.ReloadTrigger.Hash);
        }
        
        public void ThrowWeapon()
        {

        }
        
        void OnHide()
        {
            IsReady = false;
            IsHidingWeapon = true;
        }
        
        void OnHided()
        {
            //TODO Код прятанья оружия
            
            IsHidingWeapon = false;
            IsWeaponEquip = false;
            BodyParameters.IsHidingWeapon = false;
            BodyParameters.EquippedItemParameter = ItemType.None;
            BodyParameters.EquippedWeaponParameter = WeaponType.None;
        }
        
        void OnEquip()
        {
            Debug.Log("DSADSA");
            IsWeaponEquip = false;
        }
        
        void OnEquiped()
        {
            Debug.Log("!!!!");
            IsWeaponEquip = true;
            IsReady = true;
        }
    }
}