using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Behaviors;

namespace Viddy.Behaviours
{
    public class VideoStreamBehaviour : Behavior<MediaElement>
    {
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
    }
}
