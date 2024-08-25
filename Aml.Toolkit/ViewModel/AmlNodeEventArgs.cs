// Copyright (c) 2017 AutomationML e.V.
using System;

namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AmlNodeEventArgs defines event arguments for node selection in <see cref="View.AMLTreeView" />
/// </summary>
public class AmlNodeEventArgs : EventArgs
{
    #region Public Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="AmlNodeEventArgs" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    public AmlNodeEventArgs(AMLNodeViewModel source)
    {
        Source = source;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    ///     Gets the source.
    /// </summary>
    /// <value>The source.</value>
    public AMLNodeViewModel Source { get; }

    #endregion Public Properties
}