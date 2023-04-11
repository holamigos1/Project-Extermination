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
        public AnimationControllerParameter EquippedWeaponType => _equippedWeaponType;
        public AnimationControllerParameter EquippedItemType => _equippedItemType;
        public AnimationControllerParameter InteractionType => _interactionType;
        public AnimationControllerParameter EquipTrigger => _equipTrigger;
        public AnimationControllerParameter PickUpTrigger => _pickUpTrigger;
        
        [SerializeField] private AnimationControllerParameter _sideMovement;
        [SerializeField] private AnimationControllerParameter _forwardMovement;
        [SerializeField] private AnimationControllerParameter _rotationMovement;
        [SerializeField] private AnimationControllerParameter _equippedWeaponType;
        [SerializeField] private AnimationControllerParameter _equippedItemType;
        [SerializeField] private AnimationControllerParameter _interactionType;
        [SerializeField] private AnimationControllerParameter _equipTrigger;
        [SerializeField] private AnimationControllerParameter _pickUpTrigger;
    }
}