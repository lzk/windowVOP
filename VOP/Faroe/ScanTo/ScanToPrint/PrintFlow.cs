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

    public enum PrintStatus
    {
        Printer_Initial_Fail = -30,
        Printer_Restart = -29,
        Printer_No_Toner,
        Printer_Need_Update,
        Printer_Server_Unknown,
        Printer_User_intervention,
        Printer_Page_Punt,
        Printer_Warning_Up,
        Printer_Initializing,
        Printer_Processing,
        Printer_Printing,
        Printer_IO_Active,
        Printer_Manual_Feed,
        Printer_Pending_Deletion,
        Printer_Power_Save,
        Printer_Door_Open,
        Printer_OffLine,
        Printer_Out_Of_Memory,
        Printer_Toner_Low,
        Printer_Not_Avalable,
        Printer_Bin_Full,
        Printer_Paper_Problem,
        Printer_Paper_Out,
        Printer_Paper_Jam,
        Printer_Error,
        Printer_Server_Offline,
        Printer_Waiting,
        Printer_Busy,
        Printer_Pause,
        Printer_Work_Offline,
        Printer_Idle,
    }

    public class PrintFlow
    {
        public static PrintFlowType FlowType = PrintFlowType.View;
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }
        private string defaultPrinter = "";

        string m_errorMsg = "";

        public PrintFlow(string printer)
        {
            defaultPrinter = printer;
        }

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
                int ret = dll.PrintInitDialog("VOP_Print", (new WindowInteropHelper(App.Current.MainWindow)).Handle);
                PrintStatus status = (PrintStatus)ret;
                if (status != PrintStatus.Printer_Initial_Fail &&
                    status != PrintStatus.Printer_Work_Offline &&
                        status != PrintStatus.Printer_Error &&
                        status != PrintStatus.Printer_No_Toner &&
                        status != PrintStatus.Printer_Not_Avalable &&
                        status != PrintStatus.Printer_OffLine &&
                        status != PrintStatus.Printer_Out_Of_Memory &&
                        status != PrintStatus.Printer_Paper_Jam &&
                        status != PrintStatus.Printer_Paper_Out &&
                        status != PrintStatus.Printer_Server_Offline &&
                        status != PrintStatus.Printer_Paper_Problem &&
                        status != PrintStatus.Printer_Work_Offline)
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
                    //modified by yunying shang 2017-11-27 for BMS 1553
                    if (status == PrintStatus.Printer_Initial_Fail)
                        printRes = PrintError.Print_Get_Default_Printer_Fail;
                    else
                        printRes = PrintError.Print_Operation_Fail;
                    //<<============1553
                }
            }
            else
            {
                string PrinterName = string.Empty;

                PrinterName = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_PrintScanSettings.PrinterName;

                if (PrinterName == "")
                {
                    if (defaultPrinter == "")
                        PrinterName = MainWindow_Rufous.g_printerList[0];
                    else
                        PrinterName = defaultPrinter;
                }

                int ret = dll.CheckPrinterStatus(PrinterName);

                PrintStatus status = (PrintStatus)ret;
                if (status != PrintStatus.Printer_Initial_Fail &&
                    status != PrintStatus.Printer_Work_Offline &&
                        status != PrintStatus.Printer_Error &&
                        status != PrintStatus.Printer_No_Toner &&
                        status != PrintStatus.Printer_Not_Avalable &&
                        status != PrintStatus.Printer_OffLine &&
                        status != PrintStatus.Printer_Out_Of_Memory &&
                        status != PrintStatus.Printer_Paper_Jam &&
                        status != PrintStatus.Printer_Paper_Out &&
                        status != PrintStatus.Printer_Server_Offline &&
                        status != PrintStatus.Printer_Paper_Problem &&
                        status != PrintStatus.Printer_Work_Offline)
                {
                    if (dll.PrintInit(PrinterName, "VOP_Print",
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
                        printRes = PrintError.Print_Get_Default_Printer_Fail;
                    }
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
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                           Application.Current.MainWindow,
                          (string)"Printer is not ready!",
                          (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                          );
                return false;
            }
            //modified by yunying shang 2017-11-27 for BMS 1553
            else if (printRes == PrintError.Print_Get_Default_Printer_Fail)
            {
            }//<<=================1553
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
