using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Collections.Generic;
using System.Windows.Interop; // for HwndSource
using System.Threading;
using Microsoft.Win32; // for SaveFileDialog
using PdfEncoderClient;
using VOP.Controls;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Printing;
using System.Windows.Threading;
using System.Drawing.Printing;

namespace VOP
{
    public partial class ImageStatus
    {
        public ImageStatus()
        {
        }

        public ScanFiles _files;
        public int m_num = 0;

        public ImageStatus(ScanFiles files, int num)
        {
            m_num = num;
            _files = files;
        }

    }
    public partial class ScanPage_Rufous : UserControl
    {
        private int m_maxImgNum = 99;
        public Thread scanningThread = null;
        private int m_selectedPage = 0;
        private int m_pageCount = 1;

        private List<ScanFiles> scanFileList = null;
        private List<ImageStatus> selectedFileList = null;

        public List<ScanFiles> ScanFileList
        {
            set
            {
                scanFileList = value;

                if(scanFileList != null)
                {
                    selectedFileList = new List<ImageStatus>();

                    //foreach (ScanFiles files in scanFileList)
                    for(int i= scanFileList.Count-1; i>=0; i--)
                    {
                        ImageStatus newImage = new ImageStatus();
                        newImage._files = scanFileList[i];
                        newImage.m_num = scanFileList.IndexOf(scanFileList[i]) + 1;
                        selectedFileList.Add(newImage);
                        UpdateSelItemNum();
                    }                    

                    m_pageCount = scanFileList.Count / 8;
                    if (scanFileList.Count % 8 > 0)
                        m_pageCount += 1;

                    UpdateImageFiles();

                    //foreach (ScanFiles files in scanFileList)
                    //{
                    //    ImageItem newImage = new ImageItem();
                    //    newImage.m_images = files;
                    //    newImage.ImageSingleClick += ImageItemSingleClick;
                    //    newImage.ImageDoubleClick += ImageItemDoubleClick;
                    //    newImage.CloseIconClick += ImageItemCloseIconClick;
                    //    newImage.m_num = scanFileList.IndexOf(files) + 1;
                    //    UpdateSelItemNum();
                    //    newImage.Margin = new Thickness(10);
                    //    this.image_wrappanel.Children.Insert(0, newImage);
                    //}

                    if (scanFileList.Count > 8)
                    {
                        RightBtn.IsEnabled = true;
                    }
                }
            }
        }

        public bool UpdateImageFiles()
        {
            this.image_wrappanel.Children.Clear();

            List<ImageStatus> list = new List<ImageStatus>();

            for(int i= 0; i<8 &&(m_selectedPage * 8 + i)<selectedFileList.Count ; i++)
            {
                list.Add(selectedFileList[m_selectedPage * 8 + i]);
            }           

            for(int i=0; i<list.Count;i++)
            {
                ImageItem newImage = new ImageItem();
                newImage.m_images = list[i]._files;
                newImage.ImageSingleClick += ImageItemSingleClick;
                newImage.ImageDoubleClick += ImageItemDoubleClick;
                newImage.CloseIconClick += ImageItemCloseIconClick;   
                newImage.m_num = list[i].m_num;              
                newImage.Margin = new Thickness(10);
                this.image_wrappanel.Children.Insert(i, newImage);
            }
            UpdateImageOrder();
            return true;
        }

        public ScanPage_Rufous()
        {
            InitializeComponent();

            LeftBtn.IsEnabled = false;
            RightBtn.IsEnabled = false;
        }


        private void LeftButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)//RoutedEventArgs e)
        {
            m_selectedPage--;
            if (m_selectedPage == 0)
            {
                LeftBtn.IsEnabled = false;
                RightBtn.IsEnabled = true;
            }
            else
            {
                if (m_selectedPage < (m_pageCount - 1))
                {
                    RightBtn.IsEnabled = true;
                }
                LeftBtn.IsEnabled = true;
            }
            UpdateImageFiles();
        }

        private void RightButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)//RoutedEventArgs e)
        {
            m_selectedPage++;
            if (m_selectedPage >= (m_pageCount-1))
            {
                RightBtn.IsEnabled = false;
                LeftBtn.IsEnabled = true;
            }
            else
            {
                RightBtn.IsEnabled = true;
                if (m_selectedPage > 0)
                    LeftBtn.IsEnabled = true;   
            }
            UpdateImageFiles();
        }

        private void ScanToAPButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton2 btn = sender as ImageButton2;

            List<string> files = new List<string>();
            if (MainWindow_Rufous.g_settingData.m_commonScanSettings.ADFMode == true &&
                scanFileList.Count>=2)
            {
                if (GetSelectedItemCount() <= 2)
                {
                    GetSelectedFile(files);
                }
                else
                {
                    GetSelectedFileToAP(files, 2);
                    SelectTwoFiles(2, files);
                }
            }
            else
            {
                if (GetSelectedItemCount() <= 2)
                {
                    GetSelectedFile(files);
                }
                else
                {
                    GetSelectedFileToAP(files, 1);
                    SelectTwoFiles(1, files);
                }
            }

            SelectAllCheckBox.IsChecked = false;

            APFlow flow = new APFlow();
            flow.ParentWin = m_MainWin;
            flow.FileList = files;
            APFlow.FlowType = APFlowType.View;
            flow.Run();

        }

        private void ScanToFileButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton2 btn = sender as ImageButton2;

            List<string> files = new List<string>();
            GetSelectedFile(files);

            FileFlow flow = new FileFlow();
            flow.ParentWin = m_MainWin;
            flow.FileList = files;
            FileFlow.FlowType = FileFlowType.View;
            flow.Run();

        }

        private void ScanToEmailButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton2 btn = sender as ImageButton2;

            List<string> files = new List<string>();
            GetSelectedFile(files);

            EmailFlow flow = new EmailFlow();
            flow.ParentWin = m_MainWin;
            flow.FileList = files;
            EmailFlow.FlowType = EmailFlowType.View;
            flow.Run();

        }

        private void ScanToPrintButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(MainWindow_Rufous.g_settingData.m_printerName == null)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                 Application.Current.MainWindow,
                                "Not find printer!" ,
                                (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
            }
            else
            {
                ImageButton2 btn = sender as ImageButton2;

                List<string> files = new List<string>();
                GetSelectedFile(files);

                List<string> listPrinters = new List<string>();
                PrinterSettings settings = new PrinterSettings();
                string strDefaultPrinter = string.Empty;
                PrintServer myPrintServer = new PrintServer(null);
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    PrintDriver queuedrv = pq.QueueDriver;

                    listPrinters.Add(pq.Name);

                    settings.PrinterName = pq.Name;

                    if (settings.IsDefaultPrinter)
                    {
                        strDefaultPrinter = pq.Name;
                    }
                }

                if(listPrinters.Count < 1)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                     Application.Current.MainWindow,
                                    "Not find printer!",
                                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Error"));
                }
                else
                { 
                    PrintFlow flow = new PrintFlow(strDefaultPrinter);
                    flow.ParentWin = m_MainWin;
                    flow.FileList = files;
                    PrintFlow.FlowType = PrintFlowType.View;
                    flow.Run();
                }
            }
        }

        private void ScanToFtpButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!m_MainWin.scanDevicePage.IsOnLine())
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
            Application.Current.MainWindow,
           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Network_fail"),
           (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
            );

                return;
            }

            ImageButton2 btn = sender as ImageButton2;

            List<string> files = new List<string>();
            GetSelectedFile(files);

            FtpFlow flow = new FtpFlow();
            flow.ParentWin = m_MainWin;
            flow.FileList = files;
            FtpFlow.FlowType = FtpFlowType.View;
            flow.Run();

        }

        private void ScanToCloudButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!m_MainWin.scanDevicePage.IsOnLine())
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
            Application.Current.MainWindow,
           (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_Network_fail"),
           (string)Application.Current.MainWindow.TryFindResource("ResStr_Error")
            );

                return;
            }
            ImageButton2 btn = sender as ImageButton2;

            List<string> files = new List<string>();
            GetSelectedFile(files);

            if (MainWindow_Rufous.g_settingData.m_couldSaveType == "DropBox")
            {
                DropBoxFlow flow = new DropBoxFlow();
                flow.ParentWin = m_MainWin;
                flow.FileList = files;
                DropBoxFlow.FlowType = CloudFlowType.View;
                flow.Run();
            }
            else if (MainWindow_Rufous.g_settingData.m_couldSaveType == "EverNote")
            {
                EvernoteFlow flow = new EvernoteFlow();
                flow.ParentWin = m_MainWin;
                flow.FileList = files;
                EvernoteFlow.FlowType = CloudFlowType.View;
                flow.Run();
            }
            else if (MainWindow_Rufous.g_settingData.m_couldSaveType == "OneDrive")
            {
                OneDriveFlow flow = new OneDriveFlow();
                flow.ParentWin = m_MainWin;
                flow.FileList = files;
                OneDriveFlow.FlowType = CloudFlowType.View;
                flow.Run();
            }
        }

        private void ImageItemCloseIconClick(object sender, RoutedEventArgs e)
        {
            if (VOP.Controls.MessageBoxExResult.Yes ==
                    VOP.Controls.MessageBoxEx.Show(
                        VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                        m_MainWin,
                        (string)this.TryFindResource("ResStr_Are_you_sure_to_delete_the_selected_picture"),
                        (string)this.TryFindResource("ResStr_Prompt")
                        )
               )
            {
                ImageItem img = (ImageItem)sender;

                int index = 0;
                foreach ( object obj in image_wrappanel.Children )
                {
                    if ( obj == img )
                    {
                        //modified by yunying shang 2017-12-01 for BMS 1648
                        foreach (ImageStatus img1 in selectedFileList)
                        {
                            if (img1._files == img.m_images)
                            {
                                selectedFileList.Remove(img1);
                                break;
                            }
                        }

                        image_wrappanel.Children.RemoveAt( index );
                        //<<============1648
                        break;
                    }
                    index++;
                }
            }
            //add by yunying shang 2017-10-19 for BMS 1178
            //if (image_wrappanel.Children.Count == GetSelectedItemCount())
            if(selectedFileList.Count == GetSelectedItemCount())
                SelectAllCheckBox.IsChecked = true;
            else
                SelectAllCheckBox.IsChecked = false;
            //<<===================1178
        }

        private void ImageItemSingleClick(object sender, RoutedEventArgs e)
        {
            ImageItem img = (ImageItem)sender;

            if ( 0 == img.m_num )
            {
                if ( GetSelectedItemCount() < m_maxImgNum )
                {
                    img.m_num = GetSelectedItemCount()+1;
                    for (int i = 0; i < selectedFileList.Count; i++)
                    {
                        if (selectedFileList[i]._files == img.m_images)
                        {
                            selectedFileList[i].m_num = img.m_num;
                            break;
                        }
                    }
                }
            }
            else
            {
                for(int i=0; i<selectedFileList.Count;i++)
                {
                    if (selectedFileList[i]._files == img.m_images)
                    {
                        selectedFileList[i].m_num = 0;
                        break;
                    }
                }
                img.m_num = 0;
            }
            
            UpdateSelItemNum();

            //if(image_wrappanel.Children.Count == GetSelectedItemCount())
            if(selectedFileList.Count == GetSelectedItemCount())//modified by yunying shang 2017-11-08 for BMS 1319
               SelectAllCheckBox.IsChecked = true;
            else
               SelectAllCheckBox.IsChecked = false;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            SelectAll((bool)SelectAllCheckBox.IsChecked);
        }

        private void SelectAll(bool isCheck)
        {
            int index = selectedFileList.Count - m_selectedPage*8;

            for (int i = 0; i < selectedFileList.Count; i++)
            {
                if (isCheck)
                    selectedFileList[i].m_num = selectedFileList.Count - i;
                else
                    selectedFileList[i].m_num = 0;
            }
            

            for (int i = 0; i < image_wrappanel.Children.Count; i++)
            {
                ImageItem img = image_wrappanel.Children[i] as ImageItem;

                if (isCheck)
                {
                    img.m_num = index;
                }
                else
                {   
                    img.m_num = 0;
                }

                index--;
            }
        }

        private void SelectTwoFiles(int selCount, List<string> files)
        {
            int j = 0;     
            foreach (ImageStatus img in selectedFileList)
            {
                int i = 0;
                for ( i = 0; i < selCount; i++)
                {
                    if (img._files.m_pathOrig == files[i])
                    {
                        selectedFileList[j].m_num = selCount-i;
                        break;
                    }
                }
                if (i >= selCount)
                    selectedFileList[j].m_num = 0;
                j++;
            }


            foreach (ImageItem img in image_wrappanel.Children)
            {
                int i = 0;
                for (i = 0; i < selCount; i++)
                {
                    if (img.m_images.m_pathOrig == files[i])
                    {
                        img.m_num = selCount-i;
                        break;
                    }
                }

                if(i >= selCount)
                    img.m_num = 0;                
            }
        }

        private void ImageItemDoubleClick(object sender, RoutedEventArgs e)
        {
            ImageItem img = (ImageItem)sender;

            if (0 == img.m_num && GetSelectedItemCount() < m_maxImgNum)
            {
                img.m_num = GetSelectedItemCount() + 1;
                //modified by yunying shang 2017-11-22 for BMS 1509
                for (int i = 0; i < selectedFileList.Count; i++)
                {
                    if (selectedFileList[i]._files == img.m_images)
                    {
                        selectedFileList[i].m_num = img.m_num;
                        break;
                    }
                }//<<================1509
            }

            //add by yunying shang 2017-10-19 for BMS 1182
            //if (image_wrappanel.Children.Count == GetSelectedItemCount())
            if(scanFileList.Count == GetSelectedItemCount())
                SelectAllCheckBox.IsChecked = true;
            else
                SelectAllCheckBox.IsChecked = false;
            //<<=======================1182

            List<string> selectfiles = new List<string>();
            GetSelectedFile(selectfiles);

            ScanPreview win = new ScanPreview();
            win.Owner       = m_MainWin;
            win.m_images    = img.m_images;
            win.isPrint     = false;
            win.selectFileList = selectfiles;
            win.ShowDialog();

            if ( true == win.isPrint )
            {
                List<string> files = new List<string>();

                if ( 0 != win.m_rotatedAngle && null != win.m_rotatedObj )
                    files.Add( win.m_rotatedObj.m_pathOrig );
                else
                    files.Add( img.m_images.m_pathOrig );

               // m_MainWin.SwitchToPrintingPage( files );
            }

            if ( 0 != win.m_rotatedAngle && null != win.m_rotatedObj )
            {
                int index = -1;

                for (int i = 0; i < image_wrappanel.Children.Count; i++)
                {
                    if (image_wrappanel.Children[i] == img)
                    {
                        index = i;
                        break;
                    }
                }

                if ( -1 != index )
                {
                    ImageItem tmp  = new ImageItem();
                    tmp.m_images = win.m_rotatedObj;

                    if ( tmp.m_iSimgReady )
                    {
                        //modified by yunying 2017-11-17 for BMS 
                        int imgIndex = scanFileList.IndexOf(img.m_images);
                        this.image_wrappanel.Children.RemoveAt( index );
                        this.selectedFileList.RemoveAt(imgIndex);
                        this.scanFileList.RemoveAt(imgIndex);

                        // Collect the rubbish files.
                        App.rubbishFiles.Add( img.m_images );

                        tmp.ImageSingleClick += ImageItemSingleClick;
                        tmp.ImageDoubleClick += ImageItemDoubleClick;
                        tmp.CloseIconClick += ImageItemCloseIconClick;

                        if (GetSelectedItemCount() < m_maxImgNum)
                        {
                            tmp.m_num = GetSelectedItemCount() + 1;
                        }
                        else
                        {
                            tmp.m_num = 0;
                        }

                        tmp.Margin = new Thickness( 5 );
                        this.image_wrappanel.Children.Insert(index, tmp );
                        this.scanFileList.Insert(imgIndex, tmp.m_images );

                        ImageStatus newImage = new ImageStatus();
                        newImage._files = tmp.m_images;
                        newImage.m_num = tmp.m_num;

                        this.selectedFileList.Insert(imgIndex, newImage);
                    }
                }
            }
        }


        ///<summary>
        /// Pointer to the MainWindow, in order to use global data more
        /// conveniently 
        ///</summary>
        private MainWindow_Rufous _MainWin = null;
        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if ( null == _MainWin )
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitFontSize();

            SelectAllCheckBox.IsChecked = true;
            SelectAll(true);

            //add by yunying shang 2017-11-29 for BMS 1561
            if (GetSelectedItemCount() > 8)
            {
                LeftBtn.IsEnabled = false;
                RightBtn.IsEnabled = true;
            }
            else
            {
                LeftBtn.IsEnabled = false;
                RightBtn.IsEnabled = false;
            }//<<===============1561
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // zh-CN
            {
                //btnPrint.FontSize = btnSave.FontSize = btnSetting.FontSize = btnScan.FontSize = 17.87;
                //txtBlk_ScannedImageSize.FontSize = txtBlkImgSize.FontSize = txtProgressLabel.FontSize = 14; 
            }
            else
            {
                //btnPrint.FontSize = btnSave.FontSize = btnSetting.FontSize = btnScan.FontSize = 14;
            }
        }

        /// <summary>
        /// Get the file paths of selected image item.
        /// </summary>
        /// <param name="files"> Container used to store the file name.  </param>
        private void GetSelectedFile( List<string> files )
        {
            files.Clear();

            int nCount = GetSelectedItemCount();

            if ( nCount > 0 )
            {
                files.AddRange( new string[nCount] );

                //foreach (object obj in image_wrappanel.Children)
                //{
                //    ImageItem img = obj as ImageItem;

                //    if ( null != img && 0 < img.m_num )
                //    {
                //        files[img.m_num-1] = img.m_images.m_pathOrig;
                //    }
                //}  
                foreach (ImageStatus img in selectedFileList)
                {
                    if (0 < img.m_num)
                    {
                        files[img.m_num-1] = img._files.m_pathOrig;
                    }
                }
            }

            if (files == null || files.Count == 0)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                      Application.Current.MainWindow,
                     "Please select one or more pictures to process",
                     (string)this.TryFindResource("ResStr_Prompt"));
            }
        }

        /// <summary>
        /// Get the file paths of selected image item for scan to AP.
        /// </summary>
        /// <param name="files"> Container used to store the file name.  </param>
        private void GetSelectedFileToAP(List<string> files, int count)
        {
            files.Clear();

            int nCount = GetSelectedItemCount();

            if (nCount > 0 && nCount >= count)
            {
                files.AddRange(new string[count]);

                int j = 0;
                for(int i=0; i<nCount;i++)
                {
                    ImageStatus img = selectedFileList[i];

                    if (0 < img.m_num && (img.m_num > (nCount - count)))
                    {
                        img = selectedFileList[i];
                        files[j] = img._files.m_pathOrig;
                        j++;
                        if (j >= count)
                            break;
                    }
                }
            }

            if (files == null || files.Count == 0)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                      Application.Current.MainWindow,
                     "Please select one or more pictures to process",
                     (string)this.TryFindResource("ResStr_Prompt"));
            }
        }


        /// <summary>
        /// Update the number of selected image items.
        /// </summary>
        private void UpdateSelItemNum()
        {
            List<int> lst = new List<int>();

            //for ( int i=0; i<image_wrappanel.Children.Count; i++ )
            //{
            //    ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

            //    if ( null != img1 && 0 < img1.m_num )
            //    {
            //        lst.Add( img1.m_num );
            //    }
            //}

            //lst.Sort();

            //int nMiss = 0;
            //for ( int i=0; i<lst.Count; i++ )
            //{
            //    if ( lst[i] != i+1 )
            //    {
            //        nMiss = i+1;
            //        break;
            //    }
            //}

            //if ( nMiss > 0 )
            //{
            //    for ( int i=0; i<image_wrappanel.Children.Count; i++ )
            //    {
            //        ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

            //        if ( null != img1 && 0 < img1.m_num && nMiss <= img1.m_num )
            //        {
            //            img1.m_num--; 
            //        }
            //    }
            //}


            for (int i = 0; i < selectedFileList.Count; i++)
            {
                ImageStatus item = selectedFileList[i] as ImageStatus;

                if (0 < item.m_num)
                {
                    lst.Add(item.m_num);
                }
            }

            lst.Sort();

            int nMiss = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i] != i + 1)
                {
                    nMiss = i + 1;
                    break;
                }
            }

            if (nMiss > 0)
            {
                for (int i = 0; i < selectedFileList.Count; i++)
                {
                    ImageStatus item = selectedFileList[i] as ImageStatus;

                    if (0 < item.m_num && nMiss <= item.m_num)
                    {
                        item.m_num--;
                    }
                }
            }

            if (image_wrappanel.Children.Count > 0)
            {
                UpdateImageOrder();
            }
        }

        private void UpdateImageOrder()
        {
            for (int i = 0; i < image_wrappanel.Children.Count; i++)
            {
                ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

                if (null != img1 && 0 < img1.m_num)
                {
                    img1.m_num = selectedFileList[m_selectedPage * 8 + i].m_num;
                }
            }
        }

        /// <summary>
        /// Get the amount of selected items.
        /// </summary>
        private int GetSelectedItemCount()
        {
            int nCount = 0;
            //foreach ( object obj in image_wrappanel.Children )
            //{
            //    ImageItem img = obj as ImageItem;

            //    if ( null != img && 0 < img.m_num )
            //        nCount++;
            //}
            foreach(ImageStatus img in selectedFileList)
            {
                if (img.m_num > 0)
                    nCount++;
            }

            return nCount;
        }


        /// <summary>
        /// Get free space of disk specified by strPath.
        /// </summary>
        /// <param name="strPath"> Path of folder or file. </param>
        private bool DoseHasEnoughSpace( string strPath )
        {
            bool bIsHasEnoughSpace = true;

            if ( strPath.Length >= 3 )
            {
                string strDisk = strPath.Substring(0, 3);
                long lFreeSpace = GetTotalFreeSpace( strDisk );

                long lNeedSpace = 0;

                foreach (object obj in image_wrappanel.Children)
                {
                    ImageItem img = obj as ImageItem;

                    if ( null != img && 0 < img.m_num )
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo( img.m_images.m_pathOrig );
                        lNeedSpace += fi.Length;
                    }
                }

                bIsHasEnoughSpace = lFreeSpace > lNeedSpace;
            }

            return bIsHasEnoughSpace;
        }

        private bool IsTempImageExist()
        {
            foreach (object obj in image_wrappanel.Children)
            {
                ImageItem img = obj as ImageItem;

                if (null != img && 0 < img.m_num)
                {
                    if (!File.Exists(img.m_images.m_pathOrig))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Format of parameter driveName: "X:\". Case insensitive.
        /// </summary>
        private long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName &&
                        string.Compare( drive.Name, driveName, StringComparison.OrdinalIgnoreCase) == 0 )
                {
                    return drive.TotalFreeSpace;
                }
            }

            return -1;
        }

        private void Button_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)//RoutedEventArgs e)
        {

            if(image_wrappanel.Children.Count > 0)
            {
                if (VOP.Controls.MessageBoxExResult.Yes ==
                 VOP.Controls.MessageBoxEx.Show(
                     VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                     m_MainWin,
                     "Do you want to delete all images before leaving this page?",
                     (string)this.TryFindResource("ResStr_Prompt")
                     ))
                {

                    image_wrappanel.Children.Clear();
                    m_MainWin.GotoPage("ScanSelectionPage", null);
                }
            }
            else
            {
                m_MainWin.GotoPage("ScanSelectionPage", null);
            }
  
        }
 
    }
}
