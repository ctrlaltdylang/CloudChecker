using System.Net;
using System.Text.RegularExpressions;

namespace CloudChecker
{
    public class WeatherAPI
    {
        //TODO: create data model to JsonDeserialize to instead of using regex
        public string GetWeather()
        {
            var response = "";
            var location = "YOUR LOCATION ID HERE"; // <- change location id here, from https://openweathermap.org
            var apiKey = "YOUR API KEY HERE";  // <- API key from https://openweathermap.org
            var api = "https://api.openweathermap.org/data/2.5/weather?id=" + location + "&appid=" + apiKey + "&units=imperial";

            using (var wb = new WebClient())
            {

                response = wb.DownloadString(api);
            }

            return RegexResponse(response);
        }

        private string RegexResponse(string response)
        {
            // Gets weather forecast type (cloudy, rain, etc.)
            var regex = new Regex(@".description...[^,]+");
            Match pattern = regex.Match(response);
            var match = pattern.ToString();
            string[] words = match.Split(':');
            var weather = "";
            foreach (var word in words)
            {
                if (!word.Contains("description"))
                {
                    weather = word;
                    weather = weather.Replace("\"", string.Empty).Trim();
                }
            }

            // Gets Min Temp in °F
            var minTempRegex = new Regex(@".temp_min...[^,]+");
            Match minTempPattern = minTempRegex.Match(response);
            var minTempMatch = minTempPattern.ToString();
            string[] unsplitMinTemp = minTempMatch.Split(':');
            var minTemp = "";
            foreach (var word in unsplitMinTemp)
            {
                if (!word.Contains("temp_min"))
                {
                    minTemp = word;
                    minTemp = minTemp.Replace("\"", string.Empty).Trim();
                }
            }

            // Gets Max Temp in °F
            var maxTempRegex = new Regex(@".temp_max...[^,]+");
            Match maxTempPattern = maxTempRegex.Match(response);
            var maxTempMatch = maxTempPattern.ToString();
            string[] unsplitMaxTemp = maxTempMatch.Split(':');
            var maxTemp = "";
            foreach (var word in unsplitMaxTemp)
            {
                if (!word.Contains("temp_max"))
                {
                    maxTemp = word;
                    maxTemp = maxTemp.Replace("\"", string.Empty).Trim();
                    maxTemp = maxTemp.Replace("}", string.Empty).Trim();
                }
            }

            // City name from api call
            var cityRegex = new Regex(@".name...[^,]+");
            Match cityPattern = cityRegex.Match(response);
            var cityMatch = cityPattern.ToString();
            string[] unsplitCity = cityMatch.Split(':');
            var city = "";
            foreach (var word in unsplitCity)
            {
                if (!word.Contains("name"))
                {
                    city = word;
                    city = city.Replace("\"", string.Empty).Trim();
                }
            }

            string regexResponse = "The weather today in " + city + " is: " + weather + "\n" + "Min Temp: " + minTemp + "°F \n" + "Max Temp: " + maxTemp + "°F \n";
            return regexResponse;
        }
    }
}
