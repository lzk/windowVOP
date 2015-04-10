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


namespace VOP
{
    /// <summary>
    /// Interaction logic for ScanPreview.xaml
    /// </summary>
    public partial class ScanPreview : Window
    {
        public BitmapSource m_src = null;
        public double m_scale = 1;
        public double m_scaleFitToWindow = 1;

        public ScanFiles m_images;

        public ScanPreview()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri myUri = new Uri(m_images.m_pathView, UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
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

                if (m_src.Width > 800)
                    this.previewImg.Width = 800;
                else
                    this.previewImg.Width = 800;

                if (m_src.Height > 500)
                    this.previewImg.Height = 500;
                else
                    this.previewImg.Height = 500;

                double scaling1 = this.previewImg.Width / m_src.Width;
                double scaling2 = this.previewImg.Height / m_src.Height;

                m_scale = (scaling1 < scaling2) ? scaling1 : scaling2;
                m_scaleFitToWindow = m_scale;
                this.previewImg.Source = common.RotateBitmap(m_src, m_images.m_rotate);

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

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void print_btn_click(object sender, RoutedEventArgs e)
        {

          
        }


        private void save_btn_click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "TIF|*.tif|PDF|*.pdf|JPG|*.jpg";
            bool? result = save.ShowDialog();

            if (result == true)
            {

                // This index is 1-based, not 0-based
                if (3 == save.FilterIndex)
                {
                    Uri myUri = new Uri(m_images.m_pathOrig, UriKind.RelativeOrAbsolute);
                    BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                    BitmapSource origSource = decoder.Frames[0];

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    switch (m_images.m_rotate)
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

                    if (null != origSource)
                        encoder.Frames.Add(BitmapFrame.Create(origSource));

                    FileStream fs = File.Open(save.FileName, FileMode.Create);
                    encoder.Save(fs);
                    fs.Close();
                }
                else if (1 == save.FilterIndex)
                {
                    BitmapSource origSource = common.GetOrigBitmapSource(m_images);
                    TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(origSource));

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

                            Uri myUri = new Uri(m_images.m_pathOrig, UriKind.RelativeOrAbsolute);
                            BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                            BitmapSource origSource = decoder.Frames[0];

                            if (null != origSource)
                                help.AddImage(origSource, m_images.m_rotate);

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
                m_images.m_rotate += 90;
                m_images.m_rotate = m_images.m_rotate % 360;

                previewImg.Source = common.RotateBitmap(m_src, m_images.m_rotate);
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

            if (-90 == m_images.m_rotate || -270 == m_images.m_rotate)
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

    }
}
