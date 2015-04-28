// *********************************************************************** Assembly :
// AMLToolkit Author : Josef Prinz Created : 03-10-2015
// 
// Last Modified By : Josef Prinz Last Modified On : 04-23-2015 ***********************************************************************
// <copyright file="AMLTreeView.cs" company="AutomationML e.V.">
//    Copyright © AutomationML e.V. 2015
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AMLToolkit.ViewModel;
using AMLToolkit.XamlClasses;

/// <summary>
///    The AMLToolkit namespace.
/// </summary>
namespace AMLToolkit.View
{
    /// <summary>
    ///    The AMLTreeView is a Control, which arranges CAEX-ElementTrees in a TreeView
    ///    and assigns a default icon for each distinctive CAEX-Element <example>This is a
    ///    code example which shows, how to create a ViewModel with AMLDocument Data,
    ///    which can be bound to a TreeView. The example uses the AMLEngine.
    ///    <code keepSeeTags="true" title="c#">
    ///             {
    ///             // read the AMLDocument
    ///             var doc = CAEXDocument.LoadFromFile("myFile.aml");
    ///             // create a viewModel, using the TreeView Template <see cref="AMLToolkit.ViewModel.AMLTreeViewTemplate.CompleteInstanceHierarchyTree" />
    ///             var viewModel = new AMLToolkit.ViewModel.AMLTreeViewModel(
    ///             (XmlElement)doc.CAEXFile.Node,
    ///             AMLToolkit.ViewModel.AMLTreeViewTemplate.CompleteInstanceHierarchyTree);
    ///             var treeView = new AMLTreeView ();
    ///             treeView.TreeViewModel = viewModel;
    ///             }
    ///       </code></example>
    /// </summary>
    public class AMLTreeView : Control
    {
        #region Public Fields

        /// <summary>
        /// Using a DependencyProperty as the backing store for AmlNodeStyle. This enables
        /// animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty AmlNodeStyleProperty =
            DependencyProperty.Register("AmlNodeStyle", typeof(Style), typeof(AMLTreeView), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for TheTreeView. This enables
        /// animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty TheTreeViewProperty =
            DependencyProperty.Register("TheTreeView", typeof(TreeView), typeof(AMLTreeView), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for TreeViewModel. This enables
        /// animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty TreeViewModelProperty =
            DependencyProperty.Register("TreeViewModel", typeof(AMLTreeViewModel), typeof(AMLTreeView), new PropertyMetadata(null));

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        ///    The _last mouse down
        /// </summary>
        private Point _lastMouseDown;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///    Initializes static members of the <see cref="AMLTreeView"/> class.
        /// </summary>
        static AMLTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AMLTreeView), new FrameworkPropertyMetadata(typeof(AMLTreeView)));
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        ///    Wird ausgelöst, wenn sich das selektierte Objekt ändert.
        /// </summary>
        public event EventHandler<AmlNodeEventArgs> SelectedItemChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets or sets the aml node style.
        /// </summary>
        /// <value>The aml node style.</value>
        public Style AmlNodeStyle
        {
            get { return (Style)GetValue(AmlNodeStyleProperty); }
            set { SetValue(AmlNodeStyleProperty, value); }
        }

        /// <summary>
        ///    Gets the internal TreeView Control Object
        /// </summary>
        /// <value>The TreeView.</value>
        public TreeView TheTreeView
        {
            get { return (TreeView)GetValue(TheTreeViewProperty); }
            private set { SetValue(TheTreeViewProperty, value); }
        }

        /// <summary>
        ///    Gets or sets the TreeView model. The TreeView's ItemsSource Property is
        ///    bound to the Root's Children Collection of the TreeViewModel
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
        ///    Wird bei einer Überschreibung in einer abgeleiteten Klasse stets
        ///    aufgerufen, wenn <see
        ///    cref="M:System.Windows.FrameworkElement.ApplyTemplate"/> von Anwendungscode
        ///    oder internem Prozesscode aufgerufen wird.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetValue(TheTreeViewProperty, GetTemplateChild("TheTreeView") as TreeView);

            TheTreeView.SelectedItemChanged += SelectedItemOfTreeViewChanged;
            TheTreeView.DragOver += TheTreeView_DragOver;
            TheTreeView.Drop += TheTreeView_Drop;
            TheTreeView.MouseMove += TheTreeView_MouseMove;
            TheTreeView.MouseDown += TheTreeView_MouseDown;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///    Selektion eines Elements im TreeView wird an das Control durchgereicht
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">     The e.</param>
        private void SelectedItemOfTreeViewChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = TheTreeView.SelectedItem as AMLNodeViewModel;
            if (this.SelectedItemChanged != null)
                this.SelectedItemChanged(this, new AmlNodeEventArgs(selectedItem));
        }

        /// <summary>
        ///    Handles the DragOver event of the TheTreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">     
        ///    The <see cref="DragEventArgs"/> instance containing the event data.
        /// </param>
        private void TheTreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem item = VisualTreeUtilities.FindVisualParent<TreeViewItem>(e.OriginalSource as UIElement);
                if (item != null)
                {
                    var targetItem = item.DataContext as AMLNodeViewModel;
                    var draggedItem = e.Data.GetData(typeof(AMLNodeViewModel)) as AMLNodeViewModel;

                    if (draggedItem != null && TreeViewModel.CanDragDrop != null && TreeViewModel.CanDragDrop(TreeViewModel, draggedItem, targetItem))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///    Handles the Drop event of the TheTreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">     
        ///    The <see cref="DragEventArgs"/> instance containing the event data.
        /// </param>
        private void TheTreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = VisualTreeUtilities.FindVisualParent<TreeViewItem>(e.OriginalSource as UIElement);
                var draggedItem = e.Data.GetData(typeof(AMLNodeViewModel)) as AMLNodeViewModel;

                if (TargetItem != null && draggedItem != null)
                {
                    e.Handled = true;

                    var targetItem = TargetItem.DataContext as AMLNodeViewModel;

                    if (targetItem != null)
                    {
                        if (TreeViewModel.CanDragDrop != null && TreeViewModel.CanDragDrop(TreeViewModel, draggedItem, targetItem))
                        {
                            if (TreeViewModel.DoDragDrop != null)
                            {
                                TreeViewModel.DoDragDrop(TreeViewModel, draggedItem, targetItem);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///    Handles the MouseDown event of the TheTreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">     
        ///    The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance
        ///    containing the event data.
        /// </param>
        private void TheTreeView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(TheTreeView);
            }
        }

        /// <summary>
        ///    Handles the MouseMove event of the TheTreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">     
        ///    The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing
        ///    the event data.
        /// </param>
        private void TheTreeView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            e.Handled = false;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(TheTreeView);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    TreeViewItem item = VisualTreeUtilities.FindVisualParent<TreeViewItem>(e.OriginalSource as UIElement);
                    if (item != null)
                    {
                        var _draggedItem = (AMLNodeViewModel)TheTreeView.SelectedItem;
                        if (_draggedItem != null)
                        {
                            var dragData = new DataObject(typeof(AMLNodeViewModel), _draggedItem);
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(TheTreeView, dragData, DragDropEffects.Move);

                            e.Handled = true;
                        }
                    }
                }
            }
        }

        #endregion Private Methods
    }
}