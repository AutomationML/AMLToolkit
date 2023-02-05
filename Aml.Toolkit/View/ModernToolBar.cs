using Aml.Toolkit.Operations;
using System.Windows;
using System.Windows.Controls;

namespace Aml.Toolkit.View
{
    /// <summary>
    /// class defining a toolbar with a modern style layout
    /// </summary>
    /// <seealso cref="ContentControl" />
    public class ModernToolBar : ContentControl
    {
        #region Public Fields

        /// <summary>
        ///     The standard tool bar Operations property. With this Property a ToolBar containing some
        ///     Standard ToolBar Operations can be designed
        /// </summary>
        public static readonly DependencyProperty ToolBarOperationsProperty =
            DependencyProperty.Register(nameof(ToolBarOperations),
                typeof(ItemOperations), typeof(ModernToolBar), new PropertyMetadata(null));

        #endregion Public Fields

        #region Private Fields

        private ItemsControl toolBarListBox;

        #endregion Private Fields

        #region Public Constructors

        static ModernToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernToolBar),
                new FrameworkPropertyMetadata(typeof(ModernToolBar)));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets the standard tool bar Operations.
        /// </summary>
        /// <value>The standard tool bar Operations.</value>
        public ItemOperations ToolBarOperations
        {
            get => (ItemOperations)GetValue(ToolBarOperationsProperty);
            set => SetValue(ToolBarOperationsProperty, value);
        }

        #endregion Public Properties

        #region Public Methods

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            toolBarListBox = GetTemplateChild("ToolBarListBoc") as ItemsControl;

            if (toolBarListBox != null)
            {
                toolBarListBox.DataContext = this;
            }
        }

        #endregion Public Methods
    }
}