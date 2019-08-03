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

namespace PiCodes.Models
{
    public class CodesCollection : ObservableCollection<Code>
    {
        public static string RequestAdress = "https://www.papajohns.by/api/stock/codes";
        public ObservableCollection<string> CityList { get; set; }
        protected override event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        private string city = "Все города";
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

        string codesFile = "codes.txt";
        public CodesCollection()
        {
            IsRefreshing = false;
            CityList = new ObservableCollection<string>();
        }

        public async Task<string> RefreshAsync()
        {
            if (IsRefreshing) return "";
            if (!CrossConnectivity.Current.IsConnected) return "Нет подключения к сети";
            IsRefreshing = true;
            Clear();
            CityList.Clear();
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

        private async Task AddCodeAsync(Match match)
        {
           await Task.Run(() =>
           {
               string output;
               string pattern = @"""name"":""\d* - ";
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
               Add(temp);
               foreach (var city in temp.City)
                   if (!CityList.Contains(city)) CityList.Add(city);
           });
        }
        public async Task WriteCodes()
        {
            string serialized = JsonConvert.SerializeObject(this);
            await DependencyService.Get<IFileWorker>().SaveTextAsync(codesFile, serialized);
        }

        public async Task ReadCodes()
        {
            if (await DependencyService.Get<IFileWorker>().ExistsAsync(codesFile))
            {
                string text = await DependencyService.Get<IFileWorker>().LoadTextAsync(codesFile);
                if (text == null) return;
                foreach (var i in JsonConvert.DeserializeObject<CodesCollection>(text))
                {
                    Add(i);
                    foreach(var city in i.City)
                      if (!CityList.Contains(city)) CityList.Add(city);
                }
                    
            }
        }
    }
}
