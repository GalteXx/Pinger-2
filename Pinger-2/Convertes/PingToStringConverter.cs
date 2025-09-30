using System.Windows.Data;

namespace Pinger_2.Convertes
{
    internal class PingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not TimeSpan Value)
                throw new ArgumentException("Invalid value type");
            if (Value == TimeSpan.MinValue || Value == TimeSpan.FromMilliseconds(-1d))
                return "N/A";
            if (Value == TimeSpan.MaxValue)
                return "Timeout";
            return $"{double.Round(Value.TotalMilliseconds, 1)} ms";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
