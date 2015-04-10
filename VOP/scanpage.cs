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

#region scan parameters
        private EnumScanDocType   m_docutype   = EnumScanDocType.Photo;
        private EnumScanResln     m_scanResln  = EnumScanResln._300x300;
        private EnumPaperSizeScan m_paperSize  = EnumPaperSizeScan._A4;
        private EnumColorType     m_color      = EnumColorType.color_24bit;
        private int               m_brightness = 50;
        private int               m_contrast   = 50;
#endregion

        public ScanPage()
        {
            InitializeComponent();

            ScanFiles objScan   = new ScanFiles();
            objScan.m_pathOrig  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_pathView  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_pathThumb  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_colorMode  = EnumColorType.color_24bit;
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
			win.m_images = scanFileList[0];
            win.ShowDialog();
        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            ScanSetting win = new ScanSetting();

            win.m_docutype   = m_docutype;
            win.m_scanResln  = m_scanResln;
            win.m_paperSize  = m_paperSize;
            win.m_color      = m_color;
            win.m_brightness = m_brightness;
            win.m_contrast   = m_contrast;

            win.Owner = m_MainWin;
         
            if (true == win.ShowDialog())
            {
                m_docutype   = win.m_docutype;
                m_scanResln  = win.m_scanResln;
                m_paperSize  = win.m_paperSize;
                m_color      = win.m_color;
                m_brightness = win.m_brightness;
                m_contrast   = win.m_contrast;
            }
        }

        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        private MainWindow _MainWin = null;
        public MainWindow m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if ( null == _MainWin )
                {
                    return ( MainWindow )App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }


        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            string szOrig  = "";
            string szView  = "";
            string szThumb = "";

            int scanMode   = (int)m_color;
            int resolution = (int)m_scanResln;
            int width      = 0;
            int height     = 0;
            int contrast   = m_contrast;
            int brightness = m_brightness;
            int docutype   = (int)m_docutype;
            uint uMsg      = 0;

            switch ( m_paperSize )
            { 
                case EnumPaperSizeScan._A4         :
                    width  = 8268;
                    height = 11693;
                    break;
                case EnumPaperSizeScan._A5         :
                    width  = 5827;
                    height = 8268;
                    break;
                case EnumPaperSizeScan._B5         :
                    width  = 7165;
                    height = 10118;
                    break;
                case EnumPaperSizeScan._Letter     :
                    width  = 8504;
                    height = 10984;
                    break;
                case EnumPaperSizeScan._4x6Inch :
                    width  = 4000;
                    height = 6000;
                    break;
                default:
                    width  = 8268;
                    height = 11693;
                    break;
            }

            dll.ScanEx(
                    m_MainWin.statusPanelPage.m_selectedPrinter,
                    szOrig,
                    szView,
                    szThumb,
                    scanMode,
                    resolution,
                    width,
                    height,
                    contrast,
                    brightness,
                    docutype,
                    uMsg );
        }
    }
}
