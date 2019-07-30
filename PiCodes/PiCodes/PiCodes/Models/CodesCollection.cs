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

        string codesFile = "codes.txt";
        public CodesCollection()
        {
            IsRefreshing = false;
        }

        public async Task<string> RefreshAsync()
        {
            if (IsRefreshing) return "";
            if (!CrossConnectivity.Current.IsConnected(0)) return "Нет подключения к сети";
            IsRefreshing = true;
            Clear();
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
               Add(new Code(Regex.Unescape(code), Regex.Unescape(note)));
           });
        }
        public async Task WriteCodes()
        {
            string serialized = JsonConvert.SerializeObject(this);
            await DependencyService.Get<IFileWorker>().SaveTextAsync(codesFile, serialized);
        }
    }
}
