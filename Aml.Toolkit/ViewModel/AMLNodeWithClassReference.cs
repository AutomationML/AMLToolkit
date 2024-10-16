// Copyright (c) 2017 AutomationML e.V.
using Aml.Engine.AmlObjects;
using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Extensions;
using Aml.Engine.Services;
using Aml.Engine.Xml.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLNodeWithClassReference is the ViewModel for all CAEX-Elements, which
///     may have references to CAEX-Classes. The ViewModel provides an additional
///     property <see cref="ClassReference" /> for these Elements.
/// </summary>
public class AMLNodeWithClassReference : AMLNodeViewModel
{
    #region Private Fields

    /// <summary>
    ///     <see cref="ShowLinks" />
    /// </summary>
    private bool _showLinks;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeWithClassReference" /> class.
    /// </summary>
    /// <param name="tree">            The TreeViewModel, containing the node</param>
    /// <param name="parent">          The parent.</param>
    /// <param name="CaexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeWithClassReference(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement CaexNode,
        bool lazyLoadChildren) :
        base(tree, parent, CaexNode, lazyLoadChildren)
    {
        SetClassPathReferenceAttribute(this);
        RefreshNodeInformation(false);

        if (tree != null)
        {
            tree.TreeViewLayoutUpdated += TreeViewLayoutUpdated;
        }
    }

    /// <summary>
    /// Event handler called when treeview layout is updated
    /// </summary>
    protected virtual void TreeViewLayoutUpdated(object sender, TreeViewLayoutUpdateEventArgs e)
    {
        RaisePropertyChanged(nameof(HasClassOrVersionReference));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeWithClassReference" /> class.
    /// </summary>
    /// <param name="parent">          The parent.</param>
    /// <param name="CaexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeWithClassReference(AMLNodeViewModel parent, XElement CaexNode, bool lazyLoadChildren) :
        this(null, parent, CaexNode, lazyLoadChildren)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets or sets the name of the class path reference attribute.
    /// </summary>
    /// <value>The class path reference attribute.</value>
    public string ClassPathReferenceAttribute { get; set; }



    /// <summary>
    ///     Gets and sets the ClassReference
    /// </summary>
    /// <value>The class reference.</value>
    public string ClassReference
    {
        get
        {
            if (string.IsNullOrEmpty(ClassPathReferenceAttribute))
            {
                return null;
            }

            if (CAEXObject is IMirror mirror && mirror.Master != null)
            {
                return mirror.Master.Name;
            }

            var reference = CAEXNode.Attribute(ClassPathReferenceAttribute)?.Value;
            return !string.IsNullOrEmpty(reference) ? reference.Split('/').Last() : null;
        }
    }

    /// <summary>
    ///     Gets the class reference label.
    /// </summary>
    /// <value>
    ///     The class reference label.
    /// </value>
    public string ClassReferenceLabel => IsMirror ? "Master:" : CAEXObject is AttributeTypeType ? "Type:" : "Class:";

    /// <summary>
    ///     Gets a value indicating whether this instance has external data.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has external data; otherwise, <c>false</c>.
    /// </value>
    public bool HasExternalData => CAEXObject is ExternalInterfaceType ie
                                   && !string.IsNullOrEmpty(((ObjectWithAMLAttributes)ie).RefURIAttribute?.FilePath);


    private bool HasVisibleInterfaceClass(InternalLinkType il)
    {
        var selectedClasses = Tree.FilteredClasses;
        var interfaceClassInstance = il.AInterface.Equals(CAEXObject) ? il.AInterface : il.BInterface;
        if (interfaceClassInstance?.InterfaceClass == null)
        {
            return true;
        }

        var classFilter = selectedClasses.FirstOrDefault(s => s.Name == interfaceClassInstance.InterfaceClass.Name);
        return classFilter == default || classFilter.IsSelected;
    }

    /// <summary>
    ///     Gets and sets the HasLinks
    /// </summary>
    public bool HasLinks
    {
        get
        {
            if (IsDerived)
            {
                return false;
            }

            if (CAEXObject is ExternalInterfaceType { AssociatedObject: SystemUnitClassType } ie)
            {
                // ToDo check if any class is visible
                // current impl. doesnot work because the Filter updates are not in sync with the node update

                //var filter = Tree.FilteredClasses;
                //if (filter != null)
                //{
                //    return ie.InternalLinksToInterface().Any(il => HasVisibleInterfaceClass(il));
                //}

                return ie.InternalLinksToInterface().Any();
            }

            return false;
        }
    }

    /// <summary>
    ///     Gets the minimum cardinality.
    /// </summary>
    /// <value>
    ///     The minimum cardinality.
    /// </value>
    public string MinCardinality
    {
        get
        {
            return CAEXObject is ExternalInterfaceType ie ? ie.MinCardinality()?.ToString() ?? "0" : "";
        }
    }

    /// <summary>
    ///     Gets the maximum cardinality warning when violated.
    /// </summary>
    /// <value>
    ///     The maximum cardinality warn.
    /// </value>
    public string MaxCardinalityWarn
    {
        get
        {
            return CAEXObject is ExternalInterfaceType ie ? ie.MaxCardinalityViolation() ? "!" : "" : "";
        }
    }

    /// <summary>
    ///     Gets a warning when the minimum cardinality is violated.
    /// </summary>
    /// <value>
    ///     The minimum cardinality warn.
    /// </value>
    public string MinCardinalityWarn
    {
        get
        {
            return CAEXObject is ExternalInterfaceType ie ? ie.MinCardinalityViolation() ? "!" : "" : "";
        }
    }

    /// <summary>
    ///     Gets the maximum cardinality.
    /// </summary>
    /// <value>
    ///     The maximum cardinality.
    /// </value>
    public string MaxCardinality
    {
        get
        {
            return CAEXObject is ExternalInterfaceType ie ? ie.MaxCardinality()?.ToString() ?? "n" : "";
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this instance has an assigned cardinality attribute.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance has cardinality; otherwise, <c>false</c>.
    /// </value>
    public bool HasCardinality
    {
        get
        {
            return CAEXObject is ExternalInterfaceType ie && ie.HasVerifiableCardinality();
        }
    }

    private void SetIsMaster()
    {
        IsMaster = CAEXNode.IsInternalElement()
            ? ((InternalElementType)CAEXObject).IsMaster()
            : CAEXNode.IsExternalInterface() && new ExternalInterfaceType(CAEXNode).IsMaster();
    }

    /// <summary>
    ///     Gets a value indicating whether this instance is mirror.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is mirror; otherwise, <c>false</c>.
    /// </value>
    public override bool IsMirror =>
        CAEXNode.IsInternalElement()
            ? new InternalElementType(CAEXNode).IsMirror
            : CAEXNode.IsExternalInterface() && new ExternalInterfaceType(CAEXNode).IsMirror;


    /// <summary>
    ///     Gets and sets the ShowLinks
    /// </summary>
    public bool ShowLinks
    {
        get => _showLinks;
        set
        {
            if (_showLinks == value)
            {
                return;
            }

            if (value && !HasLinks)
            {
                return;
            }

            UpdateLinks(value, true);
        }
    }

    /// <summary>
    ///     Gets and sets the ShowReference
    /// </summary>
    public bool ShowReference => true;

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    ///     Closes the link.
    /// </summary>
    /// <param name="partner">The partner.</param>
    public void CloseLink(AMLNodeViewModel partner)
    {
        if (_showLinks)
        {
            //_ = new Action(() =>
            //      {
            if (CAEXObject is ExternalInterfaceType { AssociatedObject: SystemUnitClassType } ie)
            {
                if (ie.InternalLinksToInterface().Any(il => il.AInterface?.Node != partner.CAEXNode &&
                                                            il.BInterface?.Node != partner.CAEXNode))
                {
                    return;
                }
            }

            _showLinks = false;
            RaisePropertyChanged(nameof(ShowLinks));
            //      }
            //).OnUIThread();
        }
    }

    /// <summary>
    /// Determines, if the version string is visible
    /// </summary>
    protected bool ShowVersion => !string.IsNullOrEmpty(Version) && Tree?.TreeViewLayout.ShowClassVersion == true;

    /// <summary>
    /// Determines, if the class reference string is visible
    /// </summary>
    protected bool ShowClassRef => !string.IsNullOrEmpty(ClassReference) && Tree?.TreeViewLayout.ShowClassReference == true;

    /// <summary>
    /// Determines, if a version or class reference string is visible
    /// </summary>
    public bool HasClassOrVersionReference => ShowVersion || ShowClassRef;

    /// <summary>
    ///     Refreshes the node information. This Method can be overridden in derived
    ///     classes. The Method should be called, if the CAEX-Elements Data has changed
    ///     and the Changes should be visible in any View, that has a binding to this ViewModel.
    /// </summary>
    public override void RefreshNodeInformation(bool expand)
    {
        base.RefreshNodeInformation(expand);

        RaisePropertyChanged(nameof(ShowReference));

        SetIsMaster();
        RaisePropertyChanged(nameof(IsMaster));
        RaisePropertyChanged(nameof(IsMirror));
        RaisePropertyChanged(nameof(ShowLinks));
        RaisePropertyChanged(nameof(HasLinks));
        RaisePropertyChanged(nameof(HasCardinality));
        RaisePropertyChanged(nameof(ClassReference));
        RaisePropertyChanged(nameof(ClassReferenceLabel));
        RaisePropertyChanged(nameof(HasExternalData));
        RaisePropertyChanged(nameof(MaxCardinality));
        RaisePropertyChanged(nameof(MinCardinality));

        RaisePropertyChanged(nameof(MaxCardinalityWarn));
        RaisePropertyChanged(nameof(MinCardinalityWarn));


        RaisePropertyChanged(nameof(HasClassOrVersionReference));
        //Tree.TreeViewLayout.RaisePropertyChanged("HideExpander");

        if (!HasLinks)
        {
            ShowLinks = false;
        }
        else if (ShowLinks)
        {
            UpdateLinks(true, true);
        }
    }

    /// <summary>
    ///     Updates the links.
    /// </summary>
    /// <param name="show">if set to <c>true</c> [show].</param>
    /// <param name="invalidate">if set to <c>true</c> [invalidate].</param>
    /// <param name="force">if set to <c>true</c> [force].</param>
    /// <param name="withMirrors"></param>
    public void UpdateLinks(bool show, bool invalidate = false, bool force = false, bool withMirrors = true)
    {
        if (IsDerived)
        {
            return;
        }
        //_ = new Action(() =>
        //      {
        RaisePropertyChanged(nameof(MinCardinalityWarn));
        RaisePropertyChanged(nameof(MaxCardinalityWarn));

        if (Tree.AmlTreeView != null && (_showLinks != show || force))
        {
            _showLinks = show;
            RaisePropertyChanged(nameof(ShowLinks));

            if (_showLinks)
            {
                Tree.AmlTreeView.AddLinksAdorner();
                if (CAEXObject is ExternalInterfaceType { AssociatedObject: SystemUnitClassType } ie)
                {
                    foreach (var il in ie.InternalLinksToInterface())
                    {
                        var (from, to) = GetConnectedNodes(il);
                        if (from == null || to == null)
                        {
                            continue;
                        }

                        Tree.AmlTreeView.InternalLinksAdorner.Connect(from, to);
                        if (Equals(from) && ((ExternalInterfaceType)to.CAEXObject)
                            .InternalLinksToInterface().Take(2).Count() == 1)
                        {
                            to.ShowLinks = true;
                        }
                        else if (((ExternalInterfaceType)from.CAEXObject).InternalLinksToInterface()
                                 .Take(2).Count() == 1)
                        {
                            from.ShowLinks = true;
                        }
                    }
                }
            }
            else if (Tree.AmlTreeView.InternalLinksAdorner != null)
            {
                if (CAEXObject is ExternalInterfaceType { AssociatedObject: SystemUnitClassType } ie)
                {
                    foreach (var il in ie.InternalLinksToInterface())
                    {
                        foreach (var partner in GetPartnerNodes(il))
                        {
                            partner.CloseLink(this);
                        }
                    }

                    Tree.AmlTreeView.InternalLinksAdorner.DisConnectAll(this);
                }
            }
        }

        if (invalidate && Tree.AmlTreeView?.InternalLinksAdorner != null)
        {
            //_ = Execute.OnUIThread(
            //    () =>
            //    {
            Tree.AmlTreeView.InternalLinksAdorner.Redraw();
            //});
        }

        RaisePropertyChanged(nameof(ShowLinks));
        //}
        //).OnUIThread();
    }

    #endregion Public Methods

    #region Internal Methods

    internal InternalLinkType GetInternalLink(AMLNodeWithClassReference partner)
    {
        return CAEXObject is not ExternalInterfaceType ie
            ? null
            : ie.AssociatedObject is SystemUnitClassType
            ? ie.InternalLinksToInterface().FirstOrDefault(il => (il.AInterface?.Node == CAEXNode &&
                                                                       il.BInterface?.Node == partner.CAEXNode) ||
                                                                      (il.AInterface?.Node == partner.CAEXNode &&
                                                                       il.BInterface?.Node == CAEXNode))
            : null;
    }

    internal IEnumerable<ExternalInterfaceType> GetPartners()
    {
        if (CAEXObject is not ExternalInterfaceType ie)
        {
            yield break;
        }

        if (ie.AssociatedObject is not SystemUnitClassType)
        {
            yield break;
        }

        foreach (var il in ie.InternalLinksToInterface())
        {
            if (il.AInterface?.Node == CAEXNode && il.BInterface != null)
            {
                yield return il.BInterface;
            }
            else if (il.BInterface?.Node == CAEXNode && il.AInterface != null)
            {
                yield return il.AInterface;
            }
        }
    }


    internal void RefreshPartners()
    {
        foreach (var partner in GetPartners())
        {
            var item = AMLTreeViewModel.FindTreeViewItemInTree(Tree.Root.Children, partner.Node);
            item?.RefreshNodeInformation(false);
        }
    }

    #endregion Internal Methods

    #region Private Methods

    /// <summary>
    ///     Sets the name of the class path reference attribute, used to get the value
    ///     for the <see cref="ClassReference" />.
    /// </summary>
    /// <param name="node">The node.</param>
    private static void SetClassPathReferenceAttribute(AMLNodeWithClassReference node)
    {
        node.ClassPathReferenceAttribute = node.CAEXNode.Name.LocalName switch
        {
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING => CAEX_CLASSModel_TagNames
                .ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH,
            CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING => node.CAEXNode.CAEXSchema() == CAEXDocument.CAEXSchema.CAEX2_15
                ? ""
                : CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFATTRIBUTETYPE,
            CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING => CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFATTRIBUTETYPE,
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING or CAEX_CLASSModel_TagNames.ROLECLASS_STRING
                or CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING
                or CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING => CAEX_CLASSModel_TagNames
                    .ATTRIBUTE_NAME_REFBASECLASSPATH,
            _ => ""
        };
    }

    private (AMLNodeWithClassReference From, AMLNodeWithClassReference To) GetConnectedNodes(InternalLinkType il) =>
        il.AInterface?.Node == CAEXNode
            ? (this, Tree.SelectCaexNode(il.BInterface?.Node, false, false, true) as AMLNodeWithClassReference)
            : (Tree.SelectCaexNode(il.AInterface?.Node, false, false, true) as AMLNodeWithClassReference, this);

    private IEnumerable<AMLNodeWithClassReference> GetPartnerNodes(InternalLinkType il)
    {
        if (il.AInterface == null || il.BInterface == null || Tree?.Root == null)
        {
            return [];
        }

        var partner = il.AInterface.Equals(CAEXObject) ? il.BInterface : il.AInterface;
        return AMLTreeViewModel.FindTreeViewItemsInTree(Tree.Root.Children, partner.Node)
            .OfType<AMLNodeWithClassReference>();
    }

    #endregion Private Methods
}