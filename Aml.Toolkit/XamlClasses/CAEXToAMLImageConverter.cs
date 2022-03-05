using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Aml.Engine.CAEX.CAEX_CLASSModel_TagNames;

namespace Aml.Toolkit.XamlClasses
{
    /// <summary>
    /// This converter class can be used to convert the name of an CAEX element to its image.
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class CAEXNameToAMLImageConverter : IValueConverter
    {
        #region Public Methods

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string caexName)
            {
                return null;
            }

            static object GetResourceValue(string keyName)
            {
                // Search all dictionaries
                return Application.Current.Resources[keyName];               
            }

            return caexName switch
            {
                INTERFACECLASSLIB_STRING => GetResourceValue("ICLibImage"),
                INTERFACECLASS_STRING => GetResourceValue("ICImage"),
                ATTRIBUTETYPELIB_STRING => GetResourceValue("ATLibImage"),
                ATTRIBUTETYPE_STRING => GetResourceValue("ATImage"),
                ATTRIBUTE_STRING => GetResourceValue("AttributeImage"),
                MAPPINGOBJECT_ATTRIBUTENAME_STRING => GetResourceValue("ANMImage"),
                EXTERNALINTERFACE_STRING => GetResourceValue("EIImage"),
                INTERNALELEMENT_STRING => GetResourceValue("IEImage"),
                INTERNALLINK_STRING => GetResourceValue("ILImage"),
                INSTANCEHIERARCHY_STRING => GetResourceValue("IHImage"),
                MAPPINGOBJECT_INTERFACEID_STRING or MAPPINGOBJECT_INTERFACENAME_STRING => GetResourceValue("INMImage"),
                MAPPINGOBJECT_STRING => GetResourceValue("MOImage"),
                ROLECLASS_STRING => GetResourceValue("RCImage"),
                ROLECLASSLIB_STRING => GetResourceValue("RCLibImage"),
                ROLEREQUIREMENTS_STRING => GetResourceValue("RRImage"),
                SUPPORTEDROLECLASS_STRING => GetResourceValue("SRCImage"),
                SYSTEMUNITCLASSLIB_STRING => GetResourceValue("SUCLibImage"),
                SYSTEMUNITCLASS_STRING => GetResourceValue("SUCImage"),
                _ => null,
            };
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}