using UnityEngine;

namespace Misc.Extensions
{
    public static class TransformExtensions
    {
        public static bool HasChild(this Transform transform) =>
            transform.childCount != 0;
        
        public static Transform GetFirstChild (this Transform transform) =>
            transform.GetChild(0);
        
        public static GameObject GetFirstChildObj (this Transform transform) =>
            transform.GetChild(0).gameObject;
        
        public static Bounds RenderBounds (this Transform objTransform) 
        {
            var bounds = new Bounds(objTransform.position, Vector3.zero);
            
            if (objTransform.TryGetComponent(out Renderer rendererComp)) 
                bounds.Encapsulate(rendererComp.bounds);

            foreach (Transform child in objTransform.transform)
                bounds.Encapsulate(child.RenderBounds());
            
            return bounds;
        }
    }
}