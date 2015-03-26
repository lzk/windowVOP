﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace VOP.Controls
{
    enum EnumEncryptType
    {
        NoSecurity = 0,
        WEP = 1,
        WPA2_PSK_AES = 3,
        MixedModePSK = 4
    };

    enum EnumWifiSignalLevel
    {
        Weakness =0,
        Weak = 1,
        Normal = 2,
        Strong = 3,
        stronger = 4
    };

    class ToggleButtonEx : ToggleButton
    {
        public static readonly DependencyProperty SSIDTextProperty =
                                            DependencyProperty.Register("SSIDText",
                                            typeof(string),
                                            typeof(ToggleButtonEx),
                                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSSIDTextPropertyChanged)));

        public static readonly RoutedEvent SSIDTextPropertyEvent =
                                            EventManager.RegisterRoutedEvent("SSIDTextPropertyChanged", RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(ToggleButtonEx));

        public static readonly DependencyProperty EncryptionTextProperty =
                                    DependencyProperty.Register("EncryptionText",
                                    typeof(string),
                                    typeof(ToggleButtonEx),
                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEncryptionTextPropertyChanged)));

        public static readonly RoutedEvent EncryptionTextPropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptionTextPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ToggleButtonEx));

        public static readonly DependencyProperty EncryptTypeProperty =
                                    DependencyProperty.Register("EncryptType",
                                    typeof(EnumEncryptType),
                                    typeof(ToggleButtonEx),
                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEncryptTypePropertyChanged)));

        public static readonly RoutedEvent EncryptTypePropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptTypePropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ToggleButtonEx));

        public static readonly DependencyProperty WifiSignalLevelProperty =
                            DependencyProperty.Register("WifiSignalLevel",
                            typeof(EnumWifiSignalLevel),
                            typeof(ToggleButtonEx),
                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWifiSignalLevelPropertyChanged)));

        public static readonly RoutedEvent WifiSignalLevelPropertyEvent =
                                           EventManager.RegisterRoutedEvent("WifiSignalLevelPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(ToggleButtonEx));

        static ToggleButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonEx), new FrameworkPropertyMetadata(typeof(ToggleButtonEx)));
        }

        public string SSIDText
        {
            get { return (string)GetValue(SSIDTextProperty); }
            set { SetValue(SSIDTextProperty, value); }
        }

        public string EncryptionText
        {
            get { return (string)GetValue(EncryptionTextProperty); }
            set { SetValue(EncryptionTextProperty, value); }
        }

        public EnumEncryptType EncryptType
        {
            get { return (EnumEncryptType)GetValue(EncryptTypeProperty); }
            set { SetValue(EncryptTypeProperty, value); }
        }

        public EnumWifiSignalLevel WifiSignalLevel
        {
            get { return (EnumWifiSignalLevel)GetValue(WifiSignalLevelProperty); }
            set { SetValue(WifiSignalLevelProperty, value); }
        }
        
        public event RoutedEventHandler SSIDTextPropertyChanged
        {
            add { AddHandler(SSIDTextPropertyEvent, value); }
            remove { RemoveHandler(SSIDTextPropertyEvent, value); }
        }

        public event RoutedEventHandler EncryptionTextPropertyChanged
        {
            add { AddHandler(EncryptionTextPropertyEvent, value); }
            remove { RemoveHandler(EncryptionTextPropertyEvent, value); }
        }

        public event RoutedEventHandler EncryptTypePropertyChanged
        {
            add { AddHandler(EncryptTypePropertyEvent, value); }
            remove { RemoveHandler(EncryptTypePropertyEvent, value); }
        }

        public event RoutedEventHandler WifiSignalLevelPropertyChanged
        {
            add { AddHandler(WifiSignalLevelPropertyEvent, value); }
            remove { RemoveHandler(WifiSignalLevelPropertyEvent, value); }
        }
        private static void OnSSIDTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.ToggleButtonEx btn = sender as VOP.Controls.ToggleButtonEx;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButtonEx.SSIDTextPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }

        private static void OnEncryptionTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.ToggleButtonEx btn = sender as VOP.Controls.ToggleButtonEx;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButtonEx.EncryptionTextPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }

        private static void OnEncryptTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.ToggleButtonEx btn = sender as VOP.Controls.ToggleButtonEx;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButtonEx.EncryptTypePropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }

        private static void OnWifiSignalLevelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.ToggleButtonEx btn = sender as VOP.Controls.ToggleButtonEx;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButtonEx.WifiSignalLevelPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
    }
}
