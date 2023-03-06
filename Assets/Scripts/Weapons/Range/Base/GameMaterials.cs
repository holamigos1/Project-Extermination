using System.Threading.Tasks;
using GameData.ResourcesPathfs;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Weapons.Range.Base
{
    public enum MaterialType
    {
        Defualt,
        Metal,
        Wood,
        Glass
    }
    
    [CreateAssetMenu(fileName = "Game Materials", menuName = "Game Data/Materials", order = 1)]
    public class GameMaterials : ScriptableObject
    {
        [TabGroup("Метал")]  public Material[] MetalMaterials;
        [TabGroup("Дерево")] public Material[] WoodMaterials;
        [TabGroup("Стекло")] public Material[] GlassMaterials;
        
        public static GameMaterials ExistingMaterials
        {
            get
            {
                string fullAssetPath = ResourcesPaths.MATERIAL_TYPES_PATH + "Game Materials";
                
                if (_existingMaterials == null) _existingMaterials = (GameMaterials) Resources.Load(fullAssetPath);
                else return _existingMaterials;
                if (_existingMaterials != null) return _existingMaterials;
                
                GameMaterials inst = CreateInstance<GameMaterials>();
                AssetDatabase.CreateAsset(inst, "Assets/Resources/"+ ResourcesPaths.MATERIAL_TYPES_PATH + "Game Materials.asset");
                _existingMaterials = inst;
                return inst;
            }
        }
        
        [MenuItem("Tools/Materials Types")]
        static void Open() =>
            Selection.activeObject = GameMaterials.ExistingMaterials;
        

        private static GameMaterials _existingMaterials;
    }

    public static partial class Extentions
    {
        public static MaterialType GetMaterialType(this Material material)
        {
            MaterialType resultType = MaterialType.Defualt;
            
            if(GameMaterials.ExistingMaterials.MetalMaterials != null)
                Parallel.ForEach(GameMaterials.ExistingMaterials.MetalMaterials, (mat) =>
                    { if (mat.name == material.name) resultType = MaterialType.Metal; });
            
            if(GameMaterials.ExistingMaterials.WoodMaterials != null)
                Parallel.ForEach(GameMaterials.ExistingMaterials.WoodMaterials, (mat) => 
                    { if (mat.name == material.name) resultType = MaterialType.Wood; });
            
            if(GameMaterials.ExistingMaterials.GlassMaterials != null)
                Parallel.ForEach(GameMaterials.ExistingMaterials.GlassMaterials, (mat) => 
                    { if (mat.name == material.name) resultType = MaterialType.Glass; });

            return resultType;
        }

        public static MaterialType GetMaterialType(this Collision collision) =>
            collision.gameObject.TryGetComponent(out Renderer renderer) is true ? 
                renderer.sharedMaterial.GetMaterialType() : 
                MaterialType.Defualt;
        }
}