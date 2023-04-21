using GameAnimation.Data;
using GameAnimation.Sheets.Base;
using UnityEngine;
using AnimatorControllerParameter = GameAnimation.Data.AnimatorControllerParameter;

namespace GameAnimation.Sheets
{
    [CreateAssetMenu(fileName = "Human Animator Sheet", menuName = "Scriptable Data/Animator Sheets/Human", order = 1)]
    public class HumanAnimatorSheet : AnimatorParametersSheet
    {
        public AnimatorControllerParameter SideMovement => _sideMovement;
        public AnimatorControllerParameter ForwardMovement => _forwardMovement;
        public AnimatorControllerParameter RotationMovement => _rotationMovement;
        public AnimatorControllerParameter _equippedWeaponTypeIntInt => _equippedWeaponTypeInt;
        public AnimatorControllerParameter EquippedItemTypeInt => _equippedItemTypeInt;
        public AnimatorControllerParameter InteractionTypeInt => _interactionTypeInt;
        public AnimatorControllerParameter EquipTrigger => _equipTrigger;
        public AnimatorControllerParameter AttackTrigger => _attackTrigger;
        public AnimatorControllerParameter ReloadTrigger => _reloadTrigger;
        public AnimatorControllerParameter IsHidingWeapon => _isHidingWeapon;
        public AnimatorControllerParameter IsAimBool => _isAimBool;
        
        [SerializeField] private AnimatorControllerParameter _sideMovement;
        [SerializeField] private AnimatorControllerParameter _forwardMovement;
        [SerializeField] private AnimatorControllerParameter _rotationMovement;
        [SerializeField] private AnimatorControllerParameter _equippedWeaponTypeInt;
        [SerializeField] private AnimatorControllerParameter _equippedItemTypeInt;
        [SerializeField] private AnimatorControllerParameter _interactionTypeInt;
        [SerializeField] private AnimatorControllerParameter _equipTrigger;
        [SerializeField] private AnimatorControllerParameter _attackTrigger;
        [SerializeField] private AnimatorControllerParameter _isHidingWeapon;
        [SerializeField] private AnimatorControllerParameter _isAimBool;
        [SerializeField] private AnimatorControllerParameter _reloadTrigger;
    }
}