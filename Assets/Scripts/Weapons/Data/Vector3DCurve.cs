using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Misc.Extensions;
using Unity.Mathematics;

namespace Weapons.Data
{
	/// <summary> Трёхмерный вариант AnimationCurve. </summary>
	/// <seealso cref="AnimationCurve"/>
	[Serializable]
	public struct Vector3DCurve
	{
		[SerializeField] private AnimationCurve xAxisCurve;
		[SerializeField] private AnimationCurve yAxisCurve;
		[SerializeField] private AnimationCurve zAxisCurve;
		
		public bool IsValid => xAxisCurve.keys.Length != 0
							   || yAxisCurve.keys.Length != 0
							   || zAxisCurve.keys.Length != 0;

		public float LastKeyframeTime
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				float maxTime = -1f;
				maxTime = math.max(maxTime, xAxisCurve.LastKeyframe().time);
				maxTime = math.max(maxTime, yAxisCurve.LastKeyframe().time);
				maxTime = math.max(maxTime, zAxisCurve.LastKeyframe().time);
				return maxTime;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 Evaluate(float time) => 
			new(xAxisCurve.Evaluate(time), yAxisCurve.Evaluate(time), zAxisCurve.Evaluate(time));
	}
}