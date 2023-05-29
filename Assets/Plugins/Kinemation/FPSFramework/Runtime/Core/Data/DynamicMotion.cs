using System;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct DynamicMotion
	{
		// Curve-based animation
		[FormerlySerializedAs("rot")] public Vector3DCurve       rotation;
		[FormerlySerializedAs("loc")] public Vector3DCurve       location;
		public                               LocationAndRotation outMotion;
		// How fast to blend to another motion
		[SerializeField] private float blendSpeed;
		[SerializeField] private float playRate;

		private float               _blendAlpha;
		// Used to blend to the currentMotion
		private LocationAndRotation _cachedMotion;
		private float               _motionLength;
		private float               _playBack;

		public void Reset() =>
			outMotion = _cachedMotion = new LocationAndRotation(Vector3.zero, Quaternion.identity);

		public void Play(ref DynamicMotion previousMotion)
		{
			_cachedMotion = previousMotion.outMotion;
			playRate = Mathf.Approximately(playRate, 0f) ? 
				1f : playRate;
			
			_motionLength = Mathf.Max(location.LastKeyframeTime, rotation.LastKeyframeTime);
			_playBack = 0f;
			_blendAlpha = 0f;
		}

		private LocationAndRotation Evaluate() => 
			new(location.Evaluate(_playBack), Quaternion.Euler(rotation.Evaluate(_playBack)));

		// Return currently playing motion
		public void UpdateMotion()
		{
			if (Mathf.Approximately(_playBack, _motionLength))
			{
				outMotion = new LocationAndRotation(Vector3.zero, Quaternion.identity);
				return;
			}

			_playBack += Time.deltaTime * playRate;
			_playBack = Mathf.Clamp(_playBack, 0f, _motionLength);
			LocationAndRotation currentMotion = Evaluate();

			_blendAlpha += Time.deltaTime * blendSpeed;
			_blendAlpha = Mathf.Min(1f, _blendAlpha);

			LocationAndRotation result = CoreToolkitLib.Lerp(_cachedMotion, currentMotion, _blendAlpha);
			outMotion = result;
		}
	}
}