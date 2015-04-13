using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

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
        public static uint WM_STATUS_UPDATE = Win32.RegisterWindowMessage("35abddc8c9f59ddfebcf8a3bfdd8ea9b4ef9dfd8");

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
        public static void Main() {
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
        }
    }
}
