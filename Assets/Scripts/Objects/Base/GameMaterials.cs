using System;
using System.Collections.Generic;
using GameData.ResourcesPathfs;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Objects.Base
{
    public enum MaterialType
    {
        Defualt,
        Metal,
        Wood,
        Glass
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "Game Materials", menuName = "Game Data/Materials", order = 1)]
    public class GameMaterials : ScriptableObject
    {
        private Dictionary<string, MaterialType> _materialNames;
        
        [TabGroup("Метал")] 
        [AssetSelector] [SerializeField]
        private Material[] _metalMaterials;
        
        [TabGroup("Дерево")] 
        [AssetSelector] [SerializeField]
        private Material[] _woodMaterials;
        
        [TabGroup("Стекло")]
        [AssetSelector] [SerializeField]
        private Material[] _glassMaterials;

        public Dictionary<string, MaterialType> MaterialNames
        {
            get
            {
                if (_materialNames != null) 
                    return _materialNames;
                
                _materialNames = new Dictionary<string, MaterialType>();

                FillMaterialsDictionary(_metalMaterials, MaterialType.Metal);
                FillMaterialsDictionary(_woodMaterials, MaterialType.Wood);
                FillMaterialsDictionary(_glassMaterials, MaterialType.Glass);
                
                void FillMaterialsDictionary(Material[] matArray, MaterialType asType)
                { 
                    foreach (var material in matArray)
                        _materialNames.Add(material.name, asType);
                }

                return _materialNames;
            }
        }
        
        public static GameMaterials ExistingMaterials
        {
            get
            {
                string materialTypesPath  = ResourcesPaths.MATERIAL_TYPES_PATH + "Game Materials";
                string materialTypesSavePath = "Assets/Resources/"+ ResourcesPaths.MATERIAL_TYPES_PATH + "Game Materials.asset";
                
                if (_existingMaterials == null) 
                    _existingMaterials = Resources.Load(materialTypesPath) as GameMaterials;
                
                if (_existingMaterials != null) 
                    return _existingMaterials;
                
                _existingMaterials = CreateInstance<GameMaterials>();
                AssetDatabase.CreateAsset(_existingMaterials, materialTypesSavePath);
                return _existingMaterials;
            }
        }
        
        private static GameMaterials _existingMaterials;

        [MenuItem("Tools/Materials Types")]
        static void Open() =>
            Selection.activeObject = GameMaterials.ExistingMaterials;
    }

    public static partial class Extentions
    {
        public static MaterialType GetMaterialType(this Material material)
        {
            GameMaterials.ExistingMaterials.MaterialNames.TryGetValue(material.name, out MaterialType type);
            
            //TODO Сделать бинарный поиск типа материала
            
            return type;
        }

        public static bool GetMaterialType(this Collision collision, out MaterialType materialType)
        {
            materialType = MaterialType.Defualt;
            bool isContainsRender = collision.gameObject.TryGetComponent(out Renderer renderer);
            
            if (isContainsRender)
                materialType = renderer.sharedMaterial.GetMaterialType();
            
            return isContainsRender;
        }
            
    }
}