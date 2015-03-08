using System;

namespace Viddi.Services
{
    public interface IToastService
    {
        void Show(string message, string title = "", Action tapAction = null);
    }
}
