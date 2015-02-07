using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;

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
        Task<bool> PinVideo(string videoId, string pinName);
        Task<bool> UnpinVideo(string videoId, string pinName);
        Task<bool> PinChannel(string channelId, string pinName);
        Task<bool> UnpinChannel(string channelId);
        Task<bool> PinUser(string userId);
        Task<bool> UnpinUser(string userId);
        Task SaveVisualElementToFile(UIElement element, string filename, int height, int width);
        string GetTileImageUrl(TileService.TileType tileType, string id = "");
        string GetTileFileName(TileService.TileType tileType, string id = "");
    }

    public class TileService : ITileService
    {
        private const string MediumTileLocation = "ms-appdata:///Assets/{0}Tile.png";
        private const string SmallTileLocation = "ms-appdata:///Assets/{0}SmallTile.png";
        private const string WideTileLocation = "ms-appdata:///Assets/{0}WideTile.png";
        private const string SourceTileLocation = "ms-appdata:///Local/" + SourceTileFile;
        private const string SourceTileFile = "{0}{1}.png";
        private const string Arguments = "http://ferretlabs.com/viddy?tileType={0}&id={1}";

        private readonly IStorageService _storageService;
        private readonly IApplicationSettingsService _appSettings;

        public TileService(IStorageService storageService, IApplicationSettingsService appSettings)
        {
            _storageService = storageService;
            _appSettings = appSettings;
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

        public async Task<bool> PinVideoRecord()
        {
            var uri = GetTileImageUrl(TileType.VideoRecord);
            var arguments = string.Format(Arguments, TileType.VideoRecord, string.Empty);
            var tileId = GetTileId(TileType.VideoRecord);
            var tileName = "Record video";
            return await PinTile(tileId, tileName, arguments, uri);
        }

        public Task<bool> UnpinVideoRecord()
        {
            return Unpin(TileType.VideoRecord.ToString());
        }

        public Task<bool> PinVideo(string videoId, string pinName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UnpinVideo(string videoId, string pinName)
        {
            return Unpin(GetTileId(TileType.Video, videoId));
        }

        public Task<bool> PinChannel(string channelId, string pinName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UnpinChannel(string channelId)
        {
            return Unpin(GetTileId(TileType.Channel, channelId));
        }

        public Task<bool> PinUser(string userId)
        {
            throw new System.NotImplementedException();
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

        public string GetTileFileName(TileType tileType, string id = "")
        {
            return string.Format(SourceTileFile, tileType, id);
        }
        #endregion



        private static string GetTileId(TileType tileType, string id = "")
        {
            return string.Format("{0}{1}", tileType, id);
        }

        private static bool IsPinned(string tileId)
        {
            return SecondaryTile.Exists(tileId);
        }

        private static async Task<bool> PinTile(string tileId, string tileName, string arguments, string uri)
        {
            try
            {
                var secondaryTile = new SecondaryTile(tileId, tileName, arguments, new Uri(uri, UriKind.Absolute), TileSize.Square150x150)
                {
                    BackgroundColor = Colors.Transparent
                };
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
                secondaryTile.VisualElements.Square30x30Logo = new Uri(uri);

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

        public enum TileType
        {
            VideoRecord,
            Channel,
            User,
            Video
        }
    }
}
