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
using System.Windows.Forms;
using System.Threading;
using System.Windows.Interop;

namespace VOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///    

    public partial class MainWindow : Window
    {
        CopyPage    winCopyPage   = new CopyPage   ();
        public FileSelectionPage winFileSelectionPage = null;
        public PrintPage   winPrintPage  = new PrintPage  ();
        ScanPage    winScanPage   = new ScanPage   ();
        SettingPage winSettingPage= new SettingPage();
        public StatusPanel statusPanelPage = new StatusPanel();

        private ImageBrush imgBk_Brush_1 = null;
        private ImageBrush imgBk_Brush_2 = null;

        /// <summary>
        /// Thread used to update status of current printer.
        /// </summary>
        public Thread statusUpdater = null;

        /// <summary>
        /// Event used to sync between status update thread and main UI.
        /// </summary>
        public ManualResetEvent m_updaterAndUIEvent = new ManualResetEvent(true);

        private bool PrinterExist(string strPrinterName)
        {
            bool bExist = false;
            List<string> list_printers = new List<string>();

            common.GetSupportPrinters(list_printers);

            foreach (string printername in list_printers)
            {
                if (null != strPrinterName && printername == strPrinterName)
                {
                    bExist = true;
                    break;
                }
            }

            return bExist;
        }

        public static string GetPrinterDrvName(
                string strPrinterName
                )
        {
            string strDrvName = "";

            try
            {
                PrintServer myPrintServer = new PrintServer(null);
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    if (strPrinterName == pq.Name)
                    {
                        strDrvName = pq.QueueDriver.Name;
                        break;
                    }
                }
            }
            catch
            {
            }

            return strDrvName;
        }




        public MainWindow()
        {
            InitializeComponent();

            Init();
        }

        void Init()
        {
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            InitTrayMenu();

            imgBk_Brush_1 = (ImageBrush)this.FindResource("imgBk_Brush_1");
            imgBk_Brush_2 = (ImageBrush)this.FindResource("imgBk_Brush_2");




        }


        #region TrayMenu
        NotifyIcon notifyIcon;
        void InitTrayMenu()
        {    
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = System.Windows.Forms.Application.ProductName;
            this.notifyIcon.Text = System.Windows.Forms.Application.ProductName;
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = false;
            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(500);

            System.Windows.Forms.ContextMenuStrip contextMenu = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem item1 = new System.Windows.Forms.ToolStripMenuItem();
            item1.Click += item1_Click;
            item1.Text = "退出";
           
            contextMenu.Items.Add(item1);
            this.notifyIcon.ContextMenuStrip = contextMenu;
        }

        void item1_Click(object sender, EventArgs e)
        {
            this.MainWindowExitPoint();
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Normal;
            this.notifyIcon.Visible = false;
        }

        #endregion  // TrayMenu

        public void LoadedMainWindow( object sender, RoutedEventArgs e )
        {

            winFileSelectionPage = new FileSelectionPage(this);

            statusPageView.Child = statusPanelPage;
            setTabItemFromIndex(0);

            this.subPageView.Child = winFileSelectionPage;      // for test  
            this.statusPanelPage.Visibility = Visibility.Hidden;

            IsScanCopy_Usable = true;
            
            AddMessageHook();

            statusUpdater = new Thread(UpdateStatusCaller);
            statusUpdater.Start();
        }

        public void MyMouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if ( position.Y < 40 && position.Y > 0 )
                this.DragMove();
        }

        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

            if ( null != btn )
            {
                if ( "btnClose" == btn.Name )
                {
                    this.MainWindowExitPoint();
                }
                else if ("btnMinimize" == btn.Name)
                {
                    this.ShowInTaskbar = false;
                    this.notifyIcon.Visible = true;

                    this.WindowState = WindowState.Minimized;
                }
            }
        }

        private void NvgBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Control btn = sender as System.Windows.Controls.Control;

            if ( null != btn )
            {
                if ( "btnPrint" == btn.Name )
                {                

                     setTabItemFromIndex(0);

                }
                else if ( "btnCopy" == btn.Name )
                {                   

                    setTabItemFromIndex(1);
                }
                else if ( "btnScan" == btn.Name )
                {                    

                    setTabItemFromIndex(2);
                }
                else if ( "btnSetting" == btn.Name )
                {                

                    setTabItemFromIndex(3);
                }
                else if ( "btnLogin" == btn.Name )
                {
                    LoginWindow loginWnd = new LoginWindow();
                    loginWnd.ShowActivated = true;
                    Nullable<bool> dialogResult = loginWnd.ShowDialog();

                    string strPhoneNumber;
                    if(dialogResult == true)
                        strPhoneNumber = loginWnd.m_strPhoneNumber;
                }
            }

        }

        #region Set_TabItemIndex

        private bool isScanCopy_Usable = true;
        private bool IsScanCopy_Usable
        {
            get { return isScanCopy_Usable; }
            set
            {
                isScanCopy_Usable = value;

                if (isScanCopy_Usable)
                {
                    line4.Visibility = Visibility.Visible;
                    line5.Visibility = Visibility.Visible;
                    line9.Visibility = Visibility.Visible;
                    line10.Visibility = Visibility.Visible;

                    Scan_Grid.Visibility = Visibility.Visible;
                    Copy_Grid.Visibility = Visibility.Visible;
                    Grid.SetColumn(Setting_Grid, 9);
                    Grid.SetRow(Setting_Grid, 2);

                    tabItem_Copy.Visibility = Visibility.Visible;
                    tabItem_Scan.Visibility = Visibility.Visible;

                    Grid.SetColumn(tabItem_Setting, 9);
                    Grid.SetRow(tabItem_Setting, 3);

                    winSettingPage.mainGrid.Background = imgBk_Brush_2;

                }
                else
                {
                    line4.Visibility = Visibility.Hidden;
                    line5.Visibility = Visibility.Hidden;
                    line9.Visibility = Visibility.Hidden;
                    line10.Visibility = Visibility.Hidden;

                    Scan_Grid.Visibility = Visibility.Hidden;
                    Copy_Grid.Visibility = Visibility.Hidden;
                    Grid.SetColumn(Setting_Grid, 5);
                    Grid.SetRow(Setting_Grid, 2);

                    tabItem_Copy.Visibility = Visibility.Hidden;
                    tabItem_Scan.Visibility = Visibility.Hidden;

                    Grid.SetColumn(tabItem_Setting, 5);
                    Grid.SetRow(tabItem_Setting, 3);

                    winSettingPage.mainGrid.Background = imgBk_Brush_1;
                }
            }
        }


        private bool setTabItemFromName(string tabItemName)
        {
            if (0 == String.Compare(tabItemName, "printer", true))
            {
                setTabItemFromIndex(0);   
            }
            else if (0 == String.Compare(tabItemName, "copy", true))
            {
                setTabItemFromIndex(1);
            }
            else if (0 == String.Compare(tabItemName, "scan", true))
            {
                setTabItemFromIndex(2);
            }
            else if (0 == String.Compare(tabItemName, "setting", true))
            {
                setTabItemFromIndex(3);
            } 
            else
            {
                return false;
            }

            return true;
        }
        private bool setTabItemFromIndex(int index)
        {
            if (IsScanCopy_Usable)
            {
                if (0 == index)
                {
                    this.subPageView.Child = winPrintPage;
                    this.statusPanelPage.Visibility = Visibility.Visible;

                tabItem_Printer.IsSelect = true;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (1 == index)
            {
                this.subPageView.Child = winCopyPage;
                this.statusPanelPage.Visibility = Visibility.Visible;

                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = true;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (2 == index)
            {
                this.subPageView.Child = winScanPage;
                this.statusPanelPage.Visibility = Visibility.Visible;

                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = true;
                tabItem_Setting.IsSelect = false;
            }
            else if (3 == index)
            {
                this.subPageView.Child = winSettingPage;
               // this.statusPanelPage.Visibility = Visibility.Hidden;

                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = true;
            }
            else
            {
                return false;
            }
            }
            else
            {
                if (0 == index)
                {
                    this.subPageView.Child = winPrintPage;
                    this.statusPanelPage.Visibility = Visibility.Visible;

                    tabItem_Printer.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if (3 == index)
                {
                    this.subPageView.Child = winSettingPage;
                    this.statusPanelPage.Visibility = Visibility.Hidden;

                    tabItem_Printer.IsSelect = false;
                    tabItem_Setting.IsSelect = true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        #endregion // end Set_TabItemIndex

        private void btnLogin_btnClick(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Exit falg. True if need to exit thread statusUpdater.
        /// </summary>
        bool bExitUpdater = false;

        // Those variable were used for post WM_STATUS_UPDATE message, do not
        // for other usage.
        private byte _toner  = 0;
        private byte _status = (byte)EnumStatus.Offline; 
        private byte _job    = (byte)EnumMachineJob.UnknowJob;

        public void UpdateStatusCaller()
        {
            int nFailCnt = 0;

            m_updaterAndUIEvent.Reset();

            // TODO: Add Thread Sync mechanism for share variable
            bExitUpdater = false;

            while ( !bExitUpdater )
            {
                if (false == dll.GetPrinterStatus( statusPanelPage.m_selectedPrinter, ref _status, ref _toner, ref _job) )
                {
                    nFailCnt++;
                    
                    // If status getting fail more than 3 times, reset the status
                    if ( nFailCnt >= 3 )
                    {
                        _toner  = 0;
                        _status = (byte)EnumStatus.Offline; 
                        _job    = (byte)EnumMachineJob.UnknowJob;
                    }
                }            
                else
                {
                    nFailCnt = 0;
                }

                // TODO: post the status message to the main window
                Win32.PostMessage( (IntPtr)0xffff, App.WM_STATUS_UPDATE, IntPtr.Zero , IntPtr.Zero );


                for ( int i=0; i<6; i++ )
                {
                    if ( bExitUpdater )
                        break;

                    System.Threading.Thread.Sleep(500);
                }
            }

            m_updaterAndUIEvent.Set();
        }

        private void MainWindowExitPoint()
        {
            bExitUpdater = true;
            m_updaterAndUIEvent.WaitOne();

            this.Close();
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

           if (msg == App.WM_STATUS_UPDATE )
           {
               byte toner  = 0;
               byte status = (byte)EnumStatus.Offline; 
               byte job    = (byte)EnumMachineJob.UnknowJob;

               // TODO: add sync mechanism 
               toner  = _toner ;
               status = _status;
               job    = _job   ;

               this.statusPanelPage.m_toner         = toner;
               this.statusPanelPage.m_currentStatus = (EnumStatus)status;
               this.statusPanelPage.m_job           = (EnumMachineJob)job;


           }

            return IntPtr.Zero;
        }
    }
}
