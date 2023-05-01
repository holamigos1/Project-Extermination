// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class BlendingLayer : AnimLayer
    {
        // Source static pose
        [SerializeField] protected AnimationClip anim;
        // Character ref
        [SerializeField] protected GameObject character;
        [SerializeField] protected Transform spineRootBone;
        [SerializeField] protected Transform rootBone;
        [SerializeField] protected Quaternion spineBoneRotMS;

        // MS: mesh space
        public virtual void EvaluateSpineMS()
        {
            if (character == null || anim == null || rootBone == null || spineRootBone == null)
            {
                return;
            }

            Vector3 cachedLoc = character.transform.position;
            anim.SampleAnimation(character, 0f);
            character.transform.position = cachedLoc;

            // Convert spine rotation to mesh space
            spineBoneRotMS = Quaternion.Inverse(rootBone.rotation) * spineRootBone.rotation;
        }

        public override void OnPreAnimUpdate()
        {
            base.OnPreAnimUpdate();
            
            spineRootBone.rotation = Quaternion.Slerp(spineRootBone.rotation,
                rootBone.rotation * spineBoneRotMS, smoothLayerAlpha);
        }
    }
}