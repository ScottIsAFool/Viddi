using System.Threading.Tasks;

namespace Viddy.ViewModel.Item
{
    public interface IFollowViewModel
    {
        bool IsFollowedByMe { get; set; }
        bool ChangingFollowState { get; set; }
        string FollowingText { get; }
        bool CanFollow { get; }
        Task RefreshFollowerDetails();
        Task ChangeFollowingState(bool isFollowing);
    }
}