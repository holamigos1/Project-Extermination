using GameAnimation.Data;
using GameAnimation.Sheets;
using UnityEngine;

namespace Characters.Humanoid.Base
{
    //Триггеры не в его зоне ответственности
    public class HumanoidBodyParameters
    {
        public HumanoidBodyParameters(Animator humanAnimator, HumanAnimatorSheet humanAnimatorSheet)
        {
            HumanAnimatorSheet = humanAnimatorSheet;
            _humanAnimator = humanAnimator;
            
            if (_humanAnimator.isHuman == false)
                Debug.LogError("Ты название класса видишь? Человеческий аватар в аниматоре поставил?");
        }

        public Vector3 MovementAcceleration => _movementAcceleration;
        public MovementType CurrentMovementType { get; set; } = MovementType.Walking;

        public readonly HumanAnimatorSheet HumanAnimatorSheet;
        private readonly Animator _humanAnimator;
        
        //TODO Придумай как избавится от магических чисел
        private const float MOVEMENT_SMOOTH_TIME = 0.15f; 
        
        public void FrameTick() //Кто салоцировал тот и апдейтит
        {
            _humanAnimator.SetFloat(HumanAnimatorSheet.ForwardMovement.Hash, 
                Mathf.SmoothDamp(ForwardDeltaParameter, _speededMovementParametersGoal.y, ref _movementAcceleration.z, MOVEMENT_SMOOTH_TIME));

            _humanAnimator.SetFloat(HumanAnimatorSheet.SideMovement.Hash, 
                Mathf.SmoothDamp(SideDeltaParameter, _speededMovementParametersGoal.x, ref _movementAcceleration.x, MOVEMENT_SMOOTH_TIME));
        }

        private Vector2 _speededMovementParametersGoal => _movementParametersGoal * (int)CurrentMovementType;
        private Vector2 _movementParametersGoal;
        private Vector3 _movementAcceleration;
        private Vector3 _rotationAcceleration;

        public float ForwardDeltaParameter
        {
            get => _humanAnimator.GetFloat(HumanAnimatorSheet.ForwardMovement.Hash);
            set => _movementParametersGoal.y = value;
        }
        
        public float SideDeltaParameter
        {
            get => _humanAnimator.GetFloat(HumanAnimatorSheet.SideMovement.Hash);
            set => _movementParametersGoal.x = value;
        }
        
        public float RotationDeltaParameter
        {
            get => _humanAnimator.GetFloat(HumanAnimatorSheet.RotationMovement.Hash);
            set => _rotationAcceleration.x = value;
        }
        
        public bool IsAiming
        {
            get => _humanAnimator.GetBool(HumanAnimatorSheet.IsAimBool.Hash);
            set => _humanAnimator.SetBool(HumanAnimatorSheet.IsAimBool.Hash, value);
        }
        
        public bool IsHidingWeapon
        {
            get => _humanAnimator.GetBool(HumanAnimatorSheet.IsHidingWeapon.Hash);
            set => _humanAnimator.SetBool(HumanAnimatorSheet.IsHidingWeapon.Hash, value);
        }
        
        public WeaponType EquippedWeaponParameter
        {
            get => (WeaponType)_humanAnimator.GetInteger(HumanAnimatorSheet._equippedWeaponTypeIntInt.Hash);
            set => _humanAnimator.SetInteger(HumanAnimatorSheet._equippedWeaponTypeIntInt.Hash,(int)value);
        }
        
        public ItemType EquippedItemParameter
        {
            get => (ItemType)_humanAnimator.GetInteger(HumanAnimatorSheet.EquippedItemTypeInt.Hash);
            set => _humanAnimator.SetInteger(HumanAnimatorSheet.EquippedItemTypeInt.Hash,(int)value);
        }
        
        public InteractionType InteractionTypeParameter
        {
            get => (InteractionType)_humanAnimator.GetInteger(HumanAnimatorSheet.InteractionTypeInt.Hash);
            set => _humanAnimator.SetInteger(HumanAnimatorSheet.InteractionTypeInt.Hash,(int)value);
        }
        
        public enum MovementType 
        {
            Walking = 1,
            //TODO Joge = 2, 
            Sprinting = 2
        }
    }
}