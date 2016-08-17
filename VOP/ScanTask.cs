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

namespace VOP
{
 
    public class ScanTask
    {
        private uint WM_VOPSCAN_PROGRESS = Win32.RegisterWindowMessage("vop_scan_progress2");
        private uint WM_VOPSCAN_COMPLETED = Win32.RegisterWindowMessage("vop_scan_completed");

        public ScanFiles DoScan(string deviceName, ScanParam param)
        {
            if (param == null)
                return null;

            ScanFiles files = new ScanFiles();
            string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString("D10");

            if (false == Directory.Exists(App.cacheFolder))
            {
                Directory.CreateDirectory(App.cacheFolder);
            }

            files.m_colorMode = param.ColorType;
            files.m_pathOrig = App.cacheFolder + "\\vopOrig" + strSuffix + ".bmp";
            files.m_pathView = App.cacheFolder + "\\vopView" + strSuffix + ".bmp";
            files.m_pathThumb = App.cacheFolder + "\\vopThum" + strSuffix + ".bmp";

            //m_shareObj.m_pathOrig = @"F:\PdfSave\300dpiOrig.bmp";
            //m_shareObj.m_pathView = @"F:\PdfSave\300dpiView.bmp";
            //m_shareObj.m_pathThumb = @"F:\PdfSave\300dpiThum.bmp";
            
            int nWidth = 0;
            int nHeight = 0;

            common.GetPaperSize(param.PaperSize, ref nWidth, ref nHeight);

            int nResult = dll.ScanEx(
                    deviceName,
                    files.m_pathOrig,
                    files.m_pathView,
                    files.m_pathThumb,
                    (int)param.ColorType,
                    (int)param.ScanResolution,
                    nWidth,
                    nHeight,
                    param.Contrast,
                    param.Brightness,
                    (int)param.DocType,
                    WM_VOPSCAN_PROGRESS);

            return files;
        }
    }
}
