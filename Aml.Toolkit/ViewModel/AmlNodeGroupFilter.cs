// ***********************************************************************
// Assembly         : Aml.Toolkit
// Author           : Josef Prinz
// Created          : 07-21-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 07-21-2015
// ***********************************************************************
// <copyright file="AmlNodeGroupFilter.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AmlNodeGroupFilter holds all Filter-Predicates, defined for a Node
/// </summary>
public class AmlNodeGroupFilter
{
    #region Private Fields

    /// <summary>
    ///     The _filters
    /// </summary>
    private readonly List<Predicate<AMLNodeViewModel>> _filters;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AmlNodeGroupFilter" /> class.
    /// </summary>
    public AmlNodeGroupFilter()
    {
        _filters = [];
        Filter = InternalFilter;
    }

    #endregion Public Constructors

    #region Internal Properties

    /// <summary>
    ///     Gets the filter.
    /// </summary>
    /// <value>The filter.</value>
    internal Predicate<AMLNodeViewModel> Filter { get; }

    #endregion Internal Properties

    #region Public Events

    /// <summary>
    ///     Occurs when the groupfilter is updated.
    /// </summary>
    public event EventHandler<FilterEvent> FilterUpdate;

    #endregion Public Events

    #region Private Methods

    /// <summary>
    ///     The Internal Filter Method calls all filters until the first returns false.
    /// </summary>
    /// <param name="node">The Node.</param>
    /// <returns><c>true</c> if Node pass the Filter, <c>false</c> otherwise.</returns>
    private bool InternalFilter(AMLNodeViewModel node)
    {
        return _filters.All(f => f(node));
    }

    #endregion Private Methods

    #region Public Methods

    /// <summary>
    ///     Adds the filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    public void AddFilter(Predicate<AMLNodeViewModel> filter)
    {
        _filters.Add(filter);
    }

    /// <summary>
    ///     All Filters are refreshed. Nodes, which don't pass the filter become invisible.
    /// </summary>
    public void Refresh()
    {
        FilterUpdate?.Invoke(this, new FilterEvent());

        //foreach (EventHandler<FilterEvent> filter in FilterUpdate.GetInvocationList())
        //{
        //    var filterArgs = new FilterEvent();

        //    filter(this, filterArgs);

        //    if (filterArgs.VisibilityChanged)
        //    {
        //    }
        //}
    }

    /// <summary>
    ///     Removes the filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    public void RemoveFilter(Predicate<AMLNodeViewModel> filter)
    {
        if (_filters.Contains(filter))
        {
            _ = _filters.Remove(filter);
        }
    }

    #endregion Public Methods
}

/// <summary>
///     Class FilterEvent defines the Arguments in the <see cref="AmlNodeGroupFilter.FilterUpdate" />
/// </summary>
public class FilterEvent : EventArgs
{
    #region Public Properties

    /// <summary>
    ///     Gets or sets the filtered node.
    /// </summary>
    /// <value>
    ///     The filtered node.
    /// </value>
    public AMLNodeViewModel FilteredNode { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the node visibilty has changed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if [visibilty changed]; otherwise, <c>false</c>.
    /// </value>
    public bool VisibiltyChanged { get; set; }

    #endregion Public Properties
}