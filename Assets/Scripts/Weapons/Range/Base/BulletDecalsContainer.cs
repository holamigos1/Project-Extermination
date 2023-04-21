using System;
using GameData.ResourcesPathfs;
using GameObjects.Base;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons.Range.Base
{
    [Serializable]
    [CreateAssetMenu(fileName = "Bullet Holes Data", menuName = "Scriptable Data/Bullet Holes", order = 1)]
    public class BulletDecalsContainer : ScriptableObject
    {
        public static BulletDecalsContainer existingBulletDecals
        {
            get
            {
                string decalsAssetPath = ResourcesPaths.DECALS_PATH + "Bullet Holes Data";
                string decalsAssetSavePath = "Assets/Resources/"+ ResourcesPaths.DECALS_PATH + "Bullet Holes Data.asset";
                
                if (_existingBulletDecals == null) 
                    _existingBulletDecals = Resources.Load(decalsAssetPath) as BulletDecalsContainer;
                
                if (_existingBulletDecals != null)
                    return _existingBulletDecals;
                
                _existingBulletDecals = CreateInstance<BulletDecalsContainer>();
                AssetDatabase.CreateAsset(_existingBulletDecals, decalsAssetSavePath);
                return _existingBulletDecals;
            }
        }
        private static BulletDecalsContainer _existingBulletDecals;
        
        
        [InfoBox("Спрайты пулевых отверстий для материала без типа!")] 
        [TabGroup("По умолчанию")]
        [SerializeField] [AssetSelector]
        private Sprite[] DefualtBulletDecals;
        
        [TabGroup("Метал")] 
        [SerializeField] [AssetSelector]   
        private Sprite[] MetalBulletDecals;
        
        [TabGroup("Дерево")]
        [SerializeField] [AssetSelector]  
        private Sprite[] WoodBulletDecals;
        
        [TabGroup("Стекло")] 
        [SerializeField] [AssetSelector]  
        private Sprite[] GlassBulletDecals;
        
        [TabGroup("Мясо & Плоть")] 
        [SerializeField] [AssetSelector]  
        private Sprite[] FleshBulletDecals;

        private static Sprite[] GetDecalsSprites(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Defualt: return existingBulletDecals.DefualtBulletDecals;
                case MaterialType.Metal: return existingBulletDecals.MetalBulletDecals;
                case MaterialType.Wood: return existingBulletDecals.WoodBulletDecals;
                case MaterialType.Glass: return existingBulletDecals.GlassBulletDecals;
                case MaterialType.Flesh: return existingBulletDecals.FleshBulletDecals;
                default: return existingBulletDecals.DefualtBulletDecals;
            }
        }
        
        public static Sprite GetBulletHoleSprite(MaterialType materialType)
        {
            Sprite[] bulletHolesSprites = GetDecalsSprites(materialType);
            int randomElem = Random.Range(0, bulletHolesSprites.Length);
            return bulletHolesSprites[randomElem];
        }
        
        
        [MenuItem("Tools/Bullet Holes")]
        static void Open() =>
            Selection.activeObject = BulletDecalsContainer.existingBulletDecals;
    }
}