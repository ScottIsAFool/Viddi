using Viddy.ViewModel.Account;

namespace Viddy.ViewModel.Item
{
    public interface ICanShowFollowers
    {
        string Id { get; }
        FollowersViewModel Followers { get; }
    }
}