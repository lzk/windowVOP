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


namespace VOP.Controls
{
    public class DropBoxFlow
    {
        private const string ApiKey = "odlfrurcifss4yc";
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }

        public int Run()
        {
            if (FileList == null || FileList.Count == 0)
                return 1;

            DropboxCertHelper.InitializeCertPinning();

            var accessToken = this.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return 1;
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
                RunUploadTask(client);

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

            return 0;
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

          //  Properties.Settings.Default.Reset();
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
