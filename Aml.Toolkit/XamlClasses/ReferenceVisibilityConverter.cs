using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
        return value != null && (bool)value
 /* Unmerged change from project 'Aml.Toolkit (net8.0-windows)'
 Before:
             return Visibility.Collapsed;
         }

         return Visibility.Visible;
 After:
             return Visibility.Collapsed : (object)Visibility.Visible;
 */

 /* Unmerged change from project 'Aml.Toolkit (net6.0-windows)'
 Before:
             return Visibility.Collapsed;
         }

         return Visibility.Visible;
 After:
             return Visibility.Collapsed : (object)Visibility.Visible;
 */
 ? Visibility.Collapsed : (object)Visibility.Visible;
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
        return values[0] is bool show && values[1] is string name && show && !string.IsNullOrEmpty(name)
            ? Visibility.Visible
            : (object)Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}