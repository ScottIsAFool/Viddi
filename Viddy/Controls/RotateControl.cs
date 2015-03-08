using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Viddi.Controls
{
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "Content")]
    public sealed class RotateContentControl : Control
    {
        private ContentControl _mContent;

        public RotateContentControl()
        {
            DefaultStyleKey = typeof(RotateContentControl);
        }

        protected override void OnApplyTemplate()
        {
            _mContent = GetTemplateChild("Content") as ContentControl;
            base.OnApplyTemplate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_mContent != null)
            {
                if (((int)Direction) % 180 == 90)
                {
                    _mContent.Measure(new Size(availableSize.Height, availableSize.Width));
                    return new Size(_mContent.DesiredSize.Height, _mContent.DesiredSize.Width);
                }
                
                _mContent.Measure(availableSize);
                return _mContent.DesiredSize;
            }
            
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_mContent != null)
            {
                _mContent.RenderTransform = new RotateTransform() { Angle = (int)this.Direction };
                if (Direction == RotateDirection.Up)
                    _mContent.Arrange(new Rect(new Point(0, finalSize.Height),
                                      new Size(finalSize.Height, finalSize.Width)));
                else if (Direction == RotateDirection.Down)
                    _mContent.Arrange(new Rect(new Point(finalSize.Width, 0),
                                      new Size(finalSize.Height, finalSize.Width)));
                else if (Direction == RotateDirection.UpsideDown)
                    _mContent.Arrange(new Rect(new Point(finalSize.Width, finalSize.Height), finalSize));
                else
                    _mContent.Arrange(new Rect(new Point(), finalSize));
                return finalSize;
            }
            
            return base.ArrangeOverride(finalSize);
        }


        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(RotateContentControl), null);

        public enum RotateDirection : int
        {
            Normal = 0,
            Down = 90,
            UpsideDown = 180,
            Up = 270
        }

        public RotateDirection Direction
        {
            get { return (RotateDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(RotateDirection),
            typeof(RotateContentControl), new PropertyMetadata(RotateDirection.Down, OnDirectionPropertyChanged));

        public static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((int)e.OldValue) % 180 == ((int)e.NewValue) % 180)
                (d as RotateContentControl).InvalidateArrange(); //flipping 180 degrees only changes flow not size
            else
                (d as RotateContentControl).InvalidateMeasure(); //flipping 90 or 270 degrees changes size too, so remeasure
        }
    }
}