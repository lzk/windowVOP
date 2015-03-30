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

namespace VOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///    

    public partial class MainWindow : Window
    {
        public static string selectedprinter = null; 

        CopyPage    winCopyPage   = new CopyPage   ();
        public FileSelectionPage winFileSelectionPage = null;
        public PrintPage   winPrintPage  = new PrintPage  ();
        ScanPage    winScanPage   = new ScanPage   ();
        SettingPage winSettingPage= new SettingPage();
        StatusPanel statusPanelPage = new StatusPanel();

        private ImageBrush imgBk_Brush_1 = null;
        private ImageBrush imgBk_Brush_2 = null;

        public static PrinterInfo[] printerInfos = 
        {
            new PrinterInfo("Lenovo ABC M001"   , false , false ) ,
            new PrinterInfo("Lenovo ABC M001 w" , false , true  ) ,
            new PrinterInfo("Lenovo ABC P001"   , true  , false ) ,
            new PrinterInfo("Lenovo ABC P001 w" , true  , true  ) ,
        };

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


        public static bool IsSFPPrinter(
                string strDrvName       // Name of printer driver
                )
        {
            bool bResult = false;

            foreach (PrinterInfo el in printerInfos)
            {
                if (el.m_name == strDrvName)
                {
                    bResult = el.m_isSFP;
                    break;
                }
            }

            return bResult;
        }

        public static bool IsSupportWifi(
        string strDrvName       // Name of printer driver
        )
        {
            return true;

            bool bResult = false;

            foreach (PrinterInfo el in printerInfos)
            {
                if (el.m_name == strDrvName)
                {
                    bResult = el.m_isWiFi;
                    break;
                }
            }

            return bResult;
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
            this.Close();
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
                    this.Close();
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
    }
}
