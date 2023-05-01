using System;
using GameAnimation.Data;
using UnityEngine;

namespace GameAnimation.StateMachineBehaviours
{
    public class HideItemStateBehaviour : StateMachineBehaviour
    {
        public ItemType ItemType;
        public WeaponType WeaponType;
        
        public event Action OnStateEntering = delegate {  };
        public event Action OnStateExiting= delegate {  };

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnStateEntering?.Invoke();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnStateExiting?.Invoke();
        }
    }
}