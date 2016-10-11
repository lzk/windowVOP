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

        private void QRCodeButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

            //ScanTask task = new ScanTask();
            //ScanParam param = new ScanParam(
            //    EnumScanResln._300x300,
            //    EnumPaperSizeScan._A4,
            //    EnumColorType.color_24bit,
            //    true,
            //    50,
            //    50);

            //List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", param);

            //if (files == null)
            //    return;

            //  if(task.ScanResult == Scan_RET.RETSCAN_OK)
            {
                // QRCodeWindow win = new QRCodeWindow(files[0].m_pathOrig);

                OpenFileDialog open = null;
                bool? result = null;
                open = new OpenFileDialog();
                open.Filter = "All Images|*.jpg;*.bmp;*.png;*.tif|JPEG|*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";
                open.Multiselect = false;

                result = open.ShowDialog();
                if (result == true)
                {
                        QRCodeWindow win = new QRCodeWindow(open.FileName);

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

            //ScanTask task = new ScanTask();
            //ScanParam param = new ScanParam(
            //    EnumScanResln._300x300,
            //    EnumPaperSizeScan._A4,
            //    EnumColorType.color_24bit,
            //    true,
            //    50,
            //    50);

            // List<ScanFiles> files = task.DoScan("Lenovo M7208W (副本 1)", param);

            //if (files == null)
            //    return;

            // if (task.ScanResult == Scan_RET.RETSCAN_OK)
            {
                List<ScanFiles> files = new List<ScanFiles>();
                files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_A00.JPG"));
                files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_A01.JPG"));
                files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_A02.JPG"));
                files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_B00.JPG"));
                files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_B01.JPG"));
                files.Add(new ScanFiles(@"G:\work\Rufous\pic\1313783656_C300_B02.JPG"));

                m_MainWin.GotoPage("ScanPage", files);
            }
        }

        private void SettingsButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_MainWin.GotoPage("SettingsPage", null);
        }
    }
}
