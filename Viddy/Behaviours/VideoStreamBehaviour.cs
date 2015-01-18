using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Cimbalino.Toolkit.Behaviors;
using Microsoft.PlayerFramework;

namespace Viddy.Behaviours
{
    public class VideoStreamBehaviour : Behavior<MediaPlayer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MediaStarted += AssociatedObjectOnMediaStarted;
            AssociatedObject.MediaClosed += AssociatedObjectOnMediaClosed;
            AssociatedObject.MediaFailed += AssociatedObjectOnMediaFailed;
            AssociatedObject.CurrentStateChanged += AssociatedObjectOnCurrentStateChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MediaStarted -= AssociatedObjectOnMediaStarted;
            AssociatedObject.MediaClosed -= AssociatedObjectOnMediaClosed;
            AssociatedObject.MediaFailed -= AssociatedObjectOnMediaFailed;
            AssociatedObject.CurrentStateChanged -= AssociatedObjectOnCurrentStateChanged;
        }

        private void AssociatedObjectOnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            IsPlaying = false;
        }

        private void AssociatedObjectOnMediaClosed(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
        }

        private void AssociatedObjectOnMediaStarted(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
        }
        
        private void AssociatedObjectOnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            IsPlaying = AssociatedObject.CurrentState == MediaElementState.Playing;
        }

        public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
            "File", typeof (StorageFile), typeof (VideoStreamBehaviour), new PropertyMetadata(default(StorageFile), OnFileChanged));

        public StorageFile File
        {
            get { return (StorageFile) GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty IsAutoPlayProperty = DependencyProperty.Register(
            "IsAutoPlay", typeof (bool), typeof (VideoStreamBehaviour), new PropertyMetadata(default(bool)));

        public bool IsAutoPlay
        {
            get { return (bool) GetValue(IsAutoPlayProperty); }
            set { SetValue(IsAutoPlayProperty, value); }
        }

        public static readonly DependencyProperty IsPauseProperty = DependencyProperty.Register(
            "IsPause", typeof (bool), typeof (VideoStreamBehaviour), new PropertyMetadata(default(bool), OnIsPauseChanged));

        public bool IsPause
        {
            get { return (bool) GetValue(IsPauseProperty); }
            set { SetValue(IsPauseProperty, value); }
        }

        public static readonly DependencyProperty IsStartProperty = DependencyProperty.Register(
            "IsStart", typeof (bool), typeof (VideoStreamBehaviour), new PropertyMetadata(default(bool), OnIsStartChanged));

        public bool IsStart
        {
            get { return (bool) GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(
            "IsPlaying", typeof (bool), typeof (VideoStreamBehaviour), new PropertyMetadata(default(bool)));

        public bool IsPlaying
        {
            get { return (bool) GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        private static void OnIsStartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var b = sender as VideoStreamBehaviour;
            if (b == null)
            {
                return;
            }

            b.StartStop(false);
        }

        private static void OnIsPauseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var b = sender as VideoStreamBehaviour;
            if (b == null)
            {
                return;
            }

            b.StartStop(true);
        }

        private static void OnFileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var b = sender as VideoStreamBehaviour;
            if (b == null)
            {
                return;
            }

            b.SetStream();
        }

        private async void SetStream()
        {
            if (File == null)
            {
                return;
            }

            var stream = await File.OpenAsync(FileAccessMode.Read);
            AssociatedObject.SetSource(stream, File.ContentType);

            if (IsAutoPlay)
            {
                AssociatedObject.Play();
            }
        }

        private void StartStop(bool isPause)
        {
            if (isPause)
            {
                if (AssociatedObject.CanPause)
                {
                    AssociatedObject.Pause();
                }
            }
            else
            {
                AssociatedObject.Play();
            }
        }
    }
}
