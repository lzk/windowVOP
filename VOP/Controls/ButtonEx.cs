using System.Windows;
using System.Windows.Controls;

namespace VOP.Controls
{
    class ButtonEx : Button
    {
        public static readonly DependencyProperty IsActiveExProperty =
            DependencyProperty.Register("IsActiveEx",
            typeof(bool),
            typeof(ButtonEx),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsActiveExChanged)));

        public static readonly RoutedEvent IsActivePropertyEvent =
            EventManager.RegisterRoutedEvent("IsActiveExPropertyChanged", RoutingStrategy.Bubble,
                    typeof(RoutedEventHandler), typeof(ButtonEx));

        static ButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
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
            VOP.Controls.ButtonEx btn = sender as VOP.Controls.ButtonEx;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ButtonEx.IsActivePropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
    }
}
