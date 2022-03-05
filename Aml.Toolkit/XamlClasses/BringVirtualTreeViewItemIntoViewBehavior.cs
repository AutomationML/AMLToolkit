using Aml.Toolkit.ViewModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;

namespace Aml.Toolkit.XamlClasses
{
    /// <summary>
    ///     Class implements an attached behaviour to bring a selected TreeViewItem in VIRTUALIZED TreeView
    ///     into view when selection is driven by the viewmodel (not the user). The System.Windows.Interactivity
    ///     library is required for this behavior to compile.
    ///     Sample Usage:
    ///     &lt;i:Interaction.Behaviors>
    ///     &lt;behav:BringVirtualTreeViewItemIntoViewBehavior SelectedItem = "{Binding SelectPathItem}" />
    ///     &lt;/ i:Interaction.Behaviors>
    ///     This behaviour requires a binding to a path like structure of tree view (viewmodel) items.
    ///     This implementation requieres an array of objects (object[] SelectedItem) that represents
    ///     each tree view item along the path that should be browsed with this behaviour.
    ///     Allows two-way binding of a TreeView's selected item.
    ///     Sources:
    ///     http://stackoverflow.com/q/183636/46635
    /// </summary>
    public class BringVirtualTreeViewItemIntoViewBehavior
    {
        /// <summary>
        /// Brings the node into view
        /// </summary>
        /// <param name="nodepath"></param>
        /// <param name="tree"></param>
        internal static void BringNodeToView(AMLNodeViewModel[] nodepath, TreeView tree)
        {
            // Sanity check: Are we looking at the least required data we need?
            if (nodepath == null)
            {
                return;
            }

            if (nodepath.Length <= 1)
            {
                return;
            }

            // params look good so lets find the attached tree view (aka ItemsControl)

            var currentParent = tree as ItemsControl;

            // Now loop through each item in the array of bound path items and make sure they exist
            for (var i = 0; i < nodepath.Length; i++)
            {
                var node = nodepath[i];

                // first try the easy way
                if (currentParent.ItemContainerGenerator.ContainerFromItem(node) is not TreeViewItem newParent)
                {
                    // if this failed, it's probably because of virtualization, and we will have to do it the hard way.
                    // this code is influenced by TreeViewItem.ExpandRecursive decompiled code, and the MSDN sample at http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
                    // see also the question at http://stackoverflow.com/q/183636/46635
                    currentParent.ApplyTemplate();
                    var itemsPresenter = (ItemsPresenter)currentParent.Template.FindName("ItemsHost", currentParent);
                    if (itemsPresenter != null)
                    {
                        if (!itemsPresenter.ApplyTemplate())
                        {
                            currentParent.UpdateLayout();
                        }
                    }
                    else
                    {
                        currentParent.UpdateLayout();
                    }

                    var virtualizingPanel = GetItemsHost(currentParent) as VirtualizingPanel;

                    //CallEnsureGenerator(virtualizingPanel);
                    var index = currentParent.Items.IndexOf(node);
                    if (index < 0)
                    {
                        // This is raised when the item in the path array is not part of the tree collection
                        // This can be tricky, because Binding an ObservableDictionary to the treeview will
                        // require that we need an array of KeyValuePairs<K,T>[] here :-(
#if DEBUG
                        throw new System.InvalidOperationException("Node '" + node + "' cannot be fount in container");
#else
                        return;
#endif
                    }

                    virtualizingPanel?.BringIndexIntoViewPublic(index);
                    newParent = currentParent.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
                }

                if (newParent == null)
                {
#if DEBUG
                    throw new System.InvalidOperationException("Tree view item cannot be found or created for node '" + node +
                                                        "'");
#else
                    return;
#endif
                }

                if (node == nodepath[nodepath.Length - 1])
                {
                    //newParent.IsSelected = true;
                    newParent.BringIntoView();
                    break;
                }

                // Make sure nodes (except for last child node) are expanded to make children visible
                if (i < nodepath.Length - 1)
                {
                    newParent.IsExpanded = true;
                }

                currentParent = newParent;
            }
        }


        #region Functions to get internal members using reflection

        // Some functionality we need is hidden in internal members, so we use reflection to get them

        #region ItemsControl.ItemsHost

        private static readonly PropertyInfo ItemsHostPropertyInfo =
            typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <returns></returns>
        private static Panel GetItemsHost(ItemsControl itemsControl)
        {
            Debug.Assert(itemsControl != null);
            return ItemsHostPropertyInfo.GetValue(itemsControl, null) as Panel;
        }

        #endregion ItemsControl.ItemsHost

        
        #endregion Functions to get internal members using reflection
    }
}
