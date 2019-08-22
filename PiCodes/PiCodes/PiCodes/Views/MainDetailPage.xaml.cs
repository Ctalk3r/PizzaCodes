using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PiCodes.ViewModels;
using PiCodes.Models;

namespace PiCodes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainDetailPage : ContentPage
    {
        public MainDetailPage()
        {
            InitializeComponent();
        }
        private async void DisplayCode(object sender, ItemTappedEventArgs e)
        {
            Code code = e.Item as Code;
            if(code != null)
            await DisplayAlert("Информация", code.FullInfo, "ОК");
        }
    }
}