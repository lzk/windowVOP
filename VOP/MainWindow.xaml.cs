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
        public static string m_strPhoneNumber = "";
        public static bool   m_bLocationIsChina = false;
        public static byte   m_byWifiInitStatus = 0;
        public static List<UploadPrintInfo> m_UploadPrintInfoSet = new List<UploadPrintInfo>();

        private bool m_isAnimationPopup = false;  // True if animation window had popup.
        public  bool m_isCloseAnimation = false;  // True if animation window need to close.
        public  string m_animationUri = "";       // Animation Uri need to display

        // Old status used to popup animation window.
        private EnumStatus m_oldStatus = EnumStatus.Offline;

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

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            const int GEOCLASS_NATION = 16;
            int nGeoID = Win32.GetUserGeoID(GEOCLASS_NATION);
            if (45 == nGeoID)
            {
                m_bLocationIsChina = true;
            }

            statusPanelPage.eventPrinterSwitch += PrinterSwitch;

            winCopyPage    .m_MainWin = this;
            winScanPage    .m_MainWin = this;
            winSettingPage .m_MainWin = this;
            winFileSelectionPage.m_MainWin = this;
            winPrintPage.m_MainWin = this;
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
                        btnLogin.IsLogon = true;
                        btnLogin.bottomText = strUserName;
                    }
                }

                uploadCRMThread = new Thread(UploadCRM_LocalInfoToServerCaller);
                uploadCRMThread.Start();

                //MerchantInfoSet MerSet = new MerchantInfoSet();
                //MaintainInfoSet maintainSet = new MaintainInfoSet();
                //string strMerInfo = "";
                //string strMaintainInfo = "";
                //SessionInfo session = new SessionInfo();
                //m_RequestManager.GetSession(ref session);
                //m_RequestManager.GetMerchantSet(0, 200, ref MerSet, ref strMerInfo);
                //m_RequestManager.GetMaintainInfoSet(0, 200, ref maintainSet, ref strMaintainInfo);
                //SaveInfoDataIntoXamlFile(strMerInfo, strMaintainInfo);
            }

            Thread uploadPrintInfoThread = new Thread(UploadPrintInfoToServerCaller);
            uploadPrintInfoThread.Start();
           
            this.SourceInitialized += new EventHandler(win_SourceInitialized);  
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
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                var directory = new DirectoryInfo(documentsPath);
                string strUsersPublic = directory.Parent.FullName;
                string strDirectory = strUsersPublic + "\\Lenovo\\";
                Directory.CreateDirectory(strDirectory);
                string path = strDirectory + "PrintInfo.xaml";

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
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                var directory = new DirectoryInfo(documentsPath);
                string strUsersPublic = directory.Parent.FullName;
                string strDirectory = strUsersPublic + "\\Lenovo\\";
                Directory.CreateDirectory(strDirectory);

                string path = strDirectory + "PrintInfo.xaml";
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
        }

        public static bool SaveInfoDataIntoXamlFile(string strMerInfo, string strMaintainInfo)
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

                doc.Save(strDirectory + "Data.xaml");

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

                XmlElement eleTime = doc.CreateElement(string.Empty, "UserName", string.Empty);
                XmlText textTime = doc.CreateTextNode(strUserName);
                eleTime.AppendChild(textTime);
                eleBody.AppendChild(eleTime);

                XmlElement eleData = doc.CreateElement(string.Empty, "Password", string.Empty);
                XmlText textData = doc.CreateTextNode(strPassword);
                eleData.AppendChild(textData);
                eleBody.AppendChild(eleData);

                doc.Save(strDirectory + "UserInfo.xaml");

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
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                var directory = new DirectoryInfo(documentsPath);
                string strUsersPublic = directory.Parent.FullName;
                string strPath = strUsersPublic + "\\Lenovo\\UserInfo.xaml";

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
            System.Windows.Forms.MenuItem menuItem1 = new System.Windows.Forms.MenuItem((string)this.TryFindResource("ResStr_Exit"));
            menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            contextMenu1.MenuItems.Clear();
            contextMenu1.MenuItems.Add(menuItem1);

            notifyIcon1.ContextMenu = contextMenu1;

            notifyIcon1.Text = (string)this.FindResource("ResStr_VirtualOperationPanel");
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
            dll.SaveDefaultPrinter(); //save default printer name before vop action
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
                    this.Close();
                }
                else if ("btnMinimize" == btn.Name)
                {                             
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
                    loginWnd.Owner = this;
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
                    txtPageName.Text = (string)this.FindResource("ResStr_ExtraAdd_Print");
                    this.subPageView.Child = winFileSelectionPage;
                
                    tabItem_Print.IsSelect = true;
                    tabItem_Copy.IsSelect = false;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Copy == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_ExtraAdd_Copy");
                    this.subPageView.Child = winCopyPage;
                 
                    tabItem_Print.IsSelect = false;
                    tabItem_Copy.IsSelect = true;
                    tabItem_Scan.IsSelect = false;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Scan == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_ExtraAdd_Scan");
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
                    txtPageName.Text = (string)this.FindResource("ResStr_ExtraAdd_Print");
                    this.subPageView.Child = winFileSelectionPage;
                   
                    tabItem_Print.IsSelect = true;
                    tabItem_Setting.IsSelect = false;
                }
                else if ( EnumSubPage.Setting == subpage )
                {
                    txtPageName.Text = (string)this.FindResource("ResStr_Setting");
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
        public static bool bExitUpdater = false;
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


            bExit = true;
            bExitUpdater = true;
            m_updaterAndUIEvent.WaitOne();
            SavePrintInfoIntoXamlFile();
            dll.RecoverDevModeData();
            notifyIcon1.Visible = false;
            dll.ResetDefaultPrinter();
            PdfPrint.CloseAll();

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
                   if ( false == common.IsOffline( status ) )
                   {
                       m_isOnlineDetected = true;
                       ExpandSubpage();
                   }
               }

               winCopyPage.PassStatus( status, job, toner );
               winScanPage.PassStatus( status, job, toner );
               winPrintPage.PassStatus(status, job, toner);
               winSettingPage.PassStatus(status, job, toner);

               if ( EnumStatus.NofeedJam               == status
                       || EnumStatus.JamAtRegistStayOn == status
                       || EnumStatus.JamAtExitNotReach == status
                       || EnumStatus.JamAtExitStayOn   == status )
               {
                   if ( m_oldStatus != status )
                   {
                       switch ( status )
                       {
                           case EnumStatus.JamAtExitNotReach  : m_animationUri = "pack://siteoforigin:,,,/../../Media/JamAtExitNotReach.mp4"; break;
                           case EnumStatus.JamAtExitStayOn    : m_animationUri = "pack://siteoforigin:,,,/../../Media/JamAtExitStayOn.mp4"  ; break;
                           case EnumStatus.JamAtRegistStayOn  : m_animationUri = "pack://siteoforigin:,,,/../../Media/JamAtRegistStayOn.mp4"; break;
                           case EnumStatus.NofeedJam          : m_animationUri = "pack://siteoforigin:,,,/../../Media/NofeedJam.mp4"        ; break;
                           default: 
                                                                  m_animationUri = "pack://siteoforigin:,,,/../../Media/NofeedJam.mp4"      ; break;
                       }

                       if ( false == m_isAnimationPopup )
                       {
                           m_isCloseAnimation = false;  
                           m_isAnimationPopup = true;
                           MessageBoxEx_Video win = new MessageBoxEx_Video(new Uri(m_animationUri), (string)this.TryFindResource("ResStr_The_paper_jam_occurred_please_follow_the_instructions_"), (string)this.FindResource("ResStr_Error"));
                           win.m_MainWin = this;
                           win.Owner = this;
                           win.ShowDialog();
                           m_isAnimationPopup = false;
                       }
                   }
               }
               else
               {
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
               Application.Current.Shutdown(0);
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

            winCopyPage.ResetToDefaultValue();
            winScanPage.ResetToDefaultValue();

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

            if ( m_isOnlineDetected || false == common.IsOffline( (EnumStatus)status) )
            {
                ExpandSubpage();
                m_isOnlineDetected = true;
            }

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
        
        public void RemoveScanImage()
        {
             winScanPage.image_wrappanel.Children.Clear();
        }

        public void ShowAboutPageOnly()
        {
            SetTabItemFromIndex(EnumSubPage.Setting);

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
            winSettingPage.InitWindowLayout();

            RemoveScanImage();
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
                                                    VOP.Controls.MessageBoxExStyle.YesNo,
                                                    this,
                                                    (string)this.TryFindResource("ResStr_The_scanned_images_will_be_deleted_after_closing_the_VOP__Are_you_sure_to_close_the_VOP_"),
                                                    (string)this.TryFindResource("ResStr_Warning_2")
                                                    )
                    )
                {
                    SetTabItemFromIndex(EnumSubPage.Scan);
                    e.Cancel = true;
                }
            }

            if ( false == e.Cancel )
                MainWindowExitPoint();
        }
    }
}
