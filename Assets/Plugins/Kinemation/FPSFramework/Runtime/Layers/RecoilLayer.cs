// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class RecoilLayer : AnimLayer
	{
		[SerializeField] private bool useMeshSpace;

		private RecoilAnimation recoilComponent;

		public override void OnAnimStart() =>
			recoilComponent = gameObject.GetComponent<RecoilAnimation>();

		public override void OnAnimUpdate()
		{
			Transform masterDynamic = MasterIK;
			var recoilAnim = new LocationAndRotation(recoilComponent.OutLocation, Quaternion.Euler(recoilComponent.OutRotation));

			var baseT = new LocationAndRotation(masterDynamic.position, masterDynamic.rotation);

			if (useMeshSpace)
			{
				CoreToolkitLib.MoveInBoneSpace(RootBone, masterDynamic,
											   recoilAnim.position, 1f);
				CoreToolkitLib.RotateInBoneSpace(RootBone.rotation, masterDynamic,
												 recoilAnim.rotation, 1f);
			}
			else
			{
				CoreToolkitLib.MoveInBoneSpace(masterDynamic, masterDynamic,
											   recoilAnim.position, 1f);
				CoreToolkitLib.RotateInBoneSpace(masterDynamic.rotation, masterDynamic,
												 recoilAnim.rotation, 1f);
			}

			var newT = new LocationAndRotation(masterDynamic.position, masterDynamic.rotation);

			masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, SmoothLayerAlpha);
			masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, SmoothLayerAlpha);
		}
	}
}