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
        public MainWindow_Rufous m_MainWin { get; set; }
        SettingButton_Rufous btnScanParameter = new SettingButton_Rufous(SettingType.ScanParameter);
        SettingButton_Rufous btnQuickScanSettings = new SettingButton_Rufous(SettingType.QuickScanSettings);
        SettingButton_Rufous btnQRCodeSettings = new SettingButton_Rufous(SettingType.QRCodeSettings);
        //SettingButton_Rufous btnScanToFile = new SettingButton_Rufous(SettingType.ScanToFile);
        //SettingButton_Rufous btnScanToPrint = new SettingButton_Rufous(SettingType.ScanToPrint);
        //SettingButton_Rufous btnScanToEmail = new SettingButton_Rufous(SettingType.ScanToEmail);
        //SettingButton_Rufous btnScanToFtp = new SettingButton_Rufous(SettingType.ScanToFtp);
        //SettingButton_Rufous btnScanToAP = new SettingButton_Rufous(SettingType.ScanToAP);
        //SettingButton_Rufous btnScanToCloud = new SettingButton_Rufous(SettingType.ScanToCloud);

        SettingButton_Rufous btnwifi = new SettingButton_Rufous(SettingType.Wireless);
        SettingButton_Rufous btnTCPIP = new SettingButton_Rufous(SettingType.TCPIP);
        SettingButton_Rufous btnSoftAP = new SettingButton_Rufous(SettingType.SoftAP);
        SettingButton_Rufous btnDevice = new SettingButton_Rufous(SettingType.Device);

        List<SettingButton_Rufous> m_listSettingButton = new List<SettingButton_Rufous>();

        ScanParameterView scanParameterView = new ScanParameterView();
        //ScanToCloudView scanToCloudView = new ScanToCloudView();
        //ScanToFtpView scanToFtpView = new ScanToFtpView();
        //ScanToPrintView scanToPrintView = new ScanToPrintView();
        //ScanToEmailView scanToEmailView = new ScanToEmailView();
        //ScanToFileView scanToFileView = new ScanToFileView();
        //ScanToAPView scanToAPView = new ScanToAPView();

        QRCodeBarcodeView qrcodebarcodeView = new QRCodeBarcodeView();
        WifiView_Rufous wifiView = new WifiView_Rufous();
        TcpipView_Rufous tcpipView = new TcpipView_Rufous();
        SoftapView_Rufous softAPView = new SoftapView_Rufous();
        DeviceView deviceView = new DeviceView();       

        QuickScanSettings quickScanSettings = new QuickScanSettings();

        public ScanSettingPage_Rufous()
        {
            InitializeComponent();

            m_listSettingButton.Clear();

            int tabbtn_width = 175;
            int tabbtn_height = 66;

//            btnScanParameter.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Common");
            btnScanParameter.btn.Content = "Scan To Settings";
            //btnScanParameter.Margin = new Thickness(0, 1, 0, 9);
            btnScanParameter.Width = tabbtn_width;
            btnScanParameter.Height = tabbtn_height;
            btnScanParameter.HorizontalAlignment = HorizontalAlignment.Left;
            btnScanParameter.btn.Name = "btnScanParameter";
            btnScanParameter.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnScanParameter);

            btnQuickScanSettings.btn.Content = "Quick Scan Settings";
           //btnQuickScanSettings.Margin = new Thickness(0, 1, 0, 9);
            btnQuickScanSettings.Width = tabbtn_width;
            btnQuickScanSettings.Height = tabbtn_height;
            btnQuickScanSettings.HorizontalAlignment = HorizontalAlignment.Left;
            btnQuickScanSettings.btn.Name = "btnQuickScanSettings";
            btnQuickScanSettings.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnQuickScanSettings);


            btnQRCodeSettings.btn.Content = "QRCode/Barcode";
           //btnQRCodeSettings.Margin = new Thickness(0, 1, 0, 9);
            btnQRCodeSettings.Width = tabbtn_width;
            btnQRCodeSettings.Height = tabbtn_height;
            btnQRCodeSettings.HorizontalAlignment = HorizontalAlignment.Left;
            btnQRCodeSettings.btn.Name = "btnQRCodeSettings";
            btnQRCodeSettings.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnQRCodeSettings);
            //btnScanToPrint.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Scan_Print");
            //btnScanToPrint.Margin = new Thickness(0, 1, 0, 9);
            //btnScanToPrint.Width = tabbtn_width;
            //btnScanToPrint.Height = tabbtn_height;
            //btnScanToPrint.HorizontalAlignment = HorizontalAlignment.Left;
            //btnScanToPrint.btn.Name = "btnScanToPrint";
            //btnScanToPrint.btn.Click += SettingBtnClick;
            //m_listSettingButton.Add(btnScanToPrint);

            //btnScanToFile.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Scan_File");
            //btnScanToFile.Margin = new Thickness(0, 1, 0, 9);
            //btnScanToFile.Width = tabbtn_width;
            //btnScanToFile.Height = tabbtn_height;
            //btnScanToFile.HorizontalAlignment = HorizontalAlignment.Left;
            //btnScanToFile.btn.Name = "btnScanToFile";
            //btnScanToFile.btn.Click += SettingBtnClick;
            //m_listSettingButton.Add(btnScanToFile);

            //btnScanToAP.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Scan_App");
            //btnScanToAP.Margin = new Thickness(0, 1, 0, 9);
            //btnScanToAP.Width = tabbtn_width;
            //btnScanToAP.Height = tabbtn_height;
            //btnScanToAP.HorizontalAlignment = HorizontalAlignment.Left;
            //btnScanToAP.btn.Name = "btnScanToAP";
            //btnScanToAP.btn.Click += SettingBtnClick;
            //m_listSettingButton.Add(btnScanToAP);

            //btnScanToEmail.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Scan_Email");
            //btnScanToEmail.Margin = new Thickness(0, 1, 0, 9);
            //btnScanToEmail.Width = tabbtn_width;
            //btnScanToEmail.Height = tabbtn_height;
            //btnScanToEmail.HorizontalAlignment = HorizontalAlignment.Left;
            //btnScanToEmail.btn.Name = "btnScanToEmail";
            //btnScanToEmail.btn.Click += SettingBtnClick;
            //m_listSettingButton.Add(btnScanToEmail);

            //btnScanToFtp.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Scan_FTP");
            //btnScanToFtp.Margin = new Thickness(0, 1, 0, 9);
            //btnScanToFtp.Width = tabbtn_width;
            //btnScanToFtp.Height = tabbtn_height;
            //btnScanToFtp.HorizontalAlignment = HorizontalAlignment.Left;
            //btnScanToFtp.btn.Name = "btnScanToFTP";
            //btnScanToFtp.btn.Click += SettingBtnClick;
            //m_listSettingButton.Add(btnScanToFtp);


            //btnScanToCloud.btn.Content = (string)this.TryFindResource("ResStr_Faroe_Scan_Cloud");
            //btnScanToCloud.Margin = new Thickness(0, 1, 0, 9);
            //btnScanToCloud.Width = tabbtn_width;
            //btnScanToCloud.Height = tabbtn_height;
            //btnScanToCloud.HorizontalAlignment = HorizontalAlignment.Left;
            //btnScanToCloud.btn.Name = "btnScanToCloud";
            //btnScanToCloud.btn.Click += SettingBtnClick;
            //m_listSettingButton.Add(btnScanToCloud);

            btnwifi.btn.Content = (string)this.TryFindResource("ResStr_Printer_Wi_Fi");
            //btnwifi.Margin = new Thickness(0, 1, 0, 9);
            btnwifi.Width = tabbtn_width;
            btnwifi.Height = tabbtn_height;
            btnwifi.HorizontalAlignment = HorizontalAlignment.Left;
            btnwifi.btn.Name = "btnwifi";
            btnwifi.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnwifi);

            btnTCPIP.btn.Content = (string)this.TryFindResource("ResStr_TCP_IPv4");
            //btnTCPIP.Margin = new Thickness(0, 1, 0, 9);
            btnTCPIP.Width = tabbtn_width;
            btnTCPIP.Height = tabbtn_height;
            btnTCPIP.HorizontalAlignment = HorizontalAlignment.Left;
            btnTCPIP.btn.Name = "btnTCPIP";
            btnTCPIP.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnTCPIP);

            btnSoftAP.btn.Content = (string)this.TryFindResource("ResStr_Soft_AP");
            //btnSoftAP.Margin = new Thickness(0, 1, 0, 9);
            btnSoftAP.Width = tabbtn_width;
            btnSoftAP.Height = 65;// tabbtn_height;
            btnSoftAP.HorizontalAlignment = HorizontalAlignment.Left;
            btnSoftAP.btn.Name = "btnSoftAP";
            btnSoftAP.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnSoftAP);

            btnDevice.btn.Content = (string)this.TryFindResource("ResStr_Device");
            //btnDevice.Margin = new Thickness(0, 1, 0, 9);
            btnDevice.Width = tabbtn_width;
            btnDevice.Height = 64;// tabbtn_height;
            btnDevice.HorizontalAlignment = HorizontalAlignment.Left;
            btnDevice.btn.Name = "btnDevice";
            btnDevice.btn.PreviewMouseLeftButtonDown += SettingBtnClick;
            m_listSettingButton.Add(btnDevice);

        }
        
        private void SetActiveButton(SettingType settingType)
        {
            foreach(SettingButton_Rufous btn in m_listSettingButton)
            {
                if(btn.m_settingType == settingType)
                {
                    btn.btn.IsActiveEx = true;
                }
                else
                {
                    btn.btn.IsActiveEx = false;
                }
            }
        }

        private void ClickSettingButton(SettingType settingType)
        {
            //foreach (SettingButton_Rufous btn in m_listSettingButton)
            //{
            //    if (btn.m_settingType == settingType)
            //    {
            //        RoutedEventArgs argsEvent = new RoutedEventArgs();
            //        argsEvent.RoutedEvent = Button.ClickEvent;
            //        argsEvent.Source = this;
            //        btn.btn.RaiseEvent(argsEvent);
            //        break;
            //    }
            //}

            if (settingType == SettingType.ScanParameter)
            {
                SetActiveButton(SettingType.ScanParameter);
                scanParameterView.m_MainWin = this.m_MainWin;
                this.settingView.Child = scanParameterView;
            }
            if (settingType == SettingType.QuickScanSettings)
            {
                SetActiveButton(SettingType.QuickScanSettings);
                quickScanSettings.m_MainWin = this.m_MainWin;
                this.settingView.Child = quickScanSettings;
            }
            if (settingType == SettingType.QRCodeSettings)
            {
                SetActiveButton(SettingType.QRCodeSettings);
                qrcodebarcodeView.m_MainWin = this.m_MainWin;
                this.settingView.Child = qrcodebarcodeView;
            }
            else if (settingType == SettingType.Wireless)
            {
                SetActiveButton(SettingType.Wireless);
                wifiView.m_MainWin = this.m_MainWin;
                this.settingView.Child = wifiView;
            }
            else if (settingType == SettingType.TCPIP)
            {
                SetActiveButton(SettingType.TCPIP);
                tcpipView.m_MainWin = this.m_MainWin;
                this.settingView.Child = tcpipView;
            }
            else if (settingType == SettingType.SoftAP)
            {
                SetActiveButton(SettingType.SoftAP);
                softAPView.m_MainWin = this.m_MainWin;
                this.settingView.Child = softAPView;
            }
            else if (settingType == SettingType.Device)
            {
                SetActiveButton(SettingType.Device);
                deviceView.m_MainWin = this.m_MainWin;
                this.settingView.Child = deviceView;
            }
        }

        public void InitWindowLayout()
        {
            setting_tab_btn.Children.Clear();
            setting_tab_btn.Children.Add(btnScanParameter);
            setting_tab_btn.Children.Add(btnQuickScanSettings);
            setting_tab_btn.Children.Add(btnQRCodeSettings);
            //setting_tab_btn.Children.Add(btnScanToPrint);
            //setting_tab_btn.Children.Add(btnScanToFile);
            //setting_tab_btn.Children.Add(btnScanToAP);
            //setting_tab_btn.Children.Add(btnScanToEmail);
            //setting_tab_btn.Children.Add(btnScanToFtp);
            //setting_tab_btn.Children.Add(btnScanToCloud);
            setting_tab_btn.Children.Add(btnwifi);
            setting_tab_btn.Children.Add(btnTCPIP);
            setting_tab_btn.Children.Add(btnSoftAP);
            setting_tab_btn.Children.Add(btnDevice);         

            ClickSettingButton(SettingType.ScanParameter);
          
        }

        public void handler_loaded_settingpage( object sender, RoutedEventArgs e )
        {
            InitWindowLayout();
        }

        private void SettingBtnClick(object sender, System.Windows.Input.MouseButtonEventArgs e)//RoutedEventArgs e)
        {
            VOP.Controls.ButtonEx2 srcButton = e.Source as VOP.Controls.ButtonEx2;

            if ("btnScanParameter" == srcButton.Name)
            {
                SetActiveButton(SettingType.ScanParameter);
                scanParameterView.m_MainWin = this.m_MainWin;
                this.settingView.Child = scanParameterView;
            }
            if ("btnQuickScanSettings" == srcButton.Name)
            {
                SetActiveButton(SettingType.QuickScanSettings);
                quickScanSettings.m_MainWin = this.m_MainWin;
                this.settingView.Child =quickScanSettings ;
            }
            if ("btnQRCodeSettings" == srcButton.Name)
            {
                SetActiveButton(SettingType.QRCodeSettings);
                qrcodebarcodeView.m_MainWin = this.m_MainWin;
                this.settingView.Child = qrcodebarcodeView;
            }
            //else if ("btnScanToFile" == srcButton.Name)
            //{
            //    SetActiveButton(SettingType.ScanToFile);
            //    scanToFileView.m_MainWin = this.m_MainWin;
            //    this.settingView.Child = scanToFileView;
            //}
            //else if ("btnScanToPrint" == srcButton.Name )
            //{
            //    SetActiveButton(SettingType.ScanToPrint);
            //    scanToPrintView.m_MainWin = this.m_MainWin;
            //    this.settingView.Child = scanToPrintView;
            //}
            //else if ("btnScanToEmail" == srcButton.Name)
            //{
            //    SetActiveButton(SettingType.ScanToEmail);
            //    scanToEmailView.m_MainWin = this.m_MainWin;
            //    this.settingView.Child = scanToEmailView;
            //}
            //else if ("btnScanToFTP" == srcButton.Name)
            //{
            //    SetActiveButton(SettingType.ScanToFtp);
            //    scanToFtpView.m_MainWin = this.m_MainWin;
            //    this.settingView.Child = scanToFtpView;
            //}
            //else if ("btnScanToAP" == srcButton.Name)
            //{
            //    SetActiveButton(SettingType.ScanToAP);
            //    scanToAPView.m_MainWin = this.m_MainWin;
            //    this.settingView.Child = scanToAPView;
            //}
            //else if ("btnScanToCloud" == srcButton.Name)
            //{
            //    SetActiveButton(SettingType.ScanToCloud);
            //    scanToCloudView.m_MainWin = this.m_MainWin;
            //    this.settingView.Child = scanToCloudView;
            //}
            else if ("btnwifi" == srcButton.Name)
            {
                SetActiveButton(SettingType.Wireless);
                wifiView.m_MainWin = this.m_MainWin;
                this.settingView.Child = wifiView;
            }
            else if ("btnTCPIP" == srcButton.Name)
            {
                SetActiveButton(SettingType.TCPIP);
                tcpipView.m_MainWin = this.m_MainWin;
                this.settingView.Child = tcpipView;
            }
            else if ("btnSoftAP" == srcButton.Name)
            {
                SetActiveButton(SettingType.SoftAP);
                softAPView.m_MainWin = this.m_MainWin;
                this.settingView.Child = softAPView;
            }
            else if ("btnDevice" == srcButton.Name)
            {
                SetActiveButton(SettingType.Device);
                deviceView.m_MainWin = this.m_MainWin;
                this.settingView.Child = deviceView;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

             m_MainWin.GotoPage("ScanSelectionPage", null);

        }

        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        //private MainWindow_Rufous _MainWin = null;

        //public MainWindow_Rufous m_MainWin
        //{
        //    set
        //    {
        //        _MainWin = value;
        //    }

        //    get
        //    {
        //        if ( null == _MainWin )
        //        {
        //            return (MainWindow_Rufous)App.Current.MainWindow;
        //        }
        //        else
        //        {
        //            return _MainWin;
        //        }
        //    }
        //}

        public void PassStatus(bool online)
        {
            if (null != m_MainWin)
            {
                wifiView.PassStatus(online);
                tcpipView.PassStatus(online);
                softAPView.PassStatus(online);
                deviceView.PassStatus(online);
            }
        }
    }
}
