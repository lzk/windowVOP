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
using ZXing.OneD;
using ZXing.QrCode;
using ZXing.Multi;
using ZXing.Multi.QrCode;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Rendering;

using System.Threading;


namespace VOP.Controls
{
    enum CropLocation
    {
        TOP_LEFT,
        TOP_MIDDLE,
        TOP_RIGHT,
        MIDDLE_LEFT,
        MIDDLE_MIDDLE,
        MIDDLE_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_MIDDLE,
        BOTTOM_RIGHT,
    }
    /// <summary>
    /// Interaction logic for IdCardTypeSelectWindow.xaml
    /// </summary>
    public partial class QRCodeWindow : Window
    {
        public bool IsQRCode { get; set; }

        private QRCodeMultiReader qr_reader;
        private GenericMultipleBarcodeReader oneD_reader;
        private Dictionary<DecodeHintType, object> hints;
        private List<CropLocation> cropLocationList;

        string imagePath = null;


        //public BitmapSource PictureSource { get; set; }

        //public int GetPictureSourceWidth()
        //{
        //    if (PictureSource != null)
        //    {
        //        return (int)PictureSource.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //        new Func<int>(
        //        () =>
        //        {
        //            return PictureSource.PixelWidth;
        //        }
        //        ));
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //public int GetPictureSourceHeight()
        //{
        //    if (PictureSource != null)
        //    {
        //        return (int)PictureSource.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //        new Func<int>(
        //        () =>
        //        {
        //            return PictureSource.PixelHeight;
        //        }
        //        ));
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //public CroppedBitmap GetPictureSourceRect(Int32Rect intRect)
        //{
        //    if (PictureSource != null)
        //    {
        //        return (CroppedBitmap)PictureSource.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //        new Func<Int32Rect, CroppedBitmap>(
        //        (x) =>
        //        {
        //            return Crop(PictureSource, x);
        //        }
        //        ), intRect);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public static Bitmap BitmapFromSourceDelegate(BitmapSource bitmapsource)
        //{
        //    if (bitmapsource != null)
        //    {
        //        return (Bitmap)bitmapsource.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //        new Func<BitmapSource, Bitmap>(
        //        (x) =>
        //        {
        //            return BitmapFromSource(x);
        //        }
        //        ), bitmapsource);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public QRCodeWindow()
        {
            InitializeComponent();
        }

        public QRCodeWindow(string path)
        {
            InitializeComponent();

            IsQRCode = true;
            imagePath = path;

            hints = new Dictionary<DecodeHintType, object>();
            hints.Add(DecodeHintType.TRY_HARDER, true);

            qr_reader = new QRCodeMultiReader();
            oneD_reader = new GenericMultipleBarcodeReader(new MultiFormatOneDReader(hints));

            cropLocationList = new List<CropLocation>();
            cropLocationList.Add(CropLocation.TOP_LEFT);
            cropLocationList.Add(CropLocation.TOP_MIDDLE);
            cropLocationList.Add(CropLocation.TOP_RIGHT);
            cropLocationList.Add(CropLocation.MIDDLE_LEFT);
            cropLocationList.Add(CropLocation.MIDDLE_MIDDLE);
            cropLocationList.Add(CropLocation.MIDDLE_RIGHT);
            cropLocationList.Add(CropLocation.BOTTOM_LEFT);
            cropLocationList.Add(CropLocation.BOTTOM_MIDDLE);
            cropLocationList.Add(CropLocation.BOTTOM_RIGHT);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
  
            Execute();
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

        public Result[] Decode(Bitmap bitmap)
        {
            try
            {

                LuminanceSource source = new BitmapLuminanceSource(bitmap);
                BinaryBitmap bbitmap = new BinaryBitmap(new GlobalHistogramBinarizer(source));

                Result[] results = null;

                if(IsQRCode)
                {
                    results = qr_reader.decodeMultiple(bbitmap, hints);
                }
                else
                {
                    results = oneD_reader.decodeMultiple(bbitmap, hints);
                }
             
                
                if(results != null)
                {
                    return results;
                }
                else
                {
                    return GoDeeper(bitmap);
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public Result[] GoDeeper(Bitmap bitmap)
        {
            Result[] results = null;
           

            BitmapSource src = ConvertBitmap(bitmap);

            if (src != null)
            {
                int w = src.PixelWidth;
                int h = src.PixelHeight;

                //for (int L = 0; L <= 1; L++)
                //{
                //    for (int k = 1; k <= 2; k++)
                //    {
                //        for (int j = 2; j <= 3; j++)
                //        {
                //            for (int i = 0; i < j; i++)
                //            {

                //                Int32Rect intRect = new Int32Rect((int)w/2*L + offset, (int)(h * i / j) + offset, w / k - offset, (int)h / j - offset);
                //                BitmapSource rectImage = Crop(src, intRect);
                //                if (rectImage != null)
                //                {
                //                    LuminanceSource source = new BitmapLuminanceSource(BitmapFromSource(rectImage));
                //                    BinaryBitmap bbitmap = new BinaryBitmap(new GlobalHistogramBinarizer(source));

                //                    Result[] anyResults = null;

                //                    if (IsQRCode)
                //                    {
                //                        anyResults = qr_reader.decodeMultiple(bbitmap, hints);
                //                    }
                //                    else
                //                    {
                //                        anyResults = oneD_reader.decodeMultiple(bbitmap, hints);
                //                    }

                //                    if (results != null)
                //                    {
                //                        if (anyResults != null)
                //                        {
                //                            results.Concat(anyResults);
                //                        }
                //                    }
                //                    else
                //                    {
                //                        results = anyResults;
                //                    }

                //                }
                //            }
             
                //        }
                //    }
                //}

                //if (results != null)
                //{
                //    return results;
                //}

                double offset = 0d;
                for (offset = 0d; offset < 200; offset+=50)
                {
                    foreach (CropLocation loc in cropLocationList)
                    {
                        int tempid = 1;
                        for (double i = 0.9d; i > 0.5d; i -= 0.1d)
                        {
                            Rect rect = new Rect(0d, 0d, (double)w, (double)h);

                            rect.Scale(i, i);

                            switch (loc)
                            {
                                case CropLocation.TOP_LEFT:
                                    rect.Location = new System.Windows.Point(offset, offset);
                                    break;
                                case CropLocation.TOP_MIDDLE:
                                    rect.Location = new System.Windows.Point(((double)w - rect.Width) / 2.0f, offset);
                                    break;
                                case CropLocation.TOP_RIGHT:
                                    rect.Location = new System.Windows.Point((double)w - rect.Width - offset, offset);
                                    break;
                                case CropLocation.MIDDLE_LEFT:
                                    rect.Location = new System.Windows.Point(offset, ((double)h - rect.Height) / 2.0f);
                                    break;
                                case CropLocation.MIDDLE_MIDDLE:
                                    rect.Location = new System.Windows.Point(((double)w - rect.Width) / 2.0f, ((double)h - rect.Height) / 2.0f);
                                    break;
                                case CropLocation.MIDDLE_RIGHT:
                                    rect.Location = new System.Windows.Point((double)w - rect.Width - offset, ((double)h - rect.Height) / 2.0f);
                                    break;
                                case CropLocation.BOTTOM_LEFT:
                                    rect.Location = new System.Windows.Point(offset, (double)h - rect.Height - offset);
                                    break;
                                case CropLocation.BOTTOM_MIDDLE:
                                    rect.Location = new System.Windows.Point(((double)w - rect.Width) / 2.0f, (double)h - rect.Height - offset);
                                    break;
                                case CropLocation.BOTTOM_RIGHT:
                                    rect.Location = new System.Windows.Point((double)w - rect.Width - offset, (double)h - rect.Height - offset);
                                    break;
                                default:
                                    rect.Location = new System.Windows.Point(((double)w - rect.Width) / 2.0f, ((double)h - rect.Height) / 2.0f);
                                    break;
                            }

                            Int32Rect intRect = new Int32Rect((int)(rect.X), (int)(rect.Y), (int)rect.Width, (int)rect.Height);

                            BitmapSource rectImage = Crop(src, intRect);

                          //  string filename = string.Format("pic_{0}_scale{1}_offset{2}.jpg", loc.ToString(), tempid.ToString(), offset.ToString());
                          //  SaveImageToTemp(rectImage, @"E:\BarCode\pic\temp\" + filename);
                            if (rectImage != null)
                            {

                                try
                                {
                                    LuminanceSource source = new BitmapLuminanceSource(BitmapFromSource(rectImage));
                                    GlobalHistogramBinarizer bin = new GlobalHistogramBinarizer(source);
                                    BinaryBitmap bbitmap = new BinaryBitmap(bin);

                                    Result[] anyResults = null;

                                    if (IsQRCode)
                                    {
                                        anyResults = qr_reader.decodeMultiple(bbitmap, hints);
                                    }
                                    else
                                    {
                                        anyResults = oneD_reader.decodeMultiple(bbitmap, hints);
                                    }

                                    results = anyResults;

                                    if (results != null)
                                    {
                                        return results;
                                    }
                                }
                                catch (Exception)
                                {
                                }

                            }
                            tempid++;
                        }
                    }
                }
   
            }

            return results;
        }

        private void SaveImageToTemp(BitmapSource src, string path)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            FileStream fs = File.Open(path, FileMode.Create);
            encoder.Save(fs);
            fs.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void GotoResultPage(Result[] results, BitmapSource src, bool isManual = false)
        {
            QRCodeResultPage resultPage = new QRCodeResultPage(this);
            PageView.Child = resultPage;

            if (isManual)
            {
                resultPage.UpdateView();
            }
            else
            {
                resultPage.QRCodeImageSource = src;
            }

            resultPage.QRCodeResult = results;
        }

        public void GotoManualPage()
        {
            QRCodeManualPage manualPage = new QRCodeManualPage(this);
            manualPage.ImageUri = new Uri(imagePath);
            PageView.Child = manualPage;
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

        public CroppedBitmap Crop(BitmapSource src, Int32Rect intRect)
        {
            CroppedBitmap bitmap = null;


            if (intRect.X >= 0
                && intRect.Y >= 0
                && intRect.X + intRect.Width <= src.PixelWidth
                && intRect.Y + intRect.Height <= src.PixelHeight)
            {
                bitmap = new CroppedBitmap(src, intRect);
            }

            return bitmap;
        }

#region thread helpers
        private MessageBoxEx_Simple_Busy_QRCode qr_pbw = null;
        private ManualResetEvent asyncEvent = new ManualResetEvent(false);
        private bool isNeededProgress = false;

        private void pbw_Loaded(object sender, RoutedEventArgs e)
        {
            asyncEvent.Set();
        }

        void QRCodeCallbackMethod()
        {
            if (isNeededProgress)
            {
                asyncEvent.WaitOne();

                if (qr_pbw != null)
                {
                    qr_pbw.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                     new Action(
                     delegate()
                     {
                         qr_pbw.Close();
                     }
                     ));
                }
            }
        }
#endregion
    }
}
