using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Misc.Extensions
{
    public static class AssetDataBaseExtensions
    {
        /// <summary>Выгружает массив ссылок на объекты Unity переданного типа TObj из AssetDatabase</summary>
        /// <typeparam name="TObj">Тип наследник UnityEngine.Object</typeparam>
        /// <returns>Массив TObj[] если таковые ассеты существуют в проекте, иначе вернёт пустой массив TObj[]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>Сохраняет ScriptableObject по заданной директории в папке Assets.</summary>
        /// <param name="savePath">Путь сохранения в папке Assets. Последний элемент в заданном пути воспринимается как имя создаваемого файла, расширение прописывать не обязательно. Если по заданному пути первой будет папка Assets, то она будет пропущена.</param>
        /// <typeparam name="TScriptObj">Тип наследник ScriptableObject</typeparam>
        /// <returns>Сохранённый экземпляр объекта TScriptObj</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TScriptObj CreateAsset<TScriptObj>(string[] savePath) 
            where TScriptObj : ScriptableObject
        {
            if (savePath == null || savePath.Length == 0)
                throw new ArgumentNullException($"Переданный путь сохранения {typeof(TScriptObj).Name} пустой!");
            
            savePath[Index.FromEnd(1)] = AdjustFileExtension(savePath[Index.FromEnd(1)], ".asset");

            TScriptObj objInstance = ScriptableObject.CreateInstance<TScriptObj>();
            
            CreateAssetDirectory(savePath[..Index.FromEnd(1)]);
            
            if (savePath[Index.Start] != "Assets")
                savePath[Index.Start] = "Assets/" + savePath[Index.Start];
            
            AssetDatabase.CreateAsset(objInstance, string.Join('/', savePath));
            
            return objInstance;
        }

        /// <summary>Создаёт директорию в папке Assets корня проекта.</summary>
        /// <param name="directoryFolders">Путь создаваемой директории в папке Assets. Если по заданному пути первой будет папка Assets, то она будет пропущена.</param>
        /// <returns>Возвращает true если директория создалась. Если путь уже существовал вернёт false. Если параметры пусты вернёт false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CreateAssetDirectory(string[] directoryFolders)
        {
            var currentPath = "Assets";

            if (directoryFolders == null || directoryFolders.Length == 0)
                return false;

            if (directoryFolders[Index.Start] == currentPath)
                directoryFolders[Index.Start] = "";
                
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFindAssetGuids(Type assetType, out string[] foundedGuids)
        {
            string preparedFilter = AssetDBFilters.TypeFilter(assetType);
            foundedGuids = AssetDatabase.FindAssets(preparedFilter);
            return foundedGuids.Length > 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ContainsExtension(string path, string extension) =>
            Path.GetExtension(path) == extension;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string AdjustFileExtension(string filePath, string fileExtension) =>
            filePath += Path.GetExtension(filePath) switch
            {
                var existExtension when existExtension == fileExtension => string.Empty,
                "" => fileExtension,
                var unknownExtension => throw new ArgumentException($"Переданный путь к файлу уде содержит в себе другое расширение: {unknownExtension}.")
            };
    }

    public struct AssetDBFilters
    {
        //вряд-ли юнитеки будут менять эти префиксы
        private const string TypePrefix = "t:";
        private const string AssetLabelPrefix = "l:";
        private const string RefPrefix = "ref:";
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RefIDFilter(int instanceID) =>
            RefPrefix + instanceID;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TypeFilter(Type type) =>
            TypePrefix + type.Name;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AssetLabelFilter(string labelName) =>
            AssetLabelPrefix + labelName;
    }
}