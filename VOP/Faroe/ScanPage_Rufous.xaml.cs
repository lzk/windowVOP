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

namespace VOP
{
    public partial class ScanPage_Rufous : UserControl
    {
        private int m_maxImgNum = 99;
        public Thread scanningThread = null;
        private int m_iMousePress = 0;

        private List<ScanFiles> scanFileList = null; 


        public List<ScanFiles> ScanFileList
        {
            set
            {
                scanFileList = value;

                if(scanFileList != null)
                {
                    foreach (ScanFiles files in scanFileList)
                    {
                        ImageItem newImage = new ImageItem();
                        newImage.m_images = files;
                        newImage.ImageSingleClick += ImageItemSingleClick;
                        newImage.ImageDoubleClick += ImageItemDoubleClick;
                        newImage.CloseIconClick += ImageItemCloseIconClick;
                        newImage.m_num = scanFileList.IndexOf(files) + 1;
                        UpdateSelItemNum();
                        newImage.Margin = new Thickness(10);
                        this.image_wrappanel.Children.Insert(0, newImage);

                    }

                    // AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                    // worker.InvokeQuickScanMethod(UpdateImageFiles, (string)this.TryFindResource("ResStr_Faroe_Uploading_Files"));

                }
            }
        }

        public bool UpdateImageFiles()
        {
            foreach (ScanFiles files in scanFileList)
            {
                ImageItem newImage = new ImageItem();
                newImage.m_images = files;
                newImage.ImageSingleClick += ImageItemSingleClick;
                newImage.ImageDoubleClick += ImageItemDoubleClick;
                newImage.CloseIconClick += ImageItemCloseIconClick;
                newImage.m_num = scanFileList.IndexOf(files) + 1;
                UpdateSelItemNum();
                newImage.Margin = new Thickness(10);
                this.image_wrappanel.Children.Insert(0, newImage);

            }
            return true;
        }
        public ScanPage_Rufous()
        {
            InitializeComponent();
        }

        private void ScanToAPButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

            List<string> files = new List<string>();
            GetSelectedFile(files);

            APFlow flow = new APFlow();
            flow.ParentWin = m_MainWin;
            flow.FileList = files;
            APFlow.FlowType = APFlowType.View;
            flow.Run();

        }

        private void ScanToFileButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

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
            ImageButton btn = sender as ImageButton;

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
                ImageButton btn = sender as ImageButton;

                List<string> files = new List<string>();
                GetSelectedFile(files);

                List<string> listPrinters = new List<string>();
                PrintServer myPrintServer = new PrintServer(null);
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    PrintDriver queuedrv = pq.QueueDriver;

                    listPrinters.Add(pq.Name);
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
                    PrintFlow flow = new PrintFlow();
                    flow.ParentWin = m_MainWin;
                    flow.FileList = files;
                    PrintFlow.FlowType = PrintFlowType.View;
                    flow.Run();
                }
            }
        }

        private void ScanToFtpButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ImageButton btn = sender as ImageButton;

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
            ImageButton btn = sender as ImageButton;

            List<string> files = new List<string>();
            GetSelectedFile(files);

            DropBoxFlow flow = new DropBoxFlow();
            flow.ParentWin = m_MainWin;
            flow.FileList = files;
            DropBoxFlow.FlowType = CloudFlowType.View;
            flow.Run();

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
                        image_wrappanel.Children.RemoveAt( index );
                        break;
                    }
                    index++;
                }
            }
            //add by yunying shang 2017-10-19 for BMS 1178
            if (image_wrappanel.Children.Count == GetSelectedItemCount())
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
                }
            }
            else
            {
                img.m_num = 0;
            }
            
            UpdateSelItemNum();

            if ( 0 < GetSelectedItemCount() )
            {
               // btnPrint.IsEnabled = true;
              //  btnSave.IsEnabled = true;
            }
            else
            {
              //  btnPrint.IsEnabled = false;
              //  btnSave.IsEnabled = false;
            }

            if(image_wrappanel.Children.Count == GetSelectedItemCount())
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
            int index = 1;
            for (int i = image_wrappanel.Children.Count - 1; i >=0 ; i--)
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
              
                index++;
            }
        }

        private void ImageItemDoubleClick(object sender, RoutedEventArgs e)
        {
            ImageItem img = (ImageItem)sender;

            if (0 == img.m_num && GetSelectedItemCount() < m_maxImgNum)
                img.m_num = GetSelectedItemCount() + 1;

            //  btnPrint.IsEnabled = true;
            //  btnSave.IsEnabled = true;

            //add by yunying shang 2017-10-19 for BMS 1182
            if (image_wrappanel.Children.Count == GetSelectedItemCount())
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
                for ( int i=0; i<image_wrappanel.Children.Count; i++ )
                {
                    if ( image_wrappanel.Children[i] == img )
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
                        this.image_wrappanel.Children.RemoveAt( index );

                        // Collect the rubbish files.
                        App.rubbishFiles.Add( img.m_images );

                        tmp.ImageSingleClick += ImageItemSingleClick;
                        tmp.ImageDoubleClick += ImageItemDoubleClick;
                        tmp.CloseIconClick += ImageItemCloseIconClick;

                        if (GetSelectedItemCount() < m_maxImgNum)
                            tmp.m_num = GetSelectedItemCount() + 1;
                        else
                        {
                            tmp.m_num = 0;
                        }

                        tmp.Margin = new Thickness( 5 );
                        this.image_wrappanel.Children.Insert(index, tmp );
                        App.scanFileList.Add( tmp.m_images );
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
           
            if ( 0 < GetSelectedItemCount() )
            {
                //btnPrint.IsEnabled = true;
               // btnSave.IsEnabled = true;
            }
            else
            {
               // btnPrint.IsEnabled = false;
                //btnSave.IsEnabled = false;
            }

            InitFontSize();

            SelectAllCheckBox.IsChecked = true;
            SelectAll(true);
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

                foreach (object obj in image_wrappanel.Children)
                {
                    ImageItem img = obj as ImageItem;

                    if ( null != img && 0 < img.m_num )
                    {
                        files[img.m_num-1] = img.m_images.m_pathOrig;
                    }
                }  
            }

            if (files == null || files.Count == 0)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                      Application.Current.MainWindow,
                     "Please select one or more pictures to process",
                     "Prompt");
            }
        }

        /// <summary>
        /// Update the number of selected image items.
        /// </summary>
        private void UpdateSelItemNum()
        {
            List<int> lst = new List<int>();

            for ( int i=0; i<image_wrappanel.Children.Count; i++ )
            {
                ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

                if ( null != img1 && 0 < img1.m_num )
                {
                    lst.Add( img1.m_num );
                }
            }

            lst.Sort();

            int nMiss = 0;
            for ( int i=0; i<lst.Count; i++ )
            {
                if ( lst[i] != i+1 )
                {
                    nMiss = i+1;
                    break;
                }
            }

            if ( nMiss > 0 )
            {
                for ( int i=0; i<image_wrappanel.Children.Count; i++ )
                {
                    ImageItem img1 = image_wrappanel.Children[i] as ImageItem;

                    if ( null != img1 && 0 < img1.m_num && nMiss <= img1.m_num )
                    {
                        img1.m_num--; 
                    }
                }
            }
        }

        /// <summary>
        /// Get the amount of selected items.
        /// </summary>
        private int GetSelectedItemCount()
        {
            int nCount = 0;
            foreach ( object obj in image_wrappanel.Children )
            {
                ImageItem img = obj as ImageItem;

                if ( null != img && 0 < img.m_num )
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if(image_wrappanel.Children.Count > 0)
            {
                if (VOP.Controls.MessageBoxExResult.Yes ==
                 VOP.Controls.MessageBoxEx.Show(
                     VOP.Controls.MessageBoxExStyle.YesNo_NoIcon,
                     m_MainWin,
                     "Do you want to delete all images before leaving scan page?",
                     (string)this.TryFindResource("ResStr_Prompt")
                     )
            )
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
