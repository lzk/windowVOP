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
        private ScanFiles _files;
        public BitmapSource m_source = null; // if the file loading fail, m_source remain null

        public static readonly DependencyProperty srcProperty = DependencyProperty.Register("src", typeof (ImageSource), typeof (ImageItem), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
        public static readonly DependencyProperty markerProperty = DependencyProperty.Register("marker", typeof (ImageSource), typeof (ImageItem), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

        public ImageItem()
        {
            InitializeComponent();
        }

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private ImageSource src
        {
          get
          {
            return (ImageSource) this.GetValue(ImageItem.srcProperty);
          }
          set
          {
            this.SetValue(ImageItem.srcProperty, (object) value);
          }
        }

        public ImageSource marker
        {
          get
          {
            return (ImageSource) this.GetValue(ImageItem.markerProperty);
          }
          set
          {
            this.SetValue(ImageItem.markerProperty, (object) value);
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

            this.marker = bi3;
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
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                BitmapSource bmpSrc = decoder.Frames[0];

                if (m_images.m_colorMode == EnumColorType.black_white)
                {
                    bmpSrc = BitmapFrame.Create(new TransformedBitmap(bmpSrc, new ScaleTransform(0.1, 0.1)));
                }

                m_source = bmpSrc;

                this.Background = Brushes.White;
                this.Width = 64;
                this.Height = 64;
                src = bmpSrc;
            }
            catch
            {
                m_source = null;
            }
        }
    }
}
