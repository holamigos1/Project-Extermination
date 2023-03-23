using System;
using System.Collections.Generic;
using Misc;
using UnityEngine;

namespace Animation
{
    public class StateMachineBehaviorListener : StateMachineBehaviour
    {
        public event Action<int, string> OnStateEnterEvent;
        public event Action<int, string> OnStateExitEvent;

        public static SortedList<int, string> States = new SortedList<int, string>();

        static StateMachineBehaviorListener()
        {
            /*
            AddState("Grounded");
            AddState("Pickup");
            AddState("Mine");
            AddState("Downward Chop");
            AddState("Horizontal Chop");
            */
        }

        private static void AddState(string stateName)
        {
            States[Animator.StringToHash(stateName)] = stateName;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (IsValid(animator, stateInfo, layerIndex))
                OnStateEnterEvent?.Invoke(stateInfo.shortNameHash, States[stateInfo.shortNameHash]);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            if (IsValid(animator, stateInfo, layerIndex))
                OnStateExitEvent?.Invoke(stateInfo.shortNameHash, States[stateInfo.shortNameHash]);
        }

        private bool IsValid(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
#if (UNITY_EDITOR)
            if (States.ContainsKey(stateInfo.shortNameHash)) return true;
            
            string stateName = animator.GetCurrentStateName(layerIndex);
            Debug.LogError("State '" + stateName + "' missing.");
            States[stateInfo.shortNameHash] = stateName;
            return true;
#else
            return States.ContainsKey(stateInfo.shortNameHash);
#endif
        }
    }
}