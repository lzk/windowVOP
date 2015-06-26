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
        private const int RETSCAN_NO_ENOUGH_SPACE= 5;
        private const int RETSCAN_ERROR_PORT     = 6;
        private const int RETSCAN_CANCEL         = 7;
        private const int RETSCAN_BUSY           = 8;
        private const int RETSCAN_ERROR          = 9;
#endregion
        private EnumStatus m_currentStatus = EnumStatus.Offline;

        public Thread scanningThread = null;

        private uint WM_VOPSCAN_PROGRESS = Win32.RegisterWindowMessage("vop_scan_progress2");
        private uint WM_VOPSCAN_COMPLETED = Win32.RegisterWindowMessage("vop_scan_completed");

        // share data between UI thread and scanning thread. 
        private ScanFiles m_shareObj = null; 
        private object objLock = new object(); 

        // InitialDirectory for SaveFileDialog.
        private string strInitalDirectory = "";

        // Flags present the WndProc had been hooked or not.
        private bool m_bHooked = false;

        // Flag present the whether doing scanning job.
        private bool _isScanning = false; 
        public bool m_isScanning 
        {
            get 
            {
                return _isScanning;
            }
        }

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

            if ( 0 == img.m_num )
                img.m_num = GetSelectedItemCount()+1;
            else
                img.m_num = 0;
            
            UpdateSelItemNum();

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

            img.m_num = GetSelectedItemCount()+1;

            btnPrint.IsEnabled = true;
            btnSave.IsEnabled = true;

            ScanPreview win = new ScanPreview();
            win.Owner       = m_MainWin;
			win.m_images    = img.m_images;
            win.isPrint     = false;

            win.ShowDialog();

            if ( true == win.isPrint )
            {
                List<string> files = new List<string>();

                if ( 0 != win.m_rotatedAngle && null != win.m_rotatedObj )
                    files.Add( win.m_rotatedObj.m_pathOrig );
                else
                    files.Add( img.m_images.m_pathOrig );

                m_MainWin.SwitchToPrintingPage( files );
            }

            if ( 0 != win.m_rotatedAngle && null != win.m_rotatedObj )
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

                        // Collect the rubbish files.
                        App.rubbishFiles.Add( img.m_images );

                        tmp.ImageSingleClick += ImageItemSingleClick;
                        tmp.ImageDoubleClick += ImageItemDoubleClick;
                        tmp.CloseIconClick += ImageItemCloseIconClick;

                        tmp.m_num = GetSelectedItemCount()+1;

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

        public void DoScanning()
        {
            string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString( "D10" );

            if ( false == Directory.Exists(App.cacheFolder) ) 
            {
                Directory.CreateDirectory( App.cacheFolder );
            }

            lock ( objLock )
            {
                m_shareObj = new ScanFiles();
                m_shareObj.m_colorMode = m_color;
                m_shareObj.m_pathOrig  = App.cacheFolder + "\\vopOrig" + strSuffix + ".bmp";
                m_shareObj.m_pathView  = App.cacheFolder + "\\vopView" + strSuffix + ".bmp";
                m_shareObj.m_pathThumb = App.cacheFolder + "\\vopThum" + strSuffix + ".bmp";
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
            this.image_wrappanel.IsEnabled = false;
            btnPrint.IsEnabled = false;
            btnSave.IsEnabled = false;

            _isScanning = true;
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

            InitFontSize();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // zh-CN
            {
                btnPrint.FontSize = btnSave.FontSize = btnSetting.FontSize = btnScan.FontSize = 17.87;
                txtBlk_ScannedImageSize.FontSize = txtBlkImgSize.FontSize = txtProgressLabel.FontSize = 14; 
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
            } 
            else if ( WM_VOPSCAN_COMPLETED == msg )
            {
                _isScanning = false;
                 handled = true;

                 btnCancel.IsEnabled  = false;
                 btnSetting.IsEnabled = true;
                 m_MainWin.statusPanelPage.EnableSwitchPrinter( true );
                 this.image_wrappanel.IsEnabled = true;

                 if ( 0 < GetSelectedItemCount() )
                 {
                     btnPrint.IsEnabled = true;
                     btnSave.IsEnabled = true;
                 }
                 
                 btnScan.IsEnabled = ( false == common.IsOffline(m_currentStatus) && false == m_isScanning );

                 progressBar1.Value = 0;

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
                         img.m_num = 0;
                         UpdateSelItemNum();

                         img.Margin = new Thickness( 5 );
                         this.image_wrappanel.Children.Insert(0, img );
                         App.scanFileList.Add( img.m_images );
                     }
                     else
                     {
                         VOP.Controls.MessageBoxEx.Show(
                                 VOP.Controls.MessageBoxExStyle.Simple,
                                 m_MainWin,
                                 (string)this.FindResource( "ResStr_Operation_cannot_be_carried_out_due_to_insufficient_memory_or_hard_disk_space_Please_try_again_after_freeing_memory_or_hard_disk_space_" ),
                                 (string)this.FindResource( "ResStr_Error" )
                                 );
                     }
                 }
                 else if ( RETSCAN_CANCEL == (int)wParam )
                 {
                 }
                 else if ( RETSCAN_NO_ENOUGH_SPACE == (int)wParam )
                 {
                     VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_insufficient_system_disk_space"), (string)this.FindResource("ResStr_Error"));
                 }
                 else
                 {
                     m_MainWin.statusPanelPage.ShowMessage( (string)this.FindResource("ResStr_Scan_Fail"), Brushes.Red );

                     if ( RETSCAN_OPENFAIL == (int)wParam
                             || RETSCAN_ERRORDLL == (int)wParam
                             || RETSCAN_ERROR_PORT == (int)wParam)
                     {
                         VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                                 m_MainWin,
                                 (string)this.FindResource( "ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_" ),
                                 (string)this.FindResource( "ResStr_Error" ));
                     }
                     else if ( RETSCAN_ERROR == (int)wParam )
                     {
                         VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                                 m_MainWin,
                                 (string)this.FindResource( "ResStr_Operation_can_not_be_carried_out_due_to_machine_malfunction_"),
                                 (string)this.FindResource( "ResStr_Error" ));
                     }
                     else if ( RETSCAN_BUSY == (int)wParam )
                     {
                         VOP.Controls.MessageBoxEx.Show( VOP.Controls.MessageBoxExStyle.Simple,
                                 m_MainWin,
                                 (string)this.FindResource( "ResStr_The_machine_is_busy__please_try_later_" ),
                                 (string)this.FindResource( "ResStr_Warning" ));
                     }

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
            FileSelectionPage.IsInitPrintSettingPage = true;//Init print setting

            m_MainWin.SwitchToPrintingPage( files );
        }

        /// <summary>
        /// Get the file paths of selected image item.
        /// </summary>
        /// <param name="files"> Container used to store the file name.  </param>
        private void GetSelectedFile( List<string> files )
        {
            files.Clear();

            int nCount = GetSelectedItemCount();

            if ( nCount > 0 )
            {
                files.Capacity = nCount;

                foreach (object obj in image_wrappanel.Children)
                {
                    ImageItem img = obj as ImageItem;

                    if ( null != img && 0 < img.m_num )
                    {
                        files[img.m_num-1] = img.m_images.m_pathOrig;
                    }
                }  
            }

        }

        /// <summary>
        /// Update the number of selected image items.
        /// </summary>
        private void UpdateSelItemNum()
        {
            List<int> lst = new List<int>();

            for ( int i=0; i<image_wrappanel.Children.Count; i++ )
            {
                ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

                if ( null != img1 && 0 < img1.m_num )
                {
                    lst.Add( img1.m_num );
                }
            }

            lst.Sort();

            int nMiss = 0;
            for ( int i=0; i<lst.Count; i++ )
            {
                if ( lst[i] != i+1 )
                {
                    nMiss = i+1;
                    break;
                }
            }

            if ( nMiss > 0 )
            {
                for ( int i=0; i<image_wrappanel.Children.Count; i++ )
                {
                    ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

                    if ( null != img1 && 0 < img1.m_num && nMiss <= img1.m_num )
                    {
                        img1.m_num--; 
                    }
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

                if ( null != img && 0 < img.m_num )
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
                
                if ( false == DoseHasEnoughSpace(save.FileName) )
                {
                    VOP.Controls.MessageBoxEx.Show(
                            VOP.Controls.MessageBoxExStyle.Simple,
                            m_MainWin,
                            (string)this.FindResource( "ResStr_Operation_cannot_be_carried_out_due_to_insufficient_memory_or_hard_disk_space_Please_try_again_after_freeing_memory_or_hard_disk_space_" ),
                            (string)this.FindResource( "ResStr_Error" )
                            );

                    return;
                }

                // This index is 1-based, not 0-based
                try
                {
                    List<string> files = new List<string>();
                    GetSelectedFile( files );

                    if (3 == save.FilterIndex)
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                        foreach ( string path in files )
                        {
                            Uri myUri = new Uri( path, UriKind.RelativeOrAbsolute);
                            BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad );
                            BitmapSource origSource = decoder.Frames[0];

                            if (null != origSource)
                                encoder.Frames.Add(BitmapFrame.Create(origSource));
                        }  

                        FileStream fs = File.Open(save.FileName, FileMode.Create);
                        encoder.Save(fs);
                        fs.Close();
                    }
                    else if (1 == save.FilterIndex)
                    {
                        TiffBitmapEncoder encoder = new TiffBitmapEncoder();

                        foreach ( string path in files )
                        {
                            Uri myUri = new Uri( path, UriKind.RelativeOrAbsolute );
                            BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad );
                            BitmapSource origSource = decoder.Frames[0];

                            BitmapMetadata bitmapMetadata = new BitmapMetadata("tiff");
                            bitmapMetadata.ApplicationName = "Virtual Operation Panel";

                            if (null != origSource)
                                encoder.Frames.Add(BitmapFrame.Create(origSource, null, bitmapMetadata, null));
                        }  

                        FileStream fs = File.Open(save.FileName, FileMode.Create);
                        encoder.Save(fs);
                        fs.Close();
                    }
                    else if (2 == save.FilterIndex)
                    {
                        if ( false == DoseHasEnoughSpace( App.cacheFolder ) )
                        {
                            VOP.Controls.MessageBoxEx.Show(
                                    VOP.Controls.MessageBoxExStyle.Simple,
                                    Application.Current.MainWindow,
                                    (string)this.FindResource("ResStr_insufficient_system_disk_space"),
                                    (string)this.FindResource("ResStr_Error"));
                        }
                        else
                        {
                            using (PdfHelper help = new PdfHelper())
                            {
                                help.Open(save.FileName);

                                foreach ( string path in files )
                                {
                                    Uri myUri = new Uri( path, UriKind.RelativeOrAbsolute );
                                    BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad );
                                    BitmapSource origSource = decoder.Frames[0];

                                    if ( null != origSource )
                                        help.AddImage(origSource, 0);
                                }  

                                help.Close();
                            }
                        }

                    }
                }
                catch
                {
                    VOP.Controls.MessageBoxEx.Show(
                            VOP.Controls.MessageBoxExStyle.Simple,
                            m_MainWin,
                            (string)this.FindResource( "ResStr_Operation_cannot_be_carried_out_due_to_insufficient_memory_or_hard_disk_space_Please_try_again_after_freeing_memory_or_hard_disk_space_" ),
                            (string)this.FindResource( "ResStr_Error" )
                            );
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
            btnSetting.IsEnabled = true;
            btnScan.IsEnabled = ( false == common.IsOffline(m_currentStatus) && false == m_isScanning );

			//Configure the ProgressBar
            progressBar1.Minimum    = 0;
            progressBar1.Maximum    = 100;
            progressBar1.Value      = 0;

            txtBlkImgSize.Text = FormatSize( GetScanSize() );
        }

        /// <summary>
        /// Get free space of disk specified by strPath.
        /// </summary>
        /// <param name="strPath"> Path of folder or file. </param>
        private bool DoseHasEnoughSpace( string strPath )
        {
            bool bIsHasEnoughSpace = true;

            if ( strPath.Length >= 3 )
            {
                string strDisk = strPath.Substring(0, 3);
                long lFreeSpace = GetTotalFreeSpace( strDisk );

                long lNeedSpace = 0;

                foreach (object obj in image_wrappanel.Children)
                {
                    ImageItem img = obj as ImageItem;

                    if ( null != img && 0 < img.m_num )
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo( img.m_images.m_pathOrig );
                        lNeedSpace += fi.Length;
                    }
                }

                bIsHasEnoughSpace = lFreeSpace > lNeedSpace;
            }

            return bIsHasEnoughSpace;
        }

        /// <summary>
        /// Format of parameter driveName: "X:\". Case insensitive.
        /// </summary>
        private long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName &&
                        string.Compare( drive.Name, driveName, StringComparison.OrdinalIgnoreCase) == 0 )
                {
                    return drive.TotalFreeSpace;
                }
            }

            return -1;
        }

        /// <summary>
        /// Status update thread will invoke this interface to update status of subpage.
        /// </summary>
        public void PassStatus( EnumStatus st, EnumMachineJob job, byte toner )
        {
            m_currentStatus = st;
            btnScan.IsEnabled = ( false == common.IsOffline(m_currentStatus) && false == m_isScanning );
        }
    }
}
