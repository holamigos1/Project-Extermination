using System;
using GameAnimation.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameAnimation.Editor
{
    [CustomPropertyDrawer(typeof(AnimationControllerState))]
    public class AnimatorStateDrawer : PropertyDrawer
    {
        private int _selectedIndex;
        private SerializedProperty _hashProperty;
        private SerializedProperty _nameProperty;
        private SerializedProperty _fullPathProperty;
        private AnimationControllerState[] _savedStates;

        private (string[] Names, string[] FullPathNames) GetStatesNames(AnimationControllerState[] states)
        {
            if (states.Length == 0) 
                return (Array.Empty<string>(), Array.Empty<string>());
            
            var namesArray = new string[_savedStates.Length];
            var fullNamesArray = new string[_savedStates.Length];

            int iterator = 0;
            foreach (AnimationControllerState state in states)
            {
                fullNamesArray[iterator] = state.FullPathName;
                namesArray[iterator++] = state.Name;
            }
            
            return (namesArray, fullNamesArray);
        }
        
        private AnimationControllerState[] GetStates(SerializedProperty property)
        {
            if (property.CheckForAnimator(out Animator animator))
                return animator.GetStates();

            //todo проверки по остальным типам
            
            return Array.Empty<AnimationControllerState>();
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _savedStates ??= GetStates(property);
            _hashProperty ??= property.FindPropertyRelative("fullPathHash");
            _nameProperty ??= property.FindPropertyRelative("name");
            _fullPathProperty ??= property.FindPropertyRelative("fullPathName");
            
            if(_savedStates.Length == 0) return;

            if(_selectedIndex == 0)
                for(int iterator = 0; iterator < _savedStates.Length; iterator++)
                    if (_savedStates[iterator] == _hashProperty.intValue)
                        _selectedIndex = iterator;
            
            //EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            var statesNames = GetStatesNames(_savedStates);
            
            EditorGUI.BeginChangeCheck();
            
            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, statesNames.FullPathNames);

            EditorGUI.EndChangeCheck();

            _hashProperty.intValue = _savedStates[_selectedIndex];
            _nameProperty.stringValue = _savedStates[_selectedIndex].Name;
            _fullPathProperty.stringValue = _savedStates[_selectedIndex].FullPathName;
            
            EditorGUI.LabelField(position, new GUIContent(" ", _fullPathProperty.stringValue)); // <-- Displays tooltip
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_savedStates != null && _savedStates.Length > 0) 
                return base.GetPropertyHeight(property, label);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }   
}