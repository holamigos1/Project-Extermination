using GameAnimation.Data;
using UnityEngine;

namespace GameAnimation.AnimatorControllers
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class HumanoidBody : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Transform _transform;
        [SerializeField, HideInInspector] private GameObject _gameObject;
        [SerializeField, HideInInspector] private Animator _animator;

        [SerializeField] private AnimationControllerState _state;
        [SerializeField] private AnimationControllerState _state2;
        
        private void Reset()
        {
            _animator = GetComponent<Animator>();
            _transform = transform;
            _gameObject = gameObject;
        }
    }
}
