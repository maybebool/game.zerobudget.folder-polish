using System.Collections.Generic;
using FolderPolish.Editor.EditorWindows;
using FolderPolish.Editor.TreeViews;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FolderPolish.Editor.Mechanics {
    
    /// <summary>
    /// Class <c>ProjectPolish</c> handles the Asset selection methods for the EditorWindows
    /// </summary>
    public class ProjectPolish {
        
        #region Variables
        
        
        public static List<string> UnusedAssetPaths;
        private static List<string> _assetPaths;
        private static List<string> _usedAssetPaths;
        private static List<string> _scenesInProject;
        private static List<string> _protectedAssets;

        #endregion

        #region Methods
        
        
        /// <summary>
        /// Checks the selected Scenes for Asset-Dependencies via AssetDatabase ad adds them to a list.
        /// Builds a list of unused Assets
        /// </summary>
        
        //https://github.com/AlexeyPerov/Unity-Dependencies-Hunter/blob/master/Packages/DependenciesHunter/Editor/DependenciesHunter.cs
        public static void ScanSelectedScenes() {
            _usedAssetPaths = new List<string>();

            for (int i = 0; i < OverviewTreeView.SceneItemSelections.Length; i++) {
                if (OverviewTreeView.SceneItemSelections[i].Selected) {
                    var scenePath = MainEditorWindow.NameFromScenes[i];
                    if (!AssetDatabase.IsValidFolder(scenePath)) {
                        var usedAssets = AssetDatabase.GetDependencies(scenePath, true);
                        for (int j = 0; j < usedAssets.Length; j++) {
                            EditorUtility.DisplayProgressBar("Loading Scene Dependencies...", "", 0f);
                            var assetPath = usedAssets[j];
                            if (!_usedAssetPaths.Contains(assetPath) && !AssetDatabase.IsValidFolder(assetPath))
                                _usedAssetPaths.Add(assetPath);
                        }
                    }
                }
            }
            BuildListOfUnusedAssets();
            LaunchCleaningWindow();
        }
        
        /// <summary>
        /// Creates a TreeView of Scenes found in Project. The found scenes are stored in a list
        /// </summary>
        /// <param name="scenesFilter">separates the scenes</param>
        /// <returns></returns>
        // https://rteditor.battlehub.net/manual/vtv.html
        //https://git.gvk.idi.ntnu.no/justworks/playground/-/blob/1e1a6bb7b7124274cbaeb90cc40f3e15706983fa/Assets/_Assets/Editor/TreeView/UnityTreeView.cs
        public static List<TreeViewItem> BuildTreeViewItemsList(List<string> scenesFilter) {
            var allItems = new List<TreeViewItem>();
            var treeViewItems = scenesFilter;
            const string assets = "Assets/";
            var assetsPathArray = assets.ToCharArray();
            treeViewItems = DetectUnusedFolders(treeViewItems);
            var lastPath = new string[0];
            var lastPathName = "";

            for (int i = 0; i < treeViewItems.Count; i++) {
                EditorUtility.DisplayProgressBar("Building TreeView...", "", 0.66f);
                var itemPath = scenesFilter[i];
                itemPath = itemPath.TrimStart(assetsPathArray);
                var splitString = itemPath.Split('/', '/');
                var indexDepth = splitString.Length - 1;
                var displayName = splitString[indexDepth];
                var pathName = "";

                for (int j = 0; j < splitString.Length; j++) {
                    pathName += splitString[j];
                }

                if (lastPathName == "") {
                    lastPathName = pathName;
                }

                if (splitString.Length != lastPath.Length && pathName.Contains(lastPathName)) {
                }

                var newItem = new TreeViewItem(i, indexDepth, displayName);
                allItems.Add(newItem);
            }

            EditorUtility.ClearProgressBar();
            return allItems;
        }
        
        /// <summary>
        /// Return a List (TreeViewItems) of Assets of the parent Assets/
        /// Can be used to store Assets from scanned Scenes
        /// </summary>
        /// <returns></returns>
        public static List<TreeViewItem> BuildTreeViewItemsList() {
            var allItems = new List<TreeViewItem>();
            const string assets = "Assets/";
            var assetsPathArray = assets.ToCharArray();
            UnusedAssetPaths = DetectUnusedFolders(UnusedAssetPaths);
            for (int i = 0; i < UnusedAssetPaths.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Building TreeView...", "", 0.66f);
                var itemPath = UnusedAssetPaths[i];
                itemPath = itemPath.TrimStart(assetsPathArray);
                var splitString = itemPath.Split('/', '/');
                var indexDepth = splitString.Length - 1;
                var displayName = splitString[indexDepth];
                var newItem = new TreeViewItem(i, indexDepth, displayName);
                allItems.Add(newItem);
            }
            EditorUtility.ClearProgressBar();
            return allItems;
        }

        [MenuItem("Folder Polish/Scan all Scenes in Project", false, 1)]
        private static void ScanScenesInProject() {
            EditorUtility.DisplayDialog("Asset Cleaner", "Make sure you saved all your files.", "OK");
            _scenesInProject = Indexing();

            if (_scenesInProject != null) {
                MainEditorWindow.SceneSelectorWindowShower(_scenesInProject);
            }
        }

        private static bool IsSceneAsset(string path, List<string> scenePaths) {
            if (scenePaths.Contains(path)) return true;
            else return false;
        }
        
        private static bool CheckForUnselectedPath(string path) {
            for (int i = 0; i < OptionSetter.ExcludedFolders.Count; i++) {
                var tmp = OptionSetter.ExcludedFolders[i];
                tmp = tmp.Substring(7, tmp.Length - 7);
                if (path.Contains(tmp)) return true;
            }
            return false;
        }
        
        //https://github.com/AlexeyPerov/Unity-Dependencies-Hunter/blob/master/Packages/DependenciesHunter/Editor/DependenciesHunter.cs
        private static List<string> Indexing() {
            _assetPaths = new List<string>();
            var scenes = new List<string>();
            var scenePaths = TypeFinder.AcquireScenes<SceneAsset>();
            var searchFolders = new string[1];
            searchFolders[0] = "Assets";
            var assetGUIDs = AssetDatabase.FindAssets("", searchFolders);

            for (int i = 0; i < assetGUIDs.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);

                if (!CheckForUnselectedPath(path)) {
                    const bool processAsset = true;

                    if (processAsset) {
                        try {
                            var typeString = AssetDatabase.GetMainAssetTypeAtPath(path).ToString();

                            switch (typeString) {
                                case "UnityEditor.DefaultAsset" when !AssetDatabase.IsValidFolder(path): {
                                    if (path.EndsWith(".cs", true, null)) {
                                        if (!OptionSetter.ExcludeScripts) {
                                            _assetPaths.Add(path);
                                        }
                                    }else {
                                        _assetPaths.Add(path);
                                    }
                                    break;
                                }
                                case "UnityEditor.DefaultAsset":
                                    _assetPaths.Add(path);
                                    scenes.Add(path);
                                    break;
                                case "UnityEditor.SceneAsset":
                                    scenes.Add(path);
                                    _assetPaths.Add(path);
                                    break;
                                case "UnityEditor.MonoScript": {
                                    if (!OptionSetter.ExcludeScripts) {
                                        _assetPaths.Add(path);
                                    }
                                    break;
                                }
                                case "UnityEngine.TextAsset":
                                    _assetPaths.Add(path);
                                    break;
                                default:
                                    _assetPaths.Add(path);
                                    break;
                            }
                        }
                        catch {
                            _assetPaths.Add(path);
                            Debug.Log("Shit doesnt work like I want it");
                        }
                    }
                }
                else {
                    if (IsSceneAsset(path, scenePaths)) {
                        scenes.Add(path);
                    }
                    else if (AssetDatabase.IsValidFolder(path)) {
                        _assetPaths.Add(path);
                        scenes.Add(path);
                    }
                }
            }

            if (OptionSetter.ExcludeSO) {
                RemoveScriptableObjects();
            }
            
            EditorUtility.ClearProgressBar();
            return scenes.Count > 0 ? scenes : null;
        }
        
        
        private static void RemoveScriptableObjects() {
            var so = TypeFinder.DetectScriptableObjects<ScriptableObject>();

            foreach (string s in so) {
                var removing = true;
                while (removing) {
                    removing = _assetPaths.Remove(s);
                }
            }
        }

        

        private static List<string> DetectUnusedFolders(List<string> tree) {
            var foldersToCheck = new List<string>();

            foreach (string s in tree) {
                if (AssetDatabase.IsValidFolder(s)) foldersToCheck.Add(s);
            }

            for (int i = foldersToCheck.Count - 1; i >= 0; i--) {
                var foundItem = false;

                for (int j = 0; j < tree.Count; j++) {
                    var s = tree[j];

                    if (s.StartsWith(foldersToCheck[i]) && s != foldersToCheck[i]) {
                        foundItem = true;
                        break;
                    }
                }

                if (!foundItem) {
                    tree.Remove(foldersToCheck[i]);
                }
            }

            return tree;
        }


        private static void BuildListOfUnusedAssets() {
            UnusedAssetPaths = new List<string>();

            for (int i = 0; i < _assetPaths.Count; i++) {
                var s = _assetPaths[i];
                EditorUtility.DisplayProgressBar("Building List Of Unused Assets...", "", 0.33f);

                if (!_usedAssetPaths.Contains(s)) {
                    UnusedAssetPaths.Add(s);
                }
            }
        }

        private static void LaunchCleaningWindow() {
            if (_assetPaths.Count > _usedAssetPaths.Count) {
                var items = BuildTreeViewItemsList();
                FolderPolishWindow.UnusedAssetsWindowShower(items);
            }
            else EditorUtility.DisplayDialog("Folder Polish", "Failed: 0 unused assets in this Project", "OK");
        }
        
        #endregion


    }
}