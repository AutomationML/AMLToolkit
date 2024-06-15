using Aml.Engine.CAEX;
using Aml.Engine.Xml.Extensions;
using System.Linq;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLNodeWithoutName is the ViewModel for all CAEX-Elements, which don't
///     have a Name-Attribute. The DisplayName for these Elements is generated, using
///     the Value of the Attribute, which Name is defined in the <see cref="NameSubstituteAttribute" />.
/// </summary>
public class AMLNodeWithoutName : AMLNodeViewModel
{
    #region Public Methods

    /// <inheritdoc />
    public override void RefreshNodeInformation(bool expand)
    {
        base.RefreshNodeInformation(expand);
        Parent?.RefreshNodeInformation(expand);

        RaisePropertyChanged(nameof(ShortName));
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    ///     Sets the name substitute attribute for the specified node. ///
    /// </summary>
    /// <param name="node">The node.</param>
    private static void SetNameSubstituteAttribute(AMLNodeWithoutName node)
    {
        node.NameSubstituteAttribute = node.CAEXNode.Name.LocalName switch
        {
            CAEX_CLASSModel_TagNames.CAEX_FILE => "FileName",
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING => CAEX_CLASSModel_TagNames
                .ATTRIBUTE_NAME_REFBASEROLECLASSPATH,
            CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING => CAEX_CLASSModel_TagNames
                .ATTRIBUTE_NAME_REFROLECLASSPATH,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING => string.Empty,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING => CAEX_CLASSModel_TagNames
                .ATTRIBUTE_SYSTEM_UNIT_ATTRIBUTE_NAME,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING => CAEX_CLASSModel_TagNames
                .ATTRIBUTE_SYSTEM_UNIT_INTERFACE_NAME,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACEID_STRING => CAEX_CLASSModel_TagNames
                .ATTRIBUTE_SYSTEM_UNIT_INTERFACE_ID,
            _ => node.NameSubstituteAttribute
        };
    }

    #endregion Private Methods

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeWithoutName" /> class.
    /// </summary>
    /// <param name="tree">            The Tree, containing the node</param>
    /// <param name="parent">          The parent.</param>
    /// <param name="caexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeWithoutName(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement caexNode,
        bool lazyLoadChildren) :
        base(tree, parent, caexNode, lazyLoadChildren)
    {
        SetNameSubstituteAttribute(this);

        //if (caexNode.IsRoleRequirement() && tree != null)
        //{
        //    //tree.TreeViewLayout.PropertyChanged -= LayoutPropertyChanged;
        //    //tree.TreeViewLayout.PropertyChanged += LayoutPropertyChanged;
        //    IsVisible = IsVisibleInLayout;
        //}
    }

    //public override bool IsVisible
    //{
    //    get => CAEXNode.IsRoleRequirement() ?  base.IsVisible && Tree.TreeViewLayout.ShowRoleReqNodes : base.IsVisible;
    //    set => base.IsVisible = value;
    //}

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLNodeWithoutName" /> class.
    /// </summary>
    /// <param name="parent">          The parent.</param>
    /// <param name="caexNode">        The caex node.</param>
    /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
    public AMLNodeWithoutName(AMLNodeViewModel parent, XElement caexNode, bool lazyLoadChildren) :
        this(null, parent, caexNode, lazyLoadChildren)
    {
    }

    #endregion Public Constructors

    #region Public Properties

    /// <inheritdoc />
    public override bool IsRoleReference => CAEXNode.IsRoleRequirement() || CAEXNode.IsSupportedRoleClass();

    //internal bool IsVisibleInLayout => Tree.TreeViewLayout.ShowRoleReqNodes;

    /// <summary>
    /// Change the visibility of this group node if a corresponding layout property changes
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    //private void LayoutPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    switch (e.PropertyName)
    //    {
    //        case nameof(AMLLayout.ShowRoleReqNodes):
    //            IsVisible = IsVisibleInLayout;
    //            break;
    //    }
    //}
    /// <summary>
    ///     Gets the DisplayName for the Caex-Element. The Display-Name is build from
    ///     the Caex-Name of the Element and the Value of the Attribute, which Name is
    ///     defined in the <see cref="NameSubstituteAttribute" />.
    /// </summary>
    /// <value>The name.</value>
    public override string Name
    {
        get
        {
            switch (CAEXNode.Name.LocalName)
            {
                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING:
                    return "Mapping";

                case CAEX_CLASSModel_TagNames.CAEX_FILE:
                    {
                        var name = CAEXNode.Attribute(NameSubstituteAttribute)?.Value;
                        return string.IsNullOrEmpty(name) ? CAEXNode.Name.LocalName : name;
                    }

                case CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING:
                case CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING:
                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING:
                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING:
                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACEID_STRING:
                    {
                        var name = CAEXNode.Attribute(NameSubstituteAttribute)?.Value;
                        return string.IsNullOrEmpty(name) ? "" : name;
                    }
            }

            return CAEXNode.Name.LocalName;
        }

        set => base.Name = value;
    }

    /// <summary>
    ///     Gets or sets the name of the substitute attribute, used to build the
    ///     DisplayName <see cref="Name" />.
    /// </summary>
    /// <value>The name substitute attribute.</value>
    public string NameSubstituteAttribute { get; set; }

    /// <summary>
    ///     Gets the short name which only contains the last stripped part of the class path
    /// </summary>
    public string ShortName => Name?.Split('/').Last();

    #endregion Public Properties
}