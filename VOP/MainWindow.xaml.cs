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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///    

    public enum PrinterModel
    {
        SFP,
        MFP
    }

    public class UploadPrintInfo
    {
        public bool m_bUploadSuccess = false;
        public CRM_PrintInfo m_PrintInfo = new CRM_PrintInfo();
    }

    public partial class MainWindow : Window
    {
        public bool m_popupIDCard = true; // True if ID Card Copy confirm dialog need to pop up.
        public bool m_popupNIn1   = true; // True if N in 1 Copy confirm dialog need to pop up.
        public bool m_crmAgreement = false;
        public bool m_crmAgreementDialogShowed = false;
        public bool m_isMainWinLoaded = false;


        private EnumSubPage m_currentPage = EnumSubPage.Print;

        private bool bGrayIcon = false; // True if the tray icon was set to gray.

        public static RequestManager m_RequestManager = new RequestManager();

        public FileSelectionPage   winFileSelectionPage   = new FileSelectionPage();
        public PrintPage           winPrintPage           = new PrintPage();
        public StatusPanel         statusPanelPage        = new StatusPanel();
        public TroubleshootingPage winTroubleshootingPage = new TroubleshootingPage();

        private CopyPage    winCopyPage    = new CopyPage   ();
        private ScanPage    winScanPage    = new ScanPage   ();
        private SettingPage winSettingPage = new SettingPage();

        /// <summary>
        /// Thread used to update status of current printer.
        /// </summary>
        private Thread statusUpdater = null;
        private Thread uploadCRMThread = null;
        public Thread m_thdMerchantInfoFromServer = null;
        public ManualResetEvent m_InitMerchantInfoEvent = new ManualResetEvent(false);
        public Thread m_thdMaintainInfoFromServer = null;
        public ManualResetEvent m_InitMaintainInfoEvent = new ManualResetEvent(false);  

        /// <summary>
        /// Event used to sync between status update thread and main UI.
        /// </summary>
        private ManualResetEvent m_updaterAndUIEvent = new ManualResetEvent(true);

        private bool m_isOnlineDetected = false;    // true is one online printer has been seleted.

        public string m_strPassword = "";
        public static string m_strPhoneNumber = "";
        public static bool m_bLogon = false;
        public static bool   m_bLocationIsChina = false;
        public static byte   m_byWifiInitStatus = 0;
        public static List<UploadPrintInfo> m_UploadPrintInfoSet = new List<UploadPrintInfo>();

        public  bool m_isCloseAnimation = false;  // True if animation window need to close.

        // Old status used to popup paper jam animation window.
        private EnumStatus m_oldStatus = EnumStatus.Offline;

        public static MerchantInfoSet m_MerchantInfoSet = new MerchantInfoSet();
        public static MaintainInfoSet m_MaintainSet = new MaintainInfoSet();

		private Thread thread_PrinterInfo2 = null;
        //public Thread thread_InitHostname = null;

        public bool PasswordCorrect(Window parent)
        {
            bool bCorrect = false;
            if (m_strPassword.Length > 0)
            {
                string strPrinterName = statusPanelPage.m_selectedPrinter;
                PasswordRecord m_rec = new PasswordRecord(strPrinterName, m_strPassword);
                AsyncWorker worker = new AsyncWorker(parent);

                if (worker.InvokeMethod<PasswordRecord>(strPrinterName, ref m_rec, DllMethodType.ConfirmPassword, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        bCorrect = true;
                    }
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

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            const int GEOCLASS_NATION = 16;
            int nGeoID = Win32.GetUserGeoID(GEOCLASS_NATION);
            if (45 == nGeoID)
            {
                m_bLocationIsChina = true;
            }

            statusPanelPage.eventPrinterSwitch += PrinterSwitch;
            statusPanelPage.eventErrorMarkerClick += ErrorMarkerClick;

            winCopyPage    .m_MainWin = this;
            winScanPage    .m_MainWin = this;
            winSettingPage .m_MainWin = this;
            winFileSelectionPage.m_MainWin = this;
            winPrintPage.m_MainWin = this;
            winTroubleshootingPage.m_MainWin = this;
            Init();

            btnLogin.Visibility = m_bLocationIsChina ? Visibility.Visible : Visibility.Hidden;

            if (0x804 == App.LangId)
            {
                TextElement.FontFamilyProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(new FontFamily("微软雅黑,SimSun")));
                TextBlock.FontFamilyProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(new FontFamily("微软雅黑,SimSun")));            
            }
            else
            {
                TextElement.FontFamilyProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(new FontFamily("Arial")));
                TextBlock.FontFamilyProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(new FontFamily("Arial")));
            }
           
            if (true == m_bLocationIsChina)
            {
                string strUserName = "";
                string strPassword = "";
                if (true == ReadUserInfoFromXamlFile(ref strUserName, ref strPassword))
                {
                    JSONResultFormat1 js = new JSONResultFormat1();
                    if (true == m_RequestManager.CheckVerifyCode(strUserName, strPassword, ref js))
                    {
                        m_strPhoneNumber = strUserName;
                        m_bLogon = btnLogin.IsLogon = true;
                        btnLogin.bottomText = strUserName;
                    }
                }

                uploadCRMThread = new Thread(UploadCRM_LocalInfoToServerCaller);
                uploadCRMThread.Start();

                m_thdMerchantInfoFromServer = new Thread(GetMerchantInfoFromServerProc);
                m_thdMerchantInfoFromServer.Priority = ThreadPriority.Highest;
                m_thdMerchantInfoFromServer.Start();

                m_thdMaintainInfoFromServer = new Thread(GetMaintainInfoFromServerProc);
                m_thdMaintainInfoFromServer.Priority = ThreadPriority.Highest;
                m_thdMaintainInfoFromServer.Start();
            }

            Thread uploadPrintInfoThread = new Thread(UploadPrintInfoToServerCaller);
            uploadPrintInfoThread.Start();

            this.SourceInitialized += new EventHandler(win_SourceInitialized);  
        }

        public void InitHostname()
        {
            string strPrinterName = statusPanelPage.m_selectedPrinter;
            StringBuilder ipAddress = new StringBuilder(100);
            Trace.WriteLine("InitHostname().");
            dll.CheckPortAPI2(strPrinterName, ipAddress);
        }

        public void UploadPrinterInfo2()
        {

            if (m_bLocationIsChina == false || m_crmAgreement == false)
                return;


            string strPrinterModel = "";
            string strDrvName = "";
            string strPrinterName = statusPanelPage.m_selectedPrinter;

            CRM_PrintInfo2 info = new CRM_PrintInfo2();
            AsyncWorker worker = new AsyncWorker();

            UserCenterInfoRecord rec = worker.GetUserCenterInfo(statusPanelPage.m_selectedPrinter);

            dll.OutputDebugStringToFile_(string.Format("GetUserCenterInfo {0}\r\n", rec.CmdResult));

            if (rec.CmdResult == EnumCmdResult._ACK)
            {
                common.GetPrinterDrvName(strPrinterName, ref strDrvName);

                bool isSFP = common.IsSFPPrinter(strDrvName);
                bool isWiFiModel = common.IsSupportWifi(strDrvName);

                if (isSFP)
                {
                    if (isWiFiModel)
                        strPrinterModel = "Lenovo LJ2208W";
                    else
                        strPrinterModel = "Lenovo LJ2208";
                }
                else
                {
                    if (isWiFiModel)
                        strPrinterModel = "Lenovo M7208W";
                    else
                        strPrinterModel = "Lenovo M7208";
                }

                info.AddPrintData(strPrinterModel, rec.SecondSerialNO, rec.SerialNO4AIO, rec.TotalCounter);

                //Serialization to xml file, locate in system temp VOP_CRM folder
                string fileName = statusPanelPage.m_selectedPrinter + ".xml";
                RequestManager.Serialize<CRM_PrintInfo2>(info, App.crmFolder + @"\" + fileName);

                //Process crm_p = null;
                //if (App.CheckProcessExist("CRMUploader", ref crm_p) == false)
                //{
                //    ProcessStartInfo procInfo = new ProcessStartInfo();
                //    procInfo.FileName = @".\CRMUploader.exe";

                //    procInfo.CreateNoWindow = true;
                //    procInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //    procInfo.UseShellExecute = false;

                //    try
                //    {
                //        Process p = Process.Start(procInfo);
                //    }
                //    catch (Exception)
                //    {

                //    }
                //}


                //CRM_PrintInfo2 info2 = RequestManager.Deserialize<CRM_PrintInfo2>(App.crmFolder + @"\" + fileName);
                //JSONResultFormat2 rtValue = new JSONResultFormat2();
                //m_RequestManager.UploadCRM_PrintInfo2ToServer(ref info, ref rtValue);
            }
        }

        public void GetMaintainInfoFromServerProc()
        {
            VOP.MainWindow.m_MaintainSet.Clear();

            DateTime dtSaveTime = new DateTime();
            string strMerchantInfo = "";
            string strMaintainInfo = "";
            if ((false == VOP.MainWindow.ReadCRMDataFromXamlFile("Maintain.xaml", ref dtSaveTime, ref strMaintainInfo)) || strMaintainInfo.Length < 100)
            {
                VOP.MainWindow.ReadInfoDataFromXamlFile(ref strMerchantInfo, ref strMaintainInfo);
                VOP.MainWindow.SaveCRMDataIntoXamlFile("Maintain.xaml", DateTime.Now, strMaintainInfo);
            }
            VOP.MainWindow.m_RequestManager.ParseJsonData<MaintainInfoSet>(strMaintainInfo, JSONReturnFormat.MaintainInfoSet, ref VOP.MainWindow.m_MaintainSet);

            m_InitMaintainInfoEvent.Set();

            MerchantInfoSet merchantInfoSet = new MerchantInfoSet();
            MaintainInfoSet maintainSet = new MaintainInfoSet();

            string strResult = "";
            if (true == VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, 5, ref maintainSet, ref strResult))
            {
                int nTotalCount = maintainSet.m_nTotalCount;
                maintainSet.Clear();

                if (true == VOP.MainWindow.m_RequestManager.GetMaintainInfoSet(0, nTotalCount, ref maintainSet, ref strResult))
                {
                    VOP.MainWindow.m_MaintainSet = maintainSet;
                    VOP.MainWindow.SaveCRMDataIntoXamlFile("Maintain.xaml", DateTime.Now, strResult);
                }
            }
        }

        public void GetMerchantInfoFromServerProc()
        {
            VOP.MainWindow.m_MerchantInfoSet.Clear();

            DateTime dtSaveTime = new DateTime();
            string strMerchantInfo = "";
            string strMaintainInfo = "";
            if (false == VOP.MainWindow.ReadCRMDataFromXamlFile("Merchant.xaml", ref dtSaveTime, ref strMerchantInfo) || strMerchantInfo.Length < 100)
            {
                VOP.MainWindow.ReadInfoDataFromXamlFile(ref strMerchantInfo, ref strMaintainInfo);
                VOP.MainWindow.SaveCRMDataIntoXamlFile("Merchant.xaml", DateTime.Now, strMerchantInfo);
            }

            VOP.MainWindow.m_RequestManager.ParseJsonData<MerchantInfoSet>(strMerchantInfo, JSONReturnFormat.MerchantInfoSet, ref VOP.MainWindow.m_MerchantInfoSet);

            m_InitMerchantInfoEvent.Set();

            MerchantInfoSet merchantInfoSet = new MerchantInfoSet();
            MaintainInfoSet maintainSet = new MaintainInfoSet();

            string strResult = "";
            if (true == VOP.MainWindow.m_RequestManager.GetMerchantSet(0, 5, ref merchantInfoSet, ref strResult))
            {
                int nTotalCount = merchantInfoSet.m_nTotalCount;
                merchantInfoSet.Clear();

                if (true == VOP.MainWindow.m_RequestManager.GetMerchantSet(0, nTotalCount, ref merchantInfoSet, ref strResult))
                {
                    VOP.MainWindow.m_MerchantInfoSet = merchantInfoSet;
                    VOP.MainWindow.SaveCRMDataIntoXamlFile("Merchant.xaml", DateTime.Now, strResult);
                }
            }
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
                        MainWindow _this = System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual as MainWindow;

                        if(null != _this)
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

        public XmlAttribute CreateAttribute(XmlNode node, string attributeName, string value)
        {
            try
            {
                XmlDocument doc = node.OwnerDocument;
                XmlAttribute attr = null;
                attr = doc.CreateAttribute(attributeName);
                attr.Value = value;
                node.Attributes.SetNamedItem(attr);
                return attr;
            }
            catch (Exception err)
            {
                string desc = err.Message;
                return null;
            }
        } 
        
        public bool SavePrintInfoIntoXamlFile()
        {
            bool bSuccess = false;
            try
            {
                string path = App.cfgFolder + "PrintInfo.xaml";

                XmlDocument doc = new XmlDocument();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement eleRoot = doc.CreateElement(string.Empty, "printinfoset", string.Empty);
                doc.AppendChild(eleRoot);
                lock (printinfoLock)
                {
                    foreach (UploadPrintInfo item in m_UploadPrintInfoSet)
                    {
                        if (false == item.m_bUploadSuccess)
                        {
                            XmlNode node = doc.CreateElement("printinfo");
                            node.Attributes.Append(CreateAttribute(node, "MobileCode", item.m_PrintInfo.m_strMobileCode));
                            node.Attributes.Append(CreateAttribute(node, "Mobile", item.m_PrintInfo.m_strMobileNumber));
                            node.Attributes.Append(CreateAttribute(node, "DeviceBrand", item.m_PrintInfo.m_strDeviceBrand));
                            node.Attributes.Append(CreateAttribute(node, "DeviceModel", item.m_PrintInfo.m_strDeviceModel));
                            node.Attributes.Append(CreateAttribute(node, "PrintType", item.m_PrintInfo.m_strPrintType));

                            node.Attributes.Append(CreateAttribute(node, "PrintMode", item.m_PrintInfo.m_strPrintMode));
                            node.Attributes.Append(CreateAttribute(node, "PrintDocType", item.m_PrintInfo.m_strPrintDocType));
                            node.Attributes.Append(CreateAttribute(node, "PrintCopys", item.m_PrintInfo.m_strPrintCopys));
                            node.Attributes.Append(CreateAttribute(node, "PrintPages", item.m_PrintInfo.m_strPrintPages));
                            node.Attributes.Append(CreateAttribute(node, "PrinterModel", item.m_PrintInfo.m_strPrinterModel));

                            node.Attributes.Append(CreateAttribute(node, "PrinterName", item.m_PrintInfo.m_strPrinterName));
                            node.Attributes.Append(CreateAttribute(node, "PrinterType", item.m_PrintInfo.m_strPrinterType));
                            node.Attributes.Append(CreateAttribute(node, "IsSuccess", item.m_PrintInfo.m_strPrintSuccess));
                            node.Attributes.Append(CreateAttribute(node, "version", item.m_PrintInfo.m_strVersion));
                            node.Attributes.Append(CreateAttribute(node, "flag", item.m_PrintInfo.m_strFlag));

                            eleRoot.AppendChild(node);
                        }
                    }
                }

                doc.Save(path);
                bSuccess = true;
            }
            catch
            {

            }
            return bSuccess;
        }

        public bool ReadPrintInfoIntoXamlFile()
        {
            bool bSuccess = false;
            try
            {
                string path = App.cfgFolder + "PrintInfo.xaml";
                XDocument doc = XDocument.Load(path);

                var query = (from item in doc.Element("printinfoset").Elements() select item).ToList();
                UploadPrintInfo upi = new UploadPrintInfo();
                foreach (var item in query)
                {
                    upi.m_PrintInfo.m_strMobileCode = (item.Attribute("MobileCode") == null ? (string)null : item.Attribute("MobileCode").Value);
                    upi.m_PrintInfo.m_strMobileNumber = (item.Attribute("Mobile") == null ? (string)null : item.Attribute("Mobile").Value);
                    upi.m_PrintInfo.m_strDeviceBrand = (item.Attribute("DeviceBrand") == null ? (string)null : item.Attribute("DeviceBrand").Value);
                    upi.m_PrintInfo.m_strDeviceModel = (item.Attribute("DeviceModel") == null ? (string)null : item.Attribute("DeviceModel").Value);
                    upi.m_PrintInfo.m_strPrintType = (item.Attribute("PrintType") == null ? (string)null : item.Attribute("PrintType").Value);
                    upi.m_PrintInfo.m_strPrintMode = (item.Attribute("PrintMode") == null ? (string)null : item.Attribute("PrintMode").Value);
                    upi.m_PrintInfo.m_strPrintDocType = (item.Attribute("PrintDocType") == null ? (string)null : item.Attribute("PrintDocType").Value);
                    upi.m_PrintInfo.m_strPrintCopys = (item.Attribute("PrintCopys") == null ? (string)null : item.Attribute("PrintCopys").Value);
                    upi.m_PrintInfo.m_strPrintPages = (item.Attribute("PrintPages") == null ? (string)null : item.Attribute("PrintPages").Value);
                    upi.m_PrintInfo.m_strPrinterModel = (item.Attribute("PrinterModel") == null ? (string)null : item.Attribute("PrinterModel").Value);
                    upi.m_PrintInfo.m_strPrinterName = (item.Attribute("PrinterName") == null ? (string)null : item.Attribute("PrinterName").Value);
                    upi.m_PrintInfo.m_strPrinterType = (item.Attribute("PrinterType") == null ? (string)null : item.Attribute("PrinterType").Value);
                    upi.m_PrintInfo.m_strPrintSuccess = (item.Attribute("IsSuccess") == null ? (string)null : item.Attribute("IsSuccess").Value);
                    upi.m_PrintInfo.m_strVersion = (item.Attribute("version") == null ? (string)null : item.Attribute("version").Value);
                    upi.m_PrintInfo.m_strFlag = (item.Attribute("flag") == null ? (string)null : item.Attribute("flag").Value);
                    try
                    {
                        upi.m_PrintInfo.m_time = DateTime.Parse((item.Attribute("time") == null ? (string)null : item.Attribute("time").Value));
                    }
                    catch
                    {

                    }
                   
                    lock (printinfoLock)
                    {
                        m_UploadPrintInfoSet.Add(upi);
                    }
                }
            }
            catch
            {

            }
            return bSuccess;
        }

        public void UploadPrintInfoToServerCaller()
        {

            try
            {
                ReadPrintInfoIntoXamlFile();

                while (!bExit)
                {
                    if (m_bLocationIsChina == true && m_crmAgreement == true)
                    {
                        SessionInfo session = new SessionInfo();
                        if (true == m_RequestManager.GetSession(ref session))
                        {//if network is ok
                            lock (printinfoLock)
                            {
                                for (int i = m_UploadPrintInfoSet.Count - 1; i >= 0; i--)
                                {
                                    if (m_UploadPrintInfoSet[i].m_bUploadSuccess)
                                        m_UploadPrintInfoSet.Remove(m_UploadPrintInfoSet[i]);
                                }
                            }


                            for (int i = 0; i < m_UploadPrintInfoSet.Count; i++)
                            {
                                JSONResultFormat2 rtValue = new JSONResultFormat2();
                                m_UploadPrintInfoSet[i].m_bUploadSuccess = m_RequestManager.UploadCRM_PrintInfoToServer(ref m_UploadPrintInfoSet[i].m_PrintInfo, ref rtValue);
                                if (!m_UploadPrintInfoSet[i].m_bUploadSuccess)
                                {
                                    if (rtValue.m_strMessage == "flag重复")
                                        m_UploadPrintInfoSet[i].m_bUploadSuccess = true;
                                }
                            }
                        }

                        SavePrintInfoIntoXamlFile();
                    }

                    for (int i = 0; i < 500; i++)
                    {
                        if (bExit)
                            break;

                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
            catch
            {

            }
        }

        public void UploadPrintInfo(CRM_PrintInfo printInfo)
        {

            if (m_bLocationIsChina == false || m_crmAgreement == false)
                return;

            Thread thread = new Thread(() =>
            {
                try
                {
                    UploadPrintInfo upi = new UploadPrintInfo();
                    upi.m_PrintInfo = printInfo;

                    JSONResultFormat2 rtValue = new JSONResultFormat2();
                    upi.m_bUploadSuccess = m_RequestManager.UploadCRM_PrintInfoToServer(ref upi.m_PrintInfo, ref rtValue);
                    if (!upi.m_bUploadSuccess)
                    {
                        if (rtValue.m_strMessage == "flag重复")
                            upi.m_bUploadSuccess = true;
                    }

                    if (false == upi.m_bUploadSuccess)
                    {
                        lock (printinfoLock)
                        {
                            m_UploadPrintInfoSet.Add(upi);
                        }
                    }
                }
                catch
                {

                }
            });

            thread.Start();
        }

        public static bool SaveInfoDataIntoXamlFile(string strMerInfo, string strMaintainInfo)
        {
            bool bSuccess = false;
            try
            {
                XmlDocument doc = new XmlDocument();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement eleBody = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(eleBody);

                XmlElement eleTime = doc.CreateElement(string.Empty, "MerInfo", string.Empty);
                string str = Encrypt.EncryptDES(strMerInfo, "des12345");
                XmlText textTime = doc.CreateTextNode(str);
                eleTime.AppendChild(textTime);
                eleBody.AppendChild(eleTime);

                XmlElement eleData = doc.CreateElement(string.Empty, "MaintainInfo", string.Empty);
                str = Encrypt.EncryptDES(strMaintainInfo, "des12345");
                XmlText textData = doc.CreateTextNode(str);
                eleData.AppendChild(textData);
                eleBody.AppendChild(eleData);

                doc.Save( App.cfgFolder + "Data.xaml");

                bSuccess = true;
            }
            catch
            {

            }

            return bSuccess;
        }

        public static bool ReadInfoDataFromXamlFile(ref string strMerInfo, ref string strMaintainInfo)
        {
            bool bSuccess = false;

            try
            {
                string documentsPath = App.currentFolder;
                string strPath = documentsPath + "\\InfoData\\Data.xaml";

                if (File.Exists(strPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(strPath);
                    XmlNode xmlNode = doc.SelectSingleNode("body/MerInfo");
                    strMerInfo = Encrypt.DecryptDES(xmlNode.InnerText, "des12345");
                    XmlNode xmlNode2 = doc.SelectSingleNode("body/MaintainInfo");
                    strMaintainInfo = Encrypt.DecryptDES(xmlNode2.InnerText, "des12345");

                    bSuccess = true;
                }
            }
            catch
            {

            }

            return bSuccess;
        }

        public static bool SaveUserInfoIntoXamlFile(string strUserName, string strPassword)
        {
            bool bSuccess = false;
            try
            {
                XmlDocument doc = new XmlDocument();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement eleBody = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(eleBody);

                XmlElement eleTime = doc.CreateElement(string.Empty, "UserName", string.Empty);
                XmlText textTime = doc.CreateTextNode(strUserName);
                eleTime.AppendChild(textTime);
                eleBody.AppendChild(eleTime);

                XmlElement eleData = doc.CreateElement(string.Empty, "Password", string.Empty);
                XmlText textData = doc.CreateTextNode(strPassword);
                eleData.AppendChild(textData);
                eleBody.AppendChild(eleData);

                doc.Save( App.cfgFolder + "UserInfo.xaml");

                bSuccess = true;
            }
            catch
            {

            }

            return bSuccess;
        }

        public static bool ReadUserInfoFromXamlFile(ref string strUserName, ref string strPassword)
        {
            bool bSuccess = false;

            try
            {
                string strPath = App.cfgFolder + "UserInfo.xaml";

                if (File.Exists(strPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(strPath);

                    XmlNode xmlNode = doc.SelectSingleNode("body/UserName");
                    strUserName = xmlNode.InnerText;
                    XmlNode xmlNode2 = doc.SelectSingleNode("body/Password");
                    strPassword = xmlNode2.InnerText;

                    bSuccess = true;
                }
            }
            catch
            {

            }

            return bSuccess;
        }

        public static bool SaveCRMDataIntoXamlFile(string strFileName, DateTime date, string strData)
        {
            bool bSuccess = false;
            try
            {
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

                doc.Save( App.cfgFolder + strFileName);

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
                string strPath = App.cfgFolder + strFileName;

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
                if (m_bLocationIsChina == true && m_crmAgreement == true)
                {
                    if (true == UploadCRM_LocalInfoToServer())
                        break;
                }
             
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

            notifyIcon1.Text = (string)this.FindResource("ResStr_Lenovo_Printer");
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

        public void LoadedMainWindow( object sender, RoutedEventArgs e )
        {

            this.Visibility = System.Windows.Visibility.Hidden;
            dll.SaveDefaultPrinter(); //save default printer name before vop action
            statusPageView.Child = statusPanelPage;

            this.statusPanelPage.Visibility = Visibility.Visible;          
            UpdateLED( EnumStatus.Offline );

            ShowTroubleshootingPage();

            ShowStartupWindow();

            GetPopupSetting( App.cfgFile, ref m_popupIDCard, ref m_popupNIn1 );

            this.Visibility = System.Windows.Visibility.Visible;

            if(!m_crmAgreementDialogShowed && m_bLocationIsChina)
            {
                if (SelfCloseRegistry.Open())
                {
                    string regStr = SelfCloseRegistry.GetAgreement();
                    m_crmAgreement = ("true" == regStr.ToLower());
                    SelfCloseRegistry.Close();
                }

                if (!m_crmAgreement)
                {
                    m_crmAgreement = true;
                    this.IsEnabled = false;
                    ShowCRMAgreementWindow();
                    this.IsEnabled = true;
                  
                }
                m_crmAgreementDialogShowed = true;
            }

            thread_PrinterInfo2 = new Thread(UploadPrinterInfo2);
            thread_PrinterInfo2.Start();

            //thread_InitHostname = new Thread(InitHostname);
            //thread_InitHostname.Start();

            m_isMainWinLoaded = true;

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

        private bool m_bShowLogonView = false;
        private void NvgBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Control btn = sender as System.Windows.Controls.Control;

            if ( null != btn )
            {
                if ( "btnPrint" == btn.Name || "tabItem_Print" == btn.Name )
                {                
                     SetTabItemFromIndex( EnumSubPage.Print );
                }
                else if ("btnCopy" == btn.Name || "tabItem_Copy" == btn.Name)
                {                   
                    SetTabItemFromIndex( EnumSubPage.Copy );
                }
                else if ("btnScan" == btn.Name || "tabItem_Scan" == btn.Name)
                {                    
                    SetTabItemFromIndex( EnumSubPage.Scan );
                }
                else if ( "btnSetting" == btn.Name || "tabItem_Setting" == btn.Name)
                {                
                    SetTabItemFromIndex( EnumSubPage.Setting );
                }
            }

        }

        #region Set_TabItemIndex

        /// <summary>
        /// Switch sub page. 
        /// </summary>
        private void SetTabItemFromIndex( EnumSubPage subpage )
        {       
            m_currentPage = subpage;

            if (false == statusPanelPage.m_isSFP)
            {
                if (EnumSubPage.Print == subpage)
                {
                    this.subPageView.Child = winFileSelectionPage;
                    Background_SubPageView.Source = new BitmapImage(new Uri("Images\\PagePrint.tif", UriKind.RelativeOrAbsolute));

                    tabItem_Print.IsSelect = true;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if (EnumSubPage.Copy == subpage)
                {
                    this.subPageView.Child = winCopyPage;
                    Background_SubPageView.Source = new BitmapImage(new Uri("Images\\PageCopy.tif", UriKind.RelativeOrAbsolute));

                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = true;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if (EnumSubPage.Scan == subpage)
                {
                    this.subPageView.Child = winScanPage;
                    Background_SubPageView.Source = new BitmapImage(new Uri("Images\\PageScan.tif", UriKind.RelativeOrAbsolute));

                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = true;
                    tabItem_Setting.IsSelect = false;

                    //Add by KevinYin for BMS bug(0059861) begin
                    byte toner         = 0;
                    EnumStatus status  = EnumStatus.Offline;
                    EnumMachineJob job = EnumMachineJob.UnknowJob;
 
                    lock ( statusLock )
                    {
                        toner  = _toner ;
                        status = (EnumStatus)_status;
                        job    = (EnumMachineJob)_job   ;
                    }
                    winScanPage.PassStatus(status, job, toner);
                    //Add by KevinYin for BMS bug(0059861) end
                }
                else if (EnumSubPage.Setting == subpage)
                {
                    this.subPageView.Child = winSettingPage;
                    Background_SubPageView.Source = new BitmapImage(new Uri("Images\\PageSetting.tif", UriKind.RelativeOrAbsolute));

                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = true;
                }
            }
            else
            {
                if (EnumSubPage.Print == subpage)
                {
                    this.subPageView.Child = winFileSelectionPage;
                    Background_SubPageView.Source = new BitmapImage(new Uri("Images\\PagePrint.tif", UriKind.RelativeOrAbsolute));

                    tabItem_Print.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if (EnumSubPage.Setting == subpage)
                {
                    this.subPageView.Child = winSettingPage;
                    Background_SubPageView.Source = new BitmapImage(new Uri("Images\\PageCopy.tif", UriKind.RelativeOrAbsolute));

                    tabItem_Print.IsSelect = false;
                    tabItem_Setting.IsSelect = true;
                }
            }

            tabItem_Setting.tabItemStyle = CustomTabItemStyle.Right;
        }


        #endregion // end Set_TabItemIndex


        /// <summary>
        /// Exit falg. True if need to exit thread statusUpdater.
        /// </summary>
        private bool _bExitUpdater = false;
        public bool bExitUpdater
        {
            get
            {
                return _bExitUpdater;
            }
            set
            {
                if ( true == value )
                    UpdateLED( EnumStatus.Offline );

                _bExitUpdater = value;
            }
        }

        private bool m_isShowedMaintainWindow = false;

        // NOTE: Those variable were used for post WM_STATUS_UPDATE message, do not for other usage.
        private byte _toner  = 0;
        private byte _status = (byte)EnumStatus.Offline; 
        private byte _job    = (byte)EnumMachineJob.UnknowJob;
        private object statusLock = new object(); // Use to sync status share varibles.
        private object printinfoLock = new object(); // Use to sync status share varibles.

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
                if (_status != (byte)EnumStatus.Unknown)
                {
                    Win32.PostMessage((IntPtr)0xffff, App.WM_STATUS_UPDATE, IntPtr.Zero, IntPtr.Zero);
                }
                      
                for ( int i=0; i<6; i++ )
                {
                    if ( bExitUpdater )
                        break;

                    System.Threading.Thread.Sleep(500);
                }
            }

            // When the status update thread pause, the icon turn to offline.
            bGrayIcon = true; 
            System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,, /Images/printerGray.ico")).Stream;
            notifyIcon1.Icon = new System.Drawing.Icon(iconStream);

            m_updaterAndUIEvent.Set();
        }

        /// <summary>
        /// MainWindow unit exit point.
        /// </summary>
        private void MainWindowExitPoint()
        {
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

            if (thread_PrinterInfo2 != null && thread_PrinterInfo2.IsAlive == true)
            {
                thread_PrinterInfo2.Join();
            }


            bExit = true;
            bExitUpdater = true;
            m_updaterAndUIEvent.WaitOne();
            SavePrintInfoIntoXamlFile();
            dll.RecoverDevModeData();
            notifyIcon1.Visible = false;
            dll.ResetDefaultPrinter();
            PdfPrint.CloseAll();

            SetPopupSetting( App.cfgFile, m_popupIDCard, m_popupNIn1 );
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

               UpdateLED( status );

               bool bIsNeedMaintain = common.IsStatusNeedMaintain(status);

               bool bUseGrayIcon = ( EnumStatus.Offline == status || bIsNeedMaintain || toner <= 10 );

               if ( bUseGrayIcon != bGrayIcon )
               {
                   string strIcon = bUseGrayIcon ? "pack://application:,,, /Images/printerGray.ico" : "pack://application:,,, /Images/printer.ico";
                   System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri( strIcon )).Stream;
                   notifyIcon1.Icon = new System.Drawing.Icon(iconStream);
                   bGrayIcon = bUseGrayIcon;
               }

               if(true == m_bLocationIsChina)
               {
                   if (false == m_isShowedMaintainWindow)
                   {
                       if ( bIsNeedMaintain )
                       {
                           m_isShowedMaintainWindow = true;
                           //Add by KevinYin for BMS Bug(59606) begin
                           if (null != App.Current.MainWindow && App.Current.MainWindow.Visibility == Visibility.Hidden)
                               App.Current.MainWindow.Show();
                           //Add by KevinYin for BMS Bug(59606) end

                           MaintainWindow mw = new MaintainWindow();
                           mw.Owner = App.Current.MainWindow;
                           mw.ShowDialog();
                       }
                   }
               }

               if ( false == m_isOnlineDetected || EnumSubPage.Print == m_currentPage && winTroubleshootingPage == subPageView.Child )
               {
                   if ( false == common.IsOffline( status ) )
                   {
                       statusPanelPage.UpdatePrinterProperty();
                       m_isOnlineDetected = true;                    
                       ExpandSubpage();
                   }
               }

               winCopyPage.PassStatus( status, job, toner );
               winScanPage.PassStatus( status, job, toner );
               winPrintPage.PassStatus(status, job, toner);
               winSettingPage.PassStatus(status, job, toner);

               if ( EnumStatus.NofeedJam               == status
                       || EnumStatus.InitializeJam     == status
                       || EnumStatus.JamAtRegistStayOn == status
                       || EnumStatus.JamAtExitNotReach == status
                       || EnumStatus.JamAtExitStayOn   == status )
               {
                   statusPanelPage.btnErrorMarker.Visibility = System.Windows.Visibility.Visible;
               }
               else
               {
                   statusPanelPage.btnErrorMarker.Visibility = System.Windows.Visibility.Hidden;
                   m_isCloseAnimation = true;  
               }

               m_oldStatus = status;

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
            if (StatusXmlHelper.GetPrinterInfo(statusPanelPage.m_selectedPrinter, out deviceStatus, out machineJob, out tonerCapacity, App.pathSimulationFile ))
            {
                bIsOK = true;          
              
                status = (byte)StatusXmlHelper.GetStatusTypeFormString(deviceStatus);
                job    = (byte)StatusXmlHelper.GetJobTypeFormString(machineJob);
                toner = 0; // Fixed #0059793.

                if ( (byte)EnumStatus.Offline != status  )
                {
                    double dToner = 0.0;
                    if (double.TryParse(tonerCapacity, out dToner))
                    {
                        toner = (byte)(int)dToner;
                    }
                    else
                    {
                        bIsOK = false;
                    }              
                }
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

            if (thread_PrinterInfo2 != null && thread_PrinterInfo2.IsAlive == true)
            {
                thread_PrinterInfo2.Join();
            }

            winCopyPage.ResetToDefaultValue();
            winScanPage.ResetToDefaultValue();
            winPrintPage.ResetToDefaultValue();

            dll.ResetBonjourAddr();

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
            UpdateLED( (EnumStatus)status );

            // Move ShowTroubleshootingPage from statusPanel to here to make tab expand code together.
            string strDrvName = "";
            if (false == common.GetPrinterDrvName( statusPanelPage.m_selectedPrinter, ref strDrvName))
            {
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple(
                        (string)this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"),
                        (string)this.FindResource("ResStr_Error")
                        );
                messageBox.Owner = this;
                messageBox.ShowDialog();
                ShowTroubleshootingPage();
            }
            else
            {
                if ( m_isOnlineDetected || false == common.IsOffline( (EnumStatus)status) )
                {              
                    ExpandSubpage();
                    m_isOnlineDetected = true;
                }
            }

            statusUpdater = new Thread(UpdateStatusCaller);
            statusUpdater.Start();

            if (m_isMainWinLoaded == true)
            {
                thread_PrinterInfo2 = new Thread(UploadPrinterInfo2);
                thread_PrinterInfo2.Start();
            }
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
            SetTabItemFromIndex(EnumSubPage.Print);

            if (false == statusPanelPage.m_isSFP)
            {
                line1.Visibility = Visibility.Visible;
                line2.Visibility = Visibility.Visible;
                line3.Visibility = Visibility.Visible;

                Print_Grid.Visibility = Visibility.Visible;
                Grid.SetColumn(Print_Grid, 1);
                Grid.SetRow(Print_Grid, 0);

                Scan_Grid.Visibility = Visibility.Visible;
                Grid.SetColumn(Scan_Grid, 3);
                Grid.SetRow(Scan_Grid, 0);

                Copy_Grid.Visibility = Visibility.Visible;
                Grid.SetColumn(Copy_Grid, 5);
                Grid.SetRow(Copy_Grid, 0);

                Setting_Grid.Visibility = Visibility.Visible;
                Grid.SetColumn(Setting_Grid, 7);
                Grid.SetRow(Setting_Grid, 0);

                tabItem_Print.Visibility = Visibility.Visible;
                Grid.SetColumn(tabItem_Print, 1);
                Grid.SetRow(tabItem_Print, 1);                
               
                tabItem_Copy.Visibility = Visibility.Visible;
                Grid.SetColumn(tabItem_Copy, 3);
                Grid.SetRow(tabItem_Copy, 1);

                tabItem_Scan.Visibility = Visibility.Visible;
                Grid.SetColumn(tabItem_Scan, 5);
                Grid.SetRow(tabItem_Scan, 1);

                tabItem_Setting.Visibility = Visibility.Visible;
                Grid.SetColumn(tabItem_Setting, 7);
                Grid.SetRow(tabItem_Setting, 1);               

                Grid.SetColumnSpan(tabItem_Container, 7);                
            }
            else
            {
                line1.Visibility = Visibility.Visible;
                line2.Visibility = Visibility.Hidden;
                line3.Visibility = Visibility.Hidden;

                Print_Grid.Visibility = Visibility.Visible;
                Grid.SetColumn(Print_Grid, 1);
                Grid.SetRow(Print_Grid, 0);

                Scan_Grid.Visibility = Visibility.Hidden;
                Copy_Grid.Visibility = Visibility.Hidden;

                Setting_Grid.Visibility = Visibility.Visible;
                Grid.SetColumn(Setting_Grid, 3);
                Grid.SetRow(Setting_Grid, 0);

                tabItem_Print.Visibility = Visibility.Visible;
                Grid.SetColumn(tabItem_Print, 1);
                Grid.SetRow(tabItem_Print, 1);

                tabItem_Copy.Visibility = Visibility.Hidden;
                tabItem_Scan.Visibility = Visibility.Hidden;

                tabItem_Setting.Visibility = Visibility.Visible;
                Grid.SetColumn(tabItem_Setting, 3);
                Grid.SetRow(tabItem_Setting, 1);

                Grid.SetColumnSpan(tabItem_Container, 3);
            }

            EnableTabItems(true);
        }

        public void RemoveScanImage()
        {
            winScanPage.image_wrappanel.Children.Clear();
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

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            if ( 0 < winScanPage.image_wrappanel.Children.Count )
            {
                if (VOP.Controls.MessageBoxExResult.Yes !=
                        VOP.Controls.MessageBoxEx.Show(
                                                    VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                                                    this,
                                                    (string)this.TryFindResource("ResStr_The_scanned_images_will_be_deleted_after_closing_the_VOP__Are_you_sure_to_close_the_VOP_"),
                                                    (string)this.TryFindResource("ResStr_Prompt")
                                                    )
                    )
                {
                    SetTabItemFromIndex(EnumSubPage.Scan);
                    e.Cancel = true;
                }
            }
            else
            {
                if (VOP.Controls.MessageBoxExResult.Yes !=
                        VOP.Controls.MessageBoxEx.Show(
                                                    VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                                                    this,
                                                    (string)this.TryFindResource("ResStr_Do_you_want_to_exit_the_Lenovo_Printer"),
                                                    (string)this.TryFindResource("ResStr_Prompt")
                                                    )
                    )
                {
                    e.Cancel = true;
                }
            }

            if ( false == e.Cancel )
                MainWindowExitPoint();
        }

        // Update LED light color according the status.
        private void UpdateLED(EnumStatus s)
        {
            StatusDisplayType type = common.GetStatusTypeForUI(s);

            imgLEDRed.Visibility = Visibility.Hidden;
            imgLEDGray.Visibility = Visibility.Hidden;
            imgLEDGreen.Visibility = Visibility.Hidden;

            switch (type)
            {
                case StatusDisplayType.Ready:
                case StatusDisplayType.Sleep:
                case StatusDisplayType.Warning:
                case StatusDisplayType.Busy:
                    imgLEDGreen.Visibility = Visibility.Visible;
                    break;
                case StatusDisplayType.Error:
                    imgLEDRed.Visibility = Visibility.Visible;
                    break;
                case StatusDisplayType.Offline:
                    imgLEDGray.Visibility = Visibility.Visible;
                    break;
                default:
                    imgLEDGray.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ShowStartupWindow()
        {
            StartupWindow win = new StartupWindow(4);
            win.Owner = this;
            win.ShowDialog();
        }

        public void ShowCRMAgreementWindow()
        {
            bool? result = null;
            CRMAgreementWindow win = new CRMAgreementWindow();
            win.Owner = this;
            win.IsAgreementChecked = m_crmAgreement;
            result = win.ShowDialog();

            if (result == true)
            {
                m_crmAgreement = win.IsAgreementChecked;
            }
        }

        public void ShowTroubleshootingPage()
        {
            SetTabItemFromIndex(EnumSubPage.Print);
            subPageView.Child = winTroubleshootingPage;

            EnableTabItems(false);
        }
		
		public void EnableTabItems(bool isEnable)
        {
            btnPrint.IsEnabled = btnCopy.IsEnabled = btnScan.IsEnabled = btnSetting.IsEnabled = isEnable;
            tabItem_Print.IsEnabled = tabItem_Copy.IsEnabled = tabItem_Scan.IsEnabled = tabItem_Setting.IsEnabled = isEnable;
        }

        public void UpdateLogonBtnStatus(bool _bLogon)
        {
            try
            {
                this.btnLogin.IsLogon = _bLogon;
            }
            catch
            {

            }
        }

        private void OnbtnLoginClicked(object sender, RoutedEventArgs e)
        {
            if (logonview.IsVisible == false && ModifyUserInfoview.IsVisible == false)
            {
                logonview.Visibility = Visibility.Visible;
                ModifyUserInfoview.Visibility = Visibility.Hidden;
                mainview.Visibility = Visibility.Hidden;
                statusPageView.Visibility = Visibility.Hidden;
                UserCenterView ucv = new UserCenterView();
                logonview.Child = ucv;
                Background_SubPageView.Visibility = Visibility.Hidden;
            }
        }

        public void ShowUserCenterView(bool bShow)
        {
            if (bShow)
            {
                logonview.Visibility = Visibility.Visible;
                ModifyUserInfoview.Visibility = Visibility.Hidden;
                mainview.Visibility = Visibility.Hidden;
                statusPageView.Visibility = Visibility.Hidden;
                UserCenterView ucv = new UserCenterView();
                logonview.Child = ucv;
                Background_SubPageView.Visibility = Visibility.Hidden;

            }
            else
            {
                logonview.Visibility = Visibility.Hidden;
                ModifyUserInfoview.Visibility = Visibility.Hidden;
                mainview.Visibility = Visibility.Visible;
                statusPageView.Visibility = Visibility.Visible;
                Background_SubPageView.Visibility = Visibility.Visible;
            }
        }

        public void ShowModifyUserInfoView()
        {
            ModifyUserInfoview.Visibility = Visibility.Visible;
            mainview.Visibility = Visibility.Hidden;
            logonview.Visibility = Visibility.Hidden;
            statusPageView.Visibility = Visibility.Hidden;
            ModifyUserInfo ucv = new ModifyUserInfo();
            ModifyUserInfoview.Child = ucv;
            Background_SubPageView.Visibility = Visibility.Hidden;
        }

        // Get the popup setting from register. 
        private void GetPopupSetting( string xmlFile, ref bool popupIDCard, ref bool popupNIn1 )
        {
            popupIDCard = true;
            popupNIn1   = true;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load( xmlFile );

                XmlNode xmlNode1 = xmlDoc.SelectSingleNode( "/VOPCfg/elPopupIDCard" );
                XmlNode xmlNode2 = xmlDoc.SelectSingleNode( "/VOPCfg/elPopupNIn1" );
                XmlNode xmlNode3 = xmlDoc.SelectSingleNode("/VOPCfg/crmAgreement");
                XmlNode xmlNode4 = xmlDoc.SelectSingleNode("/VOPCfg/crmAgreementDialogShowed");

                if (null != xmlNode1 && null != xmlNode2 && xmlNode4 != null)
                {
                    popupIDCard = ( "True" == xmlNode1.InnerText ); 
                    popupNIn1 = ( "True" == xmlNode2.InnerText );
                    m_crmAgreement = ("True" == xmlNode3.InnerText); 
                    m_crmAgreementDialogShowed = ("True" == xmlNode4.InnerText);
                }
 
            }
            catch
            {
            }

        }

        // Set the popup setting from register. 
        private void SetPopupSetting( string xmlFile, bool popupIDCard, bool popupNIn1 )
        {
            string parentFolder = System.IO.Path.GetDirectoryName( xmlFile );

            if ( false == Directory.Exists( parentFolder ) ) 
            {
                Directory.CreateDirectory( parentFolder );
            }

            XmlDocument xmlDoc = new XmlDocument();

            string version    = "1.0";
            string encoding   = "gb2312";
            string standalone = "yes";

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding, standalone);
            XmlNode root = xmlDoc.CreateElement( "VOPCfg" );
            xmlDoc.AppendChild(xmlDeclaration);
            xmlDoc.AppendChild(root);

            XmlElement elPopupIDCard = xmlDoc.CreateElement( "elPopupIDCard" );
            XmlElement elPopupNIn1   = xmlDoc.CreateElement( "elPopupNIn1" );
            XmlElement crmAgreement   = xmlDoc.CreateElement( "crmAgreement" );
            XmlElement crmAgreementDialogShowed = xmlDoc.CreateElement("crmAgreementDialogShowed");
            
            elPopupIDCard.InnerXml = popupIDCard.ToString();
            elPopupNIn1.InnerXml   = popupNIn1.ToString();
            crmAgreement.InnerXml  = m_crmAgreement.ToString();
            crmAgreementDialogShowed.InnerXml = m_crmAgreementDialogShowed.ToString();

            root.AppendChild( elPopupIDCard );
            root.AppendChild( elPopupNIn1   );
            root.AppendChild(crmAgreement);
            root.AppendChild(crmAgreementDialogShowed);

            xmlDoc.Save( xmlFile );

        }

        public void ErrorMarkerClick()
        {
            if ( EnumStatus.NofeedJam               == m_oldStatus
                    || EnumStatus.JamAtRegistStayOn == m_oldStatus
                    || EnumStatus.InitializeJam     == m_oldStatus
                    || EnumStatus.JamAtExitNotReach == m_oldStatus
                    || EnumStatus.JamAtExitStayOn   == m_oldStatus )
            {
                string[] _gifs = null;       // Animation Uri need to display
                string title = "";

                GetPopupInfo( ref _gifs, ref title, m_oldStatus );

                m_isCloseAnimation = false;  
                MessageBoxEx_Video win = new MessageBoxEx_Video( _gifs, title );
                win.m_MainWin = this;
                win.Owner = this;
                win.ShowDialog(); // TODO: Why this modeless dialog will not block WndProc()?
            }
        }

        private void GetPopupInfo( ref string[] _gifs, ref string title, EnumStatus status )
        {
            if (0x804 == App.LangId)
            {
                switch ( status )
                {
                    case EnumStatus.JamAtExitStayOn    : 

                        _gifs = new string[3] { 
                            "pack://application:,,, /Media/JamAtExitStayOn1_zh.gif",
                                "pack://application:,,, /Media/JamAtExitStayOn2_zh.gif",
                                "pack://application:,,, /Media/JamAtExitStayOn3_zh.gif" };

                        break;
                    case EnumStatus.NofeedJam          :
                        _gifs = new string[2] { "pack://application:,,, /Media/NofeedJam1_zh.gif", "pack://application:,,, /Media/NofeedJam2_zh.gif" };
                        break;
                    case EnumStatus.JamAtRegistStayOn  : 
                    case EnumStatus.JamAtExitNotReach  : 
                    case EnumStatus.InitializeJam  : 
                    default: 
                        _gifs = new string[3] {
                            "pack://application:,,, /Media/JamInside1_zh.gif",
                                "pack://application:,,, /Media/JamInside2_zh.gif",
                                "pack://application:,,, /Media/JamInside3_zh.gif"
                        };
                        break;
                }
            }
            else
            {
                switch ( status )
                {
                    case EnumStatus.JamAtExitStayOn    : 
                        _gifs = new string[3] { 
                            "pack://application:,,, /Media/JamAtExitStayOn1_en.gif",
                                "pack://application:,,, /Media/JamAtExitStayOn2_en.gif",
                                "pack://application:,,, /Media/JamAtExitStayOn3_en.gif"
                        };
                        break;
                    case EnumStatus.NofeedJam          :
                        _gifs = new string[2] { "pack://application:,,, /Media/NofeedJam1_en.gif", "pack://application:,,, /Media/NofeedJam2_en.gif" };
                        break;
                    case EnumStatus.JamAtRegistStayOn  : 
                    case EnumStatus.JamAtExitNotReach  : 
                    case EnumStatus.InitializeJam  : 
                    default: 
                        _gifs = new string[3] {
                            "pack://application:,,, /Media/JamInside1_en.gif",
                                "pack://application:,,, /Media/JamInside2_en.gif",
                                "pack://application:,,, /Media/JamInside3_en.gif"
                        };
                        break;
                }
            }


            switch ( status )
            {
                case EnumStatus.JamAtExitStayOn    : 
                    title = (string)FindResource( "ResStr_Jam_back" );
                    break;
                case EnumStatus.NofeedJam          :
                    title = (string)FindResource( "ResStr_Out_of_Paper" );
                    break;
                case EnumStatus.JamAtRegistStayOn  : 
                    title = (string)FindResource( "ResStr_Jam_front" );
                    break;
                case EnumStatus.JamAtExitNotReach  : 
                    title = (string)FindResource( "ResStr_Jam_whole" );
                    break;
                case EnumStatus.InitializeJam  : 
                    title = (string)FindResource( "ResStr_Jam_whole" );
                    break;
                default: 
                    title = (string)FindResource( "ResStr_Jam_whole" );
                    break;
            }

        }

    }
}
