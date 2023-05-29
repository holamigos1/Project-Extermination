// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using Plugins.Kinemation.FPSFramework.Runtime.Core.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class SlotLayer : AnimLayer
	{
		public MotionPlayer motionPlayer;

		public void PlayMotion(DynamicMotion motionToPlay) =>
			motionPlayer.Play(motionToPlay);

		public override void OnAnimStart() =>
			motionPlayer.Reset();

		public override void OnAnimUpdate()
		{
			motionPlayer.UpdateMotion();

			CoreToolkitLib.MoveInBoneSpace(RootBone, MasterIK, motionPlayer.Get().position,
										   SmoothLayerAlpha);
			CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, MasterIK,
											 motionPlayer.Get().rotation, SmoothLayerAlpha);
		}
	}
}