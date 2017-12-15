using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using System.Threading;

using System.Net.Http.Headers;
using Microsoft.Graph;
using Microsoft.Identity.Client;


namespace VOP
{   
    public enum ClientType
    {
        Consumer,
        Business
    };
    public class OneDriveFlow
    {
        private const string ApiKey = "odlfrurcifss4yc";
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }
        public MainWindow_Rufous m_MainWin { get; set; }
        public static CloudFlowType FlowType = CloudFlowType.View; 
        public static string SavePath = "";
        
        public bool isCancel = false;
        public const string MsaClientId = "4f54ce63-d8e7-48c1-992f-f4072a6e5945";
        public const string MsaReturnUrl = "urn:ietf:wg:oauth:2.0:oob";


        public static string[] Scopes = { "Files.ReadWrite.All" };

        public static PublicClientApplication IdentityClientApp = new PublicClientApplication(MsaClientId);

        public static string TokenForUser = null;
        public static DateTimeOffset Expiration;

//        private static GraphServiceClient m_client = null;

        private const int UploadChunkSize = 10 * 1024 * 1024;       // 10 MB
        //private IOneDriveClient oneDriveClient { get; set; }
        private GraphServiceClient client { get; set; }
        private ClientType clientType { get; set; }
        private DriveItem CurrentFolder { get; set; }
        private DriveItem SelectedItem { get; set; }

        public bool isSigin = false;

//        private OneDriveTile _selectedTile;

        // Get an access token for the given context and resourceId. An attempt is first made to 
        // acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
     


        /// <summary>
        /// Signs the user out of the service.
        /// </summary>
               
        private static void PresentServiceException(Exception exception)
        {
            string message = null;
            var oneDriveException = exception as ServiceException;
            if (oneDriveException == null)
            {
                message = exception.Message;
            }
            else
            {
                message = string.Format("{0}{1}", Environment.NewLine, oneDriveException.ToString());
            }

            MessageBox.Show(string.Format("OneDrive reported the following error: {0}", message));
        }
        public bool Run()
        {
            isCancel = false;

            if (FileList == null || FileList.Count == 0)
            {
                if(FlowType != CloudFlowType.SimpleView)
                    return false;
            }
            try
            {
                this.client = AuthenticationHelper.GetAuthenticatedClient();
            }
            catch (ServiceException exception)
            {
                PresentServiceException(exception);
            }    
          
            try
            {
                if (FlowType == CloudFlowType.Quick)
                {
                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                    return worker.InvokeQuickScanMethod(RunUpload, (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_wait"));
                }
                else
                {
                    if (RunSignIn() && isSigin)
                        RunUploadTask(client);
                    else
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            ParentWin,
                    (string)ParentWin.TryFindResource("ResStr_Connect_OneDrive_Fail"),//"Connection Onedirive failed!", 
                    (string)ParentWin.TryFindResource("ResStr_Warning"));
                    }
                }
            

                // Tests below are for Dropbox Business endpoints. To run these tests, make sure the ApiKey is for
                // a Dropbox Business app and you have an admin account to log in.

                /*
                var client = new DropboxTeamClient(accessToken, userAgent: "SimpleTeamTestApp", httpClient: httpClient);
                await RunTeamTests(client);
                */
            }
            catch (HttpException)
            {
             
            }

            return true;
        }
        private bool RunSignIn()
        {
            try
            {
                var task = Task.Run((Func<Task<bool>>)SignIn);
                task.Wait();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void RunUploadTask(GraphServiceClient client)
        {
            try
            {
                var viewr = new OneDriveFileViewer(client, FileList,CurrentFolder);
                viewr.Owner = ParentWin;
                viewr.ShowDialog();
            }
            catch (Exception e)
            {
            }          
        }
       
        private string GetAccessToken()
        {

            string accessToken = "";
            string uId = "";

            accessToken = Properties.Settings.Default.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
            {
                try
                {
                      
                    var login = new LoginForm(ApiKey);
                    login.Owner = ParentWin;
                    login.ShowDialog();

                    if (login.Result)
                    {
                        accessToken = login.AccessToken;
                        uId = login.Uid;
                    }
                    else
                    {
                        isCancel = true;
                    }
                   
                    Properties.Settings.Default.AccessToken = accessToken;
                    Properties.Settings.Default.Uid = uId;

                    Properties.Settings.Default.Save();

                }
                catch (Exception ex)
                {

                }
            }

            return accessToken;
        }

        private async Task<bool> SignIn()
        {
            if (null == this.client)
            {
                isSigin = false;
                return false;
            }
            try
            {
                DriveItem folder;

                var expandValue = this.clientType == ClientType.Consumer
                    ? "thumbnails,children($expand=thumbnails)"
                    : "thumbnails,children";

                folder = await this.client.Drive.Root.Request().Expand(expandValue).GetAsync();

                CurrentFolder = folder;
                isSigin = true;
                return true;
 //               RunUploadTask(client,folder);
            }
            catch (Exception exception)
            {
                //    PresentServiceException(exception);
                //MessageBox.Show("Connection OneDrive failed!");
                isSigin = false;
                return false;
            }
        }
        private bool RunUpload()
        {
            try
            {
                var task = Task.Run((Func<Task<bool>>)UploadFilesToDefaultPath);
                task.Wait();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
       
        private async Task<bool> UploadFilesToDefaultPath()
        {   
            if (FileList == null)
                return false;

            foreach (string filePath in FileList)
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                await Upload(client, MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.DefaultOneDrivePath, filePath);
            }

            return true;
        }

        private async Task Upload(GraphServiceClient client,string defaultPath, string filePath)
        {
            if (defaultPath != "/")
            {
                await LoadFolderFromPath(defaultPath);
            }
            else
            {
                await LoadFolderFromPath();
            }
            var targetFolder = CurrentFolder;
            string Filename = "";
            using (var stream = GetFileStreamForUpload(targetFolder.Name, filePath, out Filename))
            {
                if (stream != null)
                {
                    try
                    {
                        var uploadedItem =
                            await
                                this.client.Drive.Items[targetFolder.Id].ItemWithPath(Filename).Content.Request()
                                    .PutAsync<DriveItem>(stream);
                    }
                    catch (Exception exception)
                    {
                        //PresentServiceException(exception);
                    }
                }
            }
        }
        private async Task LoadFolderFromPath(string path = null)
        {
            if (null == this.client) return;
            // Update the UI for loading something new
            try
            {
                DriveItem folder;

                var expandValue = this.clientType == ClientType.Consumer
                    ? "thumbnails,children($expand=thumbnails)"
                    : "thumbnails,children";

                if (path == null)
                {
                    folder = await this.client.Drive.Root.Request().Expand(expandValue).GetAsync();
                }
                else
                {
                    folder =
                        await
                            this.client.Drive.Root.ItemWithPath("/" + path)
                                .Request()
                                .Expand(expandValue)
                                .GetAsync();
                }

                CurrentFolder = folder;
            }
            catch (Exception exception)
            {
                //PresentServiceException(exception);
            }

        }
        private System.IO.Stream GetFileStreamForUpload(string targetFolderName, string filename, out string originalFilename)
        {
            try
            {
                originalFilename = System.IO.Path.GetFileName(filename);
                return new System.IO.FileStream(filename, System.IO.FileMode.Open);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error uploading file: " + ex.Message);
                originalFilename = null;
                return null;
            }
        }
    }
}
