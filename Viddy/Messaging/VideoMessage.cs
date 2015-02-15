using GalaSoft.MvvmLight.Messaging;
using Viddy.ViewModel.Item;

namespace Viddy.Messaging
{
    public class VideoMessage : NotificationMessage
    {
        public VideoItemViewModel Video { get; set; }

        public VideoMessage(VideoItemViewModel video) : base(string.Empty)
        {
            Video = video;
        }

        public VideoMessage(VideoItemViewModel video, string notification) : base(notification)
        {
            Video = video;
        }
    }
}