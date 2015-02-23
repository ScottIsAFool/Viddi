using Viddy.Core.Model;
using VidMePortable.Model;

namespace Viddy.Core.Extensions
{
    public static class VidMeExtensions
    {
        public static string ToViddyLink(this Video video)
        {
            return TileType.Video.ToViddyLink(video.VideoId);
        }

        public static string ToViddyLink(this Channel channel)
        {
            return TileType.Channel.ToViddyLink(channel.ChannelId);
        }

        public static string ToViddyLink(this User user)
        {
            return TileType.User.ToViddyLink(user.UserId);
        }

        public static string ToViddyLink(this TileType type, string id)
        {
            return ToLink(type.ToString(), id);
        }

        private static string ToLink(string type, string id)
        {
            return string.Format("viddy://{0}?id={1}", type, id);
        }
    }
}
