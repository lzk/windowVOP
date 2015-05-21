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
        private double m_scaleFitToWindow = 1;    // scaling rate when image fit to preview window.
        private BitmapSource m_src        = null; // image object for preview image.
        private double m_scale            = 1;    // scaling rate of preview image.

        public ScanFiles m_images     = null;
        public bool isPrint           = false;    // turn if user click print.
        public int m_rotatedAngle     = 0;        // rotated angle of preview image.
        public ScanFiles m_rotatedObj = null;     

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
                Uri myUri = new Uri(m_images.m_pathView, UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad  );
                BitmapSource bmpSrc = decoder.Frames[0];

                if (m_images.m_colorMode == EnumColorType.black_white)
                {
                    bmpSrc = BitmapFrame.Create(new TransformedBitmap(bmpSrc, new ScaleTransform(0.4, 0.4)));
                }

                m_src = bmpSrc;

            }
            catch
            {
                m_src = null;
            }

            if (m_src != null)
            {
                double scaling1 = this.scrollPreview.Width / m_src.Width;
                double scaling2 = this.scrollPreview.Height / m_src.Height;

                m_scale = (scaling1 < scaling2) ? scaling1 : scaling2;

                if ( m_scale > 1 )
                    m_scale = 1;

                m_scaleFitToWindow = m_scale;

                this.previewImg.Width  = m_src.Width *m_scale;
                this.previewImg.Height = m_src.Height*m_scale;
                this.previewImg.Source = m_src;

                CenterImage();
            }
            else
            {
                VOP.Controls.MessageBoxEx.Show(
                        VOP.Controls.MessageBoxExStyle.Simple,
                        this,
                        (string)this.FindResource( "ResStr_Operation_cannot_be_carried_out_due_to_insufficient_memory_or_hard_disk_space_Please_try_again_after_freeing_memory_or_hard_disk_space_" ),
                        (string)this.FindResource( "ResStr_Error" )
                        );

                this.Close();
            }
        }

        private void CenterImage()
        {
            if (m_src != null)
            {
                if (previewImg.Height > scrollPreview.ViewportHeight)
                    scrollPreview.ScrollToVerticalOffset((previewImg.Height - scrollPreview.ViewportHeight) / 2);

                if (previewImg.Width > scrollPreview.ViewportWidth)
                    scrollPreview.ScrollToHorizontalOffset((previewImg.Width - scrollPreview.ViewportWidth) / 2);
            }
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if (position.Y > 0)
                this.DragMove();
        }

        private void imagebtn_click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            string name = btn.Name;

            if (name == "btn_zoomin")
            {
                m_scale += 0.1;
            }
            else if (name == "btn_zoomout")
            {
                m_scale -= 0.1;
            }

            else if (name == "btn_turn")
            {
                m_scale = m_scaleFitToWindow;
                m_rotatedAngle += 90;
                m_rotatedAngle = m_rotatedAngle % 360;

                if ( 0 != m_rotatedAngle )
                {
                    try
                    {
                        CachedBitmap cache = new CachedBitmap(m_src, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        previewImg.Source = BitmapFrame.Create(new TransformedBitmap(cache, new RotateTransform(m_rotatedAngle)));
                        GC.Collect();
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
                else
                {
                    previewImg.Source = m_src;
                }
            }
            else if (name == "btn_normal")
            {
                m_scale = m_scaleFitToWindow;
            }

            if (m_scale <= 0.1)
            {
                if (m_scaleFitToWindow < 0.1)
                    m_scale = m_scaleFitToWindow;
                else
                    m_scale = 0.1;
            }
            else if (m_scale >= 4)
                m_scale = 4;

            if (-90 == m_rotatedAngle || -270 == m_rotatedAngle)
            {
                previewImg.Height = m_src.Width * m_scale;
                previewImg.Width = m_src.Height * m_scale;
            }
            else
            {
                previewImg.Width = m_src.Width * m_scale;
                previewImg.Height = m_src.Height * m_scale;
            }

            CenterImage();
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
            FileSelectionPage.IsInitPrintSettingPage = true;
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
            string args2 = "\"" + objSrc.m_pathView  + "\" \"" + objDst.m_pathView  + "\" \"" + nAngle.ToString();
            string args3 = "\"" + objSrc.m_pathThumb + "\" \"" + objDst.m_pathThumb + "\" \"" + nAngle.ToString();

            try
            {
                // Get the ExitCode of VopHelper. 0 success, otherwise fail.
                int nExitCode1 = 0;
                int nExitCode2 = 0;
                int nExitCode3 = 0;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = App.vopHelperExe;
                Process exeProcess = null;

                startInfo.Arguments = args1;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
                nExitCode1 = exeProcess.ExitCode;

                startInfo.Arguments = args2;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
                nExitCode2 = exeProcess.ExitCode;

                startInfo.Arguments = args3;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
                nExitCode3 = exeProcess.ExitCode;

                bSuccess = ( 0 == nExitCode1 
                        && 0 == nExitCode2 
                        && 0 == nExitCode3);
            }
            catch
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( 0 != m_rotatedAngle % 360 )
            {
                if ( VOP.Controls.MessageBoxExResult.Yes == 
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo, this, 
                            (string)this.TryFindResource("ResStr_Scanning_image_has_been_changed__please_confirm_whether_save_it_or_not_"), 
                            (string)this.TryFindResource("ResStr_Warning_2") ))
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
                    }
                }
                else
                {
                    m_rotatedAngle = 0;
                }
            }
        }
    }
}
