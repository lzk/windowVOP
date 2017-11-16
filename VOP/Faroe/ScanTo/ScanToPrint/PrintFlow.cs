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
using System.Windows.Interop;

namespace VOP
{
    public enum PrintFlowType
    {
        View,
        Quick,
    };

    public enum DuplexPrintType
    {
        NonDuplex,
        FlipOnLongEdge,
        FlipOnShortEdge
    }

    public class PrintFlow
    {
        public static PrintFlowType FlowType = PrintFlowType.View;
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }

        string m_errorMsg = "";

        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
            }

            PrintError printRes = PrintError.Print_OK;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if(FlowType == PrintFlowType.View)
            {
                if (dll.PrintInitDialog("VOP_Print", (new WindowInteropHelper(App.Current.MainWindow)).Handle))
                {

                    foreach (string path in FileList)
                    {
                        dll.AddImagePath(path);
                    }

                    printRes = (PrintError)worker.InvokeDoWorkMethod(dll.DoPrintImage, 
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_print_pic_wait")
                        );
                }
                else
                {
                    printRes = PrintError.Print_Operation_Fail;
                }
            }
            else
            {
                if (dll.PrintInit(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_PrintScanSettings.PrinterName, "VOP_Print",
                        (int)0, new IdCardSize(), true, (int)DuplexPrintType.NonDuplex, true, 100))
                {

                    foreach (string path in FileList)
                    {
                        dll.AddImagePath(path);
                    }

                    printRes = (PrintError)worker.InvokeDoWorkMethod(dll.DoPrintImage,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_print_pic_wait")
                        );
                }
                else
                {
                    printRes = PrintError.Print_Operation_Fail;
                }
            }
            

            if (printRes == PrintError.Print_OK)
            {
                //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                //                         Application.Current.MainWindow,
                //                        "Print completed",
                //                        "Prompt");
            }
            else if (printRes == PrintError.Print_Operation_Fail)
            {
             
            }
            else
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                            Application.Current.MainWindow,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_print_fail") + m_errorMsg,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                           );
                return false;
            }

            return true;
        }

    }
}
