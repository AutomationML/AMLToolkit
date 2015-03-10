// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLTreeViewTemplate.cs" company="inpro">
//     Copyright (c) inpro. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using CAEX_ClassModel;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AMLTreeViewTemplate defines default TreeViewTemplates for CAEX-Elements, that can be displayed in a TreeView.
    /// If a TreeView is build with one of the defined Templates, it is still possible to configure the TreeView, using a
    /// Node-Filter.
    /// </summary>
    public static class AMLTreeViewTemplate
    {
        #region Public Fields

        /// <summary>
        /// The complete InstanceHierarchyTree contains the following Element-Names:
        /// <list type="number">       
        /// <item>
        /// <term>InstanceHierarchy</term>
        /// </item> 
        /// <item>
        /// <term>InternalElement</term>
        /// </item> 
        /// <item>
        /// <term>InternalLink</term>
        /// </item>  
        /// <item>
        /// <term>ExternalInterface</term>
        /// </item> 
        /// <item>
        /// <term>RoleRequirements</term>
        /// </item> 
        /// <item>
        /// <term>SupportedRoleClass</term>
        /// </item> 
        /// <item>
        /// <term>MappingObject</term>
        /// </item> 
        /// <item>
        /// <term>AttributeNameMapping</term>
        /// </item> 
        /// <item>
        /// <term>InterfaceNameMapping</term>
        /// </item>
        /// </list>
        /// </summary>
        public static List<string> CompleteInstanceHierarchyTree = new List<string> {
            CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.INTERNALLINK_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING,
            CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING
        };

        /// <summary>
        /// The complete SystemUnitClassLibrary-Tree contains the following Element-Names:
        /// <list type="number">        
        /// <item>
        /// <term>SystemUnitClassLibrary</term>
        /// </item>        
        /// <item>
        /// <term>SystemUnitClass</term>
        /// </item> 
        /// <item>
        /// <term>InternalElement</term>
        /// </item>  
        /// <item>
        /// <term>InternalLink</term>
        /// </item> 
        /// <item>
        /// <term>ExternalInterface</term>
        /// </item> 
        /// <item>
        /// <term>RoleRequirements</term>
        /// </item> 
        /// <item>
        /// <term>SupportedRoleClass</term>
        /// </item> 
        /// <item>
        /// <term>MappingObject</term>
        /// </item> 
        /// <item>
        /// <term>AttributeNameMapping</term>
        /// </item> 
        /// <item>
        /// <term>InterfaceNameMapping</term>
        /// </item>
        /// </list>
        /// </summary>
        public static List<string> CompleteSystemUnitClassTree = new List<string> {
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.INTERNALLINK_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING,
            CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING,
            CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING };

        /// <summary>
        /// The extended InstanceHierarchyTree, which is a subset of the <see cref="CompleteInstanceHierarchyTree"/> contains the following Element-Names:
        /// <list type="number">       
        /// <item>
        /// <term>InstanceHierarchy</term>
        /// </item> 
        /// <item>
        /// <term>InternalElement</term>
        /// </item> 
        /// <item>
        /// <term>ExternalInterface</term>
        /// </item> 
        /// <item>
        /// <term>RoleRequirements</term>
        /// </item> 
        /// </list>
        /// </summary>
        public static List<string> ExtendedInstanceHierarchyTree = new List<string> {
            CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING
        };

        /// <summary>
        /// The extended RoleClassLibrary-Tree contains the following Element-Names:
        /// <list type="number">       
        /// <item>
        /// <term>RoleClassLibrary</term>
        /// </item> 
        /// <item>
        /// <term>RoleClass</term>
        /// </item> 
        /// <item>
        /// <term>ExternalInterface</term>
        /// </item>
        /// </list>
        /// </summary>
        public static List<string> ExtendedRoleClassTree = new List<string> {
            CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.ROLECLASS_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING 
        };

        /// <summary>
        /// The extended SystemUnitClassLibrary-Tree contains the following Element-Names:
        /// <item>
        /// <term>SystemUnitClassLibrary</term>
        /// </item>        
        /// <item>
        /// <term>SystemUnitClass</term>
        /// </item> 
        /// <item>
        /// <term>InternalElement</term>
        /// </item> 
        /// <item>
        /// <term>ExternalInterface</term>
        /// </item> 
        /// </summary>
        public static List<string> ExtendedSystemUnitClassTree = new List<string> {
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING };


        /// <summary>
        /// The InterfaceClassLibrary-Tree contains the following Element-Names:
        /// <item>
        /// <term>InterfaceClassLibrary</term>
        /// </item>        
        /// <item>
        /// <term>InterfaceClass</term>
        /// </item>
        /// </summary>
        public static List<string> InterfaceClassTree = new List<string> {
            CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING };

        /// <summary>
        /// The simple InstanceHierarchyTree, which is a subset of the <see cref="ExtendedInstanceHierarchyTree"/> contains the following Element-Names:
        /// <list type="number">       
        /// <item>
        /// <term>InstanceHierarchy</term>
        /// </item> 
        /// <item>
        /// <term>InternalElement</term>
        /// </item> 
        /// </list>
        /// </summary>
        public static List<string> SimpleInstanceHierarchyTree = new List<string> {
            CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING,
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING };

        /// <summary>
        /// The simple RoleClassLibrary-Tree contains the following Element-Names:
        /// <list type="number">       
        /// <item>
        /// <term>RoleClassLibrary</term>
        /// </item> 
        /// <item>
        /// <term>RoleClass</term>
        /// </item> 
        /// </list>
        /// </summary>
        public static List<string> SimpleRoleClassTree = new List<string> {
            CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.ROLECLASS_STRING };

        /// <summary>
        /// The simple SystemUnitClassLibrary-Tree contains the following Element-Names:
        /// <item>
        /// <term>SystemUnitClassLibrary</term>
        /// </item>        
        /// <item>
        /// <term>SystemUnitClass</term>
        /// </item> 
        /// </summary>
        public static List<string> SimpleSystemUnitClassTree = new List<string> {
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING,
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING };

        #endregion Public Fields
    }
}