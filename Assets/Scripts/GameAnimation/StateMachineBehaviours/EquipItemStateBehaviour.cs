using GameAnimation.Data;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GameAnimation.StateMachineBehaviours
{
    public class EquipItemStateBehaviour : StateMachineBehaviour
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