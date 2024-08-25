// Copyright (c) 2017 AutomationML e.V.
using System;

namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Event arguments defining layout changes for a tree view
/// </summary>
public class TreeViewLayoutUpdateEventArgs : EventArgs
{
    /// <summary>
    ///     Creates an instance
    /// </summary>
    /// <param name="oldLayout"></param>
    /// <param name="newLayout"></param>
    public TreeViewLayoutUpdateEventArgs(AMLLayout oldLayout, AMLLayout newLayout)
    {
        OldLayout = oldLayout;
        NewLayout = newLayout;
    }

    /// <summary>
    ///     The old tree layout
    /// </summary>
    public AMLLayout OldLayout { get; }

    /// <summary>
    ///     The newly assigned tree layout
    /// </summary>
    public AMLLayout NewLayout { get; }
}