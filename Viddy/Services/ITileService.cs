using System;
using System.Linq;
using System.Threading.Tasks;
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
        Task<bool> PinVideo(string videoId);
        Task<bool> UnpinVideo(string videoId);
        Task<bool> PinChannel(string channelId);
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
            return IsPinned(GetTileId(userId, TileType.User));
        }

        public bool IsVideoPinned(string videoId)
        {
            return IsPinned(GetTileId(videoId, TileType.Video));
        }

        public bool IsChannelPinned(string channelId)
        {
            return IsPinned(GetTileId(channelId, TileType.Channel));
        }

        public Task<bool> PinVideoRecord()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UnpinVideoRecord()
        {
            return Unpin(TileType.VideoRecord.ToString());
        }

        public Task<bool> PinVideo(string videoId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UnpinVideo(string videoId)
        {
            return Unpin(GetTileId(videoId, TileType.Video));
        }

        public Task<bool> PinChannel(string channelId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UnpinChannel(string channelId)
        {
            return Unpin(GetTileId(channelId, TileType.Channel));
        }

        public Task<bool> PinUser(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UnpinUser(string userId)
        {
            return Unpin(GetTileId(userId, TileType.User));
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

        

        private static string GetTileId(string id, TileType tileType)
        {
            return string.Format("{0}{1}", tileType, id);
        }

        private static bool IsPinned(string tileId)
        {
            return SecondaryTile.Exists(tileId);
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
