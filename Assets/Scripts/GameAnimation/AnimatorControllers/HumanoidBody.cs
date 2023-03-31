using GameAnimation.Data;
using UnityEngine;

namespace GameAnimation.AnimatorControllers
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HumanoidBody : MonoBehaviour
    {
        public Animator HumanoidAnimator =>
            _animator ??= GetComponent<Animator>();
        
        private Animator _animator;

        [SerializeField] private AnimationControllerState _state;
        [SerializeField] private AnimationControllerState _state2;
    }
}
