// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class LeftHandIKLayer : AnimLayer
    {
        public Transform leftHandTarget;
        [SerializeField] private string blendCurveName;
        [SerializeField] private bool useCurveBlending = false;

        public override void OnAnimUpdate()
        {
            var target = GetGunData().leftHandTarget == null ? leftHandTarget : GetGunData().leftHandTarget;
            var leftHand = core.rigData.leftHand.obj.transform;
            var finalAlpha = useCurveBlending ? GetAnimator().GetFloat(blendCurveName) : smoothLayerAlpha;

            leftHand.position = Vector3.Lerp(leftHand.position, target.position, finalAlpha);
            leftHand.rotation = Quaternion.Slerp(leftHand.rotation, target.rotation, finalAlpha);
        }
    }
}