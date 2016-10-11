using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace VOP.Controls
{
	/// <summary>
	/// Interaction logic for ImageCropper.xaml
	/// </summary>
	public partial class ImageCropper : UserControl
	{
		public static double thumbCornerWidth = 0;
        public static double imageMargin = 7;
        public static double designerItemWHRatio = 1;
        bool IsFitted = false;
        public static double imageToTop = 0;
        public static double imageToLeft = 0;
        public static double imageWidth = 0;
        public static double imageHeight = 0;
        double scaleRatioApply = 1.0;

        private int currentImageRotation = 0;

        int m_PixelWidth = 0;
        int m_PixelHeight = 0;
        double m_DpiX = 0;
        double m_DpiY = 0;

        public static Int32Rect intRectOffset;

        BitmapImage imageSource = null;

		public ImageCropper()
        {
			this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.ImageCropper_Loaded);
		}

        public Uri ImagePath
        {
            get { return (Uri)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(Uri), typeof(ImageCropper),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ImagePathProperty_Changed)));

        public static readonly DependencyProperty DesignerItemColorProperty =
         DependencyProperty.Register("DesignerItemColor", typeof(SolidColorBrush), typeof(ImageCropper));

        public Brush DesignerItemColor
        {
            get { return (SolidColorBrush)GetValue(DesignerItemColorProperty); }
            set { SetValue(DesignerItemColorProperty, value); }
        }

        private double PixelToUnit(int p, double dpi)
        {
            return  (double)p / dpi * 96;
        }

        private int UnitToPixel(double u, double dpi)
        {
            return (int)(u / 96 * dpi);
        }

        private void SwapWidthHeight()
        {
            int temp = m_PixelWidth;
            m_PixelWidth = m_PixelHeight;
            m_PixelHeight = temp;

            double dpiTemp = m_DpiX;
            m_DpiX = m_DpiY;
            m_DpiY = dpiTemp;
        }

        public void SetDesignerItemColor(Brush brush)
        {
            DesignerItemColor = brush;
        }

        private void ImageCropper_Loaded(Object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.Green;
            SetDesignerItemColor(brush);

            BitmapSource src = imageSource as BitmapSource;

            if (src != null)
            {
                m_PixelWidth = src.PixelWidth;
                m_PixelHeight = src.PixelHeight;
                m_DpiX = src.DpiX;
                m_DpiY = src.DpiY;

                ReArrangeDesignerItem(src.PixelWidth, src.PixelHeight, src.DpiX, src.DpiY);

                //scale down image
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = imageWidth / src.PixelWidth;
                scaleTransform.ScaleY = imageHeight / src.PixelHeight;

                TransformedBitmap tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = src;
                tb.Transform = scaleTransform;
                tb.EndInit();

                this.backgroundImage.Source = tb;  
            }
        }

        private void ReArrangeDesignerItem(int width, int height, double dpiX, double dpiY)
        {
            double whRatio = 1.0;
            double scaleRatioX = 1.0;
            double scaleRatioY = 1.0;
            double designerItemToTop = 0;
            double designerItemToLeft = 0;

            imageToTop = imageMargin;
            imageToLeft = imageMargin;
            imageWidth = 0;
            imageHeight = 0;

            //imageWidth = PixelToUnit(width, dpiX);
            //imageHeight = PixelToUnit(height, dpiY);

            imageWidth = width;
            imageHeight = height;

            if (imageWidth < (this.ActualWidth - 2 * imageMargin) && imageHeight < (this.ActualHeight - 2 * imageMargin))
            {
                this.backgroundImage.Stretch = Stretch.None;
                IsFitted = true;
            }
            else
            {
                this.backgroundImage.Stretch = Stretch.Uniform;
                IsFitted = false;
            }

            whRatio = imageWidth / imageHeight;
            scaleRatioX = imageWidth / (this.ActualWidth - 2 * imageMargin);
            scaleRatioY = imageHeight / (this.ActualHeight - 2 * imageMargin);

            if (IsFitted == true)
            {
                imageToTop = (this.ActualHeight - imageHeight) / 2;
                imageToLeft = (this.ActualWidth - imageWidth) / 2;
            }
            else
            {
                //Image uniformlly strech to control, calculate the gap on unfitted side
                if (scaleRatioX > scaleRatioY)
                {
                    imageWidth = this.ActualWidth - 2 * imageMargin;
                    imageHeight = imageWidth / whRatio;
                    imageToTop = (this.ActualHeight - imageHeight) / 2;
                    scaleRatioApply = scaleRatioX;       
                }
                else if (scaleRatioX < scaleRatioY)
                {
                    imageHeight = this.ActualHeight - 2 * imageMargin;
                    imageWidth = imageHeight * whRatio;
                    imageToLeft = (this.ActualWidth - imageWidth) / 2;
                    scaleRatioApply = scaleRatioY;
                }
                else
                {
                    imageWidth = this.ActualWidth - 2 * imageMargin;
                    imageHeight = this.ActualHeight - 2 * imageMargin;
                    scaleRatioApply = scaleRatioX;
                }
            }

            //Init designerItem location
            ContentControl designerItem = imageCropperContent;
            Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;

            designerItem.MinWidth = 16;
            designerItem.MinHeight = 16 / designerItemWHRatio;

            if(!QRCodeResultPage.redRect.IsEmpty)
            {
                Rect rect = QRCodeResultPage.redRect;

                if(!intRectOffset.IsEmpty)
                {
                    rect.X += (double)intRectOffset.X;
                    rect.Y += (double)intRectOffset.Y;
                }

                rect.Scale(1 / scaleRatioApply, 1 / scaleRatioApply);

                designerItem.Width = rect.Width + QRCodeResultPage.rectMargin * 2;
                designerItem.Height = rect.Height + QRCodeResultPage.rectMargin * 2;
                Canvas.SetTop(designerItem, imageToTop + rect.Y - QRCodeResultPage.rectMargin);
                Canvas.SetLeft(designerItem, imageToLeft + rect.X - QRCodeResultPage.rectMargin);

                QRCodeResultPage.redRect = new Rect();
            }
            else
            {
                if (whRatio > designerItemWHRatio)
                {
                    designerItem.Height = imageHeight - thumbCornerWidth * 2;
                    designerItem.Width = designerItem.Height * designerItemWHRatio;
                    designerItemToLeft = (imageWidth - designerItem.Width) / 2;
                    Canvas.SetLeft(designerItem, imageToLeft + designerItemToLeft);
                    Canvas.SetTop(designerItem, imageToTop + thumbCornerWidth);

                }
                else if (whRatio < designerItemWHRatio)
                {
                    designerItem.Width = imageWidth - thumbCornerWidth * 2;
                    designerItem.Height = designerItem.Width / designerItemWHRatio;
                    designerItemToTop = (imageHeight - designerItem.Height) / 2;
                    Canvas.SetTop(designerItem, imageToTop + designerItemToTop);
                    Canvas.SetLeft(designerItem, imageToLeft + thumbCornerWidth);
                }
                else
                {
                    designerItem.Width = imageWidth - thumbCornerWidth * 2;
                    designerItem.Height = imageHeight - thumbCornerWidth * 2;
                    Canvas.SetTop(designerItem, imageToTop + thumbCornerWidth);
                    Canvas.SetLeft(designerItem, imageToLeft + thumbCornerWidth);
                }

            }

        }

        private static void ImagePathProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageCropper cropper = d as ImageCropper;
           
            Uri newUri = (Uri)e.NewValue;

            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = newUri;
            myBitmapImage.CacheOption = BitmapCacheOption.None;
            myBitmapImage.EndInit();

            cropper.imageSource = myBitmapImage;
        }

        public void Rotated90()
        {
            currentImageRotation = (currentImageRotation + 90) % 360;
            backgroundImage.LayoutTransform = new RotateTransform(currentImageRotation);

            SwapWidthHeight();
            ReArrangeDesignerItem(m_PixelWidth, m_PixelHeight, m_DpiX, m_DpiY);
        }

        private Rect ResizeDesignerItemToImageSource(Rect rectIn)
        {
            Rect rectOut = new Rect();

            switch (currentImageRotation)
            {
                case 0:
                    rectOut = rectIn;
                    break;
                case 90:
                    rectOut.X = rectIn.Y;
                    rectOut.Y = imageWidth - rectIn.X - rectIn.Width;
                    rectOut.Width = rectIn.Height;
                    rectOut.Height = rectIn.Width;
                    break;
                case 180:
                    rectOut.X = imageWidth - rectIn.X - rectIn.Width;
                    rectOut.Y = imageHeight - rectIn.Y - rectIn.Height;
                    rectOut.Width = rectIn.Width;
                    rectOut.Height = rectIn.Height;
                    break;
                case 270:
                    rectOut.X = imageHeight - rectIn.Y - rectIn.Height;
                    rectOut.Y = rectIn.X;
                    rectOut.Width = rectIn.Height;
                    rectOut.Height = rectIn.Width;
                    break;
            }

            return rectOut;
        }

        public int GetCurrentImageRotation()
        {
            return currentImageRotation;
        }

        public BitmapSource GetCroppedImage()
        {
            CroppedBitmap croppedImage = null;

           // BitmapSource src = this.backgroundImage.Source as BitmapSource;
            BitmapSource src = this.imageSource as BitmapSource;

            if (src == null)
                return null;

            double w = src.PixelWidth;
            double h = src.PixelHeight;

            ContentControl designerItem = imageCropperContent;
            Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;


            Rect rectOut = ResizeDesignerItemToImageSource(new Rect(Canvas.GetLeft(designerItem) - imageToLeft,
                                                                    Canvas.GetTop(designerItem) - imageToTop,
                                                                    designerItem.ActualWidth,
                                                                    designerItem.ActualHeight));

            //double toLeft = UnitToPixel(rectOut.X, src.DpiX);
            //double toTop = UnitToPixel(rectOut.Y, src.DpiY);
            //double designerItemWidth = UnitToPixel(rectOut.Width, src.DpiX);
            //double designerItemHeight = UnitToPixel(rectOut.Height, src.DpiY);

            double toLeft = rectOut.X;
            double toTop = rectOut.Y;
            double designerItemWidth = rectOut.Width;
            double designerItemHeight = rectOut.Height;

            Rect rect = new Rect(toLeft, toTop, designerItemWidth, designerItemHeight);

            if(IsFitted == false)
            {
                rect.Scale(scaleRatioApply, scaleRatioApply);
            }
         
            Int32Rect intRect = new Int32Rect((int)(rect.X), (int)(rect.Y), (int)rect.Width, (int)rect.Height);

            if (   intRect.X >= 0
                && intRect.Y >= 0
                && intRect.X + intRect.Width <= w
                && intRect.Y + intRect.Height <= h)
            {
                intRectOffset = intRect;
                croppedImage = new CroppedBitmap(src, intRect);
            }
       
            return croppedImage;
        }
	}
}