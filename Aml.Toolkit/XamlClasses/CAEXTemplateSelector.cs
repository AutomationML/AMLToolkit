// *********************************************************************** Assembly :
// Aml.Toolkit Author : Josef Prinz Created : 03-09-2015
//
// Last Modified By : Josef Prinz Last Modified On : 03-09-2015 ***********************************************************************
// <copyright file="CAEXTemplateSelector.cs" company="inpro">
//    Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************

using Aml.Engine.CAEX;
using Aml.Toolkit.ViewModel;
using System.Windows;
using System.Windows.Controls;

/// <summary>
///    The XamlClasses namespace.
/// </summary>
namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     Class CAEXTemplateSelector selects DataTemplates to view CAEX-Elements, based
///     on the Element's Name <seealso cref="CAEX_CLASSModel_TagNames" />
/// </summary>
public class CAEXTemplateSelector : DataTemplateSelector
{
    #region Public Methods

    /// <inheritdoc />
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var node = item as AMLNodeViewModel;

        if (node is AMLNodeGroupViewModel virtNode)
        {
            switch (virtNode.GroupName)
            {
                case AMLLayout.SET_INTERFACE_GROUPING:
                    return VirtEITemplate;
                case AMLLayout.SET_ROLE_GROUPING:
                    return VirtRRTemplate;
            }
        }
        else if (node?.CAEXNode != null)
        {
            switch (node.CAEXNode.Name.LocalName)
            {
                case CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING:
                    return AttributeTemplate;

                case CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING:
                    return InternalElementTemplate;

                case CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING:
                    return InstanceHierarchyTemplate;

                case CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING:
                    return SystemUnitClassLibTemplate;

                case CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING:
                    return SystemUnitClassTemplate;

                case CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING:
                    return AttributeTypeTemplate;

                case CAEX_CLASSModel_TagNames.INTERNALLINK_STRING:
                    return InternalLinkTemplate;

                case CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING:
                    return RoleClassLibTemplate;

                case CAEX_CLASSModel_TagNames.ROLECLASS_STRING:
                    return RoleClassTemplate;

                case CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING:
                    return InterfaceClassLibTemplate;

                case CAEX_CLASSModel_TagNames.ATTRIBUTETYPELIB_STRING:
                    return AttributeTypeLibTemplate;

                case CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING:
                    return InterfaceClassTemplate;

                case CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING:
                    return ExternalInterfaceTemplate;

                case CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING:
                    return RoleRequirementTemplate;

                case CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING:
                    return SupportedRoleClassTemplate;

                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING:
                    return MappingObjectTemplate;

                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING:
                    return AttributeMappingTemplate;

                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACEID_STRING:
                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING:
                    return InterfaceMappingTemplate;
            }
        }

        return base.SelectTemplate(item, container);
    }

    #endregion Public Methods

    #region Public Properties

    /// <summary>
    ///     Gets or sets the DataTemplate to display an AttributeNameMapping Caex-Element.
    /// </summary>
    /// <value>The attribute mapping template.</value>
    public DataTemplate AttributeMappingTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an Attribute Caex-Element.
    /// </summary>
    /// <value>The attribute mapping template.</value>
    public DataTemplate AttributeTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an AttributeTypeLib Caex-Element.
    /// </summary>
    /// <value>The attribute tyle library template.</value>
    public DataTemplate AttributeTypeLibTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a AttributeType Caex-Element.
    /// </summary>
    /// <value>
    ///     The attribute type template.
    /// </value>
    public DataTemplate AttributeTypeTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the the DataTemplate to display  an external interface CAEX-Element.
    /// </summary>
    /// <value>
    ///     The external interface template.
    /// </value>
    public DataTemplate ExternalInterfaceTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an InstanceHierarchy Caex-Element.
    /// </summary>
    /// <value>The instance hierarchy template.</value>
    public DataTemplate InstanceHierarchyTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an InterfaceClassLib Caex-Element.
    /// </summary>
    /// <value>The interface class library template.</value>
    public DataTemplate InterfaceClassLibTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an InterfaceClass Caex-Element.
    /// </summary>
    /// <value>The interface class template.</value>
    public DataTemplate InterfaceClassTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an InterfaceNameMapping Caex-Element.
    /// </summary>
    /// <value>The interface mapping template.</value>
    public DataTemplate InterfaceMappingTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display an InternalElement Caex-Element.
    /// </summary>
    /// <value>The internal element template.</value>
    public DataTemplate InternalElementTemplate { get; set; }


    /// <summary>
    ///     Gets or sets the DataTemplate to display an InternalLink Caex-Element.
    /// </summary>
    /// <value>The internal link template.</value>
    public DataTemplate InternalLinkTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a MappingObject Caex-Element.
    /// </summary>
    /// <value>The mapping object template.</value>
    public DataTemplate MappingObjectTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a RoleClassLib Caex-Element.
    /// </summary>
    /// <value>The role class library template.</value>
    public DataTemplate RoleClassLibTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a RoleClass Caex-Element.
    /// </summary>
    /// <value>The role class template.</value>
    public DataTemplate RoleClassTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a RoleRequirement Caex-Element.
    /// </summary>
    /// <value>The role requirement template.</value>
    public DataTemplate RoleRequirementTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a SupportedRoleClass Caex-Element.
    /// </summary>
    /// <value>The supported role class template.</value>
    public DataTemplate SupportedRoleClassTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a SystemUnitClassLib Caex-Element.
    /// </summary>
    /// <value>The system unit class library template.</value>
    public DataTemplate SystemUnitClassLibTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a SystemUnitClass Caex-Element.
    /// </summary>
    /// <value>The system unit class template.</value>
    public DataTemplate SystemUnitClassTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a Virtual Node to group ExternalInterface Caex-Elements.
    /// </summary>
    public DataTemplate VirtEITemplate { get; set; }

    /// <summary>
    ///     Gets or sets the DataTemplate to display a Virtual Node to group RoleRequirement Caex-Elements.
    /// </summary>
    public DataTemplate VirtRRTemplate { get; set; }

    #endregion Public Properties
}