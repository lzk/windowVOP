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
    public partial class ScanSettingPage_Rufous : UserControl
    {
        SettingButton btnScanParameter = new SettingButton(IconType.ScanParameter);
        SettingButton btnScanToFile = new SettingButton(IconType.ScanToFile);
        SettingButton btnScanToPrint = new SettingButton(IconType.Unknown);
        SettingButton btnScanToEmail = new SettingButton(IconType.Unknown);
        SettingButton btnScanToFtp = new SettingButton(IconType.Unknown);
        SettingButton btnScanToAP = new SettingButton(IconType.Unknown);
        SettingButton btnScanToCloud = new SettingButton(IconType.Unknown);
        SettingButton btnScanToA3 = new SettingButton(IconType.Unknown);
        SettingButton btnScanToQRCode = new SettingButton(IconType.Unknown);

        List<SettingButton> m_listSettingButton = new List<SettingButton>();

        ScanParameterView scanParameterView = new ScanParameterView();
        ScanToFileView scanToFileView = new ScanToFileView();


        public ScanSettingPage_Rufous()
        {
            InitializeComponent();

            m_listSettingButton.Clear();

            int tabbtn_width = 175;
            int tabbtn_height = 35;

            btnScanParameter.btn.Content = "Scan";
            btnScanParameter.Margin = new Thickness(0, 1, 0, 9);
            btnScanParameter.Width = tabbtn_width;
            btnScanParameter.Height = tabbtn_height;
            btnScanParameter.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanParameter.btn.Name = "btnScanParameter";
            btnScanParameter.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanParameter);

            btnScanToFile.btn.Content = "Scan To File";
            btnScanToFile.Margin = new Thickness(0, 1, 0, 9);
            btnScanToFile.Width = tabbtn_width;
            btnScanToFile.Height = tabbtn_height;
            btnScanToFile.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToFile.btn.Name = "btnScanToFile";
            btnScanToFile.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToFile);

            btnScanToPrint.btn.Content = "Scan To Print";
            btnScanToPrint.Margin = new Thickness(0, 1, 0, 9);
            btnScanToPrint.Width = tabbtn_width;
            btnScanToPrint.Height = tabbtn_height;
            btnScanToPrint.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToPrint.btn.Name = "btnScanToPrint";
            btnScanToPrint.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToPrint);

            btnScanToEmail.btn.Content = "Scan To Email";
            btnScanToEmail.Margin = new Thickness(0, 1, 0, 9);
            btnScanToEmail.Width = tabbtn_width;
            btnScanToEmail.Height = tabbtn_height;
            btnScanToEmail.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToEmail.btn.Name = "btnScanToEmail";
            btnScanToEmail.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToEmail);

            btnScanToFtp.btn.Content = "Scan To FTP";
            btnScanToFtp.Margin = new Thickness(0, 1, 0, 9);
            btnScanToFtp.Width = tabbtn_width;
            btnScanToFtp.Height = tabbtn_height;
            btnScanToFtp.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToFtp.btn.Name = "btnScanToFTP";
            btnScanToFtp.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToFtp);

            btnScanToAP.btn.Content = "Scan To Application";
            btnScanToAP.Margin = new Thickness(0, 1, 0, 9);
            btnScanToAP.Width = tabbtn_width;
            btnScanToAP.Height = tabbtn_height;
            btnScanToAP.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToAP.btn.Name = "btnScanToAP";
            btnScanToAP.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToAP);

            btnScanToCloud.btn.Content = "Scan To Cloud";
            btnScanToCloud.Margin = new Thickness(0, 1, 0, 9);
            btnScanToCloud.Width = tabbtn_width;
            btnScanToCloud.Height = tabbtn_height;
            btnScanToCloud.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToCloud.btn.Name = "btnScanToCloud";
            btnScanToCloud.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToCloud);

            btnScanToA3.btn.Content = "A3 Scan";
            btnScanToA3.Margin = new Thickness(0, 1, 0, 9);
            btnScanToA3.Width = tabbtn_width;
            btnScanToA3.Height = tabbtn_height;
            btnScanToA3.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToA3.btn.Name = "btnScanToA3";
            btnScanToA3.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToA3);

            btnScanToQRCode.btn.Content = "QR Code Scan";
            btnScanToQRCode.Margin = new Thickness(0, 1, 0, 9);
            btnScanToQRCode.Width = tabbtn_width;
            btnScanToQRCode.Height = tabbtn_height;
            btnScanToQRCode.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanToQRCode.btn.Name = "btnScanToQRCode";
            btnScanToQRCode.btn.Click += SettingBtnClick;
            m_listSettingButton.Add(btnScanToQRCode);

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
            setting_tab_btn.Children.Add(btnScanParameter);
            setting_tab_btn.Children.Add(btnScanToFile);
          //  setting_tab_btn.Children.Add(btnScanToPrint);
            setting_tab_btn.Children.Add(btnScanToEmail);
          //  setting_tab_btn.Children.Add(btnScanToFtp);
            setting_tab_btn.Children.Add(btnScanToAP);
          //  setting_tab_btn.Children.Add(btnScanToCloud);
          //  setting_tab_btn.Children.Add(btnScanToA3);
          //  setting_tab_btn.Children.Add(btnScanToQRCode);

            ClickSettingButton(IconType.ScanParameter);
          
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
               // this.settingView.Child = wifiView;
            }
            else if ("btnSoftAp" == srcButton.Name)
            {
                SetActiveButton(IconType.SoftAP);
               // this.settingView.Child = softapView;
            }
            else if ("btnTCPIP" == srcButton.Name)
            {
                SetActiveButton(IconType.TCPIP);
               // this.settingView.Child = tcpipView;
            }
            else if ("btnTCPIPv6" == srcButton.Name)
            {
                SetActiveButton(IconType.TCPIPv6);
               // this.settingView.Child = ipv6View;
            }
            else if ("btnPowerSave" == srcButton.Name)
            {
                SetActiveButton(IconType.PowerSave);
                //this.settingView.Child = powersaveView;
            }
            else if ("btnScanParameter" == srcButton.Name)
            {
                SetActiveButton(IconType.ScanParameter);
                this.settingView.Child = scanParameterView;
            }
            else if ("btnScanToFile" == srcButton.Name)
            {
                SetActiveButton(IconType.ScanToFile);
                this.settingView.Child = scanToFileView;
            }
            else if ("btnPassword" == srcButton.Name)
            {
                SetActiveButton(IconType.Password);
               // this.settingView.Child = passwordView;
            }
        }

        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if ( null == _MainWin )
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
