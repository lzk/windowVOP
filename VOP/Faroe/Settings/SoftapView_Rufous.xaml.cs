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

namespace VOP
{
    /// <summary>
    /// Interaction logic for softap.xaml
    /// </summary>

    public partial class SoftapView_Rufous : UserControl
    {
        SoftAPSetting softAPSetting = new SoftAPSetting();
        SoftAPSetting softAPSettingInit = new SoftAPSetting();
        private EnumStatus m_currentStatus = EnumStatus.Ready;

        public SoftapView_Rufous()
        {
            InitializeComponent();
        }

        private void OnLoadedSoftapView(object sender, RoutedEventArgs e)
        {
            //UpdateApplyBtnStatus();

            init_config();
        }

        public void init_config(bool _bDisplayProgressBar = true)
        {
            softAPSettingInit.m_bEnableSoftAp = false; // disable
            softAPSettingInit.m_ssid = "";
            softAPSettingInit.m_pwd = "";
            
            SoftApRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<SoftApRecord>("", ref m_rec, DllMethodType.GetSoftAp, this);
            }
            else
            {
                m_rec = worker.GetSoftAp("");
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                softAPSetting.m_ssid = softAPSettingInit.m_ssid = m_rec.SSID;
                softAPSetting.m_pwd = softAPSettingInit.m_pwd = m_rec.PWD;
                softAPSetting.m_bEnableSoftAp = softAPSettingInit.m_bEnableSoftAp = m_rec.WifiEnable;
                VOP.MainWindow.m_byWifiInitStatus = 0x01;
            }            

            tbSSID.Text = softAPSettingInit.m_ssid;
            tbPWD.Text = softAPSettingInit.m_pwd;
            chkbtn_wifi_enable.IsChecked = softAPSettingInit.m_bEnableSoftAp;
        }


        public void HandleCheck(object sender, RoutedEventArgs e)
        {
            //UpdateApplyBtnStatus();
        }

        private void UpdateApplyBtnStatus()
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

        public bool apply()
        {
            bool isApplySuccess = false;

            // soft ap config
            string str_ssid = tbSSID.Text;
            string str_pwd = tbPWD.Text;
            bool isEnableSoftAp = (chkbtn_wifi_enable.IsChecked == true);

            if (is_InputVailible())
            {
                SoftApRecord m_rec = new SoftApRecord("", str_ssid, str_pwd, isEnableSoftAp);
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeMethod<SoftApRecord>("", ref m_rec, DllMethodType.SetSoftAp, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
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
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_Msg_10"), (string)this.FindResource("ResStr_Warning"));
                }
                else if (str_pwd.Length < 8 || str_pwd.Length >= 64)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_Msg_3"), (string)this.FindResource("ResStr_Warning"));
                }
            }

            if (isApplySuccess)
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                    Application.Current.MainWindow,
                   (string)this.FindResource("ResStr_Msg_1"),
                   "Prompt");
            else
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                    Application.Current.MainWindow,
                   (string)this.FindResource("ResStr_Setting_Fail"),
                   "Prompt");

            return isApplySuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            apply();
            //UpdateApplyBtnStatus();
        }

        public bool is_InputVailible()
        {
            bool bValidatePassWord = true;

            string str_ssid = tbSSID.Text;
            string str_pwd = tbPWD.Text;
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
            TextBox pwd = sender as TextBox;
            softAPSetting.m_pwd = pwd.Text;

            //UpdateApplyBtnStatus();
        }

        private void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string strText = txtBox.Text;

            bool bValidate = common.IsAsciiLetter(strText);
            if (!bValidate)
            {
                txtBox.Text = softAPSetting.m_ssid;
            }
            
            softAPSetting.m_ssid = txtBox.Text;
            //UpdateApplyBtnStatus();
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
        public void PassStatus(bool online)
        {
            btnApply.IsEnabled = online;
        }
    }
}
