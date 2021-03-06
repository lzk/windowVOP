﻿using System;
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
using VOP.Controls;
using System.Text.RegularExpressions;

namespace VOP
{
    /// <summary>
    /// Interaction logic for WifiView.xaml
    /// </summary>
    public partial class WifiView_Rufous : UserControl
    {
 //       VOP.Controls.WifiSetting wifiSettingInit = new VOP.Controls.WifiSetting();
        VOP.Controls.WifiSetting wifiSetting = new VOP.Controls.WifiSetting();
        private bool m_bConnectOthApMode = false;
        private bool m_currentStatus = false;
       
        public WifiView_Rufous()
        {
            InitializeComponent();
        }

        private void OnLoadWifiView(object sender, RoutedEventArgs e)
        {
            if (_MainWin.settingPageIndex != 3)
                return;

            btnConnectOthAp.Visibility = Visibility.Hidden;
            btnConnectOthAp.IsEnabled = false;
            manualConnect.Visibility = Visibility.Hidden;
            manualConnect.IsEnabled = false;
            rowManual.Height = new GridLength(0);
            autoConnect.Visibility = Visibility.Hidden;
            gdWifi.Visibility = Visibility.Hidden;
            gdWifi.IsEnabled = false;
            autoConnect.IsEnabled = false;
            rowAuto.Height = GridLength.Auto;
            m_bConnectOthApMode = false;

            WiFiInfoRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            worker.InvokeMethod<WiFiInfoRecord>("", ref m_rec, DllMethodType.GetWiFiInfo, this);

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                wifiSetting.wifiEnable = m_rec.WifiEnable;
                wifiSetting.m_ssid = m_rec.SSID;
                wifiSetting.m_pwd = m_rec.PWD;
                wifiSetting.m_encryption = (byte)m_rec.Encryption;
                wifiSetting.m_wepKeyId = m_rec.WepKeyId;
            }

            if (null != m_rec && 0x01 == (m_rec.WifiEnable & 0x01))
            {
                chkWifi.IsChecked = true;
                btnConnectOthAp.Visibility = Visibility.Visible;
                btnConnectOthAp.IsEnabled = true;
                manualConnect.Visibility = Visibility.Hidden;
                manualConnect.IsEnabled = false;
                rowManual.Height = new GridLength(0);
                autoConnect.Visibility = Visibility.Visible;
                autoConnect.IsEnabled = true;
                gdWifi.Visibility = Visibility.Visible;
                gdWifi.IsEnabled = true;
                rowAuto.Height = GridLength.Auto;
                
                cbo_ssid_refresh();
            }
            else
            {
                chkWifi.IsChecked = false;
            }

            scrollview.ScrollToTop();

            chkDisplayPwd.IsChecked = false;
        }

        public bool is_InputVailible()
        {
            // wifi config
            string ssid = "";
            string pwd = "";
            byte encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
            byte wepKeyId = 0;

            GetUIValues(out ssid, out pwd, out encryption, out wepKeyId);

            bool bValidatePassWord = true;
            int nCharCount = pwd.Length;

            if (encryption == (byte)EnumEncryptType.NoSecurity)
            {
                return (ssid.Length > 0 && ssid.Length <= 32);
            }
            else if (encryption == (byte)EnumEncryptType.WEP)
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

                return (ssid.Length > 0 && ssid.Length <= 32) && bValidatePassWord;
            }
            else if (encryption == (byte)EnumEncryptType.WPA2_PSK_AES || encryption == (byte)EnumEncryptType.MixedModePSK)
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
                return ((ssid.Length > 0 && ssid.Length <= 32) && bValidatePassWord);
            }
            else
            {
                return false;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "btnConnectOthAp")
            {
                btnConnectOthAp.Visibility = Visibility.Hidden;
                btnConnectOthAp.IsEnabled = false;
                manualConnect.Visibility = Visibility.Visible;
                manualConnect.IsEnabled = true;
                rowManual.Height = GridLength.Auto;
                autoConnect.Visibility = Visibility.Hidden;
                autoConnect.IsEnabled = false;
                //modified by yunying shang 2017-12-05 for BMS　1651
                gdWifi.Visibility = Visibility.Visible;
                gdWifi.IsEnabled = true;
                //<<=================1651
                rowAuto.Height = new GridLength(0);
                wepKey0.IsChecked = true;
                m_bConnectOthApMode = true;
                chkDisplayPwd.IsChecked = false;


                wifiSetting.m_ssid = "";
                wifiSetting.m_pwd = "";
                wifiSetting.m_encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
                wifiSetting.m_wepKeyId = 1;

                WiFiInfoRecord m_rec = null;
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                worker.InvokeMethod<WiFiInfoRecord>("", ref m_rec, DllMethodType.GetWiFiInfo, this);
                
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    wifiSetting.m_ssid = m_rec.SSID;
                    wifiSetting.m_pwd = m_rec.PWD;
                    wifiSetting.m_encryption = (byte)m_rec.Encryption;
                    wifiSetting.m_wepKeyId = m_rec.WepKeyId;
                }
                
                tbSSID.Text = wifiSetting.m_ssid;
                tbPwd.Text = wifiSetting.m_pwd;
                pbPwd.Password = wifiSetting.m_pwd;
                common.SelectItemByContext(cboEncrytion, wifiSetting.m_encryption);

                if (wifiSetting.m_wepKeyId == 0X01)
                {
                    wepKey0.IsChecked = true;
                }
                else if (wifiSetting.m_wepKeyId == 0X02)
                {
                    wepKey1.IsChecked = true;
                }
                else if (wifiSetting.m_wepKeyId == 0X03)
                {
                    wepKey2.IsChecked = true;
                }
                else if (wifiSetting.m_wepKeyId == 0X04)
                {
                    wepKey3.IsChecked = true;
                }

                UpdateControlsStatus();
                scrollview.ScrollToTop();
            }
            else if (btn.Name == "btnCancel")
            {
                btnConnectOthAp.Visibility = Visibility.Visible;
                btnConnectOthAp.IsEnabled = true;
                manualConnect.Visibility = Visibility.Hidden;
                manualConnect.IsEnabled = false;
                rowManual.Height = new GridLength(0);
                autoConnect.Visibility = Visibility.Visible;
                autoConnect.IsEnabled = true;
                gdWifi.Visibility = Visibility.Visible;
                gdWifi.IsEnabled = true;
                rowAuto.Height = GridLength.Auto;
                scrollview.ScrollToTop();
                m_bConnectOthApMode = false;
            }
            else if(btn.Name == "btnConnect")
            {
                apply();
            }
        }

        public bool apply()
        {
            bool isApplySuccess = false;

            string ssid = "";
            string pwd = "";
            byte encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
            byte wepKeyId = 1;
            byte wifiEnable = 1;
            GetUIValues(out ssid, out pwd, out encryption, out wepKeyId);

            if (is_InputVailible())
            {
                if (encryption == (byte)EnumEncryptType.NoSecurity)
                {
                    pwd = "";
                }
                else if (encryption == (byte)EnumEncryptType.WEP)
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
                // Begin: Fix bug # 61388
                int result = dll.GetWifiChangeStatus("", ref wifiInit);
                if ( 0 == result) // success?
                {
                    VOP.MainWindow_Rufous.m_byWifiInitStatus = wifiInit;
                }
                else
                {                    
                    Win32.OutputDebugString("apply->dll.GetWifiChangeStatus() ---> failed...  ， Result =    " + result.ToString());
                }
                // End: Fix bug # 61388

                WiFiInfoRecord m_rec = new WiFiInfoRecord("", wifiEnable, 0, ssid, (encryption != (byte)EnumEncryptType.NoSecurity) ? pwd : "", (EnumEncryptType)encryption, wepKeyId);
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeMethod<WiFiInfoRecord>("", ref m_rec, DllMethodType.SetWiFiInfo, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        wifiSetting.wifiEnable = m_rec.WifiEnable;
                        wifiSetting.m_ssid = ssid;
                        wifiSetting.m_pwd = pwd;
                        wifiSetting.m_encryption = encryption;
                        wifiSetting.m_wepKeyId = wepKeyId;
                        isApplySuccess = true;
                    }

                }

                if (isApplySuccess)
                {
                    if (wifiEnable != VOP.MainWindow_Rufous.m_byWifiInitStatus)
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                           Application.Current.MainWindow,
                          (string)this.FindResource("ResStr_Msg_1"),
                          (string)this.TryFindResource("ResStr_Prompt"));
                    else
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                         Application.Current.MainWindow,
                        (string)this.FindResource("ResStr_Setting_Successfully_"),
                        (string)this.TryFindResource("ResStr_Prompt"));
                }
                else
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                 Application.Current.MainWindow,
                (string)this.FindResource("ResStr_Setting_Fail"),
                (string)this.TryFindResource("ResStr_Prompt"));
            }
            else
            {
                if (ssid.Length <= 0 || ssid.Length > 32)
                {
                    VOP.Controls.MessageBoxEx.Show(MessageBoxExStyle.Simple_Warning, Application.Current.MainWindow, 
                        (string)this.FindResource("ResStr_Msg_9"), (string)this.FindResource("ResStr_Warning"));
                }
                else
                {
                    if (encryption == (byte)EnumEncryptType.WEP)
                    {
                        VOP.Controls.MessageBoxEx.Show(MessageBoxExStyle.Simple_Warning, 
                            Application.Current.MainWindow, (string)this.FindResource("ResStr_Msg_2"), 
                            (string)this.FindResource("ResStr_Warning"));
                    }
                    else
                    {
                        VOP.Controls.MessageBoxEx.Show(MessageBoxExStyle.Simple_Warning, 
                            Application.Current.MainWindow, 
                            (string)this.FindResource("ResStr_Msg_3"),
                            (string)this.FindResource("ResStr_Warning"));
                    }
                }
            }

            //if (isApplySuccess && encryption == (byte)EnumEncryptType.NoSecurity)
            //    tb_pwd.Text = pwd;

            return isApplySuccess;
        }

        public void cbo_ssid_refresh(bool _bDisplayProgressBar = true)
        {
            wifilist.Children.Clear();

            byte wifiInit = 0;

            int result = dll.GetWifiChangeStatus("", ref wifiInit);
            // Begin: Fix bug # 61388
            if (0 == result) // success?
            {
                VOP.MainWindow_Rufous.m_byWifiInitStatus = wifiInit;
            }
            else
            {
                Win32.OutputDebugString("cbo_ssid_refresh->dll.GetWifiChangeStatus() ---> failed...  ， Result =    " + result.ToString());
            }
            // End: Fix bug # 61388

            if (wifiInit == 0x00)
                return;

            ApListRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<ApListRecord>("", ref m_rec, DllMethodType.GetApList, this);
            }
            else
            {
                m_rec = worker.GetApList("");
            }

            //WiFiInfoRecord m_wifi = null;
            //AsyncWorker worker1 = new AsyncWorker(Application.Current.MainWindow);

            //worker1.InvokeMethod<WiFiInfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_wifi, DllMethodType.GetWiFiInfo, this);
            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                for (int i = 0; i < m_rec.SsidList.Count; i++)
                {
                    if (!String.IsNullOrEmpty(m_rec.SsidList[i]))
                    {
                        VOP.Controls.WiFiItem wifiitem = new VOP.Controls.WiFiItem();
                        Byte[] strTemp = Encoding.Default.GetBytes(m_rec.SsidList[i]);//bms#1340
                        wifiitem.SSIDText = Encoding.UTF8.GetString(strTemp);
                        if ((byte)EnumEncryptType.NoSecurity == m_rec.EncryptionList[i])    //No Security
                        {
                            wifiitem.EncryptionText = (string)this.FindResource("ResStr_No_Security");
                            wifiitem.EncryptType = VOP.EnumEncryptType.NoSecurity;
                        }
                        else if ((byte)EnumEncryptType.WEP == m_rec.EncryptionList[i]) //WEP
                        {
                            wifiitem.EncryptionText = (string)this.FindResource("ResStr_Protected_by_WEP");
                            wifiitem.EncryptType = VOP.EnumEncryptType.WEP;
                        }
                        else if ((byte)EnumEncryptType.WPA2_PSK_AES == m_rec.EncryptionList[i])   //3. WPA2-PSK-AES 
                        {
                            wifiitem.EncryptionText = (string)this.FindResource("ResStr_Protected_by_WPA2"); 
                            wifiitem.EncryptType = VOP.EnumEncryptType.WPA2_PSK_AES;
                        }
                        else if ((byte)EnumEncryptType.MixedModePSK == m_rec.EncryptionList[i])   //4.Mixed Mode PSK
                        {
                            wifiitem.EncryptionText = (string)this.FindResource("ResStr_Protected_by_Mixed_Mode_PSK");
                            wifiitem.EncryptType = VOP.EnumEncryptType.MixedModePSK;
                        }
                        wifiitem.WifiSignalLevel = VOP.Controls.EnumWifiSignalLevel.stronger;

                        if (m_rec.ConnectedStatusList[i] == true)
                        {
                            wifiitem.Connected = true;
                            wifiitem.EncryptionText = (string)this.FindResource("ResStr_Connected");
                        }
                        wifiitem.ConnectedPropertyChanged += wifiitem_ConnectedPropertyChanged;
                        if(!CheckStringChinessReg(wifiitem.SSIDText))
                            wifilist.Children.Add(wifiitem);
                    }
                }
            }
        }
        public bool CheckStringChinessReg(string text)
        {
            bool res = false;
            if (Regex.IsMatch(text, @"[\u4e00-\u9fbb]+$"))
                res = true;
            return res;
        }
        private void wifiitem_ConnectedPropertyChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                WiFiItem wi = sender as WiFiItem;

                foreach (WiFiItem obj in wifilist.Children)
                {
                    if (wi.SSIDText != obj.SSIDText
                        && obj.Connected == true
                        && wi.Connected == true)
                    {
                        obj.Connected = false;

                        if (EnumEncryptType.NoSecurity == obj.EncryptType)    //No Security
                        {
                            obj.EncryptionText = (string)this.FindResource("ResStr_No_Security");
                        }
                        else if (EnumEncryptType.WEP == obj.EncryptType) //WEP
                        {
                            obj.EncryptionText = (string)this.FindResource("ResStr_Protected_by_WEP");
                        }
                        else if (EnumEncryptType.WPA2_PSK_AES == obj.EncryptType)   //3. WPA2-PSK-AES 
                        {
                            obj.EncryptionText = (string)this.FindResource("ResStr_Protected_by_WPA2");
                        }
                        else if (EnumEncryptType.MixedModePSK == obj.EncryptType)   //4.Mixed Mode PSK
                        {
                            obj.EncryptionText = (string)this.FindResource("ResStr_Protected_by_Mixed_Mode_PSK");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void RefreshButton_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            cbo_ssid_refresh();
        }

        private void OncboEncrytionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateControlsStatus();
        }

        private void GetUIValues(out string ssid, out string pwd, out byte encryption, out byte wepKeyId)
        {
            ssid = "";
            pwd = "";
            encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
            wepKeyId = 0;

            ComboBoxItem cbo_item_encrypt = cboEncrytion.SelectedItem as ComboBoxItem;

            if (null != cbo_item_encrypt
                    && null != cbo_item_encrypt.DataContext)
            {
                ssid = tbSSID.Text;
                encryption = (byte)cbo_item_encrypt.DataContext;
               
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

                if (true == chkDisplayPwd.IsChecked)
                {
                    pwd = tbPwd.Text;
                }
                else
                {
                    pwd = pbPwd.Password;
                }

                if (null == ssid)
                    ssid = "";

                if (null == pwd)
                    pwd = "";
            }
        }

        private void OnClickDisplayPWD(object sender, RoutedEventArgs e)
        {
            if (true == chkDisplayPwd.IsChecked)
            {
                pbPwd.Visibility = Visibility.Hidden;
                pbPwd.IsEnabled = false;
                tbPwd.Visibility = Visibility.Visible;
                tbPwd.IsEnabled = true;
                tbPwd.Text = pbPwd.Password;
            }
            else
            {
                pbPwd.Visibility = Visibility.Visible;
                pbPwd.IsEnabled = true;
                tbPwd.Visibility = Visibility.Hidden;
                tbPwd.IsEnabled = false;
                pbPwd.Password = tbPwd.Text;
            }
        }

        public void UpdateControlsStatus()
        {
            string ssid = "";
            string pwd = "";
            byte encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
            byte wepKeyId = 0;

            GetUIValues(out ssid, out pwd, out encryption, out wepKeyId);

            if (encryption == (byte)EnumEncryptType.NoSecurity)
            {
                //tbkPwd.Text = "Passphrase";            
                chkDisplayPwd.IsEnabled = false;
                tbPwd.MaxLength = 26;
                tbPwd.IsEnabled = false;
                pbPwd.MaxLength = 26;
                pbPwd.IsEnabled = false;
                if (null != wepGrid)
                {
                    wepGrid.Visibility = Visibility.Hidden;
                    wepGrid.IsEnabled = false;
                }
            }
            else if (encryption == (byte)EnumEncryptType.WEP)
            {
              //  tbkPwd.Text = "WEP Key";
                chkDisplayPwd.IsEnabled = true;
                tbPwd.MaxLength = 26;
                tbPwd.IsEnabled = true;
                pbPwd.MaxLength = 26;
                pbPwd.IsEnabled = true;
                if (null != wepGrid)
                {
                    wepGrid.Visibility = Visibility.Visible;
                    wepGrid.IsEnabled = true;
                }                   
            }
            else
            {
                //tbkPwd.Text = "Passphrase";                
                chkDisplayPwd.IsEnabled = true;
                tbPwd.MaxLength = 64;
                tbPwd.IsEnabled = true;
                pbPwd.MaxLength = 64;
                pbPwd.IsEnabled = true;
                if (null != wepGrid)
                {
                    wepGrid.Visibility = Visibility.Hidden;
                    wepGrid.IsEnabled = false;
                }
            }

            if (encryption == (byte)EnumEncryptType.NoSecurity)
            {
                tbkPwd.IsEnabled = false;
            }
            else
            {
                tbkPwd.IsEnabled = true;
            }
        }
        
        private void OnchkWifiChecked(object sender, RoutedEventArgs e)
        {
            if (null != btnConnectOthAp &&
                null != autoConnect &&
                null != manualConnect)
            {
                if (m_bConnectOthApMode)
                {
                    btnConnectOthAp.Visibility = Visibility.Hidden;
                    btnConnectOthAp.IsEnabled = false;
                    autoConnect.Visibility = Visibility.Hidden;
                    autoConnect.IsEnabled = false;
                    manualConnect.Visibility = Visibility.Visible;
                    manualConnect.IsEnabled = true;
                    gdWifi.Visibility = Visibility.Visible;
                    gdWifi.IsEnabled = true;
                    rowManual.Height = GridLength.Auto;
                    rowAuto.Height = new GridLength(0);
                }
                else
                {
                    btnConnectOthAp.Visibility = Visibility.Visible;
                    btnConnectOthAp.IsEnabled = true;
                    autoConnect.Visibility = Visibility.Visible;
                    autoConnect.IsEnabled = true;
                    manualConnect.Visibility = Visibility.Hidden;
                    manualConnect.IsEnabled = false;
                    gdWifi.Visibility = Visibility.Hidden;
                    gdWifi.IsEnabled = false;
                    rowManual.Height = new GridLength(0);
                    rowAuto.Height = GridLength.Auto;
                }
            }

            scrollview.ScrollToTop();
        }

        private void OnchkWifiUnchecked(object sender, RoutedEventArgs e)
        {
            btnConnectOthAp.Visibility = Visibility.Hidden;
            btnConnectOthAp.IsEnabled = false;
            autoConnect.Visibility = Visibility.Hidden;
            autoConnect.IsEnabled = false;
            manualConnect.Visibility = Visibility.Hidden;
            manualConnect.IsEnabled = false;
            gdWifi.Visibility = Visibility.Hidden;
            gdWifi.IsEnabled = false;
            rowManual.Height = new GridLength(0);
            rowAuto.Height = new GridLength(0);
            wifilist.Children.Clear();

            scrollview.ScrollToTop();
        }

        private void OnClickWifiCheckBox(object sender, RoutedEventArgs e)
        {
            byte wifiEnable = 0;
            bool bSuccess = false;
            Nullable<bool> bEnable = chkWifi.IsChecked;
            if (true == bEnable)
                wifiEnable = 1;
            
            byte wifiInit = 0;
            // Begin: Fix bug # 61388
            int result = dll.GetWifiChangeStatus("", ref wifiInit);         
            if (0 == result) // success?
            {
                VOP.MainWindow_Rufous.m_byWifiInitStatus = wifiInit;
            }
            else
            {
                Win32.OutputDebugString("OnClickWifiCheckBox->dll.GetWifiChangeStatus() ---> failed...  ， Result =    " + result.ToString());
            }
            // End: Fix bug # 61388
            
            WiFiInfoRecord m_rec = new WiFiInfoRecord("", wifiEnable, 1, "", "", EnumEncryptType.WPA2_PSK_AES, 0);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<WiFiInfoRecord>("", ref m_rec, DllMethodType.SetWiFiInfo, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    bSuccess = true;
                }
            }
           
            if (!bSuccess)
            {
                bEnable = !chkWifi.IsChecked;
                chkWifi.IsChecked = bEnable;

                if (((MainWindow_Rufous)App.Current.MainWindow).m_strPassword.Length == 0)
                {
                    if (VOP.MainWindow_Rufous.m_byWifiInitStatus == 0x01 && true == bEnable)
                    {
                        cbo_ssid_refresh();
                    }
                }
            }
            else
            {
                if (true == bEnable)
                {
                    cbo_ssid_refresh();
                }
            }

            if (bSuccess)
            {
                if (wifiEnable != VOP.MainWindow_Rufous.m_byWifiInitStatus)
                {
                    MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                                    Application.Current.MainWindow,
                                    (string)this.TryFindResource("ResStr_Msg_1"),
                                    (string)this.TryFindResource("ResStr_Prompt"));
                }
                else
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                      Application.Current.MainWindow,
                     (string)this.FindResource("ResStr_Setting_Successfully_"),
                     (string)this.TryFindResource("ResStr_Prompt"));
            }
            else
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                     Application.Current.MainWindow,
                    (string)this.FindResource("ResStr_Setting_Fail"),
                    (string)this.TryFindResource("ResStr_Prompt"));
        }

        public void PassStatus(bool online)
        {
            m_currentStatus = online;
            this.IsEnabled = online;
            // autoConnect.IsEnabled = (false == common.IsOffline(m_currentStatus)); 
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (true == (bool)e.NewValue) 
                imgLine.Source = new BitmapImage(new Uri("pack://application:,,, /Images/GreenLine.png", UriKind.RelativeOrAbsolute));
                else
                    imgLine.Source = new BitmapImage(new Uri("pack://application:,,, /Images/Line.png", UriKind.RelativeOrAbsolute));
            }
            catch
            {

            }
        }

        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }
    }
}
