using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Cimbalino.Toolkit.Behaviors;

namespace Viddy.Behaviours
{
    public class UpdateTextBindingOnPropertyChanged : Behavior<TextBox>
    {
        public static readonly DependencyProperty EnterHitCommandProperty =
            DependencyProperty.Register("EnterHitCommand", typeof (ICommand), typeof (UpdateTextBindingOnPropertyChanged), new PropertyMetadata(default(ICommand)));

        public ICommand EnterHitCommand
        {
            get { return (ICommand) GetValue(EnterHitCommandProperty); }
            set { SetValue(EnterHitCommandProperty, value); }
        }

        // Fields
        private BindingExpression expression;

        // Methods
        protected override void OnAttached()
        {
            base.OnAttached();
            this.expression = base.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            base.AssociatedObject.TextChanged += OnTextChanged;
            base.AssociatedObject.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyRoutedEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != VirtualKey.Enter) return;
            if (EnterHitCommand != null && EnterHitCommand.CanExecute(null))
            {
                EnterHitCommand.Execute(null);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.TextChanged -= OnTextChanged;
            this.expression = null;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            this.expression.UpdateSource();
        }
    }
}
