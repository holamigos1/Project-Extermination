using System;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct DynamicBone
	{
		[Tooltip("Actual bone")] 
		public Transform target;

		[Tooltip("Target for elbows/knees")] 
		public Transform hintTarget;

		[Tooltip("The representation of the DynamicBone in space")]
		public GameObject obj;

		public void Retarget()
		{
			if (target == null)
				return;

			obj.transform.position = target.position;
			obj.transform.rotation = target.rotation;
		}

		public void Rotate(Quaternion parent, Quaternion rotation, float alpha) =>
			CoreToolkitLib.RotateInBoneSpace(parent, obj.transform, rotation, alpha);

		public void Move(Transform parent, Vector3 offset, float alpha) =>
			CoreToolkitLib.MoveInBoneSpace(parent, obj.transform, offset, alpha);
	}
}