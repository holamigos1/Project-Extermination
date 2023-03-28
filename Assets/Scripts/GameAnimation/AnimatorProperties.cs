using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using Object = UnityEngine.Object;
using Animation;
using AnimatorState = Animation.AnimatorState;

#if UNITY_EDITOR
[Serializable]
public abstract class AnimatorPropertyDrawer : PropertyDrawer
{
    protected int SelectedIndex = 0;
    protected SortedList<int, string> SortedAnimationNames = new SortedList<int, string>();
    protected readonly List<int> AnimationNamesKeys = new List<int>();
    protected readonly List<string> AnimationNamesValues = new List<string>();
    public SerializedProperty HashProperty;
    public SerializedProperty NameProperty;

    private class AnimatorList : List<Animator>
    {
        public bool Created;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (SortedAnimationNames.Count <= 0) return;
        
        AnimationNamesKeys.Clear();
        AnimationNamesValues.Clear();

        AnimationNamesKeys.Add(0);
        AnimationNamesValues.Add("[None]");

        foreach (KeyValuePair<int, string> pair in SortedAnimationNames)
        {
            AnimationNamesKeys.Add(pair.Key);
            AnimationNamesValues.Add(pair.Value);
        }

        EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();

        SelectedIndex = EditorGUI.Popup(position, label.text, SelectedIndex, AnimationNamesValues.ToArray());

        EditorGUI.EndChangeCheck();
        
        HashProperty.intValue = AnimationNamesKeys[SelectedIndex];
        NameProperty.stringValue = AnimationNamesValues[SelectedIndex];
    }
    
    protected static List<Animator> GetAnimators(SerializedProperty property)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        AnimatorList animatorList = GUIUtility.GetStateObject(typeof(AnimatorList), controlID) as AnimatorList;
        
        if(animatorList is { Created: true }) 
            return animatorList;

        foreach (Object obj in property.serializedObject.targetObjects)
        {
            if (obj is not Component component) continue;

            if (component.TryGetComponent<Animator>(out var animator))
                animatorList!.Add(animator);
            else throw new Exception("Animator вешать не забывай");
        }

        animatorList!.Created = true;
        return animatorList;
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        0 < GetAnimators(property)!.Count ?
            base.GetPropertyHeight(property, label) :
            0;
}

[CustomPropertyDrawer(typeof(AnimatorState))]
public class AnimatorStateNameDrawer : AnimatorPropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SortedAnimationNames.Clear();

        HashProperty = property.FindPropertyRelative(nameof(AnimatorState.FullPathHash));
        NameProperty = property.FindPropertyRelative(nameof(AnimatorState.Name));

        foreach (Animator animator in GetAnimators(property))
        {
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                continue;

            foreach (var animatorState in controller.GetStatesNames())
                SortedAnimationNames.Add(animatorState.FullPathHash, animatorState.Name);
        }

        base.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(AnimatorParameter))]
public class AnimatorParameterNameDrawer : AnimatorPropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SortedAnimationNames.Clear();
        
        HashProperty = property.FindPropertyRelative(nameof(AnimatorParameter.Hash));
        NameProperty = property.FindPropertyRelative(nameof(AnimatorParameter.Name));

        foreach (Animator animator in GetAnimators(property))
        {
            if (animator.runtimeAnimatorController is not AnimatorController controller)
                continue;
            
            foreach (AnimatorControllerParameter parameter in controller.parameters)
                SortedAnimationNames.Add(parameter.nameHash, parameter.name);
        }

        base.OnGUI(position, property, label);
    }
}
#endif // UNITY_EDITOR
