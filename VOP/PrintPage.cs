using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        MainWindow mainWin = null;
        List<string> filePaths = new List<string>();

        public List<string> FilePaths
        {
            set
            {
                filePaths = value;
                myImagePreviewPanel.myImagePreview.ImagePaths = filePaths;
            }
            get
            {
                return filePaths;
            }
        }

        public PrintPage()
        {
            InitializeComponent();
        }

        public PrintPage(MainWindow win)
        {
            InitializeComponent();
            this.mainWin = win;
            myImagePreviewPanel.BackArrowButton.Click += new RoutedEventHandler(OnBackArrowButtonClick);
        }

        private void OnBackArrowButtonClick(object sender, RoutedEventArgs e)
        {
            this.mainWin.subPageView.Child = this.mainWin.winFileSelectionPage;
        }
    }
}