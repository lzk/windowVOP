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

namespace VOP
{
    /// <summary>
    /// Interaction logic for WifiView.xaml
    /// </summary>
    public partial class WifiView : UserControl
    {
 //       VOP.Controls.WifiSetting wifiSettingInit = new VOP.Controls.WifiSetting();
        VOP.Controls.WifiSetting wifiSetting = new VOP.Controls.WifiSetting();
        private bool m_bConnectOthApMode = false;
       
        public WifiView()
        {
            InitializeComponent();           
        }

        private void OnLoadWifiView(object sender, RoutedEventArgs e)
        {
            WiFiInfoRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            worker.InvokeMethod<WiFiInfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.GetWiFiInfo);
           

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                wifiSetting.wifiEnable = m_rec.WifiEnable;
                wifiSetting.m_ssid = m_rec.SSID;
                wifiSetting.m_pwd = m_rec.PWD;
                wifiSetting.m_encryption = (byte)m_rec.Encryption;
                wifiSetting.m_wepKeyId = m_rec.WepKeyId;
            }

            if (0x01 == (m_rec.WifiEnable & 0x01))
            {
                chkWifi.IsChecked = true;
                cbo_ssid_refresh();
            }
            else
            {
                chkWifi.IsChecked = false;
            }
            scrollview.ScrollToTop();
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
                manualConnect.Visibility = Visibility.Visible;
                rowManual.Height = GridLength.Auto;
                autoConnect.Visibility = Visibility.Hidden;
                rowAuto.Height = new GridLength(0);
                scrollview.ScrollToTop();
                wepKey0.IsChecked = true;
                m_bConnectOthApMode = true;
            }
            else if (btn.Name == "btnCancel")
            {
                btnConnectOthAp.Visibility = Visibility.Visible;
                manualConnect.Visibility = Visibility.Hidden;
                rowManual.Height = new GridLength(0); 
                autoConnect.Visibility = Visibility.Visible;
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

                WiFiInfoRecord m_rec = new WiFiInfoRecord(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, wifiEnable, ssid, (encryption != (byte)EnumEncryptType.NoSecurity) ? pwd : "", (EnumEncryptType)encryption, wepKeyId);
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeMethod<WiFiInfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.SetWiFiInfo))
                {
                    if (m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        wifiSetting.wifiEnable = m_rec.WifiEnable;
                        wifiSetting.m_ssid = ssid;
                        wifiSetting.m_pwd = pwd;
                        wifiSetting.m_encryption = encryption;
                        wifiSetting.m_wepKeyId = wepKeyId;
                        isApplySuccess = true;
                    }

                }
            }

            //if (isApplySuccess && encryption == (byte)EnumEncryptType.NoSecurity)
            //    tb_pwd.Text = pwd;
            
            ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage(isApplySuccess ? "设置成功" : "设置失败");

            return isApplySuccess;
        }


        public void cbo_ssid_refresh(bool _bDisplayProgressBar = true)
        {
            wifilist.Children.Clear();

            ApListRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<ApListRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.GetApList);
            }
            else
            {
                m_rec = worker.GetApList(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter);
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                for (int i = 0; i < m_rec.SsidList.Count; i++)
                {
                    if (!String.IsNullOrEmpty(m_rec.SsidList[i]))
                    {

                        VOP.Controls.WiFiItem wifiitem = new VOP.Controls.WiFiItem();
                        wifiitem.SSIDText = m_rec.SsidList[i];
                        if ((byte)EnumEncryptType.NoSecurity == m_rec.EncryptionList[i])    //No Security
                        {
                            wifiitem.EncryptionText = "网络未加密";
                            wifiitem.EncryptType = VOP.EnumEncryptType.NoSecurity;
                        }
                        else if ((byte)EnumEncryptType.WEP == m_rec.EncryptionList[i]) //WEP
                        {
                            wifiitem.EncryptionText = "通过WEP进行保护";
                            wifiitem.EncryptType = VOP.EnumEncryptType.WEP;
                        }
                        else if ((byte)EnumEncryptType.WPA2_PSK_AES == m_rec.EncryptionList[i])   //3. WPA2-PSK-AES 
                        {
                            wifiitem.EncryptionText = "通过WPA2-PSK-AES进行保护";
                            wifiitem.EncryptType = VOP.EnumEncryptType.WPA2_PSK_AES;
                        }
                        else if ((byte)EnumEncryptType.MixedModePSK == m_rec.EncryptionList[i])   //4.Mixed Mode PSK
                        {
                            wifiitem.EncryptionText = "通过Mixed Mode PSK进行保护";
                            wifiitem.EncryptType = VOP.EnumEncryptType.MixedModePSK;
                        }
                        wifiitem.WifiSignalLevel = VOP.Controls.EnumWifiSignalLevel.stronger;

                        wifilist.Children.Add(wifiitem);
                    }
                }
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
                    wepKeyId = 0x00;
                }
                else if (true == wepKey1.IsChecked)
                {
                    wepKeyId = 0x01;
                }
                else if (true == wepKey2.IsChecked)
                {
                    wepKeyId = 0x02;
                }
                else if (true == wepKey3.IsChecked)
                {
                    wepKeyId = 0x03;
                }

                if (true == chkDisplayPwd.IsChecked)
                {
                    pwd = pbPwd.Password;
                }
                else
                {
                    pwd = tbPwd.Text;
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
                    wepGrid.Visibility = Visibility.Hidden;
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
                    wepGrid.Visibility = Visibility.Visible;
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
                    wepGrid.Visibility = Visibility.Hidden;
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
                    autoConnect.Visibility = Visibility.Hidden;
                    manualConnect.Visibility = Visibility.Visible;
                }
                else
                {
                    btnConnectOthAp.Visibility = Visibility.Visible;
                    autoConnect.Visibility = Visibility.Visible;
                    manualConnect.Visibility = Visibility.Hidden;
                }
            }
        }

        private void OnchkWifiUnchecked(object sender, RoutedEventArgs e)
        {
            btnConnectOthAp.Visibility = Visibility.Hidden;
            autoConnect.Visibility = Visibility.Hidden;
            manualConnect.Visibility = Visibility.Hidden;
        }

        private void OnClickWifiCheckBox(object sender, RoutedEventArgs e)
        {
            byte wifiEnable = 0;
            Nullable<bool> bEnable = chkWifi.IsChecked;
            if (true == bEnable)
                wifiEnable = 1;

            WiFiInfoRecord m_rec = new WiFiInfoRecord(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, wifiEnable, "", "", EnumEncryptType.WPA2_PSK_AES, 0);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            worker.InvokeMethod<WiFiInfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.SetWiFiInfo);

            if (true == bEnable)
            {
                cbo_ssid_refresh();
            }
        }
    }
}
