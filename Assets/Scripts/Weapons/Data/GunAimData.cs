using System;
using UnityEngine;

namespace Weapons.Data
{
	[Serializable]
	public struct GunAimData
	{
		public TargetAimData    target;
		public Transform        pivotPoint;
		public Transform        aimPoint;
		public LocationAndRotation pointAimOffset;
		public float            aimSpeed;
	}
}