using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VOP;
namespace VOP.Controls
{

    class DeviceButton : Button
    {
        public static readonly DependencyProperty ConnectedProperty =
                            DependencyProperty.Register("Connected",
                            typeof(bool),
                            typeof(DeviceButton),
                            new PropertyMetadata(false, new PropertyChangedCallback(OnConnectedPropertyChanged)));

        public static readonly RoutedEvent ConnectedPropertyEvent =
                                           EventManager.RegisterRoutedEvent("ConnectedPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(DeviceButton));

        public bool Connected
        {
            get { return (bool)GetValue(ConnectedProperty); }
            set { SetValue(ConnectedProperty, value); }
        }

        public event RoutedEventHandler ConnectedPropertyChanged
        {
            add { AddHandler(ConnectedPropertyEvent, value); }
            remove { RemoveHandler(ConnectedPropertyEvent, value); }
        }

        private static void OnConnectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DeviceButton control = sender as DeviceButton;
            if (control != null)
            {
                var newValue = (bool)args.NewValue;
                var oldValue = (bool)args.OldValue;

                RoutedPropertyChangedEventArgs<bool> e =
                    new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue, ConnectedPropertyEvent);

                control.OnConnectedPropertyChanged(e);
            }
        }

        virtual protected void OnConnectedPropertyChanged(RoutedPropertyChangedEventArgs<bool> e)
        {
            RaiseEvent(e);
        }


        static DeviceButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DeviceButton), new FrameworkPropertyMetadata(typeof(DeviceButton)));
        }
    }
}
