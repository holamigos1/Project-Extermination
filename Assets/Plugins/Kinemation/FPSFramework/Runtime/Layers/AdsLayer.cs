// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class AdsLayer : AnimLayer
	{
		[Header("SightsAligner"), Range(0f, 1f)]
        
		public float aimLayerAlphaLoc;

		[Range(0f, 1f)]  public    float     aimLayerAlphaRot;
		[SerializeField] protected Transform aimTarget;

		protected float adsAlpha;
		protected float pointAlpha;

		protected float            smoothAdsAlpha;
		protected LocationAndRotation smoothAimPoint;
		protected float            smoothPointAlpha;

		public void SetAdsAlpha(float weight) =>
			adsAlpha = Mathf.Clamp01(weight);

		public void SetPointAlpha(float weight) =>
			pointAlpha = Mathf.Clamp01(weight);

		public override void OnAnimUpdate()
		{
			Transform dynamicMaster = MasterIK;

			Vector3 baseLoc = dynamicMaster.position;
			Quaternion baseRot = dynamicMaster.rotation;

			ApplyPointAiming();
			ApplyAiming();

			Vector3 postLoc = dynamicMaster.position;
			Quaternion postRot = dynamicMaster.rotation;

			dynamicMaster.position = Vector3.Lerp(baseLoc, postLoc, SmoothLayerAlpha);
			dynamicMaster.rotation = Quaternion.Slerp(baseRot, postRot, SmoothLayerAlpha);
		}

		public void CalculateAimData()
		{
			GunAimData aimData = GunData.gunAimData;

			string stateName = aimData.target.stateName.Length > 0 ?
				aimData.target.stateName :
				aimData.target.staticPose.name;

			if (Rig_animator != null)
			{
				Rig_animator.Play(stateName);
				Rig_animator.Update(0f);
			}

			// Cache the local data, so we can apply it without issues
			aimData.target.aimLocation = aimData.pivotPoint.InverseTransformPoint(aimTarget.position);
			aimData.target.aimRotation = Quaternion.Inverse(aimData.pivotPoint.rotation) * RootBone.rotation;
		}

		protected virtual void ApplyAiming()
		{
			GunAimData aimData = GunData.gunAimData;

			//Apply Aiming
			Transform masterTransform = MasterIK;

			smoothAdsAlpha = CoreToolkitLib.Glerp(smoothAdsAlpha, adsAlpha, aimData.aimSpeed);

			ApplyHandsOffset();

			Vector3 scopeAimLoc = Vector3.zero;
			Quaternion scopeAimRot = Quaternion.identity;

			if (aimData.aimPoint != null)
			{
				scopeAimRot = Quaternion.Inverse(aimData.pivotPoint.rotation) * aimData.aimPoint.rotation;
				scopeAimLoc = -aimData.pivotPoint.InverseTransformPoint(aimData.aimPoint.position);
			}

			if (!smoothAimPoint.position.Equals(scopeAimLoc))
				smoothAimPoint.position = CoreToolkitLib.Glerp(smoothAimPoint.position, scopeAimLoc, aimData.aimSpeed);

			if (!smoothAimPoint.rotation.Equals(scopeAimRot))
				smoothAimPoint.rotation = CoreToolkitLib.Glerp(smoothAimPoint.rotation, scopeAimRot, aimData.aimSpeed);

			LocationAndRotation additiveAim = aimData.target != null ?
				new LocationAndRotation(aimData.target.aimLocation, aimData.target.aimRotation) :
				new LocationAndRotation(Vector3.zero, Quaternion.identity);

			Vector3 addAimLoc = additiveAim.position;
			Quaternion addAimRot = additiveAim.rotation;

			// Base Animation layer
			Vector3 baseLoc = masterTransform.position;
			Quaternion baseRot = masterTransform.rotation;

			CoreToolkitLib.MoveInBoneSpace(masterTransform, masterTransform, addAimLoc, 1f);
			masterTransform.rotation *= addAimRot;
			CoreToolkitLib.MoveInBoneSpace(masterTransform, masterTransform, smoothAimPoint.position, 1f);

			addAimLoc = masterTransform.position;
			addAimRot = masterTransform.rotation;

			ApplyAbsAim(smoothAimPoint.position, smoothAimPoint.rotation);

			// Blend between Absolute and Additive
			masterTransform.position = Vector3.Lerp(masterTransform.position, addAimLoc, aimLayerAlphaLoc);
			masterTransform.rotation = Quaternion.Slerp(masterTransform.rotation, addAimRot, aimLayerAlphaRot);

			float aimWeight = Mathf.Clamp01(smoothAdsAlpha - smoothPointAlpha);

			// Blend Between Non-Aiming and Aiming
			masterTransform.position = Vector3.Lerp(baseLoc, masterTransform.position, aimWeight);
			masterTransform.rotation = Quaternion.Slerp(baseRot, masterTransform.rotation, aimWeight);
		}

		protected virtual void ApplyPointAiming()
		{
			GunAimData aimData = GunData.gunAimData;
			smoothPointAlpha = CoreToolkitLib.GlerpLayer(smoothPointAlpha, pointAlpha * adsAlpha,
														 aimData.aimSpeed);

			CoreToolkitLib.MoveInBoneSpace(RootBone, MasterIK,
										   aimData.pointAimOffset.position * smoothPointAlpha, 1f);

			Quaternion pointAimRot = Quaternion.Slerp(Quaternion.identity, aimData.pointAimOffset.rotation,
													  smoothPointAlpha);

			CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, MasterIK,
											 pointAimRot, 1f);
		}

		protected virtual void ApplyHandsOffset() =>
			CoreToolkitLib.MoveInBoneSpace(RootBone, MasterIK,
										   GunData.handsOffset * (1f - smoothAdsAlpha), 1f);

		// Absolute aiming overrides base animation
		protected virtual void ApplyAbsAim(Vector3 loc, Quaternion rot)
		{
			Vector3 offset = -loc;

			MasterIK.position = aimTarget.position;
			MasterIK.rotation = RootBone.rotation * rot;
			CoreToolkitLib.MoveInBoneSpace(MasterIK, MasterIK, -offset, 1f);
		}
	}
}