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
            ScanTask task = new ScanTask();
            ScanParam param = new ScanParam(
                EnumScanDocType.Graphic,
                EnumScanResln._300x300,
                EnumPaperSizeScan._A4,
                EnumColorType.color_24bit,
                50,
                50);

         //   ScanFiles files = task.DoScan("Lenovo M7208W (副本 1)", param);

          //  if(task.ScanResult == Scan_RET.RETSCAN_OK)
            {
              //  QRCodeWindow win = new QRCodeWindow(files.m_pathOrig);
                QRCodeWindow win = new QRCodeWindow(@"E:\BarCode\pic\try\bar_g300_2.jpg");
                win.Owner = m_MainWin;
                win.ShowDialog();
            }
        }
    }
}
