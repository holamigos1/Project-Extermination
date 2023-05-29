using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct CachedBones
	{
		public List<Quaternion>      lookUp;
		public (Vector3, Quaternion) leftHand;
		public (Vector3, Quaternion) pelvis;
		public (Vector3, Quaternion) rightHand;
	}
}