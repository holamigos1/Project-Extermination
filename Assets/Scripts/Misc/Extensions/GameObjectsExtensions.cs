using System.Runtime.CompilerServices;
using UnityEngine;

namespace Misc.Extensions
{
    public static class GameObjectsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bounds RenderBounds (this GameObject objTransform) 
        {
            var bounds = new Bounds(objTransform.transform.position, Vector3.zero);
            
            if (objTransform.TryGetComponent(out Renderer rendererComp)) 
                bounds.Encapsulate(rendererComp.bounds);

            foreach (Transform child in objTransform.transform)
                bounds.Encapsulate(child.RenderBounds());
            
            return bounds;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeObjectHierarhyLayers(this GameObject gameObject, string layerName) =>
            ChangeObjectHierarhyLayers(gameObject, LayerMask.NameToLayer(layerName));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeObjectHierarhyLayers(this GameObject gameObject, int layerID)
        {
            gameObject.layer = layerID;
            
            foreach (Transform child in gameObject.transform)
                ChangeObjectHierarhyLayers(child.gameObject, layerID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RaycastHit GetRaycastBlockingObject(this Transform rayStartPos, Vector3 rayDirection, LayerMask rayBlockingMask) =>
            GetRaycastBlockingObject(rayStartPos.position, rayDirection, Mathf.Infinity ,rayBlockingMask); 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RaycastHit GetRaycastBlockingObject(Vector3 rayStartPos, Vector3 rayDirection, float distance, int layerMask)
        {
            var ray = new Ray(rayStartPos, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask).IsFalse()) 
                return new RaycastHit();
            
            if (hitInfo.transform == null) 
                return new RaycastHit();
            
            Debug.DrawRay(rayStartPos, rayDirection * hitInfo.distance, Color.blue, 1f);
            
            return hitInfo;
        }

        public static GameObject ToGameObject(this MonoBehaviour monoBehaviour) =>
            monoBehaviour.gameObject;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetTag(this GameObject gameObject, string tag)
        {
            gameObject.tag = tag;
            return gameObject;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetLayer(this GameObject gameObject, LayerMask layer)
        {
            gameObject.layer = layer;
            return gameObject;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject SetParent(this GameObject gameObject, Transform parent)
        {
            gameObject.transform.parent = parent;
            return gameObject;
        }
    }
}