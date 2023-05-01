using System.Collections.Generic;
using UnityEngine;

namespace GameObjects
{
    public static class GameObjectExtensions
    {
        public static List<T> GetComponentsInAllChildren<T>(this Transform transform) 
            where T : MonoBehaviour
        {
            List<T> componentList = new List<T>();

            foreach (Transform childrenTransform in transform)
            {
                T[] components = childrenTransform.GetComponents<T>();
     
                foreach (T component in components)
                    if (component != null)
                        componentList.Add(component);
                
                GetComponentsInAllChildren<T>(childrenTransform);
            }
     
            return componentList;
        }
        
        public static List<T> GetComponentsInAllChildren<T>(this Transform transform, List<T> componentList) 
            where T : MonoBehaviour
        {
            foreach (Transform childrenTransform in transform)
            {
                T[] components = childrenTransform.GetComponents<T>();
     
                foreach (T component in components)
                    if (component != null)
                        componentList.Add(component);
                
                GetComponentsInAllChildren(childrenTransform, componentList);
            }
     
            return componentList;
        }
    }
}