using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ThemeManagerRt;
using Viddy.Extensions;
using Viddy.ViewModel;
using Viddy.Views;
using Viddy.Views.Account;

namespace Viddy
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

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
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                //Debugger.Break();
            }

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
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            ThemeManager.DefaultTheme = ElementTheme.Light;
            Frame rootFrame = Window.Current.Content as Frame;

            Locator.Auth.StartService();

            if (e.PreviousExecutionState == ApplicationExecutionState.Running
                && rootFrame != null && rootFrame.Content != null)
            {
                if (rootFrame.Content is AccountView)
                {
                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.AppLaunchedMsg));
                }
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(AccountView), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            ThemeManager.DefaultTheme = ElementTheme.Light;
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
                                var queryString = uri.QueryString();
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
                                if (Locator.Upload != null)
                                {
                                    Messenger.Default.Send(new NotificationMessage(file, Constants.Messages.VideoFileMsg));
                                    SimpleIoc.Default.GetInstance<INavigationService>().Navigate<UploadVideoView>();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}