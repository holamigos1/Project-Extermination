using System;
using UnityEngine;

namespace GameAnimation.Data
{
    [Serializable]
    public record AnimatorControllerLayer(int LayerIndex, string Name)
    {
        public int LayerIndex => layerIndex;
        public string Name => name;
        
        [SerializeField] private int layerIndex  = LayerIndex;
        [SerializeField] private string name = Name;

        public static implicit operator int(AnimatorControllerLayer parameter) => parameter.LayerIndex;
        public static implicit operator string(AnimatorControllerLayer parameter) => parameter.Name;
        public override string ToString() => $"{GetType().Name}: {Name} INDEX:{LayerIndex}";
    }
}