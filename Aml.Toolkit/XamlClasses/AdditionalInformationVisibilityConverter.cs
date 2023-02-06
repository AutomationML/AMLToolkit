// ***********************************************************************
// Assembly         : Aml.Toolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 04-28-2015
// ***********************************************************************
// <copyright file="BooleanOrToVisibilityConverter.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

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
public class AdditionalInformationVisibilityConverter : IMultiValueConverter
{
    #region Public Methods

    /// <summary>
    ///     Converts the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is not bool)
        {
            return Visibility.Collapsed;
        }

        var firstBool = (bool)values[0];

        if (!firstBool)
        {
            return Visibility.Collapsed;
        }

        return values[1] switch
        {
            string when string.IsNullOrEmpty((string)values[1]) => Visibility.Collapsed,
            string => Visibility.Visible,
            null => Visibility.Collapsed,
            _ => Visibility.Visible
        };
    }

    /// <summary>
    ///     Converts the values back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetTypes">The target types.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion Public Methods
}