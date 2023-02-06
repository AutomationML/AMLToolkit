using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Xml.Linq;
using Aml.Engine.CAEX;
using Aml.Toolkit.ViewModel;

namespace Aml.Toolkit.View;

/// <summary>
/// </summary>
/// <seealso cref="Control" />
public class AMLTreeViewSearch : Control
{
    #region Public Properties

    /// <summary>
    ///     Gets or sets the aml TreeView.
    /// </summary>
    public AMLTreeView AmlTreeView
    {
        get => (AMLTreeView)GetValue(AmlTreeViewProperty);
        set => SetValue(AmlTreeViewProperty, value);
    }

    #endregion Public Properties

    #region Private Properties

    private string Hierarchy
    {
        get
        {
            if (AmlTreeView.TreeViewModel.CAEXTagNames.Contains(CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING))
            {
                return "Instance Hierarchies";
            }

            if (AmlTreeView.TreeViewModel.CAEXTagNames.Contains(CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING))
            {
                return "SystemUnitClass Libraries";
            }

            if (AmlTreeView.TreeViewModel.CAEXTagNames.Contains(CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING))
            {
                return "RoleClass Libraries";
            }

            if (AmlTreeView.TreeViewModel.CAEXTagNames.Contains(CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING))
            {
                return "InterfaceClass Libraries";
            }

            if (AmlTreeView.TreeViewModel.CAEXTagNames.Contains(CAEX_CLASSModel_TagNames.ATTRIBUTETYPELIB_STRING))
            {
                return "AttributeType Libraries";
            }

            return "elements";
        }
    }

    #endregion Private Properties

    #region Public Methods

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("ElementChoiceButton") is Button button)
        {
            button.Click += Button_Click;
        }
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary>
    ///     Handles the StartFilterRequest event of the FilterItemViewModel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void StartFilterRequest(object sender, EventArgs e)
    {
        _ = Dispatcher?.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        {
            AmlTreeView.TreeViewModel.NodeFilters.RemoveFilter(NodeIsInSearchResult);
            AmlTreeView.TreeViewModel.NodeFilters.Refresh();

            if (string.IsNullOrEmpty(_filterItemViewModel.SearchText))
            {
                _searchresultList?.Clear();
                return;
            }

            var treeViewItem = AmlTreeView.TreeViewModel.SelectedElements.FirstOrDefault();
            if (treeViewItem != null)
            {
                _filterItemViewModel.Search(treeViewItem.CAEXNode);
            }

            // search in all hierarchies
            else
            {
                var hierarchies = (from hierarchy in AmlTreeView.TreeViewModel.Root.VisibleChildren
                    where hierarchy is not AMLNodeGroupViewModel
                    select hierarchy.CAEXNode).ToList();

                if (hierarchies.Count > 0)
                {
                    _filterItemViewModel.Search(hierarchies);
                }
            }
        }));
    }

    #endregion Protected Methods

    #region Public Fields

    /// <summary>
    ///     The aml TreeView property
    /// </summary>
    public static readonly DependencyProperty AmlTreeViewProperty =
        DependencyProperty.Register(nameof(AmlTreeView), typeof(AMLTreeView), typeof(AMLTreeViewSearch),
            new PropertyMetadata(default(AMLTreeView), OnTreeViewChanged));


    private static void OnTreeViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not AMLTreeView tree)
        {
            return;
        }

        if (tree.DataContext is AMLTreeViewModel treeViewModel)
        {
            treeViewModel.SelectionChanged += (o, s) =>
            {
                if (d is not AMLTreeViewSearch treeViewSearch)
                {
                    return;
                }

                if (treeViewSearch._filterItemViewModel.InUse)
                {
                    treeViewSearch._filterItemViewModel.Restart();
                }
            };
        }
    }

    #endregion Public Fields

    #region Private Fields

    private AmlSearchViewModel _filterItemViewModel;

    private List<XElement> _searchresultList;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    ///     Initializes the <see cref="AMLTreeViewSearch" /> class.
    /// </summary>
    static AMLTreeViewSearch()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AMLTreeViewSearch),
            new FrameworkPropertyMetadata(typeof(AMLTreeViewSearch)));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AMLTreeViewSearch" /> class.
    /// </summary>
    public AMLTreeViewSearch()
    {
        DataContextChanged += AMLTreeViewSearch_DataContextChanged;
    }

    #endregion Public Constructors

    #region Private Methods

    private void AMLTreeViewSearch_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not AmlSearchViewModel searchVm)
        {
            return;
        }

        _filterItemViewModel = searchVm;
        _filterItemViewModel.StartFilterRequest += StartFilterRequest;
        _filterItemViewModel.SearchCompleted += SearchCompleted;
        _filterItemViewModel.PropertyChanged += FilterPropertyChanged;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.ContextMenu == null)
        {
            return;
        }

        btn.ContextMenu.IsEnabled = true;
        btn.ContextMenu.PlacementTarget = btn;
        btn.ContextMenu.Placement = PlacementMode.Bottom;
        btn.ContextMenu.IsOpen = true;
    }

    private void FilterPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        {
            if (e.PropertyName == nameof(AmlSearchViewModel.FilterTree))
            {
                if (_filterItemViewModel.FilterTree)
                {
                    if (_searchresultList is { Count: > 0 })
                    {
                        AmlTreeView.TreeViewModel.NodeFilters.AddFilter(NodeIsInSearchResult);
                    }
                }
                else
                {
                    AmlTreeView.TreeViewModel.NodeFilters.RemoveFilter(NodeIsInSearchResult);
                }

                AmlTreeView.TreeViewModel.NodeFilters.Refresh();
            }

            if (AmlTreeView != null)
            {
                _filterItemViewModel.SearchScope = AmlTreeView.TreeViewModel.SelectedNode != null
                    ? AmlTreeView.TreeViewModel.SelectedNode.Name
                    : $"all {Hierarchy}";
            }
        }));
    }

    private bool NodeIsInSearchResult(AMLNodeViewModel node)
    {
        for (var i = 0; i < _searchresultList.Count; i++)
        {
            if (node is AMLNodeGroupViewModel { IsVisibleInLayout: true })
            {
                if (node.Children.Any(n => n.CAEXNode == _searchresultList[i]))
                {
                    return true;
                }
            }
            else
            {
                if (_searchresultList[i] == node.CAEXNode)
                {
                    return true;
                }

                if (_searchresultList[i].Ancestors().Contains(node.CAEXNode))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SearchCompleted(object sender, EventArgs e)
    {
        if (!_filterItemViewModel.FilterOn)
        {
            return;
        }

        _searchresultList = _filterItemViewModel.Results.ToList();
        if (_searchresultList.Count > 0 && _filterItemViewModel.FilterTree)
        {
            AmlTreeView.TreeViewModel.NodeFilters.AddFilter(NodeIsInSearchResult);
        }

        for (var i = 0; i < _searchresultList.Count; i++)
        {
            var treeViewItem =
                AmlTreeView.TreeViewModel.SelectCaexNode(_searchresultList[i], false, false, true, true);
            _filterItemViewModel.AdSelectedNode(treeViewItem);
        }

        for (var i = 0; i < _filterItemViewModel.SelectedNodes.Count; i++)
        {
            _filterItemViewModel.SelectedNodes[i].IsMarked = true;
        }
        //_filterItemViewModel.SelectedNodes.BorderColor = Brushes.DarkGreen;

        AmlTreeView.TreeViewModel.NodeFilters.Refresh();
    }

    #endregion Private Methods
}