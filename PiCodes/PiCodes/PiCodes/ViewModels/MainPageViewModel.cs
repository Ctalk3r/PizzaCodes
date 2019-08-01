using Newtonsoft.Json;
using PiCodes.Models;
using System;
using System.Collections.Generic;
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
        public CodesCollection codes { get; set; }
        public string ToolbarIcon { get; set; }
        public ICommand RefreshButtonCommand { protected set; get; }
        public MainPageViewModel()
        {
            codes = new CodesCollection();
            codes.ReadCodes();
            RefreshButtonCommand = new Command(OnRefreshButtonClicked);
        }
        private async void OnRefreshButtonClicked(object sender)
        {
            if (!codes.IsRefreshing)
            {
                
                string mes = await codes.RefreshAsync();
                await codes.WriteCodes();
 
                await Application.Current.MainPage.DisplayAlert("Информация", codes.IsRefreshing.ToString(), "ОК");
            }
        }

    }
}
