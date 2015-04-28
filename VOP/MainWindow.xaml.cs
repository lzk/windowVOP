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
        private Thread uploadCRMThread = null;

        /// <summary>
        /// Event used to sync between status update thread and main UI.
        /// </summary>
        private ManualResetEvent m_updaterAndUIEvent = new ManualResetEvent(true);

        private bool m_isOnlineDetected = false;    // true is one online printer has been seleted.

        public string m_strPassword = "";
        public string m_strPhoneNumber = "";
        public static bool   m_bLocationIsChina = false;

        private bool m_isAnimationPopup = false;  // True if animation window had popup.
        public  bool m_isCloseAnimation = false;  // True if animation window need to close.
        public  string m_animationUri = "";       // Animation Uri need to display

        public bool PasswordCorrect()
        {
            bool bCorrect = false;
            if (m_strPassword.Length > 0)
            {
                string strPrinterName = statusPanelPage.m_selectedPrinter;
                PasswordRecord m_rec = new PasswordRecord(strPrinterName, m_strPassword);
                AsyncWorker worker = new AsyncWorker(this);

                m_rec = worker.ConfirmPassword(m_rec);

                if (m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    bCorrect = true;
                }

            }     
            return bCorrect;
        }

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
            InitializeComponent();

            const int GEOCLASS_NATION = 16;
            int nGeoID = Win32.GetUserGeoID(GEOCLASS_NATION);
            if (45 == nGeoID)
            {
                m_bLocationIsChina = true;
            }

            statusPanelPage.eventPrinterSwitch += PrinterSwitch;
            App.g_autoMachine.eventStateUpdate += winCopyPage.HandlerStateUpdate;
            App.g_autoMachine.eventStateUpdate += winScanPage.HandlerStateUpdate;
            App.g_autoMachine.eventStateUpdate += winSettingPage.HandlerStateUpdate;
            App.g_autoMachine.eventStateUpdate += winPrintPage.HandlerStateUpdate;

            winCopyPage    .m_MainWin = this;
            winScanPage    .m_MainWin = this;
            winSettingPage .m_MainWin = this;
            winFileSelectionPage.m_MainWin = this;
            winPrintPage.m_MainWin = this;
            Init();

            SessionInfo session = new SessionInfo();
            m_RequestManager.GetSession(ref session);

            btnLogin.Visibility = m_bLocationIsChina ? Visibility.Visible : Visibility.Hidden;
            
            RegistryKey rsg = null;
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Lenovo\\Printer SSW\\Version", false);
            Int32 nLanguage = 0x804;
            object obj = null;
            if (null != rsg)
            {
                obj = rsg.GetValue("language", RegistryValueKind.DWord);
                nLanguage = (Int32)obj;

                if (0x804 == nLanguage)
                    this.FontFamily = new FontFamily("幼圆");
                else
                    this.FontFamily = new FontFamily("Arial");

                rsg.Close();
            }

            if (true == m_bLocationIsChina)
            {
                uploadCRMThread = new Thread(UploadCRM_LocalInfoToServerCaller);
                uploadCRMThread.Start();
            }
        }

        public static bool SaveCRMDataIntoXamlFile(string strFileName, DateTime date, string strData)
        {
            bool bSuccess = false;
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                var directory = new DirectoryInfo(documentsPath);
                string strUsersPublic = directory.Parent.FullName;
                string strDirectory = strUsersPublic + "\\Lenovo\\";

                Directory.CreateDirectory(strDirectory);

                XmlDocument doc = new XmlDocument();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement eleBody = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(eleBody);

                XmlElement eleTime = doc.CreateElement(string.Empty, "Time", string.Empty);
                XmlText textTime = doc.CreateTextNode(date.ToShortDateString());
                eleTime.AppendChild(textTime);
                eleBody.AppendChild(eleTime);

                XmlElement eleData = doc.CreateElement(string.Empty, "Data", string.Empty);
                XmlText textData = doc.CreateTextNode(strData);
                eleData.AppendChild(textData);
                eleBody.AppendChild(eleData);

                doc.Save(strDirectory + strFileName);

                bSuccess = true;
            }
            catch
            {

            }

            return bSuccess;
        }

        public static bool ReadCRMDataFromXamlFile(string strFileName, ref DateTime date, ref string strData)
        {
            bool bSuccess = false;
           
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                var directory = new DirectoryInfo(documentsPath);
                string strUsersPublic = directory.Parent.FullName;
                string strPath = strUsersPublic + "\\Lenovo\\" + strFileName;

                if (File.Exists(strPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(strPath);

                    string strText = "";
                    XmlNode xmlNode = doc.SelectSingleNode("body/Time");
                    strText = xmlNode.InnerText;
                    date = DateTime.Parse(strText);
                    XmlNode xmlNode2 = doc.SelectSingleNode("body/Data");
                    strData = xmlNode2.InnerText;

                    bSuccess = true;
                }
            }
            catch
            {

            }

            return bSuccess;
        }

        bool bExit = false;
        public void UploadCRM_LocalInfoToServerCaller()
        {
            while(!bExit)
            {
                if (true == UploadCRM_LocalInfoToServer())
                    break;

                for (int i = 0; i < 1200; i++)
                {
                    if (bExit)
                        break;

                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private bool UploadCRM_LocalInfoToServer()
        {
            CRM_LocalInfo lci = new CRM_LocalInfo();
            lci.m_strMobileNumber = m_strPhoneNumber;
            lci.m_strAppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            JSONResultFormat2 rtValue = new JSONResultFormat2();

            return m_RequestManager.UploadCRM_LocalInfoToServer(ref lci, ref rtValue);
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
        System.Windows.Forms.NotifyIcon notifyIcon1;
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

            // Handle the Double Click event to activate the form.        
            notifyIcon1.DoubleClick += notifyIcon1_Click;
        }

        void notifyIcon1_Click(object sender, EventArgs e)
        {
             this.Show();

            this.ShowInTaskbar = true;         
        }

        private void menuItem1_Click(object sender, System.EventArgs e)
        {
            this.MainWindowExitPoint();
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

            ShowAboutPageOnly();

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

                    //this.WindowState = WindowState.Minimized;
                    this.Hide();
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
                     SetTabItemFromIndex( EnumSubPage.Print );
                }
                else if ( "btnCopy" == btn.Name )
                {                   
                    SetTabItemFromIndex( EnumSubPage.Copy );
                }
                else if ( "btnScan" == btn.Name )
                {                    
                    SetTabItemFromIndex( EnumSubPage.Scan );
                }
                else if ( "btnSetting" == btn.Name )
                {                
                    SetTabItemFromIndex( EnumSubPage.Setting );
                }
                else if ( "btnLogin" == btn.Name )
                {
                    LoginWindow loginWnd = new LoginWindow();
                    loginWnd.ShowActivated = true;
                    Nullable<bool> dialogResult = loginWnd.ShowDialog();

                    if(dialogResult == true)
                    {
                        m_strPhoneNumber = loginWnd.m_strPhoneNumber;
                        btnLogin.IsLogon = true;
                        btnLogin.bottomText = m_strPhoneNumber;
                    }
                }
            }

        }

        #region Set_TabItemIndex

        /// <summary>
        /// Switch sub page. 
        /// </summary>
        private bool SetTabItemFromIndex( EnumSubPage subpage )
        {
            if ( false == statusPanelPage.m_isSFP )
            {
                if ( EnumSubPage.Print == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_Print");
                    this.subPageView.Child = winFileSelectionPage;
                
                    tabItem_Print.IsSelect = true;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Copy == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_Copy");
                    this.subPageView.Child = winCopyPage;
                 
                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = true;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Scan == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_Scan");
                    this.subPageView.Child = winScanPage;
                 
                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Setting == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_Setting");
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
                if ( EnumSubPage.Print == subpage )
                {
                    this.subPageView.Child = winFileSelectionPage;
                   
                    tabItem_Print.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Setting == subpage )
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
        private bool bExitUpdater = false;
        private bool m_isShowedMaintainWindow = false;

        // NOTE: Those variable were used for post WM_STATUS_UPDATE message, do not for other usage.
        private byte _toner  = 0;
        private byte _status = (byte)EnumStatus.Offline; 
        private byte _job    = (byte)EnumMachineJob.UnknowJob;
        private object statusLock = new object(); // Use to sync status share varibles.

        public void UpdateStatusCaller()
        {
            int nFailCnt = 0;

            m_updaterAndUIEvent.Reset();

            // The UpdateStatusCaller only read the share variables,
            // so don't add Thread Sync mechanism for share variable yet.
            bExitUpdater = false;

            byte _tmpToner  = 0;
            byte _tmpStatus = (byte)EnumStatus.Offline; 
            byte _tmpJob    = (byte)EnumMachineJob.UnknowJob;

            while ( !bExitUpdater )
            {
                if (false == GetPrinterStatusEx(statusPanelPage.m_selectedPrinter, ref _tmpStatus, ref _tmpToner, ref _tmpJob))
                {
                    nFailCnt++;
                    
                    // If status getting fail more than 3 times, reset the status
                    if ( nFailCnt >= 3 )
                    {
                        _tmpToner  = 0;
                        _tmpStatus = (byte)EnumStatus.Offline; 
                        _tmpJob    = (byte)EnumMachineJob.UnknowJob;
                    }
                }            
                else
                {
                    nFailCnt = 0;
                }

                lock ( statusLock )
                {
                    _toner  = _tmpToner ;
                    _status = _tmpStatus;
                    _job    = _tmpJob   ;
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
            bool bAllowExit = true;

            if ( null != winScanPage.scanningThread 
                    && true == winScanPage.scanningThread.IsAlive )
            {
                dll.CancelScanning();
                while ( true == winScanPage.scanningThread.IsAlive )
                {
                    // TODO: This statement will block UI thread. 
                    System.Threading.Thread.Sleep(100);
                }
            }

            if ( 0 < winScanPage.image_wrappanel.Children.Count )
            {
                if ( VOP.Controls.MessageBoxExResult.Yes != 
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo, this, "VOP关闭后，扫描图片将会被删除，是否关闭，请确认。", "提示") )
                {
                    SetTabItemFromIndex( EnumSubPage.Scan );
                    bAllowExit = false;
                }
            }

            if ( bAllowExit )
            {
                bExit = true;
                bExitUpdater = true;
                m_updaterAndUIEvent.WaitOne();
                notifyIcon1.Visible = false;
                this.Close();
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

           if (msg == App.WM_STATUS_UPDATE )
           {
               byte toner         = 0;
               EnumStatus status  = EnumStatus.Offline;
               EnumMachineJob job = EnumMachineJob.UnknowJob;

               lock ( statusLock )
               {
                   toner  = _toner ;
                   status = (EnumStatus)_status;
                   job    = (EnumMachineJob)_job   ;
               }

               statusPanelPage.UpdateStatusPanel( status, job, toner );

               if(true == m_bLocationIsChina)
               {
                   if (false == m_isShowedMaintainWindow)
                   {
                       if ((status >= EnumStatus.PolygomotorOnTimeoutError && status <= EnumStatus.CTL_PRREQ_NSignalNoCome) 
                               || status == EnumStatus.ScanMotorError 
                               || status == EnumStatus.ScanDriverCalibrationFail 
                               || status == EnumStatus.NetWirelessDongleCfgFail)
                       {
                           m_isShowedMaintainWindow = true;
                           MaintainWindow mw = new MaintainWindow();
                           mw.Owner = App.Current.MainWindow;
                           mw.ShowDialog();
                       }
                   }
               }

               if ( false == m_isOnlineDetected )
               {
                   bool bIsOnline = !( EnumStatus.Offline == status 
                           || EnumStatus.PowerOff == status 
                           || EnumStatus.Unknown == status );

                   if ( true == bIsOnline )
                   {
                       m_isOnlineDetected = true;
                       ExpandSubpage();
                   }
               }

               App.g_autoMachine.TranferState(job);
               App.g_autoMachine.TranferState(status);


               if ( EnumStatus.NofeedJam               == status
                       || EnumStatus.JamAtRegistStayOn == status
                       || EnumStatus.JamAtExitNotReach == status
                       || EnumStatus.JamAtExitStayOn   == status )
               {

                   switch ( status )
                   {
                       case EnumStatus.NofeedJam            : m_animationUri = "pack://siteoforigin:,,,/../../Media/JamAtExitNotReach.mp4"; break;
                       case EnumStatus.JamAtRegistStayOn    : m_animationUri = "pack://siteoforigin:,,,/../../Media/JamAtExitStayOn.mp4"  ; break;
                       case EnumStatus.JamAtExitNotReach    : m_animationUri = "pack://siteoforigin:,,,/../../Media/JamAtRegistStayOn.mp4"; break;
                       case EnumStatus.JamAtExitStayOn      : m_animationUri = "pack://siteoforigin:,,,/../../Media/NofeedJam.mp4"        ; break;
                       default: 
                                                              m_animationUri = "pack://siteoforigin:,,,/../../Media/NofeedJam.mp4"        ; break;
                   }

                   // TODO: Update animation Uri.
                   if ( false == m_isAnimationPopup )
                   {
                       m_isCloseAnimation = false;  
                       m_isAnimationPopup = true;
                       MessageBoxEx_Video win = new MessageBoxEx_Video( new Uri( m_animationUri ), "", "" );
                       win.m_MainWin = this;
                       win.Owner = this;
                       win.ShowDialog();
                       m_isAnimationPopup = false;
                   }
               }
               else
               {
                   m_isCloseAnimation = true;  
               }

           }
           else if (msg == App.WM_VOP)
           {
               PopupWindow();
           }

            return IntPtr.Zero;
        }



        /// <summary>
        /// If the DeviceStatus.xml file exists, then read the file to simulate the status of device,
        /// or read the device status information
        /// </summary>
        /// <param name="printername"></param>
        /// <param name="status"></param>
        /// <param name="toner"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public  bool GetPrinterStatusEx(
                String printername,
                ref byte status,
                ref byte toner,
                ref byte job)
        {
            string deviceStatus  = "";
            string machineJob    = "";
            string tonerCapacity = "";

            bool bIsOK = false;

            // simulate Device status Info.
            if (StatusXmlHelper.GetPrinterInfo(statusPanelPage.m_selectedPrinter, out deviceStatus, out machineJob, out tonerCapacity, "DeviceStatus.xml"))
            {
                toner  = (byte)(int)double.Parse(tonerCapacity);
                status = (byte)StatusXmlHelper.GetStatusTypeFormString(deviceStatus);
                job    = (byte)StatusXmlHelper.GetJobTypeFormString(machineJob);

                bIsOK = true;
            }
            else 
            {
                bIsOK = dll.GetPrinterStatus(statusPanelPage.m_selectedPrinter, ref status, ref toner, ref job);
            }

            return bIsOK;
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

            winCopyPage.ResetToDefaultValue();
            winScanPage.ResetToDefaultValue();

            App.g_autoMachine.ResetAutoMachine();

            byte toner  = 0;
            byte status = (byte)EnumStatus.Offline; 
            byte job    = (byte)EnumMachineJob.UnknowJob;
            if (false == GetPrinterStatusEx(statusPanelPage.m_selectedPrinter, ref status, ref toner, ref job))
            {
                toner  = 0;
                status = (byte)EnumStatus.Offline; 
                job    = (byte)EnumMachineJob.UnknowJob;
            }

            statusPanelPage.UpdateStatusPanel( (EnumStatus)status, (EnumMachineJob)job, toner );

            bool bIsOnline = !( (byte)EnumStatus.Offline == status 
                    || (byte)EnumStatus.PowerOff == status 
                    || (byte)EnumStatus.Unknown == status );

            if ( m_isOnlineDetected || bIsOnline )
            {
                ExpandSubpage();
                m_isOnlineDetected = true;
            }

            // After UI already loaded, tranfer auto machine. 
            App.g_autoMachine.TranferState( (EnumMachineJob)job );
            App.g_autoMachine.TranferState( (EnumStatus)status );

            statusUpdater = new Thread(UpdateStatusCaller);
            statusUpdater.Start();
        }

        /// <summary>
        /// Switch to print page.
        /// </summary>
        /// <param name="files"> file list need to preview in print page.</param>
        public void SwitchToPrintingPage( List<string> files )
        {
            SetTabItemFromIndex( EnumSubPage.Print );

            winPrintPage.FilePaths = files;
            winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintImages;
            subPageView.Child = winPrintPage;
        }
        
        /// <summary>
        /// Expand other subpage according the model type ( 3in1 or SFP )
        /// </summary>
        private void ExpandSubpage()
        {
            line3.Visibility = Visibility.Visible;
            line8.Visibility = Visibility.Visible;
            Print_Grid.Visibility = Visibility.Visible;
            tabItem_Print.Visibility = Visibility.Visible;
            SetTabItemFromIndex( EnumSubPage.Print );

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
            winSettingPage.m_bOnlyDispalyAboutView = false;
        }

        private void ShowAboutPageOnly()
        {
            SetTabItemFromIndex( EnumSubPage.Setting );

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
            winSettingPage.m_bOnlyDispalyAboutView = true;
        }

        public enum EnumSubPage
        {
            Print   ,
            Copy    ,
            Scan    ,
            Setting ,
        }

        private void PopupWindow()
        {
            this.Visibility = Visibility.Visible;
            this.Activate();
            this.Topmost = true;  // important
            this.Topmost = false; // important
            this.Focus();         // important
        }
    }
}
