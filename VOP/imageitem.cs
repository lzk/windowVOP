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

        private int  _num = 0; // Number mark in the right top corner. Assign zero when the item was unselected.
        public int  m_num
        {
            get
            {
                return _num;
            }
            set
            {
                _num = value;
                CheckImage( 0 < value );
            }
        }

        private void CheckImage( bool ischeck ) 
        {
            if ( ischeck )
            {
                // simple visual definition
                var grid = new Grid { Width = 22, Height = 22 };

                System.Windows.Shapes.Ellipse ell1 = new System.Windows.Shapes.Ellipse();
                ell1.Width = 22;
                ell1.Height = 22;
                ell1.Fill = Brushes.White;

                System.Windows.Shapes.Ellipse ell2 = new System.Windows.Shapes.Ellipse();
                ell2.Width = 20;
                ell2.Height = 20;

                SolidColorBrush br1 = new SolidColorBrush();
                br1.Color = Color.FromArgb(255, 0x49, 0xA9, 0);

                ell2.Fill = br1;

                TextBlock tb = new TextBlock();
                tb.Text = m_num.ToString();
                tb.Foreground = Brushes.White;
                tb.FontWeight = FontWeights.Bold;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                grid.Children.Add( ell1 );
                grid.Children.Add( ell2 );
                grid.Children.Add( tb );

                grid.Measure(new Size(grid.Width, grid.Height));
                grid.Arrange(new Rect(new Size(grid.Width, grid.Height)));

                // create a BitmapSource from the visual
                var rtb = new RenderTargetBitmap(
                        (int)grid.Width,
                        (int)grid.Height,
                        96,
                        96,
                        PixelFormats.Pbgra32);
                rtb.Render(grid);

                imgMark.Source = rtb;
            }
            else
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("Images/close.png", UriKind.Relative);
                bi3.EndInit();
                imgMark.Source = bi3;
            }
        }

        private void MyMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if ( e.LeftButton == MouseButtonState.Pressed )
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
        }

        public void MouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            if ( MouseButtonState.Pressed == e.LeftButton 
                    && 0 == m_num 
                    && CloseIconClick != null )
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
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                BitmapSource bi3 = decoder.Frames[0];

                // Begin: Fix 61368
                if (m_images.m_colorMode == EnumColorType.black_white)
                {
                    bi3 = BitmapFrame.Create(new TransformedBitmap(bi3, new ScaleTransform(0.1, 0.1)));

                    // https://msdn.microsoft.com/en-us/library/system.windows.media.imaging.formatconvertedbitmap(v=vs.100).aspx
                    FormatConvertedBitmap bmpSrc = new FormatConvertedBitmap();
                    bmpSrc.BeginInit();
                    bmpSrc.Source = bi3;
                    bmpSrc.DestinationFormat = PixelFormats.Gray2;
                    bmpSrc.EndInit();

                    imgBody.Source = bmpSrc;
                }
                else
                {
                    imgBody.Source = bi3;
                }
                // End: Fix 61368

                this.Background = Brushes.White;
                this.Width = 105;
                this.Height = 140;
            }
            catch
            {
            }

            m_iSimgReady = ( null != imgBody.Source );
        }
    }
}
