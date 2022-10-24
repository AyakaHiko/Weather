using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Weather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void findBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var data = WeatherServer.GetWeather(cityBox.Text.Trim());
                if (InvokeRequired)
                    Invoke(new Action(() => _fillWeather(data)));
                else
                {
                 _fillWeather(data);   
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void _fillWeather(Weather data)
        {
            pressureInfo.Text = data.Pressure.ToString();
            humidityInfo.Text = data.Humidity.ToString();
            temperatureInfo.Text = data.Temperature.ToString();

        }
        
    }

    public class WeatherServer
    {
        private static string _apiKey = "8f3711470cbfa8043c11f3075e50619e";


        private static string _request(string city)
        {
            string api = $@"https://api.openweathermap.org/data/2.5/weather?q={city}&mode=xml&units=metric&appid={_apiKey}";

            HttpWebRequest request = WebRequest.CreateHttp(api);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
                return sb.ToString();
            }
        }

        public static Weather GetWeather(string city)
        {
            var stringRequest = _request(city);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(stringRequest);
            var root = doc.DocumentElement;
            Weather weather= new Weather{City = city};
            foreach (XmlNode node in root.ChildNodes)
            {
                var value = node?.Attributes["value"]?.Value;
                switch (node.Name)
                {
                    case "temperature":
                        var form = new NumberFormatInfo
                        {
                            NumberDecimalSeparator = "."
                        };
                        weather.Temperature = Convert.ToDecimal(value, form);
                        break;
                    case "humidity":
                        weather.Humidity = Convert.ToInt32(value);
                        break;
                    case "pressure":
                        weather.Pressure = Convert.ToInt32(value);
                        break;
                }
            }
            return weather;
        }
    }

    public class Weather
    {
        public string City { get; set; }
        public decimal Temperature { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
    }
}
