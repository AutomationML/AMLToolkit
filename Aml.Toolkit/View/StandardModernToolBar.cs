// ***********************************************************************
// Assembly         : WPFUIControls
// Author           : prinz
// Created          : 06-27-2014
//
// Last Modified By : prinz
// Last Modified On : 06-27-2014
// ***********************************************************************
// <copyright file="StandardModernToolBar.cs" company="inpro">
//     Copyright (c) inpro. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Aml.Toolkit.Operations;
using System;
using System.Windows;

/// <summary>
/// The UserControls namespace.
/// </summary>
namespace Aml.Toolkit.View
{
    /// <summary>
    ///     Class StandardModernToolBar.
    /// </summary>
    public class StandardModernToolBar : ModernToolBar
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandardModernToolBar" /> class.
        /// </summary>
        public StandardModernToolBar()
        {
            StandardOperations = new EditOperations();
            ToolBarOperations = StandardOperations;
        }

        #endregion Public Constructors

        #region Public Fields

        // Using a DependencyProperty as the backing store for AssociatedObject.  This enables animation, styling, binding, etc...
        /// <summary>
        ///     The associated object property. This Object is used to set the command parameter for the ToolBar Operations
        /// </summary>
        public static readonly DependencyProperty AssociatedObjectProperty =
            DependencyProperty.Register(nameof(AssociatedObject), typeof(UIElement), typeof(StandardModernToolBar),
                new PropertyMetadata(null, AssociatedObjectChanged));

        // Using a DependencyProperty as the backing store for StandardOperations.  This enables animation, styling, binding, etc...
        /// <summary>
        ///     The standard Operations property
        /// </summary>
        public static readonly DependencyProperty StandardOperationsProperty =
            DependencyProperty.Register(nameof(StandardOperations), typeof(EditOperations), typeof(StandardModernToolBar),
                new PropertyMetadata(null));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        ///     Gets or sets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        public UIElement AssociatedObject
        {
            get => (UIElement)GetValue(AssociatedObjectProperty);
            set => SetValue(AssociatedObjectProperty, value);
        }

        /// <summary>
        ///     Gets or sets the standard Operations.
        /// </summary>
        /// <value>The standard Operations.</value>
        public EditOperations StandardOperations
        {
            get => (EditOperations)GetValue(StandardOperationsProperty);
            set => SetValue(StandardOperationsProperty, value);
        }

        #endregion Public Properties

        #region Public Methods

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///     Associates the object changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void AssociatedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toolbar = d as StandardModernToolBar;
            toolbar?.SetAssociatedObject(e.NewValue as UIElement);
        }

        private void SetAssociatedObject(UIElement uIElement)
        {
            foreach (var item in StandardOperations)
            {
                item.CommandParameter = uIElement;
            }
        }

        /// <summary>
        ///     Handles the CustomItemsChanged event of the StandardModernToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardModernToolBar_CustomOperationsChanged(object sender, EventArgs e)
        {
            SeparatorItem.AddSeparatorItem(StandardOperations);
        }

        #endregion Private Methods
    }
}