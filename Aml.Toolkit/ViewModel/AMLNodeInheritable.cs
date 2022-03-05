// ***********************************************************************
// Assembly : Aml.Toolkit
// Author   : Josef Prinz Created : 03-09-2015
//
// Last Modified By : Josef Prinz Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLNodeWithClassReference.cs" company="inpro">
//    Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// ***********************************************************************

using Aml.Engine.AmlObjects.Extensions;
using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Extensions;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    ///     Class AMLNodeAttribute is the ViewModel for all CAEX-&gt;Attribute-Elements.
    ///     The ViewModel provides an additional property
    ///     <see
    ///         cref="AMLNodeAttribute.AttributeValue" />
    ///     for these Elements.
    /// </summary>
    public class AMLNodeInheritable : AMLNodeWithClassReference
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

            RaisePropertyChanged(nameof( IsDerived));
            RaisePropertyChanged(nameof( IsFacetted));
            RaisePropertyChanged(nameof( IsOverridden));
        }

        #endregion Public Methods

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMLNodeAttribute" /> class.
        /// </summary>
        /// <param name="parent">          The parent.</param>
        /// <param name="CaexNode">        The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeInheritable(AMLNodeViewModel parent, XElement CaexNode, bool lazyLoadChildren) :
            this(null, parent, CaexNode, lazyLoadChildren)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLNodeInheritable"/> class.
        /// </summary>
        /// <param name="tree">The TreeViewModel, containing the node</param>
        /// <param name="parent">The parent.</param>
        /// <param name="CaexNode">The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeInheritable(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement CaexNode,
            bool lazyLoadChildren) :
            base(tree, parent, CaexNode, lazyLoadChildren)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <inheritdoc/>
        public override bool IsDerived => (CAEXObject.CAEXDocument == null) || Parent?.CAEXObject is IClassWithBaseClassReference classObject
                    && classObject.IsInherited(CAEXObject as CAEXBasicObject);

        /// <summary>
        /// Gets a value indicating whether this instance is facetted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is facetted; otherwise, <c>false</c>.
        /// </value>
        public override bool IsFacetted
        {
            get
            {
                return CAEXObject switch
                {
                    AttributeType at => at.IsFacetAttribute(),
                    ExternalInterfaceType ext => ext.IsFacetInterface(),
                    _ => false
                };
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is overridden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is overridden; otherwise, <c>false</c>.
        /// </value>
        public override bool IsOverridden => Parent?.CAEXObject is IClassWithBaseClassReference classObject
                    && classObject.IsOverridden(CAEXObject as CAEXBasicObject);

        #endregion Public Properties
    }
}