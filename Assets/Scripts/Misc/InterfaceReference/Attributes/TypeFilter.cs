using System;
using System.Linq;
using Misc.Extensions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Misc.InterfaceReference.Attributes
{
    /// <summary>
    /// Generic attribute to make a property of Object type to filter types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TypeFilterAttribute : PropertyAttribute
    {
        public readonly Type[] Types;

        public TypeFilterAttribute(params Type[] types)
        {
            this.Types = types;
        }
    }
}

#if UNITY_EDITOR
namespace Misc.InterfaceReference.Attributes
{
    /// <summary>
    /// Custom drawer of TypeFilterAttribute. 
    /// </summary>
    [CustomPropertyDrawer(typeof(TypeFilterAttribute))]
    public class TypeFilterAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label, property.isExpanded && property.hasChildren);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeFilterAttribute = attribute as TypeFilterAttribute;
            
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUIUtils.DrawProperty(position, property, label);
                
                if (typeFilterAttribute != null)
                {
                    Type[] requiredTypes = typeFilterAttribute.Types;
                    
                #region Preliminary Check

                    PreliminaryCheck(property, requiredTypes);
                    
                #endregion
                    
                #region Default Check

                    DefaultCheck(property, requiredTypes);

                #endregion
                }
                else
                {
                    Debug.LogError("Failed to assign the attribute.");
                }
            }
            else
            {
                Debug.LogWarning("Type Filter Attribute supports only Object Reference fields.");
            }
        }
        
    #region Preliminary Check
        /// <summary>
        /// For a comfy Drag n drop of a GameObject.
        /// It takes the first component that match with one of the required types.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="requiredTypes"></param>
        private static void PreliminaryCheck(SerializedProperty property, Type[] requiredTypes)
        {
            UnityEngine.Object target = property.objectReferenceValue;
            if (target == null)
            {
                return;
            }
            
            if (target is GameObject targetGameObject)
            {
                Component targetComponent = null;

                if (requiredTypes?.Length > 0)
                {
                    foreach (Type requiredType in requiredTypes)
                    {
                        if (requiredType == null)
                            continue;

                        targetComponent = targetGameObject.GetComponent(requiredType);
                        
                        if (targetComponent != null)
                            break;
                    }
                }

                if (targetComponent != null)
                {
                    property.objectReferenceValue = targetComponent;
                }
                else
                {
                    Debug.LogWarning("You must assign a GameObject that has at least one component of the required types that you have specified.");
                }
            }
        }
        
    #endregion

    #region Default Check
        
        /// <summary>
        /// Check if property object reference match with one of the required types otherwise set it to null.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="requiredTypes"></param>
        private void DefaultCheck(SerializedProperty property, Type[] requiredTypes)
        {
            UnityEngine.Object target = property.objectReferenceValue;
            
            if (target == null)
                return;

            if (requiredTypes?.Length > 0)
            {
                Type objectType = target.GetType();
                bool check = requiredTypes.Where(requiredType => requiredType != null).Any(requiredType => requiredType.IsAssignableFrom(objectType));

                if (!check)
                {
                    property.objectReferenceValue = null;
                }
            }            
        }
        
    #endregion
    }
}
#endif