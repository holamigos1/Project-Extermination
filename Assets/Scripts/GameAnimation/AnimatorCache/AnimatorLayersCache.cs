using System;
using System.Collections.Generic;
using Animation;
using UnityEditor.Animations;

namespace AnimatorCache
{
    readonly struct AnimatorLayersCache
    {
        public AnimatorLayersCache(int cacheSize)
        {
            //TODO Пытаешься избежать O(n) от переполнения? Но не забудь потом чекнуть память скока реально надо
            _animatorLayersDictionary = new Dictionary<int, Dictionary<int, string>>(cacheSize);
        }

        ///<summary> 1) int Key AnimatorController.GetInstanceID() <br/>2) int Key Index <br/>3) string Value LayerName </summary>
        private readonly Dictionary<int, Dictionary<int, string>> _animatorLayersDictionary;

        public void SaveLayers(AnimatorController animController)
        {
            var iterator = 0;
            foreach (AnimatorControllerLayer layer in animController.layers)
                _animatorLayersDictionary[animController.GetInstanceID()][iterator++] = layer.name;
        }

        public AnimatorLayer[] LoadLayers(AnimatorController animController)
        {
            if (!_animatorLayersDictionary.TryGetValue(animController.GetInstanceID(), 
                    out var foundedLayer))
            {
                SaveLayers(animController);
                AnimatorLayer[] response = LoadLayers(animController);
                return response.Length == 0 ?
                    Array.Empty<AnimatorLayer>() : 
                    response;
            }
            
            var loadedLayerArray = new AnimatorLayer[foundedLayer.Count];

            int iterator = 0;
            foreach (var parametersPair in foundedLayer)
            {
                loadedLayerArray[iterator].Name = parametersPair.Value;
                loadedLayerArray[iterator].LayerIndex = parametersPair.Key;
                iterator++;
            }

            return loadedLayerArray;
        }
    }
}
