using ChatShared;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace ChatClient.Converters.Privilege {
    public class NeutralConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is PrivilegeValue v) {
                switch (v) {
                case PrivilegeValue.Positive: {
                    return Brushes.Transparent;
                }
                case PrivilegeValue.Neutral: {
                    return Brushes.Yellow;
                }
                case PrivilegeValue.Negative: {
                    return Brushes.Transparent;
                }
                default: {
                    return Brushes.Transparent;
                }
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
