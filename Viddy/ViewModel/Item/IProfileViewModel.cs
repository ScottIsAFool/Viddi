namespace Viddy.ViewModel.Item
{
    public interface IProfileViewModel
    {
        string UserFollowers { get; }
        string UserPlays { get; }
        string Description { get; }
        string CoverUrl { get; }
        string AvatarUrl { get; }
        string Name { get; }
        string UserVideoCount { get; }
    }
}