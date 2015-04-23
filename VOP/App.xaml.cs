using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading; // for Mutex
using System.Globalization; // for Multi-Langulage UI


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
        public static AutoMachine g_autoMachine = new AutoMachine();

        static Mutex mutex = new Mutex(true, "4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_mutex");
        public static uint WM_STATUS_UPDATE = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_status");
        public static uint WM_CHECK_MAINTAIN_DATA_Expired = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_maintain");
        public static uint WM_CHECK_MERCHANT_INFO_Expired = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_merchant");
        public static uint WM_VOP = Win32.RegisterWindowMessage("4d8526fa07abfc03085ef2899b5b4d2ecaa3d711_vop");

        App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
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
                        // TODO: clear cache.
                        // File.Delete(obj.m_pathOrig);
                        // File.Delete(obj.m_pathView);
                        // File.Delete(obj.m_pathThumb);
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
        public static string Culture { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GetCulture();
        }

        // https://msdn.microsoft.com/zh-cn/library/kx54z3k7(VS.80).aspx
        public void GetCulture()
        {
            Culture = string.Empty;          
            CultureInfo CurrentUICulture = CultureInfo.CurrentUICulture;       

            Culture = CurrentUICulture.Name;           

            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"Resources\StringResource.{0}.xaml", Culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            if (resourceDictionary == null)
            {
                requestedCulture = @"Resources\StringResource.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            }
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
