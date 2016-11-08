using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using VOP.Controls;
using Microsoft.Win32;
using System.Collections.Generic;

namespace VOP
{
    public partial class ScanSelectionPage_Rufous : UserControl
    {
        private enum ScanSelectionState { Exit }

        public MainWindow_Rufous m_MainWin { get; set; }

        public ScanSelectionPage_Rufous()
        {
            InitializeComponent();
          
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetScreenText(1);
        }


        private void ScreenButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tb = ScreenBtn.Template.FindName("DetailText", ScreenBtn) as TextBlock;

            QuickScan qs = new QuickScan();
            switch (tb.Text)
            {
                case "Scan To Print":
                    qs.ScanToPrint();
                    break;
                case "Scan To File":
                    qs.ScanToFile();
                    break;
                case "Scan To Application":
                    qs.ScanToAP();
                    break;
                case "Scan To Email":
                    qs.ScanToEmail();
                    break;
                case "Scan To Ftp":
                    qs.ScanToFtp();
                    break;
                case "Scan To Cloud":
                    qs.ScanToCloud();
                    break;
                default:
                    break;
            }
        }


        private void QRCodeButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

            ScanTask task = new ScanTask();
            ScanParam param = new ScanParam(
                EnumScanResln._200x200,
                EnumPaperSizeScan._A4,
                EnumColorType.color_24bit,
                true,
                50,
                50);

            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", param);

            if (files == null)
                return;

            if (task.ScanResult == Scan_RET.RETSCAN_OK)
            {
                List<string> list = new List<string>();
                foreach (ScanFiles file in files)
                {
                    list.Add(file.m_pathOrig);
                }

                QRCodeWindow win = new QRCodeWindow(list);

                //OpenFileDialog open = null;
                //bool? result = null;
                //open = new OpenFileDialog();
                //open.Filter = "All Images|*.jpg;*.bmp;*.png;*.tif|JPEG|*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";
                //open.Multiselect = true;

                //result = open.ShowDialog();
                //if (result == true)
                {
                       // QRCodeWindow win = new QRCodeWindow(new List<string>(open.FileNames));

                        if (btn.Name == "ImageButton1")
                        {
                            win.IsQRCode = true;
                        }
                        else
                        {
                            win.IsQRCode = false;
                        }

                        win.Owner = m_MainWin;
                        win.ShowDialog();
        
                }
            
               
            }
           
        }

        private void ScanToButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

            ScanTask task = new ScanTask();
            ScanParam param = new ScanParam(
                EnumScanResln._300x300,
                EnumPaperSizeScan._A4,
                EnumColorType.color_24bit,
                true,
                50,
                50);

            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_commonScanSettings);

            if (files == null)
                return;

            if (task.ScanResult == Scan_RET.RETSCAN_OK)
            {
                //List<ScanFiles> files = new List<ScanFiles>();
                //files.Add(new ScanFiles(@"G:\work\Rufous\pic\0592995421_C200_A00.JPG"));
                //files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00_180.JPG"));
                //files.Add(new ScanFiles(@"G:\work\Rufous\pic\temp-000.JPG"));

                m_MainWin.GotoPage("ScanPage", files);
            }
           
        }

        private void DeviceButton_Click(object sender, RoutedEventArgs e)
        {
            m_MainWin.GotoPage("DevicePage", null);
        }

        private void SettingsButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_MainWin.GotoPage("SettingsPage", null);
        }

        private void SetScreenText(int number)
        {
            ScreenBtn.Content = number.ToString();
            TextBlock tb = ScreenBtn.Template.FindName("DetailText", ScreenBtn) as TextBlock;
            int index = MainWindow_Rufous.g_settingData.m_MatchList[number - 1].Value;
            tb.Text = ScanParameterView.ScanToItems[index];
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
           int result = 0;
           if(Int32.TryParse(ScreenBtn.Content.ToString(), out result))
           {
                if(result > 1)
                {
                    result -= 1;
                    SetScreenText(result);
                }
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            if (Int32.TryParse(ScreenBtn.Content.ToString(), out result))
            {
                if (result < SettingData.MaxShortCutNum)
                {
                    result += 1;
                    SetScreenText(result);
                }
            }
        }
    }
}
