using System;
using System.Collections.Generic;
using FolderPolish.Editor.Mechanics;
using FolderPolish.Editor.TreeViews;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FolderPolish.Editor.EditorWindows {
    
    /// <summary>
    /// Class <c>MainEditorWindow</c> handles the frontend for the Main Menu 
    /// </summary>
    public class MainEditorWindow : EditorWindow {
        
        #region Variables
        // public
        /// <value>
        /// Property <c>TreeViewState</c> represents serializable states information for the TreeView
        /// </value>
        public TreeViewState TreeViewState { get; set; }
        
        /// <value>
        /// Property <c>NameFromScenes</c> represents a List of Scene names
        /// </value>
        public static List<string> NameFromScenes { get; private set; } = new();

        // private
        private static List<TreeViewItem> _treeViewItems;
        private static OverviewTreeView _mSceneTreeView;
        private static GUIStyle _buttonStyle;
        private static GUIStyle _polishButtonStyle;

        // GUI
        private Rect TopBarRect => new(10f, 10f, position.width - 20f, 50f);
        private Rect SceneSelectRect => new(10f, 40f, position.width - 20f, position.height -60f);

        #endregion

        #region MonoBehaviour Methods

        private void OnEnable() {
            try {
                _buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
                _polishButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
                _buttonStyle.normal.textColor = Color.cyan;
                _polishButtonStyle.normal.textColor = Color.green;
                _buttonStyle.hover.textColor = Color.white;
                _polishButtonStyle.hover.textColor = Color.white;
                if (TreeViewState == null) TreeViewState = new TreeViewState();
                _mSceneTreeView = new OverviewTreeView(TreeViewState, _treeViewItems);
                _mSceneTreeView.ExpandAll();

            }
            catch (Exception) {
#if UNITY_EDITOR
                Debug.Log("MainMenu.OnEnable error");
#endif
                throw;
            }
        }

        private void OnGUI() {
            TopBar(TopBarRect);
            DoTreeView(SceneSelectRect);
#if UNITY_EDITOR
            Debug.Log("Main Menu OnGUI works");
#endif
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the TreeView of the detected Scenes in Project
        /// </summary>
        /// <param name="sceneNames"> name of scanned scenes</param>
        public static void SceneSelectorWindowShower(List<string> sceneNames) {
            try {
                _treeViewItems = ProjectPolish.BuildTreeViewItemsList(sceneNames);
                var window = GetWindow<MainEditorWindow>();
                window.titleContent = new GUIContent("Select Scenes In Project");
                window.minSize = new Vector2(350, 250);
                window.Show();
                NameFromScenes = sceneNames;

                for (int i = 0; i < PolishTreeView.PolishItemSelections.Length; i++) {
                    PolishTreeView.PolishItemSelections[i].Selected = true;
                }

            }
            catch (Exception e) {
                Console.WriteLine(e + "failed");
                throw;
            }
        }

        private static void DoTreeView(Rect rect) {
            try {
                GUILayout.BeginArea(rect);
                if (_treeViewItems.Count > 0) {
                    _mSceneTreeView.OnGUI(rect);
                }
                else {
                    var noAssets = new Rect(rect.x, rect.y, rect.width, 20);
                    EditorGUI.LabelField(noAssets, "0 unused assets.");
                }
                GUILayout.EndArea();

            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }


        private void TopBar(Rect rect) {
            GUILayout.BeginArea(rect);
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField("Main Menu",style);
            EditorGUILayout.BeginHorizontal("box");
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Expand all", _buttonStyle)) {
                    _mSceneTreeView.ExpandAll();
                }

                if (GUILayout.Button("Check for unused Assets", _polishButtonStyle)) {
                    ProjectPolish.ScanSelectedScenes();
                    Close();
                }

                if (GUILayout.Button("Collapse all", _buttonStyle)) {
                    _mSceneTreeView.CollapseAll();
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        #endregion
    }
}