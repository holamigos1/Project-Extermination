using System;
using Misc.InterfaceReference.Attributes;
using UnityEngine;

namespace Misc.InterfaceReference
{
    /// <summary> Создайте пользовательский класс который наследуется от этого InterfaceReference, чтобы ссылаться на интерфейс типа I. <br/>
    /// &lt;I&gt; должен быть интерфейсом. <br/>
    /// Полезен для хранения ссылок на компоненты/объекты типа t; </summary>
    /// <typeparam name="I">должен быть классом (where I : class)</typeparam>
    [Serializable]
    public class InterfaceReference<I> : ISerializationCallbackReceiver 
        where I : class
    {
        public UnityEngine.Object Target
        {
            get => target;
            set
            {
                SetTarget(value);
                SetName(target);
            }
        }
        
        public I Interface => Target != null ? 
            (I)(object)Target :
            null;

        [SerializeField, NotEditable] 
        private string name = null;
        
        /// <summary>
        /// Exposed field assignable from inspector
        /// </summary>
        [SerializeField] 
        private UnityEngine.Object target = null;

        private void SetTarget(UnityEngine.Object value)
        {
            if (value == null)
            {
                target = null;
                return;
            }
                
            if (value is I)
            {
                target = value;
            }
            else
            {
                target = null;                
                Debug.LogWarning($"You should set an Object that implements <i>{typeof(I).FullName}</i>.");
            } 
        }

        private void SetName(UnityEngine.Object obj) =>
            name = obj != null ?
                $"{obj.name} ({typeof(I).Name})" : 
                "None";
        

    #region ISerializationCallbackReceiver Implementation
        
        public void OnBeforeSerialize()
        {
            CheckTarget();
            SetName(Target);
        }

        private void CheckTarget()
        {
            if (Target == null)
            {
                return;
            }
            
            //For a comfy Drag n drop of a GameObject
            if (Target is GameObject)
            {
                var gameObject = (GameObject) target;
                Target = gameObject.GetComponent(typeof(I));
                
                if (Target == null)
                {
                    Debug.LogWarning($"You must assign a gameObject that have at least one component of type <i>{typeof(I).FullName}</i>.");
                    return;
                }
            }
            
            if (Target is not I)
            {
                //skip assignment by property during edit mode.
                target = null;
                Debug.LogWarning($"You must assign an Object that implements <i>{typeof(I).FullName}</i>.");
            }
        }

        public void OnAfterDeserialize()
        {
            
        }
        
    #endregion

    #region Operators
        
        public static implicit operator I(InterfaceReference<I> interfaceReference) =>
            interfaceReference.Interface;

    #endregion

        public override string ToString() =>
            $"{typeof(InterfaceReference<I>).Name} {name}";
    }
}
