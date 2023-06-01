using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons.Data
{
	[Serializable]
	public struct LocationAndRotation
	{
		[LabelText("Позиция")]
		[SuffixLabel("Метры    ")]
		public Vector3    position;
		
		[LabelText("Вращение")]
		[SuffixLabel("Градусы °")]
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