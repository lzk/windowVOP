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
		public static double thumbCornerWidth = 6;
        public static double designerItemWHRatio = 1;
        bool IsFitted = false;
        public static double imageToTop = 0;
        public static double imageToLeft = 0;
        public static double imageWidth = 0;
        public static double imageHeight = 0;
        double scaleRatioApply = 1.0;

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

            BitmapSource src = this.backgroundImage.Source as BitmapSource;

            if(src != null)
            {
                if (src.Width < this.ActualWidth && src.Height < this.ActualHeight)
                {
                    this.backgroundImage.Stretch = Stretch.None;
                    IsFitted = true;
                }
                else
                {
                    this.backgroundImage.Stretch = Stretch.Uniform;
                    IsFitted = false;
                }

                whRatio = (double)src.PixelWidth / src.PixelHeight;
                scaleRatioX = src.PixelWidth / this.ActualWidth;
                scaleRatioY = src.PixelHeight / this.ActualHeight;

                if (IsFitted == true)
                {
                    imageWidth = (double)src.Width;
                    imageHeight = (double)src.Height;
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

                designerItem.MinWidth = 50;
                designerItem.MinHeight = 50 / designerItemWHRatio;

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
            }
        }

        private static void ImagePathProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageCropper cropper = d as ImageCropper;
            Uri newUri = (Uri)e.NewValue;

            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = newUri;
            myBitmapImage.EndInit();

            cropper.backgroundImage.Source = myBitmapImage;
        }

        public BitmapSource GetCroppedImage()
        {
            CroppedBitmap croppedImage = null;

            BitmapSource src = this.backgroundImage.Source as BitmapSource;

            if (src == null)
                return null;        

            ContentControl designerItem = imageCropperContent;
            Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
 
            double toLeft = Canvas.GetLeft(designerItem); 
            double toTop = Canvas.GetTop(designerItem);

            Rect rect = new Rect(toLeft - imageToLeft, toTop - imageToTop, designerItem.ActualWidth, designerItem.ActualHeight);

            if(IsFitted == false)
            {
                rect.Scale(scaleRatioApply, scaleRatioApply);
            }
         
            Int32Rect intRect = new Int32Rect((int)(rect.X), (int)(rect.Y), (int)rect.Width, (int)rect.Height);

            if (   intRect.X >= 0 
                && intRect.Y >= 0 
                && intRect.X + intRect.Width <= src.PixelWidth 
                && intRect.Y + intRect.Height <= src.PixelHeight)
            {
                croppedImage = new CroppedBitmap(src, intRect);
            }
       
            return croppedImage;
        }
	}
}