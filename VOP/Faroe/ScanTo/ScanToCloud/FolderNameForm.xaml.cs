using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FolderNameForm : Window
    {
        public string m_folderName = "";

        public FolderNameForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            this.tbFolderName.Focus();          
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            string text = tbFolderName.Text;
            if (tbFolderName.Text == "")
            {
                string str = (string)Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
                string content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder");
                string message = string.Format(str, (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name"));

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The folder name cannot be empty",
                  (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return;
            }
            else
            {
                //modified by yunying shang 2018-02-02 for BMS 2195 and 2197
                Win32.OutputDebugString("The input folder name is " + text);
                if (text.Trim() != "")
                {
                    Regex regex = new Regex(@"^[^\/\:\*\?\""\<\>\|\,]+$");
                    Match m = regex.Match(text);
                    if (!m.Success)
                    {
                        string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Invalid_xxx");
                        message = string.Format(message, (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name"));
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                            message,//"Invalid folder name.", 
                            (string)this.TryFindResource("ResStr_Warning"));
                        this.tbFolderName.Focus();
                        return;
                    }
                }
                else
                {
                    string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Invalid_xxx");
                    message = string.Format(message, (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name"));
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        Application.Current.MainWindow,
                        message,//"Invalid folder name.", 
                        (string)this.TryFindResource("ResStr_Warning"));
                    this.tbFolderName.Text = "";
                    this.tbFolderName.Focus();
                    return;
                }//<<================2195 and 2197
            }

            m_folderName = text;//tbFolderName.Text;
            DialogResult = true;
            this.Close();
        }
       
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
