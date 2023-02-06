// *********************************************************************** Assembly :
// Aml.Toolki Author : Josef Prinz Created : 03-10-2015
//
// Last Modified By : Josef Prinz Last Modified On : 03-10-2015 ***********************************************************************
// <copyright file="AMLNodeWithClassAndRoleReference.cs" company="inpro">
//    Copyright (c) AutomationML e.V. All rights reserved.
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************

using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
using Aml.Engine.AmlObjects;
using Aml.Engine.CAEX;
using Aml.Engine.Xml.Extensions;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLNodeWithClassAndRoleReference is the ViewModel for all CAEX-Elements,
///     which may have references to CAEX-Classes and Roles. The ViewModel provides an
///     additional property
///     <see
///         cref="RoleReference" />
///     for these Elements. The
///     RoleReference is build from the first RoleRequirement found in the Children
///     Collection of the Element.
/// </summary>
public class AMLNodeWithClassAndRoleReference : AMLNodeInheritable
{
    #region Public Methods

    /// <summary>
    ///     Refreshes the node information. This Method can be overridden in derived
    ///     classes. The Method should be called, if the CAEX-Elements Data has changed
    ///     and the Changes should be visible in any View, that has a binding to this ViewModel.
    /// </summary>
    public override void RefreshNodeInformation(bool expand)
    {
        base.RefreshNodeInformation(expand);

        RaisePropertyChanged(nameof(RoleReference));
        RaisePropertyChanged(nameof(IsGroup));
        RaisePropertyChanged(nameof(IsFacetted));
        RaisePropertyChanged(nameof(IsPort));
        RaisePropertyChanged(nameof(HasReference));
    }

    #endregion Public Methods

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see
    ///         cref="AMLNodeWithClassAndRoleReference" />
    ///     class.
    /// </summary>
    /// <param name="tree">            The TreeViewModel, containing the node</param>
    /// <param name="parent">          The parent.</param>
    /// <param name="caexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeWithClassAndRoleReference(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement caexNode,
        bool lazyLoadChildren)
        : base(tree, parent, caexNode, lazyLoadChildren)
    {
        PropertyChangedEventManager.AddHandler(parent.Tree.TreeViewLayout, LayoutPropertyChanged, string.Empty);
        _childrenCollection.Filter += FilterVisibleItems;
    }

    private void FilterVisibleItems(object sender, FilterEventArgs e)
    {
        e.Accepted = NodeVisibleFilter(e.Item);
    }

    private void LayoutPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ChildrenView.Refresh();
    }


    //public override bool IsVisible
    //{
    //    get => base.IsVisible;
    //    set
    //    {
    //        base.IsVisible = value;
    //        ChildrenView.Refresh();
    //    }
    //}

    private bool CanExpand()
    {
        bool IsVisible(XElement n)
        {
            return n.Name.LocalName switch
            {
                CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING or
                    CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING => Tree.TreeViewLayout.ShowRoleReqNodes,
                _ => true
            };
        }

        return HasDummyChild || Children.Any(n => IsVisible(n.CAEXNode)) ||
               Tree.ModelChilds(this, false).Any(IsVisible);
    }

    private bool NodeVisibleFilter(object obj)
    {
        if (obj is not AMLNodeBaseViewModel node)
        {
            return false;
        }

        return node switch
        {
            AMLExpandableDummyNode => CanExpand(),
            AMLNodeGroupViewModel { IsRoleGroup: true } => Tree.TreeViewLayout.ShowRoleGrouping &&
                                                           Tree.TreeViewLayout.ShowRoleReqNodes,
            AMLNodeGroupViewModel => Tree.TreeViewLayout.ShowInterfaceGrouping,
            AMLNodeWithoutName { IsRoleReference: true } => Tree.TreeViewLayout.ShowRoleReqNodes,
            _ => true
        };
    }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see
    ///         cref="AMLNodeWithClassAndRoleReference" />
    ///     class.
    /// </summary>
    /// <param name="parent">          The parent.</param>
    /// <param name="caexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeWithClassAndRoleReference(AMLNodeViewModel parent, XElement caexNode, bool lazyLoadChildren) :
        this(null, parent, caexNode, lazyLoadChildren)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets a value indicating whether this instance has a class- or role reference.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has a reference; otherwise, <c>false</c>.
    /// </value>
    public bool HasReference => !string.IsNullOrEmpty(RoleReference) || !string.IsNullOrEmpty(ClassReference);

    /// <summary>
    ///     Returns <c>true</c>, if this is a facet.
    /// </summary>
    public override bool IsFacetted => CAEXNode.IsInternalElement() && new AMLFacet(CAEXNode).IsFacet;

    /// <summary>
    ///     Returns <c>true</c>, if this is a group.
    /// </summary>
    public override bool IsGroup => CAEXNode.IsInternalElement() && new AMLGroup(CAEXNode).IsGroup;


    /// <summary>
    ///     Returns <c>true</c>, if this is a port.
    /// </summary>
    public override bool IsPort => CAEXNode.IsInternalElement() && new AMLPort(CAEXNode).IsPort;

    /// <summary>
    ///     Gets and sets the RoleReference
    /// </summary>
    /// <value>The role reference.</value>
    public string RoleReference
    {
        get
        {
            if (CAEXObject is not SystemUnitClassType suc)
            {
                return "";
            }

            var refs = suc.RoleReferences.Select(r =>
                    r.RoleReference != null
                        ? r.RoleReference.Substring(r.RoleReference.LastIndexOf('/') + 1)
                        : "")
                .Distinct();

            return string.Join(", ", refs);
        }
    }

    #endregion Public Properties
}