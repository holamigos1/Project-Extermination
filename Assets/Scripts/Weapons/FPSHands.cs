using GameData.AnimationTags;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Animator))]
    public class FPSHands : MonoBehaviour
    {
        public bool IsReady => HandsAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.IDLE) ||
                               HandsAnimator.GetBool(AnimationParams.IS_IDLE);

        public bool IsHidden => HandsAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.NONE);
        
        public Animator HandsAnimator
        {
            get
            {
                if (_handsAnimator == null) 
                    _handsAnimator = GetComponent<Animator>();
                return _handsAnimator;
            }
        }

        private Animator _handsAnimator;
        
        public void PerformAttack()
        {
            if(IsReady == false) return;

            int attackAnimationID = Random.Range(1, 3 + 1); //TODO магичиские числа
            
            HandsAnimator.SetInteger(AnimationParams.ATTACK_ID, attackAnimationID);
            HandsAnimator.SetTrigger(AnimationParams.PERFORM_ATTACK);
        }
        
        public void ShowFists()
        {
            HandsAnimator.SetTrigger(AnimationParams.SHOW_FISTS);
        }
        
        public void HideHands()
        {
            if(IsReady == false) return;
            
            HandsAnimator.SetTrigger(AnimationParams.HIDE_FISTS);
        }
    }
}
