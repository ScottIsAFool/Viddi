using GalaSoft.MvvmLight.Messaging;
using Viddy.ViewModel;

namespace Viddy.Messaging
{
    public class VideoMessage : MessageBase
    {
        public VideoItemViewModel Video { get; set; }

        public VideoMessage(VideoItemViewModel video)
        {
            Video = video;
        }
    }
}