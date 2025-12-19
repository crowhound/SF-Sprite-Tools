using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

namespace SFEditor.Utilities
{
    public static class AssetDatabaseEditorUtilities
    {
        private static string _cachedPath;

        public static List<T> FindAssetsOfType<T>(string searchGlobFilter = "") where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"{searchGlobFilter} t:{typeof(T).Name}");

            foreach(string guid in guids)
            {
                _cachedPath = AssetDatabase.GUIDToAssetPath(guid);

                assets.Add(AssetDatabase.LoadAssetAtPath<T>(_cachedPath));
            }

            return assets;
        }
        /// <summary>
        /// Returns the first asset found using the search glob filter if any is passed in. If no filter string is passed in it will just find the first type without worrying about any filters being applied.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchGlobFilter"></param>
        /// <returns></returns>
        public static T FindFirstAssetOfType<T>(string searchGlobFilter = "") where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"{searchGlobFilter} t:{typeof(T).Name}");
            _cachedPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(_cachedPath);
        }

        public static T CreateScriptableObjectSafely<T>(this string defaultPath) where T : ScriptableObject
        {
            if(!FileUtility.IsFolderPathValid(defaultPath))
                FileUtility.CreateFolderPath(defaultPath);

            T asset = ScriptableObject.CreateInstance<T>();
            Debug.Log(asset.GetType());
            AssetDatabase.CreateAsset(asset, defaultPath);
            AssetDatabase.SaveAssets();
            return asset;
        }

        #region Directory Utilities
        /// <summary>
        /// Gets the logical folder that an asset is in. The logical folder is the folder relative to the Unity Project's root not the folder path from the drive or storage device.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetLogicalFolderFromAsset(this ScriptableObject script)
        {
            // TODO: Replace the return Split with Unity's Path.Extensions GetDirectoryName

            // Gets the asset in the asset database.
            var asset = MonoScript.FromScriptableObject(script);

            _cachedPath = AssetDatabase.GetAssetPath(asset);
            Debug.Log(Path.GetDirectoryName(_cachedPath));

            return _cachedPath.Split(asset.name)[0];
        }
        /// <summary>
        /// Gets the logical folder that an asset is in. The logical folder is the folder relative to the Unity Project's root not the folder path from the drive or storage device.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetLogicalFolderFromAsset(this MonoScript asset)
        {
            // TODO: Replace the return Split with Unity's Path.Extensions GetDirectoryName
            _cachedPath = AssetDatabase.GetAssetPath(asset);
            return _cachedPath.Split(asset.name)[0];
        }
        #endregion


        /// <summary>
        /// Gets the currently selected Unity Object asset in the editor and returns true if the asset matches the type passed in. If it is of the same type and returns true it will also out the object converted over to the defined type from the base UnityEngine.Object type. Retruns false if the asset is not of the type or is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityObject"></param>
        /// <returns></returns>
        public static bool TryGetSelectedObjectOfType<T>(out T unityObject) where T : Object
        {
            // If there is no Unity Object selected return early.
            if(Selection.activeObject == null)
            {
                unityObject = null;
                return false;
            }

            // If there is a Unity Object selected make sure it is the correct type.
            if(Selection.activeObject?.GetType() == typeof(T))
            {
                // Convert from UnityEngine.Object to the wanted type and return true while outing the selected object.
                unityObject = (T)Selection.activeObject;
                return true;

            }

            unityObject = null;
            return false;
        }

    }
}
