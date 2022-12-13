using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FolderPolish.Editor.TreeViews {
    
    /// <summary>
    /// Class <c>OverviewTreeView</c> handles the TreeView definition for the Main Menu Window  
    /// </summary>
    public class OverviewTreeView : TreeView {
        
        #region Properties

        /// <value>
        /// Property <c>SceneTreeViewItems</c> represents a Scene as TreeViewItem in the MainMenu
        /// </value>
        public static List<TreeViewItem> SceneTreeViewItems { get; set; }

        /// <value>
        /// Property <c>SceneItemSelections</c> represents ID and state of a Scene in the Main Menu 
        /// </value>
        public static IdItemSelection[] SceneItemSelections { get; private set; }

        #endregion

        #region Unity Methods

        protected override void RowGUI(RowGUIArgs args) {
            extraSpaceBeforeIconAndLabel = 20f;
            var toggleRect = args.rowRect;
            toggleRect.x += GetContentIndent(args.item);
            toggleRect.width = 16f;
            var evt = Event.current;

            if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition)) {
                ToggleSelection(args.item.id);
            }

            var item = GetItem(args.item.id);

            if (item.ID == -1) {
                return;
            }
            EditorGUI.Toggle(toggleRect, item.Selected);
            base.RowGUI(args);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepares a List of TreeViewItems for the TreeView based on the selected Assets.
        /// </summary>
        /// <param name="state">The TreeViewState contains serializable state information for the TreeView</param>
        /// <param name="tree">The TreeViewItem is used to build the tree representation of a tree data structure.</param>
        public OverviewTreeView(TreeViewState state, List<TreeViewItem> tree) : base(state) {
            SceneTreeViewItems = tree;
            SceneItemSelections = new IdItemSelection[tree.Count];

            for (int i = 0; i < tree.Count; i++) {
                SceneItemSelections[i] = new IdItemSelection(SceneTreeViewItems[i].id, true);
            }

            Reload();
        }

        private IdItemSelection GetItem(int id) {
            for (int i = 0; i < SceneTreeViewItems.Count; i++) {
                if (SceneItemSelections[i].ID == id) {
                    return SceneItemSelections[i];
                }
            }

            return new IdItemSelection(-1, true);
        }

        private void ToggleSelection(int id) {
            for (int i = 0; i < SceneItemSelections.Length; i++) {
                if (SceneItemSelections[i].ID == id) {
                    bool newValue = !SceneItemSelections[i].Selected;
                    SceneItemSelections[i].Selected = newValue;
                    if (!newValue) ToggleParent(id, newValue);
                    ToggleChildren(id, newValue);
                    return;
                }
            }
        }


        private void ToggleChildren(int id, bool selection) {
            TreeViewItem item = null;

            for (int i = 0; i < SceneTreeViewItems.Count; i++) {
                if (SceneTreeViewItems[i].id == id) {
                    item = SceneTreeViewItems[i];
                    break;
                }
            }

            if (item == null || !item.hasChildren)
                return;

            var children = item.children;

            if (children.Count > 0 && children != null) {
                foreach (TreeViewItem t in children) {
                    for (int i = 0; i < SceneItemSelections.Length; i++) {
                        if (SceneItemSelections[i].ID == t.id) {
                            SceneItemSelections[i].Selected = selection;
                            break;
                        }
                    }

                    if (t.hasChildren) ToggleChildren(t.id, selection);
                }
            }
        }

        private void ToggleParent(int id, bool selection) {
            for (int i = 0; i < SceneTreeViewItems.Count; i++) {
                if (SceneTreeViewItems[i].id == id) {
                    var item = SceneTreeViewItems[i];

                    if (item.parent != null && item.parent.depth >= 0) {
                        for (int j = 0; j < SceneItemSelections.Length; j++) {
                            if (SceneItemSelections[j].ID == item.parent.id) {
                                SceneItemSelections[j].Selected = selection;
                                if (item.parent.parent != null) ToggleParent(item.parent.id, selection);
                            }
                        }
                    }
                }
            }
        }


        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
            SetupParentsAndChildrenFromDepths(root, SceneTreeViewItems);
            return root;
        }

        #endregion
    }
}