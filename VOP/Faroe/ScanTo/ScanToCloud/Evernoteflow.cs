using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

using EvernoteSDK;


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

        public bool isCancel = false;

        public bool Run()
        {
            isCancel = false;

            if (FileList == null || FileList.Count == 0)
            {
                if (FlowType != CloudFlowType.SimpleView)
                    return false;
            }

            try
            {
                // Be sure to put your own consumer key and consumer secret here.
                ENSession.SetSharedSessionConsumerKey(ApiKey, ApiSecret);

                if (ENSession.SharedSession.IsAuthenticated == false)
                {
                    ENSession.SharedSession.AuthenticateToEvernote();
                }

                // Get a list of all notebooks in the user's account.
                if (FlowType == CloudFlowType.View)
                {
                    //string textToFind = "*";

                    //List<ENSessionFindNotesResult> myResultsList = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch(textToFind), null,
                    //    ENSession.SearchScope.All, ENSession.SortOrder.RecentlyUpdated, 500);

                    //List<string> notelist = new List<string>();

                    //if (myResultsList.Count > 0)
                    //{ 
                        
                    //    foreach (ENSessionFindNotesResult nb in myResultsList)
                    //    {
                    //        notelist.Add(nb.Title);
                    //    }
                    //}
                    bool? result = null;
                    EverNoteViewer viewer = new EverNoteViewer();
                    //viewer.NoteList = notelist;
                    viewer.FileList = FileList;
                    viewer.Owner = Application.Current.MainWindow;
                    result = viewer.ShowDialog();

                    if (result == true)
                    {

                        //string title = viewer.NoteTitle;
                        //string content = viewer.NoteContent;
                        //ENNote myResourceNote = new ENNote();
                        //foreach (string filePath in FileList)
                        //{
                        //    string fileName = System.IO.Path.GetFileName(filePath);
                        //    byte[] myFile = StreamFile(filePath);
                        //    ENResource myResource = new ENResource(myFile, "image/jpg", fileName);//"application/pdf"                 
                        //    myResourceNote.Resources.Add(myResource);
                        //}
                        //myResourceNote.Title = string.Format("Scan to EverNote: {0}", title);//, i);
                        //content = string.Format("{0}.Attach the scaling Files.", content);
                        //myResourceNote.Content = ENNoteContent.NoteContentWithString(content);
                        //ENNoteRef myResourceRef = ENSession.SharedSession.UploadNote(myResourceNote, null);
                    }
                }
                else
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
            }

            catch (Exception ex)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                           Application.Current.MainWindow,
                           (string)"Scan to cloud error, " + ex.Message,
                          (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
            }
            return true;
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
