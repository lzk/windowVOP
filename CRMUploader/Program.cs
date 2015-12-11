using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;
using VOP;

namespace CRMUploader
{
    public class SendInfo
    {
        public CRM_PrintInfo2 m_printInfo = null;
        public FileInfo m_fileInfo = null;
        public bool IsDirty = false;
        
        public bool CheckedIsDirty(SendInfo obj)
        {
            return IsDirty = (m_printInfo.m_strPrintData != obj.m_printInfo.m_strPrintData)
                || (m_printInfo.m_strMobileNumber != obj.m_printInfo.m_strMobileNumber);
        }

        public bool HasFileInfo()
        {
            return m_fileInfo != null;
        }

        public SendInfo(CRM_PrintInfo2 printInfo, FileInfo fileInfo)
        {
            m_printInfo = printInfo;
            m_fileInfo = fileInfo;
        }
    }

    public class Program
    {
        private enum ProgramState 
        { 
            Init,
            CheckIsReady,
            OnIdle,
            GetData, 
            ReadFile, 
            Send_In_One,
            Send_Separated,
            Exit 
        }

        public static bool IsSendInOne = false;

        const int GEOCLASS_NATION = 16;
        public bool m_bLocationIsChina = false;
        public bool m_crmAgreement = true;

        public static string crmFolder = System.IO.Path.GetTempPath() + "VOP_CRM";

        ProgramState currentState = ProgramState.CheckIsReady;
        Dictionary<string, SendInfo> infoList = new Dictionary<string, SendInfo>();

        public Program()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            DirectoryInfo directory = new DirectoryInfo(documentsPath);
            string strUsersPublic = directory.Parent.FullName;
            crmFolder = strUsersPublic + "\\Lenovo\\VOP_CRM";

            int nGeoID = Win32.GetUserGeoID(GEOCLASS_NATION);
            if (45 == nGeoID)
            {
                m_bLocationIsChina = true;
            }
        }


        void Execute()
        {
            while (currentState != ProgramState.Exit)
            {
                switch (currentState)
                {
                   
                    case ProgramState.OnIdle:
                        {
                            Trace.WriteLine("CRM Uploader: Enter sleep.");
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                            currentState = ProgramState.CheckIsReady;
                        }
                        break;
                    case ProgramState.CheckIsReady:
                        {
                            if (m_bLocationIsChina == false || m_crmAgreement == false)
                            {
                                currentState = ProgramState.Exit;
                                break;
                            }

                            Trace.WriteLine(String.Format("CRM Uploader: CheckIsReady."));                        

                        }
                        break;
                    case ProgramState.GetData:
                        {
                            string strPrinterModel = "";
                            string strDrvName = "";

                            AsyncWorker worker = new AsyncWorker();
                            List<string> listPrinters = new List<string>();

                            common.GetSupportPrinters(listPrinters);

                            foreach(string printerName in listPrinters)
                            {
                                CRM_PrintInfo2 info = new CRM_PrintInfo2();

                                UserCenterInfoRecord rec = worker.GetUserCenterInfo(printerName);

                                Trace.WriteLine(String.Format("CRM Uploader: GetData {0}.", printerName));       

                                if (rec.CmdResult == EnumCmdResult._ACK)
                                {
                                    common.GetPrinterDrvName(printerName, ref strDrvName);

                                    bool isSFP = common.IsSFPPrinter(strDrvName);
                                    bool isWiFiModel = common.IsSupportWifi(strDrvName);

                                    if (isSFP)
                                    {
                                        if (isWiFiModel)
                                            strPrinterModel = "Lenovo LJ2208W";
                                        else
                                            strPrinterModel = "Lenovo LJ2208";
                                    }
                                    else
                                    {
                                        if (isWiFiModel)
                                            strPrinterModel = "Lenovo M7208W";
                                        else
                                            strPrinterModel = "Lenovo M7208";
                                    }

                                    info.AddPrintData(strPrinterModel, rec.SecondSerialNO, rec.SerialNO4AIO, rec.TotalCounter);

                                    if (infoList.ContainsKey(printerName))
                                    {
                                        SendInfo oldData = infoList[printerName];
                                        SendInfo newData = new SendInfo(info, null);
                                        newData.CheckedIsDirty(oldData);
                                        infoList[printerName] = newData;
                                    }
                                    else
                                    {
                                        infoList.Add(printerName, new SendInfo(info, null));
                                    }
                                }
                            }
                       
                            currentState = ProgramState.ReadFile;     
                        }
                        break;
                    case ProgramState.ReadFile:
                        {

                            IEnumerable<FileInfo> fileList = GetDirectoryFiles(crmFolder);

                            foreach (FileInfo fileInfo in fileList)
                            {
                                CRM_PrintInfo2 info = null;

                                if (RequestManager.Deserialize<CRM_PrintInfo2>(fileInfo.FullName, ref info) == true)
                                {
                                    Trace.WriteLine(String.Format("CRM Uploader: ReadFile {0}.", fileInfo.FullName));       
                                    if (infoList.ContainsKey(fileInfo.Name))
                                    {
                                        SendInfo oldData = infoList[fileInfo.Name];
                                        SendInfo newData = new SendInfo(info, fileInfo);
                                        newData.CheckedIsDirty(oldData);
                                        infoList[fileInfo.Name] = newData;
                                    }
                                    else
                                    {
                                        infoList.Add(fileInfo.Name, new SendInfo(info, fileInfo));
                                    }

                                }
                            }

           
                            currentState = ProgramState.Send_Separated;
                            
                        }
                        break;
                    case ProgramState.Send_Separated:
                        {

                            foreach (KeyValuePair<string, SendInfo> item in infoList)
                            {
                                string printerName = item.Key;
                                SendInfo info = item.Value;
                                JSONResultFormat2 rtValue = new JSONResultFormat2();

                                if (info.IsDirty)
                                {
                                    if (RequestManager.UploadCRM_PrintInfo2ToServer(info.m_printInfo, ref rtValue))
                                    {
                                        Trace.WriteLine(String.Format("CRM Uploader: Sended {0}.", printerName));      

                                        if (info.HasFileInfo())
                                            info.m_fileInfo.Delete();
                                    }
                                    else
                                    {
                                        if (!info.HasFileInfo())
                                            RequestManager.Serialize<CRM_PrintInfo2>(info.m_printInfo, crmFolder + @"\" + printerName);
                                    }
                                }
                               
                            }

                            currentState = ProgramState.OnIdle;
                        }
                        break;
                  
                    default:
                        currentState = ProgramState.Exit;
                        break;
                }
            }
        }


        IEnumerable<FileInfo> GetDirectoryFiles(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);

            return dir.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
        }


        bool IsDirectoryFileExist(string dirPath)
        {
            IEnumerable<FileInfo> list = GetDirectoryFiles(dirPath);
            if (list.Count() != 0)
                return true;
            else
                return false;
        }

        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                p.Execute();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch(Exception)
            {

            }
        }
    }
}
