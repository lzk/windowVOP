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
            DialogResult = true;
            this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    
    }
}
