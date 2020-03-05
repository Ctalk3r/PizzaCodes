using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PiCodes.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;

namespace PiCodes.Models
{
    public class CodesCollection : ObservableCollection<Code>
    {
        public static string RequestAdress = "https://www.papajohns.by/api/stock/codes";
        public ObservableCollection<string> CityList { get; set; }
        public string CombinedType => Type + (Type == "Пиццы" && Diameter.Length == 4 ? " " + Diameter : "");
        public List<Code> SaveCodes;
        public List<Code> ReverseCodes;
        protected override event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Type { get; set; } = "Все";

        public string Diameter { get; set; } = "Все диаметры";
        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get => isRefreshing;

            protected set
            {
                if (value != isRefreshing)
                {
                    isRefreshing = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string city;
        public string City
        {
            get => city;

            protected set
            {
                if (value != city)
                {
                    city = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string currentCity = "Все города";
        public string CurrentCity
        {
            get => currentCity;

            set
            {
                if (value != currentCity)
                {
                    currentCity = value;
                    RaisePropertyChanged();
                }
            }
        }

        static string codesFile = "codes.json";
        static string paramsFile = "params.txt";

        public CodesCollection()
        {
            IsRefreshing = false;
            CityList = new ObservableCollection<string>();
            SaveCodes = new List<Code>();          
            ReverseCodes = new List<Code>();          
        }

        public async Task<string> RefreshAsync()
        {
            if (IsRefreshing) return "";
            if (!CrossConnectivity.Current.IsConnected)
            {
                RaisePropertyChanged("IsRefreshing");
                return "Нет подключения к сети";
            }
            IsRefreshing = true;
            Clear();
            SaveCodes.Clear();
            if(CityList.Count == 0) CityList.Add("Все города");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(RequestAdress);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent responseContent = response.Content;
                var json = await responseContent.ReadAsStringAsync();
                json = json.Trim();
                Regex regex = new Regex(@"""name"":""\d+[^""]*""", RegexOptions.Compiled);
                MatchCollection matches = regex.Matches(json);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        await AddCodeAsync(match);
                    }
                    Sort();
                    foreach (Code temp in SaveCodes)
                        if ((CurrentCity == null || temp.City.Contains(CurrentCity) || CurrentCity == "Все города") &&
                        ((Type == null) || (Type == "Все") || (Type == "Пиццы" && temp.IsPizza() && (temp.Diameter == Diameter || Diameter == "Все диаметры"))
                        || (Type == "Скидки" && temp.IsDiscount()) || (Type == "Другая еда" && temp.IsSmthElse())))
                            Add(temp);
                    IsRefreshing = false;
                    return "Коды загружены";
                }
                else
                {
                    IsRefreshing = false;
                    return "Кодов нет";
                }
            }
            else
            {
                IsRefreshing = false;
                return "Не удалось загрузить коды";
            }
        }

        public static void Reverse(CodesCollection codes)
        {
            Code a;
            for (int i = 0; i < codes.Count / 2; i++)
            {
                a = codes[i];
                codes[i] = codes[codes.Count - i - 1];
                codes[codes.Count - i - 1] = a;
            }
        }

        public void FillReverse()
        {
            ReverseCodes = Enumerable.Reverse(SaveCodes).ToList();
        }

        public void Sort(bool isReverse = false)
        {

            if (isReverse == false)
                SaveCodes = SaveCodes.OrderBy(code => code.Price).ThenBy(code => code.Name).ToList();
            else
                SaveCodes = SaveCodes.OrderByDescending(code => code.Price).ThenBy(code => code.Name).ToList();
        }
        private async Task AddCodeAsync(Match match)
        {
           await Task.Run(() =>
           {
               string output;

               string pattern = @"""name"":""\d*[ ]?-[ ]?";
               Regex reg = new Regex(pattern);
               output = reg.Replace(match.Value, "");
               pattern = @"[^-]*- ";
               reg = new Regex(pattern);
               Match cur = reg.Match(output);

               string code;
               int curLength = cur.Value.Length;
               string note;
               code = cur.Value.Substring(0, curLength - 3);
               note = output.Substring(curLength);
               note = note.Substring(0, note.Length - 1);
               Code temp = new Code(Regex.Unescape(code), Regex.Unescape(note));
               SaveCodes.Add(temp);
               Device.BeginInvokeOnMainThread(() =>
               {
                   if (temp.City.Count() != 0)
                       foreach (var city in temp.City)
                           if (!CityList.Contains(city)) CityList.Add(city);
               });
           });
        }

        public async Task WriteCodes()
        {
            string serialized = JsonConvert.SerializeObject(SaveCodes);
            await DependencyService.Get<IFileWorker>().SaveTextAsync(codesFile, serialized);   
        }
        public async Task WriteParams()
        {
            await DependencyService.Get<IFileWorker>().SaveTextAsync(paramsFile, CurrentCity + ',' + Type + ',' + Diameter);        
        }

        public async Task<bool> IsCodesFile()
        {
            return await DependencyService.Get<IFileWorker>().ExistsAsync(codesFile);
        }
        public async Task ReadCodes()
        {
            CurrentCity = "Все города";
            if (await DependencyService.Get<IFileWorker>().ExistsAsync(codesFile))
            {
                string text = await DependencyService.Get<IFileWorker>().LoadTextAsync(codesFile);
                if (text == null) return;
                if (CityList.Count == 0) CityList.Add("Все города");
                foreach (var i in JsonConvert.DeserializeObject<List<Code>>(text))
                {
                    foreach (var city in i.City)
                        if (!CityList.Contains(city)) CityList.Add(city);
                }

                if (await DependencyService.Get<IFileWorker>().ExistsAsync(paramsFile))
                { 
                    string text2 = await DependencyService.Get<IFileWorker>().LoadTextAsync(paramsFile);
                    string[] parameters = text2.Split(',');
                    CurrentCity = parameters[0];
                    Type = parameters[1];
                    RaisePropertyChanged("Type");
                    RaisePropertyChanged("CombinedType");
                    Diameter = parameters[2];
                }

                foreach (var i in JsonConvert.DeserializeObject<CodesCollection>(text))
                {
                    SaveCodes.Add(i);
                    if ((i.City.Contains(CurrentCity) || i.City.Contains("Все города") || CurrentCity == "Все города") &&
                       ((Type == null) || (Type == "Все") || (Type == "Пиццы" && i.IsPizza() && (i.Diameter == Diameter || Diameter == "Все диаметры"))
                       || (Type == "Скидки" && i.IsDiscount()) || (Type == "Другая еда" && i.IsSmthElse()))) Add(i);
                }
                FillReverse();
            }
        }
    }
}
