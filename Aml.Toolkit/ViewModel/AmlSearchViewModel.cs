// ***********************************************************************
// Assembly         : AMLEditor
// Author           : Josef Prinz
// Created          : 01-23-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 01-24-2015
// ***********************************************************************
// <copyright file="AmlSearchViewModel.cs" organization="AutomationML e.V.">
//     Copyright (c) AutomationML e.V.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Aml.Editor.MVVMBase;
using Aml.Editor.Plugin.Contract.Commanding;
using Aml.Editor.Plugin.Contracts;
using Aml.Engine.CAEX;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AmlSearchViewModel uses XPath Queries to find XmlNodes in an XmlDocument
/// </summary>
public class AmlSearchViewModel : ViewModelBase
{
    private static readonly ImageSource
        GotoNextIcon = Application.Current.Resources["GotoNextArrowIcon"] as ImageSource;

    private static readonly ImageSource
        GotoPrevIcon = Application.Current.Resources["GotoPrevArrowIcon"] as ImageSource;

    private string _gotoMode = "Go down";

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AmlSearchViewModel" /> class.
    /// </summary>
    public AmlSearchViewModel()
    {
        IsClosed = true;
        IsCaseSensitive = true;
        IsBusy = false;
        LastRecentSearchStringsCollection = [];
        CaexTagnamesCollection = [];
        CompleteWordOnly = false;
        FilterOn = false;
        FilterTree = false;
        SearchAllTags = false;

        _backgroundWorker = new BackgroundWorker
        {
            WorkerReportsProgress = false,
            WorkerSupportsCancellation = true
        };
        _backgroundWorker.DoWork += PerformSearch;
        _backgroundWorker.RunWorkerCompleted += SearchprocessCompleted;

        SearchCommand = new SimpleCommand<object>(
            o => true,
            x =>
            {
                if (x is string s)
                {
                    SearchText = s;
                    Reset();
                    SearchCommandExecute(s);
                }

                else
                {
                    SearchText = null;
                    Reset();
                }
            }
        );
    }

    #endregion Public Constructors

    #region Internal Properties

    /// <summary>
    ///     The already selected nodes in the tree view after the search
    /// </summary>
    internal List<AMLNodeViewModel> SelectedNodes { get; set; }

    #endregion Internal Properties

    /// <summary>
    ///     Gets or sets the search result.
    /// </summary>
    /// <value>The search result.</value>
    //private XPathNodeIterator SearchResult
    //{
    //    get
    //    {
    //        if (this.SearchResultIterators != null)
    //        {
    //            foreach (var iterator in this.SearchResultIterators)
    //                if (iterator.CurrentPosition < iterator.Count)
    //                    return iterator;
    //        }

    //        return null;
    //    }
    //}

    #region Private Properties

    private IEnumerator<XElement> SearchResult
    {
        get { return _searchResultIterators?.Select(iterator => iterator.GetEnumerator()).FirstOrDefault(); }
    }

    #endregion Private Properties

    /// <summary>
    ///     Gets or sets the goto mode.
    /// </summary>
    /// <value>
    ///     The goto mode.
    /// </value>
    public string GotoMode
    {
        get => _gotoMode;
        set
        {
            Set(ref _gotoMode, value);
            RaisePropertyChanged(nameof(GotoIcon));
        }
    }

    /// <summary>
    ///     Gets the goto modes.
    /// </summary>
    /// <value>
    ///     The goto modes.
    /// </value>
    public List<string> GotoModes { get; } = ["Go down", "Go up"];

    /// <summary>
    ///     Gets the goto icon.
    /// </summary>
    /// <value>
    ///     The goto icon.
    /// </value>
    public ImageSource GotoIcon => GotoMode == "Go down" ? GotoNextIcon : GotoPrevIcon;

    #region Private Fields

    /// <summary>
    ///     <see cref="FilterOn" />
    /// </summary>
    private bool _filterOn;

    /// <summary>
    ///     The actual selected node. this is an index to the list of already selected nodes
    /// </summary>
    private int _actualSelectedNode;

    /// <summary>
    ///     The background worker used for xpath queries
    /// </summary>
    private readonly BackgroundWorker _backgroundWorker;

    /// <summary>
    ///     <see cref="CancelCommand" />
    /// </summary>
    private RelayCommand<object> _cancelCommand;

    /// <summary>
    ///     <see cref="CloseCommand" />
    /// </summary>
    private RelayCommand<object> _closeCommand;

    /// <summary>
    ///     <see cref="CompleteWordOnly" />
    /// </summary>
    private bool _completeWord;

    /// <summary>
    ///     <see cref="GotoNextCommand" />
    /// </summary>
    private RelayCommand<object> _gotoNextCommand;

    /// <summary>
    ///     <see cref="GotoPrevCommand" />
    /// </summary>
    private RelayCommand<object> _gotoPrevCommand;

    /// <summary>
    ///     The is busy
    /// </summary>
    private bool _isBusy;

    /// <summary>
    ///     <see cref="IsCaseSensitive" />
    /// </summary>
    private bool _isCaseSensitive;

    /// <summary>
    ///     <see cref="IsClosed" />
    /// </summary>
    private bool _isClosed;

    /// <summary>
    ///     The matches
    /// </summary>
    private int _matches;


    /// <summary>
    ///     The search result iterators
    /// </summary>
    //private ConcurrentBag<XPathNodeIterator> SearchResultIterators;
    private ConcurrentBag<IEnumerable<XElement>> _searchResultIterators;

    /// <summary>
    ///     <see cref="SearchResultList" />
    /// </summary>
    private ObservableCollection<object> _searchResultList;

    /// <summary>
    ///     The search text
    /// </summary>
    private string _searchText;

    /// <summary>
    ///     <see cref="StatusText" />
    /// </summary>
    private string _statusText = "";

    /// <summary>
    ///     <see cref="CaexTagnamesCollection" />
    /// </summary>
    private List<TagnameFindViewModel> _tagnames;

    #endregion Private Fields

    #region Public Events

    /// <summary>
    ///     Occurs when [search completed].
    /// </summary>
    public event EventHandler SearchCompleted;

    /// <summary>
    ///     Occurs when [start search request].
    /// </summary>
    public event EventHandler StartFilterRequest;

    #endregion Public Events

    #region Public Properties

    /// <summary>
    ///     Gets the caex tagnames.
    /// </summary>
    /// <value>The caex tagnames.</value>
    public ListCollectionView CaexTagnames =>
        CollectionViewSource.GetDefaultView(CaexTagnamesCollection) as ListCollectionView;

    /// <summary>
    ///     Gets and sets the CaexTagnames
    /// </summary>
    /// <value>The caex tagnames collection.</value>
    public List<TagnameFindViewModel> CaexTagnamesCollection
    {
        get => _tagnames;
        set => Set(ref _tagnames, value);
    }

    /// <summary>
    ///     The CancelCommand - Command
    /// </summary>
    /// <value>The cancel command.</value>
    public ICommand CancelCommand => _cancelCommand ??= new RelayCommand<object>(CancelCommandExecute,
        CancelCommandCanExecute);

    /// <summary>
    ///     Gets a value indicating whether this instance can move backward.
    /// </summary>
    /// <value><c>true</c> if this instance can move backward; otherwise, <c>false</c>.</value>
    public bool CanMoveBackward => HasResult &&
                                   _actualSelectedNode > 0;

    /// <summary>
    ///     Gets a value indicating whether this instance can move forward.
    /// </summary>
    /// <value><c>true</c> if this instance can move forward; otherwise, <c>false</c>.</value>
    public bool CanMoveForward =>
        true; //return HasResult &&//    ((SearchResult != null && (SearchResult.CurrentPosition < SearchResult.Count)) ||//    (actualSelectedNode < SelectedNodes.Count - 1));


    /// <summary>
    ///     The CloseCommand - Command
    /// </summary>
    /// <value>The close command.</value>
    public ICommand CloseCommand => _closeCommand ??= new RelayCommand<object>(CloseCommandExecute,
        CloseCommandCanExecute);

    /// <summary>
    ///     Gets and sets the CompleteWordOnly
    /// </summary>
    /// <value><c>true</c> if [complete word only]; otherwise, <c>false</c>.</value>
    public bool CompleteWordOnly
    {
        get => _completeWord;
        set => Set(ref _completeWord, value);
    }

    /// <summary>
    ///     Gets and sets the FilterOn
    /// </summary>
    public bool FilterOn
    {
        get => _filterOn;
        set
        {
            if (_filterOn == value)
            {
                return;
            }

            _filterOn = value;
            RaisePropertyChanged();

            Reset();
        }
    }

    /// <summary>
    ///     The GotoNextCommand - Command
    /// </summary>
    /// <value>The goto next command.</value>
    public ICommand GotoNextCommand => _gotoNextCommand ??= new RelayCommand<object>(GotoNextCommandExecute,
        GotoNextCommandCanExecute);


    /// <summary>
    ///     The GotoPrevCommand - Command
    /// </summary>
    /// <value>The goto previous command.</value>
    public ICommand GotoPrevCommand => _gotoPrevCommand ??= new RelayCommand<object>(GotoPrevCommandExecute,
        GotoPrevCommandCanExecute);

    /// <summary>
    ///     Gets a value indicating whether this instance has result.
    /// </summary>
    /// <value><c>true</c> if this instance has result; otherwise, <c>false</c>.</value>
    public bool HasResult => SearchResult != null || SelectedNodes is { Count: > 0 };

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is busy.
    /// </summary>
    /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            RaisePropertyChanged();
        }
    }

    private bool _filterTree;

    /// <summary>
    ///     Set to <c>true</c> if the tree visualization uses the node filters
    /// </summary>
    public bool FilterTree
    {
        get => _filterTree;
        set => Set(ref _filterTree, value);
    }


    /// <summary>
    ///     Gets and sets the IsCaseSensitive
    /// </summary>
    /// <value><c>true</c> if this instance is case sensitive; otherwise, <c>false</c>.</value>
    public bool IsCaseSensitive
    {
        get => _isCaseSensitive;
        set
        {
            if (!Set(ref _isCaseSensitive, value) || IsBusy)
            {
                return;
            }

            if (FilterOn)
            {
                Reset();
                StartFilterRequest?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///     Gets and sets the IsClosed
    /// </summary>
    /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
    public bool IsClosed
    {
        get => _isClosed;
        set => Set(ref _isClosed, value);
    }

    /// <summary>
    ///     Gets the last recent search strings.
    /// </summary>
    /// <value>The last recent search strings.</value>
    public ICollectionView LastRecentSearchStrings =>
        CollectionViewSource.GetDefaultView(LastRecentSearchStringsCollection);

    /// <summary>
    ///     Gets or sets the last recent search strings collection.
    /// </summary>
    /// <value>The last recent search strings collection.</value>
    public ConcurrentBag<string> LastRecentSearchStringsCollection { get; set; }

    /// <summary>
    ///     Gets or sets the matches.
    /// </summary>
    /// <value>The matches.</value>
    public int Matches
    {
        get => _matches;
        set
        {
            _matches = value;
            RaisePropertyChanged();
        }
    }


    /// <summary>
    ///     Gets a value indicating whether [no result].
    /// </summary>
    /// <value><c>true</c> if [no result]; otherwise, <c>false</c>.</value>
    public bool NoResult => _searchResultIterators != null &&
                            !string.IsNullOrEmpty(SearchText) && !HasResult;

    /// <summary>
    ///     Gets the results.
    /// </summary>
    public IEnumerable<XElement> Results
    {
        get
        {
            if (_searchResultIterators != null)
            {
                foreach (var iterator in _searchResultIterators)
                {
                    using var current = iterator.GetEnumerator();
                    while (current.MoveNext())
                    {
                        RaisePropertyChanged(nameof(HasResult));
                        RaisePropertyChanged(nameof(NoResult));
                        RaisePropertyChanged(nameof(CanMoveForward));
                        RaisePropertyChanged(nameof(CanMoveBackward));

                        yield return current.Current;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to search all tags.
    /// </summary>
    /// <value>
    ///     <c>true</c> if [search all tags]; otherwise, <c>false</c>.
    /// </value>
    public bool SearchAllTags { get; set; }

    /// <summary>
    ///     The SearchCommand - Command
    /// </summary>
    /// <value>The search command.</value>
    public ICommand SearchCommand { get; set; }

    /// <summary>
    ///     Gets and sets the SearchResult
    /// </summary>
    /// <value>The search result list.</value>
    public ObservableCollection<object> SearchResultList
    {
        get => _searchResultList;
        set => Set(ref _searchResultList, value);
    }

    /// <summary>
    ///     Gets or sets the search text.
    /// </summary>
    /// <value>The search text.</value>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!Set(ref _searchText, value))
            {
                return;
            }

            if (!string.IsNullOrEmpty(value))
            {
                _searchText = _searchText.Trim('\r', '\n');
            }

            else
            {
                Reset();
            }

            //if (FilterOn && StartFilterRequest != null)
            //{
            //    cancellation?.Cancel();

            //    cancellation = new CancellationTokenSource();
            //    ChangeFilter();
            //}

            RaisePropertyChanged();
            RaisePropertyChanged(nameof(InUse));
        }
    }


    /// <summary>
    ///     <see cref="SearchScope" />
    /// </summary>
    private string _searchScope = "all";

    /// <summary>
    ///     Gets and sets the SearchScope
    /// </summary>
    public string SearchScope
    {
        get => _searchScope;

        set => Set(ref _searchScope, value);
    }


    /// <summary>
    ///     Gets or sets the selected tag.
    /// </summary>
    /// <value>The selected tag.</value>
    public string SelectedTag
    {
        get
        {
            var sb = new StringBuilder();
            foreach (var tag in _tagnames.Where(tag => tag.IsChecked))
            {
                if (sb.Length > 0)
                {
                    _ = sb.Append(';');
                }

                _ = sb.Append(tag.DisplayName);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    ///     Gets and sets the StatusText
    /// </summary>
    /// <value>The status text.</value>
    public string StatusText
    {
        get => _statusText;
        set => Set(ref _statusText, value);
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    ///     Creates the interface class library tagname collection.
    /// </summary>
    /// <param name="searchViewModel">The search view model.</param>
    /// <param name="objectsOnly"></param>
    public static void CreateAttributeTypeLibTagnameCollection(AmlSearchViewModel searchViewModel,
        bool objectsOnly = false)
    {
        searchViewModel.CaexTagnamesCollection.Clear();
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING,
            objectsOnly));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.ATInheritenceRel,
            false));

        searchViewModel.RegisterTags();
        _ = searchViewModel.CaexTagnames.MoveCurrentToFirst();
    }

    /// <summary>
    ///     Creates the instance hierarchy tagname collection.
    /// </summary>
    /// <param name="searchViewModel">The vm.</param>
    /// <param name="objectsOnly"></param>
    public static void CreateInstanceHierarchyTagnameCollection(AmlSearchViewModel searchViewModel,
        bool objectsOnly = false)
    {
        searchViewModel.CaexTagnamesCollection.Clear();
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            objectsOnly));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            objectsOnly));

        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel("Separator", false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.RRReference, false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.SRReference, false));

        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.IEClassReference,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.EIClassReference,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.ILReference, false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.ATvalue, false));

        searchViewModel.RegisterTags();
        _ = searchViewModel.CaexTagnames.MoveCurrentToFirst();
    }

    /// <summary>
    ///     Creates the interface class library tagname collection.
    /// </summary>
    /// <param name="searchViewModel">The vm.</param>
    /// <param name="objectsOnly"></param>
    public static void CreateInterfaceClassLibTagnameCollection(AmlSearchViewModel searchViewModel,
        bool objectsOnly = false)
    {
        searchViewModel.CaexTagnamesCollection.Clear();
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING,
            objectsOnly));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.EIClassReference,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.ICInheritenceRel,
            false));

        searchViewModel.RegisterTags();
        _ = searchViewModel.CaexTagnames.MoveCurrentToFirst();
    }

    /// <summary>
    ///     Creates the role class library tagname collection.
    /// </summary>
    /// <param name="searchViewModel">The vm.</param>
    /// <param name="objectsOnly"></param>
    public static void CreateRoleClassLibTagnameCollection(AmlSearchViewModel searchViewModel, bool objectsOnly = false)
    {
        searchViewModel.CaexTagnamesCollection.Clear();
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(CAEX_CLASSModel_TagNames.ROLECLASS_STRING,
            objectsOnly));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            objectsOnly));

        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel("Separator", false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.RCInheritenceRel,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.EIClassReference,
            false));

        searchViewModel.RegisterTags();
        _ = searchViewModel.CaexTagnames.MoveCurrentToFirst();
    }

    /// <summary>
    ///     Creates the system unit class library tagname collection.
    /// </summary>
    /// <param name="searchViewModel">The vm.</param>
    /// <param name="objectsOnly"></param>
    public static void CreateSystemUnitClassLibTagnameCollection(AmlSearchViewModel searchViewModel,
        bool objectsOnly = false)
    {
        searchViewModel.CaexTagnamesCollection.Clear();
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING,
            objectsOnly));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING,
            objectsOnly));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(
            CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING,
            objectsOnly));

        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel("Separator", false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.RRReference, false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.SRReference, false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.IEClassReference,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.EIClassReference,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.SUCInheritenceRel,
            false));
        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.ILReference, false));

        searchViewModel.CaexTagnamesCollection.Add(new TagnameFindViewModel(TagnameFindViewModel.ATvalue, false));

        searchViewModel.RegisterTags();
        _ = searchViewModel.CaexTagnames.MoveCurrentToFirst();
    }

    #endregion Public Methods

    #region Internal Methods

    /// <summary>
    ///     Adds a Node to the list of the selected nodes. This Method is called, when the Navigator
    ///     moves to the next node in the Search Result and the node was resolved to a Tree view item.
    /// </summary>
    /// <param name="node">The node.</param>
    internal void AdSelectedNode(AMLNodeViewModel node)
    {
        if (SelectedNodes.Contains(node))
        {
            return;
        }

        SelectedNodes.Add(node);
        // _actualSelectedNode = SelectedNodes.Count - 1;
    }

    /// <summary>
    ///     Resets this instance.
    /// </summary>
    internal void Reset()
    {
        _searchResultIterators = null;
        SearchResultList = null;
        _actualSelectedNode = -1;
        Matches = 0;

        ResetSelectedNodes();

        RaisePropertyChanged(nameof(HasResult));
        RaisePropertyChanged(nameof(NoResult));
        RaisePropertyChanged(nameof(CanMoveForward));
        RaisePropertyChanged(nameof(CanMoveBackward));
    }

    internal void ResetSelectedNodes()
    {
        if (SelectedNodes == null)
        {
            return;
        }

        foreach (var node in SelectedNodes)
        {
            node.IsMarked = false;
        }

        SelectedNodes.Clear();
    }

    /// <summary>
    ///     Searches the specified start node.
    /// </summary>
    /// <param name="startNode">The start node.</param>
    internal void Search(XElement startNode)
    {
        if (_backgroundWorker.IsBusy)
        {
            _backgroundWorker.CancelAsync();
            return;
        }

        _backgroundWorker.RunWorkerAsync(startNode);
    }


    /// <summary>
    ///     Gets and sets the InUse
    /// </summary>
    public bool InUse => !string.IsNullOrEmpty(SearchText);


    /// <summary>
    ///     Searches the specified hierarchies.
    /// </summary>
    /// <param name="hierarchies">The hierarchies.</param>
    internal void Search(List<XElement> hierarchies)
    {
        if (_backgroundWorker.IsBusy)
        {
            _backgroundWorker.CancelAsync();
            return;
        }

        _backgroundWorker.RunWorkerAsync(hierarchies);
    }

    #endregion Internal Methods

    #region Protected Methods

    /// <summary>
    ///     Test, if the <see cref="GotoNextCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    protected virtual bool GotoNextCommandCanExecute(object parameter) => true;

    //if ( GotoMode != "Go down" )
    //{
    //    return GotoPrevCommandCanExecute (parameter);
    //}
    //return CanMoveForward;
    /// <summary>
    ///     Test, if the <see cref="GotoNextCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    protected virtual void GotoNextCommandExecute(object parameter)
    {
        if (GotoMode != "Go down")
        {
            if (CanMoveBackward)
            {
                GotoPrevCommandExecute(parameter);
            }

            return;
        }

        if (!CanMoveForward)
        {
            return;
        }

        if (SelectedNodes != null && _actualSelectedNode < SelectedNodes.Count - 1)
        {
            _actualSelectedNode++;
            //this.SelectedNodes[actualSelectedNode].IsSelected = true;
            SelectedNodes[_actualSelectedNode].IsMarked = false;
            SelectedNodes[_actualSelectedNode].IsSelected = true;
            SelectedNodes[_actualSelectedNode].BringIntoView();
            // ShowPosition(true);
        }
        //else if (SearchCompleted != null)
        //{
        //    SearchCompleted(this, EventArgs.Empty);

        //    ShowPosition(true);
        //}

        RaisePropertyChanged(nameof(CanMoveForward));
        RaisePropertyChanged(nameof(CanMoveBackward));

        //LastSelected++;
    }

    /// <summary>
    ///     Test, if the <see cref="GotoPrevCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    protected virtual bool GotoPrevCommandCanExecute(object parameter) => CanMoveBackward;

    /// <summary>
    ///     Test, if the <see cref="GotoPrevCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    protected virtual void GotoPrevCommandExecute(object parameter)
    {
        if (SelectedNodes != null && _actualSelectedNode > 0)
        {
            _actualSelectedNode--;
            SelectedNodes[_actualSelectedNode].IsSelected = true;
            SelectedNodes[_actualSelectedNode].IsMarked = false;
            SelectedNodes[_actualSelectedNode].BringIntoView();
        }

        RaisePropertyChanged(nameof(CanMoveForward));
        RaisePropertyChanged(nameof(CanMoveBackward));
    }


    /// <summary>
    ///     The <see cref="SearchCommand" /> Execution Action.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    protected virtual void SearchCommandExecute(object parameter)
    {
        StartFilterRequest?.Invoke(this, EventArgs.Empty);
    }

    #endregion Protected Methods

    #region Private Methods

    /// <summary>
    ///     Test, if the <see cref="CancelCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    private bool CancelCommandCanExecute(object parameter) => _backgroundWorker.IsBusy;

    /// <summary>
    ///     The <see cref="CancelCommand" /> Execution Action.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    private void CancelCommandExecute(object parameter)
    {
        _backgroundWorker.CancelAsync();
    }

    /// <summary>
    ///     Test, if the <see cref="CloseCommand" /> can execute.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    /// <returns>true, if command can execute</returns>
    private bool CloseCommandCanExecute(object parameter) => true;

    /// <summary>
    ///     The <see cref="CloseCommand" /> Execution Action.
    /// </summary>
    /// <param name="parameter">TODO The parameter.</param>
    private void CloseCommandExecute(object parameter)
    {
        IsClosed = true;

        Reset();
    }

    /// <summary>
    ///     Handles the DoWork event of the backgroundWorker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="DoWorkEventArgs" /> instance containing the event data.</param>
    private void PerformSearch(object sender, DoWorkEventArgs e)
    {
        IsBusy = true;

        // SearchResultIterators = new ConcurrentBag<XPathNodeIterator>();

        _searchResultIterators = [];

        // var NodeNavigators = new ConcurrentBag<XPathNavigator>();
        var nodeNavigators = new ConcurrentBag<XElement>();

        switch (e.Argument)
        {
            case XElement startNode1:
                //NodeNavigators.Add(startNode.CreateNavigator());

                nodeNavigators.Add(startNode1);
                break;
            case List<XElement> list:
                {
                    foreach (var startNode in list)
                    {
                        nodeNavigators.Add(startNode);
                    }
                    // NodeNavigators.Add(startNode.CreateNavigator());

                    break;
                }
        }

        if (!LastRecentSearchStringsCollection.Contains(SearchText))
        {
            LastRecentSearchStringsCollection.Add(SearchText);
        }

        SelectedNodes = [];
        _actualSelectedNode = -1;

        _ = Parallel.ForEach(CaexTagnamesCollection, tag =>
        {
            if (tag.IsChecked)
            {
                Search(tag, nodeNavigators);
            }
        });

        RaisePropertyChanged(nameof(HasResult));
        RaisePropertyChanged(nameof(NoResult));
        RaisePropertyChanged(nameof(CanMoveForward));
        RaisePropertyChanged(nameof(CanMoveBackward));
    }

    private void RegisterTags()
    {
        _tagnames[0].IsChecked = true;

        foreach (var tag in _tagnames)
        {
            PropertyChangedEventManager.AddHandler(tag, Tag_PropertyChanged, string.Empty);
        }
    }

    private void Search(TagnameFindViewModel tag,
        ConcurrentBag<XElement> NodeNavigators /*ConcurrentBag<XPathNavigator> NodeNavigators*/)
    {
        // var query = tag.XPathQuery(this.CompleteWordOnly, this.IsCaseSensitive, this.SearchText);

        if (NodeNavigators.Count == 1)
        {
            var navigator = NodeNavigators.First();
            _searchResultIterators.Add(tag.Query(navigator, CompleteWordOnly, IsCaseSensitive, SearchText));
        }
        else
        {
            _ = Parallel.ForEach(NodeNavigators,
                currentNavigator =>
                {
                    _searchResultIterators.Add(tag.Query(currentNavigator, CompleteWordOnly, IsCaseSensitive,
                        SearchText)); /* currentNavigator.Select(query));*/
                });
        }
    }

    /// <summary>
    ///     Handles the RunWorkerCompleted event of the backgroundWorker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RunWorkerCompletedEventArgs" /> instance containing the event data.</param>
    private void SearchprocessCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (SearchCompleted != null && HasResult)
        {
            SearchResultList = [];

            //Matches = 0;
            //foreach (var result in SearchResultIterators)
            //    Matches += result.Count;

            //if (Matches > 40)
            //{
            //    StatusText = string.Format("{0} Matches found", Matches);
            //    Matches = 40;
            //    foreach (var result in SearchResultIterators)
            //        for (int i = 0; i < result.Count && Matches > 0; i++, Matches--)
            //            SearchResultList.Add(false);

            //    SearchResultList[0] = true;
            //}
            //else
            //{
            foreach (var _ in _searchResultIterators)
            {
                //for (int i = 0; i < result.Count; i++)
                SearchResultList.Add(false);
            }

            SearchResultList[0] = true;
            //}
            SearchCompleted(this, EventArgs.Empty);
        }

        IsBusy = false;

        if (!NoResult)
        {
            return;
        }

        StatusText = "";
        StatusText = $"no Matches for '{SearchText}'";
    }


    ///// <summary>
    /////     Shows the position.
    ///// </summary>
    ///// <param name="moveForward">if set to <c>true</c> [move forward].</param>
    //private void ShowPosition(bool moveForward)
    //{
    //    var result = SearchResultList.FirstOrDefault(a => (bool)a);
    //    if (result != null)
    //    {
    //        var index = SearchResultList.IndexOf(result);
    //        if (index < 0 || index >= SearchResultList.Count)
    //        {
    //            return;
    //        }

    //        SearchResultList[index] = false;
    //        if (moveForward && index + 1 < SearchResultList.Count)
    //        {
    //            SearchResultList[index + 1] = true;
    //        }
    //        else if (!moveForward && index - 1 >= 0)
    //        {
    //            SearchResultList[index - 1] = true;
    //        }
    //    }
    //    else if (SearchResultList.Count > 0)
    //    {
    //        SearchResultList[0] = true;
    //    }
    //}

    private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(SelectedTag));
    }

    /// <summary>
    ///     Proceed to the next result of the search
    /// </summary>
    /// <param name="forward"></param>
    public void GotoNextResult(bool forward)
    {
        if (forward && GotoNextCommandCanExecute(null))
        {
            GotoNextCommandExecute(null);
        }
        else if (!forward && GotoPrevCommandCanExecute(this))
        {
            GotoPrevCommandExecute(null);
        }
    }


    internal void Restart()
    {
        if (!IsBusy && !string.IsNullOrEmpty(SearchText))
        {
            var search = SearchText;
            SearchText = search;
        }
    }

    #endregion Private Methods
}