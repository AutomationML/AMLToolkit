// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLNodeWithClassReference.cs" company="inpro">
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
    /// Class AMLNodeWithClassReference is the ViewModel for all CAEX-Elements, which may have references to CAEX-Classes.
    /// The ViewModel provides an additional property <see cref="AMLNodeWithClassReference.ClassReference"/> for these Elements.
    /// </summary>
    public class AMLNodeWithClassReference : AMLNodeViewModel
    {
        #region Private Fields

        /// <summary>
        /// <see cref="ClassReference" />
        /// </summary>
        private string classReference;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLNodeViewModel" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="CaexNode">The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeWithClassReference(AMLNodeViewModel parent, System.Xml.XmlElement CaexNode, bool lazyLoadChildren) :
            base(parent, CaexNode, lazyLoadChildren)
        {
            SetClassPathReferenceAttribute(this);
            RefreshNodeInformation();            
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the class path reference attribute. 
        /// </summary>
        /// <value>The class path reference attribute.</value>
        public string ClassPathReferenceAttribute { get; set; }

        /// <summary>
        /// Gets and sets the ClassReference
        /// </summary>
        /// <value>The class reference.</value>
        public string ClassReference
        {
            get
            {
                return classReference;
            }
            private set
            {
                if (classReference != value)
                {
                    classReference = value; base.RaisePropertyChanged(() => ClassReference);
                }
            }
        }

        /// <summary>
        /// Refreshes the node information. This Method can be overridden in derived classes. The Method
        /// should be called, if the CAEX-Elements Data has changed and the Changes should be visible in any
        /// View, that has a binding to this ViewModel.
        /// </summary>
        public override void RefreshNodeInformation()
        {
            base.RefreshNodeInformation();

            if (CAEXNode != null && CAEXNode.HasAttribute(ClassPathReferenceAttribute))
            {
                var reference = CAEXNode.GetAttributeValue(ClassPathReferenceAttribute);
                if (!string.IsNullOrEmpty(reference))
                {
                    ClassReference = System.IO.Path.GetFileNameWithoutExtension(reference);
                }
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Sets the name of the class path reference attribute, used to get the value for the <see cref="AMLNodeWithClassReference.ClassReference"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        private static void SetClassPathReferenceAttribute(AMLNodeWithClassReference node)
        {
            switch (node.CAEXNode.Name)
            {
                case CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING:
                    node.ClassPathReferenceAttribute = CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH;
                    break;

                case CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING:
                case CAEX_CLASSModel_TagNames.ROLECLASS_STRING:
                case CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING:
                case CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING:
                    node.ClassPathReferenceAttribute = CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH;
                    break;

                default:
                    node.ClassPathReferenceAttribute = "not an object with a ClassPath - Reference";
                    break;
            }
        }

        #endregion Private Methods
    }
}