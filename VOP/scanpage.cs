﻿using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Collections.Generic;
using System.Windows.Interop; // for HwndSource
using System.Threading;
using Microsoft.Win32; // for SaveFileDialog
using PdfEncoderClient;

namespace VOP
{
    public partial class ScanPage : UserControl
    {
#region scan parameters
        private EnumScanDocType   m_docutype   = EnumScanDocType.Photo;
        private EnumScanResln     m_scanResln  = EnumScanResln._300x300;
        private EnumPaperSizeScan m_paperSize  = EnumPaperSizeScan._A4;
        private EnumColorType     m_color      = EnumColorType.color_24bit;
        private int               m_brightness = 50;
        private int               m_contrast   = 50;
#endregion

#region Return value of dll.ScanEx
        private const int RETSCAN_OK             = 0;
        private const int RETSCAN_ERRORDLL       = 1;
        private const int RETSCAN_OPENFAIL       = 2;
        private const int RETSCAN_ERRORPARAMETER = 3;
        private const int RETSCAN_CMDFAIL        = 4;
        private const int RETSCAN_NO_ENOUGH_SPACE= 5;
        private const int RETSCAN_ERROR_PORT     = 6;
        private const int RETSCAN_CANCEL         = 7;
#endregion
        private Thread scanningThread = null;
        private uint WM_VOPSCAN_PROGRESS = Win32.RegisterWindowMessage("vop_scan_progress2");
        private uint WM_VOPSCAN_COMPLETED = Win32.RegisterWindowMessage("vop_scan_completed");
        // share data between UI thread and scanning thread. TODO: add sync
        // for objScan
        private ScanFiles objScan = null; 

        public ScanPage()
        {
            InitializeComponent();

            ScanFiles objScan   = new ScanFiles();
            objScan.m_pathOrig  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_pathView  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_pathThumb  = @"F:\VOP\VOP\Images\image1.bmp";
            objScan.m_colorMode  = EnumColorType.color_24bit;
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );
            App.scanFileList.Add( objScan );

            foreach( ScanFiles obj in App.scanFileList )
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

            if ( 0 < GetSelectedItemCount() )
            {
                btnPrint.IsEnabled = true;
                btnSave.IsEnabled = true;
            }
            else
            {
                btnPrint.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
        }

        private void ImageItemDoubleClick(object sender, RoutedEventArgs e)
        {

            ImageItem img = (ImageItem)sender;

            ScanPreview win = new ScanPreview();
            win.Owner = App.Current.MainWindow;
			win.m_images = img.m_images;
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

                txtBlkImgSize.Text = GetScanSize().ToString()+" bytes";
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

        public void DoScanning()
        {
            objScan = new ScanFiles();
            objScan.m_colorMode = m_color;

            string strFolder = System.IO.Path.GetTempPath()+"VOPCache\\";
            string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString( "D10" );

            if ( false == Directory.Exists(strFolder) ) 
            {
                Directory.CreateDirectory( strFolder );
            }

            objScan.m_pathOrig  = strFolder + "vopOrig" + strSuffix + ".bmp";
            objScan.m_pathView  = strFolder + "vopView" + strSuffix + ".bmp";
            objScan.m_pathThumb = strFolder + "vopThum" + strSuffix + ".bmp";

            int scanMode   = (int)m_color;
            int resolution = (int)m_scanResln;
            int nWidth     = 0;
            int nHeight    = 0;
            int contrast   = m_contrast;
            int brightness = m_brightness;
            int docutype   = (int)m_docutype;

            common.GetPaperSize( m_paperSize, ref nWidth, ref nHeight );

            int nResult = dll.ScanEx(
                    m_MainWin.statusPanelPage.m_selectedPrinter ,
                    objScan.m_pathOrig     ,
                    objScan.m_pathView     ,
                    objScan.m_pathThumb    ,
                    scanMode   ,
                    resolution ,
                    nWidth     ,
                    nHeight    ,
                    contrast   ,
                    brightness ,
                    docutype   ,
                    WM_VOPSCAN_PROGRESS);

            Win32.PostMessage( (IntPtr)0xffff, WM_VOPSCAN_COMPLETED, (IntPtr)nResult, IntPtr.Zero );

        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = true;
            scanningThread = new Thread(DoScanning);
            scanningThread.Start();

        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int s = GetScanSize();

            txtBlkImgSize.Text = s.ToString()+" bytes";

            btnPrint.IsEnabled = false;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

			//Configure the ProgressBar
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            txtProgressPercent.Text = "0";

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }


        /// <summary>
        /// Scaning Image size in byte.
        /// </summary>
        private int GetScanSize()
        {
            double size = 0;

            int nWidth = 0;
            int nHeight = 0;
            int dpi = (int)m_scanResln;
            double fClrDeep = 1;

            common.GetPaperSize( m_paperSize, ref nWidth, ref nHeight );

            switch ( m_color )
            {
                case EnumColorType.black_white :
                    fClrDeep = 1.0/8;
                    break;
                case EnumColorType.grayscale_8bit :
                    fClrDeep = 1;
                    break;
                case EnumColorType.color_24bit :
                    fClrDeep = 3;
                    break;
                case EnumColorType.color_48bit :
                    fClrDeep = 6;
                    break;
                default:
                    fClrDeep = 1;
                    break;
            }

            size = nWidth/1000*nHeight/1000*dpi*dpi*fClrDeep;

            return (int)size;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ( WM_VOPSCAN_PROGRESS == msg )
            {                        
                 handled = true;

                 progressBar1.Value = wParam.ToInt32();
                 txtProgressPercent.Text = wParam.ToString();
            } 
            else if ( WM_VOPSCAN_COMPLETED == msg )
            {
                 handled = true;
                 btnCancel.IsEnabled = false;

                 if ( RETSCAN_OK == (int)wParam )
                 {
                     ImageItem img  = new ImageItem();
                     img.m_images = objScan;

                     if ( null != img.m_source )
                     {
                         img.ImageSingleClick += ImageItemSingleClick;
                         img.ImageDoubleClick += ImageItemDoubleClick;
                         img.CheckImage( false );
                         img.Margin = new Thickness( 5 );
                         this.image_wrappanel.Children.Insert(0, img );
                         App.scanFileList.Add( objScan );
                     }
                 }
                 else if ( RETSCAN_CANCEL == (int)wParam )
                 {
                     // TODO: Clear this message in release version.
                     m_MainWin.statusPanelPage.ShowMessage( "Scan cancel" );

                     progressBar1.Value = 0;
                     txtProgressPercent.Text = "0";
                 }
                 else
                 {
                     m_MainWin.statusPanelPage.ShowMessage( "Scan Fail" );
                 }
            }

            return IntPtr.Zero;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            dll.CancelScanning();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            List<string> files = new List<string>();
            GetSelectedFile( files );

            m_MainWin.SwitchToPrintingPage( files );
        }

        /// <summary>
        /// Get the file paths of selected image item.
        /// </summary>
        /// <param name="files"> Container used to store the file name.  </param>
        private void GetSelectedFile( List<string> files )
        {
            files.Clear();

            foreach (object obj in image_wrappanel.Children)
            {
                ImageItem img = obj as ImageItem;

                if ( null != img && true == img.m_ischeck )
                {
                    files.Add( img.m_images.m_pathOrig );
                }
            }  
        }

        /// <summary>
        /// Get the amount of selected items.
        /// </summary>
        private int GetSelectedItemCount()
        {
            int nCount = 0;
            foreach ( object obj in image_wrappanel.Children )
            {
                ImageItem img = obj as ImageItem;

                if ( null != img && true == img.m_ischeck )
                    nCount++;
            }

            return nCount;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            if ( 1 < GetSelectedItemCount() )
                save.Filter = "TIF|*.tif|PDF|*.pdf";
            else 
                save.Filter = "TIF|*.tif|PDF|*.pdf|JPG|*.jpg";

            bool? result = save.ShowDialog();

            if (result == true)
            {
                // This index is 1-based, not 0-based
                if (3 == save.FilterIndex)
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                    foreach (object obj in image_wrappanel.Children)
                    {
                        ImageItem img = obj as ImageItem;

                        if ( null != img && true == img.m_ischeck )
                        {
                            Uri myUri = new Uri(img.m_images.m_pathOrig, UriKind.RelativeOrAbsolute);
                            BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                            BitmapSource origSource = decoder.Frames[0];

                            if (null != origSource)
                            {
                                switch (img.m_images.m_rotate)
                                {
                                    case 0:
                                        encoder.Rotation = Rotation.Rotate0;
                                        break;
                                    case 90:
                                        encoder.Rotation = Rotation.Rotate90;
                                        break;
                                    case 180:
                                        encoder.Rotation = Rotation.Rotate180;
                                        break;
                                    case 270:
                                        encoder.Rotation = Rotation.Rotate270;
                                        break;

                                }
                                encoder.Frames.Add(BitmapFrame.Create(origSource));
                            }
                        }
                    }  

                    FileStream fs = File.Open(save.FileName, FileMode.Create);
                    encoder.Save(fs);
                    fs.Close();
                }
                else if (1 == save.FilterIndex)
                {
                    TiffBitmapEncoder encoder = new TiffBitmapEncoder();

                    foreach (object obj in image_wrappanel.Children)
                    {
                        ImageItem img = obj as ImageItem;

                        if (null != img && true == img.m_ischeck)
                        {
                            BitmapSource origSource = common.GetOrigBitmapSource( img.m_images );

                            if ( null != origSource )
                                encoder.Frames.Add(BitmapFrame.Create(origSource));
                        }
                    }  

                    FileStream fs = File.Open(save.FileName, FileMode.Create);
                    encoder.Save(fs);
                    fs.Close();
                }
                else if (2 == save.FilterIndex)
                {

                    try
                    {
                        using (PdfHelper help = new PdfHelper())
                        {
                            help.Open(save.FileName);

                            foreach (object obj in image_wrappanel.Children)
                            {
                                ImageItem img = obj as ImageItem;

                                if (null != img && true == img.m_ischeck)
                                {
                                    Uri myUri = new Uri(img.m_images.m_pathOrig, UriKind.RelativeOrAbsolute);
                                    BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                    BitmapSource origSource = decoder.Frames[0];

                                    if ( null != origSource )
                                        help.AddImage(origSource, img.m_images.m_rotate);
                                }
                            }  

                            help.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }

                }
            }
            
        }
    }
}
