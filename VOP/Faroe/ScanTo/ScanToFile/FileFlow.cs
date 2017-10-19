﻿using System;
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
using System.Drawing;

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
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_save_file_ok"),
                              (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt")
                              );
            }
            else if (result == ScanFileSaveError.FileSave_Cancel)
            {

            }
            else
            {
             
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_save") + m_errorMsg,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                return false;
            }

          
            return true;
        }

        ScanFileSaveError SaveFileView()
        {
            SaveFileDialog save = new SaveFileDialog();
            int MAX_FILE_NAME_CHARS_NUMBER = 25;
            int MAX_PATH = 260;

            //if (FileList.Count > 1)
            //   save.Filter = "TIF|*.tif|PDF|*.pdf";
            //else
                save.Filter = "TIF|*.tif|PDF|*.pdf|JPG|*.jpg|BMP|*.bmp";

            bool? result = save.ShowDialog();

            if (MAX_PATH <= (save.FileName.Length + 1 + MAX_FILE_NAME_CHARS_NUMBER))//
            {
                result = false;
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_save") + m_errorMsg,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
            }
            else
                result = true;

            if (result == true)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        //modified by yunying shang 2017-10-16
                        if (3 == save.FilterIndex)
                        {
                            int i = 1;                  
                            foreach (string path in FileList)
                            {
                                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                                Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                                JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                BitmapSource origSource = decoder.Frames[0];

                                if (null != origSource)
                                    encoder.Frames.Add(BitmapFrame.Create(origSource));

                                string filename = save.FileName;
                                if (FileList.Count > 1)
                                {
                                    filename = System.IO.Path.GetDirectoryName(save.FileName) +"\\" +
                                    System.IO.Path.GetFileNameWithoutExtension(save.FileName) + Convert.ToString(i) + ".jpg";
                                }
                                FileStream fs = File.Open(filename, FileMode.Create);
                                encoder.Save(fs);
                                fs.Close();
                                i++;
                            }
                            
                        }//<<=================================
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
                        //add by yunying shang 2017-10-16
                        else if (4 == save.FilterIndex)
                        {
                            int i = 1;
                            foreach (string path in FileList)
                            {
                                using (Bitmap source = new Bitmap(path))
                                {
                                    using (Bitmap bmp = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                                    {
                                        Graphics.FromImage(bmp).DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height));

                                        string filename = save.FileName;
                                        if (FileList.Count > 1)
                                        {
                                            filename = System.IO.Path.GetDirectoryName(save.FileName) + "\\" +
                                            System.IO.Path.GetFileNameWithoutExtension(save.FileName) + Convert.ToString(i) + ".bmp";
                                        }

                                        bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                                    }
                                    i++;
                                }
                            }
                        }//==========================
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

                    qr_pbw = new MessageBoxEx_Simple_Busy_QRCode((string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Saving_pic_file"));
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
            if(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName == "" 
                || MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath == "")
            {
                return ScanFileSaveError.FileSave_Error;
            }
           
            Thread thread = new Thread(() =>
            {
                try
                {
                    string time = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, 
                        DateTime.Now.Month,
                        DateTime.Now.Day, 
                        DateTime.Now.Hour,
                        DateTime.Now.Minute,
                        DateTime.Now.Second);

                    if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.SaveType == "TIFF")
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

                        string fileExt = System.IO.Path.GetExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName).ToLower();
                        string savePath = "";
                        if (fileExt != ".tif")
                        {
                            savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath + 
                                 @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName + time +  ".tif";
                        }
                        else
                        {
                            savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName)
                                + time  + ".tif";
                        }

                        if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath))
                        {
                            Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
                        }

                        FileStream fs = File.Open(savePath, FileMode.Create);
                        encoder.Save(fs);
                        fs.Close();
                    }
                    else if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.SaveType == "PDF")
                    {
                        using (PdfHelper help = new PdfHelper())
                        {
                            string fileExt = System.IO.Path.GetExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName).ToLower();
                            string savePath = "";
                            if (fileExt != ".pdf")
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                     @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName + time +".pdf";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                    @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName)
                                    + time + ".pdf";
                            }

                            if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath))
                            {
                                Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
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
                    //add by yunying shang
                    if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.SaveType == "JPG")
                    {
                        if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath))
                        {
                            Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
                        }

                        string fileExt = System.IO.Path.GetExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName).ToLower();
                        string savePath = "";

                        int i = 1;
                        
                        foreach (string path in FileList)
                        {
                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                            Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                            JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                            BitmapSource origSource = decoder.Frames[0];

                            if (null != origSource)
                                encoder.Frames.Add(BitmapFrame.Create(origSource));

                            if (FileList.Count > 1)
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                     @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName + time + Convert.ToString(i) + ".jpg";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                    @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName)
                                    + time  + ".jpg";
                            }

                            FileStream fs = File.Open(savePath, FileMode.Create);
                            encoder.Save(fs);
                            fs.Close();

                            i++;
                        }                        
                    }
                    if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.SaveType == "BMP")
                    {
                        if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath))
                        {
                            Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
                        }

                        string fileExt = System.IO.Path.GetExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName).ToLower();
                        string savePath = "";

                        int i = 1;

                        foreach (string path in FileList)
                        {
                            if (FileList.Count > 1)
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                     @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName
                                      + time + Convert.ToString(i) + ".bmp";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                    @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName) 
                                     + time + ".bmp";
                            }
                            using (Bitmap source = new Bitmap(path))
                            {
                                using (Bitmap bmp = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                                {
                                    Graphics.FromImage(bmp).DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height));
                                    bmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);
                                }
                            }
                            i++;
                        }
                    }//<<===========================================
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

                qr_pbw = new MessageBoxEx_Simple_Busy_QRCode((string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Saving_pic_file"));
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
