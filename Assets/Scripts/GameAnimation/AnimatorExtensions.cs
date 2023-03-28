using Animation;
using AnimatorCache;
using UnityEditor.Animations;
using UnityEngine;
using AnimatorState = Animation.AnimatorState;

public static class AnimatorExtensions
{
    static AnimatorExtensions()
    {
        Debug.Log("аЧЁ");
    }
    
    private static readonly AnimatorStatesCache AnimationStatesCache = new (cacheSize: 1000);
    private static readonly AnimatorLayersCache AnimationLayersCache = new (cacheSize: 1000);
    private static readonly AnimatorParametersCache AnimationParametersCache = new (cacheSize: 1000);
    
    public static string GetCurrentStateName(this Animator animator, int layerIndex)
    {
        if (animator.runtimeAnimatorController is not AnimatorController controller)
            return null;

        int controllerInstanceID = controller.GetInstanceID();

        return null;
    }

    public static AnimatorState[] GetStatesNames(this AnimatorController animator)
    {
        return AnimationStatesCache.LoadStates(animator);
    }
    
    public static AnimatorState[] GetStatesNames(this Animator animator)
    {
        if (animator.runtimeAnimatorController is not AnimatorController controller)
            return null;
        
        return AnimationStatesCache.LoadStates(controller);
    }

    public static AnimatorParameter[] GetParametersNames(this Animator animator)
    {
        if (animator.runtimeAnimatorController is not AnimatorController controller)
            return null;
        
        return AnimationParametersCache.LoadParameters(controller);
    }
}