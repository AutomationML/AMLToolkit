﻿using Aml.Engine.AmlObjects;
using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Extensions;
using Aml.Engine.Services;
using Aml.Engine.Xml.Extensions;
using Aml.Toolkit.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    ///     Class AMLNodeWithClassReference is the ViewModel for all CAEX-Elements, which
    ///     may have references to CAEX-Classes. The ViewModel provides an additional
    ///     property <see cref="ClassReference" /> for these Elements.
    /// </summary>
    public class AMLNodeWithClassReference : AMLNodeViewModel
    {
        #region Private Fields

        private List<AMLNodeViewModel> _missingLinks;

        /// <summary>
        ///     <see cref="ShowLinks" />
        /// </summary>
        private bool _showLinks;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMLNodeWithClassReference" /> class.
        /// </summary>
        /// <param name="tree">            The TreeViewModel, containing the node</param>
        /// <param name="parent">          The parent.</param>
        /// <param name="CaexNode">        The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeWithClassReference(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement CaexNode,
            bool lazyLoadChildren) :
            base(tree, parent, CaexNode, lazyLoadChildren)
        {
            SetClassPathReferenceAttribute(this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMLNodeWithClassReference" /> class.
        /// </summary>
        /// <param name="parent">          The parent.</param>
        /// <param name="CaexNode">        The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeWithClassReference(AMLNodeViewModel parent, XElement CaexNode, bool lazyLoadChildren) :
            this(null, parent, CaexNode, lazyLoadChildren)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the class path reference attribute.
        /// </summary>
        /// <value>The class path reference attribute.</value>
        public string ClassPathReferenceAttribute { get; set; }

        /// <summary>
        ///     Gets and sets the ClassReference
        /// </summary>
        /// <value>The class reference.</value>
        public string ClassReference
        {
            get
            {
                if (string.IsNullOrEmpty(ClassPathReferenceAttribute))
                {
                    return null;
                }

                if (CAEXObject is IMirror mirror && mirror.Master != null)
                {
                    return mirror.Master.Name;
                }

                var reference = CAEXNode.Attribute(ClassPathReferenceAttribute)?.Value;
                return !string.IsNullOrEmpty(reference) ? reference.Split('/').Last() : null;
            }
        }

        /// <summary>
        /// Gets the class reference label.
        /// </summary>
        /// <value>
        /// The class reference label.
        /// </value>
        public string ClassReferenceLabel => IsMirror ? "Master:" : CAEXObject is AttributeType ? "Type" : "Class:";

        /// <summary>
        /// Gets a value indicating whether this instance has external data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has external data; otherwise, <c>false</c>.
        /// </value>
        public bool HasExternalData
        {
            get
            {
                if (CAEXObject is ExternalInterfaceType ie /* && ie.IsExternalDataConnector()*/
                )
                {
                    return !string.IsNullOrEmpty(((ObjectWithAMLAttributes)ie).RefURIAttribute?.FilePath);
                }

                return false;
            }
        }

        /// <summary>
        ///     Gets and sets the HasLinks
        /// </summary>
        public bool HasLinks
        {
            get
            {
                if (CAEXObject is ExternalInterfaceType ie && ie.AssociatedObject is SystemUnitClassType)
                {
                    return ie.InternalLinksToInterface().Any();
                }

                return false;
            }
        }

        public string MinCardinality
        {
            get
            {
                if (CAEXObject is ExternalInterfaceType ie )
                {
                    var cardinality = ie.Attribute.FirstOrDefault (a=> 
                            AutomationMLBaseAttributeTypeLib.IsCardinality(a));

                    if (cardinality != null)
                    {
                        var value = cardinality.Attribute["MinOccur"]?.Value;
                        return value ?? "0";
                    }
                }
                return "0";
            }
        }

      

        public string MaxCardinalityWarn
        {
            get 
            { 
                var max = MaxCardinality;
                if (string.IsNullOrEmpty(max) || max== "n")
                return "";

                if (int.TryParse(max, out var maxCardinality))
                {
                    if (CAEXObject is ExternalInterfaceType ie && ie.AssociatedObject is SystemUnitClassType)
                    {
                        return (ie.InternalLinksToInterface().Count() > maxCardinality) ? "!" :"";
                    }
                }
                
                return "";
            }
        }

        public string MinCardinalityWarn
        {
            get 
            { 
                var min = MinCardinality;
                if (string.IsNullOrEmpty(min) || min== "0")
                return "";

                if (int.TryParse(min, out var minCardinality))
                {
                    if (CAEXObject is ExternalInterfaceType ie && ie.AssociatedObject is SystemUnitClassType)
                    {
                        return (ie.InternalLinksToInterface().Count() < minCardinality) ? "!" :"";
                    }
                }
                
                return "";
            }
        }

        public string MaxCardinality
        {
            get
            {
                if (CAEXObject is ExternalInterfaceType ie )
                {
                    var cardinality = ie.Attribute.FirstOrDefault (a=> 
                            AutomationMLBaseAttributeTypeLib.IsCardinality(a));

                    if (cardinality != null)
                    {
                        var value = cardinality.Attribute["MaxOccur"]?.Value;
                        return value ?? "n";
                    }
                }
                return "n";
            }
        }

        public bool HasCardinality
        {
            get
            {
                if (CAEXObject is ExternalInterfaceType ie && ie.AssociatedObject is SystemUnitClassType)
                {
                    var cardinality = ie.Attribute.FirstOrDefault (a=> 
                            AutomationMLBaseAttributeTypeLib.IsCardinality(a));

                    return cardinality != null;
                }
                    

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has missing links.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has missing links; otherwise, <c>false</c>.
        /// </value>
        public bool HasMissingLinks => ShowLinks && MissingLinks > 0;

        /// <summary>
        /// Gets a value indicating whether this instance is master.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is master; otherwise, <c>false</c>.
        /// </value>
        public override bool IsMaster
        {
            get
            {
                if (CAEXNode.IsInternalElement())
                {
                    return new InternalElementType(CAEXNode).IsMaster();
                }

                return CAEXNode.IsExternalInterface() && new ExternalInterfaceType(CAEXNode).IsMaster();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mirror.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mirror; otherwise, <c>false</c>.
        /// </value>
        public override bool IsMirror
        {
            get
            {
                return CAEXNode.IsInternalElement()
                    ? new InternalElementType(CAEXNode).IsMirror
                    : CAEXNode.IsExternalInterface() && new ExternalInterfaceType(CAEXNode).IsMirror;
            }
        }

        /// <summary>
        /// Gets the missing link connections.
        /// </summary>
        /// <value>
        /// The missing link connections.
        /// </value>
        public List<AMLNodeViewModel> MissingLinkConnections =>
            _missingLinks ??= new List<AMLNodeViewModel>();

        /// <summary>
        /// Gets the missing links.
        /// </summary>
        /// <value>
        /// The missing links.
        /// </value>
        public int MissingLinks => _missingLinks?.Count ?? 0;

        /// <summary>
        ///     Gets and sets the ShowLinks
        /// </summary>
        public bool ShowLinks
        {
            get => _showLinks;
            set
            {
                if (_showLinks == value)
                {
                    return;
                }

                if (value && !HasLinks)
                {
                    return;
                }

                UpdateLinks(value, true);
            }
        }

        /// <summary>
        ///     Gets and sets the ShowReference
        /// </summary>
        public bool ShowReference => true;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Closes the link.
        /// </summary>
        /// <param name="partner">The partner.</param>
        public void CloseLink(AMLNodeViewModel partner)
        {
            if (_showLinks)
            {
                _ = new Action(() =>
                      {
                          if (CAEXObject is ExternalInterfaceType ie)
                          {
                              if (ie.AssociatedObject is SystemUnitClassType)
                              {
                                  if (ie.InternalLinksToInterface().Any(il => il.AInterface?.Node != partner.CAEXNode &&
                                                                              il.BInterface?.Node != partner.CAEXNode))
                                  {
                                      return;
                                  }
                              }
                          }

                          _showLinks = false;
                          RaisePropertyChanged(nameof( ShowLinks));
                      }
                ).OnUIThread();
            }
        }

        /// <summary>
        ///     Refreshes the node information. This Method can be overridden in derived
        ///     classes. The Method should be called, if the CAEX-Elements Data has changed
        ///     and the Changes should be visible in any View, that has a binding to this ViewModel.
        /// </summary>
        public override void RefreshNodeInformation(bool expand)
        {
            base.RefreshNodeInformation(expand);

            RaisePropertyChanged(nameof( ShowReference));
            RaisePropertyChanged(nameof( IsMaster));
            RaisePropertyChanged(nameof( IsMirror));
            RaisePropertyChanged(nameof( ShowLinks));
            RaisePropertyChanged(nameof( HasLinks));            
            RaisePropertyChanged(nameof( HasCardinality));
            RaisePropertyChanged(nameof( ClassReference));
            RaisePropertyChanged(nameof( HasExternalData));            
            RaisePropertyChanged(nameof( MaxCardinality));            
            RaisePropertyChanged(nameof( MinCardinality));
                     
            RaisePropertyChanged(nameof( MaxCardinalityWarn));            
            RaisePropertyChanged(nameof( MinCardinalityWarn));
            //Tree.TreeViewLayout.RaisePropertyChanged("HideExpander");

            if (!HasLinks)
            {
                ShowLinks = false;
            }
            else if (ShowLinks)
            {
                UpdateLinks(true, true);
            }
        }

        /// <summary>
        /// Updates the links.
        /// </summary>
        /// <param name="show">if set to <c>true</c> [show].</param>
        /// <param name="invalidate">if set to <c>true</c> [invalidate].</param>
        /// <param name="force">if set to <c>true</c> [force].</param>
        public void UpdateLinks(bool show, bool invalidate = false, bool force = false)
        {
            _ = new Action(() =>
                  {
                      RaisePropertyChanged(nameof( MinCardinalityWarn));
                      RaisePropertyChanged(nameof( MaxCardinalityWarn));

                      if (Tree.AmlTreeView != null && (_showLinks != show || force))
                      {
                          _showLinks = show;

                          RaisePropertyChanged(nameof( ShowLinks));

                          if (_showLinks)
                          {
                              Tree.AmlTreeView.AddLinksAdorner();
                              MissingLinkConnections.Clear();
                              if (CAEXObject is ExternalInterfaceType ie)
                              {
                                  if (ie.AssociatedObject is SystemUnitClassType)
                                  {
                                      foreach (var il in ie.InternalLinksToInterface())
                                      {
                                          var (from, to) = GetConnectedNodes(il);
                                          if (from == null || to == null)
                                          {
                                              continue;
                                          }

                                          Tree.AmlTreeView.InternalLinksAdorner.Connect(from, to);
                                          if (Equals(from) && ((ExternalInterfaceType)to.CAEXObject)
                                              .InternalLinksToInterface().Take(2).Count() == 1)
                                          {
                                              to.ShowLinks = true;
                                          }
                                          else if (((ExternalInterfaceType)from.CAEXObject).InternalLinksToInterface()
                                              .Take(2).Count() == 1)
                                          {
                                              from.ShowLinks = true;
                                          }
                                      }
                                  }
                              }
                          }
                          else if (Tree.AmlTreeView.InternalLinksAdorner != null)
                          {
                              if (CAEXObject is ExternalInterfaceType ie)
                              {
                                  if (ie.AssociatedObject is SystemUnitClassType)
                                  {
                                      foreach (var il in ie.InternalLinksToInterface())
                                      {
                                          var (from, to) = GetConnectedNodes(il);
                                          if (from != this)
                                          {
                                              from?.CloseLink(this);
                                          }
                                          else if (to != this)
                                          {
                                              to?.CloseLink(this);
                                          }
                                      }

                                      Tree.AmlTreeView.InternalLinksAdorner.DisConnectAll(this);
                                  }
                              }
                          }
                      }

                      if (invalidate && Tree.AmlTreeView?.InternalLinksAdorner != null)
                      {
                          _ = Execute.OnUIThread(
                              () =>
                              {
                                  Tree.AmlTreeView.InternalLinksAdorner.Redraw();
                                  RaisePropertyChanged(nameof( HasMissingLinks));
                              });
                      }

                      RaisePropertyChanged(nameof( ShowLinks));
                  }
            ).OnUIThread();
        }

        #endregion Public Methods

        #region Internal Methods

        internal InternalLinkType GetInternalLink(AMLNodeWithClassReference partner)
        {
            if (CAEXObject is not ExternalInterfaceType ie)
            {
                return null;
            }

            if (ie.AssociatedObject is SystemUnitClassType)
            {
                return ie.InternalLinksToInterface().FirstOrDefault(il => il.AInterface?.Node == CAEXNode &&
                    il.BInterface?.Node == partner.CAEXNode || il.AInterface?.Node == partner.CAEXNode && il.BInterface?.Node == CAEXNode);
            }

            return null;
        }

        internal IEnumerable<ExternalInterfaceType> GetPartners()
        {
            if (CAEXObject is not ExternalInterfaceType ie)
            {
                yield break;
            }

            if (ie.AssociatedObject is not SystemUnitClassType)
            {
                yield break;
            }

            foreach (var il in ie.InternalLinksToInterface())
            {
                if (il.AInterface?.Node == CAEXNode && il.BInterface != null)
                {
                    yield return il.BInterface;
                }
                else if (il.BInterface?.Node == CAEXNode && il.AInterface != null)
                {
                    yield return il.AInterface;
                }
            }
        }

        internal void MissingLink(AMLNodeViewModel missing, bool add)
        {
            if (add && GetPartners().Contains(missing.CAEXObject))
            {
                MissingLinkConnections.Add(missing);
            }
            else
            {
                _ = MissingLinkConnections.Remove(missing);
            }

            RaisePropertyChanged(nameof( MissingLinks));
            RaisePropertyChanged(nameof( HasMissingLinks));
        }

        internal void RefreshPartners()
        {
            foreach (var partner in GetPartners())
            {
                var item = AMLTreeViewModel.FindTreeViewItemInTree(Tree.Root.Children, partner.Node);
                item?.RefreshNodeInformation(false);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        ///     Sets the name of the class path reference attribute, used to get the value
        ///     for the <see cref="ClassReference" />.
        /// </summary>
        /// <param name="node">The node.</param>
        private static void SetClassPathReferenceAttribute(AMLNodeWithClassReference node)
        {
            node.ClassPathReferenceAttribute = node.CAEXNode.Name.LocalName switch
            {
                CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING => CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH,
                CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING => node.CAEXNode.CAEXSchema() == CAEXDocument.CAEXSchema.CAEX2_15
? ""
: CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFATTRIBUTETYPE,
                CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING => CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFATTRIBUTETYPE,
                CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING or CAEX_CLASSModel_TagNames.ROLECLASS_STRING or CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING or CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING => CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH,
                _ => "",
            };
        }

        private (AMLNodeWithClassReference From, AMLNodeWithClassReference To) GetConnectedNodes(InternalLinkType il)
        {
            return il.AInterface?.Node == CAEXNode
                ? (this, Tree.SelectCaexNode(il.BInterface?.Node, false, false, true) as AMLNodeWithClassReference)
                : (Tree.SelectCaexNode(il.AInterface?.Node, false, false, true) as AMLNodeWithClassReference, this);
        }

        #endregion Private Methods
    }
}