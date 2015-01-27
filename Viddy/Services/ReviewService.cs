using Cimbalino.Toolkit.Services;

namespace Viddy.Services
{
    public class ReviewService
    {
        private readonly IApplicationSettingsService _settingsService;
        public static ReviewService Current { get; private set; }

        private int _runCount;
        private bool _alreadyResponded;

        public ReviewService(IApplicationSettingsService settingsService)
        {
            _settingsService = settingsService;
            Current = this;

            GetRunCount();
            GetAlreadyResponded();
        }

        public void Responded()
        {
            _alreadyResponded = true;
            _settingsService.Roaming.Set(Constants.StorageSettings.PhoneAlreadyRespondedSetting, _alreadyResponded);
        }

        public void IncreaseCount()
        {
            _runCount++;
            _settingsService.Local.Set(Constants.StorageSettings.LaunchedCountSetting, _runCount);
        }

        public bool CanShowReviews
        {
            get { return _runCount > 5 && !_alreadyResponded; }
        }

        private void GetAlreadyResponded()
        {
            var responded = _settingsService.Roaming.Get<bool>(Constants.StorageSettings.PhoneAlreadyRespondedSetting);
            _alreadyResponded = responded;
        }

        private void GetRunCount()
        {
            var count = _settingsService.Local.Get<int>(Constants.StorageSettings.LaunchedCountSetting);
            _runCount = count;
        }
    }
}
