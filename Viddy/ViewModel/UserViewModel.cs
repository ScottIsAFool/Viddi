using System.Threading.Tasks;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace Viddy.ViewModel
{
    public class UserViewModel : ViewModelBase
    {
        public User User { get; set; }

        public UserViewModel(User user)
        {
            User = user;
        }

        public string UserFollowers
        {
            get { return User != null && User.FollowerCount > 0 ? string.Format("{0:N0} followers", User.FollowerCount) : null; }
        }

        public string UserPlays
        {
            get
            {
                double views;
                return User != null
                       && !string.IsNullOrEmpty(User.VideoViews)
                       && double.TryParse(User.VideoViews, out views)
                       && views > 0
                    ? string.Format("{0:N0} plays", views)
                    : null;
            }
        }

        public string UserVideoCount
        {
            get { return User != null && User.VideoCount > 0 ? string.Format("{0:N0} videos", User.VideoCount) : null; }
        }

        public bool DisplayBio
        {
            get { return User != null && !string.IsNullOrEmpty(User.Bio); }
        }

        public bool DisplayByLine
        {
            get { return User != null && !string.IsNullOrEmpty(UserFollowers) && !string.IsNullOrEmpty(UserPlays) && !string.IsNullOrEmpty(UserVideoCount); }
        }
    }
}
