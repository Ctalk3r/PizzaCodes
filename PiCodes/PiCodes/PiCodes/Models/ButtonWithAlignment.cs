using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PiCodes.Models
{
    public class ButtonWithAlignment : Button
    {
        [Obsolete]
        public static BindableProperty HorizontalTextAlignmentProperty =
            BindableProperty.Create<ButtonWithAlignment, TextAlignment>
            (x => x.HorizontalTextAlignment, TextAlignment.Center);

        [Obsolete]
        public TextAlignment HorizontalTextAlignment
        {
            get { return (TextAlignment)this.GetValue(HorizontalTextAlignmentProperty); }
            set { this.SetValue(HorizontalTextAlignmentProperty, value); }
        }

        [Obsolete]
        public static BindableProperty VerticalTextAlignmentProperty =
            BindableProperty.Create<ButtonWithAlignment, TextAlignment>
            (x => x.VerticalTextAlignment, TextAlignment.Center);

        [Obsolete]
        public TextAlignment VerticalTextAlignment
        {
            get { return (TextAlignment)this.GetValue(VerticalTextAlignmentProperty); }
            set { this.SetValue(VerticalTextAlignmentProperty, value); }
        }
    }
}
