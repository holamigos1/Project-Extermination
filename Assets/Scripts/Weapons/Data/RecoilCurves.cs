using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.Data
{
	[Serializable]
	public struct RecoilCurves
	{
		[FormerlySerializedAs("semiRotCurve")] public Vector3DCurve semiRotationCurve;
		[FormerlySerializedAs("semiLocCurve")] public Vector3DCurve semiLocationCurve;
		[FormerlySerializedAs("autoRotCurve")] public Vector3DCurve autoRotationCurve;
		[FormerlySerializedAs("autoLocCurve")] public Vector3DCurve autoLocationCurve;

		private List<AnimationCurve> _curves;
	}
}