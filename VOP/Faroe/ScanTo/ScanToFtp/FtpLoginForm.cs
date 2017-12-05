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
            string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");

            if (tbServerName.Text == "")
            {
                message = string.Format(message, "Server Address");
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  message,//"The Server Address cannot be empty",
                  (string)this.TryFindResource("ResStr_Error"));
                return;
            }
            else if (tbUserName.Text == "")
            {
                message = string.Format(message, "User Name");
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  message, //"The User Name cannot be empty",
                  (string)this.TryFindResource("ResStr_Error"));
                return;
            }
            else if (pbPWD.Password == "")
            {
                message = string.Format(message, "Password");
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  message,//"The Password cannot be empty",
                  (string)this.TryFindResource("ResStr_Error"));
                return;
            }
            else if (tbTargetPath.Text == "")
            {
                message = string.Format(message, "Target Path");
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  message,//"The Target Path cannot be empty",
                  (string)this.TryFindResource("ResStr_Error"));
                return;
            }

            message = (string)Application.Current.MainWindow.TryFindResource("ResStr_specify_incorrect");

            if (tbServerName.Text.Length < 7)
            {
                message = string.Format(message, "Server Address");

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 message, //"The Server Address format is incorrect, Please check your Server Address and enter again.",
                 (string)this.TryFindResource("ResStr_Error"));
                return;
            }

            string strServerName = tbServerName.Text.Substring(0, 6);
            string strTargetPath = tbTargetPath.Text.Substring(0, 1);

            if (strServerName.ToUpper() != "FTP://")
            {
                message = string.Format(message, "Server Address");

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 message,//"The Server Address format is incorrect, Please check your Server Address and enter again.",
                 (string)this.TryFindResource("ResStr_Error"));
                tbServerName.Focus();
                return;
            }
            if (strTargetPath != "/")
            {
                message = string.Format(message, "Target Path");

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 message,//"The Target Path format is incorrect, Please check your Target Path and enter again.",
                 (string)this.TryFindResource("ResStr_Error"));
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
                if (i >= strTargetPath.Length && strTargetPath.Length >= 2)
                {
                    message = string.Format(message, "Target Path");

                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     message, //"The Target Path format is incorrect, Please check your Target Path and enter again.",
                     (string)this.TryFindResource("ResStr_Error"));
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
