using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.Data
{
	[Serializable]
	public struct VectorSpringData
	{
		[FormerlySerializedAs("x")] public SpringData xAxis;
		[FormerlySerializedAs("y")] public SpringData yAxis;
		[FormerlySerializedAs("z")] public SpringData zAxis;
		public                             Vector3    scale;
	}
}