using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading; // for Mutex
using System.Globalization; // for Multi-Langulage UI
using Microsoft.Win32;

namespace VOP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Scanned images file list. The files in list need to be cleared, so
        /// define in App.
        /// </summary>
        public static List<ScanFiles> scanFileList = new List<ScanFiles>(); 
        public static List<ScanFiles> rubbishFiles = new List<ScanFiles>(); // Rubbish files list, delete them when exit.

        public static AutoMachine g_autoMachine = new AutoMachine();

        static Mutex mutex = new Mutex(true, "4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_mutex");
        public static uint WM_STATUS_UPDATE = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_status");
        public static uint WM_CHECK_MAINTAIN_DATA_Expired = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_maintain");
        public static uint WM_CHECK_MERCHANT_INFO_Expired = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_merchant");
        public static uint WM_VOP = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_vop");
        public static double gScalingRate = 1.0; // Scaling rate used to scale windows's according the screen resolution.

        App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");

            // Calculate the scaling rate for resolution.
            int nWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int nHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            double scaling1 = nWidth / 1600.0;
            double scaling2 = nHeight / 900.0;

            gScalingRate = (scaling1 < scaling2) ? scaling1 : scaling2;
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main() 
        {
            if(mutex.WaitOne(TimeSpan.Zero, true)) 
            {
                VOP.App app = new VOP.App();
                app.InitializeComponent();
                app.Run();

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
            else
            {
                Win32.PostMessage( (IntPtr)0xffff, WM_VOP, IntPtr.Zero , IntPtr.Zero );
            }

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

            RegistryKey rsg = null;
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Lenovo\\Printer SSW\\Version", false);
            Int32 nLanguage = 0x804;
            object obj = null;
            if (null != rsg)
            {
                obj = rsg.GetValue("language", RegistryValueKind.DWord);
                nLanguage = (Int32)obj;         

                rsg.Close();
            }           
            
            switch(nLanguage)
            {
                case 1033:
                    Culture = "en-US";
                    break;
                case 2052:
                    Culture = "zh-CN";
                    break;
                default:
                    Culture = "zh-CN";
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
    }
}
