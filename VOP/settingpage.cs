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
        SettingButton btnAbout = new SettingButton(IconType.About);

        List<SettingButton> m_listSettingButton = new List<SettingButton>();
        public SettingPage()
        {
            InitializeComponent();
            m_listSettingButton.Clear();

            int tabbtn_width = 188;
            int tabbtn_height = 27;

            btnwifi.btn.Content = "Wi-Fi";
            btnwifi.Margin = new Thickness(0, 10, 0, 8);
            btnwifi.Width = tabbtn_width;
            btnwifi.Height = tabbtn_height;
            btnwifi.HorizontalAlignment = HorizontalAlignment.Left;
            btnwifi.btn.Name = "btnwifi";
            btnwifi.btn.Click += SettingBtnClick;           
            m_listSettingButton.Add(btnwifi);

            btnSoftAp.btn.Content = "SoftAP";
            btnSoftAp.Margin = new Thickness(0, 0, 0, 8);
            btnSoftAp.Width = tabbtn_width;
            btnSoftAp.Height = tabbtn_height;
            btnSoftAp.HorizontalAlignment = HorizontalAlignment.Left;
            btnSoftAp.btn.Name = "btnSoftAp";
            btnSoftAp.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnSoftAp);

            btnTCPIP.btn.Content = "TCPIP";
            btnTCPIP.Margin = new Thickness(0, 0, 0, 8);
            btnTCPIP.Width = tabbtn_width;
            btnTCPIP.Height = tabbtn_height;
            btnTCPIP.HorizontalAlignment = HorizontalAlignment.Left;
            btnTCPIP.btn.Name = "btnTCPIP";
            btnTCPIP.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnTCPIP);

            btnPowerSave.btn.Content = "Power Save";
            btnPowerSave.Margin = new Thickness(0, 0, 0, 8);
            btnPowerSave.Width = tabbtn_width;
            btnPowerSave.Height = tabbtn_height;
            btnPowerSave.HorizontalAlignment = HorizontalAlignment.Left;
            btnPowerSave.btn.Name = "btnPowerSave";
            btnPowerSave.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnPowerSave);

            btnUserConfig.btn.Content = "User Config";
            btnUserConfig.Margin = new Thickness(0, 0, 0, 8);
            btnUserConfig.Width = tabbtn_width;
            btnUserConfig.Height = tabbtn_height;
            btnUserConfig.HorizontalAlignment = HorizontalAlignment.Left;
            btnUserConfig.btn.Name = "btnUserConfig";
            btnUserConfig.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnUserConfig);

            btnPwd.btn.Content = "Password";
            btnPwd.Margin = new Thickness(0, 0, 0, 8);
            btnPwd.Width = tabbtn_width;
            btnPwd.Height = tabbtn_height;
            btnPwd.HorizontalAlignment = HorizontalAlignment.Left;
            btnPwd.btn.Name = "btnPassword";
            btnPwd.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnPwd);

            btnAbout.btn.Content = "About";
            btnAbout.Margin = new Thickness(0, 0, 0, 8);
            btnAbout.Width = tabbtn_width;
            btnAbout.Height = tabbtn_height;
            btnAbout.HorizontalAlignment = HorizontalAlignment.Left;
            btnAbout.btn.Name = "btnAbout";
            btnAbout.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnAbout);
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

        public void handler_loaded_settingpage( object sender, RoutedEventArgs e )
        {
            setting_tab_btn.Children.Clear();
            Grid.SetColumnSpan(setting_tab_btn, 2);

            string strPrinterDrvName = VOP.MainWindow.GetPrinterDrvName(VOP.MainWindow.selectedprinter);
            if (VOP.MainWindow.IsSupportWifi(strPrinterDrvName))
            {
                setting_tab_btn.Children.Add(btnwifi);
                setting_tab_btn.Children.Add(btnSoftAp);
                setting_tab_btn.Children.Add(btnTCPIP);
            }

            setting_tab_btn.Children.Add(btnPowerSave);
            setting_tab_btn.Children.Add(btnUserConfig);
            setting_tab_btn.Children.Add(btnPwd);
            setting_tab_btn.Children.Add(btnAbout);
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx srcButton = e.Source as VOP.Controls.ButtonEx;
            if ( "btnwifi" == srcButton.Name )
            {
                srcButton.IsActiveEx = true;
                SetActiveButton(IconType.Wireless);
            }
            else if ("btnSoftAp" == srcButton.Name)
            {
                SetActiveButton(IconType.SoftAP);
            }
            else if ("btnTCPIP" == srcButton.Name)
            {
                SetActiveButton(IconType.TCPIP);
            }
            else if ("btnPowerSave" == srcButton.Name)
            {
                SetActiveButton(IconType.PowerSave);
            }
            else if ("btnUserConfig" == srcButton.Name)
            {
                SetActiveButton(IconType.UserConfig);
            }
            else if ("btnPassword" == srcButton.Name)
            {
                SetActiveButton(IconType.Password);
            }
            else if ("btnAbout" == srcButton.Name)
            {
                SetActiveButton(IconType.About);
            }
        }

    }
}
