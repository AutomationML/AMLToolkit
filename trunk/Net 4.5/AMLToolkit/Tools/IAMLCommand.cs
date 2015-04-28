// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 04-27-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-27-2015
// ***********************************************************************
// <copyright file="IAMLCommand.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Tools namespace.
/// </summary>
namespace AMLToolkit.Tools
{
    /// <summary>
    /// Interface IAMLCommand defines properties and methods for commands, which can be managed by <see cref="AMLUndoRedoManager"/>
    /// </summary>
    public interface IAMLCommand
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        string DisplayName { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns><c>true</c> if execute has an effect, which is reversable by <see cref="UnExecute"/>, <c>false</c> otherwise.</returns>
        bool Execute();

        /// <summary>
        /// Reverse Method for the effect, created by the <see cref="Execute"/>.
        /// </summary>
        /// <returns><c>true</c> if UnExecute  has an effect, which is reversable by <see cref="Execute"/>, <c>false</c> otherwise.</returns>
        bool UnExecute();

        #endregion Public Methods
    }
}