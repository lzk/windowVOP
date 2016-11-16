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
    public partial class ScanToEmailView : UserControl
    {
        string attachmentType = "";
        public ScanToEmailView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            attachmentType = MainWindow_Rufous.g_settingData.m_attachmentType;

            if (MainWindow_Rufous.g_settingData.m_attachmentType == "PDF")
            {
                cbAttachType.SelectedIndex = 0;
            }
            else
            {
                cbAttachType.SelectedIndex = 1;
            }

            tbRecipient.Text = MainWindow_Rufous.g_settingData.m_recipient;
            tbSubject.Text = MainWindow_Rufous.g_settingData.m_subject;
        }

        private void cbAttachType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbAttachType.SelectedIndex == 0)
            {
                attachmentType = "PDF";
            }
            else if(cbAttachType.SelectedIndex == 1)
            {
                attachmentType = "JPG";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanResln = MainWindow_Rufous.g_settingData.m_emailScanSettings.ScanResolution;
            settingWin.m_paperSize = MainWindow_Rufous.g_settingData.m_emailScanSettings.PaperSize;
            settingWin.m_color = MainWindow_Rufous.g_settingData.m_emailScanSettings.ColorType;
            settingWin.m_brightness = MainWindow_Rufous.g_settingData.m_emailScanSettings.Brightness;
            settingWin.m_contrast = MainWindow_Rufous.g_settingData.m_emailScanSettings.Contrast;

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_emailScanSettings.ScanResolution = settingWin.m_scanResln;
                MainWindow_Rufous.g_settingData.m_emailScanSettings.PaperSize = settingWin.m_paperSize;
                MainWindow_Rufous.g_settingData.m_emailScanSettings.ColorType = settingWin.m_color;
                MainWindow_Rufous.g_settingData.m_emailScanSettings.Brightness = settingWin.m_brightness;
                MainWindow_Rufous.g_settingData.m_emailScanSettings.Contrast = settingWin.m_contrast;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            MainWindow_Rufous.g_settingData.m_recipient = tbRecipient.Text;
            MainWindow_Rufous.g_settingData.m_subject = tbSubject.Text;
            MainWindow_Rufous.g_settingData.m_attachmentType = attachmentType;
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
