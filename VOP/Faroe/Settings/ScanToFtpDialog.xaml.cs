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

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanToFtpDialog.xaml
    /// </summary>
    public partial class ScanToFtpDialog : Window
    {
        public ScanToFTPParam m_scanToFTPParams = new ScanToFTPParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToFtpDialog()
        {
            InitializeComponent();
        }
        private void ScanToFtpDialog_Loaded(object sender, RoutedEventArgs e)
        {
            tbServerName.Text = m_scanToFTPParams.ServerAddress;
            tbUserName.Text = m_scanToFTPParams.UserName;
            pbPWD.Password = m_scanToFTPParams.Password;
            tbTargetPath.Text = m_scanToFTPParams.TargetPath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = m_scanParams;

            if (settingWin.ShowDialog() == true)
            {
                m_scanParams = (ScanParam)settingWin.m_scanParams.Clone();
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbServerName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Server Address cannot be empty",
                  "Error");
                return;
            }
            else if (tbUserName.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The User Name cannot be empty",
                  "Error");
                return;
            }
            else if (pbPWD.Password == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Password cannot be empty",
                  "Error");
                return;
            }
            else if (tbTargetPath.Text == "")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Target Path cannot be empty",
                  "Error");
                return;
            }
            if (tbServerName.Text.Length < 7)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Server Address format is incorrect, Please check you server name and enter again.",
                 "Error");
                return;
            }
            string strServerName = tbServerName.Text.Substring(0, 6);
            string strTargetPath = tbTargetPath.Text.Substring(0, 1);
            if (strServerName.ToUpper() != "FTP://")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Server Address format is incorrect, Please check you server name and enter again.",
                 "Error");
                return;
            }
            if (strTargetPath != "/")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Target Path format is incorrect, Please check you target path and enter again.",
                 "Error");
                return;
            }
            m_scanToFTPParams.ServerAddress = tbServerName.Text;
            m_scanToFTPParams.UserName = tbUserName.Text;
            m_scanToFTPParams.Password = pbPWD.Password;
            m_scanToFTPParams.TargetPath = tbTargetPath.Text;
            this.DialogResult = true;
            this.Close();
        }
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            string strText = e.Text;
            //marked by yunying shang 2017-10-24 for BMS 1170
            //if (strText.Length > 0 && !Char.IsLetterOrDigit(strText, 0))

            //{
            //e.Handled = true;
            //}

        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // if (e.Key == Key.Space)
            //    e.Handled = true;
        }
        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }
    }
}
