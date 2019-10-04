using Newtonsoft.Json;
using PiCodes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PiCodes.ViewModels
{
    class MainPageViewModel
    {
        public CodesCollection Codes { get; set; }
        public string ToolbarIcon { get; set; }

        public ICommand RefreshButtonCommand { protected set; get; }

        public MainPageViewModel()
        {
            Codes = new CodesCollection();
            Codes.ReadCodes();
            RefreshButtonCommand = new Command(OnRefreshButtonClicked);
        }
        private async void OnRefreshButtonClicked(object sender)
        {
            if (!Codes.IsRefreshing)
            {
                
                string mes = await Codes.RefreshAsync();
                await Codes.WriteCodes();
 
                await Application.Current.MainPage.DisplayAlert("Информация", mes, "ОК");
            }
        }

    }
}
