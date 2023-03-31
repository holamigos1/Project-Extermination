using Weapons.Range.Base;
using GameAnimation.Data;
using UnityEngine;

namespace Weapons.Range
{
    public class Pistol : Firearm
    {
        [SerializeField] private AnimationControllerState State;
        [SerializeField] private AnimationControllerState State2;
        [SerializeField] private AnimationControllerState State3;
        [SerializeField] private AnimationControllerParameter Parameter;
    }
}