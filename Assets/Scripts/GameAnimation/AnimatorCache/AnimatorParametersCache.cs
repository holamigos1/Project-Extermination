using System;
using System.Collections.Generic;
using Animation;
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
            foreach (var animParameter in animController.parameters)
                _animatorParametersDictionary[animController.GetInstanceID()][animParameter.nameHash] =
                    new(animParameter.name, animParameter.type);
        }

        public AnimatorParameter[] LoadParameters(AnimatorController animController)
        {
            if (!_animatorParametersDictionary.TryGetValue(animController.GetInstanceID(), 
                    out var foundedParameter))
            {
                SaveParameters(animController);
                AnimatorParameter[] response = LoadParameters(animController);
                return response.Length == 0 ?
                    Array.Empty<AnimatorParameter>() : 
                    response;
            }

            var loadedParametersArray = new AnimatorParameter[foundedParameter.Count];

            int iterator = 0;
            foreach (var parametersPair in foundedParameter)
            {
                loadedParametersArray[iterator].Hash = parametersPair.Key;
                loadedParametersArray[iterator].Name = parametersPair.Value.Name;
                loadedParametersArray[iterator].ParameterType = parametersPair.Value.Type;
                iterator++;
            }

            return loadedParametersArray;
        }
    }
}
