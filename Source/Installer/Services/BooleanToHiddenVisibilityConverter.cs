using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace PsGet.Installer.Services {
    public class BooleanToHiddenVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is bool && targetType == typeof(Visibility)) {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is Visibility && targetType == typeof(bool)) {
                return (Visibility)value == Visibility.Visible;
            }
            return null;
        }
    }
}
