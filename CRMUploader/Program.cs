using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace CRMUploader
{
    public class SendInfo
    {
        public CRM_PrintInfo2 m_printInfo = null;
        public FileInfo m_fileInfo = null;

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
            ReadFile, 
            Send_In_One,
            Send_Separated,
            Exit 
        }

        public static bool IsSendInOne = false;

        public static string crmFolder = System.IO.Path.GetTempPath() + "VOP_CRM";

        ProgramState currentState = ProgramState.CheckIsReady;
        List<SendInfo> infoList = new List<SendInfo>();

        void Execute()
        {
            while (currentState != ProgramState.Exit)
            {
                switch (currentState)
                {
                   
                    case ProgramState.OnIdle:
                        {
                            Trace.WriteLine("CRM Uploader enter sleep.");
                            Thread.Sleep(new TimeSpan(0, 10, 0));
                            currentState = ProgramState.CheckIsReady;
                        }
                        break;
                    case ProgramState.CheckIsReady:
                        {
                            Trace.WriteLine("CRM Uploader check is ready.");
                            if(IsDirectoryFileExist(crmFolder))
                            {
                                if (NetworkInterface.GetIsNetworkAvailable())
                                {
                                    currentState = ProgramState.ReadFile;
                                }
                                else
                                {
                                    currentState = ProgramState.OnIdle;
                                }
                            }
                            else
                            {
                                currentState = ProgramState.Exit;
                            }
                          
                        }
                        break;
                    case ProgramState.ReadFile:
                        {
                            infoList.Clear();

                            IEnumerable<FileInfo> fileList = GetDirectoryFiles(crmFolder);

                            foreach (FileInfo fileInfo in fileList)
                            {
                                CRM_PrintInfo2 rec = null;

                                if (RequestManager.Deserialize<CRM_PrintInfo2>(fileInfo.FullName, ref rec) == true)
                                {
                                    infoList.Add(new SendInfo(rec, fileInfo));
                                }
                            }

                            if(IsSendInOne)
                            {
                                currentState = ProgramState.Send_In_One;
                            }
                            else
                            {
                                currentState = ProgramState.Send_Separated;
                            }

                        }
                        break;
                    case ProgramState.Send_Separated:
                        {
                            bool isNetWorkReachable = true;

                            foreach (SendInfo info in infoList)
                            {
                             
                                JSONResultFormat2 rtValue = new JSONResultFormat2();
                                
                                if(RequestManager.UploadCRM_PrintInfo2ToServer(info.m_printInfo, ref rtValue))
                                {
                                    info.m_fileInfo.Delete();
                                    isNetWorkReachable = true;
                                }
                                else
                                {
                                    isNetWorkReachable = false;
                                    break;
                                }
                            }

                            if (isNetWorkReachable == true)
                                currentState = ProgramState.CheckIsReady;
                            else
                                currentState = ProgramState.OnIdle;
                        }
                        break;
                    case ProgramState.Send_In_One:
                        {
                            bool isNetWorkReachable = true;

                            string printData = "[";

                            foreach (SendInfo info in infoList)
                            {
                                printData += info.m_printInfo.m_strPrintData.Trim(new Char[] { '[', ']' }) + ",";
                            }

                            printData = printData.TrimEnd(',');
                            printData += "]";

                            CRM_PrintInfo2 lastRecord = infoList[infoList.Count - 1].m_printInfo;
                            lastRecord.m_strPrintData = printData;

                            JSONResultFormat2 rtValue = new JSONResultFormat2();
                            if(RequestManager.UploadCRM_PrintInfo2ToServer(lastRecord, ref rtValue))
                            {
                                foreach (SendInfo info in infoList)
                                {
                                    info.m_fileInfo.Delete();
                                }

                                isNetWorkReachable = true;
                            }
                            else
                            {
                                isNetWorkReachable = false;
                            }

                            if (isNetWorkReachable == true)
                                currentState = ProgramState.CheckIsReady;
                            else
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
