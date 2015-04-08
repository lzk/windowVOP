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

    public enum PrinterModel
    {
        SFP,
        MFP
    }

    public partial class MainWindow : Window
    {
        public static RequestManager m_RequestManager = new RequestManager();

        public FileSelectionPage winFileSelectionPage = new FileSelectionPage();
        public PrintPage winPrintPage = new PrintPage();
        public StatusPanel statusPanelPage = new StatusPanel();

        private CopyPage    winCopyPage    = new CopyPage   ();
        private ScanPage    winScanPage    = new ScanPage   ();
        private SettingPage winSettingPage = new SettingPage();

        private ImageBrush imgBk_Brush_1 = null;
        private ImageBrush imgBk_Brush_2 = null;
        private ImageBrush imgBk_Brush_3 = null;
        private ImageBrush imgBk_Brush_4 = null;

        /// <summary>
        /// Thread used to update status of current printer.
        /// </summary>
        private Thread statusUpdater = null;

        /// <summary>
        /// Event used to sync between status update thread and main UI.
        /// </summary>
        private ManualResetEvent m_updaterAndUIEvent = new ManualResetEvent(true);

        private bool m_isOnlineDetected = false;    // true is one online printer has been seleted.

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



        public MainWindow()
        {
            statusPanelPage.eventPrinterSwitch += PrinterSwitch;
            App.g_autoMachine.eventStateUpdate += winCopyPage.HandlerStateUpdate;
            App.g_autoMachine.eventStateUpdate += winScanPage.HandlerStateUpdate;
            App.g_autoMachine.eventStateUpdate += winSettingPage.HandlerStateUpdate;
            App.g_autoMachine.eventStateUpdate += winPrintPage.HandlerStateUpdate;

            InitializeComponent();

            winCopyPage    .m_MainWin = this;
            winScanPage    .m_MainWin = this;
            winSettingPage .m_MainWin = this;
            winFileSelectionPage.m_MainWin = this;
            winPrintPage.m_MainWin = this;
            Init();

            SessionInfo session = new SessionInfo();
            m_RequestManager.GetSession(ref session);
        }

        void Init()
        {
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
            InitTrayMenu();

            imgBk_Brush_1 = (ImageBrush)this.FindResource("imgBk_Brush_1");
            imgBk_Brush_2 = (ImageBrush)this.FindResource("imgBk_Brush_2");
            imgBk_Brush_3 = (ImageBrush)this.FindResource("imgBk_Brush_3");
            imgBk_Brush_4 = (ImageBrush)this.FindResource("imgBk_Brush_4");
        }


        #region TrayMenu
        NotifyIcon notifyIcon1;
        void InitTrayMenu()
        {
            // Create the NotifyIcon. 
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();

            // The Icon property sets the icon that will appear 
            // in the systray for this application.
            System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,, /Images/printer.ico")).Stream;
            notifyIcon1.Icon = new System.Drawing.Icon(iconStream);

            // The ContextMenu property sets the menu that will 
            // appear when the systray icon is right clicked.
            System.Windows.Forms.ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem("退出");
            menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            contextMenu1.MenuItems.Clear();
            contextMenu1.MenuItems.Add(menuItem1);

            notifyIcon1.ContextMenu = contextMenu1;

            notifyIcon1.Text = "ABC Virtual Panel";
            notifyIcon1.Visible = true;

            // Handle the Click event to activate the form.
            notifyIcon1.MouseUp += NotifyIcon1_MouseUp;

            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
        }

        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }


        private void menuItem1_Click(object sender, System.EventArgs e)
        {
            this.MainWindowExitPoint();
        }

        public void NotifyIcon1_MouseUp(Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                // MessageBox.Show( "right button click" );
            }
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        #endregion  // TrayMenu

        public void handler_closed(Object sender, EventArgs e)
        {
            MainWindowExitPoint();
        }

        public void LoadedMainWindow( object sender, RoutedEventArgs e )
        {
            statusPageView.Child = statusPanelPage;
            this.statusPanelPage.Visibility = Visibility.Visible;
            AddMessageHook();
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
                    this.notifyIcon1.Visible = true;

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
                    MaintainWindow mw = new MaintainWindow();
                    mw.ShowDialog();

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

        private bool setTabItemFromIndex(int index)
        {
            if ( false == statusPanelPage.m_isSFP )
            {
                if (0 == index)
                {
                    this.subPageView.Child = winFileSelectionPage;
                
                    tabItem_Print.IsSelect = true;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if (1 == index)
                {
                    this.subPageView.Child = winCopyPage;
                 
                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = true;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if (2 == index)
                {
                    this.subPageView.Child = winScanPage;
                 
                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if (3 == index)
                {
                    this.subPageView.Child = winSettingPage;
                  
                    tabItem_Print.IsSelect = false;
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
                    this.subPageView.Child = winFileSelectionPage;
                   
                    tabItem_Print.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if (3 == index)
                {
                    this.subPageView.Child = winSettingPage;
                    
                    tabItem_Print.IsSelect = false;
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

        /// <summary>
        /// MainWindow unit exit point.
        /// </summary>
        private void MainWindowExitPoint()
        {
            bExitUpdater = true;
            m_updaterAndUIEvent.WaitOne();
            notifyIcon1.Visible = false;

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

               App.g_autoMachine.TranferState((EnumMachineJob)job);
               App.g_autoMachine.TranferState((EnumStatus)status);

           }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Handle printer switch event.
        /// </summary>
        /// <remarks>
        /// Execute the following logic:
        /// * Stop status update thread
        /// * Update Auto Machine
        /// * Get Printer Status
        /// * Init other sub pages
        /// * Start Status Update Thread
        /// </remarks>
        private void PrinterSwitch()
        {
            // Stop statusUpdater first before get printer status directly.
            if ( null != statusUpdater && true == statusUpdater.IsAlive )
            {
                bExitUpdater = true;
                m_updaterAndUIEvent.WaitOne();
                statusUpdater.Abort();
            }

            App.g_autoMachine.ResetAutoMachine();


            byte toner  = 0;
            byte status = (byte)EnumStatus.Offline; 
            byte job    = (byte)EnumMachineJob.UnknowJob;
            if (false == dll.GetPrinterStatus( statusPanelPage.m_selectedPrinter, ref status, ref toner, ref job) )
            {
                toner  = 0;
                status = (byte)EnumStatus.Offline; 
                job    = (byte)EnumMachineJob.UnknowJob;
            }

            statusPanelPage.m_job = (EnumMachineJob)job;
            statusPanelPage.m_toner = toner;
            statusPanelPage.m_currentStatus = (EnumStatus)status;

            bool bIsOffline = ( ( (byte)EnumStatus.Offline == status ) );

            if ( bIsOffline == false )
                m_isOnlineDetected = true;


            // TODO: uncomment this statement: if ( m_isOnlineDetected )
            if ( true )
            {
                line3.Visibility = Visibility.Visible;
                line8.Visibility = Visibility.Visible;
                Print_Grid.Visibility = Visibility.Visible;
                tabItem_Print.Visibility = Visibility.Visible;
                setTabItemFromIndex(0);

                if ( false == statusPanelPage.m_isSFP )
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

                    winSettingPage.mainGrid.Background = imgBk_Brush_4;

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

                    winSettingPage.mainGrid.Background = imgBk_Brush_2;
                }
            }
            else
            {
                line3.Visibility = Visibility.Hidden;
                line4.Visibility = Visibility.Hidden;
                line5.Visibility = Visibility.Hidden;
                line8.Visibility = Visibility.Hidden;
                line9.Visibility = Visibility.Hidden;
                line10.Visibility = Visibility.Hidden;


                Print_Grid.Visibility = Visibility.Hidden;
                Scan_Grid.Visibility = Visibility.Hidden;
                Copy_Grid.Visibility = Visibility.Hidden;
                Grid.SetColumn(Setting_Grid, 3);
                Grid.SetRow(Setting_Grid, 2);

                tabItem_Print.Visibility = Visibility.Hidden;
                tabItem_Copy.Visibility = Visibility.Hidden;
                tabItem_Scan.Visibility = Visibility.Hidden;

                Grid.SetColumn(tabItem_Setting, 3);
                Grid.SetRow(tabItem_Setting, 3);

                winSettingPage.mainGrid.Background = imgBk_Brush_1;

                setTabItemFromIndex(3);
            }

            // After UI already loaded, tranfer auto machine. 
            App.g_autoMachine.TranferState( (EnumMachineJob)job );
            App.g_autoMachine.TranferState( (EnumStatus)status );

            statusUpdater = new Thread(UpdateStatusCaller);
            statusUpdater.Start();
        }

    }
}
