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
                var grid = new Grid { Width = 13, Height = 13 };
                System.Windows.Shapes.Ellipse ell = new System.Windows.Shapes.Ellipse();
                ell.Width = 13;
                ell.Height = 13;

                RadialGradientBrush radialGradient = new RadialGradientBrush();
                radialGradient.GradientOrigin = new Point(0.5, 0.5);
                radialGradient.Center = new Point(0.5, 0.5);

                radialGradient.RadiusX = 0.5; 
                radialGradient.RadiusY = 0.5;

                // Create four gradient stops.
                radialGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
                radialGradient.GradientStops.Add(new GradientStop( Color.FromArgb( 0xFF, 0x54, 0x9C, 0xF1 ), 0.5));
                radialGradient.GradientStops.Add(new GradientStop(Colors.White, 1.0));

                // Freeze the brush (make it unmodifiable) for performance benefits.
                radialGradient.Freeze();

                ell.Fill = radialGradient;

                TextBlock tb = new TextBlock();
                tb.Text = m_num.ToString();
                tb.Foreground = Brushes.White;
                tb.FontWeight = FontWeights.Bold;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                grid.Children.Add( ell );
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
