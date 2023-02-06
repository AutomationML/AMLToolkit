// ***********************************************************************
// Assembly         : AMLEditorUIControls
// Author           : Josef Prinz
// Created          : 05-04-2016
//
// Last Modified By : Josef Prinz
// Last Modified On : 05-04-2016
// ***********************************************************************
// <copyright file="MultipleSelectionTreeView.cs" company="inpro">
//     Copyright © inpro 2016
// </copyright>
// <summary>This solution is copied from this
// <a href="http://chrigas.blogspot.de/2014/08/wpf-treeview-with-multiple-selection.html">blog</a>.
// </summary>
// ***********************************************************************

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Aml.Toolkit.ViewModel;
using Aml.Toolkit.XamlClasses;

/// <summary>
/// The CustomControls namespace.
/// </summary>
namespace Aml.Toolkit.View;

/// <summary>
///     Class TreeViewMultipleSelectionAttached:
///     With the WPF TreeView it is not possible to select multiple items. But it can be easily extended.
///     Therefore an Attached Property can be used.
/// </summary>
public class TreeViewMultipleSelectionAttached
{
    #region Private Fields

    /// <summary>
    ///     Selection and deselection is happening by the DependencyProperty IsItemSelected.
    ///     This property is used in XAML to apply a certain style on the selected TreeViewItems.
    /// </summary>
    public static readonly DependencyProperty IsItemSelectedProperty =
        DependencyProperty.RegisterAttached(
            "IsItemSelected",
            typeof(bool),
            typeof(TreeViewMultipleSelectionAttached),
            new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

    /// <summary>
    ///     With the IsMultipleSelection property a TreeView is marked in XAML to have multiple selection abilities.
    /// </summary>
    public static readonly DependencyProperty IsMultipleSelectionEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsMultipleSelectionEnabled",
            typeof(bool),
            typeof(TreeViewMultipleSelectionAttached),
            new PropertyMetadata(false));

    /// <summary>
    ///     With the IsMultipleSelection property a TreeView is marked in XAML to have multiple selection abilities.
    /// </summary>
    public static readonly DependencyProperty IsMultipleSelectionProperty =
        DependencyProperty.RegisterAttached(
            "IsMultipleSelection",
            typeof(bool),
            typeof(TreeViewMultipleSelectionAttached),
            new PropertyMetadata(false, OnMultipleSelectionPropertyChanged));

    /// <summary>
    ///     To associate the List of all selected TreeViewItems with the corresponding TreeView
    ///     the DependencyProperty SelectedItems is used. This property can be used in XAML to
    ///     bind the multiple selected items to a property of the DataContext.
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(TreeViewMultipleSelectionAttached),
            new PropertyMetadata(SelectedItemsChanged));

    /// <summary>
    ///     After deselecting all TreeViewItems by setting the IsItemSelected DependencyProperty to false,
    ///     the clicked TreeViewItem is marked as selected by setting the IsItemSelected DependencyProperty to true.
    ///     After that the TreeViewItem is marked as StartItem within the corresponding TreeView.
    ///     Therefore the private StartItem DependencyProperty is used. The StartItem DependencyProperty
    ///     is used as starting point for a subsequent multiple selection where a continuous range is selected.
    /// </summary>
    private static readonly DependencyProperty StartItemProperty =
        DependencyProperty.RegisterAttached(
            "StartItem",
            typeof(TreeViewItem),
            typeof(TreeViewMultipleSelectionAttached),
            new PropertyMetadata());

    #endregion Private Fields

    //static TreeViewMultipleSelectionAttached()
    //{
    //    ////E.g.create a dummy ListBox with dummy data, then remove one of its items;
    //    //the corresponding container will get the { DisconnectedItem} as its DataContext.

    //    ListBox l = new ListBox();
    //    l.ItemsSource = new List<string> { "2", "§" };
    //    l.UpdateLayout();
    //    var c = l.ItemContainerGenerator.ContainerFromItem("2");
    //    (l.ItemsSource as List<string>).Clear();
    //}

    #region Public Fields

    #endregion Public Fields

    #region Public Methods

    /// <summary>
    ///     This method is called by <see cref="SelectSingleItem" /> to deselected all Items.
    /// </summary>
    /// <param name="treeView">The tree view.</param>
    /// <param name="treeViewItem">The tree view item.</param>
    public static void DeSelectAllItems(TreeView treeView, TreeViewItem treeViewItem)
    {
        if (treeView != null)
        {
            for (var i = 0; i < treeView.Items.Count; i++)
            {
                if (treeView.ItemContainerGenerator.ContainerFromIndex(i) is not TreeViewItem item)
                {
                    continue;
                }

                SetIsItemSelected(item, false);
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }

                DeSelectAllItems(null, item);

                if (item.DataContext is AMLNodeViewModel selectedTreeViewItem)
                {
                    selectedTreeViewItem.IsSelected = false;
                }
            }
        }
        else
        {
            for (var i = 0; i < treeViewItem.Items.Count; i++)
            {
                if (treeViewItem.ItemContainerGenerator.ContainerFromIndex(i) is not TreeViewItem item)
                {
                    continue;
                }

                SetIsItemSelected(item, false);
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }

                DeSelectAllItems(null, item);
            }
        }
    }

    /// <summary>
    ///     Gets the is item selected.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool GetIsItemSelected(TreeViewItem element) => (bool)element.GetValue(IsItemSelectedProperty);

    /// <summary>
    ///     Gets the is multiple selection.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool GetIsMultipleSelection(TreeView element) => (bool)element.GetValue(IsMultipleSelectionProperty);

    /// <summary>
    ///     Gets the is multiple selection.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool GetIsMultipleSelectionEnabled(TreeView element) =>
        (bool)element.GetValue(IsMultipleSelectionEnabledProperty);

    /// <summary>
    ///     Gets the selected items.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>IList.</returns>
    public static IList GetSelectedItems(TreeView element) => (IList)element.GetValue(SelectedItemsProperty);

    /// <summary>
    ///     Sets the is item selected.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    public static void SetIsItemSelected(TreeViewItem element, bool value)
    {
        element.SetValue(IsItemSelectedProperty, value);
    }

    /// <summary>
    ///     Sets the is multiple selection.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    public static void SetIsMultipleSelection(TreeView element, bool value)
    {
        element.SetValue(IsMultipleSelectionProperty, value);
    }

    /// <summary>
    ///     Sets the is multiple selection.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    public static void SetIsMultipleSelectionEnabled(TreeView element, bool value)
    {
        element.SetValue(IsMultipleSelectionEnabledProperty, value);
    }

    /// <summary>
    ///     Sets the selected items.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    public static void SetSelectedItems(TreeView element, IList value)
    {
        element.SetValue(SelectedItemsProperty, value);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    ///     To find the TreeView corresponding to the TreeViewItem this recursive method FindTreeView is used.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>TreeView.</returns>
    private static TreeView FindTreeView(DependencyObject dependencyObject)
    {
        return dependencyObject switch
        {
            null => null,
            TreeView treeView => treeView,
            _ => VisualTreeUtilities.FindVisualParent<TreeView>(dependencyObject)
        };
    }

    /// <summary>
    ///     This method finds the TreeViewItem that is invoked with a click on the item.
    ///     This happens with this recursive FindTreeViewItem method.
    /// </summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>TreeViewItem.</returns>
    private static TreeViewItem FindTreeViewItem(DependencyObject dependencyObject)
    {
        return dependencyObject switch
        {
            null => null,
            TreeViewItem treeViewItem => treeViewItem,
            _ => VisualTreeUtilities.FindVisualParent<TreeViewItem>(dependencyObject)
        };
    }

    /// <summary>
    ///     With the GetAllItems method all TreeViewItems of a TreeView are recursively collected into a collection.
    ///     Furthermore all items are deselected by the DeSelectAllItems method.
    /// </summary>
    /// <param name="treeView">The tree view.</param>
    /// <param name="treeViewItem">The tree view item.</param>
    /// <param name="allItems">All items.</param>
    private static void GetAllItems(TreeView treeView, TreeViewItem treeViewItem,
        ICollection<TreeViewItem> allItems)
    {
        if (treeView == null)
        {
            for (var i = 0; i < treeViewItem.Items.Count; i++)
            {
                if (treeViewItem.ItemContainerGenerator.ContainerFromIndex(i) is not TreeViewItem item)
                {
                    continue;
                }

                allItems.Add(item);
                GetAllItems(null, item, allItems);
            }
        }
        else
        {
            for (var i = 0; i < treeView.Items.Count; i++)
            {
                if (treeView.ItemContainerGenerator.ContainerFromIndex(i) is not TreeViewItem item)
                {
                    continue;
                }

                allItems.Add(item);
                GetAllItems(null, item, allItems);
            }
        }
    }

    /// <summary>
    ///     Gets the start item.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>TreeViewItem.</returns>
    private static TreeViewItem GetStartItem(TreeView element) => (TreeViewItem)element.GetValue(StartItemProperty);

    /// <summary>
    ///     In the RegisterAttached method of <see cref="IsItemSelectedProperty" />we have defined a PropertyMetadata
    ///     object that defines this call back method (OnIsItemSelectedPropertyChanged) that is called if the property
    ///     (selection of the TreeViewItem) has changed. In that method the Header of TreeViewItem will be added
    ///     to or removed to a List of all selected TreeViewItems. This List is associated with the corresponding TreeView.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void OnIsItemSelectedPropertyChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var treeViewItem = d as TreeViewItem;
        var treeView = FindTreeView(treeViewItem);
        if (treeViewItem == null || !treeViewItem.HasHeader || treeView == null)
        {
            return;
        }

        var selectedItems = GetSelectedItems(treeView);
        if (selectedItems == null)
        {
            return;
        }

        if (GetIsItemSelected(treeViewItem))
        {
            if (treeViewItem.Header.GetType().Name == "Microsoft.Internal.NamedObject")
            {
                return;
            }

            //if (treeViewItem.Header is DisconnectedItem)
            _ = selectedItems.Add(treeViewItem.Header);
        }
        else
        {
            selectedItems.Remove(treeViewItem.Header);
        }
    }

    /// <summary>
    ///     In the RegisterAttached method of the <see cref="IsMultipleSelectionProperty" /> we have defined a
    ///     PropertyMetadata object that defines this call back method that is called if the property has changed.
    ///     In this method a handler for the MouseLeftButtonDownEvent of TreeViewItem is added or removed to the TreeView.
    ///     Remember to mark the handledEventsToo parameter in the AddHandler method as true to allow calling
    ///     the handler OnTreeViewItemClicked correctly.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void OnMultipleSelectionPropertyChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeView treeView)
        {
            return;
        }

        if (e.NewValue is not bool value)
        {
            return;
        }

        if (value)
        {
            treeView.AddHandler(UIElement.MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(OnTreeViewItemClicked), true);
        }
        else
        {
            treeView.RemoveHandler(UIElement.MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(OnTreeViewItemClicked));
        }
    }

    /// <summary>
    ///     The OnTreeViewItemClicked method is called on each click at a TreeViewItem.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
    private static void OnTreeViewItemClicked(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = FindTreeViewItem(
            e.OriginalSource as DependencyObject);
        var treeView = sender as TreeView;

        var isEnabled = GetIsMultipleSelectionEnabled(treeView);

        if (treeViewItem == null || treeView == null)
        {
            return;
        }

        switch (Keyboard.Modifiers)
        {
            case ModifierKeys.Control when isEnabled:
                SelectMultipleItemsRandomly(treeView, treeViewItem);
                break;
            case ModifierKeys.Shift when isEnabled:
                SelectMultipleItemsContinuously(treeView, treeViewItem);
                break;
            default:
                SelectSingleItem(treeView, treeViewItem);
                break;
        }
    }

    private static void SelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var treeView = d as TreeView;
        if (e.NewValue is INotifyCollectionChanged icoll)
        {
            CollectionChangedEventManager.AddHandler(icoll,
                (o, i) => { TreeViewMultipleSelectionAttached_CollectionChanged(treeView, o, i); });
        }
        //if (e.OldValue is INotifyCollectionChanged)
        //{
        //    (e.OldValue as INotifyCollectionChanged).CollectionChanged -= TreeViewMultipleSelectionAttached_CollectionChanged;

        //}
    }

    /// <summary>
    ///     If a LMB + Shift is performed when the method SelectMutlipleItemsContinuously is called.
    ///     If no StartItem is set, no action takes place. If the selected TreeViewItem is
    ///     equal to the startItem then the SelectSingleItem method is called and then the method is returned.
    ///     If both conditions not met multiple continuously TreeViewItems selection takes place.
    /// </summary>
    /// <param name="treeView">The tree view.</param>
    /// <param name="treeViewItem">The tree view item.</param>
    private static void SelectMultipleItemsContinuously(TreeView treeView,
        TreeViewItem treeViewItem)
    {
        var startItem = GetStartItem(treeView);
        if (startItem == null)
        {
            return;
        }

        if (startItem == treeViewItem)
        {
            SelectSingleItem(treeView, treeViewItem);
            return;
        }

        ICollection<TreeViewItem> allItems = new List<TreeViewItem>();
        GetAllItems(treeView, null, allItems);
        DeSelectAllItems(treeView, null);
        var isBetween = false;
        foreach (var item in allItems)
        {
            if (item == treeViewItem || item == startItem)
            {
                // toggle to true if first element is found and
                // back to false if last element is found
                isBetween = !isBetween;

                // set boundary element
                SetIsItemSelected(item, true);
                continue;
            }

            if (isBetween)
            {
                SetIsItemSelected(item, true);
            }
        }
    }

    /// <summary>
    ///     If a LMB + Ctrl is performed when the method SelectMultipleItemsRandomly is called.
    ///     In this method the IsItemSelected DependencyProperty of the clicked TreeViewItem is toggled.
    ///     Furthermore the StartItem DependencyProperty will be set of not already set or unset if no
    ///     TreeViewItem is selected anymore.
    /// </summary>
    /// <param name="treeView">The tree view.</param>
    /// <param name="treeViewItem">The tree view item.</param>
    private static void SelectMultipleItemsRandomly(TreeView treeView,
        TreeViewItem treeViewItem)
    {
        SetIsItemSelected(treeViewItem, !GetIsItemSelected(treeViewItem));
        if (GetStartItem(treeView) == null)
        {
            if (GetIsItemSelected(treeViewItem))
            {
                SetStartItem(treeView, treeViewItem);
            }
        }
        else
        {
            var selectedItems = GetSelectedItems(treeView);
            if (selectedItems is { Count: 0 })
            {
                SetStartItem(treeView, null);
            }
        }
    }

    /// <summary>
    ///     The <see cref="OnTreeViewItemClicked" /> method checks what kind of click is happening.
    ///     There are Left-Mouse-Button (LMB) + Ctrl, LMB + Shift, and all other cases with LMB.
    ///     In the third case a single click is performed.
    /// </summary>
    /// <param name="treeView">The tree view.</param>
    /// <param name="treeViewItem">The tree view item.</param>
    private static void SelectSingleItem(TreeView treeView,
        TreeViewItem treeViewItem)
    {
        var isSelected = GetIsItemSelected(treeViewItem);
        if (isSelected && GetSelectedItems(treeView).Count == 1)
        {
            return;
        }

        // first deselect all items
        DeSelectAllItems(treeView, null);
        SetIsItemSelected(treeViewItem, true);
        SetStartItem(treeView, treeViewItem);
    }

    /// <summary>
    ///     Sets the start item.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    private static void SetStartItem(TreeView element, TreeViewItem value)
    {
        element.SetValue(StartItemProperty, value);
    }

    private static void TreeViewMultipleSelectionAttached_CollectionChanged(TreeView tree, object sender,
        NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add when e.NewItems != null:
            {
                foreach (var item in e.NewItems.OfType<ITreeNode>())
                {
                    if (!item.IsSelected)
                    {
                        item.IsSelected = true;
                    }
                }

                break;
            }
            case NotifyCollectionChangedAction.Remove when e.OldItems != null:
            {
                foreach (var item in e.OldItems.OfType<ITreeNode>())
                {
                    if (item.IsSelected)
                    {
                        item.IsSelected = false;
                    }
                }

                break;
            }
            case NotifyCollectionChangedAction.Reset:
                DeSelectAllItems(tree, null);
                break;
        }
    }

    #endregion Private Methods
}