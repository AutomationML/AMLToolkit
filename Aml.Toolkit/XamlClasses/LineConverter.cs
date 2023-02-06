using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Aml.Toolkit.XamlClasses;

/// <summary>
///     This converter class is used to change the visibility of internal link lines, dependend of the
///     existence of children in a connected tree node.
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
public class LineConverter : IValueConverter
{
    #region Public Methods

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var item = (TreeViewItem)value;
        var ic = ItemsControl.ItemsControlFromItemContainer(item);
        return item != null && ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => false;

    #endregion Public Methods
}