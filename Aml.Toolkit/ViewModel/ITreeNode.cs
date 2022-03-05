using System.Collections.Generic;

namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    /// Interface class, defining the basic properties of a tree node.
    /// </summary>
    public interface ITreeNode
    {
        #region Public Properties

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        IList<ITreeNode> Children { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the node's parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        ITreeNode Parent { get; }

        #endregion Public Properties
    }
}