﻿// Copyright (c) 2017 AutomationML e.V.
using Aml.Editor.Plugin.Contracts;
using Aml.Engine.AmlObjects;
using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Commands;
using Aml.Engine.CAEX.Extensions;
using Aml.Engine.Services;
using Aml.Engine.Services.Interfaces;
using Aml.Engine.Xml.Extensions;
using Aml.Toolkit.View;
using Aml.Toolkit.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Delegate CanDragDropPredicate
/// </summary>
/// <param name="treeView">The TreeView where the target is located</param>
/// <param name="source">The source which is dragged.</param>
/// <param name="target">The target for the drop.</param>
/// <returns><c>true</c> if source drop on target is allowed, <c>false</c> otherwise.</returns>
public delegate bool CanDragDropPredicate(AMLTreeViewModel treeView, AMLNodeViewModel source,
    AMLNodeViewModel target);

/// <summary>
///     Delegate DoDragDropAction
/// </summary>
/// <param name="treeView">The TreeView where the target is located</param>
/// <param name="source">The source which is dragged.</param>
/// <param name="target">The target for the drop.</param>
public delegate void DoDragDropAction(AMLTreeViewModel treeView, AMLNodeViewModel source, AMLNodeViewModel target);

/// <summary>
///     Class AMLTreeViewModel can build AMLNode-Trees for any CAEX-Element
/// </summary>
public class AMLTreeViewModel : AMLNodeViewModel
{
    #region Protected Internal Fields

    /// <summary>
    ///     Gets a value, indicating if the node dragging is active.
    /// </summary>
    protected internal bool IsDragging = false;

    #endregion Protected Internal Fields

    #region Internal Properties

    /// <summary>
    /// </summary>
    internal HashSet<XElement> ExpandedLinkNodes { get; set; }

    #endregion Internal Properties

    #region Private Fields

    /// <summary>
    ///     accepted change mode options
    /// </summary>
    private const CAEXElementChangeMode AcceptedChangeModes = CAEXElementChangeMode.NameChanged | TreeChangeModes;

    /// <summary>
    ///     tree change mode options
    /// </summary>
    private const CAEXElementChangeMode TreeChangeModes =
        CAEXElementChangeMode.Moved | CAEXElementChangeMode.Added | CAEXElementChangeMode.Deleted;


    private readonly Stack<AMLNodeViewModel> _selectionNotifications = new();
    private readonly List<AMLNodeViewModel> registeredPartners = [];

    /// <summary>
    ///     <see cref="CanDragDrop" />
    /// </summary>
    private CanDragDropPredicate _canDragDrop;

    /// <summary>
    ///     <see cref="DoDragDrop" />
    /// </summary>
    private DoDragDropAction _doDragDrop;

    /// <summary>
    ///     <see cref="FilterItemViewModel" />
    /// </summary>
    private AmlSearchViewModel _filterItemViewModel;

    /// <summary>
    ///     <see cref="Focused" />
    /// </summary>
    private bool _focused;

    private bool _isActive;

    /// <summary>
    ///     <see cref="IsSearchEnabled" />
    /// </summary>
    private bool _isSearchEnabled;

    private AMLNodeViewModel _markedNode;

    /// <summary>
    ///     <see cref="Root" />
    /// </summary>
    private AMLNodeViewModel _root;

    //private int _markedIndex;
    /// <summary>
    ///     <see cref="ShowHiddenLinksCommand" />
    /// </summary>
    private RelayCommand<object> _showHiddenLinks;

    /// <summary>
    ///     <see cref="TreeViewLayout" />
    /// </summary>
    private AMLLayout _treeViewLayout;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLTreeViewModel" /> class.
    /// </summary>
    /// <param name="rootNode">The root node for the TreeView.</param>
    /// <param name="caexTagNames">
    ///     List of CAEX-Names, which define the visible Elements in the TreeView. A Name in the
    ///     List can be any CAEX-Element Name.
    /// </param>
    public AMLTreeViewModel(XElement rootNode, HashSet<string> caexTagNames)
        : base(null, rootNode, false)
    {
        TreeViewLayout = AMLLayout.CloneFromDefault();
        CAEXTagNames = caexTagNames;

        NodeFilters = new AmlNodeGroupFilter();
        NodeFilters.AddFilter(FilterNodesWithName);
        Tree = this;
        IsVisible = true;

        if (rootNode != null)
        {
            SetRoot(rootNode);
        }

        IsExpanded = true;

        CaexCommand.CAEXElementChangedEvent += OnCAEXElementChanged;
        var updater = ServiceLocator.GetService<IAutoUpdate>();
        if (updater != null)
        {
            updater.ReferenceUpdated += Updater_ReferenceUpdated;
        }

        CollectionChangedEventManager.AddHandler(SelectedElements, SelectedElementsChanged);
        RaiseNotifySelection = true;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLTreeViewModel" /> class.
    /// </summary>
    /// <param name="CaexTagNames">The caex tag names.</param>
    public AMLTreeViewModel(HashSet<string> CaexTagNames)
        : base(null, null, false)
    {
        TreeViewLayout = AMLLayout.CloneFromDefault();
        CAEXTagNames = CaexTagNames;
        NodeFilters = new AmlNodeGroupFilter();
        NodeFilters.AddFilter(FilterNodesWithName);
        Tree = this;
        IsVisible = true;
        IsExpanded = true;
    }

    #endregion Public Constructors

    #region Public Events

    /// <summary>
    ///     Occurs when [selection changed].
    /// </summary>
    public event EventHandler<AmlNodeEventArgs> SelectionChanged;

    /// <summary>
    ///     Occurs when [tree updated].
    /// </summary>
    public event EventHandler<AmlNodeEventArgs> TreeUpdated;

    /// <summary>
    ///     Event raised when a tree view layout is updated
    /// </summary>
    public event EventHandler<TreeViewLayoutUpdateEventArgs> TreeViewLayoutUpdated;

    #endregion Public Events

    #region Public Properties

    /// <summary>
    ///     Gets the aml TreeView.
    /// </summary>
    /// <value>The aml TreeView.</value>
    public AMLTreeView AmlTreeView => View as AMLTreeView;

    /// <summary>
    ///     Gets and sets the CanDragDropPredicate.
    /// </summary>
    public CanDragDropPredicate CanDragDrop
    {
        get => _canDragDrop;
        set => Set(ref _canDragDrop, value);
    }

    /// <summary>
    ///     Gets and sets the TheDragDropAction
    /// </summary>
    public DoDragDropAction DoDragDrop
    {
        get => _doDragDrop;
        set => Set(ref _doDragDrop, value);
    }


    /// <summary>
    ///     Gets the filtered classes.
    /// </summary>
    /// <value>The filtered classes.</value>
    public virtual List<(bool IsSelected, string Name)> FilteredClasses { get; } = null;

    /// <summary>
    ///     Gets and sets the FilterItemViewModel
    /// </summary>
    public AmlSearchViewModel FilterItemViewModel
    {
        get => _filterItemViewModel;
        set => Set(ref _filterItemViewModel, value);
    }

    /// <summary>
    ///     Gets and sets the Focused
    /// </summary>
    public bool Focused
    {
        get => _focused;
        set => Set(ref _focused, value);
    }

    /// <summary>
    ///     If the model is active
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => Set(ref _isActive, value);
    }

    /// <summary>
    ///     Gets and sets the IsSearchEnabled
    /// </summary>
    public bool IsSearchEnabled
    {
        get => _isSearchEnabled;
        set => Set(ref _isSearchEnabled, value);
    }

    /// <summary>
    ///     Gets or sets the marked node.
    /// </summary>
    /// <value>The marked node.</value>
    public AMLNodeViewModel MarkedNode
    {
        get => _markedNode;
        set
        {
            _markedNode = value;

            if (View is AMLTreeView amlTree && value != null)
            {
                amlTree.MarkNode(value);
            }
        }
    }

    /// <summary>
    ///     Gets the Node-Filters, defined for the TreeView. The Node-Filters may contain more than
    ///     one Filter. As default, the Node-Name Filter is set, which filters the Tree with the
    ///     names, defined in the <see cref="TreeViewLayout" />. To Refresh a Filter, simply call
    ///     <code> NodeFilters.Refresh()</code>
    /// </summary>
    public AmlNodeGroupFilter NodeFilters { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether selections should raise an event.
    /// </summary>
    /// <value><c>true</c> if [raise notify selection]; otherwise, <c>false</c>.</value>
    public bool RaiseNotifySelection { get; set; }

    /// <summary>
    ///     Gets and sets the Root
    /// </summary>
    public virtual AMLNodeViewModel Root
    {
        get => _root;
        set => Set(ref _root, value);
    }

    /// <summary>
    ///     Gets the selected CAEX object.
    /// </summary>
    /// <value>The selected CAEX object.</value>
    public CAEXWrapper SelectedCAEXObject => SelectedElements.FirstOrDefault()?.CAEXObject;

    /// <summary>
    ///     Gets the selected XML element.
    /// </summary>
    /// <value>The selected XML element.</value>
    public XElement SelectedElement => SelectedElements.FirstOrDefault()?.CAEXNode;

    /// <summary>
    ///     Gets the collection of selected elements.
    /// </summary>
    /// <value>The selected elements.</value>
    public ObservableCollection<AMLNodeViewModel> SelectedElements { get; } =
        [];

    /// <summary>
    ///     Gets the selected tree view node.
    /// </summary>
    /// <value>The selected node.</value>
    public AMLNodeViewModel SelectedNode => SelectedElements.FirstOrDefault();

    /// <summary>
    ///     The ShowHiddenLinksCommand - Command
    /// </summary>
    public ICommand ShowHiddenLinksCommand => _showHiddenLinks ??= new RelayCommand<object>(
        ShowHiddenLinksCommandExecute,
        ShowHiddenLinksCommandCanExecute);

    /// <summary>
    ///     Gets and sets the TreeViewLayout
    /// </summary>
    public AMLLayout TreeViewLayout
    {
        get => _treeViewLayout;
        set => Set(ref _treeViewLayout, value);
    }

    /// <summary>
    ///     Gets or sets the view.
    /// </summary>
    public Control View { get; set; }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    ///     Clears all content from the tree view
    /// </summary>
    public virtual void ClearAll(bool raiseNotification=true)
    {
        if (!raiseNotification)
        {
            RaiseNotifySelection = false;
        }
        ClearSelections();
        Commands.Clear();

        if (_children != null)
        {
            Children.Clear();
        }

        AmlTreeView?.InternalLinksAdorner?.Clear();
        Root = null;
        RaiseNotifySelection = true;
    }

    /// <summary>
    ///     Configures the context menu.
    /// </summary>
    public virtual void ConfigureContextMenu()
    {
        //SelectedNode?.RaisePropertyChanged (nameof(Commands));
    }

    /// <summary>
    ///     Clears the selections.
    /// </summary>
    public void ClearSelections()
    {
        if (SelectedElements == null || SelectedElements.Count == 0)
        {
            return;
        }

        foreach (var item in SelectedElements)
        {
            item.IsSelected = false;
        }

        SelectedElements.Clear();
    }

    /// <summary>
    ///     Gets the caexNode.
    /// </summary>
    /// <param name="caexNode">The next item.</param>
    /// <returns>AMLTreeViewItem.</returns>
    public AMLNodeViewModel GetCaexNodeFromTree(XElement caexNode)
    {
        if (caexNode == null || Root == null)
        {
            return null;
        }

        var nodes = caexNode.AncestorsAndSelf().TakeWhile(node => node != Root.CAEXNode).ToList();
        if (nodes.Count < 1)
        {
            return null;
        }

        var currentCollection = Root.VisibleChildren?.ToList();

        if (currentCollection == null)
        {
            return null;
        }

        AMLNodeViewModel amlTreeViewItem = null;

        for (var i = nodes.Count - 1; i >= 0; i--)
        {
            var nextTreeViewItem = FindTreeViewItem(currentCollection, nodes[i]);

            if (nextTreeViewItem != null)
            {
                amlTreeViewItem = nextTreeViewItem;
                if (i > 0)
                {
                    if (nextTreeViewItem.HasDummyChild)
                    {
                        return null;
                    }

                    currentCollection = nextTreeViewItem.Children?.ToList();
                }
            }

            if (nextTreeViewItem == null)
            {
                break;
            }
        }

        return amlTreeViewItem;
    }

    /// <summary>
    ///     Gets the caexNode.
    /// </summary>
    /// <param name="caexNode">The next item.</param>
    /// <returns>AMLTreeViewItem.</returns>
    public IEnumerable<AMLNodeViewModel> GetCaexNodesFromTree(XElement caexNode)
    {
        if (caexNode == null || Root == null)
        {
            yield break;
        }

        var nodes = caexNode.AncestorsAndSelf().TakeWhile(node => node != Root.CAEXNode).ToList();
        if (nodes.Count < 1)
        {
            yield break;
        }

        var currentCollection = Root.VisibleChildren?.ToList();

        if (currentCollection == null)
        {
            yield break;
        }

        for (var i = nodes.Count - 1; i >= 0; i--)
        {
            var nextTreeViewItem = FindTreeViewItem(currentCollection, nodes[i]);

            if (nextTreeViewItem == null)
            {
                continue;
            }

            yield return nextTreeViewItem;

            if (i > 0)
            {
                if (nextTreeViewItem.HasDummyChild)
                {
                    continue;
                }

                currentCollection = nextTreeViewItem.Children?.ToList();
            }
        }
    }

    /// <summary>
    ///     gets the children of the node, which should appear in the list.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="expand"></param>
    /// <returns></returns>
    public virtual IEnumerable<XElement> ModelChilds(AMLNodeViewModel node, bool expand)
    {
        var names = node.CAEXTagNames;

        var children = ModelChilds(node.CAEXObject, names, node.IsDerived, node.ShowMirrorData, node.ShowInheritance,
            expand);
        node.HasChilds = children.Any();

        return children;
    }

    /// <summary>
    ///     Selects the caexNode.
    /// </summary>
    /// <param name="caexNode">The next item.</param>
    /// <param name="select">if set to <c>true</c> [select].</param>
    /// <param name="changeSelection"></param>
    /// <param name="expand"></param>
    /// <param name="mark"></param>
    /// <param name="bringIntoView"></param>
    /// <returns>AMLTreeViewItem.</returns>
    public AMLNodeViewModel SelectCaexNode(XElement caexNode, bool select, bool changeSelection = false,
        bool expand = false, bool mark = false, bool bringIntoView = false)
    {
        if (caexNode == null || Root == null)
        {
            return null;
        }

        var nodes = caexNode.AncestorsAndSelf().TakeWhile(node => node != Root.CAEXNode).ToList();
        if (nodes.Count < 1)
        {
            return null;
        }

        AMLNodeViewModel amlTreeViewItem = null;
        var currentCollection = Root.VisibleChildren?.ToList();


        if (currentCollection != null)
        {
            for (var i = nodes.Count - 1; i >= 0; i--)
            {
                var nextTreeViewItem = FindTreeViewItem(currentCollection, nodes[i]);
                if (amlTreeViewItem != null && nextTreeViewItem == null && expand)
                {
                    amlTreeViewItem.IsExpanded = true;
                    nextTreeViewItem = FindTreeViewItem(currentCollection, nodes[i]);
                }

                if (nextTreeViewItem != null)
                {
                    amlTreeViewItem = nextTreeViewItem;
                    if (i > 0)
                    {
                        if (nextTreeViewItem.HasDummyChild && !mark && !select && !expand)
                        {
                            return null;
                        }

                        if ((expand || select || mark) && !nextTreeViewItem.IsExpanded)
                        {
                            nextTreeViewItem.IsExpanded = true;
                        }

                        currentCollection = nextTreeViewItem.Children?.ToList();
                    }
                }
            }
        }

        if (amlTreeViewItem != null && (select || mark))
        {
            if (!amlTreeViewItem.IsSelected && select)
            {
                if (changeSelection)
                {
                    ClearSelections();
                }

                Select(amlTreeViewItem);
            }

            if (mark)
            {
                amlTreeViewItem.IsMarked = true;
            }

            Tree.MarkNode(bringIntoView || select, amlTreeViewItem);
        }
        else if (!select && amlTreeViewItem != null && !amlTreeViewItem.CAEXNode.Equals(caexNode))
        {
            return null;
        }

        return amlTreeViewItem;
    }

    /// <summary>
    ///     Sets the root for this TreeView
    /// </summary>
    /// <param name="rootNode">The root node.</param>
    public void SetRoot(XElement rootNode)
    {
        ClearAll();

        if (rootNode == null)
        {
            return;
        }

        Root = CreateNode(rootNode, false); //new AMLNodeWithoutName(this, this, rootNode, false)
        Root.IsVisible = true;

        AddNode(Root);

        Root.LoadChildren();
        Root.IsExpanded = true;
    }

    /// <summary>
    ///     Sets the root for this TreeView
    /// </summary>
    /// <param name="rootNode">The root node.</param>
    public void SetRoot(AMLNodeViewModel rootNode, bool notifySelection=true)
    {
        RaiseNotifySelection = notifySelection;
        ClearAll();

        if (rootNode == null)
        {
            return;
        }

        Root = CreateNode(rootNode.CAEXNode, false); //new AMLNodeWithoutName(this, this, rootNode, false)
        Root.IsVisible = true;
        Root.ShowMirrorData = rootNode.ShowMirrorData;
        Root.ShowInheritance = rootNode.ShowInheritance;

        AddNode(Root);

        Root.LoadChildren();
        Root.IsExpanded = true;

        RaiseNotifySelection = true;
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    public static void SetVerified(AMLNodeViewModel node)
    {
        node?.RaisePropertyChanged(nameof(IsVerified)); 
        node?.RaisePropertyChanged(nameof(IsNotVerified));
    }


    /// <summary>
    ///     Use this method, if the tree view layout should be changed
    /// </summary>
    /// <param name="updatedLayout"></param>
    public void UpdateLayout(AMLLayout updatedLayout)
    {
        var oldLayout = TreeViewLayout.Copy();
        TreeViewLayout.Update(updatedLayout);
        TreeViewLayoutUpdated?.Invoke(this, new TreeViewLayoutUpdateEventArgs(oldLayout, TreeViewLayout));

        if (updatedLayout.ResolveExternals != oldLayout.ResolveExternals)
        {
            Root?.RefreshHierarchy();
        }

        TreeViewLayout.PropertyChanged -= TreeViewLayout_PropertyChanged;
        TreeViewLayout.PropertyChanged += TreeViewLayout_PropertyChanged;
    }

    private void TreeViewLayout_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        //return;

        // has no effect on link lines ???
        switch (e.PropertyName)
        {
            case nameof(AMLLayout.JumpMode):
            case nameof(AMLLayout.DrawDashedLines):
            case nameof(AMLLayout.DrawColoredLines):
            case nameof(AMLLayout.HideExpander):
                AmlTreeView?.InternalLinksAdorner?.InvalidateArrange();
                AmlTreeView?.InternalLinksAdorner?.UpdateLayout();
                break;
        }
    }

    #endregion Public Methods

    #region Internal Methods

    /// <summary>
    ///     Finds the TreeView item.
    /// </summary>
    /// <param name="currentCollection">The current collection.</param>
    /// <param name="XElement">The XML element.</param>
    /// <returns>AMLNodeViewModel.</returns>
    internal static AMLNodeViewModel FindTreeViewItem(IEnumerable<AMLNodeViewModel> currentCollection,
        XElement XElement)
    {
        foreach (var item in currentCollection)
        {
            if (item is AMLNodeGroupViewModel)
            {
                var findInGroup = FindTreeViewItem(item.Children, XElement);
                if (findInGroup != null)
                {
                    return findInGroup;
                }
            }

            if (item.CAEXNode.Equals(XElement))
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    ///     Finds the TreeView item.
    /// </summary>
    /// <param name="currentCollection">The current collection.</param>
    /// <param name="XElement">The XML element.</param>
    /// <returns>AMLNodeViewModel.</returns>
    internal static AMLNodeViewModel FindTreeViewItemInTree(IEnumerable<AMLNodeViewModel> currentCollection,
        XElement XElement)
    {
        foreach (var item in currentCollection)
        {
            if (item.CAEXNode.Equals(XElement))
            {
                return item;
            }

            if (item.HasDummyChild || !(item.Children?.Count > 0))
            {
                continue;
            }

            var foundInTree = FindTreeViewItemInTree([.. item.Children], XElement);
            if (foundInTree != null)
            {
                return foundInTree;
            }
        }

        return null;
    }

    /// <summary>
    ///     Finds the TreeView item.
    /// </summary>
    /// <param name="currentCollection">The current collection.</param>
    /// <param name="XElement">The XML element.</param>
    /// <returns>AMLNodeViewModel.</returns>
    internal static IEnumerable<AMLNodeViewModel> FindTreeViewItemsInTree(
        IEnumerable<AMLNodeViewModel> currentCollection,
        XElement XElement)
    {
        foreach (var item in currentCollection.ToList())
        {
            if (item is not AMLNodeGroupViewModel && item.CAEXNode.Equals(XElement))
            {
                yield return item;
            }

            if (item.HasDummyChild || !(item.Children?.Count > 0))
            {
                continue;
            }

            var foundInTree = FindTreeViewItemsInTree(item.Children, XElement).ToList();
            foreach (var citem in foundInTree)
            {
                if (citem is AMLNodeGroupViewModel)
                {
                    continue;
                }

                yield return citem;
            }
        }
    }

    #endregion Internal Methods

    #region Protected Internal Methods

    /// <summary>
    /// </summary>
    /// <param name="caexObject"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    protected internal virtual bool? GetVerificationState(CAEXObject caexObject, out string service)
    {
        service = null;
        return null;
    }

    /// <summary>
    ///     Selects the link.
    /// </summary>
    /// <param name="link">The link.</param>
    protected internal virtual void SelectLink(InternalLinkType link)
    {
    }

    #endregion Protected Internal Methods

    #region Protected Methods

    /// <summary>
    ///     Gets the changed tree node according to the provided change event arguments, supplied by
    ///     the AML engine command manager
    /// </summary>
    /// <param name="e">
    ///     The <see cref="CAEXElementChangeEventArgs" /> instance containing the event data.
    /// </param>
    /// <returns></returns>
    protected AMLNodeViewModel ChangedTreeNode(CAEXElementChangeEventArgs e)
    {
        if ((e.ChangeMode & CAEXElementChangeMode.ChangedEvent) == CAEXElementChangeMode.None)
        {
            return null;
        }

        if (e.CAEXElement.Name.LocalName == CAEX_CLASSModel_TagNames.INTERNALLINK_STRING)
        {
            return FindTreeViewItemInTree(Root.Children, e.CAEXParent);
        }

        var xElement = e.CAEXElement;

        if ((e.ChangeMode & CAEXElementChangeMode.ValueChanged) != CAEXElementChangeMode.None)
        {
            // some value changes are relevant for node layouts
            if (e.CAEXParent != null && e.CAEXParent.IsAttribute() && e.CAEXParent.Attribute("Name").Value == "refURI")
            {
                xElement = e.CAEXParent.Parent;
            }
        }

        return !CAEXTagNames.Contains(xElement.Name.LocalName)
            ? null
            : e.ChangeMode.HasFlag(CAEXElementChangeMode.Deleted) ||
            e.ChangeMode.HasFlag(CAEXElementChangeMode.Added)
            ? Root.CAEXNode == e.CAEXParent ? Root : FindTreeViewItemInTree(Root.Children, e.CAEXParent)
            : FindTreeViewItemInTree(Root.Children, xElement);
    }

    /// <summary>
    ///     Gets the changed tree node according to the provided change event arguments, supplied by
    ///     the AML engine command manager
    /// </summary>
    /// <param name="e">
    ///     The <see cref="CAEXElementChangeEventArgs" /> instance containing
    ///     the event data.
    /// </param>
    /// <returns></returns>
    protected IEnumerable<AMLNodeViewModel> ChangedTreeNodes(CAEXElementChangeEventArgs e)
    {
        static XElement NodeParent(XElement xElement, XElement parent)
        {
            var element = xElement;
            while (element.Name.LocalName == CAEX_CLASSModel_TagNames.REVISION_STRING)
            {
                element = element.Parent;
                if (element == null)
                {
                    return parent ?? xElement;
                }
            }
            return element;
        }

        if (Root == null)
        {
            return [];
        }

        if ((e.ChangeMode & CAEXElementChangeMode.ChangedEvent) == CAEXElementChangeMode.None)
        {
            return [];
        }

        if (e.CAEXElement.Name.LocalName == CAEX_CLASSModel_TagNames.INTERNALLINK_STRING)
        {
            return FindTreeViewItemsInTree(Root.Children, e.CAEXParent);
        }

        var xElement = e.CAEXElement;

        if ((e.ChangeMode & CAEXElementChangeMode.ValueChanged) != CAEXElementChangeMode.None)
        {
            // some value changes are relevant for node layouts
            if (e.CAEXParent != null && e.CAEXParent.IsAttribute())
            {
                switch (e.CAEXParent.Attribute("Name").Value)
                {
                    case AutomationMLBaseAttributeTypeLib.MinOccurrenceAttribute:
                    case AutomationMLBaseAttributeTypeLib.MaxOccurrenceAttribute:
                    case AutomationMLBaseAttributeTypeLib.DirectionAttribute:
                    case AutomationMLBaseAttributeTypeLib.CategoryAttribute:
                    case RefURIAttributeType.REF_URI_ATTRIBUTE:
                        xElement = e.CAEXParent.Parent;
                        break;
                }
            }
        }

        if (xElement.Name.LocalName is CAEX_CLASSModel_TagNames.REVISION_NEWVERSION_STRING or
            CAEX_CLASSModel_TagNames.VERSION_STRING or
            CAEX_CLASSModel_TagNames.REVISION_OLDVERSION_STRING)
        {
            if (xElement.Parent != null && e.CAEXParent != null)
            {
                xElement = NodeParent(xElement.Parent, e.CAEXParent);
            }
        }
        else
        {
            xElement = NodeParent(xElement, e.CAEXParent);
        }

        if (!CAEXTagNames.Contains(xElement.Name.LocalName))
        {
            while (xElement is { Name.LocalName: CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING })
            {
                xElement = xElement.Parent;
            }

            if (xElement == null || !CAEXTagNames.Contains(xElement.Name.LocalName))
            {
                return [];
            }
        }

        return e.ChangeMode.HasFlag(CAEXElementChangeMode.Deleted) ||
            e.ChangeMode.HasFlag(CAEXElementChangeMode.Added)
            ? Root.CAEXNode == e.CAEXParent
                ? [Root]
                : FindTreeViewItemsInTree(Root.Children, NodeParent(e.CAEXParent, e.CAEXParent))
            : FindTreeViewItemsInTree(Root.Children, xElement);
    }


    /// <summary>
    ///     Executes the update.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    ///     The <see cref="CAEXElementChangeEventArgs" /> instance containing the event data.
    /// </param>
    protected void ExecuteUpdate(object sender, CAEXElementChangeEventArgs e)
    {
        if (Root == null)
        {
            return;
        }

        Execute.OnUIThread(() =>
        {
            SelectedElements.ToList().ForEach(s =>
            {
                if (s.CAEXObject?.IsDeleted == true)
                {
                    SelectedElements.Remove(s);
                }
            });

            IEnumerable<AMLNodeViewModel> treeNodes;

            if ((e.ChangeMode & AcceptedChangeModes) == CAEXElementChangeMode.None)
            {
                if ((e.ChangeMode & CAEXElementChangeMode.ChangedEvent) == CAEXElementChangeMode.None)
                {
                    return;
                }

                if (e.CAEXAttributeName is not CAEX_CLASSModel_TagNames.CHANGEMODE_ATTRIBUTE and
                    not CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REF_PARTNER_SIDE_A and
                    not CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REF_PARTNER_SIDE_B)
                {
                    switch (e.ChangeMode & CAEXElementChangeMode.ValueChanged)
                    {
                        case CAEXElementChangeMode.None:
                            return;

                        default:

                            if (e.CAEXElement.Name.LocalName is CAEX_CLASSModel_TagNames.VERSION_STRING or
                                CAEX_CLASSModel_TagNames.REVISION_NEWVERSION_STRING or
                                CAEX_CLASSModel_TagNames.REVISION_OLDVERSION_STRING)
                            {
                                break;
                            }
                            // some value changes are relevant for node layouts
                            if (e.CAEXParent == null || !e.CAEXParent.IsAttribute())
                            {
                                return;
                            }

                            switch (e.CAEXParent.Attribute("Name").Value)
                            {
                                case AutomationMLBaseAttributeTypeLib.MinOccurrenceAttribute:
                                case AutomationMLBaseAttributeTypeLib.MaxOccurrenceAttribute:
                                case AutomationMLBaseAttributeTypeLib.DirectionAttribute:
                                case AutomationMLBaseAttributeTypeLib.CategoryAttribute:
                                case RefURIAttributeType.REF_URI_ATTRIBUTE:
                                    break;
                                default: return;
                            }

                            break;
                    }
                }
            }

            if (!(treeNodes = ChangedTreeNodes(e)).Any())
            {
                return;
            }

            if (e.CAEXAttributeName is CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REF_PARTNER_SIDE_A or
                    CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REF_PARTNER_SIDE_B)
            { 
               ;
            }


            foreach (var treeNode in treeNodes.ToList())
            {
                if ((e.ChangeMode & TreeChangeModes) != CAEXElementChangeMode.None)
                {
                    if ((e.ChangeMode & CAEXElementChangeMode.Moved) != 0)
                    {
                        treeNode.Parent.RefreshTree(true);
                        treeNode.RefreshNodeInformation(false);
                        TreeUpdated?.Invoke(this, new AmlNodeEventArgs(treeNode.Parent));
                    }
                    else
                    {
                        treeNode.RefreshTree(e.ChangeMode.HasFlag(CAEXElementChangeMode.Added), true);
                        treeNode.RefreshNodeInformation(false);
                        TreeUpdated?.Invoke(this, new AmlNodeEventArgs(treeNode));
                    }
                }
                else
                {
                    treeNode.RefreshNodeInformation(false);
                }

                if (treeNode is AMLNodeInheritable)
                {
                    if ((e.ChangeMode & CAEXElementChangeMode.NameRefChanged) != 0)
                    {
                        if (treeNode.IsOverridden)
                        {
                            treeNode.Parent.RefreshTree(false);
                        }
                    }

                    UpdateInheritance(treeNode, e.ChangeMode);
                }

                UpdateMasterAnMirrorReferences(treeNode);
            }

            if ((e.ChangeMode & AcceptedChangeModes) != CAEXElementChangeMode.None)
            {
                if (e.CAEXElement.IsInternalLink())
                {
                    var refs = e.CAEXElement.IDReferenceAttributes();
                    if (refs.Length>0)
                    {
                        foreach (var reference in refs)
                        {
                            var value = e.CAEXElement.Attribute(reference)?.Value;
                            if (value == null)
                            {
                                continue;
                            }

                            var ilInterface = InterfaceFromAttributeValue(value, e.CAEXDocument);
                            if (ilInterface != null)
                            {
                                foreach (var node in GetCaexNodesFromTree(ilInterface.Node))
                                {
                                    node.RefreshNodeInformation(false);
                                    UpdateMasterAnMirrorReferences(node);
                                }
                            }
                        }
                    }
                }
                AmlTreeView?.InternalLinksAdorner?.InvalidateVisual();
            }
        });
    }

    /// <summary>
    ///     Called when the AML engine command manager notifies a change of any CAEX element.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    ///     The <see cref="CAEXElementChangeEventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnCAEXElementChanged(object sender, CAEXElementChangeEventArgs e)
    {
        if (Root == null || View == null)
        {
            return;
        }

        ExecuteUpdate(sender, e);
    }

    /// <summary>
    ///     Handling the selection change event of the tree view
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    ///     The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void SelectedElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                {
                    foreach (var node in SelectedElements)
                    {
                        node.IsSelected = false;
                    }

                    break;
                }
            case NotifyCollectionChangedAction.Add when e.NewItems != null:
                {
                    foreach (var node in e.NewItems.OfType<AMLNodeViewModel>())
                    {
                        node.IsSelected = true;
                    }

                    break;
                }
            case NotifyCollectionChangedAction.Replace when e.NewItems != null && e.OldItems != null:
                {
                    foreach (var node in e.NewItems.OfType<AMLNodeViewModel>())
                    {
                        node.IsSelected = true;
                    }

                    foreach (var node in e.OldItems.OfType<AMLNodeViewModel>())
                    {
                        node.IsSelected = false;
                    }

                    break;
                }
            case NotifyCollectionChangedAction.Remove when e.OldItems != null:
                {
                    foreach (var node in e.OldItems.OfType<AMLNodeViewModel>())
                    {
                        node.IsSelected = false;
                    }

                    break;
                }
        }

        if (!IsDragging)
        {
            //var selected = e.NewItems?.OfType<AMLNodeViewModel>().FirstOrDefault();
            //if (selected == null)
            //    return;

            if (RaiseNotifySelection)
            {
                NotifySelection(e.NewItems?.OfType<AMLNodeViewModel>().FirstOrDefault());
            }

            RaiseNotifySelection = true;
        }
    }

    /// <summary>
    ///     The <see cref="ShowHiddenLinksCommand" /> Execution Action.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    protected virtual void ShowHiddenLinksCommandExecute(object parameter)
    {
        if (parameter is AMLNodeWithClassReference node)
        {
            _ = SelectCaexNode(node.CAEXNode, true, true);
        }
    }

    /// <summary>
    ///     Unsubscribes to caex events
    /// </summary>
    protected void Unsubscribe()
    {
        CaexCommand.CAEXElementChangedEvent -= OnCAEXElementChanged;
        var updater = ServiceLocator.GetService<IAutoUpdate>();
        if (updater != null)
        {
            updater.ReferenceUpdated -= Updater_ReferenceUpdated;
        }

        CollectionChangedEventManager.RemoveHandler(SelectedElements, SelectedElementsChanged);
    }

    #endregion Protected Methods

    #region Private Methods

    /// <summary>
    /// </summary>
    /// <param name="externalInterface"></param>
    /// <returns></returns>
    private static IEnumerable<ExternalInterfaceType> GetInterfaces(CAEXWrapper externalInterface)
    {
        if (externalInterface is not ExternalInterfaceType ie)
        {
            yield break;
        }

        if (ie.AssociatedObject is not SystemUnitClassType)
        {
            yield break;
        }

        foreach (var il in ie.InternalLinksToInterface())
        {
            yield return il.AInterface != externalInterface ? il.AInterface : il.BInterface;
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    private static AMLNodeViewModel ParentWithType<T>(AMLNodeViewModel node) where T : ICAEXBasicObject
    {
        while (true)
        {
            if (node.Parent?.CAEXObject is T)
            {
                return node.Parent;
            }

            if (node.Parent == null)
            {
                return null;
            }

            node = node.Parent;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private bool FilterNodesWithName(AMLNodeViewModel node) =>
        node.CAEXNode == null || CAEXTagNames.Contains(node.CAEXNode.Name.LocalName);

    /// <summary>
    ///     Gets the Interface from the provided InternalLink Attribute value, which is from "RefPartnerSideA" or
    ///     from the "RefPartnerSideB" attribute.
    /// </summary>
    /// <param name="internalLinkAttributeValue">The value of the InternalLink attribute, defining the Interface reference.</param>
    /// <param name="caexDocument"></param>
    /// <returns>the external interface</returns>
    private static ExternalInterfaceType InterfaceFromAttributeValue(string internalLinkAttributeValue,
        CAEXDocument caexDocument)
    {
        //ToDo check for ALIAS
        if (string.IsNullOrEmpty(internalLinkAttributeValue))
        {
            return null;
        }

        var schema = caexDocument.Schema;
        if (schema != CAEXDocument.CAEXSchema.CAEX2_15 &&
            !CAEXPathBuilder.IsInterfaceReference(internalLinkAttributeValue))
        {
            return ServiceLocator.QueryService.FindByID(caexDocument,
                internalLinkAttributeValue) as ExternalInterfaceType;
        }

        var pathParts = CAEXPathBuilder.PathNameArray(internalLinkAttributeValue, CAEXDocument.CAEXSchema.CAEX2_15);
        if (pathParts.Length != 2)
        {
            return null;
        }

        var suc = ServiceLocator.QueryService.FindByID(caexDocument, pathParts[0]);
        if (suc == null)
        {
            return null;
        }

        var ei = suc.Node?.Elements(suc.Node.XName(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING))
            .FirstOrDefault(n => n.CAEXNameOfElement() == pathParts[1]);

        return ei != null ? new ExternalInterfaceType(ei) : null;
    }

    /// <summary>
    /// </summary>
    /// <param name="bringIntoView"></param>
    /// <param name="amlTreeViewItem"></param>
    private void MarkNode(bool bringIntoView, AMLNodeViewModel amlTreeViewItem)
    {
        _markedNode = amlTreeViewItem;

        if (View is AMLTreeView amlTree && amlTreeViewItem != null && bringIntoView)
        {
            amlTree.MarkNode(amlTreeViewItem);
        }
    }

    private static IEnumerable<XElement> GetModelElements(CAEXWrapper node, HashSet<string> names, bool expand)
    {
        if (ServiceLocator.GetService<IDatabaseService>() is IDatabaseService service)
        {
            foreach (var name in names)
            {
                var elements = service.Elements(node.Node, name, expand);
                if (!expand && elements.Any())
                {
                    return elements;
                }
            }

            if (!expand)
            {
                return [];
            }
        }

        if (!expand)
        {
            var element = node.Node.Elements().FirstOrDefault(n => names.Contains(n.Name.LocalName));
            return element == null ? ([]) : ([element]);
        }

        return node.Node.Elements().Where(n => names.Contains(n.Name.LocalName));
    }

    /// <summary>
    ///     gets the children of the node, which should appear in the list.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="names"></param>
    /// <param name="isDerived"></param>
    /// <param name="showMirrorData"></param>
    /// <param name="showInheritance"></param>
    /// <param name="expand"></param>
    /// <returns></returns>
    private static IEnumerable<XElement> ModelChilds(CAEXWrapper node, HashSet<string> names,
        bool isDerived, bool showMirrorData, bool showInheritance, bool expand)
    {
        if (isDerived)
        {
            // ToDo allow nested item overriding
            //if (names.Contains(CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING))
            //{
            //    names = new List<string> {CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING};
            //}
            //else
            //{
            return [];
            //}
        }

        var sort = false;

        if (node is IMirror { IsMirror: true } mirror && showMirrorData)
        {
            var master = mirror.Master;
            if (master != null)
            {
                return ModelChilds(master, names, false, true, showInheritance, expand);
            }
        }

        var items = GetModelElements(node, names, expand);

        if (node is SystemUnitFamilyType sucClass && showInheritance)
        {
            // include inherited elements if Show Inheritance is set
            var inheritedElements = sucClass.GetInheritedElements(true).Select(e => e.Node);
            IEnumerable<InternalElementType> deleted =
                sucClass.InternalElement.Where(a => a.ChangeMode == ChangeMode.Delete).ToList();

            if (deleted.Any())
            {
                inheritedElements = inheritedElements.Concat(deleted.Select(n => n.Node));
            }

            if (inheritedElements.Any())
            {
                items = items.Concat(inheritedElements);
                sort = true;
            }
        }

        // ToDo show inherited attributes
        if (node is AttributeFamilyType aft && showInheritance)
        {
            // include inherited elements if Show Inheritance is set
            var inheritedAttributes = aft.GetInheritedAttributes().Select(e => e.Node);
            var deleted = aft.Attribute.Where(a => a.ChangeMode == ChangeMode.Delete);
            IEnumerable<AttributeType> elements = deleted.ToList();
            if (elements.Any())
            {
                inheritedAttributes = inheritedAttributes.Concat(elements.Select(n => n.Node));
            }

            if (!inheritedAttributes.Any())
            {
                return items;
            }

            items = items.Concat(inheritedAttributes);
        }
        else
        {
            if (node is not IClassWithExternalInterface ieClass || !showInheritance)
            {
                return items;
            }

            {
                // include inherited elements if Show Inheritance is set
                var inheritedInterfaces = ieClass.GetInheritedInterfaces().Select(e => e.Node);
                var deleted = ieClass.ExternalInterface.Where(a => a.ChangeMode == ChangeMode.Delete);
                IEnumerable<ExternalInterfaceType> elements = deleted.ToList();
                if (elements.Any())
                {
                    inheritedInterfaces = inheritedInterfaces.Concat(elements.Select(n => n.Node));
                }

                if (!inheritedInterfaces.Any())
                {
                    return sort ? items.Distinct().OrderBy(i => i.Name.LocalName) : items;
                }

                items = items.Concat(inheritedInterfaces);
            }
        }

        return items.Distinct().OrderBy(i => i.Name.LocalName);
    }

    /// <summary>
    /// </summary>
    /// <param name="aMLNodeViewModel"></param>
    private void NotifySelection(AMLNodeViewModel aMLNodeViewModel)
    {
        //SelectionChanged?.Invoke(this, new AmlNodeEventArgs(aMLNodeViewModel));
        //RaisePropertyChanged(nameof( SelectedCAEXObject);
        //RaisePropertyChanged(nameof( SelectedElement);
        //RaisePropertyChanged(nameof( SelectedNode);

        //return;

        SelectionChanged?.Invoke(this, new AmlNodeEventArgs(aMLNodeViewModel));

        //lock (_selectionNotifications)
        //{
        //    if (_selectionNotifications.Count == 0)
        //    {
        //        if (SelectionChanged != null)
        //        {
        //            var operation = Execute.OnUIThread(() =>
        //                SelectionChanged?.Invoke(this, new AmlNodeEventArgs(aMLNodeViewModel)));
        //            if (operation != null)
        //            {
        //                _selectionNotifications.Push(aMLNodeViewModel);
        //                operation.Completed += SelectionOperationCompleted;
        //            }
        //        }

        //        RaisePropertyChanged(nameof(SelectedCAEXObject));
        //        RaisePropertyChanged(nameof(SelectedElement));
        //        RaisePropertyChanged(nameof(SelectedNode));
        //    }
        //    else if (_selectionNotifications.Count > 0)
        //    {
        //        _selectionNotifications.Push(aMLNodeViewModel);
        //    }
        //}
    }

    /// <summary>
    /// </summary>
    /// <param name="amlTreeViewItem"></param>
    private void Select(AMLNodeViewModel amlTreeViewItem)
    {
        amlTreeViewItem.CanNavigate = false;

        if (SelectedElements.Count == 0)
        {
            SelectedElements.Add(amlTreeViewItem);
        }
        else if (SelectedElements[0] != amlTreeViewItem)
        {
            SelectedElements[0].IsSelected = false;
            SelectedElements[0] = amlTreeViewItem;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectionOperationCompleted(object sender, EventArgs e)
    {
        lock (_selectionNotifications)
        {
            if (_selectionNotifications.Count == 0)
            {
                return;
            }

            var last = _selectionNotifications.Pop();

            // still some waiting
            if (_selectionNotifications.Count <= 0)
            {
                return;
            }
            //if (_selectionNotifications.Count > 1)
            //    Debug.WriteLine("Selection handling: skipped " + (_selectionNotifications.Count - 1));

            _selectionNotifications.Clear();

            var operation =
                Execute.OnUIThread(() => SelectionChanged?.Invoke(this, new AmlNodeEventArgs(last)));
            if (operation != null)
            {
                _selectionNotifications.Push(last);
                operation.Completed += SelectionOperationCompleted;
            }

            RaisePropertyChanged(nameof(SelectedCAEXObject));
            RaisePropertyChanged(nameof(SelectedElement));
            RaisePropertyChanged(nameof(SelectedNode));
        }
    }

    /// <summary>
    ///     Test, if the <see cref="ShowHiddenLinksCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    private bool ShowHiddenLinksCommandCanExecute(object parameter) => true;

    /// <summary>
    /// </summary>
    /// <param name="treeNode"></param>
    /// <param name="changeMode"></param>
    private void UpdateInheritance(AMLNodeViewModel treeNode, CAEXElementChangeMode changeMode)
    {
        var classNode = treeNode.CAEXObject is IClassWithBaseClassReference
            ? treeNode
            : ParentWithType<IClassWithBaseClassReference>(treeNode);

        if (classNode?.CAEXObject is not CAEXObject caex)
        {
            return;
        }

        var references = ServiceLocator.QueryService
            .ElementsWithCAEXPathReference(caex.CAEXDocument.CAEXFile, caex.CAEXPath())
            .Where(c => c.CaexObject is IClassWithBaseClassReference).Select(c => c.CaexObject);

        foreach (var reference in references)
        {
            var node = GetCaexNodeFromTree(reference.Node);
            if (node == null)
            {
                continue;
            }

            if ((changeMode & CAEXElementChangeMode.NameChanged) != 0)
            {
                node.RefreshTreeNodes();
            }
            else
            {
                node.RefreshTree(false, true);
            }

            UpdateInheritance(node, changeMode);
        }
    }

    private void UpdateMasterAnMirrorReferences(AMLNodeViewModel treeNode)
    {
        if (treeNode.IsMaster)
        {
            UpdateMirrorItems(treeNode);
        }
        else if (treeNode.IsMirrored)
        {
            var mirror = treeNode.MirrorElement;
            if (mirror != null)
            {
                UpdateMasterItems(mirror);
            }
        }
        else
        {
            var master = treeNode.MasterElement;
            if (master != null)
            {
                UpdateMirrorItems(master);
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="treeNode"></param>
    private void UpdateMasterItems(AMLNodeViewModel treeNode)
    {
        if (treeNode.CAEXObject is IMirror mirror)
        {
            var master = mirror.Master?.Node;
            if (master == null)
            {
                return;
            }

            var node = GetCaexNodeFromTree(master);
            if (node != null)
            {
                node.RefreshNodeInformation(false);
                if (node.ShowMirrorData)
                {
                    node.RefreshTree(false, true);
                }

                UpdateMirrorItems(node);
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="treeNode"></param>
    private void UpdateMirrorItems(AMLNodeViewModel treeNode)
    {
        void Refresh(XElement mirror)
        {
            var node = GetCaexNodeFromTree(mirror);
            if (node != null)
            {
                node.RefreshNodeInformation(false);
                if (node.ShowMirrorData)
                {
                    node.RefreshTree(true, true);
                }
            }
        }

        switch (treeNode.CAEXObject)
        {
            case InternalElementType ie:
                foreach (var mirror in ServiceLocator.QueryService.InternalElementMirrors(ie))
                {
                    Refresh(mirror.Node);
                }

                break;

            case ExternalInterfaceType ei:
                foreach (var mirror in ServiceLocator.QueryService.ExternalInterfaceMirrors(ei))
                {
                    Refresh(mirror.Node);
                }

                break;

            case AttributeType at:
                foreach (var mirror in ServiceLocator.QueryService.AttributeMirrors(at))
                {
                    Refresh(mirror.Node);
                }

                break;
        }
    }

    //private void UpdateChanges(AMLNodeViewModel node, CAEXElementChangeEventArgs e)
    //{
    //    //if (e.ChangeMode.HasFlag (CAEXElementChangeMode.Deleted))
    //    //{
    //    //    if (e.CAEXAttributeName == CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH)
    //    //    {
    //    //        var id = e.OldValue?.NormalizedGUID();
    //    //        if (string.IsNullOrEmpty(id))
    //    //            return;

    // // var master = new
    // CAEXObject(Tree.Root.CAEXNode).FindCaexObjectFromId<InternalElementType>(id); // if
    // (master == null) // return;

    // // var treeNode = Tree.SelectCaexNode(master.Node, false);

    //    //        if (treeNode != null)
    //    //            treeNode.RefreshNodeInformation(false);
    //    //    }
    //    //}
    //}

    /// <summary>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="updateArgs"></param>
    private void Updater_ReferenceUpdated(object sender, UpdateEventArgs updateArgs)
    {
        if (Root == null)
        {
            return;
        }

        Execute.OnUIThread(() =>
        {
            if (Root == null)
            {
                return;
            }
            AMLNodeViewModel treeNode = null;

            if (updateArgs.ReferencedElement?.Document != null)
            {
                if (updateArgs.ReferencedElement.IsInternalLink())
                {
                    var ei = new InternalLinkType(updateArgs.ReferencedElement).InterfaceFromAttributeValue(
                        updateArgs.Reference?.Value());
                    if (ei != null)
                    {
                        treeNode = FindTreeViewItemInTree(Root.Children, ei.Node);
                    }
                }
                else
                {
                    treeNode = FindTreeViewItemInTree(Root.Children, updateArgs.ReferencedElement);
                }
            }

            if (treeNode != null)
            {
                if (treeNode.CAEXNode.Document == null)
                {
                    return;
                }

                if (treeNode is AMLNodeWithClassReference aRef && aRef.CAEXNode.IsExternalInterface())
                {
                    if (Tree?.AmlTreeView?.InternalLinksAdorner != null)
                    {
                        if (aRef.ShowLinks)
                        {
                            var oldPartners = Tree.AmlTreeView.InternalLinksAdorner.GetPartnerNodes(aRef).ToList();
                            treeNode.RefreshNodeInformation(false);
                            var newPartners = Tree.AmlTreeView.InternalLinksAdorner.GetPartnerNodes(aRef).ToList();

                            _ = oldPartners.RemoveAll(p => newPartners.Contains(p));
                            if (oldPartners.Count > 0)
                            {
                                registeredPartners.AddRange(oldPartners);
                            }
                        }
                        else if (registeredPartners?.Count > 0 && aRef.HasLinks)
                        {
                            var connected = GetInterfaces((CAEXObject)aRef.CAEXObject).ToList();
                            var redirected = registeredPartners
                                .Where(p => connected.Any(c => c != null && p.CAEXNode == c.Node))
                                .ToList();

                            if (redirected.Count > 0)
                            {
                                aRef.ShowLinks = true;
                                _ = registeredPartners.RemoveAll(p => redirected.Contains(p));
                            }
                        }
                        Tree.AmlTreeView.InternalLinksAdorner.InvalidateVisual();
                    }

                    aRef.RefreshPartners();
                    if (aRef.HasLinks && TreeViewLayout.AlwaysExpandLink)
                    {
                        aRef.UpdateLinks(true, true, true);
                    }
                    // aRef.ShowLinks = true;
                }
                // check for overwriding children if inheritance has changed
                else if (updateArgs.Reference is XAttribute att && att.IsInheritanceAttribute())
                {
                    treeNode.RefreshChildNodeInformation(false);
                }
                treeNode.RefreshNodeInformation(false);
            }
            if (updateArgs.CaexReference?.CaexObject == null ||
                (treeNode = FindTreeViewItemInTree(Root.Children, updateArgs.CaexReference?.CaexObject?.Node)) == null)
            {
                return;
            }
            if (treeNode.CAEXNode.Document == null)
            {
                return;
            }
            treeNode.RefreshNodeInformation(false);
        });
    }
    #endregion Private Methods
}