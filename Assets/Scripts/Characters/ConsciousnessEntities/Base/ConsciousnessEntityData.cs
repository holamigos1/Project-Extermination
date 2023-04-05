using UnityEngine;

namespace Characters.Data.Base
{
    public abstract class ConsciousnessEntityData : ScriptableObject
    {
        public virtual bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                
                Debug.Log(_isActive ? 
                    $"{this.GetType()} включилось." : 
                    $"{this.GetType()} вырубилось.");
            }
        }

        private bool _isActive;
    }
}