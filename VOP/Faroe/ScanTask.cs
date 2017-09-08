using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Collections.Generic;
using System.Windows.Interop; // for HwndSource
using System.Threading;
using Microsoft.Win32; // for SaveFileDialog
using PdfEncoderClient;
using VOP.Controls;
using System.ComponentModel;
using System.Diagnostics;

namespace VOP
{
 
    public class ScanTask
    {
        public static uint WM_VOPSCAN_PROGRESS = Win32.RegisterWindowMessage("vop_scan_progress");
        public Scan_RET ScanResult = Scan_RET.RETSCAN_OK;

        public List<ScanFiles> DoScan(string deviceName, ScanParam param)
        {
            //debug test
            //ScanWaitWindow_Rufous scanPbw = new ScanWaitWindow_Rufous();
            //scanPbw.Owner = Application.Current.MainWindow;
            //scanPbw.ShowDialog();

            if (param == null)
                return null;

            List<ScanFiles> files = new List<ScanFiles>();
            string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString("D10");

            if (false == Directory.Exists(App.cacheFolder))
            {
                Directory.CreateDirectory(App.cacheFolder);
            }


            string tempPath = App.cacheFolder + @"\" + strSuffix;

            
            int nWidth = 0;
            int nHeight = 0;
            string[] fileNames = null;

            common.GetPaperSize(param.PaperSize, ref nWidth, ref nHeight);

            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int nResult = worker.InvokeScanMethod(dll.ADFScan,
                    deviceName,
                    tempPath,
                    (int)param.ColorType,
                    (int)param.ScanResolution,
                    nWidth,
                    nHeight,
                    param.Contrast,
                    param.Brightness,
                    param.ADFMode,
                    param.MultiFeed,
                    param.AutoCrop,
                    param.OnePage,
                    WM_VOPSCAN_PROGRESS,
                    out fileNames);

            sw.Stop();
            Trace.WriteLine(string.Format("Elapsed={0}", sw.Elapsed));

            if (fileNames != null)
            {
                foreach (string name in fileNames)
                {
                    ScanFiles file = new ScanFiles();
                    file.m_colorMode = param.ColorType;
                    file.m_pathOrig = name;
                    file.m_pathView = name;
                    file.m_pathThumb = name;
                    files.Add(file);

                    App.scanFileList.Add(file);
                }
            }
          
            ScanResult = (Scan_RET)nResult;

            if (ScanResult != Scan_RET.RETSCAN_OK)
            {
                if (ScanResult == Scan_RET.RETSCAN_OPENFAIL)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Scan connection failed",
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_OPENFAIL_NET)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Scan connection failed",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_BUSY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Network scanner is busy",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_PAPER_JAM)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Paper jam",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_COVER_OPEN)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Cover is opened",
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_PAPER_NOT_READY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Paper is not ready",
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_ADF_NOT_READY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "ADF is not ready",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_HOME_NOT_READY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Home is not ready",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_ULTRA_SONIC)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Multi-feed error",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_CANCEL)
                {
                    
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                                "Scan failed",
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                                );
                }
            } 

            return files.Count > 0 ? files : null;
        }
    }
}
