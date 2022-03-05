// ***********************************************************************
// Assembly         : AMLEditorUIControls
// Author           : Josef Prinz
// Created          : 04-21-2016
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-21-2016
// ***********************************************************************
// <copyright file="ItemOperations.cs" company="inpro">
//     Copyright © inpro 2016
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Aml.Toolkit.Operations
{
    /// <summary>
    ///     Class ItemOperations is a Collection of <see cref="ItemOperationViewModel" /> and is used to bind
    ///     a collection of commands to a toolbar, where each command has a bound view model of type
    ///     <see cref="ItemOperationViewModel" />.
    ///     Because the same collection may be used and expanded from different consumers, the collection supports
    ///     an identifier based direct access instead of an index based access.
    /// </summary>
    public class ItemOperations : ObservableCollection<ItemOperationViewModel>
    {
        #region Public Indexers

        /// <summary>
        ///     Gets the <see cref="ItemOperationViewModel" /> with the specified operation identifier. This is not
        ///     the Index of the Item but the identifier.
        /// </summary>
        /// <param name="operationIdentifier">The operation identifier.</param>
        /// <returns>ItemOperationViewModel.</returns>
        public new ItemOperationViewModel this[int operationIdentifier] =>
            this.SingleOrDefault(item => item.Identifier == operationIdentifier);

        #endregion Public Indexers

        #region Public Properties

        /// <summary>
        ///     Gets the maximum identifier used in the collection.
        /// </summary>
        /// <value>The maximum identifier.</value>
        public int MaxIdentifier => Items.Count > 0 ? Items.Max(op => op.Identifier) : 0;

        /// <summary>
        ///     Gets the next identifier which is greater than the maximum identifier (= <see cref="MaxIdentifier" />+1).
        /// </summary>
        /// <value>The next identifier.</value>
        public int NextIdentifier => (Items.Count > 0 ? Items.Max(op => op.Identifier) : 0) + 1;

        /// <summary>
        ///     Gets all operations which are enabled. The Bit Array has set a bits to 1, if the
        ///     corresponding item at the index position is enabled, otherwise the bit is not set.
        /// </summary>
        /// <value>The selected operations.</value>
        public BitArray SelectedOperations
        {
            get
            {
                var bitArray = new BitArray(Count);
                for (var i = 0; i < Count; i++)
                {
                    bitArray[i] = base[i].IsEnabled;
                }

                return bitArray;
            }
        }

        /// <summary>
        /// Gets or sets the standard operations.
        /// </summary>
        /// <value>
        /// The standard operations.
        /// </value>
        public virtual int StandardOperations { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Adds the operations view model to the collection
        /// </summary>
        /// <param name="operationViewModel">The operation view model.</param>
        public void AddOperation(ItemOperationViewModel operationViewModel)
        {
            if (this[operationViewModel.Identifier] != null)
            {
                return;
            }

            Add(operationViewModel);
            operationViewModel.IsEnabled = true;
        }

        /// <summary>
        ///     Adds an operation without an action. a passive operation is added, if a separator is
        ///     needed. <see cref="SeparatorItem" />
        /// </summary>
        /// <param name="operationViewModel">The operation view model.</param>
        public void AddPassiveOperation(ItemOperationViewModel operationViewModel)
        {
            Add(operationViewModel);
            operationViewModel.IsEnabled = true;
        }

        /// <summary>
        ///     Determines whether the collection contains an operation with the specified operation identifier.
        /// </summary>
        /// <param name="operationIdentifier">The operation identifier.</param>
        /// <returns><c>true</c> if the specified operation identifier contains operation; otherwise, <c>false</c>.</returns>
        public bool ContainsOperation(int operationIdentifier)
        {
            return this.SingleOrDefault(item => item.Identifier == operationIdentifier) != null;
        }

        /// <summary>
        ///     Enables all operations in the collection
        /// </summary>
        public void EnableAll()
        {
            for (var i = 0; i < SelectedOperations.Length; i++)
            {
                base[i].IsEnabled = true;
            }
        }

        /// <summary>
        ///     Enables the operation with the specified identifier.
        /// </summary>
        /// <param name="operationIdentifier">The operation identifier.</param>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void EnableOperation(int operationIdentifier, bool isEnabled)
        {
            this[operationIdentifier].IsEnabled = isEnabled;
        }

        /// <summary>
        ///     Inserts the specified operation.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="operationViewModel">The operation view model.</param>
        public void InsertOperation(int index, ItemOperationViewModel operationViewModel)
        {
            Insert(index, operationViewModel);
            operationViewModel.IsEnabled = true;
        }


        /// <summary>
        /// Removes the operation with this operation identifier
        /// </summary>
        /// <param name="operationId">The copy.</param>
        public void RemoveOperation(int operationId)
        {
            var op = this[operationId];
            Remove(op);
        }


        #endregion Public Methods
    }
}