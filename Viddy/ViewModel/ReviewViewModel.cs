using GalaSoft.MvvmLight.Command;
using Viddy.Model;
using Viddy.Services;

namespace Viddy.ViewModel
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
                return new RelayCommand(() =>
                {
                    
                });
            }
        }
    }
}
