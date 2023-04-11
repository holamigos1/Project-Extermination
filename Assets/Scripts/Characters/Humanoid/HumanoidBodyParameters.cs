using GameAnimation.Sheets;
using UnityEngine;

namespace Characters.Humanoid
{
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
                Mathf.SmoothDamp(ForwardMovementParameter, _speededMovementParametersGoal.y, ref _movementAcceleration.z, MOVEMENT_SMOOTH_TIME));

            _humanAnimator.SetFloat(_humanAnimatorSheet.SideMovement.Hash, 
                Mathf.SmoothDamp(SideMovementParameter, _speededMovementParametersGoal.x, ref _movementAcceleration.x, MOVEMENT_SMOOTH_TIME));
        }

        private Vector2 _speededMovementParametersGoal => _movementParametersGoal * (int)CurrentMovementType;
        private Vector2 _movementParametersGoal;
        private Vector3 _movementAcceleration;

        public float ForwardMovementParameter
        {
            get => _humanAnimator.GetFloat(_humanAnimatorSheet.ForwardMovement.Hash);
            set => _movementParametersGoal.y = value;
        }
        
        public float SideMovementParameter
        {
            get => _humanAnimator.GetFloat(_humanAnimatorSheet.SideMovement.Hash);
            set => _movementParametersGoal.x = value;
        }
        
        public enum MovementType 
        {
            Walking = 1,
            //TODO Joge = 2, 
            Sprinting = 2
        }
    }
}