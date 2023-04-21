using System;
using UnityEngine;

namespace GameAnimation.Data
{
    [Serializable]
    public record AnimatorControllerParameter(int Hash, string Name, AnimatorControllerParameterType ParameterType)
    {
        public int Hash => hash;
        public string Name => name;
        public AnimatorControllerParameterType ParameterType => parameterType;

        [SerializeField] private int hash = Hash;
        [SerializeField] private string name = Name;
        [SerializeField] private AnimatorControllerParameterType parameterType = ParameterType;
        
        public static implicit operator int(AnimatorControllerParameter parameter) => parameter.Hash;
        public static implicit operator string(AnimatorControllerParameter parameter) => parameter.Name;
        public override string ToString() => $"{GetType().Name}: {Name} {Hash} {ParameterType}";
        public override int GetHashCode() => Hash;
    };
}