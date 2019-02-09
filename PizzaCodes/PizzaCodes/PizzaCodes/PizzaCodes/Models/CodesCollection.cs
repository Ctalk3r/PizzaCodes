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

namespace PizzaCodes.Models
{
    public class CodesCollection : BaseCollection<Code>
    {
        public static string RequestAdress = "https://www.papajohns.by/api/stock/codes";
        private bool isRefreshing;
        public bool IsRefreshing => isRefreshing;
  
        public CodesCollection()
        {
            elements = new Code[] { };
            isRefreshing = false;
        }

        public async Task<string> RefreshAsync()
        {
            if (isRefreshing) return "";
            if (!CrossConnectivity.Current.IsConnected) return "Нет подключения к сети";
            isRefreshing = true;
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
                        AddCodeAsync(match);
                    }            
                    isRefreshing = false;
                    return "Коды загружены";
                }
                else
                {
                    isRefreshing = false;
                    return "Кодов нет";
                }
            }
            else
            {
                isRefreshing = false;
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
    }
}
