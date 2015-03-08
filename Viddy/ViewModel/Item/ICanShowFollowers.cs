using Viddi.ViewModel.Account;

namespace Viddi.ViewModel.Item
{
    public interface ICanShowFollowers
    {
        string Id { get; }
        FollowersViewModel Followers { get; }
    }
}