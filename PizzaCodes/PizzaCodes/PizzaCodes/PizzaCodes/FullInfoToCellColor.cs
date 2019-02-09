using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Xamarin.Forms;
using XFGloss;

namespace PizzaCodes
{
    public class FullInfoToCellColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(((string)value).Contains("%")) return new Gradient(Gradient.RotationTopToBottom, Color.Red, Color.PaleVioletRed);
                else if (((string)value).Contains("ТРАД") || ((string)value).Contains("ТОНК")) return new Gradient(Gradient.RotationTopToBottom, Color.Green, Color.LightGreen);
                    else return new Gradient(Gradient.RotationTopToBottom, Color.Yellow, Color.LightYellow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
