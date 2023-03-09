using GameData.AnimationTags;
using UnityEngine;

namespace Weapons.Melle
{
    public class Katana : Weapon
    {
        public override void PlayFireAction()
        {
            base.PlayFireAction();

            var attackAnimationID = Random.Range((int)1, (int)3+1);//TODO Убери магические числа
            ItemAnimator.SetInteger(AnimationParams.ATTACK_ID, attackAnimationID);
            ItemAnimator.SetTrigger(AnimationParams.PERFORM_ATTACK);
        }
    }
}
