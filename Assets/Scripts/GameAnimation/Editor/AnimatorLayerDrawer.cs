using System;
using GameAnimation.Data;
using UnityEditor;
using UnityEngine;

namespace GameAnimation.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorControllerLayer))]
    public class AnimatorLayerDrawer : PropertyDrawer
    {
        private int _selectedIndex;
        private SerializedProperty _indexProperty;
        private SerializedProperty _nameProperty;
        private RuntimeAnimatorController _runtimeAnimatorController;
        private AnimatorControllerLayer[] _savedLayers;

        private string[] GetLayersNames(AnimatorControllerLayer[] layers)
        {
            if (layers.Length == 0) 
                return Array.Empty<string>();
            
            var namesArray = new string[_savedLayers.Length];

            int iterator = 0;
            foreach (AnimatorControllerLayer layer in layers)
                namesArray[iterator++] = layer.Name;

            return namesArray;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _runtimeAnimatorController = property.GetAnimationController();
            if(_runtimeAnimatorController == null) return;
            
            _savedLayers = _runtimeAnimatorController.GetLayers();
            _indexProperty ??= property.FindPropertyRelative("layerIndex");
            _nameProperty ??= property.FindPropertyRelative("name");

            if(_savedLayers.Length == 0) return;
            
            if(_selectedIndex == 0)
                for(int iterator = 0; iterator < _savedLayers.Length; iterator++)
                    if (_savedLayers[iterator] == _indexProperty.intValue)
                        _selectedIndex = iterator;
            
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            EditorGUI.BeginChangeCheck();

            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, GetLayersNames(_savedLayers));
            
            EditorGUI.EndChangeCheck();
            
            _indexProperty.intValue = _savedLayers[_selectedIndex];
            _nameProperty.stringValue = _savedLayers[_selectedIndex].Name;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_savedLayers != null && _savedLayers.Length > 0) return base.GetPropertyHeight(property, label);
            else return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}