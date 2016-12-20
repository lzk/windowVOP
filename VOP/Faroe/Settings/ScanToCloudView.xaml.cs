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
    public partial class ScanToCloudView : UserControl
    {

        public ScanToCloudView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SavePathTbx.Text = MainWindow_Rufous.g_settingData.m_dropBoxDefaultPath;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            DropBoxFlow flow = new DropBoxFlow();
            flow.ParentWin = m_MainWin;
            DropBoxFlow.FlowType = CloudFlowType.SimpleView;
            flow.Run();
            SavePathTbx.Text = DropBoxFlow.SavePath;
            MainWindow_Rufous.g_settingData.m_dropBoxDefaultPath = DropBoxFlow.SavePath;

            //reset
            DropBoxFlow.FlowType = CloudFlowType.View;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanParams = (ScanParam)MainWindow_Rufous.g_settingData.m_cloudScanSettings.Clone();

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_cloudScanSettings = (ScanParam)settingWin.m_scanParams;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            MainWindow_Rufous.g_settingData.m_dropBoxDefaultPath = SavePathTbx.Text = "";

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
