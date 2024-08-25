// Copyright (c) 2017 AutomationML e.V.
using Aml.Engine.CAEX.Commands;
using Aml.Toolkit.ViewModel;
using Aml.Toolkit.ViewModel.Commands;
using Aml.Toolkit.XamlClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;

/* Unmerged change from project 'Aml.Toolkit (net8.0-windows)'
Before:
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Aml.Engine.CAEX.Commands;
using Aml.Toolkit.ViewModel;
using Aml.Toolkit.ViewModel.Commands;
After:
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aml.Engine.Documents;
using Aml.Toolkit.Input;
using System.Windows.Media;
*/

/* Unmerged change from project 'Aml.Toolkit (net6.0-windows)'
Before:
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Aml.Engine.CAEX.Commands;
using Aml.Toolkit.ViewModel;
using Aml.Toolkit.ViewModel.Commands;
After:
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aml.Engine.Documents;
using Aml.Toolkit.Input;
using System.Windows.Media;
*/
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

/// <summary>
///    The Aml.Toolkit namespace.
/// </summary>
namespace Aml.Toolkit.View;

/// <summary>
///     The AMLTreeView is a Control, which arranges CAEX-ElementTrees in a TreeView
///     and assigns a default icon for each distinctive CAEX-Element
///     <example>
///         This is a
///         code example which shows, how to create a ViewModel with AMLDocument Data,
///         which can be bound to a TreeView. The example uses the AMLEngine.
///         <code keepSeeTags="true" title="c#">
///                               {
///                               // read the AMLDocument
///                               var doc = CAEXDocument.LoadFromFile("myFile.aml");
///                               // create a viewModel, using the TreeView Template <see
///                 cref="AMLTreeViewTemplate.CompleteInstanceHierarchyTree" />
///                               var viewModel = new Aml.Toolkit.ViewModel.AMLTreeViewModel(
///                               (XElement)doc.CAEXFile.Node,
///                               Aml.Toolkit.ViewModel.AMLTreeViewTemplate.CompleteInstanceHierarchyTree);
///                               var treeView = new AMLTreeView ();
///                               treeView.TreeViewModel = viewModel;
///                               }
///       </code>
///     </example>
/// </summary>
public class AMLTreeView : Control
{
    #region Internal Properties

    internal ScrollViewer ScrollViewer
    {
        get
        {
            if (_scrollViewer != null)
            {
                return _scrollViewer;
            }

            var border = VisualTreeHelper.GetChild(TheTreeView, 0);
            _scrollViewer = border is ScrollViewer viewer ? viewer : VisualTreeHelper.GetChild(border, 0) as ScrollViewer;

            return _scrollViewer;
        }
    }

    #endregion Internal Properties

    #region Public Events

    /// <summary>
    ///     Triggered when the selected object changes.
    /// </summary>
    public event EventHandler<AmlNodeEventArgs> SelectedItemChanged
    {
        add => _myEventSource.Subscribe(value);
        remove => _myEventSource.Unsubscribe(value);
    }

    #endregion Public Events

    #region Public Fields

    /// <summary>
    ///     DependencyProperty as the backing store for IsMultipleSelection.
    /// </summary>
    public static readonly DependencyProperty IsMultipleSelectionProperty =
        DependencyProperty.Register(nameof(IsMultipleSelection), typeof(bool), typeof(AMLTreeView),
            new PropertyMetadata(false));

    /// <summary>
    ///     Using a DependencyProperty as the backing store for TheTreeView.
    /// </summary>
    public static readonly DependencyProperty TheTreeViewProperty =
        DependencyProperty.Register(nameof(TheTreeView), typeof(TreeView), typeof(AMLTreeView),
            new PropertyMetadata(null));

    /// <summary>
    ///     Using a DependencyProperty as the backing store for TreeViewModel.
    /// </summary>
    public static readonly DependencyProperty TreeViewModelProperty =
        DependencyProperty.Register(nameof(TreeViewModel), typeof(AMLTreeViewModel), typeof(AMLTreeView),
            new PropertyMetadata(null, OnTreeViewModelChanged));

    #endregion Public Fields

    #region Private Fields

    private readonly WeakEventSource<AmlNodeEventArgs> _myEventSource = new();

    private AMLNodeViewModel _draggedItem;

    /// <summary>
    ///     The _last mouse down
    /// </summary>
    private Point _lastMouseDown;

    private ScrollViewer _scrollViewer;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    ///     Initializes static members of the <see cref="AMLTreeView" /> class.
    /// </summary>
    static AMLTreeView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AMLTreeView),
            new FrameworkPropertyMetadata(typeof(AMLTreeView)));
        Execute.InitializeWithDispatcher();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLTreeView" /> class.
    /// </summary>
    public AMLTreeView()
    {
        //Unloaded += AMLTreeView_Unloaded;
        KeyDown += AMLTreeView_KeyDown;

        ContextMenuOpening += AMLTreeView_ContextMenuOpening;
    }

    private void AMLTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        TreeViewModel?.ConfigureContextMenu();
        ContextMenu?.GetBindingExpression(ItemsControl.ItemsSourceProperty)?.UpdateTarget();
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets or sets the internal links adorner.
    /// </summary>
    /// <value>
    ///     The internal links adorner.
    /// </value>
    public TreeViewLinksAdorner InternalLinksAdorner { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance supports multiple selection.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance supports multiple selection; otherwise, <c>false</c>.
    /// </value>
    public bool IsMultipleSelection
    {
        get => (bool)GetValue(IsMultipleSelectionProperty);
        set => SetValue(IsMultipleSelectionProperty, value);
    }

    /// <summary>
    ///     Gets the internal TreeView Control Object
    /// </summary>
    /// <value>The TreeView.</value>
    public TreeView TheTreeView
    {
        get => (TreeView)GetValue(TheTreeViewProperty);
        private set => SetValue(TheTreeViewProperty, value);
    }

    /// <summary>
    ///     Gets or sets the TreeView model. The TreeView's ItemsSource Property is
    ///     bound to the Root's Children Collection of the TreeViewModel
    /// </summary>
    /// <value>The TreeView model.</value>
    public AMLTreeViewModel TreeViewModel
    {
        get => (AMLTreeViewModel)GetValue(TreeViewModelProperty);
        set => SetValue(TreeViewModelProperty, value);
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    ///     Unselects all.
    /// </summary>
    public void DeselectAll()
    {
        if (TreeViewModel == null)
        {
            return;
        }

        TreeViewMultipleSelectionAttached.DeSelectAllItems(TheTreeView, null);

        if (TreeViewModel?.SelectedElements.Count > 0)
        {
            TreeViewModel.SelectedElements?.Clear();
        }

        InternalLinksAdorner?.ClearSelection(true);
    }

    /// <summary>
    ///     Calculates the Coordinates of a Node in the TreeView, relative to the
    ///     TreeView and returns the Node Location.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>
    ///     Location of the Node if it is contained in the treeView and visible.
    ///     Otherwise Point(0d, 0d) is returned.
    /// </returns>
    public Point NodeCoordinates(AMLNodeViewModel node)
    {
        var Parents = new Stack<AMLNodeViewModel>();
        var parent = node;

        while (parent.Tree.Root != parent.Parent)
        {
            Parents.Push(parent);
            parent = parent.Parent;
        }

        var tvItem = TheTreeView.ItemContainerGenerator.ContainerFromItem(parent) as TreeViewItem;
        while (Parents.Count > 0 && tvItem != null)
        {
            parent = Parents.Pop();
            tvItem = tvItem.ItemContainerGenerator.ContainerFromItem(parent) as TreeViewItem;
        }

        return tvItem?.TransformToAncestor(TheTreeView).Transform(new Point(0d, 0d)) ?? new Point(0d, 0d);
    }

    /// <summary>
    ///     Wird bei einer Überschreibung in einer abgeleiteten Klasse stets
    ///     aufgerufen, wenn
    ///     <see
    ///         cref="M:System.Windows.FrameworkElement.ApplyTemplate" />
    ///     von Anwendungscode
    ///     oder internem Prozesscode aufgerufen wird.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        //RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
        if (GetTemplateChild("InnerTreeView") is not TreeView tree)
        {
            return;
        }

        SetValue(TheTreeViewProperty, tree);

        tree.PreviewDragOver -= TheTreeView_DragOver;
        tree.Drop -= TheTreeView_Drop;
        tree.MouseMove -= TheTreeView_MouseMove;
        tree.PreviewMouseDown -= TheTreeView_MouseDown;
        tree.PreviewMouseUp -= Tree_PreviewMouseUp;
        tree.MouseUp -= Tree_MouseUp;
        tree.KeyUp -= Tree_KeyUp;

        tree.PreviewDragOver += TheTreeView_DragOver;
        tree.Drop += TheTreeView_Drop;
        tree.MouseMove += TheTreeView_MouseMove;
        tree.PreviewMouseDown += TheTreeView_MouseDown;
        tree.PreviewMouseUp += Tree_PreviewMouseUp;
        tree.MouseUp += Tree_MouseUp;
        tree.KeyUp += Tree_KeyUp;
    }

    #endregion Public Methods

    #region Internal Methods

    internal void AddLinksAdorner()
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer == null || InternalLinksAdorner != null)
        {
            return;
        }

        adornerLayer.Add(InternalLinksAdorner = new TreeViewLinksAdorner(this));

        adornerLayer.MouseLeftButtonDown -= AdornerLayerLineSelectionEvent;
        adornerLayer.MouseLeftButtonDown += AdornerLayerLineSelectionEvent;
        //this.MouseLeftButtonUp -= AdornerLayerLineSelection;
        //this.MouseLeftButtonUp += AdornerLayerLineSelection;
    }

    internal void MarkNode(AMLNodeViewModel node)
    {
        if (TheTreeView == null)
        {
            return;
        }

        _ = Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        {
            InternalLinksAdorner?.Suspend();

            var parents = new Stack<AMLNodeViewModel>();

            if (node == null)
            {
                return;
            }

            while (node.Tree.Root != node.Parent)
            {
                parents.Push(node);
                node = node.Parent;
            }

            parents.Push(node);

            try
            {
                BringVirtualTreeViewItemIntoViewBehavior.BringNodeToView(parents.ToArray(), TheTreeView);
                //TheTreeView.Focus();
            }
            catch (InvalidOperationException)
            {
            }

            InternalLinksAdorner?.Resume();
        }));
    }

    internal void RemoveLinksAdorner()
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer == null || InternalLinksAdorner == null)
        {
            return;
        }

        adornerLayer.Remove(InternalLinksAdorner);
        InternalLinksAdorner = null;

        //this.MouseLeftButtonUp -= AdornerLayerLineSelection;
        //adornerLayer.MouseLeftButtonUp -= AdornerLayerLineSelection;
    }

    #endregion Internal Methods

    #region Private Methods

    private static void OnTreeViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var tree = d as AMLTreeView;

        if (e.OldValue != null)
        {
            tree?.SetTreeViewModel(e.OldValue as AMLTreeViewModel, false);
        }

        tree?.SetTreeViewModel(e.NewValue as AMLTreeViewModel, true);
    }

    private bool AdornerLayerLineSelection(MouseEventArgs e)
    {
        if (TreeViewModel == null)
        {
            return false;
        }

        if (TreeViewModel.IsReadonly)
        {
            return false;
        }

        if (InternalLinksAdorner == null)
        {
            return false;
        }

        var point = TranslatePoint(e.GetPosition(this), InternalLinksAdorner);
        var link = InternalLinksAdorner?.SelectLink(point);
        if (link == null)
        {
            return false;
        }

        TreeViewModel.SelectLink(link);
        return true;
    }

    private void AdornerLayerLineSelectionEvent(object sender, MouseButtonEventArgs e)
    {
        _ = AdornerLayerLineSelection(e);
    }

    private void AMLTreeView_KeyDown(object sender, KeyEventArgs e)
    {
        var item = e.OriginalSource as FrameworkElement;
        if (item?.DataContext is not AMLNodeWithClassReference node)
        {
            node = TheTreeView.SelectedItem as AMLNodeWithClassReference;
        }

        if (node == null)
        {
            return;
        }

        if (e.Key == Key.Left)
        {
            if (node.HasLinks && node.ShowLinks)
            {
                node.ShowLinks = false;
            }
        }
        else if (e.Key == Key.Right)
        {
            if (node.HasLinks && !node.ShowLinks)
            {
                node.ShowLinks = true;
            }
        }
    }

    private void AMLTreeView_Unloaded(object sender, RoutedEventArgs e)
    {
        SetTreeViewModel(TreeViewModel, false);
    }

    private void AMLTreeViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (TreeViewModel == null)
        {
            return;
        }

        if (e.PropertyName == nameof(TreeViewModel.Focused))
        {
            _ = Focus();
        }
    }

    /// <summary>
    ///     Search for an element of a certain type in the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of element to find.</typeparam>
    /// <param name="visual">The parent element.</param>
    /// <returns></returns>
    private T FindChildControl<T>(Visual visual) where T : Visual
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
        {
            var child = (Visual)VisualTreeHelper.GetChild(visual, i);
            if (child is T correctlyTyped)
            {
                return correctlyTyped;
            }

            var descendent = FindChildControl<T>(child);
            if (descendent != null)
            {
                return descendent;
            }
        }

        return null;
    }


    private void SelectedItemOfTreeViewChanged()
    {
        if (TreeViewModel == null || TheTreeView.SelectedItem is not AMLNodeViewModel selectedItem ||
            selectedItem == TreeViewModel.SelectedElements?.FirstOrDefault())
        {
            return;
        }

        selectedItem.CanNavigate = true;
        RaiseSelectionEvent(selectedItem);
    }


    internal void RaiseSelectionEvent(AMLNodeViewModel node)
    {
        if (TreeViewModel.SelectedElements is { Count: 0 })
        {
            TreeViewModel.SelectedElements.Add(node);
        }
        else
        {
            if (TreeViewModel.SelectedElements != null)
            {
                TreeViewModel.SelectedElements[0] = node;
            }
        }

        _myEventSource.Raise(this, new AmlNodeEventArgs(node));
    }

    private void SetTreeViewModel(AMLTreeViewModel aMLTreeViewModel, bool set)
    {
        if (aMLTreeViewModel == null)
        {
            return;
        }

        if (set)
        {
            aMLTreeViewModel.PropertyChanged += AMLTreeViewModel_PropertyChanged;
            aMLTreeViewModel.TreeViewLayoutUpdated += TreeViewLayoutChanged;
            //PropertyChangedEventManager.AddHandler(aMLTreeViewModel, AMLTreeViewModel_PropertyChanged, string.Empty);
            //PropertyChangedEventManager.AddHandler(aMLTreeViewModel.TreeViewLayout, TreeViewLayoutChanged, nameof(AMLLayout.ShowLinkLines));

            aMLTreeViewModel.View = this;
        }
        else
        {
            aMLTreeViewModel.PropertyChanged -= AMLTreeViewModel_PropertyChanged;
            aMLTreeViewModel.TreeViewLayoutUpdated -= TreeViewLayoutChanged;

            //PropertyChangedEventManager.RemoveHandler(aMLTreeViewModel, AMLTreeViewModel_PropertyChanged, string.Empty);
            //PropertyChangedEventManager.RemoveHandler(aMLTreeViewModel.TreeViewLayout, TreeViewLayoutChanged, nameof(AMLLayout.ShowLinkLines));
        }
    }

    /// <summary>
    ///     Handles the DragOver event of the TheTreeView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///     The <see cref="DragEventArgs" /> instance containing the event data.
    /// </param>
    private void TheTreeView_DragOver(object sender, DragEventArgs e)
    {
        try
        {
            //Debug.WriteLine($"Drag over { TreeViewModel.Root.Name}");
            e.Effects = DragDropEffects.None;

            AMLNodeViewModel targetItem;

            // Verify that this is a valid drop and then store the drop target
            var item = VisualTreeUtilities.FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (item == null)
            {
                // check, if the treeview could be used as a drop target
                targetItem = TreeViewModel;
            }
            else
            {
                targetItem = item.DataContext as AMLNodeViewModel;
            }

            if (e.Data?.GetData(typeof(AMLNodeViewModel)) is AMLNodeViewModel draggedItem &&
                TreeViewModel.CanDragDrop != null &&
                TreeViewModel.CanDragDrop(TreeViewModel, draggedItem, targetItem))
            {
                e.Effects = DragDropEffects.Move;
                TreeViewModel.IsDragging = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }
        catch
        {
        }
    }

    /// <summary>
    ///     Handles the Drop event of the TheTreeView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///     The <see cref="DragEventArgs" /> instance containing the event data.
    /// </param>
    private void TheTreeView_Drop(object sender, DragEventArgs e)
    {
        try
        {
            //Debug.WriteLine($"Drop {TreeViewModel.Root.Name}");

            // Verify that this is a valid drop and then store the drop target
            var item =
                VisualTreeUtilities.FindVisualParent<TreeViewItem>(e.OriginalSource as DependencyObject);

            AMLNodeViewModel targetItem;

            if (item == null)
            {
                // check, if the treeview could be used as a drop target
                targetItem = TreeViewModel;
            }
            else
            {
                targetItem = item.DataContext as AMLNodeViewModel;
            }


            if (targetItem != null && e.Data?.GetData(typeof(AMLNodeViewModel)) is AMLNodeViewModel draggedItem)
            {
                e.Handled = true;

                if (TreeViewModel.CanDragDrop != null &&
                    TreeViewModel.CanDragDrop(TreeViewModel, draggedItem, targetItem))
                {
                    TreeViewModel.DoDragDrop?.Invoke(TreeViewModel, draggedItem, targetItem);
                    _lastMouseDown = new Point(0, 0);
                }
            }

            TreeViewModel.IsDragging = false;
        }
        catch
        {
        }
    }

    /// <summary>
    ///     Handles the MouseDown event of the TheTreeView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///     The <see cref="MouseButtonEventArgs" /> instance
    ///     containing the event data.
    /// </param>
    private void TheTreeView_MouseDown(object sender, MouseButtonEventArgs e)
    {
        //Debug.WriteLine($"Mouse down {TreeViewModel.Root.Name}");

        if (TreeViewModel == null)
        {
            return;
        }

        if (e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        TreeViewModel.IsDragging = false;
        _lastMouseDown = e.GetPosition(TheTreeView);
        _draggedItem = null;

        var uiElement = e.OriginalSource as DependencyObject;
        var item = VisualTreeUtilities.FindVisualParent<TreeViewItem>(uiElement) ??
                   VisualTreeUtilities.FindParentWithType<TreeViewItem>(uiElement);

        if (item != null)
        {
            _draggedItem = (AMLNodeViewModel)item.DataContext;
        }
        else if (ScrollViewer != null)
        {
            var rec = new Rect(0, 0, ScrollViewer.ViewportWidth,
                ScrollViewer.ViewportHeight); //CHANGE THIS DIMENSION TO YOUR LIKING
            if (!rec.Contains(_lastMouseDown))
            {
                return;
            }

            FrameworkElement frameworkElement = null;

            VisualTreeHelper.HitTest(TheTreeView,
                // Hit test filter.
                null,
                // Hit test result.
                delegate (HitTestResult result)
                {
                    frameworkElement = result.VisualHit as FrameworkElement;
                    if (frameworkElement is TreeViewItem)
                    {
                        return HitTestResultBehavior.Stop;
                    }

                    frameworkElement = VisualTreeUtilities.FindVisualParent<TreeViewItem>(frameworkElement);
                    return frameworkElement is TreeViewItem
                        ? HitTestResultBehavior.Stop
                        : HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(e.GetPosition(TheTreeView)));

            if (frameworkElement is TreeViewItem)
            {
                return;
            }

            if (!AdornerLayerLineSelection(e))
            {
                DeselectAll();
            }

            _lastMouseDown = new Point(0, 0);
        }
        else
        {
            if (!AdornerLayerLineSelection(e))
            {
                DeselectAll();
            }

            _lastMouseDown = new Point(0, 0);
        }
    }

    /// <summary>
    ///     Handles the MouseMove event of the TheTreeView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">
    ///     The <see cref="MouseEventArgs" /> instance containing
    ///     the event data.
    /// </param>
    private void TheTreeView_MouseMove(object sender, MouseEventArgs e)
    {
        if (TreeViewModel == null)
        {
            return;
        }


        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        e.Handled = false;
        if (_lastMouseDown == new Point(0, 0))
        {
            return;
        }


        var currentPosition = e.GetPosition(TheTreeView);

        var distance = Point.Subtract(currentPosition, _lastMouseDown).Length;
        if (!(distance > 15.0))
        {
            return;
        }

        if (_draggedItem == null)
        {
            return;
        }

        TreeViewModel.IsDragging = true;
        var dragData = new DataObject(typeof(AMLNodeViewModel), _draggedItem);
        var effects = DragDrop.DoDragDrop(TheTreeView, dragData, DragDropEffects.Move);
        e.Handled = effects != DragDropEffects.None;
    }

    private void Tree_KeyUp(object sender, KeyEventArgs e)
    {
        SelectedItemOfTreeViewChanged();
    }

    private void Tree_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (TreeViewModel == null)
        {
            return;
        }

        if (TreeViewModel.IsDragging)
        {
            return;
        }

        SelectedItemOfTreeViewChanged();
    }

    private void Tree_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        _lastMouseDown = new Point(0, 0);
    }

    private void TreeViewLayoutChanged(object sender, TreeViewLayoutUpdateEventArgs e)
    {
        var layout = e.NewLayout;

        if (e.OldLayout.ShowLinkLines != e.NewLayout.ShowLinkLines)
        {
            if (layout.ShowLinkLines)
            {
                AddLinksAdorner();
            }
            else
            {
                RemoveLinksAdorner();
            }
        }
    }

    #endregion Private Methods
}