using UnityEngine;

namespace Misc.Extensions
{
    public static class GameObjectsExtensions
    {
        public static Bounds RenderBounds (this GameObject objTransform) 
        {
            Bounds bounds = new Bounds(objTransform.transform.position, Vector3.zero);
            
            if (objTransform.TryGetComponent(out Renderer rendererComp)) 
                bounds.Encapsulate(rendererComp.bounds);

            foreach (Transform child in objTransform.transform)
                bounds.Encapsulate(child.RenderBounds());
            
            return bounds;
        }
    
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

        public static RaycastHit GetRaycastBlockingObj(this Transform rayStartPos, Vector3 rayDirection, LayerMask rayBlockingMask) =>
            GetRaycastBlockingObj(rayStartPos.position, rayDirection, 1000f ,rayBlockingMask); //todo magic 1000f

        public static RaycastHit GetRaycastBlockingObj(Vector3 rayStartPos, Vector3 rayDirection, float distance, int layerMask)
        {
            Ray ray = new Ray(rayStartPos, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask) == false) 
                return new RaycastHit();
            
            if (hitInfo.transform == null) 
                return new RaycastHit();
            
            Debug.DrawRay(rayStartPos, rayDirection * hitInfo.distance, Color.blue, 1f);
            
            return hitInfo;
        }   
    }
}