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
using System.Windows.Forms;

using ZXing;
using ZXing.OneD;
using ZXing.QrCode;
using ZXing.Multi;
using ZXing.Multi.QrCode;
using ZXing.Multi.QrCode.Internal;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Rendering;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

//using OnBarcode.Barcode.BarcodeScanner;

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
        public Result result = null;
        public Int32Rect subRect = Int32Rect.Empty;

        public QRCodeResult()
        {

        }

        public QRCodeResult(Result result, Int32Rect subRect)
        {
            this.result = result;
            this.subRect = subRect;
        }
    }

    public class QRCodeResultComparer : IEqualityComparer<QRCodeResult>
    {
        public bool Equals(QRCodeResult x, QRCodeResult y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            if (x.result.ResultPoints == null || x.result.ResultPoints.Length == 0 
                || y.result.ResultPoints == null || y.result.ResultPoints.Length == 0)
            {
                return false;
            }

            if (x.result.ResultPoints.Length == y.result.ResultPoints.Length)
            {
                if(x.result.ResultPoints.Length == 2)
                {
                    if (x.result.Text == y.result.Text)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    for (int i = 0; i < x.result.ResultPoints.Length; i++)
                    {
                        ResultPoint px = new ResultPoint(x.result.ResultPoints[i].X + x.subRect.X,
                                                         x.result.ResultPoints[i].Y + x.subRect.Y);

                        ResultPoint py = new ResultPoint(y.result.ResultPoints[i].X + y.subRect.X,
                                                         y.result.ResultPoints[i].Y + y.subRect.Y);

                        if (Math.Abs(px.X - py.X) > 1 || Math.Abs(px.Y - py.Y) > 1)
                        {
                            //Trace.WriteLine(string.Format("{0} {1} {2} {3}", x.result.Text, px.ToString(), y.result.Text, py.ToString()));
                            return false;
                        }

                    }

                    return true;
                }
               
            }
            else
            {
                return false;
            }
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(QRCodeResult res)
        {
            string hash_point = res.result.Text;
            //Check whether the object is null
            if (Object.ReferenceEquals(res, null)) return 0;

            //for (int i = 0; i < res.result.ResultPoints.Length; i++)
            //{
            //   ResultPoint p = new ResultPoint(res.result.ResultPoints[i].X + res.subRect.X ,
            //                                   res.result.ResultPoints[i].Y + res.subRect.Y);


            //    hash_point += p.ToString();
            //}

            //return 1;
           // Trace.WriteLine(string.Format("{0} {1} {2}", res.result.Text, hash_point, hash_point.GetHashCode()));
            return hash_point.GetHashCode();
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
        private Dictionary<DecodeHintType, object> hints_qr;
        private Dictionary<DecodeHintType, object> hints_bar;
        private List<CropLocation> cropLocationList;

        List<string> imagePath = null;

        private Otsu ot = new Otsu();
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

        public QRCodeWindow(List<string> path)
        {
            InitializeComponent();

            IsQRCode = true;
            imagePath = path;

            hints_qr = new Dictionary<DecodeHintType, object>();
            hints_qr.Add(DecodeHintType.TRY_HARDER, true);

            hints_bar = new Dictionary<DecodeHintType, object>();
            hints_bar.Add(DecodeHintType.TRY_HARDER, true);

            byquad_reader = new GenericMultipleBarcodeReader(new ByQuadrantReader(new QRCodeReader()));
            qr_reader = new QRCodeMultiReader();
            oneD_reader = new GenericMultipleBarcodeReader(new MultiFormatOneDReader(hints_bar));

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
  
            Execute();
        }

        private void Execute()
        {
            if (File.Exists(imagePath[0]))
            {
                QRCodeResultPage.redRect = Rect.Empty;
                ImageCropper.intRectOffset = Int32Rect.Empty;

                Bitmap bitmap = GetBitmap(imagePath[0]);
                Bitmap temp = bitmap;

                //if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                //{
                //    try
                //    {
                //        temp = (Bitmap)bitmap.Clone();
                //        ot.Convert2GrayScaleFast(temp);
                //        int otsuThreshold = ot.getOtsuThreshold((Bitmap)temp);
                //        ot.threshold(temp, otsuThreshold);
                //    }
                //    catch (Exception) { }

                //}

                Stopwatch sw = new Stopwatch();
                sw.Start();

                AsyncWorker worker = new AsyncWorker(this);
                QRCodeResult[] result = worker.InvokeQRCodeMethod(Decode, temp);

                sw.Stop();
                Trace.WriteLine(string.Format("QR Decode Elapsed = {0}", sw.Elapsed));

                if (result != null)
                {
                    GotoResultPage(result, ConvertBitmap(bitmap));
                }
                else
                {
                    GotoManualPage();
                }
               
            }
            GC.Collect();
        }

        public QRCodeResult[] Decode(Bitmap bitmap)
        {
            try
            {
                //BarcodeDetail[] str = BarcodeScanner.ScanInDetails(bitmap, BarcodeType.All);

                LuminanceSource source = new BitmapLuminanceSource(bitmap);
                BinaryBitmap bbitmap = new BinaryBitmap(new HybridBinarizer(source));

                QRCodeResult[] results = null;
     
                if(MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest == true)
                {
                    return GoDeepest(bitmap);
                }
                else
                {
                    results = GetDecodeResult(bbitmap, Int32Rect.Empty);

                    if (results != null)
                    {
                        return results;
                    }

                    return GoDeeper(bitmap);
                }   
            }
            catch(Exception)
            {
                return null;
            }
        }

        private QRCodeResult[] GetDecodeResult(BinaryBitmap bbitmap, Int32Rect subRect)
        {
            Result[] anyResults = null;

            if (IsQRCode)
            {
                anyResults = qr_reader.decodeMultiple(bbitmap, hints_qr);
            }
            else
            {
                anyResults = oneD_reader.decodeMultiple(bbitmap, hints_bar);
            }

            if (anyResults != null)
            {
                QRCodeResult[] res = new QRCodeResult[anyResults.Length];

                for(int i = 0; i < anyResults.Length; i++)
                {
                    res[i] = new QRCodeResult(anyResults[i], subRect);
                }

                return res;
            }
            else
            {
                return null;
            }
        }

        public QRCodeResult[] GoDeepest(Bitmap bitmap)
        {
            cropLocationList = new List<CropLocation>();

            if(MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_Standard"))
            {
                cropLocationList.Add(CropLocation.TOP_LEFT);
                cropLocationList.Add(CropLocation.TOP_RIGHT);
                cropLocationList.Add(CropLocation.BOTTOM_LEFT);
                cropLocationList.Add(CropLocation.BOTTOM_RIGHT);
            }
            else if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_Fine"))
            {
                cropLocationList.Add(CropLocation.TOP_LEFT);
                cropLocationList.Add(CropLocation.TOP_MIDDLE);
                cropLocationList.Add(CropLocation.TOP_RIGHT);
                cropLocationList.Add(CropLocation.BOTTOM_LEFT);
                cropLocationList.Add(CropLocation.BOTTOM_MIDDLE);
                cropLocationList.Add(CropLocation.BOTTOM_RIGHT);
            }
            else if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_SuperFine"))
            {
                cropLocationList.Add(CropLocation.TOP_LEFT);
                cropLocationList.Add(CropLocation.TOP_MIDDLE);
                cropLocationList.Add(CropLocation.TOP_RIGHT);
                cropLocationList.Add(CropLocation.MIDDLE_LEFT);
                cropLocationList.Add(CropLocation.MIDDLE_RIGHT);
                cropLocationList.Add(CropLocation.BOTTOM_LEFT);
                cropLocationList.Add(CropLocation.BOTTOM_MIDDLE);
                cropLocationList.Add(CropLocation.BOTTOM_RIGHT);
            }
            else if (MainWindow_Rufous.g_settingData.m_QRcode_decode_hardest_level == (string)this.TryFindResource("ResStr_Faroe_HighestQuality"))
            {
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

            QRCodeResult[] result = null;
            BitmapSource src = ConvertBitmap(bitmap);

            if (src != null)
            {
                int w = src.PixelWidth;
                int h = src.PixelHeight;

                double offset = 0d;
                for (offset = 0d; offset < 25; offset += 2)
                {
                    foreach (CropLocation loc in cropLocationList)
                    {
                        int tempid = 1;
                        for (double i = 0.9d; i > 0.3d; i -= 0.1d)
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

                           // string filename = string.Format("pic_{0}_scale{1}_offset{2}.jpg", loc.ToString(), i.ToString(), offset.ToString());
                           // SaveImageToTemp(rectImage, @"G:\work\Rufous\pic\temp\" + filename);
                            if (rectImage != null)
                            {

                                try
                                {
                                    LuminanceSource source = new BitmapLuminanceSource(BitmapFromSource(rectImage));
                                    HybridBinarizer bin = new HybridBinarizer(source);
                                    BinaryBitmap bbitmap = new BinaryBitmap(bin);

                                    QRCodeResult[] anyResults = null;

                                    anyResults = GetDecodeResult(bbitmap, intRect);            

                                    if(anyResults != null)
                                    {
                                        if(result == null)
                                        {
                                            result = anyResults;
                                        }
                                        else
                                        {
                                            IEnumerable<QRCodeResult> union = result.Union(anyResults, new QRCodeResultComparer());
                                            result = union.ToArray();
                                        }
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

        public QRCodeResult[] GoDeeper(Bitmap bitmap)
        {
            cropLocationList = new List<CropLocation>();
            cropLocationList.Add(CropLocation.TOP_LEFT);
         //   cropLocationList.Add(CropLocation.TOP_MIDDLE);
            cropLocationList.Add(CropLocation.TOP_RIGHT);
         //   cropLocationList.Add(CropLocation.MIDDLE_LEFT);
            cropLocationList.Add(CropLocation.MIDDLE_MIDDLE);
         //   cropLocationList.Add(CropLocation.MIDDLE_RIGHT);
            cropLocationList.Add(CropLocation.BOTTOM_LEFT);
          //  cropLocationList.Add(CropLocation.BOTTOM_MIDDLE);
            cropLocationList.Add(CropLocation.BOTTOM_RIGHT);

            QRCodeResult[] result = null;
            BitmapSource src = ConvertBitmap(bitmap);

            if (src != null)
            {
                int w = src.PixelWidth;
                int h = src.PixelHeight;

                double offset = 0d;
                for (offset = 0d; offset < 25; offset += 2)
                {
                    foreach (CropLocation loc in cropLocationList)
                    {
                        int tempid = 1;
                        for (double i = 0.9d; i > 0.3d; i -= 0.1d)
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

                           // string filename = string.Format("pic_{0}_scale{1}_offset{2}.jpg", loc.ToString(), tempid.ToString(), offset.ToString());
                           // SaveImageToTemp(rectImage, @"G:\work\Rufous\pic\temp\" + filename);
                            if (rectImage != null)
                            {

                                try
                                {
                                    LuminanceSource source = new BitmapLuminanceSource(BitmapFromSource(rectImage));
                                    HybridBinarizer bin = new HybridBinarizer(source);
                                    BinaryBitmap bbitmap = new BinaryBitmap(bin);

                                    QRCodeResult[] anyResults = null;

                                    anyResults = GetDecodeResult(bbitmap, intRect);

                                    if (anyResults != null)
                                    {
                                        return anyResults;
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

        public void GotoResultPage(QRCodeResult[] result, BitmapSource src, bool isManual = false)
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
            manualPage.ImageUri = new Uri(imagePath[0]);
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

                //BitmapSource returnSrc = BitmapSource.Create(bmp.Width, bmp.Height, dpiX, dpiY, PixelFormats.Bgr24, null,
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

            //using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            //{
            //    float dpiX = 0;
            //    float dpiY = 0;

            //    tmpImage = System.Drawing.Image.FromStream(fs);

            //    dpiX = tmpImage.HorizontalResolution;
            //    dpiY = tmpImage.VerticalResolution;

            //    returnImage = new Bitmap(tmpImage);
            //    returnImage.SetResolution(dpiX, dpiY);

            //    tmpImage.Dispose();
            //    fs.Dispose();
            //}

            tmpImage = Bitmap.FromFile(imagePath);
            returnImage = (Bitmap)tmpImage.Clone();
            tmpImage.Dispose();

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
        private void btnClose_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
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
