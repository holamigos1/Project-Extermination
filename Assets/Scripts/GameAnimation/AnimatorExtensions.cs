﻿using System;
using GameAnimation.AnimatorCache;
using GameAnimation.Data;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

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

        public static AnimationControllerState[] GetStates(this AnimatorController animator) =>
            _animationStatesCache.LoadStates(animator);
    
        public static AnimationControllerParameter[] GetParameters(this AnimatorController animator) =>
            _animationParametersCache.LoadParameters(animator);
    
        public static AnimationControllerLayer[] GetLayers(this AnimatorController animator) =>
            _animationLayersCache.LoadLayers(animator);
    
        public static AnimationControllerState[] GetStates(this RuntimeAnimatorController animator) =>
            _animationStatesCache.LoadStates(animator as AnimatorController);
    
        public static AnimationControllerParameter[] GetParameters(this RuntimeAnimatorController animator) =>
            _animationParametersCache.LoadParameters(animator as AnimatorController);
    
        public static AnimationControllerLayer[] GetLayers(this RuntimeAnimatorController animator) =>
            _animationLayersCache.LoadLayers(animator as AnimatorController);
    
    
        public static AnimationControllerState[] GetStates(this Animator animator)
        {
            if (animator.runtimeAnimatorController == null) 
                return Array.Empty<AnimationControllerState>();
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;
        
            return _animationStatesCache.LoadStates(controller);
        }

        public static AnimationControllerParameter[] GetParameters(this Animator animator)
        {
            if (animator.runtimeAnimatorController == null) 
                return Array.Empty<AnimationControllerParameter>();
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;
        
            return _animationParametersCache.LoadParameters(controller);
        }
    
        public static AnimationControllerLayer[] GetLayers(this Animator animator)
        {
            if (animator.runtimeAnimatorController == null) 
                return Array.Empty<AnimationControllerLayer>();
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                return null;
        
            return _animationLayersCache.LoadLayers(controller);
        }
    }
}