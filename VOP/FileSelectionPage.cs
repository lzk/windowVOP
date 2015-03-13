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
        public FileSelectionPage()
        {
            InitializeComponent();
        }

        private void OnClickFilePrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        private void OnClickIdCardPrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool? result = null;
            IdCardTypeItem selectedItem = null;

            IdCardTypeSelectWindow selectWin = new IdCardTypeSelectWindow();
            selectWin.Owner = App.Current.MainWindow;

            result = selectWin.ShowDialog();

            if (result == true)
            {
                selectedItem = selectWin.SelectedTypeItem;

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "JPEG|*.jpg|BMP|*.bmp|PNG|*.png|TIFF|*.tif";

                result = open.ShowDialog();

                if (result == true)
                {
                    IdCardEditWindow editWin = new IdCardEditWindow();
                    editWin.Owner = App.Current.MainWindow;
                    editWin.ImageUri = new Uri(open.FileName);
                    editWin.SelectedTypeItem = selectedItem;
                    ImageCropper.designerItemWHRatio = selectedItem.Width / selectedItem.Height;
                    result = editWin.ShowDialog();
                }
            }


        }
    }
}
