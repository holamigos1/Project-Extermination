using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	/// <summary> Curve-based animation </summary>
	[Serializable]
	public struct DynamicMotion
	{
		[LabelText("Кривые позиции")]
		[SuffixLabel("Метры    ")]
		public Vector3DCurve location;
		
		[LabelText("Кривые вращений")]
		[SuffixLabel("Градусы °")]
		public Vector3DCurve rotation;
		
		[LabelText("Длинна перехода в эту анимацию")]
		[Tooltip("Если анимация начинается из другой анимации, о переход значений из той в эту анимацию займёт указанное в этом значении время.")]
		[SuffixLabel("Секунды  ")]
		[Range(0.01f, 3)]
		[SerializeField]
		private float _motionBlendSpeed;
		
		[SerializeField]
		[Range(0.01f,5)]
		[LabelText("Множитель времени кривых")]
		[Tooltip("При получении значений кривых по значению времени, то время умножается на этот модификатор.\n"
				 + "Можно не менять все кривые а просто менять этот параметр, делая движение быстрее или медленее.")]
		[SuffixLabel("Множитель")]
		private float _playRateFactor;
		
		[LabelText("Текущие значения анимации")]
		[HideInInspector]
		public LocationAndRotation _currentMotionValues;

		private float               _currentMotionBlendAlpha;
		private LocationAndRotation _cachedMotionPosition;
		private float               _motionEndTime;
		private float               _motionPlayedSeconds;

		public void Reset() =>
			_currentMotionValues = _cachedMotionPosition = new LocationAndRotation(Vector3.zero, Quaternion.identity);

		public void Play(ref DynamicMotion previousMotion)
		{
			_cachedMotionPosition = previousMotion._currentMotionValues;
			_motionEndTime = Mathf.Max(location.LastKeyframeTime, rotation.LastKeyframeTime);
			_motionPlayedSeconds = 0f;
			_currentMotionBlendAlpha = 0f;
		}

		private LocationAndRotation Evaluate(float value) => 
			new(location.Evaluate(value), Quaternion.Euler(rotation.Evaluate(value)));

		// Return currently playing motion
		public void UpdateMotion()
		{
			if (Mathf.Approximately(_motionPlayedSeconds, _motionEndTime))
			{
				_currentMotionValues = new LocationAndRotation(Vector3.zero, Quaternion.identity);
				return;
			}

			_motionPlayedSeconds += Time.deltaTime * _playRateFactor;
			_motionPlayedSeconds = Mathf.Clamp(_motionPlayedSeconds, 
											   0f, 
											   _motionEndTime);
			
			_currentMotionBlendAlpha = Mathf.Min(1f, 
												 _motionPlayedSeconds / _motionBlendSpeed);
			
			_currentMotionValues = CoreToolkitLib.Lerp(_cachedMotionPosition, 
													   Evaluate(_motionPlayedSeconds), _currentMotionBlendAlpha);
		}
	}
}