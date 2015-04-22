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
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace VOP
{
    public partial class PrintPage : UserControl
    {
        public MainWindow m_MainWin { get; set; }
        List<string> filePaths = new List<string>();
        public enum PrintType{PrintFile,PrintImages,PrintIdCard,PrintFile_Image}
        public PrintType CurrentPrintType { get; set; }
        public IdCardTypeItem SelectedTypeItem { get; set; }

        private bool needFitToPage = false;

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
        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
 //           dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, m_paperSize = 0, m_paperOrientation = 1, m_mediaType = 0, m_paperOrder = 1, m_printQuality = 0, m_scalingType = 0, m_scalingRatio = 100, m_nupNum = 1, m_typeofPB = 0, m_posterType = 0, m_ADJColorBalance = 0, m_colorBalanceTo = 1, m_densityValue = 0, m_duplexPrint = 1, m_reversePrint = 1, m_tonerSaving = 0);
//            dll.SetPrinterInfo(m_MainWin.statusPanelPage.m_selectedPrinter, 0, 1, 0, 1, 0, 0, 100, 1, 0, 0, 0, 1, 0, 1, 1, 0);
//            dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, 1);
        }

        private void PrintPageOnLoaded(object sender, RoutedEventArgs e)
        {
            spinnerControl1.FormattedValue = "";
            spinnerControl1.FormattedValue = "1";

            TextBox tb = spinnerControl1.Template.FindName("tbTextBox", spinnerControl1) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
        }

        private void SpinnerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                e.Handled = true;  
            }
        }

        private void SpinnerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int textValue = 0;

            if(int.TryParse(textBox.Text, out textValue))
            {
                if(textValue > 99)
                {
                    textBox.Text = "99";
                }

                if (textValue < 1)
                {
                    textBox.Text = "1";
                }
            }
          
        }

        private void OnBackArrowButtonClick(object sender, RoutedEventArgs e)
        {
            this.m_MainWin.subPageView.Child = this.m_MainWin.winFileSelectionPage;
        }

        private void OnCopysValidationHasError(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrintButton.IsEnabled = !e.NewValue;

            if(e.NewValue)
            {
                MessageBoxEx_Simple messageBox = new MessageBoxEx_Simple("有效值输入范围为1-99，请确认后再次输入。", "错误");
                messageBox.Owner = App.Current.MainWindow;
                messageBox.ShowDialog();
            }
        }

        private void AdvancedSettingButtonClick(object sender, RoutedEventArgs e)
        {
            bool? result = null;
            dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)spinnerControl1.Value);
            PrintSettingPage printWin = new PrintSettingPage();
            printWin.Owner = App.Current.MainWindow;
            printWin.m_MainWin = m_MainWin;
            printWin.m_CurrentPrintType = CurrentPrintType;

            result = printWin.ShowDialog();
            if (result == true)
            {
                needFitToPage = (bool)printWin.chk_FitToPaperSize.IsChecked;
            }
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            PrintError printRes = PrintError.Print_OK;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            dll.SetCopies(m_MainWin.statusPanelPage.m_selectedPrinter, (sbyte)spinnerControl1.Value);
            switch(CurrentPrintType)
            {
                case PrintType.PrintFile:
                case PrintType.PrintFile_Image:

                    m_MainWin.Topmost = true;
                    if (FilePaths.Count == 1)
                    {
                        printRes = worker.InvokePrintFileMethod(dll.PrintFile,
                               m_MainWin.statusPanelPage.m_selectedPrinter,
                               FilePaths[0],
                               needFitToPage);
                    }
                    m_MainWin.Topmost = false;
                    break;
                case PrintType.PrintImages:

                    if (dll.PrintInit(m_MainWin.statusPanelPage.m_selectedPrinter, "Print Images",
                                     (int)enumIdCardType.NonIdCard, new IdCardSize(), needFitToPage))
                    {
                        foreach (string path in FilePaths)
                        {
                            dll.AddImagePath(path);
                        }

                        printRes = (PrintError)worker.InvokeDoWorkMethod(dll.DoPrintImage);
                    }
                    else
                    {
                        printRes = PrintError.Print_Operation_Fail;
                    }

                    break;
                case PrintType.PrintIdCard:
                    IdCardSize idCardSize = new IdCardSize();
                    idCardSize.Width = SelectedTypeItem.Width;
                    idCardSize.Height = SelectedTypeItem.Height;

                    if (dll.PrintInit(m_MainWin.statusPanelPage.m_selectedPrinter, "Print Id Card", (int)SelectedTypeItem.TypeId, idCardSize, needFitToPage))
                    {
                        using(IdCardPrintHelper helper = new IdCardPrintHelper())
                        {
                            foreach (BitmapSource src in IdCardEditWindow.croppedImageList)
                            {
                                helper.AddImage(src);
                            }

                            printRes = (PrintError)worker.InvokeDoWorkMethod(dll.DoPrintIdCard);
                        }
                    }
                    else
                    {
                        printRes = PrintError.Print_Operation_Fail;
                    }

                    break;
            }

            if (printRes == PrintError.Print_File_Not_Support)
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
            else if (printRes == PrintError.Print_Operation_Fail)
            {
                m_MainWin.statusPanelPage.ShowMessage("打印失败", Brushes.Black );
            }
            else if (printRes == PrintError.Print_OK)
            {
                m_MainWin.statusPanelPage.ShowMessage("打印成功", Brushes.Black );
            }
        }

        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.

            if (EnumState.doingJob == state || EnumState.stopWorking == state)
            {
                PrintButton.IsEnabled = false;
            }
            else
            {
                PrintButton.IsEnabled = true;
            }
        }
    }

    public class IdCardPrintHelper : IDisposable
    {
        List<IntPtr>  hGlobalList = new List<IntPtr>();
        List<IStream> iStreamList = new List<IStream>();

        bool disposed = false;

        public IdCardPrintHelper()
        {
            
        }

        ~IdCardPrintHelper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
            }

            foreach (IStream istream in iStreamList)
            {
                Marshal.ReleaseComObject(istream);
            }

            foreach(IntPtr handle in hGlobalList)
            {
                Marshal.FreeHGlobal(handle);
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        public void AddImage(BitmapSource bs)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bs));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            IStream iis;
            IntPtr hGlobal = IntPtr.Zero;

            if (Win32.CreateStreamOnHGlobal((IntPtr)null, false, out iis) == 0)
            {
                Win32.GetHGlobalFromStream(iis, ref hGlobal);

                hGlobalList.Add(hGlobal);
                iStreamList.Add(iis);

                iis.Write(data, data.GetLength(0), IntPtr.Zero);
                dll.AddImageSource((IStream)iis);
            }
        }
    }
}
