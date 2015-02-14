using System;

namespace Viddy.ViewModel
{
    public interface IBackSupportedViewModel
    {
        void ChangeContext(Type callingType);
        void SaveContext();
    }
}