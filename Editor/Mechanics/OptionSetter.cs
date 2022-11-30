using System.Collections.Generic;

namespace FolderPolish.Editor.Mechanics {
    
    /// <summary>
    /// Class <c>OptionSetter</c> handles exclusion Properties for the Asset Indexing 
    /// </summary>
    public class OptionSetter {
        
        #region Variables
        
        private static bool _deleteEmptyFolders;

        #endregion

        #region Getters
        
        /// <value>
        /// Property <c>ExcludeSO</c> represents a bool for excluding SO
        /// </value>
        public static bool ExcludeSO { get; }
        
        /// <value>
        /// Property <c>ExcludedFolders</c> represents a List of excluded Folder Assets
        /// </value>
        public static List<string> ExcludedFolders { get; } = new();
        
        /// <value>
        /// Property <c>ExcludeScripts</c> represents a bool for excluded Scripts
        /// </value>
        public static bool ExcludeScripts { get; }
        
        #endregion
    }
}