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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class PrintPreview : UserControl
    {
        public IdCardTypeItem SelectedTypeItem { get; set; }
        public double PaperWidth { get; set; }
        public double PaperHeight { get; set; }

        public static BitmapSource PreviewImageSource = null;

        double PaperToTop = 10;

        public PrintPreview()
        {
            InitializeComponent();
            PaperWidth = IdCardEditWindow.A4Size.Width;
            PaperHeight = IdCardEditWindow.A4Size.Height;
        }

        private void ConvertCanvasToImageSource()
        {
            Size size = new Size(backgroundPaper.Width, backgroundPaper.Height);
            backgroundPaper.Measure(size);
            backgroundPaper.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(backgroundPaper);
            PreviewImageSource = renderBitmap;
        }

        public void Update()
        {
            double paperWHRatio = 0;
      
            paperWHRatio = PaperWidth / PaperHeight;
            Border border = VisualTreeHelper.GetParent(this) as Border;

            if(border != null)
            {   
                backgroundPaper.Height = border.ActualHeight - 2 * PaperToTop;
                backgroundPaper.Width = backgroundPaper.Height * paperWHRatio;  

                if(SelectedTypeItem != null)
                {
                    double imageWidth = 0;
                    double imageHeight = 0;
                    double imageToLeft = 0;
                    double imageToTop = 0;

                    switch(SelectedTypeItem.TypeId)
                    {
                        case enumIdCardType.IdCard:
                            if (IdCardEditWindow.croppedImageList.Count == 2)
                            {
                                imageWidth = 0;
                                imageHeight = 0;
                                imageToLeft = 0;
                                imageToTop = 0;

                                TransformGroup transformGroup = null;
                                ScaleTransform scaleTransform = null;

                                switch (IdCardEditWindow.imageRotationList[0])
                                {
                                    case 270:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        RotateTransform rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = -90;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 0:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 0;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 90:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 90;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 180:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 180;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                }

                                TransformedBitmap tb = new TransformedBitmap();
                                tb.BeginInit();
                                tb.Source = IdCardEditWindow.croppedImageList[0];
                                tb.Transform = scaleTransform;
                                tb.EndInit();

                                Image imageSideOne = new Image();
                                imageSideOne.Width = imageWidth;
                                imageSideOne.Height = imageHeight;
                                imageSideOne.RenderTransformOrigin = new Point(0.5, 0.5);
                                imageSideOne.RenderTransform = transformGroup;
                                imageSideOne.Source = tb;
                                backgroundPaper.Children.Add(imageSideOne);

                                Canvas.SetLeft(imageSideOne, imageToLeft);
                                Canvas.SetTop(imageSideOne, imageToTop);


                                switch (IdCardEditWindow.imageRotationList[1])
                                {
                                    case 270:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        RotateTransform rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = -90;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[1].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[1].Height;

                                        break;
                                    case 0:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 0;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[1].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[1].Height;

                                        break;
                                    case 90:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 90;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[1].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[1].Height;

                                        break;
                                    case 180:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height / 2 - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 180;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[1].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[1].Height;

                                        break;
                                }

                                TransformedBitmap tb2 = new TransformedBitmap();
                                tb2.BeginInit();
                                tb2.Source = IdCardEditWindow.croppedImageList[1];
                                tb2.Transform = scaleTransform;
                                tb2.EndInit();

                                Image imageSideTwo = new Image();
                                imageSideTwo.Width = imageWidth;
                                imageSideTwo.Height = imageHeight;
                                imageSideTwo.RenderTransformOrigin = new Point(0.5, 0.5);
                                imageSideTwo.RenderTransform = transformGroup;
                                imageSideTwo.Source = tb2;
                                backgroundPaper.Children.Add(imageSideTwo);

                                Canvas.SetLeft(imageSideTwo, imageToLeft);
                                Canvas.SetTop(imageSideTwo, imageToTop + backgroundPaper.Height / 2);
                            }
                            break;
                        case enumIdCardType.MarriageCertificate:
                            if (IdCardEditWindow.croppedImageList.Count == 1)
                            {
                                imageWidth = 0;
                                imageHeight = 0;
                                imageToLeft = 0;
                                imageToTop = 0;

                                TransformGroup transformGroup = null;
                                ScaleTransform scaleTransform = null;

                                switch (IdCardEditWindow.imageRotationList[0])
                                {
                                    case 270:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        RotateTransform rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = -90;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 0:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 0;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 90:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 90;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 180:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();
                                        rotateTransform.Angle = 180;

                                        transformGroup = new TransformGroup();
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                }

                                TransformedBitmap tb = new TransformedBitmap();
                                tb.BeginInit();
                                tb.Source = IdCardEditWindow.croppedImageList[0];
                                tb.Transform = scaleTransform;
                                tb.EndInit();

                                Image imageSideOne = new Image();

                                imageSideOne.Width = imageWidth;
                                imageSideOne.Height = imageHeight;
                                imageSideOne.RenderTransformOrigin = new Point(0.5, 0.5);
                                imageSideOne.RenderTransform = transformGroup;
                                imageSideOne.Source = tb;
                                backgroundPaper.Children.Add(imageSideOne);

                                Canvas.SetLeft(imageSideOne, imageToLeft);
                                Canvas.SetTop(imageSideOne, imageToTop);
                            }
                            break;
                        case enumIdCardType.HouseholdRegister:
                        case enumIdCardType.Passport:
                        case enumIdCardType.RealEstateEvaluator:
                        case enumIdCardType.DriverLicense:
                        case enumIdCardType.StudentIDcard:
                        case enumIdCardType.BirthCertificate:
                        case enumIdCardType.BankCards:
                        case enumIdCardType.Diploma:
                            if (IdCardEditWindow.croppedImageList.Count == 1)
                            {
                                imageWidth = 0;
                                imageHeight = 0;
                                imageToLeft = 0;
                                imageToTop = 0;

                                TransformGroup transformGroup = null;
                                ScaleTransform scaleTransform = null;

                                switch (IdCardEditWindow.imageRotationList[0])
                                {
                                    case 0:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        RotateTransform rotateTransform = new RotateTransform();  
                                        rotateTransform.Angle = -90;

                                        transformGroup = new TransformGroup();  
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 90:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();  
                                        rotateTransform.Angle = 0;

                                        transformGroup = new TransformGroup();  
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 180:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();  
                                        rotateTransform.Angle = 90;

                                        transformGroup = new TransformGroup();  
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                    case 270:

                                        imageWidth = backgroundPaper.Width * (SelectedTypeItem.Height / IdCardEditWindow.A4Size.Width);
                                        imageHeight = backgroundPaper.Height * (SelectedTypeItem.Width / IdCardEditWindow.A4Size.Height);

                                        imageToLeft = (backgroundPaper.Width - imageWidth) / 2;
                                        imageToTop = (backgroundPaper.Height - imageHeight) / 2;

                                        rotateTransform = new RotateTransform();  
                                        rotateTransform.Angle = 180;

                                        transformGroup = new TransformGroup();  
                                        transformGroup.Children.Add(rotateTransform);

                                        scaleTransform = new ScaleTransform();
                                        scaleTransform.ScaleX = imageWidth / IdCardEditWindow.croppedImageList[0].Width;
                                        scaleTransform.ScaleY = imageHeight / IdCardEditWindow.croppedImageList[0].Height;

                                        break;
                                }

                                TransformedBitmap tb = new TransformedBitmap();
                                tb.BeginInit();
                                tb.Source = IdCardEditWindow.croppedImageList[0];
                                tb.Transform = scaleTransform;
                                tb.EndInit();

                                Image imageSideOne = new Image();

                                imageSideOne.Width = imageWidth;
                                imageSideOne.Height = imageHeight;
                                imageSideOne.RenderTransformOrigin = new Point(0.5, 0.5);
                                imageSideOne.RenderTransform = transformGroup;
                                imageSideOne.Source = tb;
                                backgroundPaper.Children.Add(imageSideOne);

                                Canvas.SetLeft(imageSideOne, imageToLeft);
                                Canvas.SetTop(imageSideOne, imageToTop);
                            }
                            break;
                        default:
                            break;
                    }

                    ConvertCanvasToImageSource();
                }
            }
        }
    }
}
