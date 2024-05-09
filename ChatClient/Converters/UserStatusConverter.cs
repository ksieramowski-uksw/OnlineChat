using ChatShared;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace ChatClient.Converters {
    public class UserStatusConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is UserStatus color) {
                switch (color) {
                case UserStatus.Online: {
                    return Brushes.Green;
                }
                case UserStatus.Offline: {
                    return Brushes.Gray;
                }
                default:
                    return Brushes.Transparent;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
