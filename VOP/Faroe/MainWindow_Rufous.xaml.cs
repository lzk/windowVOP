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
using System.Net.NetworkInformation;

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
        private Thread statusUpdater = null;
        private ManualResetEvent m_updaterAndUIEvent = new ManualResetEvent(true);
        private bool _bExitUpdater = false;

        public static byte m_byWifiInitStatus = 0;
        public string m_strPassword = "";
        public bool _bScanning = false;

        private bool bGrayIcon = false; // True if the tray icon was set to gray.

        public MainWindow_Rufous()
        {
            InitializeComponent();

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            InitTrayMenu();
            g_settingData.InitSettingData();

            thread_searchIP = new Thread(InitIPList);
            thread_searchIP.Start();

            statusUpdater = new Thread(UpdateStatusCaller);
            statusUpdater.Start();

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

        #region TrayMenu
        System.Windows.Forms.NotifyIcon notifyIcon1;
        void InitTrayMenu()
        {
            // Create the NotifyIcon. 
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();

            // The Icon property sets the icon that will appear 
            // in the systray for this application.
            System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,, /Images/printerGray.ico")).Stream;
            bGrayIcon = true;
            notifyIcon1.Icon = new System.Drawing.Icon(iconStream);

            // The ContextMenu property sets the menu that will 
            // appear when the systray icon is right clicked.
            System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem((string)this.TryFindResource("ResStr_Exit"));
            menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            contextMenu1.MenuItems.Clear();
            contextMenu1.MenuItems.Add(menuItem1);

            notifyIcon1.ContextMenu = contextMenu1;

            notifyIcon1.Text = "Faroe Scan";
            notifyIcon1.Visible = true;

            // Handle the Double Click event to activate the form.        
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
        }

        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void menuItem1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        #endregion  // TrayMenu

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
            bool canConnected = false;
            ScanDevicePage_Rufous.ListIP();

            StringBuilder usbName = new StringBuilder(50);
            if (dll.CheckUsbScan(usbName) == 1)
            {
                if (MainWindow_Rufous.g_settingData.m_DeviceName == usbName.ToString())
                {
                    dll.SetConnectionMode(usbName.ToString(), true);
                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                    // SetDeviceButtonState(true);
                    MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                    canConnected = true;
                }
            }

            if (ScanDevicePage_Rufous.ipList != null)
            {                       
                foreach (string ip in ScanDevicePage_Rufous.ipList)
                {
                    if (MainWindow_Rufous.g_settingData.m_DeviceName == ip)
                    {
                        dll.SetConnectionMode(ip, false);
                        //SetDeviceButtonState(true);
                        Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                        canConnected = true;
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                    }
                }                
            }

            if (canConnected == false)
            {
                if (dll.CheckUsbScan(usbName) == 1)
                {                 
                    dll.SetConnectionMode(usbName.ToString(), true);
                    MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                }
                else
                {
                    if (ScanDevicePage_Rufous.ipList != null)
                    {
                        foreach (string ip in ScanDevicePage_Rufous.ipList)
                        {
                            dll.SetConnectionMode(ip, false);
                            MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                            break;
                        }
                    }
                }
            }
        }

        public void CheckDeviceStatus()
        {
            if (dll.CheckConnection())
            {
                //SetDeviceButtonState(true);
                //modified by yunying shang 2017-10-19 for BMS 1172
                if (MainWindow_Rufous.g_settingData.m_isUsbConnect == false)
                {
                    //add by yunying shang 2017-10-23 for BMS 1019
                    if (!scanDevicePage.IsOnLine())
                    {
                        Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                    }
                    else//<<===============1019
                    {
                        NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                        bool bFound = false;
                        foreach (NetworkInterface adapter in fNetworkInterfaces)
                        {
                            if (adapter.Description.Contains("802") ||
                                adapter.Description.Contains("Wi-Fi") ||
                                adapter.Description.Contains("Wireless"))
                            {
                                bFound = true;
                            }
                        }

                        if (bFound == false)
                        {
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                        }
                        else
                        {
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                        }
                    }
                }
                else
                {
                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                }//<<=================1172
            }
            else
            {
                Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
            }
        }

        public void UpdateStatusCaller()
        {
            m_updaterAndUIEvent.Reset();

            _bExitUpdater = false;
            while (!_bExitUpdater && !_bScanning)
            {
                if (dll.CheckConnection())
                {
                    //SetDeviceButtonState(true);
                    //modified by yunying shang 2017-10-19 for BMS 1172
                    if (MainWindow_Rufous.g_settingData.m_isUsbConnect == false)
                    {
                        //add by yunying shang 2017-10-23 for BMS 1019
                        if (!scanDevicePage.IsOnLine())
                        {
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                        }
                        else//<<===============1019
                        {
                            NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                            bool bFound = false;
                            foreach (NetworkInterface adapter in fNetworkInterfaces)
                            {
                                if (adapter.Description.Contains("802") ||
                                    adapter.Description.Contains("Wi-Fi") ||
                                    adapter.Description.Contains("Wireless"))
                                {
                                    bFound = true;
                                }
                            }

                            if (bFound == false)
                            {
                                Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                            }
                            else
                            {
                                Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                            }
                        }
                    }
                    else
                    { 
                        Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                    }//<<=================1172
                }
                else
                {

                    //SetDeviceButtonState(false);
                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                }

                for (int i = 0; i < 6; i++)
                {
                    if (_bExitUpdater)
                        break;
                    System.Threading.Thread.Sleep(500);
                }
            }

            m_updaterAndUIEvent.Set();
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
                bool bUseGrayIcon = false;
                if ((int)wParam == 1)
                {
                    scanSelectionPage.DeviceButton.Connected = true;
                    scanSettingsPage.PassStatus(true);
                    bUseGrayIcon = false;
                    if (MainWindow_Rufous.g_settingData.m_isUsbConnect)
                    {
                        scanSelectionPage.tbStatus.Text = "USB";
                    }
                    else
                    {
                        scanSelectionPage.tbStatus.Text = MainWindow_Rufous.g_settingData.m_DeviceName;
                    }
                }
                else
                {
                    scanSelectionPage.tbStatus.Text = "Disconnected";
                    MainWindow_Rufous.g_settingData.m_DeviceName = "";
                    scanSelectionPage.DeviceButton.Connected = false;
                    scanSettingsPage.PassStatus(false);
                    bUseGrayIcon = true;
                }
                if (bUseGrayIcon != bGrayIcon)
                {
                    string strIcon = bUseGrayIcon ? "pack://application:,,, /Images/printerGray.ico" : "pack://application:,,, /Images/printer.ico";
                    System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri(strIcon)).Stream;
                    notifyIcon1.Icon = new System.Drawing.Icon(iconStream);
                    bGrayIcon = bUseGrayIcon;
                }

            }
            else if (msg == App.WM_VOP)
            {
                PopupWindow();
            }
            else if (msg == App.closeMsg)
            {
                notifyIcon1.Dispose();
                Environment.Exit(0);
            }

            return IntPtr.Zero;
        }

        private void PopupWindow()
        {
            this.Visibility = Visibility.Visible;
            this.Activate();
            this.Topmost = true;  // important
            this.Topmost = false; // important
            this.Focus();         // important
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            if (VOP.Controls.MessageBoxExResult.Yes !=
                    VOP.Controls.MessageBoxEx.Show(
                                                VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                                                this,
                                                "Do you want to exit the Faroe VOP?",
                                                (string)this.TryFindResource("ResStr_Prompt")
                                                )
                )
            {
                e.Cancel = true;
            }
           

            if (false == e.Cancel)
                MainWindowExitPoint();
        }

        private void MainWindowExitPoint()
        {
            scanPage.image_wrappanel.Children.Clear();
            
            if (null != scanPage.scanningThread
                    && true == scanPage.scanningThread.IsAlive)
            {
                dll.CancelScanning();
                while (true == scanPage.scanningThread.IsAlive)
                {
                    // TODO: This statement will block UI thread. 
                    System.Threading.Thread.Sleep(100);
                }
            }
            if (thread_searchIP != null && thread_searchIP.IsAlive == true)
            {
                thread_searchIP.Join();
            }

            _bExitUpdater = true;
            m_updaterAndUIEvent.WaitOne();            
            notifyIcon1.Visible = false;
            SettingData.Serialize(g_settingData, App.cfgFile);       
        }
  
        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

            if (null != btn)
            {
                if ("btnClose" == btn.Name)
                {         
                    this.Close();
                }
                else if ("btnMinimize" == btn.Name)
                {
                    btnMinimize.Focusable = true;
                    btnMinimize.Focus();
                    this.Hide();
                    btnMinimize.Focusable = false;
                }
            }
        }
    }
}
