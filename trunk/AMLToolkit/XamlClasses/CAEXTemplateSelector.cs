// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="CAEXTemplateSelector.cs" company="inpro">
//     Copyright (c) inpro. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using System.Windows.Controls;
using AMLToolkit.ViewModel;
using CAEX_ClassModel;

/// <summary>
/// The XamlClasses namespace.
/// </summary>
namespace AMLToolkit.XamlClasses
{
    /// <summary>
    /// Class CAEXTemplateSelector selects DataTemplates to view CAEX-Elements, based on the Element's Name <seealso cref="CAEX_ClassModel.CAEX_CLASSModel_TagNames"/>
    /// </summary>
    public class CAEXTemplateSelector : DataTemplateSelector
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the DataTemplate to display an AttributeNameMapping Caex-Element.
        /// </summary>
        /// <value>The attribute mapping template.</value>
        public DataTemplate AttributeMappingTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display an InstanceHierarchy Caex-Element.
        /// </summary>
        /// <value>The instance hierarchy template.</value>
        public DataTemplate InstanceHierarchyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display an InterfaceClassLib Caex-Element.
        /// </summary>
        /// <value>The interface class library template.</value>
        public DataTemplate InterfaceClassLibTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display an InterfaceClass Caex-Element.
        /// </summary>
        /// <value>The interface class template.</value>
        public DataTemplate InterfaceClassTemplate { get; set; }


        /// <summary>
        /// Gets or sets the DataTemplate to display an InternalLink Caex-Element.
        /// </summary>
        /// <value>The internal link template.</value>
        public DataTemplate InternalLinkTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display an InterfaceNameMapping Caex-Element.
        /// </summary>
        /// <value>The interface mapping template.</value>
        public DataTemplate InterfaceMappingTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display an InternalElement Caex-Element.
        /// </summary>
        /// <value>The internal element template.</value>
        public DataTemplate InternalElementTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a MappingObject Caex-Element.
        /// </summary>
        /// <value>The mapping object template.</value>
        public DataTemplate MappingObjectTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a RoleClassLib Caex-Element.
        /// </summary>
        /// <value>The role class library template.</value>
        public DataTemplate RoleClassLibTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a RoleClass Caex-Element.
        /// </summary>
        /// <value>The role class template.</value>
        public DataTemplate RoleClassTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a RoleRequirement Caex-Element.
        /// </summary>
        /// <value>The role requirement template.</value>
        public DataTemplate RoleRequirementTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a SupportedRoleClass Caex-Element.
        /// </summary>
        /// <value>The supported role class template.</value>
        public DataTemplate SupportedRoleClassTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a SystemUnitClassLib Caex-Element.
        /// </summary>
        /// <value>The system unit class library template.</value>
        public DataTemplate SystemUnitClassLibTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate to display a SystemUnitClass Caex-Element.
        /// </summary>
        /// <value>The system unit class template.</value>
        public DataTemplate SystemUnitClassTemplate { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gibt beim Überschreiben in einer abgeleiteten Klasse ein <see cref="T:System.Windows.DataTemplate" />-Objekt auf der Grundlage einer benutzerdefinierten Logik zurück.
        /// </summary>
        /// <param name="item">Das Datenobjekt, für das die Vorlage ausgewählt werden soll.</param>
        /// <param name="container">Das datengebundene Objekt.</param>
        /// <returns>Gibt eine <see cref="T:System.Windows.DataTemplate" /> oder null zurück.Der Standardwert ist null.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var node = (item as AMLNodeViewModel);

            if (node != null && node.CAEXNode != null)
            {
                switch (node.CAEXNode.Name)
                {
                    case CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING:
                        return InternalElementTemplate;

                    case CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING:
                        return InstanceHierarchyTemplate;

                    case CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING:
                        return SystemUnitClassLibTemplate;

                    case CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING:
                        return SystemUnitClassTemplate;

                    case CAEX_CLASSModel_TagNames.INTERNALLINK_STRING:
                        return InternalLinkTemplate;

                    case CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING:
                        return RoleClassLibTemplate;

                    case CAEX_CLASSModel_TagNames.ROLECLASS_STRING:
                        return RoleClassTemplate;

                    case CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING:
                        return InterfaceClassLibTemplate;

                    case CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING:
                    case CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING:
                        return InterfaceClassTemplate;

                    case CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING:
                        return RoleRequirementTemplate;

                    case CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING:
                        return SupportedRoleClassTemplate;

                    case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING:
                        return MappingObjectTemplate;

                    case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING:
                        return AttributeMappingTemplate;

                    case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING:
                        return InterfaceMappingTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

        #endregion Public Methods
    }
}