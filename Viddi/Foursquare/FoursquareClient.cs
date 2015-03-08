using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Viddi.Core;

namespace Viddi.Foursquare
{
    public class FoursquareClient
    {
        private readonly HttpClient _httpClient;

        public FoursquareClient()
        {
            _httpClient = new HttpClient(new HttpClientHandler {AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip});
        }

        public async Task<List<Venue>> GetVenuesAsync(double longitude, double latitude)
        {
            var areaDetails = await GetGeneralArea(longitude, latitude);
            var url = GetSearchUrl(longitude, latitude);

            var venues = await GetVenues(url);

            areaDetails.AddRange(venues);

            return areaDetails;
        }

        private Task<List<Venue>> GetGeneralArea(double longitude, double latitude)
        {
            var url = GetSearchGeneralAreaUrl(longitude, latitude);

            return GetVenues(url);
        }

        private async Task<List<Venue>> GetVenues(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Venue>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var searchResults = JsonConvert.DeserializeObject<SearchResponse>(json);

            if (searchResults != null && searchResults.Response != null)
            {
                var group = searchResults.Response;
                return @group.Venues ?? new List<Venue>();
            }

            return new List<Venue>();
        }


        private static string GetSearchUrl(double longitude, double latitude)
        {
            return string.Format(Constants.FourSquareSearchUrl, Constants.FourSquareClientId, Constants.FourSquareClientSecret, latitude, longitude);
        }

        private static string GetSearchGeneralAreaUrl(double longitude, double latitude)
        {
            return string.Format(Constants.FourSquareGeneralAreaUrl, Constants.FourSquareClientId, Constants.FourSquareClientSecret, latitude, longitude);
        }
    }
}
