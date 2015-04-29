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
        public static double designerItemWHRatio = 1;
        bool IsFitted = false;
        public static double imageToTop = 0;
        public static double imageToLeft = 0;
        public static double imageWidth = 0;
        public static double imageHeight = 0;
        double scaleRatioApply = 1.0;
        int rotatedDegree = 0;

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

        private double PixelToUnit(int p, double dpi)
        {
            return  (double)p / dpi * 96;
        }

        private int UnitToPixel(double u, double dpi)
        {
            return (int)(u / 96 * dpi);
        }

        private void ImageCropper_Loaded(Object sender, RoutedEventArgs e)
        {
            double whRatio = 1.0;
            double scaleRatioX = 1.0;
            double scaleRatioY = 1.0;
            double designerItemToTop = 0;
            double designerItemToLeft = 0;

            imageToTop = 0;
            imageToLeft = 0;
            imageWidth = 0;
            imageHeight = 0;

            BitmapSource src = imageSource as BitmapSource;

            if(src != null)
            {
                imageWidth = PixelToUnit(src.PixelWidth, src.DpiX);
                imageHeight = PixelToUnit(src.PixelHeight, src.DpiY);

                if (imageWidth < this.ActualWidth && imageHeight < this.ActualHeight)
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
                scaleRatioX = imageWidth / this.ActualWidth;
                scaleRatioY = imageHeight / this.ActualHeight;

                if (IsFitted == true)
                {
                    imageWidth = PixelToUnit(src.PixelWidth, src.DpiX);
                    imageHeight = PixelToUnit(src.PixelHeight, src.DpiY);
                    imageToTop = (this.ActualHeight - imageHeight) / 2;
                    imageToLeft = (this.ActualWidth - imageWidth) / 2;
                }
                else
                {
                    //Image uniformlly strech to control, calculate the gap on unfitted side
                    if (scaleRatioX > scaleRatioY)
                    {
                        imageWidth = this.ActualWidth;
                        imageHeight = this.ActualWidth / whRatio;
                        imageToTop = (this.ActualHeight - imageHeight) / 2;
                        scaleRatioApply = scaleRatioX;       
                    }
                    else if (scaleRatioX < scaleRatioY)
                    {
                        imageWidth = this.ActualHeight * whRatio;
                        imageHeight = this.ActualHeight;
                        imageToLeft = (this.ActualWidth - imageWidth) / 2;
                        scaleRatioApply = scaleRatioY;
                    }
                    else
                    {
                        imageWidth = this.ActualWidth;
                        imageHeight = this.ActualHeight;
                        scaleRatioApply = scaleRatioX;
                    }
                }

                //Init designerItem location
                ContentControl designerItem = imageCropperContent;
                Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;

                designerItem.MinWidth = 16;
                designerItem.MinHeight = 16 / designerItemWHRatio;

                if(whRatio > designerItemWHRatio)
                {
                    designerItem.Height = imageHeight - thumbCornerWidth * 2;
                    designerItem.Width = designerItem.Height * designerItemWHRatio;
                    designerItemToLeft = (imageWidth - designerItem.Width) / 2;
                    Canvas.SetLeft(designerItem, imageToLeft + designerItemToLeft);
                    Canvas.SetTop(designerItem, imageToTop + thumbCornerWidth);
                  
                }
                else if(whRatio < designerItemWHRatio)
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

                //scale down image
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = imageWidth / PixelToUnit(src.PixelWidth, src.DpiX);
                scaleTransform.ScaleY = imageHeight / PixelToUnit(src.PixelHeight, src.DpiY);

                TransformedBitmap tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = src;
                tb.Transform = scaleTransform;
                tb.EndInit();

                this.backgroundImage.Source = tb;
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
            rotatedDegree += 90;
            backgroundImage.RenderTransformOrigin = new Point(0.5, 0.5);
            backgroundImage.RenderTransform = new RotateTransform(rotatedDegree % 360);

            BitmapSource src = backgroundImage.Source as BitmapSource;

            imageWidth = PixelToUnit(src.PixelWidth, src.DpiX);
            imageHeight = PixelToUnit(src.PixelHeight, src.DpiY);

            if (imageWidth < this.ActualWidth && imageHeight < this.ActualHeight)
            {
                this.backgroundImage.Stretch = Stretch.None;
                IsFitted = true;
            }
            else
            {
                this.backgroundImage.Stretch = Stretch.Uniform;
                IsFitted = false;
            }
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

            double toLeft = UnitToPixel(Canvas.GetLeft(designerItem) - imageToLeft, src.DpiX);
            double toTop = UnitToPixel(Canvas.GetTop(designerItem) - imageToTop, src.DpiY);
            double designerItemWidth = UnitToPixel(designerItem.ActualWidth, src.DpiX);
            double designerItemHeight = UnitToPixel(designerItem.ActualHeight, src.DpiY);

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
                croppedImage = new CroppedBitmap(src, intRect);
            }
       
            return croppedImage;
        }
	}
}