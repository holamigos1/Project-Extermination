using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct AimOffsetBone
	{
		[FormerlySerializedAs("bone")] public Transform boneTransform;
		public  Vector2   maxAngle;
	}
}