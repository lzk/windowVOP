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
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using ZXing;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Rendering;



namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for IdCardTypeSelectWindow.xaml
    /// </summary>
    public partial class QRCodeWindow : Window
    {
        private readonly BarcodeReader reader = new BarcodeReader();
        string imagePath = null;

        public QRCodeWindow()
        {
            InitializeComponent();
        }

        public QRCodeWindow(string path)
        {
            InitializeComponent();
            imagePath = path;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            Execute();
        }

        public static BitmapSource ConvertBitmap(Bitmap bmp)
        {
            IntPtr hBitmap = bmp.GetHbitmap(); 
            try 
            {
               
         
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                hBitmap,
                                IntPtr.Zero,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
            }
            finally 
            {
                Win32.DeleteObject(hBitmap);
            }

        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        private Bitmap GetBitmap(string imagePath)
        {
            System.Drawing.Image tmpImage;
            Bitmap returnImage;

            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                tmpImage = System.Drawing.Image.FromStream(fs);
                returnImage = new Bitmap(tmpImage);
                tmpImage.Dispose();
                fs.Dispose();
            }

            return returnImage;
        }

        private void Execute()
        {
            if (File.Exists(imagePath))
            {
                Bitmap bitmap = GetBitmap(imagePath);

                AsyncWorker worker = new AsyncWorker(this);
                Result[] results = worker.InvokeQRCodeMethod(Decode, bitmap);

                if (results != null)
                {
                    GotoResultPage(results, ConvertBitmap(bitmap));
                }
                else
                {
                    GotoManualPage();
                }
               
            }
            GC.Collect();
        }

        public void GotoResultPage(Result[] results, BitmapSource src, bool isManual=false)
        {
            QRCodeResultPage resultPage = new QRCodeResultPage(this);
            PageView.Child = resultPage;

            if(isManual)
            {
                resultPage.UpdateView();
            }
            else
            {
                resultPage.QRCodeImageSource = src;
            }

            if (results != null)
            {
                resultPage.QRCodeType = results[0].BarcodeFormat.ToString();
                resultPage.QRCodeResult = results;
            }
            else
            {
                resultPage.QRCodeType = "";
                resultPage.ResultView.AppendText("No barcode recognized");
            }
        }

        public void GotoManualPage()
        {
            QRCodeManualPage manualPage = new QRCodeManualPage(this);
            manualPage.ImageUri = new Uri(imagePath);
            PageView.Child = manualPage;
        }

        public Result[] Decode(Bitmap bitmap)
        {
            try
            {
                reader.Options.TryHarder = true;
                return reader.DecodeMultiple(bitmap);       
            }
            catch(Exception)
            {
                return null;
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}
