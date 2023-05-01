// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class RecoilLayer : AnimLayer
    {
        [SerializeField] private bool useMeshSpace;

        private RecoilAnimation recoilComponent;

        public override void OnAnimStart()
        {
            recoilComponent = gameObject.GetComponent<RecoilAnimation>();
        }

        public override void OnAnimUpdate()
        {
            var masterDynamic = GetMasterIK();
            var recoilAnim = new LocRot(recoilComponent.OutLoc,Quaternion.Euler(recoilComponent.OutRot));
            
            LocRot baseT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            if (useMeshSpace)
            {
                CoreToolkitLib.MoveInBoneSpace(GetRootBone(), masterDynamic,
                    recoilAnim.position, 1f);
                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, masterDynamic,
                    recoilAnim.rotation, 1f);
            }
            else
            {
                CoreToolkitLib.MoveInBoneSpace(masterDynamic, masterDynamic,
                    recoilAnim.position, 1f);
                CoreToolkitLib.RotateInBoneSpace(masterDynamic.rotation, masterDynamic,
                    recoilAnim.rotation, 1f);
            }
            
            LocRot newT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, smoothLayerAlpha);
            masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, smoothLayerAlpha);
        }
    }
}
