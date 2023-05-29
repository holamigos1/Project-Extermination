using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons.Data
{
	[Serializable]
	public struct MoveSwayData
	{
		[FormerlySerializedAs("maxMoveLocSway")] public Vector3 maxMoveLocationSway;
		[FormerlySerializedAs("maxMoveRotSway")] public Vector3 maxMoveRotationSway;
	}
}