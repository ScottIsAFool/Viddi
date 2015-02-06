namespace Viddy.Services
{
    public interface ITileService
    {
        bool IsVideoRecordPinned { get; }
        bool IsUserPinned(string userId);
        bool IsVideoPinned(string videoId);
        bool IsChannelPinned(string channelId);
    }

    public class TileService : ITileService
    {
        public bool IsVideoRecordPinned { get; private set; }
        public bool IsUserPinned(string userId)
        {
            return false;
        }

        public bool IsVideoPinned(string videoId)
        {
            return false;
        }

        public bool IsChannelPinned(string channelId)
        {
            return false;
        }
    }
}
