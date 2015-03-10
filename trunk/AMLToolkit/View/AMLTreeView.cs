// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLTreeView.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using System.Windows.Controls;
using AMLToolkit.ViewModel;

/// <summary>
/// The AMLToolkit namespace.
/// </summary>
namespace AMLToolkit.View
{
    /// <summary>
    /// The AMLTreeView is a Control, which arranges CAEX-ElementTrees in a TreeView and assigns a default icon for each distinctive CAEX-Element
    /// <example>This is a code example which shows, how to create a ViewModel with AMLDocument Data, which can be bound to a TreeView. The
    /// example uses the AMLEngine.
    /// <code keepSeeTags="true"> 
    /// {
    ///    // read the AMLDocument
    ///    var doc = CAEXDocument.LoadFromFile("myFile.aml");
    ///    
    ///    // create a viewModel, using the TreeView Template <see cref="AMLToolkit.ViewModel.AMLTreeViewTemplate.CompleteInstanceHierarchyTree"/>
    ///    var viewModel = new AMLToolkit.ViewModel.AMLTreeViewModel(
    ///             (XmlElement)doc.CAEXFile.Node,
    ///             AMLToolkit.ViewModel.AMLTreeViewTemplate.CompleteInstanceHierarchyTree);
    ///             
    ///    var treeView = new AMLTreeView ();
    ///    treeView.TreeViewModel = viewModel;
    /// }
    /// </code>
    /// </example>  
    /// </summary>
    public class AMLTreeView : Control
    {
        #region Public Fields

        /// <summary>
        /// The TreeView property
        /// </summary>
        public static readonly DependencyProperty TheTreeViewProperty =
            DependencyProperty.Register("TheTreeView", typeof(TreeView), typeof(AMLTreeView), new PropertyMetadata(null));

        /// <summary>
        /// The TreeView model property, used to populate the TreeView
        /// </summary>
        public static readonly DependencyProperty TreeViewModelProperty =
            DependencyProperty.Register("TreeViewModel", typeof(AMLTreeViewModel), typeof(AMLTreeView), new PropertyMetadata(null, treeViewModelChanged));

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Initializes static members of the <see cref="AMLTreeView"/> class.
        /// </summary>
        static AMLTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AMLTreeView), new FrameworkPropertyMetadata(typeof(AMLTreeView)));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the internal TreeView Control Object
        /// </summary>
        /// <value>The TreeView.</value>
        public TreeView TheTreeView
        {
            get { return (TreeView)GetValue(TheTreeViewProperty); }
            private set { SetValue(TheTreeViewProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TreeView model. The TreeView's ItemsSource Property is bound to the Root's Children Collection of the TreeViewModel
        /// </summary>
        /// <value>The TreeView model.</value>
        public AMLTreeViewModel TreeViewModel
        {
            get { return (AMLTreeViewModel)GetValue(TreeViewModelProperty); }
            set { SetValue(TreeViewModelProperty, value); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Wird bei einer Überschreibung in einer abgeleiteten Klasse stets aufgerufen, wenn <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" /> 
        /// von Anwendungscode oder internem Prozesscode aufgerufen wird.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetValue(TheTreeViewProperty, GetTemplateChild("TheTreeView") as TreeView);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Sets the TreeViewModel as the Control's DataContext.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void treeViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AMLTreeView)
            {
                (d as AMLTreeView).DataContext = e.NewValue;
            }
        }

        #endregion Private Methods
    }
}