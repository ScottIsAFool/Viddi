using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Viddy.Foursquare
{
    [DebuggerDisplay("Name: {Name}, Id = {Id}")]
    public class Venue
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Response
    {
        [JsonProperty("venues")]
        public List<Venue> Venues { get; set; }
    }

    public class SearchResponse
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }
}
