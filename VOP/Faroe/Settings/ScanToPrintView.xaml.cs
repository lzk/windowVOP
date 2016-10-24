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
    public partial class ScanToPrintView : UserControl
    {

        public ScanToPrintView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            int index = 0;
            for (int i = 0; i < MainWindow_Rufous.g_printerList.Count; i++)
            {
                if(MainWindow_Rufous.g_printerList[i] == MainWindow_Rufous.g_settingData.m_printerName)
                {
                    index = i;
                }

                cboPrinters.Items.Add(MainWindow_Rufous.g_printerList[i]);
            }

            cboPrinters.SelectedIndex = index;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingDialog settingWin = new ScanSettingDialog();
            settingWin.Owner = m_MainWin;

            settingWin.m_scanResln = MainWindow_Rufous.g_settingData.m_printScanSettings.ScanResolution;
            settingWin.m_paperSize = MainWindow_Rufous.g_settingData.m_printScanSettings.PaperSize;
            settingWin.m_color = MainWindow_Rufous.g_settingData.m_printScanSettings.ColorType;
            settingWin.m_brightness = MainWindow_Rufous.g_settingData.m_printScanSettings.Brightness;
            settingWin.m_contrast = MainWindow_Rufous.g_settingData.m_printScanSettings.Contrast;

            if (settingWin.ShowDialog() == true)
            {
                MainWindow_Rufous.g_settingData.m_printScanSettings.ScanResolution = settingWin.m_scanResln;
                MainWindow_Rufous.g_settingData.m_printScanSettings.PaperSize = settingWin.m_paperSize;
                MainWindow_Rufous.g_settingData.m_printScanSettings.ColorType = settingWin.m_color;
                MainWindow_Rufous.g_settingData.m_printScanSettings.Brightness = settingWin.m_brightness;
                MainWindow_Rufous.g_settingData.m_printScanSettings.Contrast = settingWin.m_contrast;
            }
        }

        private void cboPrinters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != cboPrinters.SelectedItem)
            {
                MainWindow_Rufous.g_settingData.m_printerName = this.cboPrinters.SelectedItem.ToString();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
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
