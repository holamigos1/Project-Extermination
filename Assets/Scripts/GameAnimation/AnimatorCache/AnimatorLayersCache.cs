using System.Collections.Generic;
using GameAnimation.Data;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorControllerLayer = GameAnimation.Data.AnimatorControllerLayer;

namespace GameAnimation.AnimatorCache
{
    internal readonly struct AnimatorLayersCache
    {
        public AnimatorLayersCache(int cacheSize)
        {
            //TODO Пытаешься избежать O(n) от переполнения? Но не забудь потом чекнуть память скока реально надо
            _animatorLayersDictionary = new Dictionary<int, AnimatorControllerLayer[]>(cacheSize);
        }

        ///<summary> 1) int Key AnimatorController.GetInstanceID() <br/>2) int Key Index <br/>3) string Value LayerName </summary>
        private readonly Dictionary<int, AnimatorControllerLayer[]> _animatorLayersDictionary;

        public void SaveLayers(AnimatorController animController)
        {
            int controllerInstanceID = animController.GetInstanceID();
            
            if(_animatorLayersDictionary.ContainsKey(controllerInstanceID) == false)
                _animatorLayersDictionary.Add(controllerInstanceID, new AnimatorControllerLayer[animController.layers.Length]);
            
            ushort iterator = 0;
            foreach (UnityEditor.Animations.AnimatorControllerLayer layer in animController.layers)
                _animatorLayersDictionary[controllerInstanceID][iterator] = new AnimatorControllerLayer(iterator++, layer.name);
        }

        public AnimatorControllerLayer[] LoadLayers(AnimatorController animController)
        {
            int controllerInstanceID = animController.GetInstanceID();
            
            if (_animatorLayersDictionary.ContainsKey(controllerInstanceID) == false)
                    SaveLayers(animController);
            
            return _animatorLayersDictionary[controllerInstanceID];
        }
    }
}
