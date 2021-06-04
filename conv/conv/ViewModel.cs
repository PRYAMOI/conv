using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace conv
{
    class ViewModel : INotifyPropertyChanged
    {
        JObject json;

        ObservableCollection<string> valuteCodesList;

        string firstVal = "";
        string secondVal = "";

        int Index1 = 35; // RUB
        int Index2 = 11; // USD

        private double ValuePerNominal(string code)
        {
            JToken token = json["Valute"][code];
            return token.Value<double>("Value") / token.Value<double>("Nominal");
        }

        public ViewModel()
        {
            using (WebClient client = new WebClient())
            {
                valuteCodesList = new ObservableCollection<string>();
                string html = client.DownloadString("https://www.cbr-xml-daily.ru/daily_json.js");
                json = JObject.Parse(html);
                json["Valute"]["RUB"] = new JObject() { new JProperty("CharCode", "RUB"), new JProperty("Value", 1), new JProperty("Nominal", 1) };
                foreach (JToken valute in json["Valute"])
                {
                    foreach (JToken t in valute)
                    {
                        valuteCodesList.Add(t.Value<string>("CharCode"));
                    }
                }
            }
        }

        public ObservableCollection<string> ValuteCodes
        {
            get { return valuteCodesList; }
        }


        public int FirstValuteCodeIndex
        {
            get { return Index1; }
            set
            {
                if (Index1 != value)
                {
                    Index1 = value;
                    OnPropertyChanged(nameof(FirstValuteCodeIndex));

                    // Update target valute
                    SecondValute = secondVal;
                }
            }
        }

        public int SecondValuteCodeIndex
        {
            get { return Index2; }
            set
            {
                if (Index2 != value)
                {
                    Index2 = value;
                    OnPropertyChanged(nameof(SecondValuteCodeIndex));

                    // Upate target valute
                    FirstValute = firstVal;
                }
            }
        }

        public string FirstValute
        {
            get { return firstVal; }
            set
            {
                firstVal = value;

                if (value == "")
                {
                    ValUnfoc2 = "";
                    OnPropertyChanged(nameof(FirstValute));
                    return;
                }
                else if (value == ",")
                {
                    firstVal = "0" + value;
                }
                else if (value == "-")
                {
                    return;
                }

                string firstCode = ValuteCodes[FirstValuteCodeIndex];
                string secondCode = ValuteCodes[SecondValuteCodeIndex];

                string converted = (double.Parse(firstVal) * ValuePerNominal(firstCode) / ValuePerNominal(secondCode)) + "";
                if (converted.Contains(","))
                {
                    if (converted.Length - converted.IndexOf(",") > 3)
                    {
                        converted = converted.Substring(0, converted.IndexOf(",") + 3);
                    }
                }

                ValUnfoc2 = converted;
                firstVal = value;
                OnPropertyChanged(nameof(FirstValute));
            }
        }

        public string ValUnfoc1
        {
            get { return firstVal; }
            set
            {
                firstVal = value;
                OnPropertyChanged(nameof(FirstValute));
            }
        }

        public string SecondValute
        {
            get { return secondVal; }
            set
            {
                secondVal = value;

                if (value == "")
                {
                    ValUnfoc1 = "";
                    OnPropertyChanged(nameof(SecondValute));
                    return;
                }
                else if (value == ",")
                {
                    secondVal = "0" + value;
                }
                else if (value == "-")
                {
                    return;
                }

                string firstCode = ValuteCodes[FirstValuteCodeIndex];
                string secondCode = ValuteCodes[SecondValuteCodeIndex];

                string converted = (double.Parse(secondVal) * ValuePerNominal(secondCode) / ValuePerNominal(firstCode)) + "";
                if (converted.Contains(","))
                {
                    if (converted.Length - converted.IndexOf(",") > 3)
                    {
                        converted = converted.Substring(0, converted.IndexOf(",") + 3);
                    }
                }

                ValUnfoc1 = converted;
                secondVal = value;
                OnPropertyChanged(nameof(SecondValute));
            }
        }
        public string ValUnfoc2
        {
            get { return secondVal; }
            set
            {
                secondVal = value;
                OnPropertyChanged(nameof(SecondValute));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
