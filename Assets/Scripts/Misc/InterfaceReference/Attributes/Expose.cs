using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Misc.InterfaceReference.Attributes
{
    /// <summary>
    /// Purpose: after declaring a custom Interface Reference you can directly expose the target
    /// in inspector without have the 'foldout'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ExposeAttribute : PropertyAttribute
    { }
}

#if UNITY_EDITOR
namespace Misc.InterfaceReference.Attributes
{
    /// <summary>
    /// Custom drawer of ExposeAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ExposeAttribute))]
    public class ExposeAttributeDrawer : PropertyDrawer
    {
        private SerializedProperty _serializedTarget = null;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded && property.hasChildren);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var exposeAttribute = attribute as ExposeAttribute;
            
            if (property.propertyType == SerializedPropertyType.Generic)
            {
                _serializedTarget ??= property.FindPropertyRelative("target");

                if (_serializedTarget != null)
                {
                    if (exposeAttribute != null)
                        EditorGUIUtils.DrawProperty(position, _serializedTarget, label);
                    
                    else
                        Debug.LogError("Failed to assign the attribute.");
                }
                else
                    Debug.LogWarning($"Expose Attribute supports only Interface Reference fields. (Or fields with a sub-{nameof(SerializedProperty)} with name <i>target</i>");
                
            }
            else
            {
                Debug.LogWarning("Serialized Property is not of Generic type.");
            }
        }
    }
}
#endif