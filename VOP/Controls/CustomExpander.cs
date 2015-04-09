using System.Windows;
using System.Windows.Controls;


namespace VOP.Controls
{
    class CustomExpander : Expander
    {
        #region SSIDText property
        public static readonly DependencyProperty SSIDTextProperty =
                                            DependencyProperty.Register("SSIDText",
                                            typeof(string),
                                            typeof(CustomExpander),
                                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSSIDTextPropertyChanged)));

        public static readonly RoutedEvent SSIDTextPropertyEvent =
                                            EventManager.RegisterRoutedEvent("SSIDTextPropertyChanged", RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(CustomExpander));

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

        private static void OnSSIDTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.CustomExpander btn = sender as VOP.Controls.CustomExpander;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(CustomExpander.SSIDTextPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }

        #endregion

        #region EncryptionText property
        public static readonly DependencyProperty EncryptionTextProperty =
                                    DependencyProperty.Register("EncryptionText",
                                    typeof(string),
                                    typeof(CustomExpander),
                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEncryptionTextPropertyChanged)));

        public static readonly RoutedEvent EncryptionTextPropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptionTextPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(CustomExpander));

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

        private static void OnEncryptionTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.CustomExpander btn = sender as VOP.Controls.CustomExpander;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(CustomExpander.EncryptionTextPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
        #endregion

        #region EncryptType property
        public static readonly DependencyProperty EncryptTypeProperty =
                                    DependencyProperty.Register("EncryptType",
                                    typeof(EnumEncryptType),
                                    typeof(CustomExpander),
                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEncryptTypePropertyChanged)));

        public static readonly RoutedEvent EncryptTypePropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptTypePropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(CustomExpander));

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
        private static void OnEncryptTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.CustomExpander btn = sender as VOP.Controls.CustomExpander;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(CustomExpander.EncryptTypePropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
        #endregion

        #region WifiSignalLevel property
        public static readonly DependencyProperty WifiSignalLevelProperty =
                            DependencyProperty.Register("WifiSignalLevel",
                            typeof(EnumWifiSignalLevel),
                            typeof(CustomExpander),
                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWifiSignalLevelPropertyChanged)));

        public static readonly RoutedEvent WifiSignalLevelPropertyEvent =
                                           EventManager.RegisterRoutedEvent("WifiSignalLevelPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(CustomExpander));


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

        private static void OnWifiSignalLevelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.CustomExpander btn = sender as VOP.Controls.CustomExpander;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(CustomExpander.WifiSignalLevelPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
        #endregion

        static CustomExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomExpander), new FrameworkPropertyMetadata(typeof(CustomExpander)));
        }
    }
}
