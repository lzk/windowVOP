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
        private string m_errorMsg = "";
        private string m_password = "";
        private string m_username = "";
        private string m_serverAddress = "";
        private string m_targetPath = "";

        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
            }

            if (FlowType == FtpFlowType.View)
            {
                bool? result = null;
                FtpLoginForm frm = new FtpLoginForm();
                frm.Owner = Application.Current.MainWindow;

                frm.m_serverAddress = MainWindow_Rufous.g_settingData.m_serverAddress;//m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.ServerAddress;
                frm.m_userName = MainWindow_Rufous.g_settingData.m_userName;//m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.UserName;
                frm.m_password = MainWindow_Rufous.g_settingData.m_password;//m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.Password;
                frm.m_targetPath = MainWindow_Rufous.g_settingData.m_targetPath;//m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.TargetPath;

                result = frm.ShowDialog();

                if (result == true)
                {
                    // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.ServerAddress = frm.m_serverAddress;
                    // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.UserName = frm.m_userName;
                    // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.Password = frm.m_password;
                    // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.TargetPath = frm.m_targetPath;

                    m_password = frm.m_password;
                    m_username = frm.m_userName;
                    m_serverAddress = frm.m_serverAddress;
                    m_targetPath = frm.m_targetPath;

                    MainWindow_Rufous.g_settingData.m_serverAddress = m_serverAddress;
                    MainWindow_Rufous.g_settingData.m_userName = m_username;
                    MainWindow_Rufous.g_settingData.m_password = m_password;
                    MainWindow_Rufous.g_settingData.m_targetPath = m_targetPath;

                    //string Uri = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.ServerAddress
                    //    + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.TargetPath;
                    string Uri = m_serverAddress + m_targetPath;

                    if ((Uri.Length + FileList[0].Length) > 260)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                    Application.Current.MainWindow,
                     (string)"Your Specify the Server Address and Tartget Path are too long!",
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                    );
                        return false;
                    }

                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                    if (worker.InvokeQuickScanMethod(UploadFiles, (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ftp_wait")))
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                    Application.Current.MainWindow,
                                     (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ok"),
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt")
                                    );
                    }
                    else
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                        Application.Current.MainWindow,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail") + m_errorMsg,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                        return false;
                    }

                }
            }
            else
            {
                m_serverAddress = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.ServerAddress;
                m_username = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.UserName;
                m_password = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.Password;
                m_targetPath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.TargetPath;

                string Uri = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.ServerAddress
                    + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.TargetPath;

                if ((Uri.Length + FileList[0].Length) > 260)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                Application.Current.MainWindow,
                 (string)"Your Specify the Server Address and Tartget Path are too long!",
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                );
                    return false;
                }

                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeQuickScanMethod(UploadFiles, (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_ftp_wait")))
                {
                   
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                    Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail") + m_errorMsg,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                    return false;
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
                    m_username, m_password);
                       // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.UserName,
                        //MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.Password);

                ftpReq.Proxy = null;

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

                    client.Proxy = null;
                    client.Credentials = new NetworkCredential(
                        m_username, m_password);
                    // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.UserName,
                    // MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.Password);

                    string targetUri = m_serverAddress + m_targetPath;//MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.ServerAddress 
                       // + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FTPScanSettings.TargetPath;
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
