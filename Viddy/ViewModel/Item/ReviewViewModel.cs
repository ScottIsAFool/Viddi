using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Model;
using Viddy.Services;

namespace Viddy.ViewModel.Item
{
    public class ReviewViewModel : ViewModelBase, IListType
    {
        public ListType ListType { get { return ListType.Review; } }

        public bool ShowFeedback { get; set; }

        public RelayCommand YesReviewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ReviewService.Current.Responded();
                    CloseControl();
                });
            }
        }

        public RelayCommand NoReviewCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ReviewService.Current.Responded();
                    ShowFeedback = true;
                });
            }
        }

        public RelayCommand YesFeedbackCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                });
            }
        }

        public RelayCommand NoFeedbackCommand
        {
            get
            {
                return new RelayCommand(CloseControl);
            }
        }

        private static void CloseControl()
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.HideReviewsMsg));
        }
    }
}
