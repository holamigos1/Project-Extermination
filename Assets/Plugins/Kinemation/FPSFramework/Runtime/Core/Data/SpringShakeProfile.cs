using System;
using UnityEngine;
using Weapons.Data;
using Random = UnityEngine.Random;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct SpringShakeProfile
	{
		[SerializeField] public VectorSpringData springData;
		[SerializeField] public float            dampSpeed;
		[SerializeField] public Vector2          pitch;
		[SerializeField] public Vector2          yaw;
		[SerializeField] public Vector2          roll;

		public Vector3 GetRandomTarget() =>
			new(Random.Range(pitch.x, pitch.y), Random.Range(yaw.x, yaw.y),
				Random.Range(roll.x, roll.y));
	}
}