using System;
using UnityEngine;

namespace Weapons.Data
{
	[Serializable]
	public struct LocationAndRotation
	{
		public Vector3    position;
		public Quaternion rotation;

		public LocationAndRotation(Vector3 location, Quaternion rotation)
		{
			this.position = location;
			this.rotation = rotation;
		}
        
		public LocationAndRotation(Transform transform)
		{
			position = transform.position;
			rotation = transform.rotation;
		}
	}
}