using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using Animation;
using UnityEngine;
using AnimatorState = Animation.AnimatorState;

namespace AnimatorCache
{
    readonly struct AnimatorStatesCache
    {
        public AnimatorStatesCache(int cacheSize)
        {
            //TODO Пытаешься избежать O(n) от переполнения? Но не забудь потом чекнуть память скока реально надо
            _animatorStatesDictionary = new Dictionary<int, Dictionary<int, (string FullPath, string StateName)>>(cacheSize);
        }
        
        ///<summary> 1) int AnimatorController.GetInstanceID() <br/>2) int FullPathHash</summary>
        private readonly Dictionary<int, Dictionary<int, (string FullPathName, string StateName)>> _animatorStatesDictionary;
        

        public void SaveStates(AnimatorController animController)
        {
            foreach (AnimatorControllerLayer layer in animController.layers)
            {
                string fullNamePath = layer.name + "." + layer.stateMachine;
                GetStatesNames(animController.GetInstanceID(), layer.stateMachine, fullNamePath);
            }
        }

        //TODO Рекурсия
        private void GetStatesNames(int animControllerInst, AnimatorStateMachine animStateMachine, string fullNamePath)
        {
            foreach (ChildAnimatorState childState in animStateMachine.states)
            {
                fullNamePath = fullNamePath + "." + childState.state.name;
                int pathHash = Animator.StringToHash(fullNamePath);
                _animatorStatesDictionary[animControllerInst][pathHash] = new(fullNamePath, childState.state.name);
            }
            
            foreach (ChildAnimatorStateMachine subStateMachines in animStateMachine.stateMachines)
                GetStatesNames(animControllerInst, 
                    subStateMachines.stateMachine,
                    fullNamePath + "." + subStateMachines.stateMachine.name);
        }

        public AnimatorState[] LoadStates(AnimatorController animController)
        {
            if (!_animatorStatesDictionary.TryGetValue(animController.GetInstanceID(), 
                    out var foundedStatesPairs))
            {
                SaveStates(animController);
                AnimatorState[] response = LoadStates(animController);
                return response.Length == 0 ?
                    Array.Empty<AnimatorState>() : 
                    response;
            }

            var loadedStatesArray = new AnimatorState[foundedStatesPairs.Count];
        
            //TODO если стейтов будет больше 1000 в контроллерах то надо заюзать Dictionary.AsParallel().ForAll(); (наверное это будет очень не скоро)
            
            int iterator = 0;
            foreach (var parametersPair in foundedStatesPairs) 
            {
                loadedStatesArray[iterator].Name = parametersPair.Value.StateName;
                loadedStatesArray[iterator].FullPathHash = parametersPair.Key;
                iterator++;
            }

            return loadedStatesArray;
        }
    }
}