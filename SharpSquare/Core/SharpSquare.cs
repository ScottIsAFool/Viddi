using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FourSquare.SharpSquare.Entities;
using Newtonsoft.Json;

namespace FourSquare.SharpSquare.Core
{
    public class SharpSquare
    {
        public const string AuthenticateUrl = "https://foursquare.com/oauth2/authenticate";
        public const string AccessTokenUrl = "https://foursquare.com/oauth2/access_token";
        public const string ApiUrl = "https://api.foursquare.com/v2";
        public const string ApiVersion = "20140101";

        private readonly string clientId;
        private readonly string clientSecret;

        private readonly HttpClient httpClient;
        private readonly HttpMessageHandler httpMessageHandler = new HttpClientHandler();
        private string accessToken;

        public SharpSquare(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            this.httpClient = new HttpClient(this.httpMessageHandler);
        }

        public SharpSquare(string clientId, string clientSecret, string accessToken)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.accessToken = accessToken;

            this.httpClient = new HttpClient(this.httpMessageHandler);
        }

        private async Task<string> Request(string url, HttpMethod httpMethod)
        {
            return await this.Request(url, httpMethod, null);
        }

        private async Task<string> Request(string url, HttpMethod httpMethod, string data)
        {
            if (httpMethod == HttpMethod.Get)
            {
                return await this.httpClient.GetStringAsync(url);
            }

            if (httpMethod == HttpMethod.Post)
            {
                HttpResponseMessage response = await this.httpClient.PostAsync(url, new StringContent(data));
                return await response.Content.ReadAsStringAsync();
            }

            return string.Empty;
        }

        private string SerializeDictionary(Dictionary<string, string> dictionary)
        {
            var parameters = new StringBuilder();

            foreach (var keyValuePair in dictionary)
            {
                parameters.Append(keyValuePair.Key + "=" + keyValuePair.Value + "&");
            }

            return parameters.Remove(parameters.Length - 1, 1).ToString();
        }

        public async Task<FourSquareSingleResponse<T>> GetSingle<T>(string endpoint, Dictionary<string, string> parameters = null, bool unauthenticated = false)
            where T : FourSquareEntity
        {
            string serializedParameters = "";

            if (parameters != null)
            {
                serializedParameters = "&" + this.SerializeDictionary(parameters);
            }

            string oauthToken;
            if (unauthenticated)
            {
                oauthToken = string.Format("client_id={0}&client_secret={1}", this.clientId, this.clientSecret);
            }
            else
            {
                oauthToken = string.Format("oauth_token={0}", this.accessToken);
            }

            string json = await this.Request(string.Format("{0}{1}?{2}{3}&v={4}", ApiUrl, endpoint, oauthToken, serializedParameters, ApiVersion), HttpMethod.Get);
            var fourSquareResponse = JsonConvert.DeserializeObject<FourSquareSingleResponse<T>>(json);
            return fourSquareResponse;
        }

        public async Task<FourSquareSingleRootResponse<T>> GetSingleRoot<T>(string endpoint, Dictionary<string, string> parameters = null, bool unauthenticated = false)
            where T : FourSquareEntity
        {
            string serializedParameters = "";

            if (parameters != null)
            {
                serializedParameters = "&" + this.SerializeDictionary(parameters);
            }

            string oauthToken;
            if (unauthenticated)
            {
                oauthToken = string.Format("client_id={0}&client_secret={1}", this.clientId, this.clientSecret);
            }
            else
            {
                oauthToken = string.Format("oauth_token={0}", this.accessToken);
            }

            string json = await this.Request(string.Format("{0}{1}?{2}{3}&v={4}", ApiUrl, endpoint, oauthToken, serializedParameters, ApiVersion), HttpMethod.Get);
            var fourSquareResponse = JsonConvert.DeserializeObject<FourSquareSingleRootResponse<T>>(json);
            return fourSquareResponse;
        }
        
        public async Task<FourSquareMultipleResponse<T>> GetMultiple<T>(string endpoint, Dictionary<string, string> parameters = null, bool unauthenticated = false)
            where T : FourSquareEntity
        {
            string serializedParameters = "";
            if (parameters != null)
            {
                serializedParameters = "&" + this.SerializeDictionary(parameters);
            }

            string oauthToken;
            if (unauthenticated)
            {
                oauthToken = string.Format("client_id={0}&client_secret={1}", this.clientId, this.clientSecret);
            }
            else
            {
                oauthToken = string.Format("oauth_token={0}", this.accessToken);
            }

            string json =
                await this.Request(string.Format("{0}{1}?{2}{3}&v={4}", ApiUrl, endpoint, oauthToken, serializedParameters, ApiVersion), HttpMethod.Get);
            var fourSquareResponse = JsonConvert.DeserializeObject<FourSquareMultipleResponse<T>>(json);
            return fourSquareResponse;
        }

        private async Task Post(string endpoint, Dictionary<string, string> parameters = null)
        {
            string serializedParameters = "";

            if (parameters != null)
            {
                serializedParameters = "&" + this.SerializeDictionary(parameters);
            }

            await this.Request(string.Format("{0}{1}?oauth_token={2}{3}", ApiUrl, endpoint, this.accessToken, serializedParameters), HttpMethod.Post);
        }

        private async Task<FourSquareSingleResponse<T>> Post<T>(string endpoint, Dictionary<string, string> parameters = null) where T : FourSquareEntity
        {
            string serializedParameters = "";
            if (parameters != null)
            {
                serializedParameters = "&" + this.SerializeDictionary(parameters);
            }

            string json =
                await this.Request(string.Format("{0}{1}?oauth_token={2}{3}", ApiUrl, endpoint, this.accessToken, serializedParameters), HttpMethod.Post);
            var fourSquareResponse = JsonConvert.DeserializeObject<FourSquareSingleResponse<T>>(json);
            return fourSquareResponse;
        }

        public string GetAuthenticateUrl(string redirectUri, string responseType = "code")
        {
            return string.Format("{0}?client_id={1}&response_type={3}&redirect_uri={2}", AuthenticateUrl, this.clientId, redirectUri, responseType);
        }

        public async Task<string> GetAccessToken(string redirectUri, string code)
        {
            string url = string.Format("{0}?client_id={1}&client_secret={2}&grant_type=authorization_code&redirect_uri={3}&code={4}", AccessTokenUrl,
                this.clientId, this.clientSecret, redirectUri, code);
            string json = await this.Request(url, HttpMethod.Get);
            string token = JsonConvert.DeserializeObject<dynamic>(json).access_token;
            this.SetAccessToken(token);
            return token;
        }

        public void SetAccessToken(string fsAccessToken)
        {
            this.accessToken = fsAccessToken;
        }

        //Venue
        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID
        ///     Gives details about a venue, including location, mayorship, tags, tips, specials, and category.
        ///     Authenticated users will also receive information about who is here now.
        ///     If the venue ID given is one that has been merged into another "master" venue, the response will show data about
        ///     the "master" instead of giving you an error.
        /// </summary>
        public async Task<Venue> GetVenue(string venueId)
        {
            return (await this.GetSingle<Venue>("/venues/" + venueId, unauthenticated: true)).response["venue"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/add
        ///     Allows users to add a new venue.
        ///     If this method returns an error, give the user the option to edit her inputs. The method may return an HTTP 409
        ///     error if the new venue looks like a duplicate of an existing venue. This response will include two useful values:
        ///     candidateDuplicateVenues and ignoreDuplicatesKey. In this situation we recommend you try these two options: 1) use
        ///     one of the candidateDuplicateVenues included in the response of the 409 error, or 2) ignore duplicates and force
        ///     the addition of a new venue by resubmitting the same venue add request with two additional parameters:
        ///     ignoreDuplicates set to true and ignoreDuplicatesKey set to the value from the earlier error response.
        ///     In addition to this, give users the ability to say "never mind, check-in here anyway" and perform a manual
        ///     ("venueless") checkin by specifying just the venue name
        ///     All fields are optional, but one of either a valid address or a geolat/geolong pair must be provided. We recommend
        ///     that developers provide a geolat/geolong pair in every case.
        ///     Caller may also, optionally, pass in a category (primarycategoryid) to which you want this venue assigned. You can
        ///     browse a full list of categories using the /categories method. On adding venue, we recommend that applications show
        ///     the user this hierarchy and allow them to choose something suitable.
        /// </summary>
        public async Task<Venue> AddVenue(Dictionary<string, string> parameters)
        {
            return (await this.Post<Venue>("/venues/add", parameters)).response["venue"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/categories
        ///     Returns a hierarchical list of categories applied to venues.
        ///     When designing client applications, please download this list only once per session, but also avoid caching this
        ///     data for longer than a week to avoid stale information.
        ///     This endpoint is part of the venues API (https://developer.foursquare.com/overview/venues.html).
        /// </summary>
        public async Task<List<Category>> GetVenueCategories()
        {
            return (await this.GetMultiple<Category>("/venues/categories", unauthenticated: true)).response["categories"];
        }

        public async Task<List<FourSquareEntityItems<VenueExplore>>> ExploreVenues(Dictionary<string, string> parameters)
        {
            var result = await this.GetSingleRoot<FourSquareEntityExploreVenuesGroups<VenueExplore>>("/venues/explore", parameters, true);
            return result.response.groups;
        }
        
        /// <summary>
        ///     https://api.foursquare.com/v2/venues/explore
        ///     Returns a list of recommended venues near the current location.
        ///     If authenticated, the method will potentially personalize the ranking based on you and your friends. If you do not
        ///     authenticate, you will not get this personalization.
        ///     This endpoint is part of the venues API (https://developer.foursquare.com/overview/venues.html).
        /// </summary>
        /// <summary>
        ///     https://api.foursquare.com/v2/venues/managed
        ///     Get a list of venues the current user manages.
        /// </summary>
        public async Task<List<Venue>> GetManagedVenues(Dictionary<string, string> parameters)
        {
            List<FourSquareEntityItems<Venue>> venueGroups =
                (await this.GetMultiple<FourSquareEntityItems<Venue>>("/venues/managed", parameters, true)).response["venues"];

            var venueList = new List<Venue>();

            foreach (var venueItems in venueGroups)
            {
                venueList.AddRange(venueItems.items);
            }

            return venueList;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/search
        ///     Returns a list of venues near the current location, optionally matching the search term.
        ///     To ensure the best possible results, pay attention to the intent parameter below. And if you're looking for "top"
        ///     venues or recommended venues, use the explore endpoint instead.
        ///     If lat and long is provided, each venue includes a distance. If authenticated, the method will return venue
        ///     metadata related to you and your friends. If you do not authenticate, you will not get this data.
        ///     Note that most of the fields returned inside venue can be optional. The user may create a venue that has no
        ///     address, city or state (the venue is created instead at the geolat/geolong specified). Your client should handle
        ///     these conditions safely.
        ///     You'll also notice a stats block that reveals some count data about the venue. herenow shows the number of people
        ///     currently there (this value can be 0).
        ///     This endpoint is part of the venues API (https://developer.foursquare.com/overview/venues.html).
        /// </summary>
        public async Task<List<Venue>> SearchVenues(Dictionary<string, string> parameters)
        {
            FourSquareEntityItems<Venue> venues = (await this.GetSingle<FourSquareEntityItems<Venue>>("/venues/search", parameters, true)).response["groups"];
            return venues.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/timeseries
        ///     Get daily venue stats for a list of venues over a time range.
        /// </summary>
        public async Task<List<VenueTimeSerie>> GetVenueTimeSeriesData(Dictionary<string, string> parameters)
        {
            return (await this.GetMultiple<VenueTimeSerie>("/venues/timeseries", parameters, true)).response["timeseries"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/suggestcompletion
        ///     Returns a list of mini-venues partially matching the search term, near the location.
        /// </summary>
        public async Task<List<Venue>> GetSuggestCompletionVenues(Dictionary<string, string> parameters)
        {
            return (await this.GetMultiple<Venue>("/venues/suggestcompletion", parameters, true)).response["minivenues"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/trending
        ///     Returns a list of venues near the current location with the most people currently checked in.
        ///     This endpoint is part of the venues API (https://developer.foursquare.com/overview/venues.html).
        /// </summary>
        public async Task<List<Venue>> GetTrendingVenues(Dictionary<string, string> parameters = null)
        {
            return (await this.GetMultiple<Venue>("/venues/trending", parameters, true)).response["venues"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/herenow
        ///     Provides a count of how many people are at a given venue, plus the first page of the users there, friends-first,
        ///     and if the current user is authenticated.
        ///     This is an experimental API. We're excited about the innovation we think it enables as a much more efficient
        ///     version of fetching all data about a venue, but we're also still learning if this right approach. Please give it a
        ///     shot and provide feedback on the mailing list.
        /// </summary>
        public async Task<List<Checkin>> GetVenueHereNow(string venueId, Dictionary<string, string> parameters = null)
        {
            FourSquareEntityItems<Checkin> checkins =
                (await this.GetSingle<FourSquareEntityItems<Checkin>>("/venues/" + venueId + "/herenow", parameters, true)).response["hereNow"];

            return checkins.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/tips
        ///     Returns tips for a venue.
        /// </summary>
        public async Task<List<Tip>> GetVenueTips(string venueId, Dictionary<string, string> parameters = null)
        {
            FourSquareEntityItems<Tip> tips =
                (await this.GetSingle<FourSquareEntityItems<Tip>>("/venues/" + venueId + "/tips", parameters, true)).response["tips"];
            return tips.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/photos
        ///     Returns photos for a venue.
        /// </summary>
        public async Task<List<Photo>> GetVenuePhotos(string venueId, Dictionary<string, string> parameters)
        {
            FourSquareEntityItems<Photo> photos =
                (await this.GetSingle<FourSquareEntityItems<Photo>>("/venues/" + venueId + "/photos", parameters, true)).response["photos"];
            return photos.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/links
        ///     Returns URLs or identifiers from third parties that have been applied to this venue, such as how the New York Times
        ///     refers to this venue and a URL for additional information from nytimes.com. This is part of the foursquare Venue
        ///     Map.
        ///     This is an experimental endpoint and very much subject to change. Please provide us feedback in the forum.
        /// </summary>
        public async Task<List<Link>> GetVenueLinks(string venueId)
        {
            FourSquareEntityItems<Link> links = (await this.GetSingle<FourSquareEntityItems<Link>>("/venues/" + venueId + "/links", unauthenticated:true)).response["links"];

            return links.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/marktodo
        ///     Allows you to mark a venue to-do, with optional text.
        /// </summary>
        public async Task<Todo> SetVenueToDo(string venueId, string text)
        {
            var parameters = new Dictionary<string, string> {{"text", text}};

            return (await this.Post<Todo>("/venues/" + venueId + "/marktodo", parameters)).response["todo"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/flag
        ///     Allows users to indicate a venue is incorrect in some way.
        ///     Flags are pushed into a moderation queue. If a closed flag is approved, the venue will no longer show up in search
        ///     results. Moderators will attempt to correct cases of mislocated or duplicate venues as appropriate. If the user has
        ///     the correct address for a mislocated venue, use proposeedit instead.
        /// </summary>
        public async Task SetVenueFlag(string venueId, string problem)
        {
            var parameters = new Dictionary<string, string> {{"problem", problem}};
            await this.Post("/venues/" + venueId + "/flag", parameters);
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/venues/VENUE_ID/proposeedit
        ///     Allows you to propose a change to a venue.
        ///     If the user knows a correct address, use this method to save it. Otherwise, use flag to flag the venue instead (you
        ///     need not specify a new address or geolat/geolong in that case).
        /// </summary>
        public async Task SetVenueProposeEdit(string venueId, Dictionary<string, string> parameters = null)
        {
            await this.Post("/venues/" + venueId + "/proposeedit", parameters);
        }

        //Checkin
        /// <summary>
        ///     https://api.foursquare.com/v2/checkins/CHECKIN_ID
        ///     Get details of a checkin.
        /// </summary>
        public async Task<Checkin> GetCheckin(string checkinId)
        {
            return (await this.GetSingle<Checkin>("/checkins/" + checkinId)).response["checkin"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/checkins/add
        ///     Allows you to check in to a place.
        ///     Checkins will always have notifications included.
        /// </summary>
        public async Task<Checkin> AddCheckin(Dictionary<string, string> parameters)
        {
            return (await this.Post<Checkin>("/checkins/add", parameters)).response["checkin"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/checkins/recent
        ///     Returns a list of recent checkins from friends.
        /// </summary>
        public async Task<List<Checkin>> GetRecentCheckin(Dictionary<string, string> parameters = null)
        {
            return (await GetMultiple<Checkin>("/checkins/recent", parameters)).response["recent"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/checkins/CHECKIN_ID/addcomment
        ///     Comment on a checkin-in.
        /// </summary>
        public async Task AddChekinComment(string checkinId, string text)
        {
            var parameters = new Dictionary<string, string> {{"text", text}};
            await this.Post("/checkins/" + checkinId + "/addcomment", parameters);
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/checkins/CHECKIN_ID/deletecomment
        ///     Remove a comment from a checkin, if the acting user is the author or the owner of the checkin.
        /// </summary>
        public async Task DeleteChekinComment(string checkinId, string commentId)
        {
            var parameters = new Dictionary<string, string> {{"commentId", commentId}};
            await this.Post("/checkins/" + checkinId + "/deletecomment", parameters);
        }

        //Tips
        /// <summary>
        ///     https://api.foursquare.com/v2/tips/TIP_ID
        ///     Gives details about a tip, including which users (especially friends) have marked the tip to-do or done.
        /// </summary>
        public async Task<Tip> GetTip(string tipId)
        {
            return (await this.GetSingle<Tip>("/tips/" + tipId, unauthenticated: true)).response["tip"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/tips/add
        ///     Allows you to add a new tip at a venue.
        /// </summary>
        public async Task<Tip> AddTip(Dictionary<string, string> parameters)
        {
            return (await this.Post<Tip>("/tips/add", parameters)).response["tip"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/tips/search
        ///     Returns a list of tips near the area specified.
        /// </summary>
        public async Task<List<Tip>> SearchTips(Dictionary<string, string> parameters)
        {
            return (await this.GetMultiple<Tip>("/tips/search", parameters, true)).response["tips"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/tips/TIP_ID/marktodo
        ///     Allows you to mark a tip to-do.
        /// </summary>
        public async Task<Todo> SetTipToDo(string tipId)
        {
            return (await this.Post<Todo>("/tips/" + tipId + "/marktodo")).response["todo"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/tips/TIP_ID/markdone
        ///     Allows the acting user to mark a tip done.
        /// </summary>
        public async Task SetTipDone(string tipId)
        {
            await this.Post("/tips/" + tipId + "/markdone");
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/tip/TIP_ID/unmark
        ///     Allows you to remove a tip from your to-do list or done list.
        /// </summary>
        public async Task SetTipUnMark(string tipId)
        {
            await this.Post("/tips/" + tipId + "/unmark");
        }

        //Photo
        /// <summary>
        ///     https://api.foursquare.com/v2/photos/PHOTO_ID
        ///     Get details of a photo.
        /// </summary>
        public async Task<Photo> GetPhoto(string photoId)
        {
            return (await this.GetSingle<Photo>("/photos/" + photoId)).response["photo"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/photos/add
        ///     Allows users to add a new photo to a checkin, tip, or a venue in general.
        ///     All fields are optional, but exactly one of the id fields (checkinId, tipId, venueId) must be passed in.
        ///     In addition, the image file data must be posted. The photo should be uploaded as a jpeg and the Content-Type should
        ///     be set to "image/jpeg".
        ///     Attaching a photo to a tip or a venue makes it visible to anybody, while attaching a photo to a checkin makes it
        ///     visible only to the people who can see the checkin (the user's friends, unless the checkin has been sent to Twitter
        ///     or Facebook.).
        ///     Multiple photos can be attached to a checkin or venue, but there can only be one photo per tip.
        ///     To avoid double-tweeting, if you are sending a checkin that will be immediately followed by a photo, do not set
        ///     broadcast=twitter on the checkin, and just set it on the photo.
        /// </summary>
        public async Task<Photo> AddPhoto(Dictionary<string, string> parameters)
        {
            return (await this.Post<Photo>("/photos/add", parameters)).response["photo"];
        }

        //Settings
        /// <summary>
        ///     https://api.foursquare.com/v2/settings/all
        ///     Returns a setting for the acting user.
        /// </summary>
        public async Task<Setting> GetSettings()
        {
            return (await this.GetSingle<Setting>("/settings/all")).response["settings"];
        }

        /*NEXT Version
         public Setting GetAllSetting(Dictionary<string, string> parameters)
        {
            return GetSingle<Setting>("/settings/all", parameters).response["settings"];
        }*/

        /// <summary>
        ///     https://api.foursquare.com/v2/settings/SETTING_ID/set
        ///     Change a setting for the given user.
        /// </summary>
        public async Task SetSetting(string settingId, string value)
        {
            var parameters = new Dictionary<string, string> {{"value", value}};
            await this.Post("/settings/" + settingId + "/set", parameters);
        }

        //Special
        /// <summary>
        ///     https://api.foursquare.com/v2/specials/SPECIAL_ID
        ///     Gives details about a special, including text and unlock rules.
        /// </summary>
        public async Task<Special> GetSpecial(string specialId)
        {
            return (await this.GetSingle<Special>("/specials/" + specialId, unauthenticated: true)).response["special"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/specials/search
        ///     Returns a list of specials near the current location.
        ///     This is an experimental API. We'd love your feedback as we solidify it over the next few weeks.
        /// </summary>
        public async Task<List<Special>> SearchSpecials(Dictionary<string, string> parameters)
        {
            FourSquareEntityItems<Special> specials = (await GetSingle<FourSquareEntityItems<Special>>("/specials/search", parameters)).response["specials"];
            return specials.items;
        }

        #region User

        // User
        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID
        ///     Returns profile information for a given user, including selected badges and mayorships.
        ///     If the user is a friend, contact information, Facebook ID, and Twitter handle and the user's last checkin may also
        ///     be present.
        ///     In addition, the pings field will indicate whether checkins from this user will trigger a ping (notifications to
        ///     mobile devices). This setting can be changed via setpings. Note that this setting is overriden if pings is false in
        ///     settings (no pings will be sent, even if this user is set to true).
        /// </summary>
        public async Task<User> GetUser(string userId)
        {
            FourSquareSingleResponse<User> user = await this.GetSingle<User>("/users/" + userId);
            return user.response["user"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/search
        ///     Returns an array of compact users's profiles.
        /// </summary>
        public async Task<List<User>> SearchUsers(Dictionary<string, string> parameters)
        {
            return (await GetMultiple<User>("/users/search", parameters)).response["results"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/requests
        ///     Shows a user the list of users with whom they have a pending friend request (i.e., someone tried to add the acting
        ///     user as a friend, but the acting user has not accepted).
        /// </summary>
        public async Task<List<User>> GetUserRequests()
        {
            return (await this.GetMultiple<User>("/users/requests")).response["requests"];
        }

        // TODO
        /*
        public List<Badge> GetBadges(string userId)
        {
            return GetMultiple<Badge>("/users/" + userId + "/badges").response["badges"];
        }
        */

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/checkins
        ///     Returns a history of checkins for the authenticated user.
        /// </summary>
        public async Task<List<Checkin>> GetUserCheckins(string userId, Dictionary<string, string> parameters = null)
        {
            FourSquareEntityItems<Checkin> checkins =
                (await GetSingle<FourSquareEntityItems<Checkin>>("/users/" + userId + "/checkins", parameters)).response["checkins"];
            return checkins.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/friends
        ///     Returns an array of a user's friends.
        /// </summary>
        public async Task<List<User>> GetUserFriends(string userId, Dictionary<string, string> parameters)
        {
            FourSquareEntityItems<User> friends =
                (await GetSingle<FourSquareEntityItems<User>>("/users/" + userId + "/friends", parameters)).response["friends"];
            return friends.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/tips
        ///     Returns tips from a user.
        /// </summary>
        public async Task<List<Tip>> GetUserTips(string userId, Dictionary<string, string> parameters = null)
        {
            FourSquareEntityItems<Tip> tips = (await GetSingle<FourSquareEntityItems<Tip>>("/users/" + userId + "/tips", parameters)).response["tips"];
            return tips.items;
        }

        /*/// <summary>
        /// https://api.foursquare.com/v2/users/USER_ID/todos
        /// Returns todos from a user. 
        /// </summary>
        public List<Todo> GetUserTodos(string userId)
        {
            return GetUserTodos(userId, null);
        }

        public List<Todo> GetUserTodos(string userId, Dictionary<string, string> parameters)
        {
            FourSquareEntityItems<Todo> todos = GetSingle<FourSquareEntityItems<Todo>>("/users/" + userId + "/todos", parameters).response["todos"];
           
            return todos.items;
        }*/

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/venuehistory
        ///     Returns a list of all venues visited by the specified user, along with how many visits and when they were last
        ///     there.
        ///     This is an experimental API. We're excited about the innovation we think it enables as a much more efficient
        ///     version of fetching all of a user's checkins, but we're also still learning if this right approach. Please give it
        ///     a shot and provide feedback on the mailing list. Note that although the venuehistory endpoint currently returns all
        ///     of the user's data, we expect to return only the last 6 months, requiring callers to page backwards as needed. We
        ///     may also remove the lastHereAt value. Additionally, for anomalous users, we'll cap out at 500 unique venues.
        /// </summary>
        public async Task<List<VenueHistory>> GetUserVenueHistory(Dictionary<string, string> parameters = null)
        {
            FourSquareEntityItems<VenueHistory> venues =
                (await this.GetSingle<FourSquareEntityItems<VenueHistory>>("/users/self/venuehistory")).response["venues"];
            return venues.items;
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/request
        ///     Sends a friend request to another user.
        /// </summary>
        public async Task<User> SendUserRequest(string userId)
        {
            return (await this.Post<User>("/users/" + userId + "/request")).response["user"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/unfriend
        ///     Cancels any relationship between the acting user and the specified user.
        ///     Removes a friend, unfollows a celebrity, or cancels a pending friend request.
        /// </summary>
        public async Task<User> SendUserUnfriend(string userId)
        {
            return (await this.Post<User>("/users/" + userId + "/unfriend")).response["user"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/approve
        ///     Approves a pending friend request from another user.
        /// </summary>
        public async Task<User> SendUserApprove(string userId)
        {
            return (await this.Post<User>("/users/" + userId + "/approve")).response["user"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/deny
        ///     Denies a pending friend request from another user.
        /// </summary>
        public async Task<User> SendUserDeny(string userId)
        {
            return (await this.Post<User>("/users/" + userId + "/deny")).response["user"];
        }

        /// <summary>
        ///     https://api.foursquare.com/v2/users/USER_ID/setpings
        ///     Changes whether the acting user will receive pings (phone notifications) when the specified user checks in.
        /// </summary>
        public async Task<User> SetUserPings(string userId, string value)
        {
            var parameters = new Dictionary<string, string> {{"value", value}};

            return (await this.Post<User>("/users/" + userId + "/setpings", parameters)).response["user"];
        }

        #endregion
    }
}