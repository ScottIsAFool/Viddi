using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    public class CommentViewModel : ViewModelBase, IListType
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly VideoPlayerViewModel _videoPlayerViewModel;

        public CommentViewModel(Comment comment, VideoPlayerViewModel videoPlayerViewModel)
        {
            Comment = comment;
            _vidMeClient = SimpleIoc.Default.GetInstance<IVidMeClient>();
            _videoPlayerViewModel = videoPlayerViewModel;
        }

        public ListType ListType { get { return ListType.Normal; } }

        public Comment Comment { get; set; }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(() =>
                {

                });
            }
        }
    }
}
