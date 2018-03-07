using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Diagnostics;

using System.IO;
using System.Drawing;
using System.Collections;

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using ZXing;
using ZXing.OneD;
using ZXing.QrCode;
using ZXing.Multi;
using ZXing.Multi.QrCode;
using ZXing.Multi.QrCode.Internal;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Rendering;

using Aspose.BarCode.BarCodeRecognition;

using PdfEncoderClient;
using System.Windows.Media.Imaging;

namespace VOP
{
    public struct ZONE_RECT
    {
        public int x;
        public int y;
        public int width;
        public int height;
    };

    public partial class DetectResult
    {

        public string fileName = null;
        public string resultFileName = null;
        public Result result = null;
        public string barcodeResult = null;
        public string barcodeEncodeType = null;
        public string barcodeResultType = null;
        public int resultX = 0;
        public int resultY = 0;
        public int resultWidth = 0;
        public int resultHeight = 0;
        public int srcWidth = 0;
        public int srcHeight = 0;

        public DetectResult()
        {

        }

        public DetectResult(string fileName, Result result)
        {
            this.fileName = fileName;
            this.result = result;
        }

        public bool FindResult(Result result, ZONE_RECT rect)
        {
            float minX = 10000, minY = 10000, maxX = 0, maxY = 0;

            for (int j = 0; j < result.ResultPoints.Length; j++)
            {
                if (result.ResultPoints[j].X < minX)
                {
                    minX = result.ResultPoints[j].X;
                }

                if (result.ResultPoints[j].X > maxX)
                {
                    maxX = result.ResultPoints[j].X;
                }

                if (result.ResultPoints[j].Y < minY)
                {
                    minY = result.ResultPoints[j].Y;
                }

                if (result.ResultPoints[j].Y > maxY)
                {
                    maxY = result.ResultPoints[j].Y;
                }
            }

            minX += rect.x;
            maxX += rect.x;
            minY += rect.y;
            maxY += rect.y;

            float offset = Math.Max(100, (maxX - minX) / 5);
            minX = Math.Max(0, minX - offset);
            minY = Math.Max(0, minY - offset);

            if (this.result != null
                &&this.result.Text == result.Text
                && this.resultX >= (minX - 80)
                && this.resultX <= (minX + 80)
                && this.resultY >= (minY - 80)
                && this.resultY <= (minY + 80)
                )
                return true;

            return false;
        }
    }

    public class SeparateResult
    {
        public List<string> fileNames = null;
        public string barcodeContent;

        public SeparateResult()
        {

        }

        public SeparateResult(string barcodeContent)
        {
            this.barcodeContent = barcodeContent;
        }

        public void AddFile(string fileName)
        {
            if (fileNames == null)
                fileNames = new List<string>();

            fileNames.Add(fileName);
        }

        public bool FindResult(string barcodeContent)
        {
            if (this.barcodeContent != null && this.barcodeContent == barcodeContent)
                return true;

            return false;
        }
    }

    class QRCodeDetection
    {
        List<string> imagePath = null;
        bool m_bCancel = false;

        public QRCodeDetection(List<string> path)
        {
            imagePath = path;
        }

        public int ExcuteDecode(Window parent)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            m_bCancel = false;

            AsyncWorker worker = new AsyncWorker(parent);
            int result = worker.InvokeQRCodeDetectMethod(Detect);

            if (result == 2) //Cancel
                m_bCancel = true;

            sw.Stop();

            return 0;
        }

        public int ExcuteSeparation(Window parent)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            m_bCancel = false;

            AsyncWorker worker = new AsyncWorker(parent);
            int result = worker.InvokeQRCodeDetectMethod(Separation);

            if (result == 2) //Cancel
                m_bCancel = true;

            sw.Stop();

            return 0;
        }

        public int Separation()
        {
            int nResult = 0;

            ArrayList SeparateArray = new ArrayList();
            int nCurrentIndex = -1;

            foreach (string fileName in imagePath)
            {
                if (m_bCancel == true)
                    break;

                if (File.Exists(fileName))
                {
                    try
                    {
                        FileStream imageStreamSource = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        Bitmap srcBitmap = new Bitmap(imageStreamSource);
                        int nWidth = srcBitmap.Width;
                        int nHeight = srcBitmap.Height;
                        int nSubHeight = nHeight / 5;

                        Rectangle cloneRect = new Rectangle(0, 0, nWidth, nSubHeight);
                        Bitmap subBitmap = srcBitmap.Clone(cloneRect, srcBitmap.PixelFormat);

                        string barcodeContent = GetBarcodeContent(ref subBitmap);
                        if(barcodeContent == null)
                        {
                            cloneRect = new Rectangle(0, nHeight-nSubHeight, nWidth, nSubHeight);
                            subBitmap = srcBitmap.Clone(cloneRect, srcBitmap.PixelFormat);

                            barcodeContent = GetBarcodeContent(ref subBitmap);
                        }

                        imageStreamSource.Close();

                        if(barcodeContent == null)
                        {
                            if(nCurrentIndex == -1)
                            {
                                SeparateResult separateResult = new SeparateResult();
                                separateResult.AddFile(fileName);
                                SeparateArray.Add(separateResult);
                                nCurrentIndex++;
                            }
                            else if(SeparateArray.Count > nCurrentIndex)
                            {
                                SeparateResult separateResult = (SeparateResult)SeparateArray[nCurrentIndex];
                                separateResult.AddFile(fileName);
                            }
                        }
                        else
                        {
                            bool bExist = false;
                            for (int nIndex=0; nIndex<SeparateArray.Count; nIndex++)
                            {
                                SeparateResult separateResult = (SeparateResult)SeparateArray[nIndex];
                                if(separateResult.barcodeContent == barcodeContent)
                                {
                                    separateResult.AddFile(fileName);
                                    nCurrentIndex = nIndex;
                                    bExist = true;
                                    break;
                                }
                            }

                            if (bExist == false)
                            {
                                SeparateResult separateResult = new SeparateResult();
                                separateResult.AddFile(fileName);
                                separateResult.barcodeContent = barcodeContent;
                                SeparateArray.Add(separateResult);
                                nCurrentIndex++;
                            }
                        }
                    }

                    catch (Exception)
                    {

                    }
                }
            }

            if (m_bCancel == false)
                SaveSeparatedFiles(SeparateArray);

            GC.Collect();

            return nResult;
        }

        public int SaveSeparatedFiles(ArrayList SeparateArray)
        {
            int nResult = 0;

            try
            {
                if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_separateFilePath))
                {
                    Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_separateFilePath);
                }
            }
            catch(Exception)
            {
                MainWindow_Rufous.g_settingData.m_separateFilePath = App.PictureFolder;
            }

            foreach (SeparateResult separateResult in SeparateArray)
            {
                if(MainWindow_Rufous.g_settingData.m_separateFileType == 0)
                    SaveFiles2PDF(separateResult);
                else
                    SaveFiles2TIFF(separateResult);
            }

            return nResult;
        }

        public int SaveFiles2PDF(SeparateResult separateResult)
        {
            int nResult = 0;
            string saveFileName = null;
            string pdfFileName = null;

            try
            {
                using (PdfHelper help = new PdfHelper())
                {
                    foreach (string fileName in separateResult.fileNames)
                    {
                        if (saveFileName == null)
                        {
                            pdfFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);

                            if (separateResult.barcodeContent != null)
                            {
                                pdfFileName += "#" + separateResult.barcodeContent + ".pdf";
                            }
                            else
                            {
                                pdfFileName += "#0.pdf";
                            }

                            saveFileName = MainWindow_Rufous.g_settingData.m_separateFilePath + "\\" + pdfFileName;
                            if (saveFileName.Length >= 260 || MainWindow_Rufous.g_settingData.m_separateFilePath.Length >= 248)
                            {
                                saveFileName = System.IO.Path.GetDirectoryName(fileName) + "\\" + pdfFileName;
                            }

                            help.Open(saveFileName);
                        }

                        Uri myUri = new Uri(fileName, UriKind.RelativeOrAbsolute);
                        JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                        BitmapSource origSource = decoder.Frames[0];

                        if (null != origSource)
                            help.AddImage(origSource, 0);
                    }

                    help.Close();

                    System.Diagnostics.Process.Start(saveFileName);

                }
            }
            catch(Exception)
            {

            }

            return nResult;
        }

        public int SaveFiles2TIFF(SeparateResult separateResult)
        {
            int nResult = 0;
            string saveFileName = null;
            string tiffFileName = null;

            try
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();

                foreach (string fileName in separateResult.fileNames)
                {
                    if (saveFileName == null)
                    {
                        tiffFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);

                        if (separateResult.barcodeContent != null)
                        {
                            tiffFileName += "#" + separateResult.barcodeContent + ".tif";
                        }
                        else
                        {
                            tiffFileName += "#0.tif";
                        }

                        saveFileName = MainWindow_Rufous.g_settingData.m_separateFilePath + "\\" + tiffFileName;
                        if (saveFileName.Length >= 260 || MainWindow_Rufous.g_settingData.m_separateFilePath.Length >= 248)
                        {
                            saveFileName = System.IO.Path.GetDirectoryName(fileName) + "\\" + tiffFileName;
                        }

                    }

                    Uri myUri = new Uri(fileName, UriKind.RelativeOrAbsolute);
                    JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                    BitmapSource origSource = decoder.Frames[0];

                    BitmapMetadata bitmapMetadata = new BitmapMetadata("tiff");
                    bitmapMetadata.ApplicationName = "Virtual Operation Panel";

                    if (null != origSource)
                        encoder.Frames.Add(BitmapFrame.Create(origSource, null, bitmapMetadata, null));
                }

                FileStream fs = File.Open(saveFileName, FileMode.Create);
                encoder.Save(fs);
                fs.Close();

                System.Diagnostics.Process.Start(saveFileName);

            }
            catch (Exception)
            {

            }

            return nResult;
        }

        public Result[] FindSubQRCode(ref Bitmap srcBitmap, ZONE_RECT rect, bool bFullImage)
        {
            Result[] results = null;
            int nCount = 1;

            if (bFullImage == true)
                nCount = 1;
            try
            {
                for (int i = 0; i < nCount; i++)
                {
                    ZONE_RECT tempRect = rect;
                    rect.x = Math.Max(0, rect.x - i);
                    rect.y = Math.Max(0, rect.y - i);
                    rect.width = Math.Min(srcBitmap.Width-rect.x, rect.width + i * 2);
                    rect.height = Math.Min(srcBitmap.Height - rect.y, rect.height + i * 2);

                    if (rect.width < 30 || rect.height < 30)
                        continue;

                    Rectangle cloneRect = new Rectangle(rect.x, rect.y, rect.width, rect.height);

                    Bitmap subBitmap = srcBitmap.Clone(cloneRect, srcBitmap.PixelFormat);

                    LuminanceSource source = null;
                    if (bFullImage)
                        source = new BitmapLuminanceSource(srcBitmap);
                    else
                        source = new BitmapLuminanceSource(subBitmap);

                    BinaryBitmap bbitmap1 = new BinaryBitmap(new GlobalHistogramBinarizer(source));
                    //BinaryBitmap bbitmap2 = new BinaryBitmap(new HybridBinarizer(source));

                    IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
                    hints.Add(DecodeHintType.TRY_HARDER, true);

                    QRCodeMultiReader qrReader = new QRCodeMultiReader();
                    results = qrReader.decodeMultiple(bbitmap1, hints);
                    if(results != null)
                    {
                        break;
                    }

                    //results = qrReader.decodeMultiple(bbitmap2, hints);
                    //if (results != null)
                    //{
                    //    break;
                    //}

                }
            }
            catch(Exception)
            {

            }

            return results;
        }

        [DllImport("qrcodezone.dll", EntryPoint = "#1")]
        public static extern int FindQRCodeZones([MarshalAs(UnmanagedType.LPWStr)]String fileName, [Out] ZONE_RECT[] lpRect, int nMaxArraySize);

        public int Detect()
        {
            int nResult = 0;
            ArrayList resultArray = new ArrayList();
            string strPath = null;
            string strSubFileName = null;

            foreach (string fileName in imagePath)
            {
                if (m_bCancel == true)
                    break;

                if (File.Exists(fileName))
                {
                    try
                    {
                        int nLength = 0;

                        if (strPath == null)
                        {
                            nLength = fileName.LastIndexOf('\\');
                            if (nLength > 0)
                            {
                                strPath = fileName.Substring(0, nLength) + "\\";
                            }
                        }

                        nLength = fileName.LastIndexOf('.');
                        if (nLength > 0)
                        {
                            strSubFileName = fileName.Substring(0, nLength);
                        }

                        ArrayList resultArray_inOneImage = new ArrayList();
                        ArrayList resultArray_inOneImage_barcode = new ArrayList();

                        QRCodeMultiReader qrReader = new QRCodeMultiReader();

                        FileStream imageStreamSource = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        Bitmap srcBitmap = new Bitmap(imageStreamSource);

//                        LuminanceSource source = new BitmapLuminanceSource(srcBitmap);
  //                      BinaryBitmap bbitmap1 = new BinaryBitmap(new GlobalHistogramBinarizer(source));
    //                    BinaryBitmap bbitmap2 = new BinaryBitmap(new HybridBinarizer(source));

      //                  IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
        //                hints.Add(DecodeHintType.TRY_HARDER, true);
                        //hints.Add(DecodeHintType.CHARACTER_SET, "UTF-8");

                        if (MainWindow_Rufous.g_settingData.m_decodeType == 0 || MainWindow_Rufous.g_settingData.m_decodeType == 2)//QRCode or All
                        {
                            try
                            {
                                ZONE_RECT[] lpRect = new ZONE_RECT[2000];

                                int nArraySize = 0;

                                try
                                {
                                    nArraySize = FindQRCodeZones(fileName, lpRect, lpRect.Length);
                                }
                                catch(Exception)
                                {

                                }

                                //                                if(nArraySize == 0)
                                {
                                    lpRect[nArraySize].x = 0;
                                    lpRect[nArraySize].y = 0;
                                    lpRect[nArraySize].width = srcBitmap.Width;
                                    lpRect[nArraySize].height = srcBitmap.Height;

                                    nArraySize++; 
                                }

                                System.DateTime date1 = DateTime.Now;

                                for (int index=0; index < nArraySize; index++)
                                {
                                    if (m_bCancel == true)
                                        break;

                                    bool bFullImage = false;
                                    if (index == nArraySize - 1)
                                        bFullImage = true;

                                    Result[] results = FindSubQRCode(ref srcBitmap, lpRect[index], bFullImage);
                                    if (results != null)
                                    {
                                        int nCount = results.Count();
                                        for (int i = 0; i < nCount; i++)
                                        {
                                            if (m_bCancel == true)
                                                break;

                                            if (ContentsExists(resultArray_inOneImage, results[i], lpRect[index]) == false)
                                            {
                                                DetectResult detectResult = new DetectResult(fileName, results[i]);
                                                string subFileName = strSubFileName + "result3" + index.ToString() + "_" + i.ToString() + ".jpg";
                                                ZONE_RECT qrcodeZone = CreateResultImage(ref srcBitmap, subFileName, results[i], lpRect[index]);
                                                detectResult.srcWidth = srcBitmap.Width;
                                                detectResult.srcHeight = srcBitmap.Height;
                                                detectResult.resultX = qrcodeZone.x;
                                                detectResult.resultY = qrcodeZone.y;
                                                detectResult.resultWidth = qrcodeZone.width;
                                                detectResult.resultHeight = qrcodeZone.height;

                                                detectResult.resultFileName = subFileName;
                                                resultArray.Add(detectResult);
                                                resultArray_inOneImage.Add(detectResult);
                                            }
                                        }
                                    }

                                    if (index == nArraySize - 1)
                                        break;

                                    System.TimeSpan diff = DateTime.Now - date1;
                                    if (diff.Seconds>30)
                                    {
                                        index = nArraySize - 2;
                                    }
                                }
                                if(resultArray_inOneImage.Count <=0 )//Use Aspose.Barcode
                                {
                                    Aspose.BarCode.License l1 = new Aspose.BarCode.License();
                                    l1.SetLicense("VOP.Aspose.Total.lic");

                                    BarCodeReader barCodeReader;
                                    barCodeReader = new BarCodeReader(imageStreamSource, DecodeType.QR);
                                    int index = 0;

                                    while (barCodeReader.Read())
                                    {
                                        string resultText = barCodeReader.GetCodeText();

                                        //if (ContentsExists(resultArray_inOneImage, results[i], lpRect[index]) == false)
                                        {
                                            BarCodeRegion region = barCodeReader.GetRegion();
                                            DetectResult detectResult = new DetectResult(fileName, null);
                                            detectResult.barcodeResult = resultText;
                                            detectResult.barcodeEncodeType = "QR_CODE";// barCodeReader.GetCodeType().ToString();
                                            if(resultText.Contains("http"))
                                                detectResult.barcodeResultType = "URI";
                                            else
                                                detectResult.barcodeResultType = "TEXT";
                                            string subFileName = strSubFileName + "barcodeResult1" + index.ToString() + ".jpg";
                                            System.Drawing.Point barcodePoint = CreateBarcodeResultImage(ref srcBitmap, subFileName, region);
                                            detectResult.resultWidth = barcodePoint.X;
                                            detectResult.resultHeight = barcodePoint.Y;
                                            detectResult.srcWidth = srcBitmap.Width;
                                            detectResult.srcHeight = srcBitmap.Height;
                                            detectResult.resultFileName = subFileName;
                                            index++;
                                            resultArray.Add(detectResult);
                                            resultArray_inOneImage.Add(detectResult);
                                        }

                                    }
                                    barCodeReader.Close();
                                }

                                /*
                                                                Result[] results1 = qrReader.decodeMultiple(bbitmap1, hints);
                                                                if (results1 != null)
                                                                {
                                                                    int nCount = results1.Count();
                                                                    for (int i = 0; i < nCount; i++)
                                                                    {
                                                                        DetectResult detectResult = new DetectResult(fileName, results1[i]);
                                                                        string subFileName = strSubFileName + "result1" + i.ToString() + ".jpg";
                                                                        System.Drawing.Point qrcodePoint = CreateResultImage(srcBitmap, subFileName, results1[i]);
                                                                        detectResult.srcWidth = srcBitmap.Width;
                                                                        detectResult.srcHeight = srcBitmap.Height;
                                                                        detectResult.resultWidth = qrcodePoint.X;
                                                                        detectResult.resultHeight = qrcodePoint.Y;
                                                                        detectResult.resultFileName = subFileName;
                                                                        resultArray.Add(detectResult);
                                                                        resultArray_inOneImage.Add(detectResult);
                                                                    }
                                                                }

                                                                Result[] results2 = qrReader.decodeMultiple(bbitmap2, hints);
                                                                if (results2 != null)
                                                                {
                                                                    int nCount = results2.Count();
                                                                    for (int i = 0; i < nCount; i++)
                                                                    {
                                                                        if (ContentsExists(resultArray_inOneImage, results2[i]) == false)
                                                                        {
                                                                            DetectResult detectResult = new DetectResult(fileName, results2[i]);
                                                                            string subFileName = strSubFileName + "result2" + i.ToString() + ".jpg";
                                                                            System.Drawing.Point qrcodePoint = CreateResultImage(srcBitmap, subFileName, results2[i]);
                                                                            detectResult.srcWidth = srcBitmap.Width;
                                                                            detectResult.srcHeight = srcBitmap.Height;
                                                                            detectResult.resultWidth = qrcodePoint.X;
                                                                            detectResult.resultHeight = qrcodePoint.Y;

                                                                            detectResult.resultFileName = subFileName;
                                                                            resultArray.Add(detectResult);
                                                                            resultArray_inOneImage.Add(detectResult);
                                                                        }
                                                                    }
                                                                }
                                */
                            }
                            catch (Exception ex)
                            {
                                //string theMessage = "Exception=" + ex.ToString();
                                //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                //                            Application.Current.MainWindow,
                                //                            theMessage,
                                //                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning")
                                //                            );

                            }
                        }

                        if (MainWindow_Rufous.g_settingData.m_decodeType == 1 || MainWindow_Rufous.g_settingData.m_decodeType == 2)//Bar Code or All
                        {
                            try
                            {
                                const int MINBARCODELENDTH = 5;

                                LuminanceSource source = new BitmapLuminanceSource(srcBitmap);
                                BinaryBitmap bbitmap1 = new BinaryBitmap(new GlobalHistogramBinarizer(source));
                                BinaryBitmap bbitmap2 = new BinaryBitmap(new HybridBinarizer(source));

                                IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
                                hints.Add(DecodeHintType.TRY_HARDER, true);
//                                hints.Add(DecodeHintType.CHARACTER_SET, "UTF-8");

                                GenericMultipleBarcodeReader oneD_reader = new GenericMultipleBarcodeReader(new MultiFormatOneDReader(hints));
                                Result[] barCodeResults1 = oneD_reader.decodeMultiple(bbitmap1, hints);
                                if (barCodeResults1 != null)
                                {
                                    int nCount = barCodeResults1.Count();
                                    for (int i = 0; i < nCount; i++)
                                    {
                                        if (barCodeResults1[i].ToString().Length < MINBARCODELENDTH)
                                            continue;

                                        DetectResult detectResult = new DetectResult(fileName, barCodeResults1[i]);
                                        string subFileName = strSubFileName + "barcodeResult1" + i.ToString() + ".jpg";
                                        ZONE_RECT zoneRect = new ZONE_RECT();
                                        zoneRect.x = 0;
                                        zoneRect.y = 0;
                                        ZONE_RECT qrcodeZone = CreateResultImage(ref srcBitmap, subFileName, barCodeResults1[i], zoneRect);
                                        detectResult.srcWidth = srcBitmap.Width;
                                        detectResult.srcHeight = srcBitmap.Height;
                                        detectResult.resultX = qrcodeZone.x;
                                        detectResult.resultY = qrcodeZone.y;
                                        detectResult.resultWidth = qrcodeZone.width;
                                        detectResult.resultHeight = qrcodeZone.height;

                                        detectResult.resultFileName = subFileName;
                                        resultArray.Add(detectResult);
                                        resultArray_inOneImage_barcode.Add(detectResult);
                                    }
                                }

                                Result[] barCodeResults2 = oneD_reader.decodeMultiple(bbitmap2, hints);
                                if (barCodeResults2 != null)
                                {
                                    int nCount = barCodeResults2.Count();
                                    for (int i = 0; i < nCount; i++)
                                    {
                                        if (barCodeResults2[i].ToString().Length < MINBARCODELENDTH)
                                            continue;

                                        ZONE_RECT zoneRect = new ZONE_RECT();
                                        zoneRect.x = 0;
                                        zoneRect.y = 0;

                                        if (ContentsExists(resultArray_inOneImage_barcode, barCodeResults2[i], zoneRect) == false)
                                        {
                                            DetectResult detectResult = new DetectResult(fileName, barCodeResults2[i]);
                                            string subFileName = strSubFileName + "barcodeResult2" + i.ToString() + ".jpg";
                                            ZONE_RECT qrcodeZone = CreateResultImage(ref srcBitmap, subFileName, barCodeResults2[i], zoneRect);
                                            detectResult.srcWidth = srcBitmap.Width;
                                            detectResult.srcHeight = srcBitmap.Height;
                                            detectResult.resultX = qrcodeZone.x;
                                            detectResult.resultY = qrcodeZone.y;
                                            detectResult.resultWidth = qrcodeZone.width;
                                            detectResult.resultHeight = qrcodeZone.height;

                                            detectResult.resultFileName = subFileName;
                                            resultArray.Add(detectResult);
                                            resultArray_inOneImage_barcode.Add(detectResult);
                                        }
                                    }
                                }

                               // if (resultArray_inOneImage_barcode.Count <= 0)
                                {
                                    Aspose.BarCode.License l1 = new Aspose.BarCode.License();
                                    l1.SetLicense("VOP.Aspose.Total.lic");

                                    BarCodeReader barCodeReader;
                                    barCodeReader = new BarCodeReader(imageStreamSource, DecodeType.AllSupportedTypes);
                                    int index = 0;

                                    while (barCodeReader.Read())
                                    {
                                        if (barCodeReader.GetCodeType() == DecodeType.QR)
                                            continue;

                                        string resultText = barCodeReader.GetCodeText();
                                        if (resultText.Length < MINBARCODELENDTH)
                                            continue;

                                        if (BarcodeContentsExists(resultArray_inOneImage_barcode, resultText) == false)
                                        {
                                            BarCodeRegion region = barCodeReader.GetRegion();
                                            DetectResult detectResult = new DetectResult(fileName, null);
                                            detectResult.barcodeResult = resultText;
                                            detectResult.barcodeEncodeType = barCodeReader.GetCodeType().ToString();
                                            detectResult.barcodeResultType = "TEXT";
                                            string subFileName = strSubFileName + "barcodeResult1" + index.ToString() + ".jpg";
                                            System.Drawing.Point barcodePoint = CreateBarcodeResultImage(ref srcBitmap, subFileName, region);
                                            detectResult.resultWidth = barcodePoint.X;
                                            detectResult.resultHeight = barcodePoint.Y;
                                            detectResult.srcWidth = srcBitmap.Width;
                                            detectResult.srcHeight = srcBitmap.Height;
                                            detectResult.resultFileName = subFileName;
                                            index++;
                                            resultArray.Add(detectResult);
                                            resultArray_inOneImage_barcode.Add(detectResult);
                                        }

                                    }
                                    barCodeReader.Close();
                                }
                            }
                            catch (Exception)
                            {
                            }

                        }

                        if (resultArray_inOneImage.Count <=0 && resultArray_inOneImage_barcode.Count <= 0)
                        {
                            DetectResult detectResult = new DetectResult(fileName, null);
                            detectResult.srcWidth = srcBitmap.Width;
                            detectResult.srcHeight = srcBitmap.Height;
                            detectResult.barcodeEncodeType = "N/A";
                            detectResult.barcodeResultType = "N/A";
                            detectResult.barcodeResult = "Null";
                            resultArray.Add(detectResult);
                        }
                        imageStreamSource.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (m_bCancel == false)
                WriteToHTML(strPath, resultArray);
/*
            foreach (DetectResult detectResult in resultArray)
            {
                try
                {
                    File.Delete(detectResult.fileName);
                    File.Delete(detectResult.resultFileName);
                }
                catch (Exception)
                {
                }
            }
*/

            GC.Collect();

            return nResult;
        }

        public string GetBarcodeContent(ref Bitmap subBitmap)
        {
            string barcodeContent = null;
            const int MINBARCODELENDTH_SEP = 8;

            try
            {
                LuminanceSource source = new BitmapLuminanceSource(subBitmap);
                BinaryBitmap bbitmap1 = new BinaryBitmap(new GlobalHistogramBinarizer(source));

                IDictionary<DecodeHintType, object> hints = new Dictionary<DecodeHintType, object>();
                hints.Add(DecodeHintType.TRY_HARDER, true);
                GenericMultipleBarcodeReader oneD_reader = new GenericMultipleBarcodeReader(new MultiFormatOneDReader(hints));
                Result[] barCodeResults = null;
                try
                {
                    barCodeResults = oneD_reader.decodeMultiple(bbitmap1, hints);
                }
                catch (Exception)
                {

                }

                if (barCodeResults != null)
                {
                    int nCount = barCodeResults.Count();
                    for (int i = 0; i < nCount; i++)
                    {
                        barcodeContent = barCodeResults[i].ToString();
                        if (barcodeContent.Length >= MINBARCODELENDTH_SEP)
                            break;
                    }
                }
                if (barcodeContent == null || barcodeContent.Length < MINBARCODELENDTH_SEP)
                {
                    BinaryBitmap bbitmap2 = new BinaryBitmap(new HybridBinarizer(source));
                    try
                    {
                        barCodeResults = oneD_reader.decodeMultiple(bbitmap1, hints);
                        if (barCodeResults != null)
                        {
                            int nCount = barCodeResults.Count();
                            for (int i = 0; i < nCount; i++)
                            {
                                barcodeContent = barCodeResults[i].ToString();
                                if (barcodeContent.Length >= MINBARCODELENDTH_SEP)
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }

                }

                if (barcodeContent == null || barcodeContent.Length < MINBARCODELENDTH_SEP)
                {
                    Aspose.BarCode.License l1 = new Aspose.BarCode.License();
                    l1.SetLicense("VOP.Aspose.Total.lic");

                    BarCodeReader barCodeReader;
                    barCodeReader = new BarCodeReader(subBitmap, DecodeType.AllSupportedTypes);
                    while (barCodeReader.Read())
                    {
                        if (barCodeReader.GetCodeType() == DecodeType.QR)
                            continue;

                        barcodeContent = barCodeReader.GetCodeText();

                        if (barcodeContent.Length >= MINBARCODELENDTH_SEP)
                            break;
                    }
                }
            }
            catch(Exception)
            {

            }
            if (barcodeContent != null && barcodeContent.Length >= MINBARCODELENDTH_SEP)
                return barcodeContent;
            else
                return null;

        }
        public System.Drawing.Point CreateBarcodeResultImage(ref Bitmap srcBitmap, string subFileName, BarCodeRegion region)
        {
            float minX = 10000, minY = 10000, maxX = 0, maxY = 0;

            for (int j = 0; j < region.Points.Length; j++)
            {
                if (region.Points[j].X < minX)
                {
                    minX = region.Points[j].X;
                }

                if (region.Points[j].X > maxX)
                {
                    maxX = region.Points[j].X;
                }

                if (region.Points[j].Y < minY)
                {
                    minY = region.Points[j].Y;
                }

                if (region.Points[j].Y > maxY)
                {
                    maxY = region.Points[j].Y;
                }
            }
            float xoffset = Math.Max(100, (maxX - minX) / 5);
            float yoffset = Math.Max(100, (maxY - minY) / 5);
            minX = Math.Max(0, minX - xoffset);
            minY = Math.Max(0, minY - yoffset);
            maxX = Math.Min(srcBitmap.Width, maxX + xoffset);
            maxY = Math.Min(srcBitmap.Height, maxY + yoffset);

            Rectangle cloneRect = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
            Bitmap subBitmap = srcBitmap.Clone(cloneRect, srcBitmap.PixelFormat);
            FileStream fs = File.Open(subFileName, FileMode.Create);
            subBitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();

            return new System.Drawing.Point((int)(maxX - minX), (int)(maxY - minY));
        }

        public ZONE_RECT CreateResultImage(ref Bitmap srcBitmap, string subFileName, Result result, ZONE_RECT zoneRect)
        {

            float minX = 10000, minY = 10000, maxX = 0, maxY = 0;

            for (int j = 0; j < result.ResultPoints.Length; j++)
            {
                if (result.ResultPoints[j].X < minX)
                {
                    minX = result.ResultPoints[j].X;
                }

                if (result.ResultPoints[j].X > maxX)
                {
                    maxX = result.ResultPoints[j].X;
                }

                if (result.ResultPoints[j].Y < minY)
                {
                    minY = result.ResultPoints[j].Y;
                }

                if (result.ResultPoints[j].Y > maxY)
                {
                    maxY = result.ResultPoints[j].Y;
                }
            }

            minX += zoneRect.x;
            maxX += zoneRect.x;
            minY += zoneRect.y;
            maxY += zoneRect.y;

            float offset = Math.Max(100, (maxX-minX) / 5);
            minX = Math.Max(0, minX - offset);
            minY = Math.Max(0, minY - offset);
            maxX = Math.Min(srcBitmap.Width, maxX + offset);
            maxY = Math.Min(srcBitmap.Height, maxY + offset);

            Rectangle cloneRect = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
            Bitmap subBitmap = srcBitmap.Clone(cloneRect, srcBitmap.PixelFormat);
            FileStream fs = File.Open(subFileName, FileMode.Create);
            subBitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();

            ZONE_RECT retRect;
            retRect.x = cloneRect.X;
            retRect.y = cloneRect.Y;
            retRect.width = cloneRect.Width;
            retRect.height = cloneRect.Height;

            return retRect;
        }
        public bool ContentsExists(ArrayList resultArray, Result result, ZONE_RECT rect)
        {
            bool bRet = false;

            foreach(DetectResult detectResult in resultArray)
            {
                if(detectResult.FindResult(result, rect) == true)
                {
                    bRet = true;
                    break;
                }
            }

            return bRet;
        }

        public bool BarcodeContentsExists(ArrayList resultArray, string result)
        {
            bool bRet = false;

            foreach (DetectResult detectResult in resultArray)
            {
                if (detectResult.result.Text == result)
                {
                    bRet = true;
                    break;
                }
            }

            return bRet;
        }

        public int WriteToHTML(string strPath, ArrayList resultArray)
        {
            int nRet = -1;

            if (resultArray.Count <= 0)
                return nRet;

            string htmlFileName = strPath + MainWindow_Rufous.g_settingData.m_decodeResultFile;// "QRCodeBarcodeResult.html";
            int nLength = 0;

            string extentionFile = null;
            nLength = htmlFileName.LastIndexOf('.');
            if(nLength>0)
                extentionFile = htmlFileName.Substring(nLength);
            if (extentionFile == null || extentionFile != ".html")
            {
                htmlFileName += ".html";
            }

            using (StreamWriter htmlWriter = new StreamWriter(htmlFileName, false, Encoding.UTF8))
            {
                htmlWriter.WriteLine("<html> <head> <title>QRCode/Barcode Detection Result</title> </head> <body>");
                htmlWriter.WriteLine("<style> table { font-family: \"Microsoft YaHei\"; font-size:12px } </style>");
                htmlWriter.WriteLine("<body>");
                htmlWriter.WriteLine("<H3>Result:</H3>");

                htmlWriter.WriteLine("<table border=\"1\" cellspacing=\"0\" vspace=\"0\" hspace=\"0\" cellpadding=\"10\">");
                htmlWriter.WriteLine("<tr>");
                htmlWriter.WriteLine("<th> File Name	     </th>");
                htmlWriter.WriteLine("<th> Source Bitmap	     </th>");
                htmlWriter.WriteLine("<th> Decode Bitmap	     </th>");
                htmlWriter.WriteLine("<th> Code Type	 </th>");
                htmlWriter.WriteLine("<th> Result Type	 </th>");
                htmlWriter.WriteLine("<th> Contents  	 </th>");
                htmlWriter.WriteLine("</tr>");

                ArrayList tempArray = new ArrayList();
                string szFileSameName = null;
                int nIndex = 0;
                resultArray.Add(new DetectResult());

                foreach (DetectResult detectResult in resultArray)
                {
                    nIndex++;

                    if(szFileSameName == null)
                    {
                        szFileSameName = detectResult.fileName;
                        tempArray.Add(detectResult);
                        if (nIndex < resultArray.Count)
                            continue;
                    }
                    else if(szFileSameName == detectResult.fileName)
                    {
                        tempArray.Add(detectResult);
                        if(nIndex < resultArray.Count)
                            continue;
                    }

                    bool bFirstLine = true;
                    int nCount = tempArray.Count;
                    foreach (DetectResult tempDetectResult in tempArray)
                    {
                        htmlWriter.WriteLine("<tr>");

                        if (bFirstLine == true)
                        {
                            String tempFileName = System.IO.Path.GetFileName(tempDetectResult.fileName);
                            htmlWriter.WriteLine("<td rowspan=\"" + nCount.ToString() + "\">" + tempFileName + "</td>");

                            if (tempDetectResult.fileName != null)
                            {
                                int nWidth = tempDetectResult.srcWidth;
                                int nHeight = tempDetectResult.srcHeight;
                                if(nWidth > nHeight)
                                {
                                    if(nWidth>300)
                                    {
                                        nHeight = 300 * nHeight / nWidth;
                                        nWidth = 300;
                                    }
                                }
                                else
                                {
                                    if (nHeight > 300)
                                    {
                                        nWidth = 300 * nWidth / nHeight;
                                        nHeight = 300;
                                    }
                                }
                                htmlWriter.WriteLine("<td rowspan=\"" + nCount.ToString() + "\">" + "<img src=\"" + tempDetectResult.fileName + "\" height=\"" + nHeight.ToString() + "\" width=\"" + nWidth.ToString() + "\"></td>");
                                App.decodeFiles.Add(tempDetectResult.fileName);

                            }
                            else
                            {
                                htmlWriter.WriteLine("<td rowspan=\"" + nCount.ToString() + "\"></td>");
                            }

                            bFirstLine = false;
                        }

                        if (tempDetectResult.barcodeResult == null)
                        {
                            if (tempDetectResult.resultFileName != null)
                            {
                                int nWidth = tempDetectResult.resultWidth;
                                int nHeight = tempDetectResult.resultHeight;

                                if (nWidth > nHeight)
                                {
                                    if (nWidth > 100)
                                    {
                                        nHeight = 100 * nHeight / nWidth;
                                        nWidth = 100;
                                    }
                                }
                                else
                                {
                                    if (nHeight > 100)
                                    {
                                        nWidth = 100 * nWidth / nHeight;
                                        nHeight = 100;
                                    }
                                }

                                htmlWriter.WriteLine("<td>" + "<img src=\"" + tempDetectResult.resultFileName + "\" height=\"" + nHeight.ToString() + "\" width=\"" + nWidth.ToString() + "\"></td>");
                                App.decodeFiles.Add(tempDetectResult.resultFileName);
                            }
                            else
                            {
                                htmlWriter.WriteLine("<td>Null</td>");
                            }

                            htmlWriter.WriteLine("<td>" + tempDetectResult.result.BarcodeFormat.ToString() + "</td>");

                            if (tempDetectResult.result.BarcodeFormat != BarcodeFormat.RSS_14)
                            {
                                ParsedResult res = ResultParser.parseResult(tempDetectResult.result);
                                htmlWriter.WriteLine("<td>" + res.Type.ToString() + "</td>");
                                if (res.Type == ParsedResultType.URI)
                                {
                                    htmlWriter.WriteLine("<td>  <a href=" + tempDetectResult.result.ToString() + ">" + tempDetectResult.result.ToString() + "</a> </td>");
                                }
                                else
                                {
                                    htmlWriter.WriteLine("<td>" + tempDetectResult.result.ToString() + "</td>");
                                }
                            }
                            else
                            {
                                htmlWriter.WriteLine("<td>N/A</td>");
                                htmlWriter.WriteLine("<td>" + tempDetectResult.result.ToString() + "</td>");
                            }
                        }
                        else
                        {
                            if (tempDetectResult.resultFileName != null)
                            {
                                int nWidth = tempDetectResult.resultWidth;
                                int nHeight = tempDetectResult.resultHeight;

                                if (nWidth > nHeight)
                                {
                                    if (nWidth > 100)
                                    {
                                        nHeight = 100 * nHeight / nWidth;
                                        nWidth = 100;
                                    }
                                }
                                else
                                {
                                    if (nHeight > 100)
                                    {
                                        nWidth = 100 * nWidth / nHeight;
                                        nHeight = 100;
                                    }
                                }

                                htmlWriter.WriteLine("<td>" + "<img src=\"" + tempDetectResult.resultFileName + "\" height=\"" + nHeight.ToString() + "\" width=\"" + nWidth.ToString() + "\"></td>");
                                App.decodeFiles.Add(tempDetectResult.resultFileName);
                            }
                            else
                            {
                                htmlWriter.WriteLine("<td>Null</td>");
                            }

                            htmlWriter.WriteLine("<td>" + tempDetectResult.barcodeEncodeType + "</td>");
                            htmlWriter.WriteLine("<td>" + tempDetectResult.barcodeResultType + "</td>");
                            if (tempDetectResult.barcodeResultType == "URI")
                            {
                                htmlWriter.WriteLine("<td>  <a href=" + tempDetectResult.barcodeResult.ToString() + ">" + tempDetectResult.barcodeResult.ToString() + "</a> </td>");
                            }
                            else
                            {
                                htmlWriter.WriteLine("<td>" + tempDetectResult.barcodeResult + "</td>");
                            }
                        }

                        htmlWriter.WriteLine("</tr>");
                    }

                    tempArray.Clear();
                    szFileSameName = detectResult.fileName;
                    tempArray.Add(detectResult);
                }

                htmlWriter.WriteLine("</body>");
                htmlWriter.WriteLine("</html>");

                htmlWriter.Close();

                System.Diagnostics.Process.Start(htmlFileName);
                App.decodeFiles.Add(htmlFileName);

                nRet = 0;
            }

            return nRet;
        }

    }
}
