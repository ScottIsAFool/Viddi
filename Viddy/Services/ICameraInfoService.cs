using System.Threading.Tasks;

namespace Cimbalino.Toolkit.Services
{
    public interface ICameraInfoService
    {
        Task StartService();
        Task<bool> HasPrimaryCamera();
        Task<bool> HasFrontFacingCamera();
        Task<bool> HasFlash();
        Task<bool> SupportsFocus();
    }
}