using Aml.Editor.MVVMBase;
using System;
using System.Windows.Data;

namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    /// the abstract base class for all aml tree nodes.
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public abstract class AMLNodeBaseViewModel : ViewModelBase
    {
        /// <summary>
        ///     The _is selected
        /// </summary>
        private bool _isSelected;

        /// <summary>
        ///     <see cref="IsVisible" />
        /// </summary>
        protected bool _isVisible = true;


        /// <summary>
        ///     The _is expanded
        /// </summary>
        protected bool _isExpanded;


        /// <summary>
        ///     Gets/sets whether the TreeViewItem associated with this object is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        /// <summary>
        ///     Gets and sets the IsVisible
        /// </summary>
        public virtual bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        /// <summary>
        ///     Gets/sets whether the TreeViewItem associated with this object is expanded.
        /// </summary>
        /// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
        public virtual bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance should always expand all children.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is expanding all; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpandingAll { get; set; }

        /// <summary>
        /// The children collection
        /// </summary>
        protected readonly CollectionViewSource _childrenCollection = new();

        /// <summary>
        ///     Gets the children view. Binding to this Property enables filtering and
        ///     ordering of the Children Collection
        /// </summary>
        /// <value>The children view.</value>
        public ListCollectionView ChildrenView => _childrenCollection?.View as ListCollectionView;




        /// <summary>
        /// Sets the tree view item to the corresponding expanded state
        /// as indicated in the <seealso cref="IsExpanded"/> property.
        /// </summary>
        /// <param name="isExpanded"></param>
        public void SetExpand(bool isExpanded)
        {
            IsExpanded = isExpanded;
        }

        /// <summary>
        /// Sets the tree view item to the corresponding selected state
        /// as indicated in the <seealso cref="IsSelected"/> property.
        /// </summary>
        /// <param name="isSelected"></param>
        public void SetSelect(bool isSelected)
        {
            IsSelected = isSelected;
        }


        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        public virtual void LoadChildren(bool raise = true)
        {
            throw new NotImplementedException();
        }



    }
}
