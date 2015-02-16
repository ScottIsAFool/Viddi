using GalaSoft.MvvmLight.Messaging;

namespace Viddy.Messaging
{
    public class ProtocolMessage : MessageBase
    {
        public enum ProtocolType
        {
            Channel,
            User, 
            Search,
            Video
        }

        public ProtocolMessage(ProtocolType protocolType, string content, bool secondaryContent = false)
        {
            Type = protocolType;
            Content = content;
            SecondaryContent = secondaryContent;
        }

        public ProtocolType Type { get; set; }
        public string Content { get; set; }
        public bool SecondaryContent { get; set; }
    }
}