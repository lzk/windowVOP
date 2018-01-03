using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using EvernoteSDK;

namespace VOP
{
    /// <summary>
    /// Interaction logic for EvernoteView.xaml
    /// </summary>
    public partial class EverNoteViewer : Window
    {
        private List<string> notelist = new List<string>();
        private string title = "";
        private string content = "";
        public List<string> FileList { get; set; }

        public EverNoteViewer()
        {
            InitializeComponent();
        }

        public List<String> NoteList
        {
            get { return notelist; }

            set { notelist = value; }
        }

        public string NoteTitle
        {
            get { return title; }
        }

        public string NoteContent
        {
            get { return content; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            InitNoteList();
        }

        //marked by yunying shang 2017-11-30 for BMS 1621
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
        //    PasswordBox pb = sender as PasswordBox;
        //    string strText = e.Text;
        //    if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))
        //    {
        //        e.Handled = true;
        //    }
        }

        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void InitNoteList()
        {
            listNote.Items.Clear();

            foreach (string item in notelist)
            {
                listNote.Items.Add(item);
            }
        }

        private bool GetNoteList(List<string> NoteList)
        {
            string textToFind = "*";
            try
            {
                List<ENSessionFindNotesResult> myResultsList = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch(textToFind), null,
                ENSession.SearchScope.All, ENSession.SortOrder.RecentlyUpdated, 500);

               // List<string> notelist = new List<string>();

                if (myResultsList.Count > 0)
                {
                    NoteList.Clear();//add by yunying shang 2017-11-30 for BMS 1620
                    foreach (ENSessionFindNotesResult nb in myResultsList)
                    {
                        NoteList.Add(nb.Title);
                    }
                }
            }

            catch (Exception ex)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                Application.Current.MainWindow,
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Get_EverNote_List_error"),//"Get Ever Note List Error!",
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));

                return false;
            }

            return true;

        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (tbNoteTitle.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                Application.Current.MainWindow,
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Evernote_Tile_could_not_be_empty"),//"The Ever Note Title could not be empty!",
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                tbNoteTitle.Focus();
                return;
            }

            

            //add by yunying shang 2017-11-30 for BMS 1621
            int i = 0;
            for (i = 0; i < tbNoteTitle.Text.Length; i++)
            {
                if (tbNoteTitle.Text[i] != ' ')
                    break;
            }
            if (i >= tbNoteTitle.Text.Length)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                Application.Current.MainWindow,
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Evernote_Title_could_not_be_all_space"),//"The Ever Note Title characters could not be all space!",
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                tbNoteTitle.Text = "";
                tbNoteTitle.Focus();
                return;
            }//<<===================
            title = tbNoteTitle.Text;
            content = tbNoteContent.Text;

            try
            {
                ENNote myResourceNote = new ENNote();

                //modified by yunying shang 2017-12-07 for BMS 1623
                //add by yunying shang 2017-11-30 for BMS 1623

                if (FileList.Count > 60)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                   (string)(string)Application.Current.MainWindow.TryFindResource("ResStr_Evernote_attachment_files_could_not_more_than_60"),
                  (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    return;
                }
                else
                {
                    long totalsize = 0;
                    foreach (string filePath in FileList)
                    {
                        System.IO.FileInfo fileInfo = null;
                        fileInfo = new System.IO.FileInfo(filePath);
                        if (totalsize > 100 * 1024 * 1024)
                        {
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                           Application.Current.MainWindow,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Images_files_Size_too_large"),//"Total image files size are too large, could not upload to EverNote server!",
                          (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                            return;
                        }
                        totalsize += fileInfo.Length;
                    }
                    if (totalsize > 100 * 1024 * 1024)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                       Application.Current.MainWindow,
                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Images_files_Size_too_large"),//"Total image files size are too large, could not upload to EverNote server!",
                      (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                        return;
                    }
                }//<<=================1623
                foreach (string filePath in FileList)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    byte[] myFile = StreamFile(filePath);

                    UploadStaus.Text = "Uploading file " + fileName;

                    ENResource myResource = new ENResource(myFile, "image/jpg", fileName);//"application/pdf"                 
                    myResourceNote.Resources.Add(myResource);
                }
                myResourceNote.Title = title;//string.Format("Scan to EverNote: {0}", title);//, i); 
                content = string.Format("{0}.Attach the scaling Files.", content);
                myResourceNote.Content = ENNoteContent.NoteContentWithString(content);
                ENNoteRef myResourceRef = ENSession.SharedSession.UploadNote(myResourceNote, null);

                //modified by yunying shang 2017-12-13 for BMS 1722
                notelist.Add(title);
                listNote.Items.Add(title);

                //if(GetNoteList(notelist))
                //{ 
                //    InitNoteList();
                //}

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                Application.Current.MainWindow,
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Upload_success"),
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt"));
                tbNoteTitle.Text = "";
                tbNoteContent.Text = "";
                //<<===============1722
            }
            catch (Exception ex)
            {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
               Application.Current.MainWindow,
               (string)Application.Current.MainWindow.TryFindResource("ResStr_Upload_files_fail")//"Upload files fail, "
               + ex.Message,
              (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));

                tbNoteTitle.Focus();
            }
            
            //DialogResult = true;
            //this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

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
