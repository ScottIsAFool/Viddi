using System;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Numerics;

namespace Viddy.Controls
{
    public sealed class BlurredImageControl : Control
    {
        public static readonly DependencyProperty BlurProperty = DependencyProperty.Register("Blur", typeof(float), typeof(BlurredImageControl), new PropertyMetadata(15.0f));
        
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof (object), typeof (BlurredImageControl), new PropertyMetadata(default(object), SourceSet));

        public object Image
        {
            get { return (object) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        private CanvasControl _control;
        private bool _imageLoaded;
        private CanvasBitmap _image;
        private ContentPresenter _contentPresenter;
        private ScaleEffect _scaleEffect;
        private GaussianBlurEffect _blurEffect;

        public BlurredImageControl()
        {
            DefaultStyleKey = typeof(BlurredImageControl);
        }

        public float Blur
        {
            get { return (float)GetValue(BlurProperty); }
            set { SetValue(BlurProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _contentPresenter = GetTemplateChild("imagePresenter") as ContentPresenter;

            RenderImage();
        }

        private static void SourceSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BlurredImageControl;
            if (control != null)
            {
                control.RenderImage();
            }
        }

        private void RenderImage()
        {
            if (_contentPresenter != null)
            {
                _control = new CanvasControl();
                _control.Draw += OnDraw;
                _control.CreateResources += OnCreateResources;

                _contentPresenter.Content = _control;
            }
        }

        private async void OnCreateResources(CanvasControl sender, object args)
        {
            if (Image == null)
            {
                return;
            }

            _imageLoaded = false;
            _scaleEffect = new ScaleEffect();
            _blurEffect = new GaussianBlurEffect();

            var file = Image as StorageFile;
            if(file != null)
            {
                using (var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView))
                {
                    _image = await CanvasBitmap.LoadAsync(sender.Device, thumbnail);
                }

                _imageLoaded = true;
                sender.Invalidate();
            }

            var url = Image as string;
            if (!string.IsNullOrEmpty(url))
            {
                _image = await CanvasBitmap.LoadAsync(sender.Device, new Uri(url));

                _imageLoaded = true;
                sender.Invalidate();
            }
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (_imageLoaded)
            {
                using (var session = args.DrawingSession)
                {
                    session.Units = CanvasUnits.Pixels;

                    double displayScaling = DisplayInformation.GetForCurrentView().LogicalDpi / 96.0;

                    double pixelWidth = sender.ActualWidth * displayScaling;

                    var scalefactor = pixelWidth / _image.Size.Width;

                    _scaleEffect.Source = _image;
                    _scaleEffect.Scale = new Vector2()
                    {
                        X = (float)scalefactor,
                        Y = (float)scalefactor
                    };

                    _blurEffect.Source = _scaleEffect;
                    _blurEffect.BlurAmount = Blur;

                    session.DrawImage(_blurEffect, 0.0f, 0.0f);
                }
            }
        }
    }
}
