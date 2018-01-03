using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Printing;
using System.Drawing.Printing;

namespace VOP
{
    public class QuickScan
    {
        public QuickScan()
        {

        }

        private void DebugAddScanFiles(ref List<ScanFiles> files)
        {
            files = new List<ScanFiles>();
            files.Add(new ScanFiles(@"G:\work\Rufous\pic\0592995421_C200_A00.JPG"));
            files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00_180.JPG"));
            files.Add(new ScanFiles(@"G:\work\Rufous\pic\temp-000.JPG"));
        }

        public Scan_RET ScanToAP()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings);
            
            if (files == null)
                return task.ScanResult;

            if (task.ScanResult != Scan_RET.RETSCAN_OK)
                return task.ScanResult;

            List<string> fileLs = new List<string>();

            foreach (ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            APFlow flow = new APFlow();
            flow.ParentWin = Application.Current.MainWindow;
            flow.FileList = fileLs;
            APFlow.FlowType = APFlowType.Quick;
            flow.Run();

            return Scan_RET.RETSCAN_OK;
        }

        public Scan_RET ScanToFile()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings);
            //DebugAddScanFiles(ref files);
            if (files == null)
                return task.ScanResult;

            if (task.ScanResult != Scan_RET.RETSCAN_OK)
                return task.ScanResult;

            List<string> fileLs = new List<string>();

            foreach (ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            FileFlow flow = new FileFlow();
            flow.ParentWin = Application.Current.MainWindow;
            flow.FileList = fileLs;
            FileFlow.FlowType = FileFlowType.Quick;

            if(flow.Run())
            {
                ScanPreview_Rufous win = new ScanPreview_Rufous();
                win.Owner = Application.Current.MainWindow;
                win.ImagePaths = fileLs;
                win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_save_file_ok");
                win.ShowDialog();
            }

            return Scan_RET.RETSCAN_OK;
        }


        public Scan_RET ScanToEmail()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings);

            if (files == null)
                return task.ScanResult;

            if (task.ScanResult != Scan_RET.RETSCAN_OK)
                return task.ScanResult;

            List<string> fileLs = new List<string>();

            foreach (ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            EmailFlow flow = new EmailFlow();
            flow.ParentWin = Application.Current.MainWindow;
            flow.FileList = fileLs;
            EmailFlow.FlowType = EmailFlowType.Quick;

            if (flow.Run())
            {
                ScanPreview_Rufous win = new ScanPreview_Rufous();
                win.Owner = Application.Current.MainWindow;
                win.ImagePaths = fileLs;
                win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_ok");
                win.ShowDialog();
            }

            return Scan_RET.RETSCAN_OK;
        }

        public Scan_RET ScanToPrint()
        {
            if (MainWindow_Rufous.g_settingData.m_printerName == null)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                 Application.Current.MainWindow,
                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Not_Find_Printer"),//"Not find printer!",
                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return Scan_RET.RETSCAN_ERROR;
            }
            else
            {
                ScanTask task = new ScanTask();
                List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings);

                if (files == null)
                    return task.ScanResult;

                if (task.ScanResult != Scan_RET.RETSCAN_OK)
                    return task.ScanResult;

                List<string> fileLs = new List<string>();

                foreach (ScanFiles f in files)
                {
                    fileLs.Add(f.m_pathOrig);
                }

                //modified by yunying shang 2017-12-11 for BMS 1743
                List<string> listPrinters = new List<string>();
                string strDefaultPrinter = string.Empty;
                try
                {
                    PrinterSettings settings = new PrinterSettings();
                    PrintServer myPrintServer = new PrintServer(null);
                    PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                    
                    foreach (PrintQueue pq in myPrintQueues)
                    {
                        PrintDriver queuedrv = pq.QueueDriver;

                        listPrinters.Add(pq.Name);

                        settings.PrinterName = pq.Name;

                        if (settings.IsDefaultPrinter)
                        {
                            strDefaultPrinter = pq.Name;
                        }
                    }
                }
                catch (Exception ex)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                     Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Find_Printer_Error") + ex.Message,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return Scan_RET.RETSCAN_ERROR;
                }//<<==================1743

                if (listPrinters.Count < 1)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                     Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Not_Find_Printer"),//"Not find printer!",
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                }
                else
                { 
                    PrintFlow flow = new PrintFlow(strDefaultPrinter);
                    flow.ParentWin = Application.Current.MainWindow;
                    flow.FileList = fileLs;
                    PrintFlow.FlowType = PrintFlowType.Quick;

                    if (flow.Run())
                    {
                        ScanPreview_Rufous win = new ScanPreview_Rufous();
                        win.Owner = Application.Current.MainWindow;
                        win.ImagePaths = fileLs;
                        win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_print_ok");
                        win.ShowDialog();
                    }
                }


                return Scan_RET.RETSCAN_OK;
            }
        }

        public Scan_RET ScanToFtp()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings);

            if (files == null)
                return task.ScanResult;

            if (task.ScanResult != Scan_RET.RETSCAN_OK)
                return task.ScanResult;

            List<string> fileLs = new List<string>();

            foreach (ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            FtpFlow flow = new FtpFlow();
            flow.ParentWin = Application.Current.MainWindow;
            flow.FileList = fileLs;
            FtpFlow.FlowType = FtpFlowType.Quick;

            if (flow.Run())
            {
                ScanPreview_Rufous win = new ScanPreview_Rufous();
                win.Owner = Application.Current.MainWindow;
                win.ImagePaths = fileLs;
                win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ok");
                win.ShowDialog();
            }

            return Scan_RET.RETSCAN_OK;
        }

        public Scan_RET ScanToCloud()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_ScanSettings);
            //DebugAddScanFiles(ref files);
            if (files == null)
                return task.ScanResult;

            if (task.ScanResult != Scan_RET.RETSCAN_OK)
                return task.ScanResult;

            List<string> fileLs = new List<string>();

            foreach(ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.SaveType == "DropBox")
            {
                DropBoxFlow flow = new DropBoxFlow();
                flow.ParentWin = Application.Current.MainWindow;
                DropBoxFlow.FlowType = CloudFlowType.Quick;
                flow.FileList = fileLs;

                if (flow.Run())
                {
                    //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                    //               Application.Current.MainWindow,
                    //              "Upload completed",
                    //              "Prompt");

                    ScanPreview_Rufous win = new ScanPreview_Rufous();
                    win.Owner = Application.Current.MainWindow;
                    win.ImagePaths = fileLs;
                    win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ok");
                    win.ShowDialog();
                }
                else
                {
                    //if (flow.isCancel != true)
                    //    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    //                    Application.Current.MainWindow,
                    //                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail"),
                    //                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return Scan_RET.RETSCAN_ERROR;
                }
            }
            else if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.SaveType == "EverNote")
            {
                EvernoteFlow flow = new EvernoteFlow();
                flow.ParentWin = Application.Current.MainWindow;
                EvernoteFlow.FlowType = CloudFlowType.Quick;
                flow.FileList = fileLs;

                if (flow.Run())
                {
                    ScanPreview_Rufous win = new ScanPreview_Rufous();
                    win.Owner = Application.Current.MainWindow;
                    win.ImagePaths = fileLs;
                    win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ok");
                    win.ShowDialog();
                }
                else
                {
                    if (flow.isCancel != true)
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                        Application.Current.MainWindow,
                                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail"),
                                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return Scan_RET.RETSCAN_ERROR;
                }
            }
            else if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.SaveType == "OneDrive")
            {
                OneDriveFlow flow = new OneDriveFlow();
                flow.ParentWin = Application.Current.MainWindow;
                OneDriveFlow.FlowType = CloudFlowType.Quick;
                flow.FileList = fileLs;

                if (flow.Run())
                {
                    ScanPreview_Rufous win = new ScanPreview_Rufous();
                    win.Owner = Application.Current.MainWindow;
                    win.ImagePaths = fileLs;
                    win.messageBlock.Text = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ok");
                    win.ShowDialog();
                }
                else
                {
                    if (flow.isCancel != true)
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                        Application.Current.MainWindow,
                                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail"),
                                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return Scan_RET.RETSCAN_ERROR;
                }
            }
            return Scan_RET.RETSCAN_OK;
        }
    }
}
