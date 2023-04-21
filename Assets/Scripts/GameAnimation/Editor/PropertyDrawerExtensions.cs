﻿using GameAnimation.Sheets.Base;
using UnityEditor;
using UnityEngine;

namespace GameAnimation.Editor
{
    public static class PropertyDrawerExtensions
    {
        public static bool CheckForAnimator(this SerializedProperty property, out Animator animator)
        {
            foreach (Object targetObj in property.serializedObject.targetObjects)
            {
                if (targetObj is not Component component) 
                    continue;
                
                if (false == component.TryGetComponent<Animator>(out var animatorComponent)) 
                    break;
                
                animator = animatorComponent;
                return true;
            }

            animator = null;
            return false;
        }
        
        public static RuntimeAnimatorController GetAnimationController(this SerializedProperty property)
        {
            var animatorSheet = 
                property.serializedObject.targetObject as AnimatorParametersSheet;
            
            return animatorSheet != null ?
                animatorSheet.TargetController : 
                null;
        }
    }
}