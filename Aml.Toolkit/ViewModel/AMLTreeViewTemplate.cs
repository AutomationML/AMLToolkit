// ***********************************************************************
// Assembly : Aml.Toolkit
// Author : Josef Prinz
// Created : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLTreeViewTemplate.cs" company="inpro">
//    Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************

using System.Collections.Generic;
using Aml.Engine.CAEX;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLTreeViewTemplate defines default TreeViewTemplates for CAEX-Elements,
///     that can be displayed in a TreeView. If a TreeView is build with one of the
///     defined Templates, it is still possible to configure the TreeView, using a Node-Filter.
/// </summary>
public static class AMLTreeViewTemplate
{
    #region Public Fields

    /// <summary>
    ///     The AttributeTypeLibrary-Tree contains the following Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>AttributeTypeLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>AttributeType</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> AttributeTypeLibTree =
        new()
        {
            CAEX_CLASSModel_TagNames.ATTRIBUTETYPELIB_STRING,
            CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING,
            CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING
        };

    /// <summary>
    ///     The complete InstanceHierarchyTree contains the following Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>InstanceHierarchy</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalElement</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalLink</term>
    ///         </item>
    ///         <item>
    ///             <term>ExternalInterface</term>
    ///         </item>
    ///         <item>
    ///             <term>RoleRequirements</term>
    ///         </item>
    ///         <item>
    ///             <term>SupportedRoleClass</term>
    ///         </item>
    ///         <item>
    ///             <term>MappingObject</term>
    ///         </item>
    ///         <item>
    ///             <term>AttributeNameMapping</term>
    ///         </item>
    ///         <item>
    ///             <term>InterfaceNameMapping</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> CompleteInstanceHierarchyTree =
        new()
        {
            CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.INTERNALLINK_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING,
            CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACEID_STRING
        };

    /// <summary>
    ///     The complete SystemUnitClassLibrary-Tree contains the following
    ///     Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>SystemUnitClassLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>SystemUnitClass</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalElement</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalLink</term>
    ///         </item>
    ///         <item>
    ///             <term>ExternalInterface</term>
    ///         </item>
    ///         <item>
    ///             <term>RoleRequirements</term>
    ///         </item>
    ///         <item>
    ///             <term>SupportedRoleClass</term>
    ///         </item>
    ///         <item>
    ///             <term>MappingObject</term>
    ///         </item>
    ///         <item>
    ///             <term>AttributeNameMapping</term>
    ///         </item>
    ///         <item>
    ///             <term>InterfaceNameMapping</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> CompleteSystemUnitClassLibTree =
        new()
        {
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.INTERNALLINK_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACEID_STRING
        };

    /// <summary>
    ///     The extended InstanceHierarchyTree, which is a subset of the
    ///     <see
    ///         cref="CompleteInstanceHierarchyTree" />
    ///     contains the following
    ///     Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>InstanceHierarchy</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalElement</term>
    ///         </item>
    ///         <item>
    ///             <term>ExternalInterface</term>
    ///         </item>
    ///         <item>
    ///             <term>RoleRequirements</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> ExtendedInstanceHierarchyTree =
        new()
        {
            CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING,
            CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING
        };

    /// <summary>
    ///     The extended RoleClassLibrary-Tree contains the following Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>RoleClassLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>RoleClass</term>
    ///         </item>
    ///         <item>
    ///             <term>ExternalInterface</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> ExtendedRoleClassLibTree = new()
    {
        CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING,
        CAEX_CLASSModel_TagNames.ROLECLASS_STRING,
        CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING
    };

    /// <summary>
    ///     The extended SystemUnitClassLibrary-Tree contains the following
    ///     Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>SystemUnitClassLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>SystemUnitClass</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalElement</term>
    ///         </item>
    ///         <item>
    ///             <term>ExternalInterface</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> ExtendedSystemUnitClassLibTree = new()
    {
        CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING,
        CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING,
        CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
        CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
        CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING,
        CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING
    };

    /// <summary>
    ///     The InterfaceClassLibrary-Tree contains the following Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>InterfaceClassLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>InterfaceClass</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> InterfaceClassLibTree = new()
    {
        CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING,
        CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING,
        CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING
    };

    /// <summary>
    ///     The simple InstanceHierarchyTree, which is a subset of the
    ///     <see
    ///         cref="ExtendedInstanceHierarchyTree" />
    ///     contains the following
    ///     Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>InstanceHierarchy</term>
    ///         </item>
    ///         <item>
    ///             <term>InternalElement</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> SimpleInstanceHierarchyTree =
        new()
        {
            CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING
        };

    /// <summary>
    ///     The simple RoleClassLibrary-Tree contains the following Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>RoleClassLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>RoleClass</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> SimpleRoleClassLibTree =
        new()
        {
            CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.ROLECLASS_STRING
        };

    /// <summary>
    ///     The simple SystemUnitClassLibrary-Tree contains the following
    ///     Element-Names:
    ///     <list type="number">
    ///         <item>
    ///             <term>SystemUnitClassLibrary</term>
    ///         </item>
    ///         <item>
    ///             <term>SystemUnitClass</term>
    ///         </item>
    ///     </list>
    /// </summary>
    public static readonly HashSet<string> SimpleSystemUnitClassLibTree =
        new()
        {
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING
        };

    #endregion Public Fields
}