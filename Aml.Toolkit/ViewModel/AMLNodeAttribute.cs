// Copyright (c) 2017 AutomationML e.V.
using Aml.Engine.CAEX;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLNodeAttribute is the ViewModel for all CAEX-&gt;Attribute-Elements.
///     The ViewModel provides an additional property
///     <see
///         cref="AttributeValue" />
///     for these Elements.
/// </summary>
public class AMLNodeAttribute : AMLNodeInheritable
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

        RaisePropertyChanged(nameof(AttributeValue));
        RaisePropertyChanged(nameof(IsMirror));
    }

    #endregion Public Methods

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeAttribute" /> class.
    /// </summary>
    /// <param name="parent">          The parent.</param>
    /// <param name="CaexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeAttribute(AMLNodeViewModel parent, XElement CaexNode, bool lazyLoadChildren) :
        this(null, parent, CaexNode, lazyLoadChildren)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeAttribute" /> class.
    /// </summary>
    /// <param name="tree">The tree.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="CaexNode">The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeAttribute(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement CaexNode,
        bool lazyLoadChildren) :
        base(tree, parent, CaexNode, lazyLoadChildren)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets the AttributeValue
    /// </summary>
    public string AttributeValue
    {
        get
        {
            return CAEXObject is AttributeTypeType att && !string.IsNullOrEmpty(att.Value) ? $"{att.Value} {att.Unit}" : string.Empty;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this instance is a mirror.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is mirror; otherwise, <c>false</c>.
    /// </value>
    public override bool IsMirror => CAEXObject is AttributeType { IsMirror: true } att && att.Master != null;

    #endregion Public Properties
}