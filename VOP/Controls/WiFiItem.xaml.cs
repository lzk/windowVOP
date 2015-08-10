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
        public byte wifiEnable = 0;
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
                                            new PropertyMetadata("", new PropertyChangedCallback(OnSSIDTextPropertyChanged)));

        public static readonly RoutedEvent SSIDTextPropertyEvent =
                                            EventManager.RegisterRoutedEvent("SSIDTextPropertyChanged", RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(WiFiItem));

        public string SSIDText
        {
            get { return (string)GetValue(SSIDTextProperty); }
            set { 
                SetValue(SSIDTextProperty, value);
                wepKey0.GroupName = "WepKeyRadio" + value;
                wepKey1.GroupName = "WepKeyRadio" + value;
                wepKey2.GroupName = "WepKeyRadio" + value;
                wepKey3.GroupName = "WepKeyRadio" + value;
            }
        }

        public event RoutedEventHandler SSIDTextPropertyChanged
        {
            add { AddHandler(SSIDTextPropertyEvent, value); }
            remove { RemoveHandler(SSIDTextPropertyEvent, value); }
        }

        private static void OnSSIDTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WiFiItem control = sender as WiFiItem;
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
                                    typeof(WiFiItem),
                                    new PropertyMetadata("", new PropertyChangedCallback(OnEncryptionTextPropertyChanged)));

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

        private static void OnEncryptionTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WiFiItem control = sender as WiFiItem;
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
                                    typeof(WiFiItem),
                                    new PropertyMetadata(EnumEncryptType.WPA2_PSK_AES, new PropertyChangedCallback(OnEncryptTypePropertyChanged)));

        public static readonly RoutedEvent EncryptTypePropertyEvent =
                                           EventManager.RegisterRoutedEvent("EncryptTypePropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(WiFiItem));

        public EnumEncryptType EncryptType
        {
            get { return (EnumEncryptType)GetValue(EncryptTypeProperty); }
            set { 
                SetValue(EncryptTypeProperty, value);

                if (value == EnumEncryptType.WEP)
                {
                    pbPwd.MaxLength = 26;
                    tbPwd.MaxLength = 26;
                }
                else
                {
                    tbPwd.MaxLength = 64;
                    pbPwd.MaxLength = 64;
                }
            }
        }

        public event RoutedEventHandler EncryptTypePropertyChanged
        {
            add { AddHandler(EncryptTypePropertyEvent, value); }
            remove { RemoveHandler(EncryptTypePropertyEvent, value); }
        }

        private static void OnEncryptTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WiFiItem control = sender as WiFiItem;
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
                            typeof(WiFiItem),
                            new PropertyMetadata(EnumWifiSignalLevel.Normal, new PropertyChangedCallback(OnWifiSignalLevelPropertyChanged)));

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

        private static void OnWifiSignalLevelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WiFiItem control = sender as WiFiItem;
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

        #region Connected property
        public static readonly DependencyProperty ConnectedProperty =
                            DependencyProperty.Register("Connected",
                            typeof(bool),
                            typeof(WiFiItem),
                            new PropertyMetadata(false, new PropertyChangedCallback(OnConnectedPropertyChanged)));

        public static readonly RoutedEvent ConnectedPropertyEvent =
                                           EventManager.RegisterRoutedEvent("ConnectedPropertyChanged", RoutingStrategy.Bubble,
                                           typeof(RoutedEventHandler),
                                           typeof(WiFiItem));

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
            WiFiItem control = sender as WiFiItem;
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
            set { SetValue(IsExpandedProperty, value); }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WiFiItem control = sender as WiFiItem;
            if (control != null)
            {
                var newValue = (bool)args.NewValue;
                var oldValue = (bool)args.OldValue;

                RoutedPropertyChangedEventArgs<bool> e =
                    new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue, WifiSignalLevelPropertyEvent);

                control.OnIsExpandedPropertyChanged(e);
            }
        }

        virtual protected void OnIsExpandedPropertyChanged(RoutedPropertyChangedEventArgs<bool> e)
        {
            RaiseEvent(e);
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
            if (EncryptType == EnumEncryptType.WEP)
            {
                wepGrid.Visibility = Visibility.Visible;
                rowWep.Height = GridLength.Auto;
                wepKey0.IsChecked = true;
            }
            else 
            {
                if (EncryptType == EnumEncryptType.NoSecurity)
                {
                    cbDisplayPwd.IsEnabled = false;
                    tbPwd.IsEnabled = false;
                    pbPwd.IsEnabled = false;
                    btnConnect.IsEnabled = true;
                }
                wepGrid.Visibility = Visibility.Hidden;
            }

            if (EncryptType == EnumEncryptType.NoSecurity)
            {
                tkPwd.IsEnabled = false;
            }
            else
            {
                tkPwd.IsEnabled = true;
            }

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
         //   PasswordChanged();
        }

        private void pbPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // PasswordChanged();
        }

        private void PasswordChanged()
        {
            if (EncryptType != EnumEncryptType.NoSecurity)
            {
                if(is_InputVailible())
                {
                    btnConnect.IsEnabled = true;
                } 
                else
                {
                    btnConnect.IsEnabled = false;
                }
            }
            else
            {
                btnConnect.IsEnabled = true;
            }
        }

        public bool is_InputVailible()
        {
            string pwd = "";
            byte wepKeyId = 0;

            GetUIValues(out pwd, out wepKeyId);

            bool bValidatePassWord = true;
            int nCharCount = pwd.Length;

            if (EncryptType == EnumEncryptType.NoSecurity)
            {
                return true;
            }
            else if (EncryptType == EnumEncryptType.WEP)
            {
                switch (nCharCount)
                {
                    case 5:
                    case 13:
                        break;
                    case 10:
                    case 26:
                        foreach (char ch in pwd)
                        {
                            if (Char.IsDigit(ch) || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f'))
                            {
                                continue;
                            }
                            else
                            {
                                bValidatePassWord = false;
                                break;
                            }
                        }
                        break;
                    default:
                        bValidatePassWord = false;
                        break;
                }

                return bValidatePassWord;
            }
            else if (EncryptType == EnumEncryptType.WPA2_PSK_AES || EncryptType == EnumEncryptType.MixedModePSK)
            {
                if (nCharCount >= 8 && nCharCount <= 63)
                {
                    foreach (char ch in pwd)
                    {
                        if (((UInt16)ch) > 128)
                        {
                            return false;
                        }
                    }
                    bValidatePassWord = true;
                }
                else if (nCharCount == 64)
                {
                    foreach (char ch in pwd)
                    {
                        if (Char.IsDigit(ch) || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f'))
                        {
                            continue;
                        }
                        else
                        {
                            bValidatePassWord = false;
                            break;
                        }
                    }
                }
                else
                {
                    bValidatePassWord = false;
                }
                return bValidatePassWord;
            }
            else
            {
                return false;
            }
        }

        private void GetUIValues(out string pwd,out byte wepKeyId)
        {
            pwd = "";
            wepKeyId = 1;
            
            pwd = (true == cbDisplayPwd.IsChecked) ? tbPwd.Text : pbPwd.Password;
            
            if (true == wepKey0.IsChecked)
            {
                wepKeyId = 0x01;
            }
            else if (true == wepKey1.IsChecked)
            {
                wepKeyId = 0x02;
            }
            else if (true == wepKey2.IsChecked)
            {
                wepKeyId = 0x03;
            }
            else if (true == wepKey3.IsChecked)
            {
                wepKeyId = 0x04;
            }
        }

        public bool apply()
        {
            bool isApplySuccess = false;

            string pwd = "";
            byte wepKeyId = 1;
            byte wifiEnable = 1;

            GetUIValues(out pwd, out wepKeyId);

            if (is_InputVailible())
            {
                if (EncryptType == EnumEncryptType.NoSecurity)
                {
                    pwd = "";
                }
                else if (EncryptType == EnumEncryptType.WEP)
                {
                    if (pwd.Length > 26)
                    {
                        pwd = pwd.Substring(1, 26);
                    }
                }
                else
                {

                }
                
                byte wifiInit = 0;
                dll.GetWifiChangeStatus(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref wifiInit);
                VOP.MainWindow.m_byWifiInitStatus = wifiInit;

                WiFiInfoRecord m_rec = new WiFiInfoRecord(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter,
                    wifiEnable,
                    0,
                    SSIDText, 
                    (EncryptType != EnumEncryptType.NoSecurity) ? pwd : "", 
                    EncryptType, 
                    wepKeyId);
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeMethod<WiFiInfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.SetWiFiInfo, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        isApplySuccess = true;
                    }

                }
            }

            if (isApplySuccess)
            {
                Connected = true;
                EncryptionText = (string)this.FindResource("ResStr_Connected");

                tbPwd.Text = pwd;
                pbPwd.Password = pwd;
            }
            
            if (isApplySuccess)
            {
                if (wifiEnable != VOP.MainWindow.m_byWifiInitStatus)
                    ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Msg_1"), Brushes.Black);
                else
                    ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Setting_Successfully_"), Brushes.Black);
            }
            else
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Setting_Fail"), Brushes.Red);
 
            return isApplySuccess;
        }

        private void OnClickConnectBtn(object sender, RoutedEventArgs e)
        {
            if(is_InputVailible())
            {
                apply();
            }
            else
            {
                if(EncryptType == EnumEncryptType.WEP)
                    VOP.Controls.MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_Msg_2"), (string)this.FindResource("ResStr_Warning"));
                else
                    VOP.Controls.MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_Msg_3"), (string)this.FindResource("ResStr_Warning"));
            }
        }
    }
}
