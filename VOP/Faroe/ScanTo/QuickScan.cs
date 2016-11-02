using System;
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
            flow.Run();

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
            flow.Run();

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
            flow.Run();

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
            flow.Run();

            return true;
        }

        public bool ScanToCloud()
        {
            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_cloudScanSettings);

            //List<ScanFiles> files = new List<ScanFiles>();
            //files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_A00.JPG"));
            //files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_A01.JPG"));
            //files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_A02.JPG"));
            //files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_B00.JPG"));
            //files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_B01.JPG"));
            //files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_B02.JPG"));

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
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                               Application.Current.MainWindow,
                              "Upload completed",
                              "Prompt");
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
