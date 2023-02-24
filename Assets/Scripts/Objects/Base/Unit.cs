using Sirenix.OdinInspector;
using UnityEngine;

namespace Objects.Base
{
    public abstract class Unit : MonoBehaviour
    {
        [ShowInInspector] public float HealthPoints => _healthPoints;
        
        private float _healthPoints = 100;
    }
}