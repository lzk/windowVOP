using System.Windows.Controls;
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

        // share data between UI thread and scanning thread. 
        private ScanFiles m_shareObj = null; 
        private object objLock = new object(); 

        // InitialDirectory for SaveFileDialog.
        private string strInitalDirectory = "";

        // Flags present the WndProc had been hooked or not.
        private bool m_bHooked = false;

        public ScanPage()
        {
            InitializeComponent();
            ResetToDefaultValue();

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
            win.Owner       = m_MainWin;
			win.m_images    = img.m_images;
            win.isPrint     = false;

            win.ShowDialog();

            if ( true == win.isPrint )
            {
                List<string> files = new List<string>();
                files.Add( img.m_images.m_pathOrig );
                m_MainWin.SwitchToPrintingPage( files );
            }

            if ( 0 != win.m_rotatedAngle%360 && null != win.m_rotatedObj )
            {
                int index = -1;
                for ( int i=0; i<image_wrappanel.Children.Count; i++ )
                {
                    if ( image_wrappanel.Children[i] == img )
                    {
                        index = i;
                        break;
                    }
                }

                if ( -1 != index )
                {
                    ImageItem tmp  = new ImageItem();
                    tmp.m_images = win.m_rotatedObj;

                    if ( tmp.m_iSimgReady )
                    {
                        this.image_wrappanel.Children.RemoveAt( index );

                        // Remove original cache files.
                        File.Delete(img.m_images.m_pathOrig);
                        File.Delete(img.m_images.m_pathView);
                        File.Delete(img.m_images.m_pathThumb);

                        tmp.ImageSingleClick += ImageItemSingleClick;
                        tmp.ImageDoubleClick += ImageItemDoubleClick;
                        tmp.CloseIconClick += ImageItemCloseIconClick;
                        tmp.CheckImage( false );
                        tmp.Margin = new Thickness( 5 );
                        this.image_wrappanel.Children.Insert(index, tmp );
                        App.scanFileList.Add( tmp.m_images );
                    }
                }
            }
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

                txtBlkImgSize.Text = FormatSize( GetScanSize() );
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
            btnScan.IsEnabled = ( EnumState.init == state );
        }

        public void DoScanning()
        {
            string strFolder = System.IO.Path.GetTempPath()+"VOPCache\\";
            string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString( "D10" );

            if ( false == Directory.Exists(strFolder) ) 
            {
                Directory.CreateDirectory( strFolder );
            }

            lock ( objLock )
            {
                m_shareObj = new ScanFiles();
                m_shareObj.m_colorMode = m_color;
                m_shareObj.m_pathOrig  = strFolder + "vopOrig" + strSuffix + ".bmp";
                m_shareObj.m_pathView  = strFolder + "vopView" + strSuffix + ".bmp";
                m_shareObj.m_pathThumb = strFolder + "vopThum" + strSuffix + ".bmp";
            }

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
                    m_shareObj.m_pathOrig     ,
                    m_shareObj.m_pathView     ,
                    m_shareObj.m_pathThumb    ,
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
            btnCancel.IsEnabled  = true;
            btnScan.IsEnabled    = false;
            btnSetting.IsEnabled = false;
            m_MainWin.statusPanelPage.EnableSwitchPrinter( false );

            scanningThread = new Thread(DoScanning);
            scanningThread.Start();

        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if ( false == m_bHooked )
            {
                HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
                source.AddHook(WndProc);

                m_bHooked = true;
            }
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

                 btnCancel.IsEnabled  = false;
                 btnScan.IsEnabled    = true;
                 btnSetting.IsEnabled = true;
                 m_MainWin.statusPanelPage.EnableSwitchPrinter( true );
                 
                 progressBar1.Value = 0;
                 txtProgressPercent.Text = "0";

                 if ( RETSCAN_OK == (int)wParam )
                 {
                     ImageItem img  = new ImageItem();

                     lock ( objLock )
                     {
                         img.m_images = m_shareObj;
                     }

                     if ( img.m_iSimgReady )
                     {
                         img.ImageSingleClick += ImageItemSingleClick;
                         img.ImageDoubleClick += ImageItemDoubleClick;
                         img.CloseIconClick += ImageItemCloseIconClick;
                         img.CheckImage( false );
                         img.Margin = new Thickness( 5 );
                         this.image_wrappanel.Children.Insert(0, img );
                         App.scanFileList.Add( img.m_images );
                     }
                 }
                 else if ( RETSCAN_CANCEL == (int)wParam )
                 {
                 }
                 else
                 {
                     m_MainWin.statusPanelPage.ShowMessage( "Scan Fail", Brushes.Black );
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

            save.InitialDirectory = strInitalDirectory; 

            if ( 1 < GetSelectedItemCount() )
                save.Filter = "TIF|*.tif|PDF|*.pdf";
            else 
                save.Filter = "TIF|*.tif|PDF|*.pdf|JPG|*.jpg";

            bool? result = save.ShowDialog();

            if (result == true)
            {
                strInitalDirectory = save.FileName;

                int position = strInitalDirectory.LastIndexOf('\\'); 
                if (position > 0)
                    strInitalDirectory = strInitalDirectory.Substring( 0, position );

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
                            BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad );
                            BitmapSource origSource = decoder.Frames[0];

                            if (null != origSource)
                                encoder.Frames.Add(BitmapFrame.Create(origSource));
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
                                    BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad );
                                    BitmapSource origSource = decoder.Frames[0];

                                    if ( null != origSource )
                                        help.AddImage(origSource, 0);
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

        /// <summary>
        /// Format size in byte to string, leave two fraction if size large
        /// than 1 kb.
        /// </summary>
        private string FormatSize( int size )
        {
            int _1k = 1024;
            int _1m = _1k*1024;
            int _1g = _1m*1024;

            double fSize = size;

            string str = "";

            if ( _1k > size )
            {
                str = size.ToString()+"B";
            }
            else if ( _1m > size )
            {
                fSize/=_1k;
                str = fSize.ToString("F2")+"KB";
            }
            else if ( _1g > size )
            {
                fSize/=_1m;
                str = fSize.ToString("F2")+"MB";
            }
            else
            {
                fSize/=_1g;
                str = fSize.ToString("F2")+"GB";
            }

            return str;
        }

        public void ResetToDefaultValue()
        {
            m_docutype   = EnumScanDocType.Photo;
            m_scanResln  = EnumScanResln._300x300;
            m_paperSize  = EnumPaperSizeScan._A4;
            m_color      = EnumColorType.color_24bit;
            m_brightness = 50;
            m_contrast   = 50;

            strInitalDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            btnPrint.IsEnabled   = false;
            btnSave.IsEnabled    = false;
            btnCancel.IsEnabled  = false;
            btnScan.IsEnabled    = true;
            btnSetting.IsEnabled = true;

			//Configure the ProgressBar
            progressBar1.Minimum    = 0;
            progressBar1.Maximum    = 100;
            progressBar1.Value      = 0;
            txtProgressPercent.Text = "0";

            txtBlkImgSize.Text = FormatSize( GetScanSize() );
        }
    }
}
