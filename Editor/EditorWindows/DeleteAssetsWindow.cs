using FolderPolish.Editor.Mechanics;
using FolderPolish.Editor.TreeViews;
using UnityEditor;
using UnityEngine;

namespace FolderPolish.Editor.EditorWindows {
    
    /// <summary>
    /// Class <c>DeleteAssetsWindow</c> handles the frontend for the Delete Window
    /// </summary>
    public class DeleteAssetsWindow : EditorWindow {
        
        #region Variables
        
        private string _path = "";
        private static GUIStyle _deleteDesign;

        #endregion

        #region Unity Methods
        private void OnGUI() {
            GUI.contentColor = Color.red;
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Delete", GUILayout.Height(40))) {
                    UnusedAssetsAnnihilation();
                    Close();
                }
            }
            ShowNotification(new GUIContent("Are you sure you want to delete your selection?"));
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Window for the deletion process
        /// </summary>
        /// <param name="rect">rectangle X,Y that defines width and height</param>
        public static void OpenConfirmationWindow(Rect rect) {
            _deleteDesign = new GUIStyle(EditorStyles.miniButton);
            _deleteDesign.hover.textColor = Color.cyan;
            var window = GetWindow<DeleteAssetsWindow>();
            window.titleContent = new GUIContent("Confirm Polish?");
            var size = new Vector2(500, 200);
            window.minSize = size;
            window.maxSize = size;
            window.position = rect;
            window.Show();
        }

        private void UnusedAssetsAnnihilation() {
            foreach (var item in PolishTreeView.PolishItemSelections) {
                if (!item.Selected) continue;
                _path = ProjectPolish.UnusedAssetPaths[item.ID];
                if (!AssetDatabase.IsValidFolder(_path)) {
                    AssetDatabase.DeleteAsset(_path);
                }
            }
        }

        #endregion
    }
}