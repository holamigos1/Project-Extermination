// Designed by Kinemation, 2023

using Plugins.Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;
using Weapons.Data;

namespace Plugins.Kinemation.FPSFramework.Runtime.Layers
{
	public class LegIK : AnimLayer
	{
		[SerializeField] protected float footTraceLength;
		[SerializeField] protected float footInterpSpeed;
		[SerializeField] protected float pelvisInterpSpeed;

		[SerializeField] protected LayerMask        layerName;
		protected                  LocationAndRotation smoothLfIK;
		protected                  float            smoothPelvis;

		protected LocationAndRotation smoothRfIK;
		protected Vector3          traceEnd;

		protected Vector3 traceStart;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(traceStart, traceEnd);
		}

		private LocationAndRotation TraceFoot(Transform footTransform)
		{
			Vector3 origin = footTransform.position;
			origin.y = Pelvis_bone.position.y;

			traceStart = origin;
			traceEnd = traceStart - RootBone.up * footTraceLength;

			var target = new LocationAndRotation(footTransform.position, footTransform.rotation);
			Quaternion finalRotation = footTransform.rotation;
			if (Physics.Raycast(origin, -RootBone.up, out RaycastHit hit, footTraceLength, layerName))
			{
				Quaternion rotation = footTransform.rotation;
				finalRotation = Quaternion.FromToRotation(RootBone.up, hit.normal) * rotation;
				finalRotation.Normalize();
				target.position = hit.point;

				float animOffset = RootBone.InverseTransformPoint(footTransform.position).y;
				target.position.y += animOffset;
			}

			target.position -= footTransform.position;
			target.rotation = Quaternion.Inverse(footTransform.rotation) * finalRotation;

			return target;
		}

		public override void OnAnimUpdate()
		{
			Transform rightFoot = Right_foot_bone._boneObject.transform;
			Transform leftFoot = Left_foot_bone._boneObject.transform;

			Vector3 rf = rightFoot.position;
			Vector3 lf = leftFoot.position;

			LocationAndRotation rfIK = TraceFoot(rightFoot);
			LocationAndRotation lfIK = TraceFoot(leftFoot);

			smoothRfIK = CoreToolkitLib.Glerp(smoothRfIK, rfIK, footInterpSpeed);
			smoothLfIK = CoreToolkitLib.Glerp(smoothLfIK, lfIK, footInterpSpeed);

			CoreToolkitLib.MoveInBoneSpace(RootBone, rightFoot, smoothRfIK.position, SmoothLayerAlpha);
			CoreToolkitLib.MoveInBoneSpace(RootBone, leftFoot, smoothLfIK.position, SmoothLayerAlpha);

			rightFoot.rotation *= Quaternion.Slerp(Quaternion.identity, smoothRfIK.rotation, SmoothLayerAlpha);
			leftFoot.rotation *= Quaternion.Slerp(Quaternion.identity, smoothLfIK.rotation, SmoothLayerAlpha);

			Vector3 dtR = rightFoot.position - rf;
			Vector3 dtL = leftFoot.position - lf;

			float pelvisOffset = dtR.y < dtL.y ?
				dtR.y :
				dtL.y;
			smoothPelvis = CoreToolkitLib.Glerp(smoothPelvis, pelvisOffset, pelvisInterpSpeed);

			CoreToolkitLib.MoveInBoneSpace(RootBone, Pelvis_bone,
										   new Vector3(0f, smoothPelvis, 0f), SmoothLayerAlpha);
		}
	}
}