// Designed by Kinemation, 2023

using Kinemation.FPSFramework.Runtime.Layers;
using UnityEditor;
using UnityEngine;

namespace Kinemation.FPSFramework.Editor.Layers
{
    [CustomEditor(typeof(LookLayer), true)]
    public class LookLayerEditor : UnityEditor.Editor
    {
        private string[] tabs = {"Blending", "Aim Offset", "Leaning"};
        private int selectedTab;
        
        private SerializedProperty runInEditor;
        private SerializedProperty layerAlpha;
        private SerializedProperty lerpSpeed;
        private SerializedProperty handsAlpha;
        private SerializedProperty handsLerp;
        private SerializedProperty pelvisAlpha;
        private SerializedProperty pelvisLerp;
        
        private SerializedProperty pelvisOffset;
        private SerializedProperty lookUpOffset;
        private SerializedProperty lookRightOffset;
        private SerializedProperty enableAutoDistribution;
        private SerializedProperty enableManualSpineControl;
        private SerializedProperty aimUp;
        private SerializedProperty aimRight;
        private SerializedProperty smoothAim;
        
        private SerializedProperty leanDirection;
        private SerializedProperty leanAmount;
        private SerializedProperty leanSpeed;
        
        private SerializedProperty detectZeroFrames;
        private SerializedProperty checkZeroFootIK;
        private SerializedProperty useRightOffset;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }
            
            runInEditor = serializedObject.FindProperty("runInEditor");
            layerAlpha = serializedObject.FindProperty("layerAlpha");
            lerpSpeed = serializedObject.FindProperty("lerpSpeed");
            handsAlpha = serializedObject.FindProperty("handsLayerAlpha");
            handsLerp = serializedObject.FindProperty("handsLerpSpeed");
            pelvisAlpha = serializedObject.FindProperty("pelvisLayerAlpha");
            pelvisLerp = serializedObject.FindProperty("pelvisLerpSpeed");

            pelvisOffset = serializedObject.FindProperty("pelvisOffset");
            lookUpOffset = serializedObject.FindProperty("lookUpOffset");
            lookRightOffset = serializedObject.FindProperty("lookRightOffset");
            enableAutoDistribution = serializedObject.FindProperty("enableAutoDistribution");
            enableManualSpineControl = serializedObject.FindProperty("enableManualSpineControl");
            aimUp = serializedObject.FindProperty("aimUp");
            aimRight = serializedObject.FindProperty("aimRight");
            smoothAim = serializedObject.FindProperty("smoothAim");

            leanDirection = serializedObject.FindProperty("leanDirection");
            leanAmount = serializedObject.FindProperty("leanAmount");
            leanSpeed = serializedObject.FindProperty("leanSpeed");

            detectZeroFrames = serializedObject.FindProperty("detectZeroFrames");
            checkZeroFootIK = serializedObject.FindProperty("checkZeroFootIK");
            useRightOffset = serializedObject.FindProperty("useRightOffset");
        }

        private void DrawBlendingTab()
        {
            EditorGUILayout.PropertyField(layerAlpha);
            EditorGUILayout.PropertyField(lerpSpeed);
            EditorGUILayout.PropertyField(handsAlpha);
            EditorGUILayout.PropertyField(handsLerp);
            EditorGUILayout.PropertyField(pelvisAlpha);
            EditorGUILayout.PropertyField(pelvisLerp);
        }
        
        private void DrawOffsetTab()
        {
            EditorGUILayout.PropertyField(pelvisOffset);
            EditorGUILayout.PropertyField(lookUpOffset);
            EditorGUILayout.PropertyField(lookRightOffset);
            EditorGUILayout.PropertyField(enableAutoDistribution);
            EditorGUILayout.PropertyField(enableManualSpineControl);
            EditorGUILayout.PropertyField(aimUp);
            EditorGUILayout.PropertyField(aimRight);
            EditorGUILayout.PropertyField(smoothAim);
        }
        
        private void DrawLeanTab()
        {
            EditorGUILayout.PropertyField(leanDirection);
            EditorGUILayout.PropertyField(leanAmount);
            EditorGUILayout.PropertyField(leanSpeed);
        }

        private void DrawDefault()
        {
            EditorGUILayout.PropertyField(detectZeroFrames);
            EditorGUILayout.PropertyField(checkZeroFootIK);
            EditorGUILayout.PropertyField(useRightOffset);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(runInEditor);
            GUILayout.BeginVertical();
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);
            GUILayout.EndVertical();
            switch (selectedTab)
            {
                case 0:
                    DrawBlendingTab();
                    break;
                case 1:
                    DrawOffsetTab();
                    break;
                case 2:
                    DrawLeanTab();
                    break;
            }

            DrawDefault();
            serializedObject.ApplyModifiedProperties();
        }
    }
}