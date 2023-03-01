using GameData.AnimationTags;
using UnityEngine;
using Weapons.Basic;

namespace Weapons.Melle
{
    public class Katana : Weapon
    {
        public override void PlayFireAction()
        {
            base.PlayFireAction();
            if(ItemAnimator.GetCurrentAnimatorStateInfo(0).IsName(AnimationParams.IDLE) == false) return;
            
            var attackAnimationID = Random.Range((int)1, (int)3+1);//TODO Убери магические числа
            ItemAnimator.SetInteger(AnimationParams.ATTACK_ID, attackAnimationID);
            ItemAnimator.SetTrigger(AnimationParams.PERFORM_ATTACK);
        }
    }
}
