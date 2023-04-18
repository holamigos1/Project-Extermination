// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class DemoLoco : LocomotionLayer
    {
        protected static readonly int RotX = Animator.StringToHash("RotX");
        protected static readonly int RotY = Animator.StringToHash("RotY");
        protected static readonly int RotZ = Animator.StringToHash("RotZ");
        protected static readonly int LocX = Animator.StringToHash("LocX");
        protected static readonly int LocY = Animator.StringToHash("LocY");
        protected static readonly int LocZ = Animator.StringToHash("LocZ");

        [Header("Sprint")] 
        [SerializeField] protected AnimationCurve sprintBlendCurve;
        [SerializeField] protected LocRot sprintPose;
        protected float sprintPlayback;
        protected bool sprint;

        protected float smoothSprintLean;

        public void SetSprint(bool isSprinting)
        {
            sprint = isSprinting;
        }
        
        public void SetLocoWeight(float weight)
        {
            locoAlpha = Mathf.Clamp01(weight);
        }

        protected virtual void ApplyLocomotion()
        {
            smoothLocoAlpha = CoreToolkitLib.Glerp(smoothLocoAlpha, locoAlpha, 5f);

            var master = GetMasterIK();
            var animator = GetAnimator();

            Vector3 curveData = new Vector3();
            curveData.x = animator.GetFloat(RotX);
            curveData.y = animator.GetFloat(RotY);
            curveData.z = animator.GetFloat(RotZ);

            var animRot = Quaternion.Euler(curveData * 100f);
            animRot.Normalize();

            curveData.x = animator.GetFloat(LocX);
            curveData.y = animator.GetFloat(LocY);
            curveData.z = animator.GetFloat(LocZ);

            if (sprint)
            {
                sprintPlayback += Time.deltaTime;
            }
            else
            {
                sprintPlayback -= Time.deltaTime;
            }

            sprintPlayback = Mathf.Clamp(sprintPlayback, 0f,
                sprintBlendCurve[sprintBlendCurve.length - 1].time);

            float sprintAlpha = sprintBlendCurve.Evaluate(sprintPlayback);

            var mouseInput = GetCharData().deltaAimInput;

            smoothSprintLean = CoreToolkitLib.Glerp(smoothSprintLean, 4f * mouseInput.x, 3f);
            smoothSprintLean = Mathf.Clamp(smoothSprintLean, -15f, 15f);

            var leanVector = new Vector3(0f, smoothSprintLean, -smoothSprintLean);
            var sprintLean = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(leanVector),
                sprintAlpha);
            
            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, GetPelvis(), sprintLean, 1f);
            
            smoothLocoAlpha *= (1f - sprintAlpha);
            
            GetAnimator().SetLayerWeight(2, 1f - sprintAlpha);
            GetAnimator().SetLayerWeight(3, 1f - sprintAlpha);

            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), master,
                Vector3.Lerp(Vector3.zero, curveData / 100f, smoothLocoAlpha), 1f);

            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, master,
                Quaternion.Slerp(Quaternion.identity, animRot, smoothLocoAlpha), 1f);

            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), master,
                Vector3.Lerp(Vector3.zero, sprintPose.position, sprintAlpha), 1f);

            CoreToolkitLib.RotateInBoneSpace(master.rotation, master,
                Quaternion.Slerp(Quaternion.identity, sprintPose.rotation, sprintAlpha), 1f);
        }
        
        public override void OnAnimUpdate()
        {
            var masterDynamic = GetMasterIK();
            LocRot baseT = new LocRot(masterDynamic.position, masterDynamic.rotation);
            
            ApplyReadyPose();
            ApplyLocomotion();

            LocRot newT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, smoothLayerAlpha);
            masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, smoothLayerAlpha);
        }
    }
}