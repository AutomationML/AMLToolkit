// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-10-2015
// ***********************************************************************
// <copyright file="AMLTreeViewModel.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ///    <see cref="Root"/>
        /// </summary>
        private AMLNodeViewModel root;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///    Initializes a new instance of the <see cref="AMLTreeViewModel"/> class.
        /// </summary>
        /// <param name="RootNode">   The root node for the TreeView.</param>
        /// <param name="CaexTagNames">
        ///    List of CAEX-Names, which define the visible Elements in the TreeView. A Name in the List can be any CAEX-Element Name.
        /// </param>
        public AMLTreeViewModel(XmlElement RootNode, List<string> CaexTagNames)
            : base(null, RootNode, false)
        {
            this.CAEXTagNames = CaexTagNames;
            this.Root = new AMLNodeWithoutName (this, RootNode, false);
            this.Children.Add(this.Root);
            this.Root.LoadChildren();
            this.TreeViewLayout = AMLLayout.DefaultLayout;
        }

        #endregion Public Constructors

        #region Public Properties
               

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
        ///  <see cref="TreeViewLayout"/>
        /// </summary>    
        private AMLLayout treeViewLayout;

        /// <summary>
        ///  Gets and sets the TreeViewLayout
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

                    if (treeViewLayout!=null)
                    {
                        treeViewLayout.NamesOfVisibleElements.CollectionChanged += NamesOfVisibleElements_CollectionChanged;
                    }
                    base.RaisePropertyChanged(() => TreeViewLayout);
                }
            }
        }

        void NamesOfVisibleElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Root.ApplyFilterWithName(TreeViewLayout.NamesOfVisibleElements);
        }
        

        #endregion Public Properties
    }
}