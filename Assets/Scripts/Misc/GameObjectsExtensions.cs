using UnityEngine;

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
}