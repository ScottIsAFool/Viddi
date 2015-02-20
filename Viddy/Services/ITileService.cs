using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using Viddy.Core.Extensions;
using Viddy.Extensions;
using VidMePortable.Model;

namespace Viddy.Services
{
    public interface ITileService
    {
        bool IsVideoRecordPinned { get; }
        bool IsUserPinned(string userId);
        bool IsVideoPinned(string videoId);
        bool IsChannelPinned(string channelId);
        Task<bool> PinVideoRecord();
        Task<bool> UnpinVideoRecord();
        Task<bool> PinVideo(Video video);
        Task<bool> UnpinVideo(string videoId);
        Task<bool> PinChannel(Channel channel);
        Task<bool> UnpinChannel(string channelId);
        Task<bool> PinUser(User user);
        Task<bool> UnpinUser(string userId);
        Task SaveVisualElementToFile(UIElement element, string filename, int height, int width);
        string GetTileImageUrl(TileService.TileType tileType, string id = "");
        string GetTileFileName(TileService.TileType tileType, string id = "", bool isWideTile = false);
        string GetTileId(TileService.TileType tileType, string id = "");
        object GetPinnedItemDetails(TileService.TileType tileType, string id);
        void UpdateTileCount(int count);
    }

    public class TileService : ITileService
    {
        private const string SourceTileLocation = "ms-appdata:///Local/" + SourceTileFile;
        private const string WideTileLocation = "ms-appdata:///Local/" + WideTileFile;
        private const string WideTileFile = "{0}{1}Wide.png";
        private const string SourceTileFile = "{0}{1}.png";
        private const string Arguments = "viddy://{0}?id={1}";

        private readonly IStorageService _storageService;
        private readonly IApplicationSettingsService _appSettings;

        public TileService(IStorageService storageService, IApplicationSettingsService appSettings)
        {
            _storageService = storageService;
            _appSettings = appSettings;
        }

        public static bool IsFromSecondaryTile(string args)
        {
            return !string.IsNullOrEmpty(args) && args.Contains("tileType");
        }

        #region ITileService Implementations

        public bool IsVideoRecordPinned
        {
            get { return IsPinned(TileType.VideoRecord.ToString()); }
        }

        public bool IsUserPinned(string userId)
        {
            return IsPinned(GetTileId(TileType.User, userId));
        }

        public bool IsVideoPinned(string videoId)
        {
            return IsPinned(GetTileId(TileType.Video, videoId));
        }

        public bool IsChannelPinned(string channelId)
        {
            return IsPinned(GetTileId(TileType.Channel, channelId));
        }

        public Task<bool> PinVideoRecord()
        {
            return PinTile(string.Empty, TileType.VideoRecord, "Record video", null, false);
        }

        public Task<bool> UnpinVideoRecord()
        {
            return Unpin(TileType.VideoRecord.ToString());
        }

        public Task<bool> PinVideo(Video video)
        {
            return PinTile(video.VideoId, TileType.Video, video.Title, video, false);
        }

        public Task<bool> UnpinVideo(string videoId)
        {
            return Unpin(GetTileId(TileType.Video, videoId));
        }

        public Task<bool> PinChannel(Channel channel)
        {
            return PinTile(channel.ChannelId, TileType.Channel, channel.Title, channel);
        }

        public Task<bool> UnpinChannel(string channelId)
        {
            return Unpin(GetTileId(TileType.Channel, channelId));
        }

        public Task<bool> PinUser(User user)
        {
            return PinTile(user.UserId, TileType.User, user.Username, user);
        }

        public Task<bool> UnpinUser(string userId)
        {
            return Unpin(GetTileId(TileType.User, userId));
        }

        public async Task SaveVisualElementToFile(UIElement element, string filename, int height, int width)
        {
            //element.Measure(new Size(width, height));
            //element.Arrange(new Rect { Height = height, Width = width });
            //element.UpdateLayout();
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(element, width, height);

            using (var file = await _storageService.Local.CreateFileAsync(filename))
            {
                await renderTargetBitmap.SavePngAsync(file);
            }
        }

        public string GetTileImageUrl(TileType tileType, string id = "")
        {
            return string.Format(SourceTileLocation, tileType, id);
        }

        public string GetTileFileName(TileType tileType, string id = "", bool isWideTile = false)
        {
            return isWideTile ? string.Format(WideTileFile, tileType, id) : string.Format(SourceTileFile, tileType, id);
        }

        public string GetTileId(TileType tileType, string id = "")
        {
            return string.Format("{0}{1}", tileType, id);
        }

        public object GetPinnedItemDetails(TileType tileType, string id)
        {
            var tileId = GetTileId(tileType, id);
            switch (tileType)
            {
                case TileType.VideoRecord:
                    return null;
                case TileType.Channel:
                    return GetItem<Channel>(tileId);
                case TileType.User:
                    return GetItem<User>(tileId);
                case TileType.Video:
                    return GetItem<Video>(tileId);
                default:
                    return null;
            }
        }

        public void UpdateTileCount(int count)
        {
            var badgeContent = new BadgeNumericNotificationContent((uint) count);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());
        }

        #endregion

        private static bool IsPinned(string tileId)
        {
            return SecondaryTile.Exists(tileId);
        }

        private string GetWideTileImageUrl(TileType tileType, string id = "")
        {
            return string.Format(WideTileLocation, tileType, id);
        }

        private async Task<bool> PinTile(string itemId, TileType tileType, string tileName, object item, bool includeWideTile = true)
        {
            var uri = GetTileImageUrl(tileType, itemId);
            var wideUri = GetWideTileImageUrl(tileType, itemId);
            var arguments = string.Format(Arguments, tileType, itemId);
            var tileId = GetTileId(tileType, itemId);

            if (tileType != TileType.VideoRecord)
            {
                SaveItem(tileId, item);
            }

            try
            {
                var secondaryTile = new SecondaryTile(tileId, tileName, arguments, new Uri(uri, UriKind.Absolute), TileSize.Square150x150)
                {
                    BackgroundColor = Colors.Transparent
                };
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
                secondaryTile.VisualElements.Square30x30Logo = new Uri(uri);
                if (includeWideTile)
                {
                    secondaryTile.VisualElements.Wide310x150Logo = new Uri(wideUri);
                    secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
                }

                return await secondaryTile.RequestCreateAsync();
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        private static async Task<bool> Unpin(string tileId)
        {
            if (string.IsNullOrEmpty(tileId)) return false;

            var tiles = await SecondaryTile.FindAllAsync();
            var tile = tiles.FirstOrDefault(x => x.TileId == tileId);
            if (tile == null) return false;

            return await tile.RequestDeleteAsync();
        }

        private void SaveItem(string key, object item)
        {
            if (_appSettings.Local.Contains(key))
            {
                _appSettings.Local.Remove(key);
            }

            _appSettings.Local.SetS(key, item);
        }

        private T GetItem<T>(string key)
        {
            return !_appSettings.Local.Contains(key) ? default(T) : _appSettings.Local.GetS<T>(key);
        }

        public enum TileType
        {
            VideoRecord,
            Channel,
            User,
            Video
        }
    }
}
