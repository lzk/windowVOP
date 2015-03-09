using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using VOP.Controls;

namespace VOP
{
    public partial class FileSelectionPage : UserControl
    {
        public FileSelectionPage()
        {
            InitializeComponent();

        }

        private void OnClickIdCardPrint(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IdCardTypeSelectWindow selectWin = new IdCardTypeSelectWindow();
            selectWin.Owner = App.Current.MainWindow;
            selectWin.ShowDialog();
        }
    }
}
