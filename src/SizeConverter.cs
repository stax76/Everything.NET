
using System;
using System.Globalization;
using System.Windows.Data;

namespace EverythingNET
{
    public class SizeConverter : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long bytes = (long)value;

            if (bytes == -1)
                return "";

            if (bytes < 1024)
                return bytes + " B";

            if (bytes < 1024 * 1024)
                return System.Convert.ToInt32(bytes / 1024.0) + " KB";

            if (bytes < 1024 * 1024 * 1024)
                return (bytes / 1024.0 / 1024.0).ToString("F2") + " MB";

            return (bytes / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " GB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
