using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct AimOffset
	{
		public List<AimOffsetBone> bones;
		public int                 indexOffset;

		[NonSerialized] 
		public List<Vector2> Angles;

		public void Init()
		{
			if (Angles == null)
				Angles = new List<Vector2>();
			else
				Angles.Clear();

			bones ??= new List<AimOffsetBone>();

			for (var i = 0; i < bones.Count - indexOffset; i++)
			{
				AimOffsetBone bone = bones[i];
				Angles.Add(bone.maxAngle);
			}
		}

		public bool IsValid =>
			bones != null && Angles != null;

		public bool IsChanged =>
			bones.Count != Angles.Count;
	}
}