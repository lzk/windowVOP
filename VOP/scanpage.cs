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
        List<ScanFiles> scanFileList = new List<ScanFiles>();

        public ScanPage()
        {
            InitializeComponent();

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
                img.CloseIconClick += ImageItemCloseIconClick;

                if ( null != img.m_source )
                {
                    this.image_wrappanel.Children.Insert(0, img );
                }
            }
        }

        private void ImageItemCloseIconClick(object sender, RoutedEventArgs e)
        {
            ImageItem img = (ImageItem)sender;

            int index = 0;
            foreach ( object obj in image_wrappanel.Children )
            {
                if ( obj == img )
                {
                    image_wrappanel.Children.RemoveAt( index );
                    break;
                }
                index++;
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
            ScanPreview win = new ScanPreview();
            win.Owner = App.Current.MainWindow;
            win.ShowDialog();
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            ScanSetting win = new ScanSetting();
            
            win.Owner = App.Current.MainWindow;
            win.ShowDialog();
        }
    }
}
