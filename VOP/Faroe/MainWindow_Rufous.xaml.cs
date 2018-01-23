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
using System.Runtime.InteropServices;

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

        private Thread thread_searchIP = null;
        private Thread statusUpdater = null;
        private Thread scanButtonCheck = null;
        private ManualResetEvent m_updaterAndUIEvent = new ManualResetEvent(true);
        private bool _bExitUpdater = false;
        private bool _bExitCheckButton = false;

        public static byte m_byWifiInitStatus = 0;
        public string m_strPassword = "";
        public bool _bScanning = false;

        private bool bGrayIcon = false; // True if the tray icon was set to gray.
        private const string USBSCANSTRING = "\\\\.\\usbscan";

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

            //add by yunying shang 2018-01-19 for Push Scan
            scanButtonCheck = new Thread(CheckScanButton);
            scanButtonCheck.Start();
            //<<=================       
            
            this.SourceInitialized += new EventHandler(win_SourceInitialized);  
        }

        //add by yunying shang 2017-12-06 for BMS 1533
        public string GetDeviceName(string devicename)
        {
            if (devicename == string.Empty)
            {
                if (MainWindow_Rufous.g_settingData.m_isUsbConnect == false)
                    return MainWindow_Rufous.g_settingData.m_DeviceName;
                else
                {
                    int index = MainWindow_Rufous.g_settingData.m_DeviceName.LastIndexOf(' ');
                    if (index > 0)
                    {
                        string str = USBSCANSTRING;
                        str += MainWindow_Rufous.g_settingData.m_DeviceName.Substring(index+1);

                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            else
            {
                int index = devicename.LastIndexOf(' ');
                if (index > 0)
                {
                    string str = USBSCANSTRING;
                    str += devicename.Substring(index+1);

                    return str;
                }
                else
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }//<<================1533

        public void LoadedMainWindow(object sender, RoutedEventArgs e)
        {
            g_settingData = SettingData.Deserialize(App.cfgFile);

            int iRtn = CheckDeviceStatus();

            //modified by yunying shang 2017-11-29 for BMS 1419
            if (iRtn <= 0)
            {
                scanSelectionPage.DeviceButton.Connected = false;
                scanSettingsPage.PassStatus(false);
                scanSelectionPage.tbStatus.Text = "Disconnected";
            }
            else
            {
                if (MainWindow_Rufous.g_settingData.m_DeviceName != "")
                {

                    if (MainWindow_Rufous.g_settingData.m_DeviceName.Contains("USB"))
                    {
                        if (iRtn == 1 || iRtn == 3)
                        {
                            MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                            dll.SetConnectionMode(GetDeviceName(""), true);
                        }
                        else
                        {
                            MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                            dll.SetConnectionMode("", false);
                        }
                    }
                    else
                    {
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                        if (iRtn == 2 || iRtn == 3)
                        {
                            if (!dll.TestIpConnected(MainWindow_Rufous.g_settingData.m_DeviceName))
                            {
                                if (iRtn == 3)
                                {                                    
                                    StringBuilder usbname = new StringBuilder(50);
                                    if (dll.CheckUsbScan(usbname)==1)
                                    {
                                        MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                                        dll.SetConnectionMode(GetDeviceName(usbname.ToString()), true);
                                    }
                                }
                                else
                                {
                                    MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                                    dll.SetConnectionMode("", false);
                                }
                            }
                            else
                            {
                                dll.SetConnectionMode(MainWindow_Rufous.g_settingData.m_DeviceName, false);
                            }
                        }
                        else
                        {
                            MainWindow_Rufous.g_settingData.m_isUsbConnect = true;

                            StringBuilder usbname = new StringBuilder(50);
                            if (dll.CheckUsbScan(usbname) == 1)
                            {
                                dll.SetConnectionMode(GetDeviceName(usbname.ToString()), true);
                            }
                        }
                    }
                    
                }
                else
                {
                    if (iRtn == 1 || iRtn == 3)
                    {
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                        dll.SetConnectionMode(GetDeviceName(""), true);
                    }
                    else
                    {
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                        dll.SetConnectionMode("", false);
                    }
                }
            }//<<=======================1419

            common.GetAllPrinters(g_printerList);

            MainPageView.Child = scanSelectionPage;
            scanSelectionPage.m_MainWin = this;            
            AddMessageHook();
            if (App.gPushScan)
            {
                scanSelectionPage.PushScan();
                App.gPushScan = false;
            }
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
                if (dll.CheckUsbScanByName(GetDeviceName("")) == 1)  
                        //MainWindow_Rufous.g_settingData.m_DeviceName == usbName.ToString())
                {
                    dll.SetConnectionMode(GetDeviceName(""), true);
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
                    dll.SetConnectionMode(GetDeviceName(usbName.ToString()), true);
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

        public int CheckDeviceStatus()
        {
            int bResult = -1;

            StringBuilder usbName = new StringBuilder(50);

            if (dll.CheckUsbScan(usbName) == 1)
            {
                Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                bResult = 1;
            }

            if (!scanDevicePage.IsOnLine())
            {
                Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
            }
            else//<<===============1019
            {
                //marked by yunying shang 2019-01-11 for BMS1230
                //NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                //bool bFound = false;
                //foreach (NetworkInterface adapter in fNetworkInterfaces)
                //{
                //    if (adapter.Description.Contains("802") ||
                //        adapter.Description.Contains("Wi-Fi") ||
                //        adapter.Description.Contains("Wireless"))
                //    {
                //        bFound = true;
                //    }
                //}

                //if (bFound == false)
                //{
                //    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                //}
                //else<<==============1230
                {
                   // Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                    if (bResult > 0)
                        bResult = 3;
                    else
                        bResult = 2;
                }
            }
            

            return bResult;
        }

        public void UpdateStatusCaller()
        {
            m_updaterAndUIEvent.Reset();

            _bExitUpdater = false;
            while (!_bExitUpdater)// && !_bScanning)
            {
                bool bConnect = false;
               if (dll.CheckConnectionByName(GetDeviceName("")))
                {
                    Win32.OutputDebugString("Connection success!");
                    //SetDeviceButtonState(true);
                    //modified by yunying shang 2017-10-19 for BMS 1172
                    if (MainWindow_Rufous.g_settingData.m_isUsbConnect == false)
                    {
                        //add by yunying shang 2017-10-23 for BMS 1019
                        if (!scanDevicePage.IsOnLine())
                        {
                            Win32.OutputDebugString("not on line!");
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                        }
                        else//<<===============1019
                        {
                            //marked by yunying shang 1320
                            //NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                            //bool bFound = false;
                            //foreach (NetworkInterface adapter in fNetworkInterfaces)
                            //{
                            //    if (adapter.Description.Contains("802") ||
                            //        adapter.Description.Contains("Wi-Fi") ||
                            //        adapter.Description.Contains("Wireless"))
                            //    {
                            //        bFound = true;
                            //    }
                            //}

                            //if (bFound == false)
                            //{
                            //    Win32.OutputDebugString("not find the wifi network!");
                            //    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                            //}
                            //else//<<===============1320
                            {
                                if (dll.TestIpConnected(MainWindow_Rufous.g_settingData.m_DeviceName))
                                {
                                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                                    bConnect = true;
                                }
                                else
                                {
                                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                                }
                            }
                        }
                    }
                    else
                    {
                        Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                        bConnect = true;
                    }//<<=================1172
                }
                else
                {
                    if (MainWindow_Rufous.g_settingData.m_isUsbConnect == true)
                    {
                        StringBuilder usbname = new StringBuilder(50);
                        if (dll.CheckUsbScan(usbname) == 1)
                        {
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)1, IntPtr.Zero);
                            bConnect = true;
                        }
                        else
                        {
                            Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                        }
                    }
                    else
                    {
                        //SetDeviceButtonState(false);
                        Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, (IntPtr)0, IntPtr.Zero);
                    }
                }

               //marked by yunying shang, this button command could not be used on SW
                //if (bConnect && MainWindow_Rufous.g_settingData.m_isUsbConnect == true)
                //{
                //    if (dll.GetButtonPressed() == 1)
                //    {
                //        Win32.PostMessage((IntPtr)0xffff, App.WM_BUTTON_PRESSED, IntPtr.Zero, IntPtr.Zero);
                //    }
                //}
                for (int i = 0; i < 6; i++)
                {
                    if (_bExitUpdater)
                        break;
                    System.Threading.Thread.Sleep(500);
                }
            }

            m_updaterAndUIEvent.Set();
        }

        //add by yunying shang 2018-01-19 for Push Scan
        public void CheckScanButton()
        {
            _bExitCheckButton = false;

            if (dll.OpenScanToPC() > 0)
            {

                while (!_bExitCheckButton)
                {
                    if (dll.GetScanButton() > 0)
                    {
                        Win32.PostMessage((IntPtr)0xffff, App.WM_BUTTON_PRESSED, IntPtr.Zero, IntPtr.Zero);
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        if (_bExitCheckButton)
                            break;
                        System.Threading.Thread.Sleep(500);
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
                if(!scanSelectionPage.ImageButton4.IsEnabled)
                {
                    scanSelectionPage.ImageButton4.IsEnabled = true;
                    scanSelectionPage.ImageButton4.Focus();
                } 
                else if (!scanSelectionPage.ImageButton3.IsEnabled)
                {
                    scanSelectionPage.ImageButton3.IsEnabled = true;
                    scanSelectionPage.ImageButton3.Focus();
                }
                if (App.gPushScan)
                {
                    scanSelectionPage.PushScan();
                    App.gPushScan = false;
                }
                           
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
        private void btnMinimize_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnMinimize.Focusable = true;
                btnMinimize.Focus();
                this.Hide();
                btnMinimize.Focusable = false;
            }            
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
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
                    //MainWindow_Rufous.g_settingData.m_DeviceName = "";
                    scanSelectionPage.DeviceButton.Connected = false;
                    scanSettingsPage.PassStatus(false);
                    bUseGrayIcon = true;

                    //add by yunying shang 2017-11-10 for BMS 1372
                    if (_bScanning)
                    {
                        _bScanning = false;
                        dll.ADFCancel();
                        // VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        // Application.Current.MainWindow,
                        //(string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Device_Disconnected"),
                        //(string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                        // );

                    }//<<===============1372
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
            else if (msg == App.WM_COPYDATA)
            {
                App.gPushScan = true;
                App.COPYDATASTRUCT cds = (App.COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(App.COPYDATASTRUCT));
                string rece = cds.lpData;
                if (rece == "PushScan")
                {
                    GotoPage("ScanSelectionPage", null);
                }
            }
            else if (msg == App.WM_BUTTON_PRESSED)
            {
                //modified by yunying shang 2018-01-19 for Push Scan
                if (MainPageView.Child == scanPage)
                {
                    scanPage.Button_Click(null, null);

                    if (MainPageView.Child == scanSelectionPage)
                        scanSelectionPage.ScanToButtonClick(null, null);
                    
                }
                else
                {
                    GotoPage("ScanSelectionPage", null);
                    scanSelectionPage.ScanToButtonClick(null, null);
                }
                
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
                                                (string)this.TryFindResource("ResStr_Do_you_want_to_exit"),
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
            _bExitCheckButton = true;
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
