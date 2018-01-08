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
using System.Net.NetworkInformation;

namespace VOP
{
 
    public class ScanTask
    {
        public static uint WM_VOPSCAN_PROGRESS = Win32.RegisterWindowMessage("vop_scan_progress");
        public static uint WM_VOPSCAN_UPLOAD = Win32.RegisterWindowMessage("vop_scan_upload");
        public static uint WM_VOPSCAN_PAGECOMPLETE = Win32.RegisterWindowMessage("vop_scan_pagecomplete");
        public Scan_RET ScanResult = Scan_RET.RETSCAN_OK;
        private const string USBSCANSTRING = "\\\\.\\usbscan";

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        public static bool IsOnLine()
        {
            try
            {
                System.Int32 dwFlag = new Int32();
                if (InternetGetConnectedState(out dwFlag, 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        private string GetDeviceName(string devicename)
        {
            int index = devicename.LastIndexOf(' ');
            if (index > 0)
            {
                string str = USBSCANSTRING;
                str += devicename.Substring(index + 1);

                return str;
            }
            else
            {
                return string.Empty;
            }


            return string.Empty;
        }

        public List<ScanFiles> DoScan(string deviceName, ScanParam param)
        {
            //debug test
            //ScanWaitWindow_Rufous scanPbw = new ScanWaitWindow_Rufous();
            //scanPbw.Owner = Application.Current.MainWindow;
            //scanPbw.ShowDialog();

            if (param == null)
                return null;

            List<ScanFiles> files = new List<ScanFiles>();

//            string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString("D10");

            //modifified by yunying shang 2017-10-27 for BMS 1236
            string strSuffix = string.Format("{0}{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}", "img", 
                DateTime.Now.Year, 
                DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute,
                DateTime.Now.Second);
            //<<=============1236
            if (false == Directory.Exists(App.cacheFolder))
            {
                Directory.CreateDirectory(App.cacheFolder);
            }


            string tempPath = App.PictureFolder + @"\" + strSuffix;

            
            int nWidth = 0;
            int nHeight = 0;
            string[] fileNames = null;

            common.GetPaperSize(param.PaperSize, ref nWidth, ref nHeight);

            //add by yunying shang 2017-10-18 for BMS 1019
            if (MainWindow_Rufous.g_settingData.m_isUsbConnect == false)
            {
                NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                bool bFound = false;
                foreach (NetworkInterface adapter in fNetworkInterfaces)
                {
                    if (adapter.Description.Contains("802") ||
                        adapter.Description.Contains("Wi-Fi") ||
                        adapter.Description.Contains("Wireless"))
                    {
                        bFound = true;
                    }
                }
                //add by yunying shang 2017-10-23 for BMS 1019
                if (bFound == false || !IsOnLine())
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_conn_fail"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                                );
                    return null;
                }
                else
                {
                    if (!dll.TestIpConnected(MainWindow_Rufous.g_settingData.m_DeviceName))
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                    Application.Current.MainWindow,
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_conn_fail"),
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                                    );
                        return null;
                    }
                }
            }
            else
            {
                //modified by yunying shang 2017-12-15 for BMS 1796
                if (dll.CheckConnectionByName(GetDeviceName(deviceName)))
                {

                }
                else
                {
                    StringBuilder usbname = new StringBuilder(50);
                    if (dll.CheckUsbScan(usbname) == 1)
                    {
                        dll.SetConnectionMode(GetDeviceName(usbname.ToString()), true);
                    }
                    else
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                    Application.Current.MainWindow,
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_conn_fail"),
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                                    );
                        return null;
                    }
                }//<<================1796
            }
            //<<=================================


next:
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
                    WM_VOPSCAN_UPLOAD,
                    param.AutoColorDetect,
                    param.SkipBlankPage,
                    param.Gamma,
                    (int)param.ScanMediaType,
                    out fileNames);

//            sw.Stop();
            Trace.WriteLine(string.Format("Elapsed={0}", sw.Elapsed));

            //Win32.OutputDebugString("Scanning Finished!");
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
            sw.Stop();
            ScanResult = (Scan_RET)nResult;

            string error = string.Format("ScanResult is {0}", ScanResult);
            Win32.OutputDebugString(error);
            if (ScanResult != Scan_RET.RETSCAN_OK)
            {
                if (ScanResult == Scan_RET.RETSCAN_OPENFAIL)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                               Application.Current.MainWindow,
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Device_Not_Ready"),
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_OPENFAIL_NET)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Device_Not_Ready"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_BUSY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_net_scanner_busy"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_PAPER_JAM)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_paper_jam"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_COVER_OPEN)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_cover_open"),
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_PAPER_NOT_READY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_paper_not_ready"),
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_ADFCOVER_NOT_READY ||
                    ScanResult == Scan_RET.RETSCAN_ADFDOC_NOT_READY ||
                    ScanResult == Scan_RET.RETSCAN_ADFPATH_NOT_READY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_adf_not_ready"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_HOME_NOT_READY)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_home_not_ready"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_ULTRA_SONIC)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_multifeed_error"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                               );
                }
                else if (ScanResult == Scan_RET.RETSCAN_CANCEL)
                {

                }
                else if (ScanResult == Scan_RET.RETSCAN_ERROR_POWER1)
                {
                    VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo_NoIcon1,
                               Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Power_Bank"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt")
                               );
                    if (VOP.Controls.MessageBoxExResult.Yes == ret)
                    {
                        param.ADFMode = false;
                        param.AutoCrop = false;
                        param.MultiFeed = false;
                        if (param.PaperSize == EnumPaperSizeScan._LongPage)
                            param.PaperSize = EnumPaperSizeScan._Auto;
                        if (param.ScanMediaType > 0)
                            param.ScanMediaType = 0;
                        goto next;
                    }
                }
                else if (ScanResult == Scan_RET.RETSCAN_ERROR_POWER2)
                {
                    if (MainWindow_Rufous.g_settingData.m_isUsbConnect == false)
                    {
                        VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                    Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Power_Bus_Wifi"),
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                                    );
                    }
                    else
                    {
                        VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo_NoIcon1,
                                   Application.Current.MainWindow,
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Power_Bus"),
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt")
                                   );
                        if (VOP.Controls.MessageBoxExResult.Yes == ret)
                        {
                            param.ADFMode = false;
                            param.AutoCrop = false;
                            param.MultiFeed = false;
                            if (param.PaperSize == EnumPaperSizeScan._LongPage)
                                param.PaperSize = EnumPaperSizeScan._Auto;
                            if (param.ScanMediaType > 0)
                                param.ScanMediaType = 0;
                            goto next;
                        }
                    }
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_fail"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                                );
                }
            } 

            return files.Count > 0 ? files : null;
        }
    }
}
