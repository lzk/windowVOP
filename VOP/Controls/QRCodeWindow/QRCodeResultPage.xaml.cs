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
using System.IO;
using ZXing;

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class QRCodeResultPage : UserControl
    {
        public QRCodeWindow ParentWin { get; set; }

        public QRCodeResultPage()
        {
            InitializeComponent();
        }

        public QRCodeResultPage(QRCodeWindow win)
        {
            InitializeComponent();
            ParentWin = win;
        }

        public BitmapSource QRCodeImageSource
        {
            set
            {
                if (value != null)
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(value));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        BitmapImage myBitmapImage = new BitmapImage();
                        myBitmapImage.BeginInit();
                        myBitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                        myBitmapImage.DecodePixelWidth = 400;
                        myBitmapImage.EndInit();

                        ImageView.Source = myBitmapImage;
                    }

                }
            }
        }

        public string QRCodeType
        {
            set
            {
                if (value != null)
                {
                    CodeType.Text = value;
                }
            }
        }

        public Result[] QRCodeResult
        {
            set
            {
                if (value != null)
                {
                    foreach(var v in value)
                    {
                        ResultView.AppendText(v.Text);
                    }
                  
                }
            }
        }

        public void UpdateView()
        {
            TransformGroup transformGroup = null;

            switch (QRCodeManualPage.imageRotationList[0])
            {
                case 270:

                    RotateTransform rotateTransform = new RotateTransform();
                    rotateTransform.Angle = -90;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

                    break;
                case 0:

                    rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 0;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

                    break;
                case 90:

                    rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 90;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

                    break;
                case 180:

                    rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 180;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

               
                    break;
            }

            ImageView.RenderTransformOrigin = new Point(0.5, 0.5);
            ImageView.RenderTransform = transformGroup;
            ImageView.Source = QRCodeManualPage.croppedImageList[0];

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWin != null)
                ParentWin.GotoManualPage();
        }
    }
}
