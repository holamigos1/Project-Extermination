// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class DemoLoco : LocomotionLayer
	{
		protected static readonly int RotX = Animator.StringToHash("RotX");
		protected static readonly int RotY = Animator.StringToHash("RotY");
		protected static readonly int RotZ = Animator.StringToHash("RotZ");
		protected static readonly int LocX = Animator.StringToHash("LocX");
		protected static readonly int LocY = Animator.StringToHash("LocY");
		protected static readonly int LocZ = Animator.StringToHash("LocZ");

		[Header("Sprint"), SerializeField] 
        
		protected AnimationCurve sprintBlendCurve;

		[SerializeField] protected LocationAndRotation sprintPose;

		protected float smoothSprintLean;
		protected bool  sprint;
		protected float sprintPlayback;

		public void SetSprint(bool isSprinting) =>
			sprint = isSprinting;

		public void SetLocoWeight(float weight) =>
			locoAlpha = Mathf.Clamp01(weight);

		protected virtual void ApplyLocomotion()
		{
			smoothLocoAlpha = CoreToolkitLib.Glerp(smoothLocoAlpha, locoAlpha, 5f);

			Transform master = MasterIK;
			Animator animator = Rig_animator;

			var curveData = new Vector3();
			curveData.x = animator.GetFloat(RotX);
			curveData.y = animator.GetFloat(RotY);
			curveData.z = animator.GetFloat(RotZ);

			Quaternion animRot = Quaternion.Euler(curveData * 100f);
			animRot.Normalize();

			curveData.x = animator.GetFloat(LocX);
			curveData.y = animator.GetFloat(LocY);
			curveData.z = animator.GetFloat(LocZ);

			if (sprint)
				sprintPlayback += Time.deltaTime;
			else
				sprintPlayback -= Time.deltaTime;

			sprintPlayback = Mathf.Clamp(sprintPlayback, 0f,
										 sprintBlendCurve[sprintBlendCurve.length - 1].time);

			float sprintAlpha = sprintBlendCurve.Evaluate(sprintPlayback);

			Vector2 mouseInput = CharData.deltaAimInput;

			smoothSprintLean = CoreToolkitLib.Glerp(smoothSprintLean, 4f * mouseInput.x, 3f);
			smoothSprintLean = Mathf.Clamp(smoothSprintLean, -15f, 15f);

			var leanVector = new Vector3(0f, smoothSprintLean, -smoothSprintLean);
			Quaternion sprintLean = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(leanVector),
													 sprintAlpha);

			CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, Pelvis_bone, sprintLean, 1f);

			smoothLocoAlpha *= 1f - sprintAlpha;

			Rig_animator.SetLayerWeight(2, 1f - sprintAlpha);
			Rig_animator.SetLayerWeight(3, 1f - sprintAlpha);

			CoreToolkitLib.MoveInBoneSpace(RootBone, master,
										   Vector3.Lerp(Vector3.zero, curveData / 100f, smoothLocoAlpha), 1f);

			CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, master,
											 Quaternion.Slerp(Quaternion.identity, animRot, smoothLocoAlpha), 1f);

			CoreToolkitLib.MoveInBoneSpace(RootBone, master,
										   Vector3.Lerp(Vector3.zero, sprintPose.position, sprintAlpha), 1f);

			CoreToolkitLib.RotateInBoneSpace(master.rotation, master,
											 Quaternion.Slerp(Quaternion.identity, sprintPose.rotation, sprintAlpha),
											 1f);
		}

		public override void OnAnimUpdate()
		{
			Transform masterDynamic = MasterIK;
			var baseT = new LocationAndRotation(masterDynamic.position, masterDynamic.rotation);

			ApplyReadyPose();
			ApplyLocomotion();

			var newT = new LocationAndRotation(masterDynamic.position, masterDynamic.rotation);

			masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, SmoothLayerAlpha);
			masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, SmoothLayerAlpha);
		}
	}
}