using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading; // for Mutex


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
    }
}
