namespace Viddy.ViewModel
{
    public class ReviewViewModel : ViewModelBase, IListType
    {
        public ListType ListType { get { return ListType.Review; } }
    }
}
