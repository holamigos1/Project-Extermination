using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plugins.Kinemation.FPSFramework.Runtime.Core.Data
{
	[Serializable]
	public struct DynamicBone
	{
		[Tooltip("Actual bone")]
		[LabelText("Transform кости")]
		public Transform _bone;
		
		[Tooltip("Target for elbows/knees")]
		[LabelText("Transform колени или локтя")]
		public Transform _boneHint;
		
		[Tooltip("The representation of the DynamicBone in space")]
		[LabelText("Объект кости")]
		public GameObject _boneObject;

		public void Retarget()
		{
			if (_bone == null)
				return;

			_boneObject.transform.position = _bone.position;
			_boneObject.transform.rotation = _bone.rotation;
		}

		public void Rotate(Quaternion parent, Quaternion rotation, float alpha) =>
			CoreToolkitLib.RotateInBoneSpace(parent, _boneObject.transform, rotation, alpha);

		public void Move(Transform parent, Vector3 offset, float alpha) =>
			CoreToolkitLib.MoveInBoneSpace(parent, _boneObject.transform, offset, alpha);
	}
}