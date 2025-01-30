// Copyright (c) 2017 AutomationML e.V.
using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLNodeGroupViewModel is used, to group a set of CAEX objects using an extra Node in the Treeview. The
///     Visibility of the AMLNodeGroupViewModel
///     can be toggled. If the group node becomes invisible, the grouped items are moved to the parent node. If the Node
///     becomes visible, the associated
///     items are moved from the parent to this instance.
/// </summary>
public class AMLNodeGroupViewModel : AMLNodeViewModel
{
    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeGroupViewModel" /> class.
    /// </summary>
    /// <param name="parent">          The parent.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    /// <param name="groupElementNames">defining the group name.</param>
    public AMLNodeGroupViewModel(AMLNodeViewModel parent, bool lazyLoadChildren,
        IEnumerable<string> groupElementNames)
        : base(parent, parent.CAEXNode, lazyLoadChildren)
    {
        ElementNames = groupElementNames.ToList();
        if (parent.Tree == null)
        {
            return;
        }

        PropertyChangedEventManager.AddHandler(parent.Tree.TreeViewLayout, LayoutPropertyChanged, string.Empty);

        //var _layoutUpdate = new DelegatingWeakEventListener((EventHandler<PropertyChangedEventArgs>)LayoutPropertyChanged);
        //GenericWeakEventManager.AddListener(
        //    parent.Tree.TreeViewLayout,
        //      "PropertyChanged",
        //    _layoutUpdate);
    }

    #endregion Public Constructors

    #region Internal Properties

    /// <summary>
    ///     Gets a value indicating whether a group with this name should be visible according to the layout properties.
    /// </summary>
    internal bool IsVisibleInLayout => GroupName == AMLLayout.SET_ROLE_GROUPING
        ? Tree.TreeViewLayout.ShowRoleGrouping && Tree.TreeViewLayout.ShowRoleReqNodes
        : Tree.TreeViewLayout.ShowInterfaceGrouping;

    /// <summary>
    ///     Gets a value indicating if the group members a role references
    /// </summary>
    internal bool IsRoleGroup => GroupName == AMLLayout.SET_ROLE_GROUPING;

    #endregion Internal Properties

    #region Private Fields

    private static readonly IEnumerable<string> InterfaceGroup = [CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING];

    private static readonly IEnumerable<string> RoleRefGroup = [CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING, CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING];

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    ///     Gets the allowed element names for the group.
    /// </summary>
    /// <value>
    ///     The element names.
    /// </value>
    public List<string> ElementNames { get; }

    /// <summary>
    ///     Gets the group items, associated to this group.
    /// </summary>
    public IEnumerable<XElement> GroupItems =>
        Parent.IsMirror && Parent.ShowMirrorData && Parent.Master != null
            ? Parent.Master.Node.Elements().Where(e => ElementNames.Contains(e.Name.LocalName))
            : CAEXNode?.Elements().Where(e => ElementNames.Contains(e.Name.LocalName)) ?? [];

    /// <summary>
    ///     Gets the name of the group.
    /// </summary>
    public string GroupName => ElementNames.Contains(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING)
        ? AMLLayout.SET_INTERFACE_GROUPING
        : AMLLayout.SET_ROLE_GROUPING;

    /// <summary>
    ///     Gets and sets the IsVisible. If the group node becomes invisible, the grouped items are moved to the parent node.
    ///     If the Node becomes visible, the associated
    ///     items are moved from the parent to this instance.
    /// </summary>
    public override bool IsVisible
    {
        get => base.IsVisible;
        set
        {
            if (base.IsVisible == value)
            {
                return;
            }

            // if no group items exists, the group visibility cannot be changed to visible.
            if (value && !GroupItems.Any())
            {
                return;
            }

            if (value && !IsVisibleInLayout)
            {
                return;
            }

            base.IsVisible = value;
            if (base.IsVisible)
            {
                MoveToGroup();
            }
            else
            {
                MoveToParent();
            }
        }
    }

    /// <summary>
    ///     Gets and sets the MappedValue
    /// </summary>
    public override double MappedValue
    {
        get => 0;
        set => base.MappedValue = value;
    }

    /// <summary>
    ///     Gets a value indicating whether this instance has new version.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has new version; otherwise, <c>false</c>.
    /// </value>
    public override bool HasNewVersion => false;

    /// <summary>
    ///     Gets a value indicating whether this instance has old version.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has old version; otherwise, <c>false</c>.
    /// </value>
    public override bool HasOldVersion => false;

    /// <summary>
    ///     Gets a value indicating whether this instance is derived.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is derived; otherwise, <c>false</c>.
    /// </value>
    public override bool IsDerived => false;

    /// <summary>
    ///     Gets a value indicating whether this instance is deleted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
    /// </value>
    public override bool IsDeleted => false;

    /// <summary>
    ///     Gets a value indicating whether this instance is facetted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is facetted; otherwise, <c>false</c>.
    /// </value>
    public override bool IsFacetted => false;

    /// <summary>
    ///     Gets a value indicating whether this instance is port.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is port; otherwise, <c>false</c>.
    /// </value>
    public override bool IsPort => false;

    /// <summary>
    ///     Gets a value indicating whether this instance represents a group.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance represents a group; otherwise, <c>false</c>.
    /// </value>
    public override bool IsGroup => false;


    /// <summary>
    ///     Gets a value indicating whether this instance is mirror.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is mirror; otherwise, <c>false</c>.
    /// </value>
    public override bool IsMirror => false;

    /// <summary>
    ///     Gets a value indicating whether this instance is overridden.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is overridden; otherwise, <c>false</c>.
    /// </value>
    public override bool IsOverridden => false;

    /// <summary>
    ///     Gets a value indicating whether this instance is a role reference.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is role reference; otherwise, <c>false</c>.
    /// </value>
    public override bool IsRoleReference => false;

    /// <summary>
    ///     Gets and sets the Name
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public override string Name
    {
        get => CAEXObject is IObjectWithRoleReference irole
            ?  ShortNameFromReference(irole.RoleReference)
            : base.Name;
        set => base.Name = value;
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    ///     Invoked when the child items need to be loaded on demand. If the group node is invisible,
    ///     the loaded group items are moved to the parent node after loading.
    /// </summary>
    public override void LoadChildren(bool raise = true)
    {
        LoadChildren(GroupItems, raise);

        // synchronize the visiblity states
        if (!IsVisibleInLayout)
        {
            if (IsVisible)
            {
                IsVisible = false;
            }
            else
            {
                MoveToParent();
            }
        }
        else if (IsVisibleInLayout && !IsVisible)
        {
            IsVisible = true;
        }
    }

    /// <inheritdoc />
    public override void RefreshNodeInformation(bool expand)
    {
        Parent.RefreshNodeInformation(expand);
    }

    #endregion Public Methods

    #region Internal Methods

    /// <summary>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static AMLNodeGroupViewModel Create(AMLNodeViewModel parent, string name)
    {
        return name switch
        {
            AMLLayout.SET_INTERFACE_GROUPING => new AMLNodeGroupViewModel(parent, true,
                parent.CAEXTagNames.Intersect(InterfaceGroup)),
            AMLLayout.SET_ROLE_GROUPING => new AMLNodeGroupViewModel(parent, true,
                parent.CAEXTagNames.Intersect(RoleRefGroup)),
            _ => null
        };
    }

    /// <summary>
    /// </summary>
    /// <param name="elementName"></param>
    /// <returns></returns>
    internal static string GroupingPropertyname(string elementName)
    {
        return elementName switch
        {
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING => AMLLayout.SET_INTERFACE_GROUPING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING or CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING =>
                AMLLayout.SET_ROLE_GROUPING,
            _ => string.Empty
        };
    }

    #endregion Internal Methods

    #region Private Methods

    /// <summary>
    ///     Change the visibility of this group node if a corresponding layout property changes
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
    private void LayoutPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(AMLLayout.ShowInterfaceGrouping):
            case nameof(AMLLayout.ShowRoleGrouping):
            case nameof(AMLLayout.ShowRoleReqNodes):
                IsVisible = IsVisibleInLayout;
                break;
        }
    }

    /// <summary>
    ///     Moves the associated group items from the parent node to the group node.
    /// </summary>
    private void MoveToGroup()
    {
        var childs = new List<AMLNodeViewModel>();

        if (Parent != null)
        {
            childs.AddRange(Parent.Children.Where(child => child.CAEXNode != null
                                                           && ElementNames.Contains(child.CAEXNode.Name.LocalName)));

            for (var i = 0; i < childs.Count; i++)
            {
                _ = Parent.Children.Remove(childs[i]);
                AddNode(childs[i], i == childs.Count - 1);
                childs[i].Parent = this;
            }
        }

        if (childs.Count > 0)
        {
            IsExpanded = true;
        }
    }

    /// <summary>
    ///     Moves the associated group items from this node to the parent node.
    /// </summary>
    private void MoveToParent()
    {
        if (Parent == null)
        {
            return;
        }

        var index = Parent.Children.IndexOf(this) + 1;
        foreach (var child in Children)
        {
            Parent.InsertNode(index++, child);
            child.Parent = Parent;
        }

        Children.Clear();
        IsExpanded = false;
    }

    /// <summary>
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    private string ShortNameFromReference(string reference) =>
       CAEXPathBuilder.NormalizePath(Path.GetFileNameWithoutExtension(reference), out _, CAEXObject.CAEXSchema());

    #endregion Private Methods
}