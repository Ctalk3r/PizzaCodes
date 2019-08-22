using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using PiCodes.Models;

namespace PiCodes.Models
{
    public class Code : IEquatable<Code>, IElement, INotifyPropertyChanged
    {
        private string name, shortName, note;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(Code other)
        {
            return this.FullInfo == other.FullInfo ? true : false;
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name == value)
                    return;
                name = value;
                RaisePropertyChanged();
            }
        }

        public string ShortName
        {
            get { return shortName; }
            set
            {
                if (shortName == value)
                    return;
                shortName = value;
                RaisePropertyChanged();
            }
        }

        public string Note
        {
            get { return note; }
            set
            {
                if (note == value)
                    return;
                note = value;
                RaisePropertyChanged();
            }
        }

        public string[] City { get; private set; }
        public string Diameter { get; private set; }
        public double Price { get; private set; }
        public string ShortInfo  { get; private set; }
        public string FullInfo => $"Имя - {Name}\nДоп. информация - {Note}";

        public bool IsPizza()
        {
            return FullInfo.Contains("ТРАД") || FullInfo.Contains("ТОНК");
        }

        public bool IsDiscount()
        {
            return FullInfo.Contains("%");
        }

        public bool IsSmthElse()
        {
            return !IsPizza() && !IsDiscount();
        }

        public string GetInfo(bool shortInfo)
        {
            if (shortInfo)
                return ShortInfo;
            else
                return FullInfo;
        }

        public Code(string name, string note)
        {              
            List<string> temp = name.Split(',').ToList();     
            for (int i = 0; i < temp.Count(); i++)
                temp[i] = temp[i].Trim();
            Name = string.Join("\n", temp);
            ShortName = temp.OrderBy((x) => x.Length).ToList()[0];
            Note = note;
            Price = double.Parse(FullInfo.Substring(FullInfo.LastIndexOf("От", StringComparison.InvariantCultureIgnoreCase) + 3,
                                 FullInfo.LastIndexOf("руб", StringComparison.InvariantCultureIgnoreCase) -
                                (FullInfo.LastIndexOf("От", StringComparison.InvariantCultureIgnoreCase) + 4)).Replace(',','.'), CultureInfo.InvariantCulture);

            if (!IsPizza()) Diameter = "";
            else
            if (FullInfo.IndexOf("ТРАД", StringComparison.InvariantCultureIgnoreCase) != -1)
                Diameter = FullInfo.Substring(FullInfo.IndexOf("ТРАД", StringComparison.InvariantCultureIgnoreCase) - 2, 2) + "см";
            else Diameter = FullInfo.Substring(FullInfo.IndexOf("ТОНК", StringComparison.InvariantCultureIgnoreCase) - 2, 2) + "см";

            City = FullInfo.Substring(FullInfo.LastIndexOf("-", StringComparison.InvariantCultureIgnoreCase) + 2).Split(',', '.');
            for (int i = 0; i < City.Count(); i++)
                City[i] = City[i].Trim();

            ShortInfo = Note.Substring(0, Note.IndexOf("-", StringComparison.InvariantCultureIgnoreCase));
            //ShortInfo = ShortInfo.Length < 25 ? ShortInfo : ShortInfo.Substring(0, 25) + "...";
        }
    }
}
