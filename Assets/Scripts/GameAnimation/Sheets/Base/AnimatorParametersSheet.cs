using UnityEngine;

namespace GameAnimation.Sheets.Base
{
    public abstract class AnimatorParametersSheet : ScriptableObject
    {
        public RuntimeAnimatorController TargetController => _targetController;
        
        [SerializeField] 
        private RuntimeAnimatorController _targetController;
    }
}