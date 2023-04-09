using UnityEditor;

public static class AssetDataBaseExtensions
{
    public static T LoadAssetAtFilter<T>(string filter) where T : UnityEngine.Object
    {
        var assetGuid = AssetDatabase.FindAssets(filter).First();
        return assetGuid == "" ? 
            default(T) : 
            AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuid));
    }
}