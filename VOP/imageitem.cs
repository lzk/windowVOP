using System.Windows.Controls;
using System.Windows.Media; // For ImageSource
using System.Windows;       // For DependencyProperty
using System.Windows.Media.Imaging;
using System;
using System.IO;     
using System.Windows.Input;

namespace VOP
{
    public partial class ImageItem : UserControl
    {
        public bool m_ischeck = false;
        public bool m_iSimgReady = false; // false if image has not loaded.

        public ImageItem()
        {
            InitializeComponent();
        }

        private ScanFiles _files;
        public ScanFiles m_images
        {
            get
            {
                return _files; 
            }
            set
            {
                _files = value;
                UpdateImage();
            }
        }

        public void CheckImage( bool ischeck ) 
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();

            if ( ischeck )
                bi3.UriSource = new Uri("Images/check.png", UriKind.Relative);
            else
                bi3.UriSource = new Uri("Images/close.png", UriKind.Relative);
            bi3.EndInit();

            imgMark.Source = bi3;
            m_ischeck = ischeck;
        }

        private void MyMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1  )
            {
                if ( ImageDoubleClick != null )
                    this.ImageDoubleClick((object) this, (RoutedEventArgs) null);
            }
            else
            {
                if ( ImageSingleClick != null )
                    this.ImageSingleClick((object) this, (RoutedEventArgs) null);
            }
        }

        public void MouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            if ( false == m_ischeck && CloseIconClick != null )
            {
                this.CloseIconClick((object) this, (RoutedEventArgs) null);
                e.Handled = true;
            }
        }


        public event RoutedEventHandler ImageSingleClick;
        public event RoutedEventHandler ImageDoubleClick;
        public event RoutedEventHandler CloseIconClick;

        public void UpdateImage()
        {
            try
            {
                Uri myUri = new Uri(m_images.m_pathThumb, UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad );
                BitmapSource bmpSrc = decoder.Frames[0];

                if (m_images.m_colorMode == EnumColorType.black_white)
                {
                    bmpSrc = BitmapFrame.Create(new TransformedBitmap(bmpSrc, new ScaleTransform(0.1, 0.1)));
                }

                this.Background = Brushes.White;
                this.Width = 64;
                this.Height = 64;
                imgBody.Source = bmpSrc;
            }
            catch
            {
            }

            m_iSimgReady = ( null != imgBody.Source );
        }
    }
}
