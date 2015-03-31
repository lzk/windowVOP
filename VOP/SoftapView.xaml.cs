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

namespace VOP
{
    /// <summary>
    /// Interaction logic for softap.xaml
    /// </summary>
    
    public class SoftAPSetting
    {
        public string m_ssid = "";
        public string m_pwd = "";
        public bool m_bEnableSoftAp = false;
    }

    public partial class SoftapView : UserControl
    {
        SoftAPSetting softAPSetting = new SoftAPSetting();
        SoftAPSetting softAPSettingInit = new SoftAPSetting();

        public SoftapView()
        {
            InitializeComponent();
        }

        private void OnLoadedSoftapView(object sender, RoutedEventArgs e)
        {
            EnableApplyButton();

        }

        public void HandleCheck(object sender, RoutedEventArgs e)
        {
            EnableApplyButton();
        }

        private void EnableApplyButton()
        {
            if(is_InputVailible())
            {
                if(  softAPSettingInit.m_ssid != softAPSetting.m_ssid
                    || softAPSettingInit.m_bEnableSoftAp != softAPSetting.m_bEnableSoftAp
                    || softAPSettingInit.m_pwd != softAPSetting.m_pwd)
                {
                    btnApply.IsEnabled = true;
                }
                else
                {
                    btnApply.IsEnabled = false;
                }
            }
            else
            {
                btnApply.IsEnabled = false;
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            bool isApplySuccess = false;

            // soft ap config
            string str_ssid = tbSSID.Text;
            string str_pwd = pbPWD.Password;
            bool isEnableSoftAp = (chkbtn_wifi_enable.IsChecked == true);

            if (is_InputVailible())
            {
                SoftApRecord m_rec = new SoftApRecord(VOP.MainWindow.selectedprinter, str_ssid, str_pwd, isEnableSoftAp);
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeMethod<SoftApRecord>(VOP.MainWindow.selectedprinter, ref m_rec, DllMethodType.SetSoftAp))
                {
                    if (m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        softAPSettingInit.m_ssid = softAPSetting.m_ssid = str_ssid;
                        softAPSettingInit.m_pwd = softAPSetting.m_pwd = str_pwd;
                        softAPSettingInit.m_bEnableSoftAp = softAPSetting.m_bEnableSoftAp = isEnableSoftAp;
                        isApplySuccess = true;
                    }
                }

                //if (null != event_config_dirty)
                //    event_config_dirty(is_dirty());
            }
            else
            {
                if (str_ssid.Length <= 0)
                {
                    MessageBox.Show("The SSID input error! The SSID must be 1-32 characters.");
                    return;
                }

                if (str_pwd.Length < 8 || str_pwd.Length >= 64)
                {
                    MessageBox.Show("The passphrase input error! The least length will be 8 words and the max length will be 63 in ASCII or 64 in HEX.");
                    return;
                }
            }

            EnableApplyButton();
        }

        public bool is_InputVailible()
        {
            bool bValidatePassWord = true;

            string str_ssid = tbSSID.Text;
            string str_pwd = pbPWD.Password;
            int nCharCount = str_pwd.Length;

            if (nCharCount >= 8 && nCharCount <= 63)
            {
                foreach (char ch in str_pwd)
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
                foreach (char ch in str_pwd)
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

            return ((str_ssid.Length > 0 && str_ssid.Length <= 32) && bValidatePassWord);
        }

        public void handler_password_changed(object sender, RoutedEventArgs e)
        {
            PasswordBox pwd = sender as PasswordBox;
            string strText = pwd.Password;

            bool bValidate = true;
            foreach (char ch in strText)
            {
                if (((UInt16)ch) > 128)
                {
                    bValidate = false;
                    break;
                }
            }

            if (!bValidate)
            {
                pwd.Password = softAPSetting.m_pwd;
            }

            softAPSetting.m_pwd = pwd.Password;

            EnableApplyButton();
        }

        private void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string strText = txtBox.Text;
            int nCaretIndex = txtBox.CaretIndex;
            if (!common.IsVailibleSSID(strText))
            {
                strText = txtBox.Text = softAPSetting.m_ssid;
                tbSSID.CaretIndex = nCaretIndex;
            }

            softAPSetting.m_ssid = strText;
            EnableApplyButton();
        }
    }
}
