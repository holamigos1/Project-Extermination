using System;

namespace Weapons.Data
{
	[Serializable]
	public struct GunBlockData
	{
		public float  weaponLength;
		public float  startOffset;
		public float  threshold;
		public LocationAndRotation restPose;
	}
}