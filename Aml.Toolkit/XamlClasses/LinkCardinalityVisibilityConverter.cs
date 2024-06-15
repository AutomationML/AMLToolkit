using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
/// </summary>
public class LinkCardinalityVisibilityConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Length == 2
            ? !(bool)values[0] ? Visibility.Collapsed : !(bool)values[1] ? Visibility.Collapsed : (object)Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}