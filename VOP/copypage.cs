using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage

namespace VOP
{
    public partial class CopyPage : UserControl
    {
        public CopyPage()
        {
            InitializeComponent();

        }

        private void SettingBtnClick(object sender, RoutedEventArgs e)
        {
            CopySetting win = new CopySetting();
            
            win.Owner = App.Current.MainWindow;
            win.ShowDialog();
        }

    }
}
