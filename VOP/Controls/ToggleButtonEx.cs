using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VOP;
namespace VOP.Controls
{
    public enum EnumWifiSignalLevel
    {
        Weakness =0,
        Weak = 1,
        Normal = 2,
        Strong = 3,
        stronger = 4
    };

    class ToggleButtonEx : ToggleButton
    {
        #region SSIDText property
        public static readonly DependencyProperty SSIDTextProperty =
                                            DependencyProperty.Register("SSIDText",
                                            typeof(string),
                                            typeof(ToggleButtonEx),
                                            new PropertyMetadata("", new PropertyChangedCallback(OnSSIDTextPropertyChanged)));

        public static readonly RoutedEvent SSIDTextPropertyEvent =
                                            EventManager.RegisterRoutedEvent("SSIDTextPropertyChanged", RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(ToggleButtonEx));

        public string SSIDText
        {
            get { return (string)GetValue(SSIDTextProperty); }
            set { SetValue(SSIDTextProperty, value); }
        }

        public event RoutedEventHandler SSIDTextPropertyChanged
        {
            add { AddHandler(SSIDTextPropertyEvent, value); }
            remove { RemoveHandler(SSIDTextPropertyEvent, value); }
        }

        private static void OnSSIDTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleButtonEx control = sender as ToggleButtonEx;
            if (control != null)
            {
                var newValue = (string)args.NewValue;
                var oldValue = (string)args.OldValue;

                RoutedPropertyChangedEventArgs<string> e =
                    new RoutedPropertyChangedEventArgs<string>(oldValue, newValue, SSIDTextPropertyEvent);

                control.OnSSIDTextPropertyChanged(e);
            }
        }

        virtual protected void OnSSIDTextPropertyChanged(RoutedPropertyChangedEventArgs<string> e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region EncryptionText property
        public static readonly DependencyProperty EncryptionTextProperty =
                                    DependencyProperty.Register("EncryptionText",
                                    typeof(string),
                                    typeof(ToggleButtonEx),
                                    new PropertyMetadata("", new PropertyChangedCallback(OnEncryptionTextPropertyChanged)));

        public static readonly RoutedEvent EncryptionTextPropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptionTextPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ToggleButtonEx));

        public string EncryptionText
        {
            get { return (string)GetValue(EncryptionTextProperty); }
            set { SetValue(EncryptionTextProperty, value); }
        }

        public event RoutedEventHandler EncryptionTextPropertyChanged
        {
            add { AddHandler(EncryptionTextPropertyEvent, value); }
            remove { RemoveHandler(EncryptionTextPropertyEvent, value); }
        }

        private static void OnEncryptionTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleButtonEx control = sender as ToggleButtonEx;
            if (control != null)
            {
                var newValue = (string)args.NewValue;
                var oldValue = (string)args.OldValue;

                RoutedPropertyChangedEventArgs<string> e =
                    new RoutedPropertyChangedEventArgs<string>(oldValue, newValue, EncryptionTextPropertyEvent);

                control.OnEncryptionTextPropertyChanged(e);
            }
        }

        virtual protected void OnEncryptionTextPropertyChanged(RoutedPropertyChangedEventArgs<string> e)
        {
            RaiseEvent(e);
        }
        #endregion

        #region EncryptType property
        public static readonly DependencyProperty EncryptTypeProperty =
                                    DependencyProperty.Register("EncryptType",
                                    typeof(EnumEncryptType),
                                    typeof(ToggleButtonEx),
                                    new PropertyMetadata(EnumEncryptType.WPA2_PSK_AES, new PropertyChangedCallback(OnEncryptTypePropertyChanged)));

        public static readonly RoutedEvent EncryptTypePropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptTypePropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ToggleButtonEx));

        public EnumEncryptType EncryptType
        {
            get { return (EnumEncryptType)GetValue(EncryptTypeProperty); }
            set { SetValue(EncryptTypeProperty, value); }
        }

        public event RoutedEventHandler EncryptTypePropertyChanged
        {
            add { AddHandler(EncryptTypePropertyEvent, value); }
            remove { RemoveHandler(EncryptTypePropertyEvent, value); }
        }

        private static void OnEncryptTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleButtonEx control = sender as ToggleButtonEx;
            if (control != null)
            {
                var newValue = (EnumEncryptType)args.NewValue;
                var oldValue = (EnumEncryptType)args.OldValue;

                RoutedPropertyChangedEventArgs<EnumEncryptType> e =
                    new RoutedPropertyChangedEventArgs<EnumEncryptType>(oldValue, newValue, EncryptTypePropertyEvent);

                control.OnEncryptTypePropertyChanged(e);
            }
        }

        virtual protected void OnEncryptTypePropertyChanged(RoutedPropertyChangedEventArgs<EnumEncryptType> e)
        {
            RaiseEvent(e);
        }
        #endregion

        #region WifiSignalLevel property
        public static readonly DependencyProperty WifiSignalLevelProperty =
                            DependencyProperty.Register("WifiSignalLevel",
                            typeof(EnumWifiSignalLevel),
                            typeof(ToggleButtonEx),
                            new PropertyMetadata(EnumWifiSignalLevel.Normal, new PropertyChangedCallback(OnWifiSignalLevelPropertyChanged)));

        public static readonly RoutedEvent WifiSignalLevelPropertyEvent =
                                           EventManager.RegisterRoutedEvent("WifiSignalLevelPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ToggleButtonEx));


        public EnumWifiSignalLevel WifiSignalLevel
        {
            get { return (EnumWifiSignalLevel)GetValue(WifiSignalLevelProperty); }
            set { SetValue(WifiSignalLevelProperty, value); }
        }

        public event RoutedEventHandler WifiSignalLevelPropertyChanged
        {
            add { AddHandler(WifiSignalLevelPropertyEvent, value); }
            remove { RemoveHandler(WifiSignalLevelPropertyEvent, value); }
        }

        private static void OnWifiSignalLevelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleButtonEx control = sender as ToggleButtonEx;
            if (control != null)
            {
                var newValue = (EnumWifiSignalLevel)args.NewValue;
                var oldValue = (EnumWifiSignalLevel)args.OldValue;

                RoutedPropertyChangedEventArgs<EnumWifiSignalLevel> e =
                    new RoutedPropertyChangedEventArgs<EnumWifiSignalLevel>(oldValue, newValue, WifiSignalLevelPropertyEvent);

                control.OnWifiSignalLevelPropertyChanged(e);
            }
        }

        virtual protected void OnWifiSignalLevelPropertyChanged(RoutedPropertyChangedEventArgs<EnumWifiSignalLevel> e)
        {
            RaiseEvent(e);
        }
        #endregion

        static ToggleButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonEx), new FrameworkPropertyMetadata(typeof(ToggleButtonEx)));
        }
    }
}
