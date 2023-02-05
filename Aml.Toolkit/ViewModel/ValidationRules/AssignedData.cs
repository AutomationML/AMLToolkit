using Aml.Engine.CAEX;
using System.Windows;

namespace Aml.Toolkit.ViewModel.ValidationRules
{
    /// <summary>
    /// class used to visualize validation data
    /// </summary>
    /// <seealso cref="DependencyObject" />
    public class AssignedData : DependencyObject
    {
        #region Public Fields


        /// <summary>
        /// The caex object property
        /// </summary>
        public static readonly DependencyProperty CaexObjectProperty =
            DependencyProperty.Register(nameof(CaexObject), typeof(CAEXBasicObject), typeof(AssignedData),
                new PropertyMetadata(null, PropertyChangedCallback));

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the caex object.
        /// </summary>
        /// <value>
        /// The caex object.
        /// </value>
        public CAEXBasicObject CaexObject
        {
            get => (CAEXBasicObject)GetValue(CaexObjectProperty);
            set => SetValue(CaexObjectProperty, value);
        }

        #endregion Public Properties

        #region Private Methods

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Private Methods
    }
}