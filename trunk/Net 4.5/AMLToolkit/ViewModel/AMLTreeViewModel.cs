// *********************************************************************** Assembly :
// AMLToolkit Author : Josef Prinz Created : 03-09-2015
// 
// Last Modified By : Josef Prinz Last Modified On : 03-10-2015 ***********************************************************************
// <copyright file="AMLTreeViewModel.cs" company="inpro">
//    Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Xml;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    ///    Class AMLTreeViewModel can build AMLNode-Trees for any CAEX-Element
    /// </summary>
    public class AMLTreeViewModel : AMLNodeViewModel
    {
        #region Private Fields

        /// <summary>
        ///    <see cref="CanDragDrop"/>
        /// </summary>
        private CanDragDropPredicate _CanDragDrop;

        /// <summary>
        ///    <see cref="DoDragDrop"/>
        /// </summary>
        private DoDragDropAction _DoDragDrop;

        /// <summary>
        ///    <see cref="Root"/>
        /// </summary>
        private AMLNodeViewModel root;

        /// <summary>
        ///    <see cref="TreeViewLayout"/>
        /// </summary>
        private AMLLayout treeViewLayout;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///    Initializes a new instance of the <see cref="AMLTreeViewModel"/> class.
        /// </summary>
        /// <param name="RootNode">    The root node for the TreeView.</param>
        /// <param name="CaexTagNames">
        ///    List of CAEX-Names, which define the visible Elements in the TreeView. A
        ///    Name in the List can be any CAEX-Element Name.
        /// </param>
        public AMLTreeViewModel(XmlElement RootNode, List<string> CaexTagNames)
            : base(null, RootNode, false)
        {
            this.CAEXTagNames = CaexTagNames;
            this.Root = new AMLNodeWithoutName(this, RootNode, false);
            this.Children.Add(this.Root);
            this.Root.LoadChildren();
            this.TreeViewLayout = AMLLayout.DefaultLayout;
        }

        #endregion Public Constructors

        #region Public Delegates

        /// <summary>
        ///    Delegate CanDragDropPredicate
        /// </summary>
        /// <param name="treeView">The TreeView where the target is located</param>
        /// <param name="source">The source which is dragged.</param>
        /// <param name="target">The target for the drop.</param>
        /// <returns>
        ///    <c>true</c> if source drop on target is allowed, <c>false</c> otherwise.
        /// </returns>
        public delegate bool CanDragDropPredicate(AMLTreeViewModel treeView, AMLNodeViewModel source, AMLNodeViewModel target);

        /// <summary>
        ///    Delegate DoDragDropAction
        /// </summary>
        /// <param name="treeView">The TreeView where the target is located</param>
        /// <param name="source">The source which is dragged.</param>
        /// <param name="target">The target for the drop.</param>
        public delegate void DoDragDropAction(AMLTreeViewModel treeView, AMLNodeViewModel source, AMLNodeViewModel target);

        #endregion Public Delegates

        #region Public Properties

        /// <summary>
        ///    Gets and sets the CanDragDropPredicate.
        /// </summary>
        public CanDragDropPredicate CanDragDrop
        {
            get
            {
                return _CanDragDrop;
            }
            set
            {
                if (_CanDragDrop != value)
                {
                    _CanDragDrop = value; base.RaisePropertyChanged(() => CanDragDrop);
                }
            }
        }

        /// <summary>
        ///    Gets and sets the TheDragDropAction
        /// </summary>
        public DoDragDropAction DoDragDrop
        {
            get
            {
                return _DoDragDrop;
            }
            set
            {
                if (_DoDragDrop != value)
                {
                    _DoDragDrop = value; base.RaisePropertyChanged(() => DoDragDrop);
                }
            }
        }

        /// <summary>
        ///    Gets and sets the Root
        /// </summary>
        public AMLNodeViewModel Root
        {
            get
            {
                return root;
            }
            set
            {
                if (root != value)
                {
                    root = value; base.RaisePropertyChanged(() => Root);
                }
            }
        }

        /// <summary>
        ///    Gets and sets the TreeViewLayout
        /// </summary>
        public AMLLayout TreeViewLayout
        {
            get
            {
                return treeViewLayout;
            }
            set
            {
                if (treeViewLayout != value)
                {
                    treeViewLayout = value;

                    if (treeViewLayout != null)
                    {
                        treeViewLayout.NamesOfVisibleElements.CollectionChanged += NamesOfVisibleElements_CollectionChanged;
                    }
                    base.RaisePropertyChanged(() => TreeViewLayout);
                }
            }
        }

        #endregion Public Properties

        #region Private Methods

        private void NamesOfVisibleElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Root.ApplyFilterWithName(TreeViewLayout.NamesOfVisibleElements);
        }

        #endregion Private Methods
    }
}