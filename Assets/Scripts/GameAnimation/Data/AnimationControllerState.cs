using System;
using UnityEngine;

namespace GameAnimation.Data
{
    [Serializable]
    public record AnimationControllerState(int FullPathHash, string FullPathName, string Name)
    {
        public int FullPathHash => fullPathHash;
        public string FullPathName => fullPathName;
        public string Name => name;

        [SerializeField] private int fullPathHash = FullPathHash;
        [SerializeField] private string fullPathName = FullPathName; 
        [SerializeField] private string name = Name;

        public static implicit operator int(AnimationControllerState parameter) => parameter.FullPathHash;
        public static implicit operator string(AnimationControllerState parameter) => parameter.Name;
        public override string ToString() => $"{GetType().Name}: {Name} {FullPathHash} {FullPathName}";
        public override int GetHashCode() => Animator.StringToHash(Name);
    }
}