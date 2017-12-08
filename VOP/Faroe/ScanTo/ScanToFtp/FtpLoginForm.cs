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

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FtpLoginForm : Window
    {
        public string m_serverAddress = "";
        public string m_userName = "";
        public string m_password = "";
        public string m_targetPath = "";

        public FtpLoginForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            tbServerName.Text = m_serverAddress;
            tbUserName.Text = m_userName;
            pbPWD.Password = m_password;
            tbTargetPath.Text = m_targetPath;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            string str = (string)Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
            string content = "";
            string message = "";
            if (tbServerName.Text == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_server_addr1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Server Address cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
            else if (tbUserName.Text == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_username1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message, //"The User Name cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
            else if (pbPWD.Password == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_password1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Password cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
            else if (tbTargetPath.Text == "")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_targetPath1");
                message = string.Format(str, content);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                  message,//"The Target Path cannot be empty",
                  (string)this.TryFindResource("ResStr_Warning"));
                return;
            }

            str = (string)Application.Current.MainWindow.TryFindResource("ResStr_specify_incorrect");

            if (tbServerName.Text.Length < 7)
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_server_addr1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                  Application.Current.MainWindow,
                 message, //"The Server Address format is incorrect, Please check your Server Address and enter again.",
                 (string)this.TryFindResource("ResStr_Warning"));
                return;
            }

            string strServerName = tbServerName.Text.Substring(0, 6);
            string strTargetPath = tbTargetPath.Text.Substring(0, 1);

            if (strServerName.ToUpper() != "FTP://")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_server_addr1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                  Application.Current.MainWindow,
                 message,//"The Server Address format is incorrect, Please check your Server Address and enter again.",
                 (string)this.TryFindResource("ResStr_Warning"));
                tbServerName.Focus();
                return;
            }
            if (strTargetPath != "/")
            {
                content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_targetPath1");
                message = string.Format(str, content);

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                  Application.Current.MainWindow,
                 message,//"The Target Path format is incorrect, Please check your Target Path and enter again.",
                 (string)this.TryFindResource("ResStr_Warning"));
                tbTargetPath.Focus();
                return;
            }
            else
            {
                strTargetPath = tbTargetPath.Text;
                int i = 0;
                for (i = 0; i < strTargetPath.Length; i++)
                {
                    if (strTargetPath[i] != '/')
                        break;
                }
                if (i >= strTargetPath.Length && strTargetPath.Length >= 2
                    || strTargetPath.Contains('\\')
                    || strTargetPath.Contains('?')
                    || strTargetPath.Contains('*'))
                {
                    content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_targetPath1");
                    message = string.Format(str, content);

                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                      Application.Current.MainWindow,
                     message, //"The Target Path format is incorrect, Please check your Target Path and enter again.",
                     (string)this.TryFindResource("ResStr_Warning"));
                    return;
                }
            }
            m_serverAddress = tbServerName.Text;
            m_userName = tbUserName.Text;
            m_password = pbPWD.Password;
            m_targetPath = tbTargetPath.Text;
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
