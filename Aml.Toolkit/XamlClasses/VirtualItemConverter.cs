using System;
using System.Globalization;
using System.Windows.Data;

namespace Aml.Toolkit.XamlClasses
{
    /// <summary>
    /// converter clas to change the name of an interface group node
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class InterfaceGroupNameConverter : IValueConverter
    {
        #region Public Methods

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var header = (string)value;
            return !string.IsNullOrEmpty(header) ? $"{header}-Interfaces" : "";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }

    /// <summary>
    /// converter clas to change the name of a role group node
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class RoleGroupNameConverter : IValueConverter
    {
        #region Public Methods

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var header = (string)value;
            return !string.IsNullOrEmpty(header) ? $"{header}-Role references" : "";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }

}