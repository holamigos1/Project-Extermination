using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Misc
{
    public static class AssetDataBaseExtensions
    {
        /// <summary>Выгружает массив ссылок на объекты Unity переданного типа TObj из AssetDatabase</summary>
        /// <typeparam name="TObj">Тип наследник UnityEngine.Object</typeparam>
        /// <returns>Массив TObj[] если таковые ассеты существуют в проекте, иначе вернёт пустой массив TObj[]</returns>
        public static TObj[] LoadAssets<TObj>() 
            where TObj : UnityEngine.Object
        {
            if (TryFindAssetGuids(typeof(TObj), out string[] assetsGuids) == false)
                return Array.Empty<TObj>();
            
            var response = new TObj[assetsGuids.Length];

            for (int iterator = assetsGuids.Length - 1; iterator >= 0; iterator--)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetsGuids[iterator]);
                response[iterator] = AssetDatabase.LoadAssetAtPath<TObj>(assetPath);
            }

            return response;
        }

        /// <summary>Сохраняет ScriptableObject в заданной директории.</summary>
        /// <param name="savePath">Путь сохранения в папке Assets. Убедитесь что в пути указано имя объекта и расширение .asset , e.g. ../Object.asset</param>
        /// <typeparam name="TScriptObj">Тип наследник ScriptableObject</typeparam>
        /// <returns>Сохранённый экземпляр объекта TScriptObj</returns>
        public static TScriptObj SaveAsset<TScriptObj>(string[] savePath) 
            where TScriptObj : ScriptableObject
        {
            if (!ContainsExtension(savePath[^1], ".asset"))
                return null;
            
            var objInstance = ScriptableObject.CreateInstance<TScriptObj>();
            CreateAssetDirectory(savePath[0..^1]);
            AssetDatabase.CreateAsset(objInstance, string.Join('/', savePath));
            return objInstance;
        }

        /// <summary>Создаёт директорию в папке Assets корня проекта.</summary>
        /// <param name="directoryFolders">Путь создаваемой директории в папке Assets. Если по заданному пути первой будет папка Assets, то она будет пропущена.</param>
        /// <returns>Возвращает true если директория создалась, а если путь уже существовал вернёт false.</returns>
        public static bool CreateAssetDirectory(string[] directoryFolders)
        {
            string currentPath = "Assets";

            if (directoryFolders[0] == currentPath)
                directoryFolders[0] = "";
                
            if (AssetDatabase.IsValidFolder(currentPath +'/'+ string.Join('/', directoryFolders)))
                return false;

            foreach (string folder in directoryFolders)
            {
                if(string.IsNullOrEmpty(folder)) 
                    continue;
                
                if (AssetDatabase.IsValidFolder(currentPath +'/'+ folder) == false)        
                    AssetDatabase.CreateFolder(currentPath, folder);

                currentPath += '/' + folder;
            }

            return true;
        }

        private static bool TryFindAssetGuids(Type assetType, out string[] foundedGuids)
        {
            string preparedFilter = AssetDBFilters.TypeFilter(assetType);
            foundedGuids = AssetDatabase.FindAssets(preparedFilter);
            return string.IsNullOrEmpty(foundedGuids.First()) == false;
        }
        
        private static bool ContainsExtension(string path, string extension)
        {
            if (Path.GetExtension(path) == extension)
                return true;
            
            Debug.LogError($"Переданный путь ({path}) не содержит в себе расширения {extension}!");
            return false;
        }
    }

    public struct AssetDBFilters
    {
        //вряд-ли юнитеки будут менять эти префиксы
        private const string TypePrefix = "t:";
        private const string AssetLabelPrefix = "l:";
        private const string RefPrefix = "ref:";
        public static string RefIDFilter(int instanceID) =>
            RefPrefix + instanceID;

        public static string TypeFilter(Type type) =>
            TypePrefix + type.Name;
        
        public static string AssetLabelFilter(string labelName) =>
            AssetLabelPrefix + labelName;
    }
}