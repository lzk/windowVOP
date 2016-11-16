﻿using System.Windows.Controls;
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
                }
            }
          
            ScanResult = (Scan_RET)nResult;

            if (ScanResult != Scan_RET.RETSCAN_OK)
            {
                if (ScanResult == Scan_RET.RETSCAN_OPENFAIL)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "USB scan connection failed",
                               "Error");
                }
                else if (ScanResult == Scan_RET.RETSCAN_OPENFAIL_NET)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               "Network scan connection failed",
                               "Error");
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                                "Scan failed",
                                "Error");
                }
            } 

            return files.Count > 0 ? files : null;
        }
    }
}