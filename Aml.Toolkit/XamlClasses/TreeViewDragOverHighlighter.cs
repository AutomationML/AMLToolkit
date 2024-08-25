// Copyright (c) 2017 AutomationML e.V.
using System.Windows;
using System.Windows.Controls;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
/// </summary>
public static class TreeViewDragOverHighlighter
{
    #region Public Fields

    /// <summary>
    ///     Dependency Property IsPossibleDropTarget.
    ///     Is true if the TreeViewItem is a possible drop target (i.e., if it would receive
    ///     the OnDrop event if the mouse button is released right now).
    /// </summary>
    public static readonly DependencyProperty IsPossibleDropTargetProperty;

    #endregion Public Fields

    #region Public Constructors

    /// <summary>
    ///     Initializes the <see cref="TreeViewDragOverHighlighter" /> class.
    /// </summary>
    static TreeViewDragOverHighlighter()
    {
        // Get all drag enter/leave events for TreeViewItem.
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDragEnterEvent,
            new DragEventHandler(OnDragEvent), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDragLeaveEvent,
            new DragEventHandler(OnDragLeave), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDragOverEvent,
            new DragEventHandler(OnDragEvent), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDropEvent,
            new DragEventHandler(OnDragDrop), true);

        IsPossibleDropTargetProperty = IsPossibleDropTargetKey.DependencyProperty;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    ///     Getter for IsPossibleDropTarget
    /// </summary>
    public static bool GetIsPossibleDropTarget(DependencyObject obj) =>
        (bool)obj.GetValue(IsPossibleDropTargetProperty);

    #endregion Public Methods

    #region Private Fields

    /// <summary>
    ///     Property key (since this is a read-only DP) for the IsPossibleDropTarget property.
    /// </summary>
    private static readonly DependencyPropertyKey IsPossibleDropTargetKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "IsPossibleDropTarget",
            typeof(bool),
            typeof(TreeViewDragOverHighlighter),
            new FrameworkPropertyMetadata(null,
                CalculateIsPossibleDropTarget));

    /// <summary>
    ///     the TreeViewItem that is the current drop target
    /// </summary>
    private static TreeViewItem _currentItem;

    /// <summary>
    ///     Indicates whether the current TreeViewItem is a possible
    ///     drop target
    /// </summary>
    private static bool _dropPossible;

    #endregion Private Fields

    #region Private Methods

    /// <summary>
    ///     Coercion method which calculates the IsPossibleDropTarget property.
    /// </summary>
    private static object CalculateIsPossibleDropTarget(DependencyObject item, object value) =>
        item == _currentItem && _dropPossible;

    private static void OnDragDrop(object sender, DragEventArgs args)
    {
        lock (IsPossibleDropTargetProperty)
        {
            _dropPossible = false;

            _currentItem?.InvalidateProperty(IsPossibleDropTargetProperty);

            if (sender is TreeViewItem tvi)
            {
                tvi.InvalidateProperty(IsPossibleDropTargetProperty);
            }
        }
    }

    /// <summary>
    ///     Called when an item is dragged over the TreeViewItem.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="DragEventArgs" /> instance containing the event data.</param>
    private static void OnDragEvent(object sender, DragEventArgs args)
    {
        lock (IsPossibleDropTargetProperty)
        {
            _dropPossible = false;

            if (_currentItem != null)
            {
                // Tell the item that previously had the mouse that it no longer does.
                DependencyObject oldItem = _currentItem;
                _currentItem = null;
                oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }

            _dropPossible = args.Effects != DragDropEffects.None;

            if (sender is not TreeViewItem tvi)
            {
                return;
            }

            _currentItem = tvi;
            // Tell that item to re-calculate the IsPossibleDropTarget property
            _currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
        }
    }

    /// <summary>
    ///     Called when the drag cursor leaves the TreeViewItem
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="DragEventArgs" /> instance containing the event data.</param>
    private static void OnDragLeave(object sender, DragEventArgs args)
    {
        lock (IsPossibleDropTargetProperty)
        {
            _dropPossible = false;

            if (_currentItem != null)
            {
                // Tell the item that previously had the mouse that it no longer does.
                DependencyObject oldItem = _currentItem;
                _currentItem = null;
                oldItem.InvalidateProperty(IsPossibleDropTargetProperty);

                //Debug.WriteLine("leave set to null " + sender);
            }

            if (sender is not TreeViewItem tvi)
            {
                return;
            }

            _currentItem = tvi;
            tvi.InvalidateProperty(IsPossibleDropTargetProperty);
        }
    }

    #endregion Private Methods
}