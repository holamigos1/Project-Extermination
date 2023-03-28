using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Animation
{
    [Serializable]
    public struct AnimatorState
    {
        public int FullPathHash;
        public string Name;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(AnimatorState parameter) => parameter.FullPathHash;
        public override string ToString() => $"{GetType().Name}: {Name}";
    }


    [Serializable]
    public struct AnimatorParameter
    {
        public int Hash;
        public string Name;
        public AnimatorControllerParameterType ParameterType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(AnimatorParameter parameter) => parameter.Hash;
        public override string ToString() => $"{GetType().Name}: {Name}";
    }
    
    
    [Serializable]
    public struct AnimatorLayer
    {
        public int Hash;
        public int LayerIndex;
        public string Name;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(AnimatorLayer parameter) => parameter.LayerIndex;
        public override string ToString() => $"{GetType().Name}: {Name}";
    }
}