// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class SlotLayer : AnimLayer
    {
        public MotionPlayer motionPlayer;
        
        public void PlayMotion(DynamicMotion motionToPlay)
        {
            motionPlayer.Play(motionToPlay);
        }

        public override void OnAnimStart()
        {
            motionPlayer.Reset();
        }

        public override void OnAnimUpdate()
        {
            motionPlayer.UpdateMotion();
            
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), GetMasterIK(), motionPlayer.Get().position, 
                smoothLayerAlpha);
            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, GetMasterIK(), 
                motionPlayer.Get().rotation, smoothLayerAlpha);
        }
    }
}