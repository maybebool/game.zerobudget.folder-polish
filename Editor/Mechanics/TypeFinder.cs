using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolderPolish.Editor.Mechanics {
    
    /// <summary>
    /// Class <c>TypeFinder</c> handles search methods for scenes and SO
    /// </summary>
    public class TypeFinder : MonoBehaviour {
        /// <summary>
        /// Searches for scenes in Assets path and returns a list
        /// </summary>
        /// <typeparam name="T">SceneObject with references in Editor </typeparam>
        /// <returns>List of SceneAssets</returns>
        public static List<string> AcquireScenes<T>() where T : SceneAsset {
            var searchFolders = new string[1];
            searchFolders[0] = "Assets";
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, searchFolders);
            var returnList = new List<string>();
            for (int i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                returnList.Add(path);
            }
            return returnList;
        }
        /// <summary>
        /// Searches all paths from Assets Folder for SO and returns a list if SO are found
        /// </summary>
        /// <typeparam name="T"> Type of Scriptable Objects</typeparam>
        /// <returns></returns>
        public static List<string> DetectScriptableObjects<T>() where T : ScriptableObject {
            var searchFolders = new string[1];
            searchFolders[0] = "Assets";
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, searchFolders);
            var returnList = new List<string>();
            for (int i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                returnList.Add(path);
            }
            return returnList;
        }
    }
}