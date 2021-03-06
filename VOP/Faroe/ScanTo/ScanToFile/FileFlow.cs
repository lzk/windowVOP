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
using System.Security.AccessControl;

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

        //add by yunying shang 2018-01-11 for BMS 2040
        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            string str = str_HardDiskName;
            int index = str.LastIndexOf(':');
            str = str.Substring(0, index);

            long totalSize = 0;

            str_HardDiskName = str + ":\\";

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (System.IO.DriveInfo drive in drives)
            {

                if (drive.Name == str_HardDiskName.ToUpper())//modified by yunying 2018-03-05 for BMS 2490
                {
                    totalSize = drive.TotalFreeSpace / (1024 * 1024);
                }
            }

            return totalSize;

        }//<<========================2040

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
                //if (FlowType == FileFlowType.View)
                //    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                //               Application.Current.MainWindow,
                //               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_save_file_ok"),
                //              (string)Application.Current.MainWindow.TryFindResource("ResStr_Prompt")
                //              );
            }
            else if (result == ScanFileSaveError.FileSave_Cancel)
            {
                //Win32.OutputDebugString("Scan to File is canceled!");
            }
            //add by yunying shang 2017-11-20 for BMS 1176
            else if (result == ScanFileSaveError.FileSave_NotAccess)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_You_do_not_have_permission"),
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return false;
            }//<<=================1176
            else if (result == ScanFileSaveError.FileSave_OutOfMemory)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                Application.Current.MainWindow,
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_not_enough"),
                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return false;
            }
            else
            {

                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                Application.Current.MainWindow,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_save") + m_errorMsg,
                               (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
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
            bool? result = false;
            try
            {
               // while (result == false)
                { 
                    result = save.ShowDialog();

                    string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_File_Path_and_Name_too_long");

                    if (MAX_PATH <= (save.FileName.Length + 1 + MAX_FILE_NAME_CHARS_NUMBER))//
                    {
                        result = false;
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                        Application.Current.MainWindow,
                                       (string)message,//"You Specify the file path and file name is too long, please specify again " + m_errorMsg,
                                       (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                    }
                    //modified by yunying shang 2017-10-31 for BMS 1260
                    //marked by yunying shang 2017-10-20 for BMS 1185
                    else
                    {
                       // result = true;
                        if (result == true && (save.FileName == null || save.FileName.Length == 0))
                        {
                            result = false;
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                Application.Current.MainWindow,
                                (string)message,//"You Specify the file path and file name is too long, please specify again!",
                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                        }
                    }//<<=============1185
                    //<<=======================1260
                }
            }
            catch (Exception ex)
            {
                result = false;
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                Application.Current.MainWindow,
               (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Fail_save") + ex.Message,
               (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
            }

            if (result == true)
            {
                //add by yunying shang 2018-01-11 for BMS 2040
                long totalsize = 0;
                foreach (string filePath in FileList)
                {
                    System.IO.FileInfo fileInfo = null;
                    fileInfo = new System.IO.FileInfo(filePath);

                    totalsize += fileInfo.Length;
                }
                totalsize = totalsize / (1024 * 1024);
                long size = GetHardDiskSpace(save.FileName);
                if( totalsize > size || totalsize * 5 > size)
                {
                    return ScanFileSaveError.FileSave_OutOfMemory;
                }

                if (size < 300)
                {
                    return ScanFileSaveError.FileSave_OutOfMemory;
                }

                //<<==================2040
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
                                        filename = System.IO.Path.GetDirectoryName(save.FileName) + "\\" +
                                        System.IO.Path.GetFileNameWithoutExtension(save.FileName) + "_" + Convert.ToString(i) + ".jpg";
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
                                                System.IO.Path.GetFileNameWithoutExtension(save.FileName) + "_" + Convert.ToString(i) + ".bmp";
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

                                //dll.OutputDebugStringToFile_("Open PDF File");

                                foreach (string path in FileList)
                                    {
                                    // dll.OutputDebugStringToFile_("Current JPG file name ");
                                    // dll.OutputDebugStringToFile_(path);
                                    Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
                                        JpegBitmapDecoder decoder = new JpegBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                        BitmapSource origSource = decoder.Frames[0];

                                        if (null != origSource)
                                        {
                                            help.AddImage(origSource, 0);
                                        // dll.OutputDebugStringToFile_("Add Image to PDF File");
                                    }
                                        else
                                        {
                                        // dll.OutputDebugStringToFile_("Image source is null");
                                    }
                                    }

                                    help.Close();
                                //dll.OutputDebugStringToFile_("Close PDF File");
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

                        qr_pbw = new MessageBoxEx_Simple_Busy_QRCode((string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Saving_pic_file"));
                        qr_pbw.Owner = ParentWin;
                        qr_pbw.Loaded += pbw_Loaded;
                        bool? rtn = qr_pbw.ShowDialog();

                        //modified by yunying shang 2018-03-09 for BMS 2607
                        if (rtn != true)
                        {
                            fileSaveStatus = ScanFileSaveError.FileSave_Cancel;
                            return ScanFileSaveError.FileSave_Cancel;
                        }//<<================2607
                    }

                    thread.Join();

                    if (fileSaveStatus == ScanFileSaveError.FileSave_Error)
                    {
                        return ScanFileSaveError.FileSave_Error;
                    }

                    return ScanFileSaveError.FileSave_OK;
                }
               
            }
            else
            {
                return ScanFileSaveError.FileSave_Cancel;
            }

        }

        private bool PathExist(string path)
        {
            int find = path.LastIndexOf(':');
            if (find > 0)
            {
                string str = path.Substring(0, find + 1);
                if (System.IO.Directory.Exists(str))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        ScanFileSaveError SaveFileQuick()
        {
            if(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName == "" 
                || MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath == "")
            {
                return ScanFileSaveError.FileSave_Error;
            }

            string filepath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath;

            //System.Security.Principal.WindowsIdentity wid = System.Security.Principal.WindowsIdentity.GetCurrent();
            //System.Security.Principal.WindowsPrincipal printcipal = new System.Security.Principal.WindowsPrincipal(wid);
            //bool bIsAdmin = printcipal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            if (true == Directory.Exists(filepath))
            {
                //add by yunying shang 2017-11-20 for BMS 1176
                bool bWrite = true;
                try
                {
                    //modified by yunying shang 2017-12-01 for BMS 1554
                    DirectorySecurity sec = Directory.GetAccessControl(filepath, AccessControlSections.Access);//AccessControlSections.All);               
                    AuthorizationRuleCollection rules = sec.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));//NTAccount));
                                                                                                                                             //<<==============1554

                    foreach (FileSystemAccessRule rule in rules)
                    {
                        //modified by yunying shang 2017-12-01 for BMS 1643
                        AccessControlType accessType = rule.AccessControlType;
                        if ((accessType != AccessControlType.Deny))
                        //&& ((rule.FileSystemRights & FileSystemRights.WriteData) != 0))
                        {
                            bWrite = true;
                        }
                        else
                        {
                            bWrite = false;
                            break;
                        }//<<==========1643
                    }
                }
                catch (Exception ex)
                {
                    Win32.OutputDebugString(ex.Message);
                    bWrite = true;
                }
                if (!bWrite)
                {
                    return ScanFileSaveError.FileSave_NotAccess;
                }//<<===============1176

            }

            long totalsize = 0;
            foreach (string filePath in FileList)
            {
                System.IO.FileInfo fileInfo = null;
                fileInfo = new System.IO.FileInfo(filePath);

                totalsize += fileInfo.Length;
            }
            totalsize = totalsize / (1024 * 1024);
            long size = GetHardDiskSpace(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
            if (totalsize > size || totalsize * 5 > size)
            {
                return ScanFileSaveError.FileSave_OutOfMemory;
            }

            if (size < 300)
            {
                return ScanFileSaveError.FileSave_OutOfMemory;
            }
            Thread thread = new Thread(() =>
            {
                try
                {
                    string time = string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}", 
                        DateTime.Now.Year, 
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
                                 @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName + "_" + time +  ".tif";
                        }
                        else
                        {
                            savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName)
                                + "_" + time  + ".tif";
                        }

                        if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath))
                        {
                            Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
                        }

                        if (savePath.Length > 260)
                        {
                            fileSaveStatus = ScanFileSaveError.FileSave_Error;
                        }
                        else
                        { 
                            FileStream fs = File.Open(savePath, FileMode.Create);
                            encoder.Save(fs);
                            fs.Close();
                        }
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
                                     @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName + "_" + time + ".pdf";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                    @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName)
                                    + "_" + time + ".pdf";
                            }

                            if (false == Directory.Exists(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath))
                            {
                                Directory.CreateDirectory(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath);
                            }

                            if (savePath.Length > 260)
                            {
                                fileSaveStatus = ScanFileSaveError.FileSave_Error;
                            }
                            else
                            { 
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
                                     @"\" + MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName + "_" + time + Convert.ToString(i) + ".jpg";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                    @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName)
                                    + "_" + time + ".jpg";
                            }

                            if (savePath.Length > 260)
                            {
                                fileSaveStatus = ScanFileSaveError.FileSave_Error;
                                break;
                            }
                            else
                            { 
                                FileStream fs = File.Open(savePath, FileMode.Create);
                                encoder.Save(fs);
                                fs.Close();
                            }

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
                                      + "_" + time + "_" + Convert.ToString(i) + ".bmp";
                            }
                            else
                            {
                                savePath = MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FilePath +
                                    @"\" + System.IO.Path.GetFileNameWithoutExtension(MainWindow_Rufous.g_settingData.m_MatchList[MainWindow_Rufous.g_settingData.CutNum].m_FileScanSettings.FileName) 
                                     + "_" + time + ".bmp";
                            }
                            if (savePath.Length > 260)
                            {
                                fileSaveStatus = ScanFileSaveError.FileSave_Error;
                                break;
                            }
                            else
                            { 
                                using (Bitmap source = new Bitmap(path))
                                {
                                    using (Bitmap bmp = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                                    {
                                        Graphics.FromImage(bmp).DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height));
                                        bmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);
                                    }
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
