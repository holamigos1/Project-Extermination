// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class WeaponCollision : AnimLayer
    {
        [SerializeField] protected LayerMask layerMask;
        
        protected Vector3 start;
        protected Vector3 end;
        protected LocRot smoothPose;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, end);
        }

        public override void OnAnimUpdate()
        {
            float traceLength = GetGunData().blockData.weaponLength;
            float startOffset = GetGunData().blockData.startOffset;
            float threshold = GetGunData().blockData.threshold;
            LocRot restPose = GetGunData().blockData.restPose;
            
            start = GetMasterIK().position - GetMasterIK().forward * startOffset;
            end = start + GetMasterIK().forward * traceLength;
            LocRot offsetPose = new LocRot(Vector3.zero, Quaternion.identity);
            
            if (Physics.Raycast(start, GetMasterIK().forward, out RaycastHit hit, traceLength, layerMask))
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
            
            CoreToolkitLib.MoveInBoneSpace(GetMasterIK(), GetMasterIK(), smoothPose.position, 
                smoothLayerAlpha);
            CoreToolkitLib.RotateInBoneSpace(GetMasterIK().rotation, GetMasterIK(), smoothPose.rotation, 
                smoothLayerAlpha);
        }
    }
}