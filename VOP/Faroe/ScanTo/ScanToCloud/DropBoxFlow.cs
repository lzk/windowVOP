using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using System.Threading;


namespace VOP
{
    public enum CloudFlowType
    {
        View,
        SimpleView,
        Quick,
    };

    public class DropBoxFlow
    {
        private const string ApiKey = "odlfrurcifss4yc";
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }
        public static CloudFlowType FlowType = CloudFlowType.View; 
        public static string SavePath = "";
        DropboxClient m_client = null;

        public bool isCancel = false;

        public bool Run()
        {
            isCancel = false;

            if (FileList == null || FileList.Count == 0)
            {
                if(FlowType != CloudFlowType.SimpleView)
                    return false;
            }
                
            DropboxCertHelper.InitializeCertPinning();

            var accessToken = this.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            IWebProxy webProxy = WebRequest.DefaultWebProxy;
            if (null != webProxy)
            {
                webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            }

            // Specify socket level timeout which decides maximum waiting time when on bytes are
            // received by the socket.
            var httpClient = new HttpClient(new WebRequestHandler
            {
                ReadWriteTimeout = 10 * 1000,
                Proxy = webProxy,
                Credentials = CredentialCache.DefaultCredentials
            })
            {
                // Specify request level timeout which decides maximum time taht can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };
        
            try
            {
                var config = new DropboxClientConfig("SimpleTestApp")
                {
                    HttpClient = httpClient,
                };

                var client = new DropboxClient(accessToken, config);
                m_client = client;

                if (FlowType == CloudFlowType.Quick)
                {
                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                    return worker.InvokeQuickScanMethod(RunUpload, "Uploading picture to DropBox, please wait...");
                }
                else
                {
                    RunUploadTask(client);
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


        private void RunUploadTask(DropboxClient client)
        {
            //await GetCurrentAccount(client);

            //var path = "/DotNetApi/Help";
            //var folder = await CreateFolder(client, path);
            //var list = await ListFolder(client, path);

            //var firstFile = list.Entries.FirstOrDefault(i => i.IsFile);
            //if (firstFile != null)
            //{
            //    await Download(client, path, firstFile.AsFile);
            //}

            //await Upload(client, path, "Test.txt", "This is a text file");

            //await ChunkUpload(client, path, "Binary");

          
            try
            {
                var viewr = new FileViewer(client, FileList);
                viewr.Owner = ParentWin;
                viewr.ShowDialog();
            }
            catch (Exception e)
            {

            }
          
        }

      
        //private async Task RunTeamTests(DropboxTeamClient client)
        //{
        //    var members = await client.Team.MembersListAsync();

        //    var member = members.Members.FirstOrDefault();

        //    if (member != null)
        //    {
        //        // A team client can perform action on a team member's behalf. To do this,
        //        // just pass in team member id in to AsMember function which returns a user client.
        //        // This client will operates on this team member's Dropbox.
        //        var userClient = client.AsMember(member.Profile.TeamMemberId);
        //        RunUploadTask(userClient);
        //    }
        //}

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
                await Upload(m_client, MainWindow_Rufous.g_settingData.m_dropBoxDefaultPath, fileName, filePath);
            }

            return true;
        }


        private async Task Upload(DropboxClient client, string folder, string fileName, string fileContent)
        {

            using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(fileContent)))
            {
                var response = await client.Files.UploadAsync(folder + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);
            }
        }

        private async Task Download(DropboxClient client, string folder, FileMetadata file)
        {

            using (var response = await client.Files.DownloadAsync(folder + "/" + file.Name))
            {

            }
        }

        //private async Task<MembersListResult> ListTeamMembers(DropboxTeamClient client)
        //{
        //    var members = await client.Team.MembersListAsync();

        //    foreach (var member in members.Members)
        //    {

        //    }

        //    return members;
        //}
    }
}
