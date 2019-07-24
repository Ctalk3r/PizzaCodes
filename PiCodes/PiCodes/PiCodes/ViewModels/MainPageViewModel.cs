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
    class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CodesCollection codes { get; set; }
        public ICommand RefreshButtonCommand { protected set; get; }
        public MainPageViewModel()
        {
            codes = new CodesCollection();
            codes.Add(new Code("", ""));
            RefreshButtonCommand = new Command(OnRefreshButtonClicked);
        }
        protected void OnPropertyChanged([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        private async void OnRefreshButtonClicked(object sender)
        {
            if (!codes.IsRefreshing)
            {
                string mes = await codes.RefreshAsync();
                //await codes.WriteCodes();
                await Application.Current.MainPage.DisplayAlert("Информация", mes, "ОК");
            }
        }

    }
}
