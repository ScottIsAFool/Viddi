using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace ViddiLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void LaunchViddiButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            LaunchViddi(string.Empty);
        }

        private void SearchViddiButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            LaunchViddi("search?query=kendrick");
        }

        private void ShowUserButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            LaunchViddi("user?id=441167");
        }

        private void ShowChannelButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            LaunchViddi("channel?id=5");
        }

        private void VideoRecordButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            LaunchViddi("record");
        }

        private void LaunchViddi(string uri)
        {
            var fullUri = "Viddi://" + uri;
            Windows.System.Launcher.LaunchUriAsync(new Uri(fullUri, UriKind.Absolute));
        }

        private void SearchViddiNsfwButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            LaunchViddi("search?query=kendrick&nsfw=true");
        }
    }
}
