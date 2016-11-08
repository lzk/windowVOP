using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Win32;              // for SaveFileDialog
using System.IO;                    // for File.Create
using PdfEncoderClient;
using System.Windows.Interop;
using System.Diagnostics;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanPreview.xaml
    /// </summary>
    public partial class ScanPreview : Window
    {
        public ScanFiles m_images     = null;
        public bool isPrint           = false;    // turn if user click print.
        public int m_rotatedAngle     = 0;        // Rotated angle of preview image. Value: { 0, 90, 180, 270 }.
        public ScanFiles m_rotatedObj = null;     

        private bool oldValueForPrintSettingPage = FileSelectionPage.IsInitPrintSettingPage;

        // Actual size of preview image in pixels.
        private double m_actualWidth = 0;

        public List<string> selectFileList = null;

        public ScanPreview()
        {
            InitializeComponent();

            this.Width = this.Width*  App.gScalingRate;
            this.Height = this.Height*App.gScalingRate;

            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //System.Drawing.Image img = System.Drawing.Image.FromFile( m_images.m_pathView );

                if (!File.Exists(m_images.m_pathView))
                {
                    VOP.Controls.MessageBoxEx.Show(
                       VOP.Controls.MessageBoxExStyle.Simple,
                       this,
                       (string)this.FindResource("ResStr_Image_file_not_found"),
                       (string)this.FindResource("ResStr_Error")
                       );

                    this.Close();
                }

                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri( m_images.m_pathView, UriKind.RelativeOrAbsolute );

                // Fixed bms bug #0059616: Set the DecodePixelWidth to avoid rotation operation cached memory.
                // Comment from MSDN:
                // To save significant application memory, set the DecodePixelWidth or   
                // DecodePixelHeight of the BitmapImage value of the image source to the desired  
                // height or width of the rendered image. If you don't do this, the application will  
                // cache the image as though it were rendered as its normal size rather then just  
                // the size that is displayed. 
                // Note: In order to preserve aspect ratio, set DecodePixelWidth 
                // or DecodePixelHeight but not both.
                //bi3.DecodePixelWidth = img.Width;
                //m_actualWidth = img.Width;

                bi3.EndInit();

                m_actualWidth = bi3.PixelWidth;

                // Begin: Fix 61368
                if (m_images.m_colorMode == EnumColorType.black_white)
                {
                    BitmapSource bmpSrc = bi3;
                    bmpSrc = BitmapFrame.Create(new TransformedBitmap(bmpSrc, new ScaleTransform(0.4, 0.4)));

                    // https://msdn.microsoft.com/en-us/library/system.windows.media.imaging.formatconvertedbitmap(v=vs.100).aspx
                    FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
                    convertedBitmap.BeginInit();
                    convertedBitmap.Source = bmpSrc;
                    convertedBitmap.DestinationFormat = PixelFormats.Gray2;
                    convertedBitmap.EndInit();
                    previewImg.Source = convertedBitmap;
                }
                else
                {
                    previewImg.Source = bi3;
                }
                // End: Fix 61368

                FitTheWindow();
                CenterImage();
            }
            catch(Exception)
            {
              
            }

            InitFontSize();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // zh-CN
            {
                //btnOK.FontSize = btnPrint.FontSize = 17.87;
            }
            else
            {
               // btnOK.FontSize = btnPrint.FontSize = 14;
            }
        }

        private void CenterImage()
        {
            if ( null != previewImg.Source )
            {
                if (previewImg.Height > scrollPreview.ViewportHeight)
                    scrollPreview.ScrollToVerticalOffset((previewImg.Height - scrollPreview.ViewportHeight) / 2);

                if (previewImg.Width > scrollPreview.ViewportWidth)
                    scrollPreview.ScrollToHorizontalOffset((previewImg.Width - scrollPreview.ViewportWidth) / 2);
            }
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            double y = e.GetPosition(LayoutRoot).Y;
            bool isAtTitle = false;

            foreach(RowDefinition rd in LayoutRoot.RowDefinitions)
            {
                if ( y <= rd.ActualHeight )
                {
                    isAtTitle = true;
                }

                break; // Only check the 1st row.
            }

            if ( true == isAtTitle )
                this.DragMove();
        }

        private void imagebtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            string name = btn.Name;

            if (name == "btn_zoomin" || name == "btn_zoomout" )
            {
                double scaling = 1.0;

                if (name == "btn_zoomin")
                    scaling += 0.1;
                else
                    scaling -= 0.1;

         
                switch (m_rotatedAngle)
                {
                    case 0:
                    case 180:
                            previewImg.Width = previewImg.Width * scaling;
                            previewImg.Height = previewImg.Height * scaling;
                        break;
                    case 90:
                    case 270:
                            previewImg.Width = previewImg.Height * scaling;
                            previewImg.Height = previewImg.Width * scaling;
                        break;
                }

            }
            else if ( name == "btn_normal" )
            {
                FitTheWindow();
            }
            else if (name == "btn_turn")
            {
                m_rotatedAngle += 90;
                m_rotatedAngle %= 360;

                try
                {
                    double oldWidth = previewImg.Width;
                    double oldHeight = previewImg.Height;

                    //CachedBitmap cache = new CachedBitmap(previewImg.Source as BitmapSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    //previewImg.Source = BitmapFrame.Create(new TransformedBitmap(cache, new RotateTransform(90)));

                    previewImg.LayoutTransform = new RotateTransform(m_rotatedAngle);

                    previewImg.Width = oldHeight;
                    previewImg.Height = oldWidth;

                    GC.Collect();

                    FitTheWindow();
                }
                catch
                {
                    VOP.Controls.MessageBoxEx.Show(
                            VOP.Controls.MessageBoxExStyle.Simple,
                            this,
                            (string)this.FindResource( "ResStr_Operation_cannot_be_carried_out_due_to_insufficient_memory_or_hard_disk_space_Please_try_again_after_freeing_memory_or_hard_disk_space_" ),
                            (string)this.FindResource( "ResStr_Error" )
                            );

                }
            }

            CenterImage();

            btn_zoomout.IsEnabled = ( previewImg.Height > scrollPreview.ViewportHeight/2 );
            btn_zoomin.IsEnabled  = ( previewImg.Width < m_actualWidth*4 );
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            isPrint = true;
            oldValueForPrintSettingPage = FileSelectionPage.IsInitPrintSettingPage;
            FileSelectionPage.IsInitPrintSettingPage = true;////Init print setting
            this.Close();
        }

        /// <summary>
        /// Rotate objSrc to objDst with angle nAngle.
        /// </summary>
        /// <returns> Success return true, otherwise return false. </returns>
        private bool RotateScannedFiles( ScanFiles objSrc, ScanFiles objDst, int nAngle )
        {
            bool bSuccess = true;

            string args1 = "\"" + objSrc.m_pathOrig  + "\" \"" + objDst.m_pathOrig  + "\" \"" + nAngle.ToString();
            //string args2 = "\"" + objSrc.m_pathView  + "\" \"" + objDst.m_pathView  + "\" \"" + nAngle.ToString();
            //string args3 = "\"" + objSrc.m_pathThumb + "\" \"" + objDst.m_pathThumb + "\" \"" + nAngle.ToString();

            try
            {
                // Get the ExitCode of VopHelper. 0 success, otherwise fail.
                int nExitCode1 = 0;
                //int nExitCode2 = 0;
                //int nExitCode3 = 0;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = App.vopHelperExe;
                Process exeProcess = null;

                startInfo.Arguments = args1;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
                nExitCode1 = exeProcess.ExitCode;

                //startInfo.Arguments = args2;
                //exeProcess = Process.Start(startInfo);
                //exeProcess.WaitForExit();
                //nExitCode2 = exeProcess.ExitCode;

                //startInfo.Arguments = args3;
                //exeProcess = Process.Start(startInfo);
                //exeProcess.WaitForExit();
                //nExitCode3 = exeProcess.ExitCode;

                bSuccess = ( 0 == nExitCode1 );
            }
            catch
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( 0 != m_rotatedAngle )
            {

                VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo_NoIcon, this, 
                            (string)this.TryFindResource("ResStr_Scanning_image_has_been_changed__please_confirm_whether_save_it_or_not_"),
                            (string)this.TryFindResource("ResStr_Prompt"));

                if ( VOP.Controls.MessageBoxExResult.Yes == ret )
                {
                    m_rotatedObj = new ScanFiles();
                    m_rotatedObj.m_colorMode = m_images.m_colorMode;

                    m_rotatedObj.m_pathOrig  = m_images.m_pathOrig.Insert( m_images.m_pathOrig.Length-4   , m_rotatedAngle.ToString() );
                    m_rotatedObj.m_pathView  = m_images.m_pathView.Insert( m_images.m_pathView.Length-4   , m_rotatedAngle.ToString() );
                    m_rotatedObj.m_pathThumb = m_images.m_pathThumb.Insert( m_images.m_pathThumb.Length-4 , m_rotatedAngle.ToString() );

                    AsyncWorker worker = new AsyncWorker( this );
                    if ( false == worker.InvokeRotateScannedFiles( RotateScannedFiles, m_images, m_rotatedObj, m_rotatedAngle ) )
                    {
                        VOP.Controls.MessageBoxEx.Show(
                                VOP.Controls.MessageBoxExStyle.Simple,
                                this,
                                (string)this.FindResource( "ResStr_Operation_cannot_be_carried_out_due_to_insufficient_memory_or_hard_disk_space_Please_try_again_after_freeing_memory_or_hard_disk_space_" ),
                                (string)this.FindResource( "ResStr_Error" )
                                );

                        m_rotatedAngle = 0; // Fixed #0059434.
                    }
                }
                else if ( VOP.Controls.MessageBoxExResult.No == ret )
                {
                    m_rotatedAngle = 0;
                }
                else
                {
                    e.Cancel = true;
                    isPrint = false;
                    FileSelectionPage.IsInitPrintSettingPage = oldValueForPrintSettingPage;
                }
            }
        }

        // Make the preview image fit the scroll view.
        private void FitTheWindow()
        {
            double scaling1 = 0.0;
            double scaling2 = 0.0;
            double scaling0 = 0.0;

            switch(m_rotatedAngle)
            {
                case 0:
                case 180:
                    scaling1 = ( this.scrollPreview.ActualWidth  -10 ) / previewImg.Source.Width;
                    scaling2 = ( this.scrollPreview.ActualHeight -10 ) / previewImg.Source.Height;
                    scaling0 = (scaling1 < scaling2) ? scaling1 : scaling2;                  
                    break;
                case 90:
                case 270:
                    scaling1 = ( this.scrollPreview.ActualWidth  -10 ) / previewImg.Source.Height;
                    scaling2 = ( this.scrollPreview.ActualHeight -10 ) / previewImg.Source.Width;
                    scaling0 = (scaling1 < scaling2) ? scaling1 : scaling2;                
                    break;            
            }

            previewImg.Width  = previewImg.Source.Width * scaling0;
            previewImg.Height = previewImg.Source.Height * scaling0;
        }
    }
}
