// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class SwayLayer : AnimLayer
	{
		[Header("Deadzone Rotation"), SerializeField]
        
		protected Transform headBone;

		[SerializeField] protected FreeAimData freeAimData;
		[SerializeField] protected bool        bFreeAim;
		[SerializeField] protected bool        useCircleMethod;

		protected Quaternion deadZoneRot;
		protected Vector2    deadZoneRotTarget;

		protected float   smoothFreeAimAlpha;
		protected Vector3 smoothMoveSwayLoc;

		protected Vector3 smoothMoveSwayRot;
		protected Vector3 swayLoc;
		protected Vector3 swayRot;

		protected Vector2 swayTarget;

		public void SetFreeAimEnable(bool enable) =>
			bFreeAim = enable;

		public override void OnAnimUpdate()
		{
			if (Mathf.Approximately(Time.deltaTime, 0f))
				return;

			Transform master = MasterIK;
			var baseT = new LocationAndRotation(master.position, master.rotation);

			freeAimData = GunData.freeAimData;

			ApplySway();
			ApplyFreeAim();
			ApplyMoveSway();

			var newT = new LocationAndRotation(MasterIK.position, MasterIK.rotation);

			MasterIK.position = Vector3.Lerp(baseT.position, newT.position, SmoothLayerAlpha);
			MasterIK.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, SmoothLayerAlpha);
		}

		protected virtual void ApplyFreeAim()
		{
			float deltaRight = CharData.deltaAimInput.x;
			float deltaUp = CharData.deltaAimInput.y;

			if (bFreeAim)
			{
				deadZoneRotTarget.x += deltaUp * freeAimData.scalar;
				deadZoneRotTarget.y += deltaRight * freeAimData.scalar;
			}
			else
			{
				deadZoneRotTarget = Vector2.zero;
			}

			deadZoneRotTarget.x = Mathf.Clamp(deadZoneRotTarget.x, -freeAimData.maxValue, freeAimData.maxValue);

			if (useCircleMethod)
			{
				float maxY = Mathf.Sqrt(Mathf.Pow(freeAimData.maxValue, 2f) - Mathf.Pow(deadZoneRotTarget.x, 2f));
				deadZoneRotTarget.y = Mathf.Clamp(deadZoneRotTarget.y, -maxY, maxY);
			}
			else
			{
				deadZoneRotTarget.y = Mathf.Clamp(deadZoneRotTarget.y, -freeAimData.maxValue, freeAimData.maxValue);
			}

			deadZoneRot.x = CoreToolkitLib.Glerp(deadZoneRot.x, deadZoneRotTarget.x, freeAimData.speed);
			deadZoneRot.y = CoreToolkitLib.Glerp(deadZoneRot.y, deadZoneRotTarget.y, freeAimData.speed);

			Quaternion q = Quaternion.Euler(new Vector3(deadZoneRot.x, deadZoneRot.y, 0f));
			q.Normalize();

			smoothFreeAimAlpha = CoreToolkitLib.Glerp(smoothFreeAimAlpha, bFreeAim ?
														  1f :
														  0f, 10f);
			q = Quaternion.Slerp(Quaternion.identity, q, smoothFreeAimAlpha);

			CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, headBone, q, 1f);
		}

		protected virtual void ApplySway()
		{
			Transform masterDynamic = MasterIK;

			float deltaRight = CoreAnim.rigData.characterData.deltaAimInput.x / Time.deltaTime;
			float deltaUp = CoreAnim.rigData.characterData.deltaAimInput.y / Time.deltaTime;

			swayTarget += new Vector2(deltaRight, deltaUp) * 0.01f;
			swayTarget.x = CoreToolkitLib.GlerpLayer(swayTarget.x * 0.01f, 0f, 5f);
			swayTarget.y = CoreToolkitLib.GlerpLayer(swayTarget.y * 0.01f, 0f, 5f);

			var targetLoc = new Vector3(swayTarget.x, swayTarget.y, 0f);
			var targetRot = new Vector3(swayTarget.y, swayTarget.x, swayTarget.x);

			swayLoc = CoreToolkitLib.SpringInterp(swayLoc, targetLoc, ref CoreAnim.rigData.gunData.springData.location);
			swayRot = CoreToolkitLib.SpringInterp(swayRot, targetRot, ref CoreAnim.rigData.gunData.springData.rotation);

			Quaternion rot = CoreAnim.rigData.rootBone.rotation;

			CoreToolkitLib.RotateInBoneSpace(rot, masterDynamic, Quaternion.Euler(swayRot), 1f);
			CoreToolkitLib.MoveInBoneSpace(CoreAnim.rigData.rootBone, masterDynamic, swayLoc, 1f);
		}

		protected virtual void ApplyMoveSway()
		{
			var moveRotTarget = new Vector3();
			var moveLocTarget = new Vector3();

			MoveSwayData moveSwayData = GunData.moveSwayData;
			Vector2 moveInput = CharData.moveInput;

			moveRotTarget.x = moveInput.y * moveSwayData.maxMoveRotationSway.x;
			moveRotTarget.y = moveInput.x * moveSwayData.maxMoveRotationSway.y;
			moveRotTarget.z = moveInput.x * moveSwayData.maxMoveRotationSway.z;

			moveLocTarget.x = moveInput.x * moveSwayData.maxMoveLocationSway.x;
			moveLocTarget.y = moveInput.y * moveSwayData.maxMoveLocationSway.y;
			moveLocTarget.z = moveInput.y * moveSwayData.maxMoveLocationSway.z;

			smoothMoveSwayRot.x = CoreToolkitLib.Glerp(smoothMoveSwayRot.x, moveRotTarget.x, 3.8f);
			smoothMoveSwayRot.y = CoreToolkitLib.Glerp(smoothMoveSwayRot.y, moveRotTarget.y, 3f);
			smoothMoveSwayRot.z = CoreToolkitLib.Glerp(smoothMoveSwayRot.z, moveRotTarget.z, 5f);

			smoothMoveSwayLoc.x = CoreToolkitLib.Glerp(smoothMoveSwayLoc.x, moveLocTarget.x, 2.2f);
			smoothMoveSwayLoc.y = CoreToolkitLib.Glerp(smoothMoveSwayLoc.y, moveLocTarget.y, 3f);
			smoothMoveSwayLoc.z = CoreToolkitLib.Glerp(smoothMoveSwayLoc.z, moveLocTarget.z, 2.5f);

			CoreToolkitLib.MoveInBoneSpace(CoreAnim.rigData.rootBone, MasterIK,
										   smoothMoveSwayLoc, 1f);
			CoreToolkitLib.RotateInBoneSpace(MasterIK.rotation, MasterIK,
											 Quaternion.Euler(smoothMoveSwayRot), 1f);
		}
	}
}