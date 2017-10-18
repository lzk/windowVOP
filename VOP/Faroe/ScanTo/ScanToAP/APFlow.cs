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

        string m_errorMsg = "";
       
        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
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
                        if (programType == "Paint")
                        {
                            string processFilename = @"C:\Windows\System32\mspaint.exe";

                            foreach (string f in FileList)
                            {
                                ProcessStartInfo info = new ProcessStartInfo();

                                info.FileName = processFilename;
                                info.Arguments = String.Format("\"{0}\"", f);
                                info.CreateNoWindow = false;
                                info.WindowStyle = ProcessWindowStyle.Normal;
                                info.UseShellExecute = false;

                                Process p = Process.Start(info);
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
                                Process.Start(path);
                            }
                        }
                        else
                        {
                            foreach (string f in FileList)
                            {
                                Process.Start(f);
                            }                             
                        }
                    }
                    catch (Exception ex)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                         Application.Current.MainWindow,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_open_p_fail") + ex.Message,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                                        );
                        return false;
                    }
                }
            }
            else
            {
                programType = MainWindow_Rufous.g_settingData.m_programType;

                try
                {
                    if (programType == "Paint")
                    {
                        string processFilename = @"C:\Windows\System32\mspaint.exe";

                        foreach (string f in FileList)
                        {
                            ProcessStartInfo info = new ProcessStartInfo();

                            info.FileName = processFilename;
                            info.Arguments = String.Format("\"{0}\"", f);
                            info.CreateNoWindow = false;
                            info.WindowStyle = ProcessWindowStyle.Normal;
                            info.UseShellExecute = false;

                            Process p = Process.Start(info);
                        }
                      
                    }
                    else if (programType == "OthersApplication")
                    {
                        bool? result = null;
                        OthersAPSelectWin Others = new OthersAPSelectWin();
                        Others.Owner = ParentWin;
                        result = Others.ShowDialog();

                        if (result == true)
                        {
                            programType = Others.m_programType;
                            string path = Others.m_filePath;
                            Process.Start(path);
                        }
                    }
                    else
                    {
                        foreach (string f in FileList)
                        {
                            Process.Start(f);
                        }                      
                    }
                }
                catch (Exception ex)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                     Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_open_p_fail") + ex.Message,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                                    );
                    return false;
                }
            }
          
            return true;
        }

    }
}
