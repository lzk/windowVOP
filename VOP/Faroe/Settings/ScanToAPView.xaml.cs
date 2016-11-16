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
    public partial class ScanToAPView : UserControl
    {
        string programType = "";

        public ScanToAPView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            programType = MainWindow_Rufous.g_settingData.m_programType;

            if (MainWindow_Rufous.g_settingData.m_programType == "Paint")
            {
                cbProgramType.SelectedIndex = 0;
            }
            else
            {
                cbProgramType.SelectedIndex = 1;
            }

        }

        private void cbProgramType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbProgramType.SelectedIndex == 0)
            {
                programType = "Paint";
            }
            else if(cbProgramType.SelectedIndex == 1)
            {
                programType = "Photo Viewer";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanResln = MainWindow_Rufous.g_settingData.m_apScanSettings.ScanResolution;
            settingWin.m_paperSize = MainWindow_Rufous.g_settingData.m_apScanSettings.PaperSize;
            settingWin.m_color = MainWindow_Rufous.g_settingData.m_apScanSettings.ColorType;
            settingWin.m_brightness = MainWindow_Rufous.g_settingData.m_apScanSettings.Brightness;
            settingWin.m_contrast = MainWindow_Rufous.g_settingData.m_apScanSettings.Contrast;

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_apScanSettings.ScanResolution = settingWin.m_scanResln;
                MainWindow_Rufous.g_settingData.m_apScanSettings.PaperSize = settingWin.m_paperSize;
                MainWindow_Rufous.g_settingData.m_apScanSettings.ColorType = settingWin.m_color;
                MainWindow_Rufous.g_settingData.m_apScanSettings.Brightness = settingWin.m_brightness;
                MainWindow_Rufous.g_settingData.m_apScanSettings.Contrast = settingWin.m_contrast;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            MainWindow_Rufous.g_settingData.m_programType = programType;
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
