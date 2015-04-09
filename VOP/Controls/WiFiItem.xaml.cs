using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for WiFiItem.xaml
    /// </summary>
    public class WifiSetting
    {
        public string m_ssid = "";
        public string m_pwd = "";
        public byte m_encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
        public byte m_wepKeyId = 1;
    }

    public partial class WiFiItem : UserControl
    {
        public WifiSetting wifiSetting = new WifiSetting();

        #region SSIDText property
        public static readonly DependencyProperty SSIDTextProperty =
                                            DependencyProperty.Register("SSIDText",
                                            typeof(string),
                                            typeof(WiFiItem),
                                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSSIDTextPropertyChanged)));

        public static readonly RoutedEvent SSIDTextPropertyEvent =
                                            EventManager.RegisterRoutedEvent("SSIDTextPropertyChanged", RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(WiFiItem));

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
            VOP.Controls.WiFiItem btn = sender as VOP.Controls.WiFiItem;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(WiFiItem.SSIDTextPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }

        #endregion

        #region EncryptionText property
        public static readonly DependencyProperty EncryptionTextProperty =
                                    DependencyProperty.Register("EncryptionText",
                                    typeof(string),
                                    typeof(WiFiItem),
                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEncryptionTextPropertyChanged)));

        public static readonly RoutedEvent EncryptionTextPropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptionTextPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(WiFiItem));

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
            VOP.Controls.WiFiItem btn = sender as VOP.Controls.WiFiItem;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(WiFiItem.EncryptionTextPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
        #endregion

        #region EncryptType property
        public static readonly DependencyProperty EncryptTypeProperty =
                                    DependencyProperty.Register("EncryptType",
                                    typeof(EnumEncryptType),
                                    typeof(WiFiItem),
                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEncryptTypePropertyChanged)));

        public static readonly RoutedEvent EncryptTypePropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptTypePropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(WiFiItem));

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
            VOP.Controls.WiFiItem btn = sender as VOP.Controls.WiFiItem;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(WiFiItem.EncryptTypePropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
        #endregion

        #region WifiSignalLevel property
        public static readonly DependencyProperty WifiSignalLevelProperty =
                            DependencyProperty.Register("WifiSignalLevel",
                            typeof(EnumWifiSignalLevel),
                            typeof(WiFiItem),
                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWifiSignalLevelPropertyChanged)));

        public static readonly RoutedEvent WifiSignalLevelPropertyEvent =
                                           EventManager.RegisterRoutedEvent("WifiSignalLevelPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(WiFiItem));


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
            VOP.Controls.WiFiItem btn = sender as VOP.Controls.WiFiItem;
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(WiFiItem.WifiSignalLevelPropertyEvent);
            btn.RaiseEvent(routedEventArgs);
        }
        #endregion


        #region IsExpanded property
        public static readonly DependencyProperty IsExpandedProperty =
                            DependencyProperty.Register("IsExpanded",
                            typeof(bool),
                            typeof(WiFiItem),
                            new PropertyMetadata(false, new PropertyChangedCallback(OnIsExpandedPropertyChanged)));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set 
            { 
                SetValue(IsExpandedProperty, value);
            }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.WiFiItem wi = sender as VOP.Controls.WiFiItem;
            wi.wifiExpander.IsExpanded = (bool)e.NewValue;
        }

        public static readonly RoutedEvent IsExpandedPropertyEvent =
                                   EventManager.RegisterRoutedEvent("IsExpandedPropertyChanged",
                                   RoutingStrategy.Direct,
                                   typeof(RoutedPropertyChangedEventHandler<object>), typeof(WiFiItem));


        public event RoutedPropertyChangedEventHandler<object> IsExpandedPropertyChanged
        {
            add { AddHandler(IsExpandedPropertyEvent, value); }
            remove { RemoveHandler(IsExpandedPropertyEvent, value); }
        }

  
        #endregion

        public WiFiItem()
        {
            InitializeComponent();
        }

        private void OnLoadWifiItem(object sender, RoutedEventArgs e)
        {
            if(wifiExpander.EncryptType == EnumEncryptType.WEP)
            {
                wepGrid.Visibility = Visibility.Visible;
                rowWep.Height = GridLength.Auto;
                wepKey0.IsChecked = true;
            }
            else
            {
                wepGrid.Visibility = Visibility.Hidden;
            }
            //wifiExpander.SSIDText = SSIDText;
            //wifiExpander.EncryptionText = EncryptionText;
            //wifiExpander.EncryptType = EncryptType;

            wifiSetting.m_ssid = wifiExpander.SSIDText;
            wifiSetting.m_encryption = (byte)wifiExpander.EncryptType;

        }

        private void OnClickCancelButton(object sender, RoutedEventArgs e)
        {
            wifiExpander.IsExpanded = false;
        }

        private void OnClickDisplayPWD(object sender, RoutedEventArgs e)
        {
            if(true == cbDisplayPwd.IsChecked)
            {
                pbPwd.Visibility = Visibility.Hidden;
                tbPwd.Visibility = Visibility.Visible;
                tbPwd.Text = pbPwd.Password;
            }
            else
            {
                pbPwd.Visibility = Visibility.Visible;
                tbPwd.Visibility = Visibility.Hidden;
                pbPwd.Password = tbPwd.Text;
            }
        }

        private void tbPwd_TextChanged(object sender, TextChangedEventArgs e)
        {
  
        }

        private void pbPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void wifiExpander_IsExpandedPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.CustomExpander ce = sender as VOP.Controls.CustomExpander;
        }

      
    }
}
