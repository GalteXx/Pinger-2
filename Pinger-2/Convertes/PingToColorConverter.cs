using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pinger_2.Convertes
{
    class PingToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is not TimeSpan timeSpan || values[1] is not TimeSpan maxPing) 
                throw new ArgumentException("Invalid value or parameter type");

            if(timeSpan == TimeSpan.FromMilliseconds(-1d))
                return targetType == typeof(Color) ? Colors.Gray 
                    : new SolidColorBrush(Colors.Gray);

            var clamValue = Math.Clamp(timeSpan.TotalMilliseconds/maxPing.TotalMilliseconds, 0, 1);

            byte r, g, b = 0;

            if (clamValue < 0.5)
            {
                double t = clamValue / 0.5;
                r = (byte)(255 * t);
                g = 255;
            }
            else
            {
                double t = (clamValue - 0.5) / 0.5;
                r = 255;
                g = (byte)(255 * (1 - t));
            }
            if(targetType == typeof(Color))
                return Color.FromRgb(r, g, b);
            else if(targetType == typeof(Brush))
                return new SolidColorBrush(Color.FromRgb(r, g, b));
            else
                throw new ArgumentException("Invalid target type");
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
