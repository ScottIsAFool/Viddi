using System;

namespace Viddi.ViewModel
{
    public interface IBackSupportedViewModel
    {
        void ChangeContext(Type callingType);
        void SaveContext();
    }
}