// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLNodeWithoutName.cs" company="inpro">
//     Copyright (c) inpro. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AMLToolkit.Model;
using CAEX_ClassModel;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AMLNodeWithoutName is the ViewModel for all CAEX-Elements, which don't have a Name-Attribute. The
    /// DisplayName for these Elements is generated, using the Value of the Attribute, which Name is defined in
    /// the <see cref="AMLNodeWithoutName.NameSubstituteAttribute"/>.
    /// </summary>
    public class AMLNodeWithoutName : AMLNodeViewModel
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLNodeViewModel" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="CaexNode">The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeWithoutName(AMLNodeViewModel parent, System.Xml.XmlElement CaexNode, bool lazyLoadChildren) :
            base(parent, CaexNode, lazyLoadChildren)
        {
            SetNameSubstituteAttribute(this);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the DisplayName for the Caex-Element. The Display-Name is build from the Caex-Name of the Element and the
        /// Value of the Attribute, which Name is defined in the <see cref="NameSubstituteAttribute"/>.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                switch (CAEXNode.Name)
                {
                    case CAEX_CLASSModel_TagNames.CAEX_FILE:
                        return "CAEXFile: " + CAEXNode.GetAttributeValue(NameSubstituteAttribute);

                    case CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING:
                        return "RoleRequirement: " + CAEXNode.GetAttributeValue(NameSubstituteAttribute);

                    case CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING:
                        return "SupportedRoleClass: " + CAEXNode.GetAttributeValue(NameSubstituteAttribute);

                    case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING:
                        return "Mapping";

                    case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING:
                        return "AttributeNameMapping: " + CAEXNode.GetAttributeValue(NameSubstituteAttribute);

                    case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING:
                        return "InterfaceNameMapping: " + CAEXNode.GetAttributeValue(NameSubstituteAttribute);
                }

                return "Object without a name";
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the name of the substitute attribute, used to build the DisplayName <see cref="Name"/>.
        /// </summary>
        /// <value>The name substitute attribute.</value>
        public string NameSubstituteAttribute { get; set; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Sets the name substitute attribute for the specified node.         /// 
        /// </summary>
        /// <param name="node">The node.</param>
        private static void SetNameSubstituteAttribute(AMLNodeWithoutName node)
        {
            switch (node.CAEXNode.Name)
            {
                case CAEX_CLASSModel_TagNames.CAEX_FILE:
                    node.NameSubstituteAttribute = "FileName";
                    break;

                case CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING:
                    node.NameSubstituteAttribute = CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASEROLECLASSPATH;
                    break;

                case CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING:
                    node.NameSubstituteAttribute = CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFROLECLASSPATH;
                    break;

                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING:
                    node.NameSubstituteAttribute = string.Empty;
                    break;

                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING:
                    node.NameSubstituteAttribute = CAEX_CLASSModel_TagNames.ATTRIBUTE_SYSTEM_UNIT_ATTRIBUTE_NAME;
                    break;

                case CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING:
                    node.NameSubstituteAttribute = CAEX_CLASSModel_TagNames.ATTRIBUTE_SYSTEM_UNIT_INTERFACE_NAME;
                    break;
            }
        }

        #endregion Private Methods
    }
}