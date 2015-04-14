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
        public enum PrintType { PrintFile, PrintImages, PrintIdCard }
        public PrintType CurrentPrintType { get; set; }
        public IdCardTypeItem SelectedTypeItem { get; set; }

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
            printWin.m_MainWin = m_MainWin;

            result = printWin.ShowDialog();
            if (result == true)
            {
               
            }
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            PrintError printRes = PrintError.Print_OK;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            switch(CurrentPrintType)
            {
                case PrintType.PrintFile:

                    if (FilePaths.Count == 1)
                    {
                        printRes = worker.InvokePrintFileMethod(dll.PrintFile,
                               m_MainWin.statusPanelPage.m_selectedPrinter,
                               FilePaths[0]);
                    }

                    break;
                case PrintType.PrintImages:

                    if (dll.PrintInit(m_MainWin.statusPanelPage.m_selectedPrinter, "Print Images", (int)enumIdCardType.NonIdCard, new IdCardSize()))
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

                    if (dll.PrintInit(m_MainWin.statusPanelPage.m_selectedPrinter, "Print Id Card", (int)SelectedTypeItem.TypeId, idCardSize))
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

            }
        }

        public void HandlerStateUpdate( EnumState state )
        {
            // TODO: update UI when auto machine state change.
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
