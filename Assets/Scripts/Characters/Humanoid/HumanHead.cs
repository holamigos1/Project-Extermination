using UnityEngine;

namespace Characters.Humanoid
{
    public class HumanHead
    {
        public HumanHead(Animator animator)
        {
            _hunanAnimator = animator;
        }

        public Transform HeadTransform => _hunanAnimator.GetBoneTransform(HumanBodyBones.Head);

        private readonly Animator _hunanAnimator;
        
        public void LockAt(Vector3 worldPosition)
        {
            
        }
    }
}