using System.Collections.Generic;
using GameAnimation.Data;
using UnityEditor.Animations;

namespace GameAnimation.AnimatorCache
{
    internal readonly struct AnimatorLayersCache
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
            int controllerInstanceID = animController.GetInstanceID();
            
            if(_animatorLayersDictionary.ContainsKey(controllerInstanceID) == false)
                _animatorLayersDictionary.Add(controllerInstanceID, new Dictionary<int, string>());
            
            ushort iterator = 0;
            foreach (UnityEditor.Animations.AnimatorControllerLayer layer in animController.layers)
                _animatorLayersDictionary[controllerInstanceID].TryAdd(iterator++, layer.name);
        }

        public AnimationControllerLayer[] LoadLayers(AnimatorController animController)
        {
            int controllerInstanceID = animController.GetInstanceID();
            
            if (_animatorLayersDictionary.ContainsKey(controllerInstanceID) == false)
                    SaveLayers(animController);
            
            var loadedLayerArray = new AnimationControllerLayer[_animatorLayersDictionary[controllerInstanceID].Count];

            int iterator = 0;
            foreach (var parametersPair in _animatorLayersDictionary[controllerInstanceID])
                loadedLayerArray[iterator++] = new AnimationControllerLayer(parametersPair.Key, parametersPair.Value, iterator);
            
            return loadedLayerArray;
        }
    }
}
