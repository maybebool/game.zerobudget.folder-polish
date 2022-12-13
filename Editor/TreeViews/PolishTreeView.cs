using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FolderPolish.Editor.TreeViews {
    /// <summary>
    /// Class <c>PolishTreeView</c> handles the TreeView definition for the Cleaning Window  
    /// </summary>
    public class PolishTreeView : TreeView {

        #region Properties

        /// <value>
        /// Property <c>PolishTreeViewItems</c> represents a Asset as TreeViewItem in the Cleaning Menu
        /// </value>
        public static List<TreeViewItem> PolishTreeViewItems { get; set; }

        /// <value>
        /// Property <c>PolishItemSelections</c> represents a ID and state of an Asset in the Cleaning Menu
        /// </value>
        public static IdItemSelection[] PolishItemSelections { get; private set; }
        
        #endregion

        #region Methods
        
        /// <summary>
        /// Prepares a List of TreeViewItems for the TreeView based on the selected Assets.
        /// </summary>
        /// <param name="state">The TreeViewState contains serializable state information for the TreeView</param>
        /// <param name="tree">The TreeViewItem is used to build the tree representation of a tree data structure.</param>
        public PolishTreeView(TreeViewState state, List<TreeViewItem> tree) : base(state) {
            PolishTreeViewItems = tree;
            PolishItemSelections = new IdItemSelection[tree.Count];

            for (int i = 0; i < tree.Count; i++) {
                PolishItemSelections[i] = new IdItemSelection(PolishTreeViewItems[i].id, true);
            }

            Reload();
        }

        private IdItemSelection GetItem(int id) {
            for (int i = 0; i < PolishTreeViewItems.Count; i++) {
                if (PolishItemSelections[i].ID == id) {
                    return PolishItemSelections[i];
                }
            }

            return new IdItemSelection(-1, true);
        }


        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
            SetupParentsAndChildrenFromDepths(root, PolishTreeViewItems);
            return root;
        }


        private void ToggleSelection(int id) {
            for (int i = 0; i < PolishItemSelections.Length; i++) {
                if (PolishItemSelections[i].ID == id) {
                    bool newValue = !PolishItemSelections[i].Selected;
                    PolishItemSelections[i].Selected = newValue;
                    if (!newValue) ToggleParent(id, newValue);
                    ToggleChildren(id, newValue);
                    return;
                }
            }
        }

        private void ToggleChildren(int id, bool selection) {
            TreeViewItem item = null;

            for (int i = 0; i < PolishTreeViewItems.Count; i++) {
                if (PolishTreeViewItems[i].id == id) {
                    item = PolishTreeViewItems[i];
                    break;
                }
            }

            if (item == null || !item.hasChildren)
                return;

            var children = item.children;

            if (children.Count > 0 && children != null) {
                foreach (TreeViewItem t in children) {
                    for (int i = 0; i < PolishItemSelections.Length; i++) {
                        if (PolishItemSelections[i].ID == t.id) {
                            PolishItemSelections[i].Selected = selection;
                            break;
                        }
                    }

                    if (t.hasChildren) ToggleChildren(t.id, selection);
                }

                return;
            }
        }

        private void ToggleParent(int id, bool selection) {
            for (int i = 0; i < PolishTreeViewItems.Count; i++) {
                if (PolishTreeViewItems[i].id == id) {
                    TreeViewItem item = PolishTreeViewItems[i];

                    if (item.parent != null && item.parent.depth >= 0) {
                        for (int j = 0; j < PolishItemSelections.Length; j++) {
                            if (PolishItemSelections[j].ID == item.parent.id) {
                                PolishItemSelections[j].Selected = selection;
                                if (item.parent.parent != null) ToggleParent(item.parent.id, selection);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Unity Method

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
    }
}