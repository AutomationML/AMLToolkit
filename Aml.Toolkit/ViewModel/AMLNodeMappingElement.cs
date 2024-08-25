// Copyright (c) 2017 AutomationML e.V.
using Aml.Engine.CAEX;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLNodeWithoutName is the ViewModel for all CAEX-Elements, which don't
///     have a Name-Attribute. The DisplayName for these Elements is generated, using
///     the Value of the Attribute, which Name is defined in the <see cref="AMLNodeWithoutName.NameSubstituteAttribute" />.
/// </summary>
public class AMLNodeMappingElement : AMLNodeWithoutName
{
    #region Public Methods

    /// <inheritdoc />
    public override void RefreshNodeInformation(bool expand)
    {
        base.RefreshNodeInformation(expand);

        RaisePropertyChanged(nameof(SucName));
        RaisePropertyChanged(nameof(RcName));
    }

    #endregion Public Methods

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeWithoutName" /> class.
    /// </summary>
    /// <param name="tree">            The Tree, containing the node</param>
    /// <param name="parent">          The parent.</param>
    /// <param name="caexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeMappingElement(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement caexNode,
        bool lazyLoadChildren) :
        base(tree, parent, caexNode, lazyLoadChildren)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeWithoutName" /> class.
    /// </summary>
    /// <param name="parent">          The parent.</param>
    /// <param name="caexNode">        The CAEX node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeMappingElement(AMLNodeViewModel parent, XElement caexNode, bool lazyLoadChildren) :
        this(null, parent, caexNode, lazyLoadChildren)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets the name of the role class.
    /// </summary>
    public string RcName
    {
        get
        {
            return CAEXObject is IMappingElementType me
                ? me is InterfaceIDMappingType im && im.RoleInterface != null
                    ? im.RoleInterface.Name
                    : me.RoleClassElementIdentifier
                : "";
        }
    }

    /// <summary>
    ///     Gets the name of the system unit class
    /// </summary>
    public string SucName
    {
        get
        {
            return CAEXObject is IMappingElementType me
                ? me is InterfaceIDMappingType im && im.SystemUnitInterface != null
                    ? im.SystemUnitInterface.Name
                    : me.SystemUnitClassElementIdentifier
                : "";
        }
    }

    #endregion Public Properties
}