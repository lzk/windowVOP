using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Threading;
using EvernoteSDK;
using System.Runtime.InteropServices;

namespace VOP
{
    public class EvernoteFlow
    {
        private const string ApiKey = "yunyingshang-4638";
        private const string ApiSecret = "0988f34d80df967c";
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }
        public static CloudFlowType FlowType = CloudFlowType.View;
        public static string SavePath = "";
        public string m_errorMsg = "";
        private List<string> notelist = new List<string>();

        public bool isCancel = false;
        private bool bReset = false;
        
        private bool connectSuccess = false;

        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);


        public bool Run()
        {        
            isCancel = false;

            if (FileList == null || FileList.Count == 0)
            {
                if (FlowType != CloudFlowType.SimpleView)
                    return false;
            }

            if (FlowType == CloudFlowType.View)
            {
                bReset = MainWindow_Rufous.g_settingData.m_bNeedReset;
            }
            else
            {
                bReset = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.NeedReset;
            }
           
            if (bReset)
            {
                ENSession.SetSharedSessionConsumerKey(ApiKey, ApiSecret);
                ScanToEverNote(true);
                if (connectSuccess == false)
                {
                    //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    //                Application.Current.MainWindow,
                    //                (string)"Connect to EverNote server fail, please confirm you computer setting and your specify user name and password!",
                    //                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return false;
                }
            }
            else
            {
                ENSession.SetSharedSessionConsumerKey(ApiKey, ApiSecret);
                if (ENSession.SharedSession.IsAuthenticated != true)
                {
                    ScanToEverNote(false);
                    if (connectSuccess == false)
                    {
                        return false;
                    }
                }
            }
            // Get a list of all notebooks in the user's account.
            if (FlowType == CloudFlowType.View)
            {
               
                if (GetNoteList())
                {
                    bool? result = null;
                    EverNoteViewer viewer = new EverNoteViewer();
                    viewer.NoteList = notelist;
                    viewer.FileList = FileList;
                    viewer.Owner = Application.Current.MainWindow;
                    result = viewer.ShowDialog();
                    MainWindow_Rufous.g_settingData.m_bNeedReset = false;
                }
            }
            else
            {
                if(UpdateLoadFilesToEverNote())
                {

                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                    Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_upload_fail"),
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return false;
                }

                MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.NeedReset = false;
            }

            return true;
        }

        public bool GetNoteList()
        {
            string textToFind = "*";
            try
            {
                List<ENSessionFindNotesResult> myResultsList = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch(textToFind), null,
                ENSession.SearchScope.All, ENSession.SortOrder.RecentlyUpdated, 500);

                if (myResultsList.Count > 0)
                {
                    foreach (ENSessionFindNotesResult nb in myResultsList)
                    {
                        notelist.Add(nb.Title);
                    }
                }
            }

            catch (Exception ex)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                Application.Current.MainWindow,
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Initial_Evernote_and_Get_Evernote_fail"),//"Initial Evernote and get EverNote list fail!",
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));

                return false;
            }

            return true;

        }

        bool UpdateLoadFilesToEverNote()
        {
            //modified by yunying shang 2017-12-07 for BMS 1623
            if (FileList.Count > 60)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
               Application.Current.MainWindow,
               (string)Application.Current.MainWindow.TryFindResource("ResStr_Evernote_attachment_files_could_not_more_than_60"),//"The ever note attachment files could not be more than 60!",
              (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return false;
            }
            else
            {
                long totalsize = 0;
                foreach (string filePath in FileList)
                {
                    System.IO.FileInfo fileInfo = null;
                    fileInfo = new System.IO.FileInfo(filePath);

                    
                    if (fileInfo.Length > 100 * 1024 * 1024)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                       Application.Current.MainWindow,
                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Images_files_Size_too_large"),//"Total image files size are too large, could not upload to EverNote server!",
                      (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                        return false;
                    }
                    totalsize += fileInfo.Length;
                }
                if (totalsize > 100 * 1024 * 1024)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Images_files_Size_too_large"),//"Total image files size are too large, could not upload to EverNote server!",
                  (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return false;
                }
            }//<<===========1623

            try
            {
                string title = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.EverNoteTitle;
                string content = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_CloudScanSettings.EverNoteContent;
                ENNote myResourceNote = new ENNote();
                foreach (string filePath in FileList)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    byte[] myFile = StreamFile(filePath);
                    // Be sure to replace this with a real JPG file
                    ENResource myResource = new ENResource(myFile, "image/jpg", fileName);//"application/pdf"                 
                    myResourceNote.Resources.Add(myResource);
                }
                myResourceNote.Title = title;// string.Format("Scan to EverNote: {0}", title);//, i);
                content = string.Format("{0}.Attach the scaling Files.", content);
                myResourceNote.Content = ENNoteContent.NoteContentWithString(content);
                ENNoteRef myResourceRef = ENSession.SharedSession.UploadNote(myResourceNote, null);
            }
            catch (Exception ex)
            {
                //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                //           Application.Current.MainWindow,
                //           (string)"upload file fail!",
                //          (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));

                return false;
            }

            return true;
        }

        public void ScanToEverNote(bool bReset)
        {
            try
            {
                connectSuccess = ENSession.SharedSession.AuthenticateToEvernote(bReset);

                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
            }

            catch (Exception ex)
            {
                connectSuccess = false;
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                Application.Current.MainWindow,
                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Connect_to_Evernote_server_fail"),
                                   //"Connect to EverNote server fail, please confirm you computer setting and your specify user name and password!",
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
            }

        }

        // Support routine for displaying a note thumbnail.
        static byte[] StreamFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file stream length
            byte[] ImageData = new byte[Convert.ToInt32(fs.Length - 1) + 1];

            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

            //Close the File Stream
            fs.Close();
            //return the byte data
            return ImageData;
        }
    }
}
