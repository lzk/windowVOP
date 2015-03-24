using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Collections.Generic;

namespace VOP
{
    public partial class ScanPage : UserControl
    {
        public ScanPage()
        {
            InitializeComponent();

            List<ScanFiles> scanFileList = new List<ScanFiles>();

            ScanFiles objScan   = new ScanFiles();
            objScan.m_pathOrig  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_pathView  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_pathThumb  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_colorMode  = enum_color.color_24bit;
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );
            scanFileList.Add( objScan );

            foreach( ScanFiles obj in scanFileList )
            {
                ImageItem img  = new ImageItem();
                img.m_images = obj;
                img.CheckImage( false );
                img.Margin = new Thickness( 5 );

                img.ImageSingleClick += ImageItemSingleClick;
                img.ImageDoubleClick += ImageItemDoubleClick;

                if ( null != img.m_source )
                {
                    this.image_wrappanel.Children.Insert(0, img );
                }
            }
        }

        private void ImageItemSingleClick(object sender, RoutedEventArgs e)
        {
            ImageItem img = (ImageItem)sender;

            img.m_ischeck = !img.m_ischeck;
            img.CheckImage( img.m_ischeck );
        }

        private void ImageItemDoubleClick(object sender, RoutedEventArgs e)
        {
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            ScanSetting win = new ScanSetting();
            
            win.Owner = App.Current.MainWindow;
            win.ShowDialog();
        }
    }
}
