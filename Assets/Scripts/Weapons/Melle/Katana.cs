using Data.AnimationTags;
using UnityEngine;
using Weapons.Basic;

namespace Weapons.Melle
{
    public class Katana : Weapon
    {
        public override void PlayFireAction()
        {
            base.PlayFireAction();
            
            var attackAnimationID = Random.Range((int)1, (int)3+1);//мать его maxExclusive
            _animator.SetInteger(AnimationParams.PERFORM_ATTACK_ACTION, attackAnimationID);
            _animator.SetTrigger(AnimationParams.PERFORM_ATTACK);
        }
    }
}
