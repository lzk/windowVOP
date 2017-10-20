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

        public int ScreenTextNumber = 1;

        public ScanSelectionPage_Rufous()
        {
            InitializeComponent();          
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetScreenText(ScreenTextNumber);
            MainWindow_Rufous.g_settingData.CutNum = ScreenTextNumber-1;
        }

        private void ScreenButton_Click(object sender, RoutedEventArgs e)
        {

            TextBlock tb = ScreenBtn.Template.FindName("DetailText", ScreenBtn) as TextBlock;

            QuickScan qs = new QuickScan();            
            switch (MainWindow_Rufous.g_settingData.m_MatchList[ScreenTextNumber - 1].Value)
            {
                case 0:
                    qs.ScanToPrint();
                    break;
                case 1:
                    qs.ScanToFile();
                    break;
                case 2:
                    qs.ScanToAP();
                    break;
                case 3:
                    qs.ScanToEmail();
                    break;
                case 4:
                    qs.ScanToFtp();
                    break;
                case 5:
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
                EnumScanResln._300x300,
                EnumScanMediaType._Normal,
                EnumPaperSizeScan._A4,
                EnumColorType.color_24bit,
                false,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.MultiFeed,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.AutoCrop,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.Brightness,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.Contrast,
                false);


#if (DEBUG)
            OpenFileDialog open = null;
            bool? result = null;
            open = new OpenFileDialog();
            open.Filter = "All Images|*.jpeg;*.jpg;*.bmp;*.png;*.tif|JPEG|*.jpeg;*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";
            open.Multiselect = true;

            result = open.ShowDialog();
            if (result == true)
            {
                QRCodeWindow win = new QRCodeWindow(new List<string>(open.FileNames));

                if (btn.Name == "ImageButton1")
                {
                    ImageCropper.designerItemWHRatio = 1.0;
                    win.IsQRCode = true;
                }
                else
                {
                    ImageCropper.designerItemWHRatio = 2.0;
                    win.IsQRCode = false;
                }

                win.Owner = m_MainWin;
                win.ShowDialog();

            }

#elif (!DEBUG)

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
#endif

        }

        private void ScanToButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ImageButton btn = sender as ImageButton;

            ScanTask task = new ScanTask();

            List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", MainWindow_Rufous.g_settingData.m_commonScanSettings);
            //List<ScanFiles> files = new List<ScanFiles>();
            if (files == null)
                return;

            if (task.ScanResult == Scan_RET.RETSCAN_OK)
            {
                //                List<ScanFiles> files = new List<ScanFiles>();
                //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\1 error.JPG"));
                //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\1.JPG"));
                //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\qrcode fail.JPG"));
                //files.Add(new ScanFiles(@"I:\work\CODE\Faroe VOP\Install\Faroe_WinVOP_v1007_170905\Faroe_WinVOP_v1007_170905\1.JPG"));
                //files.Add(new ScanFiles(@"I:\work\CODE\Faroe VOP\Install\Faroe_WinVOP_v1007_170905\Faroe_WinVOP_v1007_170905\2.JPG"));
                ////files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00_180.JPG"));
                ////files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00.JPG"));
                ////files.Add(new ScanFiles(@"G:\work\Rufous\pic\0592995421_C200_A00.JPG"));
                ////files.Add(new ScanFiles(@"G:\work\Rufous\pic\0592601031_C300_A00.JPG"));
                ////files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00_180.JPG"));
                ////files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00.JPG"));

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
            tb.Text = MainWindow_Rufous.g_settingData.m_MatchList[number - 1].ItemName;
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
                    ScreenTextNumber = result;
                    MainWindow_Rufous.g_settingData.CutNum = result-1;
                    if (result == 1)
                    {
                        LeftBtn.IsEnabled = false;
                        RightBtn.IsEnabled = true;
                    }                       
                    else
                        LeftBtn.IsEnabled = true;
                    RightBtn.IsEnabled = true;
                }
            }
        }
        
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;       
            if (Int32.TryParse(ScreenBtn.Content.ToString(), out result))
            {
                if (result < MainWindow_Rufous.g_settingData.m_MatchList.Count)
                {
                    result += 1;
                    SetScreenText(result);
                    ScreenTextNumber = result;
                    MainWindow_Rufous.g_settingData.CutNum = result-1;
                    if (result == MainWindow_Rufous.g_settingData.m_MatchList.Count)
                    {
                        RightBtn.IsEnabled = false;
                        LeftBtn.IsEnabled = true;
                    }
                    else
                        RightBtn.IsEnabled = true;
                    LeftBtn.IsEnabled = true;
                }
            }
        }
    }
}
