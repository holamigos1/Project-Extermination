using System.Collections.Generic;
using UnityEngine;

namespace Systems.Base
{
    public static partial class Extentions
    {
        public static void ChangeFamilyLayout(this GameObject gameObject, int layoutID)
        {
            gameObject.layer = layoutID;
            
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = layoutID;
 
                Transform _HasChildren = child.GetComponentInChildren<Transform>();
                
                if (_HasChildren != null) ChangeFamilyLayout(child.gameObject, layoutID);
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
    }
}