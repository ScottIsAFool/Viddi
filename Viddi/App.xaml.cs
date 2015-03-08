using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.Windows.Core.Extensions;
using ScottIsAFool.Windows.Core.Logging;
using ScottIsAFool.Windows.Core.Services;
using ScottIsAFool.Windows.Helpers;
using Viddi.Core;
using Viddi.Core.Model;
using Viddi.Messaging;
using Viddi.Services;
using Viddi.ViewModel;
using Viddi.ViewModel.Item;
using Viddi.Views;
using Viddi.Views.Account;
using VidMePortable.Model;

namespace Viddi
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        private TransitionCollection _transitions;
        private readonly ILog _log;

        public static ViewModelLocator Locator
        {
            get { return Current.Resources["Locator"] as ViewModelLocator; }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            _log = new WinLogger(GetType());
            WinLogger.LogConfiguration = new LogConfiguration {LoggingIsEnabled = true, LogType = LogType.WriteToFile};
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            _log.ErrorException("Unhandled", e.Exception);

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as Frame;
            Messenger.Default.Send(new PinMessage());

            AppStarted();

            if (!string.IsNullOrEmpty(e.Arguments) && Uri.IsWellFormedUriString(e.Arguments, UriKind.Absolute) && e.Arguments.StartsWith("viddi://"))
            {
                var uri = new Uri(e.Arguments);
                if (!string.IsNullOrEmpty(uri.Host))
                {
                    HandleUriLaunch(uri);
                    return;
                }
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 10;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Arguments))
                {
                    rootFrame.BackStack.Clear();
                }
            }

            var pageToLoad = PageToLoad(e.Arguments);

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    _transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        _transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrameFirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(pageToLoad, new NavigationParameters {ShowHomeButton = pageToLoad != typeof (MainView)}))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            if (e.TileId != "App")
            {
                rootFrame.Navigate(pageToLoad);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void AppStarted()
        {
            if (_appStarted) return;

            Window.Current.VisibilityChanged += CurrentOnVisibilityChanged;

            Locator.Auth.StartService();
            Locator.SettingsService.StartService();
            Locator.NotificationService.StartService();
            Locator.Review.IncreaseCount();
            Locator.TaskService.CreateService();
            _appStarted = true;
        }

        private bool _appStarted;

        private static void CurrentOnVisibilityChanged(object sender, VisibilityChangedEventArgs visibilityChangedEventArgs)
        {
            Messenger.Default.Send(new PinMessage());
        }

        public Type PageToLoad(string arguments)
        {
            var type = typeof(MainView);
            if (string.IsNullOrEmpty(arguments))
            {
                return type;
            }

            var query = new Uri(arguments).QueryDictionary();
            if (!query.ContainsKey("tileType"))
            {
                return type;
            }

            var source = query["tileType"];
            var tileType = (TileType)Enum.Parse(typeof(TileType), source);
            var id = query["id"];

            var item = Locator.TileService.GetPinnedItemDetails(tileType, id);

            switch (tileType)
            {
                case TileType.VideoRecord:
                    type = typeof(VideoRecordView);
                    break;
                case TileType.Channel:
                    Messenger.Default.Send(new ChannelMessage(new ChannelItemViewModel((Channel)item)));
                    type = typeof(ChannelView);
                    break;
                case TileType.User:
                    Messenger.Default.Send(new UserMessage(new UserViewModel((User)item)));
                    type = typeof(ProfileView);
                    break;
                case TileType.Video:
                    Messenger.Default.Send(new VideoMessage(new VideoItemViewModel((Video)item, null)));
                    type = typeof(VideoPlayerView);
                    break;
                default:
                    type = typeof(MainView);
                    break;
            }

            return type;
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            AppStarted();

            if (args != null)
            {
                if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
                {
                    var brokerArgs = args as IWebAuthenticationBrokerContinuationEventArgs;
                    if (brokerArgs != null)
                    {
                        if (brokerArgs.WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                        {
                            var url = brokerArgs.WebAuthenticationResult.ResponseData;
                            if (url.Contains(Constants.CallBackUrl))
                            {
                                var uri = new Uri(url);
                                var queryString = uri.QueryDictionary();
                                var code = queryString["code"];
                                Messenger.Default.Send(new NotificationMessage(code, Constants.Messages.AuthCodeMsg));
                            }
                        }
                    }
                }
                else if (args.Kind == ActivationKind.PickFileContinuation)
                {
                    var pickerArgs = args as IFileOpenPickerContinuationEventArgs;
                    if (pickerArgs != null)
                    {
                        if (pickerArgs.Files.Any())
                        {
                            var file = pickerArgs.Files[0];
                            if (file.ContentType.ToLower().Contains("image"))
                            {
                                Messenger.Default.Send(new NotificationMessage(file, Constants.Messages.ProfileFileMsg));
                            }
                            else
                            {
                                SendFile(file);
                            }
                        }
                    }
                }
                else if (args.Kind == ActivationKind.Protocol)
                {
                    var eventArgs = args as ProtocolActivatedEventArgs;
                    if (eventArgs != null)
                    {
                        var uri = eventArgs.Uri;
                        HandleUriLaunch(uri);
                    }
                }
            }
        }

        private static void HandleUriLaunch(Uri uri)
        {
            // viddi://search?query={0}&nsfw=true/false
            // viddi://record
            // viddi://
            // viddi://user?id={0}
            // viddi://channel?id={0}
            // viddi://video?id={0}

            Type pageToGoTo;
            var query = uri.QueryDictionary();

            if (query.ContainsKey("notificationId"))
            {
                var notificationId = query["notificationId"];
                ToastNotificationManager.History.Remove(notificationId);
            }

            switch (uri.Host)
            {
                case "search":
                    pageToGoTo = typeof (SearchView);
                    var includeNsfw = false;
                    if (query.ContainsKey("nsfw"))
                    {
                        includeNsfw = bool.Parse(query["nsfw"]);
                    }

                    Messenger.Default.Send(new ProtocolMessage(ProtocolMessage.ProtocolType.Search, query["query"], includeNsfw));
                    break;
                case "videorecord":
                case "record":
                    pageToGoTo = typeof (VideoRecordView);
                    break;
                case "user":
                    pageToGoTo = typeof (ProfileView);
                    Messenger.Default.Send(new ProtocolMessage(ProtocolMessage.ProtocolType.User, query["id"]));
                    break;
                case "channel":
                    pageToGoTo = typeof (ChannelView);
                    Messenger.Default.Send(new ProtocolMessage(ProtocolMessage.ProtocolType.Channel, query["id"]));
                    break;
                case "video":
                    pageToGoTo = typeof (VideoPlayerView);
                    Messenger.Default.Send(new ProtocolMessage(ProtocolMessage.ProtocolType.Video, query["id"]));
                    break;
                default:
                    pageToGoTo = typeof (MainView);
                    break;
            }

            var frame = new Frame();
            frame.Navigate(pageToGoTo, new NavigationParameters {ShowHomeButton = true});

            Window.Current.Content = frame;
            Window.Current.Activate();
        }

        private static void SendFile(IStorageItem file)
        {
            if (Locator.Upload != null)
            {
                Messenger.Default.Send(new NotificationMessage(file, Constants.Messages.VideoFileMsg));
                SimpleIoc.Default.GetInstance<INavigationService>().Navigate<UploadVideoView>();
            }
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            AppStarted();
            base.OnShareTargetActivated(args);
            var shareOperation = args.ShareOperation;
            if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var files = await shareOperation.Data.GetStorageItemsAsync();
                if (files.Count > 1)
                {
                    shareOperation.ReportError(Localisation.Resources.ErrorOneFileAtATime);
                    return;
                }

                var file = files[0];

                var frame = new Frame();
                Window.Current.Content = frame;
                Window.Current.Activate();
                SendFile(file);
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrameFirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = _transitions ?? new TransitionCollection {new NavigationThemeTransition()};
                rootFrame.Navigated -= RootFrameFirstNavigated;
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            Window.Current.VisibilityChanged -= VideoRecordView.CurrentOnVisibilityChanged;
            Window.Current.VisibilityChanged -= CurrentOnVisibilityChanged;

            var display = SimpleIoc.Default.GetInstance<IDisplayRequestService>();
            if (display.IsActive)
            {
                display.Release();
            }

            await SimpleIoc.Default.GetInstance<ICameraInfoService>().DisposeMediaCapture();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}