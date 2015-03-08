using Viddi.Core.Model;
using VidMePortable.Model;

namespace Viddi.Core.Extensions
{
    public static class VidMeExtensions
    {
        public static string ToViddiLink(this Video video)
        {
            return TileType.Video.ToViddiLink(video.VideoId);
        }

        public static string ToViddiLink(this Channel channel)
        {
            return TileType.Channel.ToViddiLink(channel.ChannelId);
        }

        public static string ToViddiLink(this User user)
        {
            return TileType.User.ToViddiLink(user.UserId);
        }

        public static string ToViddiLink(this TileType type, string id)
        {
            return ToLink(type.ToString(), id);
        }

        private static string ToLink(string type, string id)
        {
            return string.Format("viddi://{0}?id={1}", type.ToLower(), id);
        }
    }
}
