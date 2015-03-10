// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="BooleanOrToVisibilityConverter.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// The XamlClasses namespace.
/// </summary>
namespace AMLToolkit.XamlClasses
{
    /// <summary>
    /// Class BooleanOrToVisibilityConverter converts an array of boolean values to <see cref="Visibility.Visible"/> if at least one of the values is <c>true</c>.
    /// </summary>
    public class BooleanOrToVisibilityConverter : IMultiValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Konvertiert Quellwerte in einen Wert für das Bindungsziel.Das Datenbindungsmodul ruft diese Methode auf, wenn es Werte aus den Quellbindungen an das Bindungsziel weitergibt.
        /// </summary>
        /// <param name="values">Der Wertearray, den die Quellbindungen in dem <see cref="T:System.Windows.Data.MultiBinding" /> erzeugen.Der Wert <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> gibt an, dass die Quellbindung keinen Wert für die Konvertierung bereitstellen kann.</param>
        /// <param name="targetType">Der Typ der Bindungsziel-Eigenschaft.</param>
        /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
        /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
        /// <returns>Ein konvertierter Wert.Wenn die Methode null zurückgibt, wird der gültige null-Wert verwendet.Der Rückgabewert <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> gibt an, dass der Konverter keinen Wert erstellt und dass die Bindung den <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> verwendet, falls vorhanden, oder andernfalls den Standardwert.Der Rückgabewert <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> gibt an, dass die Bindung den Wert nicht überträgt oder den <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> oder den Standardwert verwendet.</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.All(v => v is bool))
            {
                if (values.Cast<bool>().Any(v => v))
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// Konvertiert einen Bindungsziel-Wert in Werte für die Quellbindung.
        /// </summary>
        /// <param name="value">Der Wert, den das Bindungsziel erzeugt.</param>
        /// <param name="targetTypes">Das Array der Typen, in die konvertiert werden soll.Die Arraylänge gibt die Anzahl und die Typen der Werte an, die der Methode für die Rückgabe vorgeschlagen werden.</param>
        /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
        /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
        /// <returns>Ein Array von Werten, die aus dem Zielwert in die Quellwerte zurückkonvertiert wurden.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}