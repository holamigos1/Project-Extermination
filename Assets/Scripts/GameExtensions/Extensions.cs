using System.Collections.Generic;
using UnityEngine;

namespace GameExtensions
{
    public static class Extensions
    {
        //TODO ????? 
        public static Color GetEnableToggleColor(bool isEnabled)
        {
            return isEnabled ? 
                new Color(0.32f, 0.75f, 0.5f) : 
                new Color(0.8f, 0.17f, 0.129f);
        }
        
        public static GameObject GetRaycastBlockingObj(this Transform rayStartPos, Vector3 rayEnd, int layerMask) =>
            GetRaycastBlockingObj(rayStartPos.position, rayEnd, layerMask);
        
        public static GameObject GetRaycastBlockingObj(Vector3 rayStartPos, Vector3 rayEnd, int layerMask)
        {
            Ray ray = new Ray(rayStartPos, rayEnd);

            float distance = Vector3.Distance(rayStartPos, rayEnd);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask) == false) 
                return null;
            
            if (hitInfo.transform == null) 
                return null;
            
            return hitInfo.transform.gameObject;
        }
    }
}