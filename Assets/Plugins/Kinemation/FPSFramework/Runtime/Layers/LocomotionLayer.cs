// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class LocomotionLayer : AnimLayer
	{
		[SerializeField] public    LocationAndRotation highReadyPose;
		[SerializeField] public    LocationAndRotation lowReadyPose;
		[SerializeField] protected float            interpSpeed;

		[SerializeField] protected ReadyPose readyPoseType;
		protected                  float     locoAlpha = 1f;
		protected                  float     readyPoseAlpha;
		protected                  float     smoothLocoAlpha;

		protected float smoothReadyAlpha;

		public void SetReadyPose(ReadyPose poseType) =>
			readyPoseType = poseType;

		public void SetReadyWeight(float weight) =>
			readyPoseAlpha = Mathf.Clamp01(weight);

		public override void OnAnimUpdate()
		{
			Transform masterDynamic = MasterIK;
			var baseT = new LocationAndRotation(masterDynamic.position, masterDynamic.rotation);

			ApplyReadyPose();

			var newT = new LocationAndRotation(masterDynamic.position, masterDynamic.rotation);

			masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, SmoothLayerAlpha);
			masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, SmoothLayerAlpha);
		}

		protected virtual void ApplyReadyPose()
		{
			Transform master = MasterIK;

			smoothReadyAlpha = CoreToolkitLib.Glerp(smoothReadyAlpha, readyPoseAlpha, interpSpeed);

			LocationAndRotation finalPose = readyPoseType == ReadyPose.HighReady ?
				highReadyPose :
				lowReadyPose;
			CoreToolkitLib.MoveInBoneSpace(RootBone, master,
										   Vector3.Lerp(Vector3.zero, finalPose.position, smoothReadyAlpha), 1f);
			CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, master,
											 Quaternion.Slerp(Quaternion.identity, finalPose.rotation,
															  smoothReadyAlpha), 1f);
		}
	}
}