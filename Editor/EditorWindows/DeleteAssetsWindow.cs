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
        private static GUIStyle _buttonDesign;

        #endregion

        #region MonoBehaviour Methods
        private void OnGUI() {
            ShowNotification(new GUIContent("Are you sure you want to delete the assets ?"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("CANCEL", _buttonDesign)) {
                    Close();
                } 
                if (GUILayout.Button("DELETE ASSETS", _deleteDesign)) {
                    UnusedAssetsAnnihilation();
                    Close();
                }
            }
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
            _buttonDesign = new GUIStyle(EditorStyles.miniButton);
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