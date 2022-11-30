using System;
using System.Collections.Generic;
using FolderPolish.Editor.TreeViews;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FolderPolish.Editor.EditorWindows {
    
    /// <summary>
    /// Class <c>FolderPolishWindow</c> handles the frontend for the Cleaning Menu
    /// </summary>
    public class FolderPolishWindow : EditorWindow {

        #region Variables

        [SerializeField] private TreeViewState mTreeViewState;
        private static List<TreeViewItem> _treeViewItems;
        private PolishTreeView _cleaningTreeView;
        private static GUIStyle _startPolishButtonDesign; 
        private static GUIStyle _buttonDesign;
        private Rect TopButtonbarRect => new(10f, 10f, position.width - 20f, 50f);
        private Rect TreeViewRect => new(10f, 60f, position.width - 20f, position.height - 60f);

        #endregion

        #region MonoBehaviour Methods

        private void OnEnable() {
            _buttonDesign = new GUIStyle(EditorStyles.toolbarButton);
            _startPolishButtonDesign = new GUIStyle(EditorStyles.toolbarButton);
            _buttonDesign.hover.textColor = Color.white;
            _buttonDesign.normal.textColor = Color.cyan;
            _startPolishButtonDesign.normal.textColor = Color.red;
            _startPolishButtonDesign.hover.textColor = Color.white;
            if (mTreeViewState == null) mTreeViewState = new TreeViewState();
            _cleaningTreeView = new PolishTreeView(mTreeViewState, _treeViewItems);
            _cleaningTreeView.ExpandAll();
        }

        private void OnGUI() {
            TopBarCleaningMenu(TopButtonbarRect);
            DoTreeView(TreeViewRect);
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Creates a Window (TreeView) of unused Assets based on the selected Scenes  
        /// </summary>
        /// <param name="tree">List of TreeViewItems</param>
        public static void UnusedAssetsWindowShower(List<TreeViewItem> tree) {
            try {
                _treeViewItems = tree;
                var window = GetWindow<FolderPolishWindow>();
                window.titleContent = new GUIContent("Unused Assets");
                window.minSize = new Vector2(350, 500);
                window.Show();
            }
            catch (Exception e) {
                Console.WriteLine(e + "failed");
                throw;
            }
        }
        
        private void DoTreeView(Rect rect)
        {
            if (_treeViewItems.Count > 0) {
                _cleaningTreeView.OnGUI(rect);
            }
            else {
                var noAssets = new Rect(rect.x, rect.y, rect.width, 20);
                EditorGUI.LabelField(noAssets, "0 unused assets.");
            }
        }
        
        private void TopBarCleaningMenu(Rect rect)
        {
            GUILayout.BeginArea(rect);
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField("Cleaning Menu",style);
            EditorGUILayout.BeginHorizontal("box");
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Expand All", _buttonDesign)) {
                    _cleaningTreeView.ExpandAll();
                }
                
                if (GUILayout.Button("Start Polish", _startPolishButtonDesign)) {
                    
                    var confirmPosition = new Rect(position);
                    confirmPosition.y += confirmPosition.height / 2;
                    DeleteAssetsWindow.OpenConfirmationWindow(confirmPosition);
                    Close();
                }
                
                if (GUILayout.Button("Collapse All", _buttonDesign)) {
                    _cleaningTreeView.CollapseAll();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        #endregion
        
    }
}