using System;
using GameAnimation.AnimatorCache;
using GameAnimation.Data;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorControllerLayer = GameAnimation.Data.AnimatorControllerLayer;
using AnimatorControllerParameter = GameAnimation.Data.AnimatorControllerParameter;

namespace GameAnimation
{
    public static class AnimatorExtensions
    {
        static AnimatorExtensions()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                _animationStatesCache = new(cacheSize: 1000);
                _animationLayersCache = new(cacheSize: 1000);
                _animationParametersCache = new(cacheSize: 1000);
            };
        }

        //TODO Придумать куда убрать этот кеш
    
        private static AnimatorStatesCache _animationStatesCache = new (cacheSize: 1000);
        private static AnimatorLayersCache _animationLayersCache = new (cacheSize: 1000);
        private static AnimatorParametersCache _animationParametersCache = new (cacheSize: 1000);
    
        public static string GetCurrentStateName(this Animator animator, int layerIndex)
        {
            if (animator.runtimeAnimatorController == null) 
                return null;
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;

            int controllerInstanceID = controller.GetInstanceID();

            return null;
        }

        public static AnimatorControllerState[] GetStates(this AnimatorController animator) =>
            _animationStatesCache.LoadStates(animator);
    
        public static AnimatorControllerParameter[] GetParameters(this AnimatorController animator) =>
            _animationParametersCache.LoadParameters(animator);
    
        public static AnimatorControllerLayer[] GetLayers(this AnimatorController animator) =>
            _animationLayersCache.LoadLayers(animator);
    
        public static AnimatorControllerState[] GetStates(this RuntimeAnimatorController animator) =>
            _animationStatesCache.LoadStates(animator as AnimatorController);
    
        public static AnimatorControllerParameter[] GetParameters(this RuntimeAnimatorController animator) =>
            _animationParametersCache.LoadParameters(animator as AnimatorController);
    
        public static AnimatorControllerLayer[] GetLayers(this RuntimeAnimatorController animator) =>
            _animationLayersCache.LoadLayers(animator as AnimatorController);
    
    
        public static AnimatorControllerState[] GetStates(this Animator animator)
        {
            if (animator.runtimeAnimatorController == null) 
                return Array.Empty<AnimatorControllerState>();
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;
        
            return _animationStatesCache.LoadStates(controller);
        }

        public static AnimatorControllerParameter[] GetParameters(this Animator animator)
        {
            if (animator.runtimeAnimatorController == null) 
                return Array.Empty<AnimatorControllerParameter>();
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;
        
            return _animationParametersCache.LoadParameters(controller);
        }
    
        public static AnimatorControllerLayer[] GetLayers(this Animator animator)
        {
            if (animator.runtimeAnimatorController == null) 
                return Array.Empty<AnimatorControllerLayer>();
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;
        
            return _animationLayersCache.LoadLayers(controller);
        }
    }
}