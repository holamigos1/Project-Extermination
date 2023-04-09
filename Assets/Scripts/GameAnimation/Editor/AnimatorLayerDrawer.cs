using System;
using GameAnimation.Data;
using GameAnimation.Sheets.Base;
using UnityEditor;
using UnityEngine;

namespace GameAnimation.Editor
{
    [CustomPropertyDrawer(typeof(AnimationControllerLayer))]
    public class AnimatorLayerDrawer : PropertyDrawer
    {
        private int _selectedIndex;
        private SerializedProperty _hashProperty;
        private SerializedProperty _indexProperty;
        private SerializedProperty _nameProperty;
        private AnimationControllerLayer[] _savedLayers;

        private string[] GetLayersNames(AnimationControllerLayer[] layers)
        {
            if (layers.Length == 0) 
                return Array.Empty<string>();
            
            var namesArray = new string[_savedLayers.Length];

            int iterator = 0;
            foreach (AnimationControllerLayer layer in layers)
                namesArray[iterator++] = layer.Name;

            return namesArray;
        }
        
        private AnimationControllerLayer[] GetLayers(SerializedProperty property)
        {
            if (property.CheckForAnimator(out Animator animator))
                return animator.GetLayers();
            
            var animatorSheet = property.serializedObject.targetObject as AnimatorParametersSheet;
            if (animatorSheet != null) return animatorSheet.TargetController.GetLayers();

            //todo проверки по остальным типам
            
            return Array.Empty<AnimationControllerLayer>();
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _savedLayers ??= GetLayers(property);
            _indexProperty ??= property.FindPropertyRelative("layerIndex");
            _nameProperty ??= property.FindPropertyRelative("name");
            _hashProperty ??=  property.FindPropertyRelative("hash");
            
            if(_savedLayers.Length == 0) return;
            
            if(_selectedIndex == 0)
                for(int iterator = 0; iterator < _savedLayers.Length; iterator++)
                    if (_savedLayers[iterator] == _hashProperty.intValue)
                        _selectedIndex = iterator;
            
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            EditorGUI.BeginChangeCheck();

            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, GetLayersNames(_savedLayers));
            
            EditorGUI.EndChangeCheck();

            _hashProperty.intValue = _savedLayers[_selectedIndex].Hash;
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