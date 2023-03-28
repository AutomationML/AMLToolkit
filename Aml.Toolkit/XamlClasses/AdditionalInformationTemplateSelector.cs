using ControlzEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     Selects the datatemplate for the representation of additional information attached to a tree view node.
/// </summary>
/// <seealso cref="DataTemplateSelector" />
public class AdditionalInformationTemplateSelector : DataTemplateSelector
{
    #region Public Methods

    /// <inheritdoc />
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        switch (item)
        {
            case string:
                return TextTemplate;
            case ImageSource:
                return ImageTemplate;
            case MahApps.Metro.IconPacks.PackIconBase:
                return MetroIconTemplate;
            default:
                return base.SelectTemplate(item, container);
        }
    }

    #endregion Public Methods

    #region Public Properties

    /// <summary>
    ///     Gets or sets the template for the representation of images.
    /// </summary>
    public DataTemplate ImageTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the template for the representation of metro icons from metro icon pack.
    /// </summary>
    public DataTemplate MetroIconTemplate { get; set; }

    /// <summary>
    ///     Gets or sets the template for the representation of text objects.
    /// </summary>
    public DataTemplate TextTemplate { get; set; }

    #endregion Public Properties
}