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

namespace VOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///    
    public class PrinterInfo
    {
        public string m_name;   // Name of the printer driver
        public bool m_isSFP;    // true for SFP printer
        public bool m_isWiFi;   // true for wifi printer 

        public PrinterInfo(
                string strDrvName,
                bool isSFP,
                bool isWiFi 
                )
        {
            m_name   = strDrvName; 
            m_isSFP  = isSFP;      
            m_isWiFi = isWiFi;     
        }
    }

    public partial class MainWindow : Window
    {
        public static string selectedprinter = null; 

        CopyPage    winCopyPage   = new CopyPage   ();
        public FileSelectionPage winFileSelectionPage = null;
        public PrintPage   winPrintPage  = new PrintPage  ();
        ScanPage    winScanPage   = new ScanPage   ();
        SettingPage winSettingPage= new SettingPage();
        StatusPanel statusPanelPage = new StatusPanel();

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

            get_printer_list(list_printers);

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

        // return default printer name
        private string get_printer_list(List<string> list_printers)
        {
            string strDefPrinter = null;

            list_printers.Clear();

            try
            {
                // If spooler was stopped, new PrintServer( null ) will throw
                // a exception.
                PrintServer myPrintServer = new PrintServer(null);
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    PrintDriver queuedrv = pq.QueueDriver;
                    if (IsSupportPrinter(queuedrv.Name) == true &&
                            EnumPortType.PT_UNKNOWN != (EnumPortType)dll.CheckPortAPI(pq.Name))
                        list_printers.Add(pq.Name);
                }

                PrinterSettings settings = new PrinterSettings();
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    settings.PrinterName = printer;

                    //Add by KevinYin for BMS Bug 56108 begin
                    bool isSupportPrinter = false;
                    foreach (string strPrinterName in list_printers)
                    {
                        if (printer == strPrinterName)
                        {
                            isSupportPrinter = true;
                        }
                    }
                    //Add by KevinYin for BMS Bug 56108 end

                    if (settings.IsDefaultPrinter)
                    {
                        if (isSupportPrinter)
                        {
                            strDefPrinter = printer;
                        }

                        break;
                    }
                }
            }
            catch
            {
                strDefPrinter = null;
            }

            return strDefPrinter;
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

        public static bool IsSupportPrinter(
                string strDrvName       // Name of printer driver
                )
        {
            bool isSupport = false;

            foreach (PrinterInfo el in printerInfos)
            {
                if (el.m_name == strDrvName)
                {
                    isSupport = true;
                    break;
                }
            }

            return isSupport;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        public void LoadedMainWindow( object sender, RoutedEventArgs e )
        {
            winFileSelectionPage = new FileSelectionPage(this);

            statusPageView.Child = statusPanelPage;  

            setTabItemFromIndex(0);

            this.subPageView.Child = winFileSelectionPage;      // for test  
            this.statusPanelPage.Opacity = 0.0; 
        }

        public void MyMouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if ( position.Y < 40 && position.Y > 0 )
                this.DragMove();
        }

        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if ( null != btn )
            {
                if ( "btnClose" == btn.Name )
                {
                    this.Close();
                }
            }
        }

        private void NvgBtnClick(object sender, RoutedEventArgs e)
        {
            Control btn = sender as Control;

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
        private bool setTabItemFromName(string tabItemName)
        {
            if (0 == String.Compare(tabItemName, "printer", true))
            {
                tabItem_Printer.IsSelect = true;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;    
            }
            else if (0 == String.Compare(tabItemName, "copy", true))
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = true;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (0 == String.Compare(tabItemName, "scan", true))
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = true;
                tabItem_Setting.IsSelect = false;
            }
            else if (0 == String.Compare(tabItemName, "setting", true))
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = true;
            } 
            else
            {
                return false;
            }

            return true;
        }
        private bool setTabItemFromIndex(int index)
        {
            if (0 == index)
            {
                this.subPageView.Child = winPrintPage;
                this.statusPanelPage.Opacity = 1.0;

                tabItem_Printer.IsSelect = true;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (1 == index)
            {
                this.subPageView.Child = winCopyPage;
                this.statusPanelPage.Opacity = 1.0;

                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = true;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (2 == index)
            {
                this.subPageView.Child = winScanPage;
                this.statusPanelPage.Opacity = 1.0;

                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = true;
                tabItem_Setting.IsSelect = false;
            }
            else if (3 == index)
            {
                this.subPageView.Child = winSettingPage;
                this.statusPanelPage.Opacity = 0.0;

                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = true;
            }
            else
            {
                return false;
            }

            return true;
        }


        #endregion // end Set_TabItemIndex

        private void btnLogin_btnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
