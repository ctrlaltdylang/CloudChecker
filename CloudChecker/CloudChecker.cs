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
        public static void Run([TimerTrigger("0 30 5 * * *")]TimerInfo myTimer, ILogger log)
        {
            //TODO: integrate Twilio for sending text message instead of email

            WeatherAPI weatherApi = new WeatherAPI();
            MapsAPI mapsApi = new MapsAPI();

            string weatherBody = weatherApi.GetWeather();
            string mapsBody = mapsApi.GetDistance();

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("your email here", "your email password here");

            var message = new MailMessage();

            message.To.Add(new MailAddress("to email here"));
            message.From = new MailAddress("from email here");
            message.Subject = "CloudChecker Weather Forecast";
            message.Body = weatherBody + mapsBody; 
            client.Send(message);
            
        }
    }
}
