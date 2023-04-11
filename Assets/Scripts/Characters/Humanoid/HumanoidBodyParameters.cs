using GameAnimation.Data;
using GameAnimation.Sheets;
using UnityEngine;

namespace Characters.Humanoid
{
    //Триггеры не в его зоне ответственности
    public class HumanoidBodyParameters
    {
        public HumanoidBodyParameters(Animator humanAnimator, HumanParametersSheet humanAnimatorSheet)
        {
            _humanAnimatorSheet = humanAnimatorSheet;
            _humanAnimator = humanAnimator;
            
            if (_humanAnimator.isHuman == false)
                Debug.LogError("Ты название класса видишь? Человеческий аватар в аниматоре поставил?");
        }

        public Vector3 MovementAcceleration => _movementAcceleration;
        public MovementType CurrentMovementType { get; set; } = MovementType.Walking;

        private readonly HumanParametersSheet _humanAnimatorSheet;
        private readonly Animator _humanAnimator;
        
        //TODO Придумай как избавится от магических чисел
        private const float MOVEMENT_SMOOTH_TIME = 0.15f; 
        
        public void FrameTick() //Кто салоцировал тот и апдейтит
        {
            _humanAnimator.SetFloat(_humanAnimatorSheet.ForwardMovement.Hash, 
                Mathf.SmoothDamp(ForwardDeltaParameter, _speededMovementParametersGoal.y, ref _movementAcceleration.z, MOVEMENT_SMOOTH_TIME));

            _humanAnimator.SetFloat(_humanAnimatorSheet.SideMovement.Hash, 
                Mathf.SmoothDamp(SideDeltaParameter, _speededMovementParametersGoal.x, ref _movementAcceleration.x, MOVEMENT_SMOOTH_TIME));
        }

        private Vector2 _speededMovementParametersGoal => _movementParametersGoal * (int)CurrentMovementType;
        private Vector2 _movementParametersGoal;
        private Vector3 _movementAcceleration;

        public float ForwardDeltaParameter
        {
            get => _humanAnimator.GetFloat(_humanAnimatorSheet.ForwardMovement.Hash);
            set => _movementParametersGoal.y = value;
        }
        
        public float SideDeltaParameter
        {
            get => _humanAnimator.GetFloat(_humanAnimatorSheet.SideMovement.Hash);
            set => _movementParametersGoal.x = value;
        }
        
        public float RotationDeltaParameter
        {
            get => _humanAnimator.GetFloat(_humanAnimatorSheet.SideMovement.Hash);
            set => _movementParametersGoal.x = value;
        }
        
        public WeaponType EquippedWeaponParameter
        {
            get => (WeaponType)_humanAnimator.GetInteger(_humanAnimatorSheet.EquippedWeaponType.Hash);
            set => _humanAnimator.SetInteger(_humanAnimatorSheet.EquippedWeaponType.Hash,(int)value);
        }
        
        public ItemType EquippedItemParameter
        {
            get => (ItemType)_humanAnimator.GetInteger(_humanAnimatorSheet.EquippedItemType.Hash);
            set => _humanAnimator.SetInteger(_humanAnimatorSheet.EquippedItemType.Hash,(int)value);
        }
        
        public InteractionType InteractionTypeParameter
        {
            get => (InteractionType)_humanAnimator.GetInteger(_humanAnimatorSheet.InteractionType.Hash);
            set => _humanAnimator.SetInteger(_humanAnimatorSheet.InteractionType.Hash,(int)value);
        }
        
        
        
        public enum MovementType 
        {
            Walking = 1,
            //TODO Joge = 2, 
            Sprinting = 2
        }
    }
}