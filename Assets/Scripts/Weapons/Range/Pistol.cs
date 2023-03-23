using Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons.Range.Base;

namespace Weapons.Range
{
    public class Pistol : Firearm
    {
        [BoxGroup("Стейты")][HideLabel]
        public AnimatorStates States;
        [BoxGroup("Парраметры")][HideLabel]
        public AnimatorParameters Parameters;

        void Start()
        {
            Debug.Log(States.Name); 
            Debug.Log(States.Hash);
            Debug.Log(ItemAnimator.GetCurrentStateName(0));
        }
    }
}