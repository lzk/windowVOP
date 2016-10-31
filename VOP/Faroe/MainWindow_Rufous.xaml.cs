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
using System.Printing;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Xaml;
using System.Xml;
using Newtonsoft;
using System.IO;
using System.Globalization;
using System.Xml.Linq;
using VOP.Controls;
using System.Diagnostics;

namespace VOP
{

    public partial class MainWindow_Rufous : Window
    {
        
        public static SettingData g_settingData = new SettingData();
        public static List<string> g_printerList = new List<string>();

        public ScanSelectionPage_Rufous scanSelectionPage = new ScanSelectionPage_Rufous();
        public ScanSettingPage_Rufous scanSettingsPage = new ScanSettingPage_Rufous();
        public ScanDevicePage_Rufous scanDevicePage = new ScanDevicePage_Rufous();
        public ScanPage_Rufous scanPage = new ScanPage_Rufous();
        public PrintPage printPage = new PrintPage();

        private Thread thread_searchIP = null;

        public static byte m_byWifiInitStatus = 0;
        public string m_strPassword = "";

        public MainWindow_Rufous()
        {
            InitializeComponent();

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            g_settingData.InitSettingData();

            thread_searchIP = new Thread(InitIPList);
            thread_searchIP.Start();

            this.SourceInitialized += new EventHandler(win_SourceInitialized);  
        }

        public void LoadedMainWindow(object sender, RoutedEventArgs e)
        {
            g_settingData = SettingData.Deserialize(App.cfgFile);

            common.GetAllPrinters(g_printerList);

            MainPageView.Child = scanSelectionPage;
            scanSelectionPage.m_MainWin = this;
            AddMessageHook();
        }

        public bool PasswordCorrect(Window parent)
        {
            bool bCorrect = false;
            if (m_strPassword.Length > 0)
            {
                PasswordRecord m_rec = new PasswordRecord("", m_strPassword);
                AsyncWorker worker = new AsyncWorker(parent);

                if (worker.InvokeMethod<PasswordRecord>("", ref m_rec, DllMethodType.ConfirmPassword, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        bCorrect = true;
                    }
                }

            }
            return bCorrect;
        }

        public void InitIPList()
        {
            ScanDevicePage_Rufous.ListIP();

            StringBuilder usbName = new StringBuilder(50);
            if (dll.CheckUsbScan(usbName) == 1)
            {
                if (MainWindow_Rufous.g_settingData.m_DeviceName == usbName.ToString())
                {
                    dll.SetConnectionMode(usbName.ToString(), true);
                    SetDeviceButtonState(true);
                }
            }

            if (ScanDevicePage_Rufous.ipList != null)
            {
                foreach (string ip in ScanDevicePage_Rufous.ipList)
                {
                    if (MainWindow_Rufous.g_settingData.m_DeviceName == ip)
                    {
                        dll.SetConnectionMode(ip, false);
                        SetDeviceButtonState(true);
                    }
                }
            }
        }

        public void SetDeviceButtonState(bool isConnected)
        {
            if (scanSelectionPage.DeviceButton != null)
            {
                scanSelectionPage.DeviceButton.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action<bool>(
                (x) =>
                {
                    scanSelectionPage.DeviceButton.Connected = x;
                }
                ), isConnected);
            }
        }

        public void GotoPage(string pageName, object arg)
        {
            if(pageName == "ScanPage")
            {
                MainPageView.Child = scanPage;
                scanPage.m_MainWin = this;
                scanPage.ScanFileList = (List<ScanFiles>)arg;
            }
            else if(pageName == "ScanSelectionPage")
            {
                MainPageView.Child = scanSelectionPage;
                scanSelectionPage.m_MainWin = this;
            }
            else if (pageName == "SettingsPage")
            {
                MainPageView.Child = scanSettingsPage;
                scanSettingsPage.m_MainWin = this;
            }
            else if (pageName == "DevicePage")
            {
                MainPageView.Child = scanDevicePage;
                scanDevicePage.m_MainWin = this;
            }
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        void win_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WindowProc));
            }
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle the message.
            switch (msg)
            {
                case (int)0x007E:   //#define WM_DISPLAYCHANGE                0x007E
                    {
                        MainWindow_Rufous _this = System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual as MainWindow_Rufous;

                        if (null != _this)
                        {
                            int nScreenWidth = (int)(lParam) & 0xFFFF;
                            int nScreenHeight = (int)(lParam) / 0xFFFF;

                            _this.AdjustAllWindowsSize(nScreenWidth, nScreenHeight);
                        }

                        break;
                    }
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void AdjustAllWindowsSize(int nScreenWidth, int nScreenHeight)
        {
            double OldRate = App.gScalingRate;

            App.CalcScalingRate((double)nScreenWidth, nScreenHeight);

            foreach (Window window in Application.Current.Windows)
            {
                window.Width = window.Width * App.gScalingRate / OldRate;
                window.Height = window.Height * App.gScalingRate / OldRate;
            }
        }      

        private System.IntPtr _handle = IntPtr.Zero;
        public System.IntPtr WindowHandle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                    _handle = (new WindowInteropHelper(App.Current.MainWindow)).Handle;
                return _handle;
            }
        }
        private void AddMessageHook()
        {
            HwndSource src = HwndSource.FromHwnd(WindowHandle);
            src.AddHook(new HwndSourceHook(this.WndProc));
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            System.Windows.Forms.Message m = new System.Windows.Forms.Message();
            m.HWnd = hwnd;
            m.Msg = msg;
            m.WParam = wParam;
            m.LParam = lParam;

            if (handled)
                return IntPtr.Zero;

            if (msg == App.WM_STATUS_UPDATE)
            {

            }
            else if (msg == App.WM_VOP)
            {

            }
            else if (msg == App.closeMsg)
            {
                Environment.Exit(0);
            }

            return IntPtr.Zero;
        }

        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

            if (null != btn)
            {
                if ("btnClose" == btn.Name)
                {
                    SettingData.Serialize(g_settingData, App.cfgFile);
                    this.Close();
                }
                else if ("btnMinimize" == btn.Name)
                {
                    //btnMinimize.Focusable = true;
                    //btnMinimize.Focus();
                    //this.Hide();
                    //btnMinimize.Focusable = false;
                }
            }
        }
    }
}
