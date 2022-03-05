// ***********************************************************************
// Assembly         : AMLEditorUIControls
// Author           : Josef Prinz
// Created          : 04-21-2016
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-21-2016
// ***********************************************************************
// <copyright file="SeparatorItem.cs" company="inpro">
//     Copyright © inpro 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The ViewModels namespace.
/// </summary>

namespace Aml.Toolkit.Operations
{
    /// <summary>
    ///     Class SeparatorItem can be used to place a separator in a control which has bound its itemsource property to a
    ///     command collection.
    /// </summary>
    public static class SeparatorItem
    {
        #region Public Fields

        /// <summary>
        ///     The separator
        /// </summary>
        public const int Separator = 0;

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        ///     Adds the separator item.
        /// </summary>
        /// <param name="operations">The operations.</param>
        public static void AddSeparatorItem(ItemOperations operations)
        {
            operations.AddPassiveOperation(new ItemOperationViewModel
            {
                Identifier = Separator
            });
        }

        #endregion Public Methods
    }
}