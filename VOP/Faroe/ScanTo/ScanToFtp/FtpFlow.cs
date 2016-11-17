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

namespace VOP
{
    public enum FtpFlowType
    {
        View,
        Quick,
    };

    public class FtpFlow
    {
        public static FtpFlowType FlowType = FtpFlowType.View;
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }
        ManualResetEvent m_reset = new ManualResetEvent(false);
        string m_errorMsg = "";

        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
            }

            if(FlowType == FtpFlowType.View)
            {
                bool? result = null;
                FtpLoginForm frm = new FtpLoginForm();
                frm.Owner = Application.Current.MainWindow;

                frm.m_serverAddress = MainWindow_Rufous.g_settingData.m_serverAddress;
                frm.m_userName = MainWindow_Rufous.g_settingData.m_userName;
                frm.m_password = MainWindow_Rufous.g_settingData.m_password;
                frm.m_targetPath = MainWindow_Rufous.g_settingData.m_targetPath;

                result = frm.ShowDialog();

                if (result == true)
                {
                    MainWindow_Rufous.g_settingData.m_serverAddress = frm.m_serverAddress;
                    MainWindow_Rufous.g_settingData.m_userName = frm.m_userName;
                    MainWindow_Rufous.g_settingData.m_password = frm.m_password;
                    MainWindow_Rufous.g_settingData.m_targetPath = frm.m_targetPath;

                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                    if (worker.InvokeQuickScanMethod(UploadFiles, "Uploading picture to FTP, please wait..."))
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
                                       "Upload failed. " + m_errorMsg,
                                       "Error");
                    }


                }
            }
            else
            {
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeQuickScanMethod(UploadFiles, "Uploading picture to FTP, please wait..."))
                {
                   
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                    Application.Current.MainWindow,
                                   "Upload failed. " + m_errorMsg,
                                   "Error");
                }

            }
           
            return true;
        }

        void CreateFtpFolder(string targetUri)
        {
            try
            {
                FtpWebRequest ftpReq = WebRequest.Create(targetUri) as FtpWebRequest;
                ftpReq.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpReq.Credentials = new NetworkCredential(
                        MainWindow_Rufous.g_settingData.m_userName,
                        MainWindow_Rufous.g_settingData.m_password);
                FtpWebResponse ftpResp = ftpReq.GetResponse() as FtpWebResponse;
            }
            catch (Exception)
            {

            }
        }

        bool UploadFiles()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(
                        MainWindow_Rufous.g_settingData.m_userName,
                        MainWindow_Rufous.g_settingData.m_password);

                    string targetUri = MainWindow_Rufous.g_settingData.m_serverAddress + MainWindow_Rufous.g_settingData.m_targetPath;
                    CreateFtpFolder(targetUri);

                    foreach (string filePath in FileList)
                    {
                        string fileName = System.IO.Path.GetFileName(filePath);
                        Uri targetFile = new Uri(targetUri + "/" + fileName);
                        client.UploadFile(targetFile, "STOR", filePath);
                    }

                }

            }
            catch (Exception ex)
            {
                m_errorMsg = ex.Message;
                return false;
            }
           
            return true;
        }
    }
}
