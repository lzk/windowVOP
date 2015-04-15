﻿using System;
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
            ClosePreviewWin();
        }

        /// <summary>
        /// Close preview window. Popup MessageBox to user if the image has rotated.
        /// </summary>
        private void ClosePreviewWin()
        {
            if ( 0 != m_images.m_rotate % 360 )
            {
                if ( VOP.Controls.MessageBoxExResult.Yes == 
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo, this, "扫描图片已被更改，是否保存，请确认。", "提示") )
                {
                    // TODO: rotate image
                }
            }

            this.Close();
        }

    }
}
