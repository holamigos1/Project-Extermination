using System;
using UnityEngine.Serialization;

namespace Weapons.Data
{
	[Serializable]
	public struct LocationRotationSpringData
	{
		[FormerlySerializedAs("loc")] public VectorSpringData location;
		[FormerlySerializedAs("rot")] public VectorSpringData rotation;
	}
}