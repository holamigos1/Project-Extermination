using System;
using UnityEngine;

namespace GameAnimation.Data
{
    [Serializable]
    public record AnimationControllerLayer(int Hash, string Name, int LayerIndex)
    {
        public int Hash => hash;
        public int LayerIndex => layerIndex;
        public string Name => name;
        
        [SerializeField] private int hash  = Hash;
        [SerializeField]  private int layerIndex  = LayerIndex;
        [SerializeField] private string name = Name;

        public static implicit operator int(AnimationControllerLayer parameter) => parameter.LayerIndex;
        public static implicit operator string(AnimationControllerLayer parameter) => parameter.Name;
        public override string ToString() => $"{GetType().Name}: {Name} {Hash} {LayerIndex}";
        public override int GetHashCode() => Hash;
    }
}