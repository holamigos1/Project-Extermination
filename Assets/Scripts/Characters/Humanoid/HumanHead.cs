using UnityEngine;

namespace Characters.Humanoid
{
    public class HumanHead
    {
        public HumanHead(Animator animator)
        {
            _hunanAnimator = animator;
            _headTransform = _hunanAnimator.GetBoneTransform(HumanBodyBones.Head);
        }

        public Transform HeadTransform => _headTransform;

        private readonly Transform _headTransform;
        private readonly Animator _hunanAnimator;
        
        public void LockAt(Vector3 worldPosition)
        {
            
        }
    }
}