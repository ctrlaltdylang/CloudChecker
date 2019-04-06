using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CloudChecker
{
    public static class CloudChecker
    {
        [FunctionName("CloudChecker")]
        public static void Run([TimerTrigger("0 15 5 * * *")]TimerInfo myTimer, ILogger log)
        {
            #region API Call
            var response = "";
            var location = "4076598"; // <- change location id here, from https://openweathermap.org
            var apiKey = "Your Api Key Here"; // <- API key from https://openweathermap.org
            var api = "https://api.openweathermap.org/data/2.5/weather?id=" + location + "&appid=" + apiKey + "&units=imperial";

            using (var wb = new WebClient())
            {

                response = wb.DownloadString(api);
            }

            #endregion

            #region Regex and String manipulation

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

            #endregion

            string body = "The weather today in " + city + " is: " + weather + "\n" + "Min Temp: " + minTemp + "°F \n" + "Max Temp: " + maxTemp + "°F";

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("Your Email Address Here", "Your Password Here");

            MailMessage mm = new MailMessage("from@emailhere", "to@emailhere", "CloudChecker Forecast", body);
            mm.BodyEncoding = Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            
        }
    }
}
