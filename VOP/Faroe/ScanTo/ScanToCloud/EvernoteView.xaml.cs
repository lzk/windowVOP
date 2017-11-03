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

        //public List<String> NoteList
        //{
        //    get { return notelist; }

        //    set { notelist = value; }
        //}

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


        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            string strText = e.Text;
            if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))
            {
                e.Handled = true;
            }

        }
        private void InitNoteList()
        {
            string textToFind = "*";

            List<ENSessionFindNotesResult> myResultsList = ENSession.SharedSession.FindNotes(ENNoteSearch.NoteSearch(textToFind), null,
                ENSession.SearchScope.All, ENSession.SortOrder.RecentlyUpdated, 500);

            List<string> notelist = new List<string>();

            if (myResultsList.Count > 0)
            {

                foreach (ENSessionFindNotesResult nb in myResultsList)
                {
                    notelist.Add(nb.Title);
                }
            }

            listNote.Items.Clear();

             foreach (string item in notelist)
             {
                 listNote.Items.Add(item);
             }

        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (tbNoteTitle.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                Application.Current.MainWindow,
                "The Ever Note Title could not be empty!",
                "Error");
                tbNoteTitle.Focus();
                return;
            }

            title = tbNoteTitle.Text;
            content = tbNoteContent.Text;

            ENNote myResourceNote = new ENNote();          

            foreach (string filePath in FileList)
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                byte[] myFile = StreamFile(filePath);
                ENResource myResource = new ENResource(myFile, "image/jpg", fileName);//"application/pdf"                 
                myResourceNote.Resources.Add(myResource);
            }
            myResourceNote.Title = title;//string.Format("Scan to EverNote: {0}", title);//, i); 
            content = string.Format("{0}.Attach the scaling Files.", content);
            myResourceNote.Content = ENNoteContent.NoteContentWithString(content);
            ENNoteRef myResourceRef = ENSession.SharedSession.UploadNote(myResourceNote, null);

            InitNoteList();
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
