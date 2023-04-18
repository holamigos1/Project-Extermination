// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public enum ReadyPose
    {
        LowReady,
        HighReady
    }

    public class LocomotionLayer : AnimLayer
    {
        [SerializeField] public LocRot highReadyPose;
        [SerializeField] public LocRot lowReadyPose;
        [SerializeField] protected float interpSpeed;

        protected float smoothReadyAlpha;
        protected float readyPoseAlpha = 0f;
        protected float locoAlpha = 1f;
        protected float smoothLocoAlpha;

        [SerializeField] protected ReadyPose readyPoseType;

        public void SetReadyPose(ReadyPose poseType)
        {
            readyPoseType = poseType;
        }

        public void SetReadyWeight(float weight)
        {
            readyPoseAlpha = Mathf.Clamp01(weight);
        }

        public override void OnAnimUpdate()
        {
            var masterDynamic = GetMasterIK();
            LocRot baseT = new LocRot(masterDynamic.position, masterDynamic.rotation);
            
            ApplyReadyPose();

            LocRot newT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, smoothLayerAlpha);
            masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, smoothLayerAlpha);
        }

        protected virtual void ApplyReadyPose()
        {
            var master = GetMasterIK();
            
            smoothReadyAlpha = CoreToolkitLib.Glerp(smoothReadyAlpha, readyPoseAlpha, interpSpeed);

            var finalPose = readyPoseType == ReadyPose.HighReady ? highReadyPose : lowReadyPose;
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), master, 
                Vector3.Lerp(Vector3.zero, finalPose.position, smoothReadyAlpha), 1f);
            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, master,
                Quaternion.Slerp(Quaternion.identity, finalPose.rotation, smoothReadyAlpha), 1f);
        }
    }
}
