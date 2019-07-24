using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using PiCodes;
using PiCodes.Droid;
using PiCodes.Models;
using System.ComponentModel;
using XLabs.Forms.Controls;
using XLabs.Forms.Extensions;

[assembly:ExportRenderer(typeof(ButtonWithHorizontalAlignment), typeof(ButtonWithHorizontalAlignmentRenderer))]
namespace PiCodes.Droid
{
    [Obsolete]
    public class ButtonWithHorizontalAlignmentRenderer : ButtonRenderer
    {
        public new ButtonWithHorizontalAlignment Element
        {
            get
            {
                return (ButtonWithHorizontalAlignment)base.Element;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                return;
            }

            UpdateAlignment();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ButtonWithHorizontalAlignment.VerticalTextAlignmentProperty.PropertyName ||
                e.PropertyName == ButtonWithHorizontalAlignment.HorizontalTextAlignmentProperty.PropertyName)
            {
                UpdateAlignment();
            }
            base.OnElementPropertyChanged(sender, e);
        }

        private void UpdateAlignment()
        {
            var element = this.Element as ButtonWithHorizontalAlignment;

            if (element == null || this.Control == null)
            {
                return;
            }

            this.Control.Gravity = element.VerticalTextAlignment.ToDroidVerticalGravity() |
                element.HorizontalTextAlignment.ToDroidHorizontalGravity();
        }
    }

    public static class AlignmentHelper
    {
        public static GravityFlags ToHorizontalGravityFlags(this Xamarin.Forms.TextAlignment alignment)
        {
            if (alignment == Xamarin.Forms.TextAlignment.Center)
                return GravityFlags.AxisSpecified;
            return alignment == Xamarin.Forms.TextAlignment.End ? GravityFlags.Right : GravityFlags.Left;
        }
        public static GravityFlags ToVerticalGravityFlags(this Xamarin.Forms.TextAlignment alignment)
        {
            if (alignment == Xamarin.Forms.TextAlignment.Center)
                return GravityFlags.AxisSpecified;
            return alignment == Xamarin.Forms.TextAlignment.End ? GravityFlags.Right : GravityFlags.Left;
        }
    }

}