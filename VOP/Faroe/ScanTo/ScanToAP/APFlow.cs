using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.IO;
using System.Threading;
using System.Security.Authentication;
using System.Net;
using VOP.Controls;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Outlook = Microsoft.Office.Interop.Outlook;
using PdfEncoderClient;
using Microsoft.Win32;
using System.Diagnostics;

namespace VOP
{
    public enum APFlowType
    {
        View,
        Quick,
    };

    public class APFlow
    {
        public static APFlowType FlowType = APFlowType.View;
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }

        private StringBuilder OpenOutput = new StringBuilder(256);

        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
            }

            foreach (string f in FileList)
            {

            }
            string programType = "";

            if (FlowType == APFlowType.View)
            {
                bool? result = null;
                APSelectForm sf = new APSelectForm();
                sf.Owner = ParentWin;

                result = sf.ShowDialog();
                if (result == true)
                {
                    programType = sf.m_programType;

                    try
                    {
                        int i = 0;
                        if (programType == "Paint")
                        {
                            string processFilename = System.Environment.SystemDirectory + "\\mspaint.exe";
                            i = 0;
                            foreach (string f in FileList)
                            {
                                ProcessStartInfo info = new ProcessStartInfo();

                                info.FileName = processFilename;
                                info.Arguments = String.Format("\"{0}\"", f);
                                info.CreateNoWindow = false;
                                info.WindowStyle = ProcessWindowStyle.Normal;
                                info.UseShellExecute = false;

                                Process p = Process.Start(info);

                                if (MainWindow_Rufous.g_settingData.m_commonScanSettings.ADFMode == true)
                                {
                                    if (i == 1)
                                        break;
                                }
                                else
                                {
                                    if (i == 0)
                                        break;
                                }

                                i++;
                            }
                        }
                        else if (programType == "OthersApplication")
                        {
                            OthersAPSelectWin Others = new OthersAPSelectWin();
                            Others.Owner = ParentWin;
                            result = Others.ShowDialog();

                            if (result == true)
                            {
                                programType = Others.m_programType;
                                string path = Others.m_filePath;
                                i = 0;

                                //modified by yunying shang 2017-12-19 for BMS 1815
                                Thread.Sleep(500);
                                foreach (string f in FileList)
                                {                                    
                                    try
                                    {
                                        Process p = new Process();
                                        p.StartInfo.FileName = path;
                                        p.StartInfo.Arguments = string.Format("\"{0}\"", f);
                                        p.Start();

                                        if (MainWindow_Rufous.g_settingData.m_commonScanSettings.ADFMode == true)
                                        {
                                            if (i == 1)
                                                break;
                                        }
                                        else
                                        {
                                            if (i == 0)
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Win32.OutputDebugString(ex.Message);
                                    }
                                    i++;
                                    Thread.Sleep(500);                                  
                                }//<<===============1815
                            }
                        }
                        else
                        {
                            i = 0;

                            foreach (string f in FileList)
                            {
                                //modified by yunying shang 2017-12-07 for BMS 1722
                                Process.Start("rundll32.exe", String.Format("{0} {1}", "shimgvw.dll,ImageView_Fullscreen", f));
                                //<<==============1722
                                if (MainWindow_Rufous.g_settingData.m_commonScanSettings.ADFMode == true)
                                {
                                    if (i == 1)
                                        break;
                                }
                                else
                                {
                                    if (i == 0)
                                        break;
                                }
                                i++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                         Application.Current.MainWindow,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_open_p_fail") + ex.Message,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                                        );
                        return false;
                    }
                }
            }
            else
            {
                programType = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_APScanSettings.ProgramType;
                try
                {
                    int i = 0;
                    if (programType == "Paint")
                    {
                        string processFilename = System.Environment.SystemDirectory+"\\mspaint.exe";

                        i = 0;
                        foreach (string f in FileList)
                        {
                            ProcessStartInfo info = new ProcessStartInfo();

                            info.FileName = processFilename;
                            info.Arguments = String.Format("\"{0}\"", f);
                            info.CreateNoWindow = false;
                            info.WindowStyle = ProcessWindowStyle.Normal;
                            info.UseShellExecute = false;

                            Process p = Process.Start(info);

                            if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings.ADFMode == true)
                            {
                                if (i == 1)
                                    break;
                            }
                            else
                            {
                                if (i == 0)
                                    break;
                            }
                            i++;
                        }
                      
                    }
                    else if (programType == "OthersApplication")
                    {
                       // bool? result = null;
                       // OthersAPSelectWin Others = new OthersAPSelectWin();
                        //Others.Owner = ParentWin;
                        //result = Others.ShowDialog();

                        //if (result == true)
                        {
                           // programType = Others.m_programType;
                            string path = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_APScanSettings.APPath;//Others.m_filePath;
                            i = 0;
                            foreach (string f in FileList)
                            {
                                ProcessStartInfo info = new ProcessStartInfo();

                                info.FileName = path;
                                info.Arguments = String.Format("\"{0}\"", f);

                                Process p = Process.Start(info);
                                if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings.ADFMode == true)
                                {
                                    if (i == 1)
                                        break;
                                }
                                else
                                {
                                    if (i == 0)
                                        break;
                                }
                                i++;
                            }
                        }
                    }
                    else
                    {
                        i = 0;
                        foreach (string f in FileList)
                        {
                            //Process.Start(f);
                            //modified by yunying shang 2017-12-07 for BMS 1722
                            Process.Start("rundll32.exe", String.Format("{0} {1}", "shimgvw.dll,ImageView_Fullscreen", f));
                            //<<==============1722

                            if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings.ADFMode == true)
                            {
                                if (i == 1)
                                    break;
                            }
                            else
                            {
                                if (i == 0)
                                    break;
                            }
                            i++;
                        }                      
                    }
                }
                catch (Exception ex)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                     Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_open_p_fail") + ex.Message,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                                    );
                    return false;
                }
            }
          
            return true;
        }

        private void OpenOutputDataHandler(object sendingProcess,
          DataReceivedEventArgs outLine)
        {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                // Add the text to the collected output.
                OpenOutput.Append(Environment.NewLine + "  " + outLine.Data);
            }
        }

    }
}
