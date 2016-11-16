﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public bool ScanToAP()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_apScanSettings);

            if (files == null)
                return false;
      
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

            return true;
        }

        public bool ScanToFile()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_fileScanSettings);

            if (files == null)
                return false; 

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
                win.messageBlock.Text = "Save files completed";
                win.ShowDialog();
            }

            return true;
        }


        public bool ScanToEmail()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_emailScanSettings);

            if (files == null)
                return false;

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
                win.messageBlock.Text = "Send Email completed";
                win.ShowDialog();
            }

            return true;
        }

        public bool ScanToPrint()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_printScanSettings);

            if (files == null)
                return false;

            List<string> fileLs = new List<string>();

            foreach (ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            PrintFlow flow = new PrintFlow();
            flow.ParentWin = Application.Current.MainWindow;
            flow.FileList = fileLs;
            PrintFlow.FlowType = PrintFlowType.Quick;

            if (flow.Run())
            {
                ScanPreview_Rufous win = new ScanPreview_Rufous();
                win.Owner = Application.Current.MainWindow;
                win.ImagePaths = fileLs;
                win.messageBlock.Text = "Print completed";
                win.ShowDialog();
            }

            return true;
        }

        public bool ScanToFtp()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_ftpScanSettings);

            if (files == null)
                return false;

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
                win.messageBlock.Text = "Upload completed";
                win.ShowDialog();
            }

            return true;
        }

        public bool ScanToCloud()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_cloudScanSettings);

            if (files == null)
                return false;

            List<string> fileLs = new List<string>();

            foreach(ScanFiles f in files)
            {
                fileLs.Add(f.m_pathOrig);
            }

            DropBoxFlow flow = new DropBoxFlow();
            DropBoxFlow.FlowType = CloudFlowType.Quick;
            flow.FileList = fileLs;

            if(flow.Run())
            {
                //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                //               Application.Current.MainWindow,
                //              "Upload completed",
                //              "Prompt");

                ScanPreview_Rufous win = new ScanPreview_Rufous();
                win.Owner = Application.Current.MainWindow;
                win.ImagePaths = fileLs;
                win.messageBlock.Text = "Upload completed";
                win.ShowDialog();
            }
            else
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                               "Upload failed",
                               "Error");
            }

            return true;
        }
    }
}