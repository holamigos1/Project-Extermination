using System.Collections.Generic;
using GameAnimation;
using GameAnimation.Data;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimatorCache
{
    readonly struct AnimatorParametersCache
    {
        public AnimatorParametersCache(int cacheSize)
        {
            //TODO Пытаешься избежать O(n) от переполнения? Но не забудь потом чекнуть память скока реально надо
            _animatorParametersDictionary =
                new Dictionary<int, Dictionary<int, (string Name, AnimatorControllerParameterType Type)>>(cacheSize);
        }
        
        ///<summary> 1) int AnimatorController.GetInstanceID() <br/>2) int FullPathHash</summary>
        private readonly Dictionary<int, Dictionary<int, (string Name, AnimatorControllerParameterType Type)>>
            _animatorParametersDictionary;

        public void SaveParameters(AnimatorController animController)
        {
            int controllerInstanceID = animController.GetInstanceID();
            
            if(_animatorParametersDictionary.ContainsKey(controllerInstanceID) == false)
                _animatorParametersDictionary.Add(controllerInstanceID, new Dictionary<int, (string, AnimatorControllerParameterType)>());

            foreach (var animParameter in animController.parameters)
                _animatorParametersDictionary[controllerInstanceID]
                    .TryAdd(animParameter.nameHash, (animParameter.name, animParameter.type));
        }

        public AnimationControllerParameter[] LoadParameters(AnimatorController animController)
        {
            int controllerInstanceID = animController.GetInstanceID();
            
            if (_animatorParametersDictionary.ContainsKey(controllerInstanceID) == false)
                    SaveParameters(animController);

            var loadedParametersArray = new AnimationControllerParameter[_animatorParametersDictionary[controllerInstanceID].Count];

            int iterator = 0;
            foreach (var parametersPair in _animatorParametersDictionary[controllerInstanceID])
                loadedParametersArray[iterator++] = 
                    new AnimationControllerParameter(parametersPair.Key, parametersPair.Value.Name, parametersPair.Value.Type);
            
            return loadedParametersArray;
        }
    }
}
