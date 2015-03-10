// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLNodeRegistry.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CAEX_ClassModel;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AMLNodeRegistry defines a dictionary, containing CAEX-TagNames and associated ConstructionInfo for the CAEX-Element,
    /// identifiable by the CAEX-TagName. The Class implements the Singleton Pattern. The Static <see cref="AMLNodeRegistry.Instance"/> contains default
    /// ConstructionInfo for each CAEX-Element. The Default Dictionary Entries may be changed by an application, if a CAEX-Element
    /// has a special ViewModel with a special constructor. Currently, the following associations are created for the 
    /// static Instance:
    /// 
    /// <list type="table">
    /// <listheader>
    /// <term>CAEX-Element</term>
    /// <description>Associated ViewModel-Constructor.> </description>
    /// </listheader>
    /// <item>
    /// <term>InstanceHierarchy</term>
    /// <description>Basic ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeViewModel, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>SystemUnitClassLibrary</term>
    /// <description>Basic ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeViewModel, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>RoleClassLibrary</term>
    /// <description>Basic ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeViewModel, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>InterfaceClassLibrary</term>
    /// <description>Basic ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeViewModel, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>InternalLink</term>
    /// <description>Basic ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeViewModel, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>InternalElement [has Class- and Role-Reference]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithClassAndRoleReference, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>SystemUnitClass [has Class-Reference]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithClassReference, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>RoleClass [has Class-Reference]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithClassReference, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>InterfaceClass [has Class-Reference]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithClassReference, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>ExternalInterface [has Class-Reference]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithClassReference, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>CaexFile [has no Name-Attribute]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithoutName, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>RoleRequirement [has no Name-Attribute]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithoutName, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>SupportedRoleClass [has no Name-Attribute]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithoutName, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>MappingObject [has no Name-Attribute]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithoutName, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>AttributeNameMapping [has no Name-Attribute]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithoutName, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// <item>
    /// <term>InterfaceNameMapping [has no Name-Attribute]</term>
    /// <description>Derived ViewModel: <see cref="M:AMLNodeViewModel.#ctor(AMLNodeWithoutName, System.Xml.XmlElement, System.Boolean)"/></description>
    /// </item>
    /// </list>     
    /// </summary>
    public class AMLNodeRegistry: Dictionary<string, ConstructorInfo>
    {
        private static AMLNodeRegistry m_Instance;


        /// <summary>
        /// Gets the static instance of the AMLNodeRegistry
        /// </summary>
        /// <value>The instance.</value>
        public static AMLNodeRegistry Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var baseType = typeof(AMLNodeViewModel).GetConstructor(new[] { typeof(AMLNodeViewModel), typeof (System.Xml.XmlElement), typeof(bool) });
                    var classAndRoleRefType = typeof(AMLNodeWithClassAndRoleReference).GetConstructor(new[] { typeof(AMLNodeViewModel), typeof(System.Xml.XmlElement), typeof(bool) });
                    var classRefType = typeof(AMLNodeWithClassReference).GetConstructor(new[] { typeof(AMLNodeViewModel), typeof(System.Xml.XmlElement), typeof(bool) });
                    var noNameType = typeof(AMLNodeWithoutName).GetConstructor(new[] { typeof(AMLNodeViewModel), typeof(System.Xml.XmlElement), typeof(bool) });

                    m_Instance = new AMLNodeRegistry();

                    // registration of base Types
                    m_Instance.Add(CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING, baseType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING, baseType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING, baseType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING, baseType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.INTERNALLINK_STRING, baseType);

                    // registration of class and Role Reference Types
                    m_Instance.Add(CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING, classAndRoleRefType);

                    // registration of class Reference Types
                    m_Instance.Add(CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING, classRefType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.ROLECLASS_STRING, classRefType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING, classRefType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING, classRefType);

                    // registration of no Name Types
                    m_Instance.Add(CAEX_CLASSModel_TagNames.CAEX_FILE, noNameType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING, noNameType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING, noNameType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING, noNameType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING, noNameType);
                    m_Instance.Add(CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING, noNameType);
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="AMLNodeRegistry"/> class from being created.
        /// </summary>
        private AMLNodeRegistry ()
        {
        }
    }
}
