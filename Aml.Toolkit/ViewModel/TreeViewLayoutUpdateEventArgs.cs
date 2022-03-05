// *********************************************************************** Assembly :
// Aml.Toolkit Author : Josef Prinz Created : 03-09-2015
//
// Last Modified By : Josef Prinz Last Modified On : 03-10-2015 ***********************************************************************
// <copyright file="AMLTreeViewModel.cs" company="inpro">
//    Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************

using System;

namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    /// Event arguments defining layout changes for a tree view
    /// </summary>
    public class TreeViewLayoutUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The old tree layout
        /// </summary>
        public AMLLayout OldLayout { get; }

        /// <summary>
        /// The newly assigned tree layout
        /// </summary>
        public AMLLayout NewLayout { get; }

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="oldLayout"></param>
        /// <param name="newLayout"></param>
        public TreeViewLayoutUpdateEventArgs(AMLLayout oldLayout, AMLLayout newLayout)
        {
            OldLayout = oldLayout;
            NewLayout = newLayout;
        }
    }
}