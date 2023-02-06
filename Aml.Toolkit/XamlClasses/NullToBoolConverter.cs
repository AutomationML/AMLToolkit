using System;
using System.Globalization;
using System.Windows.Data;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     converts an empty string to true
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
public class NullToBoolConverter : IValueConverter
{
    #region Public Methods

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case null:
            case string vs when string.IsNullOrEmpty(vs):
                return true;
            default:
                return false;
        }
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}