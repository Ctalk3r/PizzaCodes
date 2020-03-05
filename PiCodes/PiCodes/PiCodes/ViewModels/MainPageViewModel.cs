using Newtonsoft.Json;
using PiCodes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PiCodes.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public static void swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
        public string ToolbarIcon { get; set; }
        private string sortIcon = "sort_by_ascending";
        public string SortIcon
        {
            get => sortIcon;
            set
            {
                if (sortIcon != value)
                {
                    sortIcon = value;              
                    RaisePropertyChanged();
                    CodesCollection.Reverse(codes);
                }
            }
        }
        private bool on = false;
        public bool On
        {
            get => on;
            set
            {
                if (value != on)
                {
                    on = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool off = false;
        public bool Off
        {
            get => off;
            set
            {
                if (value != off)
                {
                    off = value;
                    RaisePropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand RefreshButtonCommand { protected set; get; }
        public ICommand SelectCityCommand { protected set; get; }
        public ICommand SortCommand { protected set; get; }
        public ICommand MasterMenuCommand { protected set; get; }

        CodesCollection codes;

        public CodesCollection Codes
        {
            get => codes;
            set
            {
                codes = value;
                RaisePropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            codes = new CodesCollection();

            Task<bool>[] tasks = new Task<bool>[1];
            tasks[0] = codes.IsCodesFile();
            Task.Factory.ContinueWhenAll(tasks, (result) =>
            {
                if (result[0].Result == true)
                {
                    On = true;
                    Off = false;
                }
                else
                {
                    On = false;
                    Off = true;
                }
            });

            RefreshButtonCommand = new Command(OnRefreshButtonClicked);
            SelectCityCommand = new Command(SelectCity);
            SortCommand = new Command(Sort);
            MasterMenuCommand = new Command(MasterMenuClicked);
            codes.ReadCodes();
        }

        public void Sort(object sender)
        {
            if (SortIcon == "sort_by_descending")
                SortIcon = "sort_by_ascending";
            else
                SortIcon = "sort_by_descending";
        }

        private int flg = 0;
        private void SelectCity(object sender)
        {
            flg++;
            if (flg == 1) return;
            var picker = sender as Picker;
            if (picker == null) return;
            string city = (string)picker.SelectedItem;
            codes.CurrentCity = city;
            MasterMenuClicked(new Button() { Text = codes.Type + (codes.Type == "Пиццы" ? codes.Diameter : ""), StyleId = "Yes" });
        }
        private async void OnRefreshButtonClicked(object sender)
        {
            if (!codes.IsRefreshing)
            {
                Off = false;
                string mes = await codes.RefreshAsync();
                await codes.WriteCodes();
                if (codes.Count != 0) On = true;
                if (codes.CurrentCity == null) codes.CurrentCity = "Все города";
                if (SortIcon == "sort_by_descending") CodesCollection.Reverse(codes);
                await Application.Current.MainPage.DisplayAlert("Информация", mes, "ОК");
                codes.FillReverse();
            }
        }
        private async void MasterMenuClicked(object sender)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            Button button = sender as Button;
            if (button == null) return;
            
            if (button.Text == "Все")
            {
                codes.Clear();
                if (SortIcon == "sort_by_descending")
                    foreach (var c in codes.ReverseCodes)
                    {
                        if (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города"))
                            codes.Add(c);
                    }
                else
                    foreach (var c in codes.SaveCodes)
                    {
                        if (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города"))
                            codes.Add(c);
                    }
            }
            else if(button.Text.StartsWith("Пиццы"))
            {
                string diameter;
                if (button.Text.Length == 5) diameter = await Application.Current.MainPage.DisplayActionSheet("Выберите размер", "Отмена", "", "Все диаметры", "23см", "30см", "35см", "40см");
                else
                    diameter = button.Text.Substring(5, button.Text.Length - 5);
                if (diameter == "Отмена" || diameter == null || diameter == "") return;
                codes.Clear();
                codes.Diameter = diameter;

                if (SortIcon == "sort_by_descending")
                    foreach (var c in codes.ReverseCodes)
                    {
                        if (c.IsPizza() && (c.Diameter == diameter || codes.Diameter == "Все диаметры") && (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города")))
                            codes.Add(c);
                    }
                else
                    foreach (var c in codes.SaveCodes)
                    {
                        if (c.IsPizza() && (c.Diameter == diameter || codes.Diameter == "Все диаметры") && (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города")))
                            codes.Add(c);
                    }
            }
            else if(button.Text == "Скидки")
            {
                codes.Clear();
                if (SortIcon == "sort_by_descending")
                    foreach (var c in codes.ReverseCodes)
                    {
                        if (c.IsDiscount() && (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города")))
                            codes.Add(c);
                    }
                else
                foreach (var c in codes.SaveCodes)
                    {
                        if (c.IsDiscount() && (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города")))
                            codes.Add(c);
                    }
            }
            else if(button.Text == "Другая еда")
            {
                codes.Clear();
                if (SortIcon == "sort_by_descending")
                    foreach (var c in codes.SaveCodes)
                    {
                        if (c.IsSmthElse() && (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города")))
                            codes.Add(c);
                    }
                else
                    foreach (var c in codes.SaveCodes)
                    {
                        if (c.IsSmthElse() && (codes.CurrentCity == "Все города" || c.City.Contains(codes.CurrentCity) || c.City.Contains("Все города")))
                            codes.Add(c);
                    }
            }

            if (Xamarin.Forms.Application.Current.MainPage is MasterDetailPage masterDetailPage && button.StyleId != "Yes")
            {
                masterDetailPage.IsPresented = false;
                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //await Application.Current.MainPage.DisplayAlert("Информация", (elapsedMs / 1000.0).ToString() + "s", "ОК");

            }
            if (!button.Text.StartsWith("Пиццы")) codes.Type = button.Text;
            else codes.Type = "Пиццы";
            await codes.WriteParams();
            RaisePropertyChanged("Codes");

        }
    }
}
