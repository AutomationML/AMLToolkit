// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-10-2015
// ***********************************************************************
// <copyright file="AMLNodeViewModel.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using AMLToolkit.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AMLNodeViewModel is the Base-ViewModel for CAEX-Elements in an AutomationML Document. The ViewModel
    /// supports LazyLoading. If a Node has not yet been expanded to view the Children, a DummyNode exists to mark the
    /// Node as a Node with Children. The DummyNode will be replaced by the Nodes Children, if it is expanded for the
    /// first time.
    /// </summary>
    public class AMLNodeViewModel : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// The dummy child
        /// </summary>
        private static readonly AMLNodeViewModel DummyChild = new AMLNodeViewModel();

        /// <summary>
        /// The _children
        /// </summary>
        private readonly ObservableCollection<AMLNodeViewModel> _children;

        /// <summary>
        /// The _parent
        /// </summary>
        private readonly AMLNodeViewModel _parent;

        /// <summary>
        /// The _caex node
        /// </summary>
        private System.Xml.XmlElement _caexNode;

        /// <summary>
        /// <see cref="ExpandAllCommand" />
        /// </summary>
        private RelayCommand<object> _expandAllCommand;

        /// <summary>
        /// The _is expanded
        /// </summary>
        private bool _isExpanded;

        /// <summary>
        /// The _is selected
        /// </summary>
        private bool _isSelected;

        /// <summary>
        /// <see cref="Name" />
        /// </summary>
        private string _name;

        /// <summary>
        /// <see cref="CAEXTagNames" />
        /// </summary>
        private List<string> caexTagNames = null;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AMLNodeViewModel" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="CaexNode">The caex node.</param>
        /// <param name="lazyLoadChildren">if set to <c>true</c> [lazy load children].</param>
        public AMLNodeViewModel(AMLNodeViewModel parent, System.Xml.XmlElement CaexNode, bool lazyLoadChildren)
        {
            _parent = parent;
            _caexNode = CaexNode;
            _children = new ObservableCollection<AMLNodeViewModel>();

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }

        #endregion Public Constructors

        #region Private Constructors

        /// <summary>
        /// This is used to create the DummyChild instance. Prevents a default instance
        /// of the <see cref="AMLNodeViewModel" /> class from being created.
        /// </summary>
        private AMLNodeViewModel()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the CAEXNode
        /// </summary>
        /// <value>The caex node.</value>
        public XmlElement CAEXNode
        {
            get
            {
                return _caexNode;
            }
        }

        /// <summary>
        /// Gets and sets the List of CAEXTagNames for valid child's, which are loaded
        /// to the TreeView. If this is not set for a Node, the List of the actual
        /// parent is used.
        /// </summary>
        /// <value>The name of the caex tag.</value>
        public List<string> CAEXTagNames
        {
            get
            {
                return caexTagNames ?? ((_parent != null) ? _parent.CAEXTagNames : null);
            }
            set
            {
                if (caexTagNames != value)
                {
                    caexTagNames = value; base.RaisePropertyChanged(() => CAEXTagNames);
                }
            }
        }

        /// <summary>
        /// Refreshes the node information. This Method can be overridden in derived classes. The Method 
        /// should be called, if the CAEX-Elements Data has changed and the Changes should be visible in any
        /// View, that has a binding to this ViewModel.
        /// </summary>
        public virtual void RefreshNodeInformation ()
        {

        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        /// <value>The children.</value>
        public ObservableCollection<AMLNodeViewModel> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Gets the children view. Binding to this Property enables filtering and
        /// ordering of the Children Collection
        /// </summary>
        /// <value>The children view.</value>
        public ListCollectionView ChildrenView
        {
            get { return CollectionViewSource.GetDefaultView(Children) as ListCollectionView; }
        }

        /// <summary>
        /// The ExpandAllCommand - Command. Execution of this Command results in the Expansion of the
        /// Node and all it's descendants. The Execution method is queued on the current Dispatcher Thread
        /// for asynchronous Execution.
        /// </summary>
        /// <value>The expand all command.</value>
        public System.Windows.Input.ICommand ExpandAllCommand
        {
            get
            {
                return this._expandAllCommand
                ??
                (this._expandAllCommand = new RelayCommand<object>(this.ExpandAllCommandExecute, this.ExpandAllCommandCanExecute));
            }
        }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        /// <value><c>true</c> if this instance has dummy child; otherwise, <c>false</c>.</value>
        public bool HasDummyChild
        {
            get { return (this.Children != null) ? this.Children.Count == 1 && this.Children[0] == DummyChild : false; }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem associated with this object is expanded.
        /// </summary>
        /// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem associated with this object is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Gets and sets the Name
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return CAEXNode != null ? CAEXNode.GetAttributeValue("Name") : "no caex object";
            }
            set
            {
                if (_name != value)
                {
                    _name = value; base.RaisePropertyChanged(() => Name);
                }
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public AMLNodeViewModel Parent
        {
            get { return _parent; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Applies the Name filter to the <see cref="ChildrenView" />. This Method is
        /// called, when the <see cref="AMLLayout.NamesOfVisibleElements" /> of the
        /// associated Layout Object changes. The Method is put on the current
        /// Dispatcher Thread for asynchronous execution. The Method ic recursively
        /// called for all Children's, that pass the Filter.
        /// </summary>
        /// <param name="VisibleNames">The visible names.</param>
        public void ApplyFilterWithName(IList<string> VisibleNames)
        {
            Predicate<object> methodCall = delegate(object node)
            {
                var AmlNode = node as AMLNodeViewModel;
                if (AmlNode.CAEXNode != null)
                {
                    if (AmlNode != null && VisibleNames.Contains(AmlNode.CAEXNode.Name))
                    {
                        AmlNode.ApplyFilterWithName(VisibleNames);
                        return true;
                    }
                    else
                        return false;
                }
                return true;
            };

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ChildrenView.Filter = methodCall;
            }));
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand. Subclasses can
        /// override this to populate the Children collection.
        /// </summary>
        public virtual void LoadChildren()
        {
            foreach (XmlNode node in this.CAEXNode.ChildNodes)
            {
                if (this.CAEXTagNames.Contains(node.Name))
                {
                    var constructor = AMLNodeRegistry.Instance[node.Name];
                    this.Children.Add(constructor.Invoke(new object[] { this, node, true }) as AMLNodeViewModel);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Test, if the <see cref="ExpandAllCommand" /> can execute.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        /// <returns>true, if command can execute</returns>
        private bool ExpandAllCommandCanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// The <see cref="ExpandAllCommand" /> Execution Action.
        /// </summary>
        /// <param name="parameter">unused parameter.</param>
        private void ExpandAllCommandExecute(object parameter)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.IsExpanded = true;
                    foreach (var child in this.Children)
                        child.ExpandAllCommand.Execute(parameter);
                }));
        }

        #endregion Private Methods
    }
}