﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading; // for Mutex
using System.Globalization; // for Multi-Langulage UI
using Microsoft.Win32;
using System.Diagnostics;
using System.Xml;

namespace VOP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string currentFolder = System.IO.Path.GetDirectoryName( System.Windows.Forms.Application.ExecutablePath);
        public static string pathSimulationFile = currentFolder + "\\DeviceStatus.xml";
        public static string vopHelperExe = currentFolder + "\\VopHelper.exe";
        public static string cacheFolder = System.IO.Path.GetTempPath()+"VOPCache"; // Folder used to store the cache file for scanning.
        public static string crmFolder = System.IO.Path.GetTempPath() + "VOP_CRM";
        public static string cfgFile = "";
        public static string cfgFolder = "";
        public static string PictureFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);

        /// <summary>
        /// Scanned images file list. The files in list need to be cleared, so
        /// define in App.
        /// </summary>
        public static List<ScanFiles> scanFileList = new List<ScanFiles>(); 
        public static List<ScanFiles> rubbishFiles = new List<ScanFiles>(); // Rubbish files list, delete them when exit.

        static Mutex mutex = new Mutex(true, "4d8526fa07abfc03085ef2899b5b4d2ecaa3d712_mutex");
        public static uint WM_STATUS_UPDATE = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_status");
        public static uint WM_CHECK_MAINTAIN_DATA_Expired = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_maintain");
        public static uint WM_CHECK_MERCHANT_INFO_Expired = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_merchant");
        public static uint WM_VOP = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_vop");
        public static uint closeMsg = Win32.RegisterWindowMessage("vop_process_selfclose");
        public static double gScalingRate = 1.0; // Scaling rate used to scale windows's according the screen resolution.

        private static Int32 gLanguage = 0x409;
        public static Int32 LangId
        {
            get{ return gLanguage; }
        }

        App()
        {
            gLanguage = GetLangID(); 

            double nWidth = SystemParameters.PrimaryScreenWidth;
            double nHeight = SystemParameters.PrimaryScreenHeight;

            CalcScalingRate(nWidth, nHeight);
        }

        // Calculate the scaling rate for resolution.
        public static void CalcScalingRate(double nScreenWidth, double nScreenHeight)
        {
            gScalingRate = 1.0;

            // Resolution:1600*900  ==> Height="638" Width="850"
            if ((nScreenWidth < 850.0) || (nScreenHeight < 638.0))
            {      
                gScalingRate = 0.85;
            }
        }

        private Int32 GetLangID()
        {
            Int32 LangId = 0x409;
            //RegistryKey rsg = null;
            //rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Lenovo\\Printer SSW\\Version", false);
           
            //object obj = null;
            //if (null != rsg)
            //{
            //    obj = rsg.GetValue("language", RegistryValueKind.DWord);
            //    LangId = (Int32)obj;

            //    rsg.Close();
            //}

            //if (LanguageRegistry.Open())
            //{
            //    LangId = LanguageRegistry.GetLangID();
            //    LanguageRegistry.Close();
            //}
            //else
            //{
            //    CultureInfo ci = CultureInfo.InstalledUICulture;
            //    LangId = ci.LCID;
            //}

            return LangId;
        }

        public static void ResetVopCfg()
        {
            try
            {
                if (File.Exists(App.cfgFile))
                {

                    XmlDocument xmlDoc = new XmlDocument();

                    string version = "1.0";
                    string encoding = "gb2312";
                    string standalone = "yes";

                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding, standalone);
                    XmlNode root = xmlDoc.CreateElement("VOPCfg");
                    xmlDoc.AppendChild(xmlDeclaration);
                    xmlDoc.AppendChild(root);

                    XmlElement elPopupIDCard = xmlDoc.CreateElement("elPopupIDCard");
                    XmlElement elPopupNIn1 = xmlDoc.CreateElement("elPopupNIn1");
                    XmlElement crmAgreement = xmlDoc.CreateElement("crmAgreement");
                    XmlElement crmAgreementDialogShowed = xmlDoc.CreateElement("crmAgreementDialogShowed");

                    elPopupIDCard.InnerXml = "True";
                    elPopupNIn1.InnerXml = "True";
                    crmAgreement.InnerXml = "False";
                    crmAgreementDialogShowed.InnerXml = "False";

                    root.AppendChild(elPopupIDCard);
                    root.AppendChild(elPopupNIn1);
                    root.AppendChild(crmAgreement);
                    root.AppendChild(crmAgreementDialogShowed);

                    xmlDoc.Save(App.cfgFile);
                }
            }
            catch(Exception)
            { }

        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main() 
        {
            // Initial configuration foler.
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            var directory = new DirectoryInfo(documentsPath);
            string strUsersPublic = directory.Parent.FullName;
            cfgFolder = strUsersPublic + "\\Lenovo\\";
            Directory.CreateDirectory(cfgFolder);
            crmFolder = strUsersPublic + "\\Lenovo\\VOP_CRM";
            cfgFile = cfgFolder + "vopcfg.xml";

            string argLine = Environment.CommandLine;
            string regStr = "";

            if (SelfCloseRegistry.Open())
            {
                regStr = SelfCloseRegistry.GetEXIT();
                SelfCloseRegistry.DeleteEXIT();
                SelfCloseRegistry.Close();
            }

            if (argLine.Contains("EXIT") || regStr == "EXIT")
            {
                Process p = null;
                if (App.CheckProcessExist("CRMUploader", ref p) == true)
                {
                    if (p != null)
                        p.Kill();
                }

                App.ResetVopCfg();

                Win32.PostMessage((IntPtr)0xffff, closeMsg, IntPtr.Zero, IntPtr.Zero);
                return;
            }

            //if(mutex.WaitOne(TimeSpan.Zero, true)) 
            {

                try
                {
                    VOP.App app = new VOP.App();
                    app.InitializeComponent();
                    app.Run();
                }
                catch (Exception) { }
              
                GC.Collect();
                GC.WaitForPendingFinalizers();

                foreach( ScanFiles obj in App.scanFileList )
                {
                    try
                    {
                        File.Delete(obj.m_pathOrig);
                        File.Delete(obj.m_pathView);
                        File.Delete(obj.m_pathThumb);
                    }
                    catch
                    {
                    }
                }

                foreach( ScanFiles obj in App.rubbishFiles )
                {
                    try
                    {
                        File.Delete(obj.m_pathOrig);
                        File.Delete(obj.m_pathView);
                        File.Delete(obj.m_pathThumb);
                    }
                    catch
                    {
                    }
                }

                mutex.ReleaseMutex();
            }
            //else
            //{
            //    Win32.PostMessage((IntPtr)0xffff, WM_VOP, IntPtr.Zero, IntPtr.Zero);
            //}

        }

        #region Multi-Langulage  
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GetCulture();
        }

        // https://msdn.microsoft.com/zh-cn/library/kx54z3k7(VS.80).aspx
        public void GetCulture()
        {
            string Culture = string.Empty;           
            
            switch(gLanguage)
            {
                case 1033:
                    Culture = "en-US";
                    break;
                case 2052:
                    Culture = "zh-CN";
                    break;
                default:
                    Culture = "en-US";
                    break;
            }

            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"Resources\StringResource.{0}.xaml", Culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));

            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }             
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Culture);
        }
        #endregion  //Multi-Langulage

        public static bool CheckProcessExist(string name, ref Process p)
        {
            Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length > 0)
            {
                p = processes[0];
                return true;
            }
            else
            {
                p = null;
                return false;
            }
                
        }
    }
}
