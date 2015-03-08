namespace Viddi.ViewModel.Item
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
        bool DisplayBio { get; }
        bool DisplayByLine { get; }
        bool IsNsfw { get; }
    }
}