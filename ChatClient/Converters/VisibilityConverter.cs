using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace ChatClient.Converters {
    public class VisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool isVisible && isVisible == true) {
                return Visibility.Visible;
            }
            else {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Visibility visibility && visibility == Visibility.Visible) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public class VisibilityConverter_Negative : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool isVisible && isVisible == false) {
                return Visibility.Visible;
            }
            else {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Visibility visibility && visibility == Visibility.Visible) {
                return false;
            }
            else {
                return true;
            }
        }
    }
}
