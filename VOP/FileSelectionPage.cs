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

namespace VOP
{
    public partial class FileSelectionPage : UserControl
    {
        private enum FileSelectionState { SelectWindow, OpenFile, EditWindow, GoPrint, Exit }
        FileSelectionState currentState;
        public static int imageFileCount = 0;
        MainWindow mainWin = null;

        public FileSelectionPage()
        {
            InitializeComponent();
        }

        public FileSelectionPage(MainWindow win)
        {
            InitializeComponent();
            this.mainWin = win;
        }

        private void OnClickFilePrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        private void OnClickIdCardPrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool? result = null;
            IdCardTypeSelectWindow selectWin = null;
            OpenFileDialog open = null;
            imageFileCount = 0;

            currentState = FileSelectionState.SelectWindow;
            IdCardEditWindow.croppedImageList.Clear();

            while(currentState != FileSelectionState.Exit)
            {
                switch(currentState)
                {
                    case FileSelectionState.SelectWindow:

                        selectWin = new IdCardTypeSelectWindow();
                        selectWin.Owner = App.Current.MainWindow;
                        result = selectWin.ShowDialog();

                        if(result == true)
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

                        IdCardEditWindow editWin = new IdCardEditWindow();
                        editWin.Owner = App.Current.MainWindow;
                        editWin.ImageUri = new Uri(open.FileName);
                        editWin.SelectedTypeItem = selectWin.SelectedTypeItem;
                        ImageCropper.designerItemWHRatio = selectWin.SelectedTypeItem.Width / selectWin.SelectedTypeItem.Height;

                        result = editWin.ShowDialog();
                        if (result == true)
                        {
                            currentState = FileSelectionState.GoPrint;

                            if(selectWin.SelectedTypeItem.PrintSides == enumIdCardPrintSides.TwoSides)
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

                        break;
                    case FileSelectionState.GoPrint:
                        this.mainWin.subPageView.Child = this.mainWin.winPrintPage;
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
