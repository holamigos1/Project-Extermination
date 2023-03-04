using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Base
{
    public static partial class Extentions
    {
        public static void ChangeGameObjsLayers(this GameObject gameObject, string layerName)
        {
            int layersID = LayerMask.NameToLayer(layerName);
            
            gameObject.layer = layersID;
            
            foreach (Transform child in gameObject.transform)
                ChangeGameObjsLayers(child.gameObject, layersID);
        }
        
        public static void ChangeGameObjsLayers(this GameObject gameObject, int layerID)
        {
            gameObject.layer = layerID;
            
            foreach (Transform child in gameObject.transform)
                ChangeGameObjsLayers(child.gameObject, layerID);
        }
        
        public static List<T> GetAs<T>(this List<object> list)
        {
            if (list == null) return null;
            if (list.Count <= 0) return null;
            
            List<T> genericList = new List<T>();

            foreach (var obj in list)
            {
                if (obj.GetType() != typeof(T)) continue;
                genericList.Add((T)obj);
            }
            
            return genericList;
        }
        
        public static T GetFirstAs<T>(this List<object> list)
        {
            if (list == null) return default;
            if (list.Count <= 0) return default;
            if (list[0].GetType() == typeof(T)) return (T)list[0];
            return default;
        }
        
        public static bool IsEmpty(this List<object> list)
        {
            if (list == null) return true;
            if (list.Count <= 0) return true;
            return false;
        }
        
        public static bool HasChild (this Transform transform) =>
            transform.childCount != 0;
        
        public static Transform GetFirstChild (this Transform transform) =>
            transform.GetChild(0);
        
        public static GameObject GetFirstChildObj (this Transform transform) =>
             transform.GetChild(0).gameObject;
        
        public static Bounds RenderBounds (this Transform objTransform) 
        {
            Bounds bounds = new Bounds(objTransform.position, Vector3.zero);
            
            if (objTransform.TryGetComponent(out Renderer rendererComp)) 
                bounds.Encapsulate(rendererComp.bounds);

            foreach (Transform child in objTransform.transform)
                bounds.Encapsulate(child.RenderBounds());
            
            return bounds;
        }
        
        public static Bounds RenderBounds (this GameObject objTransform) 
        {
            Bounds bounds = new Bounds(objTransform.transform.position, Vector3.zero);
            
            if (objTransform.TryGetComponent(out Renderer rendererComp)) 
                bounds.Encapsulate(rendererComp.bounds);

            foreach (Transform child in objTransform.transform)
                bounds.Encapsulate(child.RenderBounds());
            
            return bounds;
        }
    }
}