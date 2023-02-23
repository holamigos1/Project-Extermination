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
            _animator.SetInteger(AnimationParams.PERFORM_ATTACK, Random.Range((int)1,(int)3));
        }
    }
}
