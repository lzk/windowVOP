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
using VOP.Controls;

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        public MainWindow m_MainWin { get; set; }
        List<string> filePaths = new List<string>();
        public enum PrintType { PrintFile, PrintImages, PrintIdCard }
        public PrintType CurrentPrintType { get; set; }

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
            CurrentPrintType = PrintType.PrintFile;
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

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            if (CurrentPrintType == PrintType.PrintFile)
            {
                if(FilePaths.Count == 1)
                {
                    PrintError printRes = worker.InvokePrintFileMethod(dll.PrintFile,
                                  m_MainWin.statusPanelPage.m_selectedPrinter,
                                  FilePaths[0]);

                    if(printRes == PrintError.Print_File_Not_Support)
                    {
                        MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple("暂不支持该文件打印， 请重新选择。", "错误");
                        messageBox.Owner = App.Current.MainWindow;
                        messageBox.ShowDialog();
                        this.m_MainWin.subPageView.Child = this.m_MainWin.winFileSelectionPage;
                    }
                    else if (printRes == PrintError.Print_Memory_Fail)
                    {
                        MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple("打印文件内存分配失败！", "错误");
                        messageBox.Owner = App.Current.MainWindow;
                        messageBox.ShowDialog();
                    }
                }
            }
        }

        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.
        }
    }
}
