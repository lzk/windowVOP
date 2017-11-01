using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VOP.Controls;

namespace VOP
{
    public partial class ScanToFtpView : UserControl
    {

        public ScanToFtpView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbServerName.Text = MainWindow_Rufous.g_settingData.m_quickScanServerAddress;
            tbUserName.Text = MainWindow_Rufous.g_settingData.m_quickScanUserName;
            pbPWD.Password = MainWindow_Rufous.g_settingData.m_quickScanPassword;
            tbTargetPath.Text = MainWindow_Rufous.g_settingData.m_quickScanTargetPath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_ftpScanSettings.Clone();

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_ftpScanSettings = (ScanParam)settingWin.m_scanParams;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (tbServerName.Text == "" )
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The Server Address cannot be empty",
                  "Error");
                return;
            }
            else if (tbUserName.Text == "" )
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                   Application.Current.MainWindow,
                  "The User Name cannot be empty",
                  "Error");
                return;
            }
            else if (pbPWD.Password == "" )
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
            if(tbServerName.Text.Length < 7)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Server Address format is incorrect, Please check you server address and enter again.",
                 "Error");
                return;
            }            
            string strServerName = tbServerName.Text.Substring(0, 6);               
            string strTargetPath = tbTargetPath.Text.Substring(0, 1);
            if (strServerName.ToUpper() != "FTP://" )
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Server Address format is incorrect, Please check you server address and enter again.",
                 "Error");
                return;
            }
            if (strTargetPath != "/")
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                  Application.Current.MainWindow,
                 "The Tartget Path format is incorrect, Please check you target path and enter again.",
                 "Error");
                return;
            }
            MainWindow_Rufous.g_settingData.m_quickScanServerAddress = tbServerName.Text;
            MainWindow_Rufous.g_settingData.m_quickScanUserName = tbUserName.Text;
            MainWindow_Rufous.g_settingData.m_quickScanPassword = pbPWD.Password;
            MainWindow_Rufous.g_settingData.m_quickScanTargetPath = tbTargetPath.Text;
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

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
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
