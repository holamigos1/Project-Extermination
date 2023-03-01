using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Base
{
    public static partial class Extentions
    {
        public static void ChangeFamilyLayers(this GameObject gameObject, int layoutID)
        {
            gameObject.layer = layoutID;
            
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = layoutID;
 
                Transform hasChildren = child.GetComponentInChildren<Transform>();
                
                if (hasChildren != null) ChangeFamilyLayers(child.gameObject, layoutID);
            }
        }
        
        public static List<T> GetAs<T>(this List<object> list)
        {
            if (list == null) return null;
            if (list.Count <= 0) return null;
            
            List<T> genericList = new List<T>();
            list.ForEach(obj =>
            {
                var genericObj = (T)obj;
                genericList.Add(genericObj);
            });
            
            return genericList;
        }
        
        public static T GetFirstAs<T>(this List<object> list)
        {
            if (list == null) return default;
            if (list.Count <= 0) return default;
            return (T)list[0];
        }
        
        public static bool IsEmpty(this List<object> list)
        {
            if (list == null) return true;
            if (list.Count <= 0) return true;
            return false;
        }
        
        public static bool HasAnyChild(this Transform transform) =>
            transform.GetChild(0) != null;
        
        public static Transform GetFirstChild(this Transform transform) =>
            transform.GetChild(0);
        
        public static GameObject GetFirstChildObj(this Transform transform) =>
             transform.GetChild(0).gameObject;
        
        public static Bounds GetBounds(this GameObject gameObject)
        {
            Bounds bounds = new Bounds(gameObject.transform.position,Vector3.zero);

            foreach (Transform child in gameObject.transform)
            {
                if (child.TryGetComponent(out Renderer rendererComp)) 
                    bounds.Encapsulate(rendererComp.bounds);
                else bounds.Encapsulate(GetBounds(child.gameObject));
            }
            
            return bounds;
        }
    }
}