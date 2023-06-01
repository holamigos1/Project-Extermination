// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class LeftHandIKLayer : AnimLayer
	{
		public                   Transform leftHandTarget;
		[SerializeField] private string    blendCurveName;
		[SerializeField] private bool      useCurveBlending;

		public override void OnAnimUpdate()
		{
			Transform target = GunData.leftHandTarget == null ?
				leftHandTarget :
				GunData.leftHandTarget;
			Transform leftHand = CoreAnim._rigData.leftHandBone._boneObject.transform;
			float finalAlpha = useCurveBlending ?
				Rig_animator.GetFloat(blendCurveName) :
				SmoothLayerAlpha;

			leftHand.position = Vector3.Lerp(leftHand.position, target.position, finalAlpha);
			leftHand.rotation = Quaternion.Slerp(leftHand.rotation, target.rotation, finalAlpha);
		}
	}
}