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
        public MainWindow m_MainWin { get; set; }
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
            myImagePreviewPanel.BackArrowButton.Click += new RoutedEventHandler(OnBackArrowButtonClick);
        }

        private void PrintPageOnLoaded(object sender, RoutedEventArgs e)
        {
            spinnerControl1.FormattedValue = "";
            spinnerControl1.FormattedValue = "1";
        }

        private void OnBackArrowButtonClick(object sender, RoutedEventArgs e)
        {
            this.m_MainWin.subPageView.Child = this.m_MainWin.winFileSelectionPage;
        }

        private void AdvancedSettingButtonClick(object sender, RoutedEventArgs e)
        {
            bool? result = null;
            PrintSettingPage printWin = new PrintSettingPage();
            printWin.Owner = App.Current.MainWindow;

            result = printWin.ShowDialog();
            if (result == true)
            {
               
            }
        }

        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.
        }
    }
}
