using GameAnimation.Data;
using GameAnimation.Sheets.Base;
using UnityEngine;

namespace GameAnimation.Sheets
{
    [CreateAssetMenu(fileName = "Rifle Animator Sheet", menuName = "Game Data/Animator Sheets/Rifle", order = 1)]
    public class RifleAnimatorSheet : AnimatorParametersSheet
    {
        public AnimatorControllerState FireState => _fireState;
        public AnimatorControllerLayer DefaultLayer => _defaultLayer;
        
        [Header("Слои")]
        [SerializeField] private AnimatorControllerLayer _defaultLayer;
        
        [Header("Стейты")]
        [SerializeField] private AnimatorControllerState _fireState;
    }
}