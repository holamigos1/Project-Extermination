using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core
{
	public class SpringCameraShake : MonoBehaviour
	{
		public  SpringShakeProfile shakeProfile;
		private Vector3            _dampedTarget;
		private Quaternion         _deltaDampedTarget;
		private Vector3            _target;

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

		public void PlayCameraShake() =>
			_target = shakeProfile.GetRandomTarget();
	}
}