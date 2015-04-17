// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-10-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-10-2015
// ***********************************************************************
// <copyright file="AMLNodeWithClassAndRoleReference.cs" company="inpro">
//     Copyright (c) AutomationML e.V. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using AMLToolkit.Model;
using CAEX_ClassModel;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AMLNodeWithClassAndRoleReference is the ViewModel for all CAEX-Elements, which may have references to CAEX-Classes and Roles.
    /// The ViewModel provides an additional property <see cref="AMLNodeWithClassAndRoleReference.RoleReference"/> for these Elements. The RoleReference
    /// is build from the first RoleRequirement found in the Children Collection of the Element.
    /// </summary>
    public class AMLNodeWithClassAndRoleReference : AMLNodeWithClassReference
    {
        #region Private Fields

        /// <summary>
        /// <see cref="RoleReference" />
        /// </summary>
        private string _roleReference;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLNodeWithClassAndRoleReference" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="CaexNode">The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeWithClassAndRoleReference(AMLNodeViewModel parent, System.Xml.XmlElement CaexNode, bool lazyLoadChildren) :
            base(parent, CaexNode, lazyLoadChildren)
        {
            RefreshNodeInformation();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets and sets the RoleReference
        /// </summary>
        /// <value>The role reference.</value>
        public string RoleReference
        {
            get
            {
                return _roleReference;
            }
            set
            {
                if (_roleReference != value)
                {
                    _roleReference = value; base.RaisePropertyChanged(() => RoleReference);
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Refreshes the node information. This Method can be overridden in derived
        /// classes. The Method should be called, if the CAEX-Elements Data has changed
        /// and the Changes should be visible in any View, that has a binding to this ViewModel.
        /// </summary>
        public override void RefreshNodeInformation()
        {
            base.RefreshNodeInformation();

            if (CAEXNode != null && CAEXNode.HasChildNodes)
            {
                var role = CAEXNode.ChildElements(CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING).FirstOrDefault();
                if (role != null)
                {
                    var reference = role.RoleReference();
                    if (!string.IsNullOrEmpty(reference))
                    {
                        this.RoleReference = System.IO.Path.GetFileNameWithoutExtension(reference);
                    }
                }
            }
        }

        #endregion Public Methods
    }
}