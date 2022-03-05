namespace Aml.Toolkit.ViewModel.Graph
{
    internal class Vertex
    {
        #region Internal Methods

        internal bool IsVisible()
        {
            if (isVisited)
            {
                return isVisible;
            }

            isVisited = true;

            isVisible = IsVisible(Item);
            return isVisible;
        }

        #endregion Internal Methods

        #region Internal Fields

        internal const int widthOffset = 20;

        internal int Index;
        internal bool isVisible;
        internal bool isVisited;

        #endregion Internal Fields

        #region Internal Properties

        internal AMLNodeViewModel Item { get; set; }

        internal AMLNodeViewModel VisibleItem(out bool isParent)
        {
            isParent = false;
            if (IsVisible())
                return Item;

            isParent = true;
            return VisibleParent(Item);
        }

        #endregion Internal Properties

        #region Private Methods

        private static bool IsVisible(AMLNodeViewModel item)
        {
            if (!item.IsVisible)
            {
                return false;
            }

            var treeViewItem = item.TreeViewItem;
            return treeViewItem?.IsVisible ?? false;

            //FrameworkElement container = item.Tree.View as FrameworkElement;
            //Rect bounds = treeViewItem.TransformToAncestor(container).TransformBounds(new Rect(new Point(0, 0), new Size(treeViewItem.DesiredSize.Width + widthOffset,
            //    treeViewItem.DesiredSize.Height)));
            //Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            //return rect.Contains(bounds.TopRight) && rect.Contains(bounds.BottomRight);
        }

        private AMLNodeViewModel VisibleParent(AMLNodeViewModel item)
        {
            //if (item.IsVisible)
            //    return item;

            if (!(item.Parent is { } treeViewItem))
            {
                return null;
            }

            if (item.CAEXNode.Parent != treeViewItem.CAEXNode)
            {
                treeViewItem = treeViewItem.Tree.SelectCaexNode(item.CAEXNode.Parent, false);
            }

            if (treeViewItem == null)
                return null;

            return IsVisible(treeViewItem) ? treeViewItem : VisibleParent(treeViewItem);
        }

        #endregion Private Methods
    }
}