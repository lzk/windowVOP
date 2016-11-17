using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.IO;
using System.Threading;
using System.Security.Authentication;
using System.Net;
using VOP.Controls;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Outlook = Microsoft.Office.Interop.Outlook;
using PdfEncoderClient;
using Microsoft.Win32;

namespace VOP
{
    public enum FileFlowType
    {
        View,
        Quick,
    };

    public class FileFlow
    {
        public static FileFlowType FlowType = FileFlowType.View;
        public List<string> FileList { get; set; }
        public Window ParentWin { get; set; }

        string m_errorMsg = "";
        ScanFileSaveError fileSaveStatus = ScanFileSaveError.FileSave_OK;

        private MessageBoxEx_Simple_Busy_QRCode qr_pbw = null;
        private ManualResetEvent asyncEvent = new ManualResetEvent(false);
        private bool isNeededProgress = false;

        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
            }


            ScanFileSaveError result = ScanFileSaveError.FileSave_OK;

            if(FlowType == FileFlowType.View)
            {
                result = SaveFileView();
            }
            else
            {
                result = SaveFileQuick();
            }

            if (result == ScanFileSaveError.FileSave_OK)
            {
                if (FlowType == FileFlowType.View)
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                               Application.Current.MainWindow,
                              "Save files completed",
                              "Prompt");
            }
            else if (result == ScanFileSaveError.FileSave_Cancel)
            {

            }
            else
            {
             
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                                "Fail to save files. " + m_errorMsg,
                                "Error");
                return false;
            }

          
            return true;
        }

        ScanFileSaveError SaveFileView()
        {
            SaveFileDialog save = new SaveFileDialog();

            if (FileList.Count > 1)
                save.Filter = "TIF|*.tif|PDF|*.pdf";
            else
                save.Filter = "TIF|*.tif|PDF|*.pdf|JPG|*.jpg";

            bool? result = save.ShowDialog();

            if (result == true)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        if (3 == save.FilterIndex)
                        {
                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                            foreach (string path in FileList)
                            {
                                Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                                JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                BitmapSource origSource = decoder.Frames[0];

                                if (null != origSource)
                                    encoder.Frames.Add(BitmapFrame.Create(origSource));
                            }

                            FileStream fs = File.Open(save.FileName, FileMode.Create);
                            encoder.Save(fs);
                            fs.Close();
                        }
                        else if (1 == save.FilterIndex)
                        {
                            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

                            foreach (string path in FileList)
                            {
                                Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                                JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                BitmapSource origSource = decoder.Frames[0];

                                BitmapMetadata bitmapMetadata = new BitmapMetadata("tiff");
                                bitmapMetadata.ApplicationName = "Virtual Operation Panel";

                                if (null != origSource)
                                    encoder.Frames.Add(BitmapFrame.Create(origSource, null, bitmapMetadata, null));
                            }

                            FileStream fs = File.Open(save.FileName, FileMode.Create);
                            encoder.Save(fs);
                            fs.Close();
                        }
                        else if (2 == save.FilterIndex)
                        {
                            using (PdfHelper help = new PdfHelper())
                            {
                                help.Open(save.FileName);

                                foreach (string path in FileList)
                                {
                                    Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                                    JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                    BitmapSource origSource = decoder.Frames[0];

                                    if (null != origSource)
                                        help.AddImage(origSource, 0);
                                }

                                help.Close();
                            }
                        }
                    }
                    catch (Win32Exception ex)
                    {
                        m_errorMsg = ex.Message;
                        fileSaveStatus = ScanFileSaveError.FileSave_Error;
                    }
                    catch (COMException ex)
                    {
                        m_errorMsg = ex.Message;
                        fileSaveStatus = ScanFileSaveError.FileSave_Error;
                    }
                    catch(Exception ex)
                    {
                        m_errorMsg = ex.Message;
                        fileSaveStatus = ScanFileSaveError.FileSave_Error;
                    }

                    QRCodeCallbackMethod(null);
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = false;
                thread.Start();

                if (!thread.Join(100))
                {
                    isNeededProgress = true;

                    qr_pbw = new MessageBoxEx_Simple_Busy_QRCode("Saving picture to file, please wait...");
                    qr_pbw.Owner = ParentWin;
                    qr_pbw.Loaded += pbw_Loaded;
                    qr_pbw.ShowDialog();
                }

                thread.Join();

                if (fileSaveStatus == ScanFileSaveError.FileSave_Error)
                {
                    return ScanFileSaveError.FileSave_Error;
                }

                return ScanFileSaveError.FileSave_OK;
               
            }
            else
            {
                return ScanFileSaveError.FileSave_Cancel;
            }

        }

        ScanFileSaveError SaveFileQuick()
        {

            if(MainWindow_Rufous.g_settingData.m_fileName == "" 
                || MainWindow_Rufous.g_settingData.m_filePath == "")
            {
                return ScanFileSaveError.FileSave_Error;
            }
           
            Thread thread = new Thread(() =>
            {
                try
                {
                   
                    if (MainWindow_Rufous.g_settingData.m_fileSaveType == "TIFF")
                    {
                        TiffBitmapEncoder encoder = new TiffBitmapEncoder();

                        foreach (string path in FileList)
                        {
                            Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                            JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                            BitmapSource origSource = decoder.Frames[0];

                            BitmapMetadata bitmapMetadata = new BitmapMetadata("tiff");
                            bitmapMetadata.ApplicationName = "Virtual Operation Panel";

                            if (null != origSource)
                                encoder.Frames.Add(BitmapFrame.Create(origSource, null, bitmapMetadata, null));
                        }

                        string fileExt = System.IO.Path.GetExtension(MainWindow_Rufous.g_settingData.m_fileName).ToLower();
                        string savePath = "";
                        if (fileExt != ".tif")
                        {
                            savePath = MainWindow_Rufous.g_settingData.m_filePath + 
                                 @"\" + MainWindow_Rufous.g_settingData.m_fileName + ".tif";
                        }
                        else
                        {
                            savePath = MainWindow_Rufous.g_settingData.m_filePath +
                                @"\" + MainWindow_Rufous.g_settingData.m_fileName;
                        }

                        if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_filePath))
                        {
                            Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_filePath);
                        }

                        FileStream fs = File.Open(savePath, FileMode.Create);
                        encoder.Save(fs);
                        fs.Close();
                    }
                    else if (MainWindow_Rufous.g_settingData.m_fileSaveType == "PDF")
                    {
                        using (PdfHelper help = new PdfHelper())
                        {
                            string fileExt = System.IO.Path.GetExtension(MainWindow_Rufous.g_settingData.m_fileName).ToLower();
                            string savePath = "";
                            if (fileExt != ".pdf")
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_filePath +
                                     @"\" + MainWindow_Rufous.g_settingData.m_fileName + ".pdf";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_filePath +
                                    @"\" + MainWindow_Rufous.g_settingData.m_fileName;
                            }

                            if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_filePath))
                            {
                                Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_filePath);
                            }

                            help.Open(savePath);

                            foreach (string path in FileList)
                            {
                                Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                                JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                BitmapSource origSource = decoder.Frames[0];

                                if (null != origSource)
                                    help.AddImage(origSource, 0);
                            }

                            help.Close();
                        }
                    }
                }
                catch (Win32Exception ex)
                {
                    m_errorMsg = ex.Message;
                    fileSaveStatus = ScanFileSaveError.FileSave_Error;
                }
                catch (COMException ex)
                {
                    m_errorMsg = ex.Message;
                    fileSaveStatus = ScanFileSaveError.FileSave_Error;
                }
                catch (Exception ex)
                {
                    m_errorMsg = ex.Message;
                    fileSaveStatus = ScanFileSaveError.FileSave_Error;
                }

                QRCodeCallbackMethod(null);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = false;
            thread.Start();

            if (!thread.Join(100))
            {
                isNeededProgress = true;

                qr_pbw = new MessageBoxEx_Simple_Busy_QRCode("Saving picture to file, please wait...");
                qr_pbw.Owner = ParentWin;
                qr_pbw.Loaded += pbw_Loaded;
                qr_pbw.ShowDialog();
            }

            thread.Join();

            if (fileSaveStatus == ScanFileSaveError.FileSave_Error)
            {
                return ScanFileSaveError.FileSave_Error;
            }

            return ScanFileSaveError.FileSave_OK;
        }

        private void pbw_Loaded(object sender, RoutedEventArgs e)
        {
            asyncEvent.Set();
        }
        void QRCodeCallbackMethod(IAsyncResult ar)
        {
            if (isNeededProgress)
            {
                asyncEvent.WaitOne();

                if (qr_pbw != null)
                {
                    qr_pbw.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                     new Action(
                     delegate ()
                     {
                         qr_pbw.Close();
                     }
                     ));
                }
            }
        }
    }
}
