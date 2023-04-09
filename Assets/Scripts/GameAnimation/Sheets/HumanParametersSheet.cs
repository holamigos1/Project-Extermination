using GameAnimation.Data;
using GameAnimation.Sheets.Base;
using UnityEngine;

namespace GameAnimation.Sheets
{
    [CreateAssetMenu(fileName = "Human Animator Sheet", menuName = "Game Data/Animator Sheets/Human", order = 1)]
    public class HumanParametersSheet : AnimatorParametersSheet
    {
        public AnimationControllerParameter SideMovement => _sideMovement;
        public AnimationControllerParameter ForwardMovement => _forwardMovement;
        
        [SerializeField] private AnimationControllerParameter _sideMovement;
        [SerializeField] private AnimationControllerParameter _forwardMovement;
    }
}