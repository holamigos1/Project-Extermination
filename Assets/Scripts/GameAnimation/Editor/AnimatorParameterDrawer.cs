using System;
using GameAnimation.Data;
using UnityEditor;
using UnityEngine;

namespace GameAnimation.Editor
{
    [CustomPropertyDrawer(typeof(AnimationControllerParameter))]
    public class AnimatorParameterDrawer : PropertyDrawer
    {
        private int _selectedIndex;
        private SerializedProperty _hashProperty;
        private SerializedProperty _nameProperty;
        private SerializedProperty _typeEnumProperty;
        private AnimationControllerParameter[] _savedParameters;

        private string[] GetParameterNames(AnimationControllerParameter[] parameters)
        {
            if (parameters.Length == 0) 
                return Array.Empty<string>();
            
            var namesArray = new string[_savedParameters.Length];

            int iterator = 0;
            foreach (AnimationControllerParameter parameter in parameters)
                namesArray[iterator++] = parameter.Name;

            return namesArray;
        }
        
        private AnimationControllerParameter[] GetParameters(SerializedProperty property)
        {
            if (property.CheckForAnimator(out Animator animator))
                return animator.GetParameters();
            
            //todo проверки по остальным типам
            
            return Array.Empty<AnimationControllerParameter>();
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _savedParameters ??= GetParameters(property);
            _hashProperty ??= property.FindPropertyRelative("hash");
            _nameProperty ??= property.FindPropertyRelative("name");
            _typeEnumProperty ??= property.FindPropertyRelative("parameterType");
            
            if(_savedParameters.Length == 0) return;
            
            if(_selectedIndex == 0)
                for(int iterator = 0; iterator < _savedParameters.Length; iterator++)
                    if (_savedParameters[iterator] == _hashProperty.intValue)
                        _selectedIndex = iterator;

            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            EditorGUI.BeginChangeCheck();

            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, GetParameterNames(_savedParameters));

            EditorGUI.EndChangeCheck();
        
            _hashProperty.intValue = _savedParameters[_selectedIndex];
            _nameProperty.stringValue = _savedParameters[_selectedIndex].Name;
            _typeEnumProperty.intValue = (int)_savedParameters[_selectedIndex].ParameterType;

            string parameterType = Enum.GetName(typeof(AnimatorControllerParameterType), _typeEnumProperty.intValue);
            EditorGUI.LabelField(position, new GUIContent(" ",parameterType));
        }
            
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_savedParameters != null && _savedParameters.Length > 0) return base.GetPropertyHeight(property, label);
            else return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}