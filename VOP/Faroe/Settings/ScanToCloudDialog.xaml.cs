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
    /// Interaction logic for ScanToCloudDialog.xaml
    /// </summary>
    public partial class ScanToCloudDialog : Window
    {
        public ScanToCloudParam m_scanToCloudParams = new ScanToCloudParam();
        public ScanParam m_scanParams = new ScanParam();
        public ScanToCloudDialog()
        {
            InitializeComponent();
        }
        private void ScanToCloudDialog_Loaded(object sender, RoutedEventArgs e)
        {
            SavePathTbx.Text = m_scanToCloudParams.DefaultPath;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            DropBoxFlow flow = new DropBoxFlow();
            flow.ParentWin = m_MainWin;
            DropBoxFlow.FlowType = CloudFlowType.SimpleView;
            flow.Run();
            SavePathTbx.Text = DropBoxFlow.SavePath;
            m_scanToCloudParams.DefaultPath = DropBoxFlow.SavePath;

            //reset
            DropBoxFlow.FlowType = CloudFlowType.View;
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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            m_scanToCloudParams.DefaultPath = SavePathTbx.Text = "";
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
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
