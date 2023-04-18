using GameAnimation.Data;
using GameAnimation.Sheets.Base;
using UnityEngine;

namespace GameAnimation.Sheets
{
    [CreateAssetMenu(fileName = "Human Animator Sheet", menuName = "Game Data/Animator Sheets/Human", order = 1)]
    public class HumanParametersSheet : AnimatorParametersSheet
    {
        public AnimationControllerParameter SideMovement => _sideMovement;
        public AnimationControllerParameter ForwardMovement => _forwardMovement;
        public AnimationControllerParameter RotationMovement => _rotationMovement;
        public AnimationControllerParameter _equippedWeaponTypeIntInt => _equippedWeaponTypeInt;
        public AnimationControllerParameter EquippedItemTypeInt => _equippedItemTypeInt;
        public AnimationControllerParameter InteractionTypeInt => _interactionTypeInt;
        public AnimationControllerParameter EquipTrigger => _equipTrigger;
        public AnimationControllerParameter AttackTrigger => _attackTrigger;
        public AnimationControllerParameter ReloadTrigger => _reloadTrigger;
        public AnimationControllerParameter IsHidingWeapon => _isHidingWeapon;
        public AnimationControllerParameter IsAimBool => _isAimBool;
        
        [SerializeField] private AnimationControllerParameter _sideMovement;
        [SerializeField] private AnimationControllerParameter _forwardMovement;
        [SerializeField] private AnimationControllerParameter _rotationMovement;
        [SerializeField] private AnimationControllerParameter _equippedWeaponTypeInt;
        [SerializeField] private AnimationControllerParameter _equippedItemTypeInt;
        [SerializeField] private AnimationControllerParameter _interactionTypeInt;
        [SerializeField] private AnimationControllerParameter _equipTrigger;
        [SerializeField] private AnimationControllerParameter _attackTrigger;
        [SerializeField] private AnimationControllerParameter _isHidingWeapon;
        [SerializeField] private AnimationControllerParameter _isAimBool;
        [SerializeField] private AnimationControllerParameter _reloadTrigger;
    }
}