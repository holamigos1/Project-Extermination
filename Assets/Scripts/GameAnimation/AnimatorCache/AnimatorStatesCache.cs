using System.Collections.Generic;
using GameAnimation.Data;
using UnityEditor.Animations;
using UnityEngine;

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
            int controllerInstanceID = animController.GetInstanceID();
            
            if(_animatorStatesDictionary.ContainsKey(controllerInstanceID) == false)
                _animatorStatesDictionary.Add(controllerInstanceID, new Dictionary<int, (string, string)>());
            
            foreach (AnimatorControllerLayer layer in animController.layers)
                GetStatesNames(controllerInstanceID, layer.stateMachine, layer.name);
        }

        //TODO Рекурсия, чекай стек
        private void GetStatesNames(int animControllerInst, AnimatorStateMachine animStateMachine, string fullNamePath)
        {
            foreach (ChildAnimatorState childState in animStateMachine.states)
                _animatorStatesDictionary[animControllerInst].
                    TryAdd(Animator.StringToHash(fullNamePath + "." + childState.state.name), 
                        (fullNamePath + "." + childState.state.name, childState.state.name));

            foreach (ChildAnimatorStateMachine subStateMachines in animStateMachine.stateMachines)
                GetStatesNames(animControllerInst,
                    subStateMachines.stateMachine,
                    fullNamePath + "." + subStateMachines.stateMachine.name);
        }

        public AnimationControllerState[] LoadStates(AnimatorController animController)
        {
            int controllerInstanceID = animController.GetInstanceID();
            
            if (_animatorStatesDictionary.ContainsKey(controllerInstanceID) == false)
                    SaveStates(animController);

            var loadedStatesArray = new AnimationControllerState[_animatorStatesDictionary[controllerInstanceID].Count];
        
            //TODO если стейтов будет больше 1000 в контроллерах то надо заюзать Dictionary.AsParallel().ForAll(); (наверное это будет очень не скоро)
            
            int iterator = 0;
            foreach (var parametersPair in _animatorStatesDictionary[controllerInstanceID])
                loadedStatesArray[iterator++] = 
                    new AnimationControllerState(parametersPair.Key, parametersPair.Value.FullPathName, parametersPair.Value.StateName);
            
            return loadedStatesArray;
        }
    }
}