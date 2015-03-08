using ScottIsAFool.Windows.Core.ViewModel;

namespace Viddi.ViewModel
{
    public class AdViewModel : ViewModelBase, IListType
    {
        public ListType ListType { get { return ListType.Ad; } }
    }
}