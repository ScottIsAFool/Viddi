namespace Viddy.ViewModel
{
    public class AdViewModel : ViewModelBase, IListType
    {
        public ListType ListType { get { return ListType.Ad; } }
    }
}