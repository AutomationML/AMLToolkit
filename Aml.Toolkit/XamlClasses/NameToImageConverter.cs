using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     This class converts a resource name to the registered resource object.
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
public class NameToImageConverter : IValueConverter
{
    #region Public Methods

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var name = value as string;
        return !string.IsNullOrEmpty(name) ? Application.Current.Resources[name] : null;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}