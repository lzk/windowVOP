using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using Google.Apis.Helper;

namespace VOP
{
    class Googledocsflow
    {
        public List<string> FileList { get; set; }
        public static CloudFlowType FlowType = CloudFlowType.View;
        public static string SavePath = "";
        public static string FolderID = "";
        public string m_errorMsg = "";
        public bool isCancel = false;
        private static bool isReset = false;

        //public static MainWindow mainWindow;
        /// <summary>
        /// The remote service on which all the requests are executed.
        /// </summary>
        public static DriveService _service { get; private set; }

        /// <summary>
        /// Creates a concrete instance of the IAuthenticator to authenticate the user against Google OAuth2.0
        /// </summary>
        /// <returns></returns>
        private static IAuthenticator CreateAuthenticator()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = ClientCredentials.CLIENT_ID;
            provider.ClientSecret = ClientCredentials.CLIENT_SECRET;
            return new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
        }

        /// <summary>
        /// Method to get the authorization from the user to access their Google Drive from the application
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static IAuthorizationState GetAuthorization(NativeApplicationClient client)
        {
            Win32.OutputDebugString("GetAuthorization===Enter");
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            const string STORAGE = "faore_vop";
            const string KEY = "z},drdzf11x9;89";
            IAuthorizationState state = null;
            try
            {
                Win32.OutputDebugString("GetStringValue");
                string scope = DriveService.Scopes.Drive.GetStringValue();

                // Check if there is a cached refresh token available.                

                if (!isReset)
                {
                    Win32.OutputDebugString("GetCachedRefreshToken");
                    state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
                    if (state != null)
                    {
                        Win32.OutputDebugString("state not null");
                        try
                        {
                            client.RefreshToken(state);
                            return state; // Yes - we are done.
                        }
                        catch (DotNetOpenAuth.Messaging.ProtocolException ex)
                        {
                            Win32.OutputDebugString("Using existing refresh token failed: " + ex.Message);
                        }
                    }
                }
                Win32.OutputDebugString("RequestNativeAuthorization");
                // If we get here, there is no stored token. Retrieve the authorization from the user.
                state = AuthorizationMgr.RequestNativeAuthorization(client, scope);
                Win32.OutputDebugString("Save key to file!");
                AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
                Win32.OutputDebugString("save success!");
            }
            catch(Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                System.Windows.Application.Current.MainWindow,
                (string)"Connect to google drive fail" + ex.Message,
                   (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
            }

            Win32.OutputDebugString("GetAuthorization===Leave");
            return state;
        }

        /// <summary>
        /// This is the worker method that executes when the user clicks the GO button.
        /// It illustrates the workflow that would need to take place in an actual application.
        /// </summary>
        public bool AuthorizeAndUpload()
        {
            Win32.OutputDebugString("AuthorizeAndUpload===>Enter");
            try
            {
                // First, create a reference to the service you wish to use.
                // For this app, it will be the Drive service. But it could be Tasks, Calendar, etc.
                // The CreateAuthenticator method is passed to the service which will use that when it is time to authenticate
                // the calls going to the service.
                Win32.OutputDebugString("CreateAuthenticator");
                _service = new DriveService(CreateAuthenticator());

                Win32.OutputDebugString("Get a listing of the existing files...");
                // Get a listing of the existing files...
                List<File> fileList = Utilities.RetrieveAllFiles(_service);

                if (fileList.Count == 0 &&
                    FlowType == CloudFlowType.SimpleView)
                {
                    Win32.OutputDebugString("file count is 0 and simpleview, return");
                    return false;
                }
                if (FlowType == CloudFlowType.Quick)
                {
                    Win32.OutputDebugString("Quck Scan: to google drive");

                    string folderID = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.GoogleDriveFolderID;
                    isReset = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.NeedReset;
                    if (fileList.Count > 0)
                    {
                        // Set a flag to keep track of whether the file already exists in the drive
                        List<File> fileExists = new List<File>();
                        bool fileExist = false;

                        foreach (string filepath in FileList)
                        {
                            foreach (File item in fileList)
                            {
                                if (item.OriginalFilename == System.IO.Path.GetFileName(filepath))
                                {
                                    // File exists in the drive already!
                                    fileExists.Add(item);
                                    fileExist = true;
                                    break;
                                }
                            }

                            if (!fileExist)
                            {
                                File fileItem = new File();
                                fileItem.Id = "";
                                fileExists.Add(fileItem);
                            }

                        }

                        if (fileExist)
                        {
                            string message = (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_File_exist_Do_You_overwrite");
                            VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                                System.Windows.Application.Current.MainWindow,
                             message,
                             (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Warning"));

                            if (ret == VOP.Controls.MessageBoxExResult.Yes)
                            {
                                int i = 0;
                                foreach (string filepath in FileList)
                                {
                                    // Yes... overwrite the file
                                    File item = fileExists[i];
                                    if (item.Id != "")
                                    {
                                        Utilities.UpdateFile(_service, item.Id, item.Title, item.Description, item.MimeType, filepath, true);
                                    }
                                    else
                                    {
                                        Utilities.InsertFile(_service, System.IO.Path.GetFileName(filepath), "Scanning Image", folderID, "image/jpeg", filepath);
                                    }
                                    i++;
                                }
                            }

                            else if (ret == VOP.Controls.MessageBoxExResult.No)
                            {

                                int i = 0;
                                foreach (string filepath in FileList)
                                {
                                    // MessageBoxResult.No code here
                                    File item = fileExists[i];
                                    if (item.Id == "")
                                    {
                                        Utilities.InsertFile(_service, System.IO.Path.GetFileName(filepath), "Scanning Image", folderID, "image/jpeg", filepath);
                                    }
                                    i++;
                                }
                            }

                            else
                            {
                                // MessageBoxResult.Cancel code here
                                return false;
                            }

                        }
                        // Check to see if the file existed. If not, it is a new file and must be uploaded.
                        else
                        {
                            foreach (string filepath in FileList)
                            {
                                Utilities.InsertFile(_service, System.IO.Path.GetFileName(filepath), "Scanning Image", folderID, "image/jpeg", filepath);
                            }
                        }
                    }
                    else
                    {
                        foreach (string filepath in FileList)
                        {
                            Utilities.InsertFile(_service, System.IO.Path.GetFileName(filepath), "Scanning Image", "", "image/jpeg", filepath);
                        }
                    }
                }
                else
                {
                    Win32.OutputDebugString("scan to google drive");
                    isReset = MainWindow_Rufous.g_settingData.m_bNeedReset;
                    bool? result = null;
                    GoogleDriveFileViewer viewer = new GoogleDriveFileViewer(fileList, FileList, MainWindow_Rufous.g_settingData.m_dropBoxDefaultPath, FlowType);
                    viewer.Owner = System.Windows.Application.Current.MainWindow;
                    viewer.Service = _service;
                    result = viewer.ShowDialog();
                    MainWindow_Rufous.g_settingData.m_bNeedReset = false;
                }
            }
            catch (Exception ex)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                System.Windows.Application.Current.MainWindow,
                (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail"),
                   (string)System.Windows.Application.Current.MainWindow.TryFindResource("ResStr_Warning"));

                return false;

            }
            Win32.OutputDebugString("AuthorizeAndUpload===>Leave");
            return true;
            //System.Windows.MessageBox.Show("Upload Complete");
        }
        public bool Run()
        {
            isCancel = false;

            if (FileList == null || FileList.Count == 0)
            {
                if (FlowType != CloudFlowType.SimpleView)
                    return false;
            }
            Win32.OutputDebugString("Scan to Google Drive RUn()");
            if (FlowType == CloudFlowType.Quick)
            {
                isReset = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.NeedReset;
            }
            else
            {
                isReset = MainWindow_Rufous.g_settingData.m_bNeedReset;
            }
            try
            {
                AuthorizeAndUpload();
            }
            catch (Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
                return false;
            }
         
            return true;
        }
    }
}