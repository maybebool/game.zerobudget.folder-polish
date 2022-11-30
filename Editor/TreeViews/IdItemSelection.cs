
namespace FolderPolish.Editor.TreeViews {
    
    /// <summary>
    /// Class <c>IdItemSelection</c> handles ID and Selected Properties for the TreeViewItem  
    /// </summary>
    public struct IdItemSelection {
        /// <value>
        /// Property <c>ID</c> represents an int that sets a unique number for an Asset
        /// </value>
        public int ID { get; }
        
        /// <value>
        /// Property <c>Selected</c>  represents a bool that determines if a Asset is selected or not
        /// </value>
        public bool Selected { get; set; }

        /// <summary>
        /// returns id and state of an Item. Will be used in the TreeViews
        /// </summary>
        /// <param name="id">int that represents the unique id of an gameObject</param>
        /// <param name="selected">bool that said if a gameObject is selected or not</param>
        public IdItemSelection(int id, bool selected) {
            ID = id;
            Selected = selected;
        }
    }
}