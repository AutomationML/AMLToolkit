using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aml.Toolkit.XamlClasses
{
    public class LinkCardinalityVisibilityConverter : IMultiValueConverter
    {
        //<Binding Path="Tree.TreeViewLayout.ShowLinkCardinality" />
        //<Binding Path="HasCardinality" />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length==2)
            {
                if (!(bool)values[0])
                    return Visibility.Collapsed;
                if (!(bool)values[1])
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}