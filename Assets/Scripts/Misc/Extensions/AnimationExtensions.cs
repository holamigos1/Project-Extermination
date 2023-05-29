using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Misc.Extensions
{
	public static class AnimationExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsClear(this AnimationCurve curve) =>
			curve?.keys.Length == 0;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Keyframe LastKeyframe(this AnimationCurve curve) =>
			curve.keys.Length != 0 ?
				curve.keys[^1] :
				new Keyframe();
	}
}