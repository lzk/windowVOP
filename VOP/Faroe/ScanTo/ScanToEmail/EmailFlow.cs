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

namespace VOP
{
    public enum EmailFlowType
    {
        View,
        Quick,
    };

    public class EmailFlow
    {
        public static EmailFlowType FlowType = EmailFlowType.View;
        public List<string> FileList { get; set; }
       // List<ScanFiles> files = new List<ScanFiles>();
        public Window ParentWin { get; set; }
        string pdfName = "";
       // string tiffName = "";
        string m_errorMsg = "";

        public bool Run()
        {
            if (FileList == null || FileList.Count == 0)
            {
                return false;
            }

            string AttachmentType = "PDF";

            if (FlowType == EmailFlowType.Quick)
            {
                AttachmentType = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_EmailScanSettings.AttachmentType;
            }
            else
            {
                AttachmentType = MainWindow_Rufous.g_settingData.m_attachmentType;
            }

            if (AttachmentType  == "PDF")
            {
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                if (worker.InvokeQuickScanMethod(SavePdfFile, (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Saving_pic_PDF")))
                {
                    //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                    //               Application.Current.MainWindow,
                    //              "Save completed",
                    //              "Prompt");
                }
                else
                {
                    pdfName = "";
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                    Application.Current.MainWindow,
                                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_save_pdf") + m_errorMsg,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                                    );
                }
                
            }
            else
            {
                //AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

                //if (worker.InvokeQuickScanMethod(SaveTiffFile, (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Saving_pic_TIFF")))
                //{
                //    //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                //    //               Application.Current.MainWindow,
                //    //              "Save completed",
                //    //              "Prompt");
                //}
                //else
                //{
                //    //tiffName = "";
                //    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                //                    Application.Current.MainWindow,
                //                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_save_pdf") + m_errorMsg,
                //                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
                //                    );
                //}
            }


            try
            {
                Outlook.Application oApp = new Outlook.Application();
                Outlook.NameSpace ns = oApp.GetNamespace("MAPI");
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);


                //if (MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_EmailScanSettings.AttachmentType == "PDF")
                if (AttachmentType == "PDF")
                {
                    if (pdfName != "")
                    {
                        //string fileName = System.IO.Path.GetFileName(pdfName);
                        Outlook.Attachment oAttach = oMsg.Attachments.Add(pdfName);
                        ScanFiles file = new ScanFiles();
                        file.m_pathOrig = pdfName;
                        file.m_pathView = pdfName;
                        file.m_pathThumb = pdfName;
                        //files.Add(file);
                        App.scanFileList.Add(file);//#bms1179
                    }
                }
                else
                {

                    foreach (string filePath in FileList)
                    {
                        //string fileName = System.IO.Path.GetFileName(filePath);
                        Outlook.Attachment oAttach = oMsg.Attachments.Add(filePath);
                        ScanFiles file = new ScanFiles();
                        file.m_pathOrig = filePath;
                        file.m_pathView = filePath;
                        file.m_pathThumb = filePath;
                        //files.Add(file);
                        App.scanFileList.Add(file);
                    }

                    //add by yunying shang 2017-11-01 for BMS 1232
                    //if (tiffName != "")
                    //{
                    //    string fileName = System.IO.Path.GetFileName(tiffName);
                    //    Outlook.Attachment oAttach = oMsg.Attachments.Add(tiffName);
                    //    ScanFiles file = new ScanFiles();
                    //    file.m_pathOrig = tiffName;
                    //    file.m_pathView = tiffName;
                    //    file.m_pathThumb = tiffName;
                    //    files.Add(file);
                    //    App.scanFileList.Add(file);
                    //}
                }


                if (FlowType == EmailFlowType.Quick)
                {
                    Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                    Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add((MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_EmailScanSettings.Recipient));
                    oRecip.Resolve();

                    oMsg.Subject = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_EmailScanSettings.Subject;
                    oMsg.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
                    oMsg.HTMLBody = "Attachment are scanned pictures";
                    //oMsg.Display(false);
                    oMsg.Send();
                }
                else
                {
                    oMsg.Display(true);
                }

            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == 0x80040154)
                {
                    string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Not_Find_Outlook");
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                     Application.Current.MainWindow,
                                    (string)message,//"Could not find the outlook, please check you computer!",
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                }
                else
                {
                    //modified by yunying shang 2017-11-23 for BMS 1196
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                     Application.Current.MainWindow,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail") + ex.Message,
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                }

                if (AttachmentType == "PDF")
                {
                    File.Delete(pdfName);
                }
                //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                //                Application.Current.MainWindow,
                //               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail") ,
                //               (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                return false;
            }
            catch (Exception ex)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                  Application.Current.MainWindow,
                                 (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail") + ex.Message,
                                 (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));

                if (AttachmentType == "PDF")
                {
                    File.Delete(pdfName);
                }
                return false;
            }

            //VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
            //                      Application.Current.MainWindow,
            //                     "Send Email completed",
            //                     "Prompt");
            
            return true;
        }

        bool SavePdfFile()
        {
            try
            {
//                string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString("D10");
                string strSuffix = string.Format("{0}{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}", "img", 
                    DateTime.Now.Year, 
                    DateTime.Now.Month, 
                    DateTime.Now.Day, 
                    DateTime.Now.Hour, 
                    DateTime.Now.Minute,
                    DateTime.Now.Second);

                if (false == Directory.Exists(App.PictureFolder))
                {
                    Directory.CreateDirectory(App.PictureFolder);
                }

                pdfName = App.PictureFolder + "\\vop" + strSuffix + ".pdf";

                using (PdfHelper help = new PdfHelper())
                {
                    help.Open(pdfName);

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
            catch (Win32Exception ex)
            {
                //                m_errorMsg = ex.Message;
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                                  Application.Current.MainWindow,
                                                 (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail"),
                                                 (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                return false;
            }
            catch (Exception ex)
            {
                
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                                 Application.Current.MainWindow,
                                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail"),
                                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                //               m_errorMsg = ex.Message;
                return false;
            }
            return true;
        }

        //bool SaveTiffFile()
        //{
        //    try
        //    {
        //        //                string strSuffix = (Environment.TickCount & Int32.MaxValue).ToString("D10");
        //        string strSuffix = string.Format("{0}{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}", "img",
        //            DateTime.Now.Year,
        //            DateTime.Now.Month,
        //            DateTime.Now.Day, 
        //            DateTime.Now.Hour, 
        //            DateTime.Now.Minute,
        //            DateTime.Now.Second);
        //        if (false == Directory.Exists(App.PictureFolder))
        //        {
        //            Directory.CreateDirectory(App.PictureFolder);
        //        }

        //        tiffName = App.PictureFolder + "\\vop" + strSuffix + ".tif";

        //        TiffBitmapEncoder encoder = new TiffBitmapEncoder();

        //        foreach (string path in FileList)
        //        {
        //            Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
        //            JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
        //            BitmapSource origSource = decoder.Frames[0];

        //            BitmapMetadata bitmapMetadata = new BitmapMetadata("tiff");
        //            bitmapMetadata.ApplicationName = "Virtual Operation Panel";

        //            if (null != origSource)
        //                encoder.Frames.Add(BitmapFrame.Create(origSource, null, bitmapMetadata, null));
        //        }

        //        FileStream fs = File.Open(tiffName, FileMode.Create);
        //        encoder.Save(fs);
        //        fs.Close();

        //    }
        //    catch (Win32Exception ex)
        //    {
        //        //                m_errorMsg = ex.Message;
        //        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
        //                                          Application.Current.MainWindow,
        //                                         (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail"),
        //                                         (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {

        //        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
        //                                         Application.Current.MainWindow,
        //                                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_send_mail_fail"),
        //                                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
        //        //               m_errorMsg = ex.Message;
        //        return false;
        //    }
        //    return true;
        //}
    }
}
