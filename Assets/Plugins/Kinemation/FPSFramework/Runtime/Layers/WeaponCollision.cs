// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class WeaponCollision : AnimLayer
	{
		[SerializeField] protected LayerMask        layerMask;
		protected                  Vector3          end;
		protected                  LocationAndRotation smoothPose;

		protected Vector3 start;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(start, end);
		}

		public override void OnAnimUpdate()
		{
			float traceLength = GunData.blockData.weaponLength;
			float startOffset = GunData.blockData.startOffset;
			float threshold = GunData.blockData.threshold;
			LocationAndRotation restPose = GunData.blockData.restPose;

			start = MasterIK.position - MasterIK.forward * startOffset;
			end = start + MasterIK.forward * traceLength;
			var offsetPose = new LocationAndRotation(Vector3.zero, Quaternion.identity);

			if (Physics.Raycast(start, MasterIK.forward, out RaycastHit hit, traceLength, layerMask))
			{
				float distance = (end - start).magnitude - (hit.point - start).magnitude;
				if (distance > threshold)
				{
					offsetPose = restPose;
				}
				else
				{
					offsetPose.position = new Vector3(0f, 0f, -distance);
					offsetPose.rotation = Quaternion.Euler(0f, 0f, 15f * (distance / threshold));
				}
			}

			smoothPose = CoreToolkitLib.Glerp(smoothPose, offsetPose, 10f);

			CoreToolkitLib.MoveInBoneSpace(MasterIK, MasterIK, smoothPose.position,
										   SmoothLayerAlpha);
			CoreToolkitLib.RotateInBoneSpace(MasterIK.rotation, MasterIK, smoothPose.rotation,
											 SmoothLayerAlpha);
		}
	}
}