using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     an inverse bool to visibility converter
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
public class InverseBoolToVisibilityConverter : IValueConverter
{
    #region Public Methods

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && (bool)value)
        {
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}



/// <summary>
///     Converter to set the visibility of reference strings in tree view node headers
/// </summary>
/// <seealso cref="System.Windows.Data.IMultiValueConverter" />
public class ReferenceVisibilityConverter : IMultiValueConverter
{
    #region Public Methods

    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is bool show && values[1] is string name && show && !string.IsNullOrEmpty(name))
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}