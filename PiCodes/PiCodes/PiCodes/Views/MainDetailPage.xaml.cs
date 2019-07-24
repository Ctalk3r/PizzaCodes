using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PiCodes.ViewModels;

namespace PiCodes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainDetailPage : ContentPage
    {
        public MainDetailPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
        }
    }
}