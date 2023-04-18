using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kinemation.FPSFramework.Runtime.Core
{
    [Serializable]
    public struct SpringShakeProfile
    {
        [SerializeField] public VectorSpringData springData;
        [SerializeField] public float dampSpeed;
        [SerializeField] public Vector2 pitch;
        [SerializeField] public Vector2 yaw;
        [SerializeField] public Vector2 roll;

        public Vector3 GetRandomTarget()
        {
            return new Vector3(Random.Range(pitch.x, pitch.y), Random.Range(yaw.x, yaw.y),
                Random.Range(roll.x, roll.y));
        }
    }
    
    public class SpringCameraShake : MonoBehaviour
    {
        public SpringShakeProfile shakeProfile;
        private Vector3 _dampedTarget;
        private Vector3 _target;
        private Quaternion _deltaDampedTarget;

        // Should be applied after camera stabilization logic
        private void LateUpdate()
        {
            // Interpolate
            _target = CoreToolkitLib.SpringInterp(_target, Vector3.zero, ref shakeProfile.springData);
            _dampedTarget = CoreToolkitLib.Glerp(_dampedTarget, _target, shakeProfile.dampSpeed);
            
            _deltaDampedTarget = Quaternion.Inverse(_deltaDampedTarget) * Quaternion.Euler(_dampedTarget);
            transform.rotation *= _deltaDampedTarget;

            _deltaDampedTarget = Quaternion.Euler(_dampedTarget);
        }

        public void PlayCameraShake()
        {
            _target = shakeProfile.GetRandomTarget();
        }
    }
}
