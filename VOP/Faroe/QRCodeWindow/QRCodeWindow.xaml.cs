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
using ZXing.Multi.QrCode.Internal;
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

    public class QRCodeResult
    {
        public Result[] results = null;
        public Int32Rect subRect;

        public QRCodeResult()
        {

        }

        public QRCodeResult(Result[] results, Int32Rect subRect)
        {
            this.results = results;
            this.subRect = subRect;
        }
    }

    /// <summary>
    /// Interaction logic for IdCardTypeSelectWindow.xaml
    /// </summary>
    public partial class QRCodeWindow : Window
    {
        public bool IsQRCode { get; set; }

        private QRCodeMultiReader qr_reader;
        private GenericMultipleBarcodeReader oneD_reader;
        private GenericMultipleBarcodeReader byquad_reader;
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

            byquad_reader = new GenericMultipleBarcodeReader(new ByQuadrantReader(new QRCodeReader()));
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

                Otsu ot = new Otsu();
                Bitmap temp = (Bitmap)bitmap.Clone();
                ot.Convert2GrayScaleFast(temp);
                int otsuThreshold = ot.getOtsuThreshold((Bitmap)temp);
                ot.threshold(temp, otsuThreshold);

                AsyncWorker worker = new AsyncWorker(this);
                QRCodeResult result = worker.InvokeQRCodeMethod(Decode, temp);

                if (result != null)
                {
                    GotoResultPage(result, ConvertBitmap(temp));
                }
                else
                {
                    GotoManualPage();
                }
               
            }
            GC.Collect();
        }

        public QRCodeResult Decode(Bitmap bitmap)
        {
            try
            {

                LuminanceSource source = new BitmapLuminanceSource(bitmap);
                BinaryBitmap bbitmap = new BinaryBitmap(new GlobalHistogramBinarizer(source));

                QRCodeResult result = new QRCodeResult();

                if(IsQRCode)
                {
                    result.results = qr_reader.decodeMultiple(bbitmap, hints);
                }
                else
                {
                    result.results = oneD_reader.decodeMultiple(bbitmap, hints);
                }
             
                
                if (result.results != null)
                {
                    return result;
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

        public QRCodeResult GoDeeper(Bitmap bitmap)
        {
            QRCodeResult result = new QRCodeResult();
            BitmapSource src = ConvertBitmap(bitmap);

            if (src != null)
            {
                int w = src.PixelWidth;
                int h = src.PixelHeight;

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
                            result.subRect = intRect;

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

                                    result.results = anyResults;

                                    if (result.results != null)
                                    {
                                        return result;
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

            return result;
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

        public void GotoResultPage(QRCodeResult result, BitmapSource src, bool isManual = false)
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

            resultPage.QRCodeResult = result;
        }

        public void GotoManualPage()
        {
            QRCodeManualPage manualPage = new QRCodeManualPage(this);
            manualPage.ImageUri = new Uri(imagePath);
            PageView.Child = manualPage;
        }

        public static BitmapSource ConvertBitmap(Bitmap bmp)
        {
            float dpiX = 0;
            float dpiY = 0;
            IntPtr hBitmap = bmp.GetHbitmap();
            try
            {
                dpiX = bmp.HorizontalResolution;
                dpiY = bmp.VerticalResolution;

                //System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                //    System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                //BitmapSource returnSrc = BitmapSource.Create(bmp.Width, bmp.Height, 96, 96, PixelFormats.Bgr24, null,
                //    data.Scan0, data.Stride * bmp.Height, data.Stride);

                //bmp.UnlockBits(data);

                BitmapSource returnSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                hBitmap,
                                IntPtr.Zero,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());



                return returnSrc;
            }
            finally
            {
                Win32.DeleteObject(hBitmap);
            }

        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            double dpiX = 0;
            double dpiY = 0;

            dpiX = bitmapsource.DpiX;
            dpiY = bitmapsource.DpiY;

            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
                bitmap.SetResolution((float)dpiX, (float)dpiY);
            }
            return bitmap;
        }

        private Bitmap GetBitmap(string imagePath)
        {
            System.Drawing.Image tmpImage;
            Bitmap returnImage;

            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                float dpiX = 0;
                float dpiY = 0;

                tmpImage = System.Drawing.Image.FromStream(fs);

                dpiX = tmpImage.HorizontalResolution;
                dpiY = tmpImage.VerticalResolution;

                returnImage = new Bitmap(tmpImage);
                returnImage.SetResolution(dpiX, dpiY);

                tmpImage.Dispose();
                fs.Dispose();
            }

            return returnImage;
        }

        public CroppedBitmap Crop(BitmapSource src, Int32Rect intRect)
        {
            CroppedBitmap bitmap = null;

            if (intRect.X + intRect.Width > src.PixelWidth)
            {
                intRect.Width = intRect.Width - (intRect.X + intRect.Width - src.PixelWidth);
            }

            if (intRect.Y + intRect.Height > src.PixelHeight)
            {
                intRect.Height = intRect.Height - (intRect.Y + intRect.Height - src.PixelHeight);
            }

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
