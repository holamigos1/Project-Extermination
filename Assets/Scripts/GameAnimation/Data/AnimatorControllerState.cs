using System;
using UnityEngine;

namespace GameAnimation.Data
{
    [Serializable]
    public record AnimatorControllerState(int FullPathHash, string FullPathName, string Name)
    {
        public int Hash => hash;
        public int FullPathHash => fullPathHash;
        public string FullPathName => fullPathName;
        public string Name => name;

        [SerializeField] private int fullPathHash = FullPathHash;
        [SerializeField] private string fullPathName = FullPathName; 
        [SerializeField] private string name = Name;
        [SerializeField] private int hash = Animator.StringToHash(Name);

        public static implicit operator int(AnimatorControllerState parameter) => parameter.FullPathHash;
        public static implicit operator string(AnimatorControllerState parameter) => parameter.Name;
        public override string ToString() => $"{GetType().Name}: {Name} {FullPathHash} {FullPathName}";
        public override int GetHashCode() => Hash;
    }
}