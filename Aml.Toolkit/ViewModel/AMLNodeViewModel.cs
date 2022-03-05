// ***********************************************************************
// Assembly         : Aml.Toolkit.ViewModel
// Author           : Josef Prinz
// Created          : 03-10-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 07-23-2015
// ***********************************************************************
// <copyright file="AMLNodeViewModel.cs" company="inpro">
//     Copyright © inpro 2015
// </copyright>
// <summary></summary>
// ***********************************************************************

using Aml.Editor.Plugin.Contracts;
using Aml.Engine.CAEX;
using Aml.Engine.CAEX.Extensions;
using Aml.Engine.Services;
using Aml.Engine.Services.Interfaces;
using Aml.Engine.Xml.Extensions;
using Aml.Toolkit.Tools;
using Aml.Toolkit.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    ///     Class AMLNodeViewModel is the Base-ViewModel for CAEX-Elements in an
    ///     AutomationML Document. The ViewModel supports LazyLoading. If a Node has not
    ///     yet been expanded to view the Children, a DummyNode exists to mark the Node as
    ///     a Node with Children. The DummyNode will be replaced by the Nodes Children, if
    ///     it is expanded for the first time.
    /// </summary>
    public class AMLNodeViewModel : AMLNodeBaseViewModel, ITreeNode
    {
        #region Protected Fields

        /// <summary>
        /// The children
        /// </summary>
        protected ObservableCollection<AMLNodeViewModel> _children;

        #endregion Protected Fields

        #region Private Fields
        /// <summary>
        /// 
        /// </summary>
        private const short TOOLKIT = 1121;

        /// <summary>
        /// Empty list and view shared by all nodes that don't have children
        /// </summary>
        protected static readonly List<AMLNodeViewModel> _emptyChildren;

        private static readonly GroupComparer _groupComparer = new();

        /// <summary>
        /// List with a Dummy Child, shared by all nodes that have 
        /// children but have not been expanded
        /// </summary>
        protected static readonly List<AMLNodeViewModel> _lazyLoadChildrenWithDummy;

        /// <summary>
        ///     The dummy child
        /// </summary>
        private static readonly AMLNodeViewModel DummyChild = new AMLExpandableDummyNode(null, null, false)
        { IsVisible = false };


        /// <summary>
        ///     <see cref="AdditionalInformation" />
        /// </summary>
        private object _additionalInformation;

        /// <summary>
        ///     <see cref="AdditionalInformationDescription" />
        /// </summary>
        private string _additionalInformationDescription;

        /// <summary>
        ///     <see cref="BorderColor" />
        /// </summary>
        private Brush _borderColor = Brushes.LightSteelBlue;

        /// <summary>
        ///     <see cref="CAEXTagNames" />
        /// </summary>
        private HashSet<string> _caexTagNames;

        private ObservableCollection<AMLNodeCommand> _commands;

        /// <summary>
        ///     <see cref="Description" />
        /// </summary>
        private string _description;

        /// <summary>
        ///     <see cref="ExpandAllCommand" />
        /// </summary>
        private ICommand _expandAllCommand;

        /// <summary>
        ///     <see cref="Extension" />
        /// </summary>
        private object _extension;

        private DelegatingWeakEventListener _filterUpdate;

        /// <summary>
        ///     <see cref="HasChildrenBorder" />
        /// </summary>
        private bool _hasChildrenBorder;

        /// <summary>
        ///     <see cref="HasChildrenOverlay" />
        /// </summary>
        private bool _hasChildrenOverlay;

        /// <summary>
        ///     <see cref="IsInEditMode" />
        /// </summary>
        private bool _isInEditMode;

        /// <summary>
        ///     <see cref="IsMarked" />
        /// </summary>
        private bool _isMarked;

        /// <summary>
        ///     <see cref="MappedValue" />
        /// </summary>
        private double _mappedValue;

        /// <summary>
        ///     <see cref="Name" />
        /// </summary>
        private string _name;

        /// <summary>
        ///     <see cref="OverlayColor" />
        /// </summary>
        private Brush _overlayColor = Brushes.LightYellow;

        private ITreeNode[] _selectPathItem;

        /// <summary>
        ///  <see cref="ShowInheritance"/>
        /// </summary>
        private bool _showInheritance;

        ///// <summary>
        /////    <see cref="NumberOfVisualDescendants"/>
        ///// </summary>
        //private int _numberOfVisualDescendants = 0;
        /// <summary>
        ///     <see cref="ShowToolTip" />
        /// </summary>
        private bool _showToolTip;

        /// <summary>
        ///     <see cref="ToolTip" />
        /// </summary>
        private object _toolTip;

        private AMLTreeViewModel _tree;
        private RelayCommand<object> _collapseAllCommand;
        private bool _isVerified;
        private bool _isNotVerified;
        private bool _showMirrorData;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// 
        /// </summary>
        static AMLNodeViewModel()
        {
            _emptyChildren = new List<AMLNodeViewModel>();
            _lazyLoadChildrenWithDummy = new List<AMLNodeViewModel>
            {
                DummyChild
            };

           
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMLNodeViewModel" /> class.
        /// </summary>
        /// <param name="tree">            The TreeViewModel, containing the node</param>
        /// <param name="parent">          The parent.</param>
        /// <param name="CaexNode">        The CAEX node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeViewModel(AMLTreeViewModel tree, AMLNodeViewModel parent, XElement CaexNode,
            bool lazyLoadChildren)
        {
            Parent = parent;
            CAEXNode = CaexNode;
            //_SelectPathItem =
            //    _parent!= null ? _parent.SelectPathItem.ToList().Concat((this.Yield())).ToArray()
            //            : new ITreeNode[]{this};

            if (CAEXNode != null)
            {
                ToolTip = CAEXNode.Name;

                //if (CAEXObject is IDigitalSigned)
                //{
                //    CAEXSignature.SignatureVerified += VerificationChanged;
                //}
            }

            Tree = tree;
            if (tree != null && IsMirror )
            {
                _showMirrorData = tree.TreeViewLayout.ShowMirrorTree;
            }

            lazyLoadChildren = lazyLoadChildren || HasChilds;

            //Console.WriteLine("Culture of {0} in application domain {1}: {2}",
            //            Thread.CurrentThread.Name,
            //            AppDomain.CurrentDomain.FriendlyName,
            //            CultureInfo.CurrentCulture.Name);

            Commands.Add(new AMLNodeCommand(ExpandAllCommand)
            {
                Name = "Expand all",
                Identifier = TOOLKIT,
                IsEnabled = true
            });


            Commands.Add(new AMLNodeCommand(CollapseAllCommand)
            {
                Name = "Collapse all",
                Identifier = TOOLKIT,
                IsEnabled = true
            });



            EnabledCommands.Filter = o =>
            {
                if (o is AMLNodeCommand cmd)
                {
                    return cmd.IsEnabled;
                }

                return false;
            };

            _childrenCollection.Source = lazyLoadChildren ? _lazyLoadChildrenWithDummy : _emptyChildren;

            //this.Commands.CollectionChanged += Commands_CollectionChanged;
            _mappedValue = 1;

            
        }


        private void VerificationChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged (nameof(IsVerified));
            RaisePropertyChanged (nameof(IsVerified));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMLNodeViewModel" /> class.
        /// </summary>
        /// <param name="parent">          The parent.</param>
        /// <param name="caexNode">        The CAEX node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeViewModel(AMLNodeViewModel parent, XElement caexNode, bool lazyLoadChildren)
            : this(parent?.Tree, parent, caexNode, lazyLoadChildren)
        {
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        ///     This is used to create the DummyChild instance. Prevents a default instance
        ///     of the <see cref="AMLNodeViewModel" /> class from being created.
        /// </summary>
        private AMLNodeViewModel()
        {
            _mappedValue = 0;
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets the additional information which will be visible in the View
        ///     of the Node. The additional Information is always placed between the Node's
        ///     Icon and the Name. Overwrite this in a derived class, to define specific
        ///     representation logic. Currently the <see cref="View.AMLTreeView" /> supports Text and Image Sources.
        ///     <example>
        ///         <code>
        ///
        ///                           // display additional text
        ///                           AdditionalInformation = "Error";
        ///       </code>
        ///     </example>
        /// </summary>
        /// <value>The additional information.</value>
        public virtual object AdditionalInformation
        {
            get => _additionalInformation;
            set => Set (ref _additionalInformation, value);
        }

        /// <summary>
        ///     Gets and sets the AdditionalInformationDescription
        /// </summary>
        public virtual string AdditionalInformationDescription
        {
            get => _additionalInformationDescription;
            set => Set(ref _additionalInformationDescription, value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a role reference.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is role reference; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsRoleReference => false;

        /// <summary>
        ///     Gets and sets the BorderColor
        /// </summary>
        public Brush BorderColor
        {
            get => _borderColor;
            set => Set(ref _borderColor, value, nameof(BorderColor));
        }

        /// <summary>
        ///     Gets the CAEXNode
        /// </summary>
        /// <value>The caex node.</value>
        public XElement CAEXNode { get; }

        /// <summary>
        ///     Gets the CAEX object.
        /// </summary>
        public CAEXWrapper CAEXObject => CAEXNode?.CreateCAEXWrapper();

        /// <summary>
        ///     Gets and sets the List of CAEXTagNames for valid child's, which are loaded
        ///     to the TreeView. If this is not set for a Node, the List of the actual
        ///     parent is used.
        /// </summary>
        /// <value>The name of the caex tag.</value>
        public HashSet<string> CAEXTagNames
        {
            get => _caexTagNames ?? Parent?.CAEXTagNames;
            protected set => Set(ref _caexTagNames, value, nameof(CAEXTagNames));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can navigate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can navigate; otherwise, <c>false</c>.
        /// </value>
        public bool CanNavigate { get; set; }

        /// <summary>
        ///     Returns the loaded children of this object.
        /// </summary>
        /// <value>The children.</value>
        public ObservableCollection<AMLNodeViewModel> Children => _children ??
                                                                  new ObservableCollection<AMLNodeViewModel>();
        /// <summary>
        /// 
        /// </summary>
        IList<ITreeNode> ITreeNode.Children => Children as IList<ITreeNode>;


        /// <summary>
        ///     The Collection of Commands, which may be bound to the Context-Menu of any
        ///     AMLNode in the Tree. The Context-Menu will contain only the subset of
        ///     enabled commands of this Collection. For that, the CollectionView
        ///     <see cref="EnabledCommands" /> is defined, which has an active Filter for Enabled
        ///     Commands only.
        /// </summary>
        public ObservableCollection<AMLNodeCommand> Commands =>
            _commands ??= new ObservableCollection<AMLNodeCommand>();

        /// <summary>
        ///     Gets and sets the Description
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value, nameof(Description));
        }

        /// <summary>
        /// </summary>
        public ListCollectionView EnabledCommands =>
            (ListCollectionView)CollectionViewSource.GetDefaultView(_commands);

        /// <summary>
        ///     The ExpandAllCommand - Command. Execution of this Command results in the
        ///     Expansion of the Node and all it's descendants. The Execution method is
        ///     queued on the current Dispatcher Thread for asynchronous Execution.
        /// </summary>
        /// <value>The expand all command.</value>
        public ICommand ExpandAllCommand => _expandAllCommand ??= new RelayCommand<object>(ExpandAllCommandExecute,
            ExpandAllCommandCanExecute);

        /// <summary>
        /// Previously expanded nodes are collapsed an unloaded
        /// </summary>
        public ICommand CollapseAllCommand => _collapseAllCommand ??= new RelayCommand<object>(CollapseAllCommandExecute,
            CollapseAllCommandCanExecute);

        /// <summary>
        ///     Gets and sets the Extension which can be used to add additional properties for customized visualizations
        /// </summary>
        public object Extension
        {
            get => _extension;
            set => Set(ref _extension, value, nameof(Extension));
        }

        //(this._expandAllCommand = AsyncCommand.Create(token => ExpandAllCommandExecute(token), ExpandAllCommandCanExecute) );
        /// <summary>
        ///     Gets and sets the HasChildrenBorder
        /// </summary>
        public bool HasChildrenBorder
        {
            get => _hasChildrenBorder;
            set => Set(ref _hasChildrenBorder, value, nameof(HasChildrenBorder));
        }

        /// <summary>
        ///     Gets and sets the HasChildrenOverlay
        /// </summary>
        public bool HasChildrenOverlay
        {
            get => _hasChildrenOverlay;
            set => Set(ref _hasChildrenOverlay, value, nameof(HasChildrenOverlay));
        }

        /// <summary>
        ///     Returns true if this object's Children have not yet been populated.
        /// </summary>
        /// <value><c>true</c> if this instance has dummy child; otherwise, <c>false</c>.</value>
        public bool HasDummyChild => ReferenceEquals(_childrenCollection.Source, _lazyLoadChildrenWithDummy);

        /// <summary>
        /// Gets a value indicating whether this instance has new version.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has new version; otherwise, <c>false</c>.
        /// </value>
        public bool HasNewVersion => CAEXObject is CAEXObject caex && caex.Revision.Exists &&
                                     caex.Revision.Any(r => !string.IsNullOrEmpty(r.NewVersion));

        /// <summary>
        /// Gets a value indicating whether this instance has old version.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has old version; otherwise, <c>false</c>.
        /// </value>
        public bool HasOldVersion => !HasNewVersion && CAEXObject is CAEXObject caex && caex.Revision.Exists &&
                                     caex.Revision.Any(r => !string.IsNullOrEmpty(r.OldVersion));

        /// <summary>
        /// Gets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted => (CAEXObject as CAEXBasicObject)?.ChangeMode == ChangeMode.Delete;


        /// <summary>
        /// Gets a value indicating whether this instance is verified.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerified => Tree?.GetVerificationState(CAEXObject as CAEXObject) == true;
            

        /// <summary>
        /// Gets a value indicating whether this instance is verified.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotVerified => Tree?.GetVerificationState(CAEXObject as CAEXObject) == false;

        /// <summary>
        /// Gets a value indicating whether this instance is derived.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is derived; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDerived => false;

        /// <summary>
        ///     Gets/sets whether the TreeViewItem associated with this object is expanded.
        /// </summary>
        /// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
        public override bool IsExpanded
        {
            get => base.IsExpanded;
            set
            {
                if (value == base.IsExpanded)
                {
                    return;
                }

                _ = Expand(value);
                RaisePropertyChanged(nameof( IsExpanded));
                //RaisePropertyChanged(nameof( ChildrenView);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is facetted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is facetted; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsFacetted => false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filtered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is filtered; otherwise, <c>false</c>.
        /// </value>
        public bool IsFiltered { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance represents a group.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance represents a group; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsGroup => false;

        /// <summary>
        ///     Gets and sets the IsInEditMode
        /// </summary>
        public bool IsInEditMode
        {
            get => _isInEditMode;

            set
            {
                if (_isInEditMode == value)
                {
                    return;
                }

                _isInEditMode = value;
                RaisePropertyChanged(nameof( IsInEditMode));

                if (!_isInEditMode)
                {
                    Tree.Focused = true;
                }
            }
        }

        /// <summary>
        ///     Gets and sets the IsMarked
        /// </summary>
        public bool IsMarked
        {
            get => _isMarked;
            set
            {
                Set(ref _isMarked, value);
                if (value && IsSelected)
                {
                    IsSelected = false;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is master.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is master; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsMaster => false;

        /// <summary>
        ///     Gets a value indicating whether this instance is mirror.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is mirror; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsMirror => false;

        /// <summary>
        ///     Gets a value indicating whether this instance is overridden.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is overridden; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsOverridden => false;

        /// <summary>
        ///     Gets a value indicating whether this instance is port.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is port; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsPort => false;

        /// <summary>
        ///     Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsReadonly => IsDerived || IsFacetted ;

        /// <summary>
        ///   Gets a value indicating whether this instance is a mirrored item.
        /// </summary>
        public bool IsMirrored => (Parent != null) && (Parent.IsMirror || Parent.IsMirrored);

        /// <summary>
        ///     Gets and sets the MappedValue
        /// </summary>
        public virtual double MappedValue
        {
            get => !IsVisible || IsFiltered ? 0 : _mappedValue;

            set
            {
                if (_mappedValue == value)
                {
                    return;
                }

                _mappedValue = value;
                RaisePropertyChanged(nameof( MappedValue));
            }
        }

        /// <summary>
        ///     Gets and sets the Name
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get => CAEXNode?.Attribute("Name")?.Value ?? CAEXNode?.Name.LocalName;
            set
            {
                if (_name == value)
                {
                    return;
                }

                if (IsInEditMode && CAEXObject is CAEXObject caex)
                {
                    var started =
                        ServiceLocator.UndoRedoService?.BeginTransaction(caex.CAEXDocument(), "Change name") ?? false;
                    caex.Name = value;
                    if (started)
                    {
                        _ = (ServiceLocator.UndoRedoService?.EndTransaction(caex.CAEXDocument()));
                    }
                }

                _name = value;
                RaisePropertyChanged(nameof( Name));
            }
        }

        /// <summary>
        ///     Gets the Index of the node.
        /// </summary>
        /// <value>The index of the node.</value>
        public int NodeIndex { get; private set; }

        /// <summary>
        ///     Gets and sets the OverlayColor
        /// </summary>
        public Brush OverlayColor
        {
            get => _overlayColor;
            set => Set(ref _overlayColor, value, nameof(OverlayColor));
        }

        /// <summary>
        ///     Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public AMLNodeViewModel Parent { get; internal set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        ITreeNode ITreeNode.Parent => Parent;

        /// <summary>
        /// Gets or sets the select path item.
        /// </summary>
        /// <value>
        /// The select path item.
        /// </value>
        public ITreeNode[] SelectPathItem
        {
            get => _selectPathItem;
            set => Set(ref _selectPathItem, value, nameof(SelectPathItem));
        }

        /// <summary>
        ///  Gets and sets the ShowInheritance
        /// </summary>
        public bool ShowInheritance
        {
            get => _showInheritance;

            set
            {
                if (Set(ref _showInheritance, value, nameof(ShowInheritance)))
                {
                    RefreshTree(true);
                }
            }
        }

         /// <summary>
        ///  Gets and sets the ShowInheritance
        /// </summary>
        public bool ShowMirrorData
        {
            get => _showMirrorData;

            set
            {
                if (Set(ref _showMirrorData, value, nameof(ShowMirrorData)))
                {
                    RefreshTree(true);
                }
            }
        }

        /// <summary>
        ///     Gets and sets the ShowToolTip, if set to true, the <see cref="ToolTip" /> will be shown on the node
        /// </summary>
        public bool ShowToolTip
        {
            get => _showToolTip;
            set => Set(ref _showToolTip, value, nameof(ShowToolTip));
        }

        /// <summary>
        ///     Gets and sets a ToolTip object for a Node. The ToolTip will not be shown on a node,
        ///     if the Property <see cref="ShowToolTip" /> is set to false.
        /// </summary>
        public object ToolTip
        {
            get => _toolTip;
            set => Set(ref _toolTip, value);
        }

        /// <summary>
        ///     Gets or sets the tree, containing the node
        /// </summary>
        /// <value>The tree.</value>
        public AMLTreeViewModel Tree
        {
            get => _tree;

            set
            {
                //if (_tree != value)
                //{
                //    if (_tree != null)
                //        _tree.NodeFilters.FilterUpdate -= NodeFilters_FilterUpdate;

                //    _tree = value;

                //    if (_tree != null)
                //        _tree.NodeFilters.FilterUpdate += NodeFilters_FilterUpdate;
                //}

                if (_tree == value)
                {
                    return;
                }

                if (_filterUpdate != null && _tree != null)
                {
                    GenericWeakEventManager.RemoveListener(
                        _tree.NodeFilters,
                        "FilterUpdate",
                        _filterUpdate);
                }

                _tree = value;

                if (_tree != null)
                {
                    _filterUpdate =
                        new DelegatingWeakEventListener((EventHandler<FilterEvent>)NodeFilters_FilterUpdate);

                    GenericWeakEventManager.AddListener(
                        _tree.NodeFilters,
                        "FilterUpdate",
                        _filterUpdate);
                }
            }
        }

        /// <summary>
        ///     Gets the TreeView item.
        /// </summary>
        /// <value>
        ///     The TreeView item.
        /// </value>
        public TreeViewItem TreeViewItem
        {
            get
            {
                if (!Tree.NodeFilters.Filter(this))
                {
                    return null;
                }

                if (Tree.View == null)
                {
                    return null;
                }

                ItemsControl tv = Tree.AmlTreeView?.TheTreeView;
                if (tv == null)
                {
                    return null;
                }

                var nodeList = GetAncestors(this).ToList();
                for (var i = 2; i < nodeList.Count; i++)
                {
                    if (tv.ItemContainerGenerator.ContainerFromItem(nodeList[i]) is not ItemsControl itv)
                    {
                        continue;
                    }

                    tv = itv;
                }

                return tv.DataContext == this ? tv as TreeViewItem : null;
            }
        }

        /// <summary>
        /// Gets the virtual children.
        /// </summary>
        /// <value>
        /// The virtual children.
        /// </value>
        public IEnumerable<AMLNodeViewModel> VirtualChildren
        {
            get
            {
                if (LoadedChildren == null)
                {
                    yield break;
                }

                for (var index = 0; index < LoadedChildren.Count; index++)
                {
                    var child = LoadedChildren[index];
                    if (child is AMLNodeGroupViewModel)
                    {
                        if (child.LoadedChildren == null)
                        {
                            continue;
                        }

                        for (var i = 0; i < child.LoadedChildren.Count; i++)
                        {
                            yield return child.LoadedChildren[i];
                        }
                    }
                    else
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the visible children.
        /// </summary>
        /// <value>
        ///     The visible children.
        /// </value>
        public IEnumerable<AMLNodeViewModel> VisibleChildren =>
            LoadedChildren?.Where(n => n.IsVisible) ?? Enumerable.Empty<AMLNodeViewModel>();

        /// <summary>
        ///     Gets the index of the visible node.
        /// </summary>
        /// <value>
        ///     The index of the visible node.
        /// </value>
        public int VisibleNodeIndex
        {
            get
            {
                if (Parent == null)
                {
                    return NodeIndex;
                }

                var index = 0;
                foreach (var node in Parent.VisibleChildren)
                {
                    if (node == this)
                    {
                        return index;
                    }

                    index++;
                }

                return NodeIndex;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// 
        /// </summary>
        internal IList<AMLNodeViewModel> LoadedChildren => _childrenCollection.Source as IList<AMLNodeViewModel>;


        public AMLNodeViewModel MasterElement => (Parent != null && Parent.IsMaster) ? Parent : Parent?.MasterElement ?? null;

        public AMLNodeViewModel MirrorElement => (Parent != null && Parent.IsMirror) ? Parent : Parent?.MirrorElement ?? null;


        public CAEXObject Master => ( CAEXObject is IMirror mirror ) ? mirror.Master : null;

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        ///     Adds the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="raiseEvent">determines if a property changed event is raised</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddNode(AMLNodeViewModel node, bool raiseEvent = true)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (HasDummyChild || _children == null)
            {
                _children = new ObservableCollection<AMLNodeViewModel>();
                _childrenCollection.Source = _children;

                // RaisePropertyChanged(nameof( ChildrenView));
            }

            Children.Add(node);
            node.IsVisible = node.IsVisible || Tree.NodeFilters.Filter(node);

            if (raiseEvent)
            {
                RaisePropertyChanged(nameof( ChildrenView));
            }
        }

        /// <summary>
        /// Brings the node item into view if it is not in the visible area
        /// </summary>
        public void BringIntoView()
        {
            Tree?.Tree.AmlTreeView?.MarkNode(this);
            // RaisePropertyChanged(nameof( IsMarked);
        }

        /// <summary>
        /// Expands the node.
        /// </summary>
        /// <param name="expand">if set to <c>true</c> [expand].</param>
        /// <param name="raiseEvent">if set to <c>true</c> raise an event.</param>
        public void ExpandNode(bool expand, bool raiseEvent = true)
        {
            if (expand == base.IsExpanded)
            {
                return;
            }

            _ = Expand(expand, raiseEvent);

            if (!raiseEvent)
            {
                return;
            }

            RaisePropertyChanged(nameof( IsExpanded));
            //RaisePropertyChanged(nameof( ChildrenView);
        }


        /// <summary>
        ///     Invoked when the child items need to be loaded on demand. Subclasses can
        ///     override this to populate the Children collection.
        /// </summary>
        public override void LoadChildren(bool raise = true)
        {
            LoadChildren(Tree.ModelChilds(this), raise);
        }

        /// <summary>
        /// Loads the children not virtual.
        /// </summary>
        /// <returns></returns>
        public IList<AMLNodeViewModel> LoadedChildrenNotVirtual()
        {
            if (LoadedChildren == null)
            {
                return new List<AMLNodeViewModel>();
            }

            return LoadedChildren.Any(x => x is AMLNodeGroupViewModel) ? VirtualChildren.ToList() : LoadedChildren;
        }

        /// <summary>
        /// Refreshes the child node information.
        /// </summary>
        /// <param name="expand">if set to <c>true</c> [expand].</param>
        public void RefreshChildNodeInformation(bool expand)
        {
            var list = LoadedChildrenNotVirtual();
            for (var index = 0; index < list.Count; index++)
            {
                var child = list[index];
                child.RefreshNodeInformation(expand);
                child.RefreshChildNodeInformation(expand);
            }
        }

        /// <summary>
        ///     Refreshes the node information.
        /// </summary>
        /// <param name="expand">if set to <c>true</c> [expand].</param>
        public virtual void RefreshNodeInformation(bool expand)
        {
            RaisePropertyChanged(nameof( Name));
            RaisePropertyChanged(nameof( IsDeleted));
            RaisePropertyChanged(nameof( AdditionalInformation));
            RaisePropertyChanged(nameof( IsReadonly));
            RaisePropertyChanged(nameof( HasNewVersion));
            RaisePropertyChanged(nameof( HasOldVersion));
            
            RaisePropertyChanged(nameof( IsVerified));
            RaisePropertyChanged(nameof( IsNotVerified));
            RaisePropertyChanged(nameof( IsMirrored));
            if (HasNewVersion)
            {
                Description = "A new version exists";
            }

            if (expand)
            {
                RefreshTree(expand);
            }
        }

        /// <summary>
        ///     Refreshes the node information. This Method can be overridden in derived
        ///     classes. The Method should be called, if the CAEX-Elements Data has changed
        ///     and the Changes should be visible in any View, that has a binding to this ViewModel.
        /// </summary>
        public void RefreshTree(bool expand, bool refreshChild=false)
        {
            var treeChanged = false;
            var children = LoadedChildrenNotVirtual();

            if (CAEXNode == null)
            {
                return;
            }

            // if the children are loaded
            if (!HasDummyChild)
            {
                if (!expand)
                {
                    Tree.ExpandedLinkNodes = Tree.ExpandedLinks();
                }

                var modelChilds = Tree.ModelChilds(this).ToList();
                var obsoletes = children.Where(c => !modelChilds.Contains(c.CAEXNode)).ToList();

                foreach (var obsoleteChild in obsoletes)
                {
                    RemoveNode(obsoleteChild);
                    treeChanged = true;
                }

                if (treeChanged)
                {
                    children = RenumberedList();
                }

                for (var i = 0; i < modelChilds.Count; i++)
                {
                    var loadedChild = children.FirstOrDefault(item => item.CAEXNode.Equals(modelChilds[i]));
                    if (loadedChild == null)
                    {
                        AMLNodeViewModel item;

                        var groupNode = GroupNode(modelChilds[i].Name.LocalName);
                        if (groupNode != null)
                        {
                            if (!Children.Contains(groupNode))
                            {
                                if (i == 0)
                                {
                                    InsertNode(0, groupNode);
                                }
                                else
                                {
                                    InsertAfter(modelChilds[i - 1], groupNode);
                                }

                                groupNode.IsExpanded = true;
                                treeChanged = true;
                                children = RenumberedList();
                                if (children.Count > 0)
                                    continue;
                            }

                            groupNode.IsVisible = groupNode.IsVisibleInLayout;
                            item = groupNode.CreateNode(modelChilds[i]);
                        }
                        else
                        {
                            item = CreateNode(modelChilds[i]);
                        }

                        if (item == null)
                        {
                            continue;
                        }

                        if (i == 0)
                        {
                            if (groupNode != null && groupNode.IsVisible)
                            {
                                groupNode.InsertNode(0, item);
                            }
                            else
                            {
                                InsertNode(0, item);
                            }
                        }
                        else if (groupNode != null && groupNode.IsVisible)
                        {
                            groupNode.InsertAfter(modelChilds[i - 1], item);
                        }
                        else
                        {
                            InsertAfter(modelChilds[i - 1], item);
                        }

                        item.IsVisible = Tree.NodeFilters.Filter(item);

                        if (expand && item.HasChilds)
                        {
                            item.IsExpanded = true;
                        }

                        children = RenumberedList();
                        treeChanged = true;
                    }
                    else if (loadedChild.NodeIndex != i && i < children.Count)
                    {
                        // the loaded child is at the wrong position in the tree
                        // get the item which is at position "i"

                        var itemAtIndex = children[i];
                        var collection = loadedChild.Parent.Children;
                        var itemIndex = collection.IndexOf(itemAtIndex);
                        var loadedIndex = collection.IndexOf(loadedChild);

                        collection.Move(itemIndex, loadedIndex);

                        children = RenumberedList();
                        treeChanged = true;
                    }
                    else if (refreshChild )
                    {
                        loadedChild.RefreshNodeInformation(expand); 
                    }
                }

                if (expand)
                {
                    if (Tree.ExpandedLinkNodes?.Count > 0)
                    {
                        RefreshExpandedLinks(Tree.ExpandedLinkNodes);
                    }

                    Tree.ExpandedLinkNodes = null;
                }
            }
            //}

            if (treeChanged)
            {
                RaisePropertyChanged(nameof( ChildrenView));
                RefreshNodeInformation(false);
            }

            if (expand)
            {
                IsExpanded = true;
            }

            RaisePropertyChanged(nameof( HasDummyChild));

            var cmd = Commands.FirstOrDefault(c => c.Identifier == TOOLKIT);
            if (cmd != null)
            {
                cmd.IsEnabled = HasDummyChild || HasChilds;
            }

            var selectedElements = Tree.SelectedElements.ToList();
            for (var index = 0; index < selectedElements.Count; index++)
            {
                var node = Tree.SelectedElements.ElementAt(index);
                if (node?.CAEXNode?.Document == null)
                {
                    _ = Tree.SelectedElements.Remove(node);
                }
            }
        }

        /// <summary>
        /// Removes the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void RemoveNode(AMLNodeViewModel node)
        {
            if (node.IsSelected)
            {
                _ = Tree.SelectedElements.Remove(node);
                node.IsSelected = false;
            }

            _ = node.Parent.Children.Remove(node);
            if (node.Parent is AMLNodeGroupViewModel && node.Parent.Children.Count == 0)
            {
                node.Parent.IsVisible = false;
            }

            Tree.AmlTreeView?.InternalLinksAdorner?.RemoveVertex(node);

            if (!node.Parent.HasChilds)
            {
                //node.Parent._childrenCollection.Source = _emptyChildren;
                Children.Clear();
            }

            RaisePropertyChanged(nameof( HasDummyChild));
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        ///     Gets the group node from the children of this instance which groups elements with the provided element name. If no
        ///     such group node exists,
        ///     it returns <c>null</c>.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal AMLNodeGroupViewModel GetGroupNode(string elementName)
        {
            return Children?.OfType<AMLNodeGroupViewModel>().FirstOrDefault(g => g.ElementNames.Contains(elementName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node"></param>
        internal void InsertNode(int index, AMLNodeViewModel node)
        {
            if (HasDummyChild || _children == null)
            {
                _children = new ObservableCollection<AMLNodeViewModel>();
                _childrenCollection.Source = _children;

                RaisePropertyChanged(nameof( ChildrenView));
            }

            Children.Insert(index, node);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RefreshTreeNodes()
        {
            RefreshNodeInformation(false);
            foreach (var node in Children)
                node.RefreshTreeNodes();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RefreshHierarchy()
        {
            RefreshTree(false);
            foreach (var node in Children)
            {
                if (node is not AMLNodeGroupViewModel)
                {
                    node.RefreshHierarchy();
                }
            }
        }

        #endregion Internal Methods

        #region Protected Methods

        /// <summary>
        ///     Creates a new node for this element
        /// </summary>
        /// <param name="XElement">The XML element.</param>
        /// <param name="useGroup"></param>
        /// <returns></returns>
        protected AMLNodeViewModel CreateNode(XElement XElement, bool useGroup = true)
        {
            var constructor = AMLNodeRegistry.Instance[XElement.Name.LocalName];
            AMLNodeViewModel nodeViewModel;

            // add a special group node
            if (useGroup && this is not AMLNodeGroupViewModel &&
                CAEXNode?.Name != XElement.Name &&
                !string.IsNullOrEmpty(AMLNodeGroupViewModel.GroupingPropertyname(XElement.Name.LocalName)))
            {
                // if already exists, only one is required
                var groupNode = GetGroupNode(XElement.Name.LocalName);
                if (groupNode != null)
                {
                    return null;
                }

                nodeViewModel = AMLNodeGroupViewModel.Create(this,
                    AMLNodeGroupViewModel.GroupingPropertyname(XElement.Name.LocalName));
            }
            else
            {
                nodeViewModel = constructor.ParameterCount switch
                {
                    4 => constructor.Creator(_tree, this, XElement, false),
                    3 => constructor.Creator(this, XElement, false),
                    _ => null
                };
            }

            //if (this is AMLNodeWithClassAndRoleReference node)
            //{
            //    node.ChildrenView.Refresh();
            //}

            if (this is not AMLNodeGroupViewModel || IsVisible)
            {
                return nodeViewModel;
            }

            if (nodeViewModel != null)
            {
                nodeViewModel.Parent = Parent;
            }

            return nodeViewModel;
        }

        ///// <summary>
        /////    Counts the ordinal number.
        ///// </summary>
        ///// <param name="nodeIndex">Index of the node.</param>
        //protected void CountOrdinalNumber(int nodeIndex)
        //{
        //    //RaisePropertyChanged(nameof( OrdinalNumber);
        //    //while (nodeIndex < LoadedChildren.Count)
        //    //{
        //    //    var node = Children[nodeIndex++];
        //    //    if (!node.HasDummyChild)
        //    //          node.CountOrdinalNumber(0);
        //    //}
        //}

        /// <summary>
        /// Expands all children.
        /// </summary>
        protected virtual void ExpandAllChildren()
        {
            var isChanged = Expand(true, false);

            if (LoadedChildren != null)
            {
                for (var index = 0; index < LoadedChildren.Count; index++)
                {
                    LoadedChildren[index].ExpandAllChildren();
                }
            }

            if (!isChanged)
            {
                return;
            }

            RaisePropertyChanged(nameof(IsExpanded));
            RaisePropertyChanged(nameof(ChildrenView));
        }

        /// <summary>
        /// Loads the children.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <param name="raise">if set to <c>true</c> [raise].</param>
        protected void LoadChildren(IEnumerable<XElement> children, bool raise = true)
        {
            var nodeIndex = 0;
            var creationQuery = children.Select(n => CreateViewModel(n, ref nodeIndex)).Distinct(_groupComparer);

            _children = new ObservableCollection<AMLNodeViewModel>(creationQuery);
            _childrenCollection.Source = _children;

            foreach (var virtualNode in Children.OfType<AMLNodeGroupViewModel>().ToList())
            {
                virtualNode.ExpandNode(true, raise);
            }

            if (raise)
            {
                RaisePropertyChanged(nameof( ChildrenView));
            }
        }

        #endregion Protected Methods

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static IEnumerable<AMLNodeViewModel> GetAncestors(AMLNodeViewModel node)
        {
            var list = new List<AMLNodeViewModel> { node };
            return node.Parent != null ? GetAncestors(node.Parent).Concat(list) : list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private AMLNodeViewModel CreateViewModel(XElement node, ref int index)
        {
            var vm = CreateNode(node);
            //vm.IsFiltered = true;
            vm.Tree = Tree;
            vm.NodeIndex = index++;
            //vm.IsVisible = vm.IsVisible && Tree.NodeFilters.Filter(vm);
            //vm.IsFiltered = false;
            return vm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expand"></param>
        /// <param name="raise"></param>
        /// <returns></returns>
        private bool Expand(bool expand, bool raise = true)
        {
            if (base.IsExpanded == expand)
            {
                return false;
            }

            _isExpanded = expand;
            // Lazy load the child items, if necessary.
            if (HasDummyChild && base.IsExpanded)
            {
                LoadChildren(raise);
            }

            if (raise)
            {
                RaisePropertyChanged(nameof(IsExpanded));
            }

            return true;
        }

        /// <summary>
        ///     Test, if the <see cref="ExpandAllCommand" /> can execute.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        /// <returns>true, if command can execute</returns>
        private bool ExpandAllCommandCanExecute(object parameter)
        {
            return HasDummyChild || LoadedChildren?.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CollapseAllCommandCanExecute(object parameter)
        {
            return (CAEXObject is not CAEXFileType) && LoadedChildren?.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void CollapseAllCommandExecute(object parameter)
        {
            LoadedChildren.Clear();
            if (HasChilds)
            {
                _childrenCollection.Source = _lazyLoadChildrenWithDummy;
            }


            IsExpanded = false;

            RaisePropertyChanged(nameof(HasDummyChild));
            RaisePropertyChanged(nameof(ChildrenView));
            RaisePropertyChanged(nameof(Children));
            RaisePropertyChanged(nameof(LoadedChildren));
            RaisePropertyChanged(nameof(IsVisible));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void ExpandAllCommandExecute(object parameter /*CancellationToken token = new CancellationToken()*/)
        {
            _ = ThreadPool.QueueUserWorkItem(delegate
              {
                  _ = Execute.OnUIThread(() =>
                  {
                      var resetUpdater = false;
                      IAutoUpdate updater;
                      if ((updater = ServiceLocator.GetService<IAutoUpdate>()) != null
                          && updater.IsAutoUpdateEnabled)
                      {
                          updater.IsAutoUpdateEnabled = false;
                          resetUpdater = true;
                      }

                      ExpandAllChildren();
                      if (resetUpdater)
                      {
                          updater.IsAutoUpdateEnabled = true;
                      }

                      RaisePropertyChanged(nameof(IsExpanded));
                      RaisePropertyChanged(nameof(ChildrenView));
                  });
              });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HashSet<XElement> ExpandedLinks()
        {
            var expanded = new HashSet<XElement>();
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];
                if (child is AMLNodeInheritable ih && ih.ShowLinks)
                {
                    _ = expanded.Add(child.CAEXNode);
                }

                expanded.UnionWith(child.ExpandedLinks());
            }

            return expanded;
        }

        //_filterUpdate = new DelegatingWeakEventListener((EventHandler<FilterEvent>) NodeFilters_FilterUpdate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private AMLNodeGroupViewModel GroupNode(string elementName)
        {
            if (CAEXNode.Name.LocalName == elementName)
            {
                return null;
            }

            return GetGroupNode(elementName) ??
                   AMLNodeGroupViewModel.Create(this, AMLNodeGroupViewModel.GroupingPropertyname(elementName));
        }

        private bool? _hasChilds = null;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        internal bool HasChilds
        {
            get 
            { 
                if (_hasChilds == null ) 
                {
                    _hasChilds = CAEXTagNames != null && Tree.ModelChilds(this).Any();
                    return _hasChilds.Value;
                }
                return _hasChilds.Value;
            }

            set
            {
                _hasChilds = value;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predecessor"></param>
        /// <param name="successor"></param>
        private void InsertAfter(XElement predecessor, AMLNodeViewModel successor)
        {
            var pos = 0;
            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i].CAEXNode.Equals(predecessor))
                {
                    pos = i + 1;
                    break;
                }

                if (Children[i] is not AMLNodeGroupViewModel groupNode || !Children[i].IsVisible)
                {
                    continue;
                }

                if (!groupNode.ElementNames.Contains(predecessor.Name.LocalName))
                {
                    continue;
                }

                pos = i + 1;
                break;
            }

            Children.Insert(pos, successor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeFilters_FilterUpdate(object sender, FilterEvent e)
        {
            if (this == Tree.Root)
            {
                return;
            }

            var isVisible = Tree.NodeFilters.Filter(this);
            if (isVisible != IsVisible)
            {
                e.VisibiltyChanged = true;
                e.FilteredNode = this;
                IsVisible = isVisible;
            }
            else
            {
                e.VisibiltyChanged = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expandedLinks"></param>
        private void RefreshExpandedLinks(HashSet<XElement> expandedLinks)
        {
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];
                if (child is AMLNodeInheritable ih && expandedLinks.Contains(child.CAEXNode))
                {
                    ih.ShowLinks = true;
                }

                child.RefreshExpandedLinks(expandedLinks);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IList<AMLNodeViewModel> RenumberedList()
        {
            var updated = LoadedChildrenNotVirtual();
            for (var index = 0; index < updated.Count; index++)
            {
                updated[index].NodeIndex = index;
            }

            return updated;
        }

        #endregion Private Methods

        #region Private Classes

        /// <summary>
        /// 
        /// </summary>
        private class GroupComparer : IEqualityComparer<AMLNodeViewModel>
        {
            #region Public Methods

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(AMLNodeViewModel x, AMLNodeViewModel y)
            {
                if (x is AMLNodeGroupViewModel gx && y is AMLNodeGroupViewModel gy)
                {
                    return gx.GroupName == gy.GroupName;
                }

                return false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(AMLNodeViewModel obj)
            {
                if (obj is AMLNodeGroupViewModel gx)
                {
                    return gx.GroupName.GetHashCode();
                }

                return obj.GetHashCode();
            }

            #endregion Public Methods
        }

        #endregion Private Classes

        //private AMLNodeViewModel PreviousSibling()
        //{
        //    for (int i = NodeIndex - 1; i >= 0; i--)
        //    {
        //        if (Parent.Children[i].IsVisible && Parent.Children[i] != this)
        //            return Parent.Children[i];
        //    }
    }
}