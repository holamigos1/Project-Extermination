using System;
using GameAnimation.Data;
using GameAnimation.Sheets.Base;
using UnityEditor;
using UnityEngine;

namespace GameAnimation.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorControllerState))]
    public class AnimatorStateDrawer : PropertyDrawer
    {
        private int _selectedIndex;
        private SerializedProperty _hashProperty;
        private SerializedProperty _hashFullPathProperty;
        private SerializedProperty _nameProperty;
        private SerializedProperty _fullPathProperty;
        private AnimatorControllerState[] _savedStates;
        private RuntimeAnimatorController _runtimeAnimatorController;
        
        private (string[] Names, string[] FullPathNames) GetStatesNames(AnimatorControllerState[] states)
        {
            if (states.Length == 0) 
                return (Array.Empty<string>(), Array.Empty<string>());
            
            var namesArray = new string[_savedStates.Length];
            var fullNamesArray = new string[_savedStates.Length];

            int iterator = 0;
            foreach (AnimatorControllerState state in states)
            {
                fullNamesArray[iterator] = state.FullPathName;
                namesArray[iterator++] = state.Name;
            }
            
            return (namesArray, fullNamesArray);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _runtimeAnimatorController = property.GetAnimationController();
            if(_runtimeAnimatorController == null) return;
            
            _savedStates = _runtimeAnimatorController.GetStates();
            _hashFullPathProperty ??= property.FindPropertyRelative("fullPathHash"); //TODO Если какая нибудь собака поменять имена переменных то всё накроется
            _hashProperty ??= property.FindPropertyRelative("hash");
            _nameProperty ??= property.FindPropertyRelative("name");
            _fullPathProperty ??= property.FindPropertyRelative("fullPathName");
            
            if(_savedStates.Length == 0) return;

            if(_selectedIndex == 0)
                for(int iterator = 0; iterator < _savedStates.Length; iterator++)
                    if (_savedStates[iterator] == _hashFullPathProperty.intValue)
                        _selectedIndex = iterator;

            var statesNames = GetStatesNames(_savedStates);
            
            EditorGUI.BeginChangeCheck();
            
            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, statesNames.FullPathNames);

            EditorGUI.EndChangeCheck();

            _hashFullPathProperty.intValue = _savedStates[_selectedIndex];
            _nameProperty.stringValue = _savedStates[_selectedIndex].Name;
            _fullPathProperty.stringValue = _savedStates[_selectedIndex].FullPathName;
            _hashProperty.intValue = _savedStates[_selectedIndex].Hash;
            
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