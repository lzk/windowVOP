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

                previewImg.Source = common.RotateBitmap(m_src, m_rotatedAngle);
                GC.Collect();
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
            ClosePreviewWin();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePreviewWin();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            isPrint = true;    
            ClosePreviewWin();
        }

        /// <summary>
        /// Rotate objSrc to objDst with angle nAngle.
        /// </summary>
        private void RotateScannedFiles( ScanFiles objSrc, ScanFiles objDst, int nAngle )
        {
            string args1 = objSrc.m_pathOrig  + " " + objDst.m_pathOrig  + " " + nAngle.ToString();
            string args2 = objSrc.m_pathView  + " " + objDst.m_pathView  + " " + nAngle.ToString();
            string args3 = objSrc.m_pathThumb + " " + objDst.m_pathThumb + " " + nAngle.ToString();

            try
            {
                // TODO: Get the ExitCode of VopHelper. 0 success,
                // otherwise fail.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "VopHelper.exe";
                Process exeProcess = null;

                startInfo.Arguments = args1;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();

                startInfo.Arguments = args2;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();

                startInfo.Arguments = args3;
                exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Close preview window. Popup MessageBox to user if the image has rotated.
        /// </summary>
        private void ClosePreviewWin()
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

                    RotateScannedFiles( m_images, m_rotatedObj, m_rotatedAngle );

                    AsyncWorker worker = new AsyncWorker( this );
                    worker.InvokeRotateScannedFiles( RotateScannedFiles, m_images, m_rotatedObj, m_rotatedAngle );
                }
                else
                {
                    m_rotatedAngle = 0;
                }
            }

            this.Close();
        }
    }
}
