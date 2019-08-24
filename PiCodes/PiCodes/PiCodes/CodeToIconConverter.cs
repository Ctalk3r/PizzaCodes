using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Xamarin.Forms;
namespace PiCodes
{
    public class CodeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((string)value).Contains("%")) return "percentLogo";
            else if (((string)value).Contains("ТРАД") || ((string)value).Contains("ТОНК")) return "pizzaLogo";
            else return "otherFoodLogo";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
