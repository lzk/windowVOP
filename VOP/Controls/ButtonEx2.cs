using System.Windows;
using System.Windows.Controls;

namespace VOP.Controls
{
    class ButtonEx2 : Button
    {
        public static readonly DependencyProperty IsActiveExProperty =
            DependencyProperty.Register("IsActiveEx",
            typeof(bool),
            typeof(ButtonEx2),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsActiveExChanged)));

        public static readonly RoutedEvent IsActivePropertyEvent =
            EventManager.RegisterRoutedEvent("IsActiveExPropertyChanged", RoutingStrategy.Bubble,
                    typeof(RoutedEventHandler), typeof(ButtonEx2));

        static ButtonEx2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx2), new FrameworkPropertyMetadata(typeof(ButtonEx2)));
        }

        public bool IsActiveEx
        {
            get { return (bool)GetValue(IsActiveExProperty); }
            set { SetValue(IsActiveExProperty, value); }
        }

        public event RoutedEventHandler IsActiveExPropertyChanged
        {
            add { AddHandler(IsActivePropertyEvent, value); }
            remove { RemoveHandler(IsActivePropertyEvent, value); }
        }

        private static void OnIsActiveExChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.ButtonEx2 btn = sender as VOP.Controls.ButtonEx2;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ButtonEx2.IsActivePropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
    }
}
