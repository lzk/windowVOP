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

            LeftBtn.IsEnabled = false;
            RightBtn.IsEnabled = true;        
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int result = 0;
            if (Int32.TryParse(ScreenBtn.Content.ToString(), out result))
            {
                if (result > MainWindow_Rufous.g_settingData.m_MatchList.Count)
                    ScreenTextNumber =  MainWindow_Rufous.g_settingData.m_MatchList.Count;
            }
            SetScreenText(ScreenTextNumber);

            MainWindow_Rufous.g_settingData.CutNum = ScreenTextNumber-1;

            //add by yunying shang 2017-11-01 for BMS 1204
            if(result > 0)
            { 
                if (result <= MainWindow_Rufous.g_settingData.m_MatchList.Count 
                    && result >=1)
                {
                    if (result == MainWindow_Rufous.g_settingData.m_MatchList.Count)
                    {
                        RightBtn.IsEnabled = false;
                    }
                    else
                    {
                        if (result == 1)
                        {
                            LeftBtn.IsEnabled = false;
                        }
                        else
                            LeftBtn.IsEnabled = true;

                        RightBtn.IsEnabled = true;
                    }
                }
            }//<<=======================

            //m_MainWin.CheckDeviceStatus();
        }

        private void ScreenButton_Click(object sender, RoutedEventArgs e)
        {
            m_MainWin._bScanning = true;
            TextBlock tb = ScreenBtn.Template.FindName("DetailText", ScreenBtn) as TextBlock;

            QuickScan qs = new QuickScan();
            Scan_RET bRtn = Scan_RET.RETSCAN_OK;
            int iRtn = 0;

            switch (MainWindow_Rufous.g_settingData.m_MatchList[ScreenTextNumber - 1].Value)
            {
                case 0:
                    bRtn = qs.ScanToPrint();
                    break;
                case 1:
                    bRtn = qs.ScanToFile();
                    break;
                case 2:
                    bRtn = qs.ScanToAP();
                    break;
                case 3:
                    bRtn = qs.ScanToEmail();
                    break;

                case 4:
                   iRtn = m_MainWin.CheckDeviceStatus();
                    if ( iRtn <= 0)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_conn_fail"),
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                            );

                        return;
                    }
                    else if(iRtn == 1 && !m_MainWin.scanDevicePage.IsOnLine())//modified by yunying shang 2017-12-13 for BMS 1775
                    {

                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Network_fail"),
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                            );

                        return;
                    }
                    bRtn = qs.ScanToFtp();
                    break;

                case 5:
                    iRtn = m_MainWin.CheckDeviceStatus();
                    if (iRtn <= 0)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_scan_conn_fail"),
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                            );

                        return;
                    }
                    else if (iRtn == 1 && !m_MainWin.scanDevicePage.IsOnLine())//modified by yunying shang 2017-12-13 for BMS 1775
                    {

                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Network_fail"),
                           (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                            );
                        return;
                    }
                    bRtn = qs.ScanToCloud();
                    break;

                default:
                    break;
            }

            if (bRtn == Scan_RET.RETSCAN_CANCEL)
            {
                if (DeviceButton.Connected == false)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Device_Disconnected"),
                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                    );
                }
                //add by yunying shang 2017-12-11 for BMS 1744
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        Application.Current.MainWindow,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Scanning_is_canceled_on_machine"),//"The scanning is canceled on the machine!",
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                        );
                }//<<================1744
            }
            m_MainWin._bScanning = false;
        }

        private void QRCodeButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton2 btn = sender as ImageButton2;

//#if DEBUG
//            if (true)
//            {
//                OpenFileDialog open1 = null;
//                bool? result1 = null;
//                open1 = new OpenFileDialog();
//                open1.Filter = "All Images|*.jpeg;*.jpg;*.bmp;*.png;*.tif|JPEG|*.jpeg;*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";
//                open1.Multiselect = true;

//                result1 = open1.ShowDialog();
//                if (result1 == true)
//                {
//                    QRCodeDetection qrcodeDetection = new QRCodeDetection(new List<string>(open1.FileNames));
//                    if (btn.Name == "ImageButton1")
//                    {
//                        qrcodeDetection.ExcuteDecode(m_MainWin);
//                    }
//                    else
//                    {
//                        qrcodeDetection.ExcuteSeparation(m_MainWin);
//                    }

//                }
//                return;
//            }
//#endif
            m_MainWin._bScanning = true;
            ScanTask task = new ScanTask();
            ScanParam param = new ScanParam(
                EnumScanResln._300x300,
                EnumScanMediaType._Normal,
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.PaperSize,//EnumPaperSizeScan._A4,//Devid for bms#0001761
                EnumColorType.color_24bit,
                false,
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.MultiFeed,
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.AutoCrop,
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.Brightness,
                MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings.Contrast,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.AutoColorDetect,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.SkipBlankPage,
                MainWindow_Rufous.g_settingData.m_commonScanSettings.Gamma,
                false);


//#if (DEBUG)
//            OpenFileDialog open = null;
//            bool? result = null;
//            open = new OpenFileDialog();
//            open.Filter = "All Images|*.jpeg;*.jpg;*.bmp;*.png;*.tif|JPEG|*.jpeg;*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";
//            open.Multiselect = true;

//            result = open.ShowDialog();
//            if (result == true)
//            {
//                QRCodeWindow win = new QRCodeWindow(new List<string>(open.FileNames));

//                if (btn.Name == "ImageButton1")
//                {
//                    ImageCropper.designerItemWHRatio = 1.0;
//                    win.IsQRCode = true;
//                }
//                else
//                {
//                    ImageCropper.designerItemWHRatio = 2.0;
//                    win.IsQRCode = false;
//                }

//                win.Owner = m_MainWin;
//                win.ShowDialog();

//            }

//# elif (!DEBUG)

            if (btn.Name == "ImageButton2")
                param = MainWindow_Rufous.g_settingData.m_qrcodebarcodeScanSettings;

            string oldPictureFolder = App.PictureFolder;
            App.PictureFolder = App.cacheFolder;

            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, param);

            m_MainWin._bScanning = false;
            App.PictureFolder = oldPictureFolder;

            if (files == null)
            {
                //add by yunying shang 2017-11-29 for BMS 1601
                if (task.ScanResult == Scan_RET.RETSCAN_CANCEL)
                {
                    if (DeviceButton.Connected == false)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Device_Disconnected"),
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                            );
                    }
                    else
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Scanning_is_canceled_on_machine"), //"The scanning is canceled on the machine!",
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                            );
                    }
                }//<<================1601
                return;
            }

            if (task.ScanResult == Scan_RET.RETSCAN_OK)
            {
                List<string> list = new List<string>();
                foreach (ScanFiles file in files)
                {
                    list.Add(file.m_pathOrig);
                }

                QRCodeDetection qrcodeDetection = new QRCodeDetection(list);
                if (btn.Name == "ImageButton1")
                {
                    qrcodeDetection.ExcuteDecode(m_MainWin);
                }
                else
                {
                    qrcodeDetection.ExcuteSeparation(m_MainWin);
                }

            }
            
//#endif

        }
        public void PushScan()
        {
            MainWindow_Rufous.g_settingData.m_isUsbConnect = true;

            ScanParam paramBak = new ScanParam();
            paramBak = (ScanParam)MainWindow_Rufous.g_settingData.m_commonScanSettings.Clone();
            MainWindow_Rufous.g_settingData.m_commonScanSettings = (ScanParam)MainWindow_Rufous.g_settingData.m_pushScanSettingsofPC.Clone();

            ScanToButtonClick(null, null);
            App.gPushScan = false;
            MainWindow_Rufous.g_settingData.m_commonScanSettings = (ScanParam)paramBak.Clone();
        }

        public void ScanToButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_MainWin._bScanning = true;
           // ImageButton2 btn = sender as ImageButton2;

            ScanTask task = new ScanTask();
            List<ScanFiles> files = task.DoScan(MainWindow_Rufous.g_settingData.m_DeviceName, MainWindow_Rufous.g_settingData.m_commonScanSettings);
           // List<ScanFiles> files = new List<ScanFiles>();
            m_MainWin._bScanning = false;
            if (files != null)
            //   return;
            {
                if (task.ScanResult == Scan_RET.RETSCAN_OK)
                {
                    //List<ScanFiles> files = new List<ScanFiles>();
                    //for (int i = 0; i < 60; i++)
                    //{
                    //    string path = string.Format(@"C:\Users\Administrator\Desktop\111\{0}.jpg", i+1);
                    //    ScanFiles file = new ScanFiles(path);
                    //    files.Add(file);
                    //}
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\1.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\2.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\3.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\4.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\5.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\6.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\7.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\8.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\9.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\10.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171109111223000A.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171109111223000B.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171122163848000A.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171122163848000B.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171122163848001A.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171122163848001B.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\a6_1.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\a6_2.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\a5_1.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\a5_2.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171109111223000C.jpg"));
                    //files.Add(new ScanFiles(@"C:\Users\Administrator\Desktop\111\img20171109111223000D.jpg"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\1 error.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\1.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\qrcode fail.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\1.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\debug\qrcode fail.JPG"));
                    //files.Add(new ScanFiles(@"I:\work\CODE\Faroe VOP\Install\Faroe_WinVOP_v1007_170905\Faroe_WinVOP_v1007_170905\1.JPG"));
                    //files.Add(new ScanFiles(@"I:\work\CODE\Faroe VOP\Install\Faroe_WinVOP_v1007_170905\Faroe_WinVOP_v1007_170905\2.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\0592995421_C200_A00.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\0592601031_C300_A00.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00_180.JPG"));
                    //files.Add(new ScanFiles(@"G:\work\Rufous\pic\0529016859_C300_A00.JPG"));

                    m_MainWin.GotoPage("ScanPage", files);
                    ImageButton3.IsEnabled = false;//move by yunying shang 2017-12-16 for BMS 1812
                }                
            }
            else if (task.ScanResult == Scan_RET.RETSCAN_CANCEL)
            {
                if (DeviceButton.Connected == false)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        Application.Current.MainWindow,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Device_Disconnected"),
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                        );
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        Application.Current.MainWindow,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Scanning_is_canceled_on_machine"),//"The scanning is canceled on the machine!",
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                        );
                }
            }            
        }

        private void DeviceButton_Click(object sender, RoutedEventArgs e)
        {
            m_MainWin.GotoPage("DevicePage", null);
        }

        private void SettingsButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {                  
            m_MainWin.GotoPage("SettingsPage", null);
            ImageButton4.IsEnabled = false;
        }

        private void SetScreenText(int number)
        {
            string[] imgPath = {
                "../Images/main_img_print.png",
                "../Images/main_img_file.png",
                "../Images/main_img_apps.png",
                "../Images/main_img_email.png",
                "../Images/main_img_ftp.png",
                "../Images/main_img_cloud.png"
            };
            string[] adfmode = { "One Side", "Two Side"};
            ScreenBtn.Content = number.ToString();

            int index = MainWindow_Rufous.g_settingData.m_MatchList[number - 1].Value;

            Image img = ScreenBtn.Template.FindName("screenImage", ScreenBtn) as Image;
            img.Source = new BitmapImage(new Uri(imgPath[index], UriKind.RelativeOrAbsolute));

            TextBlock tb = ScreenBtn.Template.FindName("DetailText", ScreenBtn) as TextBlock;
            
            tb.Text = number.ToString() + ". " + MainWindow_Rufous.g_settingData.m_MatchList[number - 1].ItemName;

            //TextBlock tb1 = ScreenBtn.Template.FindName("InfoText", ScreenBtn) as TextBlock;
            //string str = string.Empty;

            TextBlock tb1 = ScreenBtn.Template.FindName("adfText", ScreenBtn) as TextBlock;
            string adf = adfmode[Convert.ToInt32(MainWindow_Rufous.g_settingData.m_MatchList[number - 1].m_ScanSettings.ADFMode)];
            tb1.Text = adf;

            TextBlock tb2 = ScreenBtn.Template.FindName("dpiText", ScreenBtn) as TextBlock;
            string dpistr = "";
            switch (MainWindow_Rufous.g_settingData.m_MatchList[number - 1].m_ScanSettings.ScanResolution)
            {
                case EnumScanResln._150x150:
                    dpistr = "150DPI";
                    break;

                case EnumScanResln._200x200:
                    dpistr = "200DPI";
                    break;

                case EnumScanResln._300x300:
                    dpistr = "300DPI";
                    break;

                case EnumScanResln._600x600:
                    dpistr = "600DPI";
                    break;
            }
            tb2.Text = dpistr;

            TextBlock tb3 = ScreenBtn.Template.FindName("colorText", ScreenBtn) as TextBlock;
            string colormode = "";
            switch (MainWindow_Rufous.g_settingData.m_MatchList[number - 1].m_ScanSettings.ColorType)
            {
                case EnumColorType.grayscale_8bit:
                    colormode = "GrayScale";
                    break;

                case EnumColorType.color_24bit:
                    colormode = "Color";
                    break;
            }
            tb3.Text = colormode;
            //str = "ADF : " + adf;
            //str += "\r\n";
            //str += "DPI : " + dpistr;
            //str += "\r\n";
            //str += "Mode : " + colormode;
            //tb1.Text = str;
        }

        private void LeftButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)//RoutedEventArgs e)
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
        
        private void RightButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)//RoutedEventArgs e)
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
