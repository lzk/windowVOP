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

        private void OkClick(object sender, RoutedEventArgs e)
        {

            if (tbServerName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Server Address cannot be empty",
                  "Error");
                tbServerName.Focus();
                return;
            }
            else if (tbUserName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The User Name cannot be empty",
                  "Error");
                tbUserName.Focus();
                return;
            }
            else if (pbPWD.Password == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Password cannot be empty",
                  "Error");
                pbPWD.Focus();
                return;
            }
            else if (tbTargetPath.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Target Path cannot be empty",
                  "Error");
                tbTargetPath.Focus();
                return;
            }
            if (tbServerName.Text.Length < 7)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Server Address format is incorrect, Please check you server address and enter again.",
                 "Error");
                tbServerName.Focus();
                return;
            }

            string strServerName = tbServerName.Text.Substring(0, 6);
            string strTargetPath = tbTargetPath.Text.Substring(0, 1);

            if (strServerName.ToUpper() != "FTP://")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Server Address format is incorrect, Please check you server address and enter again.",
                 "Error");
                tbServerName.Focus();
                return;
            }
            if (strTargetPath != "/")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Target Path format is incorrect, Please check you target path and enter again.",
                 "Error");
                tbTargetPath.Focus();
                return;
            }
            m_serverAddress = tbServerName.Text;
            m_userName = tbUserName.Text;
            m_password = pbPWD.Password;
            m_targetPath = tbTargetPath.Text;
            DialogResult = true;
            this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
