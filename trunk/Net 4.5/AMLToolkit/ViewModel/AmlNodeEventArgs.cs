// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 04-23-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-23-2015
// ***********************************************************************
// <copyright file="AmlNodeEventArgs.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AmlNodeEventArgs defines event arguments for node selection in <see cref="AMLToolkit.View.AMLTreeView"/>
    /// </summary>
    public class AmlNodeEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmlNodeEventArgs"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public AmlNodeEventArgs (AMLNodeViewModel source)
            : base()
        {
            Source = source;
        }
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public AMLNodeViewModel Source { get; private set; }
    }
}
