using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using VOP.Controls;
using Microsoft.Win32;
using System.Collections.Generic;

namespace VOP
{
    public partial class FileSelectionPage : UserControl
    {
        private enum FileSelectionState { SelectWindow, OpenFile, EditWindow, GoPrint, Exit }
        FileSelectionState currentState;
        public static int imageFileCount = 0;
        public static bool IsInitPrintSettingPage = true;
        public MainWindow m_MainWin { get; set; }

        public FileSelectionPage()
        {
            InitializeComponent();
        }

        private void FileSelectionPageOnLoaded(object sender, RoutedEventArgs e)
        {
            if(!MainWindow.m_bLocationIsChina)
            {
                IdCardButton.Visibility = Visibility.Hidden;
                ImageButton.Margin = new Thickness(60, 0, 0, 0);
                FileButton.Margin = new Thickness(90, 0, 0, 0);
            }

            InitFontSize();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x409) // en-US
            {
                ImageButton.FontSize = FileButton.FontSize = IdCardButton.FontSize = 10.0;
            }
        }

        private void OnClickImagePrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog open = null;
            bool? result = null;
            open = new OpenFileDialog();
            open.Filter = "All Images|*.jpg;*.bmp;*.png;*.tif|JPEG|*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";
            open.Multiselect = true;
            IsInitPrintSettingPage = true;

            result = open.ShowDialog();
            if (result == true)
            {
                try
                {
                    this.m_MainWin.winPrintPage.FilePaths = new List<string>(open.FileNames);
                    this.m_MainWin.subPageView.Child = this.m_MainWin.winPrintPage;
                    this.m_MainWin.winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintImages;
                }
                catch (Exception)
                {
                    MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_This_file_is_not_supported__please_select_another_one_"), (string)this.FindResource("ResStr_Warning_2"));
                    messageBox.Owner = App.Current.MainWindow;
                    messageBox.ShowDialog();
                } 
            }
        }

        private void OnClickFilePrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog open = null;
            bool? result = null;
            open = new OpenFileDialog();
            open.Filter = "All Files|*.*";
            IsInitPrintSettingPage = true;

            result = open.ShowDialog();
            if (result == true)
            {
                try
                {
                    List<string> strls = new List<string>();
                    strls.Add(open.FileName);
                    this.m_MainWin.winPrintPage.FilePaths = strls;
                    this.m_MainWin.subPageView.Child = this.m_MainWin.winPrintPage;

                    string fileExt = System.IO.Path.GetExtension(open.FileName).ToLower();

                    if (fileExt == ".bmp"
                        || fileExt == ".ico"
                        || fileExt == ".gif"
                        || fileExt == ".jpg"
                        || fileExt == ".exif"
                        || fileExt == ".png"
                        || fileExt == ".tif"
                        || fileExt == ".wmf"
                        || fileExt == ".emf")
                    {
                        this.m_MainWin.winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintFile_Image;
                    }
                    else if (fileExt == ".txt")
                    {
                        this.m_MainWin.winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintFile_Txt;
                    }
                    else if (fileExt == ".pdf")
                    {
                        this.m_MainWin.winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintFile_Pdf;
                    }
                    else
                    {
                        this.m_MainWin.winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintFile;
                    }
                }
                catch (Exception)
                {
                    MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_This_file_is_not_supported__please_select_another_one_"), (string)this.FindResource("ResStr_Warning_2"));
                    messageBox.Owner = App.Current.MainWindow;
                    messageBox.ShowDialog();
                } 
            }
        }

        private void OnClickIdCardPrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool? result = null;
            IdCardTypeSelectWindow selectWin = null;
            OpenFileDialog open = null;
            imageFileCount = 0;
            IsInitPrintSettingPage = true;

            currentState = FileSelectionState.SelectWindow;
            IdCardEditWindow.croppedImageList.Clear();
            IdCardEditWindow.imageRotationList.Clear();
          
            while (currentState != FileSelectionState.Exit)
            {
                switch (currentState)
                {
                    case FileSelectionState.SelectWindow:

                        selectWin = new IdCardTypeSelectWindow();
                        selectWin.Owner = App.Current.MainWindow;
                        result = selectWin.ShowDialog();

                        if (result == true)
                        {
                            currentState = FileSelectionState.OpenFile;
                        }
                        else
                        {
                            currentState = FileSelectionState.Exit;
                        }

                        break;
                    case FileSelectionState.OpenFile:

                        open = new OpenFileDialog();
                        open.Filter = "JPEG|*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";

                        result = open.ShowDialog();
                        if (result == true)
                        {
                            currentState = FileSelectionState.EditWindow;
                        }
                        else
                        {
                            currentState = FileSelectionState.Exit;
                        }

                        break;
                    case FileSelectionState.EditWindow:

                        try
                        {
                            IdCardEditWindow editWin = new IdCardEditWindow();
                            editWin.Owner = App.Current.MainWindow;
                            editWin.ImageUri = new Uri(open.FileName);
                            editWin.SelectedTypeItem = selectWin.SelectedTypeItem;
                            ImageCropper.designerItemWHRatio = selectWin.SelectedTypeItem.Width / selectWin.SelectedTypeItem.Height;

                            result = editWin.ShowDialog();
                            if (result == true)
                            {
                                currentState = FileSelectionState.GoPrint;

                                if (selectWin.SelectedTypeItem.PrintSides == enumIdCardPrintSides.TwoSides)
                                {
                                    if (++imageFileCount < 2)
                                    {
                                        currentState = FileSelectionState.OpenFile;
                                    }
                                }
                            }
                            else
                            {
                                currentState = FileSelectionState.Exit;
                            }
                        }
                        catch (Exception)
                        {
                            MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple((string)this.TryFindResource("ResStr_This_file_is_not_supported__please_select_another_one_"), (string)this.FindResource("ResStr_Warning_2"));
                            messageBox.Owner = App.Current.MainWindow;
                            messageBox.ShowDialog();
                            currentState = FileSelectionState.Exit;
                        } 
                       
                        break;
                    case FileSelectionState.GoPrint:
                        this.m_MainWin.subPageView.Child = this.m_MainWin.winPrintPage;
                        this.m_MainWin.winPrintPage.myImagePreviewPanel.myImagePreview.IdCardPreviewSource = PrintPreview.PreviewImageSource;
                        this.m_MainWin.winPrintPage.CurrentPrintType = PrintPage.PrintType.PrintIdCard;
                        this.m_MainWin.winPrintPage.SelectedTypeItem = selectWin.SelectedTypeItem;
                        currentState = FileSelectionState.Exit;
                        break;
                    default:
                        currentState = FileSelectionState.Exit;
                        break;
                }
            }
        }
    }
}
