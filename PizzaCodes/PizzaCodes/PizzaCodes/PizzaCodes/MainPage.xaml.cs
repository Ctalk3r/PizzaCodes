using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using PizzaCodes.Models;
using Newtonsoft.Json;

namespace PizzaCodes
{

    public partial class MainPage : MasterDetailPage
    {
        string filename = "codes.txt";
        public CodesCollection codes { get; set; }
		public MainPage()
		{
            codes = new CodesCollection();
            this.BindingContext = this;
            InitializeComponent();
            this.IsPresentedChanged += MasterOpened;
            Binding priceBinding = new Binding { Source = codes, Path = "MaxPrice" };
            slider.SetBinding(Slider.MaximumProperty, priceBinding);
        }

        private async void CodeListItemTapped(object sender, ItemTappedEventArgs e)
        {
            Code selected = e.Item as Code;
            await DisplayAlert("Информация", selected.FullInfo, "OK");
            ((ListView)sender).SelectedItem = null;
        }

        private async void OnClearButtonClicked(object sender, EventArgs e)
        {
            codes.Clear();
            await DisplayAlert("Информация", "Список очищен", "ОК");
        }

        private async void OnRefreshButtonClicked(object sender, EventArgs e)
        {          
            if (!codes.IsRefreshing)
            {
                string mes;
                mes = await codes.RefreshAsync();
                await WriteCodes();
                codesCollection.IsRefreshing = false;
                await DisplayAlert("Информация", mes, "ОК");
                slider.Maximum = codes.MaxPrice;
                slider.Value = slider.Maximum;
            }
        }

        private async Task WriteCodes()
        {
            string serialized = JsonConvert.SerializeObject(codes);
            await DependencyService.Get<IFileWorker>().SaveTextAsync(filename, serialized);
        }

        private async Task ReadCodes()
        {
            if (await DependencyService.Get<IFileWorker>().ExistsAsync(filename))
            {
                string text = await DependencyService.Get<IFileWorker>().LoadTextAsync(filename);
                var tempCodes  = JsonConvert.DeserializeObject<CodesCollection>(text);
                foreach(var i in tempCodes)
                    codes.Add(i);
            }
        }

        bool flg = false;
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (flg) return;
            await ReadCodes();
            slider.Maximum = codes.MaxPrice;
            slider.Value = slider.Maximum;
            flg = true;
        }

        protected void Sort(object sender, ItemTappedEventArgs e)
        {
            List<Code> sortedCodes;
            if((string)Resources["SortPicture"] == "Sort.png")
            {
                sortedCodes = codes.OrderBy(code => code.Price).ToList();
                Resources["SortPicture"] = "SortDescending.png";
            }
            else
            {
                sortedCodes = codes.OrderByDescending(code => code.Price).ToList();
                Resources["SortPicture"] = "Sort.png";
            }
            
            codes.Clear();
            foreach (var i in sortedCodes)
                codes.Add(i);
        }

        private void MasterOpened(object sender, EventArgs e)
        {
            if(IsPresented == false)
            {
                foreach (var code in codes)
                {
                    if (code.Price > slider.Value)
                        codes.Remove(code);
                }
            }
        }

        private async void OnAllCodesClicked(object sender, EventArgs e)
        {
            await ReadCodes();
            var t = slider.Value;
            slider.Maximum = codes.MaxPrice;
            slider.Value = t;
        }

        private void OnOnlyPizzaClicked(object sender, EventArgs e)
        {
            PizzaPicker.Focus();
        }

        private async void PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            codes.Clear();
            await ReadCodes();
     
            foreach (var code in codes)
            {
                if (!code.IsPizza() || (PizzaPicker.Items[PizzaPicker.SelectedIndex] != "Все" && code.Diameter != PizzaPicker.Items[PizzaPicker.SelectedIndex]))
                    codes.Remove(code);
            }
            var t = slider.Value;
            slider.Maximum = codes.MaxPrice;
            slider.Value = t;
        }

        private async void OnOnlyDiscountClicked(object sender, EventArgs e)
        {
            codes.Clear();
            await ReadCodes();
            foreach (var code in codes)
            {
                if (!code.IsDiscount())
                    codes.Remove(code);
            }
            var t = slider.Value;
            slider.Maximum = codes.MaxPrice;
            slider.Value = t;
        }

        private async void OnOnlyElseClicked(object sender, EventArgs e)
        {
            codes.Clear();
            await ReadCodes();
            foreach (var code in codes)
            {
                if(!code.IsSmthElse())
                    codes.Remove(code);
            }
            var t = slider.Value;
            slider.Maximum = codes.MaxPrice;
            slider.Value = t;
        }
    }
}
