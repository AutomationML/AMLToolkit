using System.Windows;
using System.Windows.Controls;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     Behavior of a tree view item allows to bring the item into view, when the node
///     gets selected from a method call.
/// </summary>
public static class TreeViewItemBehavior
{
    #region Public Fields

    /// <summary>
    ///     The bring into view when selected property
    /// </summary>
    public static readonly DependencyProperty BringIntoViewWhenSelectedProperty =
        DependencyProperty.RegisterAttached("BringIntoViewWhenSelected", typeof(bool),
            typeof(TreeViewItemBehavior), new UIPropertyMetadata(false, OnBringIntoViewWhenSelectedChanged));

    #endregion Public Fields

    #region Private Methods

    /// <summary>
    /// </summary>
    /// <param name="depObj"></param>
    /// <param name="e"></param>
    private static void OnBringIntoViewWhenSelectedChanged(DependencyObject depObj,
        DependencyPropertyChangedEventArgs e)
    {
        if (depObj is not TreeViewItem item)
        {
            return;
        }

        if (e.NewValue is bool == false)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            item.BringIntoView();
        }
    }

    #endregion Private Methods

    #region Public Methods

    /// <summary>
    ///     Gets the property of this behavior
    /// </summary>
    /// <param name="treeViewItem">The tree view item.</param>
    /// <returns></returns>
    public static bool GetBringIntoViewWhenSelected(DependencyObject treeViewItem)
    {
        if (treeViewItem == null)
        {
            return false;
        }

        return (bool)treeViewItem.GetValue(BringIntoViewWhenSelectedProperty);
    }

    /// <summary>
    ///     Sets the property of this behavior
    /// </summary>
    /// <param name="treeViewItem">The tree view item.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetBringIntoViewWhenSelected(DependencyObject treeViewItem, bool value)
    {
        treeViewItem?.SetValue(BringIntoViewWhenSelectedProperty, value);
    }

    #endregion Public Methods
}