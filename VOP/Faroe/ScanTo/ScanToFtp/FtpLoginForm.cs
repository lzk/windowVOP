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
