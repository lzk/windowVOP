using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging;
using System.Collections.Generic; // for BitmapImage

namespace VOP
{
    public partial class SettingPage : UserControl
    {
        SettingButton btnwifi = new SettingButton(IconType.Wireless);
        SettingButton btnSoftAp = new SettingButton(IconType.SoftAP);
        SettingButton btnTCPIP = new SettingButton(IconType.TCPIP);
        SettingButton btnPowerSave = new SettingButton(IconType.PowerSave);
        SettingButton btnUserConfig = new SettingButton(IconType.UserConfig);
        SettingButton btnPwd = new SettingButton(IconType.Password);

        List<SettingButton> m_listSettingButton = new List<SettingButton>();

        WifiView    wifiView = new WifiView();
        SoftapView  softapView = new SoftapView();
        TcpipView   tcpipView = new TcpipView();
        PowerSaveView powersaveView = new PowerSaveView();
        UserConfigView userconfigView = new UserConfigView();
        PasswordView passwordView = new PasswordView();

        public SettingPage()
        {
            InitializeComponent();

            m_listSettingButton.Clear();

            int tabbtn_width = 165;
            int tabbtn_height = 30;

            btnwifi.btn.Content = (string)this.FindResource("ResStr_Printer_Wi_Fi");
            btnwifi.Margin = new Thickness(0, 1, 0, 9);
            btnwifi.Width = tabbtn_width;
            btnwifi.Height = tabbtn_height;
            btnwifi.HorizontalAlignment = HorizontalAlignment.Left;
            btnwifi.btn.Name = "btnwifi";
            btnwifi.btn.Click += SettingBtnClick;           
            m_listSettingButton.Add(btnwifi);

            btnSoftAp.btn.Content = (string)this.FindResource("ResStr_Soft_AP");
            btnSoftAp.Margin = new Thickness(0, 1, 0, 9);
            btnSoftAp.Width = tabbtn_width;
            btnSoftAp.Height = tabbtn_height;
            btnSoftAp.HorizontalAlignment = HorizontalAlignment.Left;
            btnSoftAp.btn.Name = "btnSoftAp";
            btnSoftAp.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnSoftAp);

            btnTCPIP.btn.Content = (string)this.FindResource("ResStr_TCP_IP");
            btnTCPIP.Margin = new Thickness(0, 1, 0, 9);
            btnTCPIP.Width = tabbtn_width;
            btnTCPIP.Height = tabbtn_height;
            btnTCPIP.HorizontalAlignment = HorizontalAlignment.Left;
            btnTCPIP.btn.Name = "btnTCPIP";
            btnTCPIP.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnTCPIP);

            btnPowerSave.btn.Content = (string)this.FindResource("ResStr_Power_Save");
            btnPowerSave.Margin = new Thickness(0, 1, 0, 9);
            btnPowerSave.Width = tabbtn_width;
            btnPowerSave.Height = tabbtn_height;
            btnPowerSave.HorizontalAlignment = HorizontalAlignment.Left;
            btnPowerSave.btn.Name = "btnPowerSave";
            btnPowerSave.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnPowerSave);

            btnUserConfig.btn.Content = (string)this.FindResource("ResStr_User_Config");
            btnUserConfig.Margin = new Thickness(0, 1, 0, 9);
            btnUserConfig.Width = tabbtn_width;
            btnUserConfig.Height = tabbtn_height;
            btnUserConfig.HorizontalAlignment = HorizontalAlignment.Left;
            btnUserConfig.btn.Name = "btnUserConfig";
            btnUserConfig.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnUserConfig);

            btnPwd.btn.Content = (string)this.FindResource("ResStr_Modify_Password");
            btnPwd.Margin = new Thickness(0, 1, 0, 9);
            btnPwd.Width = tabbtn_width;
            btnPwd.Height = tabbtn_height;
            btnPwd.HorizontalAlignment = HorizontalAlignment.Left;
            btnPwd.btn.Name = "btnPassword";
            btnPwd.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnPwd);
        }
        
        private void SetActiveButton(IconType iconType)
        {
            foreach(SettingButton btn in m_listSettingButton)
            {
                if(btn.m_nIconType == iconType)
                {
                    btn.btn.IsActiveEx = true;
                }
                else
                {
                    btn.btn.IsActiveEx = false;
                }
            }
        }

        private void ClickSettingButton(IconType iconType)
        {
            foreach (SettingButton btn in m_listSettingButton)
            {
                if (btn.m_nIconType == iconType)
                {
                    RoutedEventArgs argsEvent = new RoutedEventArgs();
                    argsEvent.RoutedEvent = Button.ClickEvent;
                    argsEvent.Source = this;
                    btn.btn.RaiseEvent(argsEvent);
                    break;
                }
            }
        }

        public void InitWindowLayout()
        {
            setting_tab_btn.Children.Clear();
            Grid.SetColumnSpan(setting_tab_btn, 3);
            ((MainWindow)App.Current.MainWindow).m_strPassword = "";

            if (m_MainWin.statusPanelPage.m_isWiFiModel)
            {
                setting_tab_btn.Children.Add(btnwifi);
                setting_tab_btn.Children.Add(btnSoftAp);
                setting_tab_btn.Children.Add(btnTCPIP);
            }

            setting_tab_btn.Children.Add(btnPowerSave);
            setting_tab_btn.Children.Add(btnUserConfig);
            setting_tab_btn.Children.Add(btnPwd);

            if (m_MainWin.statusPanelPage.m_isWiFiModel)
            {
                ClickSettingButton(IconType.Wireless);
            }
            else
            {
                ClickSettingButton(IconType.PowerSave);
            }
        }

        public void handler_loaded_settingpage( object sender, RoutedEventArgs e )
        {
            InitWindowLayout();
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx srcButton = e.Source as VOP.Controls.ButtonEx;
            if ( "btnwifi" == srcButton.Name )
            {
                srcButton.IsActiveEx = true;
                SetActiveButton(IconType.Wireless);
                this.settingView.Child = wifiView;
            }
            else if ("btnSoftAp" == srcButton.Name)
            {
                SetActiveButton(IconType.SoftAP);
                this.settingView.Child = softapView;
            }
            else if ("btnTCPIP" == srcButton.Name)
            {
                SetActiveButton(IconType.TCPIP);
                this.settingView.Child = tcpipView;
            }
            else if ("btnPowerSave" == srcButton.Name)
            {
                SetActiveButton(IconType.PowerSave);
                this.settingView.Child = powersaveView;
            }
            else if ("btnUserConfig" == srcButton.Name)
            {
                SetActiveButton(IconType.UserConfig);
                this.settingView.Child = userconfigView;
            }
            else if ("btnPassword" == srcButton.Name)
            {
                SetActiveButton(IconType.Password);
                this.settingView.Child = passwordView;
            }
        }

        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        private MainWindow _MainWin = null;

        public MainWindow m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if ( null == _MainWin )
                {
                    return ( MainWindow )App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

        public void PassStatus(EnumStatus st, EnumMachineJob job, byte toner)
        {
            if (null != m_MainWin )
            {
                if (m_MainWin.statusPanelPage.m_isWiFiModel)
                {
                    wifiView.PassStatus(st, job, toner);
                    softapView.PassStatus(st, job, toner);
                    tcpipView.PassStatus(st, job, toner);
                }
                powersaveView.PassStatus(st, job, toner);
                userconfigView.PassStatus(st, job, toner);
                passwordView.PassStatus(st, job, toner);
            }
        }
    }
}
