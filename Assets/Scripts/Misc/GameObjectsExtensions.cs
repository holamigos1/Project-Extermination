using UnityEngine;

namespace Misc
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
        
        public static GameObject GetRaycastBlockingObj(this Transform rayStartPos, Vector3 rayEnd, LayerMask rayBlockingMask) =>
            GetRaycastBlockingObj(rayStartPos.position, rayEnd, rayBlockingMask);
        
        public static GameObject GetRaycastBlockingObj(Vector3 rayStartPos, Vector3 rayEnd, int layerMask)
        {
            float distance = Vector3.Distance(rayStartPos, rayEnd);

            rayEnd = rayEnd - rayStartPos;
            Debug.DrawRay(rayStartPos, rayEnd, Color.blue, 1f);
            Ray ray = new Ray(rayStartPos, rayEnd);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask) == false) 
                return null;
            
            if (hitInfo.transform == null) 
                return null;
            
            return hitInfo.transform.gameObject;
        }
    }
}