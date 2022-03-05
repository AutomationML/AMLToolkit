using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aml.Toolkit.XamlClasses
{
    /// <summary>
    /// Selects the datatemplate for the representation of additional information attached to a tree view node.
    /// </summary>
    /// <seealso cref="DataTemplateSelector" />
    public class AdditionalInformationTemplateSelector : DataTemplateSelector
    {
        #region Public Methods

        /// <inheritdoc/>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                string => TextTemplate,
                _ => item is ImageSource ? ImageTemplate : base.SelectTemplate(item, container)
            };
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets or sets the template for the representation of images.
        /// </summary>
        public DataTemplate ImageTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template if no representation is selected.
        /// </summary>
        public DataTemplate NullTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the representation of text objects.
        /// </summary>
        public DataTemplate TextTemplate { get; set; }

        #endregion Public Properties
    }
}