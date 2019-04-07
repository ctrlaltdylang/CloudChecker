using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace CloudChecker
{
    public class MapsAPI
    {
        private string RequestApi()
        {
            var response = "";
            var origins = "YOUR ORIGIN ADDRESS HERE";
            var destinations = "YOUR DESTINATINO ADDRESS HERE";
            var key = "YOUR GOOGLE MAPS API KEY HERE";
            var api = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + origins +
                "&destinations=" + destinations + "&key=" + key;

            using (var wb = new WebClient())
            {

                response = wb.DownloadString(api);
            }
            return response;
        }

        public string GetDistance()
        {
            var apiResponse = RequestApi();
            MapsApiResponseModel response = JsonConvert.DeserializeObject<MapsApiResponseModel>(apiResponse);
            var rows = response.rows[0];
            var elements = rows.elements[0];
            var duration = elements.duration.text;
            var distance = elements.distance.text;

            var mapsResponse = "Your morning commute: " + distance + "\n" + "Commute time: " + duration;
            return mapsResponse;
        }
    }
}
