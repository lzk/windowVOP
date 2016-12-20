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
            tbServerName.Text = MainWindow_Rufous.g_settingData.m_serverAddress;
            tbUserName.Text = MainWindow_Rufous.g_settingData.m_userName;
            pbPWD.Password = MainWindow_Rufous.g_settingData.m_password;
            tbTargetPath.Text = MainWindow_Rufous.g_settingData.m_targetPath;
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
            MainWindow_Rufous.g_settingData.m_serverAddress = tbServerName.Text;
            MainWindow_Rufous.g_settingData.m_userName = tbUserName.Text;
            MainWindow_Rufous.g_settingData.m_password = pbPWD.Password;
            MainWindow_Rufous.g_settingData.m_targetPath = tbTargetPath.Text;
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
