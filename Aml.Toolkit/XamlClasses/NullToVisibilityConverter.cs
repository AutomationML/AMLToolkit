// Copyright (c) 2017 AutomationML e.V.
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// The XamlClasses namespace.
/// </summary>
namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     Class NullToVisibilityConverter converts an object to <see cref="Visibility.Visible" /> if it
///     is not null or to  <see cref="Visibility.Collapsed" /> if it is <c>null</c>.
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    #region Public Methods

    /// <summary>
    ///     Konvertiert einen Wert.
    /// </summary>
    /// <param name="value">Der von der Bindungsquelle erzeugte Wert.</param>
    /// <param name="targetType">Der Typ der Bindungsziel-Eigenschaft.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
    /// <returns>Ein konvertierter Wert. Wenn die Methode null zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string @string
            ? string.IsNullOrEmpty(@string) ? Visibility.Collapsed : (object)Visibility.Visible
            : value == null ? Visibility.Collapsed : (object)Visibility.Visible;
    }

    /// <summary>
    ///     Konvertiert einen Wert.
    /// </summary>
    /// <param name="value">Der Wert, der vom Bindungsziel erzeugt wird.</param>
    /// <param name="targetType">Der Typ, in den konvertiert werden soll.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
    /// <returns>Ein konvertierter Wert. Wenn die Methode null zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}